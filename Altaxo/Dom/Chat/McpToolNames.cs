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
using System.Text;
using System.Text.RegularExpressions;

namespace Altaxo.Chat
{
  /// <summary>
  /// Builds and verifies the names of Mcp tools.
  /// </summary>
  /// <remarks>
  /// A Mcp client usually registers a tool under a name derived from the name reported by the server, e.g. by
  /// prefixing it with the server name. Many clients reject names containing characters other than ASCII letters,
  /// digits, underscores and hyphens (they match the name against <c>^[a-zA-Z0-9_-]+$</c>) and limit the length of
  /// the resulting name. Altaxo therefore reports tool names that consist of the allowed characters only, even
  /// though the Mcp specification itself does not require this.
  /// </remarks>
  public static partial class McpToolNames
  {
    /// <summary>
    /// Gets the maximum length of a tool name reported to a Mcp client. Clients add further prefixes, so tool
    /// names should be kept well below this limit.
    /// </summary>
    public const int MaxNameLength = 64;

    /// <summary>
    /// Gets the character that separates the tool provider identifier from the tool name.
    /// </summary>
    public const char Separator = '_';

    [GeneratedRegex("^[a-zA-Z0-9_-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex AllowedCharactersRegex();

    /// <summary>
    /// Determines whether the specified name can be used as an Mcp tool name.
    /// </summary>
    /// <param name="name">The name to check. May be null.</param>
    /// <returns>True if the name is non-empty, consists of allowed characters only and is not too long; otherwise false.</returns>
    public static bool IsValidName(string? name)
      => !string.IsNullOrEmpty(name) && name.Length <= MaxNameLength && AllowedCharactersRegex().IsMatch(name);

    /// <summary>
    /// Builds the name of a tool contributed by a tool provider.
    /// </summary>
    /// <param name="providerId">The identifier of the contributing tool provider. It may contain characters that are not allowed in tool names, e.g. dots.</param>
    /// <param name="name">The name of the tool itself, e.g. the snake case name of the method implementing the tool.</param>
    /// <returns>The sanitized provider identifier, followed by <see cref="Separator"/> and the sanitized tool name.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="providerId"/> or <paramref name="name"/> is empty, or the resulting name is longer than <see cref="MaxNameLength"/> characters.
    /// </exception>
    public static string Combine(string providerId, string name)
    {
      ArgumentException.ThrowIfNullOrWhiteSpace(providerId);
      ArgumentException.ThrowIfNullOrWhiteSpace(name);

      var result = Sanitize(providerId) + Separator + Sanitize(name);
      if (result.Length > MaxNameLength)
        throw new ArgumentException($"The Mcp tool name '{result}' is longer than {MaxNameLength} characters. Shorten the tool provider identifier '{providerId}' or the tool name '{name}'.", nameof(name));

      return result;
    }

    /// <summary>
    /// Gets the prefix that every tool name contributed by the specified tool provider must have.
    /// </summary>
    /// <param name="providerId">The identifier of the contributing tool provider.</param>
    /// <returns>The sanitized provider identifier followed by <see cref="Separator"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="providerId"/> is empty.</exception>
    public static string GetPrefix(string providerId)
    {
      ArgumentException.ThrowIfNullOrWhiteSpace(providerId);
      return Sanitize(providerId) + Separator;
    }

    /// <summary>
    /// Replaces every character that is not allowed in an Mcp tool name by an underscore.
    /// </summary>
    /// <param name="name">The name to sanitize.</param>
    /// <returns>The sanitized name.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
    public static string Sanitize(string name)
    {
      ArgumentNullException.ThrowIfNull(name);

      var builder = new StringBuilder(name.Length);
      foreach (var c in name)
        builder.Append(IsAllowed(c) ? c : Separator);

      return builder.ToString();
    }

    private static bool IsAllowed(char c) =>
      c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_' or '-';
  }
}
