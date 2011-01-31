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
	/// ComboBox for <see cref="Altaxo.Graph.Gdi.BrushType"/>.
	/// </summary>
	public partial class BrushTypeComboBox : ImageComboBox
	{
		class CC : IValueConverter
		{
			BrushTypeComboBox _cb;
			object _originalToolTip;
			bool _hasValidationError;

			public CC(BrushTypeComboBox c)
			{
				_cb = c;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var val = (BrushType)value;
				return _cb._cachedItems[val];


			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return ((ImageComboBoxItem)value).Value;
			}
		}

		static Dictionary<BrushType, ImageSource> _cachedImages = new Dictionary<BrushType, ImageSource>();

		Dictionary<BrushType, ImageComboBoxItem> _cachedItems = new Dictionary<BrushType, ImageComboBoxItem>();

		static BrushTypeComboBox()
		{
		}

		public BrushTypeComboBox()
		{
			InitializeComponent();

			_cachedItems.Add(BrushType.SolidBrush, new ImageComboBoxItem(this, BrushType.SolidBrush));
			_cachedItems.Add(BrushType.HatchBrush, new ImageComboBoxItem(this, BrushType.HatchBrush));
			_cachedItems.Add(BrushType.LinearGradientBrush, new ImageComboBoxItem(this, BrushType.LinearGradientBrush)); // trick: MiterClipped is projected to Miter item here
			_cachedItems.Add(BrushType.PathGradientBrush, new ImageComboBoxItem(this, BrushType.PathGradientBrush));
			_cachedItems.Add(BrushType.TextureBrush, new ImageComboBoxItem(this, BrushType.TextureBrush));


			Items.Add(_cachedItems[BrushType.SolidBrush]);
			Items.Add(_cachedItems[BrushType.LinearGradientBrush]);
			Items.Add(_cachedItems[BrushType.PathGradientBrush]);
			Items.Add(_cachedItems[BrushType.TextureBrush]);


			var _valueBinding = new Binding();
			_valueBinding.Source = this;
			_valueBinding.Path = new PropertyPath(_nameOfValueProp);
			_valueBinding.Converter = new CC(this);
			this.SetBinding(ComboBox.SelectedItemProperty, _valueBinding);
		}

		#region Dependency property
		private const string _nameOfValueProp = "BrushType";
		public BrushType BrushType
		{
			get { return (BrushType)GetValue(BrushTypeProperty); }
			set {	SetValue(BrushTypeProperty, value); }
		}

		public static readonly DependencyProperty BrushTypeProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(BrushType), typeof(BrushTypeComboBox),
				new FrameworkPropertyMetadata(OnBrushTypeChanged));

		private static void OnBrushTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((BrushTypeComboBox)obj).EhBrushTypeChanged(obj,args);
		}
		#endregion

		protected virtual void EhBrushTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{

		}

	

		public override string GetItemText(object item)
		{
			var val = (BrushType)item;
			return val.ToString();
		}

		public override ImageSource GetItemImage(object item)
		{
			var val = (BrushType)item;
			ImageSource result;
			if (!_cachedImages.TryGetValue(val, out result))
				_cachedImages.Add(val, result = GetImage(val));
			return result;
		}
		

		public static DrawingImage GetImage(BrushType val)
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
				case BrushType.SolidBrush:
					geometryDrawing.Brush = new SolidColorBrush(Colors.Black);
					break;
				case BrushType.LinearGradientBrush:
					geometryDrawing.Brush = new LinearGradientBrush(Colors.Black, Colors.White, 0);
					break;
				case BrushType.PathGradientBrush:
					geometryDrawing.Brush = new RadialGradientBrush(Colors.Black, Colors.White);
					break;
				case BrushType.TextureBrush:
					geometryDrawing.Brush = new SolidColorBrush(Colors.Black);
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
