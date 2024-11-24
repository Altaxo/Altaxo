#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using System.Linq;
using System.Runtime.InteropServices;


namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Represents the system requirements needed for a given package.
  /// </summary>
  public class SystemRequirements
  {
    /// <summary>
    /// The property key: .NET framework version
    /// </summary>
    public const string PropertyKeyNetFrameworkVersion = "RequiredNetFrameworkVersion";

    /// <summary>
    /// Property key for the required dotnet runtime version
    /// </summary>
    public const string PropertyKeyDotNetVersion = "RequiredDotNetVersion";

    /// <summary>
    /// The property for the required architecture (x86, x64, Arm, Arm64 etc.)
    /// </summary>
    public const string PropertyKeyArchitecture = "RequiredArchitecture";

    /// <summary>
    /// The property for the operating system. The value consist of a name (Windows, OSX, Linux) and a version number, separated by an underscore.
    /// </summary>
    public const string PropertyKeyOperatingSystem = "RequiredOperatingSystem";


    /// <summary>
    /// Determines if this system is matching the requirements of the package.
    /// </summary>
    /// <param name="packageInfo">The information about the package.</param>
    /// <returns>True if the system is matching the requirements of the package; otherwise, false.</returns>
    public static bool MatchesRequirements(PackageInfo packageInfo)
    {
      var properties = packageInfo.Properties;

#if AutoUpdateDownloader
      var systemRequirementsService = new SystemRequirementsDetermination();
#else
      var systemRequirementsService = Current.GetRequiredService<ISystemRequirementsDetermination>();
#endif

      // we investigate all keys that start with 'Required'
      // if some of the keys is unknown, we have to assume that this in an older version
      // which doesn't know about this new requirement
      // in this case we return false because we have to assume that the new requirement is not matched.
      foreach (var entry in properties.Where(entry => entry.Key.StartsWith("Required")))
      {
        switch (entry.Key)
        {
          case PropertyKeyNetFrameworkVersion:
            {
              if (!systemRequirementsService.IsNetFrameworkVersionInstalled(entry.Value))
                return false;
            }
            break;
          case PropertyKeyDotNetVersion:
            {
              if (!systemRequirementsService.IsNetCoreVersionInstalled(entry.Value))
                return false;
            }
            break;
          case PropertyKeyArchitecture:
            {
              if (!MeetsArchitectureRequirements(entry.Value))
                return false;
            }
            break;
          case PropertyKeyOperatingSystem:
            {
              if (!MeetsOSRequirements(entry.Value))
                return false;
            }
            break;
          default:
            return false; // return false if the requirement is unknown (we have to assume that this is an older version
        }
      }
      return true;
    }


    public static bool MeetsArchitectureRequirements(string value)
    {
      var thisArchitecture = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture;
      var architectureName = Enum.GetName(typeof(System.Runtime.InteropServices.Architecture), thisArchitecture);
      var parts = value.Split([';'], StringSplitOptions.RemoveEmptyEntries);

      foreach (var part in parts)
      {
        if (part == architectureName)
          return true;
      }
      return false;
    }

    public static bool MeetsOSRequirements(string value)
    {
      const string OSArchitectureWindows = "Windows_";
      const string OSArchitectureOsx = "OSX_";
      const string OSArchitectureLinux = "Linux_";

      var parts = value.Split([';'], StringSplitOptions.RemoveEmptyEntries);
      bool isFulfilled;
      foreach (var part in parts)
      {
        if (part.StartsWith(OSArchitectureWindows))
        {
          isFulfilled = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && MeetsOperatingSystemVersion(part.Substring(OSArchitectureWindows.Length));
          if (isFulfilled)
            return true;
        }
        if (part.StartsWith(OSArchitectureOsx))
        {
          isFulfilled = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && MeetsOperatingSystemVersion(part.Substring(OSArchitectureOsx.Length));
          if (isFulfilled)
            return true;
        }
        if (part.StartsWith(OSArchitectureLinux))
        {
          isFulfilled = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && MeetsOperatingSystemVersion(part.Substring(OSArchitectureLinux.Length));
          if (isFulfilled)
            return true;
        }
      }
      return false;
    }

    public static bool MeetsOperatingSystemVersion(string versionString)
    {
      if (Version.TryParse(versionString, out var requiredVersion))
      {
        var os = Environment.OSVersion;
        return os.Version >= requiredVersion;
      }
      return false;
    }
  }
}
