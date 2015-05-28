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

namespace Altaxo.Gui.Analysis.Statistics
{
	/// <summary>
	/// Interaction logic for LogarithmicBinningControl.xaml
	/// </summary>
	public partial class LogarithmicBinningControl : UserControl, ILogarithmicBinningView
	{
		public LogarithmicBinningControl()
		{
			InitializeComponent();
		}

		public bool IsUserDefinedBinOffset
		{
			get
			{
				return true == _guiIsUserDefinedBinOffset.IsChecked;
			}
			set
			{
				_guiIsUserDefinedBinOffset.IsChecked = value;
			}
		}

		public double BinOffset
		{
			get
			{
				return _guiBinOffset.SelectedValue;
			}
			set
			{
				_guiBinOffset.SelectedValue = value;
			}
		}

		public bool IsUserDefinedBinWidth
		{
			get
			{
				return true == _guiIsUserDefinedBinWidth.IsChecked;
			}
			set
			{
				_guiIsUserDefinedBinWidth.IsChecked = value;
			}
		}

		public double BinWidth
		{
			get
			{
				return _guiBinWidth.SelectedValue;
			}
			set
			{
				_guiBinWidth.SelectedValue = value;
			}
		}

		public double ResultingBinCount
		{
			set { _guiBinCount.SelectedValue = value; }
		}
	}
}