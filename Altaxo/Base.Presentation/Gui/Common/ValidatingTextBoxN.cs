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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Common
{
  public class ValidatingTextBoxN : TextBox
  {
    /// <summary>
    /// Static initialization.
    /// </summary>
    static ValidatingTextBoxN()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ValidatingTextBoxN), new FrameworkPropertyMetadata(typeof(ValidatingTextBoxN)));
    }

    public ValidatingTextBoxN()
    {
    }

    #region Change selection behaviour

    // The next three overrides change the selection behaviour of the text box as described in
    // 'How to SelectAll in TextBox when TextBox gets focus by mouse click?'
    // (http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/564b5731-af8a-49bf-b297-6d179615819f/)

    protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
      SelectAll();
      base.OnGotKeyboardFocus(e);
    }

    protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
    {
      SelectAll();
      base.OnMouseDoubleClick(e);
    }

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

    public string ValidationError
    {
      get { return (string)GetValue(ValidationErrorProperty); }
      set { SetValue(ValidationErrorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ValidationError.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ValidationErrorProperty =
        DependencyProperty.Register(nameof(ValidationError), typeof(string), typeof(ValidatingTextBoxN), new PropertyMetadata(null, EhValidationErrorChanged));

    private static void EhValidationErrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var thiss = (ValidatingTextBoxN)d;
      var binding = thiss.GetBindingExpression(TextProperty);
      if (binding is not null)
      {
        if (string.IsNullOrEmpty((string)e.NewValue))
        {
          Validation.ClearInvalid(binding);
        }
        else
        {
          Validation.MarkInvalid(binding, new ValidationError(ValidatingTextBoxValidationRule.Instance, binding, e.NewValue, null));
        }
      }
    }

    class ValidatingTextBoxValidationRule : ValidationRule
    {
      public static ValidatingTextBoxValidationRule Instance { get; } = new ValidatingTextBoxValidationRule();
      public override ValidationResult Validate(object value, CultureInfo cultureInfo)
      {
        return new ValidationResult(false, (string)value);
      }
    }
  }
}
