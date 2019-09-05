#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

namespace Altaxo.Main.Commands
{
  using System.Collections.Generic;
  using Altaxo.AddInItems;
  using Altaxo.Data;
  using Altaxo.Gui;
  using Altaxo.Gui.AddInItems;
  using Altaxo.Main.Services;
  using Altaxo.Scripting;

  public class CreateNewWorksheet : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      Current.ProjectService.CreateNewWorksheet();
    }
  }

  public class CreateNewStandardWorksheet : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      var controller = Current.ProjectService.CreateNewWorksheet();
      controller.DataTable.AddStandardColumns();
    }
  }

  public class CreateNewGraph : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      Current.ProjectService.CreateNewGraph();
    }
  }

  public class CreateNewText : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      var doc = Current.ProjectService.CreateDocument<Altaxo.Text.TextDocument>(Altaxo.Main.ProjectFolder.RootFolderName);
      Current.ProjectService.OpenOrCreateViewContentForDocument(doc);
    }
  }

  public class CreateNewWorksheetOrGraphFromFile : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      var dlg = new OpenFileOptions();

      dlg.AddFilter("*.axowks;*.axogrp", "Worksheet or graph files(*.axowks; *.axogrp");
      dlg.AddFilter("*.*", "All files (*.*)");
      dlg.RestoreDirectory = true;
      dlg.FilterIndex = 0;
      dlg.Multiselect = true;

      if (Current.Gui.ShowOpenFileDialog(dlg))
      {
        var failedToOpen = new List<string>();
        foreach (var fileName in dlg.FileNames)
        {
          if (!Current.IProjectService.TryOpenProjectDocumentFile(fileName, forceTrialRegardlessOfExtension: true))
          {
            failedToOpen.Add(fileName);
          }
        }

        if (failedToOpen.Count > 0)
        {
          var stb = new System.Text.StringBuilder();
          stb.AppendFormat("The following {0} of {1} file(s) could not be opened:\r\n", failedToOpen.Count, dlg.FileNames.Length);
          foreach (var fileName in failedToOpen)
            stb.AppendFormat("{0}\r\n", fileName);

          Current.Gui.ErrorMessageBox(stb.ToString(), "Error opening file(s)");
        }
      }
    }
  }

  public class FileOpen : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      if (Current.Project.IsDirty)
      {
        var cancelargs = new System.ComponentModel.CancelEventArgs();
        Current.IProjectService.AskForSavingOfProject(cancelargs);
        if (cancelargs.Cancel)
          return;
      }

      bool saveDirtyState = Current.Project.IsDirty; // save the dirty state of the project in case the user cancels the open file dialog
      Current.Project.IsDirty = false; // set document to non-dirty

      var openFileDialog1 = new Microsoft.Win32.OpenFileDialog
      {
        Filter = "Altaxo project files (*.axoprj)|*.axoprj|All files (*.*)|*.*",
        FilterIndex = 1,
        RestoreDirectory = true
      };

      if (true == openFileDialog1.ShowDialog((System.Windows.Window)Current.Workbench.ViewObject))
      {
        Current.IProjectService.OpenProject(FileName.Create(openFileDialog1.FileName), false);
        Current.GetService<IRecentOpen>().AddRecentProject(FileName.Create(openFileDialog1.FileName));
      }
      else // in case the user cancels the open file dialog
      {
        Current.Project.IsDirty = saveDirtyState; // restore the dirty state of the current project
      }
    }
  }

  public class FileSaveAs : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      Current.IProjectService.SaveProjectAs();
    }
  }

  public class FileSave : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      if (Current.IProjectService.CurrentProjectFileName != null)
        Current.IProjectService.SaveProject();
      else
        Current.IProjectService.SaveProjectAs();
    }
  }

  /// <summary>
  /// This command is used if in embedded object mode. It saves the current project to a file,
  /// but don't set the current file name of the project (in project service). Furthermore, the title in the title bar is not influenced by the saving.
  /// </summary>
  public class FileSaveCopyAs : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      Current.IProjectService.SaveProjectCopyAs();
    }
  }

  public class FileImportAscii : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      if (Current.Workbench.ActiveViewContent is Altaxo.Gui.Worksheet.Viewing.WorksheetController controller)
      {
        Altaxo.Data.FileCommands.ShowImportAsciiDialog(controller.DataTable);
      }
      else
      {
        Altaxo.Data.FileCommands.ShowImportAsciiDialog(null, true, false);
      }
    }
  }

  public class FileImportAsciiWithOptions : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      if (Current.Workbench.ActiveViewContent is Altaxo.Gui.Worksheet.Viewing.WorksheetController controller)
      {
        Altaxo.Data.FileCommands.ShowImportAsciiDialogAndOptions(controller.DataTable);
      }
      else
      {
        Altaxo.Data.FileCommands.ShowImportAsciiDialogAndOptions(null, true, false);
      }
    }
  }

  public class CloseProject : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      Current.IProjectService.CloseProject(false);
    }
  }

  public class FileExit : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      ((System.Windows.Window)Current.Workbench.ViewObject).Close();
    }
  }

  public class Duplicate : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      if (!(parameter is Gui.Workbench.IViewContent viewContent))
        viewContent = Current.Workbench.ActiveViewContent;

      if (viewContent is Altaxo.Gui.Worksheet.Viewing.WorksheetController)
      {
        new Altaxo.Worksheet.Commands.WorksheetDuplicate().Execute(parameter);
      }
      else if (viewContent is Altaxo.Gui.Graph.Gdi.Viewing.GraphController)
      {
        new Altaxo.Graph.Commands.DuplicateGraph().Execute(parameter);
      }
      else if (!(viewContent is null))
      {
        var viewModel = viewContent.ModelObject as IProjectItemPresentationModel;
        var doc = viewModel?.Document ?? viewContent.ModelObject as IProjectItem;
        if (null != doc)
        {
          var oldName = doc.Name;
          var newDoc = (IProjectItem)doc.Clone();

          Current.Project.AddItem(newDoc);
          Current.IProjectService.ShowDocumentView(newDoc);
        }
      }
    }
  }

  public class NewDocumentIdentifier : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      object oldId = Current.Project.DocumentIdentifier;
      if (Current.Gui.ShowDialog(ref oldId, "Enter new document identifier", false))
      {
        string newIdentifier = (string)oldId;

        if (Current.Project.DocumentIdentifier != newIdentifier)
        {
          Current.Project.DocumentIdentifier = newIdentifier;
          //Current.ProjectService.
        }
      }
    }
  }

  public class HelpAboutAltaxo : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      var ctrl = new Altaxo.Gui.Common.HelpAboutControl();

      ctrl.ShowDialog();
    }
  }

  public class FileImportOriginOpj : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      var openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
      {
        openFileDialog1.Filter = "OPJ files (*.opj)|*.opj|All files (*.*)|*.*";
        openFileDialog1.FilterIndex = 1;
        openFileDialog1.RestoreDirectory = true;
        openFileDialog1.Multiselect = false;

        if (true == openFileDialog1.ShowDialog((System.Windows.Window)Current.Workbench.ViewObject) && openFileDialog1.FileName.Length > 0)
        {
          string result = Altaxo.Serialization.Origin.Importer.Import(openFileDialog1.FileName);
          if (result != null)
            Current.Gui.ErrorMessageBox(result);
        }
      }
    }
  }

  public class NewInstanceScript : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      Altaxo.Scripting.IScriptText script = null; // or load it from somewhere

      if (script == null)
        script = new Altaxo.Scripting.ProgramInstanceScript();
      var options = new Altaxo.Gui.OpenFileOptions
      {
        Title = "Open a script or press Cancel to create a new script"
      };
      options.AddFilter("*.cs", "C# files (*.cs)");
      options.AddFilter("*.*", "All files (*.*)");
      options.FilterIndex = 0;
      if (Current.Gui.ShowOpenFileDialog(options))
      {
        string err = OpenScriptText(options.FileName, out var scripttext);
        if (null != err)
          Current.Gui.ErrorMessageBox(err);
        else
          script.ScriptText = scripttext;
      }

      object[] args = new object[] { script, new Altaxo.Gui.Scripting.ScriptExecutionHandler(EhScriptExecution) };
      if (Current.Gui.ShowDialog(args, "New instance script"))
      {
        string errors = null;
        do
        {
          errors = null;
          // store the script somewhere m_Table.TableScript = (TableScript)args[0];
          var saveOptions = new Altaxo.Gui.SaveFileOptions
          {
            Title = "Save your script"
          };
          saveOptions.AddFilter("*.cs", "C# files (*.cs)");
          saveOptions.AddFilter("*.*", "All files (*.*)");
          saveOptions.FilterIndex = 0;
          if (Current.Gui.ShowSaveFileDialog(saveOptions))
          {
            errors = SaveScriptText(saveOptions.FileName, script.ScriptText);
            if (null != errors)
              Current.Gui.ErrorMessageBox(errors);
          }
        } while (null != errors);
      }
    }

    public bool EhScriptExecution(Altaxo.Scripting.IScriptText script, IProgressReporter reporter)
    {
      return ((Altaxo.Scripting.ProgramInstanceScript)script).Execute(reporter);
    }

    private string SaveScriptText(string filename, string text)
    {
      try
      {
        using (var stream = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read))
        {
          var wr = new System.IO.StreamWriter(stream);
          wr.Write(text);
          wr.Close();
          stream.Close();
        }
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
      return null;
    }

    private string OpenScriptText(string filename, out string scripttext)
    {
      scripttext = null;
      try
      {
        using (var stream = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
        {
          var sr = new System.IO.StreamReader(stream);
          scripttext = sr.ReadToEnd();
          sr.Close();
          stream.Close();
        }
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
      return null;
    }


  }

  /// <summary>
  /// This command saves the project without worksheet scripts, and then
  /// restores the scripts.
  /// </summary>
  public class SaveProjectWithoutWorksheetScripts : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      var dict = new Dictionary<DataTable, TableScript>();

      foreach (var table in Current.Project.DataTableCollection)
      {

        dict.Add(table, table.TableScript);
        table.TableScript = null;
      }

      var oldFileName = Current.ProjectService.CurrentProjectFileName;


      try
      {
        Current.ProjectService.CurrentProjectFileName = null;

        Current.ProjectService.SaveProjectAs();
      }
      finally
      {
        foreach (var table in Current.Project.DataTableCollection)
        {
          table.TableScript = dict[table];
        }

        Current.ProjectService.CurrentProjectFileName = oldFileName;
      }

    }
  }
}
