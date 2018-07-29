#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
