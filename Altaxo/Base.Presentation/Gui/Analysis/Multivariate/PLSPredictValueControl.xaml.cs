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
	/// Interaction logic for PLSPredictValueControl.xaml
	/// </summary>
	public partial class PLSPredictValueControl : UserControl, IPLSPredictValueView
	{
		public PLSPredictValueControl()
		{
			InitializeComponent();
		}

		#region IPLSPredictValueView

		public void InitializeCalibrationModelTables(string[] tables)
		{
			cbCalibrationModelTable.Items.Clear();
			cbCalibrationModelTable.ItemsSource = tables;
			if (tables.Length > 0)
				this.cbCalibrationModelTable.SelectedIndex = 0;
		}

		public void InitializeDestinationTables(string[] tables)
		{
			this.cbDestinationTable.Items.Clear();
			this.cbDestinationTable.ItemsSource=tables;
			if (tables.Length > 0) 
				this.cbDestinationTable.SelectedIndex = 0;
		}

		public int GetCalibrationTableChoice()
		{
			return this.cbCalibrationModelTable.SelectedIndex;
		}

		public int GetDestinationTableChoice()
		{
			return this.cbDestinationTable.SelectedIndex;
		}

		#endregion
	}
}
