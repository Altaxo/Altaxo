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

namespace Altaxo.Gui.Serialization.Ascii
{
	/// <summary>
	/// Interaction logic for AsciiCharControl.xaml
	/// </summary>
	public partial class AsciiCharControl : UserControl
	{
		public AsciiCharControl()
		{
			InitializeComponent();
		}


		public string Text
		{
			get
			{
				return _guiEditBox.Text;
			}
			set
			{
				_guiEditBox.Text = value;
			}
		}

		private void EhTextChanged(object sender, TextChangedEventArgs e)
		{
			var tb = sender as TextBox;
			if (null == tb)
				return;

			string asciiValue;
			if (string.IsNullOrEmpty(tb.Text))
			{
				asciiValue = "[No value]";
			}
			else
			{
				char c = tb.Text[0];

				switch (c)
				{
					case ' ':
						asciiValue = "[Space]";
						break;
					case '\t':
						asciiValue = "[Tabulator]";
						break;
					default:
						asciiValue = string.Format("[0x{0:X}]", (int)c);
						break;
				}
			}
			_guiAsciiValue.Content = asciiValue;

			if (string.IsNullOrEmpty(tb.Text))
			{
				_guiAsciiValue.Foreground = Brushes.Red;
				_guiEmptyIndicator.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				_guiAsciiValue.Foreground = _guiEditBox.Foreground;
				_guiEmptyIndicator.Visibility = System.Windows.Visibility.Hidden;
			}

		}

		
	}
}
