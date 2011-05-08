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
	public partial class ColorComboBox : ColorComboBoxBase
	{
		
		/// <summary>Converts the <see cref="AxoColor"/> to a <see cref="ColorComboBoxItem"/> and vice versa.</summary>
		class CC : IValueConverter
		{
			ColorComboBox _cb;
			object _originalToolTip;
			bool _hasValidationError;

			public CC(ColorComboBox c)
			{
				_cb = c;
			}

			/// <summary>
			/// Converts a <see cref="AxoColor"/> to a <see cref="ColorComboBoxItem"/>.
			/// </summary>
			/// <param name="value"></param>
			/// <param name="targetType"></param>
			/// <param name="parameter"></param>
			/// <param name="culture"></param>
			/// <returns></returns>
			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var val = (NamedColor)value;
				if (_knownColorItems.ContainsKey(val))
					return _knownColorItems[val];

				if (!_cb._lastLocalUsedColorsDict.ContainsKey(val))
				{
					// if not found, insert a new item for this color
					var newItem = new ColorComboBoxItem() { Value = val };
					_cb._lastLocalUsedColors.Insert(0, val);
					_cb.Items.Insert(0, newItem);
					_cb._lastLocalUsedColorsDict.Add(val, newItem );
				}
				return _cb._lastLocalUsedColorsDict[val];
			}

			/// <summary>
			/// Converts the <see cref="ColorComboBoxItem"/> to a <see cref="AxoColor"/>.
			/// </summary>
			/// <param name="value"></param>
			/// <param name="targetType"></param>
			/// <param name="parameter"></param>
			/// <param name="culture"></param>
			/// <returns></returns>
			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				if (null != value)
					return ((ImageComboBoxItem)value).Value;
				else
					return Binding.DoNothing;
			}
		}

		/// <summary>Dictionary of cached images for each <see cref="AxoColor"/>.</summary>
		static Dictionary<AxoColor, ImageSource> _cachedImages = new Dictionary<AxoColor, ImageSource>();

		List<NamedColor> _lastLocalUsedColors = new List<NamedColor>();
		Dictionary<NamedColor, ColorComboBoxItem> _lastLocalUsedColorsDict = new Dictionary<NamedColor, ColorComboBoxItem>();

		ColorType _colorType;

		/// <summary>If true, the user can choose among all colors contained in the color collection, but can not choose or create other colors.</summary>
		protected bool _restrictColorChoiceToCollection;
	

		/// <summary>Colors that are shown as choices in the combobox.</summary>
		protected List<NamedColor> _colorCollection;

		public event DependencyPropertyChangedEventHandler SelectedColorChanged;


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

		public bool RestrictColorChoiceToCollection
		{
			get
			{
				return _restrictColorChoiceToCollection; 
			}
			set 
			{
				_restrictColorChoiceToCollection = value; 
			}
		}

		public ICollection<NamedColor> SelectableColors
		{
			set
			{
				_colorCollection = new List<NamedColor>(value);
				_filterString = string.Empty;
				FillWithFilteredItems(_filterString, false);
			}
		}

		public ColorComboBox()
		{
			InitializeComponent();

			this.SelectableColors = _knownColors;

			var _valueBinding = new Binding();
			_valueBinding.Source = this;
			_valueBinding.Path = new PropertyPath(_nameOfValueProp);
			_valueBinding.Converter = new CC(this);
			this.SetBinding(ComboBox.SelectedItemProperty, _valueBinding);
		}

		#region Dependency property
		private const string _nameOfValueProp = "SelectedColor";
		public NamedColor SelectedColor
		{
			get { return (NamedColor)GetValue(SelectedColorProperty); }
			set {	SetValue(SelectedColorProperty, value); }
		}

		public Color SelectedWpfColor
		{
			get { return GuiHelper.ToWpf(((NamedColor)GetValue(SelectedColorProperty)).Color); }
			set
			{
				AxoColor c = GuiHelper.ToAxo(value);
				SetValue(SelectedColorProperty, new NamedColor(c,NamedColor.GetColorName(c))); 
			}
		}

		public System.Drawing.Color SelectedGdiColor
		{
			get { return GuiHelper.ToGdi(((NamedColor)GetValue(SelectedColorProperty)).Color); }
			set
			{
				AxoColor c = GuiHelper.ToAxo(value);
				SetValue(SelectedColorProperty, new NamedColor(c, NamedColor.GetColorName(c)));
			}
		}

		public static readonly DependencyProperty SelectedColorProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(NamedColor), typeof(ColorComboBox),
				new FrameworkPropertyMetadata(new NamedColor(KnownAxoColors.Black), EhSelectedColorChanged));

		private static void EhSelectedColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((ColorComboBox)obj).OnSelectedColorChanged(obj,args);
		}
		#endregion

		protected virtual void OnSelectedColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (null != SelectedColorChanged)
				SelectedColorChanged(obj, args);
		}

	

		public override string GetItemText(object item)
		{
			var val = (NamedColor)item;
			return val.Name;
		}

		public override ImageSource GetItemImage(object item)
		{
			var val = (NamedColor)item;
			ImageSource result;
			if (!_cachedImages.TryGetValue(val.Color, out result))
				_cachedImages.Add(val.Color, result = GetImage(val));
			return result;
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
			if (_restrictColorChoiceToCollection)
				return;

			ColorController ctrl = new ColorController(SelectedWpfColor);
			ctrl.ViewObject = new ColorPickerControl(SelectedWpfColor);
			if (Current.Gui.ShowDialog(ctrl, "Select a color", false))
				this.SelectedWpfColor = (Color)ctrl.ModelObject;
		}

		private void EhChooseTransparencyFromContextMenu(object sender, RoutedEventArgs e)
		{
			if (_restrictColorChoiceToCollection)
				return;

			var a = (255 * (100 - int.Parse((string)((MenuItem)sender).Tag))) / 100;
			AxoColor newColor = SelectedColor.Color.ToAlphaValue((byte)a);
			SelectedColor = new NamedColor(newColor);
		}


		#region Special treatment for key processing

		string _filterString = string.Empty;


		bool FillWithFilteredItems(string filterString, bool onlyIfItemsRemaining)
		{
			List<NamedColor> lastUsed;
			
			if (_restrictColorChoiceToCollection)
				lastUsed = new List<NamedColor>();
			else
				lastUsed = GetFilteredList(_lastLocalUsedColors, filterString);

			var known = GetFilteredList(_colorCollection, filterString);
			
			if ((lastUsed.Count + known.Count) > 0 || !onlyIfItemsRemaining)
			{
				Items.Clear();
				foreach (var item in lastUsed)
					Items.Add(GetCBItem(item));
				foreach (var item in known)
					Items.Add(GetCBItem(item));

				return true;
			}

			return false;
		}

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

					if (FillWithFilteredItems(filterString, true))
						_filterString = filterString;
				}
				else if (pressedKey == Key.Delete || pressedKey == Key.Back)
				{
					if (_filterString.Length > 0)
					{
						_filterString = _filterString.Substring(0, _filterString.Length - 1);
						FillWithFilteredItems(_filterString,false);
					}
				}
			}
			base.OnKeyDown(e);
		}

		private ImageComboBoxItem GetCBItem(NamedColor c)
		{
			ImageComboBoxItem result;
			if (_cachedItems.TryGetValue(c, out result))
				return result;

			ColorComboBoxItem result1;
			if (_knownColorItems.TryGetValue(c, out result1))
				return result1;
			
			var newItem = new ColorComboBoxItem { Value = c };
			_cachedItems.Add(c,newItem);
			return newItem;
		}

		protected override void OnDropDownClosed(EventArgs e)
		{
			base.OnDropDownClosed(e);

			if (_filterString.Length > 0)
			{
				var selItem = this.SelectedItem;

				_filterString = string.Empty;
				FillWithFilteredItems(_filterString,false);

				this.SelectedItem = selItem;
			}
		}

	

		#endregion
	}
}
