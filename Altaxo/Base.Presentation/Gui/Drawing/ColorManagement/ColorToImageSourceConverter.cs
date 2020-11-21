#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Altaxo.Drawing;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  /// <summary>
  /// Converts an instance of <see cref="AxoColor"/> or <see cref="NamedColor"/> into an rectangular image of that color.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class ColorToImageSourceConverter : IValueConverter
  {
    public double SymbolSize { get; set; } = 16;

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      AxoColor color;

      if (value is AxoColor)
        color = (AxoColor)value;
      else if (value is NamedColor)
        color = ((NamedColor)value).Color;
      else
        return null;

      // draws a transparent outline to fix the borders
      var drawingGroup = new DrawingGroup();

      var fill = new RectangleGeometry(new System.Windows.Rect(0, 0, SymbolSize, SymbolSize));

      var geometryDrawing = new GeometryDrawing
      {
        Geometry = fill,
        Brush = new SolidColorBrush(GuiHelper.ToWpf(color))
      };
      drawingGroup.Children.Add(geometryDrawing);

      var geometryImage = new DrawingImage(drawingGroup);

      // Freeze the DrawingImage for performance benefits.
      geometryImage.Freeze();
      return geometryImage;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
