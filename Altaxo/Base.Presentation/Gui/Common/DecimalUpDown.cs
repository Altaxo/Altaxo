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
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
  public class DecimalUpDown : NumericUpDownBase
  {
    private const decimal DefaultMinValue = 0;
    private const decimal DefaultValue = DefaultMinValue;
    private const decimal DefaultMaxValue = 100;
    private const decimal DefaultChange = 1;
    private const int DefaultDecimalPlaces = 0;

    #region Converter

    protected override object GetNewValidationRuleAndConverter()
    {
      return new DecimalUpDownConverter(this);
    }

    protected class DecimalUpDownConverter : ValidationRule, IValueConverter
    {
      private DecimalUpDown _parent;
      private System.Globalization.CultureInfo _conversionCulture = Altaxo.Settings.GuiCulture.Instance;

      public DecimalUpDownConverter()
      {
      }

      public DecimalUpDownConverter(DecimalUpDown parent)
      {
        _parent = parent;
      }

      public object Convert(object obj, Type targetType, object parameter, CultureInfo cultureBuggyDontUse)
      {
        decimal val = (decimal)obj;

        if (_parent is not null)
        {
          if (val == _parent.Minimum && _parent.MinimumReplacementText is not null)
            return _parent.MinimumReplacementText;
          if (val == _parent.Maximum && _parent.MaximumReplacementText is not null)
            return _parent.MaximumReplacementText;
        }

        return val.ToString(_conversionCulture);
      }

      public object ConvertBack(object obj, Type targetType, object parameter, CultureInfo cultureBuggyDontUse)
      {
        return ConvertBack(obj, targetType, parameter, out var validationResult);
      }

      public override ValidationResult Validate(object obj, CultureInfo cultureInfoBuggyDontUse)
      {
        ConvertBack(obj, null, null, out var validationResult);
        return validationResult;
      }

      public object ConvertBack(object obj, Type targetType, object parameter, out ValidationResult validationResult)
      {
        validationResult = ValidationResult.ValidResult;

        string s = (string)obj;

        if (_parent is not null)
        {
          _parent.SetValue(ValueStringPropertyKey, s);
          s = s.Trim();

          if (!string.IsNullOrEmpty(_parent.MinimumReplacementText) && _parent.MinimumReplacementText.Trim() == s)
            return _parent.Minimum;
          else if (!string.IsNullOrEmpty(_parent.MaximumReplacementText) && _parent.MaximumReplacementText.Trim() == s)
            return _parent.Maximum;
          else if (string.IsNullOrEmpty(s) && _parent.ValueIfTextIsEmpty is not null)
            return _parent.ValueIfTextIsEmpty;
        }

        if (decimal.TryParse(s, System.Globalization.NumberStyles.Number, _conversionCulture, out var result))
          return result;
        else
        {
          validationResult = new ValidationResult(false, string.Format("The provided string could not be converted to a numeric value, parent is {0}", _parent is null ? "Null" : "Set"));
          return System.Windows.Data.Binding.DoNothing;
        }
      }
    }

    #endregion Converter

    #region Properties

    #region Value

    public decimal Value
    {
      get { return (decimal)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    public double ValueAsDouble
    {
      get { return (double)(decimal)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, (decimal)value); }
    }

    /// <summary>
    /// Identifies the Value dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            "Value", typeof(decimal), typeof(DecimalUpDown),
            new FrameworkPropertyMetadata(DefaultValue,
                new PropertyChangedCallback(OnValueChanged),
                new CoerceValueCallback(CoerceValue)
            )
        );

    private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var control = (DecimalUpDown)obj;

      decimal oldValue = (decimal)args.OldValue;
      decimal newValue = (decimal)args.NewValue;

      #region Fire Automation events

      var peer = UIElementAutomationPeer.FromElement(control) as DecimalUpDownAutomationPeer;
      if (peer is not null)
      {
        peer.RaiseValueChangedEvent(oldValue, newValue);
      }

      #endregion Fire Automation events

      var e = new RoutedPropertyChangedEventArgs<decimal>(
          oldValue, newValue, ValueChangedEvent);

      control.OnValueChanged(e);
    }

    /// <summary>
    /// Raises the ValueChanged event.
    /// </summary>
    /// <param name="args">Arguments associated with the ValueChanged event.</param>
    protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<decimal> args)
    {
      RaiseEvent(args);
    }

    private static object CoerceValue(DependencyObject element, object value)
    {
      decimal newValue = (decimal)value;
      var control = (DecimalUpDown)element;

      newValue = Math.Max(control.Minimum, Math.Min(control.Maximum, newValue));
      newValue = decimal.Round(newValue, control.DecimalPlaces);

      return newValue;
    }

    #endregion Value

    #region ValueIfTextIsEmpty

    public decimal? ValueIfTextIsEmpty
    {
      get { return (decimal?)GetValue(ValueIfTextIsEmptyProperty); }
      set { SetValue(ValueIfTextIsEmptyProperty, value); }
    }

    /// <summary>
    /// Identifies the Value dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueIfTextIsEmptyProperty =
        DependencyProperty.Register(
            "ValueIfTextIsEmpty", typeof(decimal?), typeof(DecimalUpDown)
    );

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
        DependencyProperty.RegisterAttachedReadOnly("ValueString", typeof(string), typeof(DecimalUpDown), new PropertyMetadata());

    public static readonly DependencyProperty ValueStringProperty = ValueStringPropertyKey.DependencyProperty;

    #endregion ValueString

    #region Minimum

    public decimal Minimum
    {
      get { return (decimal)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(
            "Minimum", typeof(decimal), typeof(DecimalUpDown),
            new FrameworkPropertyMetadata(DefaultMinValue,
                new PropertyChangedCallback(OnMinimumChanged), new CoerceValueCallback(CoerceMinimum)
            )
        );

    private static void OnMinimumChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
      element.CoerceValue(MaximumProperty);
      element.CoerceValue(ValueProperty);
    }

    private static object CoerceMinimum(DependencyObject element, object value)
    {
      decimal minimum = (decimal)value;
      var control = (DecimalUpDown)element;
      return decimal.Round(minimum, control.DecimalPlaces);
    }

    #endregion Minimum

    #region Maximum

    public decimal Maximum
    {
      get { return (decimal)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(
            "Maximum", typeof(decimal), typeof(DecimalUpDown),
            new FrameworkPropertyMetadata(DefaultMaxValue,
                new PropertyChangedCallback(OnMaximumChanged),
                new CoerceValueCallback(CoerceMaximum)
            )
        );

    private static void OnMaximumChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
      element.CoerceValue(ValueProperty);
    }

    private static object CoerceMaximum(DependencyObject element, object value)
    {
      var control = (DecimalUpDown)element;
      decimal newMaximum = (decimal)value;
      return decimal.Round(Math.Max(newMaximum, control.Minimum), control.DecimalPlaces);
    }

    #endregion Maximum

    #region Change

    public decimal Change
    {
      get { return (decimal)GetValue(ChangeProperty); }
      set { SetValue(ChangeProperty, value); }
    }

    public static readonly DependencyProperty ChangeProperty =
        DependencyProperty.Register(
            "Change", typeof(decimal), typeof(DecimalUpDown),
            new FrameworkPropertyMetadata(DefaultChange, new PropertyChangedCallback(OnChangeChanged), new CoerceValueCallback(CoerceChange)),
        new ValidateValueCallback(ValidateChange)
        );

    private static bool ValidateChange(object value)
    {
      decimal change = (decimal)value;
      return change > 0;
    }

    private static void OnChangeChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
    }

    private static object CoerceChange(DependencyObject element, object value)
    {
      decimal newChange = (decimal)value;
      var control = (DecimalUpDown)element;

      decimal coercedNewChange = decimal.Round(newChange, control.DecimalPlaces);

      //If Change is .1 and DecimalPlaces is changed from 1 to 0, we want Change to go to 1, not 0.
      //Put another way, Change should always be rounded to DecimalPlaces, but never smaller than the
      //previous Change
      if (coercedNewChange < newChange)
      {
        coercedNewChange = smallestForDecimalPlaces(control.DecimalPlaces);
      }

      return coercedNewChange;
    }

    private static decimal smallestForDecimalPlaces(int decimalPlaces)
    {
      if (decimalPlaces < 0)
      {
        throw new ArgumentException("decimalPlaces");
      }

      decimal d = 1;

      for (int i = 0; i < decimalPlaces; i++)
      {
        d /= 10;
      }

      return d;
    }

    #endregion Change

    #region DecimalPlaces

    public int DecimalPlaces
    {
      get { return (int)GetValue(DecimalPlacesProperty); }
      set { SetValue(DecimalPlacesProperty, value); }
    }

    public static readonly DependencyProperty DecimalPlacesProperty =
        DependencyProperty.Register(
            "DecimalPlaces", typeof(int), typeof(DecimalUpDown),
            new FrameworkPropertyMetadata(DefaultDecimalPlaces,
                new PropertyChangedCallback(OnDecimalPlacesChanged)
            ), new ValidateValueCallback(ValidateDecimalPlaces)
        );

    private static void OnDecimalPlacesChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
      var control = (DecimalUpDown)element;
      control.CoerceValue(ChangeProperty);
      control.CoerceValue(MinimumProperty);
      control.CoerceValue(MaximumProperty);
      control.CoerceValue(ValueProperty);
    }

    private static bool ValidateDecimalPlaces(object value)
    {
      int decimalPlaces = (int)value;
      return decimalPlaces >= 0;
    }

    #endregion DecimalPlaces

    #endregion Properties

    #region Events

    /// <summary>
    /// Identifies the ValueChanged routed event.
    /// </summary>
    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
        "ValueChanged", RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<decimal>), typeof(DecimalUpDown));

    /// <summary>
    /// Occurs when the Value property changes.
    /// </summary>
    public event RoutedPropertyChangedEventHandler<decimal> ValueChanged
    {
      add { AddHandler(ValueChangedEvent, value); }
      remove { RemoveHandler(ValueChangedEvent, value); }
    }

    #endregion Events

    #region Commands

    protected override void OnIncrease()
    {
      // avoid an overflow before coerce of the value
      var val = Value;
      if (Value <= (decimal.MaxValue - Change))
        Value += Change;
      else
        Value = decimal.MaxValue;
    }

    protected override void OnDecrease()
    {
      // avoid an underflow before coerce of the value
      if (Value >= (decimal.MinValue + Change))
        Value -= Change;
      else
        Value = decimal.MinValue;
    }

    protected override void OnGotoMinimum()
    {
      Value = Minimum;
    }

    protected override void OnGotoMaximum()
    {
      Value = Maximum;
    }

    #endregion Commands

    #region Automation

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return new DecimalUpDownAutomationPeer(this);
    }

    #endregion Automation
  }

  public class DecimalUpDownAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
  {
    public DecimalUpDownAutomationPeer(DecimalUpDown control)
      : base(control)
    {
    }

    protected override string GetClassNameCore()
    {
      return "DecimalUpDown";
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

    internal void RaiseValueChangedEvent(decimal oldValue, decimal newValue)
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

      decimal val = (decimal)value;
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

    private DecimalUpDown MyOwner
    {
      get
      {
        return (DecimalUpDown)base.Owner;
      }
    }
  }
}
