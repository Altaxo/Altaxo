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
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Altaxo.Settings.Scripting
{
 

  public delegate void AssemblyAddedEventHandler(Assembly added);

  /// <summary>
  /// This class is responsible for holding the name of all those assemblies that should be referenced
  /// in scripts.
  /// </summary>
  public static class ReferencedAssemblies
  {
    static List<Assembly> _startupAssemblies = new List<Assembly>();
    static List<Assembly> _userAssemblies = new List<Assembly>();
    static List<Assembly> _userTemporaryAssemblies = new List<Assembly>();
    public static AssemblyAddedEventHandler AssemblyAdded;


    static ReferencedAssemblies()
    {
			var assembliesLoadedSoFar = AppDomain.CurrentDomain.GetAssemblies();
			// watch further loading of assemblies
			AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);


      // Add available assemblies including the application itself 
      foreach (Assembly asm in assembliesLoadedSoFar)
      {
				if (asm.IsDynamic || (asm is System.Reflection.Emit.AssemblyBuilder))
					continue;
				try
				{
					// this will include only those assemblies that have an external file
					// we put this in a try .. catch clause since for some assemblies asking for the location will cause an UnsupportedException
					if (string.IsNullOrEmpty(asm.Location))
						continue;
				}
				catch (Exception)
				{
					continue;
				}

        // now we can add the assemblies to the startup assembly list. Those assemblies that are added are not dynamic, and should have an external file location
				lock (_startupAssemblies)
				{
					_startupAssemblies.Add(asm);
				}
      }
		
		}

		static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			var asm = args.LoadedAssembly;

			if (asm.IsDynamic || (asm is System.Reflection.Emit.AssemblyBuilder))
				return;
			try
			{
				// this will include only those assemblies that have an external file
				// we put this in a try .. catch clause since for some assemblies asking for the location will cause an UnsupportedException
				if (string.IsNullOrEmpty(asm.Location))
					return;
			}
			catch (Exception)
			{
				return;
			}

			// now our assembly is not a dynamic assembly, and has an an external file location
			lock (_startupAssemblies)
			{
				_startupAssemblies.Add(asm);
			}
		}
		

    public static void Initialize()
    {
    }

    static void OnAssemblyAdded(Assembly asm)
    {
      if (null != AssemblyAdded)
        AssemblyAdded(asm);
    }


    public static IEnumerable<Assembly> All
    {
      get
      {
        List<Assembly> list = new List<Assembly>();

        list.AddRange(_startupAssemblies);
        list.AddRange(_userTemporaryAssemblies);

        // now the user assemblies
//        foreach (Assembly asm in _userAssemblies)
  //        yield return asm;

        // now the temporary user assemblies
       // foreach (Assembly asm in _userTemporaryAssemblies)
         // yield return asm;
        return list;
      }
    }

    public static IEnumerable<string> AllLocations
    {
      get
      {
        foreach(Assembly ass in _startupAssemblies)
          yield return ass.Location;

        foreach(Assembly ass in _userAssemblies)
          yield return ass.Location;

        foreach(Assembly ass in _userTemporaryAssemblies)
          yield return ass.Location;
      }
    }

    /// <summary>
    /// Determines if a list of assembly contains an assembly with the same location than a given assembly.
    /// </summary>
    /// <param name="asm"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    static int FindAssemblyInList(Assembly asm, List<Assembly> list)
    {
      for (int i = 0; i < list.Count; i++)
        if (list[i].Location == asm.Location)
        {
          return i;
        }

      return -1;
    }

    /// <summary>
    /// Adds a assembly to the list of temporary user assemblies. If already in the list, it is assumed that
    /// the provided assembly is newer, so this assembly will replace that in the list.
    /// </summary>
    /// <param name="asm">Provided assembly to add.</param>
    public static void AddTemporaryUserAssembly(Assembly asm)
    {
      int idx = FindAssemblyInList(asm, _userTemporaryAssemblies);
      if (idx < 0)
        _userTemporaryAssemblies.Add(asm);
      else
        _userTemporaryAssemblies[idx] = asm;

      OnAssemblyAdded(asm);
    }
  }
}
