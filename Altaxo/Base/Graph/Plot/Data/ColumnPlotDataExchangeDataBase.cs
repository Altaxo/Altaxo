﻿#region Copyright

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
using Altaxo.Data;

namespace Altaxo.Graph.Plot.Data
{
  /// <summary>
  /// Base class for models that are used to exchange the underlying table or data columns of plot items.
  /// </summary>
  public abstract class ColumnPlotDataExchangeDataBase
  {
    /// <summary>
    /// Gets or sets the plot items or plot data, for which either the underlying data table or data columns should be changed.
    /// </summary>
    public IEnumerable<object> PlotItemsOrPlotData { get; protected set; } = new object[0];

    /// <summary>
    /// Enumerates all data columns used by an <see cref="Altaxo.Graph.Plot.IGPlotItem"/>
    /// </summary>
    /// <param name="plotItem">The plot item for which to enumerate the data columns in use.</param>
    /// <param name="predicate">The predicate. Should return true if the data column provided in the argument should be enumerated.</param>
    /// <returns>Enumeration of all data columns used by the plot item. Note that also the broken data columns will be reported with name and label (but of course the tuple member DataColumn is null in this case).</returns>
    public static IEnumerable<
      (
      string ColumnGroup,
      string ColumnLabel,
      IReadableColumn? Column,
      string? ColumnName,
      Action<IReadableColumn?, DataTable, int> ColumnSetAction
      )>
      EnumerateAllDataColumnsOfPlotItem(
        Altaxo.Graph.Plot.IGPlotItem plotItem,
        Predicate<(DataColumn? Column, string? ColumnName)> predicate)
    {
      if (plotItem.DataObject is IColumnPlotData data)
      {
        // first the row selection(s)
        foreach (var columnInfo in data.DataRowSelection.GetAdditionallyUsedColumns())
        {
          if ((columnInfo.Column is DataColumn dataColumn1 && predicate((dataColumn1, columnInfo.ColumnName))) || (columnInfo.Column is null && !string.IsNullOrEmpty(columnInfo.ColumnName)))
          {
            yield return (nameof(data.DataRowSelection), columnInfo.ColumnLabel, columnInfo.Column, columnInfo.ColumnName, (col, tbl, grp) => columnInfo.SetColumnAction(col ?? new IndexerColumn()));
          }
          else if ((columnInfo.Column is TransformedReadableColumn transColumn && transColumn.UnderlyingReadableColumn is DataColumn dataColumn2 && predicate((dataColumn2, columnInfo.ColumnName))))
          {
            yield return (nameof(data.DataRowSelection), columnInfo.ColumnLabel, columnInfo.Column, columnInfo.ColumnName, (col, tbl, grp) => columnInfo.SetColumnAction(col is null ? (IReadableColumn)new IndexerColumn() : transColumn.WithUnderlyingReadableColumn(col)));
          }
        }

        // now the data itself
        foreach (var t in data.GetAdditionallyUsedColumns())
        {
          foreach (var columnInfo in t.ColumnInfos)
          {
            if ((columnInfo.Column is DataColumn dataColumn1 && predicate((dataColumn1, columnInfo.ColumnName))) || (columnInfo.Column is null && !string.IsNullOrEmpty(columnInfo.ColumnName)))
            {
              yield return (t.NameOfColumnGroup, columnInfo.ColumnLabel, columnInfo.Column, columnInfo.ColumnName, columnInfo.SetColumnAction);
            }
            else if ((columnInfo.Column is TransformedReadableColumn transColumn && transColumn.UnderlyingReadableColumn is DataColumn dataColumn2 && predicate((dataColumn2, columnInfo.ColumnName))))
            {
              yield return (t.NameOfColumnGroup, columnInfo.ColumnLabel, columnInfo.Column, columnInfo.ColumnName, (col, tbl, grp) => columnInfo.SetColumnAction(col is null ? col : transColumn.WithUnderlyingReadableColumn(col), tbl, grp));
            }
          }
        }

        // now the styles
        if (plotItem.StyleObject is IEnumerable<Altaxo.Graph.Plot.Styles.IGPlotStyle> styleEn)
        {
          foreach (var style in styleEn)
          {
            foreach (var columnInfo in style.GetAdditionallyUsedColumns() ?? EmptyColumnInfoEnumeration)
            {
              if ((columnInfo.Column is DataColumn dataColumn1 && predicate((dataColumn1, columnInfo.ColumnName))) || (columnInfo.Column is null && !string.IsNullOrEmpty(columnInfo.ColumnName)))
              {
                yield return (style.GetType().Name, columnInfo.ColumnLabel, columnInfo.Column, columnInfo.ColumnName, (col, tbl, grp) => columnInfo.ColumnSetAction(col));
              }
              else if ((columnInfo.Column is TransformedReadableColumn transColumn && transColumn.UnderlyingReadableColumn is DataColumn dataColumn2 && predicate((dataColumn2, columnInfo.ColumnName))))
              {
                yield return (style.GetType().Name, columnInfo.ColumnLabel, columnInfo.Column, columnInfo.ColumnName, (col, tbl, grp) => columnInfo.ColumnSetAction(col is null ? col : transColumn.WithUnderlyingReadableColumn(col)));
              }
            }
          }
        }
        else if (plotItem.StyleObject is Altaxo.Graph.Plot.Styles.IGPlotStyle style)
        {
          foreach (var columnInfo in style.GetAdditionallyUsedColumns() ?? EmptyColumnInfoEnumeration)
          {
            if ((columnInfo.Column is DataColumn dataColumn1 && predicate((dataColumn1, columnInfo.ColumnName))) || (columnInfo.Column is null && !string.IsNullOrEmpty(columnInfo.ColumnName)))
            {
              yield return (style.GetType().Name, columnInfo.ColumnLabel, columnInfo.Column, columnInfo.ColumnName, (col, tbl, grp) => columnInfo.ColumnSetAction(col));
            }
            else if ((columnInfo.Column is TransformedReadableColumn transColumn && transColumn.UnderlyingReadableColumn is DataColumn dataColumn2 && predicate((dataColumn2, columnInfo.ColumnName))))
            {
              yield return (style.GetType().Name, columnInfo.ColumnLabel, columnInfo.Column, columnInfo.ColumnName, (col, tbl, grp) => columnInfo.ColumnSetAction(col is null ? col : transColumn.WithUnderlyingReadableColumn(col)));
            }
            else if (columnInfo.Column is null)
            {
              yield return (style.GetType().Name, columnInfo.ColumnLabel, columnInfo.Column, columnInfo.ColumnName, (col, tbl, grp) => columnInfo.ColumnSetAction(col));
            }
          }
        }
      }
    }

