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

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using sd = System.Drawing;

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Interaction logic for LineJoinComboBox.xaml
	/// </summary>
	public partial class FontFamilyComboBox : ImageComboBox
	{
		#region Converter

		private class Converter : IValueConverter
		{
			private FontFamilyComboBox _cb;

			public Converter(FontFamilyComboBox c)
			{
				_cb = c;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				if (value == null)
					return Binding.DoNothing;

				if (value is sd.FontFamily)
				{
					var val = (sd.FontFamily)value;
					return FontFamilyComboBox._cachedItems[val.Name];
				}
				else if (value is FontFamily)
				{
					var val = (FontFamily)value;
					return FontFamilyComboBox._cachedItems[val.Source];
				}
				else
					throw new ApplicationException("Unexpected type to convert: " + value.GetType());
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return ((FontComboBoxItem)value).Value;
			}
		}

		#endregion Converter

		#region Item

		public class FontComboBoxItem : ImageComboBoxItem
		{
			private static char[] _defaultChars;

			private ImageSource _imgSource;

			static FontComboBoxItem()
			{
				var chars = new List<char>();
				for (int c = 0x41; c <= 0xFF; ++c)
					chars.Add((char)c);
				for (int c = 0x20; c < 0x41; ++c)
					chars.Add((char)c);
				_defaultChars = chars.ToArray();
			}

			public FontComboBoxItem(sd.FontFamily fontFamily)
			{
				Value = fontFamily;
			}

			public override ImageSource Image
			{
				get
				{
					if (null == _imgSource)
						_imgSource = GetImage((sd.FontFamily)Value);
					return _imgSource;
				}
			}

			public override string Text
			{
				get
				{
					return ((sd.FontFamily)Value).Name;
				}
			}

			private static char[] GetDisplayText(GlyphTypeface glyphTypeFace)
			{
				string text = glyphTypeFace.SampleTexts[System.Globalization.CultureInfo.CurrentUICulture];
				if (!string.IsNullOrEmpty(text))
					return text.ToCharArray();

				// else use the default characters, but test if they are present in the typeface
				var chars = new List<char>();
				for (int i = 0; i < _defaultChars.Length && chars.Count < 4; i++)
				{
					var c = _defaultChars[i];
					if (glyphTypeFace.CharacterToGlyphMap.ContainsKey(c))
						chars.Add(c);
				}
				return chars.ToArray();
			}

			private static ImageSource GetImage(sd.FontFamily fontFamily)
			{
				const double border = 0.1;
				const double height = 1;
				const double width = 2;
				const double fontSize = 1;

				var drawingGroup = new DrawingGroup();

				var outerGeometry = new RectangleGeometry(new Rect(0, 0, width, height));

				var geometryDrawing = new GeometryDrawing() { Geometry = outerGeometry };
				geometryDrawing.Pen = new Pen(Brushes.Transparent, 0);
				drawingGroup.Children.Add(geometryDrawing);

				Typeface typeface = new Typeface(fontFamily.Name);
				GlyphTypeface glyphTypeFace;
				if (!typeface.TryGetGlyphTypeface(out glyphTypeFace))
					glyphTypeFace = null;

				if (null != glyphTypeFace)
				{
					var glyphRun = new GlyphRun();
					((System.ComponentModel.ISupportInitialize)glyphRun).BeginInit();
					glyphRun.GlyphTypeface = glyphTypeFace;
					glyphRun.FontRenderingEmSize = fontSize;
					var textChars = GetDisplayText(glyphTypeFace);
					glyphRun.Characters = textChars;

					ushort[] glyphIndices = new ushort[textChars.Length];
					double[] advanceWidths = new double[textChars.Length];

					for (int i = 0; i < textChars.Length; ++i)
					{
						int codePoint = textChars[i];
						ushort glyphIndex = glyphTypeFace.CharacterToGlyphMap[codePoint];
						double glyphWidht = glyphTypeFace.AdvanceWidths[glyphIndex];
						glyphIndices[i] = glyphIndex;
						advanceWidths[i] = glyphWidht * fontSize;
					}

					if (glyphIndices.Length > 0)
					{
						glyphRun.GlyphIndices = glyphIndices;
						glyphRun.AdvanceWidths = advanceWidths;
						glyphRun.BaselineOrigin = new Point(0, glyphTypeFace.Baseline * fontSize);
						((System.ComponentModel.ISupportInitialize)glyphRun).EndInit();

						var glyphRunDrawing = new GlyphRunDrawing(Brushes.Black, glyphRun);
						drawingGroup.Children.Add(glyphRunDrawing);
					}
				}

				drawingGroup.ClipGeometry = outerGeometry;

				DrawingImage geometryImage = new DrawingImage(drawingGroup);

				// Freeze the DrawingImage for performance benefits.
				geometryImage.Freeze();
				return geometryImage;
			}
		}

		#endregion Item

		private static List<FontComboBoxItem> _allItems = new List<FontComboBoxItem>();
		private static Dictionary<string, FontComboBoxItem> _cachedItems = new Dictionary<string, FontComboBoxItem>();

		private static HashSet<string> _gdiFontFamilyNames = new HashSet<string>();

		private static GdiToWpfBitmap _interopBitmap;

		private static sd.FontFamily GenericSansSerif;

		public event DependencyPropertyChangedEventHandler SelectedFontFamilyChanged;

		static FontFamilyComboBox()
		{
			// GenericSansSerif = new FontFamily(sd.FontFamily.GenericSansSerif.Name);
			GenericSansSerif = sd.FontFamily.GenericSansSerif;

			var list = new List<sd.FontFamily>(sd.FontFamily.Families);
			list.Sort((x, y) => string.Compare(x.Name, y.Name));

			// note: it seems always possible to get from a Gdi font family name and the Gdi font style a
			// System.Windows.Media.Typeface using the constructor with one string argument
			// the string argument must be the Gdi font family name, and appended to this "Bold" or "Italic" or "Bold Italic"

			foreach (var fontFam in list)
			{
				var item = new FontComboBoxItem(fontFam);
				_allItems.Add(item);
				_cachedItems.Add(fontFam.Name, item);
			}
		}

		public FontFamilyComboBox()
		{
			InitializeComponent();

			this.ItemsSource = _allItems;

			var binding = new Binding();
			binding.Source = this;
			binding.Path = new PropertyPath(_nameOfValueProp);
			binding.Converter = new Converter(this);
			this.SetBinding(ComboBox.SelectedItemProperty, binding);
		}

		#region Dependency property

		private const string _nameOfValueProp = "SelectedFontFamily";

		public FontFamily SelectedWpfFontFamily
		{
			get
			{
				var gdiFF = (sd.FontFamily)GetValue(SelectedFontFamilyProperty);
				return new System.Windows.Media.Typeface(gdiFF.Name).FontFamily;
			}
			set
			{
			}
		}

		public sd.FontFamily SelectedFontFamily
		{
			get
			{
				return (sd.FontFamily)GetValue(SelectedFontFamilyProperty);
			}
			set
			{
				SetValue(SelectedFontFamilyProperty, value);
			}
		}

		public static readonly DependencyProperty SelectedFontFamilyProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(sd.FontFamily), typeof(FontFamilyComboBox),
				new FrameworkPropertyMetadata(GenericSansSerif, EhSelectedFontFamilyChanged));

		private static void EhSelectedFontFamilyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((FontFamilyComboBox)obj).OnSelectedFontFamilyChanged(obj, args);
		}

		protected virtual void OnSelectedFontFamilyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (null != SelectedFontFamilyChanged)
				SelectedFontFamilyChanged(obj, args);
		}

		#endregion Dependency property

		/*
		public static ImageSource GetImage(sd.FontFamily join)
		{
			const int bmpHeight = 24;
			const int bmpWidth = 48;
			const double nominalHeight = 24; // height of a combobox item
			const double nominalWidth = (nominalHeight * bmpWidth) / bmpHeight;
			const double lineWidth = bmpHeight * 0.4;

			if (null == _interopBitmap)
				_interopBitmap = new GdiToWpfBitmap(bmpWidth, bmpHeight);

			using (var grfx = _interopBitmap.BeginGdiPainting())
			{
				grfx.CompositingMode = sdd.CompositingMode.SourceCopy;
				grfx.FillRectangle(System.Drawing.Brushes.Transparent, 0, 0, bmpWidth, bmpHeight);
				using (var font = new sd.Font(join, bmpHeight, sd.FontStyle.Regular))
				{
					// grfx.DrawString("Abc", font, sd.Brushes.Black, 0, (bmpHeight * 3) / 4);
				}
				grfx.DrawLine(sd.Pens.Black, 0, 0, bmpWidth, bmpHeight);
				_interopBitmap.EndGdiPainting();
			}

			var img = new WriteableBitmap(_interopBitmap.WpfBitmapSource);
			img.Freeze();
			return img;
		}
		*/
	}
}