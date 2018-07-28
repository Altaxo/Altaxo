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
  /// ComboBox for <see cref="LinearGradientMode"/>.
  /// </summary>
  public partial class LinearGradientModeComboBox : ImageComboBox
  {
    private class CC : IValueConverter
    {
      private LinearGradientModeComboBox _cb;

      public CC(LinearGradientModeComboBox c)
      {
        _cb = c;
      }

      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        var val = (LinearGradientMode)value;
        return _cb._cachedItems[val];
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        return ((ImageComboBoxItem)value).Value;
      }
    }

    private static Dictionary<LinearGradientMode, ImageSource> _cachedImages = new Dictionary<LinearGradientMode, ImageSource>();

    private Dictionary<LinearGradientMode, ImageComboBoxItem> _cachedItems = new Dictionary<LinearGradientMode, ImageComboBoxItem>();

    static LinearGradientModeComboBox()
    {
    }

    public LinearGradientModeComboBox()
    {
      InitializeComponent();

      foreach (var e in new LinearGradientMode[] { LinearGradientMode.Horizontal, LinearGradientMode.Vertical, LinearGradientMode.ForwardDiagonal, LinearGradientMode.BackwardDiagonal })
      {
        _cachedItems.Add(e, new ImageComboBoxItem(this, e));
        Items.Add(_cachedItems[e]);
      }

      var _valueBinding = new Binding();
      _valueBinding.Source = this;
      _valueBinding.Path = new PropertyPath(_nameOfValueProp);
      _valueBinding.Converter = new CC(this);
      this.SetBinding(ComboBox.SelectedItemProperty, _valueBinding);
    }

    #region Dependency property

    private const string _nameOfValueProp = "LinearGradientMode";

    public LinearGradientMode LinearGradientMode
    {
      get { return (LinearGradientMode)GetValue(LinearGradientModeProperty); }
      set { SetValue(LinearGradientModeProperty, value); }
    }

    public static readonly DependencyProperty LinearGradientModeProperty =
        DependencyProperty.Register(_nameOfValueProp, typeof(LinearGradientMode), typeof(LinearGradientModeComboBox),
        new FrameworkPropertyMetadata(LinearGradientMode.Horizontal, OnLinearGradientModeChanged));

    private static void OnLinearGradientModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((LinearGradientModeComboBox)obj).EhLinearGradientModeChanged(obj, args);
    }

    #endregion Dependency property

    protected virtual void EhLinearGradientModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
    }

    public override string GetItemText(object item)
    {
      var val = (LinearGradientMode)item;
      return val.ToString();
    }

    public override ImageSource GetItemImage(object item)
    {
      var val = (LinearGradientMode)item;
      ImageSource result;
      if (!_cachedImages.TryGetValue(val, out result))
        _cachedImages.Add(val, result = GetImage(val));
      return result;
    }

    public static DrawingImage GetImage(LinearGradientMode val)
    {
      double height = 1;
      double width = 2;

      //
      // Create the Geometry to draw.
      //
      GeometryGroup geometryGroup = new GeometryGroup();
      geometryGroup.Children.Add(new RectangleGeometry(new Rect(0, 0, width, height)));

      var geometryDrawing = new GeometryDrawing() { Geometry = geometryGroup };

      switch (val)
      {
        case LinearGradientMode.Horizontal:
          geometryDrawing.Brush = new System.Windows.Media.LinearGradientBrush(Colors.Black, Colors.White, 0);
          break;

        case LinearGradientMode.Vertical:
          geometryDrawing.Brush = new System.Windows.Media.LinearGradientBrush(Colors.Black, Colors.White, 90);
          break;

        case LinearGradientMode.ForwardDiagonal:
          geometryDrawing.Brush = new System.Windows.Media.LinearGradientBrush(Colors.Black, Colors.White, 45);
          break;

        case LinearGradientMode.BackwardDiagonal:
          geometryDrawing.Brush = new System.Windows.Media.LinearGradientBrush(Colors.Black, Colors.White, -45);
          break;

        default:
          break;
      }

      DrawingImage geometryImage = new DrawingImage(geometryDrawing);

      // Freeze the DrawingImage for performance benefits.
      geometryImage.Freeze();
      return geometryImage;
    }
  }
}
