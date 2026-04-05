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

#nullable enable
namespace Altaxo.Drawing
{
  /// <summary>
  /// Provides the built-in set of known named colors.
  /// </summary>
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

    /// <summary>
    /// Gets the singleton collection of built-in named colors.
    /// </summary>
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

    /// <summary>
    /// Gets the named color "AliceBlue".
    /// </summary>
    public static NamedColor AliceBlue { get { return _colors[0]; } }

    /// <summary>
    /// Gets the named color "AntiqueWhite".
    /// </summary>
    public static NamedColor AntiqueWhite { get { return _colors[1]; } }

    /// <summary>
    /// Gets the named color "Aqua".
    /// </summary>
    public static NamedColor Aqua { get { return _colors[2]; } }

    /// <summary>
    /// Gets the named color "Aquamarine".
    /// </summary>
    public static NamedColor Aquamarine { get { return _colors[3]; } }

    /// <summary>
    /// Gets the named color "Azure".
    /// </summary>
    public static NamedColor Azure { get { return _colors[4]; } }

    /// <summary>
    /// Gets the named color "Beige".
    /// </summary>
    public static NamedColor Beige { get { return _colors[5]; } }

    /// <summary>
    /// Gets the named color "Bisque".
    /// </summary>
    public static NamedColor Bisque { get { return _colors[6]; } }

    /// <summary>
    /// Gets the named color "Black".
    /// </summary>
    public static NamedColor Black { get { return _colors[7]; } }

    /// <summary>
    /// Gets the named color "BlanchedAlmond".
    /// </summary>
    public static NamedColor BlanchedAlmond { get { return _colors[8]; } }

    /// <summary>
    /// Gets the named color "Blue".
    /// </summary>
    public static NamedColor Blue { get { return _colors[9]; } }

    /// <summary>
    /// Gets the named color "BlueViolet".
    /// </summary>
    public static NamedColor BlueViolet { get { return _colors[10]; } }

    /// <summary>
    /// Gets the named color "Brown".
    /// </summary>
    public static NamedColor Brown { get { return _colors[11]; } }

    /// <summary>
    /// Gets the named color "BurlyWood".
    /// </summary>
    public static NamedColor BurlyWood { get { return _colors[12]; } }

    /// <summary>
    /// Gets the named color "CadetBlue".
    /// </summary>
    public static NamedColor CadetBlue { get { return _colors[13]; } }

    /// <summary>
    /// Gets the named color "Chartreuse".
    /// </summary>
    public static NamedColor Chartreuse { get { return _colors[14]; } }

    /// <summary>
    /// Gets the named color "Chocolate".
    /// </summary>
    public static NamedColor Chocolate { get { return _colors[15]; } }

    /// <summary>
    /// Gets the named color "Coral".
    /// </summary>
    public static NamedColor Coral { get { return _colors[16]; } }

    /// <summary>
    /// Gets the named color "CornflowerBlue".
    /// </summary>
    public static NamedColor CornflowerBlue { get { return _colors[17]; } }

    /// <summary>
    /// Gets the named color "Cornsilk".
    /// </summary>
    public static NamedColor Cornsilk { get { return _colors[18]; } }

    /// <summary>
    /// Gets the named color "Crimson".
    /// </summary>
    public static NamedColor Crimson { get { return _colors[19]; } }

    /// <summary>
    /// Gets the named color "Cyan".
    /// </summary>
    public static NamedColor Cyan { get { return _colors[20]; } }

    /// <summary>
    /// Gets the named color "DarkBlue".
    /// </summary>
    public static NamedColor DarkBlue { get { return _colors[21]; } }

    /// <summary>
    /// Gets the named color "DarkCyan".
    /// </summary>
    public static NamedColor DarkCyan { get { return _colors[22]; } }

    /// <summary>
    /// Gets the named color "DarkGoldenrod".
    /// </summary>
    public static NamedColor DarkGoldenrod { get { return _colors[23]; } }

    /// <summary>
    /// Gets the named color "DarkGray".
    /// </summary>
    public static NamedColor DarkGray { get { return _colors[24]; } }

    /// <summary>
    /// Gets the named color "DarkGreen".
    /// </summary>
    public static NamedColor DarkGreen { get { return _colors[25]; } }

    /// <summary>
    /// Gets the named color "DarkKhaki".
    /// </summary>
    public static NamedColor DarkKhaki { get { return _colors[26]; } }

