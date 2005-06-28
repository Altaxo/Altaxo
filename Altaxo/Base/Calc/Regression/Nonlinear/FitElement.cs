using System;
using Altaxo.Data;
using Altaxo.Collections;

namespace Altaxo.Calc.Regression.Nonlinear
{
	/// <summary>
	/// Holds the fit function together with the data sources for the independent and
	/// dependent variables.
	/// </summary>
  public class FitElement
  {
    #region internal classes
    class IntegerRange
    {
      int _first;
      int _count;

      protected IntegerRange()
      {
      }

      public static IntegerRange NewFromFirstAndCount(int first, int count)
      {
        return NewFromFirstAndEnd(first,first+count);
      }

      static public IntegerRange NewFromFirstAndEnd(int first, int end)
      {
        IntegerRange n = new IntegerRange();
        n._first = Math.Min(first,end);
        n._count = Math.Abs(first-end);
        return n;
      }

      public int First
      {
        get { return _first; }
      }
      public int Count
      {
        get { return _count; }
      }
      public int Last
      {
        get { return _first+_count-1; }
      }
      public int End
      {
        get
        {
          return _first + _count; 
        }
      }
    }
  
    class RangedNumericColumn
    {
      protected INumericColumn _column;
      private IntegerRange _range;

      protected IntegerRange Range
      {
        get { return _range; }
        set { _range = value; }
      }

      public INumericColumn Column
      {
        get
        {
          return _column;
        }
      }
    }
    class RangedNumericColumnAndErrorScaling : RangedNumericColumn
    {
      protected ErrorScaling _scaling;
    }

    class RangedNumericColumnArray : System.Collections.CollectionBase
    {
      public RangedNumericColumn this[int i]
      {
        get 
        {
          return (RangedNumericColumn)InnerList[i];
        }
        set
        {
          InnerList[i] = value;
        }
      }

      public void Add(RangedNumericColumn col)
      {
        InnerList.Add(col);
      }
    }

    class RangedNumericColumnAndErrorScalingArray : System.Collections.CollectionBase
    {
      public RangedNumericColumnAndErrorScaling this[int i]
      {
        get 
        {
          return (RangedNumericColumnAndErrorScaling)InnerList[i];
        }
        set
        {
          InnerList[i] = value;
        }
      }

      public void Add(RangedNumericColumnAndErrorScaling col)
      {
        InnerList.Add(col);
      }
    }


    #endregion
    /// <summary>
    /// Fitting function. Can be null if no fitting function was actually chosen.
    /// </summary>
    IFitFunction _fitFunction; 
    IntegerRange _rangeOfRows;
    INumericColumn[] _independentVariables;
    INumericColumn[] _dependentVariables;
    ErrorEvaluation[] _errorEvaluation;
    string [] _parameterNames = new string[0];
    string _parameterNameStart=string.Empty;

    // Cached values for the fitting session
    double [] _parameterValues;
    double [] _independentValues;
    double [] _dependentValuesResult;
    AscendingIntegerCollection _validNumericRows;


    public FitElement()
    {
    }

    public FitElement(INumericColumn xColumn, INumericColumn yColumn, int start, int count)
    {
      _independentVariables = new INumericColumn[1];
      _independentVariables[0] = xColumn;

      _dependentVariables = new INumericColumn[1];
      _dependentVariables[0] = yColumn;

      _rangeOfRows = IntegerRange.NewFromFirstAndCount(start,count);

    }

    public string ParameterName(int i)
    {
      if(null!=_fitFunction)
      {
        if(null==_parameterNames[i])
          return _parameterNameStart + _fitFunction.ParameterName(i);
        else
          return _parameterNames[i];
      }
      else
        return null;
    }

    public IFitFunction FitFunction
    {
      get
      {
        return _fitFunction;
      }
      set
      {
        _fitFunction = value;

        if(_fitFunction!=null)
        {
          if(_fitFunction.NumberOfIndependentVariables!=_independentVariables.Length)
            InternalReallocIndependentVariables(_fitFunction.NumberOfIndependentVariables);
          if(_fitFunction.NumberOfDependentVariables!=_dependentVariables.Length)
            InternalReallocDependentVariables(_fitFunction.NumberOfDependentVariables);
          if(_fitFunction.NumberOfParameters!=_parameterNames.Length)
            InternalReallocParameters(_fitFunction.NumberOfParameters);
        }
      }
    }


