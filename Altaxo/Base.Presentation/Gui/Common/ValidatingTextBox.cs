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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Text box that validates its text through a callback and exposes validation state.
  /// </summary>
  public class ValidatingTextBox : TextBox
  {
    private NotifyChangedValue<string> _validatedText = new NotifyChangedValue<string>();
    private bool _isInitialTextModified;
    private bool _isValidatedSuccessfully = true;
    private BindingExpressionBase _bindingExToValidatedText;

    /// <summary>
    /// Static initialization.
    /// </summary>
    static ValidatingTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ValidatingTextBox), new FrameworkPropertyMetadata(typeof(ValidatingTextBox)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatingTextBox"/> class.
    /// </summary>
    public ValidatingTextBox()
    {
      var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(TextBox.TextProperty, GetType());
      dpd.AddValueChanged(this, EhTextChanged);

      var binding = new Binding
      {
        Source = this,
        Path = new PropertyPath(nameof(ValidatedText))
      };
      binding.ValidationRules.Add(new ValidationWithErrorString(EhValidateText));
      _bindingExToValidatedText = SetBinding(TextBox.TextProperty, binding);



    }

    #region Change selection behaviour

    // The next three overrides change the selection behaviour of the text box as described in
    // 'How to SelectAll in TextBox when TextBox gets focus by mouse click?'
    // (http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/564b5731-af8a-49bf-b297-6d179615819f/)

    /// <inheritdoc/>
    protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
      SelectAll();
      base.OnGotKeyboardFocus(e);
    }

    /// <inheritdoc/>
    protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
    {
      SelectAll();
      base.OnMouseDoubleClick(e);
    }

    /// <inheritdoc/>
    protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
    {
      if (!IsKeyboardFocusWithin)
      {
        e.Handled = true;
        Focus();
      }
      else
      {
        base.OnPreviewMouseLeftButtonDown(e);
      }
    }

    #endregion Change selection behaviour

    #region Dependency property

    /// <summary>
    /// Gets or sets the validated text.
    /// </summary>
    public string ValidatedText
    {
      get { var result = (string)GetValue(ValidatedTextProperty); return result; }
      set { SetValue(ValidatedTextProperty, value); _isValidatedSuccessfully = true; }
    }

    /// <summary>
    /// Identifies the <see cref="ValidatedText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValidatedTextProperty =
        DependencyProperty.Register("ValidatedText", typeof(string), typeof(ValidatingTextBox),
        new FrameworkPropertyMetadata(OnValidatedTextChanged));

    private static void OnValidatedTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
    }

    #endregion Dependency property

    /// <summary>
    /// Gets or sets a value indicating whether the validation is carried out on every text change
    /// </summary>
    /// <value>
    ///   <c>true</c> if the validation is carried out on every text change; otherwise, <c>false</c>.
    /// </value>
    public bool IsValidatingOnEveryTextChange { get; set; }

    /// <summary>
    /// Gets or sets the initial text without marking it as modified.
    /// </summary>
    public string InitialText
    {
      set
      {
        base.Text = value;
        _isInitialTextModified = false;
        _isValidatedSuccessfully = true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the initial text has been modified.
    /// </summary>
    public bool IsInitialTextModified
    {
      get
      {
        return _isInitialTextModified;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the last validation succeeded.
    /// </summary>
    public bool IsValidatedSuccessfully
    {
      get
      {
        return _isValidatedSuccessfully;
      }
    }

    /// <summary>
    /// Handles text changes and optionally triggers validation.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void EhTextChanged(object? sender, EventArgs e)
    {
      _isInitialTextModified = true;

      if (IsValidatingOnEveryTextChange)
        _bindingExToValidatedText?.UpdateSource();
    }

    /// <summary>
    /// Is called when the content of the TextBox needs validation.
    /// </summary>
    public event ValidatingStringEventHandler? Validating;

    /// <summary>
    /// Validates the current text and returns an error string.
    /// </summary>
    /// <param name="obj">The value to validate.</param>
    /// <param name="info">The culture to use during validation.</param>
    /// <returns>The validation error text, or <see langword="null"/> if the value is valid.</returns>
    public string EhValidateText(object obj, System.Globalization.CultureInfo info)
    {
      var evt = Validating;
      if (evt is not null)
      {
        var e = new ValidationEventArgs<string>((string)GetValue(TextBox.TextProperty), info);
        evt(this, e);
        return e.ErrorText;
      }
      else
      {
        return null;
      }
    }
  }
}
