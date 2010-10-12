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

using Altaxo.Collections;

namespace Altaxo.Gui.Worksheet
{
	/// <summary>
	/// Interaction logic for MasterCurveCreationDataControl.xaml
	/// </summary>
	public partial class MasterCurveCreationDataControl : UserControl, IMasterCurveCreationDataView
	{
		List<MasterCurveCreationDataColumnControl> _columns = new List<MasterCurveCreationDataColumnControl>();

		public MasterCurveCreationDataControl()
		{
			InitializeComponent();
		}

		void AddRemoveGroups(int numberOfGroups)
		{
			if (_columns.Count > numberOfGroups)
			{
				for (int i = _columns.Count - 1; i >= numberOfGroups; i--)
				{
					_dataGrid.ColumnDefinitions.RemoveAt(_dataGrid.ColumnDefinitions.Count-1);
					_dataGrid.Children.Remove(_columns[i]);
					_columns.RemoveAt(i);
				}
			}
			else if (_columns.Count < numberOfGroups)
			{
				for (int i = _columns.Count; i < numberOfGroups; i++)
				{
					var ele = new MasterCurveCreationDataColumnControl();
					ele.SetValue(Grid.ColumnProperty, i+1);
					ele.SetValue(Grid.RowProperty, 0);
					_columns.Add(ele);
					_dataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width=new GridLength(1, GridUnitType.Star) } );
					_dataGrid.Children.Add(ele);
				}
			}
		}

		public void InitializeListData(List<SelectableListNodeList> list)
		{
			AddRemoveGroups(list.Count);

			for (int srcGroupIdx = 0; srcGroupIdx < list.Count; srcGroupIdx++)
			{
				var srcGroup = list[srcGroupIdx];
				var lb = _columns[srcGroupIdx];
				GuiHelper.Initialize(lb.ItemList, srcGroup);
			}
		}
	}
}
