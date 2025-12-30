#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Altaxo.Serialization.Jcamp
{
  /// <summary>
  /// Reader for JCAMP-DX files. Parses one or more JCAMP data blocks from a stream
  /// and exposes the parsed blocks and any error messages encountered during import.
  /// </summary>
  public partial class JcampReader
  {
    private const string TitleHeader = "##TITLE=";
    private const string OwnerHeader = "##OWNER=";
    private const string SystemNameHeader = "##SPECTROMETER/DATA SYSTEM=";
    private const string XLabelHeader = "##XLABEL=";
    private const string YLabelHeader = "##YLABEL=";
    private const string XUnitHeader = "##XUNITS=";
    private const string YUnitHeader = "##YUNITS=";
    private const string TimeHeader = "##TIME=";
    private const string DateHeader = "##DATE=";
    private const string NumberOfPointsHeader = "##NPOINTS=";
    private const string FirstXHeader = "##FIRSTX=";
    private const string LastXHeader = "##LASTX=";
    private const string DeltaXHeader = "##DELTAX=";
    private const string XFactorHeader = "##XFACTOR=";
    private const string YFactorHeader = "##YFACTOR=";
    private const string XYBlockHeader = "##XYDATA=";
    private const string BlockEndHeader = "##END=";

    private static readonly char[] splitChars = new char[] { ' ', '\t' };
    private static readonly char[] _plusMinusChars = new char[] { '+', '-' };
    private static readonly char[] _trimCharsDateTime = new char[] { ' ', '\t', ';' };


    /// <summary>
    /// The list of parsed JCAMP blocks read from the input stream. Each block contains metadata
    /// and the associated X/Y data for a single-spectrum JCAMP block.
    /// </summary>
    public List<Block> Blocks { get; } = new List<Block>();

    /// <summary>
    /// Messages about any errors during the import of the JCAMP file. Multiple block errors are concatenated.
    /// </summary>
    public string? ErrorMessages { get; protected set; } = null;

    /// <summary>
    /// Initializes a new instance of <see cref="JcampReader"/> and reads JCAMP blocks from the provided stream.
    /// The stream is read using a <see cref="StreamReader"/>; the stream must be readable.
    /// </summary>
    /// <param name="stream">The stream containing JCAMP-DX formatted text.</param>
    public JcampReader(Stream stream)
    {
      var tr = new StreamReader(stream);
      for (; ; )
      {
        var block = new Block(tr);

        if (!string.IsNullOrEmpty(block.ErrorMessages))
        {
          ErrorMessages = (ErrorMessages ?? string.Empty) + block.ErrorMessages;
        }

        if (block.XValues is not null && block.YValues is not null && block.XValues.Length > 0 && block.YValues.Length > 0)
          Blocks.Add(block);
        else
          break;
      }
    }



    /// <summary>
    /// Splits a line that contains numbers separated only by '+' or '-' signs into separate tokens.
    /// For example, the string "1.0-2.0+3.0" will be split into ["1.0", "-2.0", "+3.0"].
    /// </summary>
    /// <param name="s">The input line to split.</param>
    /// <returns>An array of string tokens representing each numeric value in the line.</returns>
    public static string[] SplitLineByPlusOrMinus(string s)
    {
      var list = new List<string>();
      s = s.Trim();
      int idx;
      int start = 0;
      do
      {
        idx = s.IndexOfAny(_plusMinusChars, start + 1);
        if (idx > 1)
        {
          list.Add(s.Substring(start, idx - start));
          start = idx;
        }
        else
        {
          list.Add(s.Substring(start));
        }
      } while (idx >= 0);

      return list.ToArray();
    }


    private static NumberFormatInfo _numberFormatCommaDecimalSeparator = new NumberFormatInfo() { NumberDecimalSeparator = "," };

    /// <summary>
    /// Tries to parse a double value from a string using the invariant number format first and then
    /// a format using a comma as decimal separator if the invariant parse fails.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="x">When this method returns, contains the parsed double if the conversion succeeded; otherwise, zero.</param>
    /// <returns>True if parsing succeeded; otherwise false.</returns>
    public static bool DoubleTryParse(string s, out double x)
    {
      if (double.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out x))
        return true;

      return double.TryParse(s, NumberStyles.Float, _numberFormatCommaDecimalSeparator, out x);
    }

    /// <summary>
    /// Parses a double value from a string using the invariant number format first and then
    /// a format using a comma as decimal separator if the invariant parse fails. Throws on failure.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="x">When this method returns, contains the parsed double value.</param>
    public static void DoubleParse(string s, out double x)
    {
      if (double.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out x))
        return;

      x = double.Parse(s, NumberStyles.Float, _numberFormatCommaDecimalSeparator);
    }



  }
}