    /// <summary>
    /// Gets the named color "DarkMagenta".
    /// </summary>
    public static NamedColor DarkMagenta { get { return _colors[27]; } }

    /// <summary>
    /// Gets the named color "DarkOliveGreen".
    /// </summary>
    public static NamedColor DarkOliveGreen { get { return _colors[28]; } }

    /// <summary>
    /// Gets the named color "DarkOrange".
    /// </summary>
    public static NamedColor DarkOrange { get { return _colors[29]; } }

    /// <summary>
    /// Gets the named color "DarkOrchid".
    /// </summary>
    public static NamedColor DarkOrchid { get { return _colors[30]; } }

    /// <summary>
    /// Gets the named color "DarkRed".
    /// </summary>
    public static NamedColor DarkRed { get { return _colors[31]; } }

    /// <summary>
    /// Gets the named color "DarkSalmon".
    /// </summary>
    public static NamedColor DarkSalmon { get { return _colors[32]; } }

    /// <summary>
    /// Gets the named color "DarkSeaGreen".
    /// </summary>
    public static NamedColor DarkSeaGreen { get { return _colors[33]; } }

    /// <summary>
    /// Gets the named color "DarkSlateBlue".
    /// </summary>
    public static NamedColor DarkSlateBlue { get { return _colors[34]; } }

    /// <summary>
    /// Gets the named color "DarkSlateGray".
    /// </summary>
    public static NamedColor DarkSlateGray { get { return _colors[35]; } }

    /// <summary>
    /// Gets the named color "DarkTurquoise".
    /// </summary>
    public static NamedColor DarkTurquoise { get { return _colors[36]; } }

    /// <summary>
    /// Gets the named color "DarkViolet".
    /// </summary>
    public static NamedColor DarkViolet { get { return _colors[37]; } }

    /// <summary>
    /// Gets the named color "DeepPink".
    /// </summary>
    public static NamedColor DeepPink { get { return _colors[38]; } }

    /// <summary>
    /// Gets the named color "DeepSkyBlue".
    /// </summary>
    public static NamedColor DeepSkyBlue { get { return _colors[39]; } }

    /// <summary>
    /// Gets the named color "DimGray".
    /// </summary>
    public static NamedColor DimGray { get { return _colors[40]; } }

    /// <summary>
    /// Gets the named color "DodgerBlue".
    /// </summary>
    public static NamedColor DodgerBlue { get { return _colors[41]; } }

    /// <summary>
    /// Gets the named color "Firebrick".
    /// </summary>
    public static NamedColor Firebrick { get { return _colors[42]; } }

    /// <summary>
    /// Gets the named color "FloralWhite".
    /// </summary>
    public static NamedColor FloralWhite { get { return _colors[43]; } }

    /// <summary>
    /// Gets the named color "ForestGreen".
    /// </summary>
    public static NamedColor ForestGreen { get { return _colors[44]; } }

    /// <summary>
    /// Gets the named color "Fuchsia".
    /// </summary>
    public static NamedColor Fuchsia { get { return _colors[45]; } }

    /// <summary>
    /// Gets the named color "Gainsboro".
    /// </summary>
    public static NamedColor Gainsboro { get { return _colors[46]; } }

    /// <summary>
    /// Gets the named color "GhostWhite".
    /// </summary>
    public static NamedColor GhostWhite { get { return _colors[47]; } }

    /// <summary>
    /// Gets the named color "Gold".
    /// </summary>
    public static NamedColor Gold { get { return _colors[48]; } }

    /// <summary>
    /// Gets the named color "Goldenrod".
    /// </summary>
    public static NamedColor Goldenrod { get { return _colors[49]; } }

    /// <summary>
    /// Gets the named color "Gray".
    /// </summary>
    public static NamedColor Gray { get { return _colors[50]; } }

    /// <summary>
    /// Gets the named color "Green".
    /// </summary>
    public static NamedColor Green { get { return _colors[51]; } }

    /// <summary>
    /// Gets the named color "GreenYellow".
    /// </summary>
    public static NamedColor GreenYellow { get { return _colors[52]; } }

    /// <summary>
    /// Gets the named color "Honeydew".
    /// </summary>
    public static NamedColor Honeydew { get { return _colors[53]; } }

    /// <summary>
    /// Gets the named color "HotPink".
    /// </summary>
    public static NamedColor HotPink { get { return _colors[54]; } }

