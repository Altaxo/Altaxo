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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Altaxo.Units;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Supports the entering of length values (units: cm, mm, points and so on), and optionally relative units (percent of something).
  /// </summary>
  public class QuantityWithUnitTextBox : TextBox, IDimensionfulQuantityView
  {
    public event DependencyPropertyChangedEventHandler SelectedQuantityChanged;

    public event DependencyPropertyChangedEventHandler SelectedQuantityWithUnitEnvironmentChanged;

    private QuantityWithUnitConverter _converter;

    /// <summary>
    /// Static initialization.
    /// </summary>
    static QuantityWithUnitTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(QuantityWithUnitTextBox), new FrameworkPropertyMetadata(typeof(QuantityWithUnitTextBox)));
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public QuantityWithUnitTextBox()
    {
      var binding = new Binding
      {
        Source = this,
        Path = new PropertyPath(nameof(SelectedQuantity)),
        Mode = BindingMode.TwoWay,
        UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
      };
      _converter = new QuantityWithUnitConverter(this, SelectedQuantityProperty);
      binding.Converter = _converter;
      binding.ValidationRules.Add(_converter);
      _converter.BindingExpression = SetBinding(TextBox.TextProperty, binding);
      TextChanged += new TextChangedEventHandler(QuantityWithUnitTextBox_TextChanged);
    }

    public bool AllowNaNValues { get { return _converter.AllowNaNValues; } set { _converter.AllowNaNValues = value; } }

    public string RepresentationOfNaN { get { return _converter.RepresentationOfNaN; } set { _converter.RepresentationOfNaN = value; } }

    public bool AllowInfiniteValues { get { return _converter.AllowInfiniteValues; } set { _converter.AllowInfiniteValues = value; } }

    public bool DisallowNegativeValues { get { return _converter.DisallowNegativeValues; } set { _converter.DisallowNegativeValues = value; } }

    public bool DisallowZeroValues { get { return _converter.DisallowZeroValues; } set { _converter.DisallowZeroValues = value; } }

    #region Change selection behaviour

    // The next three overrides change the selection behaviour of the text box as described in
    // 'How to SelectAll in TextBox when TextBox gets focus by mouse click?'
    // (http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/564b5731-af8a-49bf-b297-6d179615819f/)

    protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
      SelectAll();
      base.OnGotKeyboardFocus(e);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
      base.OnLostFocus(e); // update the Quantity by the default comversion mechanism

      if (!_converter.BindingExpression.HasError)
      {
        _converter.ClearIntermediateConversionResults(); // clear the previous conversion, so that a full new conversion from quantity to string is done when UpdateTarget is called
        _converter.BindingExpression.UpdateTarget();
      }
    }

    protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == System.Windows.Input.Key.F5) // interpret the text and update the quantity
      {
        e.Handled = true;
        _converter.BindingExpression.UpdateSource(); // interpret the text
        if (!_converter.BindingExpression.HasError) // if text was successfully interpreted
        {
          _converter.ClearIntermediateConversionResults(); // clear the previous conversion, so that a full new conversion from quantity to string is done when UpdateTarget is called
          _converter.BindingExpression.UpdateTarget(); // update the text with the full quanity including the unit
          SelectAll(); // select all text so that the user can easily change it
        }
      }

      base.OnKeyDown(e);
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

    private bool _validateWhenTextChange;

    public bool UpdateQuantityIfTextChanged
    {
      set
      {
        _validateWhenTextChange = value;
      }
    }

    private void QuantityWithUnitTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (_validateWhenTextChange)
        _converter.BindingExpression.UpdateSource();
      else
        _converter.BindingExpression.ValidateWithoutUpdate();
    }

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
      _converter.OnContextMenuOpening();
      base.OnContextMenuOpening(e);
    }

    #region Dependency property

    public static readonly DependencyProperty SelectedQuantityProperty =
    DependencyProperty.Register("SelectedQuantity", typeof(DimensionfulQuantity), typeof(QuantityWithUnitTextBox),
    new FrameworkPropertyMetadata(EhSelectedQuantityChanged));

    /// <summary>
    /// Gets/sets the quantity. The quantity consist of a numeric value together with a unit.
    /// </summary>
    public DimensionfulQuantity SelectedQuantity
    {
      get { var result = (DimensionfulQuantity)GetValue(SelectedQuantityProperty); return result; }
      set { SetValue(SelectedQuantityProperty, value); }
    }

    private static void EhSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((QuantityWithUnitTextBox)obj).OnSelectedQuantityChanged(obj, args);
    }

    /// <summary>
    /// Triggers the <see cref="SelectedQuantityChanged"/> event.
    /// </summary>
    /// <param name="obj">Dependency object (here: the control).</param>
    /// <param name="args">Property changed event arguments.</param>
    protected void OnSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (null != SelectedQuantityChanged)
        SelectedQuantityChanged(obj, args);
      if (null != DimensionfulQuantityView_QuantityChanged)
        DimensionfulQuantityView_QuantityChanged();
      if (null != SelectedQuantityWithUnitEnvironmentChanged)
        SelectedQuantityWithUnitEnvironmentChanged(obj, args);
    }

    /// <summary>Gets or sets the selected quantity as value in SI units.</summary>
    /// <value>The selected quantity as value in SI units.</value>
    public double SelectedQuantityAsValueInSIUnits
    {
      get { return SelectedQuantity.AsValueInSIUnits; }
      set { SelectedQuantity = new DimensionfulQuantity(value, _converter.UnitEnvironment.DefaultUnit.Unit.SIUnit).AsQuantityIn(_converter.UnitEnvironment.DefaultUnit); }
    }

    #endregion Dependency property

    #region Dependency property UnitEnvironment

    public static readonly DependencyProperty UnitEnvironmentProperty =
        DependencyProperty.Register(nameof(UnitEnvironment), typeof(QuantityWithUnitGuiEnvironment), typeof(QuantityWithUnitTextBox),
         new FrameworkPropertyMetadata(EhUnitEnvironmentChanged));

    /// <summary>
    /// Sets the unit environment. The unit environment determines the units the user is able to enter.
    /// </summary>
    public QuantityWithUnitGuiEnvironment UnitEnvironment
    {
      get
      {
        return (QuantityWithUnitGuiEnvironment)GetValue(UnitEnvironmentProperty);
      }

      set
      {
        SetValue(UnitEnvironmentProperty, value);
      }
    }

    private static void EhUnitEnvironmentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var thiss = (QuantityWithUnitTextBox)obj;
      if (null != args.NewValue)
        thiss._converter.UnitEnvironment = (QuantityWithUnitGuiEnvironment)args.NewValue;
    }

    #endregion Dependency property UnitEnvironment

    #region IDimensionfulQuantityView

    private event Action DimensionfulQuantityView_QuantityChanged;

    event Action IDimensionfulQuantityView.SelectedQuantityChanged
    {
      add { DimensionfulQuantityView_QuantityChanged += value; }
      remove { DimensionfulQuantityView_QuantityChanged -= value; }
    }

    QuantityWithUnitGuiEnvironment IDimensionfulQuantityView.UnitEnvironment
    {
      set { _converter.UnitEnvironment = value; }
    }

    #endregion IDimensionfulQuantityView
  }
}
