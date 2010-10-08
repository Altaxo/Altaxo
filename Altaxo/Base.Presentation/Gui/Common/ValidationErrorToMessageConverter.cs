using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Converts a collection of validation error results into a string message that can be used to be shown close to the element where the error has occured.
	/// </summary>
	[ValueConversion(typeof(ReadOnlyObservableCollection<ValidationError>), typeof(string))]
	public class ValidationErrorToMessageConverter : MarkupExtension, IValueConverter
	{
		private ValidationErrorToMessageConverter _converter;

		
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (_converter == null)
			{
				_converter = new ValidationErrorToMessageConverter();
			}
			return _converter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ReadOnlyObservableCollection<ValidationError> errors =
					value as ReadOnlyObservableCollection<ValidationError>;

			StringBuilder result = new StringBuilder();
			if (null != errors)
			{
				for (int i = 0; i < errors.Count; i++)
				{
					if (i == errors.Count - 1)
						result.Append(errors[i].ErrorContent.ToString());
					else
						result.AppendLine(errors[i].ErrorContent.ToString());
				}
			}

			return result.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}


	}
}