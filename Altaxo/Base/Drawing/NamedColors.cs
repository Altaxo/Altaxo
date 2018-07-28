#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;

namespace Altaxo.Drawing
{
  public class NamedColors : ColorManagement.ColorSet
  {
    private static NamedColors _colors;

    static NamedColors()
    {
      _colors = new NamedColors();
    }

    private NamedColors()
      : base("KnownColors", GetKnownColors())
    {
    }

    public static NamedColors Instance
    {
      get
      {
        return _colors;
      }
    }

    #region Auto generated code

    private static NamedColor[] GetKnownColors()
    {
      return new NamedColor[] {
NamedColor.FromArgb(255, 240, 248, 255, "AliceBlue"),
NamedColor.FromArgb(255, 250, 235, 215, "AntiqueWhite"),
NamedColor.FromArgb(255, 0, 255, 255, "Aqua"),
NamedColor.FromArgb(255, 127, 255, 212, "Aquamarine"),
NamedColor.FromArgb(255, 240, 255, 255, "Azure"),
NamedColor.FromArgb(255, 245, 245, 220, "Beige"),
NamedColor.FromArgb(255, 255, 228, 196, "Bisque"),
NamedColor.FromArgb(255, 0, 0, 0, "Black"),
NamedColor.FromArgb(255, 255, 235, 205, "BlanchedAlmond"),
NamedColor.FromArgb(255, 0, 0, 255, "Blue"),
NamedColor.FromArgb(255, 138, 43, 226, "BlueViolet"),
NamedColor.FromArgb(255, 165, 42, 42, "Brown"),
NamedColor.FromArgb(255, 222, 184, 135, "BurlyWood"),
NamedColor.FromArgb(255, 95, 158, 160, "CadetBlue"),
NamedColor.FromArgb(255, 127, 255, 0, "Chartreuse"),
NamedColor.FromArgb(255, 210, 105, 30, "Chocolate"),
NamedColor.FromArgb(255, 255, 127, 80, "Coral"),
NamedColor.FromArgb(255, 100, 149, 237, "CornflowerBlue"),
NamedColor.FromArgb(255, 255, 248, 220, "Cornsilk"),
NamedColor.FromArgb(255, 220, 20, 60, "Crimson"),
NamedColor.FromArgb(255, 0, 255, 255, "Cyan"),
NamedColor.FromArgb(255, 0, 0, 139, "DarkBlue"),
NamedColor.FromArgb(255, 0, 139, 139, "DarkCyan"),
NamedColor.FromArgb(255, 184, 134, 11, "DarkGoldenrod"),
NamedColor.FromArgb(255, 169, 169, 169, "DarkGray"),
NamedColor.FromArgb(255, 0, 100, 0, "DarkGreen"),
NamedColor.FromArgb(255, 189, 183, 107, "DarkKhaki"),
NamedColor.FromArgb(255, 139, 0, 139, "DarkMagenta"),
NamedColor.FromArgb(255, 85, 107, 47, "DarkOliveGreen"),
NamedColor.FromArgb(255, 255, 140, 0, "DarkOrange"),
NamedColor.FromArgb(255, 153, 50, 204, "DarkOrchid"),
NamedColor.FromArgb(255, 139, 0, 0, "DarkRed"),
NamedColor.FromArgb(255, 233, 150, 122, "DarkSalmon"),
NamedColor.FromArgb(255, 143, 188, 143, "DarkSeaGreen"),
NamedColor.FromArgb(255, 72, 61, 139, "DarkSlateBlue"),
NamedColor.FromArgb(255, 47, 79, 79, "DarkSlateGray"),
NamedColor.FromArgb(255, 0, 206, 209, "DarkTurquoise"),
NamedColor.FromArgb(255, 148, 0, 211, "DarkViolet"),
NamedColor.FromArgb(255, 255, 20, 147, "DeepPink"),
NamedColor.FromArgb(255, 0, 191, 255, "DeepSkyBlue"),
NamedColor.FromArgb(255, 105, 105, 105, "DimGray"),
NamedColor.FromArgb(255, 30, 144, 255, "DodgerBlue"),
NamedColor.FromArgb(255, 178, 34, 34, "Firebrick"),
NamedColor.FromArgb(255, 255, 250, 240, "FloralWhite"),
NamedColor.FromArgb(255, 34, 139, 34, "ForestGreen"),
NamedColor.FromArgb(255, 255, 0, 255, "Fuchsia"),
NamedColor.FromArgb(255, 220, 220, 220, "Gainsboro"),
NamedColor.FromArgb(255, 248, 248, 255, "GhostWhite"),
NamedColor.FromArgb(255, 255, 215, 0, "Gold"),
NamedColor.FromArgb(255, 218, 165, 32, "Goldenrod"),
NamedColor.FromArgb(255, 128, 128, 128, "Gray"),
NamedColor.FromArgb(255, 0, 128, 0, "Green"),
NamedColor.FromArgb(255, 173, 255, 47, "GreenYellow"),
NamedColor.FromArgb(255, 240, 255, 240, "Honeydew"),
NamedColor.FromArgb(255, 255, 105, 180, "HotPink"),
NamedColor.FromArgb(255, 205, 92, 92, "IndianRed"),
NamedColor.FromArgb(255, 75, 0, 130, "Indigo"),
NamedColor.FromArgb(255, 255, 255, 240, "Ivory"),
NamedColor.FromArgb(255, 240, 230, 140, "Khaki"),
NamedColor.FromArgb(255, 230, 230, 250, "Lavender"),
NamedColor.FromArgb(255, 255, 240, 245, "LavenderBlush"),
NamedColor.FromArgb(255, 124, 252, 0, "LawnGreen"),
NamedColor.FromArgb(255, 255, 250, 205, "LemonChiffon"),
NamedColor.FromArgb(255, 173, 216, 230, "LightBlue"),
NamedColor.FromArgb(255, 240, 128, 128, "LightCoral"),
NamedColor.FromArgb(255, 224, 255, 255, "LightCyan"),
NamedColor.FromArgb(255, 250, 250, 210, "LightGoldenrodYellow"),
NamedColor.FromArgb(255, 211, 211, 211, "LightGray"),
NamedColor.FromArgb(255, 144, 238, 144, "LightGreen"),
NamedColor.FromArgb(255, 255, 182, 193, "LightPink"),
NamedColor.FromArgb(255, 255, 160, 122, "LightSalmon"),
NamedColor.FromArgb(255, 32, 178, 170, "LightSeaGreen"),
NamedColor.FromArgb(255, 135, 206, 250, "LightSkyBlue"),
NamedColor.FromArgb(255, 119, 136, 153, "LightSlateGray"),
NamedColor.FromArgb(255, 176, 196, 222, "LightSteelBlue"),
NamedColor.FromArgb(255, 255, 255, 224, "LightYellow"),
NamedColor.FromArgb(255, 0, 255, 0, "Lime"),
NamedColor.FromArgb(255, 50, 205, 50, "LimeGreen"),
NamedColor.FromArgb(255, 250, 240, 230, "Linen"),
NamedColor.FromArgb(255, 255, 0, 255, "Magenta"),
NamedColor.FromArgb(255, 128, 0, 0, "Maroon"),
NamedColor.FromArgb(255, 102, 205, 170, "MediumAquamarine"),
NamedColor.FromArgb(255, 0, 0, 205, "MediumBlue"),
NamedColor.FromArgb(255, 186, 85, 211, "MediumOrchid"),
NamedColor.FromArgb(255, 147, 112, 219, "MediumPurple"),
NamedColor.FromArgb(255, 60, 179, 113, "MediumSeaGreen"),
NamedColor.FromArgb(255, 123, 104, 238, "MediumSlateBlue"),
NamedColor.FromArgb(255, 0, 250, 154, "MediumSpringGreen"),
NamedColor.FromArgb(255, 72, 209, 204, "MediumTurquoise"),
NamedColor.FromArgb(255, 199, 21, 133, "MediumVioletRed"),
NamedColor.FromArgb(255, 25, 25, 112, "MidnightBlue"),
NamedColor.FromArgb(255, 245, 255, 250, "MintCream"),
NamedColor.FromArgb(255, 255, 228, 225, "MistyRose"),
NamedColor.FromArgb(255, 255, 228, 181, "Moccasin"),
NamedColor.FromArgb(255, 255, 222, 173, "NavajoWhite"),
NamedColor.FromArgb(255, 0, 0, 128, "Navy"),
NamedColor.FromArgb(255, 253, 245, 230, "OldLace"),
NamedColor.FromArgb(255, 128, 128, 0, "Olive"),
NamedColor.FromArgb(255, 107, 142, 35, "OliveDrab"),
NamedColor.FromArgb(255, 255, 165, 0, "Orange"),
NamedColor.FromArgb(255, 255, 69, 0, "OrangeRed"),
NamedColor.FromArgb(255, 218, 112, 214, "Orchid"),
NamedColor.FromArgb(255, 238, 232, 170, "PaleGoldenrod"),
NamedColor.FromArgb(255, 152, 251, 152, "PaleGreen"),
NamedColor.FromArgb(255, 175, 238, 238, "PaleTurquoise"),
NamedColor.FromArgb(255, 219, 112, 147, "PaleVioletRed"),
NamedColor.FromArgb(255, 255, 239, 213, "PapayaWhip"),
NamedColor.FromArgb(255, 255, 218, 185, "PeachPuff"),
NamedColor.FromArgb(255, 205, 133, 63, "Peru"),
NamedColor.FromArgb(255, 255, 192, 203, "Pink"),
NamedColor.FromArgb(255, 221, 160, 221, "Plum"),
NamedColor.FromArgb(255, 176, 224, 230, "PowderBlue"),
NamedColor.FromArgb(255, 128, 0, 128, "Purple"),
NamedColor.FromArgb(255, 255, 0, 0, "Red"),
NamedColor.FromArgb(255, 188, 143, 143, "RosyBrown"),
NamedColor.FromArgb(255, 65, 105, 225, "RoyalBlue"),
NamedColor.FromArgb(255, 139, 69, 19, "SaddleBrown"),
NamedColor.FromArgb(255, 250, 128, 114, "Salmon"),
NamedColor.FromArgb(255, 244, 164, 96, "SandyBrown"),
NamedColor.FromArgb(255, 46, 139, 87, "SeaGreen"),
NamedColor.FromArgb(255, 255, 245, 238, "SeaShell"),
NamedColor.FromArgb(255, 160, 82, 45, "Sienna"),
NamedColor.FromArgb(255, 192, 192, 192, "Silver"),
NamedColor.FromArgb(255, 135, 206, 235, "SkyBlue"),
NamedColor.FromArgb(255, 106, 90, 205, "SlateBlue"),
NamedColor.FromArgb(255, 112, 128, 144, "SlateGray"),
NamedColor.FromArgb(255, 255, 250, 250, "Snow"),
NamedColor.FromArgb(255, 0, 255, 127, "SpringGreen"),
NamedColor.FromArgb(255, 70, 130, 180, "SteelBlue"),
NamedColor.FromArgb(255, 210, 180, 140, "Tan"),
NamedColor.FromArgb(255, 0, 128, 128, "Teal"),
NamedColor.FromArgb(255, 216, 191, 216, "Thistle"),
NamedColor.FromArgb(255, 255, 99, 71, "Tomato"),
NamedColor.FromArgb(0, 255, 255, 255, "Transparent"),
NamedColor.FromArgb(255, 64, 224, 208, "Turquoise"),
NamedColor.FromArgb(255, 238, 130, 238, "Violet"),
NamedColor.FromArgb(255, 245, 222, 179, "Wheat"),
NamedColor.FromArgb(255, 255, 255, 255, "White"),
NamedColor.FromArgb(255, 245, 245, 245, "WhiteSmoke"),
NamedColor.FromArgb(255, 255, 255, 0, "Yellow"),
NamedColor.FromArgb(255, 154, 205, 50, "YellowGreen"),
};
    }

