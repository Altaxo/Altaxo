#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
using System.Windows.Data;
using System.Windows.Media;
using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Graph;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Converts an Altaxo <see cref="NamedColor"/> value into a <see cref="System.Windows.Media.SolidColorBrush"/> value.
  /// </summary>
  public class NamedColorToWpfBrushConverter : IValueConverter
  {
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is NamedColor)
      {
        var c = (NamedColor)value;
        return new System.Windows.Media.SolidColorBrush(GuiHelper.ToWpf(c.Color));
      }
      else if (value is AxoColor)
      {
        var c = (AxoColor)value;
        return new System.Windows.Media.SolidColorBrush(GuiHelper.ToWpf(c));
      }
      else
        return System.Windows.Media.Brushes.Black;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Converts a <see cref="NamedColor"/> to the name of its parent color set.
  /// </summary>
  public class NamedColorToColorSetNameConverter : IValueConverter
  {
    /// <summary>
    /// Gets a short label for the specified item-definition level.
    /// </summary>
    /// <param name="level">The item-definition level.</param>
    /// <returns>The short label for the specified level.</returns>
    public string GetLevelString(Altaxo.Main.ItemDefinitionLevel level)
    {
      switch (level)
      {
        case Altaxo.Main.ItemDefinitionLevel.Builtin:
          return "Builtin";

        case Altaxo.Main.ItemDefinitionLevel.Application:
          return "App";

        case Altaxo.Main.ItemDefinitionLevel.UserDefined:
          return "User";

        case Altaxo.Main.ItemDefinitionLevel.Project:
          return "Project";

        default:
          throw new NotImplementedException();
      }
    }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is NamedColor)
      {
        var c = (NamedColor)value;


        if (c.ParentColorSet is not null && ColorSetManager.Instance.TryGetList(c.ParentColorSet.Name, out var colorSetEntry))
          return string.Format("{0}/{1}", GetLevelString(colorSetEntry.Level), c.ParentColorSet.Name);
        else
          return "<<no color set>>";
      }
      else
        return string.Empty;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Converts <see cref="BrushX"/> values to preview images.
  /// </summary>
  public class BrushXToImageSourceConverter : IValueConverter
  {
    private int _width = 16;
    private int _height = 16;

    /// <summary>
    /// Gets or sets the preview image width.
    /// </summary>
    public int Width
    {
      get { return _width; }
      set { _width = value; }
    }

    /// <summary>
    /// Gets or sets the preview image height.
    /// </summary>
    public int Height
    {
      get { return _height; }
      set { _height = value; }
    }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is BrushX)
        return GetImageSourceFromBrushX((BrushX)value, _width, _height);
      else if (value is NamedColor)
        return GetImageSourceFromAxoColor(((NamedColor)value).Color, _width, _height);
      else if (value is AxoColor)
        return GetImageSourceFromAxoColor((AxoColor)value, _width, _height);
      else
        return null;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Creates an image source for the specified Altaxo color.
    /// </summary>
    /// <param name="axoColor">The Altaxo color.</param>
    /// <param name="width">The image width.</param>
    /// <param name="height">The image height.</param>
    /// <returns>The created image source.</returns>
    public static ImageSource GetImageSourceFromAxoColor(AxoColor axoColor, int width, int height)
    {
      var innerRect = new Rect(0, 0, width, height);
      var geometryDrawing = new GeometryDrawing() { Geometry = new RectangleGeometry(innerRect) };
      geometryDrawing.Brush = new SolidColorBrush(GuiHelper.ToWpf(axoColor));
      var geometryImage = new DrawingImage(geometryDrawing);
      geometryImage.Freeze(); // Freeze the DrawingImage for performance benefits.
      return geometryImage;
    }

    /// <summary>
    /// Creates an image source for the specified brush.
    /// </summary>
    /// <param name="val">The brush.</param>
    /// <param name="width">The image width.</param>
    /// <param name="height">The image height.</param>
    /// <returns>The created image source.</returns>
    public static ImageSource GetImageSourceFromBrushX(BrushX val, int width, int height)
    {
      //
      // Create the Geometry to draw.
      //

      if (val.BrushType == BrushType.SolidBrush)
      {
        return GetImageSourceFromAxoColor(val.Color.Color, width, height);
      }
      else
      {
        return GuiHelper.ToWpf(val, width, height);
      }
    }
  }

  /// <summary>
  /// Converts a <see cref="BrushX"/> to a string, which represents the name of this brush.
  /// </summary>
  public class BrushXToBrushNameConverter : IValueConverter
  {
    /// <summary>
    /// Converts a <see cref="BrushX"/> to the name of this brush.
    /// </summary>
    /// <param name="value">A <see cref="BrushX"/> object.</param>
    /// <param name="targetType">Ignored. Return type is always string.</param>
    /// <param name="parameter">Ignored</param>
    /// <param name="culture">The culture to use in the converter. Ignored.</param>
    /// <returns>
    /// The name of the brush.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is BrushX)
        return GetNameForBrushX((BrushX)value);
      else if (value is NamedColor)
        return ((NamedColor)value).Name;
      else
        return null;
    }

    /// <summary>
    /// Gets a display name for the specified brush.
    /// </summary>
    /// <param name="brush">The brush.</param>
    /// <returns>The display name for the specified brush.</returns>
    public static string GetNameForBrushX(BrushX brush)
    {
      string name;
      if (brush.BrushType == BrushType.SolidBrush)
      {
        name = brush.Color.Name;
      }
      else
      {
        switch (brush.BrushType)
        {
          case BrushType.SolidBrush:
            name = "CustSB ";
            break;

          case BrushType.SigmaBellShapeLinearGradientBrush:
          case BrushType.TriangularShapeLinearGradientBrush:
          case BrushType.LinearGradientBrush:
            name = "CustLGB ";
            break;

          case BrushType.SigmaBellShapePathGradientBrush:
          case BrushType.TriangularShapePathGradientBrush:
          case BrushType.PathGradientBrush:
            name = "CustPGB ";
            break;

          case BrushType.TextureBrush:
            name = "CustTB ";
            break;

          case BrushType.SyntheticTextureBrush:
            name = "CustSTB";
            break;

          case BrushType.HatchBrush:
            name = "CustHB ";
            break;

          default:
            name = "CustBrush ";
            break;
        }

        if (brush.BrushType != BrushType.TextureBrush)
          name += brush.Color.Name;
        else
          name = name.Trim();
      }
      return name;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
