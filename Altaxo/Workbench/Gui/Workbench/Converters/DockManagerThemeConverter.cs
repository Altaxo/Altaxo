using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Themes;

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
