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
  [Flags]
  public enum RegOption
  {
    NonVolatile = 0x0,
    Volatile = 0x1,
    CreateLink = 0x2,
    BackupRestore = 0x4,
    OpenLink = 0x8
  }

  [Flags]
  public enum RegSAM
  {
    QueryValue = 0x0001,
    SetValue = 0x0002,
    CreateSubKey = 0x0004,
    EnumerateSubKeys = 0x0008,
    Notify = 0x0010,
    CreateLink = 0x0020,
    WOW64_32Key = 0x0200,
    WOW64_64Key = 0x0100,
    WOW64_Res = 0x0300,
    Read = 0x00020019,
    Write = 0x00020006,
    Execute = 0x00020019,
    AllAccess = 0x000f003f
  }

  public enum RegResult
  {
    CreatedNewKey = 0x00000001,
    OpenedExistingKey = 0x00000002
  }
}
