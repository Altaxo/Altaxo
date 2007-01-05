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

namespace Altaxo.Gui.Common
{
  #region Interfaces

  /// <summary>
  /// This interface is intended to provide a "shell" as a dialog which can host a user control.
  /// </summary>
  public interface IDialogShellView
  {
    /// <summary>
    /// Returns either the view itself if the view is a form, or the form where this view is contained into, if it is a control or so.
    /// </summary>
    System.Windows.Forms.Form Form { get; }

    /// <summary>
    /// Get / sets the controler of this view.
    /// </summary>
    IDialogShellController Controller { get; set; }

    /// <summary>
    /// Sets if the Apply button should be visible.
    /// </summary>
    bool ApplyVisible { set; }

    /// <summary>
    /// Sets the title
    /// </summary>
    string Title { set; }
  }

  

  /// <summary>
  /// Interface to the DialogShellController.
  /// </summary>
  public interface IDialogShellController
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
  /// Controls the <see cref="DialogShellView"/>.
  /// </summary>
  public class DialogShellController : IDialogShellController
  {
    private IDialogShellView m_View;
    private IApplyController m_HostedController;

    private string m_Title = String.Empty;
    private bool   m_ApplyVisible = true;

    /// <summary>
    /// Creates the controller.
    /// </summary>
    /// <param name="view">The view this controller is controlling.</param>
    /// <param name="hostedController">The controller that controls the UserControl shown in the client area of the form.</param>
    public DialogShellController(IDialogShellView view, IApplyController hostedController)
    {
      View = view;
      m_HostedController = hostedController;
      SetElements(true);
    }

    /// <summary>
    /// Creates the controller.
    /// </summary>
    /// <param name="view">The view this controller is controlling.</param>
    /// <param name="hostedController">The controller that controls the UserControl shown in the client area of the form.</param>
    /// <param name="title">Title of the dialog.</param>
    /// <param name="applyvisible">Indicates if the Apply button is visible or not.</param>
    public DialogShellController(
      IDialogShellView view, 
      IApplyController hostedController,
      string title,
      bool   applyvisible)
    {
      View = view;
      m_HostedController = hostedController;
      m_Title = title;
      m_ApplyVisible = applyvisible;

      SetElements(true);
    }
    /// <summary>
    /// Get / sets the view of this controller.
    /// </summary>
    IDialogShellView View
    {
      get { return m_View; }
      set
      {
        if(null!=m_View)
          m_View.Controller = null;

        m_View = value;
        
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


    #region IDialogShellController Members

    /// <summary>
    /// Called when the user presses the OK button. Calls the Apply method of the
    /// hosted controller, then closes the form.
    /// </summary>
    public void EhOK()
    {
      bool bSuccess = true;
      if(null!=m_HostedController)
        bSuccess = m_HostedController.Apply();

      if(bSuccess) // if successfull applied, close the form
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
      if(null!=m_HostedController)
        bSuccess = m_HostedController.Apply();
    }

    #endregion
  }
}
