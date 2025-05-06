#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
  /// Converts a boolean value to a <see cref="GridLength"/>. If the boolean is true, it returns a GridLength with the specified star value; otherwise, it returns GridLength.Auto.
  /// This converter is useful when a grid contains multiple Expanders, for dynamically adjusting the height of a grid row based on the expansion state of the expander.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  /// <example>
  /// <code>
  /// <Grid x:Name="MainGrid">
  ///   <Grid.Resources>
  ///     <local:ExpanderHeightConverter x:Key="HeightConverter"/>
  ///   </Grid.Resources>
  ///   <Grid.RowDefinitions>
  ///       <RowDefinition Height="{Binding IsExpanded, ElementName=Expander1, Converter={StaticResource HeightConverter}}" />
  ///       <RowDefinition Height="{Binding IsExpanded, ElementName=Expander2, Converter={StaticResource HeightConverter}}"/>
  ///   </Grid.RowDefinitions>
  ///
  ///   <Expander x:Name="Expander1" Header="Expander 1">
  ///       <TextBlock Text = "Content for Expander 1" />
  ///   </ Expander >
  ///   
  ///   < Expander x:Name="Expander2" Header="Expander 2" Grid.Row="1">
  ///       <TextBlock Text = "Content for Expander 2" />
  ///   </ Expander >
  /// </ Grid >

  /// </code>
  /// </example>
  public class ExpandedToGridHeightStarConverter : IValueConverter
  {
    /// <summary>
    /// Gets the instance.
    /// </summary>
    public static ExpandedToGridHeightStarConverter Instance { get; } = new ExpandedToGridHeightStarConverter();

    /// <summary>
    /// Converts a boolean value to a <see cref="GridLength"/>. If the boolean is true, it returns a GridLength with the specified star value; otherwise, it returns GridLength.Auto.
    /// </summary>
    /// <param name="value">The boolean value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// If the boolean is true, it returns a GridLength with the specified star value; otherwise, it returns GridLength.Auto.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool isExpanded)
      {
        double h;
        if (parameter is double d && d > 0)
          h = d;
        else if (parameter is int i && i > 0)
          h = i;
        else
          h = 1;

        return isExpanded ? new GridLength(h, GridUnitType.Star) : GridLength.Auto;
      }
      return GridLength.Auto;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException(); // No need for two-way conversion
    }
  }

}
