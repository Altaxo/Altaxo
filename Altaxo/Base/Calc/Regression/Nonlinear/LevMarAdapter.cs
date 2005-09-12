using System;
using System.Text;
using Altaxo.Collections;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Adapts a <see>FitEnsemble</see> to the requirements of a Levenberg-Marquardt fitting procedure.
  /// </summary>
  public class LevMarAdapter
  {
    #region inner classes
    /// <summary>Caches the temporary memory space of one fit element.</summary>
    class CachedFitElementInfo
    {
      /// <summary>Parameter array for temporary purpose.</summary>
      public double[] Parameters;
      /// <summary>Value result array for temporary purpose.</summary>
      public double[] Ys;
      /// <summary>Independent variable array for temporary purpose.</summary>
      public double[] Xs;

      /// <summary>Parameter mapping from the global parameter list to the local parameter list. Positive entries
      /// give the position in the variable parameter list, negative entries gives the position -entry-1 in the 
      /// constant parameter list.
      /// </summary>
      public int[] ParameterMapping;

      /// <summary>Designates which dependent variable columns are really in use.</summary>
      public int[] DependentVariablesInUse;

      /// <summary>
      /// Information, which of the rows are valid, i.e. all independent columns contains values, and all used dependent columns contain values in those rows.
      /// </summary>
      public IAscendingIntegerCollection ValidRows;
    }
    #endregion

    /// <summary>The fit ensemble this adapter adapts.</summary>
    FitEnsemble _fitEnsemble;

    /// <summary>
    /// List of constant parameters (i.e. parameters that are not changed during the fitting session).
    /// For convinience, all parameters are stored here (the varying parameters too), but only the constant parameters are used.
    /// </summary>
    double[] _constantParameters;

    /// <summary>Caches the temporary information of all fit elements.</summary>
    CachedFitElementInfo[] _cachedFitElementInfo;

    /// <summary>Number of total valid data points (y-values) of the fit ensemble.</summary>
    int _cachedNumberOfData;

    /// <summary>Holds the parameters that can vary during the fit.</summary>
    double[] _cachedVaryingParameters;

    double[] _cachedDependentValues; // during the fitting procedure, this holds the original y data
  
    double[] _resultingCovariances;
    double  _resultingSumChiSquare;

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
    void CalculateCachedData(ParameterSet paraSet)
    {
      // Preparation: Store the parameter names by name and index, and store
      // all parameter values in _constantParameters
      System.Collections.Hashtable paraNames = new System.Collections.Hashtable();
      System.Collections.Hashtable varyingParaNames = new System.Collections.Hashtable();

      _constantParameters = new double[paraSet.Count];
      int numberOfVaryingParameters = 0;
      for (int i = 0; i < paraSet.Count; ++i)
      {
        paraNames.Add(paraSet[i].Name, i);
        _constantParameters[i] = paraSet[i].Parameter;
        if(paraSet[i].Vary)
          ++numberOfVaryingParameters;
      }
      _cachedVaryingParameters = new double[numberOfVaryingParameters];
      for (int i = 0, k=0; i < paraSet.Count; ++i)
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
        CachedFitElementInfo info = new CachedFitElementInfo();
        _cachedFitElementInfo[i] = info;
        FitElement fitEle = _fitEnsemble[i];

        info.ValidRows = fitEle.CalculateValidNumericRows();

        info.Xs = new double[fitEle.NumberOfIndependentVariables];
        info.Parameters = new double[fitEle.NumberOfParameters];
        info.Ys = new double[fitEle.NumberOfDependentVariables];
        
        // Calculate the number of used variables
        int numVariablesUsed=0;
        for(int j=0;j<fitEle.NumberOfDependentVariables;++j)
        {
          if(fitEle.DependentVariables(j)!=null)
            ++numVariablesUsed;
        }
        info.DependentVariablesInUse = new int[numVariablesUsed];
        for (int j = 0, used=0; j < fitEle.NumberOfDependentVariables; ++j)
        {
          if (fitEle.DependentVariables(j) != null)
            info.DependentVariablesInUse[used++] = j;
        }

        // calculate the total number of data points
        _cachedNumberOfData += numVariablesUsed * info.ValidRows.Count;


        // now create the parameter mapping
        info.ParameterMapping = new int[fitEle.NumberOfParameters];

        for (int j = 0; j < info.ParameterMapping.Length; ++j)
        {
          if(!paraNames.Contains(fitEle.ParameterName(j)))
            throw new ArgumentException(string.Format("ParameterSet does not contain parameter {0}, which is used by function[{1}]",fitEle.ParameterName(j),i));

          int idx = (int)paraNames[fitEle.ParameterName(j)];
          if (paraSet[idx].Vary)
          {
            info.ParameterMapping[j] = (int)varyingParaNames[fitEle.ParameterName(j)];
          }
          else
          {
            info.ParameterMapping[j] = -idx - 1;
          }
        }
      }

      _cachedDependentValues = new double[_cachedNumberOfData];
      GetDependentValues(_cachedDependentValues);
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
    /// Stores the real data points ("measured data" or "dependent values") in an array. The data
    /// are stored from FitElement_0 to FitElement_n. For FitElements with more than one dependent
    /// variable in use, the data are stored interleaved.
    /// </summary>
    /// <param name="values"></param>
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
            values[outputValuesPointer++] = fitEle.DependentVariables(j)[validRows[i]];
          }
        }
      }
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
      EvaluateFitValues(parameter, ys);
      for(int i=ys.Length-1;i>=0;--i)
      {
        ys[i] -= _cachedDependentValues[i];
      }
    }


    /// <summary>
    /// Calculates the fitting values.
    /// </summary>
    /// <param name="param">The parameter used to calculate the values.</param>
    /// <param name="outputValues">You must provide an array to hold the calculated values. Size of the array must be
    /// at least <see>NumberOfData</see>.</param>
    /// <remarks>The values of the fit elements are stored in the order from element_0 to element_n. If there is more
    /// than one used dependent variable per fit element, the output values are stored in interleaved order.
    /// </remarks>
    public void EvalulateFitValues(double[] parameter, double[] outputValues, object additionalData)
    {
      EvaluateFitValues(parameter, outputValues);
    }

    /// <summary>
    /// Calculates the fitting values.
    /// </summary>
    /// <param name="param">The parameter used to calculate the values.</param>
    /// <param name="outputValues">You must provide an array to hold the calculated values. Size of the array must be
    /// at least <see>NumberOfData</see>.</param>
    /// <remarks>The values of the fit elements are stored in the order from element_0 to element_n. If there is more
    /// than one used dependent variable per fit element, the output values are stored in interleaved order.
    /// </remarks>
    public void EvaluateFitValues(double [] parameter, double[] outputValues)
    {
      int outputValuesPointer = 0;
      for (int ele = 0; ele < _cachedFitElementInfo.Length; ele++)
      {
        CachedFitElementInfo info = _cachedFitElementInfo[ele];
        FitElement fitEle = _fitEnsemble[ele];
        
        // copy of the parameter to the temporary array
        for (int i = 0; i < info.Parameters.Length; i++)
        {
          int idx = info.ParameterMapping[i];
          info.Parameters[i] = idx>=0 ? parameter[idx] : _constantParameters[-1-idx];
        }

       
        IAscendingIntegerCollection validRows = info.ValidRows;
        int numValidRows = validRows.Count;
        // Evaluate the function for all points
        for (int i = 0; i < numValidRows; ++i)
        {
          for (int k = info.Xs.Length - 1; k >= 0; k--)
            info.Xs[k] = fitEle.IndependentVariables(k)[validRows[i]];

          fitEle.FitFunction.Evaluate(info.Xs, info.Parameters, info.Ys);

          // copy the evaluation result to the output array (interleaved)
          for (int k = 0; k < info.DependentVariablesInUse.Length; ++k)
            outputValues[outputValuesPointer++] = info.Ys[info.DependentVariablesInUse[k]];
        }
      }
    }

    public double EvaluateChiSquare()
    {
      int info=0;
      double[] differences = new double[this.NumberOfData];
      EvaluateFitDifferences(  
        NumberOfData, 
        _cachedVaryingParameters.Length,
        _cachedVaryingParameters,
        differences,
        ref info);

      _resultingSumChiSquare=0;
      for(int i=differences.Length-1;i>=0;--i)
      {
        _resultingSumChiSquare += differences[i]*differences[i];
      }

      return _resultingSumChiSquare;
    }

  

    public void Fit()
    {
      int info=0;
      double[] differences = new double[this.NumberOfData];
      NLFit.LevenbergMarquardtFit(new NLFit.LMFunction(this.EvaluateFitDifferences),_cachedVaryingParameters,differences,1E-10,ref info);

      _resultingCovariances = new double[_cachedVaryingParameters.Length*_cachedVaryingParameters.Length];
      NLFit.ComputeCovariances(new NLFit.LMFunction(this.EvaluateFitDifferences), _cachedVaryingParameters, NumberOfData, _cachedVaryingParameters.Length,  _resultingCovariances, out _resultingSumChiSquare);
      
    }


    public double ResultingChiSquare
    {
      get
      {
        return _resultingSumChiSquare;
      }
    }

    public void CopyParametersBackTo(ParameterSet pset)
    {
      for (int ele = 0; ele < _cachedFitElementInfo.Length; ele++)
      {
        CachedFitElementInfo info = _cachedFitElementInfo[ele];
        FitElement fitEle = _fitEnsemble[ele];
        
        // copy of the parameter to the temporary array
        for (int i = 0; i < info.Parameters.Length; i++)
        {
          int idx = info.ParameterMapping[i];
          pset[i].Parameter = idx>=0 ? this._cachedVaryingParameters[idx] : _constantParameters[-1-idx];
          pset[i].Variance = idx>=0 && _resultingCovariances!=null ? _resultingCovariances[idx+idx*_cachedVaryingParameters.Length] : 0;
        }
      }
       pset.OnInitializationFinished();
    }
  }
}
