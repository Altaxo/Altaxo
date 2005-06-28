using System;
using System.Collections;

namespace Altaxo.Calc.Regression.Nonlinear
{
	/// <summary>
	/// Holds a collection of <see>FitElement</see>s and is responsible for parameter
	/// bundling.
	/// </summary>
	public class FitEnsemble : System.Collections.CollectionBase
	{
    /// <summary>
    /// Current parameter names
    /// </summary>
    string[] _parameterNames = new string[0];
    /// <summary>
    /// Current parameter values;
    /// </summary>
    double[] _parameterValues = new double[0];
    /// <summary>
    /// All parameters sorted by name.
    /// </summary>
    System.Collections.SortedList _ParametersSortedByName;

    public FitElement this[int i]
    {
      get 
      {
        return (FitElement)InnerList[i];
      }
      set
      {
        InnerList[i] = value;
      }
    }

    public void Add(FitElement e)
    {
      InnerList.Add(e);
    }


    
    #region Fit parameters

    protected void CollectParameterNames()
    {
      // Save the old parameter values
      Hashtable oldParameterValues = new Hashtable();
      for(int i=0;i<_parameterNames.Length;i++)
        oldParameterValues.Add(_parameterNames[i],_parameterValues[i]);

      _ParametersSortedByName = new System.Collections.SortedList();

      int nameposition = 0;
      for(int i=0;i<InnerList.Count;i++)
      {
        if(null==this[i].FitFunction)
          continue;
        IFitFunction func = this[i].FitFunction;
        FitElement ele = this[i];

        for(int k=0;k<func.NumberOfParameters;k++)
        {
          if(!(_ParametersSortedByName.ContainsKey(ele.ParameterName(k))))
          {
            _ParametersSortedByName.Add(ele.ParameterName(k),nameposition++);
          }
        }
      }

      // now sort the items in the order of the namepositions
      System.Collections.SortedList sortedbypos = new System.Collections.SortedList();
      foreach(DictionaryEntry en in _ParametersSortedByName)
        sortedbypos.Add(en.Value,en.Key);


      _parameterNames = new string[sortedbypos.Count];
      for(int i=0;i<_parameterNames.Length;i++)
        _parameterNames[i] = (string)sortedbypos[i];

      // restore the parameter values
      _parameterValues = new double[_parameterNames.Length];
      for(int i=0;i<_parameterNames.Length;i++)
      {
        if(oldParameterValues.Contains(_parameterNames[i]))
          _parameterValues[i] = (double)oldParameterValues[_parameterNames[i]];
      }
    }


    /// <summary>
    /// User-supplied subroutine which calculates the functions to minimize.
    /// Calculates <c>numberOfYs</c> functions dependent on <c>numberOfParameter</c> parameters and
    /// returns the calculated y values in array <c>ys</c>. The value of <c>info</c> should
    /// not be changed unless  the user wants to terminate execution of LevenbergMarquardtFit. 
    /// In this case set iflag to a negative integer. 
    /// </summary>
    public void LMFunction(
      int numberOfYs, 
      int numberOfParameter,
      double[] parameter,
      double[] ys,
      ref int info)
    {
    }

    public void InitializeFittingSession()
    {
      CollectParameterNames();

      for(int i=0;i<InnerList.Count;i++)
        this[i].InitializeFittingSession();

      // For every fit element, we need temporary:
      // - an array of doubles to hold the parameters
      // - an array of doubles to receive the results
      // - a fast method to map the global parameter list to the fit element's parameter list
      _cachedFitElementInfo = new CachedFitElementInfo[InnerList.Count];

      for(int ele=0;ele<InnerList.Count;ele++)
      { _cachedFitElementInfo[ele] = new CachedFitElementInfo();
        _cachedFitElementInfo[ele].Parameters = new double[this[ele].FitFunction.NumberOfParameters];
        _cachedFitElementInfo[ele].Ys = new double[this[ele].FitFunction.NumberOfDependentVariables];
        _cachedFitElementInfo[ele].ParameterMapping = new int[this[ele].FitFunction.NumberOfParameters];

        // Create the parameter mapping
        for(int k=0;k<_cachedFitElementInfo[ele].Parameters.Length;k++)
        {
          _cachedFitElementInfo[ele].ParameterMapping[k] = (int)_ParametersSortedByName[this[ele].ParameterName(k)];
        }
      }
    }

    class CachedFitElementInfo
    {
      public double[] Parameters;
      public double [] Ys;
      public int [] ParameterMapping;
    }
    CachedFitElementInfo[] _cachedFitElementInfo;

    public void Evaluate(double[] parameter, double[] ys)
    {
      int ysoffset=0;
      for(int ele=0;ele<InnerList.Count;ele++)
      {
        CachedFitElementInfo info = _cachedFitElementInfo[ele];

        // copy of the parameter
        for(int i=0;i<info.Parameters.Length;i++)
          info.Parameters[i] = parameter[info.ParameterMapping[i]];

        this[ele].Evaluate(info.Parameters,info.Ys);

      
        for(int i=this[ele].NumberOfDependentVariables-1;i>=0;--i)
          ys[i+ysoffset] = info.Ys[i];

        ysoffset += this[ele].NumberOfDependentVariables;
      }

    }

    #endregion
	}
}
