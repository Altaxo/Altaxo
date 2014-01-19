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

	[StructLayout(LayoutKind.Sequential)]
	public struct METAFILEPICT
	{
		public int mm;
		public int xExt;
		public int yExt;
		public IntPtr hMF;
	};

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
}