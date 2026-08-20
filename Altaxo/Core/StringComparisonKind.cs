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

namespace Altaxo
{
  /// <summary>
  /// Used to compare strings in some matcher classes.
  /// </summary>
  public enum StringComparisonKind
  {
    /// <summary>Is the test string and the match string equal?</summary>
    Equality,


    /// <summary>Is the test string and the match string not equal?</summary>
    Inequality,

    /// <summary>Is the match string contained in the string to test?</summary>
    Contains,

    /// <summary>
    /// Does the test string starts with the match string?
    /// </summary>
    StartsWith,

    /// <summary>
    /// Does the test string ends with the match string?
    /// </summary>
    EndsWith,
  }

  /// <summary>
  /// Provides extension methods for string comparison based on the StringComparisonKind enum.
  /// </summary>
  public static class StringComparisonExtensions
  {
    extension(string? value)
    {
      /// <summary>
      /// Matches two strings, depending on the provided kind of comparison
      /// </summary>
      /// <param name="matchString">The match string.</param>
      /// <param name="kind">The kind of comparison.</param>
      /// <param name="stringComparison">The details of comparison.</param>
      /// <returns>True if the strings match according to the specified kind and comparison details; otherwise, false.</returns>
      public bool Matches(
          string? matchString,
          StringComparisonKind kind,
          StringComparison stringComparison = StringComparison.Ordinal)
      {
        if (value is null || matchString is null)
        {
          return kind == StringComparisonKind.Inequality
              ? !string.Equals(value, matchString, stringComparison)
              : kind == StringComparisonKind.Equality && string.Equals(value, matchString, stringComparison);
        }

        return kind switch
        {
          StringComparisonKind.Equality => value.Equals(matchString, stringComparison),
          StringComparisonKind.Inequality => !value.Equals(matchString, stringComparison),
          StringComparisonKind.Contains => value.Contains(matchString, stringComparison),
          StringComparisonKind.StartsWith => value.StartsWith(matchString, stringComparison),
          StringComparisonKind.EndsWith => value.EndsWith(matchString, stringComparison),
          _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported StringComparisonKind.")
        };
      }
    }
  }
}
