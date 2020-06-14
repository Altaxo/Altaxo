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
  /// Service that loads plugin assemblies and their dependencies. Special care is taken that almost all
  /// of the dependencies are loaded into the default AssemblyLoadContext in order to avoid multiple loadings of the same assembly into different contexts.
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
    /// <param name="hintPath">A directory where to search for the assembly. Can be null. If not null, first this directory, and
    /// then the directory of the entry assembly is searched for the assembly.</param>
    /// <returns>The assembly that was loaded, or null if the assembly was not found.</returns>
    public Assembly? LoadAssemblyFromPartialName(string assemblyString, string hintPath)
    {
      // First of all, we look if such an assembly is already loaded 
      Assembly result = AppDomain.CurrentDomain.GetAssemblies().Where(ass => ass.GetName().Name == assemblyString).FirstOrDefault();
      if (null != result)
        return result;

      FileInfo? resolvedFile = null;
      assemblyString += ".dll";
      if (null != hintPath && Directory.Exists(hintPath))
      {
        var dirInfo = new DirectoryInfo(hintPath);
        resolvedFile = dirInfo.GetFiles(assemblyString, SearchOption.AllDirectories).FirstOrDefault();
      }

      if (resolvedFile is null)
      {
        var entryAssembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Can not retrieve entry assembly!");
        var dirInfo = new DirectoryInfo(Path.GetDirectoryName(entryAssembly.Location));
        resolvedFile = dirInfo.GetFiles(assemblyString, SearchOption.AllDirectories).FirstOrDefault();
      }

      return resolvedFile is null ? null : LoadAssemblyFromFullySpecifiedName(resolvedFile.FullName);
    }

    /// <summary>
    /// Loads the assembly, using the full file name of the assembly.
    /// </summary>
    /// <param name="fullName">The full name of the assembly.</param>
    /// <returns>The assembly that was loaded, or null if the assembly was not found.</returns>
    public Assembly LoadAssemblyFromFullySpecifiedName(string fullName)
    {
#if NETFRAMEWORK
      return System.Reflection.Assembly.LoadFrom(fullName);
#else
      var context = new LoadContextIntoDefault(fullName);
      return context.LoadFromAssemblyPath(fullName);
#endif
    }
  }
}


#if !NETFRAMEWORK
namespace Altaxo.AddInItems
{
  using System.Runtime.Loader;

  /// <summary>
  /// LoadContextIntoDefault is an assembly load context intended for plugin assemblies with dependencies.
  /// The original plugin assembly is loaded into a newly created instance of this class (this can not be helped?),
  /// but at least all dependencies of the original plugin dependency are loaded in the default context.
  /// In this way it can be avoided that we have unintentionally load multiple instances of the same assembly.
  /// </summary>
  /// <seealso cref="System.Runtime.Loader.AssemblyLoadContext" />
  public class LoadContextIntoDefault : AssemblyLoadContext
  {
    private AssemblyDependencyResolver _resolver;
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

    /// <summary>
    /// Allows an assembly to be resolved and loaded based on its <see cref="T:System.Reflection.AssemblyName" />.
    /// Here, we first look into the current application domain, and if an assembly with the same name is already loaded,
    /// we return this assembly. Otherwise, we try to resolve the assembly name, and then load the assembly into
    /// the Default (!) context (and not in the context represented by this instance).
    /// </summary>
    /// <param name="assemblyName">The object that describes the assembly to be loaded.</param>
    /// <returns>
    /// The loaded assembly, or <see langword="null" />.
    /// </returns>
    protected override Assembly? Load(AssemblyName assemblyName)
    {
      // this function is called when dependencies of the pluginAssembly should be loaded

      // First of all, we look if such an assembly is loaded already
      Assembly result = AppDomain.CurrentDomain.GetAssemblies().Where(ass => ass.GetName().Name == assemblyName.Name).FirstOrDefault();

      if (result is null)
      {
        // otherwise, we use the _resolver to resolve the dependent assembly
        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (!(assemblyPath is null))
        {
          // note that we load the dependent assemblies into the default load context,
          // and not in this context here
          // by this way we avoid that we load the same assembly in different contexts
          result = Default.LoadFromAssemblyPath(assemblyPath);
        }
      }

      System.Diagnostics.Debug.WriteLine($"{assemblyName} resolved to {result?.Location ?? "null"}");
      return result;
    }

    /// <summary>
    /// Load an unmanaged library by name.
    /// </summary>
    /// <param name="unmanagedDllName">Name of the unmanaged library. Typically this is the filename without its path or extensions.</param>
    /// <returns>
    /// A handle to the loaded library, or <see cref="F:System.IntPtr.Zero" />.
    /// </returns>
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
#endif

