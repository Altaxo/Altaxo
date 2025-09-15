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
using Microsoft.Win32;

#nullable enable

namespace Altaxo.Serialization.AutoUpdates
{

  /// <summary>
  /// Helper class to determine which version of .NET core is installed.
  /// </summary>
  public class NetCoreVersionDetermination
  {
    public static bool IsVersionInstalled(string versionString)
    {
      if (Version.TryParse(versionString, out var version))
      {
        return IsVersionInstalled(version.Major, version.Minor);
      }
      return false;
    }



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

    /// <summary>
    /// Determines whether a specific version of the .NET Core runtime is installed.
    /// You have to specify major and minor version, e.g. for .NET core 5.0: 5 and 0.
    /// The function then will return true if any 5.0 version is installed, e.g. 5.0.2 etc.
    /// </summary>
    /// <param name="major">The major version to look for.</param>
    /// <param name="minor">The minor version to look for.</param>
    /// <returns>
    ///   <c>true</c> if [is version installed] [the specified major]; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>See <see href="https://stackoverflow.com/questions/62875409/how-to-programmatically-check-the-net-core-runtime-version-installed-on-my-mach"/> for details.</remarks>
    public static bool IsVersionInstalled(int major, int minor)
    {
      if (Environment.OSVersion.Platform == PlatformID.Win32NT)
      {
        var searchString = FormattableString.Invariant($"{major}.{minor}.");
        using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
          RegistryView.Registry32).OpenSubKey(@"SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App\"))
        {
          if (baseKey is not null)
          {
            foreach (var subKeyName in baseKey.GetValueNames())
            {
              if (subKeyName.StartsWith(searchString) && baseKey.GetValue(subKeyName) is int value && value != 0)
                return true;
            }
          }
        }
      }
      else if (Environment.OSVersion.Platform == PlatformID.Unix)
      {
        // refer to the reference above for a method to determine .NET core version on Linux
        // see https://stackoverflow.com/questions/62875409/how-to-programmatically-check-the-net-core-runtime-version-installed-on-my-mach
        throw new NotImplementedException();
      }
      else
      {
        throw new NotImplementedException();
      }
      return false;
    }
  }
}
