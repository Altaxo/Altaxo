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

using Altaxo.Gui.Common.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using sdd = Altaxo.Drawing.D3D;

namespace Altaxo.Gui.Drawing.D3D
{
  /// <summary>
  /// Interaction logic for LineJoinComboBox.xaml
  /// </summary>
  public partial class LineJoinComboBox : ImageComboBox
  {
    private class CC : IValueConverter
    {
      private LineJoinComboBox _cb;

      public CC(LineJoinComboBox c)
      {
        _cb = c;
      }

      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        var val = (sdd.PenLineJoin)value;
        return _cb._cachedItems[val];
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        return ((ImageComboBoxItem)value).Value;
      }
    }

    private static Dictionary<sdd.PenLineJoin, ImageSource> _cachedImages = new Dictionary<sdd.PenLineJoin, ImageSource>();

    private Dictionary<sdd.PenLineJoin, ImageComboBoxItem> _cachedItems = new Dictionary<sdd.PenLineJoin, ImageComboBoxItem>();

    public LineJoinComboBox()
    {
      InitializeComponent();

      _cachedItems.Add(sdd.PenLineJoin.Bevel, new ImageComboBoxItem(this, sdd.PenLineJoin.Bevel));
      _cachedItems.Add(sdd.PenLineJoin.Miter, new ImageComboBoxItem(this, sdd.PenLineJoin.Miter));

      Items.Add(_cachedItems[sdd.PenLineJoin.Bevel]);
      Items.Add(_cachedItems[sdd.PenLineJoin.Miter]);

      var _valueBinding = new Binding();
      _valueBinding.Source = this;
      _valueBinding.Path = new PropertyPath(_nameOfValueProp);
      _valueBinding.Converter = new CC(this);
      this.SetBinding(ComboBox.SelectedItemProperty, _valueBinding);
    }

    #region Dependency property

    private const string _nameOfValueProp = "SelectedLineJoin";

    public sdd.PenLineJoin SelectedLineJoin
    {
      get { var result = (sdd.PenLineJoin)GetValue(SelectedLineJoinProperty); return result; }
      set { SetValue(SelectedLineJoinProperty, value); }
    }

    public static readonly DependencyProperty SelectedLineJoinProperty =
        DependencyProperty.Register(_nameOfValueProp, typeof(sdd.PenLineJoin), typeof(LineJoinComboBox),
        new FrameworkPropertyMetadata(OnSelectedLineJoinChanged));

    private static void OnSelectedLineJoinChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((LineJoinComboBox)obj).EhSelectedLineJoinChanged(obj, args);
    }

    #endregion Dependency property

    protected virtual void EhSelectedLineJoinChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
    }

    public override string GetItemText(object item)
    {
      var val = (sdd.PenLineJoin)item;
      return val.ToString();
    }

    public override ImageSource GetItemImage(object item)
    {
      var val = (sdd.PenLineJoin)item;
      ImageSource result;
      if (!_cachedImages.TryGetValue(val, out result))
        _cachedImages.Add(val, result = GetImage(val));
      return result;
    }

    public static DrawingImage GetImage(sdd.PenLineJoin join)
    {
      const double height = 1;
      const double width = 2;
      const double lineWidth = 0.375 * height;

      PenLineJoin plj;
      switch (join)
      {
        case sdd.PenLineJoin.Bevel:
          plj = PenLineJoin.Bevel;
          break;

        case sdd.PenLineJoin.Miter:
          plj = PenLineJoin.Miter;
          break;

        default:
          plj = PenLineJoin.Bevel;
          break;
      }

      var drawingGroup = new DrawingGroup();
      GeometryDrawing geometryDrawing;

      geometryDrawing = new GeometryDrawing();
      geometryDrawing.Geometry = new RectangleGeometry(new Rect(0, 0, width, height));
      geometryDrawing.Pen = new Pen(Brushes.Transparent, 0);
      drawingGroup.Children.Add(geometryDrawing);

      geometryDrawing = new GeometryDrawing();
      var figure = new PathFigure();
      figure.StartPoint = new Point(width, height * 0.875);
      figure.Segments.Add(new PolyLineSegment(new Point[]
      {
        new Point(width / 2, height / 2),
        new Point(width, height * 0.175) }, true));
      geometryDrawing.Geometry = new PathGeometry(new PathFigure[] { figure });
      geometryDrawing.Pen = new Pen(Brushes.Black, lineWidth) { LineJoin = plj };
      drawingGroup.Children.Add(geometryDrawing);

      drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0, 0, width, height));

      DrawingImage geometryImage = new DrawingImage(drawingGroup);

      geometryImage.Freeze();
      return geometryImage;
    }
  }
}
