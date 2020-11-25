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
using System.Collections.Generic;
using System.Linq;
using Altaxo.Main;

namespace Altaxo.Data
{
  /// <summary>
  /// Used for notifying receivers about what columns in this collection have changed.
  /// </summary>
  public abstract class BaseColumnCollectionChangedEventArgs : Main.SelfAccumulateableEventArgs
  {
    protected int _minColChanged;
    protected int _maxColChanged;
    protected int _minRowChanged;
    protected int _maxRowChanged;
    protected bool _hasRowCountDecreased;

    /// <summary>
    /// Stores column name changed, or deletions and additions of columns. The oldest name change is at the bottom of the list at index 0.
    /// For deletion, the NewName field is set to null. For addition, the OldName field is set to null.
    /// </summary>
    private List<(string? OldName, string? NewName)>? _columnNameChanges;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="columnNumber">The number of the column that has changed.</param>
    /// <param name="minRow">The first number of row that has changed.</param>
    /// <param name="maxRow">The last number of row (plus one) that has changed.</param>
    /// <param name="rowCountDecreased">If true, in one of the columns the row count has decreased, so a complete recalculation of the row count of the collection is neccessary.</param>
    public BaseColumnCollectionChangedEventArgs(int columnNumber, int minRow, int maxRow, bool rowCountDecreased)
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
    public void Accumulate(BaseColumnCollectionChangedEventArgs args)
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

      if (args._columnNameChanges is { } list)
      {
        _columnNameChanges ??= new List<(string? OldName, string? NewName)>();
        _columnNameChanges.AddRange(list);
      }
    }

    /// <summary>
    /// Creates a change state that reflects the removal of some columns.
    /// </summary>
    /// <param name="isPropertyColumn">True if the call comes from a collection of property columns; false if the call comes from a collection of data columns.</param>
    /// <param name="columnNames">The names of the columns that were removed.</param>
    /// <param name="firstColumnNumber">The first column number that was removed.</param>
    /// <param name="originalNumberOfColumns">The number of columns in the collection before the removal.</param>
    /// <param name="maxRowCountOfRemovedColumns">The maximum row count of the removed columns.</param>
    /// <returns>The change state that reflects the removal.</returns>
    public static BaseColumnCollectionChangedEventArgs CreateColumnRemoveArgs(bool isPropertyColumn, IEnumerable<string> columnNames, int firstColumnNumber, int originalNumberOfColumns, int maxRowCountOfRemovedColumns)
    {
      BaseColumnCollectionChangedEventArgs args;
      if (isPropertyColumn)
        args = new PropertyColumnCollectionChangedEventArgs(firstColumnNumber, 0, maxRowCountOfRemovedColumns, true);
      else
        args = new DataColumnCollectionChangedEventArgs(firstColumnNumber, 0, maxRowCountOfRemovedColumns, true);


      if (originalNumberOfColumns > args._maxColChanged)
        args._maxColChanged = originalNumberOfColumns;

      args._columnNameChanges ??= new List<(string? OldName, string? NewName)>();
      foreach (var name in columnNames)
      {
        args._columnNameChanges.Add((name, null));
      }

      return args;
    }

    /// <summary>
    /// Creates a change state that reflects the move of some columns.
    /// </summary>
    /// <param name="isPropertyColumn">True if the call comes from a collection of property columns; false if the call comes from a collection of data columns.</param>
    /// <param name="firstColumnNumber">The first column number that was removed.</param>
    /// <param name="maxColumnNumber">One more than the last affected column.</param>
    /// <returns>The change state that reflects the move.</returns>
    public static BaseColumnCollectionChangedEventArgs CreateColumnMoveArgs(bool isPropertyColumn, int firstColumnNumber, int maxColumnNumber)
    {
      BaseColumnCollectionChangedEventArgs args;
      if (isPropertyColumn)
        args = new PropertyColumnCollectionChangedEventArgs(firstColumnNumber, 0, 0, false);
      else
        args = new DataColumnCollectionChangedEventArgs(firstColumnNumber, 0, 0, false);


      args._maxColChanged = maxColumnNumber;
      return args;
    }

    /// <summary>
    /// Creates a change state that reflects the move of some rows (in all columns).
    /// </summary>
    /// <param name="isPropertyColumn">True if the call comes from a collection of property columns; false if the call comes from a collection of data columns.</param>
    /// <param name="numberOfColumns">The number of columns in the table.</param>
    /// <param name="firstRowNumber">The first row number that was affected.</param>
    /// <param name="maxRowNumber">One more than the last affected row number.</param>
    /// <returns>The change state that reflects the move.</returns>
    public static BaseColumnCollectionChangedEventArgs CreateRowMoveArgs(bool isPropertyColumn, int numberOfColumns, int firstRowNumber, int maxRowNumber)
    {
      BaseColumnCollectionChangedEventArgs args;
      if (isPropertyColumn)
        args = new PropertyColumnCollectionChangedEventArgs(0, firstRowNumber, maxRowNumber, false);
      else
        args = new DataColumnCollectionChangedEventArgs(0, firstRowNumber, maxRowNumber, false);

      args._maxColChanged = numberOfColumns;

      return args;
    }

