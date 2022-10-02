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
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Data.Selections;
using Altaxo.Data.Transformations;
using Altaxo.Graph.Plot.Data;
using Altaxo.Main;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Holds the fit function together with the data sources for the independent and
  /// dependent variables.
  /// </summary>
  public class FitElement
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    IColumnPlotData,
    ICloneable
  {
    /// <summary>Fitting function. Can be null if no fitting function is actually choosen.</summary>
    private IFitFunction? _fitFunction;

    /// <summary>Holds a reference to the underlying data table. </summary>
    protected DataTableProxy? _dataTable;

    /// <summary>The group number of the data columns. All data columns should have this group number. Data columns having other group numbers will be marked.</summary>
    protected int _groupNumber;

    /// <summary>Holds the range of rows of the data source that are used for the fitting procedure.</summary>
    private IRowSelection _rangeOfRows;

    /// <summary>Array of columns that are used as data source for the independent variables.</summary>
    private IReadableColumnProxy?[] _independentVariables;

    /// <summary>Array of columns that are used as data source for the dependent variables.</summary>
    private IReadableColumnProxy?[] _dependentVariables;

    /// <summary>Holds for each dependent variable output of the fit function the kind of transformation.
    /// If null, an identity transformation is assumed.</summary>
    private IDoubleToDoubleTransformation?[] _dependentVariableTransformations;

    /// <summary>Array of the current parameter names. The length of this array should be equal to that of <see cref="P:IFitFunction.NumberOfParameters"/>. If an element of this array is null, the parameter name
    /// of the fit function is used. Otherwise, this value overrides the original parameter name of the fit function.</summary>
    private string?[] _parameterNames = new string?[0];

    /// <summary></summary>
    private string _parameterNameStart = string.Empty;

    #region Serialization

    protected FitElement(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
    {
      switch (version)
      {
        case 0:
          DeserializeSurrogate0(info);
          break;
        case 1:
          DeserializeSurrogate1(info);
          break;
        case 2:
          DeserializeSurrogate2(info);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(version));
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.Nonlinear.FitElement", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is FitElement s)
          s.DeserializeSurrogate0(info);
        else
          s = new FitElement(info, 0);
        return s;
      }

      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version now allowed");
        /*
                FitElement s = (FitElement)obj;

                s.InternalCheckAndCorrectArraySize(true, false); // make sure the fit function has not changed unnoticed

                info.AddValue("FitFunction", s._fitFunction);
                info.AddValue("NumberOfRows", s._rangeOfRows.Count);
                info.AddValue("FirstRow", s._rangeOfRows.Start);

                info.AddArray("IndependentVariables", s._independentVariables, s._independentVariables.Length);
                info.AddArray("DependentVariables", s._dependentVariables, s._dependentVariables.Length);
                info.AddArray("VarianceEvaluation", s._errorEvaluation, s._errorEvaluation.Length);
                info.AddArray("ParameterNames", s._parameterNames, s._parameterNames.Length);
                info.AddValue("ParameterNameStart", s._parameterNameStart);
                */
      }
    }

    [MemberNotNull(nameof(_rangeOfRows), nameof(_independentVariables), nameof(_dependentVariables), nameof(_dependentVariableTransformations), nameof(_fitFunction))]
    private void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMemberAlt(ref _fitFunction, (IFitFunction?)info.GetValueOrNull("FitFunction", this) ?? new FitFunctions.General.Polynomial(1, 0));

      int numRows = info.GetInt32("NumberOfRows");
      int firstRow = info.GetInt32("FirstRow");
      ChildSetMemberAlt(ref _rangeOfRows, RangeOfRowIndices.FromStartAndCount(firstRow, numRows));

      int arraycount = info.OpenArray();
      _independentVariables = new IReadableColumnProxy[arraycount];
      for (int i = 0; i < arraycount; ++i)
      {
        ChildSetMember(ref _independentVariables[i], info.GetValueOrNull<IReadableColumnProxy>("e", this));
      }
      info.CloseArray(arraycount);

      arraycount = info.OpenArray();
      _dependentVariables = new IReadableColumnProxy[arraycount];
      for (int i = 0; i < arraycount; ++i)
      {
        ChildSetMember(ref _dependentVariables[i], info.GetValueOrNull<IReadableColumnProxy>("e", this));
      }
      info.CloseArray(arraycount);

      arraycount = info.OpenArray();
      _dependentVariableTransformations = new IDoubleToDoubleTransformation?[arraycount];
      for (int i = 0; i < arraycount; ++i)
        info.GetValueOrNull<IVarianceScaling>("e", this); // do not use this value
      info.CloseArray(arraycount);

      _parameterNames = info.GetArrayOfStrings("ParameterNames");
      for (int i = 0; i < _parameterNames.Length; ++i)
        if (_parameterNames[i] == string.Empty)
          _parameterNames[i] = null; // serialization can not distinguish between an empty string and a null string

      _parameterNameStart = info.GetString("ParameterNameStart");

      // now some afterwork
      if (InternalCheckAndCorrectArraySize(false, false))
        Current.Console.WriteLine("Error: Fitelement array size mismatch");
    }

    /// <summary>
    /// 2016-10-05 Added DataTable, GroupNumber. Changed: NunberOfRows, FirstRow now is replaced by RowSelection
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.Nonlinear.FitElement", 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FitElement)obj;

        s.InternalCheckAndCorrectArraySize(true, false); // make sure the fit function has not changed unnoticed

        info.AddValueOrNull("FitFunction", s._fitFunction);

        info.AddValueOrNull("DataTable", s._dataTable);
        info.AddValue("GroupNumber", s._groupNumber);
        info.AddValue("RowSelection", s._rangeOfRows);

        info.AddArrayOfNullableElements("IndependentVariables", s._independentVariables, s._independentVariables.Length);
        info.AddArrayOfNullableElements("DependentVariables", s._dependentVariables, s._dependentVariables.Length);
        info.AddArrayOfNullableElements("VarianceEvaluation", s._dependentVariableTransformations, s._dependentVariableTransformations.Length);
        info.AddArray("ParameterNames", s._parameterNames, s._parameterNames.Length);
        info.AddValue("ParameterNameStart", s._parameterNameStart);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is FitElement s)
          s.DeserializeSurrogate1(info);
        else
          s = new FitElement(info, 1);
        return s;
      }
    }

    [MemberNotNull(nameof(_rangeOfRows), nameof(_independentVariables), nameof(_dependentVariables), nameof(_dependentVariableTransformations), nameof(_fitFunction))]
    private void DeserializeSurrogate1(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMemberAlt<IFitFunction>(ref _fitFunction, (IFitFunction?)info.GetValueOrNull("FitFunction", this) ?? new FitFunctions.General.Polynomial(1, 0));

      ChildSetMember(ref _dataTable, (DataTableProxy?)info.GetValueOrNull("DataTable", this));

      _groupNumber = info.GetInt32("GroupNumber");

      ChildSetMember(ref _rangeOfRows, (IRowSelection)info.GetValue("RowSelection", this));

      int arraycount = info.OpenArray();
      _independentVariables = new IReadableColumnProxy[arraycount];
      for (int i = 0; i < arraycount; ++i)
      {
        ChildSetMember(ref _independentVariables[i], info.GetValueOrNull<IReadableColumnProxy>("e", this));
      }
      info.CloseArray(arraycount);

      arraycount = info.OpenArray();
      _dependentVariables = new IReadableColumnProxy[arraycount];
      for (int i = 0; i < arraycount; ++i)
      {
        ChildSetMember(ref _dependentVariables[i], info.GetValueOrNull<IReadableColumnProxy>("e", this));
      }
      info.CloseArray(arraycount);

      arraycount = info.OpenArray();
      _dependentVariableTransformations = new IDoubleToDoubleTransformation?[arraycount];
      for (int i = 0; i < arraycount; ++i)
        info.GetValueOrNull<IVarianceScaling>("e", this); // do not use this value
      info.CloseArray(arraycount);

      _parameterNames = info.GetArrayOfStrings("ParameterNames");
      for (int i = 0; i < _parameterNames.Length; ++i)
        if (_parameterNames[i] == string.Empty)
          _parameterNames[i] = null; // serialization can not distinguish between an empty string and a null string

      _parameterNameStart = info.GetString("ParameterNameStart");

      // now some afterwork
      if (InternalCheckAndCorrectArraySize(false, false))
        Current.Console.WriteLine("Error: Fitelement array size mismatch");
    }

    #region Version 2
    /// <summary>
    /// 2021-05-13 Deprecate errorEvalulation, instead use transformation of the dependent variable output of the fit function.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitElement), 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FitElement)obj;

        s.InternalCheckAndCorrectArraySize(true, false); // make sure the fit function has not changed unnoticed

        info.AddValueOrNull("FitFunction", s._fitFunction);

        info.AddValueOrNull("DataTable", s._dataTable);
        info.AddValue("GroupNumber", s._groupNumber);
        info.AddValue("RowSelection", s._rangeOfRows);

        info.AddArrayOfNullableElements("IndependentVariables", s._independentVariables, s._independentVariables.Length);
        info.AddArrayOfNullableElements("DependentVariables", s._dependentVariables, s._dependentVariables.Length);
        info.AddArrayOfNullableElements("DependentVariablesTransformations", s._dependentVariableTransformations, s._dependentVariableTransformations.Length);
        info.AddArray("ParameterNames", s._parameterNames, s._parameterNames.Length);
        info.AddValue("ParameterNameStart", s._parameterNameStart);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is FitElement s)
          s.DeserializeSurrogate2(info);
        else
          s = new FitElement(info, 2);
        return s;
      }
    }

    [MemberNotNull(nameof(_rangeOfRows), nameof(_independentVariables), nameof(_dependentVariables), nameof(_dependentVariableTransformations), nameof(_fitFunction))]
    private void DeserializeSurrogate2(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMemberAlt<IFitFunction>(ref _fitFunction, (IFitFunction?)info.GetValueOrNull("FitFunction", this) ?? new FitFunctions.General.Polynomial(1, 0));

      ChildSetMember(ref _dataTable, (DataTableProxy?)info.GetValueOrNull("DataTable", this));

      _groupNumber = info.GetInt32("GroupNumber");

      ChildSetMember(ref _rangeOfRows, (IRowSelection)info.GetValue("RowSelection", this));

      int arraycount = info.OpenArray();
      _independentVariables = new IReadableColumnProxy[arraycount];
      for (int i = 0; i < arraycount; ++i)
      {
        ChildSetMember(ref _independentVariables[i], info.GetValueOrNull<IReadableColumnProxy>("e", this));
      }
      info.CloseArray(arraycount);

      arraycount = info.OpenArray();
      _dependentVariables = new IReadableColumnProxy[arraycount];
      for (int i = 0; i < arraycount; ++i)
      {
        ChildSetMember(ref _dependentVariables[i], info.GetValueOrNull<IReadableColumnProxy>("e", this));
      }
      info.CloseArray(arraycount);

      arraycount = info.OpenArray();
      _dependentVariableTransformations = new IDoubleToDoubleTransformation?[arraycount];
      for (int i = 0; i < arraycount; ++i)
        _dependentVariableTransformations[i] = info.GetValueOrNull<IDoubleToDoubleTransformation>("e", this);
      info.CloseArray(arraycount);

      _parameterNames = info.GetArrayOfStrings("ParameterNames");
      for (int i = 0; i < _parameterNames.Length; ++i)
        if (_parameterNames[i] == string.Empty)
          _parameterNames[i] = null; // serialization can not distinguish between an empty string and a null string

      _parameterNameStart = info.GetString("ParameterNameStart");

      // now some afterwork
      if (InternalCheckAndCorrectArraySize(false, false))
        Current.Console.WriteLine("Error: Fitelement array size mismatch");
    }



    #endregion

    #endregion Serialization

    public FitElement(FitElement from)
    {
      if (from._fitFunction is ICloneable fromFitFunc1)
      {
        _fitFunction = (IFitFunction)fromFitFunc1.Clone();
        if (_fitFunction is Main.IDocumentLeafNode thisFitFunc1)
          thisFitFunc1.ParentObject = this;
      }
      else
      {
        _fitFunction = from._fitFunction;
      }

      if (_fitFunction is not null)
        _fitFunction.Changed += EhFitFunctionChanged;

      ChildCopyToMember<DataTableProxy>(ref _dataTable, from._dataTable);
      _groupNumber = from._groupNumber;
      ChildCloneToMember(ref _rangeOfRows, from._rangeOfRows);

      _independentVariables = new IReadableColumnProxy[from._independentVariables.Length];
      for (int i = 0; i < _independentVariables.Length; ++i)
      {
        ChildCloneToMember(ref _independentVariables[i], from._independentVariables[i]);
      }

      _dependentVariables = new IReadableColumnProxy[from._dependentVariables.Length];
      for (int i = 0; i < _dependentVariables.Length; ++i)
      {
        ChildCloneToMember(ref _dependentVariables[i], from._dependentVariables[i]);
      }

      _dependentVariableTransformations = (IDoubleToDoubleTransformation?[])from._dependentVariableTransformations.Clone();

      _parameterNames = (string[])from._parameterNames.Clone();
      _parameterNameStart = from._parameterNameStart;
    }

    public FitElement(IFitFunction fitFunction) : this(fitFunction, new AllRows())
    {
    }

    public FitElement(IFitFunction fitFunction, IRowSelection? rowSelection)
    {
      if (fitFunction is null)
        throw new ArgumentNullException(nameof(fitFunction));

      ChildSetMemberAlt(ref _fitFunction, fitFunction);

      _independentVariables = new IReadableColumnProxy[fitFunction.NumberOfIndependentVariables];

      _dependentVariables = new IReadableColumnProxy[fitFunction.NumberOfDependentVariables];

      _dependentVariableTransformations = new IDoubleToDoubleTransformation?[fitFunction.NumberOfDependentVariables];

      _parameterNames = new string[fitFunction.NumberOfParameters];

      _rangeOfRows = rowSelection is null ? new AllRows() : (IRowSelection)(rowSelection.Clone());
    }

    public FitElement(IFitFunction fitFunction, DataTable table, int groupNumber, IRowSelection rowSelection, IReadableColumn xColumn, IReadableColumn yColumn)
    {
      if (fitFunction is null)
        throw new ArgumentNullException(nameof(fitFunction));

      ChildSetMemberAlt(ref _fitFunction, fitFunction);

      if (rowSelection is null)
        throw new ArgumentNullException(nameof(rowSelection));

      ChildSetMember<DataTableProxy>(ref _dataTable, new DataTableProxy(table));
      _groupNumber = groupNumber;
      ChildCloneToMember(ref _rangeOfRows, rowSelection);

      _independentVariables = new IReadableColumnProxy[_fitFunction.NumberOfIndependentVariables];
      ChildSetMember(ref _independentVariables[0], ReadableColumnProxyBase.FromColumn(xColumn));

      _dependentVariables = new IReadableColumnProxy[_fitFunction.NumberOfDependentVariables];
      ChildSetMember(ref _dependentVariables[0], ReadableColumnProxyBase.FromColumn(yColumn));

      _dependentVariableTransformations = new IDoubleToDoubleTransformation[_fitFunction.NumberOfDependentVariables];

      _parameterNames = new string[_fitFunction.NumberOfParameters];
    }

    public FitElement(DataTable table, int groupNumber, IRowSelection rowSelection, IReadableColumn xColumn, IReadableColumn yColumn)
    {
      if (rowSelection is null)
        throw new ArgumentNullException(nameof(rowSelection));

      ChildSetMember<DataTableProxy>(ref _dataTable, new DataTableProxy(table));
      _groupNumber = groupNumber;
      ChildCloneToMember(ref _rangeOfRows, rowSelection);

      _independentVariables = new IReadableColumnProxy[1];
      ChildSetMember(ref _independentVariables[0], ReadableColumnProxyBase.FromColumn(xColumn));

      _dependentVariables = new IReadableColumnProxy[1];
      ChildSetMember(ref _dependentVariables[0], ReadableColumnProxyBase.FromColumn(yColumn));

      _dependentVariableTransformations = new IDoubleToDoubleTransformation[1];

      _parameterNames = new string[0];
    }

    /// <summary>
    /// Gives the ith parameter name.
    /// </summary>
    /// <param name="i">Index of the parameter whose name is to be retrieved.</param>
    /// <returns>The ith parameter name. Returns null if the <see cref="_fitFunction"/> is <c>null</c>.
    /// Otherwise, if <see cref="_parameterNames"/>[i] is not null, returns this value.
    /// If <see cref="_parameterNames"/>[i] is null then the value of <see cref="_parameterNameStart"/> is concenated with the original parameter name of the fit function and that value is returned.
    /// </returns>
    public string ParameterName(int i)
    {
      if (_parameterNames[i] is { } parameterName)
        return parameterName;
      else
        return _parameterNameStart + (_fitFunction?.ParameterName(i) ?? string.Empty);
    }

    /// <summary>
    /// Sets the ith parameter name.
    /// </summary>
    /// <param name="value">The new value of the parameter.</param>
    /// <param name="i">Index of the parameter.</param>
    public void SetParameterName(string value, int i)
    {
      if (string.IsNullOrEmpty(value))
        throw new ArgumentNullException(nameof(value), "Parameter name must not be null or empty");

      if (!(_parameterNames[i] == value))
      {
        _parameterNames[i] = value;
        EhSelfChanged(EventArgs.Empty);
      }
    }

    [MaybeNull]
    public DataTable DataTable
    {
      get
      {
        return _dataTable?.Document;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        if (object.ReferenceEquals(DataTable, value))
          return;

        if (ChildSetMember<DataTableProxy>(ref _dataTable, new DataTableProxy(value)))
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public int GroupNumber
    {
      get
      {
        return _groupNumber;
      }
      set
      {
        if (!(_groupNumber == value))
        {
          _groupNumber = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// The selection of data rows to be plotted.
    /// </summary>
    public IRowSelection DataRowSelection
    {
      get
      {
        return _rangeOfRows;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        if (!_rangeOfRows.Equals(value))
        {
          ChildSetMember(ref _rangeOfRows, value);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
    /// </summary>
    /// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
    /// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
    public IEnumerable<GroupOfColumnsInformation> GetAdditionallyUsedColumns()
    {
      yield return new GroupOfColumnsInformation("Independent variables", GetIndependentVariables());
      yield return new GroupOfColumnsInformation("Dependent variables", GetDependentVariables());
    }

    private IEnumerable<ColumnInformation> GetIndependentVariables()
    {
      for (int i = 0; i < NumberOfIndependentVariables; ++i)
      {
        int k = i;

        string nameOfVariable = FitFunction is not null && i < FitFunction.NumberOfIndependentVariables ? FitFunction.IndependentVariableName(i) : string.Empty;
        yield return new ColumnInformation(
          nameOfVariable,
          _independentVariables[k]?.Document(),
          _independentVariables[k]?.DocumentPath()?.LastPartOrDefault,
          (col, table, group) =>
          {
            if (table is not null)
            {
              DataTable = table;
              GroupNumber = group;
              SetIndependentVariable(k, col);
            }
          }
        );
      }
    }

    private IEnumerable<ColumnInformation>
      GetDependentVariables()
    {
      for (int i = 0; i < NumberOfDependentVariables; ++i)
      {
        int k = i;

        string nameOfVariable = FitFunction is not null && k < FitFunction.NumberOfDependentVariables ? FitFunction.DependentVariableName(k) : string.Empty;
        yield return new ColumnInformation(
          nameOfVariable,
          _dependentVariables[k]?.Document(),
          _dependentVariables[k]?.DocumentPath()?.LastPartOrDefault,
          (col, table, group) =>
          {
            if (table is not null)
            {
              DataTable = table;
              GroupNumber = group;
              SetDependentVariable(k, col);
            }
          }
        );
      }
    }

    /// <summary>
    /// Gets the maximum row index that can be deduced from the data columns. The calculation does <b>not</b> include the DataRowSelection.
    /// </summary>
    /// <returns>The maximum row index that can be deduced from the data columns.</returns>
    public int GetMaximumRowIndexExclusiveFromDataColumns()
    {
      int maxRowIndex = int.MaxValue;

      foreach (var proxy in _independentVariables.Concat(_dependentVariables))
      {
        var column = proxy?.Document();

        if (column is not null && column.Count.HasValue)
          maxRowIndex = Math.Min(maxRowIndex, column.Count.Value);
      }
      // if both columns are indefinite long, we set the length to zero
      if (maxRowIndex == int.MaxValue || maxRowIndex < 0)
        maxRowIndex = 0;

      return maxRowIndex;
    }

    /// <summary>
    /// Returns the ith independent variable column. Can return <c>null</c> if the column was set properly, but was
    /// disposed in the mean time.
    /// </summary>
    /// <param name="i">Index.</param>
    /// <returns>The ith independent variable column, or <c>null if such a column is no longer available.</c></returns>
    public IReadableColumn? IndependentVariables(int i)
    {
      return _independentVariables[i]?.Document();
    }

    /// <summary>
    /// Sets the ith independent variable column. The column is hold by a reference aware of disposed events, so that it can be null if retrieved afterwards.
    /// </summary>
    /// <param name="i">Index.</param>
    /// <param name="col">Independent variable column to set.</param>
    public void SetIndependentVariable(int i, IReadableColumn? col)
    {
      if (!object.ReferenceEquals(_independentVariables[i]?.Document(), col))
      {
        ChildSetMember(ref _independentVariables[i], ReadableColumnProxyBase.FromColumn(col));
        EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Returns the ith dependent variable column. Can return <c>null</c> if the column was set properly, but was
    /// disposed in the mean time.
    /// </summary>
    /// <param name="i">Index.</param>
    /// <returns>The ith dependent variable column, or <c>null if such a column is no longer available.</c></returns>
    public IReadableColumn? DependentVariables(int i)
    {
      return _dependentVariables[i]?.Document();
    }

    /// <summary>
    /// Sets the ith dependent variable column. The column is hold by a reference aware of disposed events, so that it can be null if retrieved afterwards.
    /// </summary>
    /// <param name="i">Index.</param>
    /// <param name="col">Dependent variable column to set.</param>
    public void SetDependentVariable(int i, IReadableColumn? col)
    {
      if (!object.ReferenceEquals(_dependentVariables[i]?.Document(), col))
      {
        ChildSetMember(ref _dependentVariables[i], ReadableColumnProxyBase.FromColumn(col));

        if (col is null)
        {
          _dependentVariableTransformations[i] = null;
        }
      }
    }

    /// <summary>
    /// Gets the transformation for the ith dependent variable output of the fit function.
    /// </summary>
    /// <param name="i">The index of the dependent variable.</param>
    /// <returns>The transformation for the ith dependent variable output of the fit function.</returns>
    public IDoubleToDoubleTransformation? GetDependentVariableTransformation(int i)
    {
      return _dependentVariableTransformations[i];
    }

    /// <summary>
    /// Sets the transformation for the ith dependent variable output of the fit function.
    /// </summary>
    /// <param name="i">The index of the dependent variable.</param>
    /// <param name="val">Transformation for the ith dependent variable output of the fit function.</param>
    public void SetDependentVariableTransformation(int i, IDoubleToDoubleTransformation? val)
    {
      _dependentVariableTransformations[i] = val;
    }

    /// <summary>
    /// Gets the dependent variable name, and if a transformation is applied, the representation of this transformation as a function.
    /// </summary>
    /// <param name="i">The index of the dependent variable.</param>
    /// <returns>The dependent variable name, and if a transformation is applied, the representation of this transformation as a function.</returns>
    public string GetDependentVariableNameWithTransformation(int i)
    {
      var result = _fitFunction?.DependentVariableName(i);
      if (string.IsNullOrEmpty(result))
        return string.Empty;

      if (_dependentVariableTransformations[i] is { } transformation)
      {
        result = transformation.GetRepresentationAsFunction(result);
      }
      return result;
    }

    /// <summary>
    /// Returns true if the regression procedure has to include weights in the calculation.
    /// Otherwise, if weights are not used for all used dependent variables (ConstantVarianceScaling with Scaling==1), <c>false</c> is returned.
    /// </summary>
    public bool UseWeights
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets / sets the fitting function.
    /// </summary>
    public IFitFunction? FitFunction
    {
      get
      {
        return _fitFunction;
      }
      set
      {
        if (!object.ReferenceEquals(_fitFunction, value))
        {
          if (_fitFunction is not null)
          {
            _fitFunction.Changed -= EhFitFunctionChanged;
          }

          if (value is ICloneable fromFitFunc1)
          {
            _fitFunction = (IFitFunction)fromFitFunc1.Clone();
            if (_fitFunction is Main.IDocumentLeafNode thisFitFunc1)
              thisFitFunc1.ParentObject = this;
          }
          else
          {
            _fitFunction = value;
          }

          if (_fitFunction is not null)
          {
            _fitFunction.Changed += EhFitFunctionChanged;
          }

          InternalCheckAndCorrectArraySize(false, false);

          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Evaluates the fit function values and transforms them according to the transformations set.
    /// </summary>
    /// <param name="independent">The independent.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="FV">The fv.</param>
    /// <returns></returns>
    public void FitFunctionEvaluate(double[] independent, double[] parameters, double[] FV)
    {
      FitFunction!.Evaluate(independent, parameters, FV);
      for (int i = 0; i < _dependentVariableTransformations.Length; ++i)
      {
        if (_dependentVariableTransformations[i] is { } t)
          FV[i] = t.Transform(FV[i]);
      }
    }

    public void FitFunctionEvaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? dependentVariableChoice, IVector<double> FV)
    {
      FitFunction!.Evaluate(independent, parameters, dependentVariableChoice, FV);
      int numberOfDependentVariablesInUse = dependentVariableChoice is null ? FitFunction!.NumberOfDependentVariables : dependentVariableChoice.Count(x => x);
      int idxDependentVariable = -1;
      for (int i = 0; i < _dependentVariableTransformations.Length; ++i)
      {
        if (dependentVariableChoice is null || dependentVariableChoice[i] == true)
        {
          ++idxDependentVariable;
          if (_dependentVariableTransformations[i] is { } t)
          {
            for (int j = 0, k = idxDependentVariable; j < independent.RowCount; ++j, k += numberOfDependentVariablesInUse)
            {
              FV[k] = t.Transform(FV[k]);
            }
          }
        }
      }
    }

    private void EhFitFunctionChanged(object? sender, EventArgs e)
    {
      InternalCheckAndCorrectArraySize(false, true);
    }

    /// <summary>
    /// Checks the size of the arrays (independent variables, dependent variables, parameters) against the corresponding values of the fit function and throws an InvalidOperationException when a mismatch is detected.
    /// The most probably cause of this is when the fit function has changed the number of parameters (or dependent or independent) variables unnoticed by this FitElement.
    /// </summary>
    /// <param name="throwOnMismatch">If <c>true</c>, an InvalidOperationException is thrown if the corresponding number from the fit function and the array size mismatch.</param>
    /// <param name="forceChangedEvent">If <c>true</c>, the <see cref="E:Changed"/> event is fired even if no mismatch was detected.</param>
    /// <returns><c>True</c> if any mismatch occurred, so that the array size has changed. Otherwise, <c>False</c> is returned.</returns>
    private bool InternalCheckAndCorrectArraySize(bool throwOnMismatch, bool forceChangedEvent)
    {
      if (_fitFunction is null)
        return false;

      bool hasMismatch = false;

      if (_fitFunction.NumberOfIndependentVariables != _independentVariables.Length)
      {
        hasMismatch = true;
        if (throwOnMismatch)
          throw new InvalidOperationException("Mismatch between number of independent variables of the fit function and of the array. Probably the fit function was changed after assigning them to the fit element, and dit not fire the Changed event");
        else
          InternalReallocIndependentVariables(_fitFunction.NumberOfIndependentVariables);
      }
      if (_fitFunction.NumberOfDependentVariables != _dependentVariables.Length)
      {
        hasMismatch = true;
        if (throwOnMismatch)
          throw new InvalidOperationException("Mismatch between number of dependent variables of the fit function and of the array. Probably the fit function was changed after assigning them to the fit element, and dit not fire the Changed event");
        else
          InternalReallocDependentVariables(_fitFunction.NumberOfDependentVariables);
      }
      if (_fitFunction.NumberOfParameters != _parameterNames.Length)
      {
        hasMismatch = true;
        if (throwOnMismatch)
          throw new InvalidOperationException("Mismatch between number of parameters of the fit function and of the array. Probably the fit function was changed after assigning them to the fit element, and dit not fire the Changed event");
        else
          InternalReallocParameters(_fitFunction.NumberOfParameters);
      }

      if (hasMismatch | forceChangedEvent)
        EhSelfChanged(EventArgs.Empty);

      return hasMismatch;
    }

    private void InternalReallocIndependentVariables(int noIndep)
    {
      var oldArr = _independentVariables;
      var newArr = new IReadableColumnProxy?[noIndep];
      for (int i = Math.Min(newArr.Length, oldArr.Length) - 1; i >= 0; i--)
        newArr[i] = oldArr[i];

      _independentVariables = newArr;
    }

    private void InternalReallocDependentVariables(int noDep)
    {
      {
        var oldArr = _dependentVariables;
        var newArr = new IReadableColumnProxy?[noDep];
        for (int i = Math.Min(newArr.Length, oldArr.Length) - 1; i >= 0; i--)
          newArr[i] = oldArr[i];
        _dependentVariables = newArr;
      }
      {
        // do the same also with the error scaling

        var oldArr = _dependentVariableTransformations;
        var newArr = new IDoubleToDoubleTransformation?[noDep];
        for (int i = Math.Min(newArr.Length, oldArr.Length) - 1; i >= 0; i--)
          newArr[i] = oldArr[i];
        _dependentVariableTransformations = newArr;
      }
    }

    private void InternalReallocParameters(int noPar)
    {
      _parameterNames = new string[noPar];
    }

    /// <summary>
    /// Gets the number of independent variables of this fit element.
    /// </summary>
    public int NumberOfIndependentVariables
    {
      get
      {
        return _fitFunction?.NumberOfIndependentVariables ?? 0;
      }
    }

    /// <summary>
    /// Gets the maximum possible number of dependent variables of this fit element. Please note that the actual used number of dependent variables can be less than this (see <see cref="NumberOfUsedDependentVariables"/>).
    /// </summary>
    public int NumberOfDependentVariables
    {
      get
      {
        return _fitFunction?.NumberOfDependentVariables ?? 0;
      }
    }

    /// <summary>
    /// Returns the number of dependent variables that are currently used for fitting.
    /// </summary>
    public int NumberOfUsedDependentVariables
    {
      get
      {
        int sum = 0;
        if (_dependentVariables is not null)
        {
          int len = _dependentVariables.Length;
          if (_fitFunction is not null)
            len = Math.Min(len, _fitFunction.NumberOfDependentVariables);

          for (int i = len - 1; i >= 0; --i)
            if (_dependentVariables[i]?.Document() is not null)
              sum++;
        }
        return sum;
      }
    }

    /// <summary>
    /// Gets the number of parameters.
    /// </summary>
    public int NumberOfParameters
    {
      get
      {
        return _fitFunction?.NumberOfParameters ?? 0;
      }
    }

    /// <summary>
    /// Calculates the valid numeric rows of the data source, i.e. that rows that can be used for fitting. Both dependent and independent variable sources are considered. A row is considered valid, if
    /// all (independent and dependent) variables of this row have finite numeric values.
    /// </summary>
    /// <returns>The set of rows that can be used for fitting.</returns>
    public IAscendingIntegerCollection CalculateValidNumericRows()
    {
      // also obtain the valid rows both of the independent and of the dependent variables
      var cols = new IReadableColumn?[_independentVariables.Length + _dependentVariables.Length];
      int i;
      var selectedCols = new AscendingIntegerCollection();
      // note: for a fitting session all independent variables columns must
      // be not null
      int maxLength = int.MaxValue;
      for (i = 0; i < _independentVariables.Length; i++)
      {
        cols[i] = _independentVariables[i]?.Document();
        selectedCols.Add(i);
        if (cols[i]?.Count is int cnt)
          maxLength = Math.Min(maxLength, cnt);
      }

      // note: for a fitting session some of the dependent variables can be null
      for (int j = 0; j < _dependentVariables.Length; ++j, ++i)
      {
        if (_dependentVariables[j]?.Document() is { } dvj)
        {
          cols[i] = dvj;
          selectedCols.Add(i);
          if (cols[i]?.Count is int cnt)
            maxLength = Math.Min(maxLength, cnt);
        }
      }
      if (maxLength == int.MaxValue)
        maxLength = 0; // in case non of the columns has a defined length

      bool[] arr = new bool[0];
      if (_dataTable?.Document?.DataColumns is { } validDataColumnCollection)
        arr = Altaxo.Calc.LinearAlgebra.DataTableWrapper.GetValidNumericRows(cols, selectedCols, _rangeOfRows.GetSelectedRowIndicesFromTo(0, maxLength, validDataColumnCollection, maxLength), maxLength);

      return Altaxo.Calc.LinearAlgebra.DataTableWrapper.GetCollectionOfValidNumericRows(arr);
    }

    #region ICloneable Members

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public object Clone()
    {
      return new FitElement(this);
    }

    #endregion ICloneable Members

    #region Document node functions

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_dataTable is not null)
        yield return new Main.DocumentNodeAndName(_dataTable, () => _dataTable = null, nameof(DataTable));

      if (_rangeOfRows is not null)
        yield return new Main.DocumentNodeAndName(_rangeOfRows, () => _rangeOfRows = null!, nameof(DataRowSelection));

      if (_independentVariables is not null)
      {
        for (int i = 0; i < _independentVariables.Length; ++i)
        {
          if (_independentVariables[i] is { } ivi)
            yield return new Main.DocumentNodeAndName(ivi, "IndependentVariable" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
      }

      if (_dependentVariables is not null)
      {
        for (int i = 0; i < _dependentVariables.Length; ++i)
        {
          if (_dependentVariables[i] is { } dvi)
            yield return new Main.DocumentNodeAndName(dvi, "DependentVariable" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
      }

      if (_fitFunction is Main.IDocumentLeafNode)
        yield return new Main.DocumentNodeAndName((Main.IDocumentLeafNode)_fitFunction, () => _fitFunction = null!, "FitFunction");
    }

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public virtual void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      if (_dataTable is { } _)
        Report(_dataTable, this, nameof(DataTable));

      {
        if (_independentVariables is { } v)
        {
          for (int i = 0; i < v.Length; ++i)
          {
            if (v[i] is { } vv)
              Report(vv, this, FormattableString.Invariant($"IndependentVariable{i}"));
          }
        }
      }
      {
        if (_dependentVariables is { } v)
        {
          for (int i = 0; i < v.Length; ++i)
          {
            if (v[i] is { } vv)
              Report(vv, this, FormattableString.Invariant($"DependentVariable{i}"));
          }
        }
      }

    }

    #endregion Document node functions
  }
}
