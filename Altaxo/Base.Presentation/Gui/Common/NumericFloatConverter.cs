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
  /// <summary>
  /// Converts <see cref="float"/> values to and from their string representation and validates the entered text.
  /// </summary>
  public class NumericFloatConverter : ValidationRule, IValueConverter
  {
    /// <summary>
    /// Default value for <see cref="IsMinValueInclusive"/>.
    /// </summary>
    public const bool DefaultValue_IsMinValueInclusive = true;
    /// <summary>
    /// Default value for <see cref="IsMaxValueInclusive"/>.
    /// </summary>
    public const bool DefaultValue_IsMaxValueInclusive = true;
    /// <summary>
    /// Default value for <see cref="MinValue"/>.
    /// </summary>
    public const float DefaultValue_MinValue = float.NegativeInfinity;
    /// <summary>
    /// Default value for <see cref="MaxValue"/>.
    /// </summary>
    public const float DefaultValue_MaxValue = float.PositiveInfinity;
    /// <summary>
    /// Gets or sets a value indicating whether infinite values are accepted.
    /// </summary>
    public bool AllowInfiniteValues { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether NaN values are accepted.
    /// </summary>
    public bool AllowNaNValues { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether zero is rejected.
    /// </summary>
    public bool DisallowZeroValues { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether negative values are rejected.
    /// </summary>
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

    /// <summary>
    /// Stores the configured minimum value.
    /// </summary>
    public float _minValue = DefaultValue_MinValue;

    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    public float MinValue { get { return _minValue; } set { _minValue = value; } }

    /// <summary>
    /// Gets or sets a value indicating whether the minimum value is inclusive.
    /// </summary>
    public bool IsMinValueInclusive { get; set; } = DefaultValue_IsMinValueInclusive;

    /// <summary>
    /// Stores the configured maximum value.
    /// </summary>
    public float _maxValue = DefaultValue_MaxValue;

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    public float MaxValue { get { return _maxValue; } set { _maxValue = value; } }

    /// <summary>
    /// Gets or sets a value indicating whether the maximum value is inclusive.
    /// </summary>
    public bool IsMaxValueInclusive { get; set; } = DefaultValue_IsMaxValueInclusive;

    private System.Globalization.CultureInfo _conversionCulture = Altaxo.Settings.GuiCulture.Instance;

    private string _lastConvertedString;
    private float? _lastConvertedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericFloatConverter"/> class.
    /// </summary>
    public NumericFloatConverter()
    {
    }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureDontUseIsBuggy)
    {
      var val = (float)value;

      if (_lastConvertedString is not null && val == _lastConvertedValue)
      {
        return _lastConvertedString;
      }

      _lastConvertedValue = val;
      _lastConvertedString = val.ToString(_conversionCulture);
      return _lastConvertedString;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    private ValidationResult ConvertAndValidate(object value, out float result)
    {
      var s = (string)value;

      if (_lastConvertedValue is not null && s == _lastConvertedString)
      {
        result = (float)_lastConvertedValue;
        return ValidateSuccessfullyConvertedValue(result);
      }

      if (float.TryParse(s, System.Globalization.NumberStyles.Float, _conversionCulture, out result))
      {
        return ValidateSuccessfullyConvertedValue(result);
      }

      return new ValidationResult(false, "String could not be converted to a number");
    }

    private ValidationResult ValidateSuccessfullyConvertedValue(float result)
    {
      if (float.IsNaN(result) && !AllowNaNValues)
        return new ValidationResult(false, "This string represents NaN ('Not a Number'). This is not allowed here!");
      if (float.IsInfinity(result) && !AllowInfiniteValues)
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
