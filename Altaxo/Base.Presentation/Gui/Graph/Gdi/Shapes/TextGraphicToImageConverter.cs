#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Data;
using System.Windows;
using Altaxo.Drawing;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Shapes;
using System.Drawing;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  /// <summary>
  /// Converts a <see cref="TextGraphic"/> instance to an image that visualize that object (intended for the TextGraphic preview panel).
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class TextGraphicToImageConverter : IMultiValueConverter, IDisposable
  {
    private GdiToWpfBitmap? _previewBitmap;

    /// <summary>
    /// Converts a <see cref="BrushX"/> value. 
    /// It is expected that the argument <paramref name="values"/> contains [0] the <see cref="TextGraphic"/> object,
    /// [1] the width of the preview image, and
    /// [2] the height of the preview image.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A <see cref="System.Windows.Media.ImageSource"/> value if successfull; otherwise,  <see cref="Binding.DoNothing"/>.
    /// </returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if(values is not null && values.Length>=3 && values[0] is TextGraphic textGraphic && values[1] is double dwidth && values[2] is double dheight)
      {
        
        int height = (int)dheight;
        int width = (int)dwidth;
        if (height <= 0)
          height = 64;
        if (width <= 0)
          width = 64;

        _previewBitmap ??= new GdiToWpfBitmap(width, height);

        if (width != _previewBitmap.GdiRectangle.Width || height != _previewBitmap.GdiRectangle.Height)
        {
          _previewBitmap.Resize(width, height);
        }

        using (var g = _previewBitmap.BeginGdiPainting())
        {
          var fullRect = _previewBitmap.GdiRectangle;


          g.PageUnit = System.Drawing.GraphicsUnit.Point;

          g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
          g.FillRectangle(Brushes.Transparent, g.VisibleClipBounds);
          g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

          var paintContext = new GdiPaintContext();

          textGraphic.Paint(g, paintContext, true);

          _previewBitmap.EndGdiPainting();
        }

        return _previewBitmap.WpfBitmapSource;
      }
      else
      {
        return Binding.DoNothing;
      }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      _previewBitmap?.Dispose();
      _previewBitmap = null;
    }
  }
}
