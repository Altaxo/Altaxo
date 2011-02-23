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

namespace Altaxo.Gui.Graph.Scales.Ticks
{
	/// <summary>
	/// Interaction logic for AngularTickSpacingControl.xaml
	/// </summary>
	public partial class AngularTickSpacingControl : UserControl, IAngularTickSpacingView
	{
		public AngularTickSpacingControl()
		{
			InitializeComponent();
		}

		private void _cbOrigin_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			GuiHelper.SynchronizeSelectionFromGui(_cbOrigin);
		}

		private void _cbMajorTicks_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			GuiHelper.SynchronizeSelectionFromGui(_cbMajorTicks);
			if (null != MajorTicksChanged)
				MajorTicksChanged(sender, e);
		}

		private void _cbMinorTicks_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			GuiHelper.SynchronizeSelectionFromGui(_cbMinorTicks);
		}

		#region IAngularTickSpacingView

		public bool UseDegrees
		{
			get
			{
				return true==_rbDegree.IsChecked;
			}
			set
			{
				if (value)
					_rbDegree.IsChecked = true;
				else
					_rbRadian.IsChecked = true;
			}
		}

		public bool UsePositiveNegativeValues
		{
			get
			{
				return true==_chkPosNegValues.IsChecked;
			}
			set
			{
				_chkPosNegValues.IsChecked = value;
			}
		}

		public Collections.SelectableListNodeList MajorTicks
		{
			set { GuiHelper.Initialize(_cbMajorTicks, value); }
		}

		public Collections.SelectableListNodeList MinorTicks
		{
			set { GuiHelper.Initialize(_cbMinorTicks, value); }
		}

		public event EventHandler MajorTicksChanged;
	}

		#endregion IAngularTickSpacingView
}
