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

namespace Altaxo.UnmanagedApi.User32
{
  /// <summary>
  /// Provides unmanaged User32 function imports.
  /// </summary>
  public static class User32Func
  {
    /// <summary>
    /// Registers a clipboard format.
    /// </summary>
    /// <param name="lpszFormat">The name of the clipboard format to register.</param>
    /// <returns>The registered clipboard format identifier.</returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern short RegisterClipboardFormat(string lpszFormat);

    /// <summary>
    /// Releases a device context.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose device context is released.</param>
    /// <param name="hDC">A handle to the device context to release.</param>
    /// <returns>The value <c>1</c> if the device context was released; otherwise, <c>0</c>.</returns>
    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    /// <summary>
    /// Retrieves the name of a registered clipboard format.
    /// </summary>
    /// <param name="format">The clipboard format identifier.</param>
    /// <param name="lpszFormatName">A buffer that receives the format name.</param>
    /// <param name="cchMaxCount">The maximum number of characters to copy into <paramref name="lpszFormatName"/>.</param>
    /// <returns>The length of the copied format name, in characters.</returns>
    [DllImport("user32.dll")]
    public static extern int GetClipboardFormatName(uint format, [Out] StringBuilder
       lpszFormatName, int cchMaxCount);
  }
}
