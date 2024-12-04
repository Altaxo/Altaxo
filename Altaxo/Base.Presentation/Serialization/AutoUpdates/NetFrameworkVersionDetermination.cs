#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using Microsoft.Win32;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Class responsible for determining which .NET framework version is currently installed.
  /// </summary>
  public class NetFrameworkVersionDetermination
  {

    /// <summary>
    /// Determines whether a specified version of the .NET framework is installed.
    /// </summary>
    /// <param name="version">The version. Examples are 4.0, 4.6, 4.7.1 etc.</param>
    /// <returns>
    ///   <c>true</c> if the specified version of the .NET framework is installed; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsVersionInstalled(string version)
    {
      version = version.ToUpperInvariant();

      switch (version)
      {
        case "4.0":
          return true; // this program is compiled using Framework 4.0 so it is installed.

        case "4.5":
          return IsVersion45Installed();

        case "4.5.1":
          return IsVersion451Installed();

        case "4.5.2":
          return IsVersion452Installed();

        case "4.6":
          return IsVersion46Installed();

        case "4.6.1":
          return IsVersion461Installed();

        case "4.6.2":
          return IsVersion462Installed();

        case "4.7":
          return IsVersion47Installed();

        case "4.7.1":
          return IsVersion471Installed();

        case "4.7.2":
          return IsVersion472Installed();

        case "4.8":
          return IsVersion48Installed();

        default:
          return false;
      }
    }

    /// <summary>
    /// Determines whether a specified version of the .NET framework is installed.
    /// </summary>
    /// <param name="version">The version. Examples are 4.0, 4.6, 4.7.1 etc.</param>
    /// <returns>
    ///   <c>true</c> if the specified version of the .NET framework is installed; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsVersionInstalled(Version version)
    {
      if (version <= new Version(4, 0))
        return true;
      else if (version <= new Version(4, 5))
        return IsVersion45Installed();
      else if (version <= new Version(4, 5, 1))
        return IsVersion451Installed();
      else if (version <= new Version(4, 5, 2))
        return IsVersion452Installed();
      else if (version <= new Version(4, 6))
        return IsVersion46Installed();
      else if (version <= new Version(4, 6, 1))
        return IsVersion461Installed();
      else if (version <= new Version(4, 6, 2))
        return IsVersion462Installed();
      else if (version <= new Version(4, 7))
        return IsVersion47Installed();
      else if (version <= new Version(4, 7, 1))
        return IsVersion471Installed();
      else if (version <= new Version(4, 7, 2))
        return IsVersion472Installed();
      else if (version <= new Version(4, 8))
        return IsVersion48Installed();

      return false;
    }

    /// <summary>
    /// Reads from the registry the .NET framework release number (working for framework versions &gt;=4.5).
    /// </summary>
    /// <returns>The framework release number.</returns>
    public static int? GetFramework45ReleaseNumber()
    {
      try
      {
        using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
           RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"))
        {
          var releaseKey = (int)ndpKey.GetValue("Release");
          return releaseKey;
        }
      }
      catch (Exception)
      {
        return null;
      }
    }

    /// <summary>
    /// Determines whether .NET framework version 4.5 (or greater) is installed.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if version 4.5 (or greater) is installed; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsVersion45Installed()
    {
      var release = GetFramework45ReleaseNumber();
      return release.HasValue;
    }

    /// <summary>
    /// Determines whether .NET framework version 4.5.1 (or greater) is installed.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if version 4.5.1 (or greater) is installed; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsVersion451Installed()
    {
      var release = GetFramework45ReleaseNumber();
      return release.HasValue && release >= 378675;
    }

    /// <summary>
    /// Determines whether .NET framework version 4.5.2 (or greater) is installed.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if version 4.5.2 (or greater) is installed; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsVersion452Installed()
    {
      var release = GetFramework45ReleaseNumber();
      return release.HasValue && release >= 379893;
    }

    /// <summary>
    /// Determines whether .NET framework version 4.6 (or greater) is installed.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if version 4.6 (or greater) is installed; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsVersion46Installed()
    {
      var release = GetFramework45ReleaseNumber();
      return release.HasValue && release >= 393273;
    }

    /// <summary>
    /// Determines whether .NET framework version 4.6.1 (or greater) is installed.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if version 4.6.1 (or greater) is installed; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsVersion461Installed()
    {
      var release = GetFramework45ReleaseNumber();
      return release.HasValue && release >= 394254;
    }

    /// <summary>
    /// Determines whether .NET framework version 4.6.2 (or greater) is installed.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if version 4.6.2 (or greater) is installed; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsVersion462Installed()
    {
      var release = GetFramework45ReleaseNumber();
      return release.HasValue && release >= 394802;
    }

    /// <summary>
    /// Determines whether .NET framework version 4.7 (or greater) is installed.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if version 4.7 (or greater) is installed; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsVersion47Installed()
    {
      var release = GetFramework45ReleaseNumber();
      return release.HasValue && release >= 460798;
    }

    /// <summary>
    /// Determines whether .NET framework version 4.7.1 (or greater) is installed.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if version 4.7.1 (or greater) is installed; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsVersion471Installed()
    {
      var release = GetFramework45ReleaseNumber();
      return release.HasValue && release >= 461308;
    }

    /// <summary>
    /// Determines whether .NET framework version 4.7.2 (or greater) is installed.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if version 4.7.2 (or greater) is installed; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsVersion472Installed()
    {
      var release = GetFramework45ReleaseNumber();
      return release.HasValue && release >= 461808;
    }

    /// <summary>
    /// Determines whether .NET framework version 4.8 (or greater) is installed.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if version 4.8 (or greater) is installed; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsVersion48Installed()
    {
      var release = GetFramework45ReleaseNumber();
      return release.HasValue && release >= 528040;
    }

  }
}
