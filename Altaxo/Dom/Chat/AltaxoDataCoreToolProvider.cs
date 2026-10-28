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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using Altaxo.Data;

namespace Altaxo.Chat
{
  /// <summary>
  /// Provides the basic tools from the Altaxo data namespace, like creating tables, creating data columns and reading and writing data.
  /// </summary>
  public sealed class AltaxoDataCoreToolProvider : AltaxoToolProviderBase
  {
    /// <inheritdoc/>
    public override string Id => "altaxo.data.core";


    [Description("Creates a new empty data table in the current project and opens its worksheet."), ReadOnly(false)]
    private string CreateDataTable(
      [Description("Optional: name of the data table to create. Folder names are separated by backslashes, not by forward slashes.")] string? tableName = null
      )
    {
      var name = tableName?.Trim();
      bool nameAlreadyExisted = false;

      if (!string.IsNullOrWhiteSpace(name))
      {
        if (Current.Project.DataTableCollection.Contains(name))
        {
          name = Current.Project.DataTableCollection.FindNewItemName(name);
          nameAlreadyExisted = true;
        }
      }
      else
      {
        name = Current.Project.DataTableCollection.FindNewItemName();
      }

      var table = new Altaxo.Data.DataTable() { Name = name };
      Current.Project.DataTableCollection.Add(table);
      var worksheet = Altaxo.Current.ProjectService.OpenOrCreateWorksheetForTable(table);
      return JsonSerializer.Serialize(new
      {
        Created = true,
        TableName = table.Name,
        Message = $"Created a new data table '{table.Name}' and opened its worksheet.{(nameAlreadyExisted ? " A table with the specified name already existed, so a new name was generated." : "")}"
      });
    }

    [Description("Lists all data columns of a data table."), ReadOnly(true)]
    private string ListColumnsOfTable(
      [Description("Name of the data table to list columns for.")] string tableName)
    {
      if (Current.Project.DataTableCollection.TryGetValue(tableName, out var table))
      {
        var columns = new List<object>();

        for (int c = 0; c < table.DataColumnCount; c++)
        {
          var column = table[c];
          columns.Add(new
          {
            Name = column.Name,
            Type = column.GetType().Name,
            Index = c,
            Group = table.DataColumns.GetColumnGroup(column),
            Kind = table.DataColumns.GetColumnKind(column),
          });
        }

        return JsonSerializer.Serialize(columns);
      }
      else
      {
        return Error($"Data table '{tableName}' not found in the current project.");
      }
    }

    [Description("Adds a new column to a data table."), ReadOnly(false)]
    private string AddColumnToTable(
      [Description("Name of the data table to add columns to.")] string tableName,
      [Description("Name of the column to add.")] string columnName,
      [Description("Column type: either 'double', 'datetime' or 'text'.")] string columnType
      )
    {
      if (Current.Project.DataTableCollection.TryGetValue(tableName, out var table))
      {
        Type newColumnType = columnType.ToLowerInvariant() switch
        {
          "double" => typeof(DoubleColumn),
          "datetime" => typeof(DateTimeColumn),
          "text" => typeof(TextColumn),
          _ => typeof(TextColumn),
        };

        var newColumn = table.DataColumns.EnsureExistence(columnName, newColumnType, ColumnKind.V, 0);

        return JsonSerializer.Serialize(new
        {
          NameOfAddedColumn = table.DataColumns.GetColumnName(newColumn),
          TypeOfAddedColumn = newColumn.GetType().Name,
          Message = $"Added column '{table.DataColumns.GetColumnName(newColumn)}' to data table '{tableName}'."
        });
      }
      else
      {
        return Error($"Data table '{tableName}' not found in the current project.");
      }
    }

