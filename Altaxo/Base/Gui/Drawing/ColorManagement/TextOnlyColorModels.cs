﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Drawing;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  [DisplayName("${res:ClassNames.Altaxo.Gui.Drawing.ColorManagement.TextOnlyColorModelRGB}")]
  public class TextOnlyColorModelRGB : ITextOnlyColorModel
  {
    public string[] GetComponentsForColor(AxoColor color, IFormatProvider formatProvider)
    {
      return new string[] { color.R.ToString(formatProvider), color.G.ToString(formatProvider), color.B.ToString(formatProvider) };
    }

    public string[] GetNamesOfComponents()
    {
      return new[] { "R", "G", "B" };
    }
  }

  [DisplayName("${res:ClassNames.Altaxo.Gui.Drawing.ColorManagement.TextOnlyColorModelLinearRGB}")]
  public class TextOnlyColorModelLinearRGB : ITextOnlyColorModel
  {
    public string[] GetComponentsForColor(AxoColor color, IFormatProvider formatProvider)
    {
      return new string[]
      {
        color.ScR.ToString("F3", formatProvider),
        color.ScG.ToString("F3", formatProvider),
        color.ScB.ToString("F3", formatProvider) };
    }

    public string[] GetNamesOfComponents()
    {
      return new[] { "Lin R", "Lin G", "Lin B" };
    }
  }

  [DisplayName("${res:ClassNames.Altaxo.Gui.Drawing.ColorManagement.TextOnlyColorModelCMY}")]
  public class TextOnlyColorModelCMY : ITextOnlyColorModel
  {
    public string[] GetComponentsForColor(AxoColor color, IFormatProvider formatProvider)
    {
      return new string[] { (255 - color.R).ToString(formatProvider), (255 - color.G).ToString(formatProvider), (255 - color.B).ToString(formatProvider) };
    }

    public string[] GetNamesOfComponents()
    {
      return new[] { "C", "M", "Y" };
    }
  }

  [DisplayName("${res:ClassNames.Altaxo.Gui.Drawing.ColorManagement.TextOnlyColorModelLinearCMY}")]
  public class TextOnlyColorModelLinearCMY : ITextOnlyColorModel
  {
    public string[] GetComponentsForColor(AxoColor color, IFormatProvider formatProvider)
    {
      return new string[]
      {
        (1 - color.ScR).ToString("F3", formatProvider),
        (1 - color.ScG).ToString("F3", formatProvider),
        (1 - color.ScB).ToString("F3", formatProvider) };
    }

    public string[] GetNamesOfComponents()
    {
      return new[] { "Lin C", "Lin M", "Lin Y" };
    }
  }

  [DisplayName("${res:ClassNames.Altaxo.Gui.Drawing.ColorManagement.TextOnlyColorModelCMYK}")]
  public class TextOnlyColorModelCMYK : ITextOnlyColorModel
  {
    public string[] GetComponentsForColor(AxoColor color, IFormatProvider formatProvider)
    {
      var (a, c, m, y, k) = color.ToAcmyk();

      return new string[]
      {
        c.ToString("F3", formatProvider),
        m.ToString("F3", formatProvider),
        y.ToString("F3", formatProvider),
        k.ToString("F3", formatProvider)
      };
    }

    public string[] GetNamesOfComponents()
    {
      return new[] { "C", "M", "Y", "K" };
    }
  }

  [DisplayName("${res:ClassNames.Altaxo.Gui.Drawing.ColorManagement.TextOnlyColorModelHSB}")]
  public class TextOnlyColorModelHSB : ITextOnlyColorModel
  {
    public string[] GetComponentsForColor(AxoColor color, IFormatProvider formatProvider)
    {
      var (a, h, s, b) = color.ToAhsb();
      return new string[]
      {
        h.ToString("F3", formatProvider),
        s.ToString("F3", formatProvider),
        b.ToString("F3", formatProvider) };
    }

    public string[] GetNamesOfComponents()
    {
      return new[] { "H", "S", "B" };
    }
  }

  [DisplayName("${res:ClassNames.Altaxo.Gui.Drawing.ColorManagement.TextOnlyColorModelRGBHex}")]
  public class TextOnlyColorModelRGBHex : ITextOnlyColorModel
  {
    public string[] GetComponentsForColor(AxoColor color, IFormatProvider formatProvider)
    {
      return new string[] { string.Format(formatProvider, "#{0:X02}{1:X02}{2:X02}", color.R, color.G, color.B) };
    }

    public string[] GetNamesOfComponents()
    {
      return new[] { "RGB_Hex" };
    }
  }

  [DisplayName("${res:ClassNames.Altaxo.Gui.Drawing.ColorManagement.TextOnlyColorModelARGBHex}")]
  public class TextOnlyColorModelARGBHex : ITextOnlyColorModel
  {
    public string[] GetComponentsForColor(AxoColor color, IFormatProvider formatProvider)
    {
      return new string[] { string.Format(formatProvider, "#{0:X02}{1:X02}{2:X02}{2:X02}", color.A, color.R, color.G, color.B) };
    }

    public string[] GetNamesOfComponents()
    {
      return new[] { "ARGB_Hex" };
    }
  }
}
