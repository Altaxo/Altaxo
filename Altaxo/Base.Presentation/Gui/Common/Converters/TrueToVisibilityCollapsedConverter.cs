#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Windows;
using System.Windows.Data;

namespace Altaxo.Gui.Common.Converters
{
  /// <summary>
  /// Converter that converts a boolean to a visibility. True is translated to <see cref="Visibility.Collapsed"/>, False is translated to <see cref="Visibility.Visibility"/>.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  [ValueConversion(typeof(bool), typeof(Visibility))]
  public class TrueToVisibilityCollapsedConverter : IValueConverter
  {
    public static TrueToVisibilityCollapsedConverter Instance { get; private set; } = new TrueToVisibilityCollapsedConverter();

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool isCollapsed)
      {
        return isCollapsed ? Visibility.Collapsed : Visibility.Visible;
      }
      return Binding.DoNothing;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Visibility visibility)
      {
        return visibility == Visibility.Collapsed ? true : false;
      }
      return Binding.DoNothing;
    }
  }
}
