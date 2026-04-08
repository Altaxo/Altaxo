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
using System.Runtime.InteropServices;
using System.Text;

namespace Altaxo.UnmanagedApi.Gdi32
{
  /// <summary>
  /// Represents a bitmap file header.
  /// </summary>
  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct BITMAPFILEHEADER
  {
    /// <summary>The file type.</summary>
    public ushort Type;
    /// <summary>The file size in bytes.</summary>
    public uint Size;
    /// <summary>The first reserved field.</summary>
    public ushort Reserved1;
    /// <summary>The second reserved field.</summary>
    public ushort Reserved2;
    /// <summary>The offset to the bitmap bits.</summary>
    public uint OffBits;
  }

  /// <summary>
  /// Represents the header of an enhanced metafile.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct ENHMETAHEADER
  {
    /// <summary>The record type.</summary>
    public uint iType;
    /// <summary>The size of the header, in bytes.</summary>
    public int nSize;
    /// <summary>The bounds rectangle in device units.</summary>
    public RECT rclBounds;
    /// <summary>The frame rectangle in .01-millimeter units.</summary>
    public RECT rclFrame;
    /// <summary>The signature.</summary>
    public uint dSignature;
    /// <summary>The version.</summary>
    public uint nVersion;
    /// <summary>The metafile size in bytes.</summary>
    public uint nBytes;
    /// <summary>The number of records.</summary>
    public uint nRecords;
    /// <summary>The number of handles.</summary>
    public ushort nHandles;
    /// <summary>The reserved field.</summary>
    public ushort sReserved;
    /// <summary>The length of the description string.</summary>
    public uint nDescription;
    /// <summary>The offset of the description string.</summary>
    public uint offDescription;
    /// <summary>The number of palette entries.</summary>
    public uint nPalEntries;
    /// <summary>The device size in pixels.</summary>
    public SIZE szlDevice;
    /// <summary>The device size in millimeters.</summary>
    public SIZE szlMillimeters;
    /// <summary>The size of the pixel format descriptor.</summary>
    public uint cbPixelFormat;
    /// <summary>The offset of the pixel format descriptor.</summary>
    public uint offPixelFormat;
    /// <summary>Indicates whether OpenGL was used.</summary>
    public uint bOpenGL;
    /// <summary>The device size in micrometers.</summary>
    public SIZE szlMicrometers;
  }

  /// <summary>
  /// Represents a metafile picture.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)] // Do not use Pack = 1 here (it is definitely not working with Pack=1)!
  public struct METAFILEPICT
  {
    /// <summary>The mapping mode.</summary>
    public int mm;
    /// <summary>The width in .01-millimeter units.</summary>
    public int xExt;
    /// <summary>The height in .01-millimeter units.</summary>
    public int yExt;
    /// <summary>The metafile handle.</summary>
    public IntPtr hMF;
  };

  /// <summary>
  /// Represents the header of a placeable WMF file.
  /// </summary>
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct WmfPlaceableFileHeader
  {
    /// <summary>The header key.</summary>
    public uint key;  // 0x9aC6CDD7
    /// <summary>The metafile handle.</summary>
    public ushort hmf;
    /// <summary>The left bounding coordinate.</summary>
    public ushort bboxLeft;
    /// <summary>The top bounding coordinate.</summary>
    public ushort bboxTop;
    /// <summary>The right bounding coordinate.</summary>
    public ushort bboxRight;
    /// <summary>The bottom bounding coordinate.</summary>
    public ushort bboxBottom;
    /// <summary>The number of twips per inch.</summary>
    public ushort inch;
    /// <summary>The reserved field.</summary>
    public uint reserved;
    /// <summary>The checksum.</summary>
    public ushort checksum;
  }

  /// <summary>
  /// Represents a rectangle.
  /// </summary>
  [Serializable, StructLayout(LayoutKind.Sequential)]
  public struct RECT
  {
    /// <summary>The left coordinate.</summary>
    public int Left;
    /// <summary>The top coordinate.</summary>
    public int Top;
    /// <summary>The right coordinate.</summary>
    public int Right;
    /// <summary>The bottom coordinate.</summary>
    public int Bottom;

    /// <summary>
    /// Converts a <see cref="System.Drawing.Rectangle"/> to a <see cref="RECT"/>.
    /// </summary>
    /// <param name="r">The source rectangle.</param>
    public static implicit operator RECT(System.Drawing.Rectangle r)
    {
      var rect = new RECT
      {
        Left = r.Left,
        Top = r.Top,
        Right = r.Right,
        Bottom = r.Bottom
      };
      return rect;
    }
  }

  /// <summary>
  /// Represents a size.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct SIZE
  {
    /// <summary>The width.</summary>
    public int cx;
    /// <summary>The height.</summary>
    public int cy;
  }

