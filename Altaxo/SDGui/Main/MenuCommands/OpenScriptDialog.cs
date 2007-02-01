#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;


namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// Menu point to provoke an exception.
  /// </summary>
  public class AltaxoProvokeException : AbstractMenuCommand
  {
      bool _disable;
      internal bool Disable { set { _disable = value; } }
    public override void Run()
    {
      if (!_disable)
        throw new ApplicationException("This is a menu point to provoke an exception");
      System.Diagnostics.Debug.WriteLine("Exception thrown");
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
        IClipboardHandler editable = window != null ? window.ActiveViewContent as IClipboardHandler : null;
        if (editable != null) 
        {
          return editable.EnableCut;
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
        IClipboardHandler editable = window != null ? window.ActiveViewContent as IClipboardHandler : null;
        if (editable != null) 
        {
          editable.Cut();
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
        IClipboardHandler editable = window != null ? window.ActiveViewContent as IClipboardHandler : null;
        if (editable != null) 
        {
          return editable.EnableCopy;
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
        IClipboardHandler editable = window != null ? window.ActiveViewContent as IClipboardHandler : null;
        if (editable != null) 
        {
          editable.Copy();
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
        IClipboardHandler editable = window != null ? window.ActiveViewContent as IClipboardHandler : null;
        if (editable != null) 
        {
          return editable.EnablePaste;
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
        IClipboardHandler editable = window != null ? window.ActiveViewContent as IClipboardHandler : null;
        if (editable != null) 
        {
          editable.Paste();
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
        IClipboardHandler editable = window != null ? window.ActiveViewContent as IClipboardHandler : null;
        if (editable != null) 
        {
          return editable.EnableDelete;
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

      if (window != null && window.ViewContent is IClipboardHandler) 
      {
        if (((IClipboardHandler)window.ViewContent) != null) 
        {
          ((IClipboardHandler)window.ViewContent).Delete();
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
            ICSharpCode.SharpDevelop.Project.ProjectService.MarkFileDirty(window.ViewContent.TitleName);
            FileUtility.ObservedSave(new FileOperationDelegate(window.ViewContent.Save), window.ViewContent.TitleName);
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
          fdiag.AddExtension = true;

          string[] fileFilters = (string[])(AddInTree.GetTreeNode("/SharpDevelop/Workbench/FileFilter").BuildChildItems(this)).ToArray(typeof(string));
          fdiag.Filter = String.Join("|", fileFilters);
          for (int i = 0; i < fileFilters.Length; ++i)
          {
            if (fileFilters[i].IndexOf(Path.GetExtension(window.ViewContent.FileName == null ? window.ViewContent.UntitledName : window.ViewContent.FileName)) >= 0)
            {
              fdiag.FilterIndex = i + 1;
              break;
            }
          }

          if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK)
          {
            string fileName = fdiag.FileName;



            if (!FileService.CheckFileName(fileName))
            {
              return;
            }

            if (FileUtility.ObservedSave(new NamedFileOperationDelegate(window.ViewContent.Save), fileName) == FileOperationResult.OK)
            {
              FileService.RecentOpen.AddLastFile(fileName);

              MessageService.ShowMessage(fileName, "${res:ICSharpCode.SharpDevelop.Commands.SaveFile.FileSaved}");
            }
          }
        }
      }
    }
  }

}
