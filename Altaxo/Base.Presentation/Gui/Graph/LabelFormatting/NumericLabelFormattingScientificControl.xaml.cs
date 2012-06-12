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

namespace Altaxo.Gui.Graph.LabelFormatting
{
	/// <summary>
	/// Interaction logic for NumericLabelFormattingScientificControl.xaml
	/// </summary>
	public partial class NumericLabelFormattingScientificControl : UserControl, INumericLabelFormattingScientificView
	{
		public NumericLabelFormattingScientificControl()
		{
			InitializeComponent();
		}

		public bool ShowExponentAlways
		{
			get
			{
				return true == _guiShowExponentAlways.IsChecked;
			}
			set
			{
				_guiShowExponentAlways.IsChecked = value;
			}
		}
	}
}
