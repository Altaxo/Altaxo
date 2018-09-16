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
      var errors =
          value as ReadOnlyObservableCollection<ValidationError>;

      var result = new StringBuilder();
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
