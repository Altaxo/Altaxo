#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Text box for editing nullable <see cref="double"/> values.
  /// </summary>
  public class NullableDoubleTextBox : TextBox
  {
    /// <summary>
    /// Occurs when <see cref="SelectedValue"/> changes.
    /// </summary>
    public event DependencyPropertyChangedEventHandler? SelectedValueChanged;

    private NullableDoubleConverter _converter;

    /// <summary>
    /// Static initialization.
    /// </summary>
    static NullableDoubleTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(NullableDoubleTextBox), new FrameworkPropertyMetadata(typeof(NullableDoubleTextBox)));
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public NullableDoubleTextBox()
    {
      var binding = new Binding
      {
        Source = this,
        Path = new PropertyPath("SelectedValue"),
        Mode = BindingMode.TwoWay,
        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
      };
      _converter = new NullableDoubleConverter();
      binding.Converter = _converter;
      binding.ValidationRules.Add(_converter);
      SetBinding(TextBox.TextProperty, binding);
    }

    /// <summary>
    /// Gets or sets a value indicating whether NaN values are accepted.
    /// </summary>
    public bool AllowNaNValues { get { return _converter.AllowNaNValues; } set { _converter.AllowNaNValues = value; } }

    /// <summary>
    /// Gets or sets a value indicating whether infinite values are accepted.
    /// </summary>
    public bool AllowInfiniteValues { get { return _converter.AllowInfiniteValues; } set { _converter.AllowInfiniteValues = value; } }

    /// <summary>
    /// Gets or sets a value indicating whether negative values are rejected.
    /// </summary>
    public bool DisallowNegativeValues { get { return _converter.DisallowNegativeValues; } set { _converter.DisallowNegativeValues = value; } }

    /// <summary>
    /// Gets or sets a value indicating whether zero is rejected.
    /// </summary>
    public bool DisallowZeroValues { get { return _converter.DisallowZeroValues; } set { _converter.DisallowZeroValues = value; } }

    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    public double MinValue { get { return _converter.MinValue; } set { _converter.MinValue = value; } }

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    public double MaxValue { get { return _converter.MaxValue; } set { _converter.MaxValue = value; } }

    /// <summary>
    /// Gets or sets a value indicating whether the minimum value is inclusive.
    /// </summary>
    public bool IsMinValueInclusive { get { return _converter.IsMinValueInclusive; } set { _converter.IsMinValueInclusive = value; } }

    /// <summary>
    /// Gets or sets a value indicating whether the maximum value is inclusive.
    /// </summary>
    public bool IsMaxValueInclusive { get { return _converter.IsMaxValueInclusive; } set { _converter.IsMaxValueInclusive = value; } }

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
    /// Gets or sets the selected nullable value.
    /// </summary>
    public double? SelectedValue
    {
      get { return (double?)GetValue(SelectedValueProperty); }
      set { SetValue(SelectedValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedValue"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedValueProperty =
        DependencyProperty.Register("SelectedValue", typeof(double?), typeof(NullableDoubleTextBox),
        new FrameworkPropertyMetadata(EhSelectedValueChanged) { BindsTwoWayByDefault=true});

    private static void EhSelectedValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((NullableDoubleTextBox)obj).OnSelectedValueChanged(obj, args);
    }

    /// <summary>
    /// Triggers the <see cref="SelectedValueChanged"/> event.
    /// </summary>
    /// <param name="obj">Dependency object (here: the control).</param>
    /// <param name="args">Property changed event arguments.</param>
    protected void OnSelectedValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      SelectedValueChanged?.Invoke(obj, args);
    }

    #endregion Dependency property
  }
}
