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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
	public class NumericDoubleConverter : ValidationRule, IValueConverter
	{
		public bool AllowInfiniteValues { get; set; }

		public bool AllowNaNValues { get; set; }

		public bool DisallowZeroValues { get; set; }

		public bool DisallowNegativeValues { get; set; }

		private System.Globalization.CultureInfo _conversionCulture = Altaxo.Settings.GuiCulture.Instance;

		private string _lastConvertedString;
		private double? _lastConvertedValue;

		public NumericDoubleConverter()
		{
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureDontUseIsBuggy)
		{
			var val = (double)value;

			if (null != _lastConvertedString && val == _lastConvertedValue)
			{
				return _lastConvertedString;
			}

			_lastConvertedValue = val;
			_lastConvertedString = val.ToString(_conversionCulture);
			return _lastConvertedString;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureDontUseIsBuggy)
		{
			double result;
			var validationResult = ConvertAndValidate(value, out result);
			if (validationResult.IsValid)
			{
				_lastConvertedString = (string)value;
				_lastConvertedValue = result;
			}
			return result;
		}

		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureDontUseIsBuggy)
		{
			double result;
			var validationResult = ConvertAndValidate(value, out result);
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

			if (null != _lastConvertedValue && s == _lastConvertedString)
			{
				result = (double)_lastConvertedValue;
				return ValidateSuccessfullyConvertedValue(result);
			}

			if (double.TryParse(s, System.Globalization.NumberStyles.Float, _conversionCulture, out result))
			{
				return ValidateSuccessfullyConvertedValue(result);
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

			return ValidationResult.ValidResult;
		}
	}
}