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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Graph;
using Altaxo.Serialization;
using ICSharpCode.SharpDevelop.Gui;


namespace Altaxo.Graph.GUI
{
  /// <summary>
  /// Summary description for SDGraphControl.
  /// </summary>
  public class SDGraphController : 
    Altaxo.Graph.GUI.GraphController,
    ICSharpCode.SharpDevelop.Gui.IViewContent,
    ICSharpCode.SharpDevelop.Gui.IEditable,
    ICSharpCode.SharpDevelop.Gui.IClipboardHandler
  {
    

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SDGraphController),0)]
      public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj,typeof(SDGraphController).BaseType);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        SDGraphController s = null!=o ? (SDGraphController)o : new SDGraphController(null,true);
        info.GetBaseValueEmbedded(s,typeof(SDGraphController).BaseType,parent);
        return s;
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.    
    /// </summary>
    /// <param name="graphdoc">The graph which holds the graphical elements.</param>
    public SDGraphController(GraphDocument graphdoc)
      : this(graphdoc,false)
    {
    }

    /// <summary>
    /// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.
    /// </summary>
    /// <param name="graphdoc">The graph which holds the graphical elements.</param>
    /// <param name="bDeserializationConstructor">If true, this is a special constructor used only for deserialization, where no graphdoc needs to be supplied.</param>
    protected SDGraphController(GraphDocument graphdoc, bool bDeserializationConstructor)
      : base(graphdoc, bDeserializationConstructor)
    {
    }

    #endregion



    protected ICSharpCode.SharpDevelop.Gui.IWorkbenchWindow m_ParentWorkbenchWindowController;
    public Main.GUI.IWorkbenchWindowController ParentWorkbenchWindowController 
    { 
      get { return m_ParentWorkbenchWindowController as Main.GUI.IWorkbenchWindowController; }
      set 
      {
        if(null!=m_ParentWorkbenchWindowController)
        {
          m_ParentWorkbenchWindowController.WindowDeselected -= new EventHandler(EhParentWindowDeselected);
          m_ParentWorkbenchWindowController.WindowSelected -= new EventHandler(EhParentWindowSelected);
        }
          
        m_ParentWorkbenchWindowController = value;

        if(null!=m_ParentWorkbenchWindowController)
        {
          m_ParentWorkbenchWindowController.WindowDeselected += new EventHandler(EhParentWindowDeselected);
          m_ParentWorkbenchWindowController.WindowSelected += new EventHandler(EhParentWindowSelected);
        }
      }
    }



    #region ICSharpCode.SharpDevelop.Gui

    public void Dispose()
    {
    }

    /// <summary>
    /// This is the Windows.Forms control for the view.
    /// </summary>
    public System.Windows.Forms.Control Control 
    {
      get { return this.View as System.Windows.Forms.Control; }
    }

    /// <summary>
    /// The workbench window in which this view is displayed.
    /// </summary>
    public ICSharpCode.SharpDevelop.Gui.IWorkbenchWindow  WorkbenchWindow 
    {
      get 
      {
        return this.m_ParentWorkbenchWindowController; 
      }
      set
      {
        this.m_ParentWorkbenchWindowController = value; 
      }
    }
    
    /// <summary>
    /// A generic name for the file, when it does have no file name
    /// (e.g. newly created files)
    /// </summary>
    public string UntitledName 
    {
      get { return "UntitledTable"; }
      set {}
    }
    
    /// <summary>
    /// This is the whole name of the content, e.g. the file name or
    /// the url depending on the type of the content.
    /// </summary>
    public string ContentName 
    {
      get 
      { 
        return this.Doc.Name; 
      }
      set 
      {
      }
    }

    /// <summary>
    /// The text on the tab page when more than one view content
    /// is attached to a single window.
    /// </summary>
    public string TabPageText 
    {
      get { return ContentName; }
    }
    
    /// <summary>
    /// If this property returns true the view is untitled.
    /// </summary>
    public bool IsUntitled 
    {
      get { return false; }
    }
    
    /// <summary>
    /// If this property returns true the content has changed since
    /// the last load/save operation.
    /// </summary>
    public bool IsDirty 
    {
      get { return false; }
      set {}
    }
    
    /// <summary>
    /// If this property returns true the content could not be altered.
    /// </summary>
    public bool IsReadOnly 
    {
      get { return false; }
    }
    
    /// <summary>
    /// If this property returns true the content can't be written.
    /// </summary>
    public bool IsViewOnly 
    {
      get { return true; }
    }
    
    /// <summary>
    /// Reinitializes the content. (Re-initializes all add-in tree stuff)
    /// and redraws the content. Call this not directly unless you know
    /// what you do.
    /// </summary>
    public void RedrawContent()
    {
    }
    
    /// <summary>
    /// Saves this content to the last load/save location.
    /// </summary>
    public void Save()
    {
    }

    
    /// <summary>
    /// Saves the content to the location <code>fileName</code>
    /// </summary>
    public void Save(string fileName)
    {
    }
    
    /// <summary>
    /// Loads the content from the location <code>fileName</code>
    /// </summary>
    public void Load(string fileName)
    {
    }
    

    protected virtual void OnBeforeSave(EventArgs e)
    {
      if (BeforeSave != null) 
      {
        BeforeSave(this, e);
      }
    }
    /// <summary>
    /// Is called each time the name for the content has changed.
    /// </summary>
    public event EventHandler ContentNameChanged;
    
    /// <summary>
    /// Is called when the content is changed after a save/load operation
    /// and this signals that changes could be saved.
    /// </summary>
    public event EventHandler DirtyChanged;

    public event EventHandler BeforeSave;
    #endregion

    #region ICSharpCode.SharpDevelop.Gui.IEditable
    
    public IClipboardHandler ClipboardHandler 
    {
      get { return this; }
    }
    
    public string Text 
    {
      get { return null; }
      set {}
    }
    
    public void Undo()
    {
    }
    public void Redo()
    {
    }
    #endregion

    #region ICSharpCode.SharpDevelop.Gui.IClipboardHandler
    
    public bool EnableCut 
    {
      get { return false; }
    }
    public bool EnableCopy 
    {
      get { return true; }
    }
    public bool EnablePaste 
    {
      get { return true; }
    }
    public bool EnableDelete 
    {
      get { return true; }
    }
    public bool EnableSelectAll 
    {
      get { return false; }
    }
    
    public void Cut(object sender, EventArgs e)
    {
    }
    public void Copy(object sender, EventArgs e)
    {
    
    }
    public void Paste(object sender, EventArgs e)
    {
    }
    public void Delete(object sender, EventArgs e)
    {
      if(this.m_SelectedObjects.Count>0)
      {
        this.RemoveSelectedObjects();
      }
      else
      {
        // nothing is selected, we assume that the user wants to delete the worksheet itself
        Current.ProjectService.DeleteGraphDocument(this.Doc,false);
      }
    }
    public void SelectAll(object sender, EventArgs e)
    {
    }
    #endregion


  }
}