    /// <summary>
    /// Enumerates all data columns used by an <see cref="Altaxo.Graph.Plot.IGPlotItem"/>
    /// </summary>
    /// <param name="data">The <see cref="IColumnPlotData"/> item.</param>
    /// <param name="predicate">The predicate. Should return true if the data column provided in the argument should be enumerated.</param>
    /// <returns>Enumeration of all data columns used by the plot item. Note that also the broken data columns will be reported with name and label (but of course the tuple member DataColumn is null in this case).</returns>
    public static IEnumerable<
      (
      string ColumnGroup,
      string ColumnLabel,
      IReadableColumn? Column,
      string? ColumnName,
      Action<IReadableColumn?, DataTable, int> ColumnSetAction
      )>
      EnumerateAllDataColumnsOfItem(
        IColumnPlotData data,
        Predicate<(DataColumn? Column, string? ColumnName)> predicate)
    {
      {
        // first the row selection(s)
        foreach (var columnInfo in data.DataRowSelection.GetAdditionallyUsedColumns())
        {
          if ((columnInfo.Column is DataColumn dataColumn1 && predicate((dataColumn1, columnInfo.ColumnName))) || (columnInfo.Column is null && !string.IsNullOrEmpty(columnInfo.ColumnName)))
          {
            yield return (nameof(data.DataRowSelection), columnInfo.ColumnLabel, columnInfo.Column, columnInfo.ColumnName, (col, tbl, grp) => columnInfo.SetColumnAction(col ?? new IndexerColumn()));
          }
          else if ((columnInfo.Column is TransformedReadableColumn transColumn && transColumn.UnderlyingReadableColumn is DataColumn dataColumn2 && predicate((dataColumn2, columnInfo.ColumnName))))
          {
            yield return (nameof(data.DataRowSelection), columnInfo.ColumnLabel, columnInfo.Column, columnInfo.ColumnName, (col, tbl, grp) => columnInfo.SetColumnAction(col is null ? (IReadableColumn)new IndexerColumn() : transColumn.WithUnderlyingReadableColumn(col)));
          }
        }

        // now the data itself
        foreach (var t in data.GetAdditionallyUsedColumns())
        {
          foreach (var columnInfo in t.ColumnInfos)
          {
            if ((columnInfo.Column is DataColumn dataColumn1 && predicate((dataColumn1, columnInfo.ColumnName))) || (columnInfo.Column is null && !string.IsNullOrEmpty(columnInfo.ColumnName)))
            {
              yield return (t.NameOfColumnGroup, columnInfo.ColumnLabel, columnInfo.Column, columnInfo.ColumnName, columnInfo.SetColumnAction);
            }
            else if ((columnInfo.Column is TransformedReadableColumn transColumn && transColumn.UnderlyingReadableColumn is DataColumn dataColumn2 && predicate((dataColumn2, columnInfo.ColumnName))))
            {
              yield return (t.NameOfColumnGroup, columnInfo.ColumnLabel, columnInfo.Column, columnInfo.ColumnName, (col, tbl, grp) => columnInfo.SetColumnAction(col is null ? col : transColumn.WithUnderlyingReadableColumn(col), tbl, grp));
            }
          }
        }
      }
    }

    /// <summary>
    /// Determines whether it is possible to change the underlying table for the specified plot items.
    /// The table can be changed if all plot items have exactly the same underlying table.
    /// </summary>
    /// <param name="dataItems">The plot data items.</param>
    /// <returns>
    ///   <c>true</c> if this instance [can change table for plot items] the specified plot items; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanChangeTableForIColumnPlotDataItems(IEnumerable<IColumnPlotData> dataItems)
    {
      var firstSelectedItem = dataItems.FirstOrDefault();
      if (firstSelectedItem is null)
        return false;

      var table = firstSelectedItem?.DataTable;
      if (table is null)
        return false;

      foreach (var node in dataItems)
      {
        if (!object.ReferenceEquals(table, node?.DataTable))
          return false;
      }

      return true;
    }

    /// <summary>
    /// Gets an empty column information enumeration.
    /// </summary>
    /// <value>
    /// The empty column information enumeration.
    /// </value>
    private static IEnumerable<(
        string ColumnLabel, // Column label
        IReadableColumn? Column, // the column as it was at the time of this call
        string? ColumnName, // the name of the column (last part of the column proxies document path)
        Action<IReadableColumn?> ColumnSetAction // action to set the column during Apply of the controller
        )> EmptyColumnInfoEnumeration
    {
      get
      {
        return Enumerable.Empty<(
      string ColumnLabel, // Column label
      IReadableColumn? Column, // the column as it was at the time of this call
      string? ColumnName, // the name of the column (last part of the column proxies document path)
      Action<IReadableColumn?> ColumnSetAction // action to set the column during Apply of the controller
      )>();
      }
    }
  }
}
