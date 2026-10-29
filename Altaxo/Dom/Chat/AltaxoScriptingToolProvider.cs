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
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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

    /// <summary>Finds public Altaxo API types by name or by matching terms in their documentation.</summary>
    /// <param name="query">Space-separated terms; every term must match.</param>
    /// <param name="maxResults">The maximum number of matching types to return.</param>
    /// <param name="assemblyPrefix">The assembly-name prefix to search, or an empty string to search all loaded assemblies.</param>
    /// <returns>A JSON description of matching types or an error.</returns>
    [Description("Finds public types of the Altaxo API by name or by words in their documentation. All terms must match. Use this first when you don't know the class name."), ReadOnly(true)]
    public string FindTypes(
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

    /// <summary>Describes an Altaxo API type, its base types, documentation, and selected members.</summary>
    /// <param name="typeName">The full or simple name of the type to describe.</param>
    /// <param name="memberFilter">Optional text used to filter member names.</param>
    /// <param name="includeInherited">Whether members inherited from base types are included.</param>
    /// <param name="includeNonPublic">Whether private and internal members are included.</param>
    /// <param name="maxMembers">The maximum number of members to return.</param>
    /// <returns>A JSON description of the type or an error.</returns>
    [Description("Describes a type of the Altaxo API: base types, documentation, and its members with signatures. With memberFilter only matching members are shown, with full documentation."), ReadOnly(true)]
    public string DescribeType(
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

    /// <summary>Gets the non-abstract subclasses of a specified type in the Altaxo API.</summary>
    /// <param name="typeName">The full (preferred) or simple name of the type whose subclasses are requested.</param>
    /// <param name="maxResults">The maximum number of type names to return.</param>
    /// <returns>A JSON description of the matching types or an error.</returns>
    [Description("Gets the non-abstract subclasses of a type in the Altaxo API. For an interface, the non-abstract implementing types are returned."), ReadOnly(true)]
    public string GetNonabstractSubclassesOfType(
      [Description("Full name (preferred) or simple name of the type.")] string typeName,
      [Description("Maximum number of type names returned.")] int maxResults = 200)
    {
      if (string.IsNullOrWhiteSpace(typeName))
        return Error("Parameter 'typeName' must not be empty.");

      try
      {
        // Same resolution as describe_type: full name or simple name, across the loaded Altaxo assemblies.
        var type = ApiDiscovery.ResolveType(typeName);

        var subclasses = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(type)
                         ?? Array.Empty<Type>();

        var names = subclasses
          .Select(t => t.FullName ?? t.Name)
          .Distinct()
          .OrderBy(n => n, StringComparer.Ordinal)
          .ToArray();

        var shown = names.Length > maxResults ? names.Take(Math.Max(0, maxResults)).ToArray() : names;

        return JsonSerializer.Serialize(new
        {
          Type = type.FullName,
          Count = names.Length,
          Truncated = shown.Length < names.Length,
          NonabstractSubClasses = shown,
          Message = $"Found {names.Length} non-abstract subclasses of type '{type.FullName}'."
        }, _json);
      }
      catch (DiscoveryException ex)
      {
        return Error(ex.Message);
      }
      catch (Exception ex)
      {
        return Error($"Could not determine the subclasses of '{typeName}': {ex.Message}");
      }
    }

    /// <summary>Defines the maximum number of characters included in a result string.</summary>
    private const int MaxChars = 20_000; // protects the model's context

    /// <summary>Provides compact JSON serialization settings for script results.</summary>
    private static readonly JsonSerializerOptions _json = new() { WriteIndented = false };

    /// <summary>Lazily creates the Roslyn runner used to execute scripts.</summary>
    private readonly Lazy<ScriptRunner> _runner = new(() => new ScriptRunner());

    /// <summary>Compiles and executes a C# script in the running Altaxo process.</summary>
    /// <param name="code">The C# script source to execute.</param>
    /// <param name="timeoutSeconds">The execution timeout in seconds, from 1 to 300.</param>
    /// <param name="runOnGuiThread">Whether the script should run on the GUI thread.</param>
    /// <param name="mirrorToConsole">Whether script output should also be written to the Altaxo console.</param>
    /// <param name="cancellationToken">A token that can cancel the request.</param>
    /// <returns>A JSON representation of the script result.</returns>
    // Resulting tool name: "altaxo.scripting.execution.execute_script"
    [Description(
      "Compiles and runs a C# script inside the running Altaxo process (full trust, no sandbox). " +
      "Namespace 'Altaxo' is imported, so 'Current.Project', 'Current.ProjectService' etc. are available. " +
      "Use Print(x) for text output; the value of the last expression is returned. A timeout cannot stop code that ignores cancellation. " +
      "Use the API discovery tools (find_types, describe_type) first to check member names.")]
    [ReadOnly(false)]
    public async Task<string> ExecuteScript(
      [Description("C# script source (Roslyn scripting, top-level statements, no class needed).")]
      string code,
      [Description("Timeout in seconds (1-300).")]
      int timeoutSeconds = 30,
      [Description("Run on the GUI thread. Required for project/GUI changes; a runaway loop then freezes Altaxo.")]
      bool runOnGuiThread = true,
      [Description("Additionally write Print output to the Altaxo console window.")]
      bool mirrorToConsole = false,
      CancellationToken cancellationToken = default)
    {
      if (string.IsNullOrWhiteSpace(code))
        return Error("Parameter 'code' must not be empty.");

      try
      {
        var r = await _runner.Value.RunAsync(
          code,
          TimeSpan.FromSeconds(Math.Clamp(timeoutSeconds, 1, 300)),
          runOnGuiThread,
          mirrorToConsole,
          cancellationToken);

        return JsonSerializer.Serialize(new
        {
          success = r.Success,
          returnValue = Truncate(r.ReturnValue),
          returnType = r.ReturnType,
          output = Truncate(r.Output),
          diagnostics = r.Diagnostics, // line/column are 1-based
          exception = Truncate(r.Exception),
          durationMs = (int)r.Duration.TotalMilliseconds
        }, _json);
      }
      catch (OperationCanceledException)
      {
        return Error("Script execution was cancelled.");
      }
      catch (Exception ex)
      {
        return Error($"Script execution failed: {ex.Message}");
      }
    }

    /// <summary>Limits a result string to the maximum size exposed to the model.</summary>
    /// <param name="s">The string to truncate, or <see langword="null"/>.</param>
    /// <returns>The original string when within the limit, otherwise a truncated string with a length marker.</returns>
    private static string? Truncate(string? s) =>
      s is { Length: > MaxChars } ? s[..MaxChars] + $"\n… [truncated, {s.Length} chars total]" : s;

  }
}
