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
		List<List<UIElement>> _dataElements = new List<List<UIElement>>();
		int _currentDisplayedHeaderColumns;

		public MasterCurveCreationDataControl()
		{
			InitializeComponent();
		}

		private void ReleaseList()
		{
			foreach (var group in _dataElements)
			{
				foreach (var ele in group)
				{
					_dataGrid.Children.Remove(ele);
				}
			}
			_dataElements.Clear();
		}

		private void UpdateDataGrid()
		{
			int nColumns = _dataElements.Count;
			int nRows = 0;
			foreach (var group in _dataElements)
				nRows = Math.Max(nRows, group.Count);

			// if ColumnDefinitions or RowDefinitions don't match, add or delete them
			for (int i = _dataGrid.ColumnDefinitions.Count; i < nColumns; i++)
				_dataGrid.ColumnDefinitions.Add(new ColumnDefinition());
			for (int i = _dataGrid.RowDefinitions.Count; i < nRows; i++)
				_dataGrid.RowDefinitions.Add(new RowDefinition());

			for (int nCol = 0; nCol < nColumns; nCol++)
			{
				var colGroup = _dataElements[nCol];
				for (int nRow = 0; nRow < colGroup.Count; nRow++)
				{
					var uiEle = colGroup[nRow];
					_dataGrid.Children.Add(uiEle);
				}
			}
		}

		void UpdateHeaderGrid()
		{
			if (_dataElements.Count < _currentDisplayedHeaderColumns)
			{
				var toRemove = new List<UIElement>();
				foreach (FrameworkElement ele in _headerGrid.Children)
				{
					int col = (int)ele.GetValue(Grid.ColumnProperty);
					if (col >= _dataElements.Count)
						toRemove.Add(ele);
				}
				foreach (var ele in toRemove)
					_headerGrid.Children.Remove(ele);

				for (int i = _currentDisplayedHeaderColumns - 1; i >= _dataElements.Count; i--)
					_headerGrid.ColumnDefinitions.RemoveAt(i);
			}
			else if (_dataElements.Count > _currentDisplayedHeaderColumns)
			{
				for (int i = _currentDisplayedHeaderColumns; i < _dataElements.Count; i++)
				{
					_headerGrid.ColumnDefinitions.Add(new ColumnDefinition());

					var ele = new TextBox() { Text = string.Format("Group {0}", i) };
					ele.SetValue(Grid.ColumnProperty, i);
					ele.SetValue(Grid.RowProperty, 0);
					_headerGrid.Children.Add(ele);
				}
			}
			_currentDisplayedHeaderColumns = _dataElements.Count;
		}



		public void InitializeListData(List<SelectableListNodeList> list)
		{

			for (int srcGroupIdx = 0; srcGroupIdx < list.Count; srcGroupIdx++)
			{
				var srcGroup = list[srcGroupIdx];
				var destGroup = new List<UIElement>();
				_dataElements.Add(destGroup);
				for(int srcEleIdx=0;srcEleIdx<srcGroup.Count;srcEleIdx++)
				{
					var srcEle = srcGroup[srcEleIdx];

					var destEle = new TextBox() { Text = srcEle.Name };
					destGroup.Add(destEle);

					destEle.SetValue(Grid.ColumnProperty, srcGroupIdx);
					destEle.SetValue(Grid.RowProperty, srcEleIdx);
				}
			}

			UpdateDataGrid();
		}
	}
}
