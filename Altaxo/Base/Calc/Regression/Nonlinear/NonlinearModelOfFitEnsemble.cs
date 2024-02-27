#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;
using Altaxo.Calc.Optimization.ObjectiveFunctions;
using Altaxo.Collections;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Adapts a <see cref="FitEnsemble" /> to the requirements of a Levenberg-Marquardt fitting procedure.
  /// This means, the adapter makes the <see cref="FitEnsemble" /> compatible with the Levenberg-Marquardt algorithm.
  /// </summary>
  public class NonlinearModelOfFitEnsemble : NonlinearObjectiveFunctionNonAllocatingBase
  {
    #region CachedFitElementInfo

    /// <summary>Caches the temporary memory space of one fit element.</summary>
    private class CachedFitElementInfo
    {
      /// <summary>Parameter array for temporary purpose.</summary>
      public double[] Parameters;

      /// <summary>Independent variable array for temporary purpose. If the fit function has one independent variable (as is the case for
      /// the majority of fit functions, the matrix has only one column, and as many rows as there are independent data points.</summary>
      public IROMatrix<double> Xs;

      /// <summary>
      /// Scratch array that accomodate the dependent variables when making the evaluation of the derivative and
      /// the dependent variable has a transformation set on it.
      /// </summary>
      public IVector<double> Ys;

      /// <summary>
      /// Array in which for each parameter the value is true if this parameter is fixed.
      /// </summary>
      public bool[] IsParameterFixed;

      /// <summary>Parameter mapping from the local parameter list to the global parameter list. Positive entries
      /// give the position in the global variable parameter list, negative entries gives the position -entry-1 in the
      /// global constant parameter list.
      /// </summary>
      public int[] ParameterMapping;

      /// <summary>Designates which dependent variable columns are really in use.</summary>
      public bool[] DependentVariablesInUse;

      /// <summary>
      /// The number of dependent variables in use, i.e. the number of elements with a value of true in <see cref="DependentVariablesInUse"/>.
      /// </summary>
      public int NumberOfDependentVariablesInUse;

      /// <summary>
      /// Information, which of the rows are valid, i.e. all independent columns contains values, and all used dependent columns contain values in those rows.
      /// </summary>
      public IAscendingIntegerCollection ValidRows;

      public CachedFitElementInfo(
        double[] parameters,
        IROMatrix<double> xs,
        int[] parameterMapping,
        bool[] isParameterFixed,
        bool[] dependentVariablesInUse,
        IAscendingIntegerCollection validRows
        )
      {
        Parameters = parameters;
        Xs = xs;
        ParameterMapping = parameterMapping;
        IsParameterFixed = isParameterFixed;
        DependentVariablesInUse = dependentVariablesInUse;
        NumberOfDependentVariablesInUse = DependentVariablesInUse.Count(x => x);
        ValidRows = validRows;
        Ys = Vector<double>.Build.Dense(ValidRows.Count * NumberOfDependentVariablesInUse);
      }
    }

    /// <summary>
    /// Wraps the full jacobian matrix in a way, that a single fit element can write their derivatives into it.
    /// </summary>
    /// <seealso cref="IMatrix{T}" />
    private class JacobianMapper : IMatrix<double>
    {
      private Matrix<double> _matrix;
      private int _rowOffset;
      private int[] _parameterMapping;

      /// <summary>
      /// Initializes a new instance of the <see cref="JacobianMapper"/> class.
      /// </summary>
      /// <param name="mappedMatrix">The mapped jacobian matrix.</param>
      /// <param name="rowOffset">The row offset into the jacobian matrix.</param>
      /// parameter list of the fit element. Parameters fixed correspond to a value of -1 in the column mapping, the varying
      /// parameters correspond to values of 0, 1, 2, and so on.</param>
      public JacobianMapper(Matrix<double> mappedMatrix, int rowOffset, int[] parameterMapping)
      {
        _matrix = mappedMatrix;
        _rowOffset = rowOffset;
        _parameterMapping = parameterMapping;
      }

      public double this[int row, int col]
      {
        get
        {
          int c = _parameterMapping[col]; // get the column number that correspond to the varying parameter
          if (c >= 0)                   // c is >=0 if it is a varying parameter, otherwise, it is a constant parameter
          {
            return _matrix[_rowOffset + row, c];
          }
          else
          {
            throw new InvalidOperationException();
          }
        }
        set
        {
          int c = _parameterMapping[col]; // get the column number that correspond to the varying parameter
          if (c >= 0)                   // c is >=0 if it is a varying parameter, otherwise, it is a constant parameter
          {
            _matrix[_rowOffset + row, c] = value;
          }
        }
      }

      double IROMatrix<double>.this[int row, int col] => throw new NotImplementedException();

      public int RowCount => _matrix.RowCount - _rowOffset;

      public int ColumnCount => _matrix.ColumnCount;
    }

    #endregion inner classes

    /// <summary>The fit ensemble this adapter is using.</summary>
    private FitEnsemble _fitEnsemble;

    /// <summary>Caches the temporary information of all fit elements.</summary>
    private CachedFitElementInfo[] _cachedFitElementInfo;

    /// <summary>
    /// List of constant parameters (i.e. parameters that are not changed during the fitting session).
    /// For convenience, all parameters are stored here (the varying parameters too), but only the constant parameters are used.
    /// </summary>
    private double[] _constantParameters;

    /// <summary>If this array is set, the weights are used to scale the fit differences (yreal-yfit).</summary>
    private double[]? _cachedWeights;


    /// <summary>
    /// Constructor of the adapter.
    /// </summary>
    /// <param name="ensemble">The fit ensemble, i.e. the functions and data you intend to fit.</param>
    /// <param name="paraSet">The set of initial parameter. Must contain a initial estimation of the parameters. Contains also information which
    /// parameters can vary and which are fixed during the fitting procedure.</param>
    public NonlinearModelOfFitEnsemble(FitEnsemble ensemble, ParameterSet paraSet) : base(1)
    {
      _fitEnsemble = ensemble;
      CalculateCachedData(paraSet);
    }

    /// <summary>
    /// Internal function to set up the cached data for the fitting procedure.
    /// </summary>
    /// <param name="paraSet">The set of parameters (the information which parameters are fixed is mainly used here).</param>
    [MemberNotNull(nameof(_constantParameters), nameof(_coefficients), nameof(_cachedFitElementInfo))]
    private void CalculateCachedData(ParameterSet paraSet)
    {
      // Preparation: Store the parameter names by name and index, and store
      // all parameter values in _constantParameters
      var parameterNames = new Dictionary<string, int>();
      var varyingParameterNames = new Dictionary<string, int>();

      _constantParameters = new double[paraSet.Count];
      int numberOfVaryingParameters = 0;
      for (int i = 0; i < paraSet.Count; ++i)
      {
        parameterNames.Add(paraSet[i].Name, i);
        _constantParameters[i] = paraSet[i].Parameter;
        if (paraSet[i].Vary)
        {
          ++numberOfVaryingParameters;
        }
      }
      _coefficients = Vector<double>.Build.Dense(numberOfVaryingParameters);
      for (int i = 0, k = 0; i < paraSet.Count; ++i)
      {
        if (paraSet[i].Vary)
        {
          varyingParameterNames.Add(paraSet[i].Name, k);
          _coefficients[k++] = paraSet[i].Parameter;
        }
      }

      var totalNumberOfData = 0;
      _cachedFitElementInfo = new CachedFitElementInfo[_fitEnsemble.Count];
      for (int idxFitElement = 0; idxFitElement < _fitEnsemble.Count; idxFitElement++)
      {
        var fitElement = _fitEnsemble[idxFitElement];
        var validRows = fitElement.CalculateValidNumericRows();
        var numberOfValidRows = validRows.Count;
        var xs = Matrix<double>.Build.Dense(numberOfValidRows, fitElement.NumberOfIndependentVariables);
        for (int c = 0; c < xs.ColumnCount; ++c)
        {
          var col = fitElement.IndependentVariables(c) ?? throw new ObjectDisposedException($"Independent variables column k={c} not available or disposed.");
          for (int r = 0; r < numberOfValidRows; ++r)
          {
            xs[r, c] = col[validRows[r]];
          }
        }

        var parameters = new double[fitElement.NumberOfParameters];
        var isParameterFixedByUser = new bool[fitElement.NumberOfParameters];
        var ys = new double[fitElement.NumberOfDependentVariables];

        // Calculate the number of used variables
        int numVariablesUsed = 0;
        for (int j = 0; j < fitElement.NumberOfDependentVariables; ++j)
        {
          if (fitElement.DependentVariables(j) is not null)
          {
            ++numVariablesUsed;
          }
        }
        var dependentVariablesInUse = new bool[fitElement.NumberOfDependentVariables];
        for (int j = 0; j < fitElement.NumberOfDependentVariables; ++j)
        {
          if (fitElement.DependentVariables(j) is not null)
          {
            dependentVariablesInUse[j] = true;
          }
        }

        // calculate the total number of data points
        totalNumberOfData += numVariablesUsed * validRows.Count;

        // now create the parameter mapping
        var parameterMapping = new int[fitElement.NumberOfParameters];

        for (int j = 0; j < parameterMapping.Length; ++j)
        {
          if (!parameterNames.ContainsKey(fitElement.ParameterName(j)))
          {
            throw new ArgumentException(string.Format("ParameterSet does not contain parameter {0}, which is used by function[{1}]", fitElement.ParameterName(j), idxFitElement));
          }

          int idx = (int)parameterNames[fitElement.ParameterName(j)];
          if (paraSet[idx].Vary)
          {
            parameterMapping[j] = (int)varyingParameterNames[fitElement.ParameterName(j)];
            isParameterFixedByUser[j] = false;
          }
          else
          {
            parameterMapping[j] = -idx - 1;
            isParameterFixedByUser[j] = true;
          }
        }

        _cachedFitElementInfo[idxFitElement] = new CachedFitElementInfo(
          parameters: parameters,
          xs: xs,
          isParameterFixed: isParameterFixedByUser,
          parameterMapping: parameterMapping,
          dependentVariablesInUse: dependentVariablesInUse,
          validRows: validRows
          ); ;
      }

      if (HasToUseWeights())
      {
        _cachedWeights = new double[totalNumberOfData];
        GetWeights(_cachedWeights);
      }
      else
      {
        _cachedWeights = null;
      }

      // now allocated the vector that holds the observed y-values, and fill it
      var observedY = Vector<double>.Build.Dense(totalNumberOfData);
      int observedYOffset = 0;
      for (int idxFitElement = 0; idxFitElement < _fitEnsemble.Count; idxFitElement++)
      {
        var fitElement = _fitEnsemble[idxFitElement];
        var fitInfo = _cachedFitElementInfo[idxFitElement];
        var validRows = fitInfo.ValidRows;
        var rowCount = validRows.Count;

        for (int r = 0; r < rowCount; ++r)
        {
          var depVarsInUse = fitInfo.DependentVariablesInUse;
          for (int c = 0; c < depVarsInUse.Length; ++c)
          {
            if (depVarsInUse[c])
            {
              observedY[observedYOffset++] = fitElement.DependentVariables(c)[validRows[r]];
            }
          }
        }
      }
      ObservedY = observedY;

      _negativeGradientValue ??= Vector<double>.Build.Dense(_coefficients.Count);
      _hessianValue ??= Matrix<double>.Build.Dense(_coefficients.Count, _coefficients.Count);
      _jacobianValue ??= Matrix<double>.Build.Dense(totalNumberOfData, _coefficients.Count);
      _jacobianValueTransposed ??= Matrix<double>.Build.Dense(_coefficients.Count, totalNumberOfData);

      if (_accuracyOrder <= 2)
      {
        _f1 = Vector<double>.Build.Dense(totalNumberOfData);
        _f2 = Vector<double>.Build.Dense(totalNumberOfData);
      }
      else if (_accuracyOrder <= 4)
      {
        _f1 = Vector<double>.Build.Dense(totalNumberOfData);
        _f2 = Vector<double>.Build.Dense(totalNumberOfData);
        _f3 = Vector<double>.Build.Dense(totalNumberOfData);
        _f4 = Vector<double>.Build.Dense(totalNumberOfData);
      }
      else
      {
        _f1 = Vector<double>.Build.Dense(totalNumberOfData);
        _f2 = Vector<double>.Build.Dense(totalNumberOfData);
        _f3 = Vector<double>.Build.Dense(totalNumberOfData);
        _f4 = Vector<double>.Build.Dense(totalNumberOfData);
        _f5 = Vector<double>.Build.Dense(totalNumberOfData);
        _f6 = Vector<double>.Build.Dense(totalNumberOfData);
      }

    }

    public void CopyParametersBackTo(ParameterSet pset, IReadOnlyList<double>? standardErrors)
    {
      if (pset.Count != _constantParameters.Length)
        throw new ArgumentException("Length of parameter set pset does not match with cached length of parameter set");
      int varyingPara = 0;
      for (int i = 0; i < pset.Count; i++)
      {
        if (pset[i].Vary)
          varyingPara++;
      }

      if (varyingPara != _coefficients.Count)
        throw new ArgumentException("Number of varying parameters in pset does not match cached number of varying parameters");

      varyingPara = 0;
      for (int i = 0; i < pset.Count; i++)
      {
        if (pset[i].Vary)
        {
          pset[i] = pset[i] with
          {
            Parameter = _coefficients[varyingPara],
            Variance = standardErrors is null ? 0 : standardErrors[varyingPara],
          };
          varyingPara++;
        }
        else
        {
          pset[i] = pset[i] with
          {
            Parameter = _constantParameters[i],
            Variance = 0,
          };
        }
      }
      pset.OnInitializationFinished();
    }

    /// <summary>
    /// Copies the current parameters for the fit element with the provided index into the provided array.
    /// </summary>
    /// <param name="idxFitEle">Index of the fit element.</param>
    /// <param name="parameters">Provided array to copy the current parameters to. Must have same size as NumberOfParameters for the given fit element.</param>
    public void CopyParametersForFitElement(int idxFitEle, double[] parameters)
    {
      CachedFitElementInfo info = _cachedFitElementInfo[idxFitEle];
      // copy of the parameter to the temporary array
      for (int i = 0; i < info.Parameters.Length; i++)
      {
        int idx = info.ParameterMapping[i];
        parameters[i] = idx >= 0 ? _coefficients[idx] : _constantParameters[-1 - idx];
      }
    }


    public override IObjectiveModel Fork()
    {
      throw new NotImplementedException();
    }

    public override IObjectiveModel CreateNew()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the collection of valid numeric rows for the given fit element.
    /// </summary>
    /// <param name="idxFitElement">Index number of the fit element.</param>
    /// <returns>Collection of valid numeric rows for the given fit element.</returns>
    public IAscendingIntegerCollection GetValidNumericRows(int idxFitElement)
    {
      return _cachedFitElementInfo[idxFitElement].ValidRows;
    }

    /// <summary>
    /// Returns the array of indices of dependent variables that are currently in use (i.e. associated with a data column).
    /// </summary>
    /// <param name="idxFitElement"></param>
    /// <returns></returns>
    public int[] GetDependentVariablesInUse(int idxFitElement)
    {
      return (int[])_cachedFitElementInfo[idxFitElement].DependentVariablesInUse.Clone();
    }

    /// <summary>
    /// Stores the weights for the fit differences  in an array. The data
    /// are stored from FitElement_0 to FitElement_n. For FitElements with more than one dependent
    /// variable in use, the data are stored interleaved.
    /// </summary>
    /// <param name="values">The array used to store the values.</param>
    public void GetWeights(double[] values)
    {
      throw new NotImplementedException();
      /*
      int outputValuesPointer = 0;
      for (int ele = 0; ele < _cachedFitElementInfo.Length; ele++)
      {
        CachedFitElementInfo info = _cachedFitElementInfo[ele];
        FitElement fitEle = _fitEnsemble[ele];

        IAscendingIntegerCollection validRows = info.ValidRows;
        int numValidRows = validRows.Count;
        // Evaluate the function for all points
        for (int i = 0; i < numValidRows; ++i)
        {
          for (int j = 0; j < info.DependentVariablesInUse.Length; ++j)
          {
            double yreal = fitEle.DependentVariables(info.DependentVariablesInUse[j])?[validRows[i]] ?? double.NaN;
            values[outputValuesPointer++] = fitEle.GetErrorEvaluation(info.DependentVariablesInUse[j])?.GetWeight(yreal, validRows[i]) ?? 1;
          }
        }
      }
      */
    }

    /// <summary>
    /// Returns true if any of the fit elements use scaling weights. In this case we have to calculate
    /// the weights for all fit elements and include them in the fitting procedures.
    /// </summary>
    /// <returns>True if any of the fit elements use weights.</returns>
    public bool HasToUseWeights()
    {
      for (int i = 0; i < _fitEnsemble.Count; ++i)
        if (_fitEnsemble[i].UseWeights)
          return true;

      return false;
    }


    /// <summary>
    /// Calculates the fitting values.
    /// </summary>
    /// <param name="outputValues">You must provide an array to hold the calculated values. Size of the array must be
    /// at least <see cref="NumberOfObservations" />.</param>
    /// <param name="calculateUnusedDependentVariablesAlso">If <c>true</c>, the unused dependent variables are also calculated (and plotted).</param>
    /// <remarks>The values of the fit elements are stored in the order from element_0 to element_n. If there is more
    /// than one used dependent variable per fit element, the output values are stored in interleaved order.
    /// </remarks>
    public void EvaluateModelValues(Vector<double> outputValues, bool calculateUnusedDependentVariablesAlso = false)
    {
      EvaluateModelValues(_coefficients, outputValues, calculateUnusedDependentVariablesAlso);
    }


    /// <summary>
    /// Calculates the fitting values.
    /// </summary>
    /// <param name="parameters">The parameter used to calculate the values.</param>
    /// <param name="outputValues">You must provide an array to hold the calculated values. Size of the array must be
    /// at least <see cref="NumberOfObservations" />.</param>
    /// <param name="calculateUnusedDependentVariablesAlso">If <c>true</c>, the unused dependent variables are also calculated (and plotted).</param>
    /// <remarks>The values of the fit elements are stored in the order from element_0 to element_n. If there is more
    /// than one used dependent variable per fit element, the output values are stored in interleaved order.
    /// </remarks>
    public void EvaluateModelValues(IReadOnlyList<double> parameters, Vector<double> outputValues, bool calculateUnusedDependentVariablesAlso = false)
    {

      int outputValuesPointer = 0;
      for (int ele = 0; ele < _cachedFitElementInfo.Length; ele++)
      {
        CachedFitElementInfo info = _cachedFitElementInfo[ele];
        FitElement fitEle = _fitEnsemble[ele];

        if (fitEle.FitFunction is null)
        {
          throw new InvalidOperationException($"FitFunction of FitElement[{ele}] is not set.");
        }

        // copy of the parameter to the temporary array
        for (int i = 0; i < info.Parameters.Length; i++)
        {
          int idx = info.ParameterMapping[i];
          info.Parameters[i] = idx >= 0 ? parameters[idx] : _constantParameters[-1 - idx];
        }

        IAscendingIntegerCollection validRows = info.ValidRows;
        int numberOfValidRows = validRows.Count;
        // Evaluate the function for all points

        if (calculateUnusedDependentVariablesAlso)
        {
          // calculate all dependent variables
          fitEle.FitFunctionEvaluate(info.Xs, info.Parameters, VectorMath.ToVector(outputValues, outputValuesPointer, outputValues.Count - outputValuesPointer), null);
          outputValuesPointer += info.Xs.RowCount * info.DependentVariablesInUse.Length;
        }
        else
        {
          // usual case: do not calculate the unused dependent variables
          fitEle.FitFunctionEvaluate(info.Xs, info.Parameters, VectorMath.ToVector(outputValues, outputValuesPointer, outputValues.Count - outputValuesPointer), info.DependentVariablesInUse);
          outputValuesPointer += info.Xs.RowCount * info.NumberOfDependentVariablesInUse;
        }
      }
    }

    protected override void EvaluateFunction()
    {
      // Calculates the residuals, (y[i] - f(x[i]; p)) * L[i]
      if (ModelValues is null)
      {
        ModelValues = Vector<double>.Build.Dense(NumberOfObservations);
      }

      EvaluateModelValues(ModelValues, false);
      FunctionEvaluations++;

      // calculate the weighted residuals
      _residuals = (Weights is null)
          ? ObservedY - ModelValues
          : (ObservedY - ModelValues).PointwiseMultiply(L);

      // Calculate the residual sum of squares
      _functionValue = _residuals.DotProduct(_residuals);
    }

    protected override void EvaluateJacobian()
    {
      int rowOffset = 0; // offset into the jacobian rows (by every fit element the increase is by (NumberOfX*NumberOfDependentVariablesInUse))
      _jacobianValue.Clear();

      // up to now, we can use built-in derivatives only then, when
      // all fit functions support this
      bool allElementsHaveDerivative = _fitEnsemble.Count == _fitEnsemble.Count(x => x.FitFunction is IFitFunctionWithDerivative);

      for (int ele = 0; ele < _cachedFitElementInfo.Length; ele++)
      {
        CachedFitElementInfo info = _cachedFitElementInfo[ele];
        FitElement fitEle = _fitEnsemble[ele];

        // copy the parameters into the temporary array
        for (int i = 0; i < info.Parameters.Length; i++)
        {
          int idx = info.ParameterMapping[i];
          info.Parameters[i] = idx >= 0 ? _coefficients[idx] : _constantParameters[-1 - idx];
        }

        if (allElementsHaveDerivative && fitEle.FitFunction is IFitFunctionWithDerivative fitFunctionWithDerivative)
        {
          var jacWrapper = new JacobianMapper(_jacobianValue, rowOffset, info.ParameterMapping);
          fitEle.FitFunctionEvaluateDerivative(info.Xs, info.Parameters, info.IsParameterFixed, jacWrapper, info.DependentVariablesInUse, info.Ys);
          rowOffset += info.Xs.RowCount * info.NumberOfDependentVariablesInUse;
          ++JacobianEvaluations;
        }
        else
        {
          NumericalJacobian(_coefficients, ModelValues, _accuracyOrder);
          FunctionEvaluations += _accuracyOrder;
        }
      }

      // weighted jacobian
      if (IsFixedByUserOrBoundary is not null)
      {
        for (int j = 0; j < NumberOfParameters; j++)
        {
          if (IsFixedByUserOrBoundary[j])
          {
            _jacobianValue.ClearColumn(j);
          }
          else if (Weights is not null)
          {
            for (int i = 0; i < NumberOfObservations; i++)
            {
              _jacobianValue[i, j] *= L[i];
            }
          }
        }
      }
      else if (Weights is not null)
      {
        for (int i = 0; i < NumberOfObservations; i++)
        {
          var li = L[i];
          for (int j = 0; j < NumberOfParameters; j++)
          {
            _jacobianValue[i, j] *= li;
          }
        }
      }

      // Gradient, g = -J'W(y − f(x; p)) = -J'L(L'E) = -J'LR
      // _gradientValue = -_jacobianValue.Transpose() * _residuals;
      _jacobianValue.Transpose(_jacobianValueTransposed);
      _jacobianValueTransposed.Multiply(_residuals, _negativeGradientValue);

      // approximated Hessian, H = J'WJ + ∑LRiHi ~ J'WJ near the minimum
      // _hessianValue = _jacobianValue.Transpose() * _jacobianValue;
      _jacobianValueTransposed.Multiply(_jacobianValue, _hessianValue);
    }

    protected override Matrix<double> NumericalJacobian(Vector<double> parameters, Vector<double> currentValues, int accuracyOrder = 2)
    {
      const double sqrtEpsilon = 1.4901161193847656250E-8; // sqrt(machineEpsilon)

      Matrix<double> derivatives = _jacobianValue;

      var d = 0.000003 * parameters.PointwiseAbs().PointwiseMaximum(sqrtEpsilon);

      var h = Vector<double>.Build.Dense(NumberOfParameters);
      for (int j = 0; j < NumberOfParameters; j++)
      {
        h[j] = d[j];

        if (accuracyOrder >= 6)
        {
          // f'(x) = {- f(x - 3h) + 9f(x - 2h) - 45f(x - h) + 45f(x + h) - 9f(x + 2h) + f(x + 3h)} / 60h + O(h^6)
          EvaluateModelValues(parameters - 3 * h, _f1, false);
          EvaluateModelValues(parameters - 2 * h, _f2, false);
          EvaluateModelValues(parameters - h, _f3, false);
          EvaluateModelValues(parameters + h, _f4, false);
          EvaluateModelValues(parameters + 2 * h, _f5, false);
          EvaluateModelValues(parameters + 3 * h, _f6, false);

          var prime = (-_f1 + 9 * _f2 - 45 * _f3 + 45 * _f4 - 9 * _f5 + _f6) / (60 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else if (accuracyOrder == 5)
        {
          // f'(x) = {-137f(x) + 300f(x + h) - 300f(x + 2h) + 200f(x + 3h) - 75f(x + 4h) + 12f(x + 5h)} / 60h + O(h^5)
          var f1 = currentValues;
          EvaluateModelValues(parameters + h, _f2, false);
          EvaluateModelValues(parameters + 2 * h, _f3, false);
          EvaluateModelValues(parameters + 3 * h, _f4, false);
          EvaluateModelValues(parameters + 4 * h, _f5, false);
          EvaluateModelValues(parameters + 5 * h, _f6, false);

          var prime = (-137 * f1 + 300 * _f2 - 300 * _f3 + 200 * _f4 - 75 * _f5 + 12 * _f6) / (60 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else if (accuracyOrder == 4)
        {
          // f'(x) = {f(x - 2h) - 8f(x - h) + 8f(x + h) - f(x + 2h)} / 12h + O(h^4)
          EvaluateModelValues(parameters - 2 * h, _f1, false);
          EvaluateModelValues(parameters - h, _f2, false);
          EvaluateModelValues(parameters + h, _f3, false);
          EvaluateModelValues(parameters + 2 * h, _f4, false);

          var prime = (_f1 - 8 * _f2 + 8 * _f3 - _f4) / (12 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else if (accuracyOrder == 3)
        {
          // f'(x) = {-11f(x) + 18f(x + h) - 9f(x + 2h) + 2f(x + 3h)} / 6h + O(h^3)
          var f1 = currentValues;
          EvaluateModelValues(parameters + h, _f2, false);
          EvaluateModelValues(parameters + 2 * h, _f3, false);
          EvaluateModelValues(parameters + 3 * h, _f4, false);

          var prime = (-11 * f1 + 18 * _f2 - 9 * _f3 + 2 * _f4) / (6 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else if (accuracyOrder == 2)
        {
          // f'(x) = {f(x + h) - f(x - h)} / 2h + O(h^2)
          EvaluateModelValues(parameters + h, _f1, false);
          EvaluateModelValues(parameters - h, _f2, false);

          var prime = (_f1 - _f2) / (2 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else
        {
          // f'(x) = {- f(x) + f(x + h)} / h + O(h)
          var f1 = currentValues;
          EvaluateModelValues(parameters + h, _f2, false);

          var prime = (-f1 + _f2) / h[j];
          derivatives.SetColumn(j, prime);
        }

        h[j] = 0;
      }

      return _jacobianValue;
    }



  }
}
