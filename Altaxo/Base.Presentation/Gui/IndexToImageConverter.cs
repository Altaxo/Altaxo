#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Windows.Data;
using System.Windows.Media;

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
				if (PresentationResourceService.InstanceAvailable)
					_imageList.Add(PresentationResourceService.GetBitmapSource(name));
				else
					_imageList.Add(null);
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