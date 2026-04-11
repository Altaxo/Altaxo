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
  /// Provides unmanaged GDI32 function imports.
  /// </summary>
  public static class Gdi32Func
  {
    /// <summary>
    /// Creates a solid brush.
    /// </summary>
    /// <param name="crColor">The brush color as a packed RGB value.</param>
    /// <returns>A handle to the created brush, or <see cref="IntPtr.Zero"/> on failure.</returns>
    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateSolidBrush(uint crColor);

    /// <summary>
    /// Creates a memory device context compatible with the specified device.
    /// </summary>
    /// <param name="hdc">A handle to an existing device context.</param>
    /// <returns>A handle to the compatible memory device context, or <see cref="IntPtr.Zero"/> on failure.</returns>
    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    /// <summary>
    /// Selects an object into the specified device context.
    /// </summary>
    /// <param name="hdc">A handle to the target device context.</param>
    /// <param name="hgdiobj">A handle to the object to select.</param>
    /// <returns>The handle to the object previously selected into the device context.</returns>
    [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

    /// <summary>
    /// Retrieves information about the specified graphics object.
    /// </summary>
    /// <param name="hgdiobj">A handle to the graphics object.</param>
    /// <param name="cbBuffer">The size of the buffer pointed to by <paramref name="lpvObject"/>.</param>
    /// <param name="lpvObject">A pointer to the buffer that receives the object information.</param>
    /// <returns>The number of bytes written to the buffer.</returns>
    [DllImport("gdi32.dll")]
    public static extern int GetObject(IntPtr hgdiobj, int cbBuffer, IntPtr lpvObject);

    /// <summary>
    /// Deletes a logical pen, brush, font, bitmap, region, or palette.
    /// </summary>
    /// <param name="hObject">A handle to the GDI object to delete.</param>
    /// <returns><see langword="true"/> if the object was deleted; otherwise, <see langword="false"/>.</returns>
    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);

    /// <summary>
    /// Deletes the specified device context.
    /// </summary>
    /// <param name="hdc">A handle to the device context to delete.</param>
    /// <returns><see langword="true"/> if the device context was deleted; otherwise, <see langword="false"/>.</returns>
    [DllImport("gdi32.dll")]
    public static extern bool DeleteDC(IntPtr hdc);

    /// <summary>
    /// Flushes the GDI command buffer.
    /// </summary>
    /// <returns><see langword="true"/> if all pending drawing commands were flushed; otherwise, <see langword="false"/>.</returns>
    [DllImport("gdi32.dll")]
    public static extern bool GdiFlush();

    /// <summary>
    /// Retrieves device-specific information for the specified device.
    /// </summary>
    /// <param name="hdc">A handle to the device context.</param>
    /// <param name="capindex">The device capability index to query.</param>
    /// <returns>The requested device capability value.</returns>
    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern int GetDeviceCaps(IntPtr hdc, int capindex);

    /// <summary>
    /// Writes a character string at the specified location.
    /// </summary>
    /// <param name="hdc">A handle to the device context.</param>
    /// <param name="nXStart">The x-coordinate of the reference point.</param>
    /// <param name="nYStart">The y-coordinate of the reference point.</param>
    /// <param name="lpString">The text to draw.</param>
    /// <param name="cbString">The number of characters to draw.</param>
    /// <returns><see langword="true"/> if the text was drawn; otherwise, <see langword="false"/>.</returns>
    [DllImport("gdi32.dll")]
    public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart,
       string lpString, int cbString);

    /// <summary>
    /// Retrieves the bits of an enhanced metafile as a Windows-format metafile.
    /// </summary>
    /// <param name="hemf">A handle to the enhanced metafile.</param>
    /// <param name="cbBuffer">The size of the output buffer, in bytes.</param>
    /// <param name="lpbBuffer">The buffer that receives the metafile bits.</param>
    /// <param name="fnMapMode">The mapping mode for the metafile.</param>
    /// <param name="hdcRef">A reference device context for the mapping mode.</param>
    /// <returns>The number of bytes copied to <paramref name="lpbBuffer"/>.</returns>
    [DllImport("gdi32.dll")]
    public static extern uint GetWinMetaFileBits(IntPtr hemf, uint cbBuffer,
        [Out] byte[] lpbBuffer, int fnMapMode, IntPtr hdcRef);

    /// <summary>
    /// Creates a metafile from the specified memory buffer.
    /// </summary>
    /// <param name="nSize">The size of the metafile data, in bytes.</param>
    /// <param name="lpData">The metafile data buffer.</param>
    /// <returns>A handle to the created metafile.</returns>
    [DllImport("gdi32.dll")]
    public static extern IntPtr SetMetaFileBitsEx(uint nSize, byte[] lpData);

    /// <summary>
    /// Retrieves a handle to a display device context.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose device context is requested.</param>
    /// <returns>A handle to the display device context.</returns>
    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hWnd);

    /// <summary>
    /// Creates a device context for a Windows-format metafile.
    /// </summary>
    /// <param name="lpszFile">The file name for the metafile, or <see langword="null"/> for a memory metafile.</param>
    /// <returns>A handle to the metafile device context.</returns>
    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateMetaFile(string lpszFile);

    /// <summary>
    /// Sets the mapping mode of the specified device context.
    /// </summary>
    /// <param name="hdc">A handle to the device context.</param>
    /// <param name="fnMapMode">The new mapping mode.</param>
    /// <returns>The previous mapping mode.</returns>
    [DllImport("gdi32.dll")]
    public static extern int SetMapMode(IntPtr hdc, int fnMapMode);

    /// <summary>
    /// Sets the window origin of the specified device context.
    /// </summary>
    /// <param name="hdc">A handle to the device context.</param>
    /// <param name="X">The new x-coordinate of the window origin.</param>
    /// <param name="Y">The new y-coordinate of the window origin.</param>
    /// <param name="lpPoint">A pointer that receives the previous origin, or <see cref="IntPtr.Zero"/>.</param>
    /// <returns><see langword="true"/> if the window origin was set; otherwise, <see langword="false"/>.</returns>
    [DllImport("gdi32.dll")]
    public static extern bool SetWindowOrgEx(IntPtr hdc, int X, int Y, System.IntPtr lpPoint);

    /// <summary>
    /// Sets the window extent of the specified device context.
    /// </summary>
    /// <param name="hdc">A handle to the device context.</param>
    /// <param name="nXExtent">The horizontal extent.</param>
    /// <param name="nYExtent">The vertical extent.</param>
    /// <param name="lpSize">A pointer that receives the previous extent, or <see cref="IntPtr.Zero"/>.</param>
    /// <returns><see langword="true"/> if the window extent was set; otherwise, <see langword="false"/>.</returns>
    [DllImport("gdi32.dll")]
    public static extern bool SetWindowExtEx(IntPtr hdc, int nXExtent, int nYExtent,
       IntPtr lpSize);

    /// <summary>
    /// Closes a metafile device context and returns a handle to the metafile.
    /// </summary>
    /// <param name="hdc">A handle to the metafile device context.</param>
    /// <returns>A handle to the closed metafile.</returns>
    [DllImport("gdi32.dll")]
    public static extern IntPtr CloseMetaFile(IntPtr hdc);

    /// <summary>
    /// Copies a metafile.
    /// </summary>
    /// <param name="hmfSrc">A handle to the source metafile.</param>
    /// <param name="lpszFile">The destination file path, or <see langword="null"/> for an in-memory copy.</param>
    /// <returns>A handle to the copied metafile.</returns>
    [DllImport("gdi32.dll")]
    public static extern IntPtr CopyMetaFile(IntPtr hmfSrc, string lpszFile);

    /// <summary>
    /// Deletes a Windows-format metafile.
    /// </summary>
    /// <param name="hWmf">A handle to the metafile to delete.</param>
    /// <returns><see langword="true"/> if the metafile was deleted; otherwise, <see langword="false"/>.</returns>
    [DllImport("gdi32.dll")]
    public static extern bool DeleteMetaFile(IntPtr hWmf);

    /// <summary>
    /// Deletes an enhanced-format metafile.
    /// </summary>
    /// <param name="hEmf">A handle to the enhanced metafile to delete.</param>
    /// <returns><see langword="true"/> if the metafile was deleted; otherwise, <see langword="false"/>.</returns>
    [DllImport("gdi32.dll")]
    public static extern bool DeleteEnhMetaFile(IntPtr hEmf);

    /// <summary>
    /// Creates a bitmap compatible with the device associated with the specified device context.
    /// </summary>
    /// <param name="hdc">A handle to the device context.</param>
    /// <param name="nWidth">The bitmap width, in pixels.</param>
    /// <param name="nHeight">The bitmap height, in pixels.</param>
    /// <returns>A handle to the created bitmap.</returns>
    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth,
       int nHeight);

    /// <summary>
    /// Sets the viewport extent of the specified device context.
    /// </summary>
    /// <param name="hdc">A handle to the device context.</param>
    /// <param name="nXExtent">The horizontal viewport extent.</param>
    /// <param name="nYExtent">The vertical viewport extent.</param>
    /// <param name="lpSize">A pointer that receives the previous extent, or <see cref="IntPtr.Zero"/>.</param>
    /// <returns><see langword="true"/> if the viewport extent was set; otherwise, <see langword="false"/>.</returns>
    [DllImport("gdi32.dll")]
    public static extern bool SetViewportExtEx(IntPtr hdc, int nXExtent, int nYExtent, IntPtr lpSize);

    /// <summary>
    /// Creates an enhanced-metafile device context.
    /// </summary>
    /// <param name="hdcRef">A handle to the reference device context.</param>
    /// <param name="lpFilename">The path of the enhanced metafile to create, or <see langword="null"/>.</param>
    /// <param name="lpRect">The bounding rectangle of the picture frame.</param>
    /// <param name="lpDescription">The application-defined description string.</param>
    /// <returns>A handle to the enhanced-metafile device context.</returns>
    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateEnhMetaFile(IntPtr hdcRef, string lpFilename, [In] ref RECT lpRect, string lpDescription);

    /// <summary>
    /// Closes an enhanced-metafile device context and returns a handle to the metafile.
    /// </summary>
    /// <param name="hdc">A handle to the enhanced-metafile device context.</param>
    /// <returns>A handle to the created enhanced metafile.</returns>
    [DllImport("gdi32.dll")]
    public static extern IntPtr CloseEnhMetaFile(IntPtr hdc);

    /// <summary>
    /// Copies an enhanced metafile.
    /// </summary>
    /// <param name="hemfSrc">A handle to the source enhanced metafile.</param>
    /// <param name="lpszFile">The destination file path, or <see langword="null"/> for an in-memory copy.</param>
    /// <returns>A handle to the copied enhanced metafile.</returns>
    [DllImport("gdi32.dll")]
    public static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, string lpszFile);

    /// <summary>
    /// Retrieves the header record of an enhanced metafile.
    /// </summary>
    /// <param name="hemf">A handle to the enhanced metafile.</param>
    /// <param name="cbBuffer">The size of the header buffer, in bytes.</param>
    /// <param name="lpemh">A pointer to the buffer that receives the header.</param>
    /// <returns>The number of bytes copied to the buffer.</returns>
    [DllImport("gdi32.dll")]
    public static extern uint GetEnhMetaFileHeader(IntPtr hemf, uint cbBuffer, IntPtr lpemh);

    /// <summary>
    /// Fills a rectangle by using the specified brush.
    /// </summary>
    /// <param name="hDC">A handle to the device context.</param>
    /// <param name="lprc">The rectangle to fill.</param>
    /// <param name="hbr">A handle to the brush to use.</param>
    /// <returns>The result of the fill operation.</returns>
    [DllImport("user32.dll")]
    public static extern int FillRect(IntPtr hDC, [In] ref RECT lprc, IntPtr hbr);

    /// <summary>
    /// Creates a logical pen.
    /// </summary>
    /// <param name="fnPenStyle">The pen style.</param>
    /// <param name="nWidth">The pen width.</param>
    /// <param name="crColor">The pen color as a packed RGB value.</param>
    /// <returns>A handle to the created pen.</returns>
    [DllImport("gdi32.dll")]
    public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);

    /// <summary>
    /// Draws a line from the current position up to, but not including, the specified point.
    /// </summary>
    /// <param name="hdc">A handle to the device context.</param>
    /// <param name="nXEnd">The x-coordinate of the line end point.</param>
    /// <param name="nYEnd">The y-coordinate of the line end point.</param>
    /// <returns><see langword="true"/> if the line was drawn; otherwise, <see langword="false"/>.</returns>
    [DllImport("gdi32.dll")]
    public static extern bool LineTo(IntPtr hdc, int nXEnd, int nYEnd);

    /// <summary>
    /// Moves the current position in the specified device context.
    /// </summary>
    /// <param name="hdc">A handle to the device context.</param>
    /// <param name="X">The new x-coordinate.</param>
    /// <param name="Y">The new y-coordinate.</param>
    /// <param name="zero">A pointer that receives the previous position, or <see cref="IntPtr.Zero"/>.</param>
    /// <returns><see langword="true"/> if the position was updated; otherwise, <see langword="false"/>.</returns>
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

    /// <summary>
    /// Stretches or compresses a bitmap from a source device context into a destination device context.
    /// </summary>
    /// <param name="hdcDest">A handle to the destination device context.</param>
    /// <param name="nXOriginDest">The x-coordinate of the destination upper-left corner.</param>
    /// <param name="nYOriginDest">The y-coordinate of the destination upper-left corner.</param>
    /// <param name="nWidthDest">The destination width.</param>
    /// <param name="nHeightDest">The destination height.</param>
    /// <param name="hdcSrc">A handle to the source device context.</param>
    /// <param name="nXOriginSrc">The x-coordinate of the source upper-left corner.</param>
    /// <param name="nYOriginSrc">The y-coordinate of the source upper-left corner.</param>
    /// <param name="nWidthSrc">The source width.</param>
    /// <param name="nHeightSrc">The source height.</param>
    /// <param name="dwRop">The raster-operation code.</param>
    /// <returns><see langword="true"/> if the bitmap transfer succeeded; otherwise, <see langword="false"/>.</returns>
    [DllImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool StretchBlt([In] IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, [In] IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, TernaryRasterOperations dwRop);
  }
}
