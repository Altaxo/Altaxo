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