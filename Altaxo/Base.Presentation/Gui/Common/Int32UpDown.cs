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
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
  public class Int32UpDown : NumericUpDownBase
  {
    private const int DefaultMinValue = int.MinValue;
    private const int DefaultValue = 0;
    private const int DefaultMaxValue = int.MaxValue;
    private const int DefaultChange = 1;

    #region Converter

    protected override object GetNewValidationRuleAndConverter()
    {
      return new IntegerUpDownConverter(this);
    }

    protected class IntegerUpDownConverter : ValidationRule, IValueConverter
    {
      private Int32UpDown _parent;
      private System.Globalization.CultureInfo _conversionCulture = Altaxo.Settings.GuiCulture.Instance;

      public IntegerUpDownConverter()
      {
      }

      public IntegerUpDownConverter(Int32UpDown parent)
      {
        _parent = parent;
      }

      public object Convert(object obj, Type targetType, object parameter, CultureInfo cultureBuggyDontUse)
      {
        int val = (int)obj;

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
          _parent.SetValue(ValueStringPropertyKey, s); // we set the value string property to have the actual text value as a property
          s = s.Trim();

          if (!string.IsNullOrEmpty(_parent.MinimumReplacementText) && _parent.MinimumReplacementText.Trim() == s)
            return _parent.Minimum;
          else if (!string.IsNullOrEmpty(_parent.MaximumReplacementText) && _parent.MaximumReplacementText.Trim() == s)
            return _parent.Maximum;
          else if (string.IsNullOrEmpty(s) && _parent.ValueIfTextIsEmpty is not null)
            return _parent.ValueIfTextIsEmpty;
        }

        if (int.TryParse(s, NumberStyles.Integer, _conversionCulture, out var result))
        {
          return result;
        }
        else
        {
          validationResult = new ValidationResult(false, string.Format("The provided string could not be converted to an integer value!"));
          return System.Windows.Data.Binding.DoNothing;
        }
      }
    }

    #endregion Converter

    #region Properties

    #region Value

    public int Value
    {
      get { return (int)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    /// <summary>
    /// Identifies the Value dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            "Value", typeof(int), typeof(Int32UpDown),
            new FrameworkPropertyMetadata(DefaultValue,
                new PropertyChangedCallback(OnValueChanged),
                new CoerceValueCallback(CoerceValue)
            )
        );

    private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var control = (Int32UpDown)obj;

      int oldValue = (int)args.OldValue;
      int newValue = (int)args.NewValue;

      #region Fire Automation events

      var peer = UIElementAutomationPeer.FromElement(control) as IntegerUpDownAutomationPeer;
      if (peer is not null)
      {
        peer.RaiseValueChangedEvent(oldValue, newValue);
      }

      #endregion Fire Automation events

      var e = new RoutedPropertyChangedEventArgs<int>(
          oldValue, newValue, ValueChangedEvent);

      control.OnValueChanged(e);
    }

    /// <summary>
    /// Raises the ValueChanged event.
    /// </summary>
    /// <param name="args">Arguments associated with the ValueChanged event.</param>
    protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<int> args)
    {
      RaiseEvent(args);
    }

    private static object CoerceValue(DependencyObject element, object value)
    {
      int newValue = (int)value;
      var control = (Int32UpDown)element;

      newValue = Math.Max(control.Minimum, Math.Min(control.Maximum, newValue));

      return newValue;
    }

    #endregion Value

    #region ValueIfTextIsEmpty

    public int? ValueIfTextIsEmpty
    {
      get { return (int?)GetValue(ValueIfTextIsEmptyProperty); }
      set { SetValue(ValueIfTextIsEmptyProperty, value); }
    }

    /// <summary>
    /// Identifies the Value dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueIfTextIsEmptyProperty =
        DependencyProperty.Register(
            "ValueIfTextIsEmpty", typeof(int?), typeof(Int32UpDown)
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
        DependencyProperty.RegisterAttachedReadOnly("ValueString", typeof(string), typeof(Int32UpDown), new PropertyMetadata());

    public static readonly DependencyProperty ValueStringProperty = ValueStringPropertyKey.DependencyProperty;

    private NumberFormatInfo _numberFormatInfo = new NumberFormatInfo();

    #endregion ValueString

    #region Minimum

    public int Minimum
    {
      get { return (int)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(
            "Minimum", typeof(int), typeof(Int32UpDown),
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
      int minimum = (int)value;
      var control = (Int32UpDown)element;
      return minimum;
    }

    #endregion Minimum

    #region Maximum

    public int Maximum
    {
      get { return (int)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(
            "Maximum", typeof(int), typeof(Int32UpDown),
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
      var control = (Int32UpDown)element;
      int newMaximum = (int)value;
      return Math.Max(newMaximum, control.Minimum);
    }

    #endregion Maximum

    #region Change

    public int Change
    {
      get { return (int)GetValue(ChangeProperty); }
      set { SetValue(ChangeProperty, value); }
    }

    public static readonly DependencyProperty ChangeProperty =
        DependencyProperty.Register(
            "Change", typeof(int), typeof(Int32UpDown),
            new FrameworkPropertyMetadata(DefaultChange, new PropertyChangedCallback(OnChangeChanged), new CoerceValueCallback(CoerceChange)),
        new ValidateValueCallback(ValidateChange)
        );

    private static bool ValidateChange(object value)
    {
      int change = (int)value;
      return change > 0;
    }

    private static void OnChangeChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
    }

    private static object CoerceChange(DependencyObject element, object value)
    {
      int newChange = (int)value;
      var control = (Int32UpDown)element;

      return newChange;
    }

    #endregion Change

    #endregion Properties

    #region Events

    /// <summary>
    /// Identifies the ValueChanged routed event.
    /// </summary>
    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
        "ValueChanged", RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<int>), typeof(Int32UpDown));

    /// <summary>
    /// Occurs when the Value property changes.
    /// </summary>
    public event RoutedPropertyChangedEventHandler<int> ValueChanged
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
      if (Value <= (int.MaxValue - Change))
        Value += Change;
      else
        Value = int.MaxValue;
    }

    protected override void OnDecrease()
    {
      // avoid an underflow before coerce of the value
      if (Value >= (int.MinValue + Change))
        Value -= Change;
      else
        Value = int.MinValue;
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
      return new IntegerUpDownAutomationPeer(this);
    }

    #endregion Automation
  }

  public class IntegerUpDownAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
  {
    public IntegerUpDownAutomationPeer(Int32UpDown control)
      : base(control)
    {
    }

    protected override string GetClassNameCore()
    {
      return "Int32UpDown";
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

    internal void RaiseValueChangedEvent(int oldValue, int newValue)
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
      get { return MyOwner.Change; }
    }

    double IRangeValueProvider.Maximum
    {
      get { return MyOwner.Maximum; }
    }

    double IRangeValueProvider.Minimum
    {
      get { return MyOwner.Minimum; }
    }

    void IRangeValueProvider.SetValue(double value)
    {
      if (!IsEnabled())
      {
        throw new ElementNotEnabledException();
      }

      int val = (int)value;
      if (val < MyOwner.Minimum || val > MyOwner.Maximum)
      {
        throw new ArgumentOutOfRangeException("value");
      }

      MyOwner.Value = val;
    }

    double IRangeValueProvider.SmallChange
    {
      get { return MyOwner.Change; }
    }

    double IRangeValueProvider.Value
    {
      get { return MyOwner.Value; }
    }

    #endregion IRangeValueProvider Members

    private Int32UpDown MyOwner
    {
      get
      {
        return (Int32UpDown)base.Owner;
      }
    }
  }
}
