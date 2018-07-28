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

using Altaxo.AddInItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Altaxo.Settings.Scripting
{
  public delegate void AssemblyAddedEventHandler(Assembly added);

  /// <summary>
  /// This class is responsible for holding the name of all those assemblies that should be referenced
  /// in scripts.
  /// </summary>
  public static class ReferencedAssemblies
  {
    /// <summary>
    /// Gets a value indicating whether a .Net framework version >= 4.7  is installed.
    /// When it is, the DLL System.ValueTuple.dll must not be included in the list of referenced assemblies, because it is built-in then.
    /// </summary>
    public static bool IsFrameworkVersion47Installed { get; private set; } = Altaxo.Serialization.AutoUpdates.NetFrameworkVersionDetermination.IsVersion47Installed();

    private static List<Assembly> _startupAssemblies = new List<Assembly>();
    private static List<Assembly> _userAssemblies = new List<Assembly>();
    private static List<Assembly> _userTemporaryAssemblies = new List<Assembly>();
    private static List<Assembly> _additionalReferencedAssemblies = new List<Assembly>();
    private static List<Assembly> _assemblyIncludedInClassReference;

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

      // try to load some assemblies given in the .addin file(s)
      // TODO the following code does not work properly, make it work!
      string addInPath = Altaxo.Main.Services.StringParser.Parse("/${AppName}/CodeEditing/AdditionalAssemblyReferences");
      IList<string> additionalUserAssemblyNames = AddInTree.BuildItems<string>(addInPath, null, false);
      var additionalReferencedAssemblies = new HashSet<Assembly>();
      foreach (var additionalUserAssemblyName in additionalUserAssemblyNames)
      {
        Assembly additionalAssembly = null;
        try
        {
          additionalAssembly = Assembly.Load(additionalUserAssemblyName);
          additionalReferencedAssemblies.Add(additionalAssembly);
        }
        catch (Exception ex)
        {
          Current.MessageService.ShowWarningFormatted("Assembly with name '{0}' that was given in {1} could not be loaded. Error: {2}", additionalUserAssemblyName, "/Altaxo/CodeEditing/AdditionalAssemblyReferences", ex.Message);
        }
      }

      _additionalReferencedAssemblies = new List<Assembly>(additionalReferencedAssemblies);

      // try to load the assemblies that are covered by the class reference file
      addInPath = Altaxo.Main.Services.StringParser.Parse("/${AppName}/CodeEditing/AssembliesIncludedInClassReference");
      IList<string> namesOfAssembliesIncludedInClassReference = AddInTree.BuildItems<string>(addInPath, null, false);
      var hash = new HashSet<string>(namesOfAssembliesIncludedInClassReference.Select(s => s.ToUpperInvariant()));

      var list = new List<Assembly>();
      foreach (var ass in All)
      {
        if (hash.Contains(ass.GetName().Name.ToUpperInvariant()))
          list.Add(ass);
      }

      _assemblyIncludedInClassReference = list;
    }

    private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
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

    private static void OnAssemblyAdded(Assembly asm)
    {
      if (null != AssemblyAdded)
        AssemblyAdded(asm);
    }

    /// <summary>
    /// Gets all assemblies that should be referenced when compiling a script.
    /// </summary>
    public static IEnumerable<Assembly> All
    {
      get
      {
        List<Assembly> list = new List<Assembly>();

        if (IsFrameworkVersion47Installed)
        {
          // Do not include System.ValueTuple.dll in the list of referenced assemblies if .NetFrameworkVersion is > 4.7, because it is intrinsically there
          list.AddRange(_startupAssemblies.Where(ass => !(ass.GetName().Name.ToUpperInvariant().StartsWith("SYSTEM.VALUETUPLE"))));
        }
        else
        {
          list.AddRange(_startupAssemblies);
        }

        list.AddRange(_userTemporaryAssemblies);

        list.AddRange(_additionalReferencedAssemblies);

        return list;
      }
    }

    /// <summary>
    /// Determines if a list of assembly contains an assembly with the same location than a given assembly.
    /// </summary>
    /// <param name="asm"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    private static int FindAssemblyInList(Assembly asm, List<Assembly> list)
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

    /// <summary>
    /// Gets the assemblies that are included in the class reference help file. This enumeration can be used to decide
    /// whether to look for help in the class reference help file or in the Microsoft library help.
    /// </summary>
    public static IEnumerable<Assembly> AssembliesIncludedInClassReference
    {
      get
      {
        return _assemblyIncludedInClassReference;
      }
    }
  }
}
