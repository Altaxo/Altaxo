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

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Controller that edits a single text value through an <see cref="ISingleValueView"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(ISingleValueView))]
  public class TextValueInputController : IMVCAController
  {
    private ISingleValueView? _view;
    private string _captionText;

    private string _initialContents;
    private string _contents;
    private bool _isContentsValid = true;

    private IStringValidator? _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextValueInputController"/> class.
    /// </summary>
    /// <param name="initialcontents">The initial text shown in the view.</param>
    /// <param name="description">The descriptive caption text.</param>
    public TextValueInputController(string initialcontents, string description)
    {
      _initialContents = initialcontents;
      _contents = initialcontents;
      _captionText = description;
    }

    /// <summary>
    /// Initializes the view from the current controller state.
    /// </summary>
    private void Initialize()
    {
      if (_view is not null)
      {
        _view.DescriptionText = _captionText;
        _view.ValueText = _initialContents;
      }
    }

    private ISingleValueView? View
    {
      get { return _view; }
      set
      {
        if (_view is not null)
          _view.ValueText_Validating -= EhView_ValidatingValue1;

        _view = value;
        Initialize();

        if (_view is not null)
          _view.ValueText_Validating += EhView_ValidatingValue1;
      }
    }

    /// <summary>
    /// Gets the current input text.
    /// </summary>
    public string InputText
    {
      get { return _contents; }
    }

    /// <summary>
    /// Sets the validator used to validate the input text.
    /// </summary>
    public IStringValidator Validator
    {
      set { _validator = value; }
    }

    #region ISingleValueFormController Members

    /// <summary>
    /// Validates the current text entered in the view.
    /// </summary>
    /// <param name="e">The validation event arguments.</param>
    public void EhView_ValidatingValue1(ValidationEventArgs<string> e)
    {
      _isContentsValid = true;
      _contents = e.ValueToValidate;
      if (_validator is not null)
      {
        var err = _validator.Validate(_contents);
        if (err is not null)
        {
          _isContentsValid = false;
          e.AddError(err);
          return;
        }
      }
      else // if no validating handler, use some default validation
      {
        if (_contents is null || 0 == _contents.Length)
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
      string? Validate(string txt);
    }

    /// <summary>
    /// Provides a validator that tests if the string entered is empty.
    /// </summary>
    public class NonEmptyStringValidator : IStringValidator
    {
      /// <summary>
      /// The validation message shown when the text is empty.
      /// </summary>
      protected string m_EmptyMessage = "You have not entered text. Please enter text!";

      /// <summary>
      /// Initializes a new instance of the <see cref="NonEmptyStringValidator"/> class.
      /// </summary>
      public NonEmptyStringValidator()
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="NonEmptyStringValidator"/> class with a custom error message.
      /// </summary>
      /// <param name="errmsg">The validation message shown when the text is empty.</param>
      public NonEmptyStringValidator(string errmsg)
      {
        m_EmptyMessage = errmsg;
      }

      /// <inheritdoc/>
      public virtual string? Validate(string txt)
      {
        if (txt is null || txt.Trim().Length == 0)
          return m_EmptyMessage;
        else
          return null;
      }
    }

    #endregion Validator classes

    #region IMVCController Members

    /// <inheritdoc/>
    public object? ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view is not null)
          _view.ValueText_Validating -= EhView_ValidatingValue1;

        _view = value as ISingleValueView;
        if (_view is not null)
        {
          Initialize();
          _view.ValueText_Validating += EhView_ValidatingValue1;
        }
      }
    }

    /// <inheritdoc/>
    public object ModelObject
    {
      get { return _initialContents; }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IApplyController Members

    /// <inheritdoc/>
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
    ///   <c>true</c> if the revert operation was successful; <c>false</c> if the revert operation was not possible, for example because the controller has not stored the original state of the model.
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IApplyController Members
  } // end of class TextValueInputController
}
