﻿#region Copyright

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
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace Altaxo.Gui.Common
{
  public class NumericDoubleConverter : ValidationRule, IValueConverter
  {
    public const bool DefaultValue_IsMinValueInclusive = true;
    public const bool DefaultValue_IsMaxValueInclusive = true;
    public const double DefaultValue_MinValue = double.NegativeInfinity;
    public const double DefaultValue_MaxValue = double.PositiveInfinity;

    public bool AllowInfiniteValues { get; set; }

    public bool AllowNaNValues { get; set; }

    public bool DisallowZeroValues { get; set; }

    public bool DisallowNegativeValues
    {
      get
      {
        return (true == IsMinValueInclusive && 0 == MinValue) || MinValue > 0;
      }
      set
      {
        if (MinValue <= 0)
        {
          IsMinValueInclusive = true;
          MinValue = 0;
        }
      }
    }

    public double _minValue = DefaultValue_MinValue;

    public double MinValue { get { return _minValue; } set { _minValue = value; } }

    public bool IsMinValueInclusive { get; set; } = DefaultValue_IsMinValueInclusive;

    public double _maxValue = DefaultValue_MaxValue;

    public double MaxValue { get { return _maxValue; } set { _maxValue = value; } }

    public bool IsMaxValueInclusive { get; set; } = DefaultValue_IsMaxValueInclusive;

    private System.Globalization.CultureInfo _conversionCulture = Altaxo.Settings.GuiCulture.Instance;

    private string _lastConvertedString;
    private double? _lastConvertedValue;

    public NumericDoubleConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureDontUseIsBuggy)
    {
      var val = (double)value;

      if (_lastConvertedString is not null && val == _lastConvertedValue)
      {
        return _lastConvertedString;
      }

      _lastConvertedValue = val;
      _lastConvertedString = val.ToString(_conversionCulture);
      return _lastConvertedString;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureDontUseIsBuggy)
    {
      var validationResult = ConvertAndValidate(value, out var result);
      if (validationResult.IsValid)
      {
        _lastConvertedString = (string)value;
        _lastConvertedValue = result;
      }
      return result;
    }

    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureDontUseIsBuggy)
    {
      var validationResult = ConvertAndValidate(value, out var result);
      if (validationResult.IsValid)
      {
        _lastConvertedString = (string)value;
        _lastConvertedValue = result;
      }
      return validationResult;
    }

    private ValidationResult ConvertAndValidate(object value, out double result)
    {
      var s = (string)value;

      if (_lastConvertedValue is not null && s == _lastConvertedString)
      {
        result = (double)_lastConvertedValue;
        return ValidateSuccessfullyConvertedValue(result);
      }

      if (double.TryParse(s, System.Globalization.NumberStyles.Float, _conversionCulture, out result))
      {
        return ValidateSuccessfullyConvertedValue(result);
      }

      try
      {
        result = CSharpScript.EvaluateAsync<double>(s).Result;
        return ValidateSuccessfullyConvertedValue(result);
      }
      catch
      {
      }

      return new ValidationResult(false, "String could not be converted to a number");
    }

    private ValidationResult ValidateSuccessfullyConvertedValue(double result)
    {
      if (double.IsNaN(result) && !AllowNaNValues)
        return new ValidationResult(false, "This string represents NaN ('Not a Number'). This is not allowed here!");
      if (double.IsInfinity(result) && !AllowInfiniteValues)
        return new ValidationResult(false, "This string represents Infinity. Please enter a finite value!");
      if (DisallowZeroValues && result == 0)
        return new ValidationResult(false, "A value of zero is not valid here. Please enter a nonzero value!");
      if (DisallowNegativeValues && result < 0)
        return new ValidationResult(false, "A negative value is not valid here. Please enter a nonnegative value!");

      if (!IsMinValueInclusive && !(result > MinValue))
        return new ValidationResult(false, string.Format("A value <= {0} is not valid here. Please enter a value > {0}", MinValue));
      if (IsMinValueInclusive && !(result >= MinValue))
        return new ValidationResult(false, string.Format("A value < {0} is not valid here. Please enter a value >= {0}", MinValue));

      if (!IsMaxValueInclusive && !(result < MaxValue))
        return new ValidationResult(false, string.Format("A value >= {0} is not valid here. Please enter a value < {0}", MaxValue));
      if (IsMaxValueInclusive && !(result <= MaxValue))
        return new ValidationResult(false, string.Format("A value > {0} is not valid here. Please enter a value <= {0}", MaxValue));

      return ValidationResult.ValidResult;
    }
  }
}
