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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
  public interface IRODataCollection
  {
    AltaxoVariant this[int columnIndex, int rowIndex] { get; }

    AltaxoVariant this[string columnName, int rowIndex] { get; }
  }

  public interface IROPropertyCollection
  {
    AltaxoVariant this[int columnIndex, int propertyIndex] { get; }

    AltaxoVariant this[int columnIndex, string propertyName] { get; }

    AltaxoVariant this[string columnName, string propertyName] { get; }
  }

  /// <summary>
  /// Interface
  /// </summary>
  public interface IROTableDataSource
  {
    // Data itself
    AltaxoVariant GetData(int columnIndex, int rowIndex);

    AltaxoVariant GetData(string columnName, int rowIndex);

    AltaxoVariant GetColumnProperty(int columnIndex, int propertyIndex);

    AltaxoVariant GetColumnProperty(int columnIndex, string propertyName);

    AltaxoVariant GetColumnProperty(string columnName, string propertyName);

    int ColumnCount { get; }

    int ColumnPropertyCount { get; }

    int GetRowCount(int i);

    string GetColumnName(int i);

    string GetColumnPropertyName(int i);
  }

  public class DataTableRangeWrapper : IROTableDataSource
  {
    private DataTable _wrappedTable;
    private int _rangeMin;
    private int _rangeCount;

    public DataTableRangeWrapper(DataTable wrappedTable, int rangeMin, int rangeCount)
    {
      _wrappedTable = wrappedTable;
      _rangeMin = rangeMin;
      _rangeCount = rangeCount;
    }

    public AltaxoVariant GetData(int columnIndex, int rowIndex)
    {
      return _wrappedTable[columnIndex][rowIndex + _rangeMin];
    }

    public AltaxoVariant GetData(string columnName, int rowIndex)
    {
      return _wrappedTable[columnName][rowIndex + _rangeMin];
    }

    public AltaxoVariant GetColumnProperty(int columnIndex, int propertyIndex)
    {
      return _wrappedTable.PropertyColumns[propertyIndex][columnIndex];
    }

    public AltaxoVariant GetColumnProperty(int columnIndex, string propertyName)
    {
      return _wrappedTable.PropertyColumns[propertyName][columnIndex];
    }

    public AltaxoVariant GetColumnProperty(string columnName, int propertyIndex)
    {
      int columnIndex = _wrappedTable.DataColumns.GetColumnNumber(_wrappedTable.DataColumns[columnName]);
      return _wrappedTable.PropertyColumns[propertyIndex][columnIndex];
    }

    public AltaxoVariant GetColumnProperty(string columnName, string propertyName)
    {
      int columnIndex = _wrappedTable.DataColumns.GetColumnNumber(_wrappedTable.DataColumns[columnName]);
      return _wrappedTable.PropertyColumns[propertyName][columnIndex];
    }

    public int ColumnCount
    {
      get { return _wrappedTable.DataColumnCount; }
    }

    public int GetRowCount(int columnIndex)
    {
      int originalRowCount = _wrappedTable[columnIndex].Count;
      return Math.Max(0, Math.Min(originalRowCount - _rangeMin, _rangeCount));
    }

    public int ColumnPropertyCount
    {
      get { return _wrappedTable.PropertyColumnCount; }
    }

    public string GetColumnName(int i)
    {
      return _wrappedTable.DataColumns.GetColumnName(i);
    }

    public string GetColumnPropertyName(int i)
    {
      return _wrappedTable.PropertyColumns.GetColumnName(i);
    }
  }
}
