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
	/// Interaction logic for LineThicknessComboBox.xaml
	/// </summary>
	public partial class LineThicknessComboBox : ThicknessImageComboBox
	{
		static Dictionary<double, ImageSource> _cachedImages = new Dictionary<double, ImageSource>();

		static readonly double[] _initialValues = new double[] { 0.001, 0.125, 0.25, 0.5, 1, 2, 3, 5, 10 };

		public LineThicknessComboBox()
		{
			InitializeComponent();

			foreach(var e in _initialValues)
				Items.Add(new ImageComboBoxItem(this,e));

			SetBinding(_nameOfValueProp);
		}
	

	#region Dependency property
		const string _nameOfValueProp = "SelectedThickness";
		public double SelectedThickness
		{
			get { var result = (double)GetValue(SelectedThicknessProperty); return result; }
			set { SetValue(SelectedThicknessProperty, value); }
		}

		public static readonly DependencyProperty SelectedThicknessProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(double), typeof(LineThicknessComboBox),
				new FrameworkPropertyMetadata(1.0, OnSelectedThicknessChanged));

		private static void OnSelectedThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((LineThicknessComboBox)obj).EhSelectedThicknessChanged(obj, args);
		}
		#endregion

		protected virtual void EhSelectedThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
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


		public static DrawingImage GetImage(double thickness)
		{
			const double nominalHeight = 24; // Height as it occurs in the combobox
			const double height = 1;
			const double width = 2;

			var drawingGroup = new DrawingGroup();
			var bounds = new Rect(0, 0, width, height);

			var geometryDrawing = new GeometryDrawing { Geometry = new RectangleGeometry(bounds) };
			geometryDrawing.Pen = new Pen(Brushes.Transparent, 0);
			drawingGroup.Children.Add(geometryDrawing);


			geometryDrawing = new GeometryDrawing { Geometry = new LineGeometry(new Point(0, height / 2), new Point(width, height / 2)) };
			geometryDrawing.Pen = new Pen(Brushes.Black, Math.Min(height, thickness*height/nominalHeight));
			drawingGroup.Children.Add(geometryDrawing);
			drawingGroup.ClipGeometry = new RectangleGeometry(bounds);

			DrawingImage geometryImage = new DrawingImage(drawingGroup);

			// Freeze the DrawingImage for performance benefits.
			geometryImage.Freeze();
			return geometryImage;
		}

	}
}
