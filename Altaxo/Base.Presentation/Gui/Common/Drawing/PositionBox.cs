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

namespace Altaxo.Gui.Common.Drawing
{
	public class PositionBox : TextBox
	{
		class CC : IValueConverter
		{
			TextBox _cb;

			string _textBoxText;
			double? _value = null;

			object _originalToolTip;
			bool _hasValidationError;

			public CC(TextBox c)
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

		Binding _valueBinding;
		BindingExpressionBase _valueBindingExpression;
		CC _valueConverter;

		public PositionBox()
		{
			_valueBinding = new Binding();
			_valueBinding.Source = this;
			_valueBinding.Path = new PropertyPath(_nameOfValueProp);
			_valueConverter = new CC(this);
			_valueBinding.Converter = _valueConverter;
			_valueBinding.ValidationRules.Add(new ValidationWithErrorString(_valueConverter.EhValidateText));
			_valueBinding.Mode = BindingMode.TwoWay;
			_valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			_valueBindingExpression = this.SetBinding(TextBox.TextProperty, _valueBinding);
		}


		#region Dependency property
		private const string _nameOfValueProp = "PositionDistanceValue";

		public double PositionDistanceValue
		{
			get { var result = (double)GetValue(PositionDistanceValueProperty); return result; }
			set
			{
				SetValue(PositionDistanceValueProperty, value);
				this.Text = Altaxo.Serialization.GUIConversion.ToString(value);
			}
		}

		public static readonly DependencyProperty PositionDistanceValueProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(double), typeof(PositionBox),
				new FrameworkPropertyMetadata(OnPositionDistanceValueChanged));

		private static void OnPositionDistanceValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{

		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);
			_valueBindingExpression.UpdateTarget();
		}
		#endregion
	}
}