    /// <summary>
    /// Gets the named color "IndianRed".
    /// </summary>
    public static NamedColor IndianRed { get { return _colors[55]; } }

    /// <summary>
    /// Gets the named color "Indigo".
    /// </summary>
    public static NamedColor Indigo { get { return _colors[56]; } }

    /// <summary>
    /// Gets the named color "Ivory".
    /// </summary>
    public static NamedColor Ivory { get { return _colors[57]; } }

    /// <summary>
    /// Gets the named color "Khaki".
    /// </summary>
    public static NamedColor Khaki { get { return _colors[58]; } }

    /// <summary>
    /// Gets the named color "Lavender".
    /// </summary>
    public static NamedColor Lavender { get { return _colors[59]; } }

    /// <summary>
    /// Gets the named color "LavenderBlush".
    /// </summary>
    public static NamedColor LavenderBlush { get { return _colors[60]; } }

    /// <summary>
    /// Gets the named color "LawnGreen".
    /// </summary>
    public static NamedColor LawnGreen { get { return _colors[61]; } }

    /// <summary>
    /// Gets the named color "LemonChiffon".
    /// </summary>
    public static NamedColor LemonChiffon { get { return _colors[62]; } }

    /// <summary>
    /// Gets the named color "LightBlue".
    /// </summary>
    public static NamedColor LightBlue { get { return _colors[63]; } }

    /// <summary>
    /// Gets the named color "LightCoral".
    /// </summary>
    public static NamedColor LightCoral { get { return _colors[64]; } }

    /// <summary>
    /// Gets the named color "LightCyan".
    /// </summary>
    public static NamedColor LightCyan { get { return _colors[65]; } }

    /// <summary>
    /// Gets the named color "LightGoldenrodYellow".
    /// </summary>
    public static NamedColor LightGoldenrodYellow { get { return _colors[66]; } }

    /// <summary>
    /// Gets the named color "LightGray".
    /// </summary>
    public static NamedColor LightGray { get { return _colors[67]; } }

    /// <summary>
    /// Gets the named color "LightGreen".
    /// </summary>
    public static NamedColor LightGreen { get { return _colors[68]; } }

    /// <summary>
    /// Gets the named color "LightPink".
    /// </summary>
    public static NamedColor LightPink { get { return _colors[69]; } }

    /// <summary>
    /// Gets the named color "LightSalmon".
    /// </summary>
    public static NamedColor LightSalmon { get { return _colors[70]; } }

    /// <summary>
    /// Gets the named color "LightSeaGreen".
    /// </summary>
    public static NamedColor LightSeaGreen { get { return _colors[71]; } }

    /// <summary>
    /// Gets the named color "LightSkyBlue".
    /// </summary>
    public static NamedColor LightSkyBlue { get { return _colors[72]; } }

    /// <summary>
    /// Gets the named color "LightSlateGray".
    /// </summary>
    public static NamedColor LightSlateGray { get { return _colors[73]; } }

    /// <summary>
    /// Gets the named color "LightSteelBlue".
    /// </summary>
    public static NamedColor LightSteelBlue { get { return _colors[74]; } }

    /// <summary>
    /// Gets the named color "LightYellow".
    /// </summary>
    public static NamedColor LightYellow { get { return _colors[75]; } }

    /// <summary>
    /// Gets the named color "Lime".
    /// </summary>
    public static NamedColor Lime { get { return _colors[76]; } }

    /// <summary>
    /// Gets the named color "LimeGreen".
    /// </summary>
    public static NamedColor LimeGreen { get { return _colors[77]; } }

    /// <summary>
    /// Gets the named color "Linen".
    /// </summary>
    public static NamedColor Linen { get { return _colors[78]; } }

    /// <summary>
    /// Gets the named color "Magenta".
    /// </summary>
    public static NamedColor Magenta { get { return _colors[79]; } }

    /// <summary>
    /// Gets the named color "Maroon".
    /// </summary>
    public static NamedColor Maroon { get { return _colors[80]; } }

    /// <summary>
    /// Gets the named color "MediumAquamarine".
    /// </summary>
    public static NamedColor MediumAquamarine { get { return _colors[81]; } }

    /// <summary>
    /// Gets the named color "MediumBlue".
    /// </summary>
    public static NamedColor MediumBlue { get { return _colors[82]; } }

    /// <summary>
    /// Gets the named color "MediumOrchid".
    /// </summary>
    public static NamedColor MediumOrchid { get { return _colors[83]; } }

