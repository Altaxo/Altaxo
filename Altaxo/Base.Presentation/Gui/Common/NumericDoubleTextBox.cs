#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Text box for editing <see cref="double"/> values.
  /// </summary>
  public class NumericDoubleTextBox : TextBox
  {
    /// <summary>
    /// Occurs when <see cref="SelectedValue"/> changes.
    /// </summary>
    public event DependencyPropertyChangedEventHandler? SelectedValueChanged;

    private NumericDoubleConverter _converter;

    /// <summary>
    /// Static initialization.
    /// </summary>
    static NumericDoubleTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericDoubleTextBox), new FrameworkPropertyMetadata(typeof(NumericDoubleTextBox)));
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public NumericDoubleTextBox()
    {
      var binding = new Binding
      {
        Source = this,
        Path = new PropertyPath("SelectedValue"),
        Mode = BindingMode.TwoWay,
        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
      };
      _converter = new NumericDoubleConverter();
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

    #region SelectedValue

    /// <summary>
    /// Gets or sets the selected value.
    /// </summary>
    public double SelectedValue
    {
      get { return (double)GetValue(SelectedValueProperty); }
      set { SetValue(SelectedValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedValue"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedValueProperty =
        DependencyProperty.Register(nameof(SelectedValue), typeof(double), typeof(NumericDoubleTextBox),
        new FrameworkPropertyMetadata(EhSelectedValueChanged) { BindsTwoWayByDefault=true});

    private static void EhSelectedValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((NumericDoubleTextBox)obj).OnSelectedValueChanged(obj, args);
    }

    /// <summary>
    /// Updates listeners after <see cref="SelectedValue"/> changes.
    /// </summary>
    /// <param name="obj">Dependency object (here: the control).</param>
    /// <param name="args">Property changed event arguments.</param>
    protected void OnSelectedValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (SelectedValueChanged is not null)
        SelectedValueChanged(obj, args);
    }

    #endregion Dependency property

    #region MinValue

    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    public double MinValue
    {
      get { return (double)GetValue(MinValueProperty); }
      set { SetValue(MinValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinValue"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinValueProperty =
    DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(NumericDoubleTextBox),
    new FrameworkPropertyMetadata(NumericDoubleConverter.DefaultValue_MinValue, EhMinValueChanged));

    private static void EhMinValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((NumericDoubleTextBox)obj).OnMinValueChanged(obj, args);
    }

    /// <summary>
    /// Updates the converter when <see cref="MinValue"/> changes.
    /// </summary>
    /// <param name="obj">Dependency object (here: the control).</param>
    /// <param name="args">Property changed event arguments.</param>
    protected void OnMinValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      _converter.MinValue = (double)args.NewValue;
    }

    #endregion

    #region MaxValue

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    public double MaxValue
    {
      get { return (double)GetValue(MaxValueProperty); }
      set { SetValue(MaxValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MaxValue"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxValueProperty =
    DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(NumericDoubleTextBox),
    new FrameworkPropertyMetadata(NumericDoubleConverter.DefaultValue_MaxValue, EhMaxValueChanged));

    private static void EhMaxValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((NumericDoubleTextBox)obj).OnMaxValueChanged(obj, args);
    }

    /// <summary>
    /// Updates the converter when <see cref="MaxValue"/> changes.
    /// </summary>
    /// <param name="obj">Dependency object (here: the control).</param>
    /// <param name="args">Property changed event arguments.</param>
    protected void OnMaxValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      _converter.MaxValue = (double)args.NewValue;
    }

    #endregion

    #region IsMinValueInclusive

    /// <summary>
    /// Gets or sets a value indicating whether the minimum value is inclusive.
    /// </summary>
    public bool IsMinValueInclusive
    {
      get { return (bool)GetValue(IsMinValueInclusiveProperty); }
      set { SetValue(IsMinValueInclusiveProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsMinValueInclusive"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsMinValueInclusiveProperty =
    DependencyProperty.Register(nameof(IsMinValueInclusive), typeof(bool), typeof(NumericDoubleTextBox),
    new FrameworkPropertyMetadata(NumericDoubleConverter.DefaultValue_IsMinValueInclusive, EhIsMinValueInclusiveChanged));

    private static void EhIsMinValueInclusiveChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((NumericDoubleTextBox)obj).OnIsMinValueInclusiveChanged(obj, args);
    }

    /// <summary>
    /// Updates the converter when <see cref="IsMinValueInclusive"/> changes.
    /// </summary>
    /// <param name="obj">Dependency object (here: the control).</param>
    /// <param name="args">Property changed event arguments.</param>
    protected void OnIsMinValueInclusiveChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      _converter.IsMinValueInclusive = (bool)args.NewValue;
    }

    #endregion

    #region IsMaxValueInclusive

    /// <summary>
    /// Gets or sets a value indicating whether the maximum value is inclusive.
    /// </summary>
    public bool IsMaxValueInclusive
    {
      get { return (bool)GetValue(IsMaxValueInclusiveProperty); }
      set { SetValue(IsMaxValueInclusiveProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsMaxValueInclusive"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsMaxValueInclusiveProperty =
    DependencyProperty.Register(nameof(IsMaxValueInclusive), typeof(bool), typeof(NumericDoubleTextBox),
    new FrameworkPropertyMetadata(NumericDoubleConverter.DefaultValue_IsMaxValueInclusive, EhIsMaxValueInclusiveChanged));

    private static void EhIsMaxValueInclusiveChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((NumericDoubleTextBox)obj).OnIsMaxValueInclusiveChanged(obj, args);
    }

    /// <summary>
    /// Updates the converter when <see cref="IsMaxValueInclusive"/> changes.
    /// </summary>
    /// <param name="obj">Dependency object (here: the control).</param>
    /// <param name="args">Property changed event arguments.</param>
    protected void OnIsMaxValueInclusiveChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      _converter.IsMaxValueInclusive = (bool)args.NewValue;
    }

    #endregion

  }
}
