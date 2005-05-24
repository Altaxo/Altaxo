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

namespace Altaxo.Main.GUI
{

  #region Interfaces
  
  public interface IMultiChildViewEventSink
  {
    
  }
  public interface IMultiChildView
  {
    IMultiChildViewEventSink Controller { set; }

    void InitializeDescription(string value);
    void InitializeChilds(object[] childs, int initialFocusedChild);
  }
 

  #endregion

  /// <summary>
  /// Controller for a single value. This is a string here, but in derived classes, that can be anything that can be converted to and from a string.
  /// </summary>
  public class MultiChildController : IMVCAController, IMultiChildViewEventSink
  {
    protected IMultiChildView _view;
    protected IMVCAController[] _childController;

    protected string _descriptionText = string.Empty;

    public MultiChildController(IMVCAController[] childs)
    {
      Initialize(childs);
    }
    protected MultiChildController()
    {
    }
    protected void Initialize(IMVCAController[] childs)
    {
      _childController = childs;
      Initialize();
    }

    protected virtual void Initialize()
    {
      if(null!=_view)
      {
        object[] controls = new object[_childController.Length];
        for(int i=0;i<controls.Length;i++)
          controls[i] = _childController[i].ViewObject;

        _view.InitializeDescription(_descriptionText);
        _view.InitializeChilds(controls,0);
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
        if(false== _childController[i].Apply())
        {
          return false;
        }
      }

      return true;
    }

    #endregion
  
  }
}
