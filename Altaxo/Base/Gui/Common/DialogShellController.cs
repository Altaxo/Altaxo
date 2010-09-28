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
    /// Sets if the Apply button should be visible.
    /// </summary>
    bool ApplyVisible { set; }

    /// <summary>
    /// Sets the title
    /// </summary>
    string Title { set; }


    event Action<System.ComponentModel.CancelEventArgs> ButtonOKPressed;
    event Action ButtonCancelPressed;
    event Action ButtonApplyPressed;

  }

 
  #endregion

  /// <summary>
  /// Controls the <see cref="DialogShellView"/>.
  /// </summary>
  public class DialogShellController 
  {
    private IDialogShellView _view;
    private IApplyController _hostedController;

    private string _title = String.Empty;
    private bool   _isApplyVisible = true;

    /// <summary>
    /// Creates the controller.
    /// </summary>
    /// <param name="view">The view this controller is controlling.</param>
    /// <param name="hostedController">The controller that controls the UserControl shown in the client area of the form.</param>
    public DialogShellController(IDialogShellView view, IApplyController hostedController)
    {
      View = view;
      _hostedController = hostedController;
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
      _hostedController = hostedController;
      _title = title;
      _isApplyVisible = applyvisible;

      SetElements(true);
    }
    /// <summary>
    /// Get / sets the view of this controller.
    /// </summary>
    IDialogShellView View
    {
      get { return _view; }
      set
      {
        if (null != _view)
        {
          _view.ButtonOKPressed -= EhOK;
          _view.ButtonCancelPressed -= EhCancel;
          _view.ButtonApplyPressed -= EhApply;
        }

        _view = value;
        
        if(null!=_view)
        {
          SetElements(false);
          _view.ButtonOKPressed += EhOK;
          _view.ButtonCancelPressed += EhCancel;
          _view.ButtonApplyPressed += EhApply;
        }
      }
    }

    void SetElements(bool bInit)
    {

      if(null!=View)
      {
        View.Title = _title;
        View.ApplyVisible = _isApplyVisible;
      }
    }

    #region IDialogShellController Members

    /// <summary>
    /// Called when the user presses the OK button. Calls the Apply method of the
    /// hosted controller, then closes the form.
    /// </summary>
    public void EhOK(System.ComponentModel.CancelEventArgs e)
    {
      bool bSuccess = true;
      if(null!=_hostedController)
        bSuccess = _hostedController.Apply();

      if (!bSuccess)
        e.Cancel = true;
    }

    /// <summary>
    /// Called when the user presses the Cancel button. Then closes the form.
    /// </summary>
    public void EhCancel()
    {
    }

    /// <summary>
    /// Called when the user presses the Apply button. Calls the Apply method of the
    /// hosted controller.
    /// </summary>
    public void EhApply()
    {
      if(null!=_hostedController)
        _hostedController.Apply();
    }

    #endregion
  }
}
