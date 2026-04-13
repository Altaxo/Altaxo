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
  /// <summary>
  /// Updown control for integer values in a continuous range.
  /// </summary>
  public class Int32UpDown : NumericUpDownBase
  {
    private const int DefaultMinValue = int.MinValue;
    private const int DefaultValue = 0;
    private const int DefaultMaxValue = int.MaxValue;
    private const int DefaultChange = 1;

    #region Converter

    /// <inheritdoc/>
    protected override object GetNewValidationRuleAndConverter()
    {
      return new IntegerUpDownConverter(this);
    }

    /// <summary>
    /// Converts between integer values and their textual representation for <see cref="Int32UpDown"/>.
    /// </summary>
    protected class IntegerUpDownConverter : ValidationRule, IValueConverter
    {
      private Int32UpDown _parent;
      private System.Globalization.CultureInfo _conversionCulture = Altaxo.Settings.GuiCulture.Instance;

      /// <summary>
      /// Initializes a new instance of the <see cref="IntegerUpDownConverter"/> class.
      /// </summary>
      public IntegerUpDownConverter()
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="IntegerUpDownConverter"/> class.
      /// </summary>
      /// <param name="parent">The owning control.</param>
      public IntegerUpDownConverter(Int32UpDown parent)
      {
        _parent = parent;
      }

      /// <summary>
      /// Converts an integer value to its display text.
      /// </summary>
      /// <param name="value">The source value.</param>
      /// <param name="targetType">The target type.</param>
      /// <param name="parameter">The converter parameter.</param>
      /// <param name="culture">The culture supplied by the binding engine.</param>
      /// <returns>The converted display value.</returns>
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
        int val = (int)value;

        if (_parent is not null)
        {
          if (val == _parent.Minimum && _parent.MinimumReplacementText is not null)
            return _parent.MinimumReplacementText;
          if (val == _parent.Maximum && _parent.MaximumReplacementText is not null)
            return _parent.MaximumReplacementText;
        }

        return val.ToString(_conversionCulture);
      }

      /// <summary>
      /// Converts a display value back to an integer value.
      /// </summary>
      /// <param name="value">The display value.</param>
      /// <param name="targetType">The target type.</param>
      /// <param name="parameter">The converter parameter.</param>
      /// <param name="culture">The culture supplied by the binding engine.</param>
      /// <returns>The converted integer value.</returns>
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
        return ConvertBack(value, targetType, parameter, out var validationResult);
      }

      /// <inheritdoc/>
      public override ValidationResult Validate(object value, CultureInfo cultureInfo)
      {
        ConvertBack(value, null, null, out var validationResult);
        return validationResult;
      }

      /// <summary>
      /// Converts a display value back to an integer value and returns validation information.
      /// </summary>
      /// <param name="obj">The display value.</param>
      /// <param name="targetType">The target type.</param>
      /// <param name="parameter">The converter parameter.</param>
      /// <param name="validationResult">Receives the validation result.</param>
      /// <returns>The converted integer value or <see cref="Binding.DoNothing"/>.</returns>
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

    /// <summary>
    /// Gets or sets the selected value.
    /// </summary>
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
            { BindsTwoWayByDefault = true }
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

    /// <summary>
    /// Gets or sets the value that is used when the text box is empty.
    /// </summary>
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

    /// <summary>
    /// Gets the current text representation of the value.
    /// </summary>
    public string ValueString
    {
      get
      {
        return (string)GetValue(ValueStringProperty);
      }
    }

    private static readonly DependencyPropertyKey ValueStringPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly("ValueString", typeof(string), typeof(Int32UpDown), new PropertyMetadata());

    /// <summary>
    /// Identifies the <see cref="ValueString"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueStringProperty = ValueStringPropertyKey.DependencyProperty;

    private NumberFormatInfo _numberFormatInfo = new NumberFormatInfo();

    #endregion ValueString

    #region Minimum

    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    public int Minimum
    {
      get { return (int)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Minimum"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(
            "Minimum", typeof(int), typeof(Int32UpDown),
            new FrameworkPropertyMetadata(DefaultMinValue,
                new PropertyChangedCallback(OnMinimumChanged), new CoerceValueCallback(CoerceMinimum)
            )
        );

    private static void OnMinimumChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
      element.CoerceValue(ValueProperty);
    }

    private static object CoerceMinimum(DependencyObject element, object value)
    {
      var control = (Int32UpDown)element;
      int newMinimum = (int)value;
      return Math.Min(newMinimum, control.Maximum);
    }

    #endregion Minimum

    #region Maximum

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    public int Maximum
    {
      get { return (int)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Maximum"/> dependency property.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the increment or decrement applied by the spinner commands.
    /// </summary>
    public int Change
    {
      get { return (int)GetValue(ChangeProperty); }
      set { SetValue(ChangeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Change"/> dependency property.
    /// </summary>
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

    /// <inheritdoc/>
    protected override void OnIncrease()
    {
      // avoid an overflow before coerce of the value
      var val = Value;
      if (Value <= (int.MaxValue - Change))
        Value += Change;
      else
        Value = int.MaxValue;
    }

    /// <inheritdoc/>
    protected override bool OnIncreaseCanExecute()
    {
      return Value < Maximum;
    }

    /// <inheritdoc/>
    protected override void OnDecrease()
    {
      // avoid an underflow before coerce of the value
      if (Value >= (int.MinValue + Change))
        Value -= Change;
      else
        Value = int.MinValue;
    }

    /// <inheritdoc/>
    protected override bool OnDecreaseCanExecute()
    {
      return Value > Minimum;
    }

    /// <inheritdoc/>
    protected override void OnGotoMinimum()
    {
      Value = Minimum;
    }

    /// <inheritdoc/>
    protected override void OnGotoMaximum()
    {
      Value = Maximum;
    }

    #endregion Commands

    #region Automation

    /// <inheritdoc/>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return new IntegerUpDownAutomationPeer(this);
    }

    #endregion Automation
  }

  /// <summary>
  /// Automation peer for <see cref="Int32UpDown"/>.
  /// </summary>
  public class IntegerUpDownAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="IntegerUpDownAutomationPeer"/> class.
    /// </summary>
    /// <param name="control">The owning control.</param>
    public IntegerUpDownAutomationPeer(Int32UpDown control)
      : base(control)
    {
    }

    /// <inheritdoc/>
    protected override string GetClassNameCore()
    {
      return "Int32UpDown";
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
