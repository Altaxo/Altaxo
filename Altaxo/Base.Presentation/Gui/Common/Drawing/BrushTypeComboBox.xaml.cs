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
using Altaxo.Drawing;

namespace Altaxo.Gui.Common.Drawing
{
  /// <summary>
  /// ComboBox for <see cref="Altaxo.Drawing.BrushType"/>.
  /// </summary>
  public partial class BrushTypeComboBox : ImageComboBox
  {
    private class CC : IValueConverter
    {
      private BrushTypeComboBox _cb;

      public CC(BrushTypeComboBox c)
      {
        _cb = c;
      }

      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        var val = (BrushType)value;
        return _cb._cachedItems[val];
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        return ((ImageComboBoxItem)value).Value;
      }
    }

    private static Dictionary<BrushType, ImageSource> _cachedImages = new Dictionary<BrushType, ImageSource>();

    private Dictionary<BrushType, ImageComboBoxItem> _cachedItems = new Dictionary<BrushType, ImageComboBoxItem>();

    /// <summary>Occurs when the selected type of brush changed.</summary>
    public event DependencyPropertyChangedEventHandler SelectedBrushTypeChanged;

    static BrushTypeComboBox()
    {
    }

    public BrushTypeComboBox()
    {
      InitializeComponent();

      _cachedItems.Add(BrushType.SolidBrush, new ImageComboBoxItem(this, BrushType.SolidBrush));
      _cachedItems.Add(BrushType.LinearGradientBrush, new ImageComboBoxItem(this, BrushType.LinearGradientBrush));
      _cachedItems.Add(BrushType.TriangularShapeLinearGradientBrush, new ImageComboBoxItem(this, BrushType.TriangularShapeLinearGradientBrush));
      _cachedItems.Add(BrushType.SigmaBellShapeLinearGradientBrush, new ImageComboBoxItem(this, BrushType.SigmaBellShapeLinearGradientBrush));
      _cachedItems.Add(BrushType.PathGradientBrush, new ImageComboBoxItem(this, BrushType.PathGradientBrush));
      _cachedItems.Add(BrushType.TriangularShapePathGradientBrush, new ImageComboBoxItem(this, BrushType.TriangularShapePathGradientBrush));
      _cachedItems.Add(BrushType.SigmaBellShapePathGradientBrush, new ImageComboBoxItem(this, BrushType.SigmaBellShapePathGradientBrush));
      _cachedItems.Add(BrushType.HatchBrush, new ImageComboBoxItem(this, BrushType.HatchBrush));
      _cachedItems.Add(BrushType.SyntheticTextureBrush, new ImageComboBoxItem(this, BrushType.SyntheticTextureBrush));
      _cachedItems.Add(BrushType.TextureBrush, new ImageComboBoxItem(this, BrushType.TextureBrush));

      Items.Add(_cachedItems[BrushType.SolidBrush]);
      Items.Add(_cachedItems[BrushType.LinearGradientBrush]);
      Items.Add(_cachedItems[BrushType.TriangularShapeLinearGradientBrush]);
      Items.Add(_cachedItems[BrushType.SigmaBellShapeLinearGradientBrush]);
      Items.Add(_cachedItems[BrushType.PathGradientBrush]);
      Items.Add(_cachedItems[BrushType.TriangularShapePathGradientBrush]);
      Items.Add(_cachedItems[BrushType.SigmaBellShapePathGradientBrush]);
      Items.Add(_cachedItems[BrushType.HatchBrush]);
      Items.Add(_cachedItems[BrushType.SyntheticTextureBrush]);
      Items.Add(_cachedItems[BrushType.TextureBrush]);

      var _valueBinding = new Binding
      {
        Source = this,
        Path = new PropertyPath(_nameOfValueProp),
        Converter = new CC(this)
      };
      SetBinding(ComboBox.SelectedItemProperty, _valueBinding);
    }

    #region Dependency property

    private const string _nameOfValueProp = "BrushType";

    public BrushType BrushType
    {
      get { return (BrushType)GetValue(BrushTypeProperty); }
      set { SetValue(BrushTypeProperty, value); }
    }

    public static readonly DependencyProperty BrushTypeProperty =
        DependencyProperty.Register(_nameOfValueProp, typeof(BrushType), typeof(BrushTypeComboBox),
        new FrameworkPropertyMetadata(OnBrushTypeChanged));

    private static void OnBrushTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((BrushTypeComboBox)obj).EhBrushTypeChanged(obj, args);
    }

    #endregion Dependency property

    protected virtual void EhBrushTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((BrushTypeComboBox)obj).OnSelectedBrushTypeChanged(obj, args);
    }

    protected virtual void OnSelectedBrushTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (SelectedBrushTypeChanged is not null)
        SelectedBrushTypeChanged(obj, args);
    }

    public override string GetItemText(object item)
    {
      var val = (BrushType)item;
      return val.ToString();
    }

    public override ImageSource GetItemImage(object item)
    {
      var val = (BrushType)item;
      if (!_cachedImages.TryGetValue(val, out var result))
        _cachedImages.Add(val, result = GetImage(val));
      return result;
    }

    public static DrawingImage GetImage(BrushType val)
    {
      double height = 1;
      double width = 2;

      //
      // Create the Geometry to draw.
      //
      var geometryGroup = new GeometryGroup();
      geometryGroup.Children.Add(new RectangleGeometry(new Rect(0, 0, width, height)));

      var geometryDrawing = new GeometryDrawing() { Geometry = geometryGroup };

      switch (val)
      {
        case BrushType.SolidBrush:
          geometryDrawing.Brush = new SolidColorBrush(Colors.Black);
          break;

        case BrushType.LinearGradientBrush:
          geometryDrawing.Brush = new LinearGradientBrush(Colors.Black, Colors.White, 0);
          break;

        case BrushType.TriangularShapeLinearGradientBrush:
          {
            var gStops = new GradientStopCollection
            {
              new GradientStop(Colors.Black, 0),
              new GradientStop(Colors.White, 0.5),
              new GradientStop(Colors.Black, 1)
            };
            geometryDrawing.Brush = new LinearGradientBrush(gStops, 0);
          }
          break;

        case BrushType.SigmaBellShapeLinearGradientBrush:
          {
            var gStops = new GradientStopCollection
            {
              new GradientStop(Colors.Black, 0),
              new GradientStop(Colors.White, 0.5),
              new GradientStop(Colors.Black, 1)
            };
            geometryDrawing.Brush = new LinearGradientBrush(gStops, 0);
          }
          break;

        case BrushType.PathGradientBrush:
        case BrushType.TriangularShapePathGradientBrush:
        case BrushType.SigmaBellShapePathGradientBrush:
          geometryDrawing.Brush = new RadialGradientBrush(Colors.Black, Colors.White);
          break;

        case BrushType.HatchBrush:
        case BrushType.SyntheticTextureBrush:
        case BrushType.TextureBrush:
          geometryDrawing.Brush = new SolidColorBrush(Colors.Black);
          break;

        default:
          break;
      }

      var geometryImage = new DrawingImage(geometryDrawing);

      // Freeze the DrawingImage for performance benefits.
      geometryImage.Freeze();
      return geometryImage;
    }
  }
}
