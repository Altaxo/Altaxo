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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Common
{
	public class NumericDoubleConverter : ValidationRule, IValueConverter
	{
		public bool AllowInfiniteValues { get; set; }
		public bool AllowNaNValues { get; set; }
		public bool DisallowZeroValues { get; set; }
		public bool DisallowNegativeValues { get; set; }


		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return Altaxo.Serialization.GUIConversion.ToString((double)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double result;
			Validate(value, culture, out result);
			return result;
		}


		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			double result;
			return Validate(value, cultureInfo, out result);
		}

		private ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo, out double result)
		{
			var val = (string)value;
			if (!Altaxo.Serialization.GUIConversion.IsDouble(val, out result))
				return new ValidationResult(false, "This string could not be converted to a number!");
			if (double.IsNaN(result) && !AllowNaNValues)
				return new ValidationResult(false, "This string represents NaN ('Not a Number'). This is not allowed here!");
			if (double.IsInfinity(result) && !AllowInfiniteValues)
				return new ValidationResult(false, "This string represents Infinity. Please enter a finite value!");
			if(DisallowZeroValues && result==0)
				return new ValidationResult(false, "A value of zero is not valid here. Please enter a nonzero value!");
			if(DisallowNegativeValues && result<0)
				return new ValidationResult(false, "A negative value is not valid here. Please enter a nonnegative value!");

			return ValidationResult.ValidResult;
		}
	}
}
