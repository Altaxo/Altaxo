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
  public class LevMarAdapter2 : NonlinearObjectiveFunctionNonAllocatingBase
  {
    #region CachedFitElementInfo

    /// <summary>Caches the temporary memory space of one fit element.</summary>
    private class CachedFitElementInfo
    {
      /// <summary>Parameter array for temporary purpose.</summary>
      public double[] Parameters;

      /// <summary>Value result array for temporary purpose.</summary>
      public double[] Ys;

      /// <summary>Independent variable array for temporary purpose. If the fit function has one independent variable (as is the case for
      /// the majority of fit functions, the matrix has only one column, and as many rows as there are independent data points.</summary>
      public IROMatrix<double> Xs;

      /// <summary>Array of jacobians (derivatives of the function value with respect to the parameters) for temporary purpose.</summary>
      public IMatrix<double>? DYs;

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
        double[] ys,
        IMatrix<double>? dYs,
        int[] parameterMapping,
        bool[] dependentVariablesInUse,
        IAscendingIntegerCollection validRows
        )
      {
        Parameters = parameters;
        Xs = xs;
        Ys = ys;
        DYs = dYs;
        ParameterMapping = parameterMapping;
        DependentVariablesInUse = dependentVariablesInUse;
        NumberOfDependentVariablesInUse = DependentVariablesInUse.Count(x => x);
        ValidRows = validRows;
      }
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

    /// <summary>Holds (after the fit) the resulting covariances of the parameters.</summary>
    private double[]? _resultingCovariances;



    /// <summary>
    /// Constructor of the adapter.
    /// </summary>
    /// <param name="ensemble">The fit ensemble, i.e. the functions and data you intend to fit.</param>
    /// <param name="paraSet">The set of initial parameter. Must contain a initial estimation of the parameters. Contains also information which
    /// parameters can vary and which are fixed during the fitting procedure.</param>
    public LevMarAdapter2(FitEnsemble ensemble, ParameterSet paraSet) : base(1)
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
        for (int r = 0; r < numberOfValidRows; ++r)
        {
          for (int c = 0; c < xs.ColumnCount; ++c)
          {
            xs[r, c] = fitElement.IndependentVariables(c)?[validRows[idxFitElement]] ?? throw new ObjectDisposedException($"Independent variables column k={c} not available or disposed.");
          }
        }

        var parameters = new double[fitElement.NumberOfParameters];
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
          }
          else
          {
            parameterMapping[j] = -idx - 1;
          }
        }

        _cachedFitElementInfo[idxFitElement] = new CachedFitElementInfo(
          parameters: parameters,
          xs: xs,
          ys: ys,
          dYs: null,
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
    }

    public void CopyParametersBackTo(ParameterSet pset)
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
          pset[i].Parameter = _coefficients[varyingPara];
          pset[i].Variance = _resultingCovariances is null ? 0 : Math.Sqrt(_resultingCovariances[varyingPara + varyingPara * _coefficients.Count]);
          varyingPara++;
        }
        else
        {
          pset[i].Parameter = _constantParameters[i];
          pset[i].Variance = 0;
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
    /// Gets the covariance matrix. The covariance matrix has always a dimension of n x n, with n the number of varying parameters.
    /// </summary>
    /// <value>
    /// The covariance matrix.
    /// </value>
    public double[] CovarianceMatrix
    {
      get
      {
        if (_resultingCovariances is null)
          throw new InvalidOperationException("Please call one of the Fit functions first before accessing the result.");
        return _resultingCovariances;
      }
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
    /// <param name="parameter">The parameter used to calculate the values.</param>
    /// <param name="outputValues">You must provide an array to hold the calculated values. Size of the array must be
    /// at least <see cref="NumberOfObservations" />.</param>
    /// <param name="calculateUnusedDependentVariablesAlso">If <c>true</c>, the unused dependent variables are also calculated (and plotted).</param>
    /// <remarks>The values of the fit elements are stored in the order from element_0 to element_n. If there is more
    /// than one used dependent variable per fit element, the output values are stored in interleaved order.
    /// </remarks>
    public void EvaluateModelValues(Vector<double> outputValues, bool calculateUnusedDependentVariablesAlso = false)
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
          info.Parameters[i] = idx >= 0 ? _coefficients[idx] : _constantParameters[-1 - idx];
        }

        IAscendingIntegerCollection validRows = info.ValidRows;
        int numberOfValidRows = validRows.Count;
        // Evaluate the function for all points

        if (calculateUnusedDependentVariablesAlso)
        {
          // calculate all dependent variables
          fitEle.FitFunctionEvaluate(info.Xs, info.Parameters, null, VectorMath.ToVector(outputValues, outputValuesPointer, outputValues.Count - outputValuesPointer));
          outputValuesPointer += info.Xs.RowCount * info.DependentVariablesInUse.Length;
        }
        else
        {
          // usual case: do not calculate the unused dependent variables
          fitEle.FitFunctionEvaluate(info.Xs, info.Parameters, info.DependentVariablesInUse, VectorMath.ToVector(outputValues, outputValuesPointer, outputValues.Count - outputValuesPointer));
          outputValuesPointer += info.Xs.RowCount * info.NumberOfDependentVariablesInUse;
        }

        /*
        if (calculateUnusedDependentVariablesAlso)
        {
          // copy the evaluation result to the output array (interleaved)
          for (int k = 0; k < fitEle.NumberOfDependentVariables; ++k)
            outputValues[outputValuesPointer++] = info.Ys[k];
        }
        else
        {
          // copy the evaluation result to the output array (interleaved)
          for (int k = 0; k < info.DependentVariablesInUse.Length; ++k)
            outputValues[outputValuesPointer++] = info.Ys[info.DependentVariablesInUse[k]];
        }
        */

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
      int columnOffset = 0; // offset into the jacobian columns (by every fit element the increase is by the number of free parameters)
      int rowOffset = 0; // offset into the jacobian rows (by every fit element the increase is by (NumberOfX*NumberOfDependentVariablesInUse))

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

        if (fitEle.FitFunction is IFitFunctionWithGradient fitFunctionWithDerivative)
        {
          fitFunctionWithDerivative.EvaluateGradient(info.Xs, info.Parameters, info.DependentVariablesInUse, info.DYs);
          // Copy the derivatives to there right places
          for (int i = 0; i < info.Parameters.Length; i++)
          {
            int idx = info.ParameterMapping[i];
            if (idx >= 0) // this parameter is in use
            {
              MatrixMath.CopyColumn(info.DYs, i, _jacobianValue, rowOffset, columnOffset);
              ++columnOffset;
            }
          }
          rowOffset += info.Xs.RowCount * info.NumberOfDependentVariablesInUse;
          ++JacobianEvaluations;
        }
        else
        {
          throw new NotImplementedException();
          FunctionEvaluations += _accuracyOrder;
        }
      }

      // weighted jacobian
      if (IsFixed is not null)
      {
        for (int j = 0; j < NumberOfParameters; j++)
        {
          if (IsFixed[j])
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
      throw new NotImplementedException();
    }


  }
}
