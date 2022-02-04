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
  /// Converter that converts value null or not null to a visibility. Null is translated to <see cref="Visibility.Collapsed"/>, NotNull is translated to <see cref="Visibility.Visible"/>.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class NullToVisibilityCollapsedConverter : IValueConverter
  {
    public static NullToVisibilityCollapsedConverter Instance { get; private set; } = new NullToVisibilityCollapsedConverter();

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is null ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Converter that converts value null or not null to a visibility. NotNull is translated to <see cref="Visibility.Collapsed"/>, null is translated to <see cref="Visibility.Visible"/>.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class NotNullToVisibilityCollapsedConverter : IValueConverter
  {
    public static NotNullToVisibilityCollapsedConverter Instance { get; private set; } = new NotNullToVisibilityCollapsedConverter();

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is not null ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Converter that converts value null or not null to a visibility. Null is translated to <see cref="Visibility.Hidden"/>, NotNull is translated to <see cref="Visibility.Visible"/>.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class NullToVisibilityHiddenConverter : IValueConverter
  {
    public static NullToVisibilityHiddenConverter Instance { get; private set; } = new NullToVisibilityHiddenConverter();

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is null ? Visibility.Hidden : Visibility.Visible;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Converter that converts value null or not null to a visibility. NotNull is translated to <see cref="Visibility.Hidden"/>, null is translated to <see cref="Visibility.Visible"/>.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class NotNullToVisibilityHiddenConverter : IValueConverter
  {
    public static NotNullToVisibilityHiddenConverter Instance { get; private set; } = new NotNullToVisibilityHiddenConverter();

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is not null ? Visibility.Hidden : Visibility.Visible;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }


}
