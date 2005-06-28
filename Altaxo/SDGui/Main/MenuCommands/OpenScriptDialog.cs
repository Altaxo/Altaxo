#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using System.IO;
using System.Windows.Forms;

using Altaxo;
using Altaxo.Main;
using Altaxo.Worksheet;
using Altaxo.Worksheet.GUI;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;


namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// Summary description for OpenScriptDialog.
  /// </summary>
  public class OpenScriptDialog : AbstractMenuCommand
  {
    public override void Run()
    {

      throw new ApplicationException("This is a menu point to provoke an exception");
      
      /*
      Form form = new Form();
    

      ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBindingWrapper ctrl =
        new ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBindingWrapper();

      ctrl.TextEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
      form.Controls.Add(ctrl.TextEditorControl);
      ctrl.TextEditorControl.FileName = @"blablatest01.CS";
      ctrl.ContentName = @"blablatest01.CS";


      ICSharpCode.SharpDevelop.Services.DefaultParserService parserService = (ICSharpCode.SharpDevelop.Services.DefaultParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ICSharpCode.SharpDevelop.Services.DefaultParserService));

      if(parserService!=null)
      {
        parserService.ShowDialog(form,Altaxo.Current.MainWindow,ctrl);
      }
      else
      {
        form.ShowDialog(Altaxo.Current.MainWindow);
      }
      
      */
    }

  }
}
namespace Altaxo.Main.Commands.ScriptEditorCommands
{
  public class Cut : AbstractMenuCommand
  {
    public override bool IsEnabled 
    {
      get 
      {
        if(Owner is ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)
          return ((ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)Owner).ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCut;

        IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
        IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
        if (editable != null) 
        {
          return editable.ClipboardHandler.EnableCut;
        }
        return false;
      }
    }
    
    public override void Run()
    {
      if (IsEnabled) 
      {
        if(Owner is ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)
        {
          ((ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)Owner).ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null,null);
          return;
        }

        IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
        IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
        if (editable != null) 
        {
          editable.ClipboardHandler.Cut(null, null);
        }
      }
    }
  }
  
  public class Copy : AbstractMenuCommand
  {
    public override bool IsEnabled 
    {
      get 
      {
        if(Owner is ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)
          return ((ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)Owner).ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCopy;

        IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
        IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
        if (editable != null) 
        {
          return editable.ClipboardHandler.EnableCopy;
        }
        return false;
      }
    }
    
    public override void Run()
    {
      if (IsEnabled) 
      {
        if(Owner is ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)
        {
          ((ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)Owner).ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(null,null);
          return;
        }
        IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
        IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
        if (editable != null) 
        {
          editable.ClipboardHandler.Copy(null, null);
        }
      }
    }
  }
  
  public class Paste : AbstractMenuCommand
  {
    public override bool IsEnabled 
    {
      get 
      {
        if(Owner is ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)
          return ((ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)Owner).ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste;

        IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
        IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
        if (editable != null) 
        {
          return editable.ClipboardHandler.EnablePaste;
        }
        return false;
      }
    }
    public override void Run()
    {
      if (IsEnabled) 
      {
        if(Owner is ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)
        {
          ((ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)Owner).ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(null,null);
          return;
        }
        IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
        IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
        if (editable != null) 
        {
          editable.ClipboardHandler.Paste(null, null);
        }
      }
    }
  }
  
  public class Delete : AbstractMenuCommand
  {
    public override bool IsEnabled 
    {
      get 
      {
        if(Owner is ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)
          return ((ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)Owner).ActiveTextAreaControl.TextArea.ClipboardHandler.EnableDelete;

        IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
        IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
        if (editable != null) 
        {
          return editable.ClipboardHandler.EnableDelete;
        }
        return false;
      }
    }
    public override void Run()
    {
      if(Owner is ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)
      {
        ((ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.SharpDevelopTextAreaControl)Owner).ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(null,null);
        return;
      }

      IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
      
      if (window != null && window.ViewContent is IEditable) 
      {
        if (((IEditable)window.ViewContent).ClipboardHandler != null) 
        {
          ((IEditable)window.ViewContent).ClipboardHandler.Delete(null, null);
        }
      }
    }
  }



  public class CloseFile : AbstractMenuCommand
  {
    public override void Run()
    {
      if(System.Windows.Forms.Form.ActiveForm.Modal)
        return; 

      if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) 
      {
        WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.CloseWindow(false);
      }
    }
  }

  public class SaveFile : AbstractMenuCommand
  {
    public override void Run()
    {

      if(System.Windows.Forms.Form.ActiveForm.Modal)
        return; 

      IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
      if (window != null) 
      {
        if (window.ViewContent.IsViewOnly) 
        {
          return;
        }
        
        if (window.ViewContent.TitleName == null) 
        {
          SaveFileAs sfa = new SaveFileAs();
          sfa.Run();
        } 
        else 
        {
          FileAttributes attr = FileAttributes.ReadOnly | FileAttributes.Directory | FileAttributes.Offline | FileAttributes.System;
          if ((File.GetAttributes(window.ViewContent.TitleName) & attr) != 0) 
          {
            SaveFileAs sfa = new SaveFileAs();
            sfa.Run();
          } 
          else 
          {
            ICSharpCode.SharpDevelop.Services.IProjectService projectService = (ICSharpCode.SharpDevelop.Services.IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ICSharpCode.SharpDevelop.Services.IProjectService));
            FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
            projectService.MarkFileDirty(window.ViewContent.TitleName);
            fileUtilityService.ObservedSave(new FileOperationDelegate(window.ViewContent.Save), window.ViewContent.TitleName);
          }
        }
      }
    }
  } 



  public class SaveFileAs : AbstractMenuCommand
  {
    public override void Run()
    {
      if(System.Windows.Forms.Form.ActiveForm.Modal)
        return; 


      IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
      
      if (window != null) 
      {
        if (window.ViewContent.IsViewOnly) 
        {
          return;
        }
        if (window.ViewContent is ICustomizedCommands) 
        {
          if (((ICustomizedCommands)window.ViewContent).SaveAsCommand()) 
          {
            return;
          }
        }
        using (SaveFileDialog fdiag = new SaveFileDialog()) 
        {
          fdiag.OverwritePrompt = true;
          fdiag.AddExtension    = true;
          
          string[] fileFilters  = (string[])(AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/FileFilter").BuildChildItems(this)).ToArray(typeof(string));
          fdiag.Filter          = String.Join("|", fileFilters);
          for (int i = 0; i < fileFilters.Length; ++i) 
          {
            if (fileFilters[i].IndexOf(Path.GetExtension(window.ViewContent.TitleName == null ? window.ViewContent.UntitledName : window.ViewContent.TitleName)) >= 0) 
            {
              fdiag.FilterIndex = i + 1;
              break;
            }
          }
          
          if (fdiag.ShowDialog() == DialogResult.OK) 
          {
            string fileName = fdiag.FileName;
            
            IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
            FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
            if (!fileUtilityService.IsValidFileName(fileName)) 
            {
              IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
              messageService.ShowMessage("File name " + fileName +" is invalid");
              return;
            }
            
            if (fileUtilityService.ObservedSave(new NamedFileOperationDelegate(window.ViewContent.Save), fileName) == FileOperationResult.OK) 
            {
              fileService.RecentOpen.AddLastFile(fileName);
              IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
              messageService.ShowMessage(fileName, "File saved");
            }
          }
        }
      }
    }
  }

}
