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
using Altaxo.Main;

namespace Altaxo.Gui.Common
{
  #region Interfaces

  /// <summary>
  /// Defines the view contract for displaying multiple child controllers.
  /// </summary>
  public interface IMultiChildView
  {
    /// <summary>
    /// Begins the initialization sequence.
    /// </summary>
    void InitializeBegin();

    /// <summary>
    /// Ends the initialization sequence.
    /// </summary>
    void InitializeEnd();

    /// <summary>
    /// Initializes the layout direction.
    /// </summary>
    /// <param name="horizontalLayout"><see langword="true"/> for a horizontal layout; otherwise, <see langword="false"/>.</param>
    void InitializeLayout(bool horizontalLayout);

    /// <summary>
    /// Initializes the description text.
    /// </summary>
    /// <param name="value">The description text.</param>
    void InitializeDescription(string value);

    /// <summary>
    /// Initializes the child views.
    /// </summary>
    /// <param name="childs">The child view descriptions.</param>
    /// <param name="initialFocusedChild">The index of the child that should initially receive focus.</param>
    void InitializeChilds(ViewDescriptionElement[] childs, int initialFocusedChild);

    /// <summary>Event fired when one of the child controls is leaved.</summary>
    event EventHandler? ChildControlEntered;

    /// <summary>Event fired when one of the child controls is leaved.</summary>
    event EventHandler? ChildControlValidated;
  }

  /// <summary>
  /// Defines the controller contract for multiple child controllers.
  /// </summary>
  public interface IMultiChildController : IMVCAController
  {
    /// <summary>
    /// Initializes the controller with child controllers.
    /// </summary>
    /// <param name="childs">The child controllers managed by this controller.</param>
    /// <param name="horizontalLayout">If set to <c>true</c>, the child controls are arranged horizontally.</param>
    void Initialize(IMVCAController[] childs, bool horizontalLayout);

    /// <summary>
    /// Gets or sets the description text.
    /// </summary>
    string DescriptionText { get; set; }

    /// <summary>Event fired when one of the child controls is leaved and another entered.</summary>
    event EventHandler<InstanceChangedEventArgs> ChildControlChanged;
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for a set of child <see cref="IMVCAController" />s.
  /// </summary>
  [UserControllerForObject(typeof(IMVCAController[]))]
  [ExpectedTypeOfView(typeof(IMultiChildView))]
  public class MultiChildController : IMultiChildController, IRefreshable
  {
    /// <summary>
    /// Stores the attached view.
    /// </summary>
    protected IMultiChildView? _view;

    /// <summary>
    /// Stores the child controllers together with their associated views.
    /// </summary>
    protected ControlViewElement[] _childController = new ControlViewElement[0];

    /// <summary>
    /// Indicates whether the child controls should be arranged horizontally.
    /// </summary>
    protected bool _horizontalLayout;

    /// <summary>Event fired when one of the child controls is leaved.</summary>
    public event EventHandler<InstanceChangedEventArgs>? ChildControlChanged;

    private object? _lastActiveChild;

