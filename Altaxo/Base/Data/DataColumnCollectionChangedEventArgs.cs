#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.Data
{
  /// <summary>
  /// Used for notifying receivers about what columns in this collection have changed.
  /// </summary>
  public class DataColumnCollectionChangedEventArgs : Main.SelfAccumulateableEventArgs
  {
    protected int _minColChanged;
    protected int _maxColChanged;
    protected int _minRowChanged;
    protected int _maxRowChanged;
    protected bool _hasRowCountDecreased;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="columnNumber">The number of the column that has changed.</param>
    /// <param name="minRow">The first number of row that has changed.</param>
    /// <param name="maxRow">The last number of row (plus one) that has changed.</param>
    /// <param name="rowCountDecreased">If true, in one of the columns the row count has decreased, so a complete recalculation of the row count of the collection is neccessary.</param>
    public DataColumnCollectionChangedEventArgs(int columnNumber, int minRow, int maxRow, bool rowCountDecreased)
    {
      _minColChanged = columnNumber;
      _maxColChanged = columnNumber;
      _minRowChanged = minRow;
      _maxRowChanged = maxRow;
      _hasRowCountDecreased = rowCountDecreased;
    }

    /// <summary>
    /// Accumulates the change state by adding a change info from a column.
    /// </summary>
    /// <param name="columnNumber">The number of column that has changed.</param>
    /// <param name="minRow">The lowest row number that has changed.</param>
    /// <param name="maxRow">The highest row number that has changed (plus one).</param>
    /// <param name="hasRowCountDecreased">True if the row count of the column has decreased.</param>
    public void Accumulate(int columnNumber, int minRow, int maxRow, bool hasRowCountDecreased)
    {
      if (columnNumber < _minColChanged)
        _minColChanged = columnNumber;
      if ((columnNumber + 1) > _maxColChanged)
        _maxColChanged = columnNumber + 1;
      if (minRow < _minRowChanged)
        _minRowChanged = minRow;
      if (maxRow > _maxRowChanged)
        _maxRowChanged = maxRow;
      _hasRowCountDecreased |= hasRowCountDecreased;
    }

    /// <summary>
    /// Accumulate the change state by adding another change state.
    /// </summary>
    /// <param name="args">The other change state to be added.</param>
    public void Accumulate(DataColumnCollectionChangedEventArgs args)
    {
      if (args._minColChanged < _minColChanged)
        _minColChanged = args._minColChanged;

      if (args._maxColChanged > _maxColChanged)
        _maxColChanged = args._maxColChanged;

      if (args._minRowChanged < _minRowChanged)
        _minRowChanged = args._minRowChanged;

      if (args.MaxRowChanged > _maxRowChanged)
        _maxRowChanged = args._maxRowChanged;

      _hasRowCountDecreased |= args._hasRowCountDecreased;
    }

    /// <summary>
    /// Creates a change state that reflects the removal of some columns.
    /// </summary>
    /// <param name="firstColumnNumber">The first column number that was removed.</param>
    /// <param name="originalNumberOfColumns">The number of columns in the collection before the removal.</param>
    /// <param name="maxRowCountOfRemovedColumns">The maximum row count of the removed columns.</param>
    /// <returns>The change state that reflects the removal.</returns>
    public static DataColumnCollectionChangedEventArgs CreateColumnRemoveArgs(int firstColumnNumber, int originalNumberOfColumns, int maxRowCountOfRemovedColumns)
    {
      var args = new DataColumnCollectionChangedEventArgs(firstColumnNumber, 0, maxRowCountOfRemovedColumns, true);
      if (originalNumberOfColumns > args._maxColChanged)
        args._maxColChanged = originalNumberOfColumns;
      return args;
    }

    /// <summary>
    /// Creates a change state that reflects the move of some columns.
    /// </summary>
    /// <param name="firstColumnNumber">The first column number that was removed.</param>
    /// <param name="maxColumnNumber">One more than the last affected column.</param>
    /// <returns>The change state that reflects the move.</returns>
    public static DataColumnCollectionChangedEventArgs CreateColumnMoveArgs(int firstColumnNumber, int maxColumnNumber)
    {
      var args = new DataColumnCollectionChangedEventArgs(firstColumnNumber, 0, 0, false)
      {
        _maxColChanged = maxColumnNumber
      };
      return args;
    }

    /// <summary>
    /// Creates a change state that reflects the move of some rows (in all columns).
    /// </summary>
    /// <param name="numberOfColumns">The number of columns in the table.</param>
    /// <param name="firstRowNumber">The first row number that was affected.</param>
    /// <param name="maxRowNumber">One more than the last affected row number.</param>
    /// <returns>The change state that reflects the move.</returns>
    public static DataColumnCollectionChangedEventArgs CreateRowMoveArgs(int numberOfColumns, int firstRowNumber, int maxRowNumber)
    {
      var args = new DataColumnCollectionChangedEventArgs(0, firstRowNumber, maxRowNumber, false)
      {
        _maxColChanged = numberOfColumns
      };
      return args;
    }

    /// <summary>
    /// Create the change state that reflects the addition of one column.
    /// </summary>
    /// <param name="columnIndex">The index of the added column.</param>
    /// <param name="rowCountOfAddedColumn">The row count of the added column.</param>
    /// <returns>The newly created ChangeEventArgs for this case.</returns>
    public static DataColumnCollectionChangedEventArgs CreateColumnAddArgs(int columnIndex, int rowCountOfAddedColumn)
    {
      var args = new DataColumnCollectionChangedEventArgs(columnIndex, 0, rowCountOfAddedColumn, false);
      return args;
    }

    /// <summary>
    /// Create the change state that reflects the renaming of one column.
    /// </summary>
    /// <param name="columnIndex">The index of the renamed column.</param>
    /// <returns>The newly created ChangeEventArgs for this case.</returns>
    public static DataColumnCollectionChangedEventArgs CreateColumnRenameArgs(int columnIndex)
    {
      var args = new DataColumnCollectionChangedEventArgs(columnIndex, 0, 0, false);
      return args;
    }

    /// <summary>
    /// Create the change state that reflects the replace of one column by another (or copying data).
    /// </summary>
    /// <param name="columnIndex">The index of the column to replace.</param>
    /// <param name="oldRowCount">The row count of the old (replaced) column.</param>
    /// <param name="newRowCount">The row count of the new column.</param>
    /// <returns>The newly created ChangeEventArgs for this case.</returns>
    public static DataColumnCollectionChangedEventArgs CreateColumnCopyOrReplaceArgs(int columnIndex, int oldRowCount, int newRowCount)
    {
      var args = new DataColumnCollectionChangedEventArgs(columnIndex, 0, Math.Max(oldRowCount, newRowCount), newRowCount < oldRowCount);
      return args;
    }

    /// <summary>
    /// Returns the lowest column number that has changed.
    /// </summary>
    public int MinColChanged
    {
      get { return _minColChanged; }
    }

    /// <summary>
    /// Returns the highest column number that has changed (plus one).
    /// </summary>
    public int MaxColChanged
    {
      get { return _maxColChanged; }
    }

    /// <summary>
    /// Returns the lowest row number that has changed.
    /// </summary>
    public int MinRowChanged
    {
      get { return _minRowChanged; }
    }

    /// <summary>
    /// Returns the highest row number that has changed (plus one).
    /// </summary>
    public int MaxRowChanged
    {
      get { return _maxRowChanged; }
    }

    /// <summary>
    /// Returns whether the row count may have decreased.
    /// </summary>
    public bool HasRowCountDecreased
    {
      get { return _hasRowCountDecreased; }
    }

    public override void Add(Main.SelfAccumulateableEventArgs e)
    {
      Accumulate((DataColumnCollectionChangedEventArgs)e);
    }
  }
}
