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
using System.Windows.Data;
using AvalonDock.Themes;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Converts a string, like for instance 'VS2010', to the theme with the same name.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class DockManagerThemeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is string themeName)
      {
        switch (themeName.ToLowerInvariant())
        {
          case "aero":
            return new AeroTheme();

          case "expressiondark":
            return new ExpressionDarkTheme();

          case "expressionlight":
            return new ExpressionLightTheme();

          case "metro":
            return new MetroTheme();

          case "vs2010":
            return new VS2010Theme();

          case "vs2013dark":
            return new Vs2013DarkTheme();

          case "vs2013light":
            return new Vs2013LightTheme();

          case "vs2013blue":
            return new Vs2013BlueTheme();
        }
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Binding.DoNothing;
    }
  }
}
