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
	public partial class FreeLabelFormattingControl : UserControl, IFreeLabelFormattingView
	{
		public FreeLabelFormattingControl()
		{
			InitializeComponent();
		}


		public IMultiLineLabelFormattingBaseView MultiLineLabelFormattingBaseView { get { return _guiMultiLineLabelFormattingControl; } }



		public string FormatString
		{
			get
			{
				return _guiFormatString.Text;
			}
			set
			{
				_guiFormatString.Text = value;
			}
		}
	}
}
