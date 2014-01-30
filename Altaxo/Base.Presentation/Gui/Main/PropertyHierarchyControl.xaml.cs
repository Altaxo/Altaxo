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

namespace Altaxo.Gui.Main
{
	/// <summary>
	/// Interaction logic for PropertyBagControl.xaml
	/// </summary>
	public partial class PropertyHierarchyControl : UserControl, IPropertyHierarchyView
	{
		public event Action ItemEditing;

		public PropertyHierarchyControl()
		{
			InitializeComponent();
		}

		public Collections.SelectableListNodeList PropertyList
		{
			set { GuiHelper.Initialize(_guiPropertyList, value); }
		}

		private void EhListViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiPropertyList);
			var ev = ItemEditing;
			if (null != ev)
				ev();
		}
	}
}