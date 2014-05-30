using Altaxo.Collections;
using Altaxo.DataConnection;
using System;

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
	/// Interaction logic for Form1.xaml
	/// </summary>
	public partial class ConnectionMainControl : UserControl, IConnectionMainView
	{
		// current connection string and corresponding schema
		private string _connString;

		private OleDbSchema _schema;

		// max number of records shown on the preview dialog
		private const int MAX_PREVIEW_RECORDS = 5000;

		private const int _cmbConnStringMaxDropDownItems = 10;

		public ConnectionMainControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		public string ConnectionString
		{
			get { return _connString; }
			set
			{
				if (value != ConnectionString)
				{
					// this may take a while
					Cursor = Cursors.Wait;

					// look for item in the list
					var items = _cmbConnString.Items;
					var index = items.IndexOf(value);

					// get schema for the new connection string
					_schema = OleDbSchema.GetSchema(value);

					// handle good connection strings
					if (_schema != null)
					{
						// add good values to the list
						if (index < 0)
						{
							items.Insert(0, value);
						}
						else if (index > 0)
						{
							items.RemoveAt(index);
							items.Insert(0, value);
						}

						// trim list
						while (items.Count > _cmbConnStringMaxDropDownItems)
						{
							items.RemoveAt(items.Count - 1);
						}
					}
					else // handle bad connection strings
					{
						// remove from list
						if (index >= 0)
						{
							items.RemoveAt(index);
						}

						// do not store bad values
						value = string.Empty;
					}

					// save new value
					_connString = value;

					// show new value in combo box and table tree
					_cmbConnString.Text = value;
					UpdateTableTree();

					// new connection, clear SQL
					_txtSql.Text = string.Empty;

					// update ui
					UpdateUI();

					// done
					Cursor = null;
				}
			}
		}

		/// <summary>
		/// Gets a SQL statement that corresponds to the element that
		/// is currently selected (table, view, stored procedure, or
		/// explicit sql statement).
		/// </summary>
		public string SelectStatement
		{
			get
			{
				// table/view/sproc
				if (_tab.SelectedItem == _pgTables)
				{
					var nd = _treeTables.SelectedItem as TableTreeNode;
					return nd == null || nd.Tag == null || _schema == null
							? string.Empty
							: OleDbSchema.GetSelectStatement(nd.Tag as System.Data.DataTable);
				}
				else // explicit sql statement
				{
					return _txtSql.Text;
				}
			}
		}

		private void _tab_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateUI();
		}

		private void UpdateUI()
		{
			// enable sql builder button if we have some tables
			_btnSqlBuilder.IsEnabled = _treeTables.Items.Count > 0;

			// enable data preview if we a select statement
			_btnPreviewData.IsEnabled = !string.IsNullOrEmpty(SelectStatement);
		}

		private class TableTreeNode : NGTreeNode
		{
			public int ImageIndex { get; set; }

			public int SelectedImageIndex { get; set; }
		}

		private void UpdateTableTree()
		{
			// initialize table tree
			TableTreeNode rootNode = new TableTreeNode();

			var nodes = rootNode.Nodes;
			nodes.Clear();
			var ndTables = new TableTreeNode() { Text = Current.ResourceService.GetString("Gui.DataConnection.Tables"), ImageIndex = 0, SelectedImageIndex = 0 };
			var ndViews = new TableTreeNode() { Text = Current.ResourceService.GetString("Gui.DataConnection.Views"), ImageIndex = 1, SelectedImageIndex = 1 };
			var ndProcs = new TableTreeNode() { Text = Current.ResourceService.GetString("Gui.DataConnection.StoredProcedures"), ImageIndex = 1, SelectedImageIndex = 1 };

			// populate using current schema
			if (_schema != null)
			{
				// populate the tree
				foreach (System.Data.DataTable dt in _schema.Tables)
				{
					// create new node, save table in tag property
					var node = new TableTreeNode() { Text = dt.TableName };
					node.Tag = dt;

					// add new node to appropriate parent
					switch (OleDbSchema.GetTableType(dt))
					{
						case TableType.Table:
							ndTables.Nodes.Add(node);
							node.ImageIndex = node.SelectedImageIndex = 0;
							break;

						case TableType.View:
							ndViews.Nodes.Add(node);
							node.ImageIndex = node.SelectedImageIndex = 1;
							break;

						case TableType.Procedure:
							ndProcs.Nodes.Add(node);
							node.ImageIndex = node.SelectedImageIndex = 2;
							break;
					}
				}

				// add non-empty nodes to tree
				foreach (TableTreeNode nd in new TableTreeNode[] { ndTables, ndViews, ndProcs })
				{
					if (nd.Nodes.Count > 0)
					{
						nd.Text = string.Format("{0} ({1})", nd.Text, nd.Nodes.Count);
						nodes.Add(nd);
					}
				}

				// expand tables node
				ndTables.IsExpanded = true;

				// done
				_treeTables.ItemsSource = rootNode.Nodes;
				_tab.SelectedIndex = 0;
			}
		}

		private void _btnSqlBuilder_Click(object sender, RoutedEventArgs e)
		{
			var ctrl = new QueryDesignerController();
			ctrl.ConnectionString = ConnectionString;

			if (Current.Gui.ShowDialog(ctrl, "SQL Query Designer", false))
			{
				_txtSql.Text = ctrl.SelectStatement;
				_tab.SelectedItem = _pgSql;
				UpdateUI();
			}
		}

		private void _tab_SelectedIndexChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			UpdateUI();
		}

		private void _treeTables_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			var nd = _treeTables.SelectedItem;
			if (nd != null && ((TableTreeNode)nd).Tag is System.Data.DataTable)
			{
				PreviewData();
			}
		}

		// pick a new connection
		private void _btnConnPicker_Click(object sender, RoutedEventArgs e)
		{
			// release mouse capture to avoid wait cursor
			//	_toolStrip.Capture = false;

			// get starting connection string
			// (if empty or no provider, start with SQL source as default)
			string connString = _cmbConnString.Text;
			if (string.IsNullOrEmpty(connString) || connString.IndexOf("provider=", StringComparison.OrdinalIgnoreCase) < 0)
			{
				connString = "Provider=SQLOLEDB.1;";
			}

			// let user change it
			ConnectionString = OleDbConnString.EditConnectionString(connString);
		}

		// preview data for currently selected node
		private void PreviewData()
		{
			// make sure we have a select statement
			var sql = SelectStatement;
			if (string.IsNullOrEmpty(sql))
			{
				return;
			}

			// create table to load with data and display
			var dt = new System.Data.DataTable("Query");

			// if a table/view is selected, get table name and parameters
			if (_tab.SelectedItem == _pgTables)
			{
				// get table/view name
				var table = (_treeTables.SelectedItem as TableTreeNode).Tag as System.Data.DataTable;
				dt.TableName = table.TableName;

				// get view parameters if necessary
				var parms = OleDbSchema.GetTableParameters(table);
				if (parms != null && parms.Count > 0)
				{
					var ctrl = new ParametersController(parms);
					if (!Current.Gui.ShowDialog(ctrl, "Parameter", false))
					{
						return;
					}
				}
			}

			// get data
			try
			{
				using (var da = new System.Data.OleDb.OleDbDataAdapter(SelectStatement, ConnectionString))
				{
					// get data
					da.Fill(0, MAX_PREVIEW_RECORDS, dt);

					// show the data
					var ctrl = new DataPreviewController(dt, new System.Drawing.Size((int)ActualWidth, (int)ActualHeight));
					string title = string.Format("{0} ({1:n0} records)", dt.TableName, dt.Rows.Count);
					Current.Gui.ShowDialog(ctrl, title, false);
				}
			}
			catch (Exception x)
			{
				Current.Gui.ErrorMessageBox(string.Format("Failed to retrieve data:\r\n{0}", x.Message));
			}
		}

		// issue a warning
		private void Warning(string format, params object[] args)
		{
			string msg = string.Format(format, args);
			Current.Gui.ErrorMessageBox(msg);
		}
	}
}