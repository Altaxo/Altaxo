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
        // this will include only those assemblies that have an external file
				if (!(asm is System.Reflection.Emit.AssemblyBuilder) && asm.Location != null && asm.Location != String.Empty)
				{
					lock (_startupAssemblies)
					{
						_startupAssemblies.Add(asm);
					}
				}
      }
		
		}

		static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			var asm = args.LoadedAssembly;
			// this will include only those assemblies that have an external file
			if (!(asm is System.Reflection.Emit.AssemblyBuilder) && asm.Location != null && asm.Location != String.Empty)
			{
				lock (_startupAssemblies)
				{
					_startupAssemblies.Add(asm);
				}
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
