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
	/// Interaction logic for FunctionPlotDataControl.xaml
	/// </summary>
	public partial class FunctionPlotDataControl : UserControl, IFunctionPlotDataView
	{
		public FunctionPlotDataControl()
		{
			InitializeComponent();
		}

		private void EhEditText_Click(object sender, RoutedEventArgs e)
		{
			if (EditText != null)
				EditText(this, e);
		}

		#region IFunctionPlotDataView Members

		public event EventHandler EditText;

		public void InitializeFunctionText(string text, bool editable)
		{
			_edText.Text = text;
			_btEditText.IsEnabled = editable;
		}

		#endregion

	}
}
