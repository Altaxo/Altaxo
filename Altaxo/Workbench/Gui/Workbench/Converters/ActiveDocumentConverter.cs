using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Altaxo.Gui.Workbench
{
	/// <summary>
	/// Helper class for binding ActiveContent property of the AvalonDock DockingManager to the ActiveDocument property of the workbench viewmodel.
	/// The converter decides if the ActiveContent is an ActiveDocument.
	/// If it is, the binding will be executed; if not, the binding will do nothing in order to keep the ActiveDocument as it is.
	/// </summary>
	/// <seealso cref="IValueConverter" />
	internal class ActiveDocumentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is IViewContent)
				return value;
			else if (value is IPadContent padContent)
				return padContent.PadDescriptor;
			else if (value == null)
				return null;

			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is IViewContent)
				return value;
			else if (value is PadDescriptor padDescriptor)
				return padDescriptor.PadContent;
			else if (value == null)
				return null;
			else
				return Binding.DoNothing;
		}
	}
}