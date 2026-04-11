#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Main;

namespace Altaxo.Gui.Common
{
  #region Interfaces

  /// <summary>
  /// This interface is intended to provide a "shell" as a dialog which can host a couple of user controls on tab pages.
  /// </summary>
  public interface ITabbedElementView
  {
    /// <summary>
    /// Removes all Tab pages from the dialog.
    /// </summary>
    void ClearTabs();

    /// <summary>
    /// Adds a tab page to the dialog.
    /// </summary>
    /// <param name="title">The title of the tab page.</param>
    /// <param name="view">The view. It must currently be of type <c>Control</c>.</param>
    void AddTab(string title, object view);

    /// <summary>
    /// Activates the tab page with the title <code>title</code>.
    /// </summary>
    /// <param name="index">The index of the tab page to focus.</param>
    void BringTabToFront(int index);

    /// <summary>
    /// Occurs when the input focus enters one of the child controls of the tabs. The sender
    /// of this event is set to the child control that received the input focus.
    /// </summary>
    event EventHandler ChildControl_Entered;

    /// <summary>
    /// Occurs when the input focus leaves one of the child controls and the control is validated.
    /// The sender
    /// of this event is set to the child control that lost the input focus.
    /// </summary>
    event EventHandler ChildControl_Validated;
  }

  /// <summary>
  /// Interface to the TabbedDialogController.
  /// </summary>
  public interface ITabbedElementViewEventSink
  {
  }

  /// <summary>
  /// Interface implemented by controllers that manage tabbed elements.
  /// </summary>
  public interface ITabbedElementController : IMVCAController
  {
    /// <summary>
    /// Brings the specified tab to the front.
    /// </summary>
    /// <param name="i">The tab index.</param>
    void BringTabToFront(int i);

    /// <summary>
    /// Removes a number of tab elements.
    /// </summary>
    /// <param name="firstTab">Index of the first tab to remove.</param>
    /// <param name="count">Number of tabs to remove.</param>
    void RemoveTabRange(int firstTab, int count);
  }

  #endregion Interfaces

  /// <summary>
  /// Controls the <see cref="ITabbedElementView"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(ITabbedElementView))]
  public class TabbedElementController : ITabbedElementViewEventSink, ITabbedElementController
  {
    /// <summary>
    /// The index of the currently active front tab.
    /// </summary>
    protected int _frontTabIndex = 0;
    private ITabbedElementView? _view;
    private List<ControlViewElement> _tabs = new List<ControlViewElement>();

    /// <summary>
    /// Creates the controller.
    /// </summary>
    public TabbedElementController()
    {
      SetElements(true);
    }

    /// <summary>
    /// Gets the number of tabs.
    /// </summary>
    protected int TabCount
    {
      get
      {
        return _tabs.Count;
      }
    }

    /// <summary>
    /// Gets the tab at the specified index.
    /// </summary>
    /// <param name="i">The tab index.</param>
    /// <returns>The tab view element at the specified index.</returns>
    protected ControlViewElement Tab(int i)
    {
      return _tabs[i];
    }

    /// <inheritdoc/>
    public void BringTabToFront(int i)
    {
      _frontTabIndex = i;
      if (_view is not null)
        _view.BringTabToFront(i);
    }

    /// <summary>
    /// Adds a tab to the controller.
    /// </summary>
    /// <param name="title">The tab title.</param>
    /// <param name="controller">The controller associated with the tab.</param>
    /// <param name="view">The view object hosted in the tab.</param>
    public void AddTab(string title, IApplyController controller, object view)
    {
      _tabs.Add(new ControlViewElement(title, controller, view));
    }

    /// <inheritdoc/>
    public void RemoveTabRange(int firstTab, int count)
    {
      _tabs.RemoveRange(firstTab, count);
      SetElements(false);
    }

    private object? _lastActiveChildControl = null;

    private void EhView_ChildControlEntered(object? sender, EventArgs e)
    {
      EhView_ActiveChildControlChanged(sender, new InstanceChangedEventArgs(_lastActiveChildControl, sender));
      _lastActiveChildControl = sender;
    }

    private void EhView_ChildControlValidated(object? sender, EventArgs e)
    {
      EhView_ActiveChildControlChanged(sender, new InstanceChangedEventArgs(sender, null));
      _lastActiveChildControl = null;
    }

    /// <summary>
    /// Handles a change of the active child control.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void EhView_ActiveChildControlChanged(object? sender, InstanceChangedEventArgs e)
    {
    }

    /// <summary>
    /// Get / sets the view of this controller.
    /// </summary>
    /// <summary>
    /// Gets or sets the view of this controller.
    /// </summary>
    public ITabbedElementView? View
    {
      get { return _view; }
      set
      {
        if (_view is not null)
        {
          _view.ChildControl_Entered -= EhView_ChildControlEntered;
          _view.ChildControl_Validated -= EhView_ChildControlValidated;
        }

        _view = value;

        SetElements(false);

        if (_view is not null)
        {
          _view.ChildControl_Entered += EhView_ChildControlEntered;
          _view.ChildControl_Validated += EhView_ChildControlValidated;
        }
      }
    }

    /// <inheritdoc/>
    public object? ViewObject
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

    /// <summary>
    /// Synchronizes the tabs with the view.
    /// </summary>
    /// <param name="bInit">A value indicating whether the synchronization happens during initialization.</param>
    protected void SetElements(bool bInit)
    {
      if (View is not null)
      {
        View.ClearTabs();
        for (int i = 0; i < _tabs.Count; i++)
        {
          var tab = _tabs[i];
          View.AddTab(tab.Title, tab.View);
        }

        _frontTabIndex = Math.Min(_frontTabIndex, _tabs.Count - 1);
        View.BringTabToFront(_frontTabIndex);
      }
    }

    #region IMVCController Members

    /// <inheritdoc/>
    public virtual object ModelObject
    {
      get
      {
        throw new Exception("The method or operation must be overriden in a derived class");
      }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IApplyController Members

    /// <inheritdoc/>
    public virtual bool Apply(bool disposeController)
    {
      for (int i = 0; i < _tabs.Count; i++)
      {
        if (!_tabs[i].Controller.Apply(disposeController))
        {
          BringTabToFront(i);
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successful; <c>false</c> if the revert operation was not possible, that is, because the controller has not stored the original state of the model.
    /// </returns>
    /// <inheritdoc/>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IApplyController Members
  }
}
