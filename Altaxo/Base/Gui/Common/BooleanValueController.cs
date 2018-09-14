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

using System;

namespace Altaxo.Gui.Common
{
  #region Interfaces

  public interface IBooleanValueView
  {
    void InitializeDescription(string value);

    void InitializeBool1(bool value);

    event Action<bool> Bool1Changed;
  }

  public interface IBooleanValueController : IMVCAController
  {
    string DescriptionText { get; set; }
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for a boolean value.
  /// </summary>
  [UserControllerForObject(typeof(bool), 100)]
  [ExpectedTypeOfView(typeof(IBooleanValueView))]
  public class BooleanValueController : IBooleanValueController
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
      if (null != _view)
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
        if (null != _view)
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
        if (_view != null)
        {
          _view.Bool1Changed -= EhValidatingBool1;
        }

        _view = value as IBooleanValueView;

        if (_view != null)
        {
          _view.Bool1Changed += EhValidatingBool1;
          Initialize();
        }
      }
    }

    public virtual object ModelObject
    {
      get
      {
        return _value1Bool;
      }
    }

    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IApplyController Members

    public virtual bool Apply(bool disposeController)
    {
      _value1Bool = _value1BoolTemporary;
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

    #region ISingleValueViewEventSink Members

    public virtual void EhValidatingBool1(bool val)
    {
      _value1BoolTemporary = val;
    }

    #endregion ISingleValueViewEventSink Members
  }
}
