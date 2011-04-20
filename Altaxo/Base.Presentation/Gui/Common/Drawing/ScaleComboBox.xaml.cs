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
	public partial class ScaleComboBox : EditableImageComboBox
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

			public object ConvertBack(object obj, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				double val;
				string err = ValidateAndConvertText(obj, culture, out val);
				if(null!=err)
					throw new ArgumentOutOfRangeException(err);
				return val;
			}

			public string EhValidateText(object obj, System.Globalization.CultureInfo info)
			{
				double val;
				return ValidateAndConvertText(obj, info, out val);
			}

			public string ValidateAndConvertText(object obj, System.Globalization.CultureInfo info, out double val)
			{
				string text = ((string)obj).Trim();
				string error = null;
				val = double.NaN;
				bool isInPercent=false;

				if(text.EndsWith("%"))
					{
						text = text.Substring(0, text.Length - 1).Trim();
						isInPercent=true;
					}

				if (Altaxo.Serialization.GUIConversion.IsDouble(text, out val))
					{
						if (val == 0)
							error = "Value must be non-zero";
						else if (double.IsInfinity(val))
							error = "Value must not be infinity";
						if (double.IsNaN(val))
						error = "Value must be a valid number";
					}
					else
					{
						error =  "Provided text can not be converted to a numeric value";
					}

				if (isInPercent)
					val = val / 100;

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

		/// <summary>The items here were stored for the x scale only.</summary>
		static Dictionary<double, ImageSource> _cachedXImages = new Dictionary<double, ImageSource>();

		/// <summary>The items here were stored for the x scale only.</summary>
		static Dictionary<double, ImageSource> _cachedYImages = new Dictionary<double, ImageSource>();

		bool _isForYScale;

		Binding _valueBinding;
		CC _valueConverter;

		public event DependencyPropertyChangedEventHandler SelectedScaleChanged;

		#region Dependency property
		private const string _nameOfValueProp = "SelectedScale";
		public double SelectedScale
		{
			get { var result = (double)GetValue(SelectedScaleProperty); return result; }
			set { SetValue(SelectedScaleProperty, value); }
		}

		public static readonly DependencyProperty SelectedScaleProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(double), typeof(ScaleComboBox),
				new FrameworkPropertyMetadata(1.0, EhSelectedScaleChanged));

		private static void EhSelectedScaleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((ScaleComboBox)obj).OnSelectedScaleChanged(obj, args);
		}
		#endregion

		protected virtual void OnSelectedScaleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (null != _img)
			{
				var val = (double)args.NewValue;
				_img.Source = GetImage(val, _isForYScale);
			}

			if (SelectedScaleChanged != null)
				SelectedScaleChanged(obj, args);
		}

		public ScaleComboBox()
		{
			InitializeComponent();

			this.Items.Add(new ImageComboBoxItem(this, -2.0));
			this.Items.Add(new ImageComboBoxItem(this, -1.0));
			this.Items.Add(new ImageComboBoxItem(this, -0.5));
			this.Items.Add(new ImageComboBoxItem(this, 0.5));
			this.Items.Add(new ImageComboBoxItem(this, 1.0));
			this.Items.Add(new ImageComboBoxItem(this, 2.0));

			_valueBinding = new Binding();
			_valueBinding.Source = this;
			_valueBinding.Path = new PropertyPath(_nameOfValueProp);
			_valueConverter = new CC(this);
			_valueBinding.Converter = _valueConverter;
			_valueBinding.ValidationRules.Add(new ValidationWithErrorString(_valueConverter.EhValidateText));
			this.SetBinding(ComboBox.TextProperty, _valueBinding);
			_img.Source = GetImage(SelectedScale, _isForYScale);
		}

		public bool IsForYScale
		{
			set
			{
				_isForYScale = value;
			}
		}

		public bool IsValidated
		{
			get
			{
				return false;
			}
		}

		public override ImageSource GetItemImage(object item)
		{
			double value = (double)item;
			ImageSource result;
			if (_isForYScale)
			{
				if (!_cachedYImages.TryGetValue(value, out result))
					_cachedYImages.Add(value, result = GetImage(value, _isForYScale));
			}
			else
			{
				if (!_cachedXImages.TryGetValue(value, out result))
					_cachedXImages.Add(value, result = GetImage(value, _isForYScale));
			}

			return result;
		}

		public override string GetItemText(object item)
		{
			return (string)_valueConverter.Convert(item, typeof(string), null, System.Globalization.CultureInfo.CurrentUICulture);
		}

		public static ImageSource GetImage(double scale, bool isForY)
		{
			double height = 1;
			double width  = 1;
			double lineWidth = 0.05;

			var group = new DrawingGroup();

			// draws a transparent outline to fix the borders
			var outlineDrawing = new GeometryDrawing();
			outlineDrawing.Geometry = new RectangleGeometry(new Rect(-lineWidth, -lineWidth, width + lineWidth, height + lineWidth));
			outlineDrawing.Pen = new Pen(Brushes.Transparent,0);
			group.Children.Add(outlineDrawing);

			var absscale = Math.Abs(scale);
			if (absscale == 1)
			{
				var drawing1 = new GeometryDrawing();
				drawing1.Geometry = new EllipseGeometry(new Point(width / 2, height / 2), width / 4, height / 4);
				if(scale>0)
					drawing1.Brush = new RadialGradientBrush(Color.FromRgb(204, 204, 255), Color.FromRgb(100, 100, 255));
				else
					drawing1.Brush = new RadialGradientBrush( Color.FromRgb(100, 100, 255), Color.FromRgb(204, 204, 255));

				group.Children.Add(drawing1);

				drawing1 = new GeometryDrawing();
				drawing1.Geometry = new LineGeometry(new Point(0, height / 2), new Point(width, height / 2));
				drawing1.Pen = new Pen(Brushes.Black, lineWidth);
				group.Children.Add(drawing1);

				drawing1 = new GeometryDrawing();
				drawing1.Geometry = new LineGeometry(new Point(width / 2, 0), new Point(width / 2, height));
				drawing1.Pen = new Pen(Brushes.Black, lineWidth);
				group.Children.Add(drawing1);
			}
			else
			{

				var drawing1 = new GeometryDrawing();
				if(absscale>2)
					drawing1.Geometry = new EllipseGeometry(new Point(width / 2, height / 2), (width / 2), (height / 2)/absscale );
				else if(absscale>1)
					drawing1.Geometry = new EllipseGeometry(new Point(width / 2, height / 2), (width / 4) * absscale, (height / 4));
				else if(absscale>0.5)
					drawing1.Geometry = new EllipseGeometry(new Point(width / 2, height / 2), (width / 4), (height / 4)/absscale);
				else if(absscale>0)
					drawing1.Geometry = new EllipseGeometry(new Point(width / 2, height / 2), (width/2)*absscale, (height / 2));
				
			//	drawing1.Brush = new RadialGradientBrush(Color.FromRgb(204, 204, 255), Color.FromRgb(100, 100, 255));
				if(scale>0)
					drawing1.Brush = new LinearGradientBrush(Color.FromRgb(204, 204, 255), Color.FromRgb(100, 100, 255), 0);
				else
					drawing1.Brush = new LinearGradientBrush(Color.FromRgb(100, 100, 255), Color.FromRgb(204, 204, 255), 0);

				group.Children.Add(drawing1);

				drawing1 = new GeometryDrawing();
				drawing1.Geometry = new LineGeometry(new Point(0, height / 2), new Point(width, height / 2));
				drawing1.Pen = new Pen(Brushes.Black, lineWidth);
				group.Children.Add(drawing1);

				Point d11, d12, d13;
				Point d21, d22, d23;

				if(absscale>1)
				{
					// triangles pointing outside;
					d11 = new Point(0,height/2);
					d12 = new Point(width/4, height/2+height/8);
					d13 = new Point(width/4, height/2-height/8);
 
					d21 = new Point(width, height/2);
					d22 = new Point(width-width/4, height/2+height/8);
					d23 = new Point(width-width/4, height/2-height/8);
				}
				else
				{
					// triangles pointing inside
					d11 = new Point(width/4,height/2);
					d12 = new Point(0, height/2+height/8);
					d13 = new Point(0, height/2-height/8);
 
					d21 = new Point(width-width/4, height/2);
					d22 = new Point(width, height/2+height/8);
					d23 = new Point(width, height/2-height/8);
				}

				// now adjust the triangles a little
				if(absscale>2)
				{
					// nothing to do here, the triangles are already max outside
				}
				else if(absscale>1)
				{
					double offs = width/2- absscale*(width/4);
					d11.X += offs; d12.X += offs; d13.X += offs;
					d21.X -= offs; d22.X -= offs; d23.X -= offs;
				}
				else if(absscale>0.5)
				{
					// nothing to do here, the triangles are already max outside
				}
				else if(absscale>0)
				{
					double offs = width/4-absscale*width/2;
					d11.X += offs; d12.X += offs; d13.X += offs;
					d21.X -= offs; d22.X -= offs; d23.X -= offs;
				}

				var fig1 = new PathFigure(d11, new PathSegment[] { new LineSegment(d12, false), new LineSegment(d13, false) }, true);
				var fig2 = new PathFigure(d21, new PathSegment[] { new LineSegment(d22, false), new LineSegment(d23, false) }, true);

				drawing1 = new GeometryDrawing();
				drawing1.Geometry = new PathGeometry(new PathFigure[] { fig1, fig2 });
				drawing1.Brush = Brushes.Black;
				group.Children.Add(drawing1);
			}


			
			if (isForY)
				group.Transform = new RotateTransform(90, width / 2, height / 2);


			var geometryImage = new DrawingImage(group);

			// Freeze the DrawingImage for performance benefits.
			geometryImage.Freeze();
			return geometryImage;
		}
	}
}
