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
	/// <summary>
	/// Interaction logic for LineCapSizeComboBox.xaml
	/// </summary>
	public partial class LineCapSizeComboBox : ThicknessImageComboBox
	{
		static Dictionary<double, ImageSource> _cachedImages = new Dictionary<double, ImageSource>();

		static readonly double[] _initialValues = new double[] { 4,6,8,10,12,16,20,24,28,32 };

		public LineCapSizeComboBox()
		{
			InitializeComponent();

			foreach(var e in _initialValues)
				Items.Add(new ImageComboBoxItem(this,e));

			SetBinding(_nameOfValueProp);

			_img.Source = GetImage(SelectedLineCapSize);
		}
	

	#region Dependency property
		const string _nameOfValueProp = "SelectedLineCapSize";
		public double SelectedLineCapSize
		{
			get { return (double)GetValue(SelectedLineCapSizeProperty); }
			set { SetValue(SelectedLineCapSizeProperty, value); }
		}

		public static readonly DependencyProperty SelectedLineCapSizeProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(double), typeof(LineCapSizeComboBox),
				new FrameworkPropertyMetadata(8.0, OnSelectedLineCapSizeChanged));

		private static void OnSelectedLineCapSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((LineCapSizeComboBox)obj).EhSelectedLineCapSizeChanged(obj, args);
		}
		#endregion

		protected virtual void EhSelectedLineCapSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
			return (string)_converter.Convert(item, typeof(string), null, System.Globalization.CultureInfo.CurrentUICulture);
		}


		public static ImageSource GetImage(double val)
		{
			const double height = 1;
			const double width = 2;
			const double nominalHeight = 24; // normal height of a combobox item

			val *= height / nominalHeight;


			// draws a transparent outline to fix the borders
			var drawingGroup = new DrawingGroup();

			var geometryDrawing = new GeometryDrawing();
			geometryDrawing.Geometry = new RectangleGeometry(new Rect(0, 0, width, height));
			geometryDrawing.Pen = new Pen(Brushes.Transparent, 0);
			drawingGroup.Children.Add(geometryDrawing);

			var pathFigure = new PathFigure();
			pathFigure.StartPoint = new Point(width/6,height/2);
			pathFigure.Segments.Add(new PolyLineSegment(new Point[]{ new Point(width/2,height/2+val/2), new Point(width-width/6,height/2), new Point(width/2, height/2-val/2) }, false));
			pathFigure.IsClosed = true;
			pathFigure.IsFilled = true;
			geometryDrawing = new GeometryDrawing() { Geometry = new PathGeometry(new PathFigure[] { pathFigure }) };
			geometryDrawing.Brush = Brushes.Black;
		drawingGroup.Children.Add(geometryDrawing);

		drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0, 0, width, height));


			var geometryImage = new DrawingImage(drawingGroup);
			geometryImage.Freeze();
			return geometryImage;
		}
	}
}
