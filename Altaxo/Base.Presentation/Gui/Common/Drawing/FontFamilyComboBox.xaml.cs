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
        if (value is null)
          return Binding.DoNothing;
        if (value is string fontFamilyName)
        {
          return FontFamilyComboBox._cachedItems[fontFamilyName];
        }
        else
        {
          throw new ApplicationException("Unexpected type to convert: " + value.GetType());
        }
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

      public FontComboBoxItem(string familyName)
      {
        Value = familyName;
      }

      public override ImageSource Image
      {
        get
        {
          if (_imgSource is null)
            _imgSource = GetImage((string)Value);
          return _imgSource;
        }
      }

      public override string Text
      {
        get
        {
          return (string)Value;
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

      private static ImageSource GetImage(string fontFamilyName)
      {
        const double height = 1;
        const double width = 2;
        const double fontSize = 1;

        var drawingGroup = new DrawingGroup();

        var outerGeometry = new RectangleGeometry(new Rect(0, 0, width, height));

        var geometryDrawing = new GeometryDrawing() { Geometry = outerGeometry };
        geometryDrawing.Pen = new Pen(Brushes.Transparent, 0);
        drawingGroup.Children.Add(geometryDrawing);

        var fontX = WpfFontManager.GetFontX(fontFamilyName, 12, Altaxo.Drawing.FontXStyle.Regular);
        Typeface typeface = WpfFontManager.ToWpf(fontX);
        if (!typeface.TryGetGlyphTypeface(out var glyphTypeFace))
          glyphTypeFace = null;

        if (glyphTypeFace is not null)
        {
          var glyphRun = new GlyphRun(1);
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

        var geometryImage = new DrawingImage(drawingGroup);

        // Freeze the DrawingImage for performance benefits.
        geometryImage.Freeze();
        return geometryImage;
      }
    }

    #endregion Item

    private static List<FontComboBoxItem> _allItems = new List<FontComboBoxItem>();
    private static Dictionary<string, FontComboBoxItem> _cachedItems = new Dictionary<string, FontComboBoxItem>();

    public event DependencyPropertyChangedEventHandler SelectedFontFamilyNameChanged;

    static FontFamilyComboBox()
    {
      var list = new List<string>(Altaxo.Graph.Gdi.GdiFontManager.EnumerateAvailableGdiFontFamilyNames());
      list.Sort();

      // note: it seems always possible to get from a Gdi font family name and the Gdi font style a
      // System.Windows.Media.Typeface using the constructor with one string argument
      // the string argument must be the Gdi font family name, and appended to this "Bold" or "Italic" or "Bold Italic"

      foreach (var fontFamName in list)
      {
        var item = new FontComboBoxItem(fontFamName);
        _allItems.Add(item);
        _cachedItems.Add(fontFamName, item);
      }
    }

    public FontFamilyComboBox()
    {
      InitializeComponent();

      ItemsSource = _allItems;

      var binding = new Binding
      {
        Source = this,
        Path = new PropertyPath(nameof(SelectedFontFamilyName)),
        Converter = new Converter(this)
      };
      SetBinding(ComboBox.SelectedItemProperty, binding);
    }

    #region Dependency property

    public string SelectedFontFamilyName
    {
      get
      {
        return (string)GetValue(SelectedFontFamilyNameProperty);
      }
      set
      {
        SetValue(SelectedFontFamilyNameProperty, value);
      }
    }

    public static readonly DependencyProperty SelectedFontFamilyNameProperty =
        DependencyProperty.Register(nameof(SelectedFontFamilyName), typeof(string), typeof(FontFamilyComboBox),
        new FrameworkPropertyMetadata(Altaxo.Graph.Gdi.GdiFontManager.GenericSansSerifFontFamilyName, EhSelectedFontFamilyNameChanged));

    private static void EhSelectedFontFamilyNameChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((FontFamilyComboBox)obj).OnSelectedFontFamilyNameChanged(obj, args);
    }

    protected virtual void OnSelectedFontFamilyNameChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      SelectedFontFamilyNameChanged?.Invoke(obj, args);
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
