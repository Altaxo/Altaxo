#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

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
	/// ComboBox for .
	/// </summary>
	public partial class BrushComboBox : ColorComboBoxBase
	{
		protected class BrushComboBoxItem : ImageComboBoxItem
		{
			ImageSource _cachedDrawing;

			public override string Text
			{
				get
				{
					var brush = (BrushX)Value;
					string name;
					switch (brush.BrushType)
					{
						case BrushType.SolidBrush:
							name = "CustSB ";
							break;
						case BrushType.LinearGradientBrush:
							name = "CustLGB ";
							break;
						case BrushType.PathGradientBrush:
							name = "CustPGB ";
							break;
						case BrushType.TextureBrush:
							name = "CustTB ";
							break;
						case BrushType.HatchBrush:
							name = "CustHB ";
							break;
						default:
							name = "CustBrush ";
							break;
					}

					if (brush.BrushType != BrushType.TextureBrush)
						return name + brush.Color.Name;
					else
						return name.Trim();
				}
			}

			public override ImageSource Image
			{
				get
				{
					if (null == _cachedDrawing)
						_cachedDrawing = GetImage((BrushX)Value);

					return _cachedDrawing;
				}
			}
		}

		class CC : IValueConverter
		{
			BrushComboBox _cb;
			object _originalToolTip;
			bool _hasValidationError;

			public CC(BrushComboBox c)
			{
				_cb = c;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var val = (BrushX)value;
				var color = val.Color;
				if (val.BrushType == BrushType.SolidBrush && _knownColorItems.ContainsKey(color))
					return _knownColorItems[color];

				if (!_cb._lastLocalUsedBrushesDict.ContainsKey(val))
				{
					// if not found, insert a new item for this color
					var newItem = new BrushComboBoxItem() { Value = val };
					_cb._lastLocalUsedBrushes.Insert(0, newItem);
					_cb.Items.Insert(0, newItem);
					_cb._lastLocalUsedBrushesDict.Add(val, newItem);
				}
				return _cb._lastLocalUsedBrushesDict[val];
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var icbi = value as ImageComboBoxItem;
				if (null != icbi)
				{
					object val = icbi.Value;
					if (val is NamedColor)
						return new BrushX((NamedColor)val);
					else if (val is BrushX)
						return val;
				}
				return Binding.DoNothing;
			}
		}

		List<BrushComboBoxItem> _lastLocalUsedBrushes = new List<BrushComboBoxItem>();
		Dictionary<BrushX, BrushComboBoxItem> _lastLocalUsedBrushesDict = new Dictionary<BrushX, BrushComboBoxItem>();



		/// <summary>If true, the user can choose among all colors contained in the color collection, but can not choose or create other colors.</summary>
		protected bool _restrictColorChoiceToCollection;


		/// <summary>Colors that are shown as choices in the combobox.</summary>
		protected List<NamedColor> _colorCollection;

		public event DependencyPropertyChangedEventHandler SelectedBrushChanged;

		public BrushComboBox()
		{
			InitializeComponent();

			SetSelectableColors(_knownColors, false);

			var _valueBinding = new Binding();
			_valueBinding.Source = this;
			_valueBinding.Path = new PropertyPath(_nameOfValueProp);
			_valueBinding.Converter = new CC(this);
			this.SetBinding(ComboBox.SelectedItemProperty, _valueBinding);
		}








		public void SetSelectableColors(ICollection<NamedColor> value, bool restrictChoiceToThisCollection)
		{
			if (null == value)
				value = _knownColors;

			NamedColor prevSelected = this.SelectedBrush.Color;

			_colorCollection = new List<NamedColor>(value);
			_filterString = string.Empty;
			FillWithFilteredItems(_filterString, false);
			_restrictColorChoiceToCollection = restrictChoiceToThisCollection;

			bool isIncluded = _colorCollection.Contains(prevSelected);
			if (!isIncluded)
			{
				if (restrictChoiceToThisCollection && _colorCollection.Count > 0)
					prevSelected = _colorCollection[0];
				else if (!restrictChoiceToThisCollection)
					_colorCollection.Add(prevSelected);
			}
			var newBrush = this.SelectedBrush.Clone();
			newBrush.Color = prevSelected;
			this.SelectedBrush = newBrush;
		}



		#region Dependency property
		private const string _nameOfValueProp = "SelectedBrush";
		public BrushX SelectedBrush
		{
			get { return (BrushX)GetValue(SelectedBrushProperty); }
			set { SetValue(SelectedBrushProperty, value); }
		}

		public static readonly DependencyProperty SelectedBrushProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(BrushX), typeof(BrushComboBox),
				new FrameworkPropertyMetadata(new BrushX(NamedColor.Black), EhSelectedBrushChanged));

		private static void EhSelectedBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((BrushComboBox)obj).OnSelectedBrushChanged(obj, args);
		}
		#endregion

		protected virtual void OnSelectedBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (null != SelectedBrushChanged)
				SelectedBrushChanged(obj, args);
		}

		public static ImageSource GetImage(BrushX val)
		{

			const double border = 0.1;
			const double height = 1;
			const double width = 2;
			//
			// Create the Geometry to draw.
			//

			if (val.BrushType == BrushType.SolidBrush)
			{

				if (null == _checkerBrush)
					_checkerBrush = CreateCheckerBrush(Math.Min(width, height) / 2);

				var drawingGroup = new DrawingGroup();

				var geometryDrawing = new GeometryDrawing() { Geometry = new RectangleGeometry(new Rect(0, 0, width, height)) };
				geometryDrawing.Brush = Brushes.Black;
				drawingGroup.Children.Add(geometryDrawing);

				var innerRect = new Rect(border, border, width - 2 * border, height - 2 * border);

				geometryDrawing = new GeometryDrawing() { Geometry = new RectangleGeometry(innerRect) };
				geometryDrawing.Brush = _checkerBrush;
				drawingGroup.Children.Add(geometryDrawing);

				geometryDrawing = new GeometryDrawing() { Geometry = new RectangleGeometry(innerRect) };
				geometryDrawing.Brush = new SolidColorBrush(GuiHelper.ToWpf(val.Color));
				drawingGroup.Children.Add(geometryDrawing);

				DrawingImage geometryImage = new DrawingImage(drawingGroup);

				// Freeze the DrawingImage for performance benefits.
				geometryImage.Freeze();
				return geometryImage;
			}
			else
			{
				return GuiHelper.ToWpf(val, 16, 16);
			}
		}



		private void EhChooseTransparencyFromContextMenu(object sender, RoutedEventArgs e)
		{
			if (_restrictColorChoiceToCollection)
				return;

			var a = (255 * (100 - int.Parse((string)((MenuItem)sender).Tag))) / 100;
			var newColor = SelectedBrush.Color.Color.ToAlphaValue((byte)a);
			var newBrush = SelectedBrush.Clone();
			newBrush.Color = new NamedColor(newColor);
			SelectedBrush = newBrush;
		}


		#region Special treatment for key processing

		string _filterString = string.Empty;

		bool FillWithFilteredItems(string filterString, bool onlyIfItemsRemaining)
		{
			List<BrushComboBoxItem> lastUsed;

			if (_restrictColorChoiceToCollection)
				lastUsed = new List<BrushComboBoxItem>();
			else
				lastUsed = GetFilteredList(_lastLocalUsedBrushes, filterString);

			var known = GetFilteredList(_colorCollection, filterString);

			if ((lastUsed.Count + known.Count) > 0 || !onlyIfItemsRemaining)
			{
				Items.Clear();
				foreach (var item in lastUsed)
					Items.Add(item);
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
						FillWithFilteredItems(_filterString, false);
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
				FillWithFilteredItems(_filterString, false);

				this.SelectedItem = selItem;
			}
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
			_cachedItems.Add(c, newItem);
			return newItem;
		}

		private static List<BrushComboBoxItem> GetFilteredList(List<BrushComboBoxItem> originalList, string filterString)
		{
			var result = new List<BrushComboBoxItem>();
			filterString = filterString.ToLowerInvariant();
			foreach (var item in originalList)
			{
				if (item.Text.ToLowerInvariant().StartsWith(filterString))
					result.Add(item);
			}
			return result;
		}

		#endregion

		private void EhShowCustomBrushDialog(object sender, RoutedEventArgs e)
		{
			if (_restrictColorChoiceToCollection)
				return;

			var localBrush = this.SelectedBrush.Clone();
			var ctrl = new BrushControllerAdvanced();
			ctrl.InitializeDocument(localBrush);
			if (Current.Gui.ShowDialog(ctrl, "Edit brush properties", false))
				this.SelectedBrush = (BrushX)ctrl.ModelObject;
		}

		private void EhShowCustomColorDialog(object sender, RoutedEventArgs e)
		{
			if (_restrictColorChoiceToCollection)
				return;

			Color color = GuiHelper.ToWpf(SelectedBrush.Color);
			ColorController ctrl = new ColorController(color);
			ctrl.ViewObject = new ColorPickerControl(color);
			if (Current.Gui.ShowDialog(ctrl, "Select a color", false))
			{
				Color c = (Color)ctrl.ModelObject;
				var namedColor = NamedColor.FromArgb(c.A, c.R, c.G, c.B);
				this.SelectedBrush = new BrushX(namedColor);
			}
		}
	}
}
