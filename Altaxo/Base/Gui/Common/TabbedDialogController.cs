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
using Altaxo.Gui;

namespace Altaxo.Gui.Common
{
  #region Interfaces

  /// <summary>
  /// This interface is intended to provide a "shell" as a dialog which can host a couple of user controls in tab pages.
  /// </summary>
  public interface ITabbedDialogView
  {
    /// <summary>
    /// Returns either the view itself if the view is a form, or the form where this view is contained into, if it is a control or so.
    /// </summary>
    System.Windows.Forms.Form Form { get; }

    /// <summary>
    /// Get / sets the controler of this view.
    /// </summary>
    ITabbedDialogController Controller { get; set; }

    /// <summary>
    /// Sets if the Apply button should be visible.
    /// </summary>
    bool ApplyVisible { set; }

    /// <summary>
    /// Sets the title
    /// </summary>
    string Title { set; }

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
    void FocusTab(int index);
  }

  

  /// <summary>
  /// Interface to the TabbedDialogController.
  /// </summary>
  public interface ITabbedDialogController
  {
    /// <summary>
    /// Called when the user presses the OK button. 
    /// </summary>
    void EhOK();
    
    /// <summary>
    /// Called when the user presses the Cancel button.
    /// </summary>
    void EhCancel();
    
    /// <summary>
    /// Called when the user presses the Apply button.
    /// </summary>
    void EhApply();
  }

  #endregion

  /// <summary>
  /// Controls the <see cref="TabbedDialogView"/>.
  /// </summary>
  public class TabbedDialogController : ITabbedDialogController
  {
    private ITabbedDialogView m_View;

    private string m_Title = String.Empty;
    private bool   m_ApplyVisible = true;

    public struct TabEntry
    {
      public string Title;
      public IApplyController Controller;
      public object View;


      public TabEntry(string title, IApplyController controller, object view)
      {
        this.Title = title;
        this.Controller = controller;
        this.View = view;
      }
    }

    private System.Collections.ArrayList m_Tabs = new System.Collections.ArrayList();


    /// <summary>
    /// Creates the controller.
    /// </summary>
    public TabbedDialogController()
    {
      SetElements(true);
    }

    /// <summary>
    /// Creates the controller.
    /// </summary>
    /// <param name="title">Title of the dialog.</param>
    /// <param name="applyvisible">Indicates if the Apply button is visible or not.</param>
    public TabbedDialogController(    
      string title,
      bool   applyvisible)
    {
      m_Title = title;
      m_ApplyVisible = applyvisible;

      SetElements(true);
    }


    public void AddTab(string title, IApplyController controller, object view)
    {
      m_Tabs.Add(new TabEntry(title,controller,view));
    }

    /// <summary>
    /// Get / sets the view of this controller.
    /// </summary>
    public ITabbedDialogView View
    {
      get { return m_View; }
      set
      {
        ITabbedDialogView oldView = m_View;
        m_View = value;

        if(null!=oldView)
        {
          oldView.Controller = null;
        }
        
        if(null!=m_View)
        {
          m_View.Controller = this;
          SetElements(false);
        }
      }
    }

    void SetElements(bool bInit)
    {

      if(null!=View)
      {
        View.Title = m_Title;
        View.ApplyVisible = m_ApplyVisible;


        View.ClearTabs();
        for(int i=0;i<m_Tabs.Count;i++)
        {
          TabEntry tab = (TabEntry)m_Tabs[i];
          View.AddTab(tab.Title,tab.View);
        }
      }     
    }

    /// <summary>
    /// Shows the form as modal dialog.
    /// </summary>
    /// <param name="owner">The owner of this form.</param>
    /// <returns>True onto success (the user presses OK).</returns>
    public bool ShowDialog(System.Windows.Forms.Form owner)
    {
      return System.Windows.Forms.DialogResult.OK==m_View.Form.ShowDialog(owner);
    }


    #region ITabbedDialogController Members

    /// <summary>
    /// Called when the user presses the OK button. Calls the Apply method of the
    /// hosted controller, then closes the form.
    /// </summary>
    public void EhOK()
    {
      bool bSuccess = true;

      for(int i=0;i<m_Tabs.Count;i++)
      {
        TabEntry tab = (TabEntry)m_Tabs[i];
        if(null!=tab.Controller)
          bSuccess = tab.Controller.Apply();
      
        if(!bSuccess) // if not successfull applied, open the tab again
        {
          View.FocusTab(i);
          break;
        }
      }

      if(bSuccess)
      {
        View.Form.DialogResult = System.Windows.Forms.DialogResult.OK;
        View.Form.Close();
      }
    }

    /// <summary>
    /// Called when the user presses the Cancel button. Then closes the form.
    /// </summary>
    public void EhCancel()
    {
      View.Form.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      View.Form.Close();
    }

    /// <summary>
    /// Called when the user presses the Apply button. Calls the Apply method of the
    /// hosted controller.
    /// </summary>
    public void EhApply()
    {
      bool bSuccess = true;
      for(int i=0;i<m_Tabs.Count;i++)
      {
        TabEntry tab = (TabEntry)m_Tabs[i];
        if(null!=tab.Controller)
          bSuccess = tab.Controller.Apply();
      
        if(!bSuccess) // if not successfull applied, open the tab again
        {
          View.FocusTab(i);
          break;
        }
      }
    }

    #endregion
  }
}
