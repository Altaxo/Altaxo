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

namespace Altaxo.UnmanagedApi.GdiPlus
{
  public static class GdiPlusFunc
  {
    /// <summary>
    /// Use the EmfToWmfBits function in the GDI+ specification to convert a
    /// Enhanced Metafile to a Windows Metafile
    /// </summary>
    /// <param name="hEmf">
    /// A handle to the Enhanced Metafile to be converted
    /// </param>
    /// <param name="uBufferSize">
    /// The size of the buffer used to store the Windows Metafile bits returned
    /// </param>
    /// <param name="bBuffer">
    /// An array of bytes used to hold the Windows Metafile bits returned
    /// </param>
    /// <param name="iMappingMode">
    /// The mapping mode of the image.  This control uses MM_ANISOTROPIC.
    /// </param>
    /// <param name="flags">
    /// Flags used to specify the format of the Windows Metafile returned
    /// </param>
    [DllImport("gdiplus.dll", SetLastError = true)]
    public static extern uint GdipEmfToWmfBits(IntPtr hEmf, uint uBufferSize, byte[] bBuffer, int iMappingMode, EmfToWmfBitsFlags flags);
  }
}
