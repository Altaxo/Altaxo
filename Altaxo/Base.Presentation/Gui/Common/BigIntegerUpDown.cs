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
  public partial class BigIntegerUpDown : NumericUpDownBase
  {
    private  BigInteger DefaultMinValue = 0;
    private BigInteger DefaultValue = 0;
    private  BigInteger DefaultMaxValue = 100;
    private  BigInteger DefaultChange = 1;

    #region Converter

    protected override object GetNewValidationRuleAndConverter()
    {
      return new BigIntegerConverter();
    }

    protected BigIntegerConverter Converter => (BigIntegerConverter)_validationRuleAndConverter;

    #endregion Converter

    #region Properties

    #region Value

    public BigInteger Value
    {
      get { return (BigInteger)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

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
    /// Triggers the <see cref="SelectedValueChanged"/> event.
    /// </summary>
    /// <param name="obj">Dependency object (here: the control).</param>
    /// <param name="args">Property changed event arguments.</param>
    protected void OnValueIfTextIsEmptyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      Converter.ValueIfTextIsEmpty = (BigInteger?)args.NewValue;
    }

    #endregion ValueIfTextIsEmpty

    #region ValueString

    public string ValueString
    {
      get
      {
        return (string)GetValue(ValueStringProperty);
      }
    }

    private static readonly DependencyPropertyKey ValueStringPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly("ValueString", typeof(string), typeof(BigIntegerUpDown), new PropertyMetadata());

    public static readonly DependencyProperty ValueStringProperty = ValueStringPropertyKey.DependencyProperty;

    #endregion ValueString

    #region Minimum

    public BigInteger? Minimum
    {
      get { return (BigInteger?)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

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

    public BigInteger? Maximum
    {
      get { return (BigInteger?)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

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

    public BigInteger Change
    {
      get { return (BigInteger)GetValue(ChangeProperty); }
      set { SetValue(ChangeProperty, value); }
    }

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

    private static void OnChangeChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
    }

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

    protected override void OnIncrease()
    {
      if (Maximum.HasValue && (Value + 1) > Maximum.Value)
        Value = Maximum.Value;
      else
        Value = Value + 1;
    }

    protected override void OnDecrease()
    {
      if (Minimum.HasValue && (Value - 1) < Minimum.Value)
        Value = Minimum.Value;
      else
        Value = Value - 1;
    }

    protected override void OnGotoMinimum()
    {
      if (Minimum.HasValue)
        Value = Minimum.Value;
      else
        Value = 0;
    }

    protected override void OnGotoMaximum()
    {
      if(Maximum.HasValue)
        Value = Maximum.Value;
    }

    #endregion Commands

    #region Automation

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return new BigIntegerUpDownAutomationPeer(this);
    }

    #endregion Automation
  }

  public class BigIntegerUpDownAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
  {
    public BigIntegerUpDownAutomationPeer(BigIntegerUpDown control)
      : base(control)
    {
    }

    protected override string GetClassNameCore()
    {
      return "BigIntegerUpDown";
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Spinner;
    }

    public override object GetPattern(PatternInterface patternInterface)
    {
      if (patternInterface == PatternInterface.RangeValue)
      {
        return this;
      }
      return base.GetPattern(patternInterface);
    }

    internal void RaiseValueChangedEvent(BigInteger oldValue, BigInteger newValue)
    {
      base.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty,
          (double)oldValue, (double)newValue);
    }

    #region IRangeValueProvider Members

    bool IRangeValueProvider.IsReadOnly
    {
      get
      {
        return !IsEnabled();
      }
    }

    double IRangeValueProvider.LargeChange
    {
      get { return (double)MyOwner.Change; }
    }

    double IRangeValueProvider.Maximum
    {
      get { return (double)MyOwner.Maximum; }
    }

    double IRangeValueProvider.Minimum
    {
      get { return (double)MyOwner.Minimum; }
    }

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

    double IRangeValueProvider.SmallChange
    {
      get { return (double)MyOwner.Change; }
    }

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
