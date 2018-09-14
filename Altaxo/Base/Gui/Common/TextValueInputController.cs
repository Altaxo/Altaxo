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
  [ExpectedTypeOfView(typeof(ISingleValueView))]
  public class TextValueInputController : IMVCAController
  {
    private ISingleValueView _view;
    private string _captionText;

    private string _initialContents;
    private string _contents;
    private bool _isContentsValid = true;

    private IStringValidator _validator;

    public TextValueInputController(string initialcontents, string description)
    {
      _initialContents = initialcontents;
      _contents = initialcontents;
      _captionText = description;
    }

    private void Initialize()
    {
      if (_view != null)
      {
        _view.DescriptionText = _captionText;
        _view.ValueText = _initialContents;
      }
    }

    private ISingleValueView View
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

    public string InputText
    {
      get { return _contents; }
    }

    public IStringValidator Validator
    {
      set { _validator = value; }
    }

    #region ISingleValueFormController Members

    public void EhView_ValidatingValue1(ValidationEventArgs<string> e)
    {
      _isContentsValid = true;
      _contents = e.ValueToValidate;
      if (_validator != null)
      {
        string err = _validator.Validate(_contents);
        if (null != err)
        {
          _isContentsValid = false;
          e.AddError(err);
          return;
        }
      }
      else // if no validating handler, use some default validation
      {
        if (null == _contents || 0 == _contents.Length)
        {
          _isContentsValid = false;
          e.AddError("You have to enter a value!");
          return;
        }
      }
    }

    #endregion ISingleValueFormController Members

    #region Validator classes

    /// <summary>
    /// Provides an interface to a validator to validates the user input
    /// </summary>
    public interface IStringValidator
    {
      /// <summary>
      /// Validates if the user input in txt is valid user input.
      /// </summary>
      /// <param name="txt">The text entered by the user.</param>
      /// <returns>Null if this input is valid, error message else.</returns>
      string Validate(string txt);
    }

    /// <summary>
    /// Provides a validator that tests if the string entered is empty.
    /// </summary>
    public class NonEmptyStringValidator : IStringValidator
    {
      protected string m_EmptyMessage = "You have not entered text. Please enter text!";

      public NonEmptyStringValidator()
      {
      }

      public NonEmptyStringValidator(string errmsg)
      {
        m_EmptyMessage = errmsg;
      }

      public virtual string Validate(string txt)
      {
        if (txt == null || txt.Trim().Length == 0)
          return m_EmptyMessage;
        else
          return null;
      }
    }

    #endregion Validator classes

    #region IMVCController Members

    public object ViewObject
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
      if (_isContentsValid)
        _initialContents = _contents;
      return _isContentsValid;
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
  } // end of class TextValueInputController
}
