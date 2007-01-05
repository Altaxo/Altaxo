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
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Gui;



namespace Altaxo.Gui.SharpDevelop
{
  /// <summary>
  /// This is the a Workspace with a multiple document interface.
  /// </summary>
  public class AltaxoSDWorkbench : DefaultWorkbench, Altaxo.Gui.Common.IWorkbench
  {

   

    #region "Serialization"
  
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui","ICSharpCode.SharpDevelop.Gui.Workbench1",0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AltaxoSDWorkbench), 1)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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
   
    public void EhProjectChanged(object sender, Altaxo.Main.ProjectEventArgs e)
    {
      // UpdateMenu(null, null); // 2006-11-07 hope this is not needed any longer because of the menu update timer
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


    #region Adapter to the Altaxo.Main.Gui.IWorkbench interface

    ICollection Altaxo.Gui.Common.IWorkbench.ViewContentCollection
    {
      get
      {
        return this.ViewContentCollection;
      }
    }

    object Altaxo.Gui.Common.IWorkbench.ViewObject
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
    event EventHandler Altaxo.Gui.Common.IWorkbench.ActiveWorkbenchWindowChanged
    {
      add 
      {
        base.ActiveWorkbenchWindowChanged += value; 
      }
      remove 
      {
        base.ActiveWorkbenchWindowChanged -= value; 
      }
    }

    #endregion
  }
}