    void InternalReallocIndependentVariables(int noIndep)
    {
      INumericColumn [] oldArr = this._independentVariables;
      INumericColumn [] newArr = new INumericColumn[noIndep];
      for(int i=Math.Min(newArr.Length,oldArr.Length)-1;i>=0;i--)
        newArr[i] = oldArr[i];

      this._independentVariables = newArr;
    }

    void InternalReallocDependentVariables(int noDep)
    {
    {
      INumericColumn [] oldArr = this._dependentVariables;
      INumericColumn [] newArr = new INumericColumn[noDep];
      for(int i=Math.Min(newArr.Length,oldArr.Length)-1;i>=0;i--)
        newArr[i] = oldArr[i];
      this._dependentVariables = newArr;
    }
    {
      // do the same also with the error scaling

      ErrorEvaluation[] oldArr = _errorEvaluation;
      ErrorEvaluation[] newArr = new ErrorEvaluation[noDep];
      for(int i=Math.Min(newArr.Length,oldArr.Length)-1;i>=0;i--)
        newArr[i] = oldArr[i];
      this._errorEvaluation = newArr;
    }
    }

    void InternalReallocParameters(int noPar)
    {
      this._parameterNames = new string[noPar];
    }
    

    public int NumberOfIndependentVariables
    {
      get
      {
        return this._fitFunction!=null ? _fitFunction.NumberOfIndependentVariables : this._independentVariables.Length;
      }
    }

    public int NumberOfDependentVariables
    {
      get
      {
        return this._fitFunction!=null ? Math.Min(_fitFunction.NumberOfDependentVariables,_dependentVariables.Length) : this._dependentVariables.Length;
      }
    }

    public int NumberOfParameters
    {
      get
      {
        return this._fitFunction!=null ? _fitFunction.NumberOfParameters : 0;
      }
    }


    public void InitializeFittingSession()
    {
      _parameterValues = new double[this._parameterNames.Length];
      this._independentValues = new double[this._independentVariables.Length];
      this._dependentValuesResult = new double[this._dependentVariables.Length];

    
      // also obtain the valid rows both of the independent and of the dependent variables
      INumericColumn[] cols = new INumericColumn[_independentVariables.Length + _dependentVariables.Length];
      int i;
      AscendingIntegerCollection selectedCols = new AscendingIntegerCollection();
      // note: for a fitting session all independent variables columns must
      // be not null
      int maxLength = int.MaxValue;
      for(i=0;i<_independentVariables.Length;i++)
      {
        cols[i] = _independentVariables[i];
        selectedCols.Add(i);
        if(cols[i] is IDefinedCount)
          maxLength = Math.Min(maxLength,((IDefinedCount)cols[i]).Count);
      }
      
      // note: for a fitting session some of the dependent variables can be null
      for(int j=0;j<_dependentVariables.Length;++j,++i)
      {
        if(_dependentVariables[j]!=null)
        {
          cols[i] = _dependentVariables[j];
          selectedCols.Add(i);
          if(cols[i] is IDefinedCount)
            maxLength = Math.Min(maxLength,((IDefinedCount)cols[i]).Count);
        }
      }
      maxLength=Math.Min(maxLength,this._rangeOfRows.End);

      bool[] arr = Altaxo.Calc.LinearAlgebra.DataTableWrapper.GetValidNumericRows(cols,selectedCols,maxLength);
      _validNumericRows = Altaxo.Calc.LinearAlgebra.DataTableWrapper.GetCollectionOfValidNumericRows(arr);
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
      Evaluate(parameter,ys);
    }

   
    public void Evaluate(double[] parameter, double[] ys)
    {
      // zero the sum of errors
      for(int k=_dependentValuesResult.Length-1;k>=0;--k)
        ys[k]=0;

      // Evaluate the function for all points
      for(int i=_validNumericRows.Count-1;i>=0;i--)
      {
        for(int k=_independentVariables.Length-1;k>=0;k--)
          _independentValues[k] = _independentVariables[k].GetDoubleAt(_validNumericRows[i]);

        _fitFunction.Evaluate(_independentValues,_parameterValues,_dependentValuesResult);

        // now calculate the deviation between fit and original
        for(int k=_dependentValuesResult.Length-1;k>=0;--k)
        {
          ys[k] += _errorEvaluation[k](_dependentValuesResult[k],_dependentVariables[k].GetDoubleAt(_validNumericRows[i]));
        }
      }
    }
	}
}
