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
	/// Interaction logic for TickSpacingControl.xaml
	/// </summary>
	public partial class TickSpacingControl : UserControl, ITickSpacingView
	{
		public TickSpacingControl()
		{
			InitializeComponent();
		}

		public event Action TickSpacingTypeChanged;

		public void InitializeTickSpacingType(Collections.SelectableListNodeList names)
		{
			//ComboBox _cbTickSpacingType = (ComboBox)LogicalTreeHelper.FindLogicalNode((DependencyObject)_tickSpacingGroupBox.Header, "_cbTickSpacingType");
			GuiHelper.Initialize(_cbTickSpacingType, names);
		}

		private void EhTickSpacingType_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != TickSpacingTypeChanged)
			{
				ComboBox _cbTickSpacingType = (ComboBox)sender;
				GuiHelper.SynchronizeSelectionFromGui(_cbTickSpacingType);
				TickSpacingTypeChanged();
			}
		}

		public void SetTickSpacingView(object guiobject)
		{
			_guiDetailsHost.Child = guiobject as UIElement;
		}
	}
}