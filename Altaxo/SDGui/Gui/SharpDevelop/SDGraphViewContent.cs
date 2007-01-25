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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.GUI;
using Altaxo.Serialization;
using ICSharpCode.SharpDevelop.Gui;


namespace Altaxo.Gui.SharpDevelop
{
#if true


  public class SDGraphViewContent : AbstractViewContent, Altaxo.Gui.IMVCControllerWrapper, IEditable, IClipboardHandler
  {
    Altaxo.Graph.GUI.GraphController _controller;


    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui","Altaxo.Graph.GUI.SDGraphController",0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException("Serialization of old versions is not supported");
//        info.AddBaseValueEmbedded(obj,typeof(SDGraphController).BaseType);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        GraphController s = new GraphController(null,true);
        info.GetBaseValueEmbedded(s,typeof(GraphController),parent);

        return new SDGraphViewContent(s);
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SDGraphViewContent), 1)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SDGraphViewContent s = (SDGraphViewContent)obj;
        info.AddValue("Controller", s._controller);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        GraphController wc = (GraphController)info.GetValue("Controller", parent);
        SDGraphViewContent s = null != o ? (SDGraphViewContent)o : new SDGraphViewContent(wc);
        s._controller = wc;
        return s;
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.    
    /// </summary>
    /// <param name="graphdoc">The graph which holds the graphical elements.</param>
    public SDGraphViewContent(GraphDocument graphdoc)
      : this(graphdoc,false)
    {
    }

    /// <summary>
    /// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.
    /// </summary>
    /// <param name="graphdoc">The graph which holds the graphical elements.</param>
    /// <param name="bDeserializationConstructor">If true, this is a special constructor used only for deserialization, where no graphdoc needs to be supplied.</param>
    protected SDGraphViewContent(GraphDocument graphdoc, bool bDeserializationConstructor)
      : this(new GraphController(graphdoc))
    {
    }

    protected SDGraphViewContent(GraphController ctrl)
    {
      _controller = ctrl;
      _controller.TitleNameChanged += EhTitleNameChanged;
    }

    #endregion

    public static implicit operator Altaxo.Graph.GUI.GraphController(SDGraphViewContent ctrl)
    {
      return ctrl._controller;
    }

    public Altaxo.Graph.GUI.GraphController Controller
    {
      get { return _controller; }
    }
   
    public Altaxo.Gui.IMVCController MVCController 
      {
      get { return _controller; }
    }

    #region Abstract View Content overrides
    #region Required
    public override Control Control
    {
      get { return (Control)_controller.ViewObject; }
    }
    #endregion


    #region Optional

    /// <summary>
    /// A generic name for the file, when it does have no file name
    /// (e.g. newly created files)
    /// </summary>
    public override string UntitledName
    {
      get { return "UntitledGraph"; }
      set { }
    }

    /// <summary>
    /// This is the whole name of the content, e.g. the file name or
    /// the url depending on the type of the content.
    /// </summary>
    /// <returns>
    /// Title Name, if not set it returns UntitledName
    /// </returns>
    public override string TitleName
    {
      get
      {
        return _controller.Doc.Name;
      }
      set
      {
      }
    }
    void EhTitleNameChanged(object sender, EventArgs e)
    {
      base.OnTitleNameChanged(e);
    }

    /// <summary>
    /// Returns the file name (if any) assigned to this view.
    /// </summary>
    public override string FileName
    {
      get
      {
        return _controller.Doc.Name;
      }
      set
      {
      }
    }

    /// <summary>
    /// The text on the tab page when more than one view content
    /// is attached to a single window.
    /// </summary>
    public override string TabPageText
    {
      get { return TitleName; }
    }

    #endregion

    #endregion

    #region IEditable Members

    public string Text
    {
      get
      {
        return null;
      }
      set
      {
      }
    }

    #endregion

    #region IClipboardHandler Members

    public bool EnableCut
    {
      get { return true; }
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

    public void Cut()
    {
      _controller.CutSelectedObjectsToClipboard();
    }

    public void Copy()
    {
      _controller.CopySelectedObjectsToClipboard();
    }

    public void Paste()
    {
      _controller.PasteObjectsFromClipboard();
    }

    public void Delete()
    {
      if (_controller.NumberOfSelectedObjects > 0)
      {
        _controller.RemoveSelectedObjects();
      }
      else
      {
        // nothing is selected, we assume that the user wants to delete the worksheet itself
        Current.ProjectService.DeleteGraphDocument(_controller.Doc, false);
      }
    }

    public void SelectAll()
    {
    }

    #endregion
  }

