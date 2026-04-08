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

namespace Altaxo.UnmanagedApi.Kernel32
{
  /// <summary>
  /// Provides unmanaged Kernel32 function imports.
  /// </summary>
  public static class Kernel32Func
  {
    /// <summary>
    /// Locks a global memory object and returns a pointer to the first byte of the memory block.
    /// </summary>
    /// <param name="hMem">The handle to the global memory object.</param>
    /// <returns>A pointer to the first byte of the memory block.</returns>
    [DllImport("kernel32.dll")]
    public static extern IntPtr GlobalLock(IntPtr hMem);

    /// <summary>
    /// Decrements the lock count associated with a memory object.
    /// </summary>
    /// <param name="hMem">The handle to the global memory object.</param>
    /// <returns><see langword="true"/> if the memory object is still locked; otherwise, <see langword="false"/>.</returns>
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GlobalUnlock(IntPtr hMem);

    /// <summary>
    /// Allocates the specified number of bytes from the global heap.
    /// </summary>
    /// <param name="uFlags">The allocation flags.</param>
    /// <param name="dwBytes">The number of bytes to allocate.</param>
    /// <returns>A handle to the allocated memory object.</returns>
    [DllImport("kernel32.dll")]
    public static extern IntPtr GlobalAlloc([MarshalAs(UnmanagedType.U4)]GlobalAllocFlags uFlags, int dwBytes);

    /// <summary>
    /// Frees the specified global memory object.
    /// </summary>
    /// <param name="hMem">The handle to the global memory object.</param>
    /// <returns><see cref="IntPtr.Zero"/> if the memory object was freed successfully; otherwise, the original handle.</returns>
    [DllImport("kernel32.dll")]
    public static extern IntPtr GlobalFree(IntPtr hMem);

    /// <summary>
    /// Copies memory from one location to another.
    /// </summary>
    /// <param name="dest">The destination pointer.</param>
    /// <param name="src">The source pointer.</param>
    /// <param name="count">The number of bytes to copy.</param>
    [DllImport("kernel32.dll")]
    public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
  }
}
