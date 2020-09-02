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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Drawing.D3D.Material;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D;

namespace Altaxo.Gui.Drawing.D3D
{
  public class MaterialToImageSourceConverter : IValueConverter
  {
    private int _width = 16;
    private int _height = 16;

    public int Width
    {
      get { return _width; }
      set { _width = value; }
    }

    public int Height
    {
      get { return _height; }
      set { _height = value; }
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is IMaterial)
        return GetImageSourceFromBrushX((IMaterial)value, _width, _height);
      else if (value is NamedColor)
        return GetImageSourceFromAxoColor(((NamedColor)value).Color, _width, _height);
      else if (value is AxoColor)
        return GetImageSourceFromAxoColor((AxoColor)value, _width, _height);
      else
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public static ImageSource GetImageSourceFromAxoColor(AxoColor axoColor, int width, int height)
    {
      var innerRect = new Rect(0, 0, width, height);
      var geometryDrawing = new GeometryDrawing() { Geometry = new RectangleGeometry(innerRect) };
      geometryDrawing.Brush = new SolidColorBrush(GuiHelper.ToWpf(axoColor));
      var geometryImage = new DrawingImage(geometryDrawing);
      geometryImage.Freeze(); // Freeze the DrawingImage for performance benefits.
      return geometryImage;
    }

    public static ImageSource GetImageSourceFromBrushX(IMaterial val, int width, int height)
    {
      //
      // Create the Geometry to draw.
      //

      if (val.IsVisible == false)
      {
        return GetImageSourceFromAxoColor(AxoColors.Transparent, width, height);
      }
      else if (val.HasColor)
      {
        return GetImageSourceFromAxoColor(val.Color.Color, width, height);
      }
      else
      {
        return GetImageSourceFromAxoColor(NamedColors.Black, width, height);
      }
    }
  }

  /// <summary>
  /// Converts a <see cref="IMaterial"/> to a string, which represents the name of this brush.
  /// </summary>
  public class MaterialToMaterialNameConverter : IValueConverter
  {
    /// <summary>
    /// Converts a <see cref="Material"/> to its name.
    /// </summary>
    /// <param name="value">A <see cref="IMaterial"/> object.</param>
    /// <param name="targetType">Ignored. Return type is always string.</param>
    /// <param name="parameter">Ignored</param>
    /// <param name="culture">The culture to use in the converter. Ignored.</param>
    /// <returns>
    /// The name of the brush.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is IMaterial)
        return GetNameForMaterial((IMaterial)value);
      else if (value is NamedColor)
        return ((NamedColor)value).Name;
      else
        return null;
    }

    public static string GetNameForMaterial(IMaterial material)
    {
      string name;
      if (material is null)
      {
        name = "<<null>>";
      }
      else if (material is MaterialWithUniformColor)
      {
        name = string.Format("{0} S={1}, M={2}, N={3}", material.Color.Name, material.Smoothness, material.Metalness, material.IndexOfRefraction);
      }
      else if (material is MaterialWithoutColorOrTexture)
      {
        name = string.Format("{0} S={1}, M={2}, N={3}", "NotColored", material.Smoothness, material.Metalness, material.IndexOfRefraction);
      }
      else if (material is MaterialInvisible)
      {
        name = "Invisible (No material)";
      }
      else
      {
        name = material.GetType().Name;
      }

      return name;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
