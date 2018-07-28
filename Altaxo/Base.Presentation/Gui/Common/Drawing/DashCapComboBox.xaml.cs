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

using sdd = System.Drawing.Drawing2D;

namespace Altaxo.Gui.Common.Drawing
{
  /// <summary>
  /// Interaction logic for LineJoinComboBox.xaml
  /// </summary>
  public partial class DashCapComboBox : ImageComboBox
  {
    #region Converter

    private class Converter : IValueConverter
    {
      private DashCapComboBox _cb;

      public Converter(DashCapComboBox c)
      {
        _cb = c;
      }

      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        var val = (sdd.DashCap)value;
        return _cb._cachedItems[val];
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        return ((ImageComboBoxItem)value).Value;
      }
    }

    #endregion Converter

    private static Dictionary<sdd.DashCap, ImageSource> _cachedImages = new Dictionary<sdd.DashCap, ImageSource>();

    private Dictionary<sdd.DashCap, ImageComboBoxItem> _cachedItems = new Dictionary<sdd.DashCap, ImageComboBoxItem>();

    public DashCapComboBox()
    {
      InitializeComponent();

      foreach (sdd.DashCap e in Enum.GetValues(typeof(sdd.DashCap)))
      {
        _cachedItems.Add(e, new ImageComboBoxItem(this, e));
        Items.Add(_cachedItems[e]);
      }

      var binding = new Binding();
      binding.Source = this;
      binding.Path = new PropertyPath(_nameOfValueProp);
      binding.Converter = new Converter(this);
      this.SetBinding(ComboBox.SelectedItemProperty, binding);
    }

    #region Dependency property

    private const string _nameOfValueProp = "SelectedDashCap";

    public sdd.DashCap SelectedDashCap
    {
      get { return (sdd.DashCap)GetValue(SelectedDashCapProperty); }
      set { SetValue(SelectedDashCapProperty, value); }
    }

    public static readonly DependencyProperty SelectedDashCapProperty =
        DependencyProperty.Register(_nameOfValueProp, typeof(sdd.DashCap), typeof(DashCapComboBox),
        new FrameworkPropertyMetadata(sdd.DashCap.Flat, OnSelectedDashCapChanged));

    private static void OnSelectedDashCapChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
    }

    #endregion Dependency property

    public override string GetItemText(object item)
    {
      var val = (sdd.DashCap)item;
      return val.ToString();
    }

    public override ImageSource GetItemImage(object item)
    {
      var val = (sdd.DashCap)item;
      ImageSource result;
      if (!_cachedImages.TryGetValue(val, out result))
        _cachedImages.Add(val, result = GetImage(val));
      return result;
    }

    public static DrawingImage GetImage(sdd.DashCap val)
    {
      double height = 1;
      double width = 2;

      PenLineCap dashCap = PenLineCap.Flat;
      switch (val)
      {
        default:
        case sdd.DashCap.Flat:
          dashCap = PenLineCap.Flat;
          break;

        case sdd.DashCap.Round:
          dashCap = PenLineCap.Round;
          break;

        case sdd.DashCap.Triangle:
          dashCap = PenLineCap.Triangle;
          break;
      }

      //
      // Create the Geometry to draw.
      //
      var drawingGroup = new DrawingGroup();
      var geometryDrawing = new GeometryDrawing() { Geometry = new RectangleGeometry(new Rect(0, 0, width, height)) };
      geometryDrawing.Pen = new Pen(Brushes.Transparent, 0);
      drawingGroup.Children.Add(geometryDrawing);

      geometryDrawing = new GeometryDrawing() { Geometry = new LineGeometry(new Point(0, height / 2), new Point(width, height / 2)) };
      geometryDrawing.Pen = new Pen(Brushes.Black, height / 5) { DashCap = dashCap, DashStyle = DashStyles.Dash };
      drawingGroup.Children.Add(geometryDrawing);

      DrawingImage geometryImage = new DrawingImage(drawingGroup);

      // Freeze the DrawingImage for performance benefits.
      geometryImage.Freeze();
      return geometryImage;
    }
  }
}
