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

using sd = System.Drawing;
using sdd = System.Drawing.Drawing2D;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Interaction logic for LineJoinComboBox.xaml
	/// </summary>
	public partial class FontStyleComboBox : ImageComboBox
	{
		#region Converter

		class Converter : IValueConverter
		{
			FontStyleComboBox _cb;

			public Converter(FontStyleComboBox c)
			{
				_cb = c;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var val = (sd.FontStyle)value;
				return _cb._cachedItems[val];
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return ((ImageComboBoxItem)value).Value;
			}
		}

		#endregion


		static Dictionary<sd.FontStyle, ImageSource> _cachedImages = new Dictionary<sd.FontStyle, ImageSource>();
		Dictionary<sd.FontStyle, ImageComboBoxItem> _cachedItems = new Dictionary<sd.FontStyle, ImageComboBoxItem>();
		static GdiToWpfBitmap _interopBitmap;

		public event DependencyPropertyChangedEventHandler SelectedFontStyleChanged;


		static FontStyleComboBox()
		{
		}

		public FontStyleComboBox()
		{
			InitializeComponent();
			SetDefaultValues();

			var binding = new Binding();
			binding.Source = this;
			binding.Path = new PropertyPath(_nameOfValueProp);
			binding.Converter = new Converter(this);
			this.SetBinding(ComboBox.SelectedItemProperty, binding);
		}

		void SetDefaultValues()
		{
			foreach (sd.FontStyle ff in Enum.GetValues(typeof(sd.FontStyle)))
			{
				var item = new ImageComboBoxItem(this, ff);
				_cachedItems.Add(ff, item);
				this.Items.Add(item);
			}
		}



		#region Dependency property
		private const string _nameOfValueProp = "SelectedFontStyle";
		public sd.FontStyle SelectedFontStyle
		{
			get { return (sd.FontStyle)GetValue(SelectedFontStyleProperty); }
			set { SetValue(SelectedFontStyleProperty, value); }
		}

		public static readonly DependencyProperty SelectedFontStyleProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(sd.FontStyle), typeof(FontStyleComboBox),
				new FrameworkPropertyMetadata(sd.FontStyle.Regular, EhSelectedFontStyleChanged));

		private static void EhSelectedFontStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((FontStyleComboBox)obj).OnSelectedFontStyleChanged(obj, args);
		}

		protected virtual void OnSelectedFontStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (null != SelectedFontStyleChanged)
				SelectedFontStyleChanged(obj, args);
		}
		#endregion



		public override string GetItemText(object item)
		{
			var value = (sd.FontStyle)item;
			return value.ToString();
		}


		public override ImageSource GetItemImage(object item)
		{
			var val = (sd.FontStyle)item;
			ImageSource result;
			if (!_cachedImages.TryGetValue(val, out result))
				_cachedImages.Add(val, result = GetImage(val));

			return result;
		}

		public static ImageSource GetImage(sd.FontStyle join)
		{


			const int bmpHeight = 24;
			const int bmpWidth = 48;
			const double nominalHeight = 24; // height of a combobox item

			if (null == _interopBitmap)
				_interopBitmap = new GdiToWpfBitmap(bmpWidth, bmpHeight);

			var grfx = _interopBitmap.GdiGraphics;

			grfx.CompositingMode = sdd.CompositingMode.SourceCopy;
			grfx.FillRectangle(System.Drawing.Brushes.Transparent, 0, 0, bmpWidth, bmpHeight);
			using (var font = new sd.Font(sd.FontFamily.GenericSansSerif, bmpHeight, sd.FontStyle.Regular))
			{
				//grfx.DrawString("Abc", font, sd.Brushes.Black, 0, (bmpHeight * 3) / 4);
			}

			var img = new WriteableBitmap(_interopBitmap.WpfBitmap);
			img.Freeze();
			return img;
		}
	}
}
