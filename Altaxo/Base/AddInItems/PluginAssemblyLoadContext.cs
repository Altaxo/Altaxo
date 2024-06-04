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

#if !NETFRAMEWORK
#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Altaxo.AddInItems
{
  using System.Diagnostics;
  using System.Runtime.Loader;

  /// <summary>
  /// LoadContextIntoDefault is an assembly load context intended for plugin assemblies with dependencies.
  /// The original plugin assembly is loaded into a newly created instance of this class (this can not be helped?),
  /// but at least all dependencies of the original plugin dependency are loaded in the default context.
  /// In this way it can be avoided that we have unintentionally load multiple instances of the same assembly.
  /// </summary>
  /// <seealso cref="System.Runtime.Loader.AssemblyLoadContext" />
  public class PluginAssemblyLoadContext : AssemblyLoadContext
  {
    /// <summary>Resolver for the addin folder</summary>
    private AssemblyDependencyResolver _resolver;
    private string _pluginAssemblyFileName;

    [Conditional("LogAssemblyLoading")]
    private static void WriteAssemblyLoadingLog(string s)
    {
      Console.WriteLine(s);
      Debug.WriteLine(s);
    }

    static PluginAssemblyLoadContext()
    {
      Default.Resolving -= EhDefaultResolving;
      Default.Resolving += EhDefaultResolving;
      AppDomain.CurrentDomain.AssemblyLoad -= EhAssemblyLoad;
      AppDomain.CurrentDomain.AssemblyLoad += EhAssemblyLoad;
    }

    private static void EhAssemblyLoad(object? sender, AssemblyLoadEventArgs args)
    {
      WriteAssemblyLoadingLog($"Assembly '{args.LoadedAssembly}' was successfully loaded.");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginAssemblyLoadContext"/> class.
    /// </summary>
    /// <param name="pluginAssemblyFileName">The full file name of the original plugin assembly file.</param>
    public PluginAssemblyLoadContext(string pluginAssemblyFileName) : base(pluginAssemblyFileName) // we give the context the name of the assembly file
    {
      WriteAssemblyLoadingLog($"Constructor of {this.GetType()} fileName = {pluginAssemblyFileName}");

      _pluginAssemblyFileName = pluginAssemblyFileName;
      _resolver = new AssemblyDependencyResolver(pluginAssemblyFileName);


    }

    private static Assembly? EhDefaultResolving(AssemblyLoadContext context, AssemblyName name)
    {
      Assembly? result = null;
      var requestingAssembly = Assembly.GetEntryAssembly();

      var nameParts = name.Name!.Split(',');
      var fullFileName = Path.Combine(Path.GetDirectoryName(requestingAssembly.Location), nameParts[0] + ".dll");
      if (File.Exists(fullFileName))
      {

        // search the context
        foreach (var context1 in AssemblyLoadContext.All)
        {
          if (context1.Assemblies.FirstOrDefault(x => x == requestingAssembly) is not null)
          {
            context = context1;
            break;
          }
        }
        context ??= new PluginAssemblyLoadContext(fullFileName);
        result = context.LoadFromAssemblyPath(fullFileName);
        WriteAssemblyLoadingLog($"{typeof(PluginAssemblyLoadContext)}.EhDefaultResolving {name.Name} was resolved to {result}.");
        return result;
      }
      else
      {
        WriteAssemblyLoadingLog($"{typeof(PluginAssemblyLoadContext)}.EhDefaultResolving {name.Name} could not be resolved.");
        return null;
      }
    }

    private (Assembly? assembly, AssemblyLoadContext? context) FindAlreadyLoadedAssembly(AssemblyName name)
    {
      foreach (var context in AssemblyLoadContext.All)
      {
        if (context.Assemblies.FirstOrDefault(x => x.GetName().Name == name.Name) is { } foundAssembly)
        {
          return (foundAssembly, context);
        }
      }

      return (null, null);
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
      // var result = AppDomain.CurrentDomain.GetAssemblies().Where(ass => ass.GetName().Name == assemblyName.Name).FirstOrDefault();
      var (result, fromContext) = FindAlreadyLoadedAssembly(assemblyName);
      if (result is not null)
      {
        WriteAssemblyLoadingLog($"{this.GetType()}.Load ({this.Name}) of {assemblyName} was resolved with already loaded module {fromContext?.Name}.");
        return result;
      }


      // otherwise, we use the _resolver to resolve the dependent assembly
      var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
      if (assemblyPath is not null)
      {
        // if the assemblyPath could be determined, we look if the file has a .deps.json file, and if so, we load it into its own context
        var depsFilePath = Path.Combine(Path.GetDirectoryName(assemblyPath), Path.GetFileNameWithoutExtension(assemblyPath) + ".deps.json");
        if (File.Exists(depsFilePath))
        {
          WriteAssemblyLoadingLog($"{this.GetType()}.Load ({this.Name}) of {assemblyName} has its own dependency file and will therefore be loaded into its own context.");
          var newContext = new PluginAssemblyLoadContext(assemblyPath);
          result = newContext.LoadFromAssemblyPath(assemblyPath);
          if (result is not null)
          {
            return result;
          }
          else
          {
            WriteAssemblyLoadingLog($"{this.GetType()}.Load ({this.Name}) of {assemblyName} has its own dependency file and therefore was tried to loaded into its own context, but failed!");
          }
        }

        // note that we load the dependent assemblies into the default load context,
        // and not in this context here
        // by this way we avoid that we load the same assembly in different contexts
        var destinationContext = this; // we could also use Default here!
        result = LoadFromAssemblyPath(assemblyPath);
        if (result is not null)
        {
          WriteAssemblyLoadingLog($"{this.GetType()}.Load ({this.Name}) of {assemblyName} into context '{destinationContext.Name}', result={result}");
          return result;
        }
      }
      else
      {
        WriteAssemblyLoadingLog($"{this.GetType()}.Load ({this.Name}) of {assemblyName} AssemblyPath could not be resolved!");
      }

      var dirInfo = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!);
      var fileInfo = dirInfo.EnumerateFiles(assemblyName.Name + ".dll").FirstOrDefault();
      if (fileInfo is not null)
      {
        // note that we load the dependent assemblies into the default load context,
        // and not in this context here
        // by this way we avoid that we load the same assembly in different contexts
        result = LoadFromAssemblyPath(fileInfo.FullName);

        if (result is not null)
        {
          WriteAssemblyLoadingLog($"{this.GetType()}.Load ({this.Name}) of {assemblyName} was resolved latest, result={result}");
          return result;
        }
      }

      WriteAssemblyLoadingLog($"{this.GetType()}.Load ({this.Name}) of {assemblyName}  FAILURE (could not be resolved)");
      return null;
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
      var result = IntPtr.Zero;
      if (libraryPath is not null)
      {
        result = LoadUnmanagedDllFromPath(libraryPath);
      }

      WriteAssemblyLoadingLog($"{this.GetType()}.LoadUnmanagedDll ({this.Name}) UnmanagedDllFileName={unmanagedDllName}, libraryPath={libraryPath}, result={result}");
      return result;
    }
  }
}
#endif

