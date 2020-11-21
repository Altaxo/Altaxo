#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Altaxo.Gui.Pads.FileBrowser
{
  public class IndexToImageConverter : IValueConverter
  {
    private static List<ImageSource> _imageList;

    private static void Initialize()
    {
      _imageList = new List<ImageSource>
      {
        PresentationResourceService.GetBitmapSource("Icons.16x16.ClosedFolderBitmap"),
        PresentationResourceService.GetBitmapSource("Icons.16x16.OpenFolderBitmap"),
        PresentationResourceService.GetBitmapSource("Icons.16x16.FLOPPY"),
        PresentationResourceService.GetBitmapSource("Icons.16x16.DRIVE"),
        PresentationResourceService.GetBitmapSource("Icons.16x16.CDROM"),
        PresentationResourceService.GetBitmapSource("Icons.16x16.NETWORK"),
        PresentationResourceService.GetBitmapSource("Icons.16x16.Desktop"),
        PresentationResourceService.GetBitmapSource("Icons.16x16.PersonalFiles"),
        PresentationResourceService.GetBitmapSource("Icons.16x16.MyComputer")
      };
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (_imageList is null)
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
