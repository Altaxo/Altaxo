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
		public bool AllowInfinity { get; set; }
		public bool AllowNaN { get; set; }


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
			if(double.IsNaN(result) && !AllowNaN)
				return new ValidationResult(false, "This string represents NaN (not a number). This is not allowed here!");
			if(double.IsInfinity(result) && !AllowInfinity)
				return new ValidationResult(false, "This string represents Infinity. Please enter a finite number!");

			return ValidationResult.ValidResult;
		}
	}
}
