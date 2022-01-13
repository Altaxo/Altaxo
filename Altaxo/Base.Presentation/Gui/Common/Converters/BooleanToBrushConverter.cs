#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Windows.Media;

namespace Altaxo.Gui.Common.Converters
{
  /// <summary>
  /// Converter that converts a boolean to a brush. The default brush for true is Red, for false is Black.
  /// The brushes for true and false can be set using <see cref="BrushForTrue"/> and <see cref="BrushForFalse"/>.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class BooleanToBrushConverter : IValueConverter
  {
    public Brush BrushForTrue { get; set; } = Brushes.Red;
    public Brush BrushForFalse { get; set; } = Brushes.Black;

    public static BooleanToBrushConverter Instance { get; private set; } = new BooleanToBrushConverter();

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (value is bool isSet && isSet == true) ?
        BrushForTrue : BrushForFalse;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
