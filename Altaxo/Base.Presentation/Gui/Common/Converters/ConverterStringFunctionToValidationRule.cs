#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Converts a function Func&lt;object System.Globalization.CultureInfo, string&gt; into a ValidationRule. When the <see cref="ValidationRule"/>'s <c>Validate</c> function is called,
  /// the call is transfered to the provided function (the two arguments of the function are the arguments from the <c>Validate</c> call. If the function returns <c>null</c>,
  /// the ValidationResult is valid. If the function returns a valid string, the ValidationResult is not valid, and the parameter of the ValidationResult is the returned string.
  /// </summary>
  /// <remarks>
  /// The converter is useful when the validation should remain in the model, and the usage of <see cref="System.ComponentModel.IDataErrorInfo"/> is not appropriate (e.g. when your model should not be spoiled with invalid data).
  /// In your model, you have to implement a property, which later can be used to bound, and the validation function itself.
  /// </remarks>
  /// <example>
  /// In your model class implement a property, for instance 'RenamingValidationFunction' which returns the validation function, and of course implement the validation function itself:
  /// <code>
  /// //<![CDATA[
  ///
  /// 	public Func<object, System.Globalization.CultureInfo, string> RenamingValidationFunction
  /// 	{
  /// 		get
  /// 		{
  /// 			return ValidateRenaming;
  /// 		}
  /// 	}
  ///
  /// 		private string ValidateRenaming(object obj, System.Globalization.CultureInfo info)
  /// 		{
  /// 			string name = (string)obj;
  ///		 		if(string.IsNullOrEmpty(name))
  /// 				return "The name must not be empty";
  /// 			else
  /// 				return null;
  /// 		}
  /// //]]>
  /// </code>
  ///
  /// In your XAML, define this converter as a static resource, and if a validation rule is required, bind it to the 'RenamingValidationFunction' property, and for the converter use the static resource for the converter.
  ///
  /// </example>
  public class ConverterStringFuncToValidationRule : IValueConverter
  {
    private class ValidationRuleUsingProxyStringFunction : ValidationRule
    {
      private Func<object, System.Globalization.CultureInfo, string> _validationFunc;

      internal ValidationRuleUsingProxyStringFunction(Func<object, System.Globalization.CultureInfo, string> validationFunc)
      {
        _validationFunc = validationFunc;
      }

      public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
      {
        string result = _validationFunc(value, cultureInfo);

        if (null == result)
          return ValidationResult.ValidResult;
        else
          return new ValidationResult(false, result);
      }
    }

    /// <summary>
    /// Converts a function: Func&lt;object, System.Globalization.CultureInfo, string&gt; to a <see cref="ValidationRule"/>.
    /// </summary>
    /// <param name="value">The value that should be converted. Has to be a Func&lt;object, System.Globalization.CultureInfo, string&gt; instance.</param>
    /// <param name="targetType">Ignored.</param>
    /// <param name="parameter">Ignored.</param>
    /// <param name="culture">Ignored.</param>
    /// <returns>
    /// A validation rule, which uses the provided function to get validation results. If the provided function returns null, this is considered as successful validation. If the provided function returns a non-null string,
    /// this is considered as an error message, and the validation is considered unsuccessful, the returned string is used as the parameter for the <see cref="ValidationResult"/>.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var func = (Func<object, System.Globalization.CultureInfo, string>)value;
      return new ValidationRuleUsingProxyStringFunction(func);
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// Not implemented.
    /// </returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
