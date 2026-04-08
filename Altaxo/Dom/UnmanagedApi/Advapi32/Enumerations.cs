#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.UnmanagedApi.Advapi32
{
  /// <summary>
  /// Specifies registry key creation options.
  /// </summary>
  [Flags]
  public enum RegOption
  {
    /// <summary>Creates a nonvolatile key.</summary>
    NonVolatile = 0x0,
    /// <summary>Creates a volatile key.</summary>
    Volatile = 0x1,
    /// <summary>Creates a symbolic link.</summary>
    CreateLink = 0x2,
    /// <summary>Supports backup and restore operations.</summary>
    BackupRestore = 0x4,
    /// <summary>Opens a symbolic link.</summary>
    OpenLink = 0x8
  }

  /// <summary>
  /// Specifies registry security and access rights.
  /// </summary>
  [Flags]
  public enum RegSAM
  {
    /// <summary>Queries key values.</summary>
    QueryValue = 0x0001,
    /// <summary>Sets key values.</summary>
    SetValue = 0x0002,
    /// <summary>Creates subkeys.</summary>
    CreateSubKey = 0x0004,
    /// <summary>Enumerates subkeys.</summary>
    EnumerateSubKeys = 0x0008,
    /// <summary>Requests change notifications.</summary>
    Notify = 0x0010,
    /// <summary>Creates symbolic links.</summary>
    CreateLink = 0x0020,
    /// <summary>Accesses the 32-bit registry view.</summary>
    WOW64_32Key = 0x0200,
    /// <summary>Accesses the 64-bit registry view.</summary>
    WOW64_64Key = 0x0100,
    /// <summary>Combines both WOW64 flags.</summary>
    WOW64_Res = 0x0300,
    /// <summary>Read access.</summary>
    Read = 0x00020019,
    /// <summary>Write access.</summary>
    Write = 0x00020006,
    /// <summary>Execute access.</summary>
    Execute = 0x00020019,
    /// <summary>Full access.</summary>
    AllAccess = 0x000f003f
  }

  /// <summary>
  /// Indicates the result of a registry key creation or open operation.
  /// </summary>
  public enum RegResult
  {
    /// <summary>A new key was created.</summary>
    CreatedNewKey = 0x00000001,
    /// <summary>An existing key was opened.</summary>
    OpenedExistingKey = 0x00000002
  }
}
