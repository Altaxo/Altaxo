using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui
{
	public class ValidationEventArgs<T> : EventArgs
	{
		string _errors;
		T _valueToValidate;
		public System.Globalization.CultureInfo CultureInfo { get; private set; }

		public ValidationEventArgs(T valueToValidate)
		{
			_valueToValidate = valueToValidate;
		}

		public ValidationEventArgs(T valueToValidate, System.Globalization.CultureInfo info)
		{
			_valueToValidate = valueToValidate;
			CultureInfo = info;
		}

		public void AddError(string format, params object[] args)
		{
			if (_errors == null)
				_errors = string.Format(format, args);
			else
				_errors += "\n" + string.Format(format, args);
		}

		public T ValueToValidate
		{
			get
			{
				return _valueToValidate;
			}
		}

		public bool Cancel
		{
			get
			{
				return null != _errors;
			}
		}

		public bool HasErrors
		{
			get
			{
				return null != _errors;
			}
		}

		public string ErrorText
		{
			get
			{
				return _errors;
			}
		}
	}

	/// <summary>
	/// Event handler to validate content of Gui elements.
	/// </summary>
	/// <param name="sender">Sender of the validation request.</param>
	/// <param name="e">Validating event args. In case that the validation is not successfull, the receiver has to add an error message to the event args.</param>
	public delegate void ValidatingEventHandler<T>(object sender, ValidationEventArgs<T> e);

	/// <summary>
	/// Event handler to validate content of Gui elements that contain strings (like TextBox).
	/// </summary>
	/// <param name="sender">Sender of the validation request.</param>
	/// <param name="e">Validating event args. In case that the validation is not successfull, the receiver has to add an error message to the event args.</param>
	public delegate void ValidatingStringEventHandler(object sender, ValidationEventArgs<string> e);
}
