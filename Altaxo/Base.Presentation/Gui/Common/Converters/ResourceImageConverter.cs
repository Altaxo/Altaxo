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
	/// Converter from a resource identifier string to a Wpf imagesource.
	/// </summary>
	/// <seealso cref="System.Windows.Data.IValueConverter" />
	public class ResourceImageConverter : IValueConverter
	{
		public static ResourceImageConverter Instance { get; private set; } = new ResourceImageConverter();

		/// <summary>
		/// Converts a resource identifier string to a Wpf imagesource.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string resourceIdentifier)
			{
				return PresentationResourceService.GetBitmapSource(resourceIdentifier);
			}

			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}