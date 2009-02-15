#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion


using System;
using System.Collections.Generic;

namespace Altaxo.Serialization.Ascii
{

  /// <summary>
  /// Analyse a single line of text with regard to different separation strategies, and store
  /// the result in a dictionary that contains entries for the separation strategy and the resulting structure.
  /// </summary>
  public class AsciiLineAnalysis
  {
    /// <summary>
    /// The dictionary containing entries for separation stragegies and the corresponding resulting line structure.
    /// </summary>
    public Dictionary<IAsciiSeparationStrategy,AsciiLineStructure> Structures;

    /// <summary>
    /// Performs the analysis of a line with regard to different separation stragegies.
    /// </summary>
    /// <param name="nLine">Line number.</param>
    /// <param name="sLine">The line to analyse.</param>
    /// <param name="separationStrategies">List of separation stragegies to test with the provided text line.</param>
    public AsciiLineAnalysis(int nLine,string sLine, List<IAsciiSeparationStrategy> separationStrategies)
    {
      Structures = new Dictionary<IAsciiSeparationStrategy, AsciiLineStructure>();
      foreach (IAsciiSeparationStrategy separation in separationStrategies)
        Structures.Add(separation, GetStructure(nLine, sLine, separation));
    }

    /// <summary>
    /// Analyse the provided line of text with regard to one separation stragegy and returns the resulting structure.
    /// </summary>
    /// <param name="nLine">Line number.</param>
    /// <param name="sLine">The line to analyse.</param>
    /// <param name="separation">The separation stragegy that is used to separate the tokens of the line.</param>
    /// <returns>The resulting structure.</returns>
    public static AsciiLineStructure GetStructure(int nLine, string sLine, IAsciiSeparationStrategy separation)
    {
      AsciiLineStructure tabStruc = new AsciiLineStructure();
      tabStruc.LineNumber = nLine;

      foreach(string substring in separation.GetTokens(sLine))
      {
        if (string.IsNullOrEmpty(substring)) // just this char is a tab, so nothing is between the last and this
        {
          tabStruc.Add(typeof(DBNull));
        }
        else if (IsNumeric(substring))
        {
					if (IsIntegral(substring))
					{
						tabStruc.Add(typeof(long));
					}
					else
					{
						tabStruc.Add(typeof(double));
						tabStruc.AddToDecimalSeparatorStatistics(substring); // make a statistics of the use of decimal separator
					}
        }
        else if (IsDateTime(substring))
        {
          tabStruc.Add(typeof(System.DateTime));
        }
        else
        {
          tabStruc.Add(typeof(string));
        }
      } // end for
      return tabStruc;
    }

    /// <summary>
    /// Tests wether or not the string s is a date/time string.
    /// </summary>
    /// <param name="s">The string to test.</param>
    /// <returns>True if the string can be parsed to a date/time value.</returns>
    public static bool IsDateTime(string s)
    {
      DateTime result;
      return DateTime.TryParse(s, out result);
    }

    /// <summary>
    /// Tests if the string <c>s</c> is numeric. This is a very generic test here. We accept dots and commas as decimal separators, because the decimal separator statistics
    /// is made afterwards.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsNumeric(string s)
    {
      double result;
      if (double.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.CurrentInfo, out result))
        return true;

      return double.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out result);
    }

		/// <summary>
		/// Tests if the string <c>s</c> is an integral numeric value.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static bool IsIntegral(string s)
		{
			long result;
			return long.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out result);
		}
    
  } // end class

}
