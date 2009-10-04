using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.Reflection;

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
			OpenFileOptions options = new OpenFileOptions();
			options.DialogTitle = "Add a temporary assembly to be referenced";
			options.AddFilter("*.dll", "Libary files (*.dll)");
			options.AddFilter("*.*", "All files (*.*)");
      options.FilterIndex = 0;
      options.Multiselect = true;
      options.InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			if(Current.Gui.ShowOpenFileDialog(options))
      {
        StringBuilder stb = new StringBuilder();
        // try to create an assembly out of the filename(s)
        foreach (string filename in options.FileNames)
        {
          Assembly asm=null;
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
