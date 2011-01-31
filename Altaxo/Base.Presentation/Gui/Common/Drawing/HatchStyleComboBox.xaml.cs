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

using System.Drawing.Drawing2D;

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// ComboBox for <see cref="Altaxo.Graph.Gdi.HatchStyle"/>.
	/// </summary>
	public partial class HatchStyleComboBox : ImageComboBox
	{
		class CC : IValueConverter
		{
			HatchStyleComboBox _cb;
			object _originalToolTip;
			bool _hasValidationError;

			public CC(HatchStyleComboBox c)
			{
				_cb = c;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var val = (HatchStyle)value;
				return _cb._cachedItems[val];


			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return ((ImageComboBoxItem)value).Value;
			}
		}

		static Dictionary<HatchStyle, ImageSource> _cachedImages = new Dictionary<HatchStyle, ImageSource>();

		Dictionary<HatchStyle, ImageComboBoxItem> _cachedItems = new Dictionary<HatchStyle, ImageComboBoxItem>();

		static HatchStyleComboBox()
		{
		}

		public HatchStyleComboBox()
		{
			InitializeComponent();

			foreach(var e in new HatchStyle[]{ HatchStyle.Horizontal, HatchStyle.Vertical, HatchStyle.ForwardDiagonal, HatchStyle.BackwardDiagonal })
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
		private const string _nameOfValueProp = "HatchStyle";
		public HatchStyle HatchStyle
		{
			get { return (HatchStyle)GetValue(HatchStyleProperty); }
			set {	SetValue(HatchStyleProperty, value); }
		}

		public static readonly DependencyProperty HatchStyleProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(HatchStyle), typeof(HatchStyleComboBox),
				new FrameworkPropertyMetadata(HatchStyle.ForwardDiagonal, OnHatchStyleChanged));

		private static void OnHatchStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((HatchStyleComboBox)obj).EhHatchStyleChanged(obj,args);
		}
		#endregion

		protected virtual void EhHatchStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{

		}

	

		public override string GetItemText(object item)
		{
			var val = (HatchStyle)item;
			return val.ToString();
		}

		public override ImageSource GetItemImage(object item)
		{
			var val = (HatchStyle)item;
			ImageSource result;
			if (!_cachedImages.TryGetValue(val, out result))
				_cachedImages.Add(val, result = GetImage(val));
			return result;
		}
		

		public static DrawingImage GetImage(HatchStyle val)
		{
			double height = 1;
			double width = 2;

			//
			// Create the Geometry to draw.
			//
			GeometryGroup geometryGroup = new GeometryGroup();
			geometryGroup.Children.Add(new RectangleGeometry(new Rect(0,0,width,height)));

			var geometryDrawing = new GeometryDrawing() { Geometry = geometryGroup };

		

			DrawingImage geometryImage = new DrawingImage(geometryDrawing);

			// Freeze the DrawingImage for performance benefits.
			geometryImage.Freeze();
			return geometryImage;
		}
	}
}
