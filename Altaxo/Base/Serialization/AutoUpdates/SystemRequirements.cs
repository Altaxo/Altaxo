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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;


namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Represents the system requirements needed for a given package.
  /// </summary>
  public class SystemRequirements
  {
    /// <summary>
    /// Determines if this system is matching the requirements of the package.
    /// </summary>
    /// <param name="packageInfo">The information about the package.</param>
    /// <returns>True if the system is matching the requirements of the package; otherwise, false.</returns>
    public static bool MatchesRequirements(PackageInfo packageInfo)
    {

#if AutoUpdateDownloader
      var systemRequirementsService = new SystemRequirementsDetermination();
#else
      var systemRequirementsService = Current.GetRequiredService<ISystemRequirementsDetermination>();
#endif

      if (packageInfo.RequiredNetFrameworkVersion is not null && !systemRequirementsService.IsNetFrameworkVersionInstalled(packageInfo.RequiredNetFrameworkVersion))
      {
        return false;
      }

      if (packageInfo.RequiredDotNetVersion is not null && !systemRequirementsService.IsNetCoreVersionInstalled(packageInfo.RequiredDotNetVersion))
      {
        return false;
      }

      if (packageInfo.RequiredArchitectures.Length > 0)
      {
        bool meetsArchitecture = false;
        foreach (var arch in packageInfo.RequiredArchitectures)
        {
          meetsArchitecture |= MeetsArchitectureRequirements(arch);
        }
        if (!meetsArchitecture)
        {
          return false;
        }
      }

      if (packageInfo.RequiredOperatingSystems.Length > 0)
      {
        bool meetsOperatingSystem = false;
        foreach (var os in packageInfo.RequiredOperatingSystems)
        {
          meetsOperatingSystem |= MeetsOSRequirements(os);
        }
        if (!meetsOperatingSystem)
        {
          return false;
        }
      }


      return true;
    }


    public static bool MeetsArchitectureRequirements(Architecture value)
    {
      var thisArchitecture = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture;

      return value == thisArchitecture;
    }

    public static bool MeetsOSRequirements((OSPlatform osPlatForm, Version osVersion) value)
    {
      return RuntimeInformation.IsOSPlatform(value.osPlatForm) && Environment.OSVersion.Version >= value.osVersion;
    }

    /// <summary>
    /// Gets the highest version that is possible to install; or null, if such a version does not exist.
    /// </summary>
    /// <param name="packageInfos">The package infos.</param>
    /// <returns></returns>
    public static PackageInfo? TryGetHighestVersion(PackageInfo[] packageInfos)
    {
      // from all parsed versions, choose that one that matches the requirements
      // and has the highest version
      // if in doubt, choose that at the topmost line
      var list = new List<(PackageInfo packageInfo, int line)>();
      for (int i = 0; i < packageInfos.Length; ++i)
      {
        if (SystemRequirements.MatchesRequirements(packageInfos[i]))
        {
          list.Add((packageInfos[i], i));
        }
      }

      if (list.Count == 0)
      {
        return null;
      }
      else
      {
        list.Sort((x, y) =>
        {
          var b0 = Comparer<Version>.Default.Compare(x.packageInfo.Version, y.packageInfo.Version);
          return b0 != 0 ? b0 : Comparer<int>.Default.Compare(y.line, x.line); // note that x and y is here exchanged to choose the topmost line
        });
        return list[list.Count - 1].packageInfo;
      }
    }

    /// <summary>If there already the currently best package in the download directory, this function gets the present downloaded package.
    /// If anything is invalid (wrong format of the version file,
    /// wrong hash sum), the return value is <c>Null</c>.</summary>
    /// <param name="versionFileStream">Stream to read the version info from. This must be the opened stream of the VersionInfo.json file in the download directory.</param>
    /// <param name="storagePath">Path to the directory that stores the downloaded package.</param>
    /// <param name="leavePackageFileStreamOpen">If true, the file stream of the package file that was opened for checking the hash is left opened and
    /// returned as the second item of the returned tuple. In that case, you are responsible for  disposing the stream.</param>
    /// <returns>The info for the already present package in the download directory. If anything is invalid, the return value is null.</returns>
    public static (PackageInfo? packageInfo, FileStream? packageFileStream) TryGetPackageThatWasDownloadedAlready(Stream versionFileStream, string storagePath, bool leavePackageFileStreamOpen = false)
    {
      PackageInfo? packageInfo = null;
      try
      {
        var packageInfos = PackageInfo.ReadPackagesFromJson(versionFileStream);
        packageInfo = TryGetHighestVersion(packageInfos);
        if (packageInfo is null)
          return (null, null); // there is currently no version that can be installed on this system.

        var packageFileStream = packageInfo.VerifyLengthAndHashOfPackageZipFile(storagePath, leavePackageFileStreamOpen: leavePackageFileStreamOpen);
        return (packageInfo, packageFileStream);
      }
      catch (Exception)
      {
      }

      return (null, null);
    }
  }
}