    public static NamedColor AliceBlue { get { return _colors[0]; } }

    public static NamedColor AntiqueWhite { get { return _colors[1]; } }

    public static NamedColor Aqua { get { return _colors[2]; } }

    public static NamedColor Aquamarine { get { return _colors[3]; } }

    public static NamedColor Azure { get { return _colors[4]; } }

    public static NamedColor Beige { get { return _colors[5]; } }

    public static NamedColor Bisque { get { return _colors[6]; } }

    public static NamedColor Black { get { return _colors[7]; } }

    public static NamedColor BlanchedAlmond { get { return _colors[8]; } }

    public static NamedColor Blue { get { return _colors[9]; } }

    public static NamedColor BlueViolet { get { return _colors[10]; } }

    public static NamedColor Brown { get { return _colors[11]; } }

    public static NamedColor BurlyWood { get { return _colors[12]; } }

    public static NamedColor CadetBlue { get { return _colors[13]; } }

    public static NamedColor Chartreuse { get { return _colors[14]; } }

    public static NamedColor Chocolate { get { return _colors[15]; } }

    public static NamedColor Coral { get { return _colors[16]; } }

    public static NamedColor CornflowerBlue { get { return _colors[17]; } }

    public static NamedColor Cornsilk { get { return _colors[18]; } }

