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
	/// Interaction logic for MiterLimitComboBox.xaml
	/// </summary>
	public partial class MiterLimitComboBox : LengthImageComboBox
	{
		static Dictionary<double, ImageSource> _cachedImages = new Dictionary<double, ImageSource>();

		static readonly double[] _initialValues = new double[] { 0, 4, 6, 8, 10, 12};


		public MiterLimitComboBox()
		{
			UnitEnvironment = MiterLimitEnvironment.Instance;

			InitializeComponent();

			foreach (var e in _initialValues)
				Items.Add(new ImageComboBoxItem(this, new Science.QuantityWithUnit(e, Science.LengthUnitPoint.Instance)));

			_img.Source = GetImage(SelectedQuantityInPoints);

		}


		protected override void OnSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			base.OnSelectedQuantityChanged(obj, args);

			if (null != _img)
			{
				var val = SelectedQuantityInPoints;
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


		public static DrawingImage GetImage(double miterLimit)
		{
			const double height = 1;
			const double width = 2;
			const double lineWidth = 0.375 * height;

			var drawingGroup = new DrawingGroup();
			GeometryDrawing geometryDrawing;

			geometryDrawing = new GeometryDrawing();
			geometryDrawing.Geometry = new RectangleGeometry(new Rect(0, 0, width, height));
			geometryDrawing.Pen = new Pen(Brushes.Transparent, 0);
			drawingGroup.Children.Add(geometryDrawing);

			geometryDrawing = new GeometryDrawing();
			var figure = new PathFigure();
			figure.StartPoint = new Point(width, height * 0.875);
			figure.Segments.Add(new PolyLineSegment(new Point[] 
			{
				new Point(width / 2, height / 2),
				new Point(width, height * 0.175) }, true));
			geometryDrawing.Geometry = new PathGeometry(new PathFigure[] { figure });
			geometryDrawing.Pen = new Pen(Brushes.Black, lineWidth) { LineJoin = PenLineJoin.Miter, MiterLimit=miterLimit };
			drawingGroup.Children.Add(geometryDrawing);

			drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0, 0, width, height));

			DrawingImage geometryImage = new DrawingImage(drawingGroup);

			geometryImage.Freeze();
			return geometryImage;
		}
	}
}
