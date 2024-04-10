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
using System.Collections.Generic;

namespace Altaxo.Data
{
  /// <summary>
  /// Wrapper struct for a row of a data column collection.
  /// </summary>
  public struct DataRow : IEnumerable<AltaxoVariant>
  {
    private DataColumnCollection _col;
    private int _rowIdx;

    /// <summary>
    /// Gets a value indicating that is instance is not initialized.
    /// </summary>
    public bool IsEmpty => _col is null;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="col">The underlying <see cref="DataColumnCollection"/>.</param>
    /// <param name="rowIndex">Index of the row of the data column collection.</param>
    public DataRow(DataColumnCollection col, int rowIndex)
    {
      _col = col ?? throw new System.ArgumentNullException(nameof(col));
      _rowIdx = rowIndex;
    }

    /// <summary>
    /// Gets the index of the wrapped row.
    /// </summary>
    public int RowIndex
    {
      get { return _rowIdx; }
    }

    /// <summary>
    /// Gets the underlying data column collection.
    /// </summary>
    public DataColumnCollection ColumnCollection
    {
      get { return _col; }
    }

    /// <summary>
    /// Accesses one field of the row.
    /// </summary>
    /// <param name="i">Column index.</param>
    /// <returns>Element at column[i] and the wrapped row index.</returns>
    public AltaxoVariant this[int i]
    {
      get { return _col[i][_rowIdx]; }
      set { _col[i][_rowIdx] = value; }
    }

    /// <summary>
    /// Accesses one field of the row.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Element at column[name] and the wrapped row index.</returns>
    public AltaxoVariant this[string name]
    {
      get { return _col[name][_rowIdx]; }
      set { _col[name][_rowIdx] = value; }
    }

    #region IEnumerable<AltaxoVariant> Members

    /// <summary>
    /// Gets the enumeratur that iterates through all elements of this row.
    /// </summary>
    /// <returns>Enumeratur.</returns>
    public IEnumerator<AltaxoVariant> GetEnumerator()
    {
      for (int i = 0; i < _col.ColumnCount; i++)
        yield return _col[i][_rowIdx];
    }

    #endregion IEnumerable<AltaxoVariant> Members

    #region IEnumerable Members

    /// <summary>
    /// Gets the enumeratur that iterates through all elements of this row.
    /// </summary>
    /// <returns>Enumeratur.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      for (int i = 0; i < _col.ColumnCount; i++)
        yield return _col[i][_rowIdx];
    }

    #endregion IEnumerable Members

    #region Convenience function

    /// <summary>
    /// Copies data from another row into this row. The data is copied using the name of the column.
    /// It is copied lazy, thus it is not an error some columns don't exist on the destination or source,
    /// or if the data types doesn't match.
    /// </summary>
    /// <param name="another">Another.</param>
    public void CopyLazyByNameFrom(DataRow another)
    {
      for (int c = 0; c < another.ColumnCollection.ColumnCount; c++)
      {
        var name = another.ColumnCollection.GetColumnName(another.ColumnCollection[c]);
        if (this.ColumnCollection.Contains(name))
        {
          try
          {
            this[name] = another[name];
          }
          catch (System.Exception)
          {
          }
        }
      }
    }

    #endregion
  }
}