    [Description("Removes columns from a data table."), ReadOnly(false)]
    private string RemoveColumnsFromTable(
     [Description("Name of the data table to remove columns from.")] string tableName,
     [Description("List of column names to remove.")] IEnumerable<string> columnNames
     )
    {
      if (Current.Project.DataTableCollection.TryGetValue(tableName, out var table))
      {
        var columnsRemoved = new List<string>();
        var columnsNotFound = new List<string>();
        foreach (var columnName in columnNames)
        {
          if (table.DataColumns.TryGetColumn(columnName) is { } column)
          {
            table.DataColumns.RemoveColumn(column);
            columnsRemoved.Add(columnName);
          }
          else
          {
            columnsNotFound.Add(columnName);
          }
        }

        return JsonSerializer.Serialize(new
        {
          ColumnsRemoved = columnsRemoved,
          ColumnsNotFound = columnsNotFound,
          Message = $"Removed {columnsRemoved.Count} columns from data table '{tableName}'."
        });
      }
      else
      {
        return Error($"Data table '{tableName}' not found in the current project.");
      }
    }

    [Description("Gets the row count of a data table."), ReadOnly(true)]
    private string GetRowCountOfTable(
     [Description("Name of the data table.")] string tableName,
     [Description("Optional: column group number (default value is 0)")] int groupNumber = 0
     )
    {
      if (Current.Project.DataTableCollection.TryGetValue(tableName, out var table))
      {
        int rowCount = 0;

        for (int c = 0; c < table.DataColumnCount; c++)
        {
          var column = table[c];
          var columnGroup = table.DataColumns.GetColumnGroup(column);
          if (columnGroup != groupNumber)
            continue;
          rowCount = Math.Max(rowCount, column.Count);
        }

        return JsonSerializer.Serialize(new
        {
          RowCount = rowCount,
          Message = $"Data table '{tableName}' has {rowCount} rows in column group {groupNumber}."
        });
      }
      else
      {
        return Error($"Data table '{tableName}' not found in the current project.");
      }
    }

    private record NameValue(string Name, string Value);

    [Description("Gets the contents of a row in a data table."), ReadOnly(true)]
    private string GetRowFromTable(
      [Description("Name of the data table.")] string tableName,
      [Description("Zero-based row index.")] int rowIndex,
      [Description("Optional: column group number (default value is 0)")] int groupNumber = 0
      )
    {
      if (Current.Project.DataTableCollection.TryGetValue(tableName, out var table))
      {
        var list = new List<NameValue>();

        if (rowIndex < 0 || rowIndex >= table.DataRowCount)
          return Error($"Row index {rowIndex} is out of range [0, {table.DataRowCount}) for data table '{tableName}'.");

        for (int c = 0; c < table.DataColumnCount; c++)
        {
          var column = table[c];
          var columnGroup = table.DataColumns.GetColumnGroup(column);
          if (columnGroup != groupNumber)
            continue;
          var value = column[rowIndex].ToString() ?? string.Empty;
          list.Add(new NameValue(column.Name, value));
        }

        return JsonSerializer.Serialize(list);
      }
      else
      {
        return Error($"Data table '{tableName}' not found in the current project.");
      }
    }


    [Description("Reads the contents of a column of a data table. Returns the values as strings in invariant culture (decimal point '.', ISO 8601 for dates). Empty cells are returned as empty strings."),
 ReadOnly(true)]
    private string GetColumnValues(
  [Description("Name of the data table.")] string tableName,
  [Description("Name of the column.")] string columnName,
  [Description("Zero-based start row index. Use 0 to read from the beginning.")] int startRowIndex = 0,
  [Description("Maximum number of values to return. Use -1 to read up to the end of the column.")] int count = -1)
    {
      const int MaxCount = 10000; // guard against huge responses

      if (startRowIndex < 0)
        return Error($"Start row index {startRowIndex} is less than zero for data table '{tableName}'.");
      if (count < -1)
        return Error($"Count {count} is invalid; use -1 to read to the end.");

      if (!Current.Project.DataTableCollection.TryGetValue(tableName, out var table))
        return Error($"Data table '{tableName}' not found in the current project.");

      var column = table.DataColumns.TryGetColumn(columnName);
      if (column is null)
        return Error($"Column '{columnName}' not found in data table '{tableName}'.");

      int rowCount = column.Count;
      int available = Math.Max(0, rowCount - startRowIndex);
      int n = count < 0 ? available : Math.Min(count, available);

      bool truncated = false;
      if (n > MaxCount)
      {
        n = MaxCount;
        truncated = true;
      }

      var values = new string[n];

      if (column is DoubleColumn dc)
      {
        for (int i = 0; i < n; i++)
        {
          double d = dc[startRowIndex + i];
          values[i] = double.IsNaN(d) ? string.Empty : d.ToString("R", CultureInfo.InvariantCulture);
        }
      }
      else if (column is DateTimeColumn dtc)
      {
        for (int i = 0; i < n; i++)
        {
          DateTime dt = dtc[startRowIndex + i];
          values[i] = dt == DateTime.MinValue ? string.Empty : dt.ToString("o", CultureInfo.InvariantCulture);
        }
      }
      else if (column is TextColumn tc)
      {
        for (int i = 0; i < n; i++)
          values[i] = tc[startRowIndex + i] ?? string.Empty;
      }
      else
      {
        return Error($"Column '{columnName}' has unsupported type {column.GetType().Name}.");
      }

      return JsonSerializer.Serialize(new
      {
        TableName = tableName,
        ColumnName = columnName,
        ColumnType = column.GetType().Name,
        StartRowIndex = startRowIndex,
        ReturnedCount = n,
        ColumnLength = rowCount,
        Truncated = truncated,
        Values = values
      });
    }

