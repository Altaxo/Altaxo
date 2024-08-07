﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Analyse a single line of text with regard to different separation strategies, and store
  /// the result in a dictionary that contains entries for the separation strategy and the resulting structure.
  /// </summary>
  public class AsciiLineAnalysis
  {
    /// <summary>
    /// The dictionary containing entries for separation strategies and the corresponding resulting line structure.
    /// </summary>
    private Dictionary<AsciiLineAnalysisOption, AsciiLineComposition> _structureForSeparation;

    public AsciiLineComposition this[AsciiLineAnalysisOption separationStrategy]
    {
      get
      {
        return _structureForSeparation[separationStrategy];
      }
    }

    /// <summary>
    /// Performs the analysis of a line with regard to different separation stragegies.
    /// </summary>
    /// <param name="nLine">Line number.</param>
    /// <param name="sLine">The line to analyse.</param>
    /// <param name="separationStrategies">List of separation stragegies to test with the provided text line.</param>
    public AsciiLineAnalysis(int nLine, string sLine, List<AsciiLineAnalysisOption> separationStrategies)
    {
      _structureForSeparation = new Dictionary<AsciiLineAnalysisOption, AsciiLineComposition>();

      var tokenDictionary = new Dictionary<IAsciiSeparationStrategy, List<string>>();

      foreach (AsciiLineAnalysisOption separation in separationStrategies)
      {
        if (!tokenDictionary.TryGetValue(separation.SeparationStrategy, out var tokens))
        {
          tokens = new List<string>(separation.SeparationStrategy.GetTokens(sLine));
          tokenDictionary.Add(separation.SeparationStrategy, tokens);
        }

        _structureForSeparation.Add(separation, GetStructure(nLine, tokens, separation.NumberFormat, separation.DateTimeFormat));
      }
    }

    /// <summary>
    /// Analyse the provided line of text with regard to one separation stragegy and returns the resulting structure.
    /// </summary>
    /// <param name="nLine">Line number.</param>
    /// <param name="tokens">The content of the line, already separated into tokens.</param>
    /// <param name="numberFormat">The number culture to use.</param>
    /// <param name="dateTimeFormat">The DateTime format culture to use.</param>
    /// <returns>The resulting structure.</returns>
    public static AsciiLineComposition GetStructure(int nLine, IEnumerable<string> tokens, System.Globalization.CultureInfo numberFormat, System.Globalization.CultureInfo dateTimeFormat)
    {
      var tabStruc = ImmutableArray.CreateBuilder<AsciiColumnInfo>();

      foreach (string substring in tokens)
      {
        if (string.IsNullOrEmpty(substring)) // just this char is a tab, so nothing is between the last and this
        {
          tabStruc.Add(AsciiColumnInfo.DBNull);
        }
        else if (IsNumeric(substring, numberFormat))
        {
          if (IsIntegral(substring, numberFormat))
          {
            tabStruc.Add(AsciiColumnInfo.Integer);
          }
          else if (IsFloat(substring, numberFormat))
          {
            if (substring.Contains(numberFormat.NumberFormat.NumberDecimalSeparator))
              tabStruc.Add(AsciiColumnInfo.FloatWithDecimalSeparator);
            else
              tabStruc.Add(AsciiColumnInfo.FloatWithoutDecimalSeparator);
          }
          else
          {
            tabStruc.Add(AsciiColumnInfo.GeneralNumber);
          }
        }
        else if (IsDateTime(substring, dateTimeFormat))
        {
          tabStruc.Add(AsciiColumnInfo.DateTime);
        }
        else
        {
          tabStruc.Add(AsciiColumnInfo.Text);
        }
      } // end for

      return new AsciiLineComposition(tabStruc.ToImmutable());
    }

    /// <summary>
    /// Problem: the line: '23,2014 2,3' is not parsed as expected in the two numbers 23,2014 and 2,3 (German culture), but instead in the number 23, then into a DateTime (with year=2014 and month=2), and a number 3.
    /// In order to avoid that for instance "2014 2" is parsed to a valid DateTime, we have two possibilities (i) use DateTime.TryParseExact, or (ii) excluded certain patterns with a regular expression.
    /// While the first choice is generally slow (we must use TryParseExcact with all possible formats for a given culture, it takes 17 sec for 1 Mio calls), the second choice is faster. It is sufficient to
    /// exclude patterns which contains digits first, then one or more whitespaces, optionally a minus sign, and then digits again.
    /// </summary>
    private static System.Text.RegularExpressions.Regex _dateTimeExclusingPattern = new System.Text.RegularExpressions.Regex(@"^\d+\s+-*\d+$");

    /// <summary>
    /// Tests wether or not the string s is a date/time string.
    /// </summary>
    /// <param name="s">The string to test.</param>
    /// <param name="dateTimeFormat">The culture info used to test if <paramref name="s"/> is a date/time string.</param>
    /// <returns>True if the string can be parsed to a date/time value.</returns>
    public static bool IsDateTime(string s, System.Globalization.CultureInfo dateTimeFormat)
    {
      return DateTime.TryParse(s, dateTimeFormat.DateTimeFormat, System.Globalization.DateTimeStyles.None, out var result) && !_dateTimeExclusingPattern.Match(s).Success;
    }

    /// <summary>
    /// Tests if the string <c>s</c> is numeric. This is a very generic test here. We accept dots and commas as decimal separators, because the decimal separator statistics
    /// is made afterwards.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="numberFormat">Culture info used to decide if <paramref name="s"/> is a number.</param>
    /// <returns></returns>
    public static bool IsNumeric(string s, System.Globalization.CultureInfo numberFormat)
    {
      return double.TryParse(s, System.Globalization.NumberStyles.Any, numberFormat.NumberFormat, out var result);
    }

    /// <summary>
    /// Tests if the string <c>s</c> is an integral numeric value.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="numberFormat">Culture info used to decide if <paramref name="s"/> is an integer.</param>
    /// <returns></returns>
    public static bool IsIntegral(string s, System.Globalization.CultureInfo numberFormat)
    {
      return long.TryParse(s, System.Globalization.NumberStyles.Integer, numberFormat.NumberFormat, out var result);
    }

    /// <summary>
    /// Tests if the string <c>s</c> is an numeric value with <see cref="P:System.Globalization.NumberStyles.Float"/>.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="numberFormat">Culture info used to decide if <paramref name="s"/> is a floating point number.</param>
    /// <returns></returns>
    public static bool IsFloat(string s, System.Globalization.CultureInfo numberFormat)
    {
      return double.TryParse(s, System.Globalization.NumberStyles.Float, numberFormat.NumberFormat, out var result);
    }
  } // end class
}
