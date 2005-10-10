#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using System.Drawing;

namespace Altaxo.Graph.AxisLabeling
{
  /// <summary>
  /// Summary description for NumericAxisLabelFormattingFixed.
  /// </summary>
  public class NumericAxisLabelFormattingScientific : AbstractLabelFormatting
  {
    // int _decimalplaces;
    string _formatString = "{0}";


    public NumericAxisLabelFormattingScientific()
    {
    
    }

    public NumericAxisLabelFormattingScientific(NumericAxisLabelFormattingScientific from)
    {
      this._formatString = from._formatString;
    }

    public override object Clone()
    {
      return new NumericAxisLabelFormattingScientific(this);
    }


    protected override string FormatItem(Altaxo.Data.AltaxoVariant item)
    {
      throw new ApplicationException("Programming error: this function must not be called because the item can not be formatted as a string");
    }


    public string FormatItem(double tick)
    {
      throw new ApplicationException("Programming error: this function must not be called because the item can not be formatted as a string");
    }

    protected void SplitInFirstPartAndExponent(double ditem, out string firstpart, out string exponent)
    {
      string sitem1 = ditem.ToString("E0");

      int posOfE = sitem1.IndexOf('E');
      if (posOfE < 0)
      {
        firstpart = sitem1;
        exponent = string.Empty;
        return;
      }

      firstpart = sitem1.Substring(0, posOfE) + "·10";
      exponent = sitem1.Substring(posOfE + 1);
    }

    public override System.Drawing.SizeF MeasureItem(System.Drawing.Graphics g, System.Drawing.Font font, System.Drawing.StringFormat strfmt, Altaxo.Data.AltaxoVariant mtick, System.Drawing.PointF morg)
    {
      string firstpart , exponent;
      SplitInFirstPartAndExponent((double)mtick, out firstpart, out exponent);

      SizeF size1 = g.MeasureString(firstpart, font, new PointF(0, 0), strfmt);
      SizeF size2 = g.MeasureString(exponent, font, new PointF(size1.Width, 0), strfmt);

      return new SizeF(size1.Width + size2.Width, size1.Height);
    }

    public override void DrawItem(Graphics g, BrushHolder brush, Font font, StringFormat strfmt, Altaxo.Data.AltaxoVariant item, PointF morg)
    {
      string firstpart, exponent;
      SplitInFirstPartAndExponent((double)item, out firstpart, out exponent);

      SizeF size1 = g.MeasureString(firstpart, font, morg, strfmt);
      g.DrawString(firstpart, font, brush, morg, strfmt);
      morg.X += size1.Width;
      morg.Y += size1.Height / 3;
      using (Font font2 = new Font(font.FontFamily, (float)(font.Size * 2 / 3.0)))
      {
        g.DrawString(exponent, font2, brush, morg);
      }
      
    }
  }
}
