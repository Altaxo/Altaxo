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

namespace Altaxo.Gui.DataConnection
{
	/// <summary>
	/// Interaction logic for AltaxoOleDbDataSourceControl.xaml
	/// </summary>
	public partial class AltaxoOleDbDataSourceControl : UserControl, IAltaxoOleDbDataSourceView
	{
		public AltaxoOleDbDataSourceControl()
		{
			InitializeComponent();
		}

		public void SetQueryView(object viewObject)
		{
			_guiDataQueryBox.Content = viewObject;
		}

		public void SetImportOptionsView(object viewObject)
		{
			_guiImportOptionsBox.Content = viewObject;
		}
	}
}