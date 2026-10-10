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
using System.Collections.ObjectModel;
using ModelContextProtocol.Server;

namespace Altaxo.Chat
{
  /// <summary>
  /// Provides an immutable catalog of explicitly contributed Mcp tools.
  /// </summary>
  public sealed class McpToolRegistry
  {
    private readonly IReadOnlyList<McpServerTool> _tools;

    /// <summary>
    /// Initializes a new instance of the <see cref="McpToolRegistry"/> class.
    /// </summary>
    /// <param name="providers">The built-in and add-in tool providers.</param>
    /// <param name="services">The application services passed to each provider.</param>
    /// <exception cref="ArgumentNullException">A required argument is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">A provider or tool is invalid, or two tools have the same name.</exception>
    public McpToolRegistry(IEnumerable<IMcpToolProvider> providers, IServiceProvider services)
    {
      ArgumentNullException.ThrowIfNull(providers);
      ArgumentNullException.ThrowIfNull(services);

      var tools = new List<McpServerTool>();
      var names = new HashSet<string>(StringComparer.Ordinal);
      var providerIds = new HashSet<string>(StringComparer.Ordinal);

      foreach (var provider in providers)
      {
        if (provider is null)
          throw new ArgumentException("A tool provider cannot be null.", nameof(providers));

        if (string.IsNullOrWhiteSpace(provider.Id))
          throw new ArgumentException("A tool provider must have a non-empty identifier.", nameof(providers));
        if (!providerIds.Add(provider.Id))
          throw new ArgumentException($"Mcp tool provider identifier '{provider.Id}' is contributed more than once.", nameof(providers));

        var providerTools = provider.CreateTools(services) ??
          throw new ArgumentException($"Tool provider '{provider.Id}' returned null.", nameof(providers));

        foreach (var tool in providerTools)
        {
          if (tool is null)
            throw new ArgumentException($"Tool provider '{provider.Id}' returned a null tool.", nameof(providers));

          var name = tool.ProtocolTool.Name;
          if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"Tool provider '{provider.Id}' returned a tool with no name.", nameof(providers));
          if (!name.StartsWith(provider.Id + ".", StringComparison.Ordinal))
            throw new ArgumentException($"Tool name '{name}' must start with provider identifier '{provider.Id}'.", nameof(providers));

          if (!names.Add(name))
            throw new ArgumentException($"Mcp tool name '{name}' is contributed more than once.", nameof(providers));

          tools.Add(tool);
        }
      }

      _tools = new ReadOnlyCollection<McpServerTool>(tools);
    }

    /// <summary>
    /// Gets the tools contributed when this registry was created.
    /// </summary>
    public IReadOnlyList<McpServerTool> Tools => _tools;
  }
}
