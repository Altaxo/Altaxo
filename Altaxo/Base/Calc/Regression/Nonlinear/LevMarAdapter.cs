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
using System.Threading;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Adapts a <see cref="FitEnsemble" /> to the requirements of a Levenberg-Marquardt fitting procedure.
  /// This means, the adapter makes the <see cref="FitEnsemble" /> compatible with the Levenberg-Marquardt algorithm.
  /// </summary>
  public class LevMarAdapter
  {
    #region inner classes

    /// <summary>Caches the temporary memory space of one fit element.</summary>
    private class CachedFitElementInfo
    {
      /// <summary>Parameter array for temporary purpose.</summary>
      public double[] Parameters;

      /// <summary>Value result array for temporary purpose.</summary>
      public double[] Ys;

      /// <summary>Independent variable array for temporary purpose.</summary>
      public double[] Xs;

      /// <summary>Array of jacobians (derivatives of the function value with respect to the parameters) for temporary purpose.</summary>
      public IMatrix<double>? DYs;

      /// <summary>Parameter mapping from the local parameter list to the global parameter list. Positive entries
      /// give the position in the global variable parameter list, negative entries gives the position -entry-1 in the
      /// global constant parameter list.
      /// </summary>
      public int[] ParameterMapping;

      /// <summary>Designates which dependent variable columns are really in use.</summary>
      public int[] DependentVariablesInUse;

      /// <summary>
      /// Information, which of the rows are valid, i.e. all independent columns contains values, and all used dependent columns contain values in those rows.
      /// </summary>
      public IAscendingIntegerCollection ValidRows;

      public CachedFitElementInfo(
        double[] parameters,
        double[] xs,
        double[] ys,
        IMatrix<double>? dYs,
        int[] parameterMapping,
        int[] dependentVariablesInUse,
        IAscendingIntegerCollection validRows
        )
      {
        Parameters = parameters;
        Xs = xs;
        Ys = ys;
        DYs = dYs;
        ParameterMapping = parameterMapping;
        DependentVariablesInUse = dependentVariablesInUse;
        ValidRows = validRows;
      }
    }

    #endregion inner classes

    /// <summary>The fit ensemble this adapter is using.</summary>
    private FitEnsemble _fitEnsemble;

    /// <summary>
    /// List of constant parameters (i.e. parameters that are not changed during the fitting session).
    /// For convenience, all parameters are stored here (the varying parameters too), but only the constant parameters are used.
    /// </summary>
    private double[] _constantParameters;

    /// <summary>Caches the temporary information of all fit elements.</summary>
    private CachedFitElementInfo[] _cachedFitElementInfo;

    /// <summary>Number of total valid data points (y-values) of the fit ensemble.</summary>
    private int _cachedNumberOfData;

    /// <summary>Holds the parameters that can vary during the fit.</summary>
    private double[] _cachedVaryingParameters;

    /// <summary>During the fitting procedure, this holds the original y data.</summary>
    private double[] _cachedDependentValues; //

    /// <summary>If this array is set, the weights are used to scale the fit differences (yreal-yfit).</summary>
    private double[]? _cachedWeights;

    /// <summary>Holds (after the fit) the resulting covariances of the parameters.</summary>
    private double[]? _resultingCovariances;

    /// <summary>After the fit, this is the resulting sum of squares of the deviations.</summary>
    private double _resultingSumChiSquare;

    /// <summary>The resulting sigma square, computed as SumChiSquare/(n-r)</summary>
    private double _resultingSigmaSquare;

    /// <summary>
    /// Constructor of the adapter.
    /// </summary>
    /// <param name="ensemble">The fit ensemble, i.e. the functions and data you intend to fit.</param>
    /// <param name="paraSet">The set of initial parameter. Must contain a initial estimation of the parameters. Contains also information which
    /// parameters can vary and which are fixed during the fitting procedure.</param>
    public LevMarAdapter(FitEnsemble ensemble, ParameterSet paraSet)
    {
      _fitEnsemble = ensemble;

      CalculateCachedData(paraSet);
    }

    /// <summary>
    /// Internal function to set up the cached data for the fitting procedure.
    /// </summary>
    /// <param name="paraSet">The set of parameters (the information which parameters are fixed is mainly used here).</param>
    [MemberNotNull(nameof(_constantParameters), nameof(_cachedVaryingParameters), nameof(_cachedFitElementInfo), nameof(_cachedDependentValues))]
    private void CalculateCachedData(ParameterSet paraSet)
    {
      // Preparation: Store the parameter names by name and index, and store
      // all parameter values in _constantParameters
      var paraNames = new Dictionary<string, int>();
      var varyingParaNames = new Dictionary<string, int>();

      _constantParameters = new double[paraSet.Count];
      int numberOfVaryingParameters = 0;
      for (int i = 0; i < paraSet.Count; ++i)
      {
        paraNames.Add(paraSet[i].Name, i);
        _constantParameters[i] = paraSet[i].Parameter;
        if (paraSet[i].Vary)
          ++numberOfVaryingParameters;
      }
      _cachedVaryingParameters = new double[numberOfVaryingParameters];
      for (int i = 0, k = 0; i < paraSet.Count; ++i)
      {
        if (paraSet[i].Vary)
        {
          varyingParaNames.Add(paraSet[i].Name, k);
          _cachedVaryingParameters[k++] = paraSet[i].Parameter;
        }
      }

      _cachedNumberOfData = 0;
      _cachedFitElementInfo = new CachedFitElementInfo[_fitEnsemble.Count];
      for (int i = 0; i < _fitEnsemble.Count; i++)
      {
        // var info = new CachedFitElementInfo();

        FitElement fitEle = _fitEnsemble[i];

        var validRows = fitEle.CalculateValidNumericRows();

        var xs = new double[fitEle.NumberOfIndependentVariables];
        var parameters = new double[fitEle.NumberOfParameters];
        var ys = new double[fitEle.NumberOfDependentVariables];

        // Calculate the number of used variables
        int numVariablesUsed = 0;
        for (int j = 0; j < fitEle.NumberOfDependentVariables; ++j)
        {
          if (fitEle.DependentVariables(j) is not null)
            ++numVariablesUsed;
        }
        var dependentVariablesInUse = new int[numVariablesUsed];
        for (int j = 0, used = 0; j < fitEle.NumberOfDependentVariables; ++j)
        {
          if (fitEle.DependentVariables(j) is not null)
            dependentVariablesInUse[used++] = j;
        }

        // calculate the total number of data points
        _cachedNumberOfData += numVariablesUsed * validRows.Count;

        // now create the parameter mapping
        var parameterMapping = new int[fitEle.NumberOfParameters];

        for (int j = 0; j < parameterMapping.Length; ++j)
        {
          if (!paraNames.ContainsKey(fitEle.ParameterName(j)))
            throw new ArgumentException(string.Format("ParameterSet does not contain parameter {0}, which is used by function[{1}]", fitEle.ParameterName(j), i));

          int idx = (int)paraNames[fitEle.ParameterName(j)];
          if (paraSet[idx].Vary)
          {
            parameterMapping[j] = (int)varyingParaNames[fitEle.ParameterName(j)];
          }
          else
          {
            parameterMapping[j] = -idx - 1;
          }
        }

        _cachedFitElementInfo[i] = new CachedFitElementInfo(
          parameters: parameters,
          xs: xs,
          ys: ys,
          dYs: null,
          parameterMapping: parameterMapping,
          dependentVariablesInUse: dependentVariablesInUse,
          validRows: validRows
          ); ;


      }

      _cachedDependentValues = new double[_cachedNumberOfData];
      GetDependentValues(_cachedDependentValues);

      if (HasToUseWeights())
      {
        _cachedWeights = new double[_cachedNumberOfData];
        GetWeights(_cachedWeights);
      }
      else
      {

        _cachedWeights = null;
      }
    }

    /// <summary>Number of total valid data points (y-values) of the fit ensemble. This is the array
    /// size you will need to store the fitting functions output.</summary>
    public int NumberOfData
    {
      get
      {
        return _cachedNumberOfData;
      }
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
    /// Stores the dependent values of all elements in an array. The data
    /// are stored from FitElement_0 to FitElement_n. For FitElements with more than one dependent
    /// variable in use, the data are stored interleaved.
    /// </summary>
    /// <param name="values">The array used to store the values.</param>
    public void GetDependentValues(double[] values)
    {
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
            values[outputValuesPointer++] = fitEle.DependentVariables(info.DependentVariablesInUse[j])?[validRows[i]] ?? double.NaN;
          }
        }
      }
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
    /// Provides an adapter for the NLFit.LMFunction interface.
    /// </summary>
    /// <param name="numberOfYs">Number of dependent variables. Is ignored. Instead, the length of the ys array is used.</param>
    /// <param name="numberOfParameter">Number of parameters. Is ignored here. Instead, the length of the parameter array is used.</param>
    /// <param name="parameter">Array of parameters.</param>
    /// <param name="ys">Output: this holds the calculated differences between dependent values and model.</param>
    /// <param name="info">Not used here.</param>
    public void EvaluateFitDifferences(
        int numberOfYs,
        int numberOfParameter,
        double[] parameter,
        double[] ys,
        ref int info)
    {
      EvaluateFitValues(parameter, ys, false);

      if (_cachedWeights is null)
      {
        for (int i = ys.Length - 1; i >= 0; --i)
        {
          ys[i] -= _cachedDependentValues[i];
        }
      }
      else
      {
        for (int i = ys.Length - 1; i >= 0; --i)
        {
          ys[i] = (ys[i] - _cachedDependentValues[i]) * _cachedWeights[i];
        }
      }
    }

    /// <summary>
    /// Calculates the fitting values.
    /// </summary>
    /// <param name="parameter">The parameter used to calculate the values.</param>
    /// <param name="outputValues">You must provide an array to hold the calculated values. Size of the array must be
    /// at least <see cref="NumberOfData" />.</param>
    /// <param name="additionalData">Currently ignored.</param>
    /// <remarks>The values of the fit elements are stored in the order from element_0 to element_n. If there is more
    /// than one used dependent variable per fit element, the output values are stored in interleaved order.
    /// </remarks>
    public void EvalulateFitValues(double[] parameter, double[] outputValues, object? additionalData)
    {
      EvaluateFitValues(parameter, outputValues, false);
    }

    /// <summary>
    /// Calculates the fitting values.
    /// </summary>
    /// <param name="parameter">The parameter used to calculate the values.</param>
    /// <param name="outputValues">You must provide an array to hold the calculated values. Size of the array must be
    /// at least <see cref="NumberOfData" />.</param>
    /// <param name="calculateUnusedDependentVariablesAlso">If <c>true</c>, the unused dependent variables are also calculated (and plotted).</param>
    /// <remarks>The values of the fit elements are stored in the order from element_0 to element_n. If there is more
    /// than one used dependent variable per fit element, the output values are stored in interleaved order.
    /// </remarks>
    public void EvaluateFitValues(double[] parameter, double[] outputValues, bool calculateUnusedDependentVariablesAlso)
    {
      int outputValuesPointer = 0;
      for (int ele = 0; ele < _cachedFitElementInfo.Length; ele++)
      {
        CachedFitElementInfo info = _cachedFitElementInfo[ele];
        FitElement fitEle = _fitEnsemble[ele];

        if (fitEle.FitFunction is null)
          throw new InvalidOperationException($"FitFunction of FitElement[{ele}] is not set.");

        // copy of the parameter to the temporary array
        for (int i = 0; i < info.Parameters.Length; i++)
        {
          int idx = info.ParameterMapping[i];
          info.Parameters[i] = idx >= 0 ? parameter[idx] : _constantParameters[-1 - idx];
        }

        IAscendingIntegerCollection validRows = info.ValidRows;
        int numValidRows = validRows.Count;
        // Evaluate the function for all points
        for (int i = 0; i < numValidRows; ++i)
        {
          for (int k = info.Xs.Length - 1; k >= 0; k--)
            info.Xs[k] = fitEle.IndependentVariables(k)?[validRows[i]] ?? throw new ObjectDisposedException($"Independent variables column k={k} not available or disposed.");

          fitEle.FitFunctionEvaluate(info.Xs, info.Parameters, info.Ys);

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
        }
      }
    }

    /// <summary>
    /// Calculates the fitting values with the currently set parameters.
    /// </summary>
    /// <param name="outputValues">You must provide an array to hold the calculated values. Size of the array must be
    /// at least <see cref="NumberOfData" />.</param>
    /// <param name="calculateUnusedDependentVariablesAlso">If <c>true</c>, unused dependent variables are calculated and plotted too.</param>
    /// <remarks>The values of the fit elements are stored in the order from element_0 to element_n. If there is more
    /// than one used dependent variable per fit element, the output values are stored in interleaved order.
    /// </remarks>
    public void EvaluateFitValues(double[] outputValues, bool calculateUnusedDependentVariablesAlso)
    {
      EvaluateFitValues(_cachedVaryingParameters, outputValues, calculateUnusedDependentVariablesAlso);
    }

    /// <summary>
    /// Copies the current parameters for the fit element with the provided index into the provided array.
    /// </summary>
    /// <param name="idxFitEle">Index of the fit element.</param>
    /// <param name="parameters">Provided array to copy the current parameters to. Must have same size as NumberOfParameters for the given fit element.</param>
    public void GetParameters(int idxFitEle, double[] parameters)
    {
      CachedFitElementInfo info = _cachedFitElementInfo[idxFitEle];
      // copy of the parameter to the temporary array
      for (int i = 0; i < info.Parameters.Length; i++)
      {
        int idx = info.ParameterMapping[i];
        parameters[i] = idx >= 0 ? _cachedVaryingParameters[idx] : _constantParameters[-1 - idx];
      }
    }

    /// <summary>
    /// Calculates the jacobian values, i.e. the derivatives of the fitting values with respect to the parameters.
    /// </summary>
    /// <param name="parameter">The parameter used to calculate the values.</param>
    /// <param name="outputValues">You must provide an array to hold the calculated values. Size of the array must be
    /// at least <see cref="NumberOfData" />*<see cref="FitElement.NumberOfParameters" />.</param>
    /// <param name="adata">Currently ignored.</param>
    /// <remarks>The values of the fit elements are stored in the order from element_0 to element_n*m. If there is more
    /// than one used dependent variable per fit element, the output values are stored in interleaved order. The derivatives
    /// on one fitting value  are stored in successive order.
    /// </remarks>
    public void EvaluateFitJacobian(double[] parameter, double[] outputValues, object? adata)
    {
      outputValues.Initialize(); // make sure every element contains zero

      int outputValuesPointer = 0;
      for (int ele = 0; ele < _cachedFitElementInfo.Length; ele++)
      {
        CachedFitElementInfo info = _cachedFitElementInfo[ele];
        FitElement fitEle = _fitEnsemble[ele];

        if (!(fitEle.FitFunction is IFitFunctionWithDerivative fitFunctionWithGradient))
          throw new InvalidOperationException($"FitFunction of FitElement[{ele}] must be implementing {nameof(IFitFunctionWithDerivative)}");

        // make sure, that the dimension of the DYs is ok
        if (info.DYs is null || info.DYs.RowCount != fitEle.NumberOfDependentVariables || info.DYs.ColumnCount != fitEle.NumberOfParameters)
          info.DYs = Matrix<double>.Build.Dense(fitEle.NumberOfDependentVariables, fitEle.NumberOfParameters);

        // copy of the parameter to the temporary array
        for (int i = 0; i < info.Parameters.Length; i++)
        {
          int idx = info.ParameterMapping[i];
          info.Parameters[i] = idx >= 0 ? parameter[idx] : _constantParameters[-1 - idx];
        }

        IAscendingIntegerCollection validRows = info.ValidRows;
        int numValidRows = validRows.Count;
        // Evaluate the function for all points
        for (int i = 0; i < numValidRows; ++i)
        {
          for (int k = info.Xs.Length - 1; k >= 0; k--)
            info.Xs[k] = fitEle.IndependentVariables(k)?[validRows[i]] ?? throw new ObjectDisposedException($"Independent variables column k={k} not available or disposed."); ;


          ((IFitFunctionWithDerivative)fitEle.FitFunction).EvaluateDerivative(MatrixMath.ToROMatrixWithOneRow(info.Xs), info.Parameters, null, info.DYs, null);

          // copy the evaluation result to the output array (interleaved)
          for (int k = 0; k < info.DependentVariablesInUse.Length; ++k)
          {
            for (int l = 0; l < info.Parameters.Length; ++l)
            {
              int idx = info.ParameterMapping[l];
              if (idx >= 0)
                outputValues[outputValuesPointer + idx] += info.DYs[info.DependentVariablesInUse[k], l];
            }
            outputValuesPointer += parameter.Length; // increase output pointer only by the varying (!) number of parameters
          }
        }
      }
    }

    public double EvaluateChiSquare()
    {
      int info = 0;
      double[] differences = new double[NumberOfData];
      EvaluateFitDifferences(
          NumberOfData,
          _cachedVaryingParameters.Length,
          _cachedVaryingParameters,
          differences,
          ref info);

      _resultingSumChiSquare = 0;
      for (int i = differences.Length - 1; i >= 0; --i)
      {
        _resultingSumChiSquare += differences[i] * differences[i];
      }

      return _resultingSumChiSquare;
    }

    private class NelderMeadCostFunction : Calc.Optimization.CostFunction
    {
      private LevMarAdapter _adapter;

      public NelderMeadCostFunction(LevMarAdapter adapter)
      {
        _adapter = adapter;
      }

      public override double Value(Vector<double> x)
      {
        for (int i = 0; i < _adapter._cachedVaryingParameters.Length; ++i)
          _adapter._cachedVaryingParameters[i] = x[i];

        return _adapter.EvaluateChiSquare();
      }
    }

    public void DoSimplexMinimization(System.Threading.CancellationToken cancellationToken, Action<double> newMinimalCostValueFound)
    {
      var nm = new Altaxo.Calc.Optimization.NelderMead(new NelderMeadCostFunction(this));
      nm.Minimize(CreateVector.DenseOfArray(_cachedVaryingParameters), cancellationToken, newMinimalCostValueFound);
      for (int i = 0; i < _cachedVaryingParameters.Length; ++i)
        _cachedVaryingParameters[i] = nm.SolutionVector[i];
      _resultingSumChiSquare = nm.SolutionValue;
    }

    /// <summary>
    /// This function determines, wheter or not all fit functions provide the jacobians. In this case, the
    /// fitting procedure can make use of the jacobian.
    /// </summary>
    /// <returns>True if all fit functions provide the jacobian.</returns>
    public bool CanUseJacobianVersion()
    {
      for (int i = 0; i < _fitEnsemble.Count; i++)
      {
        if (_fitEnsemble[i].FitFunction is not null && !(_fitEnsemble[i].FitFunction is IFitFunctionWithDerivative))
          return false;
      }
      return true;
    }

    public void Fit(CancellationToken cancellationToken)
    {
      /* Up to new Fit2 is very slow, so we not use it until it is clear what causes this slow convergence
if (CanUseJacobianVersion())
    Fit2Jac();
else
*/
      Fit1(cancellationToken);
    }

    public void Fit1(CancellationToken cancellationToken)
    {
      int info = 0;
      double[] differences = new double[NumberOfData];
      NLFit.LevenbergMarquardtFit(new NLFit.LMFunction(EvaluateFitDifferences), _cachedVaryingParameters, differences, 1E-10, cancellationToken, ref info);

      _resultingCovariances = new double[_cachedVaryingParameters.Length * _cachedVaryingParameters.Length];
      NLFit.ComputeCovariances(new NLFit.LMFunction(EvaluateFitDifferences), _cachedVaryingParameters, NumberOfData, _cachedVaryingParameters.Length, _resultingCovariances, out _resultingSumChiSquare, out _resultingSigmaSquare);
    }

    /// <summary>
    /// Can only be used, if all fit functions provide the jacobian.
    /// </summary>
    public void Fit2Jac()
    {
      //int info = 0;
      object? workingmemory = null;

      NonLinearFit2.LEVMAR_DER(
          new NonLinearFit2.FitFunction(EvalulateFitValues),
          new NonLinearFit2.JacobianFunction(EvaluateFitJacobian),
          _cachedVaryingParameters,
          _cachedDependentValues,
          _cachedWeights,
          100, // itmax
          null, // opts,
          null, // info,
          ref workingmemory,
          null, // covar,
          null // arbitrary data
          );

      _resultingCovariances = new double[_cachedVaryingParameters.Length * _cachedVaryingParameters.Length];
      NLFit.ComputeCovariances(new NLFit.LMFunction(EvaluateFitDifferences), _cachedVaryingParameters, NumberOfData, _cachedVaryingParameters.Length, _resultingCovariances, out _resultingSumChiSquare, out _resultingSigmaSquare);
    }

    public double ResultingChiSquare
    {
      get
      {
        return _resultingSumChiSquare;
      }
    }

    public double ResultingSigmaSquare
    {
      get
      {
        return _resultingSigmaSquare;
      }
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

      if (varyingPara != _cachedVaryingParameters.Length)
        throw new ArgumentException("Number of varying parameters in pset does not match cached number of varying parameters");

      varyingPara = 0;
      for (int i = 0; i < pset.Count; i++)
      {
        if (pset[i].Vary)
        {
          pset[i] = pset[i] with
          {
            Parameter = _cachedVaryingParameters[varyingPara],
            Variance = _resultingCovariances is null ? 0 : Math.Sqrt(_resultingCovariances[varyingPara + varyingPara * _cachedVaryingParameters.Length]),
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
  }
}
