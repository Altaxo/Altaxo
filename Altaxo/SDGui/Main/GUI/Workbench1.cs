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
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Services;
using Reflector.UserInterface;

namespace ICSharpCode.SharpDevelop.Gui
{
  /// <summary>
  /// This is the a Workspace with a multiple document interface.
  /// </summary>
  public class Workbench1 : DefaultWorkbench, Altaxo.Main.GUI.IWorkbench
  {
    readonly static string mainMenuPath    = "/Altaxo/Workbench/MainMenu";
    readonly static string viewContentPath = "/SharpDevelop/Workbench/Views";

    #region "Serialization"
  
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Workbench1),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo  info)
      {
        Workbench1 s = (Workbench1)obj;
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo  info, object parent)
      {
        return o;
      }
    }



    #endregion

    public Workbench1()
      : base()
    {
      ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
      Icon = resourceService.GetIcon("Icons.MainApplicationIcon");
    }

    public new void InitializeWorkspace()
    {
      Menu = null;
      
      //      statusBarManager.Control.Dock = DockStyle.Bottom;
      
      ActiveWorkbenchWindowChanged += new EventHandler(UpdateMenu);

      ActiveWorkbenchWindowChanged += new EventHandler(EhAltaxoFireContentChanged);
      
      MenuComplete += new EventHandler(SetStandardStatusBar);
      SetStandardStatusBar(null, null);
      
#if OriginalSharpDevelopCode
      IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
      IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
      
      projectService.CurrentProjectChanged += new ProjectEventHandler(SetProjectTitle);
      projectService.CombineOpened         += new CombineEventHandler(CombineOpened);
      fileService.FileRemoved += new FileEventHandler(CheckRemovedFile);
      fileService.FileRenamed += new FileEventHandler(CheckRenamedFile);
      
      fileService.FileRemoved += new FileEventHandler(fileService.RecentOpen.FileRemoved);
      fileService.FileRenamed += new FileEventHandler(fileService.RecentOpen.FileRenamed);
#endif
      
      //      TopMenu.Selected   += new CommandHandler(OnTopMenuSelected);
      //      TopMenu.Deselected += new CommandHandler(OnTopMenuDeselected);
      CreateMainMenu();
      CreateToolBars();
    }


    void UpdateMenu(object sender, EventArgs e)
    {
      // update menu
      foreach (object o in TopMenu.Items) 
      {
        if (o is IStatusUpdate) 
        {
          ((IStatusUpdate)o).UpdateStatus();
        }
      }
      
      UpdateToolbars();
    }

    void SetStandardStatusBar(object sender, EventArgs e)
    {
      IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
      statusBarService.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
    }

    void CreateMainMenu()
    {
      TopMenu = new CommandBar(CommandBarStyle.Menu);
      CommandBarItem[] items = (CommandBarItem[])(AddInTreeSingleton.AddInTree.GetTreeNode(mainMenuPath).BuildChildItems(this)).ToArray(typeof(CommandBarItem));
      TopMenu.Items.Clear();
      TopMenu.Items.AddRange(items);
    }

    // this method simply copies over the enabled state of the toolbar,
    // this assumes that no item is inserted or removed.
    // TODO : make this method more add-in tree like, currently with Windows.Forms
    //        toolbars this is not possible. (drawing fragments, slow etc.)
    void CreateToolBars()
    {
      if (ToolBars == null) 
      {
#if OriginalSharpDevelopCode
        ToolbarService toolBarService = (ToolbarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ToolbarService));
#else
        // Note: original ToolbarService don't support checked toolbar items
        AltaxoToolbarService toolBarService = (AltaxoToolbarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(AltaxoToolbarService));
#endif
        CommandBar[] toolBars = toolBarService.CreateToolbars();
        ToolBars = toolBars;
      }
    }

    public new void UpdateViews(object sender, EventArgs e)
    {
      IPadContent[] contents = (IPadContent[])(AddInTreeSingleton.AddInTree.GetTreeNode(viewContentPath).BuildChildItems(this)).ToArray(typeof(IPadContent));
      foreach (IPadContent content in contents) 
      {
        ShowPad(content);
      }
    }

    public void EhProjectChanged(object sender, Altaxo.Main.ProjectEventArgs e)
    {
      UpdateMenu(null, null);
      ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
      Altaxo.Main.ProjectService projectService = (Altaxo.Main.ProjectService)ServiceManager.Services.GetService(typeof(Altaxo.Main.ProjectService));
      System.Text.StringBuilder title = new System.Text.StringBuilder();
      title.Append(resourceService.GetString("MainWindow.DialogName"));
      if (projectService != null) 
      {
        if(projectService.CurrentProjectFileName == null)
        {
          title.Append(" - ");
          title.Append(resourceService.GetString("Altaxo.Project.UntitledName"));
        }
        else
        {
          title.Append(" - ");
          title.Append(projectService.CurrentProjectFileName);
        }
        if(projectService.CurrentOpenProject!= null && projectService.CurrentOpenProject.IsDirty)
          title.Append("*");
      } 
      this.Title = title.ToString();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      Altaxo.Main.ProjectService projectService = (Altaxo.Main.ProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(Altaxo.Main.ProjectService));
      
      if (projectService != null)
      {
        // projectService.SaveCombinePreferences();
      
        if(projectService.CurrentOpenProject != null && projectService.CurrentOpenProject.IsDirty)
        {
          projectService.AskForSavingOfProject(e);
        }
      }
      if(!e.Cancel)
      {
        base.OnClosing(e);
      }
    }
    #region Altaxo.Main.Gui.IWorkbench Members

    ICollection Altaxo.Main.GUI.IWorkbench.ViewContentCollection
    {
      get
      {
        return this.ViewContentCollection;
      }
    }

    public object ViewObject
    {
      get
      {
        return this;
      }
    }

    object Altaxo.Main.GUI.IWorkbench.ActiveViewContent
    {
      get
      {
        return null!=this.ActiveWorkbenchWindow ? this.ActiveWorkbenchWindow.ActiveViewContent : null;
      }
    }

    void Altaxo.Main.GUI.IWorkbench.ShowView(object o)
    {
      base.ShowView((IViewContent)o);
    }

    void Altaxo.Main.GUI.IWorkbench.CloseContent(object o)
    {
      base.CloseContent((IViewContent)o);
    }

    void Altaxo.Main.GUI.IWorkbench.CloseAllViews()
    {
      base.CloseAllViews();
    }

    /// <summary>Fired if the current view (and so the view content) changed.</summary>
    public event EventHandler ActiveViewContentChanged;

    protected void EhAltaxoFireContentChanged(object o, EventArgs e)
    {
      if(null!=ActiveViewContentChanged)
        ActiveViewContentChanged(this,e);
    }

    #endregion
  }
}
