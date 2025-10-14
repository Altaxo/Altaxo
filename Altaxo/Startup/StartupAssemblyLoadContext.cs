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

#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Altaxo
{
  /// <summary>
  /// Assembly load context intended to be used with the topmost assembly of the application (at this time it is the assembly 'Workbench').
  /// By using this context for workbench, all assemblies that Workbench depends on are loaded into the same (this) context.
  /// We also transfer this context to the startup in assembly Workbench, so that all other main assemblies are loaded into this context, too.
  /// </summary>
  /// <seealso cref="System.Runtime.Loader.AssemblyLoadContext" />
  public class StartupAssemblyLoadContext : AssemblyLoadContext
  {
    /// <summary>Resolver for the addin folder</summary>
    private AssemblyDependencyResolver _resolver;

    /// <summary>
    /// The assembly file name of the main app (currently this is Workbench).
    /// For debugging purposes only.
    /// </summary>
    private string _mainAppAssemblyFileName;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupAssemblyLoadContext"/> class.
    /// </summary>
    /// <param name="mainAppAssemblyFileName">The full file name of the original plugin assembly file.</param>
    public StartupAssemblyLoadContext(string mainAppAssemblyFileName)
    {
      _mainAppAssemblyFileName = mainAppAssemblyFileName;
      _resolver = new AssemblyDependencyResolver(mainAppAssemblyFileName);
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
          result = this.LoadFromAssemblyPath(assemblyPath);
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
            result = this.LoadFromAssemblyPath(fileInfo.FullName);
          }
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
      if (libraryPath is not null)
      {
        return LoadUnmanagedDllFromPath(libraryPath);
      }

      return IntPtr.Zero;
    }
  }
}