    public static NamedColor Crimson { get { return _colors[19]; } }

    public static NamedColor Cyan { get { return _colors[20]; } }

    public static NamedColor DarkBlue { get { return _colors[21]; } }

    public static NamedColor DarkCyan { get { return _colors[22]; } }

    public static NamedColor DarkGoldenrod { get { return _colors[23]; } }

    public static NamedColor DarkGray { get { return _colors[24]; } }

    public static NamedColor DarkGreen { get { return _colors[25]; } }

    public static NamedColor DarkKhaki { get { return _colors[26]; } }

    public static NamedColor DarkMagenta { get { return _colors[27]; } }

    public static NamedColor DarkOliveGreen { get { return _colors[28]; } }

    public static NamedColor DarkOrange { get { return _colors[29]; } }

    public static NamedColor DarkOrchid { get { return _colors[30]; } }

    public static NamedColor DarkRed { get { return _colors[31]; } }

    public static NamedColor DarkSalmon { get { return _colors[32]; } }

    public static NamedColor DarkSeaGreen { get { return _colors[33]; } }

    public static NamedColor DarkSlateBlue { get { return _colors[34]; } }

    public static NamedColor DarkSlateGray { get { return _colors[35]; } }

    public static NamedColor DarkTurquoise { get { return _colors[36]; } }