    /// <summary>
    /// Gets the named color "MediumPurple".
    /// </summary>
    public static NamedColor MediumPurple { get { return _colors[84]; } }

    /// <summary>
    /// Gets the named color "MediumSeaGreen".
    /// </summary>
    public static NamedColor MediumSeaGreen { get { return _colors[85]; } }

    /// <summary>
    /// Gets the named color "MediumSlateBlue".
    /// </summary>
    public static NamedColor MediumSlateBlue { get { return _colors[86]; } }

    /// <summary>
    /// Gets the named color "MediumSpringGreen".
    /// </summary>
    public static NamedColor MediumSpringGreen { get { return _colors[87]; } }

    /// <summary>
    /// Gets the named color "MediumTurquoise".
    /// </summary>
    public static NamedColor MediumTurquoise { get { return _colors[88]; } }

    /// <summary>
    /// Gets the named color "MediumVioletRed".
    /// </summary>
    public static NamedColor MediumVioletRed { get { return _colors[89]; } }

    /// <summary>
    /// Gets the named color "MidnightBlue".
    /// </summary>
    public static NamedColor MidnightBlue { get { return _colors[90]; } }

    /// <summary>
    /// Gets the named color "MintCream".
    /// </summary>
    public static NamedColor MintCream { get { return _colors[91]; } }

    /// <summary>
    /// Gets the named color "MistyRose".
    /// </summary>
    public static NamedColor MistyRose { get { return _colors[92]; } }

    /// <summary>
    /// Gets the named color "Moccasin".
    /// </summary>
    public static NamedColor Moccasin { get { return _colors[93]; } }

    /// <summary>
    /// Gets the named color "NavajoWhite".
    /// </summary>
    public static NamedColor NavajoWhite { get { return _colors[94]; } }

    /// <summary>
    /// Gets the named color "Navy".
    /// </summary>
    public static NamedColor Navy { get { return _colors[95]; } }

    /// <summary>
    /// Gets the named color "OldLace".
    /// </summary>
    public static NamedColor OldLace { get { return _colors[96]; } }

    /// <summary>
    /// Gets the named color "Olive".
    /// </summary>
    public static NamedColor Olive { get { return _colors[97]; } }

    /// <summary>
    /// Gets the named color "OliveDrab".
    /// </summary>
    public static NamedColor OliveDrab { get { return _colors[98]; } }

    /// <summary>
    /// Gets the named color "Orange".
    /// </summary>
    public static NamedColor Orange { get { return _colors[99]; } }

    /// <summary>
    /// Gets the named color "OrangeRed".
    /// </summary>
    public static NamedColor OrangeRed { get { return _colors[100]; } }

    /// <summary>
    /// Gets the named color "Orchid".
    /// </summary>
    public static NamedColor Orchid { get { return _colors[101]; } }

    /// <summary>
    /// Gets the named color "PaleGoldenrod".
    /// </summary>
    public static NamedColor PaleGoldenrod { get { return _colors[102]; } }

    /// <summary>
    /// Gets the named color "PaleGreen".
    /// </summary>
    public static NamedColor PaleGreen { get { return _colors[103]; } }

    /// <summary>
    /// Gets the named color "PaleTurquoise".
    /// </summary>
    public static NamedColor PaleTurquoise { get { return _colors[104]; } }

    /// <summary>
    /// Gets the named color "PaleVioletRed".
    /// </summary>
    public static NamedColor PaleVioletRed { get { return _colors[105]; } }

    /// <summary>
    /// Gets the named color "PapayaWhip".
    /// </summary>
    public static NamedColor PapayaWhip { get { return _colors[106]; } }

    /// <summary>
    /// Gets the named color "PeachPuff".
    /// </summary>
    public static NamedColor PeachPuff { get { return _colors[107]; } }

    /// <summary>
    /// Gets the named color "Peru".
    /// </summary>
    public static NamedColor Peru { get { return _colors[108]; } }

    /// <summary>
    /// Gets the named color "Pink".
    /// </summary>
    public static NamedColor Pink { get { return _colors[109]; } }

    /// <summary>
    /// Gets the named color "Plum".
    /// </summary>
    public static NamedColor Plum { get { return _colors[110]; } }

    /// <summary>
    /// Gets the named color "PowderBlue".
    /// </summary>
    public static NamedColor PowderBlue { get { return _colors[111]; } }