  /// <summary>
  /// Represents file information used for drag-and-drop operations.
  /// </summary>
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct DROPFILES
  {
    /// <summary>The offset to the file list.</summary>
    public int pFiles;
    /// <summary>The drop point.</summary>
    public System.Drawing.Point pt;
    /// <summary>Indicates whether the coordinates are in a nonclient area.</summary>
    public uint fNC;
    /// <summary>Indicates whether the file list uses Unicode.</summary>
    public uint fWide;
  }

  #region BitmapV5

  /// <summary>
  /// Represents a CIE XYZ color.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct CIEXYZ
  {
    /// <summary>The X component.</summary>
    public uint ciexyzX; //FXPT2DOT30
    /// <summary>The Y component.</summary>
    public uint ciexyzY; //FXPT2DOT30
    /// <summary>The Z component.</summary>
    public uint ciexyzZ; //FXPT2DOT30
  }

  /// <summary>
  /// Represents a triple of CIE XYZ colors for red, green, and blue endpoints.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct CIEXYZTRIPLE
  {
    /// <summary>The red endpoint.</summary>
    public CIEXYZ ciexyzRed;
    /// <summary>The green endpoint.</summary>
    public CIEXYZ ciexyzGreen;
    /// <summary>The blue endpoint.</summary>
    public CIEXYZ ciexyzBlue;
  }

  /// <summary>
  /// Represents RGB bit masks.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct BITFIELDS
  {
    /// <summary>The blue mask.</summary>
    public uint BlueMask;
    /// <summary>The green mask.</summary>
    public uint GreenMask;
    /// <summary>The red mask.</summary>
    public uint RedMask;
  }

  /// <summary>
  /// Represents a version 5 bitmap header.
  /// </summary>
  [StructLayout(LayoutKind.Explicit)]
  public struct BITMAPV5HEADER
  {
    /// <summary>The size of this structure.</summary>
    [FieldOffset(0)]
    public uint bV5Size;

    /// <summary>The bitmap width.</summary>
    [FieldOffset(4)]
    public int bV5Width;

    /// <summary>The bitmap height.</summary>
    [FieldOffset(8)]
    public int bV5Height;

    /// <summary>The number of planes.</summary>
    [FieldOffset(12)]
    public ushort bV5Planes;

    /// <summary>The bit count.</summary>
    [FieldOffset(14)]
    public ushort bV5BitCount;

    /// <summary>The compression type.</summary>
    [FieldOffset(16)]
    public uint bV5Compression;

    /// <summary>The image size.</summary>
    [FieldOffset(20)]
    public uint bV5SizeImage;

    /// <summary>The horizontal resolution.</summary>
    [FieldOffset(24)]
    public int bV5XPelsPerMeter;

    /// <summary>The vertical resolution.</summary>
    [FieldOffset(28)]
    public int bV5YPelsPerMeter;

    /// <summary>The number of colors used.</summary>
    [FieldOffset(32)]
    public uint bV5ClrUsed;

    /// <summary>The number of important colors.</summary>
    [FieldOffset(36)]
    public uint bV5ClrImportant;

    /// <summary>The red mask.</summary>
    [FieldOffset(40)]
    public uint bV5RedMask;

    /// <summary>The green mask.</summary>
    [FieldOffset(44)]
    public uint bV5GreenMask;

    /// <summary>The blue mask.</summary>
    [FieldOffset(48)]
    public uint bV5BlueMask;

    /// <summary>The alpha mask.</summary>
    [FieldOffset(52)]
    public uint bV5AlphaMask;

    /// <summary>The color space type.</summary>
    [FieldOffset(56)]
    public uint bV5CSType;

    /// <summary>The color endpoints.</summary>
    [FieldOffset(60)]
    public CIEXYZTRIPLE bV5Endpoints;

    /// <summary>The red gamma value.</summary>
    [FieldOffset(96)]
    public uint bV5GammaRed;

    /// <summary>The green gamma value.</summary>
    [FieldOffset(100)]
    public uint bV5GammaGreen;

    /// <summary>The blue gamma value.</summary>
    [FieldOffset(104)]
    public uint bV5GammaBlue;

    /// <summary>The rendering intent.</summary>
    [FieldOffset(108)]
    public uint bV5Intent;

    /// <summary>The profile data offset.</summary>
    [FieldOffset(112)]
    public uint bV5ProfileData;

    /// <summary>The profile size.</summary>
    [FieldOffset(116)]
    public uint bV5ProfileSize;

    /// <summary>The reserved field.</summary>
    [FieldOffset(120)]
    public uint bV5Reserved;
  }

  #endregion BitmapV5
}
