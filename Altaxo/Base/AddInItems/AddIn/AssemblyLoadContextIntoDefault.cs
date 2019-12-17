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

#if NETSTANDARD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Altaxo.AddInItems
{

  /// <summary>
  /// Assembly load context to load not only DLLs dynamically, but also their dependencies.
  /// The assemblies are loaded into the default context.
  /// </summary>
  public class AssemblyLoadContextIntoDefault : AssemblyLoadContext
  {
    /// <summary>
    /// Stores directory infos of root directories used to search the assembly.
    /// </summary>
    private System.IO.DirectoryInfo[] _entryPathInfos;

    /// <summary>
    /// Creates an <see cref="AssemblyLoadContext"/> that will resolve assemblies using the entry assemblies directory and its subdirectories.
    /// </summary>
    public AssemblyLoadContextIntoDefault()
    {
      AssemblyLocator.Init();

      _entryPathInfos = new System.IO.DirectoryInfo[1];
      _entryPathInfos[0] = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
      Resolving += EhResolving;
    }

    /// <summary>
    /// Creates an <see cref="AssemblyLoadContext"/> that will resolve assemblies using first the provided path including subdirectories thereof,
    /// and then the entry assemblies directory and its subdirectories.
    /// </summary>
    public AssemblyLoadContextIntoDefault(string assemblyPath)
    {
      AssemblyLocator.Init();

      _entryPathInfos = new System.IO.DirectoryInfo[2];
      _entryPathInfos[0] = new System.IO.DirectoryInfo(assemblyPath);
      _entryPathInfos[1] = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
      Resolving += EhResolving;
    }

    /// <summary>
    /// Tries to resolve the assembly, after resolving with the default context has failed.
    /// </summary>
    /// <param name="loadContext">The assembly load context (unused).</param>
    /// <param name="assemblyName">The assembly name.</param>
    /// <returns>The resolved assembly, or null if it could not be resolved.</returns>
    private Assembly EhResolving(AssemblyLoadContext loadContext, AssemblyName assemblyName)
    {
      return LoadFromPartialName(assemblyName.Name);
    }

    /// <summary>
    /// Tries to resolve the assembly, after resolving with the default context has failed.
    /// </summary>
    /// <param name="assemblyShortName">The assembly name.</param>
    /// <returns>The resolved assembly, or null if it could not be resolved.</returns>
    public Assembly LoadFromPartialName(string assemblyShortName)
    {
      foreach (var pathInfo in _entryPathInfos)
      {
        var fileInfo = pathInfo.EnumerateFiles(assemblyShortName + ".dll", System.IO.SearchOption.AllDirectories).FirstOrDefault();
        if (null != fileInfo)
        {
          return Default.LoadFromAssemblyPath(fileInfo.FullName);
        }
      }
      return null;
    }



    /// <summary>
    /// The override of Load here returns null. This delegates the task of resolving to the default resolver.
    /// If the default resolver fails to resolve the name, then the resolving event is fired, and <see cref="EhResolving(AssemblyLoadContext, AssemblyName)"/> is called,
    /// which does the actual resolving in this class.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    protected override Assembly Load(AssemblyName assemblyName)
    {
      return null;
    }
  }
}
#endif
