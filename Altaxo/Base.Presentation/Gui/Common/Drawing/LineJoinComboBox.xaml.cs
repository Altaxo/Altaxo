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

using sdd = System.Drawing.Drawing2D;

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Interaction logic for LineJoinComboBox.xaml
	/// </summary>
	public partial class LineJoinComboBox : ImageComboBox
	{
		class CC : IValueConverter
		{
			LineJoinComboBox _cb;
			object _originalToolTip;
			bool _hasValidationError;

			public CC(LineJoinComboBox c)
			{
				_cb = c;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var val = (sdd.LineJoin)value;
				return _cb._cachedItems[val];


			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return ((ImageComboBoxItem)value).Value;
			}
		}

		static Dictionary<sdd.LineJoin, ImageSource> _cachedImages = new Dictionary<sdd.LineJoin, ImageSource>();

		Dictionary<sdd.LineJoin, ImageComboBoxItem> _cachedItems = new Dictionary<sdd.LineJoin, ImageComboBoxItem>();

		public LineJoinComboBox()
		{
			InitializeComponent();

			_cachedItems.Add(sdd.LineJoin.Bevel, new ImageComboBoxItem(this, sdd.LineJoin.Bevel));
			_cachedItems.Add(sdd.LineJoin.Miter, new ImageComboBoxItem(this, sdd.LineJoin.Miter));
			_cachedItems.Add(sdd.LineJoin.MiterClipped, new ImageComboBoxItem(this, sdd.LineJoin.Miter)); // trick: MiterClipped is projected to Miter item here
			_cachedItems.Add(sdd.LineJoin.Round, new ImageComboBoxItem(this, sdd.LineJoin.Round));


			Items.Add(_cachedItems[sdd.LineJoin.Bevel]);
			Items.Add(_cachedItems[sdd.LineJoin.Miter]);
			Items.Add(_cachedItems[sdd.LineJoin.Round]);


			var _valueBinding = new Binding();
			_valueBinding.Source = this;
			_valueBinding.Path = new PropertyPath(_nameOfValueProp);
			_valueBinding.Converter = new CC(this);
			this.SetBinding(ComboBox.SelectedItemProperty, _valueBinding);
		}

		#region Dependency property
		private const string _nameOfValueProp = "SelectedLineJoin";
		public sdd.LineJoin SelectedLineJoin
		{
			get { var result = (sdd.LineJoin)GetValue(SelectedLineJoinProperty); return result; }
			set {	SetValue(SelectedLineJoinProperty, value); }
		}

		public static readonly DependencyProperty SelectedLineJoinProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(sdd.LineJoin), typeof(LineJoinComboBox),
				new FrameworkPropertyMetadata(OnSelectedLineJoinChanged));

		private static void OnSelectedLineJoinChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((LineJoinComboBox)obj).EhSelectedLineJoinChanged(obj,args);
		}
		#endregion

		protected virtual void EhSelectedLineJoinChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{

		}

	

		public override string GetItemText(object item)
		{
			var val = (sdd.LineJoin)item;
			return val.ToString();
		}

		public override ImageSource GetItemImage(object item)
		{
			var val = (sdd.LineJoin)item;
			ImageSource result;
			if (!_cachedImages.TryGetValue(val, out result))
				_cachedImages.Add(val, result = GetImage(val));
			return result;
		}
		

		public static DrawingImage GetImage(System.Drawing.Drawing2D.LineJoin join)
		{
			const double height = 1;
			const double width = 2;
			const double lineWidth = 0.375 * height;

			PenLineJoin plj;
			switch (join)
			{
				case sdd.LineJoin.Bevel:
					plj = PenLineJoin.Bevel;
					break;
				case sdd.LineJoin.Miter:
				case sdd.LineJoin.MiterClipped:
					plj = PenLineJoin.Miter;
					break;
				case sdd.LineJoin.Round:
					plj = PenLineJoin.Round;
					break;
				default:
					plj = PenLineJoin.Bevel;
					break;
			}



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
			geometryDrawing.Pen = new Pen(Brushes.Black, lineWidth) { LineJoin = plj };
			drawingGroup.Children.Add(geometryDrawing);

			drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0, 0, width, height));

			DrawingImage geometryImage = new DrawingImage(drawingGroup);

			geometryImage.Freeze();
			return geometryImage;
		}
	}
}
