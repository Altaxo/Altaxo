#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
#nullable enable
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Altaxo.AddInItems
{
  /// <summary>
  /// Provides methods for loading plug-in assemblies and their dependencies.
  /// Special care is taken to load almost all dependencies into the default <see cref="System.Runtime.Loader.AssemblyLoadContext"/>
  /// in order to avoid loading the same assembly into multiple contexts.
  /// </summary>
  public class AssemblyLoaderService
  {
    /// <summary>
    /// Gets the instance of the service.
    /// </summary>
    /// <value>
    /// The instance of the service.
    /// </value>
    public static AssemblyLoaderService Instance { get; } = new AssemblyLoaderService();

    /// <summary>
    /// Key are possible folder names where DLLs have been located. Value is a dummy value without further meaning.
    /// </summary>
    private ConcurrentDictionary<string, bool> _dllFolderNames;

    /// <summary>
    /// Cache of loaded assemblies. Key is the full name of the assembly, value is the loaded <see cref="Assembly"/> instance.
    /// </summary>
    private ConcurrentDictionary<string, Assembly> _loadedAssemblies;

    // Remarks: if needed we could also subscribe to AssemblyLoadContext.Default.Resolving event, but currently we do not need it, because we load all assemblies into the default context.
    // then we need a bag of AssemblyDependencyResolver instances, one for each loaded assembly, and we need to check the hint path of each resolver to find the right one for the assembly that is being resolved.

    private AssemblyLoaderService()
    {
      _dllFolderNames = new ConcurrentDictionary<string, bool>();
      _loadedAssemblies = new ConcurrentDictionary<string, Assembly>();
      _dllFolderNames.TryAdd(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), true);
      // Subscribe to the AssemblyResolve event to handle assembly resolution for dependencies
      // AssemblyLoadContext.Default.Resolving += EhDefaultAssemblyResolving; // currently not needed
      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
      AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
    }


    /// <summary>
    /// Resolves assemblies that were previously loaded through the load-from context.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="args">The assembly resolution arguments.</param>
    /// <returns>The resolved assembly, or <see langword="null"/> if no matching assembly could be found.</returns>
    private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
      if (string.IsNullOrEmpty(args.Name))
        return null;

      if (_loadedAssemblies.TryGetValue(args.Name, out var loadedAssembly))
      {
        return loadedAssembly;
      }

      var fileNameParts = args.Name.Split(new char[] { ',' });
      Assembly? result = null;


      // try to load the assembly by the name, from the same directory as the calling assembly
      string path1 = args.RequestingAssembly is { } requestingAssembly ? System.IO.Path.GetDirectoryName(requestingAssembly.Location) ?? string.Empty : string.Empty;

      if (!string.IsNullOrEmpty(path1))
      {
        result = TryGetAssemblyFromFolderAndFileNameWithoutExtension(path1, fileNameParts[0]);
      }

      if (result is null)
      {
        var path2 = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty;
        if (!string.IsNullOrEmpty(path2) && path2 != path1)
        {
          result = TryGetAssemblyFromFolderAndFileNameWithoutExtension(path2, fileNameParts[0]);
        }
      }

      return result;
    }

    /// <summary>
    /// Stores assemblies as they are loaded so they can be reused during later resolution requests.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="args">The assembly load event arguments.</param>
    private void CurrentDomain_AssemblyLoad(object? sender, AssemblyLoadEventArgs args)
    {
      Assembly assembly = args.LoadedAssembly;
      if (assembly.FullName is { } fullName)
      {
        _loadedAssemblies.TryAdd(fullName, assembly);
      }
      if (!string.IsNullOrEmpty(assembly.Location))
      {
        _dllFolderNames.TryAdd(Path.GetDirectoryName(assembly.Location), true);
      }
    }

    /// <summary>
    /// Loads an assembly, given only the partial name of the assembly, e.g. 'AltaxoCore'. If the assembly is already loaded into
    /// the application domain, the already loaded assembly is returned.
    /// </summary>
    /// <param name="assemblyString">The partial assembly string.</param>
    /// <param name="hintPath">A directory in which to search for the assembly. If the directory exists, it is searched first,
    /// followed by the directory of the entry assembly.</param>
    /// <returns>The assembly that was loaded, or null if the assembly was not found.</returns>
    public Assembly? LoadAssemblyFromPartialName(string assemblyString, string hintPath)
    {
      // First of all, we look if such an assembly is already loaded
      var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(ass => ass.GetName().Name == assemblyString).FirstOrDefault();
      if (loadedAssembly is not null)
        return loadedAssembly;

      FileInfo? resolvedFile = null;
      assemblyString += ".dll";
      if (hintPath is not null && Directory.Exists(hintPath))
      {
        var dirInfo = new DirectoryInfo(hintPath);
        resolvedFile = dirInfo.GetFiles(assemblyString, SearchOption.AllDirectories).FirstOrDefault();
      }

      foreach (var folderName in _dllFolderNames.Keys)
      {
        var fileName = Path.Combine(folderName, assemblyString);
        if (File.Exists(fileName))
        {
          resolvedFile = new FileInfo(fileName);
          break;
        }
      }

      var result = resolvedFile is null ? null : LoadAssemblyFromFullySpecifiedFileName(resolvedFile.FullName);
      return result;
    }

    /// <summary>
    /// Loads the assembly, using the full file name of the assembly.
    /// </summary>
    /// <param name="fullFileName">The fully qualified file name of the assembly.</param>
    /// <returns>The loaded assembly.</returns>
    public Assembly? LoadAssemblyFromFullySpecifiedFileName(string fullFileName)
    {
      var result = AssemblyLoadContext.Default.LoadFromAssemblyPath(fullFileName);
      if (result is not null)
      {
        _loadedAssemblies.TryAdd(result.FullName, result);
        _dllFolderNames.TryAdd(Path.GetDirectoryName(fullFileName), true);
      }

      return result;
    }

    /// <summary>
    /// Attempts to load an assembly file from the specified directory.
    /// </summary>
    /// <param name="folderPathName">The directory that may contain the assembly.</param>
    /// <param name="fileNameWithoutExtension">The assembly file name without its extension.</param>
    /// <returns>The loaded assembly, or <see langword="null"/> if no matching assembly could be loaded.</returns>
    private Assembly? TryGetAssemblyFromFolderAndFileNameWithoutExtension(string folderPathName, string fileNameWithoutExtension)
    {
      var fileName = System.IO.Path.Combine(folderPathName, fileNameWithoutExtension + ".dll");
      try
      {
        if (System.IO.File.Exists(fileName))
        {
          var assembly = LoadAssemblyFromFullySpecifiedFileName(fileName);
          return assembly;
        }
      }
      catch (Exception)
      {
      }
      return null;
    }
  }
}

