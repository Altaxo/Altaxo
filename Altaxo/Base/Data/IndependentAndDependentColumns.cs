#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Altaxo.Data.Selections;
using Altaxo.Graph.Plot.Data;
using Altaxo.Main;

namespace Altaxo.Data
{
  /// <summary>
  /// Stores multiple columns for independent variables, multiple columns for dependent variables, and a selection of rows.
  /// </summary>
  /// <seealso cref="Altaxo.Main.SuspendableDocumentNodeWithSetOfEventArgs" />
  /// <seealso cref="Altaxo.Graph.Plot.Data.IColumnPlotData" />
  /// <seealso cref="System.ICloneable" />
  /// <seealso cref="Altaxo.Main.IHasDocumentReferences" />
  public class IndependentAndDependentColumns
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    IColumnPlotData,
    ICloneable,
    IHasDocumentReferences
  {
    /// <summary>Holds a reference to the underlying data table. If the Empty property of the proxy is null, the underlying table must be determined from the column proxies.</summary>
    protected DataTableProxy? _dataTable;

    /// <summary>The group number of the data columns. All data columns should have this group number. Data columns having other group numbers will be marked.</summary>
    protected int _groupNumber;

    /// <summary>Array of columns that are used as data source for the independent variables.</summary>
    protected IReadableColumnProxy?[] _independentVariables = Array.Empty<IReadableColumnProxy>();

    /// <summary>Array of columns that are used as data source for the dependent variables.</summary>
    protected IReadableColumnProxy?[] _dependentVariables = Array.Empty<IReadableColumnProxy>();

    /// <summary>
    /// The selection of data rows to be plotted.
    /// </summary>
    protected IRowSelection _rangeOfRows;

    #region Serialization

    protected IndependentAndDependentColumns(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
    {
      switch (version)
      {
        case 0:
          DeserializeSurrogate0(info);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(version));
      }
    }

    /// <summary>
    /// 2023-05-11 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(IndependentAndDependentColumns), 0)]
    protected class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (IndependentAndDependentColumns)obj;

        info.AddValueOrNull("DataTable", s._dataTable);
        info.AddValue("GroupNumber", s._groupNumber);
        info.AddValue("RowSelection", s._rangeOfRows);

        info.AddArrayOfNullableElements("IndependentVariables", s._independentVariables, s._independentVariables.Length);
        info.AddArrayOfNullableElements("DependentVariables", s._dependentVariables, s._dependentVariables.Length);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is IndependentAndDependentColumns s)
          s.DeserializeSurrogate0(info);
        else
          s = new IndependentAndDependentColumns(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_rangeOfRows), nameof(_independentVariables), nameof(_dependentVariables))]
    protected void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
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
    }


    #endregion

    public IndependentAndDependentColumns(DataTable table, int groupNumber, int numberOfIndependentColumns, int numberOfDependentColumns)
    {
      _rangeOfRows = new AllRows() { ParentObject = this };
      DataTable = table;
      GroupNumber = groupNumber;
      _independentVariables = new IReadableColumnProxy[numberOfIndependentColumns];
      _dependentVariables = new IReadableColumnProxy[numberOfDependentColumns];
    }

    public IndependentAndDependentColumns(IndependentAndDependentColumns from)
    {
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

      /*
      _dependentVariableTransformations = (IDoubleToDoubleTransformation?[])from._dependentVariableTransformations.Clone();

      _parameterNames = (string[])from._parameterNames.Clone();
      _parameterNameStart = from._parameterNameStart;
      */
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public virtual object Clone()
    {
      return new IndependentAndDependentColumns(this);
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

    protected virtual string GetIndependentVariableName(int idx)
    {
      return idx == 0 ? "X" : $"X{idx}";
    }

    private IEnumerable<ColumnInformation> GetIndependentVariables()
    {
      for (int i = 0; i < _independentVariables.Length; ++i)
      {
        int k = i;

        string nameOfVariable = GetIndependentVariableName(i);
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

    public IReadableColumn? GetIndependentVariable(int i)
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



    protected virtual string GetDependentVariableName(int idx)
    {
      return idx == 0 ? "Y" : $"Y{idx}";
    }

    private IEnumerable<ColumnInformation>
     GetDependentVariables()
    {
      for (int i = 0; i < _dependentVariables.Length; ++i)
      {
        int k = i;

        string nameOfVariable = GetDependentVariableName(i);
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

    public IReadableColumn? GetDependentVariable(int i)
    {
      return _dependentVariables[i]?.Document();
    }

    /// <summary>
    /// Sets the ith dependent variable column. The column is hold by a reference aware of disposed events, so that it can be null if retrieved afterwards.
    /// </summary>
    /// <param name="i">Index.</param>
    /// <param name="col">Dependent variable column to set.</param>
    public virtual void SetDependentVariable(int i, IReadableColumn? col)
    {
      if (!object.ReferenceEquals(_dependentVariables[i]?.Document(), col))
      {
        ChildSetMember(ref _dependentVariables[i], ReadableColumnProxyBase.FromColumn(col));
        EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Gets the maximum row index that can be deduced from the data columns.
    /// The calculation does <b>not</b> include the DataRowSelection.
    /// </summary>
    /// <returns>The maximum row index that can be deduced from the data columns.</returns>
    public virtual int GetCommonRowCountFromDataColumns()
    {
      int? maxRowIndex = null;
      foreach (var colProxy in _independentVariables)
      {
        var c = colProxy?.Document();
        if (c?.Count is int rowCount)
        {
          if (maxRowIndex.HasValue)
            maxRowIndex = Math.Min(maxRowIndex.Value, rowCount);
          else
            maxRowIndex = rowCount;
        }
      }
      return maxRowIndex ?? 0;
    }

    public virtual (double[]?[] Independent, double[]?[] Dependent, int RowCount) GetResolvedData()
    {
      var resIndependent = new double[_independentVariables.Length][];
      var resDependent = new double[_dependentVariables.Length][];
      int rowCount = 0;

      var dataTable = _dataTable.Document;

      if (dataTable is null)
        return (resIndependent, resDependent, rowCount);

      var maxRowCount = GetCommonRowCountFromDataColumns();

      var segments = _rangeOfRows.GetSelectedRowIndexSegmentsFromTo(0, maxRowCount, dataTable.DataColumns, maxRowCount).ToArray();
      foreach (var segment in segments)
      {
        rowCount += segment.endExclusive - segment.start;
      }

      var independentColumns = _independentVariables.Select(x => x?.Document()).ToArray();
      var dependentColumns = _dependentVariables.Select(x => x?.Document()).ToArray();

      resIndependent = independentColumns.Select(x => x is null ? null : new double[rowCount]).ToArray();
      resDependent = dependentColumns.Select(x => x is null ? null : new double[rowCount]).ToArray();

      int rowIndex = 0;
      foreach (var segment in segments)
      {
        for (int i = segment.start; i < segment.endExclusive; i++, ++rowIndex)
        {
          for (int k = 0; k < independentColumns.Length; k++)
          {
            if (independentColumns[k] is not null)
            {
              resIndependent[k][rowIndex] = independentColumns[k][i];
            }
          }
          for (int k = 0; k < dependentColumns.Length; k++)
          {
            if (dependentColumns[k] is not null)
            {
              resDependent[k][rowIndex] = dependentColumns[k][i];
            }
          }
        }
      }

      return (resIndependent, resDependent, rowCount);
    }

    public static DataColumn? GetRootDataColumn(IReadableColumn? column)
    {
      while (column is TransformedReadableColumn trc)
      {
        column = trc.UnderlyingReadableColumn;
      }
      return column as DataColumn;
    }

    public static AltaxoVariant GetPropertyValueOfColumn(DataColumn ycol, string propertyName)
    {
      if (ycol is null || string.IsNullOrEmpty(propertyName))
        return new AltaxoVariant();

      DataTable? table = DataTable.GetParentDataTableOf(ycol);
      if (table is not null)
      {
        if (table.PropCols.TryGetColumn(propertyName) is { } pcol1)
        {
          // if the column has a property column with that name...
          var columnNumber = table.DataColumns.GetColumnNumber(ycol);
          if (!pcol1.IsElementEmpty(columnNumber))
            return pcol1[columnNumber];
        }
      }

      if (table is not null)
      {
        // try to get the property from the hierarchy of table .. folder .. root folder
        var p = table.GetPropertyValue<object>(propertyName);
        if (p is not null)
        {
          return new AltaxoVariant(p);
        }
      }

      return new AltaxoVariant(); // empty property
    }


    public static AltaxoVariant GetPropertyValueOfCurve(IColumnPlotData curve, string propertyName)
    {
      if (curve is null || string.IsNullOrEmpty(propertyName))
        return new AltaxoVariant();

      DataTable? table = null;
      if (GetRootDataColumn(curve.GetDependentVariable(0)) is { } ycol)
      {
        table = curve.DataTable ?? DataTable.GetParentDataTableOf(ycol);
        if (table is not null)
        {
          if (table.PropCols.TryGetColumn(propertyName) is { } pcol1)
          {
            // if the column has a property column with that name...
            var columnNumber = table.DataColumns.GetColumnNumber(ycol);
            if (!pcol1.IsElementEmpty(columnNumber))
              return pcol1[columnNumber];
          }
        }
      }

      {
        // try to get the property from the data row selection of the xycurve
        foreach (var node in Altaxo.Collections.TreeNodeExtensions.TakeFromHereToFirstLeaves(curve.DataRowSelection))
        {
          if (node is IncludeSingleNumericalValue isn && isn.ColumnName == propertyName)
          {
            var p1 = isn.Value;
            return new AltaxoVariant(p1);
          }
          else if (node is IncludeSingleTextValue istv && istv.ColumnName == propertyName)
          {
            var p1 = istv.Value;
            return new AltaxoVariant(p1);
          }
        }
      }

      if (table is not null)
      {
        // try to get the property from the hierarchy of table .. folder .. root folder
        var p = table.GetPropertyValue<object>(propertyName);
        if (p is not null)
        {
          return new AltaxoVariant(p);
        }
      }

      return new AltaxoVariant(); // empty property
    }


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
  }
}
