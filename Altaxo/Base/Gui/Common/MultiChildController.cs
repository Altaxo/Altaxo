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
  
  public interface IMultiChildViewEventSink
  {
    
  }
  public interface IMultiChildView
  {
    IMultiChildViewEventSink Controller { set; }

    void InitializeBegin();
    void InitializeEnd();

    void InitializeLayout(bool horizontalLayout);

    void InitializeDescription(string value);
    void InitializeChilds(ViewDescriptionElement[] childs, int initialFocusedChild);

    /// <summary>Event fired when one of the child controls is leaved.</summary>
    event EventHandler ChildControlEntered;
    /// <summary>Event fired when one of the child controls is leaved.</summary>
    event EventHandler ChildControlValidated;
  }
  public interface IMultiChildController : IMVCAController
  {
    void Initialize(IMVCAController[] childs, bool horizontalLayout);
    string DescriptionText { get; set; }
    
    /// <summary>Event fired when one of the child controls is leaved and another entered.</summary>
    event Main.InstanceChangedEventHandler<object> ChildControlChanged;
  }

  #endregion

  /// <summary>
  /// Controller for a set of child <see cref="IMVCAController" />s.
  /// </summary>
  [UserControllerForObject(typeof(IMVCAController[]))]
  [ExpectedTypeOfView(typeof(IMultiChildView))]
  public class MultiChildController : IMultiChildController, IMultiChildViewEventSink, IRefreshable
  {
    protected IMultiChildView _view;
    protected ControlViewElement[] _childController;
    protected bool _horizontalLayout;
    /// <summary>Event fired when one of the child controls is leaved.</summary>
    public event Main.InstanceChangedEventHandler<object> ChildControlChanged;


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
      Initialize(childs,false);
    }
    protected MultiChildController()
    {
    }
    public void Initialize(IMVCAController[] childs, bool horizontalLayout)
    {
      _childController = new ControlViewElement[childs.Length];
      for (int i = 0; i < childs.Length; i++)
      {
        _childController[i] = new ControlViewElement(string.Empty,childs[i]);
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
      if(null!=_view)
      {
        _view.InitializeBegin();

        ViewDescriptionElement[] controls = new ViewDescriptionElement[_childController.Length];
        for(int i=0;i<controls.Length;i++)
          controls[i] = new ViewDescriptionElement( _childController[i].Title,_childController[i].View);

        _view.InitializeLayout(_horizontalLayout);
        _view.InitializeDescription(_descriptionText);
        _view.InitializeChilds(controls,0);

        _view.InitializeEnd();
      }
    }

    public void Refresh()
    {
      for(int i=0;i<_childController.Length;++i)
      {
        if(_childController[i] is IRefreshable)
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
        if(null!=_view)
        {
          _view.InitializeDescription(_descriptionText);
        }
      }
    }

    object _lastActiveChild;
    protected virtual void EhView_ChildControlEntered(object sender, EventArgs e)
    {
      if (ChildControlChanged != null)
        ChildControlChanged(sender, new Main.InstanceChangedEventArgs<object>(_lastActiveChild,sender));
      _lastActiveChild = sender;
    }
    protected virtual void EhView_ChildControlValidated(object sender, EventArgs e)
    {
      if (ChildControlChanged != null)
        ChildControlChanged(sender, new Main.InstanceChangedEventArgs<object>(sender, null));
      _lastActiveChild = null; // because now this was the last message from the child control
    }

    #region IMVCController Members

    public virtual object ViewObject
    {
      get
      {
        
        return _view;
      }
      set
      {
        if (_view != null)
        {
          _view.Controller = null;
          _view.ChildControlEntered -= this.EhView_ChildControlEntered;
          _view.ChildControlValidated -= this.EhView_ChildControlValidated;
        }

        _view = value as IMultiChildView;
        
        Initialize();

        if (_view != null)
        {
          _view.Controller = this;
          _view.ChildControlEntered += this.EhView_ChildControlEntered;
          _view.ChildControlValidated += this.EhView_ChildControlValidated;
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

    #endregion

    #region IApplyController Members

    public virtual bool Apply()
    {
      for(int i=0;i<_childController.Length;i++)
      {
        if(null!= _childController[i].Controller && false== _childController[i].Controller.Apply())
        {
          return false;
        }
      }

      return true;
    }

    #endregion
  
  }
}
