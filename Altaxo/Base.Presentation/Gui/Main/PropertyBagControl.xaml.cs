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
	public partial class PropertyBagControl : UserControl, IPropertyBagView
	{
		public PropertyBagControl()
		{
			InitializeComponent();
		}

		public Collections.SelectableListNodeList PropertyList
		{
			set { GuiHelper.Initialize(_guiPropertyList, value); }
		}
	}
}