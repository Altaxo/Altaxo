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
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
  public partial class BigIntegerUpDown
  {
    protected class BigIntegerConverter : ValidationRule, IValueConverter
    {
      public BigInteger? DefaultMinimumValue => null;
      public BigInteger? DefaultMaximumValue => null;

      public BigInteger? MinValue { get; set; } = null;
      public BigInteger? MaxValue { get; set; } = null;

      public BigInteger? ValueIfTextIsEmpty { get; set; } = null;

      public string MinimumReplacementText { get; set; } = "Min";
      public string MaximumReplacementText { get; set; } = "Max";

      private System.Globalization.CultureInfo _conversionCulture = Altaxo.Settings.GuiCulture.Instance;

      private string _lastConvertedString;
      private BigInteger? _lastConvertedValue;


      public BigIntegerConverter()
      {
      }


      public object Convert(object value, Type targetType, object parameter, CultureInfo cultureBuggyDontUse)
      {
        BigInteger val = (BigInteger)value;

        if (_lastConvertedString is not null && val == _lastConvertedValue)
        {
          return _lastConvertedString;
        }

        if (val == MinValue && MinimumReplacementText is not null)
        {
          return MinimumReplacementText;
        }
        else if (val == MaxValue && MaximumReplacementText is not null)
        {
          return MaximumReplacementText;
        }
        else
        {
          _lastConvertedValue = val;
          _lastConvertedString = val.ToString(_conversionCulture);
          return _lastConvertedString;
        }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureBuggyDontUse)
      {
        var validationResult = ConvertAndValidate(value, out var result);
        if (validationResult.IsValid)
        {
          _lastConvertedString = (string)value;
          _lastConvertedValue = result;
        }
        return result;
      }

      public override ValidationResult Validate(object value, CultureInfo cultureInfoBuggyDontUse)
      {
        var validationResult = ConvertAndValidate(value, out var result);
        if (validationResult.IsValid)
        {
          _lastConvertedString = (string)value;
          _lastConvertedValue = result;
        }
        return validationResult;
      }

      private ValidationResult ConvertAndValidate(object value, out BigInteger result)
      {
        var s = (string)value;

        if (_lastConvertedValue.HasValue && s == _lastConvertedString)
        {
          result = _lastConvertedValue.Value;
          return ValidateSuccessfullyConvertedValue(result);
        }
        else if (MinValue.HasValue && !string.IsNullOrEmpty(MinimumReplacementText) && MinimumReplacementText.Trim() == s)
        {
          result = MinValue.Value;
          return ValidateSuccessfullyConvertedValue(result);
        }
        else if (MaxValue.HasValue && !string.IsNullOrEmpty(MaximumReplacementText) && MaximumReplacementText.Trim() == s)
        {
          result = MaxValue.Value;
          return ValidateSuccessfullyConvertedValue(result);
        }
        else if (ValueIfTextIsEmpty.HasValue && string.IsNullOrEmpty(s))
        {
          result = ValueIfTextIsEmpty.Value;
          return ValidateSuccessfullyConvertedValue(result);
        }
        else
        {
          if (BigInteger.TryParse(s, System.Globalization.NumberStyles.Number, _conversionCulture, out result))
          {
            return ValidateSuccessfullyConvertedValue(result);
          }
          else
          {
            return new ValidationResult(false, "String could not be converted to a number");
          }
        }
      }

      private ValidationResult ValidateSuccessfullyConvertedValue(BigInteger result)
      {
        if (MinValue.HasValue && !(result >= MinValue))
          return new ValidationResult(false, $"A value < {MinValue.Value} is not valid here. Please enter a value >= {MinValue.Value}");

        if (MaxValue.HasValue && !(result <= MaxValue))
          return new ValidationResult(false, $"A value > {MaxValue.Value} is not valid here. Please enter a value <= {MaxValue.Value}");

        return ValidationResult.ValidResult;
      }

    }
  }
}
