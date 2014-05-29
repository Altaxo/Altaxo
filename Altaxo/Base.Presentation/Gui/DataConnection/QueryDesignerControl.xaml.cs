#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.DataConnection;
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
	/// Interaction logic for QueryDesignerControl.xaml
	/// </summary>
	public partial class QueryDesignerControl : UserControl
	{
		private event Action ChooseConnectionString;

		private event Action<bool> GroupByChanged;

		private event Action ChooseProperties;

		private event Action CheckSql;

		private event Action ViewResults;

		private event Action ClearQuery;

		public QueryDesignerControl()
		{
			InitializeComponent();
		}

		public void UpdateSqlDisplay(string sqlText, bool isStatusVisible)
		{
			_txtSql.Text = sqlText;
			_lblStatus.Visibility = isStatusVisible ? Visibility.Visible : Visibility.Hidden;
		}

		public void SetDataGridDataSource(QueryFieldCollection data, bool isGrouped)
		{
			_grid.ItemsSource = data;

			FixGridColumns();

			// update UI
			UpdateGridColumns(isGrouped);
		}

		// update state of the grid columns
		public void UpdateGridColumns(bool isGrouped)
		{
			throw new NotImplementedException();

			//_grid.Columns["Column"].Frozen = true;
			//_grid.Columns["GroupBy"].Visible = _builder.GroupBy;
		}

		// replace grid columns with ones with better editors
		private void FixGridColumns()
		{
			throw new NotImplementedException();
			/*

			for (int i = 0; i < _grid.Columns.Count; i++)
			{
				var col = _grid.Columns[i];
				if (col.ValueType.IsEnum)
				{
					// create combo column for enum types
					var cmb = new DataGridViewComboBoxColumn();
					cmb.ValueType = col.ValueType;
					cmb.Name = col.Name;
					cmb.DataPropertyName = col.DataPropertyName;
					cmb.HeaderText = col.HeaderText;
					cmb.DisplayStyleForCurrentCellOnly = true;
					cmb.DataSource = Enum.GetValues(col.ValueType);
					cmb.Width = col.Width;

					// replace original column with new combo column
					_grid.Columns.RemoveAt(i);
					_grid.Columns.Insert(i, cmb);
				}
				else if (col.Name == "Filter")
				{
					var btn = new DataGridViewButtonColumn();
					btn.ValueType = col.ValueType;
					btn.Name = col.Name;
					btn.DataPropertyName = col.DataPropertyName;
					btn.HeaderText = col.HeaderText;
					btn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
					btn.Width = col.Width;

					// replace original column with new combo column
					_grid.Columns.RemoveAt(i);
					_grid.Columns.Insert(i, btn);
				}
			}
			 */
		}

		private void _btnConnString_Click(object sender, RoutedEventArgs e)
		{
			var ev = ChooseConnectionString;
			if (null != ev)
			{
				ev();
			}
		}

		private void _btnGroupBy_Click(object sender, RoutedEventArgs e)
		{
			var ev = GroupByChanged;
			if (null != ev)
			{
				ev(_btnGroupBy.IsChecked == true);
			}
		}

		private void _btnProperties_Click(object sender, RoutedEventArgs e)
		{
			var ev = ChooseProperties;
			if (null != ev)
			{
				ev();
			}
		}

		private void _btnCheckSql_Click(object sender, RoutedEventArgs e)
		{
			var ev = CheckSql;
			if (null != ev)
			{
				ev();
			}
		}

		private void _btnViewResults_Click(object sender, RoutedEventArgs e)
		{
			var ev = ViewResults;
			if (null != ev)
			{
				ev();
			}
		}

		private void _btnClearQuery_Click(object sender, RoutedEventArgs e)
		{
			var ev = ClearQuery;
			if (null != ev)
			{
				ev();
			}
		}

		private void EhGrid_CellEndEdit(object sender, DataGridCellEditEndingEventArgs e)
		{
			_grid.CommitEdit(DataGridEditingUnit.Cell, true);
		}

		private void EhTreeMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			throw new NotImplementedException();
			var tvi = sender as TreeViewItem;
			if (null != tvi)
			{
				/*
				if (e.Node == _treeTables.SelectedNode &&
					e.Node.Tag is DataColumn)
				{
					AddField(e.Node.Tag);
				}
				 */
			}
		}

		private void EhGrid_BeginEdit(object sender, DataGridBeginningEditEventArgs e)
		{
			throw new NotImplementedException();
			/*

			if ("Filter"==((string)e.Column.Header))
			{
				using (var dlg = new FilterEditorForm())
				{
					var field = _grid.Rows[e.RowIndex].DataBoundItem as QueryField;
					dlg.Font = Font;
					dlg.QueryField = field;
					if (dlg.ShowDialog(this) == DialogResult.OK)
					{
						field.Filter = dlg.Value;
					}
				}
			}
			 */
		}

		private void EhTreeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			throw new NotImplementedException();
			/*
	// get node that was clicked
			Point pt = _treeTables.PointToClient(Control.MousePosition);
			TreeNode nd = _treeTables.GetNodeAt(pt);
			DataTable dt = nd == null ? null : nd.Tag as DataTable;

			// select node
			if (nd != null)
			{
				_treeTables.SelectedNode = nd;
			}

			// make sure this is a table node
			if (dt == null)
			{
				e.Cancel = true;
				return;
			}

			// populate related tables menu
			_mnuRelatedTables.DropDownItems.Clear();
			if (nd != null && nd.Tag is DataTable)
			{
				var list = new List<string>();
				foreach (DataRelation dr in _builder.Schema.Relations)
				{
					if (dr.ParentTable == dt && !list.Contains(dr.ChildTable.TableName))
					{
						list.Add(dr.ChildTable.TableName);
					}
					else if (dr.ChildTable == dt && !list.Contains(dr.ParentTable.TableName))
					{
						list.Add(dr.ParentTable.TableName);
					}
				}
				list.Sort();
				foreach (string tableName in list)
				{
					if (FindNode(tableName) != null)
					{
						_mnuRelatedTables.DropDownItems.Add(tableName);
					}
				}
			}
			  */
		}

		private void EhTreeMenu_HideThisTable(object sender, RoutedEventArgs e)
		{
		}

		private void EhTreeMenu_ShowAllTables(object sender, RoutedEventArgs e)
		{
		}

		private void EhTreeMenu_RelatedTables(object sender, RoutedEventArgs e)
		{
		}
	}
}