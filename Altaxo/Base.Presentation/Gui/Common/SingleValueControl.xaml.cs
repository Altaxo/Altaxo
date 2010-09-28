using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for SingleValueControl.xaml
	/// </summary>
	public partial class SingleValueControl : UserControl , ISingleValueView
	{
		NotifyChangedValue<string> _editText = new NotifyChangedValue<string>();
		public SingleValueControl()
		{
			InitializeComponent();
			try
			{
				var binding = new Binding();
				binding.Source = _editText;
				binding.Path = new PropertyPath("Value");
				binding.ValidationRules.Add(new ValidationWithCancelEventArgs(this.ValidateText));
				_lblEditText.SetBinding(TextBox.TextProperty, binding);
			}
			catch (Exception ex)
			{
			}
		}

		#region ISingleValueView

		public string DescriptionText
		{
			set { _lblDescription.Content = value; }
		}

		public string ValueText
		{
			get
			{
				return _lblEditText.Text;
				//return _editText.Content;
			}
			set
			{
				//_lblEditText.Text = value;
				_editText.Value = value;
			}
		}

		public event System.ComponentModel.CancelEventHandler ValueText_Validating;

		#endregion


		private void ValidateText(object value, System.Globalization.CultureInfo cultureInfo, System.ComponentModel.CancelEventArgs ev)
		{
			if (null != ValueText_Validating)
			{
				ValueText_Validating(this, ev);
			}
		}
	}
}
