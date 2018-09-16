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
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace Altaxo.Com
{
  using UnmanagedApi.Advapi32;

  public enum WOW_Mode
  {
    None,
    Reg64,
    Reg32
  }

  public static class RegistryHelper
  {
    public static RegistryKey CreateSubKey(this RegistryKey mainKey, string name, WOW_Mode mode)
    {


      RegSAM sam = RegSAM.Write | RegSAM.Read | RegSAM.QueryValue;
      if (mode == WOW_Mode.Reg32)
        sam |= RegSAM.WOW64_32Key;
      else if (mode == WOW_Mode.Reg64)
        sam |= RegSAM.WOW64_64Key;

      Advapi32Func.RegCreateKeyEx(
                mainKey.Handle,
                name,
                0,
                null,
                RegOption.NonVolatile,
                sam,
                IntPtr.Zero,
                out var resultingKey,
                out var regResult);

      return RegistryKey.FromHandle(resultingKey);
    }
  }
}
