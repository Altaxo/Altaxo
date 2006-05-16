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
using System.Collections;

namespace Altaxo.Main.GUI
{


  /// <summary>
  /// This interface has to be implemented by all controllers of content that are shown in the view
  /// area of the main window
  /// </summary>
  public interface IWorkbenchContentController : ICSharpCode.SharpDevelop.Gui.IViewContent  
  {
    /// <summary>
    /// Get / sets the related GUI component (under Windows almost a Control).
    /// </summary>
    IWorkbenchContentView WorkbenchContentView { get; set; }

    /// <summary>
    /// Get / sets the parent window controller.
    /// </summary>
    IWorkbenchWindowController ParentWorkbenchWindowController { get; set; }

    
    /// <summary>
    /// Closes the view and sets the view to null. (But leaves the controller itself intact).
    /// </summary>
    void CloseView();

    /// <summary>
    /// If the controller has no view, this function creates a default view and attaches to it.
    /// </summary>
    void CreateView();

    /// <summary>
    /// Called by the host window when the host window is about to be closed.
    /// </summary>
    /// <returns>True if this closing should be canceled, false otherwise.</returns>
    bool HostWindowClosing();

    /// <summary>
    /// Called by the host window after the host window is closed.
    /// </summary>
    void HostWindowClosed();


  }

  /// <summary>
  /// This interface has to be implemented by all GUI components that show a workbench content.
  /// </summary>
  public interface IWorkbenchContentView
  {
    
  }


  /// <summary>
  /// This interface has to be implemented by all controllers of host windows that are shown
  /// in the view area of the main window.
  /// </summary>
  public interface IWorkbenchWindowController : ICSharpCode.SharpDevelop.Gui.IWorkbenchWindow
  {
    /// <summary>
    /// Get / sets the related GUI component.
    /// </summary>
    Main.GUI.IWorkbenchWindowView View { get; set; }

    /// <summary>
    /// Get / sets the content that is shown.
    /// </summary>
    IWorkbenchContentController Content { get; set; }

    /// <summary>
    /// Closes the related GUI component, but leaves this controller intact.
    /// </summary>
    void CloseView();

    /// <summary>
    /// Called by the view if the view is closed.
    /// </summary>
    void EhView_OnClosed();


    /// <summary>
    /// Called by the view if the view is about to be closed.
    /// </summary>
    /// <param name="cancel">The initial content of CancelEventArgs.</param>
    /// <returns>True if the closing should be canceled, otherwise false.</returns>
    bool EhView_OnClosing(bool cancel);
  }

  /// <summary>
  /// This interface has to be implemented by all GUI host windows that are shown
  /// in the view area of the main window (under Windows almost Forms).
  /// </summary>
  public interface IWorkbenchWindowView
  {
    System.Windows.Forms.Form Form { get; }
    IWorkbenchWindowController Controller { get; set; }
    void SetChild(IWorkbenchContentView content);
    void Close();

    /// <summary>
    /// Sets the title of this window.
    /// </summary>
    /// <param name="title">The title of this window.</param>
    void SetTitle(string title);
  }