    /// <summary>
    /// Gets the named color "Purple".
    /// </summary>
    public static NamedColor Purple { get { return _colors[112]; } }

    /// <summary>
    /// Gets the named color "Red".
    /// </summary>
    public static NamedColor Red { get { return _colors[113]; } }

    /// <summary>
    /// Gets the named color "RosyBrown".
    /// </summary>
    public static NamedColor RosyBrown { get { return _colors[114]; } }

    /// <summary>
    /// Gets the named color "RoyalBlue".
    /// </summary>
    public static NamedColor RoyalBlue { get { return _colors[115]; } }

    /// <summary>
    /// Gets the named color "SaddleBrown".
    /// </summary>
    public static NamedColor SaddleBrown { get { return _colors[116]; } }

    /// <summary>
    /// Gets the named color "Salmon".
    /// </summary>
    public static NamedColor Salmon { get { return _colors[117]; } }

    /// <summary>
    /// Gets the named color "SandyBrown".
    /// </summary>
    public static NamedColor SandyBrown { get { return _colors[118]; } }

    /// <summary>
    /// Gets the named color "SeaGreen".
    /// </summary>
    public static NamedColor SeaGreen { get { return _colors[119]; } }

    /// <summary>
    /// Gets the named color "SeaShell".
    /// </summary>
    public static NamedColor SeaShell { get { return _colors[120]; } }

    /// <summary>
    /// Gets the named color "Sienna".
    /// </summary>
    public static NamedColor Sienna { get { return _colors[121]; } }

    /// <summary>
    /// Gets the named color "Silver".
    /// </summary>
    public static NamedColor Silver { get { return _colors[122]; } }

    /// <summary>
    /// Gets the named color "SkyBlue".
    /// </summary>
    public static NamedColor SkyBlue { get { return _colors[123]; } }

    /// <summary>
    /// Gets the named color "SlateBlue".
    /// </summary>
    public static NamedColor SlateBlue { get { return _colors[124]; } }

    /// <summary>
    /// Gets the named color "SlateGray".
    /// </summary>
    public static NamedColor SlateGray { get { return _colors[125]; } }

    /// <summary>
    /// Gets the named color "Snow".
    /// </summary>
    public static NamedColor Snow { get { return _colors[126]; } }

    /// <summary>
    /// Gets the named color "SpringGreen".
    /// </summary>
    public static NamedColor SpringGreen { get { return _colors[127]; } }

    /// <summary>
    /// Gets the named color "SteelBlue".
    /// </summary>
    public static NamedColor SteelBlue { get { return _colors[128]; } }

    /// <summary>
    /// Gets the named color "Tan".
    /// </summary>
    public static NamedColor Tan { get { return _colors[129]; } }

    /// <summary>
    /// Gets the named color "Teal".
    /// </summary>
    public static NamedColor Teal { get { return _colors[130]; } }

    /// <summary>
    /// Gets the named color "Thistle".
    /// </summary>
    public static NamedColor Thistle { get { return _colors[131]; } }

    /// <summary>
    /// Gets the named color "Tomato".
    /// </summary>
    public static NamedColor Tomato { get { return _colors[132]; } }

    /// <summary>
    /// Gets the named color "Transparent".
    /// </summary>
    public static NamedColor Transparent { get { return _colors[133]; } }

    /// <summary>
    /// Gets the named color "Turquoise".
    /// </summary>
    public static NamedColor Turquoise { get { return _colors[134]; } }

    /// <summary>
    /// Gets the named color "Violet".
    /// </summary>
    public static NamedColor Violet { get { return _colors[135]; } }

    /// <summary>
    /// Gets the named color "Wheat".
    /// </summary>
    public static NamedColor Wheat { get { return _colors[136]; } }

    /// <summary>
    /// Gets the named color "White".
    /// </summary>
    public static NamedColor White { get { return _colors[137]; } }

    /// <summary>
    /// Gets the named color "WhiteSmoke".
    /// </summary>
    public static NamedColor WhiteSmoke { get { return _colors[138]; } }

    /// <summary>
    /// Gets the named color "Yellow".
    /// </summary>
    public static NamedColor Yellow { get { return _colors[139]; } }

    /// <summary>
    /// Gets the named color "YellowGreen".
    /// </summary>
    public static NamedColor YellowGreen { get { return _colors[140]; } }


    #endregion Auto generated code
  }
}
