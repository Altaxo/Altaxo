using Altaxo.Main.Services;
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
	/// Converts a string to another string by calling <see cref="Altaxo.Main.Services.StringParser.Parse(string)"/> and returning the resulting string.
	/// </summary>
	/// <seealso cref="System.Windows.Data.IValueConverter" />
	public class StringParserConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string resourceIdentifier)
			{
				return StringParser.Parse(resourceIdentifier);
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}