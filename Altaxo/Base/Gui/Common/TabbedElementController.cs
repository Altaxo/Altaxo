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
using Altaxo.Main.GUI;

namespace Altaxo.Gui.Common
{
  #region Interfaces

  /// <summary>
  /// This interface is intended to provide a "shell" as a dialog which can host a couple of user controls in tab pages.
  /// </summary>
  public interface ITabbedElementView
  {
   

    /// <summary>
    /// Get / sets the controler of this view.
    /// </summary>
    ITabbedElementViewEventSink Controller { get; set; }

   
    /// <summary>
    /// Removes all Tab pages from the dialog.
    /// </summary>
    void ClearTabs();

    /// <summary>
    /// Adds a Tab page to the dialog
    /// </summary>
    /// <param name="title">The title of the tab page.</param>
    /// <param name="view">The view (must be currently of type Control.</param>
    void AddTab(string title, object view);

   

    /// <summary>
    /// Activates the tab page with the title <code>title</code>.
    /// </summary>
    /// <param name="index">The index of the tab page to focus.</param>
    void BringTabToFront(int index);
  }

  

  /// <summary>
  /// Interface to the TabbedDialogController.
  /// </summary>
  public interface ITabbedElementViewEventSink
  {
    
  }

  public interface ITabbedElementController 
  {
    void BringTabToFront(int i);

    /// <summary>
    /// Removes a number of tab elements.
    /// </summary>
    /// <param name="firstTab">Index of the first tab to remove.</param>
    /// <param name="count">Number of tabs to remove.</param>
    void RemoveTabRange(int firstTab, int count);
  }


  #endregion

  /// <summary>
  /// Controls the <see cref="TabbedDialogView"/>.
  /// </summary>
  public class TabbedElementController : ITabbedElementViewEventSink, ITabbedElementController
  {
  

    private ITabbedElementView _view;
    private System.Collections.ArrayList _tabs = new System.Collections.ArrayList();


    /// <summary>
    /// Creates the controller.
    /// </summary>
    public TabbedElementController()
    {
      SetElements(true);
    }

    protected int TabCount
    {
      get
      {
        return _tabs.Count;
      }
    }
    protected ControlViewElement Tab(int i)
    {
      return (ControlViewElement)_tabs[i];
    }

    public void BringTabToFront(int i)
    {
      if(_view!=null)
        _view.BringTabToFront(i);
    }

    public void AddTab(string title, IApplyController controller, object view)
    {
      _tabs.Add(new ControlViewElement(title,controller,view));
    }

    public void RemoveTabRange(int firstTab, int count)
    {
      _tabs.RemoveRange(firstTab, count);
      SetElements(false);
    }

    /// <summary>
    /// Get / sets the view of this controller.
    /// </summary>
    public ITabbedElementView View
    {
      get { return _view; }
      set
      {
        if(_view!=null)
          _view.Controller = null;

        _view = value;
        
        SetElements(false);

        if(_view!=null)
          _view.Controller = this;
        
      }
    }
    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        View = value as ITabbedElementView;
      }
    }

    protected void SetElements(bool bInit)
    {

      if(null!=View)
      {
        View.ClearTabs();
        for(int i=0;i<_tabs.Count;i++)
        {
          ControlViewElement tab = (ControlViewElement)_tabs[i];
          View.AddTab(tab.Title,tab.View);
        }
      }     
    }

   


    #region ITabbedDialogController Members

   

    #endregion
  }
}
