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

using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// ComboBox for <see cref="Altaxo.Graph.Gdi.LinearGradientShape"/>.
	/// </summary>
	public partial class LinearGradientShapeComboBox : ImageComboBox
	{
		class CC : IValueConverter
		{
			LinearGradientShapeComboBox _cb;
			object _originalToolTip;
			bool _hasValidationError;

			public CC(LinearGradientShapeComboBox c)
			{
				_cb = c;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var val = (LinearGradientShape)value;
				return _cb._cachedItems[val];


			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return ((ImageComboBoxItem)value).Value;
			}
		}

		static Dictionary<LinearGradientShape, ImageSource> _cachedImages = new Dictionary<LinearGradientShape, ImageSource>();

		Dictionary<LinearGradientShape, ImageComboBoxItem> _cachedItems = new Dictionary<LinearGradientShape, ImageComboBoxItem>();

		static LinearGradientShapeComboBox()
		{
		}

		public LinearGradientShapeComboBox()
		{
			InitializeComponent();

			foreach(var e in new LinearGradientShape[]{ LinearGradientShape.Linear, LinearGradientShape.SigmaBell, LinearGradientShape.Triangular })
			{
				_cachedItems.Add(e, new ImageComboBoxItem(this, e));
				Items.Add(_cachedItems[e]);
			}

			var _valueBinding = new Binding();
			_valueBinding.Source = this;
			_valueBinding.Path = new PropertyPath(_nameOfValueProp);
			_valueBinding.Converter = new CC(this);
			this.SetBinding(ComboBox.SelectedItemProperty, _valueBinding);
		}

		#region Dependency property
		private const string _nameOfValueProp = "LinearGradientShape";
		public LinearGradientShape LinearGradientShape
		{
			get { return (LinearGradientShape)GetValue(LinearGradientShapeProperty); }
			set {	SetValue(LinearGradientShapeProperty, value); }
		}

		public static readonly DependencyProperty LinearGradientShapeProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(LinearGradientShape), typeof(LinearGradientShapeComboBox),
				new FrameworkPropertyMetadata(LinearGradientShape.Linear, OnLinearGradientShapeChanged));

		private static void OnLinearGradientShapeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((LinearGradientShapeComboBox)obj).EhLinearGradientShapeChanged(obj,args);
		}
		#endregion

		protected virtual void EhLinearGradientShapeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{

		}

	

		public override string GetItemText(object item)
		{
			var val = (LinearGradientShape)item;
			return val.ToString();
		}

		public override ImageSource GetItemImage(object item)
		{
			var val = (LinearGradientShape)item;
			ImageSource result;
			if (!_cachedImages.TryGetValue(val, out result))
				_cachedImages.Add(val, result = GetImage(val));
			return result;
		}
		

		public static DrawingImage GetImage(LinearGradientShape val)
		{
			double height = 1;
			double width = 2;

			//
			// Create the Geometry to draw.
			//
			GeometryGroup geometryGroup = new GeometryGroup();
			geometryGroup.Children.Add(new RectangleGeometry(new Rect(0,0,width,height)));

			var geometryDrawing = new GeometryDrawing() { Geometry = geometryGroup };

			switch (val)
			{
				case LinearGradientShape.Linear:
					geometryDrawing.Brush = new System.Windows.Media.LinearGradientBrush(Colors.Black, Colors.White, 0);
					break;
				case LinearGradientShape.SigmaBell:
					{
						var gradStop = new GradientStopCollection();
						gradStop.Add(new GradientStop(Colors.Black, 0));
						gradStop.Add(new GradientStop(Colors.Gray, 0.1));
						gradStop.Add(new GradientStop(Colors.White, 0.5));
						gradStop.Add(new GradientStop(Colors.Gray, 0.9));
						gradStop.Add(new GradientStop(Colors.Black, 1));
						geometryDrawing.Brush = new System.Windows.Media.LinearGradientBrush(gradStop, 0);
					}
					break;
				case LinearGradientShape.Triangular:
					{
						var gradStop = new GradientStopCollection();
						gradStop.Add(new GradientStop(Colors.Black, 0));
						gradStop.Add(new GradientStop(Colors.White, 0.5));
						gradStop.Add(new GradientStop(Colors.Black, 1));
						geometryDrawing.Brush = new System.Windows.Media.LinearGradientBrush(gradStop, 0);
					}
					break;
				default:
					break;
			}

			DrawingImage geometryImage = new DrawingImage(geometryDrawing);

			// Freeze the DrawingImage for performance benefits.
			geometryImage.Freeze();
			return geometryImage;
		}
	}
}