    [Description("Sets the contents of a column in a data table, starting at the given row. Rows beyond the current end are appended. Empty strings clear a cell."), ReadOnly(false)]
    private string SetColumnValues(
      [Description("Name of the data table.")] string tableName,
      [Description("Name of the column.")] string columnName,
      [Description("Zero-based start row index.")] int startRowIndex,
      [Description("New values for the cells in invariant culture (decimal point '.', ISO 8601 for dates).")] string[] newValues)
    {
      if (newValues is null)
        return Error("No values were provided.");
      if (startRowIndex < 0)
        return Error($"Start row index {startRowIndex} is less than zero for data table '{tableName}'.");

      if (!Current.Project.DataTableCollection.TryGetValue(tableName, out var table))
        return Error($"Data table '{tableName}' not found in the current project.");

      var column = table.DataColumns.TryGetColumn(columnName);
      if (column is null)
        return Error($"Column '{columnName}' not found in data table '{tableName}'.");

      if (column is DoubleColumn dc)
      {
        var values = new double[newValues.Length];
        for (int i = 0; i < values.Length; i++)
        {
          var s = newValues[i];
          if (string.IsNullOrWhiteSpace(s))
            values[i] = double.NaN;
          else if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out values[i]))
            return Error($"Value '{s}' at index {i} is not a valid number.");
        }

        using (var token = table.SuspendGetToken())
        {
          for (int i = 0; i < values.Length; i++)
            dc[startRowIndex + i] = values[i];
        }
      }
      else if (column is DateTimeColumn dtc)
      {
        var values = new DateTime[newValues.Length];
        for (int i = 0; i < values.Length; i++)
        {
          var s = newValues[i];
          if (string.IsNullOrWhiteSpace(s))
            values[i] = DateTime.MinValue;
          else if (!DateTime.TryParse(s, CultureInfo.InvariantCulture,
                     DateTimeStyles.RoundtripKind | DateTimeStyles.AllowWhiteSpaces, out values[i]))
            return Error($"Value '{s}' at index {i} is not a valid date/time.");
        }

        using (var token = table.SuspendGetToken())
        {
          for (int i = 0; i < values.Length; i++)
            dtc[startRowIndex + i] = values[i];
        }
      }
      else if (column is TextColumn tc)
      {
        using (var token = table.SuspendGetToken())
        {
          for (int i = 0; i < newValues.Length; i++)
            tc[startRowIndex + i] = newValues[i];
        }
      }
      else
      {
        return Error($"Column '{columnName}' has unsupported type {column.GetType().Name}.");
      }

      return JsonSerializer.Serialize(new
      {
        Message = $"Set {newValues.Length} values in column[\"{columnName}\"] of data table '{tableName}', rows {startRowIndex} to {startRowIndex + newValues.Length - 1}."
      });
    }
  }
}