    /// <summary>
    /// Create the change state that reflects the addition of one column.
    /// </summary>
    /// <param name="isPropertyColumn">True if the call comes from a collection of property columns; false if the call comes from a collection of data columns.</param>
    /// <param name="columnName">Name of the freshly added column.</param>
    /// <param name="columnIndex">The index of the added column.</param>
    /// <param name="rowCountOfAddedColumn">The row count of the added column.</param>
    /// <returns>The newly created ChangeEventArgs for this case.</returns>
    public static BaseColumnCollectionChangedEventArgs CreateColumnAddArgs(bool isPropertyColumn, string columnName, int columnIndex, int rowCountOfAddedColumn)
    {
      BaseColumnCollectionChangedEventArgs args;
      if (isPropertyColumn)
        args = new PropertyColumnCollectionChangedEventArgs(columnIndex, 0, rowCountOfAddedColumn, false);
      else
        args = new DataColumnCollectionChangedEventArgs(columnIndex, 0, rowCountOfAddedColumn, false);

      args._columnNameChanges ??= new List<(string? OldName, string? NewName)>();
      args._columnNameChanges.Add((null, columnName));
      return args;
    }

    /// <summary>
    /// Create the change state that reflects the renaming of one column.
    /// </summary>
    /// <param name="isPropertyColumn">True if the call comes from a collection of property columns; false if the call comes from a collection of data columns.</param>
    /// <param name="oldName">The name of the column before it was renamed.</param>
    /// <param name="newName">The new name of the column.</param>
    /// <param name="columnIndex">The index of the renamed column.</param>
    /// <returns>The newly created ChangeEventArgs for this case.</returns>
    public static BaseColumnCollectionChangedEventArgs CreateColumnRenameArgs(bool isPropertyColumn, string oldName, string newName, int columnIndex)
    {
      BaseColumnCollectionChangedEventArgs args;
      if (isPropertyColumn)
        args = new PropertyColumnCollectionChangedEventArgs(columnIndex, 0, 0, false);
      else
        args = new DataColumnCollectionChangedEventArgs(columnIndex, 0, 0, false);

      args._columnNameChanges ??= new List<(string? OldName, string? NewName)>();
      args._columnNameChanges.Add((oldName, newName));
      return args;
    }

    /// <summary>
    /// Create the change state that reflects the replace of one column by another (or copying data).
    /// </summary>
    /// <param name="isPropertyColumn">True if the call comes from a collection of property columns; false if the call comes from a collection of data columns.</param>
    /// <param name="columnName">The name of the column that was replaced.</param>
    /// <param name="columnIndex">The index of the column to replace.</param>
    /// <param name="oldRowCount">The row count of the old (replaced) column.</param>
    /// <param name="newRowCount">The row count of the new column.</param>
    /// <returns>The newly created ChangeEventArgs for this case.</returns>
    public static BaseColumnCollectionChangedEventArgs CreateColumnCopyOrReplaceArgs(bool isPropertyColumn, string columnName, int columnIndex, int oldRowCount, int newRowCount)
    {
      BaseColumnCollectionChangedEventArgs args;
      if (isPropertyColumn)
        args = new PropertyColumnCollectionChangedEventArgs(columnIndex, 0, Math.Max(oldRowCount, newRowCount), newRowCount < oldRowCount);
      else
        args = new DataColumnCollectionChangedEventArgs(columnIndex, 0, Math.Max(oldRowCount, newRowCount), newRowCount < oldRowCount);

      // we treat replace as remove and add
      args._columnNameChanges ??= new List<(string? OldName, string? NewName)>();
      args._columnNameChanges.Add((columnName, null));
      args._columnNameChanges.Add((null, columnName));

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




    /// <summary>
    /// Gets the column addition, deletion, and rename operations.
    /// For additions, the OldName field in the tuple is null, and the NewName field contains the name of the added column.
    /// For deletions, the NewName field of the tuple is null, and the OldName field contains the name of the deleted column.
    /// For renamings, the OldName filed of the tuple contains the old name of the column, and the NewName field contains the new name.
    /// </summary>
    public IEnumerable<(string? OldName, string? NewName)> ColumnAdditionDeletionRenameOperations
    {
      get
      {
        return _columnNameChanges ?? Enumerable.Empty<(string? OldName, string? NewName)>();
      }
    }
  }

  public class DataColumnCollectionChangedEventArgs : BaseColumnCollectionChangedEventArgs
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="columnNumber">The number of the column that has changed.</param>
    /// <param name="minRow">The first number of row that has changed.</param>
    /// <param name="maxRow">The last number of row (plus one) that has changed.</param>
    /// <param name="rowCountDecreased">If true, in one of the columns the row count has decreased, so a complete recalculation of the row count of the collection is neccessary.</param>
    public DataColumnCollectionChangedEventArgs(int columnNumber, int minRow, int maxRow, bool rowCountDecreased)
      : base(columnNumber, minRow, maxRow, rowCountDecreased)
    {
    }

    public override void Add(Main.SelfAccumulateableEventArgs e)
    {
      Accumulate((DataColumnCollectionChangedEventArgs)e);
    }
  }

  public class PropertyColumnCollectionChangedEventArgs
        : BaseColumnCollectionChangedEventArgs
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="columnNumber">The number of the column that has changed.</param>
    /// <param name="minRow">The first number of row that has changed.</param>
    /// <param name="maxRow">The last number of row (plus one) that has changed.</param>
    /// <param name="rowCountDecreased">If true, in one of the columns the row count has decreased, so a complete recalculation of the row count of the collection is neccessary.</param>
    public PropertyColumnCollectionChangedEventArgs(int columnNumber, int minRow, int maxRow, bool rowCountDecreased)
      : base(columnNumber, minRow, maxRow, rowCountDecreased)
    {
    }

    public override void Add(Main.SelfAccumulateableEventArgs e)
    {
      Accumulate((PropertyColumnCollectionChangedEventArgs)e);
    }
  }
}
