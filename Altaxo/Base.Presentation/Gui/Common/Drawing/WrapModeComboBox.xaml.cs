﻿#region Copyright

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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Altaxo.Gui.Common.Drawing
{
  /// <summary>
  /// ComboBox for <see cref="WrapMode"/>.
  /// </summary>
  public partial class WrapModeComboBox : ImageComboBox
  {
    private class CC : IValueConverter
    {
      private WrapModeComboBox _cb;

      public CC(WrapModeComboBox c)
      {
        _cb = c;
      }

      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        var val = (WrapMode)value;
        return _cb._cachedItems[val];
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        if (value is ImageComboBoxItem icbi)
          return icbi.Value;
        else
          return Binding.DoNothing;

      }
    }

    private static Dictionary<WrapMode, ImageSource> _cachedImages = new Dictionary<WrapMode, ImageSource>();

    private Dictionary<WrapMode, ImageComboBoxItem> _cachedItems = new Dictionary<WrapMode, ImageComboBoxItem>();

    private static GeometryDrawing triangleDrawing;

    static WrapModeComboBox()
    {
      var triangleLinesSegment = new PolyLineSegment();
      triangleLinesSegment.Points.Add(new Point(50, 0));
      triangleLinesSegment.Points.Add(new Point(0, 50));
      var triangleFigure = new PathFigure
      {
        IsClosed = true,
        StartPoint = new Point(0, 0)
      };
      triangleFigure.Segments.Add(triangleLinesSegment);
      var triangleGeometry = new PathGeometry();
      triangleGeometry.Figures.Add(triangleFigure);

      triangleDrawing = new GeometryDrawing
      {
        Geometry = triangleGeometry,
        Brush = new SolidColorBrush(Color.FromArgb(255, 204, 204, 255))
      };
      var trianglePen = new Pen(Brushes.Black, 2);
      triangleDrawing.Pen = trianglePen;
      trianglePen.MiterLimit = 0;
      triangleDrawing.Freeze();
    }

    public WrapModeComboBox()
    {
      InitializeComponent();

      foreach (var e in new WrapMode[] { WrapMode.Tile, WrapMode.TileFlipX, WrapMode.TileFlipY, WrapMode.TileFlipXY, WrapMode.Clamp })
      {
        _cachedItems.Add(e, new ImageComboBoxItem(this, e));
        Items.Add(_cachedItems[e]);
      }

      var _valueBinding = new Binding
      {
        Source = this,
        Path = new PropertyPath(_nameOfValueProp),
        Converter = new CC(this)
      };
      SetBinding(ComboBox.SelectedItemProperty, _valueBinding);
    }

    #region Dependency property

    private const string _nameOfValueProp = "WrapMode";

    public WrapMode WrapMode
    {
      get { return (WrapMode)GetValue(WrapModeProperty); }
      set { SetValue(WrapModeProperty, value); }
    }

    public static readonly DependencyProperty WrapModeProperty =
        DependencyProperty.Register(_nameOfValueProp, typeof(WrapMode), typeof(WrapModeComboBox),
        new FrameworkPropertyMetadata(OnWrapModeChanged));

    private static void OnWrapModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((WrapModeComboBox)obj).EhWrapModeChanged(obj, args);
    }

    #endregion Dependency property

    protected virtual void EhWrapModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
    }

    public override string GetItemText(object item)
    {
      var val = (WrapMode)item;
      return val.ToString();
    }

    public override ImageSource GetItemImage(object item)
    {
      var val = (WrapMode)item;
      if (!_cachedImages.TryGetValue(val, out var result))
        _cachedImages.Add(val, result = GetImage(val));
      return result;
    }

    public static DrawingImage GetImage(WrapMode val)
    {
      double height = 1;
      double width = 2;

      //
      // Create the Geometry to draw.
      //
      var geometryGroup = new GeometryGroup();
      geometryGroup.Children.Add(new RectangleGeometry(new Rect(0, 0, width, height)));

      var geometryDrawing = new GeometryDrawing() { Geometry = geometryGroup };

      var brush = new DrawingBrush
      {
        Drawing = triangleDrawing
      };

      switch (val)
      {
        case WrapMode.Tile:
          brush.TileMode = TileMode.Tile;
          break;

        case WrapMode.TileFlipX:
          brush.TileMode = TileMode.FlipX;
          break;

        case WrapMode.TileFlipY:
          brush.TileMode = TileMode.FlipY;
          break;

        case WrapMode.TileFlipXY:
          brush.TileMode = TileMode.FlipXY;
          break;

        case WrapMode.Clamp:
          brush.TileMode = TileMode.None;
          break;

        default:
          break;
      }

      brush.Viewport = new Rect(0, 0, 0.5, 0.5);
      geometryDrawing.Brush = brush;
      geometryDrawing.Pen = new Pen(Brushes.Black, 0.1);

      var geometryImage = new DrawingImage(geometryDrawing);

      // Freeze the DrawingImage for performance benefits.
      geometryImage.Freeze();
      return geometryImage;
    }
  }
}
