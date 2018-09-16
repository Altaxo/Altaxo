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
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Altaxo.UnmanagedApi.Advapi32
{
  public static class Advapi32Func
  {
    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int RegCreateKeyEx(
                [In] SafeRegistryHandle hKey,
                [In] string lpSubKey,
                [In, MarshalAs(UnmanagedType.U4)] int Reserved,
                [In] string lpClass,
                [In, MarshalAs(UnmanagedType.U4)] RegOption dwOptions,
                [In, MarshalAs(UnmanagedType.U4)] RegSAM samDesired,
                [In] IntPtr lpSecurityAttributes,
                [Out] out SafeRegistryHandle phkResult,
                [Out] out RegResult lpdwDisposition);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegDeleteKey(SafeRegistryHandle hKey, string lpSubKey);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegDeleteValue(SafeRegistryHandle hKey, string lpValueName);
  }
}
