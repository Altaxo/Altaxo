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
	public partial class ShearComboBox : EditableImageComboBox
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
		#endregion

		static Dictionary<double, ImageSource> _cachedImages = new Dictionary<double, ImageSource>();

		Binding _valueBinding;

		CC _valueConverter;

		#region Dependency property
		private const string _nameOfValueProp = "Shear";
		public double Shear
		{
			get { var result = (double)GetValue(ShearProperty); return result; }
			set { SetValue(ShearProperty, value); }
		}

		public static readonly DependencyProperty ShearProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(double), typeof(ShearComboBox),
				new FrameworkPropertyMetadata(EhShearChanged));

		private static void EhShearChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((ShearComboBox)obj).OnShearValueChanged(obj, args);
		}
		#endregion



		public ShearComboBox()
		{
			InitializeComponent();

			this.Items.Add(new ImageComboBoxItem(this, -2.0));
			this.Items.Add(new ImageComboBoxItem(this, -1.0));
			this.Items.Add(new ImageComboBoxItem(this, 0.0));
			this.Items.Add(new ImageComboBoxItem(this, 1.0));
			this.Items.Add(new ImageComboBoxItem(this, 2.0));

			_valueBinding = new Binding();
			_valueBinding.Source = this;
			_valueBinding.Path = new PropertyPath(_nameOfValueProp);
			_valueConverter = new CC(this);
			_valueBinding.Converter = _valueConverter;
			_valueBinding.ValidationRules.Add(new ValidationWithErrorString(_valueConverter.EhValidateText));
			this.SetBinding(ComboBox.TextProperty, _valueBinding);
			Shear = 0;
			_img.Source = GetImage(0.0); // since null is the default value, we have to set the image explicitely here
		}

		protected virtual void OnShearValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (null != _img)
			{
				var val = (double)args.NewValue;
				_img.Source = GetImage(val);
			}
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

		public static ImageSource GetImage(double shear)
		{
			const double height = 1;
			const double width  = 1;
			const double lineWidth = 0.08;
			const double lwHalf = lineWidth / 2;

			// draws a transparent outline to fix the borders
			var outlineDrawing = new GeometryDrawing();
			outlineDrawing.Geometry = new RectangleGeometry(new Rect(-lineWidth, -lineWidth, width + lineWidth, height + lineWidth));
			outlineDrawing.Pen = new Pen(Brushes.Transparent, 0);
		//	outlineDrawing.Brush = new SolidColorBrush(Colors.LightBlue);
			


			//
			// Create the Geometry to draw.
			//
			shear = -shear;
			var geometryGroup = new GeometryGroup();
			var figure1 = new PathFigure();
			if (shear == 0)
			{
				figure1.StartPoint = new Point(0,0);
				figure1.Segments.Add(new LineSegment(new Point(0,height),true));
				figure1.Segments.Add(new LineSegment(new Point(width,height),true));
				figure1.Segments.Add(new ArcSegment(new Point(0, 0), new Size(width, height), 90, false, SweepDirection.Counterclockwise, true));
				geometryGroup.Children.Add(new PathGeometry(new PathFigure[] { figure1 }));

				geometryGroup.Children.Add(new EllipseGeometry(new Point(width / 3, height-height / 3), lwHalf, lwHalf));
			}
			else if (shear > 0)
			{
				double phi = Math.PI / 2 - Math.Atan(shear);
				Point start = new Point(width * Math.Cos(phi), height - height * Math.Sin(phi));
				figure1.StartPoint = start;
				figure1.Segments.Add(new LineSegment(new Point(0, height), true));
				figure1.Segments.Add(new LineSegment(new Point(width, height), true));
				figure1.Segments.Add(new ArcSegment(start, new Size(width, height), 180*phi/Math.PI, false, SweepDirection.Counterclockwise, true));

				var figure2 = new PathFigure();
				figure2.StartPoint = new Point(0, 0);
				figure2.Segments.Add(new ArcSegment(start,new Size(width,height), 180-180*phi/Math.PI, false, SweepDirection.Clockwise,true));
				figure2.IsFilled = false;

				geometryGroup.Children.Add(new PathGeometry(new PathFigure[] { figure1, figure2 }));
			}
			else if (shear < 0)
			{
				double phi = Math.PI / 2 - Math.Atan(-shear);
				Point start = new Point(width - width * Math.Cos(phi), height - height * Math.Sin(phi));
				figure1.StartPoint = start;
				figure1.Segments.Add(new LineSegment(new Point(width, height), true));
				figure1.Segments.Add(new LineSegment(new Point(0, height), true));
				figure1.Segments.Add(new ArcSegment(start, new Size(width, height), 180 * phi / Math.PI, false, SweepDirection.Clockwise, true));

				var figure2 = new PathFigure();
				figure2.StartPoint = new Point(width, 0);
				figure2.Segments.Add(new ArcSegment(start, new Size(width, height), 180 - 180 * phi / Math.PI, false, SweepDirection.Counterclockwise, true));
				figure2.IsFilled = false;

				geometryGroup.Children.Add(new PathGeometry(new PathFigure[] { figure1, figure2 }));
			}

			var geometryDrawing = new GeometryDrawing();
			geometryDrawing.Geometry = geometryGroup;

			// Outline the drawing with a solid color.
			geometryDrawing.Pen = new Pen(Brushes.Black, lineWidth) { LineJoin = PenLineJoin.Round };
			geometryDrawing.Brush =
					new LinearGradientBrush(
							Color.FromRgb(100, 100, 255),
							Color.FromRgb(204, 204, 255),
							new Point(0, 0),
							new Point(1, 1));

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
