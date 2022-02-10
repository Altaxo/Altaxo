using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Altaxo.Gui.Graph.Plot.Data;

namespace Altaxo.Gui.Common.Converters
{
  /// <summary>
  /// Converts the status of the plot data to a background brush.
  /// </summary>
  /// <seealso cref="System.Windows.Data.IValueConverter" />
  public class PlotDataSeverityToBackground : IValueConverter
  {
    public static PlotDataSeverityToBackground Instance { get; } = new PlotDataSeverityToBackground();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if(value is int severity)
      {
        return DefaultSeverityColumnColors.GetSeverityColor(severity);
      }
      return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
