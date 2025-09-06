#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
public class DotNetCoreSdkAssemblyResolver
{
  public static DotNetCoreSdkAssemblyResolver Instance { get; } = new DotNetCoreSdkAssemblyResolver();

  private readonly List<string> searchPaths;

#if NET9_0
  private int highestNetSdkVersion = 9;
#else
#error Target framework changed, please increment highestNetSdkVersion
#endif

  private int lowestNetSdkVersion = 8;

  /// <summary>
  /// Initializes a new instance of the <see cref="NetCoreSdkAssemblyResolver"/> class.
  /// Discovers available .NET SDK paths for assembly resolution.
  /// </summary>
  private DotNetCoreSdkAssemblyResolver()
  {
    searchPaths = DiscoverSdkPaths();
  }

  /// <summary>
  /// Resolves an assembly by its partial name from the discovered SDK paths.
  /// </summary>
  /// <param name="partialName">The partial name of the assembly to resolve.</param>
  /// <returns>The loaded <see cref="Assembly"/> if found.</returns>
  /// <exception cref="FileNotFoundException">Thrown if the assembly cannot be found in any known SDK path.</exception>
  public string Resolve(string partialName)
  {
    foreach (var path in searchPaths)
    {
      var assemblyPath = FindAssemblyInPath(path, partialName);
      if (assemblyPath != null)
      {
        return assemblyPath;
      }
    }

    throw new FileNotFoundException($"Assembly '{partialName}' not found in any known SDK path.");
  }

  /// <summary>
  /// Discovers all relevant .NET SDK and .NET Standard reference assembly paths.
  /// </summary>
  /// <returns>A list of paths to search for assemblies.</returns>
  private List<string> DiscoverSdkPaths()
  {
    var paths = new List<string>();

    string sdkRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "sdk");

    if (Directory.Exists(sdkRoot))
    {
      var sdkDirs = Directory.GetDirectories(sdkRoot)
          .Select(Path.GetFileName)
          .Where(name => Version.TryParse(name.Split('-')[0], out var _))
          .Select(name => new { Version = Version.Parse(name.Split('-')[0]), Path = Path.Combine(sdkRoot, name) })
          .OrderByDescending(v => v.Version);

      foreach (var sdk in sdkDirs)
      {
        if (sdk.Version.Major >= lowestNetSdkVersion && sdk.Version.Major <= highestNetSdkVersion)
        {
          paths.Add(sdk.Path);
        }
      }
    }

    // Add .NET Standard 2.0 reference assemblies
    var netStandardPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
        @"dotnet\packs\NETStandard.Library.Ref\2.0.0\ref\netstandard2.0"
    );

    if (Directory.Exists(netStandardPath))
    {
      paths.Add(netStandardPath);
    }

    return paths;
  }

  /// <summary>
  /// Searches for an assembly DLL file in the specified path that matches the given partial name.
  /// </summary>
  /// <param name="path">The directory path to search.</param>
  /// <param name="partialName">The partial name of the assembly.</param>
  /// <returns>The full path to the assembly DLL if found; otherwise, null.</returns>
  private string FindAssemblyInPath(string path, string partialName)
  {
    if (!Directory.Exists(path))
      return null;

    var files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
    return files.FirstOrDefault(f =>
        Path.GetFileNameWithoutExtension(f).Contains(partialName, StringComparison.OrdinalIgnoreCase));
  }
}
#endif
