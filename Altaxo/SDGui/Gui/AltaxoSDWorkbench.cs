#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

using ICSharpCode.SharpDevelop;
using ICSharpCode.Core;
using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Gui;



namespace Altaxo.Gui
{
  /// <summary>
  /// This is the a Workspace with a multiple document interface.
  /// </summary>
  public class AltaxoSDWorkbench : DefaultWorkbench, Altaxo.Gui.Common.IWorkbench
  {

   

    #region "Serialization"
  
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui","ICSharpCode.SharpDevelop.Gui.Workbench1",0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AltaxoSDWorkbench), 1)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo  info)
      {
        AltaxoSDWorkbench s = (AltaxoSDWorkbench)obj;
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo  info, object parent)
      {
        return o;
      }
    }



    #endregion

    public AltaxoSDWorkbench()
      : base()
    {
      mainMenuPath = "/Altaxo/Workbench/MainMenu";
      viewContentPath = "/Altaxo/Workbench/Pads";
      toolBarPath = "/Altaxo/Workbench/ToolBar";
      
      Icon = ResourceService.GetIcon("Icons.MainApplicationIcon");
    }
    /*
    public new void InitializeWorkspace()
    {
      Menu = null;

      ActiveWorkbenchWindowChanged += new EventHandler(EhAltaxoFireContentChanged);
      
      ActiveWorkbenchWindowChanged += new EventHandler(UpdateMenu);
      
      MenuComplete += new EventHandler(SetStandardStatusBar);
      SetStandardStatusBar(null, null);
      
     
      CreateMainMenu();
      CreateToolBars();
    }
    */

    void UpdateMenu(object sender, EventArgs e)
    {
      /*
      UpdateMenus();
      UpdateToolbars();
       */
    }

    void SetStandardStatusBar(object sender, EventArgs e)
    {
      
      StatusBarService.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
    }
    /*
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
        CommandBar[] toolBars = ToolBarService.CreateToolbars();
        ToolBars = toolBars;
      }
    }
    */
   
    public void EhProjectChanged(object sender, Altaxo.Main.ProjectEventArgs e)
    {
      UpdateMenu(null, null);
      System.Text.StringBuilder title = new System.Text.StringBuilder();
      title.Append(ResourceService.GetString("MainWindow.DialogName"));
      if (Altaxo.Current.ProjectService != null) 
      {
        if (Altaxo.Current.ProjectService.CurrentProjectFileName == null)
        {
          title.Append(" - ");
          title.Append(ResourceService.GetString("Altaxo.Project.UntitledName"));
        }
        else
        {
          title.Append(" - ");
          title.Append(Altaxo.Current.ProjectService.CurrentProjectFileName);
        }
        if (Altaxo.Current.ProjectService.CurrentOpenProject != null && Altaxo.Current.ProjectService.CurrentOpenProject.IsDirty)
          title.Append("*");
      } 
      this.Title = title.ToString();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      Altaxo.Main.IProjectService projectService = Altaxo.Current.ProjectService;
      
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

    ICollection Altaxo.Gui.Common.IWorkbench.ViewContentCollection
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

    object Altaxo.Gui.Common.IWorkbench.ActiveViewContent
    {
      get
      {
        return null!=this.ActiveWorkbenchWindow ? this.ActiveWorkbenchWindow.ActiveViewContent : null;
      }
    }

    void Altaxo.Gui.Common.IWorkbench.ShowView(object o)
    {
      base.ShowView((IViewContent)o);
    }

    void Altaxo.Gui.Common.IWorkbench.CloseContent(object o)
    {
      base.CloseContent((IViewContent)o);
    }

    void Altaxo.Gui.Common.IWorkbench.CloseAllViews()
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
