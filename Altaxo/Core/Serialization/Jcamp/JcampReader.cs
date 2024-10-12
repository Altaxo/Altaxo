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
  /// Reader for Jcamp-Dx files.
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


    public List<Block> Blocks { get; } = new List<Block>();

    /// <summary>
    /// Messages about any errors during the import of the Jcamp file.
    /// </summary>
    public string? ErrorMessages { get; protected set; } = null;

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

    public static bool DoubleTryParse(string s, out double x)
    {
      if (double.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out x))
        return true;

      return double.TryParse(s, NumberStyles.Float, _numberFormatCommaDecimalSeparator, out x);
    }

    public static void DoubleParse(string s, out double x)
    {
      if (double.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out x))
        return;

      x = double.Parse(s, NumberStyles.Float, _numberFormatCommaDecimalSeparator);
    }





  }
}
