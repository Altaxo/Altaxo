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


    [Description("Returns basic Altaxo application and process version information."), ReadOnly(true)]
    private string GetApplicationInfo()
    {
      var version = typeof(AltaxoDataCoreToolProvider).Assembly.GetName().Version?.ToString() ?? "unknown";
      return JsonSerializer.Serialize(new
      {
        Application = "Altaxo",
        Version = version,
        ProcessId = Environment.ProcessId
      });
    }

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

    /// <summary>
    /// Represents information about a project item (table, graph, or notes).
    /// </summary>
    /// <param name="Name">The name of the project item.</param>
    /// <param name="Type">The type of the project item (table, graph, or notes).</param>
    public record ItemInfo(string Name, string Type);

    [Description("Lists the project items (tables, graphs, or notes) in the current project. Call this first to find the name of a project item before using it with other tools."), ReadOnly(true)]
    private string ListProjectItems(
      [Description("Optional filter: 'table', 'graph', 'note', or omit for all")] string? type = null)
    {
      var list = new List<ItemInfo>();

      type = type?.Trim().ToLowerInvariant();

      if (string.IsNullOrEmpty(type) || type == "table" || type == "all")
      {
        foreach (var table in Current.Project.DataTableCollection)
          list.Add(new ItemInfo(table.Name, "table"));
      }
      if (string.IsNullOrEmpty(type) || type == "graph" || type == "all")
      {
        foreach (var item in Current.Project.GraphDocumentCollection)
          list.Add(new ItemInfo(item.Name, "graph"));
        foreach (var item in Current.Project.Graph3DDocumentCollection)
          list.Add(new ItemInfo(item.Name, "graph"));
      }
      if (string.IsNullOrEmpty(type) || type == "note" || type == "all")
      {
        foreach (var item in Current.Project.TextDocumentCollection)
          list.Add(new ItemInfo(item.Name, "note"));
      }

      return JsonSerializer.Serialize(list);
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

    [Description("Sets the contents of a cell in a data table."), ReadOnly(true)]
    private string SetContentsOfCellInTable(
     [Description("Name of the data table.")] string tableName,
     [Description("Name of the column.")] string columnName,
     [Description("Zero-based row index.")] int rowIndex,
     [Description("New value for the cell in invariant culture.")] string newValue
    )
    {
      if (Current.Project.DataTableCollection.TryGetValue(tableName, out var table))
      {
        var list = new List<NameValue>();

        if (rowIndex < 0)
          return Error($"Row index {rowIndex} is less than zero for data table '{tableName}'.");

        var column = table.DataColumns.TryGetColumn(columnName);
        if (column is null)
          return Error($"Column '{columnName}' not found in data table '{tableName}'.");

        var oldContent = FormattableString.Invariant($"{column[rowIndex]}");

        if (column is DoubleColumn dc)
        {
          var value = double.TryParse(newValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : double.NaN;
          column[rowIndex] = value;
        }
        else if (column is DateTimeColumn dtc)
        {
          var value = DateTime.TryParse(newValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) ? dt : DateTime.MinValue;
          column[rowIndex] = value;
        }
        else
        {
          column[rowIndex] = newValue;
        }

        var newContent = FormattableString.Invariant($"{column[rowIndex]}");

        return JsonSerializer.Serialize(new
        {
          OldContentOfCell = oldContent,
          NewContentOfCell = newContent,
          Message = $"Set content of column[\"{columnName}\"][{rowIndex}] in data table '{tableName}' to {newContent}."
        });
      }
      else
      {
        return Error($"Data table '{tableName}' not found in the current project.");
      }
    }
  }
}
