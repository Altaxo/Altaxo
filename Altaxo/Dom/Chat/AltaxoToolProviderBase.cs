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
using System.Reflection;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace Altaxo.Chat
{
  /// <summary>
  /// Base class for Altaxo tool providers that implement the IMcpToolProvider interface.
  /// </summary>
  /// <remarks>
  /// Every method declared by the derived class that is decorated with a <see cref="DescriptionAttribute"/>
  /// becomes an Mcp tool. The name of such a tool is the provider identifier and the snake case name of the
  /// method, joined by an underscore. Characters that are not allowed in Mcp tool names are replaced by
  /// underscores, so a provider identifier like <c>altaxo.data.core</c> contributes a tool named
  /// <c>altaxo_data_core_add_column_to_table</c>.
  /// </remarks>
  public abstract class AltaxoToolProviderBase : IMcpToolProvider
  {
    /// <inheritdoc/>
    public abstract string Id { get; }


    /// <inheritdoc/>
    public IReadOnlyList<McpServerTool> CreateTools(IServiceProvider services)
    {
      ArgumentNullException.ThrowIfNull(services);

      var tools = new List<McpServerTool>();
      foreach (var method in this.GetType().GetMethods(
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
      {
        if (method.GetCustomAttribute<DescriptionAttribute>() is not { } description)
          continue;

        var options = new McpServerToolCreateOptions
        {
          Name = McpToolNames.Combine(Id, JsonNamingPolicy.SnakeCaseLower.ConvertName(method.Name)),
          Description = description.Description,
          Services = services
        };

        if (method.GetCustomAttribute<ReadOnlyAttribute>() is { } readOnly)
          options.ReadOnly = readOnly.IsReadOnly;

        tools.Add(McpServerTool.Create(method, _ => this, options));
      }

      return tools;
    }

    /// <summary>
    /// Returns a JSON object with an "Error" property containing the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message to include in the JSON object.</param>
    /// <returns>A JSON object with an "Error" property containing the specified error message.</returns>
    public string Error(string errorMessage) => JsonSerializer.Serialize(new
    {
      Error = errorMessage
    });
  }
}
