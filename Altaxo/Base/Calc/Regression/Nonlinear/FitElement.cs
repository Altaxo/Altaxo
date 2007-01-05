#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using Altaxo.Calc;
using Altaxo.Data;
using Altaxo.Collections;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Holds the fit function together with the data sources for the independent and
  /// dependent variables.
  /// </summary>
  public class FitElement : ICloneable
  {
   
    /// <summary>
    /// Fitting function. Can be null if no fitting function was actually chosen.
    /// </summary>
    IFitFunction _fitFunction; 
    PositiveIntegerRange _rangeOfRows;
    NumericColumnProxy[] _independentVariables;
    NumericColumnProxy[] _dependentVariables;
    IVarianceScaling[] _errorEvaluation;
    string [] _parameterNames = new string[0];
    string _parameterNameStart=string.Empty;

    public event EventHandler Changed;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitElement),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        FitElement s = (FitElement)obj;
        
        info.AddValue("FitFunction",s._fitFunction);
        info.AddValue("NumberOfRows",s._rangeOfRows.Count);
        info.AddValue("FirstRow",s._rangeOfRows.First);

        info.AddArray("IndependentVariables",s._independentVariables,s._independentVariables.Length);
        info.AddArray("DependentVariables",s._dependentVariables,s._dependentVariables.Length);
        info.AddArray("VarianceEvaluation",s._errorEvaluation,s._errorEvaluation.Length);
        info.AddArray("ParameterNames",s._parameterNames,s._parameterNames.Length);
        info.AddValue("ParameterNameStart",s._parameterNameStart);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        FitElement s = o!=null ? (FitElement)o : new FitElement();

        s.FitFunction = (IFitFunction)info.GetValue("FitFunction",s);

        int numRows = info.GetInt32("NumberOfRows");
        int firstRow = info.GetInt32("FirstRow");
        s._rangeOfRows = PositiveIntegerRange.NewFromFirstAndCount(firstRow,numRows);

        int arraycount = info.OpenArray();
        s._independentVariables = new NumericColumnProxy[arraycount];
        for(int i=0;i<arraycount;++i)
          s._independentVariables[i] = (NumericColumnProxy)info.GetValue(s);
        info.CloseArray(arraycount);

        arraycount = info.OpenArray();
        s._dependentVariables = new NumericColumnProxy[arraycount];
        for(int i=0;i<arraycount;++i)
          s._dependentVariables[i] = (NumericColumnProxy)info.GetValue(s);
        info.CloseArray(arraycount);


        arraycount = info.OpenArray();
        s._errorEvaluation = new IVarianceScaling[arraycount];
        for(int i=0;i<arraycount;++i)
          s._errorEvaluation[i] = (IVarianceScaling)info.GetValue(s);
        info.CloseArray(arraycount);

        info.GetArray("ParameterNames",out s._parameterNames);
        for(int i=0;i<s._parameterNames.Length;++i)
          if(s._parameterNames[i]==string.Empty)
            s._parameterNames[i]=null; // serialization can not distinguish between an empty string and a null string

        s._parameterNameStart = info.GetString("ParameterNameStart");
        return s;
      }
    }

    #endregion

    public FitElement()
    {
      _independentVariables = new NumericColumnProxy[0];
    
      _dependentVariables = new NumericColumnProxy[0];
     
      _errorEvaluation = new IVarianceScaling[0];
 
      _rangeOfRows = PositiveIntegerRange.NewFromFirstAndCount(0,int.MaxValue);
    }

    public FitElement(FitElement from)
    {
      this._fitFunction = from._fitFunction;
      if (_fitFunction is ICloneable)
        this._fitFunction = (IFitFunction)((ICloneable)from.FitFunction).Clone();

      _rangeOfRows = PositiveIntegerRange.NewFromFirstAndCount(from._rangeOfRows.First, from._rangeOfRows.Count);
      _independentVariables = new NumericColumnProxy[from._independentVariables.Length];
      for (int i = 0; i < _independentVariables.Length; ++i)
      {
        if(from._independentVariables[i]!=null)
          _independentVariables[i] = (NumericColumnProxy)from._independentVariables[i].Clone();
      }

      _dependentVariables = new NumericColumnProxy[from._dependentVariables.Length];
      for (int i = 0; i < _dependentVariables.Length; ++i)
      {
        if (from._dependentVariables[i] != null)
          _dependentVariables[i] = (NumericColumnProxy)from._dependentVariables[i].Clone();
      }
      _errorEvaluation = new IVarianceScaling[from._errorEvaluation.Length];
      for (int i = 0; i < _errorEvaluation.Length; ++i)
      {
        if (from._errorEvaluation[i] != null)
          _errorEvaluation[i] = (IVarianceScaling)from._errorEvaluation[i].Clone();
      }

      _parameterNames = (string[])from._parameterNames.Clone();
      _parameterNameStart = from._parameterNameStart;

    }

    public FitElement(INumericColumn xColumn, INumericColumn yColumn, int start, int count)
    {
      _independentVariables = new NumericColumnProxy[1];
      _independentVariables[0] = new NumericColumnProxy(xColumn);

      _dependentVariables = new NumericColumnProxy[1];
      _dependentVariables[0] = new NumericColumnProxy(yColumn);

      _errorEvaluation = new IVarianceScaling[1];
      _errorEvaluation[0] = new ConstantVarianceScaling();

      _rangeOfRows = PositiveIntegerRange.NewFromFirstAndCount(start,count);

    }

    /// <summary>
    /// Gives the ith parameter name.
    /// </summary>
    /// <param name="i">Index.</param>
    /// <returns>The ith parameter name.</returns>
    public string ParameterName(int i)
    {
      if(null!=_fitFunction)
      {
        if(null==_parameterNames[i] )
          return _parameterNameStart + _fitFunction.ParameterName(i);
        else
          return _parameterNames[i];
      }
      else
        return null;
    }

    /// <summary>
    /// Sets the ith parameter name.
    /// </summary>
    /// <param name="value">The new value of the parameter.</param>
    /// <param name="i">Index of the parameter.</param>
    public void SetParameterName(string value, int i)
    {

      if(value==null)
        throw new ArgumentNullException("value","Parameter name must not be null");
      if(value.Length==0)
        throw new ArgumentException("Parameter name is empty", "value");

      string oldValue = _parameterNames[i];
      _parameterNames[i] = value;

      if(value!=oldValue)
        this.OnChanged();
    }

    /// <summary>
    /// Sets the range of rows that are used for the regression.
    /// </summary>
    /// <param name="firstIndex">First row to be used.</param>
    /// <param name="count">Number of rows to be used [from firstIndex to (firstIndex+count-1)].</param>
    public void SetRowRange(int firstIndex, int count)
    {
      this._rangeOfRows = PositiveIntegerRange.NewFromFirstAndCount(firstIndex,count);
      OnChanged();
    }

    public void SetRowRange(PositiveIntegerRange range)
    {
      this._rangeOfRows.CopyFrom(range);
      OnChanged();
    }

    public PositiveIntegerRange GetRowRange()
    {
       return this._rangeOfRows;
    }

    /// <summary>
    /// Returns the ith independent variable column. Can return <c>null</c> if the column was set properly, but was 
    /// disposed in the mean time.
    /// </summary>
    /// <param name="i">Index.</param>
    /// <returns>The ith independent variable column, or <c>null if such a column is no longer available.</c></returns>
    public INumericColumn IndependentVariables(int i)
    {
      return null==this._independentVariables[i] ? null : this._independentVariables[i].Document;
    }

    /// <summary>
    /// Sets the ith independent variable column. The column is hold by a reference aware of disposed events, so that it can be null if retrieved afterwards.
    /// </summary>
    /// <param name="i">Index.</param>
    /// <param name="col">Independent variable column to set.</param>
    public void SetIndependentVariable(int i, INumericColumn col)
    {
      this._independentVariables[i] = new NumericColumnProxy(col);
      
      this.OnChanged();
    }

    /// <summary>
    /// Returns the ith dependent variable column. Can return <c>null</c> if the column was set properly, but was 
    /// disposed in the mean time.
    /// </summary>
    /// <param name="i">Index.</param>
    /// <returns>The ith dependent variable column, or <c>null if such a column is no longer available.</c></returns>
    public INumericColumn DependentVariables(int i)
    {
      return null==this._dependentVariables[i] ? null : this._dependentVariables[i].Document;
    }

    /// <summary>
    /// Sets the ith dependent variable column. The column is hold by a reference aware of disposed events, so that it can be null if retrieved afterwards.
    /// </summary>
    /// <param name="i">Index.</param>
    /// <param name="col">Dependent variable column to set.</param>
    public void SetDependentVariable(int i, INumericColumn col)
    {
      this._dependentVariables[i] = new NumericColumnProxy(col);
      
      if(col!=null)
      {
        if(this._errorEvaluation[i]==null)
          this._errorEvaluation[i] = new ConstantVarianceScaling();
      }
      else
      {
        this._errorEvaluation[i] = null;
      }
    }

    public IVarianceScaling ErrorEvaluation(int i)
    {
      return this._errorEvaluation[i];
    }

    public void SetErrorEvaluation(int i, IVarianceScaling val)
    {
      this._errorEvaluation[i] = val;
    }

    /// <summary>
    /// Returns true if the regression procedure has to include weights in the calculation.
    /// Else, if weights are not used for all used dependent variables (ConstantVarianceScaling with Scaling==1), <c>false</c> is returned.
    /// </summary>
    public bool UseWeights
    {
      get
      {
        if(_errorEvaluation==null || _dependentVariables==null)
          return false;

        for (int i = 0; i < _errorEvaluation.Length; ++i)
        {
          if (_dependentVariables[i] != null)
          {
            if (_errorEvaluation[i] != null)
            {
              if (!(_errorEvaluation[i] is ConstantVarianceScaling) || !((ConstantVarianceScaling)_errorEvaluation[i]).IsDefault)
                return true;
            }
          }
        }
        return false;
      }
    }

    /// <summary>
    /// Gets / sets the fitting function.
    /// </summary>
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

        OnChanged();

      }
    }

    

    void InternalReallocIndependentVariables(int noIndep)
    {
      NumericColumnProxy[] oldArr = this._independentVariables;
      NumericColumnProxy[] newArr = new NumericColumnProxy[noIndep];
      for(int i=Math.Min(newArr.Length,oldArr.Length)-1;i>=0;i--)
        newArr[i] = oldArr[i];

      this._independentVariables = newArr;
    }

    void InternalReallocDependentVariables(int noDep)
    {
    {
      NumericColumnProxy[] oldArr = this._dependentVariables;
      NumericColumnProxy[] newArr = new NumericColumnProxy[noDep];
      for(int i=Math.Min(newArr.Length,oldArr.Length)-1;i>=0;i--)
        newArr[i] = oldArr[i];
      this._dependentVariables = newArr;
    }
    {
      // do the same also with the error scaling

      IVarianceScaling[] oldArr = _errorEvaluation;
      IVarianceScaling[] newArr = new IVarianceScaling[noDep];
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

    protected virtual void OnChanged()
    {
      if (null != Changed)
        Changed(this, EventArgs.Empty);
    }

    
    public IAscendingIntegerCollection CalculateValidNumericRows()
    {
      // also obtain the valid rows both of the independent and of the dependent variables
      INumericColumn[] cols = new INumericColumn[_independentVariables.Length + _dependentVariables.Length];
      int i;
      AscendingIntegerCollection selectedCols = new AscendingIntegerCollection();
      // note: for a fitting session all independent variables columns must
      // be not null
      int maxLength = int.MaxValue;
      for (i = 0; i < _independentVariables.Length; i++)
      {
        cols[i] = _independentVariables[i].Document;
        selectedCols.Add(i);
        if (cols[i] is IDefinedCount)
          maxLength = Math.Min(maxLength, ((IDefinedCount)cols[i]).Count);
      }

      // note: for a fitting session some of the dependent variables can be null
      for (int j = 0; j < _dependentVariables.Length; ++j, ++i)
      {
        if (_dependentVariables[j] != null && _dependentVariables[j].Document != null)
        {
          cols[i] = _dependentVariables[j].Document;
          selectedCols.Add(i);
          if (cols[i] is IDefinedCount)
            maxLength = Math.Min(maxLength, ((IDefinedCount)cols[i]).Count);
        }
      }
      if (maxLength == int.MaxValue)
        maxLength = 0;

      maxLength = Math.Min(maxLength, this._rangeOfRows.End);

      bool[] arr = Altaxo.Calc.LinearAlgebra.DataTableWrapper.GetValidNumericRows(cols, selectedCols, maxLength);
      return Altaxo.Calc.LinearAlgebra.DataTableWrapper.GetCollectionOfValidNumericRows(arr);

    }


    #region ICloneable Members

    public object Clone()
    {
      return new FitElement(this);
    }

    #endregion
  }
}
