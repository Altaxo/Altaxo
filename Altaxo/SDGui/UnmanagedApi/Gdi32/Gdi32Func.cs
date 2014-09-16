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
	public static class Gdi32Func
	{
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateSolidBrush(uint crColor);

		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		[DllImport("gdi32.dll")]
		public static extern int GetObject(IntPtr hgdiobj, int cbBuffer, IntPtr lpvObject);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern bool GdiFlush();

		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern Int32 GetDeviceCaps(IntPtr hdc, Int32 capindex);

		[DllImport("gdi32.dll")]
		public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart,
			 string lpString, int cbString);

		[DllImport("gdi32.dll")]
		public static extern uint GetWinMetaFileBits(IntPtr hemf, uint cbBuffer,
				[Out] byte[] lpbBuffer, int fnMapMode, IntPtr hdcRef);

		[DllImport("gdi32.dll")]
		public static extern IntPtr SetMetaFileBitsEx(uint nSize, byte[] lpData);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateMetaFile(string lpszFile);

		[DllImport("gdi32.dll")]
		public static extern int SetMapMode(IntPtr hdc, int fnMapMode);

		[DllImport("gdi32.dll")]
		public static extern bool SetWindowOrgEx(IntPtr hdc, int X, int Y, System.IntPtr lpPoint);

		[DllImport("gdi32.dll")]
		public static extern bool SetWindowExtEx(IntPtr hdc, int nXExtent, int nYExtent,
			 IntPtr lpSize);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CloseMetaFile(IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CopyMetaFile(IntPtr hmfSrc, string lpszFile);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteMetaFile(IntPtr hWmf);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteEnhMetaFile(IntPtr hEmf);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth,
			 int nHeight);

		[DllImport("gdi32.dll")]
		public static extern bool SetViewportExtEx(IntPtr hdc, int nXExtent, int nYExtent, IntPtr lpSize);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateEnhMetaFile(IntPtr hdcRef, string lpFilename, [In] ref RECT lpRect, string lpDescription);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CloseEnhMetaFile(IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern uint GetEnhMetaFileHeader(IntPtr hemf, uint cbBuffer, IntPtr lpemh);

		[DllImport("user32.dll")]
		public static extern int FillRect(IntPtr hDC, [In] ref RECT lprc, IntPtr hbr);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);

		[DllImport("gdi32.dll")]
		public static extern bool LineTo(IntPtr hdc, int nXEnd, int nYEnd);

		[DllImport("gdi32.dll")]
		public static extern bool MoveToEx(IntPtr hdc, int X, int Y, IntPtr zero);

		/// <summary>
		///    Performs a bit-block transfer of the color data corresponding to a
		///    rectangle of pixels from the specified source device context into
		///    a destination device context.
		/// </summary>
		/// <param name="hdc">Handle to the destination device context.</param>
		/// <param name="nXDest">The leftmost x-coordinate of the destination rectangle (in pixels).</param>
		/// <param name="nYDest">The topmost y-coordinate of the destination rectangle (in pixels).</param>
		/// <param name="nWidth">The width of the source and destination rectangles (in pixels).</param>
		/// <param name="nHeight">The height of the source and the destination rectangles (in pixels).</param>
		/// <param name="hdcSrc">Handle to the source device context.</param>
		/// <param name="nXSrc">The leftmost x-coordinate of the source rectangle (in pixels).</param>
		/// <param name="nYSrc">The topmost y-coordinate of the source rectangle (in pixels).</param>
		/// <param name="dwRop">A raster-operation code.</param>
		/// <returns>
		///    <c>true</c> if the operation succeedes, <c>false</c> otherwise. To get extended error information, call <see cref="System.Runtime.InteropServices.Marshal.GetLastWin32Error"/>.
		/// </returns>
		[DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

		[DllImport("gdi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool StretchBlt([In] IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, [In] IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, TernaryRasterOperations dwRop);
	}
}