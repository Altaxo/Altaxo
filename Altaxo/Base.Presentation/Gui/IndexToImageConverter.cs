using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui
{
	public class IndexToImageConverter : IValueConverter
	{
		private List<ImageSource> _imageList;

		public IndexToImageConverter(IEnumerable<string> imageResourceNames)
		{
			_imageList = new List<ImageSource>();

			foreach (var name in imageResourceNames)
			{
				var bitmapSource = WpfResourceService.Instance.GetBitmapSource(name);
				_imageList.Add(bitmapSource);
			}
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is int)
			{
				int i = (int)value;
				if (i >= 0 && i < _imageList.Count)
					return _imageList[i];
				else
					return null;
			}
			else
				return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}