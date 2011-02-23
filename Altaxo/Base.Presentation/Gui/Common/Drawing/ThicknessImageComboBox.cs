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

namespace Altaxo.Gui.Common.Drawing
{
	public class ThicknessImageComboBox : EditableImageComboBox
	{
		public class ThicknessConverter : IValueConverter
		{
			ThicknessImageComboBox _cb;

			string _textBoxText;
			double? _value = null;

			object _originalToolTip;
			bool _hasValidationError;

			public ThicknessConverter(ThicknessImageComboBox c)
			{
				_cb = c;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var newvalue = (double)value;
				if (newvalue == _value && _cb.IsFocused)
				{
					return Binding.DoNothing;
				}

				_value = newvalue;
				var result = Altaxo.Serialization.GUIConversion.GetLengthMeasureText(newvalue);
				return result;
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				string newtext = (string)value;
				_textBoxText = newtext;

				double val = 0;
				if (Altaxo.Serialization.GUIConversion.GetLengthMeasureValue(newtext, ref val))
				{
					_value = val;
					return val;
				}
				else
					throw new ArgumentOutOfRangeException("Provided string can not be converted to a numeric value");
			}

			public string EhValidateText(object obj, System.Globalization.CultureInfo info)
			{
				string error = null;
				double val = 0;
				if (Altaxo.Serialization.GUIConversion.GetLengthMeasureValue((string)obj, ref val))
				{

					if (double.IsInfinity(val))
						error = "Value must not be infinity";
					if (double.IsNaN(val))
						error = "Value must be a valid number";
					if (val <= 0)
						error = "Value must be a positive number";
				}
				else
				{
					error = "Provided text can not be converted to a numeric value";
				}

				if (null != error)
				{
					_hasValidationError = true;
					_originalToolTip = _cb.ToolTip;
					_cb.ToolTip = error;
				}
				else
				{
					_hasValidationError = false;
					_cb.ToolTip = _originalToolTip;
					_originalToolTip = null;
				}

				return error;
			}
		}


		protected ThicknessConverter _converter;

		protected void SetBinding(string nameOfValueProperty)
		{
			var binding = new Binding();
			binding.Source = this;
			binding.Path = new PropertyPath(nameOfValueProperty);
			_converter = new ThicknessConverter(this);
			binding.Converter = _converter;
			binding.ValidationRules.Add(new ValidationWithErrorString(_converter.EhValidateText));
			this.SetBinding(ComboBox.TextProperty, binding);
		}

	
	}
}
