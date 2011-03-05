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
	/// Interaction logic for DensityImagePlotItemControl.xaml
	/// </summary>
	public partial class DensityImagePlotItemControl : UserControl, IDensityImagePlotItemOptionView
	{
		public DensityImagePlotItemControl()
		{
			InitializeComponent();
		}

		private void EhCopyImageToClipboard(object sender, RoutedEventArgs e)
		{
			if (null != CopyImageToClipboard)
				CopyImageToClipboard();
		}

		private void EhSaveImageToDisc(object sender, RoutedEventArgs e)
		{
			if (null != SaveImageToDisc)
				SaveImageToDisc();
		}

		#region  IDensityImagePlotItemOptionView

		public event Action CopyImageToClipboard;

		public event Action SaveImageToDisc;

		#endregion 
	}
}
