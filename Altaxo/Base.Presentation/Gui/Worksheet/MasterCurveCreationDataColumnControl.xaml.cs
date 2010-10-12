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
	/// Interaction logic for MasterCurveCreationDataColumnControl.xaml
	/// </summary>
	public partial class MasterCurveCreationDataColumnControl : UserControl
	{
		public MasterCurveCreationDataColumnControl()
		{
			InitializeComponent();
		}

		public ListBox ItemList
		{
			get
			{
				return _itemList;
			}
		}

		public void SetTitle(string title)
		{
			_headerLabel.Content = title;
		}
	}
}
