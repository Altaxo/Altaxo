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
using System.Windows;
using System.Windows.Data;

namespace Altaxo.Gui.Common.Converters
{
  /// <summary>
  /// Converter that converts the value to a visibility. Null value or an empty string is translated to <see cref="Visibility.Collapsed"/>; otherwise, <see cref="Visibility.Visible"/> is returned.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class NullOrStringEmptyToVisibilityCollapsedConverter : IValueConverter
  {
    public static NullOrStringEmptyToVisibilityCollapsedConverter Instance { get; private set; } = new NullOrStringEmptyToVisibilityCollapsedConverter();

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is null || (value is string s && string.IsNullOrEmpty(s)) ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Converter that converts a value to a visibility. Null value or an empty string is translated to <see cref="Visibility.Visible"/>, otherwise, <see cref="Visibility.Collapsed"/> is returned.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class NotNullOrStringEmptyToVisibilityCollapsedConverter : IValueConverter
  {
    public static NotNullOrStringEmptyToVisibilityCollapsedConverter Instance { get; private set; } = new NotNullOrStringEmptyToVisibilityCollapsedConverter();

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is not null && (value is not string s || !string.IsNullOrEmpty(s)) ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Converter that converts a value  to a visibility. Null value or an empty string is translated to <see cref="Visibility.Hidden"/>; otherwise, <see cref="Visibility.Visible"/> is returned.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class NullOrStringEmptyToVisibilityHiddenConverter : IValueConverter
  {
    public static NullOrStringEmptyToVisibilityHiddenConverter Instance { get; private set; } = new NullOrStringEmptyToVisibilityHiddenConverter();

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is null || (value is string s && string.IsNullOrEmpty(s)) ? Visibility.Hidden : Visibility.Visible;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Converter that converts a valueto a visibility. Null value or an empty string is translated to <see cref="Visibility.Visible"/>; otherwise, <see cref="Visibility.Hidden"/> is returned.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class NotNullOrStringEmptyToVisibilityHiddenConverter : IValueConverter
  {
    public static NotNullOrStringEmptyToVisibilityHiddenConverter Instance { get; private set; } = new NotNullOrStringEmptyToVisibilityHiddenConverter();

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is not null && (value is not string s || !string.IsNullOrEmpty(s)) ? Visibility.Hidden : Visibility.Visible;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }


}
