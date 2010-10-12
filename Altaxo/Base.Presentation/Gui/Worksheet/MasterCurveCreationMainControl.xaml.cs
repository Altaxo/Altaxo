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
	/// Interaction logic for MasterCurveCreationMainControl.xaml
	/// </summary>
	public partial class MasterCurveCreationMainControl : UserControl, IMasterCurveCreationMainView
	{
		public MasterCurveCreationMainControl()
		{
			InitializeComponent();
		}

		public void InitializeDataTab(object guiControl)
		{
			_dataTab.Content = guiControl;
		}

		public void InitializeEditTab(object guiControl)
		{
			_editTab.Content = guiControl;
		}
	}
}
