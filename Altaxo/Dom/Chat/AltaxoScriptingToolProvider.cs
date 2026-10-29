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

using System.ComponentModel;
using System.Text.Json;

namespace Altaxo.Chat
{
  /// <summary>
  /// Provides the basic tools from the Altaxo data namespace, like creating tables, creating data columns and reading and writing data.
  /// </summary>
  /// <remarks>Each tool provider must be loaded via the .addin file of Altaxo.</remarks>
  public partial class AltaxoScriptingToolProvider : AltaxoToolProviderBase
  {
    /// <inheritdoc/>
    public override string Id => "altaxo.scripting.core";

    [Description("Finds public types of the Altaxo API by name or by words in their documentation. All terms must match. Use this first when you don't know the class name."), ReadOnly(true)]
    private string FindTypes(
  [Description("Search terms separated by spaces, e.g. 'graph document' or 'plot style'.")] string query,
  [Description("Maximum number of types returned.")] int maxResults = 50,
  [Description("Only assemblies starting with this prefix are searched. Empty = all loaded assemblies.")] string assemblyPrefix = "Altaxo")
    {
      try
      {
        return ApiDiscovery.FindTypes(query, maxResults, assemblyPrefix);
      }
      catch (DiscoveryException ex)
      {
        return Error(ex.Message);
      }
    }

    [Description("Describes a type of the Altaxo API: base types, documentation, and its members with signatures. With memberFilter only matching members are shown, with full documentation."), ReadOnly(true)]
    private string DescribeType(
      [Description("Full name (preferred) or simple name of the type.")] string typeName,
      [Description("Optional: only members whose name contains this text.")] string memberFilter = "",
      [Description("Include members inherited from base types.")] bool includeInherited = false,
      [Description("Include private and internal members.")] bool includeNonPublic = false,
      [Description("Maximum number of members listed.")] int maxMembers = 150)
    {
      try
      {
        return ApiDiscovery.DescribeType(typeName, memberFilter, includeInherited, includeNonPublic, maxMembers);
      }
      catch (DiscoveryException ex)
      {
        return Error(ex.Message);
      }
    }

    [Description("Gets non-abstract subclasses of a type in the Altaxo API."), ReadOnly(true)]
    private string GetNonabstractSubclassesOfType(
     [Description("Full name of the type.")] string typeName)
    {
      try
      {
        var type = System.Type.GetType(typeName, throwOnError: true);
        var subclasses = Altaxo.Main.Services.ReflectionService.GetSubclassesOf(type);

        if (subclasses is null || subclasses.Length == 0)
        {
          return Error($"No non-abstract subclasses of type '{typeName}' found.");
        }

        return JsonSerializer.Serialize(new
        {
          NonabstractSubClasses = subclasses,
          Message = $"Found {subclasses.Length} non-abstract subclasses of type '{typeName}'."
        });

      }
      catch (DiscoveryException ex)
      {
        return Error(ex.Message);
      }
    }
  }
}
