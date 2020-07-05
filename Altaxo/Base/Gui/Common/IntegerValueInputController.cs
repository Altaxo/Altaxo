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

namespace Altaxo.Gui.Common
{
  [ExpectedTypeOfView(typeof(ISingleValueView))]
  [UserControllerForObject(typeof(int))]
  public class IntegerValueInputController : IMVCAController
  {
    private ISingleValueView? _view;

    private int _initialContents;

    private int _enteredContents;

    private string _description;

    private IIntegerValidator? _validator;

    public IntegerValueInputController(int initialcontents)
      : this(initialcontents, "Value: ")
    {
    }

    public IntegerValueInputController(int initialcontents, string description)
    {
      _initialContents = initialcontents;
      _enteredContents = initialcontents;
      _description = description;
    }

    private void Initialize()
    {
      if (_view is null)
        throw new InvalidProgramException();

      _view.DescriptionText = _description;
      _view.ValueText = _initialContents.ToString();
    }

    private ISingleValueView? View
    {
      get { return _view; }
      set
      {
        if (_view != null)
          _view.ValueText_Validating -= EhView_ValidatingValue1;

        _view = value;
        Initialize();

        if (_view != null)
          _view.ValueText_Validating += EhView_ValidatingValue1;
      }
    }

    public int EnteredContents
    {
      get { return _enteredContents; }
    }

    public IIntegerValidator Validator
    {
      set { _validator = value; }
    }

    protected bool Validate()
    {
      if (_view is null)
        throw new InvalidProgramException();

      string value = _view.ValueText;
      string? err = null;
      if (Altaxo.Serialization.GUIConversion.IsInteger(value, out _enteredContents))
      {
        if (null != _validator)
          err = _validator.Validate(_enteredContents);
      }
      else
      {
        err = "You must enter a integer value!";
      }

      if (null != err)
        Current.Gui.ErrorMessageBox(err);

      return null == err;
    }

    #region ISingleValueFormController Members

    public void EhView_ValidatingValue1(ValidationEventArgs<string> e)
    {
      if (!int.TryParse(e.ValueToValidate, out var val))
        e.AddError("Value has to be a valid integer");
    }

    #endregion ISingleValueFormController Members

    /// <summary>
    /// Provides an interface to a validator to validates the user input
    /// </summary>
    public interface IIntegerValidator
    {
      /// <summary>
      /// Validates if the user input number i is valid user input.
      /// </summary>
      /// <param name="i">The number entered by the user.</param>
      /// <returns>Null if this input is valid, error message else.</returns>
      string? Validate(int i);
    }

    public class ZeroOrPositiveIntegerValidator : IIntegerValidator
    {
      public string? Validate(int i)
      {
        if (i < 0)
          return "The provided number must be zero or positive!";
        else
          return null;
      }
    }

    #region IMVCController Members

    public object? ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view != null)
          _view.ValueText_Validating -= EhView_ValidatingValue1;

        _view = value as ISingleValueView;
        if (_view != null)
        {
          Initialize();
          _view.ValueText_Validating += EhView_ValidatingValue1;
        }
      }
    }

    public object ModelObject
    {
      get { return _initialContents; }
    }

    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IApplyController Members

    public bool Apply(bool disposeController)
    {
      if (!Validate())
        return false;

      _initialContents = _enteredContents;
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
  } // end of class IntegerValueInputController
}
