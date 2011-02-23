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
	/// Interaction logic for FontSizeComboBox.xaml
	/// </summary>
	public partial class FontSizeComboBox : ThicknessImageComboBox
	{
		static Dictionary<double, ImageSource> _cachedImages = new Dictionary<double, ImageSource>();

		static readonly double[] _initialValues = new double[] { 4,6,8,10,12,16,18,20,24,28,32,36,54,72 };

		public event DependencyPropertyChangedEventHandler SelectedFontSizeChanged;

		public FontSizeComboBox()
		{
			InitializeComponent();

			foreach(var e in _initialValues)
				Items.Add(new ImageComboBoxItem(this,e));

			SetBinding(_nameOfValueProp);

			_img.Source = GetImage(SelectedFontSize);
		}
	

	#region Dependency property
		const string _nameOfValueProp = "SelectedFontSize";
		public double SelectedFontSize
		{
			get { return (double)GetValue(SelectedFontSizeProperty); }
			set { SetValue(SelectedFontSizeProperty, value); }
		}

		public static readonly DependencyProperty SelectedFontSizeProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(double), typeof(FontSizeComboBox),
				new FrameworkPropertyMetadata(8.0, EhSelectedFontSizeChanged));

		private static void EhSelectedFontSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((FontSizeComboBox)obj).OnSelectedFontSizeChanged(obj, args);
		}
		#endregion

		protected virtual void OnSelectedFontSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (null != _img)
			{
				var val = (double)args.NewValue;
				_img.Source = GetImage(val);
			}
			if (null != SelectedFontSizeChanged)
				SelectedFontSizeChanged(obj, args);
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
