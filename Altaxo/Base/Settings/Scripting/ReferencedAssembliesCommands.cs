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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Altaxo.Settings.Scripting
{
  using Altaxo.Gui;

  /// <summary>
  /// Contains commands related to addition/removal of referenced assemblies
  /// </summary>
  public static class ReferencedAssembliesCommands
  {
    /// <summary>
    /// This shows a dialog where the user can add a referenced assembly temporarily.
    /// </summary>
    public static void ShowAddTemporaryAssemblyDialog()
    {
      var options = new OpenFileOptions
      {
        Title = "Add a temporary assembly to be referenced"
      };
      options.AddFilter("*.dll", "Libary files (*.dll)");
      options.AddFilter("*.*", "All files (*.*)");
      options.FilterIndex = 0;
      options.Multiselect = true;
      options.InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
      if (Current.Gui.ShowOpenFileDialog(options))
      {
        var stb = new StringBuilder();
        // try to create an assembly out of the filename(s)
        foreach (string filename in options.FileNames)
        {
          Assembly asm = null;
          try
          {
            asm = Assembly.LoadFrom(filename);
          }
          catch (Exception ex)
          {
            stb.AppendFormat("File {0} could not be loaded: {1}", filename, ex.Message);
            continue;
          }

          ReferencedAssemblies.AddTemporaryUserAssembly(asm);
        }

        if (stb.Length != 0)
          Current.Gui.ErrorMessageBox(stb.ToString());
      }
    }
  }
}
