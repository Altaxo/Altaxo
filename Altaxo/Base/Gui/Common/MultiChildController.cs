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
using Altaxo.Main.GUI;

namespace Altaxo.Gui.Common
{

  #region Interfaces
  
  public interface IMultiChildViewEventSink
  {
    
  }
  public interface IMultiChildView
  {
    IMultiChildViewEventSink Controller { set; }

    void InitializeLayout(bool horizontalLayout);

    void InitializeDescription(string value);
    void InitializeChilds(object[] childs, int initialFocusedChild);
  }
  public interface IMultiChildController : IMVCAController
  {
    void Initialize(IMVCAController[] childs, bool horizontalLayout);
    string DescriptionText { get; set; }
  }

  #endregion

  /// <summary>
  /// Controller for a set of child <see cref="IMVCAController" />s.
  /// </summary>
  [UserControllerForObject(typeof(IMVCAController[]))]
  public class MultiChildController : IMultiChildController, IMultiChildViewEventSink, IRefreshable
  {
    protected IMultiChildView _view;
    protected ControlViewElement[] _childController;
    protected bool _horizontalLayout;

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
        object[] controls = new object[_childController.Length];
        for(int i=0;i<controls.Length;i++)
          controls[i] = _childController[i].View;

        _view.InitializeLayout(_horizontalLayout);
        _view.InitializeDescription(_descriptionText);
        _view.InitializeChilds(controls,0);
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
    #region IMVCController Members

    public virtual object ViewObject
    {
      get
      {
        
        return _view;
      }
      set
      {
        if(_view!=null)
          _view.Controller = null;

        _view = value as IMultiChildView;
        
        Initialize();

        if(_view!=null)
          _view.Controller = this;
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
