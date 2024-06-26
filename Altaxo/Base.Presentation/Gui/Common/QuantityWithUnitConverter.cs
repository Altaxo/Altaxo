﻿#region Copyright

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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Altaxo.Units;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Converts a <see cref="DimensionfulQuantity"/> to a string and vice versa. Additionaly, this class can act as validation rule for the strings entered.
  /// </summary>
  public class QuantityWithUnitConverter : ValidationRule, IValueConverter
  {
    private QuantityWithUnitGuiEnvironment _unitEnvironment;
    private BindingExpressionBase _binding;

    private System.Globalization.CultureInfo _conversionCulture = Altaxo.Settings.GuiCulture.Instance;

    private DimensionfulQuantity? _lastConvertedQuantity;
    private string _lastConvertedString;

    /// <summary>Used for the context menu helpers only.</summary>
    private FrameworkElement _parent;

    /// <summary>Used for the context menu helpers only.</summary>
    private DependencyProperty _quantityGetSetProperty;

    private WeakEventHandler _defaultUnitChangedHandler;
    private WeakEventHandler _numberOfDisplayedDigitsChangedHandler;

    private bool _disallowNegativeValues;

    public bool DisallowNegativeValues
    {
      get { return _disallowNegativeValues; }
      set { _disallowNegativeValues = value; }
    }

    private bool _disallowZeroValue;

    public bool DisallowZeroValues
    {
      get { return _disallowZeroValue; }
      set { _disallowZeroValue = value; }
    }

    private bool _allowInfinity = true;

    public bool AllowInfiniteValues
    {
      get { return _allowInfinity; }
      set { _allowInfinity = value; }
    }

    private bool _allowNaN;

    public bool AllowNaNValues
    {
      get { return _allowNaN; }
      set { _allowNaN = value; }
    }

    private string _representationOfNaN;

    public string RepresentationOfNaN
    {
      get { return _representationOfNaN; }
      set { _representationOfNaN = value; }
    }

    /// <summary>
    /// Empty constructor. You should set as soon as possible the <see cref="BindingExpression"/> to the binding expression between your QuantityWithUnit property and the
    /// Text property of your TextBox, ComboBox, or other Gui element. Furthermore, the <see cref="UnitEnvironment"/> property should be set
    /// to a valid unit environment.
    /// </summary>
    public QuantityWithUnitConverter()
    {
      try
      {
        _conversionCulture = Altaxo.Settings.GuiCulture.Instance;
      }
      catch (Exception)
      {
        _conversionCulture = System.Globalization.CultureInfo.CurrentUICulture;
      }
    }

    /// <summary>
    /// Can be called if the context menu of the gui element is about to be opened. Extends the context menu by additional menu items
    /// for unit conversion, and for the setting of the number of decimal places.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="quantityGetSetProperty"></param>
    public QuantityWithUnitConverter(FrameworkElement parent, DependencyProperty quantityGetSetProperty)
    {
      try
      {
        _conversionCulture = Altaxo.Settings.GuiCulture.Instance;
      }
      catch (Exception)
      {
        _conversionCulture = System.Globalization.CultureInfo.CurrentUICulture;
      }

      _parent = parent;
      _quantityGetSetProperty = quantityGetSetProperty;
    }

    /// <summary>
    /// Get/sets the unit environment used for this converter. It determines, which units are allowed to be entered, which is the default unit, and how many
    /// decimal digits are shown in the box by default.
    /// </summary>
    public QuantityWithUnitGuiEnvironment UnitEnvironment
    {
      get
      {
        return _unitEnvironment;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException();

        if (_unitEnvironment is not null)
        {
          _defaultUnitChangedHandler.Remove();
          _numberOfDisplayedDigitsChangedHandler.Remove();
        }

        _unitEnvironment = value;

        if (_unitEnvironment is not null)
        {
          var unitEnvironment = _unitEnvironment;
          unitEnvironment.DefaultUnitChanged += _defaultUnitChangedHandler = new WeakEventHandler(EhDefaultUnitChanged, unitEnvironment, nameof(unitEnvironment.DefaultUnitChanged));
          unitEnvironment.NumberOfDisplayedDigitsChanged += _numberOfDisplayedDigitsChangedHandler = new WeakEventHandler(EhNumberOfDisplayedDigitsChanged, unitEnvironment, nameof(unitEnvironment.NumberOfDisplayedDigitsChanged));
          EhNumberOfDisplayedDigitsChanged(); // will update the target, e.g. if the UnitEnvironment was set later than the Quantity
        }
      }
    }

    /// <summary>
    /// Sets the binding expression between the property with gets/sets a QuantityWithUnit and the
    /// Text property of your TextBox, ComboBox, or other Gui element.
    /// </summary>
    public BindingExpressionBase BindingExpression
    {
      get
      {
        return _binding;
      }
      set
      {
        _binding = value;
      }
    }

    private void EhDefaultUnitChanged(object? sender, EventArgs e)
    {
      if (_parent is not null && _quantityGetSetProperty is not null && _unitEnvironment is not null)
      {
        var selectedQuantity = SelectedQuantity;
        if (!selectedQuantity.IsEmpty)
          SelectedQuantity = selectedQuantity.AsQuantityIn(_unitEnvironment.DefaultUnit);
      }
    }

    private void EhNumberOfDisplayedDigitsChanged(object? sender, EventArgs e)
    {
      EhNumberOfDisplayedDigitsChanged();
    }

    public void ClearIntermediateConversionResults()
    {
      _lastConvertedQuantity = null;
      _lastConvertedString = null;
    }

    /// <summary>
    /// Converts from a <see cref="DimensionfulQuantity"/> to a string.
    /// </summary>
    /// <param name="value">The value to convert. Has to be a <see cref="DimensionfulQuantity"/>.</param>
    /// <param name="targetType">Ignored.</param>
    /// <param name="parameter">Ignored.</param>
    /// <param name="cultureDontUseIsBuggy">This parameter is not used. Instead, the current Altaxo Gui culture is used.</param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureDontUseIsBuggy)
    {
      var q = (DimensionfulQuantity)value;

      if (q.IsEmpty)
        return string.Empty;

      if (_lastConvertedQuantity is not null && q.IsEqualInValuePrefixUnit(((DimensionfulQuantity)_lastConvertedQuantity)))
      {
        return _lastConvertedString;
      }
      else
      {
        string result;

        if (double.IsNaN(q.Value) && _representationOfNaN is not null) // is a NaN for which we have a representation
        {
          result = _representationOfNaN;
        }
        else // Not a NaN with a representation
        {
          if (_unitEnvironment is not null)
            result = q.Value.ToString("G" + _unitEnvironment.NumberOfDisplayedDigits.ToString(), _conversionCulture); // bug: do not use culture parameter here, it is sometimes different from CurrentUICulture
          else
            result = q.Value.ToString(_conversionCulture);

          string end = q.Prefix.ShortCut + q.Unit.ShortCut;
          if (!string.IsNullOrEmpty(end))
            result += " " + end;
        }

        _lastConvertedQuantity = q;
        _lastConvertedString = result;
        return result;
      }
    }

    /// <summary>
    /// Converts a string to a <see cref="DimensionfulQuantity"/>.
    /// </summary>
    /// <param name="value">String to convert.</param>
    /// <param name="targetType">Ignored.</param>
    /// <param name="parameter">Ignored.</param>
    /// <param name="cultureDontUseIsBuggy">This parameter is not used. Instead, culture information is retrieved from Altaxo's settings.</param>
    /// <returns>The converted quantity.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureDontUseIsBuggy)
    {
      string s = (string)value;
      var result = ConvertValidate(s, out var q); // do not use culture parameter here, it is sometimes different from UICulture
      if (result.IsValid)
      {
        _lastConvertedString = s;
        _lastConvertedQuantity = q;
      }
      return q;
    }

    /// <summary>
    /// Validates a string, whether or not it can be converted to a <see cref="DimensionfulQuantity"/>.
    /// </summary>
    /// <param name="value">String value to validate.</param>
    /// <param name="cultureDontUseIsBuggy">This parameter is not used. Instead, culture information is retrieved from Altaxo's settings.</param>
    /// <returns>A validation result depending on the result of the validation.</returns>
    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureDontUseIsBuggy)
    {
      if (value is string s)
      {
        var result = ConvertValidate(s, out var q);
        if (result.IsValid)
        {
          _lastConvertedString = s;
          _lastConvertedQuantity = q;
        }
        return result;
      }
      else
      {
        return new ValidationResult(false, "Provided value is not a string!");
      }
    }

    private ValidationResult ConvertValidate(string s, out DimensionfulQuantity result)
    {
      if (_lastConvertedQuantity is not null && s == _lastConvertedString)
      {
        result = (DimensionfulQuantity)_lastConvertedQuantity;
        return ValidateSuccessfullyConvertedQuantity(result);
      }

      result = new DimensionfulQuantity();
      double parsedValue;
      SIPrefix prefix = null;
      s = s.Trim();

      if (_allowNaN && _representationOfNaN is not null && s.Trim() == _representationOfNaN.Trim())
      {
        result = new DimensionfulQuantity(double.NaN, SIPrefix.None, _lastConvertedQuantity.HasValue ? _lastConvertedQuantity.Value.Unit : _unitEnvironment.DefaultUnit.Unit);
        return ValidateSuccessfullyConvertedQuantity(result);
      }

      if (string.IsNullOrEmpty(s))
      {
        return new ValidationResult(false, "The text box is empty. You have to enter a valid numeric quantity");
      }

      foreach (IUnit u in _unitEnvironment.UnitsSortedByShortcutLengthDescending)
      {
        if (string.IsNullOrEmpty(u.ShortCut) || (!s.EndsWith(u.ShortCut)))
          continue;

        s = s.Substring(0, s.Length - u.ShortCut.Length);

        if (!u.Prefixes.ContainsNonePrefixOnly && s.Length > 0)
        {
          // try first prefixes of bigger length, then of smaller length
          for (int pl = SIPrefix.MaxShortCutLength; pl > 0; --pl)
          {
            if (s.Length < pl)
              continue;
            prefix = SIPrefix.TryGetPrefixFromShortcut(s.Substring(s.Length - pl));
            if (prefix is not null)
            {
              s = s.Substring(0, s.Length - prefix.ShortCut.Length);
              break;
            }
          }
        }

        if (double.TryParse(s, System.Globalization.NumberStyles.Float, _conversionCulture, out parsedValue))
        {
          result = new DimensionfulQuantity(parsedValue, prefix, u);
          return ValidateSuccessfullyConvertedQuantity(result);
        }
        else
        {
          string firstPart;
          if (prefix is not null)
            firstPart = string.Format("The last part \"{0}\" of your entered text is recognized as prefixed unit, ", prefix.ShortCut + u.ShortCut);
          else
            firstPart = string.Format("The last part \"{0}\" of your entered text is recognized as unit, ", u.ShortCut);

          string lastPart;
          if (string.IsNullOrEmpty(s.Trim()))
            lastPart = string.Format("but the first part is empty. You have to prepend a numeric value!");
          else
            lastPart = string.Format("but the first part \"{0}\" is not recognized as a numeric value!", s);

          return new ValidationResult(false, firstPart + lastPart);
        }
      }

      // if nothing is found in this way, we try to split the text
      var parts = s.Split(new char[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);

      // try to parse the first part as a number
      if (!double.TryParse(parts[0], System.Globalization.NumberStyles.Float, _conversionCulture, out parsedValue))
        return new ValidationResult(false, string.Format("The part \"{0}\" of your entered text was not recognized as a numeric value.", parts[0]));

      string unitString = parts.Length >= 2 ? parts[1] : string.Empty;

      if (string.IsNullOrEmpty(unitString))
      {
        if (_lastConvertedQuantity is not null)
        {
          result = new DimensionfulQuantity(parsedValue, ((DimensionfulQuantity)_lastConvertedQuantity).Prefix, ((DimensionfulQuantity)_lastConvertedQuantity).Unit);
          return ValidateSuccessfullyConvertedQuantity(result);
        }
        else if (_unitEnvironment is not null && _unitEnvironment.DefaultUnit is not null)
        {
          result = new DimensionfulQuantity(parsedValue, _unitEnvironment.DefaultUnit.Prefix, _unitEnvironment.DefaultUnit.Unit);
          return ValidateSuccessfullyConvertedQuantity(result);
        }
        else
        {
          return new ValidationResult(false, "No unit was given by you and no default unit could be deduced from the environment!");
        }
      }
      else
      {
        return new ValidationResult(false, GetErrorStringForUnrecognizedUnit(unitString));
      }
    }

    private Func<DimensionfulQuantity, ValidationResult> _validationAfterSuccessfulConversion;

    /// <summary>Sets a function that validates the quantity after successful conversion. Such a function can be used to exclude certain values, for instance zero, by marking them as invalid.</summary>
    /// <value>The function to validate the quantity after successful conversion. This function has to return a <see cref="ValidationResult"/>.</value>
    public Func<DimensionfulQuantity, ValidationResult> ValidationAfterSuccessfulConversion
    {
      set
      {
        _validationAfterSuccessfulConversion = value;
      }
    }

    /// <summary>Validates the successfully converted quantity. For instance, for some values (e.g. scale values) a value of zero is not appropriate.</summary>
    /// <returns>The result of the validation</returns>
    private ValidationResult ValidateSuccessfullyConvertedQuantity(DimensionfulQuantity value)
    {
      if (_validationAfterSuccessfulConversion is not null)
      {
        return _validationAfterSuccessfulConversion(value);
      }
      else
      {
        if (_disallowNegativeValues && value.Value < 0)
          return new ValidationResult(false, "A negative value is not allowed here");
        if (_disallowZeroValue && value.Value == 0)
          return new ValidationResult(false, "A zero value is not allowed here");
        if (!_allowInfinity && double.IsInfinity(value.Value))
          return new ValidationResult(false, "An infinite value is not allowed here");
        if (!_allowNaN && double.IsNaN(value.Value))
          return new ValidationResult(false, "A NaN value ('Not a Number') is not allowed here");

        return ValidationResult.ValidResult;
      }
    }

    private string GetErrorStringForUnrecognizedUnit(string unrecognizedPart)
    {
      var stb = new StringBuilder();
      stb.AppendFormat("The part \"{0}\" of your entered text is not recognized as a valid unit!\n", unrecognizedPart);
      stb.AppendFormat("Valid units are: \n");

      foreach (var u in _unitEnvironment.UnitsSortedByShortcutLengthDescending)
      {
        stb.AppendFormat("{0}\t({1}, {2})\n", u.ShortCut, u.Name, u.Prefixes.ContainsNonePrefixOnly ? "without prefix" : "with prefixes possible");
      }
      return stb.ToString();
    }

    #region Helper functions for supporting context menu

    private DimensionfulQuantity SelectedQuantity
    {
      get { var result = (DimensionfulQuantity)_parent.GetValue(_quantityGetSetProperty); return result; }
      set { _parent.SetValue(_quantityGetSetProperty, value); }
    }

    /// <summary>
    /// Can be called if the context menu of the gui element is about to be opened. Extends the context menu by additional menu items
    /// for unit conversion, and for the setting of the number of decimal places.
    /// </summary>
    public void OnContextMenuOpening()
    {
      BindingExpression.UpdateSource();
      MenuItem menuItem_ConvertTo = null;
      MenuItem menuItem_ChangeUnitTo = null;
      MenuItem menuItem_SetNumberOfDigitsDisplayed = null;
      if (_parent is null)
        return;

      if (_parent.ContextMenu is not null)
      {
        foreach (var item in _parent.ContextMenu.Items)
        {
          if (!(item is FrameworkElement))
            continue;
          var tag = ((FrameworkElement)item).Tag as string;

          if (tag == "TagConvertTo") // The root of the "Convert to.." submenu
            menuItem_ConvertTo = item as MenuItem;
          if (tag == "TagChangeUnitTo")
            menuItem_ChangeUnitTo = item as MenuItem;
          if (tag == "TagSetDigits") // The root of the "Set digits displayed .." submenu
            menuItem_SetNumberOfDigitsDisplayed = item as MenuItem;
        }
      }

      // Clear all previous menu items
      if (menuItem_ConvertTo is not null)
        menuItem_ConvertTo.Items.Clear();
      if (menuItem_ChangeUnitTo is not null)
        menuItem_ChangeUnitTo.Items.Clear();
      if (menuItem_SetNumberOfDigitsDisplayed is not null)
        menuItem_SetNumberOfDigitsDisplayed.Items.Clear();

      // make menues only when there is no validation error
      if (!(bool)_parent.GetValue(Validation.HasErrorProperty))
      {
        // count the units multiplied with the possible prefixes to decided whether to show submenues or not
        int count = 0;
        foreach (var u in _unitEnvironment.UnitsSortedByShortcutLengthDescending)
          count += u.Prefixes.Count;

        bool makeSubMenusForEachUnit = count > 10;

        if (menuItem_ConvertTo is not null)
          CreateSubmenuItems_ConvertTo(menuItem_ConvertTo, makeSubMenusForEachUnit);

        if (menuItem_ChangeUnitTo is not null)
          CreateSubmenuItems_ChangeUnitTo(menuItem_ChangeUnitTo, makeSubMenusForEachUnit);

        if (menuItem_SetNumberOfDigitsDisplayed is not null)
          CreateSubmenuItems_SetNumberOfDigitsDisplayed(menuItem_SetNumberOfDigitsDisplayed);
      }
    }

    /// <summary>
    /// Creates the submenu items for the "Convert to..." submenu.
    /// </summary>
    /// <param name="rootMenuItem">The root menu item.</param>
    /// <param name="makeSubMenusForEachUnit">If set to <c>true</c>, make sub menus for each unit.</param>
    private void CreateSubmenuItems_ConvertTo(MenuItem rootMenuItem, bool makeSubMenusForEachUnit)
    {
      if (makeSubMenusForEachUnit)
      {
        foreach (var u in _unitEnvironment.UnitsSortedByShortcutLengthDescending)
        {
          if (u.Prefixes.Count == 1)
          {
            var p = u.Prefixes.First();
            string header = string.Format("{0}{1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.AsValueIn(p, u), p.ShortCut, u.ShortCut);
            var mi = new MenuItem() { Header = header, Tag = new PrefixedUnit(p, u) };
            mi.Click += EhConvertTo_Click;
            rootMenuItem.Items.Add(mi);
          }
          else
          {
            var main = new MenuItem() { Header = u.Name };
            foreach (var p in u.Prefixes)
            {
              string header = string.Format("{0}{1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.AsValueIn(p, u), p.ShortCut, u.ShortCut);
              var mi = new MenuItem() { Header = header, Tag = new PrefixedUnit(p, u) };
              mi.Click += EhConvertTo_Click;
              rootMenuItem.Items.Add(mi);
            }
            rootMenuItem.Items.Add(main);
          }
        }
      }
      else // do not make submenues for each unit
      {
        foreach (var u in _unitEnvironment.UnitsSortedByShortcutLengthDescending)
        {
          foreach (var p in u.Prefixes)
          {
            string header = string.Format("{0}{1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.AsValueIn(p, u), p.ShortCut, u.ShortCut);
            var mi = new MenuItem() { Header = header, Tag = new PrefixedUnit(p, u) };
            mi.Click += EhConvertTo_Click;
            rootMenuItem.Items.Add(mi);
          }
        }
      }
    }

    private void CreateSubmenuItems_ChangeUnitTo(MenuItem rootMenuItem, bool makeSubMenusForEachUnit)
    {
      if (makeSubMenusForEachUnit)
      {
        foreach (var u in _unitEnvironment.UnitsSortedByShortcutLengthDescending)
        {
          if (u.Prefixes.Count == 1)
          {
            var p = u.Prefixes.First();
            string header = string.Format("{0} {1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.Value, p.ShortCut, u.ShortCut);
            var mi = new MenuItem() { Header = header, Tag = new PrefixedUnit(p, u) };
            mi.Click += EhChangeUnitTo_Click;
            rootMenuItem.Items.Add(mi);
          }
          else
          {
            var main = new MenuItem() { Header = u.Name };
            foreach (var p in u.Prefixes)
            {
              string header = string.Format("{0} {1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.Value, p.ShortCut, u.ShortCut);
              var mi = new MenuItem() { Header = header, Tag = new PrefixedUnit(p, u) };
              mi.Click += EhChangeUnitTo_Click;
              rootMenuItem.Items.Add(mi);
            }
            rootMenuItem.Items.Add(main);
          }
        }
      }
      else // do not make submenues for each unit
      {
        foreach (var u in _unitEnvironment.UnitsSortedByShortcutLengthDescending)
        {
          foreach (var p in u.Prefixes)
          {
            string header = string.Format("{0}{1} ({2} {3}{4})", p.Name, u.Name, SelectedQuantity.Value, p.ShortCut, u.ShortCut);
            var mi = new MenuItem() { Header = header, Tag = new PrefixedUnit(p, u) };
            mi.Click += EhChangeUnitTo_Click;
            rootMenuItem.Items.Add(mi);
          }
        }
      }
    }

    private void CreateSubmenuItems_SetNumberOfDigitsDisplayed(MenuItem rootMenuItem)
    {
      for (int i = 3; i <= 15; i++)
      {
        string header = string.Format("{0}", i);
        var mi = new MenuItem() { Header = header, Tag = i, IsCheckable = true, IsChecked = (_unitEnvironment is not null && i == _unitEnvironment.NumberOfDisplayedDigits) };
        mi.Click += EhSetDigitsTo_Click;
        rootMenuItem.Items.Add(mi);
      }
    }

    private void EhConvertTo_Click(object sender, RoutedEventArgs e)
    {
      var mnu = sender as MenuItem;
      if (mnu is not null && mnu.Tag is PrefixedUnit)
      {
        var prefixedUnit = (PrefixedUnit)mnu.Tag;
        var newQuantity = SelectedQuantity.AsQuantityIn(prefixedUnit);
        SelectedQuantity = newQuantity;
        if (_unitEnvironment is not null)
          _unitEnvironment.DefaultUnit = prefixedUnit;
      }
    }

    private void EhChangeUnitTo_Click(object sender, RoutedEventArgs e)
    {
      var mnu = sender as MenuItem;
      if (mnu is not null && mnu.Tag is PrefixedUnit)
      {
        var prefixedUnit = (PrefixedUnit)mnu.Tag;
        var newQuantity = new DimensionfulQuantity(SelectedQuantity.Value, prefixedUnit.Prefix, prefixedUnit.Unit);
        SelectedQuantity = newQuantity;
        if (_unitEnvironment is not null)
          _unitEnvironment.DefaultUnit = prefixedUnit;
      }
    }

    private void EhSetDigitsTo_Click(object sender, RoutedEventArgs e)
    {
      var mnu = sender as MenuItem;
      if (mnu is not null && mnu.Tag is int)
      {
        int digits = (int)mnu.Tag;
        if (_unitEnvironment is not null)
          _unitEnvironment.NumberOfDisplayedDigits = digits;

        EhNumberOfDisplayedDigitsChanged();
      }
    }

    private void EhNumberOfDisplayedDigitsChanged()
    {
      if (_lastConvertedQuantity is not null && _binding is not null && !_binding.HasError)
      {
        var quant = (DimensionfulQuantity)_lastConvertedQuantity; // save the current quantity
        _lastConvertedQuantity = null; // set the temporarily stored values to null in order to force a full conversion
        _lastConvertedString = null;
        _binding.UpdateTarget();
      }
    }

    #endregion Helper functions for supporting context menu
  }
}
