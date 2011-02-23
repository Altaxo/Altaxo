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

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for TypeAndInstanceControl.xaml
	/// </summary>
	public partial class TypeAndInstanceControl : UserControl, ITypeAndInstanceView
	{
		public TypeAndInstanceControl()
		{
			InitializeComponent();
		}

		private void EhSelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			GuiHelper.SynchronizeSelectionFromGui(_cbTypeChoice);
			if (null != TypeChoiceChanged)
				TypeChoiceChanged(this, EventArgs.Empty);
		}

		#region  ITypeAndInstanceView

		public string TypeLabel
		{
			set { _lblCSType.Content = value; } 
		}

		public void InitializeTypeNames(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbTypeChoice,list);
		}

		UIElement _instanceControl;
		public void SetInstanceControl(object instanceControl)
		{
			if (null != _instanceControl)
				_grid.Children.Remove(_instanceControl);

			_instanceControl = instanceControl as UIElement;

			if (null != _instanceControl)
			{
				_instanceControl.SetValue(Grid.ColumnProperty, 0);
				_instanceControl.SetValue(Grid.ColumnSpanProperty, 2);
				_instanceControl.SetValue(Grid.RowProperty, 2);
				_grid.Children.Add(_instanceControl);
			}
		}

		public event EventHandler TypeChoiceChanged;
	}

		#endregion
}
