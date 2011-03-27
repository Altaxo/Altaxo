using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
	public class ValidatingComboBox : ComboBox
	{
		NotifyChangedValue<string> _validatedText = new NotifyChangedValue<string>();
		bool _isInitialTextModified;
		bool _isValidatedSuccessfully = true;


		public ValidatingComboBox()
		{
			var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(TextBox.TextProperty, this.GetType());
			dpd.AddValueChanged(this, EhTextChanged);

			var binding = new Binding();
			binding.Source = this;
			binding.Path = new PropertyPath("ValidatedText");
			binding.ValidationRules.Add(new ValidationWithErrorString(this.EhValidateText));
			this.SetBinding(TextBox.TextProperty, binding);
		}


		#region Dependency property
		public string ValidatedText
		{
			get { var result = (string)GetValue(ValidatedTextProperty); return result; }
			set { SetValue(ValidatedTextProperty, value); _isValidatedSuccessfully = true; }
		}

		public static readonly DependencyProperty ValidatedTextProperty =
				DependencyProperty.Register("ValidatedText", typeof(string), typeof(ValidatingComboBox),
				new FrameworkPropertyMetadata(OnValidatedTextChanged));

		private static void OnValidatedTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{

		}
		#endregion


		public string InitialText
		{
			set
			{
				base.Text = value;
				_isInitialTextModified = false;
				_isValidatedSuccessfully = true;
			}
		}

		public bool IsInitialTextModified
		{
			get
			{
				return _isInitialTextModified;
			}
		}

		public bool IsValidatedSuccessfully
		{
			get
			{
				return _isValidatedSuccessfully;
			}
		}

		protected virtual void EhTextChanged(object sender, EventArgs e)
		{
			_isInitialTextModified = true;
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


	}
}

