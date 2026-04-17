#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

namespace Altaxo.Data
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Holds a snapshot of the column names, column kinds, and group numbers of a <see cref="DataTable"/>, both for data columns and property columns.
  /// That snapshot can be used to restore these to a <see cref="DataTable"/> at a later point in time.
  /// </summary>
  public record DataTableColumnNamesKindGroupSnapshot
  {
    private string[] _dataColumnNames;
    private ColumnKind[] _dataColumnKinds;
    private int[] _dataColumnGroupNumbers;
    private string[] _propertyColumnNames;
    private ColumnKind[] _propertyColumnKinds;
    private int[] _propertyColumnGroupNumbers;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTableColumnNamesKindGroupSnapshot"/> class.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    public DataTableColumnNamesKindGroupSnapshot(DataTable destinationTable)
    {
      _dataColumnNames = new string[destinationTable.DataColumnCount];
      _dataColumnKinds = new ColumnKind[destinationTable.DataColumnCount];
      _dataColumnGroupNumbers = new int[destinationTable.DataColumnCount];
      for (var i = 0; i < destinationTable.DataColumnCount; i++)
      {
        _dataColumnNames[i] = destinationTable.DataColumns.GetColumnName(i);
        _dataColumnKinds[i] = destinationTable.DataColumns.GetColumnKind(i);
        _dataColumnGroupNumbers[i] = destinationTable.DataColumns.GetColumnGroup(i);
      }

      _propertyColumnNames = new string[destinationTable.PropertyColumnCount];
      _propertyColumnKinds = new ColumnKind[destinationTable.PropertyColumnCount];
      _propertyColumnGroupNumbers = new int[destinationTable.PropertyColumnCount];
      for (var i = 0; i < destinationTable.PropertyColumnCount; i++)
      {
        _propertyColumnNames[i] = destinationTable.PropCols.GetColumnName(i);
        _propertyColumnKinds[i] = destinationTable.PropCols.GetColumnKind(i);
        _propertyColumnGroupNumbers[i] = destinationTable.PropCols.GetColumnGroup(i);
      }
    }

    private static void RestoreTo(DataColumnCollection col, string[]? names, ColumnKind[]? kinds, int[]? groupNumbers, bool restoreNames, bool restoreKindAndGroups)
    {

      if (restoreNames)
      {
        var dict = new Dictionary<int, string>();
        for (var i = 0; i < col.ColumnCount; i++)
        {
          if (i < names?.Length)
          {
            var c = col.TryGetColumn(names[i]);
            if (c is not null)
            {
              var ci = col.GetColumnNumber(c);
              if (i != ci)
              {
                // we have to give that column c temporarily another name to be able to set the correct name at position i
                col.SetColumnName(c, Guid.NewGuid().ToString());

                if (ci >= names.Length) // we need to store the original name only if it is outside of the range that we rename anyway.
                {
                  dict[ci] = names[i];
                }
              }
            }
            col.SetColumnName(i, names[i]);
          }
        }


        // now restore the original names of the columns that we renamed temporarily
        foreach (var kvp in dict)
        {
          col.SetColumnName(kvp.Key, col.FindUniqueColumnName(kvp.Value));
        }
      }

      if (restoreKindAndGroups)
      {
        for (var i = 0; i < col.ColumnCount; i++)
        {
          if (i < kinds?.Length)
          {
            col.SetColumnKind(i, kinds[i]);
          }
          if (i < groupNumbers?.Length)
          {
            col.SetColumnGroup(i, groupNumbers[i]);
          }
        }
      }
    }

    /// <summary>
    /// Restores names, kinds, and group numbers to the given destination table according to the snapshot. Whether names and/or kinds and group numbers are restored can be controlled separately.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="restoreNames">If set to <c>true</c>, the column names will be restored.</param>
    /// <param name="restoreKindAndGroups">If set to <c>true</c>, the column kinds and group numbers will be restored.</param>
    public void RestoreTo(DataTable destinationTable, bool restoreNames, bool restoreKindAndGroups)
    {
      RestoreTo(destinationTable.DataColumns, _dataColumnNames, _dataColumnKinds, _dataColumnGroupNumbers, restoreNames, restoreKindAndGroups);
      RestoreTo(destinationTable.PropCols, _propertyColumnNames, _propertyColumnKinds, _propertyColumnGroupNumbers, restoreNames, restoreKindAndGroups);
    }

    /// <summary>
    /// Restores names, kinds, and group numbers to the given destination table according to the snapshot. Whether names and/or kinds and group numbers are restored can be controlled separately.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="restoreDataColumnNames">If set to <c>true</c>, the column names of the data columns will be restored.</param>
    /// <param name="restoreDataColumnKindsAndGroups">If set to <c>true</c>, the column kinds and group numbers of the data columns will be restored.</param>
    /// <param name="restorePropertyColumnNames">If set to <c>true</c>, the column names of the property columns  will be restored.</param>
    /// <param name="restorePropertyColumnKindsAndGroups">If set to <c>true</c>, the column kinds and group numbers of the property columns will be restored.</param>
    public void RestoreTo(DataTable destinationTable, bool restoreDataColumnNames, bool restoreDataColumnKindsAndGroups, bool restorePropertyColumnNames, bool restorePropertyColumnKindsAndGroups)
    {
      RestoreTo(destinationTable.DataColumns, _dataColumnNames, _dataColumnKinds, _dataColumnGroupNumbers, restoreDataColumnNames, restoreDataColumnKindsAndGroups);
      RestoreTo(destinationTable.PropCols, _propertyColumnNames, _propertyColumnKinds, _propertyColumnGroupNumbers, restorePropertyColumnNames, restorePropertyColumnKindsAndGroups);
    }
  }
}
