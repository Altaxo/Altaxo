using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.Reflection;

namespace Altaxo.Settings.Scripting
{
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
      System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
      dlg.Title = "Add a temporary assembly to be referenced";
      dlg.DefaultExt = "asm";
      dlg.Filter = "Libary files (*.dll)|*.dll|All files (*.*)|*.*";
      dlg.FilterIndex = 0;
      dlg.Multiselect = true;
      dlg.InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
      if (DialogResult.OK == dlg.ShowDialog(Current.MainWindow))
      {
        StringBuilder stb = new StringBuilder();
        // try to create an assembly out of the filename(s)
        foreach (string filename in dlg.FileNames)
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
