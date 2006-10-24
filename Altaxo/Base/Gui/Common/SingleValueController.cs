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

namespace Altaxo.Gui.Common
{

  #region Interfaces
  
  public interface ISingleValueViewEventSink
  {
    void EhValidatingValue1(string val, System.ComponentModel.CancelEventArgs e);
  }
  public interface ISingleValueView
  {
    ISingleValueViewEventSink Controller { set; }

    void InitializeDescription(string value);
    void InitializeValue1(string value);
  }
  public interface ISingleValueController : IMVCAController
  {
    string DescriptionText { get; set; }
  }

  #endregion

  /// <summary>
  /// Controller for a single value. This is a string here, but in derived classes, that can be anything that can be converted to and from a string.
  /// </summary>
  [UserControllerForObject(typeof(string),100)]
  [ExpectedTypeOfView(typeof(ISingleValueView))]
  public class SingleValueController : ISingleValueController, ISingleValueViewEventSink
  {
    protected ISingleValueView _view;
    protected string _value1String;
    protected string _value1StringTemporary;

    protected string _descriptionText = "Enter value:";

    public SingleValueController(string val)
    {
      _value1String = val;
      _value1StringTemporary = val;
    }

    protected virtual void Initialize()
    {
      if(null!=_view)
      {
        _view.InitializeDescription(_descriptionText);
        _view.InitializeValue1(_value1StringTemporary);
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

        _view = value as ISingleValueView;
        
        Initialize();

        if(_view!=null)
          _view.Controller = this;
      }
    }

    public virtual object ModelObject
    {
      get
      {
        return _value1String;
      }
    }

    #endregion

    #region IApplyController Members

    public virtual bool Apply()
    {
      this._value1String = this._value1StringTemporary;
      return true;
    }

    #endregion

    #region ISingleValueViewEventSink Members

    public virtual void EhValidatingValue1(string val, System.ComponentModel.CancelEventArgs e)
    {
      _value1StringTemporary = val;
    }

    #endregion
  }
}