    /// <summary>
    /// Stores the description text shown by the view.
    /// </summary>
    protected string _descriptionText = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiChildController"/> class.
    /// </summary>
    /// <param name="childs">The child view elements.</param>
    /// <param name="horizontalLayout"><see langword="true"/> to use a horizontal layout; otherwise, <see langword="false"/>.</param>
    public MultiChildController(ControlViewElement[] childs, bool horizontalLayout)
    {
      Initialize(childs, horizontalLayout);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiChildController"/> class.
    /// </summary>
    /// <param name="childs">The child controllers.</param>
    /// <param name="horizontalLayout"><see langword="true"/> to use a horizontal layout; otherwise, <see langword="false"/>.</param>
    public MultiChildController(IMVCAController[] childs, bool horizontalLayout)
    {
      Initialize(childs, horizontalLayout);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiChildController"/> class.
    /// </summary>
    /// <param name="childs">The child controllers.</param>
    public MultiChildController(IMVCAController[] childs)
    {
      Initialize(childs, false);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiChildController"/> class.
    /// </summary>
    protected MultiChildController()
    {
    }

    /// <inheritdoc/>
    public void Initialize(IMVCAController[] childs, bool horizontalLayout)
    {
      _childController = new ControlViewElement[childs.Length];
      for (int i = 0; i < childs.Length; i++)
      {
        _childController[i] = new ControlViewElement(string.Empty, childs[i]);
      }
      _horizontalLayout = horizontalLayout;
      Initialize();
    }

    /// <summary>
    /// Initializes the controller with described child views.
    /// </summary>
    /// <param name="childs">The child view elements.</param>
    /// <param name="horizontalLayout"><see langword="true"/> to use a horizontal layout; otherwise, <see langword="false"/>.</param>
    public void Initialize(ControlViewElement[] childs, bool horizontalLayout)
    {
      _childController = new ControlViewElement[childs.Length];
      for (int i = 0; i < childs.Length; i++)
      {
        _childController[i] = childs[i].Clone();
      }
      _horizontalLayout = horizontalLayout;
      Initialize();
    }

    /// <summary>
    /// Initializes the attached view with the current child-controller configuration.
    /// </summary>
    protected virtual void Initialize()
    {
      if (_view is not null)
      {
        _view.InitializeBegin();

        var controls = new ViewDescriptionElement[_childController.Length];
        for (int i = 0; i < controls.Length; i++)
          controls[i] = new ViewDescriptionElement(_childController[i].Title, _childController[i].View);

        _view.InitializeLayout(_horizontalLayout);
        _view.InitializeDescription(_descriptionText);
        _view.InitializeChilds(controls, 0);

        _view.InitializeEnd();
      }
    }

    /// <summary>
    /// Refreshes all refreshable child controllers.
    /// </summary>
    public void Refresh()
    {
      for (int i = 0; i < _childController.Length; ++i)
      {
        if (_childController[i] is IRefreshable)
          ((IRefreshable)_childController[i]).Refresh();
      }
    }

    /// <inheritdoc/>
    public string DescriptionText
    {
      get
      {
        return _descriptionText;
      }
      set
      {
        _descriptionText = value;
        if (_view is not null)
        {
          _view.InitializeDescription(_descriptionText);
        }
      }
    }


    /// <summary>
    /// Handles activation changes between child controls.
    /// </summary>
    /// <param name="sender">The child control that became active.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void EhView_ChildControlEntered(object? sender, EventArgs e)
    {
      ChildControlChanged?.Invoke(sender, new InstanceChangedEventArgs(_lastActiveChild, sender));
      _lastActiveChild = sender;
    }

    /// <summary>
    /// Handles validation of the current child control.
    /// </summary>
    /// <param name="sender">The child control that was validated.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void EhView_ChildControlValidated(object? sender, EventArgs e)
    {
      ChildControlChanged?.Invoke(sender, new InstanceChangedEventArgs(sender, null));
      _lastActiveChild = null; // because now this was the last message from the child control
    }

    #region IMVCController Members

    /// <inheritdoc/>
    public virtual object? ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view is not null)
        {
          _view.ChildControlEntered -= EhView_ChildControlEntered;
          _view.ChildControlValidated -= EhView_ChildControlValidated;
        }

        _view = value as IMultiChildView;

        Initialize();

        if (_view is not null)
        {
          _view.ChildControlEntered += EhView_ChildControlEntered;
          _view.ChildControlValidated += EhView_ChildControlValidated;
        }
      }
    }

    /// <inheritdoc/>
    public virtual object ModelObject
    {
      get
      {
        return _childController;
      }
    }

    /// <inheritdoc/>
    /// <inheritdoc/>
    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IApplyController Members

    /// <inheritdoc/>
    public virtual bool Apply(bool disposeController)
    {
      for (int i = 0; i < _childController.Length; i++)
      {
        if (_childController[i].Controller is not null && false == _childController[i].Controller.Apply(disposeController))
        {
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
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    /// <inheritdoc/>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IApplyController Members
  }
}