    public static NamedColor DarkViolet { get { return _colors[37]; } }

    public static NamedColor DeepPink { get { return _colors[38]; } }

    public static NamedColor DeepSkyBlue { get { return _colors[39]; } }

    public static NamedColor DimGray { get { return _colors[40]; } }

    public static NamedColor DodgerBlue { get { return _colors[41]; } }

    public static NamedColor Firebrick { get { return _colors[42]; } }

    public static NamedColor FloralWhite { get { return _colors[43]; } }

    public static NamedColor ForestGreen { get { return _colors[44]; } }

    public static NamedColor Fuchsia { get { return _colors[45]; } }

    public static NamedColor Gainsboro { get { return _colors[46]; } }

    public static NamedColor GhostWhite { get { return _colors[47]; } }

    public static NamedColor Gold { get { return _colors[48]; } }

    public static NamedColor Goldenrod { get { return _colors[49]; } }

    public static NamedColor Gray { get { return _colors[50]; } }

    public static NamedColor Green { get { return _colors[51]; } }

    public static NamedColor GreenYellow { get { return _colors[52]; } }

    public static NamedColor Honeydew { get { return _colors[53]; } }

    public static NamedColor HotPink { get { return _colors[54]; } }

    public static NamedColor IndianRed { get { return _colors[55]; } }

    public static NamedColor Indigo { get { return _colors[56]; } }

    public static NamedColor Ivory { get { return _colors[57]; } }

    public static NamedColor Khaki { get { return _colors[58]; } }

    public static NamedColor Lavender { get { return _colors[59]; } }

    public static NamedColor LavenderBlush { get { return _colors[60]; } }

    public static NamedColor LawnGreen { get { return _colors[61]; } }

    public static NamedColor LemonChiffon { get { return _colors[62]; } }

    public static NamedColor LightBlue { get { return _colors[63]; } }

    public static NamedColor LightCoral { get { return _colors[64]; } }

    public static NamedColor LightCyan { get { return _colors[65]; } }

    public static NamedColor LightGoldenrodYellow { get { return _colors[66]; } }

    public static NamedColor LightGray { get { return _colors[67]; } }

    public static NamedColor LightGreen { get { return _colors[68]; } }

    public static NamedColor LightPink { get { return _colors[69]; } }

    public static NamedColor LightSalmon { get { return _colors[70]; } }

    public static NamedColor LightSeaGreen { get { return _colors[71]; } }

    public static NamedColor LightSkyBlue { get { return _colors[72]; } }

    public static NamedColor LightSlateGray { get { return _colors[73]; } }

    public static NamedColor LightSteelBlue { get { return _colors[74]; } }

    public static NamedColor LightYellow { get { return _colors[75]; } }

    public static NamedColor Lime { get { return _colors[76]; } }

    public static NamedColor LimeGreen { get { return _colors[77]; } }

