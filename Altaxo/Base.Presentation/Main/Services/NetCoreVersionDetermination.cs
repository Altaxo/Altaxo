#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System.Threading.Tasks;
using Microsoft.Win32;

#nullable enable

namespace Altaxo.Main.Services
{

  /// <summary>
  /// Helper class to determine which version of .NET core is installed.
  /// </summary>
  public class NetCoreVersionDetermination
  {
    /// <summary>
    /// Gets the highest net core version installed currently.
    /// </summary>
    /// <returns>The highest .NET core version that is currently installed; or null if .NET core is not present on this machine.</returns>
    public static Version? GetNetCoreVersion()
    {
      try
      {
        using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
           RegistryView.Registry32).OpenSubKey(@"SOFTWARE\dotnet\Setup\InstalledVersions\x64\SharedHost\"))
        {
          if (baseKey is null)
            return null;
          var versionString = (string?)baseKey.GetValue("Version");
          return versionString is null ? null : Version.Parse(versionString);
        }
      }
      catch (Exception)
      {
        return null;
      }
    }
  }
}
