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
using System.Text.Json;

namespace Altaxo.Chat
{
  /// <summary>
  /// Provides the basic tools from the Altaxo data namespace, like creating tables, creating data columns and reading and writing data.
  /// </summary>
  public sealed class AltaxoProjectCoreToolProvider : AltaxoToolProviderBase
  {
    /// <inheritdoc/>
    public override string Id => "altaxo.project.core";


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


    /// <summary>
    /// Represents information about a project item (table, graph, or notes).
    /// </summary>
    /// <param name="Name">The name of the project item.</param>
    /// <param name="Type">The type of the project item (table, graph, or notes).</param>
    public record ItemInfo(string Name, string Type);

    [Description("Lists the project items in the current project. For each item, the name and corresponding type (table, graph, textdocument) is reported. The name can contain folder parts, which are separated by backslashes. Call this method first to find the full names of project items before using them with other tools."), ReadOnly(true)]
    private string ListProjectItems(
      [Description("Optional filter: 'table', 'graph', 'textdocument', or omit for all")] string? type = null)
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
      if (string.IsNullOrEmpty(type) || type == "textdocument" || type == "all")
      {
        foreach (var item in Current.Project.TextDocumentCollection)
          list.Add(new ItemInfo(item.Name, "textdocument"));
      }

      return JsonSerializer.Serialize(list);
    }
  }
}
