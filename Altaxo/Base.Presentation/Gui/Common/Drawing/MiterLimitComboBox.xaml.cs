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
using System.Windows.Media;

namespace Altaxo.Gui.Common.Drawing
{
  using Altaxo.Units;
  using Altaxo.Units.Dimensionless;

  /// <summary>
  /// Interaction logic for MiterLimitComboBox.xaml
  /// </summary>
  public partial class MiterLimitComboBox : DimensionfulQuantityImageComboBox
  {
    private static Dictionary<double, ImageSource> _cachedImages = new Dictionary<double, ImageSource>();

    private static readonly double[] _initialValues = new double[] { 1, 2, 4, 6, 8, 10, 12 };

    static MiterLimitComboBox()
    {
      SelectedQuantityProperty.OverrideMetadata(typeof(MiterLimitComboBox), new FrameworkPropertyMetadata(new DimensionfulQuantity(10, Unity.Instance)));
    }

    public MiterLimitComboBox()
    {
      UnitEnvironment = RelationEnvironment.Instance;

      InitializeComponent();

      foreach (var e in _initialValues)
        Items.Add(new ImageComboBoxItem(this, new DimensionfulQuantity(e, Unity.Instance)));

      _img.Source = GetImage(SelectedQuantityInSIUnits);
    }

    protected override void OnSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      base.OnSelectedQuantityChanged(obj, args);

      if (null != _img)
      {
        var val = SelectedQuantityInSIUnits;
        _img.Source = GetImage(val);
      }
    }

    public override ImageSource GetItemImage(object item)
    {
      double val = ((DimensionfulQuantity)item).AsValueIn(Unity.Instance);
      ImageSource result;
      if (!_cachedImages.TryGetValue(val, out result))
        _cachedImages.Add(val, result = GetImage(val));
      return result;
    }

    public override string GetItemText(object item)
    {
      return (string)_converter.Convert(item, typeof(string), null, System.Globalization.CultureInfo.CurrentUICulture);
    }

    public static DrawingImage GetImage(double miterLimit)
    {
      const double height = 1;
      const double width = 2;
      const double lineWidth = 0.375 * height;

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
      geometryDrawing.Pen = new Pen(Brushes.Black, lineWidth) { LineJoin = PenLineJoin.Miter, MiterLimit = miterLimit };
      drawingGroup.Children.Add(geometryDrawing);

      drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0, 0, width, height));

      DrawingImage geometryImage = new DrawingImage(drawingGroup);

      geometryImage.Freeze();
      return geometryImage;
    }
  }
}
