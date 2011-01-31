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

using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// ComboBox for <see cref="Altaxo.Graph.Gdi.Color"/>.
	/// </summary>
	public partial class ColorComboBox : ImageComboBox
	{
		class ColorComboBoxItem : ImageComboBoxItem
		{
			DrawingImage _cachedDrawing;

			public override string Text
			{
				get
				{
					return GetColorName((Color)Value);
				}
			}

			public override ImageSource Image
			{
				get
				{
					if (null == _cachedDrawing)
						_cachedDrawing = GetImage((Color)Value);

					return _cachedDrawing;
				}
			}
		}

		class CC : IValueConverter
		{
			ColorComboBox _cb;
			object _originalToolTip;
			bool _hasValidationError;

			public CC(ColorComboBox c)
			{
				_cb = c;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var val = (Color)value;
				if (_knownColorItems.ContainsKey(val))
					return _knownColorItems[val];

				if (!_cb._lastLocalUsedColorsDict.ContainsKey(val))
				{
					// if not found, insert a new item for this color
					var newItem = new ColorComboBoxItem() { Value = val };
					_cb._lastLocalUsedColors.Insert(0, newItem);
					_cb.Items.Insert(0, newItem);
					_cb._lastLocalUsedColorsDict.Add(val, newItem );
				}
				return _cb._lastLocalUsedColorsDict[val];
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				if (null != value)
					return ((ImageComboBoxItem)value).Value;
				else
					return Binding.DoNothing;
			}
		}

		static Dictionary<Color, string> _knownColorNames = new Dictionary<Color, string>();
		static Dictionary<Color, ColorComboBoxItem> _knownColorItems = new Dictionary<Color, ColorComboBoxItem>();
		static List<ColorComboBoxItem> _knownColors = new List<ColorComboBoxItem>();
		static Dictionary<Color, ImageSource> _cachedImages = new Dictionary<Color, ImageSource>();

		List<ColorComboBoxItem> _lastLocalUsedColors = new List<ColorComboBoxItem>();
		Dictionary<Color, ColorComboBoxItem> _lastLocalUsedColorsDict = new Dictionary<Color, ColorComboBoxItem>();

		static Brush _checkerBrush;

		Dictionary<Color, ImageComboBoxItem> _cachedItems = new Dictionary<Color, ImageComboBoxItem>();

		ColorType _colorType;

		static ColorComboBox()
		{

			// Enumerate constant colors from the Colors class
			Type colorsType = typeof(Colors);
			var pis = colorsType.GetProperties();
			foreach (var pi in pis)
			{
				Color c = (Color)pi.GetValue(null, null);
				var item = new ColorComboBoxItem() { Value = c };
				_knownColors.Add(item);
				if (!_knownColorNames.ContainsKey(c))
				{
					_knownColorNames.Add(c, pi.Name);
					_knownColorItems.Add(c, item);
				}

			}
		}

		public ColorType ColorType
		{
			get
			{
				return _colorType;
			}
			set
			{
				_colorType = value;
			}
		}

		public static string GetColorName(Color c)
		{
			if (_knownColorNames.ContainsKey(c))
				return _knownColorNames[c];
			var crgb = Color.FromRgb(c.R, c.G, c.B);
			if (_knownColorNames.ContainsKey(crgb))
			{
				var name = _knownColorNames[crgb];
				int transp = ((255 - c.A) * 100) / 255;
				name += string.Format(" {0}%", transp);
				return name;
			}
			return c.ToString();
		}

		public ColorComboBox()
		{
			InitializeComponent();

			foreach(var e in _knownColors)
			{
				try
				{
					Items.Add(e);
				}
				catch (Exception ex)
				{
				}
			}

			var _valueBinding = new Binding();
			_valueBinding.Source = this;
			_valueBinding.Path = new PropertyPath(_nameOfValueProp);
			_valueBinding.Converter = new CC(this);
			this.SetBinding(ComboBox.SelectedItemProperty, _valueBinding);
		}

		#region Dependency property
		private const string _nameOfValueProp = "SelectedColor";
		public Color SelectedColor
		{
			get { return (Color)GetValue(SelectedColorProperty); }
			set {	SetValue(SelectedColorProperty, value); }
		}

		public static readonly DependencyProperty SelectedColorProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(Color), typeof(ColorComboBox),
				new FrameworkPropertyMetadata(Colors.Black, OnSelectedColorChanged));

		private static void OnSelectedColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((ColorComboBox)obj).EhSelectedColorChanged(obj,args);
		}
		#endregion

		protected virtual void EhSelectedColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{

		}

	

		public override string GetItemText(object item)
		{
			var val = (Color)item;
			return val.ToString();
		}

		public override ImageSource GetItemImage(object item)
		{
			var val = (Color)item;
			ImageSource result;
			if (!_cachedImages.TryGetValue(val, out result))
				_cachedImages.Add(val, result = GetImage(val));
			return result;
		}


		public static Brush CreateCheckerBrush(double checkerRepeatLength)
		{
			DrawingBrush checkerBrush = new DrawingBrush();

			GeometryDrawing backgroundSquare =
					new GeometryDrawing(
							Brushes.White,
							null,
							new RectangleGeometry(new Rect(0, 0, checkerRepeatLength, checkerRepeatLength)));

			double c2 = checkerRepeatLength / 2;

			GeometryGroup aGeometryGroup = new GeometryGroup();
			aGeometryGroup.Children.Add(new RectangleGeometry(new Rect(0, 0, c2, c2)));
			aGeometryGroup.Children.Add(new RectangleGeometry(new Rect(c2, c2, c2, c2)));

			GeometryDrawing checkers = new GeometryDrawing(Brushes.Black, null, aGeometryGroup);

			DrawingGroup checkersDrawingGroup = new DrawingGroup();
			checkersDrawingGroup.Children.Add(backgroundSquare);
			checkersDrawingGroup.Children.Add(checkers);

			checkerBrush.Drawing = checkersDrawingGroup;
			checkerBrush.Viewport = new Rect(0, 0, 0.5, 0.5);
			checkerBrush.TileMode = TileMode.Tile;

			return checkerBrush;
		}

		public static DrawingImage GetImage(Color val)
		{
		
			const double border = 0.1;
			const double height = 1;
			const double width = 2;
			//
			// Create the Geometry to draw.
			//

			if (null == _checkerBrush)
				_checkerBrush = CreateCheckerBrush(Math.Min(width, height) / 2);

			var drawingGroup = new DrawingGroup();

			var geometryDrawing = new GeometryDrawing() { Geometry = new RectangleGeometry(new Rect(0,0,width,height)) };
			geometryDrawing.Brush = Brushes.Black;
			drawingGroup.Children.Add(geometryDrawing);

			var innerRect = new Rect(border, border, width - 2 * border, height - 2 * border);

			geometryDrawing = new GeometryDrawing() { Geometry = new RectangleGeometry(innerRect) };
			geometryDrawing.Brush = _checkerBrush;
			drawingGroup.Children.Add(geometryDrawing);

			geometryDrawing = new GeometryDrawing() { Geometry = new RectangleGeometry(innerRect) };
			geometryDrawing.Brush = new SolidColorBrush(val);
			drawingGroup.Children.Add(geometryDrawing);

			DrawingImage geometryImage = new DrawingImage(drawingGroup);

			// Freeze the DrawingImage for performance benefits.
			geometryImage.Freeze();
			return geometryImage;
		}

		private void EhShowCustomColorDialog(object sender, RoutedEventArgs e)
		{

		}

		private void EhChooseTransparencyFromContextMenu(object sender, RoutedEventArgs e)
		{
			var a = (255 * (100 - int.Parse((string)((MenuItem)sender).Tag))) / 100;
			Color o = SelectedColor;
			Color newColor = Color.FromArgb((byte)a, o.R, o.G, o.B);
			SelectedColor = newColor;
		}


		#region Special treatment for key processing

		string _filterString = string.Empty;

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (this.IsDropDownOpen)
			{
				Key pressedKey = e.Key;
				string pressedString = new KeyConverter().ConvertToInvariantString(pressedKey);
				char pressedChar = pressedString.Length == 1 ? pressedString[0] : '\0';

				if (char.IsLetterOrDigit(pressedChar))
				{
					string filterString = _filterString + pressedChar;

					var lastUsed = GetFilteredList(_lastLocalUsedColors, filterString);
					var known = GetFilteredList(_knownColors, filterString);
					if ((lastUsed.Count + known.Count) > 0)
					{
						_filterString = filterString;
						Items.Clear();
						foreach (var item in lastUsed)
							Items.Add(item);
						foreach (var item in known)
							Items.Add(item);
					}
				}
				else if (pressedKey == Key.Delete || pressedKey == Key.Back)
				{
					if (_filterString.Length > 0)
					{
						_filterString = _filterString.Substring(0, _filterString.Length - 1);
						var lastUsed = GetFilteredList(_lastLocalUsedColors, _filterString);
						var known = GetFilteredList(_knownColors, _filterString);
						Items.Clear();
						foreach (var item in lastUsed)
							Items.Add(item);
						foreach (var item in known)
							Items.Add(item);
					}
				}
			}
			base.OnKeyDown(e);
		}

		protected override void OnDropDownClosed(EventArgs e)
		{
			base.OnDropDownClosed(e);

			if (_filterString.Length > 0)
			{
				var selItem = this.SelectedItem;

				_filterString = string.Empty;
				Items.Clear();
				foreach (var item in _lastLocalUsedColors)
					Items.Add(item);
				foreach (var item in _knownColors)
					Items.Add(item);

				this.SelectedItem = selItem;
			}
		}

		private static List<ColorComboBoxItem> GetFilteredList(List<ColorComboBoxItem> originalList, string filterString)
		{
			var result = new List<ColorComboBoxItem>();
			filterString = filterString.ToLowerInvariant();
			foreach (var item in originalList)
			{
				if (item.Text.ToLowerInvariant().StartsWith(filterString))
					result.Add(item);
			}
			return result;
		}

		#endregion
	}
}
