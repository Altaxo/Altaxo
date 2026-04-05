#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.IO;
using System.Linq;
using System.Reflection;

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
      var result = AppDomain.CurrentDomain.GetAssemblies().Where(ass => ass.GetName().Name == assemblyString).FirstOrDefault();
      if (result is not null)
        return result;

      FileInfo? resolvedFile = null;
      assemblyString += ".dll";
      if (hintPath is not null && Directory.Exists(hintPath))
      {
        var dirInfo = new DirectoryInfo(hintPath);
        resolvedFile = dirInfo.GetFiles(assemblyString, SearchOption.AllDirectories).FirstOrDefault();
      }

      if (resolvedFile is null)
      {
        var entryAssembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Can not retrieve entry assembly!");
        var dirInfo = new DirectoryInfo(Path.GetDirectoryName(entryAssembly.Location)!);
        resolvedFile = dirInfo.GetFiles(assemblyString, SearchOption.AllDirectories).FirstOrDefault();
      }

      return resolvedFile is null ? null : LoadAssemblyFromFullySpecifiedName(resolvedFile.FullName);
    }

    /// <summary>
    /// Loads the assembly, using the full file name of the assembly.
    /// </summary>
    /// <param name="fullName">The fully qualified file name of the assembly.</param>
    /// <returns>The loaded assembly.</returns>
    public Assembly LoadAssemblyFromFullySpecifiedName(string fullName)
    {
      var context = new LoadContextIntoDefault(fullName);
      return context.LoadFromAssemblyPath(fullName);
    }
  }
}

namespace Altaxo.AddInItems
{
  using System.Runtime.Loader;

  /// <summary>
  /// Represents an assembly load context intended for plug-in assemblies with dependencies.
  /// The original plug-in assembly is loaded into a newly created instance of this class,
  /// but at least all dependencies of the original plug-in assembly are loaded into the same context, namely <see cref="Instance"/>.
  /// The default context cannot be used because resolution of third-level assemblies would then fail.
  /// This approach avoids unintentionally loading multiple instances of the same assembly.
  /// </summary>
  /// <seealso cref="System.Runtime.Loader.AssemblyLoadContext" />
  public class LoadContextIntoDefault : AssemblyLoadContext
  {
    /// <summary>
    /// Gets the shared load context used to load dependent assemblies.
    /// </summary>
    static LoadContextIntoDefault Instance { get; } = new LoadContextIntoDefault(Assembly.GetEntryAssembly().Location);

    /// <summary>
    /// Resolves assembly dependencies for the plug-in folder.
    /// </summary>
    private AssemblyDependencyResolver _resolver;

    /// <summary>
    /// Stores the fully qualified file name of the original plug-in assembly.
    /// </summary>
    private string _pluginAssemblyFileName;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadContextIntoDefault"/> class.
    /// </summary>
    /// <param name="pluginAssemblyFileName">The full file name of the original plugin assembly file.</param>
    public LoadContextIntoDefault(string pluginAssemblyFileName)
    {
      _pluginAssemblyFileName = pluginAssemblyFileName;
      _resolver = new AssemblyDependencyResolver(pluginAssemblyFileName);
    }

    /// <inheritdoc/>
    protected override Assembly? Load(AssemblyName assemblyName)
    {
      // this function is called when dependencies of the pluginAssembly should be loaded

      // First of all, we look if such an assembly is loaded already
      var result = AppDomain.CurrentDomain.GetAssemblies().Where(ass => ass.GetName().Name == assemblyName.Name).FirstOrDefault();

      if (result is null)
      {
        // otherwise, we use the _resolver to resolve the dependent assembly
        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath is not null)
        {
          // note that we load the dependent assemblies into the default load context,
          // and not in this context here
          // by this way we avoid that we load the same assembly in different contexts
          result = Instance.LoadFromAssemblyPath(assemblyPath);
        }
        else
        {
          var dirInfo = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!);
          var fileInfo = dirInfo.EnumerateFiles(assemblyName.Name + ".dll").FirstOrDefault();
          if (fileInfo is not null)
          {
            // note that we load the dependent assemblies into the default load context,
            // and not in this context here
            // by this way we avoid that we load the same assembly in different contexts
            result = Instance.LoadFromAssemblyPath(fileInfo.FullName);
          }
        }
      }

      System.Diagnostics.Debug.WriteLine($"{assemblyName} resolved to {result?.Location ?? "null"}");
      return result;
    }

    /// <inheritdoc/>
    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
      var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
      if (!(libraryPath is null))
      {
        return LoadUnmanagedDllFromPath(libraryPath);
      }

      return IntPtr.Zero;
    }
  }

}