    public static NamedColor Linen { get { return _colors[78]; } }

    public static NamedColor Magenta { get { return _colors[79]; } }

    public static NamedColor Maroon { get { return _colors[80]; } }

    public static NamedColor MediumAquamarine { get { return _colors[81]; } }

    public static NamedColor MediumBlue { get { return _colors[82]; } }

    public static NamedColor MediumOrchid { get { return _colors[83]; } }

    public static NamedColor MediumPurple { get { return _colors[84]; } }

    public static NamedColor MediumSeaGreen { get { return _colors[85]; } }

    public static NamedColor MediumSlateBlue { get { return _colors[86]; } }

    public static NamedColor MediumSpringGreen { get { return _colors[87]; } }

    public static NamedColor MediumTurquoise { get { return _colors[88]; } }

    public static NamedColor MediumVioletRed { get { return _colors[89]; } }

    public static NamedColor MidnightBlue { get { return _colors[90]; } }

    public static NamedColor MintCream { get { return _colors[91]; } }

    public static NamedColor MistyRose { get { return _colors[92]; } }

    public static NamedColor Moccasin { get { return _colors[93]; } }

    public static NamedColor NavajoWhite { get { return _colors[94]; } }

    public static NamedColor Navy { get { return _colors[95]; } }

    public static NamedColor OldLace { get { return _colors[96]; } }

    public static NamedColor Olive { get { return _colors[97]; } }

    public static NamedColor OliveDrab { get { return _colors[98]; } }

    public static NamedColor Orange { get { return _colors[99]; } }

    public static NamedColor OrangeRed { get { return _colors[100]; } }

    public static NamedColor Orchid { get { return _colors[101]; } }

    public static NamedColor PaleGoldenrod { get { return _colors[102]; } }

    public static NamedColor PaleGreen { get { return _colors[103]; } }

    public static NamedColor PaleTurquoise { get { return _colors[104]; } }

    public static NamedColor PaleVioletRed { get { return _colors[105]; } }

    public static NamedColor PapayaWhip { get { return _colors[106]; } }

    public static NamedColor PeachPuff { get { return _colors[107]; } }

    public static NamedColor Peru { get { return _colors[108]; } }

    public static NamedColor Pink { get { return _colors[109]; } }

    public static NamedColor Plum { get { return _colors[110]; } }

    public static NamedColor PowderBlue { get { return _colors[111]; } }

    public static NamedColor Purple { get { return _colors[112]; } }

    public static NamedColor Red { get { return _colors[113]; } }

    public static NamedColor RosyBrown { get { return _colors[114]; } }

    public static NamedColor RoyalBlue { get { return _colors[115]; } }

    public static NamedColor SaddleBrown { get { return _colors[116]; } }

    public static NamedColor Salmon { get { return _colors[117]; } }

    public static NamedColor SandyBrown { get { return _colors[118]; } }

    public static NamedColor SeaGreen { get { return _colors[119]; } }

    public static NamedColor SeaShell { get { return _colors[120]; } }

    public static NamedColor Sienna { get { return _colors[121]; } }

    public static NamedColor Silver { get { return _colors[122]; } }

    public static NamedColor SkyBlue { get { return _colors[123]; } }

    public static NamedColor SlateBlue { get { return _colors[124]; } }

    public static NamedColor SlateGray { get { return _colors[125]; } }

    public static NamedColor Snow { get { return _colors[126]; } }

    public static NamedColor SpringGreen { get { return _colors[127]; } }

    public static NamedColor SteelBlue { get { return _colors[128]; } }

    public static NamedColor Tan { get { return _colors[129]; } }

    public static NamedColor Teal { get { return _colors[130]; } }

    public static NamedColor Thistle { get { return _colors[131]; } }

    public static NamedColor Tomato { get { return _colors[132]; } }

    public static NamedColor Transparent { get { return _colors[133]; } }

    public static NamedColor Turquoise { get { return _colors[134]; } }

    public static NamedColor Violet { get { return _colors[135]; } }

    public static NamedColor Wheat { get { return _colors[136]; } }

    public static NamedColor White { get { return _colors[137]; } }

    public static NamedColor WhiteSmoke { get { return _colors[138]; } }

    public static NamedColor Yellow { get { return _colors[139]; } }

    public static NamedColor YellowGreen { get { return _colors[140]; } }

    #endregion Auto generated code
  }
}
