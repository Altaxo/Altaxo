#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Altaxo
{
  /// <summary>
  /// This Class is the Core main class, it starts the program.
  /// </summary>
  internal class StartupMain
  {
    /// <summary>
    /// Starts Altaxo
    /// </summary>
    [STAThread()]
    public static void Main(string[] args)
    {
      // Make sure to use nothing except from this Assembly (event not derived from another assembly)
      // until the splash screen is shown
      // otherwise other assemblies will be loaded before the splash screen is visible

      if (AttachDebugger(args))
      {
        System.Diagnostics.Debugger.Launch();
        if (System.Diagnostics.Debugger.IsAttached)
        {
          System.Diagnostics.Debugger.Break();
        }
        else
        {
          Console.Write("Wait for a debugger to be attached...");
          for (int i = 0; i < 30; ++i)
          {
            Console.Write(".");
            System.Threading.Thread.Sleep(1000);
          }
          System.Diagnostics.Debugger.Break();
        }
      }

      string entryAssemblyName = "Workbench.dll";
      string entryClassName = "Altaxo.Gui.Startup.StartupMain";
      string entryMethodName = "Main";

      var entryAssemblyDirectoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
      var dirInfo = new DirectoryInfo(entryAssemblyDirectoryName);
      var resolvedFile = dirInfo.GetFiles(entryAssemblyName, SearchOption.AllDirectories).FirstOrDefault();

      if (resolvedFile is null)
      {
        throw new ApplicationException($"Can not find start assembly {entryAssemblyName} in folder {entryAssemblyDirectoryName}!");
      }

      var context = new StartupAssemblyLoadContext(entryAssemblyName);
      var startupAssembly = context.LoadFromAssemblyPath(resolvedFile.FullName);

      var startupClassType = startupAssembly.GetType(entryClassName);

      var startupMethod = startupClassType.GetMethod(entryMethodName, BindingFlags.Static | BindingFlags.Public);

      var startupMethodArgs = new object[2];
      startupMethodArgs[0] = args;
      startupMethodArgs[1] = context;
      startupMethod.Invoke(null, startupMethodArgs);
    }


    private static bool AttachDebugger(params string[] startupArgs)
    {
      foreach (string arg in startupArgs ?? Array.Empty<string>())
      {
        if (arg == "--attachDebugger")
          return true;
      }
      return false;
    }
  }
}
