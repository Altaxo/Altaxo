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

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Forwards the validation function while keeping the signature of the validation function.
	/// </summary>
	public class ValidationByForwarding : ValidationRule
	{
		public Func<object, System.Globalization.CultureInfo, ValidationResult> ValidationFunction { get; set; }

		public ValidationByForwarding()
		{
		}

		public ValidationByForwarding(Func<object, System.Globalization.CultureInfo, ValidationResult> validationFunction)
		{
			ValidationFunction = validationFunction;
		}

		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			if (null != ValidationFunction)
				return ValidationFunction(value, cultureInfo);
			else
				return ValidationResult.ValidResult;
		}
	}

	/// <summary>
	/// Forwards the validation function. The validation function is expected to return an error string. If the string is null,
	/// the validation result is considered to be success. If the string is not null, the validation result is considered to be failure, and the error
	/// string is used in the validation result.
	/// </summary>
	public class ValidationWithErrorString : ValidationRule
	{
		public Func<object, System.Globalization.CultureInfo, string> ValidationFunction { get; set; }

		public ValidationWithErrorString()
		{
		}

		public ValidationWithErrorString(Func<object, System.Globalization.CultureInfo, string> validationFunction)
		{
			ValidationFunction = validationFunction;
		}

		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			if (null != ValidationFunction)
			{
				string result = ValidationFunction(value, cultureInfo);
				return null == result ? ValidationResult.ValidResult : new ValidationResult(false, result);
			}
			else
			{
				return ValidationResult.ValidResult;
			}
		}
	}

	/// <summary>
	/// Validates using <see cref="System.ComponentModel.CancelEventArgs"/>. If the <see cref="System.ComponentModel.CancelEventArgs.Cancel"/> property
	/// is set to true, the validation result will be considered to be invalid.
	/// </summary>
	public class ValidationWithCancelEventArgs : ValidationRule
	{
		public Action<object, System.Globalization.CultureInfo, System.ComponentModel.CancelEventArgs> ValidationFunction { get; set; }

		public ValidationWithCancelEventArgs()
		{
		}

		public ValidationWithCancelEventArgs(Action<object, System.Globalization.CultureInfo, System.ComponentModel.CancelEventArgs> validationFunction)
		{
			ValidationFunction = validationFunction;
		}

		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			if (null != ValidationFunction)
			{
				var result = new System.ComponentModel.CancelEventArgs();
				ValidationFunction(value, cultureInfo, result);
				return null == result ? ValidationResult.ValidResult : false == result.Cancel ? ValidationResult.ValidResult : new ValidationResult(false, "The value you entered is invalid");
			}
			else
			{
				return ValidationResult.ValidResult;
			}
		}
	}
}