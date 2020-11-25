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

  public interface IMultiChildView
  {
    void InitializeBegin();

    void InitializeEnd();

    void InitializeLayout(bool horizontalLayout);

    void InitializeDescription(string value);

    void InitializeChilds(ViewDescriptionElement[] childs, int initialFocusedChild);

    /// <summary>Event fired when one of the child controls is leaved.</summary>
    event EventHandler? ChildControlEntered;

    /// <summary>Event fired when one of the child controls is leaved.</summary>
    event EventHandler? ChildControlValidated;
  }

  public interface IMultiChildController : IMVCAController
  {
    void Initialize(IMVCAController[] childs, bool horizontalLayout);

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
    protected IMultiChildView? _view;
    protected ControlViewElement[] _childController = new ControlViewElement[0];
    protected bool _horizontalLayout;

    /// <summary>Event fired when one of the child controls is leaved.</summary>
    public event EventHandler<InstanceChangedEventArgs>? ChildControlChanged;

    private object? _lastActiveChild;


    protected string _descriptionText = string.Empty;

    public MultiChildController(ControlViewElement[] childs, bool horizontalLayout)
    {
      Initialize(childs, horizontalLayout);
    }

    public MultiChildController(IMVCAController[] childs, bool horizontalLayout)
    {
      Initialize(childs, horizontalLayout);
    }

    public MultiChildController(IMVCAController[] childs)
    {
      Initialize(childs, false);
    }

    protected MultiChildController()
    {
    }

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

    public void Refresh()
    {
      for (int i = 0; i < _childController.Length; ++i)
      {
        if (_childController[i] is IRefreshable)
          ((IRefreshable)_childController[i]).Refresh();
      }
    }

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


    protected virtual void EhView_ChildControlEntered(object? sender, EventArgs e)
    {
      ChildControlChanged?.Invoke(sender, new InstanceChangedEventArgs(_lastActiveChild, sender));
      _lastActiveChild = sender;
    }

    protected virtual void EhView_ChildControlValidated(object? sender, EventArgs e)
    {
      ChildControlChanged?.Invoke(sender, new InstanceChangedEventArgs(sender, null));
      _lastActiveChild = null; // because now this was the last message from the child control
    }

    #region IMVCController Members

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

    public virtual object ModelObject
    {
      get
      {
        return _childController;
      }
    }

    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IApplyController Members

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
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IApplyController Members
  }
}
