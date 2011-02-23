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

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Interaction logic for BrushSimpleConditionalControl.xaml
	/// </summary>
	public partial class BrushSimpleConditionalControl : UserControl
	{
		public BrushSimpleConditionalControl()
		{
			InitializeComponent();
		}
		public Altaxo.Graph.Gdi.BrushX SelectedBrush
		{
			get
			{
				return _cbBrush.SelectedBrush;
			}
			set
			{
				_cbBrush.SelectedBrush = value;
			}
		}

		public bool IsBrushEnabled
		{
			get
			{
				return _chkEnableBrush.IsChecked == true;
			}
			set
			{
				_chkEnableBrush.IsChecked = value;
			}
		}
	}
}
