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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace Altaxo.Com
{
  using UnmanagedApi.Advapi32;

  /// <summary>
  /// Specifies the registry view that should be used when accessing WOW64 redirected keys.
  /// </summary>
  public enum WOW_Mode
  {
    /// <summary>
    /// Use the default registry view.
    /// </summary>
    None,
    /// <summary>
    /// Use the 64-bit registry view.
    /// </summary>
    Reg64,
    /// <summary>
    /// Use the 32-bit registry view.
    /// </summary>
    Reg32
  }

  /// <summary>
  /// Provides helper methods for registry access.
  /// </summary>
  public static class RegistryHelper
  {
    /// <summary>
    /// Creates a subkey by using the requested registry view.
    /// </summary>
    /// <param name="mainKey">The parent registry key.</param>
    /// <param name="name">The subkey name.</param>
    /// <param name="mode">The registry view to use.</param>
    /// <returns>The created or opened subkey.</returns>
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
