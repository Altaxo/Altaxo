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
	/// Interaction logic for SingleCharSeparationStrategyControl.xaml
	/// </summary>
	public partial class SingleCharSeparationStrategyControl : UserControl, ISingleCharSeparationStrategyView
	{
		public SingleCharSeparationStrategyControl()
		{
			InitializeComponent();
		}

		public char SeparatorChar
		{
			get
			{
				var txt = _guiSeparationChar.Text;
				if (txt.Length == 0)
					return ' ';
				else if (txt.Length == 1)
					return txt[0];
				else
				{
					var txtTrim = txt.Trim();
					if (txtTrim.Length == 1)
						return txtTrim[0];
					else
						return txt[0];
				}
			}
			set
			{
				_guiSeparationChar.Text = string.Empty + value;
			}
		}
	}
}
