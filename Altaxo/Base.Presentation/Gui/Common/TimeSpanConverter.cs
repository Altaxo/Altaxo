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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
  public class TimeSpanConverter : ValidationRule, IValueConverter
  {
    private System.Globalization.CultureInfo _conversionCulture = Altaxo.Settings.GuiCulture.Instance;

    private string _lastConvertedString;
    private TimeSpan? _lastConvertedValue;

    public TimeSpanConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureDontUseIsBuggy)
    {
      var val = (TimeSpan)value;

      if (null != _lastConvertedString && val == _lastConvertedValue)
      {
        return _lastConvertedString;
      }

      _lastConvertedValue = val;
      _lastConvertedString = val.ToString("c", _conversionCulture);
      return _lastConvertedString;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureDontUseIsBuggy)
    {
      var validationResult = ConvertAndValidate(value, out var result);
      if (validationResult.IsValid)
      {
        _lastConvertedString = (string)value;
        _lastConvertedValue = result;
      }
      return result;
    }

    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureDontUseIsBuggy)
    {
      var validationResult = ConvertAndValidate(value, out var result);
      if (validationResult.IsValid)
      {
        _lastConvertedString = (string)value;
        _lastConvertedValue = result;
      }
      return validationResult;
    }

    private ValidationResult ConvertAndValidate(object value, out TimeSpan result)
    {
      var s = (string)value;

      if (null != _lastConvertedValue && s == _lastConvertedString)
      {
        result = (TimeSpan)_lastConvertedValue;
        return ValidateSuccessfullyConvertedValue(result);
      }

      if (TimeSpan.TryParse(s, _conversionCulture, out result))
      {
        return ValidateSuccessfullyConvertedValue(result);
      }

      return new ValidationResult(false, "String could not be converted to a TimeSpan");
    }

    private ValidationResult ValidateSuccessfullyConvertedValue(TimeSpan result)
    {
      return ValidationResult.ValidResult;
    }
  }
}
