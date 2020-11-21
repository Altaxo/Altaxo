#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Altaxo.Drawing.D3D;
using sdd = System.Drawing.Drawing2D;

namespace Altaxo.Gui.Drawing.D3D.LineCaps
{
  /// <summary>
  /// Interaction logic for LineJoinComboBox.xaml
  /// </summary>
  public partial class LineCapComboBox : Common.Drawing.ImageComboBox
  {
    #region Converter

    private class Converter : IValueConverter
    {
      private LineCapComboBox _cb;

      public Converter(LineCapComboBox c)
      {
        _cb = c;
      }

      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        var val = (ILineCap)value;
        if (val is null)
          return _cb._cachedItems[Altaxo.Drawing.D3D.LineCaps.Flat.Instance.Name];
        else
          return _cb._cachedItems[val.Name];
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        return ((Common.Drawing.ImageComboBoxItem)value).Value;
      }
    }

    #endregion Converter

    private static Dictionary<string, ImageSource> _cachedImagesForStartCap = new Dictionary<string, ImageSource>();
    private static Dictionary<string, ImageSource> _cachedImagesForEndCap = new Dictionary<string, ImageSource>();

    private Dictionary<string, Common.Drawing.ImageComboBoxItem> _cachedItems = new Dictionary<string, Common.Drawing.ImageComboBoxItem>();

    private static GdiToWpfBitmap _interopBitmap;

    private bool _isForEndCap;

    public LineCapComboBox()
    {
      InitializeComponent();
      SetDefaultValues();

      var binding = new Binding
      {
        Source = this,
        Path = new PropertyPath(_nameOfValueProp),
        Converter = new Converter(this)
      };
      SetBinding(ComboBox.SelectedItemProperty, binding);
    }

    public bool IsForEndCap
    {
      get { return _isForEndCap; }
      set { _isForEndCap = value; }
    }

    private void SetDefaultValues()
    {
      var lineCapTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(ILineCap));

      foreach (var capType in lineCapTypes)
      {
        try
        {
          var cap = (ILineCap)Activator.CreateInstance(capType);
          var item = new Common.Drawing.ImageComboBoxItem(this, cap);
          _cachedItems.Add(cap.Name, item);
          Items.Add(item);
        }
        catch (Exception)
        {
        }
      }
    }

    #region Dependency property

    private const string _nameOfValueProp = "SelectedLineCap";

    public ILineCap SelectedLineCap
    {
      get { return (ILineCap)GetValue(SelectedLineCapProperty); }
      set { SetValue(SelectedLineCapProperty, value); }
    }

    public static readonly DependencyProperty SelectedLineCapProperty =
        DependencyProperty.Register(_nameOfValueProp, typeof(ILineCap), typeof(LineCapComboBox),
        new FrameworkPropertyMetadata(Altaxo.Drawing.D3D.LineCaps.Flat.Instance, OnSelectedLineCapChanged));

    private static void OnSelectedLineCapChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
    }

    #endregion Dependency property

    public override string GetItemText(object item)
    {
      var value = (ILineCap)item;
      return value.GetType().Name;
    }

    public override ImageSource GetItemImage(object item)
    {
      var val = (ILineCap)item;
      ImageSource result;
      if (_isForEndCap)
      {
        if (!_cachedImagesForEndCap.TryGetValue(val.Name, out result))
          _cachedImagesForEndCap.Add(val.Name, result = GetImage(val, _isForEndCap));
      }
      else
      {
        if (!_cachedImagesForStartCap.TryGetValue(val.Name, out result))
          _cachedImagesForStartCap.Add(val.Name, result = GetImage(val, _isForEndCap));
      }
      return result;
    }

    public static ImageSource GetImage(ILineCap join, bool isForEndCap)
    {
      const int bmpHeight = 24;
      const int bmpWidth = 48;
      const double lineWidth = bmpHeight * 0.4;

      if (_interopBitmap is null)
        _interopBitmap = new GdiToWpfBitmap(bmpWidth, bmpHeight);

      using (var grfx = _interopBitmap.BeginGdiPainting())
      {
        grfx.CompositingMode = sdd.CompositingMode.SourceCopy;
        grfx.FillRectangle(System.Drawing.Brushes.Transparent, 0, 0, bmpWidth, bmpHeight);

        var linePen = new System.Drawing.Pen(System.Drawing.Brushes.Black, (float)Math.Ceiling(lineWidth));
        if (isForEndCap)
        {
          //join.SetEndCap(linePen);
          //grfx.DrawLine(linePen, 0, 0.5f * bmpHeight, bmpWidth * (1 - 0.25f), 0.5f * bmpHeight);
        }
        else
        {
          //join.SetStartCap(linePen);
          //grfx.DrawLine(linePen, 0.25f * bmpWidth, 0.5f * bmpHeight, bmpWidth, 0.5f * bmpHeight);
        }
        _interopBitmap.EndGdiPainting();
      }

      var img = new WriteableBitmap(_interopBitmap.WpfBitmap);
      img.Freeze();
      return img;
    }
  }
}
