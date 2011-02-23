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
	public partial class RotationComboBox : EditableImageComboBox
	{
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
					throw new ArgumentOutOfRangeException("Provided text can not be converted to a numeric value");
			}

			public string EhValidateText(object obj, System.Globalization.CultureInfo info)
			{
				string error = null;
				double val;
				if (Altaxo.Serialization.GUIConversion.IsDouble((string)obj, out val))
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

		static Dictionary<double, ImageSource> _cachedImages = new Dictionary<double, ImageSource>();
		Binding _valueBinding;
		CC _valueConverter;
		public event DependencyPropertyChangedEventHandler SelectedRotationChanged;

		#region Dependency property
		private const string _nameOfValueProp = "SelectedRotation";
		public double SelectedRotation
		{
			get { var result = (double)GetValue(SelectedRotationProperty); return result; }
			set { SetValue(SelectedRotationProperty, value); }
		}

		public static readonly DependencyProperty SelectedRotationProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(double), typeof(RotationComboBox),
				new FrameworkPropertyMetadata(EhSelectedRotationChanged));

		private static void EhSelectedRotationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((RotationComboBox)obj).OnSelectedRotationChanged(obj, args);
		}
		#endregion



		public RotationComboBox()
		{

			InitializeComponent();

			_valueBinding = new Binding();
			_valueBinding.Source = this;
			_valueBinding.Path = new PropertyPath(_nameOfValueProp);
			_valueConverter = new CC(this);
			_valueBinding.Converter = _valueConverter;
			_valueBinding.ValidationRules.Add(new ValidationWithErrorString(_valueConverter.EhValidateText));
			this.SetBinding(ComboBox.TextProperty, _valueBinding);

			this.Items.Add(new ImageComboBoxItem(this, 0.0));
			this.Items.Add(new ImageComboBoxItem(this, 45.0));
			this.Items.Add(new ImageComboBoxItem(this, 90.0));
			this.Items.Add(new ImageComboBoxItem(this, 135.0));
			this.Items.Add(new ImageComboBoxItem(this, 180.0));
			this.Items.Add(new ImageComboBoxItem(this, 225.0));
			this.Items.Add(new ImageComboBoxItem(this, 270.0));
			this.Items.Add(new ImageComboBoxItem(this, 315.0));

			_img.Source = GetImage(SelectedRotation); // since 0 is the default value, we have to set the image here explicitly

		}


		protected virtual void OnSelectedRotationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (null != _img)
			{
				var val = (double)args.NewValue;
				_img.Source = GetImage(val);
			}

			if (null != SelectedRotationChanged)
				SelectedRotationChanged(obj, args);
		}

		public override ImageSource GetItemImage(object item)
		{
			double angle = (double)item;
			ImageSource result;
			if (!_cachedImages.TryGetValue(angle, out result))
				_cachedImages.Add(angle, result = GetImage(angle));

			return result;
		}

		public override string GetItemText(object item)
		{
			return (string)_valueConverter.Convert(item, typeof(string), null, System.Globalization.CultureInfo.CurrentUICulture);
		}

		public static ImageSource GetImage(double angle)
		{
			double AngleRad = (angle / 180) * Math.PI;

			const double DesignHeight=1;
			double offset = 0.5*DesignHeight;
			double radius = 0.40*DesignHeight;
			double lineWidth = 0.08*DesignHeight;
			
			// draws a transparent outline to fix the borders
			var outlineDrawing = new GeometryDrawing();
			outlineDrawing.Geometry = new RectangleGeometry(new Rect(0, 0, DesignHeight, DesignHeight));
			outlineDrawing.Pen = new Pen(Brushes.Transparent, 0);


			// now the geometry itself
			GeometryGroup ellipses = new GeometryGroup();
			ellipses.Children.Add(
					new EllipseGeometry(new Point(offset, offset), radius, radius)
					);

			ellipses.Children.Add(new LineGeometry(new Point(offset, offset), new Point(offset + radius * Math.Cos(AngleRad), offset + radius * Math.Sin(-AngleRad))));
			GeometryDrawing geometryDrawing = new GeometryDrawing();
			geometryDrawing.Geometry = ellipses;
			// fill with a gradient brush
			geometryDrawing.Brush =
					new LinearGradientBrush(
							Color.FromRgb(100, 100, 255),
							Color.FromRgb(204, 204, 255),
							new Point(0, 0),
							new Point(1, 1));
			geometryDrawing.Pen = new Pen(Brushes.Black, lineWidth);

			var group = new DrawingGroup();
			group.Children.Add(outlineDrawing);
			group.Children.Add(geometryDrawing);
			var geometryImage = new DrawingImage(group);

			// Freeze the DrawingImage for performance benefits.
			geometryImage.Freeze();
			return geometryImage;
		}
	}
}
