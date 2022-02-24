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

namespace Altaxo.Gui.Common.Converters
{
  /// <summary>
  /// Converts a <see cref="System.Drawing.Bitmap"/> to an ImageSource
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class SystemDrawingBitmapToImageSourceConverter : IValueConverter
  {
    public static SystemDrawingBitmapToImageSourceConverter Instance { get; } = new SystemDrawingBitmapToImageSourceConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if(value is System.Drawing.Bitmap bmp)
      {
        return GuiHelper.ToWpf(bmp);
      }
      else
      {
        return null!;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
