using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ICSharpCode.Core.Presentation;

namespace Altaxo.Gui.Pads.FileBrowser
{
	public class IndexToImageConverter : IValueConverter
	{
		static List<ImageSource> _imageList;

		static void Initialize()
		{
			_imageList = new List<ImageSource>();
			_imageList.Add(PresentationResourceService.GetBitmapSource("Icons.16x16.ClosedFolderBitmap"));
			_imageList.Add(PresentationResourceService.GetBitmapSource("Icons.16x16.OpenFolderBitmap"));
			_imageList.Add(PresentationResourceService.GetBitmapSource("Icons.16x16.FLOPPY"));
			_imageList.Add(PresentationResourceService.GetBitmapSource("Icons.16x16.DRIVE"));
			_imageList.Add(PresentationResourceService.GetBitmapSource("Icons.16x16.CDROM"));
			_imageList.Add(PresentationResourceService.GetBitmapSource("Icons.16x16.NETWORK"));
			_imageList.Add(PresentationResourceService.GetBitmapSource("Icons.16x16.Desktop"));
			_imageList.Add(PresentationResourceService.GetBitmapSource("Icons.16x16.PersonalFiles"));
			_imageList.Add(PresentationResourceService.GetBitmapSource("Icons.16x16.MyComputer"));
		}


		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (null == _imageList)
				Initialize(); // this late initialization is done here to avoid errors during xaml browsing

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
