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
  
  public interface IBooleanValueViewEventSink
  {
    void EhValidatingBool1(bool val);
  }
  public interface IBooleanValueView
  {
    IBooleanValueViewEventSink Controller { set; }

    void InitializeDescription(string value);
    void InitializeBool1(bool value);
  }
 
  public interface IBooleanValueController : IMVCAController
  {
    string DescriptionText { get; set; }
  }

  #endregion

  /// <summary>
  /// Controller for a boolean value.
  /// </summary>
  [UserControllerForObject(typeof(bool),100)]
  [ExpectedTypeOfView(typeof(IBooleanValueView))]
  public class BooleanValueController : IBooleanValueController, IBooleanValueViewEventSink
  {
    protected IBooleanValueView _view;
    protected bool _value1Bool;
    protected bool _value1BoolTemporary;

    protected string _descriptionText = "Enter value:";

    public BooleanValueController(bool val)
    {
      _value1Bool = val;
      _value1BoolTemporary = val;
    }

    protected virtual void Initialize()
    {
      if(null!=_view)
      {
        _view.InitializeDescription(_descriptionText);
        _view.InitializeBool1(_value1BoolTemporary);
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

        _view = value as IBooleanValueView;
        
        Initialize();

        if(_view!=null)
          _view.Controller = this;
      }
    }

    public virtual object ModelObject
    {
      get
      {
        return _value1Bool;
      }
    }

    #endregion

    #region IApplyController Members

    public virtual bool Apply()
    {
      this._value1Bool = this._value1BoolTemporary;
      return true;
    }

    #endregion

    #region ISingleValueViewEventSink Members

    public virtual void EhValidatingBool1(bool val)
    {
      _value1BoolTemporary = val;
    }

    #endregion
  }
}
