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
using System.Linq;
using System.Text;

namespace Altaxo.UnmanagedApi.Kernel32
{
  [Flags]
  public enum GlobalAllocFlags : uint
  {
    /// <summary>Allocates fixed memory. The return value is a pointer.</summary>
    GMEM_FIXED = 0x0000,

    /// <summary>Allocates movable memory. Memory blocks are never moved in physical memory, but they can be moved within the default heap. The return value is a handle to the memory object. To translate the handle into a pointer, use the GlobalLock function.
    ///This value cannot be combined with GMEM_FIXED.</summary>
    GMEM_MOVEABLE = 0x0002,

    /// <summary>Initializes memory contents to zero.</summary>
    GMEM_ZEROINIT = 0x0040,

    /// <summary>Combines GMEM_FIXED and GMEM_ZEROINIT.</summary>
    GPTR = 0x0040,

    /// <summary>Combines GMEM_MOVEABLE and GMEM_ZEROINIT.</summary>
    GHND = 0x0042
  }
}
