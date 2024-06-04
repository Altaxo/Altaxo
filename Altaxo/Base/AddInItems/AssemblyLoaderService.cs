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
    /// <param name="fullName">The full name of the assembly.</param>
    /// <returns>The assembly that was loaded, or null if the assembly was not found.</returns>
    public Assembly LoadAssemblyFromFullySpecifiedName(string fullName)
    {
#if NETFRAMEWORK
      return System.Reflection.Assembly.LoadFrom(fullName);
#else
      var context = new PluginAssemblyLoadContext(fullName);
      return context.LoadFromAssemblyPath(fullName);
#endif
    }
  }
}
