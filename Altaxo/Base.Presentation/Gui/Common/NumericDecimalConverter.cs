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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
  public class NumericDecimalConverter : ValidationRule, IValueConverter
  {
    public const bool DefaultValue_IsMinValueInclusive = true;
    public const bool DefaultValue_IsMaxValueInclusive = true;
    public const decimal DefaultValue_MinValue = decimal.MinValue;
    public const decimal DefaultValue_MaxValue = decimal.MaxValue;

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

    public decimal _minValue = DefaultValue_MinValue;

    public decimal MinValue { get { return _minValue; } set { _minValue = value; } }

    public bool IsMinValueInclusive { get; set; } = DefaultValue_IsMinValueInclusive;

    public decimal _maxValue = DefaultValue_MaxValue;

    public decimal MaxValue { get { return _maxValue; } set { _maxValue = value; } }

    public bool IsMaxValueInclusive { get; set; } = DefaultValue_IsMaxValueInclusive;

    private System.Globalization.CultureInfo _conversionCulture = Altaxo.Settings.GuiCulture.Instance;

    private string _lastConvertedString;
    private decimal? _lastConvertedValue;

    public NumericDecimalConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureDontUseIsBuggy)
    {
      var val = (decimal)value;

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

    private ValidationResult ConvertAndValidate(object value, out decimal result)
    {
      var s = (string)value;

      if (_lastConvertedValue is not null && s == _lastConvertedString)
      {
        result = (decimal)_lastConvertedValue;
        return ValidateSuccessfullyConvertedValue(result);
      }

      if (decimal.TryParse(s, System.Globalization.NumberStyles.Float, _conversionCulture, out result))
      {
        return ValidateSuccessfullyConvertedValue(result);
      }

      return new ValidationResult(false, "String could not be converted to a number");
    }

    private ValidationResult ValidateSuccessfullyConvertedValue(decimal result)
    {
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
