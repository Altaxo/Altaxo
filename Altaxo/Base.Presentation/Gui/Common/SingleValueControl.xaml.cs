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
		public SingleValueControl()
		{
			InitializeComponent();
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
			}
			set
			{
				_lblEditText.Text = value;
			}
		}

		public event System.ComponentModel.CancelEventHandler ValueText_Validating;

		#endregion

		private void EhLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (null != ValueText_Validating)
			{
				var ev = new System.ComponentModel.CancelEventArgs();
				ValueText_Validating(this, ev);
				if (ev.Cancel == true)
				{
					_lblEditText.Focus();
					e.Handled = true;
				}
			}


		}
	}
}