  public class WorkbenchWindowController 
    :
    IWorkbenchWindowController,
    ICSharpCode.SharpDevelop.Gui.IWorkbenchWindow 
  {
    protected Main.GUI.IWorkbenchWindowView m_View;
    protected IWorkbenchContentController m_Content;
    /// <summary>
    /// The windows title.
    /// </summary>
    protected string m_Title;

    public Main.GUI.IWorkbenchWindowView View
    {
      get { return m_View; }
      set
      {
        if(m_View!=null)
          m_View.Controller=null;

        m_View = value;

        if(m_View!=null)
          m_View.Controller = this;
      }
    }

    public IWorkbenchContentController Content
    {
      get { return m_Content; }
      set 
      {
        IWorkbenchContentController oldContent = m_Content;
        m_Content = value;

        if(oldContent != null)
        {
          oldContent.ParentWorkbenchWindowController = null;
        }

        if(m_Content!=null)
        {
          m_Content.ParentWorkbenchWindowController = this;
          this.Title = m_Content.TitleName;
        
          if(this.View!=null)
            View.SetChild(m_Content.WorkbenchContentView);
        }
      }
    }

    public void RedrawContent()
    {
      /*
      if (viewTabControl != null) 
      {
        for (int i = 0; i < viewTabControl.TabPages.Count; ++i) 
        {
          TabPage tabPage = viewTabControl.TabPages[i];
          if (i == 0) 
          {
            tabPage.Text = stringParserService.Parse(content.TabPageText);
          } 
          else 
          {
            tabPage.Text = stringParserService.Parse(((IBaseViewContent)subViewContents[i]).TabPageText);
          }
        }
      }
      */
    }

    public void OnWindowSelected(EventArgs e)
    {
    }

    public void CloseView()
    {
      if(null!=m_Content)
        m_Content.CloseView();

      if(View!=null)
        View.Close();
      
      this.View=null;
    }

    public void EhView_OnClosed()
    {
      this.View = null;

      // before the childs get the close event,
      // we sent out the message that the window is deselected
      if(null!=WindowDeselected)
        WindowDeselected(null,null);

      if(this.m_Content!=null)
        m_Content.HostWindowClosed();
    }

    public bool EhView_OnClosing(bool cancel)
    {
      bool bCancelContent=false;

      if(this.m_Content!=null)
        bCancelContent = this.m_Content.HostWindowClosing();
      
      return cancel | bCancelContent;
    }

    #region ICSharpCode.SharpDevelop.Gui

    /// <summary>
    /// The window title.
    /// </summary>
    public string Title 
    {
      get { return m_Title; }
      set 
      {
        string oldTitle = m_Title;

        m_Title = value;
        if(View!=null)
          View.SetTitle(m_Title);

        if(oldTitle!=m_Title && null!=this.TitleChanged)
          TitleChanged(this,EventArgs.Empty);
      }
    }
    
    /// <summary>
    /// The current view content which is shown inside this window.
    /// </summary>
    public ICSharpCode.SharpDevelop.Gui.IViewContent ViewContent 
    {
      get { return this.Content as ICSharpCode.SharpDevelop.Gui.IViewContent; }
    }
    

    public ArrayList SubViewContents 
    {
      get 
      {
        throw new NotImplementedException("SubViewContent is not implemented now");
      
        /*
        return subViewContents;
        */
      }
    }
    
    public ICSharpCode.SharpDevelop.Gui.IBaseViewContent ActiveViewContent 
    {
      get 
      {
        return this.m_Content;

        /*
        if (viewTabControl != null && viewTabControl.SelectedIndex > 0) 
        {
          return (IBaseViewContent)subViewContents[viewTabControl.SelectedIndex - 1];
        }
        return content;
        
        */
      }
    }

    public void SwitchView(int viewNumber)
    {

      throw new NotImplementedException();

      /*
      if (viewTabControl != null) 
      {
        this.viewTabControl.SelectedIndex = viewNumber;
      }
      */

      
    }

    public void AttachSecondaryViewContent(ICSharpCode.SharpDevelop.Gui.ISecondaryViewContent subViewContent)
    {
      throw new NotImplementedException();
      /*
      TabPage newPage;
      
      if (subViewContents == null) 
      {
        subViewContents = new ArrayList();
        
        viewTabControl      = new TabControl();
        viewTabControl.Alignment = TabAlignment.Bottom;
        viewTabControl.Dock = DockStyle.Fill;
        viewTabControl.SelectedIndexChanged += new EventHandler(viewTabControlIndexChanged);
        
        tabPage.Controls.Clear();
        tabPage.Controls.Add(viewTabControl);
        
        newPage = new TabPage(stringParserService.Parse(content.TabPageText));
        newPage.Tag = content;
        content.Control.Dock = DockStyle.Fill;
        newPage.Controls.Add(content.Control);
        viewTabControl.TabPages.Add(newPage);
      }
      subViewContent.WorkbenchWindow = this;
      subViewContents.Add(subViewContent);
      
      newPage = new TabPage(stringParserService.Parse(subViewContent.TabPageText));
      newPage.Tag = subViewContent;
      subViewContent.Control.Dock = DockStyle.Fill;
      newPage.Controls.Add(subViewContent.Control);
      viewTabControl.TabPages.Add(newPage);
    }
    
    int oldIndex = -1;
    void viewTabControlIndexChanged(object sender, EventArgs e)
    {
      if (oldIndex > 0) 
      {
        ISecondaryViewContent secondaryViewContent = subViewContents[oldIndex - 1] as ISecondaryViewContent;
        if (secondaryViewContent != null) 
        {
          secondaryViewContent.Deselected();
        }
      }
      
      if (viewTabControl.SelectedIndex > 0) 
      {
        ISecondaryViewContent secondaryViewContent = subViewContents[viewTabControl.SelectedIndex - 1] as ISecondaryViewContent;
        if (secondaryViewContent != null) 
        {
          secondaryViewContent.Selected();
        }
      }
      oldIndex = viewTabControl.SelectedIndex;
      */

    }

    /// <summary>
    /// Closes the window, if force == true it closes the window
    /// without ask, even the content is dirty.
    /// </summary>
    /// <returns>true, if window is closed</returns>
    public bool CloseWindow(bool force)
    {
      if(force)
      {
        CloseView();
        return true;
      }
      else
      {
        if(View!=null)
          View.Close();
        return true;
      }
    }
    
    /// <summary>
    /// Brings this window to front and sets the user focus to this
    /// window.
    /// </summary>
    public void SelectWindow()
    {
      
      if(this.WindowSelected!=null)
        WindowSelected(null,null);
    }

    
    //    void OnWindowSelected(EventArgs e);
    public void OnWindowDeselected(EventArgs e)
    {
      if(null!=WindowDeselected)
        WindowDeselected(null,e);
    }
    
    /// <summary>
    /// Is called when the window is selected.
    /// </summary>
    public event EventHandler WindowSelected;
    
    /// <summary>
    /// Is called when the window is deselected.
    /// </summary>
    public event EventHandler WindowDeselected;
    
    /// <summary>
    /// Is called when the title of this window has changed.
    /// </summary>
    public event EventHandler TitleChanged;
    
    /// <summary>
    /// Is called after the window closes.
    /// </summary>
    public event EventHandler CloseEvent;


    #endregion


    #region IWorkbenchWindow Members


    public bool IsDisposed
    {
      get { return this.View==null; }
    }

    #endregion
  }

}
