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
  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct BITMAPFILEHEADER
  {
    public UInt16 Type;
    public UInt32 Size;
    public UInt16 Reserved1;
    public UInt16 Reserved2;
    public UInt32 OffBits;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct ENHMETAHEADER
  {
    public uint iType;
    public int nSize;
    public RECT rclBounds;
    public RECT rclFrame;
    public uint dSignature;
    public uint nVersion;
    public uint nBytes;
    public uint nRecords;
    public ushort nHandles;
    public ushort sReserved;
    public uint nDescription;
    public uint offDescription;
    public uint nPalEntries;
    public SIZE szlDevice;
    public SIZE szlMillimeters;
    public uint cbPixelFormat;
    public uint offPixelFormat;
    public uint bOpenGL;
    public SIZE szlMicrometers;
  }

  [StructLayout(LayoutKind.Sequential)] // Do not use Pack = 1 here (it is definitely not working with Pack=1)!
  public struct METAFILEPICT
  {
    public int mm;
    public int xExt;
    public int yExt;
    public IntPtr hMF;
  };

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct WmfPlaceableFileHeader
  {
    public uint key;  // 0x9aC6CDD7
    public ushort hmf;
    public ushort bboxLeft;
    public ushort bboxTop;
    public ushort bboxRight;
    public ushort bboxBottom;
    public ushort inch;
    public uint reserved;
    public ushort checksum;
  }

  [Serializable, StructLayout(LayoutKind.Sequential)]
  public struct RECT
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public static implicit operator RECT(System.Drawing.Rectangle r)
    {
      RECT rect = new RECT();
      rect.Left = r.Left;
      rect.Top = r.Top;
      rect.Right = r.Right;
      rect.Bottom = r.Bottom;
      return rect;
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct SIZE
  {
    public int cx;
    public int cy;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct DROPFILES
  {
    public Int32 pFiles;
    public System.Drawing.Point pt;
    public UInt32 fNC;
    public UInt32 fWide;
  }

  #region BitmapV5

  [StructLayout(LayoutKind.Sequential)]
  public struct CIEXYZ
  {
    public uint ciexyzX; //FXPT2DOT30
    public uint ciexyzY; //FXPT2DOT30
    public uint ciexyzZ; //FXPT2DOT30
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct CIEXYZTRIPLE
  {
    public CIEXYZ ciexyzRed;
    public CIEXYZ ciexyzGreen;
    public CIEXYZ ciexyzBlue;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct BITFIELDS
  {
    public uint BlueMask;
    public uint GreenMask;
    public uint RedMask;
  }

  [StructLayout(LayoutKind.Explicit)]
  public struct BITMAPV5HEADER
  {
    [FieldOffset(0)]
    public uint bV5Size;

    [FieldOffset(4)]
    public int bV5Width;

    [FieldOffset(8)]
    public int bV5Height;

    [FieldOffset(12)]
    public ushort bV5Planes;

    [FieldOffset(14)]
    public ushort bV5BitCount;

    [FieldOffset(16)]
    public uint bV5Compression;

    [FieldOffset(20)]
    public uint bV5SizeImage;

    [FieldOffset(24)]
    public int bV5XPelsPerMeter;

    [FieldOffset(28)]
    public int bV5YPelsPerMeter;

    [FieldOffset(32)]
    public uint bV5ClrUsed;

    [FieldOffset(36)]
    public uint bV5ClrImportant;

    [FieldOffset(40)]
    public uint bV5RedMask;

    [FieldOffset(44)]
    public uint bV5GreenMask;

    [FieldOffset(48)]
    public uint bV5BlueMask;

    [FieldOffset(52)]
    public uint bV5AlphaMask;

    [FieldOffset(56)]
    public uint bV5CSType;

    [FieldOffset(60)]
    public CIEXYZTRIPLE bV5Endpoints;

    [FieldOffset(96)]
    public uint bV5GammaRed;

    [FieldOffset(100)]
    public uint bV5GammaGreen;

    [FieldOffset(104)]
    public uint bV5GammaBlue;

    [FieldOffset(108)]
    public uint bV5Intent;

    [FieldOffset(112)]
    public uint bV5ProfileData;

    [FieldOffset(116)]
    public uint bV5ProfileSize;

    [FieldOffset(120)]
    public uint bV5Reserved;
  }

  #endregion BitmapV5
}
