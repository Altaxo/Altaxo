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
using System.Reflection;
using System.Diagnostics;

namespace Altaxo.Gui.Common.Drawing
{
	public partial class ColorScaleComboBox : EditableImageComboBox
	{
		#region Converter

		class CC : IValueConverter
		{
			ComboBox _cb;
			object _originalToolTip;
			bool _hasValidationError;

			public CC(ComboBox c)
			{
				_cb = c;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var val = (double)value;
				return Altaxo.Serialization.GUIConversion.ToString(val);


			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				string text = (string)value;
				double val;
				if (Altaxo.Serialization.GUIConversion.IsDouble(text, out val))
					return val;
				else
					throw new ArgumentOutOfRangeException("Provided string can not be converted to a numeric value");
			}

			public string EhValidateText(object obj, System.Globalization.CultureInfo info)
			{
				string error = null;
				double val;
				if (Altaxo.Serialization.GUIConversion.IsDouble((string)obj, out val))
				{
					if (double.IsInfinity(val))
						error = "Value must not be infinity";
					else if (double.IsNaN(val))
						error = "Value must be a valid number";
					else if (val < 0)
						error = "Value must be a non-negative number";
					else if (val > 1)
						error = "Value must be less or equal than 1";

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
		#endregion

		static Dictionary<double, ImageSource> _cachedImages = new Dictionary<double, ImageSource>();

		Binding _valueBinding;

		CC _valueConverter;

		/// <summary>Occurs when the selected color scale value changed.</summary>
		public event DependencyPropertyChangedEventHandler SelectedColorScaleChanged;


		#region Dependency property
		private const string _nameOfValueProp = "SelectedColorScale";
		public double SelectedColorScale
		{
			get { var result = (double)GetValue(SelectedColorScaleProperty); return result; }
			set { SetValue(SelectedColorScaleProperty, value); }
		}

		public static readonly DependencyProperty SelectedColorScaleProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(double), typeof(ColorScaleComboBox),
				new FrameworkPropertyMetadata(1.0, EhSelectedColorScaleChanged));

		private static void EhSelectedColorScaleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((ColorScaleComboBox)obj).OnSelectedColorScaleValueChanged(obj, args);
		}
		#endregion



		public ColorScaleComboBox()
		{
			InitializeComponent();

			this.Items.Add(new ImageComboBoxItem(this, 0.0));
			this.Items.Add(new ImageComboBoxItem(this, 0.25));
			this.Items.Add(new ImageComboBoxItem(this, 0.5));
			this.Items.Add(new ImageComboBoxItem(this, 0.75));
			this.Items.Add(new ImageComboBoxItem(this, 1.0));

			_valueBinding = new Binding();
			_valueBinding.Source = this;
			_valueBinding.Path = new PropertyPath(_nameOfValueProp);
			_valueConverter = new CC(this);
			_valueBinding.Converter = _valueConverter;
			_valueBinding.ValidationRules.Add(new ValidationWithErrorString(_valueConverter.EhValidateText));
			this.SetBinding(ComboBox.TextProperty, _valueBinding);
			SelectedColorScale = 0;
			_img.Source = GetImage(0.0); // since null is the default value, we have to set the image explicitely here
		}

		protected override void ImplantImage(double width, double height)
		{
			base.ImplantImage(width, height);
			var h = _img.Height;
			const double hMargin = 6;
			_img.Margin = new Thickness(_img.Margin.Left, _img.Margin.Top + hMargin, _img.Margin.Right, _img.Margin.Bottom + hMargin);
			_img.Height = h - 2 * hMargin;
		}

		protected virtual void OnSelectedColorScaleValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (null != _img)
			{
				var val = (double)args.NewValue;
				_img.Source = GetImage(val);
			}

			if (null != SelectedColorScaleChanged)
				SelectedColorScaleChanged(this, args);
		}


		public override ImageSource GetItemImage(object item)
		{
			var val = (double)item;
			ImageSource result;
			if (!_cachedImages.TryGetValue(val, out result))
				_cachedImages.Add(val, result = GetImage(val));
			return result;
		}


		public override string GetItemText(object item)
		{
			return (string)_valueConverter.Convert(item, typeof(string), null, System.Globalization.CultureInfo.CurrentUICulture);
		}

		public static ImageSource GetImage(double val)
		{
			const double height = 1;
			const double width = 2;
			const double lineWidth = 0;

			// draws a transparent outline to fix the borders
			var geometryDrawing = new GeometryDrawing();
			geometryDrawing.Geometry = new RectangleGeometry(new Rect(-lineWidth, -lineWidth, width + lineWidth, height + lineWidth));
			geometryDrawing.Pen = new Pen(Brushes.Transparent, 0);

			var gradStops = new GradientStopCollection();
			gradStops.Add(new GradientStop(Colors.Black, 0));
			gradStops.Add(new GradientStop(Colors.White, val));
			gradStops.Add(new GradientStop(Colors.Black, 1));

			geometryDrawing.Brush = new LinearGradientBrush(gradStops, 0);
			var geometryImage = new DrawingImage(geometryDrawing);

			// Freeze the DrawingImage for performance benefits.
			geometryImage.Freeze();
			return geometryImage;
		}
	}
}
