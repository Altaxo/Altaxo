#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.UnmanagedApi.Gdi32
{
  /// <summary>
  /// Defines mapping mode constants for GDI device contexts.
  /// </summary>
  public static class MappingMode
  {
    /// <summary>Text mapping mode.</summary>
    public const int MM_TEXT = 1;
    /// <summary>Low metric mapping mode.</summary>
    public const int MM_LOMETRIC = 2;
    /// <summary>High metric mapping mode.</summary>
    public const int MM_HIMETRIC = 3;
    /// <summary>Low English mapping mode.</summary>
    public const int MM_LOENGLISH = 4;
    /// <summary>High English mapping mode.</summary>
    public const int MM_HIENGLISH = 5;
    /// <summary>Twips mapping mode.</summary>
    public const int MM_TWIPS = 6;
    /// <summary>Isotropic mapping mode.</summary>
    public const int MM_ISOTROPIC = 7;
    /// <summary>Anisotropic mapping mode.</summary>
    public const int MM_ANISOTROPIC = 8;
  }

  /// <summary>
  /// Defines device capability indexes for <c>GetDeviceCaps</c>.
  /// </summary>
  public static class DeviceCap
  {
    /// <summary>Logical pixels per inch in the X direction.</summary>
    public const int LOGPIXELSX = 88;
    /// <summary>Logical pixels per inch in the Y direction.</summary>
    public const int LOGPIXELSY = 90;

    /// <summary>Physical width in millimeters.</summary>
    public const int HORZSIZE = 4;
    /// <summary>Physical height in millimeters.</summary>
    public const int VERTSIZE = 6;
    /// <summary>Width in pixels.</summary>
    public const int HORZRES = 8;
    /// <summary>Height in pixels.</summary>
    public const int VERTRES = 10;
  }

  /// <summary>
  ///     Specifies a raster-operation code. These codes define how the color data for the
  ///     source rectangle is to be combined with the color data for the destination
  ///     rectangle to achieve the final color.
  /// </summary>
  public enum TernaryRasterOperations : uint
  {
    /// <summary>dest = source</summary>
    SRCCOPY = 0x00CC0020,

    /// <summary>dest = source OR dest</summary>
    SRCPAINT = 0x00EE0086,

    /// <summary>dest = source AND dest</summary>
    SRCAND = 0x008800C6,

    /// <summary>dest = source XOR dest</summary>
    SRCINVERT = 0x00660046,

    /// <summary>dest = source AND (NOT dest)</summary>
    SRCERASE = 0x00440328,

    /// <summary>dest = (NOT source)</summary>
    NOTSRCCOPY = 0x00330008,

    /// <summary>dest = (NOT src) AND (NOT dest)</summary>
    NOTSRCERASE = 0x001100A6,

    /// <summary>dest = (source AND pattern)</summary>
    MERGECOPY = 0x00C000CA,

    /// <summary>dest = (NOT source) OR dest</summary>
    MERGEPAINT = 0x00BB0226,

    /// <summary>dest = pattern</summary>
    PATCOPY = 0x00F00021,

    /// <summary>dest = DPSnoo</summary>
    PATPAINT = 0x00FB0A09,

    /// <summary>dest = pattern XOR dest</summary>
    PATINVERT = 0x005A0049,

    /// <summary>dest = (NOT dest)</summary>
    DSTINVERT = 0x00550009,

    /// <summary>dest = BLACK</summary>
    BLACKNESS = 0x00000042,

    /// <summary>dest = WHITE</summary>
    WHITENESS = 0x00FF0062,

    /// <summary>
    /// Capture window as seen on screen.  This includes layered windows
    /// such as WPF windows with AllowsTransparency="true"
    /// </summary>
    CAPTUREBLT = 0x40000000
  }

  /// <summary>
  /// Defines logical color space types.
  /// </summary>
  public enum LcsCsType : uint
  {
    /// <summary>Calibrated RGB color space.</summary>
    LCS_CALIBRATED_RGB = 0,
    /// <summary>sRGB color space.</summary>
    LCS_SRGB = 0x42475272, // Ascii for 'sRGB'
    /// <summary>Windows default color space.</summary>
    LCS_WINDOWS_COLOR_SPACE = 0x206E6957, // Ascii for 'Win '
    /// <summary>Linked profile.</summary>
    PROFILE_LINKED = 3,
    /// <summary>Embedded profile.</summary>
    PROFILE_EMBEDDED = 4
  }

  /// <summary>
  /// Defines bitmap compression types.
  /// </summary>
  public enum BiCompression
  {
    /// <summary>No compression.</summary>
    BI_RGB = 0,
    /// <summary>RLE compression with 8 bits per pixel.</summary>
    BI_RLE8 = 1,
    /// <summary>RLE compression with 4 bits per pixel.</summary>
    BI_RLE4 = 2,
    /// <summary>Bitfields compression.</summary>
    BI_BITFIELDS = 3,
    /// <summary>JPEG compression.</summary>
    BI_JPEG = 4,
    /// <summary>PNG compression.</summary>
    BI_PNG = 5,
  }

  /// <summary>
  /// Defines color rendering intents.
  /// </summary>
  [Flags]
  public enum LcsIntent
  {
    /// <summary>Business rendering intent.</summary>
    LCS_GM_BUSINESS = 1,
    /// <summary>Graphics rendering intent.</summary>
    LCS_GM_GRAPHICS = 2,
    /// <summary>Images rendering intent.</summary>
    LCS_GM_IMAGES = 4,
    /// <summary>Absolute colorimetric rendering intent.</summary>
    LCS_GM_ABS_COLORIMETRIC = 8
  }
}
