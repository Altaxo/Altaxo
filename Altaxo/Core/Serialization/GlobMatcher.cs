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
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Altaxo.Serialization
{
  /// <summary>
  /// Matches file paths against include and exclude glob patterns.
  /// Glob patterns are similar to Unix shell patterns, supporting wildcards like '*', '**', and '?'.
  /// </summary>
  public class GlobMatcher : Main.IImmutable
  {
    /// <summary>
    /// Gets the glob patterns that define which paths are included.
    /// </summary>
    public ImmutableList<string> PositivePatterns { get; }

    /// <summary>
    /// Gets the glob patterns that define which paths are excluded.
    /// </summary>
    public ImmutableList<string> NegativePatterns { get; }

    private ImmutableList<Regex> _positiveRegexes;

    private ImmutableList<Regex> _negativeRegexes;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobMatcher"/> class.
    /// </summary>
    /// <param name="positivePatterns">The include glob patterns.</param>
    /// <param name="negativePatterns">The exclude glob patterns.</param>
    /// <param name="caseSensitive">If set to <see langword="true"/>, matching is case-sensitive.</param>
    public GlobMatcher(IEnumerable<string> positivePatterns, IEnumerable<string> negativePatterns, bool caseSensitive = false)
    {
      PositivePatterns = positivePatterns.ToImmutableList();
      NegativePatterns = negativePatterns.ToImmutableList();

      // precompile the patterns to regex for performance
      var options = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
      _positiveRegexes = PositivePatterns.Select(p => new Regex(ConvertGlobToRegex(p.Replace('\\', '/')), options | RegexOptions.Compiled)).ToImmutableList();
      _negativeRegexes = NegativePatterns.Select(p => new Regex(ConvertGlobToRegex(p.Replace('\\', '/')), options | RegexOptions.Compiled)).ToImmutableList();
    }

    /// <summary>
    /// Determines whether the specified input path matches the configured include and exclude patterns.
    /// </summary>
    /// <param name="input">The input path to test.</param>
    /// <returns><see langword="true"/> if the path matches an include pattern and no exclude pattern; otherwise, <see langword="false"/>.</returns>
    public bool IsMatch(string input)
    {
      // Pfadtrenner vereinheitlichen
      string normInput = input.Replace('\\', '/');

      // Check negative patterns
      if (_negativeRegexes.Any(regex => regex.IsMatch(normInput)))
      {
        return false;
      }
      else
      {
        // Check positive patterns
        return _positiveRegexes.Any(regex => regex.IsMatch(normInput));
      }
    }

    /// <summary>
    /// Enumerates files that match the configured include and exclude patterns.
    /// </summary>
    /// <returns>A list of matching files.</returns>
    /// <exception cref="InvalidOperationException">Thrown when at least one include pattern is not an absolute path.</exception>
    public List<string> GetMatchingFiles(CancellationToken cancellationToken = default)
    {
      var result = new HashSet<string>();

      // check if all positive patterns are absolute paths
      foreach (var path in PositivePatterns)
      {
        if (!Path.IsPathRooted(path))
        {
          throw new InvalidOperationException("All positive patterns must be absolute paths.");
        }
      }



      foreach (var path in PositivePatterns)
      {
        var idxA = path.IndexOf('*');
        var idxB = path.IndexOf('?');
        var idxMax = Math.Max(idxA, idxB);
        if (idxMax >= 0) // the pattern contains at least one wildcard
        {
          // is it a pattern with wildcards
          var idxMin = Math.Min(idxA, idxB);
          var idx1 = idxMin < 0 ? idxMax : idxMin; // idx1 now is the first occurence of a wildcard
          var subPath = path.Substring(0, idx1);
          var idx2 = Math.Max(subPath.LastIndexOf(Path.DirectorySeparatorChar), subPath.LastIndexOf(Path.AltDirectorySeparatorChar));
          subPath = subPath.Substring(0, idx2); // subpath is the longest directory path before a wildcard occured
          var dirInfo = new DirectoryInfo(subPath);
          if (dirInfo.Exists)
          {
            var enumerationOptions = new EnumerationOptions() { RecurseSubdirectories = true, IgnoreInaccessible = true };

            foreach (var fileInfo in dirInfo.EnumerateFiles("*", enumerationOptions))
            {
              cancellationToken.ThrowIfCancellationRequested();
              if (IsMatch(fileInfo.FullName))
              {
                result.Add(fileInfo.FullName);
              }
            }
          }
        }
        else // it is a simple path, we can match it directly
        {
          var fileInfo = new FileInfo(path);
          if (fileInfo.Exists && IsMatch(fileInfo.FullName))
          {
            result.Add(fileInfo.FullName);
          }
        }
      }

      return result.ToList();
    }

    /// <summary>
    /// Determines whether an input path matches a single glob pattern.
    /// </summary>
    /// <param name="pattern">The glob pattern.</param>
    /// <param name="input">The input path to test.</param>
    /// <param name="caseSensitive">If set to <see langword="true"/>, matching is case-sensitive.</param>
    /// <returns><see langword="true"/> if the input matches the pattern; otherwise, <see langword="false"/>.</returns>
    public static bool IsMatch(string pattern, string input, bool caseSensitive = false)
    {
      // Pfadtrenner vereinheitlichen
      string normPattern = pattern.Replace('\\', '/');
      string normInput = input.Replace('\\', '/');

      string regexPattern = ConvertGlobToRegex(normPattern);
      var options = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

      return Regex.IsMatch(normInput, regexPattern, options);
    }

    /// <summary>
    /// Converts a glob pattern to an equivalent regular expression pattern.
    /// </summary>
    /// <param name="pattern">The glob pattern to convert.</param>
    /// <returns>A regular expression pattern that represents the specified glob.</returns>
    private static string ConvertGlobToRegex(string pattern)
    {
      var sb = new StringBuilder("^");
      int i = 0, len = pattern.Length;

      while (i < len)
      {
        char c = pattern[i];

        if (c == '*' && i + 1 < len && pattern[i + 1] == '*')
        {
          if (i + 2 < len && pattern[i + 2] == '/')
          {
            sb.Append("(?:.*/)?");   // "**/" -> null oder beliebig viele Ordnerebenen
            i += 3;
          }
          else
          {
            sb.Append(".*");         // "**" alleine -> alles, inkl. "/"
            i += 2;
          }
        }
        else if (c == '*')
        {
          sb.Append("[^/]*");          // "*" -> alles außer "/"
          i++;
        }
        else if (c == '?')
        {
          sb.Append("[^/]");           // "?" -> genau ein Zeichen, kein "/"
          i++;
        }
        else if ("\\^$.|+()[]{}".IndexOf(c) >= 0)
        {
          sb.Append('\\').Append(c);   // Regex-Sonderzeichen escapen
          i++;
        }
        else
        {
          sb.Append(c);
          i++;
        }
      }

      sb.Append('$');
      return sb.ToString();
    }


    /// <summary>
    /// Gets the longest common folder path shared by the specified paths.
    /// </summary>
    /// <param name="paths">The paths to analyze.</param>
    /// <param name="caseSensitive">If set to <see langword="true"/>, segment comparison is case-sensitive.</param>
    /// <returns>The longest common folder path, or an empty string when no paths are provided.</returns>
    public static string GetLongestCommonFolder(string[] paths, bool caseSensitive = false)
    {
      if (paths == null || paths.Length == 0)
        return "";

      if (paths.Length == 1)
        return GetDirectoryPart(paths[0]);

      string root = Path.GetPathRoot(paths[0].Replace('/', '\\')) ?? "";

      // Segmente aller Pfade (ohne Root) ermitteln
      var allSegments = new List<string[]>();
      foreach (var path in paths)
      {
        string normalized = path.Replace('/', '\\');
        string pathRoot = Path.GetPathRoot(normalized) ?? "";
        string remainder = normalized.Substring(pathRoot.Length);
        allSegments.Add(remainder.Split('\\', StringSplitOptions.RemoveEmptyEntries));
      }

      var comparer = caseSensitive
          ? StringComparer.Ordinal
          : StringComparer.OrdinalIgnoreCase;

      var commonSegments = new List<string>();
      int minLength = allSegments.Min(s => s.Length);

      for (int i = 0; i < minLength; i++)
      {
        string candidate = allSegments[0][i];

        bool allMatch = allSegments.All(segs => comparer.Equals(segs[i], candidate));
        if (!allMatch)
          break;

        commonSegments.Add(candidate);
      }

      string combined = string.Join("\\", commonSegments);

      if (commonSegments.Count == 0)
        return root.TrimEnd('\\');

      return root.TrimEnd('\\') + "\\" + combined;
    }

    /// <summary>
    /// Gets the directory part of the specified path.
    /// </summary>
    /// <param name="path">The path to inspect.</param>
    /// <returns>The directory part of the path, or the normalized input when no directory can be determined.</returns>
    private static string GetDirectoryPart(string path)
    {
      string normalized = path.Replace('/', '\\');
      string dir = Path.GetDirectoryName(normalized);
      return dir ?? normalized;
    }
  }
}
