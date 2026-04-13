#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2013 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 3 of the License, or
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

namespace Altaxo.Data
{
  /// <summary>
  /// Read-only accessor for table cell data.
  /// </summary>
  public interface IRODataCollection
  {
    /// <summary>
    /// Gets the data value at the specified column and row index.
    /// </summary>
    /// <param name="columnIndex">The zero-based column index.</param>
    /// <param name="rowIndex">The zero-based row index.</param>
    /// <returns>The value at the specified position.</returns>
    AltaxoVariant this[int columnIndex, int rowIndex] { get; }

    /// <summary>
    /// Gets the data value at the specified column name and row index.
    /// </summary>
    /// <param name="columnName">The column name.</param>
    /// <param name="rowIndex">The zero-based row index.</param>
    /// <returns>The value at the specified position.</returns>
    AltaxoVariant this[string columnName, int rowIndex] { get; }
  }

  /// <summary>
  /// Read-only accessor for table column properties.
  /// </summary>
  public interface IROPropertyCollection
  {
    /// <summary>
    /// Gets the property value at the specified column and property index.
    /// </summary>
    /// <param name="columnIndex">The zero-based column index.</param>
    /// <param name="propertyIndex">The zero-based property index.</param>
    /// <returns>The property value.</returns>
    AltaxoVariant this[int columnIndex, int propertyIndex] { get; }

    /// <summary>
    /// Gets the property value at the specified column index and property name.
    /// </summary>
    /// <param name="columnIndex">The zero-based column index.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>The property value.</returns>
    AltaxoVariant this[int columnIndex, string propertyName] { get; }

    /// <summary>
    /// Gets the property value at the specified column and property name.
    /// </summary>
    /// <param name="columnName">The column name.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>The property value.</returns>
    AltaxoVariant this[string columnName, string propertyName] { get; }
  }

  /// <summary>
  /// Read-only interface for accessing table data and column properties.
  /// </summary>
  public interface IROTableDataSource
  {
    // Data itself
    /// <summary>
    /// Gets a data value by column and row index.
    /// </summary>
    /// <param name="columnIndex">The zero-based column index.</param>
    /// <param name="rowIndex">The zero-based row index.</param>
    /// <returns>The data value.</returns>
    AltaxoVariant GetData(int columnIndex, int rowIndex);

    /// <summary>
    /// Gets a data value by column name and row index.
    /// </summary>
    /// <param name="columnName">The column name.</param>
    /// <param name="rowIndex">The zero-based row index.</param>
    /// <returns>The data value.</returns>
    AltaxoVariant GetData(string columnName, int rowIndex);

    /// <summary>
    /// Gets a column property by column and property index.
    /// </summary>
    /// <param name="columnIndex">The zero-based column index.</param>
    /// <param name="propertyIndex">The zero-based property index.</param>
    /// <returns>The property value.</returns>
    AltaxoVariant GetColumnProperty(int columnIndex, int propertyIndex);

    /// <summary>
    /// Gets a column property by column index and property name.
    /// </summary>
    /// <param name="columnIndex">The zero-based column index.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>The property value.</returns>
    AltaxoVariant GetColumnProperty(int columnIndex, string propertyName);

    /// <summary>
    /// Gets a column property by column name and property name.
    /// </summary>
    /// <param name="columnName">The column name.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>The property value.</returns>
    AltaxoVariant GetColumnProperty(string columnName, string propertyName);

    /// <summary>
    /// Gets the number of data columns.
    /// </summary>
    int ColumnCount { get; }

    /// <summary>
    /// Gets the number of property columns.
    /// </summary>
    int ColumnPropertyCount { get; }

    /// <summary>
    /// Gets the number of accessible rows for the specified column.
    /// </summary>
    /// <param name="i">The zero-based column index.</param>
    /// <returns>The row count for the specified column.</returns>
    int GetRowCount(int i);

    /// <summary>
    /// Gets the name of the specified data column.
    /// </summary>
    /// <param name="i">The zero-based column index.</param>
    /// <returns>The column name.</returns>
    string GetColumnName(int i);

    /// <summary>
    /// Gets the name of the specified property column.
    /// </summary>
    /// <param name="i">The zero-based property-column index.</param>
    /// <returns>The property-column name.</returns>
    string GetColumnPropertyName(int i);
  }

  /// <summary>
  /// Wraps a contiguous row range of a <see cref="DataTable"/> as a read-only table data source.
  /// </summary>
  public class DataTableRangeWrapper : IROTableDataSource
  {
    private DataTable _wrappedTable;
    private int _rangeMin;
    private int _rangeCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTableRangeWrapper"/> class.
    /// </summary>
    /// <param name="wrappedTable">The wrapped source table.</param>
    /// <param name="rangeMin">The first row index in the wrapped range.</param>
    /// <param name="rangeCount">The number of rows in the wrapped range.</param>
    public DataTableRangeWrapper(DataTable wrappedTable, int rangeMin, int rangeCount)
    {
      _wrappedTable = wrappedTable;
      _rangeMin = rangeMin;
      _rangeCount = rangeCount;
    }

    /// <inheritdoc />
    public AltaxoVariant GetData(int columnIndex, int rowIndex)
    {
      return _wrappedTable[columnIndex][rowIndex + _rangeMin];
    }

    /// <inheritdoc />
    public AltaxoVariant GetData(string columnName, int rowIndex)
    {
      return _wrappedTable[columnName][rowIndex + _rangeMin];
    }

    /// <inheritdoc />
    public AltaxoVariant GetColumnProperty(int columnIndex, int propertyIndex)
    {
      return _wrappedTable.PropertyColumns[propertyIndex][columnIndex];
    }

    /// <inheritdoc />
    public AltaxoVariant GetColumnProperty(int columnIndex, string propertyName)
    {
      return _wrappedTable.PropertyColumns[propertyName][columnIndex];
    }

    /// <summary>
    /// Gets a column property by column name and property index.
    /// </summary>
    /// <param name="columnName">The column name.</param>
    /// <param name="propertyIndex">The zero-based property index.</param>
    /// <returns>The property value.</returns>
    public AltaxoVariant GetColumnProperty(string columnName, int propertyIndex)
    {
      int columnIndex = _wrappedTable.DataColumns.GetColumnNumber(_wrappedTable.DataColumns[columnName]);
      return _wrappedTable.PropertyColumns[propertyIndex][columnIndex];
    }

    /// <inheritdoc />
    public AltaxoVariant GetColumnProperty(string columnName, string propertyName)
    {
      int columnIndex = _wrappedTable.DataColumns.GetColumnNumber(_wrappedTable.DataColumns[columnName]);
      return _wrappedTable.PropertyColumns[propertyName][columnIndex];
    }

    /// <inheritdoc />
    public int ColumnCount
    {
      get { return _wrappedTable.DataColumnCount; }
    }

    /// <inheritdoc />
    public int GetRowCount(int i)
    {
      int originalRowCount = _wrappedTable[i].Count;
      return Math.Max(0, Math.Min(originalRowCount - _rangeMin, _rangeCount));
    }

    /// <inheritdoc />
    public int ColumnPropertyCount
    {
      get { return _wrappedTable.PropertyColumnCount; }
    }

    /// <inheritdoc />
    public string GetColumnName(int i)
    {
      return _wrappedTable.DataColumns.GetColumnName(i);
    }

    /// <inheritdoc />
    public string GetColumnPropertyName(int i)
    {
      return _wrappedTable.PropertyColumns.GetColumnName(i);
    }
  }
}
