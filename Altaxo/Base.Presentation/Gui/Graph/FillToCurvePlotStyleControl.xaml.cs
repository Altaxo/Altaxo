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
	/// Interaction logic for FillToCurvePlotStyleControl.xaml
	/// </summary>
	public partial class FillToCurvePlotStyleControl : UserControl, IFillToCurvePlotStyleView
	{
		public FillToCurvePlotStyleControl()
		{
			InitializeComponent();
		}

		#region IFillToCurvePlotStyleView Members

		public bool FillToPreviousItem
		{
			get
			{
				return true==_chkFillPrevious.IsChecked;
			}
			set
			{
				_chkFillPrevious.IsChecked = value;
			}
		}

		public bool FillToNextItem
		{
			get
			{
				return true==_chkFillNext.IsChecked;
			}
			set
			{
				_chkFillNext.IsChecked = value;
			}
		}

		public Altaxo.Graph.Gdi.BrushX FillColor
		{
			get
			{
				return _cbFillColor.SelectedBrush;
			}
			set
			{
				_cbFillColor.SelectedBrush = value;
			}
		}

		#endregion

	}
}
