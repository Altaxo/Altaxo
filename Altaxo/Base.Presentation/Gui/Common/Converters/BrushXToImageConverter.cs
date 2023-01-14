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
using System.Globalization;
using System.Windows.Data;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Converters
{
  /// <summary>
  /// Converts a <see cref="BrushX"/> instance to an image that visualize that brush (intended for the brush preview panel).
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class BrushXToImageConverter : IMultiValueConverter, IDisposable
  {
    private GdiToWpfBitmap? _previewBitmap;

    /// <summary>
    /// Converts a <see cref="BrushX"/> value. 
    /// It is expected that the argument <paramref name="values"/> contains [0] the brush, [1] the width of the preview image, and [2] the height of the preview image.
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
      if (values is not null && values.Length >= 3 && values[0] is BrushX brush && values[1] is double dwidth && values[2] is double dheight)
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

        using (var grfx = _previewBitmap.BeginGdiPainting())
        {
          var fullRect = _previewBitmap.GdiRectangle;

          grfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
          grfx.FillRectangle(System.Drawing.Brushes.Transparent, fullRect);
          grfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

          var r2 = fullRect;
          r2.Inflate(-r2.Width / 4, -r2.Height / 4);
          //grfx.FillRectangle(System.Drawing.Brushes.Black, r2);

          using (var brushGdi = BrushCacheGdi.Instance.BorrowBrush(brush, fullRect.ToAxo(), grfx, 1))
          {
            grfx.FillRectangle(brushGdi, fullRect);
          }

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
