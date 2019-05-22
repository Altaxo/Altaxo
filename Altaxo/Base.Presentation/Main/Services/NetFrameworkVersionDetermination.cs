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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Altaxo.Main.Services
{
  public class NetFrameworkVersionDetermination : Serialization.AutoUpdates.INetFrameworkVersionDetermination
  {
    public bool IsVersionInstalled(string version)
    {
      return IsVersionInstalledHere(version);
    }

    public static bool IsVersionInstalledHere(string version)
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

    public static bool IsVersion45Installed()
    {
      var release = GetFramework45ReleaseNumber();
      if (!release.HasValue)
      {
        return false;
      }
      else
      {
        return true;
      }
    }

    public static bool IsVersion451Installed()
    {
      var release = GetFramework45ReleaseNumber();
      if (!release.HasValue)
      {
        return false;
      }
      else
      {
        return release >= 378675;
      }
    }

    public static bool IsVersion452Installed()
    {
      var release = GetFramework45ReleaseNumber();
      if (!release.HasValue)
      {
        return false;
      }
      else
      {
        return release >= 379893;
      }
    }

    public static bool IsVersion46Installed()
    {
      var release = GetFramework45ReleaseNumber();
      if (!release.HasValue)
      {
        return false;
      }
      else
      {
        return release >= 393273;
      }
    }

    public static bool IsVersion461Installed()
    {
      var release = GetFramework45ReleaseNumber();
      if (!release.HasValue)
      {
        return false;
      }
      else
      {
        return release >= 394254;
      }
    }

    public static bool IsVersion462Installed()
    {
      var release = GetFramework45ReleaseNumber();
      if (!release.HasValue)
      {
        return false;
      }
      else
      {
        return release >= 394802;
      }
    }

    public static bool IsVersion47Installed()
    {
      var release = GetFramework45ReleaseNumber();
      if (!release.HasValue)
      {
        return false;
      }
      else
      {
        return release >= 460798;
      }
    }

    public static bool IsVersion471Installed()
    {
      var release = GetFramework45ReleaseNumber();
      if (!release.HasValue)
      {
        return false;
      }
      else
      {
        return release >= 461308;
      }
    }

    public static bool IsVersion472Installed()
    {
      var release = GetFramework45ReleaseNumber();
      if (!release.HasValue)
      {
        return false;
      }
      else
      {
        return release >= 461808;
      }
    }

    public static bool IsVersion48Installed()
    {
      var release = GetFramework45ReleaseNumber();
      if (!release.HasValue)
      {
        return false;
      }
      else
      {
        return release >= 528040;
      }
    }

  }
}
