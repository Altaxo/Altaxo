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

#nullable disable warnings
using System;
using System.Globalization;
using System.Numerics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Numeric up-down control for editing <see cref="BigInteger"/> values.
  /// </summary>
  public partial class BigIntegerUpDown : NumericUpDownBase
  {
    private  BigInteger DefaultMinValue = 0;
    private BigInteger DefaultValue = 0;
    private  BigInteger DefaultMaxValue = 100;
    private  BigInteger DefaultChange = 1;

    #region Converter

    /// <inheritdoc/>
    protected override object GetNewValidationRuleAndConverter()
    {
      return new BigIntegerConverter();
    }

    /// <summary>
    /// Gets the converter used by the control.
    /// </summary>
    protected BigIntegerConverter Converter => (BigIntegerConverter)_validationRuleAndConverter;

    #endregion Converter

    #region Properties

    #region Value

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    public BigInteger Value
    {
      get { return (BigInteger)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    /// <summary>
    /// Gets or sets the current value as <see cref="double"/>.
    /// </summary>
    public double ValueAsDouble
    {
      get { return (double)(BigInteger)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, (BigInteger)value); }
    }

    /// <summary>
    /// Identifies the Value dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            "Value", typeof(BigInteger), typeof(BigIntegerUpDown),
            new FrameworkPropertyMetadata((BigInteger)0,
                new PropertyChangedCallback(OnValueChanged),
                new CoerceValueCallback(CoerceValue)
            )
        );

    private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var control = (BigIntegerUpDown)obj;

      BigInteger oldValue = (BigInteger)args.OldValue;
      BigInteger newValue = (BigInteger)args.NewValue;

      #region Fire Automation events

      var peer = UIElementAutomationPeer.FromElement(control) as BigIntegerUpDownAutomationPeer;
      if (peer is not null)
      {
        peer.RaiseValueChangedEvent(oldValue, newValue);
      }

      #endregion Fire Automation events

      var e = new RoutedPropertyChangedEventArgs<BigInteger>(
          oldValue, newValue, ValueChangedEvent);

      control.OnValueChanged(e);
    }

    /// <summary>
    /// Raises the ValueChanged event.
    /// </summary>
    /// <param name="args">Arguments associated with the ValueChanged event.</param>
    protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<BigInteger> args)
    {
      RaiseEvent(args);
    }

    private static object CoerceValue(DependencyObject element, object value)
    {
      var newValue = (BigInteger)value;
      var control = (BigIntegerUpDown)element;

      if (control.Minimum.HasValue && newValue < control.Minimum.Value)
        newValue = control.Minimum.Value;
      else if (control.Maximum.HasValue && newValue > control.Maximum.Value)
        newValue = control.Maximum.Value;

      return newValue;
    }

    #endregion Value

    #region ValueIfTextIsEmpty

    /// <summary>
    /// Gets or sets the value used when the text box is empty.
    /// </summary>
    public BigInteger? ValueIfTextIsEmpty
    {
      get { return (BigInteger?)GetValue(ValueIfTextIsEmptyProperty); }
      set { SetValue(ValueIfTextIsEmptyProperty, value); }
    }

    /// <summary>
    /// Identifies the Value dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueIfTextIsEmptyProperty =
        DependencyProperty.Register(
            nameof(ValueIfTextIsEmpty),
            typeof(BigInteger?),
            typeof(BigIntegerUpDown),
            new FrameworkPropertyMetadata(null, EhValueIfTextIsEmptyChanged)
    );

    private static void EhValueIfTextIsEmptyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((BigIntegerUpDown)obj).OnValueIfTextIsEmptyChanged(obj, args);
    }

    /// <summary>
    /// Updates the fallback value used when the text box is empty.
    /// </summary>
    /// <param name="obj">Dependency object (here: the control).</param>
    /// <param name="args">Property changed event arguments.</param>
    protected void OnValueIfTextIsEmptyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      Converter.ValueIfTextIsEmpty = (BigInteger?)args.NewValue;
    }

    #endregion ValueIfTextIsEmpty

    #region ValueString

    /// <summary>
    /// Gets the textual representation of the current value.
    /// </summary>
    public string ValueString
    {
      get
      {
        return (string)GetValue(ValueStringProperty);
      }
    }

    private static readonly DependencyPropertyKey ValueStringPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly("ValueString", typeof(string), typeof(BigIntegerUpDown), new PropertyMetadata());

    /// <summary>
    /// Identifies the <see cref="ValueString"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueStringProperty = ValueStringPropertyKey.DependencyProperty;

    #endregion ValueString

    #region Minimum

    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    public BigInteger? Minimum
    {
      get { return (BigInteger?)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Minimum"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(
            "Minimum", typeof(BigInteger?), typeof(BigIntegerUpDown),
            new FrameworkPropertyMetadata(null,
                new PropertyChangedCallback(OnMinimumChanged), new CoerceValueCallback(CoerceMinimum)
            )
        );

    private static void OnMinimumChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
      var thiss = (BigIntegerUpDown)element;
      element.CoerceValue(MaximumProperty);
      element.CoerceValue(ValueProperty);
      thiss.Converter.MinValue = thiss.Minimum;
    }

    private static object CoerceMinimum(DependencyObject element, object value)
    {
      var thiss = (BigIntegerUpDown)element;
      var minimum = (BigInteger?)value;
      thiss.Converter.MinValue = minimum;
      return minimum;
    }

    #endregion Minimum

    #region Maximum

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    public BigInteger? Maximum
    {
      get { return (BigInteger?)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Maximum"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(
            "Maximum", typeof(BigInteger?), typeof(BigIntegerUpDown),
            new FrameworkPropertyMetadata(null,
                new PropertyChangedCallback(OnMaximumChanged),
                new CoerceValueCallback(CoerceMaximum)
            )
        );

    private static void OnMaximumChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
      var thiss = (BigIntegerUpDown)element;
      element.CoerceValue(ValueProperty);
      thiss.Converter.MaxValue = thiss.Maximum;
    }

    private static object CoerceMaximum(DependencyObject element, object value)
    {
      var thiss = (BigIntegerUpDown)element;
      var newMaximum = (BigInteger?)value;

      if (newMaximum.HasValue && thiss.Minimum.HasValue && newMaximum < thiss.Minimum.Value)
        newMaximum = thiss.Minimum.Value;

      thiss.Converter.MaxValue = newMaximum;
      return newMaximum;
    }

    #endregion Maximum

    #region Change

    /// <summary>
    /// Gets or sets the increment used by the control.
    /// </summary>
    public BigInteger Change
    {
      get { return (BigInteger)GetValue(ChangeProperty); }
      set { SetValue(ChangeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Change"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ChangeProperty =
        DependencyProperty.Register(
            "Change", typeof(BigInteger), typeof(BigIntegerUpDown),
            new FrameworkPropertyMetadata((BigInteger)1, new PropertyChangedCallback(OnChangeChanged), new CoerceValueCallback(CoerceChange)),
        new ValidateValueCallback(ValidateChange)
        );

    private static bool ValidateChange(object value)
    {
      BigInteger change = (BigInteger)value;
      return change > 0;
    }

    /// <summary>
    /// Handles changes to the change increment.
    /// </summary>
    /// <param name="element">The dependency object.</param>
    /// <param name="args">The property changed event arguments.</param>
    private static void OnChangeChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
    }

    /// <summary>
    /// Coerces the change increment.
    /// </summary>
    /// <param name="element">The dependency object.</param>
    /// <param name="value">The value to coerce.</param>
    /// <returns>The coerced value.</returns>
    private static object CoerceChange(DependencyObject element, object value)
    {
      BigInteger newChange = (BigInteger)value;
      var control = (BigIntegerUpDown)element;

      BigInteger coercedNewChange = newChange;
      

      return coercedNewChange;
    }

    #endregion Change


    #endregion Properties

    #region Events

    /// <summary>
    /// Identifies the ValueChanged routed event.
    /// </summary>
    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
        "ValueChanged", RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<BigInteger>), typeof(BigIntegerUpDown));

    /// <summary>
    /// Occurs when the Value property changes.
    /// </summary>
    public event RoutedPropertyChangedEventHandler<BigInteger> ValueChanged
    {
      add { AddHandler(ValueChangedEvent, value); }
      remove { RemoveHandler(ValueChangedEvent, value); }
    }

    #endregion Events

    #region Commands

    /// <inheritdoc/>
    protected override void OnIncrease()
    {
      if (Maximum.HasValue && (Value + 1) > Maximum.Value)
        Value = Maximum.Value;
      else
        Value = Value + 1;
    }

    /// <inheritdoc/>
    protected override void OnDecrease()
    {
      if (Minimum.HasValue && (Value - 1) < Minimum.Value)
        Value = Minimum.Value;
      else
        Value = Value - 1;
    }

    /// <inheritdoc/>
    protected override void OnGotoMinimum()
    {
      if (Minimum.HasValue)
        Value = Minimum.Value;
      else
        Value = 0;
    }

    /// <inheritdoc/>
    protected override void OnGotoMaximum()
    {
      if(Maximum.HasValue)
        Value = Maximum.Value;
    }

    #endregion Commands

    #region Automation

    /// <inheritdoc/>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return new BigIntegerUpDownAutomationPeer(this);
    }

    #endregion Automation
  }

  /// <summary>
  /// Automation peer for <see cref="BigIntegerUpDown"/>.
  /// </summary>
  public class BigIntegerUpDownAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BigIntegerUpDownAutomationPeer"/> class.
    /// </summary>
    /// <param name="control">The associated <see cref="BigIntegerUpDown"/> control.</param>
    public BigIntegerUpDownAutomationPeer(BigIntegerUpDown control)
      : base(control)
    {
    }

    /// <inheritdoc/>
    protected override string GetClassNameCore()
    {
      return "BigIntegerUpDown";
    }

    /// <inheritdoc/>
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Spinner;
    }

    /// <inheritdoc/>
    public override object GetPattern(PatternInterface patternInterface)
    {
      if (patternInterface == PatternInterface.RangeValue)
      {
        return this;
      }
      return base.GetPattern(patternInterface);
    }

    /// <summary>
    /// Raises the automation value-changed event.
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    internal void RaiseValueChangedEvent(BigInteger oldValue, BigInteger newValue)
    {
      base.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty,
          (double)oldValue, (double)newValue);
    }

    #region IRangeValueProvider Members

    /// <summary>
    /// Gets a value indicating whether the control is read-only.
    /// </summary>
    bool IRangeValueProvider.IsReadOnly
    {
      get
      {
        return !IsEnabled();
      }
    }

    /// <summary>
    /// Gets the large change amount.
    /// </summary>
    double IRangeValueProvider.LargeChange
    {
      get { return (double)MyOwner.Change; }
    }

    /// <summary>
    /// Gets the maximum value.
    /// </summary>
    double IRangeValueProvider.Maximum
    {
      get { return (double)MyOwner.Maximum; }
    }

    /// <summary>
    /// Gets the minimum value.
    /// </summary>
    double IRangeValueProvider.Minimum
    {
      get { return (double)MyOwner.Minimum; }
    }

    /// <summary>
    /// Sets the current value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    void IRangeValueProvider.SetValue(double value)
    {
      if (!IsEnabled())
      {
        throw new ElementNotEnabledException();
      }

      BigInteger val = (BigInteger)value;
      if (val < MyOwner.Minimum || val > MyOwner.Maximum)
      {
        throw new ArgumentOutOfRangeException("value");
      }

      MyOwner.Value = val;
    }

    /// <summary>
    /// Gets the small change amount.
    /// </summary>
    double IRangeValueProvider.SmallChange
    {
      get { return (double)MyOwner.Change; }
    }

    /// <summary>
    /// Gets the current value.
    /// </summary>
    double IRangeValueProvider.Value
    {
      get { return (double)MyOwner.Value; }
    }

    #endregion IRangeValueProvider Members

    private BigIntegerUpDown MyOwner
    {
      get
      {
        return (BigIntegerUpDown)base.Owner;
      }
    }
  }
}
