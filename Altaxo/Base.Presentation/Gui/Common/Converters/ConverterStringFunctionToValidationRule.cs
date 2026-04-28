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
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common.Converters
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

        if (result is null)
          return ValidationResult.ValidResult;
        else
          return new ValidationResult(false, result);
      }
    }

    /// <summary>
    /// Converts a function, such as <see cref="Func{T1, T2, TResult}"/>, to a <see cref="ValidationRule"/>.
    /// </summary>
    /// <param name="value">The value that should be converted. It must be a function that accepts an object and a culture and returns a string.</param>
    /// <param name="targetType">The target type. It is ignored.</param>
    /// <param name="parameter">The converter parameter. It is ignored.</param>
    /// <param name="culture">The culture. It is ignored.</param>
    /// <returns>
    /// A validation rule that delegates validation to the supplied function. If the function returns <c>null</c>, the value is considered valid. If it returns a string,
    /// the string is used as the error message of the resulting <see cref="ValidationResult"/>.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var func = (Func<object, System.Globalization.CultureInfo, string>)value;
      return new ValidationRuleUsingProxyStringFunction(func);
    }

    /// <summary>
    /// Not implemented. This converter only supports one-way conversion.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>Does not return a value.</returns>
    /// <exception cref="System.NotImplementedException">Always thrown because reverse conversion is not supported.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
