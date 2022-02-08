using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

using System.Windows.Data;
using System.Windows;
using Altaxo.Drawing;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;

namespace Altaxo.Gui.Common.Converters
{
  /// <summary>
  /// Converts a <see cref="IColorProvider"/> instance to an image that visualize that provider
  /// (intended for the color provider preview panel).
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class ColorProviderToImageConverter : IValueConverter, IDisposable
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
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is IColorProvider colorProvider)
      {
        const int previewWidth = 128;
        const int previewHeight = 16;

        _previewBitmap ??= new GdiToWpfBitmap(previewWidth, previewHeight);

        var sdBitmap = _previewBitmap.GdiBitmap;

          for (int i = 0; i < previewWidth; i++)
          {
            double relVal = i / (double)(previewWidth - 1);
            var c = colorProvider.GetColor(relVal);
            for (int j = 0; j < previewHeight; j++)
              sdBitmap.SetPixel(i, j, c);
          }


        return _previewBitmap.WpfBitmapSource;
      }
      else
      {
        return Binding.DoNothing;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