#else

  /// <summary>
  /// Summary description for SDGraphControl.
  /// </summary>
  public class SDGraphController : 
    Altaxo.Graph.GUI.GraphController,
    ICSharpCode.SharpDevelop.Gui.IViewContent,
    ICSharpCode.SharpDevelop.Gui.IEditable,
    ICSharpCode.SharpDevelop.Gui.IClipboardHandler
  {
    protected ICSharpCode.SharpDevelop.Gui.IWorkbenchWindow workbenchWindow;
    public event EventHandler WorkbenchWindowChanged;

    protected virtual void OnWorkbenchWindowChanged(EventArgs e)
    {
      if (WorkbenchWindowChanged != null)
      {
        WorkbenchWindowChanged(this, e);
      }
    }

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



    public Main.GUI.IWorkbenchWindowController ParentWorkbenchWindowController 
    { 
      get { return workbenchWindow as Main.GUI.IWorkbenchWindowController; }
      set 
      {
        if(null!=workbenchWindow)
        {
          workbenchWindow.WindowDeselected -= new EventHandler(EhParentWindowDeselected);
          workbenchWindow.WindowSelected -= new EventHandler(EhParentWindowSelected);
        }
          
        workbenchWindow = value;

        if(null!=workbenchWindow)
        {
          workbenchWindow.WindowDeselected += new EventHandler(EhParentWindowDeselected);
          workbenchWindow.WindowSelected += new EventHandler(EhParentWindowSelected);
        }
      }
    }

    #region IBaseViewContent implementation (copied from AbstractBaseViewContent)
    public virtual Control Control
    {
      get { return this.View as System.Windows.Forms.Control; }
    }

    public virtual IWorkbenchWindow WorkbenchWindow
    {
      get
      {
        return workbenchWindow;
      }
      set
      {
        workbenchWindow = value;
        OnWorkbenchWindowChanged(EventArgs.Empty);
      }
    }
   

   
    #region IDisposable implementation
    public virtual void Dispose()
    {
      workbenchWindow = null;
    }
    #endregion

    #endregion		

    #region ICSharpCode.SharpDevelop.Gui

    
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
    /// <returns>
    /// Title Name, if not set it returns UntitledName
    /// </returns>
    public string TitleName 
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
    /// Returns the file name (if any) assigned to this view.
    /// </summary>
    public string FileName 
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
      get { return TitleName; }
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
    /// Is called when the view content is selected inside the window
    /// tab. NOT when the windows is selected.
    /// </summary>
    public void Selected()
    {
    }


    /// <summary>
    /// Is called when the window is switched to.
    /// -> Inside the tab (Called before Selected())
    /// -> Inside the workbench.
    /// </summary>
    public void SwitchedTo()
    {
    }
    
    /// <summary>
    /// Is called when the view content is deselected inside the window
    /// tab before the other window is selected. NOT when the windows is deselected.
    /// </summary>
    public void Deselected()
    {
    }

    public virtual void Deselecting()
    {
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
    /// If this property is true, content will be created in the tab page
    /// </summary>
    public bool CreateAsSubViewContent 
    {
      get { return false; }
    }
  
    

  

    /// <summary>
    /// Is called when the content is changed after a save/load operation
    /// and this signals that changes could be saved.
    /// </summary>
    public event EventHandler DirtyChanged;

    public event EventHandler BeforeSave;

    public event EventHandler     Saving;

    public event ICSharpCode.SharpDevelop.Gui.SaveEventHandler Saved;

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
    
    public bool EnableUndo 
    {
      get { return false; }
    }
    
    public bool EnableRedo 
    {
      get{ return false; }
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
      get { return true; }
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
    
    public void Cut()
    {
      this.CutSelectedObjectsToClipboard();
    }
    public void Copy()
    {
      this.CopySelectedObjectsToClipboard();
    }
    public void Paste()
    {
      this.PasteObjectsFromClipboard();
    }
    public void Delete()
    {
      if(this.NumberOfSelectedObjects>0)
      {
        this.RemoveSelectedObjects();
      }
      else
      {
        // nothing is selected, we assume that the user wants to delete the worksheet itself
        Current.ProjectService.DeleteGraphDocument(this.Doc,false);
      }
    }
    public void SelectAll()
    {
    }
    #endregion

    #region IViewContent Members


    public System.Collections.Generic.List<ISecondaryViewContent> SecondaryViewContents
    {
      get 
      {
        return new List<ICSharpCode.SharpDevelop.Gui.ISecondaryViewContent>();
      }
    }

    /// <summary>
    /// Builds an <see cref="INavigationPoint"/> for the current position.
    /// </summary>
    public ICSharpCode.SharpDevelop.INavigationPoint BuildNavPoint()
    {
      return null;
    }

    public event EventHandler FileNameChanged;

    #endregion
  }
#endif
}
