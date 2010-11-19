using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
	public class ValidatingTextBox : TextBox
	{
		NotifyChangedValue<string> _validatedText = new NotifyChangedValue<string>();

		public new string Text
		{
			get 
			{
				return _validatedText.Value;
			}
			set
			{ 
				_validatedText.Value = value;
			} 
		}

		/// <summary>
		/// Is called when the content of the TextBox needs validation.
		/// </summary>
		public event ValidatingStringEventHandler Validating;

		public string EhValidateText(object obj, System.Globalization.CultureInfo info)
		{
			var evt = Validating;
			if (null != evt)
			{
				var e = new ValidationEventArgs<string>((string)this.GetValue(TextBox.TextProperty), info);
				evt(this, e);
				return e.ErrorText;
			}
			else
			{
				return null;
			}
		}

		public ValidatingTextBox()
		{
			try
			{
				var binding = new Binding();
				binding.Source = _validatedText;
				binding.Path = new PropertyPath("Value");
				binding.ValidationRules.Add(new ValidationWithErrorString(this.EhValidateText));
				this.SetBinding(TextBox.TextProperty, binding);
			}
			catch (Exception ex)
			{
			}
		}
	}
}
