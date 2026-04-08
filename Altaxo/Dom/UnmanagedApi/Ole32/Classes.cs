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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Altaxo.UnmanagedApi.Ole32
{
  /// <summary>
  /// Represents a COM rectangle.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public class COMRECT
  {
    /// <summary>The left coordinate.</summary>
    public int left;
    /// <summary>The top coordinate.</summary>
    public int top;
    /// <summary>The right coordinate.</summary>
    public int right;
    /// <summary>The bottom coordinate.</summary>
    public int bottom;

    /// <summary>
    /// Initializes a new instance of the <see cref="COMRECT"/> class.
    /// </summary>
    public COMRECT()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="COMRECT"/> class.
    /// </summary>
    /// <param name="left">The left coordinate.</param>
    /// <param name="top">The top coordinate.</param>
    /// <param name="right">The right coordinate.</param>
    /// <param name="bottom">The bottom coordinate.</param>
    public COMRECT(int left, int top, int right, int bottom)
    {
      this.left = left;
      this.top = top;
      this.right = right;
      this.bottom = bottom;
    }

    /// <summary>
    /// Creates a rectangle from X, Y, width, and height values.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns>A new <see cref="COMRECT"/> instance.</returns>
    public static COMRECT FromXYWH(int x, int y, int width, int height)
    {
      return new COMRECT(x, y, x + width, y + height);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return string.Concat(new object[] { "Left = ", left, " Top ", top, " Right = ", right, " Bottom = ", bottom });
    }
  }

  /// <summary>
  /// Represents an OLE verb descriptor.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public sealed class tagOLEVERB
  {
    /// <summary>The verb identifier.</summary>
    public int lVerb;

    /// <summary>The verb name.</summary>
    [MarshalAs(UnmanagedType.LPWStr)]
    public string lpszVerbName;

    /// <summary>The verb flags.</summary>
    [MarshalAs(UnmanagedType.U4)]
    public int fuFlags;

    /// <summary>The verb attributes.</summary>
    [MarshalAs(UnmanagedType.U4)]
    public int grfAttribs;

    /// <summary>
    /// Initializes a new instance of the <see cref="tagOLEVERB"/> class.
    /// </summary>
    public tagOLEVERB()
    {
    }
  }

  /// <summary>
  /// Represents a logical palette header.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public sealed class tagLOGPALETTE
  {
    /// <summary>The palette version.</summary>
    [MarshalAs(UnmanagedType.U2)]
    public short palVersion;

    /// <summary>The number of palette entries.</summary>
    [MarshalAs(UnmanagedType.U2)]
    public short palNumEntries;

    /// <summary>
    /// Initializes a new instance of the <see cref="tagLOGPALETTE"/> class.
    /// </summary>
    public tagLOGPALETTE()
    {
    }
  }

  /// <summary>
  /// Represents a COM size structure.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public sealed class tagSIZEL
  {
    /// <summary>The width.</summary>
    public int cx;
    /// <summary>The height.</summary>
    public int cy;

    /// <summary>
    /// Initializes a new instance of the <see cref="tagSIZEL"/> class.
    /// </summary>
    public tagSIZEL()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="tagSIZEL"/> class.
    /// </summary>
    /// <param name="cx">The width.</param>
    /// <param name="cy">The height.</param>
    public tagSIZEL(int cx, int cy)
    {
      this.cx = cx;
      this.cy = cy;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="tagSIZEL"/> class by copying another instance.
    /// </summary>
    /// <param name="o">The source instance.</param>
    public tagSIZEL(tagSIZEL o)
    {
      cx = o.cx;
      cy = o.cy;
    }
  }
}
