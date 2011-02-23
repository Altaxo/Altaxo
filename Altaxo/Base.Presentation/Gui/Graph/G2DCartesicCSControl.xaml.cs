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

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for G2DCartesicCSControl.xaml
	/// </summary>
	public partial class G2DCartesicCSControl : UserControl, IG2DCartesicCSView
	{
		public G2DCartesicCSControl()
		{
			InitializeComponent();
		}

		public bool ExchangeXY
		{
			get
			{
				return _chkExchangeXY.IsChecked == true;
			}
			set
			{
				_chkExchangeXY.IsChecked = value;
			}
		}

		public bool ReverseX
		{
			get
			{
				return _chkXReverse.IsChecked == true;
			}
			set
			{
				_chkXReverse.IsChecked = value;
			}
		}

		public bool ReverseY
		{
			get
			{
				return _chkYReverse.IsChecked == true;
			}
			set
			{
				_chkYReverse.IsChecked = value;
			}
		}
	}
}
