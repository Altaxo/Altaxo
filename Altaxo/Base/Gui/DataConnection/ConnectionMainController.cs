using Altaxo.Collections;
using Altaxo.DataConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.DataConnection
{
	public interface IConnectionMainView
	{
		/// <summary>Sets the cursor inside the control to the wait cursor.</summary>
		void SetWaitCursor();

		/// <summary>Sets the cursor inside the control to the normal (arrow) cursor.</summary>
		void SetNormalCursor();

		/// <summary>Gets/sets the text of the SQL query in the second tab.</summary>
		string SqlText { get; set; }

		/// <summary>Sets content of the tree view that shows the tables, views and stored procedures of a data base.</summary>
		void SetTreeSource(NGTreeNode rootNode);

		/// <summary>Gets the currently selected item of the tree view that shows the tables, views and stored procedures of a data base.</summary>
		NGTreeNode SelectedTreeItem { get; }

		/// <summary>Shows the tab page that contains the tree view that shows the tables, views and stored procedures of a data base.</summary>
		void ShowTableTabItem();

		/// <summary>Shows the tab page that contains the text of the SQL query.</summary>
		void ShowSqlTextTabItem();

		/// <summary>Returns <c>true</c> if the tab page with the tree view hat shows the tables, views and stored procedures of a data base is currently selected.</summary>
		bool IsTableTabItemSelected { get; }

		/// <summary>Enables/disables the SQL builder button and the Preview data button.</summary>
		void UpdateUI(bool enableSqlBuilder, bool enablePreviewData);

		/// <summary>Fires when the currently selected tab page changed.</summary>
		event Action SelectedTabChanged;

		/// <summary>Fires when the uses chooses to show the SQL builder dialog.</summary>
		event Action ShowSqlBuilder;

		/// <summary>Fires when the uses wants to see a preview of either the currently selected table, view or stored procedure (first tab active), or the result of the SQL query (second tab active)</summary>
		event Action PreviewTableData;

		/// <summary>Fires when the uses wants to create a new data connection string.</summary>
		event Action ChooseConnection;
	}

	[ExpectedTypeOfView(typeof(IConnectionMainView))]
	public class ConnectionMainController : IMVCAController
	{
		private IConnectionMainView _view;

		// current connection string and corresponding schema
		private string _connString;

		private OleDbSchema _schema;

		// max number of records shown on the preview dialog
		private const int MAX_PREVIEW_RECORDS = 5000;

		private const int _cmbConnStringMaxDropDownItems = 10;

		private SelectableListNodeList _connectionStringList;

		private NGTreeNode _treeRootNode;

		public void Initialize(bool initData)
		{
			if (initData)
			{
				_connectionStringList = new SelectableListNodeList();
			}
			if (null != _view)
			{
			}
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
					_view.SetWaitCursor();

					// look for item in the list
					var index = _connectionStringList.IndexOfFirst(x => value == (string)x.Tag);

					// get schema for the new connection string
					_schema = OleDbSchema.GetSchema(value);

					// handle good connection strings
					if (_schema != null)
					{
						// add good values to the list
						if (index < 0)
						{
							_connectionStringList.Insert(0, new SelectableListNode(value, value, true));
							index = 0;
						}
						else if (index > 0)
						{
							_connectionStringList.RemoveAt(index);
							_connectionStringList.Insert(0, new SelectableListNode(value, value, true));
							index = 0;
						}

						// trim list
						while (_connectionStringList.Count > _cmbConnStringMaxDropDownItems)
						{
							_connectionStringList.RemoveAt(_connectionStringList.Count - 1);
						}
					}
					else // handle bad connection strings
					{
						// remove from list
						if (index >= 0)
						{
							_connectionStringList.RemoveAt(index);
							index = -1;
						}

						// do not store bad values
						value = string.Empty;
					}

					// save new value
					_connString = value;

					// show new value in combo box and table tree
					_connectionStringList.ClearSelectionsAll();
					if (index > 0)
						_connectionStringList[index].IsSelected = true;

					UpdateTableTree();

					// new connection, clear SQL
					_view.SqlText = string.Empty;

					// update ui
					UpdateUI();

					// done
					_view.SetNormalCursor();
				}
			}
		}

		private void UpdateTableTree()
		{
			// initialize table tree
			_treeRootNode = new NGTreeNode();

			var nodes = _treeRootNode.Nodes;
			nodes.Clear();
			var ndTables = new NGTreeNode() { Text = Current.ResourceService.GetString("Gui.DataConnection.Tables"), ImageIndex = 0, SelectedImageIndex = 0 };
			var ndViews = new NGTreeNode() { Text = Current.ResourceService.GetString("Gui.DataConnection.Views"), ImageIndex = 1, SelectedImageIndex = 1 };
			var ndProcs = new NGTreeNode() { Text = Current.ResourceService.GetString("Gui.DataConnection.StoredProcedures"), ImageIndex = 1, SelectedImageIndex = 1 };

			// populate using current schema
			if (_schema != null)
			{
				// populate the tree
				foreach (System.Data.DataTable dt in _schema.Tables)
				{
					// create new node, save table in tag property
					var node = new NGTreeNode() { Text = dt.TableName };
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
				foreach (NGTreeNode nd in new NGTreeNode[] { ndTables, ndViews, ndProcs })
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
				if (null != _view)
				{
					_view.SetTreeSource(_treeRootNode);
					_view.ShowTableTabItem();
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
				if (null == _view)
					return null;

				// table/view/sproc
				if (_view.IsTableTabItemSelected)
				{
					var nd = _view.SelectedTreeItem;
					return nd == null || nd.Tag == null || _schema == null
							? string.Empty
							: OleDbSchema.GetSelectStatement(nd.Tag as System.Data.DataTable);
				}
				else // explicit sql statement
				{
					return _view.SqlText;
				}
			}
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
			if (_view.IsTableTabItemSelected)
			{
				// get table/view name
				var table = (_view.SelectedTreeItem).Tag as System.Data.DataTable;
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
					var ctrl = new DataPreviewController(dt);
					string title = string.Format("{0} ({1:n0} records)", dt.TableName, dt.Rows.Count);
					Current.Gui.ShowDialog(ctrl, title, false);
				}
			}
			catch (Exception x)
			{
				Current.Gui.ErrorMessageBox(string.Format("Failed to retrieve data:\r\n{0}", x.Message));
			}
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (null != _view)
				{
					_view.SelectedTabChanged -= EhSelectedTabChanged;
					_view.ShowSqlBuilder -= EhShowSqlBuilder;
					_view.PreviewTableData -= EhPreviewTableData;
					_view.ChooseConnection -= EhChooseConnection;
				}

				_view = value as IConnectionMainView;

				if (null != _view)
				{
					Initialize(false);
					_view.SelectedTabChanged += EhSelectedTabChanged;
					_view.ShowSqlBuilder += EhShowSqlBuilder;
					_view.PreviewTableData += EhPreviewTableData;
					_view.ChooseConnection += EhChooseConnection;
				}
			}
		}

		public void UpdateUI()
		{
			if (null != _view)
			{
				// enable sql builder button if we have some tables
				// enable data preview if we a select statement
				_view.UpdateUI(_treeRootNode.HasChilds, !string.IsNullOrEmpty(SelectStatement));
			}
		}

		private void EhSelectedTabChanged()
		{
			UpdateUI();
		}

		private void EhShowSqlBuilder()
		{
			var ctrl = new QueryDesignerController();
			ctrl.ConnectionString = ConnectionString;

			if (Current.Gui.ShowDialog(ctrl, "SQL Query Designer", false))
			{
				_view.SqlText = ctrl.SelectStatement;
				_view.ShowSqlTextTabItem();
				UpdateUI();
			}
		}

		private void EhPreviewTableData()
		{
			var nd = _view.SelectedTreeItem;
			if (nd != null && nd.Tag is System.Data.DataTable)
			{
				PreviewData();
			}
		}

		// pick a new connection
		private void EhChooseConnection()
		{
			// release mouse capture to avoid wait cursor
			//	_toolStrip.Capture = false;

			// get starting connection string
			// (if empty or no provider, start with SQL source as default)
			var connectionChoice = _connectionStringList.FirstSelectedNode;
			string connString = null != connectionChoice ? (string)connectionChoice.Tag : null;
			if (string.IsNullOrEmpty(connString) || connString.IndexOf("provider=", StringComparison.OrdinalIgnoreCase) < 0)
			{
				connString = "Provider=SQLOLEDB.1;";
			}

			// let user change it
			ConnectionString = OleDbConnString.EditConnectionString(connString);
		}

		// issue a warning
		private void Warning(string format, params object[] args)
		{
			string msg = string.Format(format, args);
			Current.Gui.ErrorMessageBox(msg);
		}

		public object ModelObject
		{
			get { return null; }
		}

		public void Dispose()
		{
			ViewObject = null;
		}

		public bool Apply()
		{
			return true;
		}
	}
}