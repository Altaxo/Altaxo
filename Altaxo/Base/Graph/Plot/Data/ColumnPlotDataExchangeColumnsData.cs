#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Text;
using System.Threading.Tasks;
using Altaxo.Data;

namespace Altaxo.Graph.Plot.Data
{
  /// <summary>
  /// Model used to exchange common column names in multiple plot items concurrently.
  /// </summary>
  public class ColumnPlotDataExchangeColumnsData : ColumnPlotDataExchangeDataBase, ICloneable
  {
    protected List<(string ColumnGroup, string ColumnLabel, string? ColumnName, string? NewColumnName)> _columns = new List<(string ColumnGroup, string ColumnLabel, string? ColumnName, string? NewColumnName)>();

    /// <summary>
    /// Gets the columns with common column group, column label, and column name for all plot items. After processing the data by a user dialog,
    /// the data also contain a field NewColumnName, which designates the new column name that should be used instead of the old.
    /// </summary>
    /// <value>
    /// The columns with common column group, column label, and column name.
    /// </value>
    public IReadOnlyList<(string ColumnGroup, string ColumnLabel, string? ColumnName, string? NewColumnName)> Columns { get { return _columns; } }

    protected List<DataTable> _tables = new List<DataTable>();

    public int GroupNumber { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnPlotDataExchangeColumnsData"/> class.
    /// </summary>
    /// <param name="plotItems">The plot items, for which to change common columns.</param>
    public ColumnPlotDataExchangeColumnsData(IEnumerable<Altaxo.Graph.Plot.IGPlotItem> plotItems)
    {
      PlotItems = plotItems;
      CollectCommonColumnNamesAndTablesFromPlotItems();
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public object Clone()
    {
      return MemberwiseClone();
    }

    /// <summary>
    /// Sets the new name of the column.
    /// </summary>
    /// <param name="idx">The index of the column in <see cref="Columns"/>.</param>
    /// <param name="newColumnName">The new name of the column. If the argument is null, the old column(s) will be replaced with nothing.</param>
    public void SetNewColumnName(int idx, string newColumnName)
    {
      var e = _columns[idx];
      _columns[idx] = (e.ColumnGroup, e.ColumnLabel, e.ColumnName, newColumnName);
    }

    /// <summary>
    /// Collects the tuples of (ColumnGroupLabel, ColumnLabel, ColumnName) that are common to all plot items. Furthermore,
    /// it stores the underlying tables of the plot items in another collection <see cref="_tables"/>.
    /// </summary>
    public void CollectCommonColumnNamesAndTablesFromPlotItems()
    {
      HashSet<(string ColumnGroup, string ColumnLabel, string? ColumnName, string? NewColumnName)>? totalSet = null;
      var dataTables = new HashSet<DataTable>();

      // collect all column names from those plot items
      foreach (var plotItem in PlotItems)
      {
        if (plotItem.DataObject is IColumnPlotData columnPlotData && columnPlotData.DataTable != null)
        {
          dataTables.Add(columnPlotData.DataTable);

          var localSet = new HashSet<(string ColumnGroup, string ColumnLabel, string? ColumnName, string? NewColumnName)>();
          // collect from the plot item's row selection
          foreach (var columnInfo in ColumnPlotDataExchangeTableData.EnumerateAllDataColumnsOfPlotItem(plotItem, (info) => true))
          {
            localSet.Add((columnInfo.ColumnGroup, columnInfo.ColumnLabel, columnInfo.ColumnName, null));
          }

          if (totalSet is null)
            totalSet = localSet;
          else
            totalSet.IntersectWith(localSet);
        }
      }

      _columns.Clear();
      if (totalSet is not null)
      {
        foreach (var item in totalSet)
          _columns.Add(item);
      }

      _columns.Sort();
      _tables = new List<DataTable>(dataTables);
    }

    /// <summary>
    /// Determines whether it is possible to change one or more data columns with names that are common for all specified plot items.
    /// </summary>
    /// <param name="plotItems">The plot items.</param>
    /// <returns>
    ///   <c>true</c> if this instance [can change table for plot items] the specified plot items; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanChangeCommonColumnsForPlotItems(IEnumerable<Altaxo.Graph.Plot.IGPlotItem> plotItems)
    {
      var data = new ColumnPlotDataExchangeColumnsData(plotItems);

      return data.Columns.Count > 0;
    }

    /// <summary>
    /// Shows a dialog that allows to change the underlying data table for the provided plot items.
    /// </summary>
    /// <param name="plotItems">The plot items.</param>
    /// <returns>True if the dialog was shown and for at least one plot item the underlying table was exchanged; otherwise false.</returns>
    public static bool ShowChangeColumnsForSelectedItemsDialog(IEnumerable<Altaxo.Graph.Plot.IGPlotItem> plotItems)
    {
      var data = new ColumnPlotDataExchangeColumnsData(plotItems);
      if (!(data.Columns.Count > 0))
        return false; // nothing to do

      object exchangeColumnDataObject = data;
      if (!Current.Gui.ShowDialog(ref exchangeColumnDataObject, "Select new columns for plot items"))
        return false;

      data = (ColumnPlotDataExchangeColumnsData)exchangeColumnDataObject;

      // now exchange the data
      data.ExchangeColumns();

      return true;
    }

    /// <summary>
    /// Exchanges the data columns of the plot items in <see cref="ColumnPlotDataExchangeDataBase.PlotItems"/>, using the field NewTableName in each entry of <see cref="Columns"/>.
    /// </summary>
    public void ExchangeColumns()
    {
      foreach (var plotItem in PlotItems)
      {
        if (plotItem.DataObject is IColumnPlotData columnPlotData && columnPlotData.DataTable != null)
        {
          foreach (var columnInfo in ColumnPlotDataExchangeTableData.EnumerateAllDataColumnsOfPlotItem(plotItem, (info) => true))
          {
            foreach (var info in _columns) // we can not access info directly, since we have no key. So we must dumbly compare it item by item
            {
              if (
                info.ColumnGroup == columnInfo.ColumnGroup &&
                info.ColumnLabel == columnInfo.ColumnLabel &&
                info.ColumnName == columnInfo.ColumnName &&
                info.ColumnName != info.NewColumnName)
              {
                if (string.IsNullOrEmpty(info.NewColumnName))
                {
                  columnInfo.ColumnSetAction(null, columnPlotData.DataTable, GroupNumber);
                }
                else
                {
                  if (columnPlotData.DataTable.DataColumns.Contains(info.NewColumnName))
                  {
                    var newCol = columnPlotData.DataTable.DataColumns[info.NewColumnName];
                    var group = columnPlotData.DataTable.DataColumns.GetColumnGroup(newCol);
                    columnInfo.ColumnSetAction(newCol, columnPlotData.DataTable, GroupNumber);
                  }
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Gets a sorted list of group numbers that are common to all underlying tables of the plot items.
    /// </summary>
    /// <returns>Sorted list of group numbers that are common to all underlying tables of the plot items.</returns>
    public SortedSet<int> GetCommonGroupNumbersFromTables()
    {
      HashSet<int>? commonGroupNumbers = null;
      foreach (var table in _tables)
      {
        var localGroupNumbers = new HashSet<int>(table.DataColumns.GetGroupNumbersAll());

        if (commonGroupNumbers is null)
          commonGroupNumbers = localGroupNumbers;
        else
          commonGroupNumbers.IntersectWith(localGroupNumbers);
      }

      return commonGroupNumbers is not null ? new SortedSet<int>(commonGroupNumbers) : new SortedSet<int>();
    }

    /// <summary>
    /// Gets the common column names of all underlying tables of the plot items. This procedure is ignoring the group numbers of the columns.
    /// See <see cref="GetCommonColumnNamesWithGroupNumber(int)"/> if common column names with a specific group number should be retrieved.
    /// </summary>
    /// <returns>Array of column names common to all underlying tables of the plot items.</returns>
    public string[] GetCommonColumnNames()
    {
      HashSet<string>? commonColumnNames = null;
      foreach (var table in _tables)
      {
        var localHashSet = new HashSet<string>(table.DataColumns.GetColumnNames());

        if (commonColumnNames is null)
          commonColumnNames = localHashSet;
        else
          commonColumnNames.IntersectWith(localHashSet);
      }

      var result = commonColumnNames?.ToArray() ?? new string[0];
      Array.Sort(result);
      return result;
    }

    /// <summary>
    /// Gets the common column names of all underlying tables of the plot items with a specific group number.
    /// </summary>
    /// <param name="groupNumber">The group number of the table columns.</param>
    /// <returns>Array of column names common to all underlying tables of the plot items, which have the provided group number.</returns>
    public List<string> GetCommonColumnNamesWithGroupNumber(int groupNumber)
    {
      HashSet<string>? commonColumnNames = null;
      foreach (var table in _tables)
      {
        var localHashSet = new HashSet<string>(table.DataColumns.GetNamesOfColumnsWithGroupNumber(groupNumber));

        if (commonColumnNames is null)
          commonColumnNames = localHashSet;
        else
          commonColumnNames.IntersectWith(localHashSet);
      }

      var result = commonColumnNames is not null ? new List<string>(commonColumnNames) : new List<string>();
      return result;
    }
  }
}
