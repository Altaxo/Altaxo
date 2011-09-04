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
#endregion

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
		bool _isInitialTextModified;
		bool _isValidatedSuccessfully = true;


		public ValidatingTextBox()
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
				DependencyProperty.Register("ValidatedText", typeof(string), typeof(ValidatingTextBox),
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

