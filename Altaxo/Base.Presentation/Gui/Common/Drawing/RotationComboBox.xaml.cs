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
using Altaxo.Units;
using Altaxo.Units.Angle;

namespace Altaxo.Gui.Common.Drawing
{
  public partial class RotationComboBox : DimensionfulQuantityImageComboBox
  {
    private static Dictionary<double, ImageSource> _cachedImages = new Dictionary<double, ImageSource>();

    private static readonly double[] _initialValues = new double[] { 0, 45, 90, 135, 180, 225, 270, 315 };

    static RotationComboBox()
    {
      SelectedQuantityProperty.OverrideMetadata(typeof(RotationComboBox), new FrameworkPropertyMetadata(new DimensionfulQuantity(0, Degree.Instance)));
    }

    public RotationComboBox()
    {
      UnitEnvironment = AngleEnvironment.Instance;
      InitializeComponent();

      foreach (var e in _initialValues)
        Items.Add(new ImageComboBoxItem(this, new DimensionfulQuantity(e, Degree.Instance).AsQuantityIn(UnitEnvironment.DefaultUnit)));

      _img.Source = GetImage(SelectedQuantityAsValueInDegrees);
    }

    public double SelectedQuantityAsValueInDegrees
    {
      get { return SelectedQuantity.AsValueIn(Degree.Instance); }
      set
      {
        var quant = new DimensionfulQuantity(value, Degree.Instance);
        if (null != UnitEnvironment)
          quant = quant.AsQuantityIn(UnitEnvironment.DefaultUnit);
        SelectedQuantity = quant;
      }
    }

    protected override void OnSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      base.OnSelectedQuantityChanged(obj, args);

      if (null != _img)
      {
        var val = SelectedQuantityAsValueInDegrees;
        _img.Source = GetImage(val);
      }
    }

    public override ImageSource GetItemImage(object item)
    {
      double val = ((DimensionfulQuantity)item).AsValueIn(Degree.Instance);
      if (!_cachedImages.TryGetValue(val, out var result))
        _cachedImages.Add(val, result = GetImage(val));
      return result;
    }

    public override string GetItemText(object item)
    {
      return (string)_converter.Convert(item, typeof(string), null, System.Globalization.CultureInfo.CurrentUICulture);
    }

    public static ImageSource GetImage(double angle)
    {
      double AngleRad = (angle / 180) * Math.PI;

      const double DesignHeight = 1;
      double offset = 0.5 * DesignHeight;
      double radius = 0.40 * DesignHeight;
      double lineWidth = 0.08 * DesignHeight;

      // draws a transparent outline to fix the borders
      var outlineDrawing = new GeometryDrawing
      {
        Geometry = new RectangleGeometry(new Rect(0, 0, DesignHeight, DesignHeight)),
        Pen = new Pen(Brushes.Transparent, 0)
      };

      // now the geometry itself
      var ellipses = new GeometryGroup();
      ellipses.Children.Add(
          new EllipseGeometry(new Point(offset, offset), radius, radius)
          );

      ellipses.Children.Add(new LineGeometry(new Point(offset, offset), new Point(offset + radius * Math.Cos(AngleRad), offset + radius * Math.Sin(-AngleRad))));
      var geometryDrawing = new GeometryDrawing
      {
        Geometry = ellipses,
        // fill with a gradient brush
        Brush =
          new LinearGradientBrush(
              Color.FromRgb(100, 100, 255),
              Color.FromRgb(204, 204, 255),
              new Point(0, 0),
              new Point(1, 1)),
        Pen = new Pen(Brushes.Black, lineWidth)
      };

      var group = new DrawingGroup();
      group.Children.Add(outlineDrawing);
      group.Children.Add(geometryDrawing);
      var geometryImage = new DrawingImage(group);

      // Freeze the DrawingImage for performance benefits.
      geometryImage.Freeze();
      return geometryImage;
    }
  }
}
