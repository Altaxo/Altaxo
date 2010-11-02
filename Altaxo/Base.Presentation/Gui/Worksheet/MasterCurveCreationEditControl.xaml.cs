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

namespace Altaxo.Gui.Worksheet
{
	/// <summary>
	/// Interaction logic for MasterCurveCreationEditControl.xaml
	/// </summary>
	public partial class MasterCurveCreationEditControl : UserControl
	{
		public MasterCurveCreationEditControl()
		{
			InitializeComponent();
		}

		private void EhAutoShift_Click(object sender, RoutedEventArgs e)
		{
			var row = ((FrameworkElement)sender).DataContext;
			// cast row to a known type
		}
	}
}
