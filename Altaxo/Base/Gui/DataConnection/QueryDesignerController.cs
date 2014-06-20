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

using Altaxo.Collections;
using Altaxo.DataConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.DataConnection
{
	public interface IQueryDesignerView
	{
		/// <summary>Sets content of the tree view that shows the tables, views and stored procedures of a data base.</summary>
		/// <remarks>The image indices 0, 1, 2 and 3 correspond to the nodes: Table , View, Procedure and DataColumn.</remarks>
		void SetTableTreeDataSource(NGTreeNode rootNode);

		void SetDataGridDataSource(QueryFieldCollection data, bool isGrouped);

		void UpdateSqlDisplay(string sqlText, bool isStatusVisible);

		void UpdateGridColumns(bool isGrouped);

		/// <summary>Occurs when a user clicks the button to create/edit a connection string.</summary>
		event Action ChooseConnectionString;

		event Action<bool> GroupByChanged;

		event Action ChooseProperties;

		event Action CheckSql;

		event Action ViewResults;

		event Action ClearQuery;

		event Action<NGTreeNode> TreeNodeMouseDoubleClick;

		event Action<NGTreeNode, List<string>> RelatedTablesRequired;

		event Action<NGTreeNode> HideTableChosen;

		event Action ShowTablesAllChosen;

		event Action<string> RelatedTableNameChosen;
	}

	[ExpectedTypeOfView(typeof(IQueryDesignerView))]
	public class QueryDesignerController : IMVCAController
	{
		private QueryBuilder _builder;

		private NGTreeNode _treeTableNodes;

		private IQueryDesignerView _view;

		public QueryDesignerController()
		{
			Initialize(true);
		}

		/// <summary>
		/// Gets or sets the connection string that represents the underlying database.
		/// </summary>
		public string ConnectionString
		{
			get { return _builder.ConnectionString; }
			set
			{
				if (value != ConnectionString)
				{
					_builder.ConnectionString = value;
					UpdateTableTree();
				}
			}
		}

		/// <summary>
		/// Gets the SQL statement being built.
		/// </summary>
		public string SelectionStatement
		{
			get { return _builder.Sql; }
			set { }
		}

		// for easy access
		private OleDbSchema Schema
		{
			get { return _builder.Schema; }
		}

		protected void Initialize(bool initData)
		{
			if (initData)
			{
				// create query builder
				_builder = new QueryBuilder(new OleDbSchema());
				_builder.QueryFields.ListChanged += QueryFields_ListChanged;
			}
			if (null != _view)
			{
				// bind grid
				_view.SetDataGridDataSource(_builder.QueryFields, _builder.GroupBy);

				UpdateTableTree();

				_view.UpdateGridColumns(_builder.GroupBy);
			}
		}

		private void AttachView()
		{
			_view.ChooseConnectionString += EhChooseConnectionString;
			_view.GroupByChanged += EhGroupByChanged;
			_view.ChooseProperties += EhChooseProperties;
			_view.CheckSql += EhCheckSql;
			_view.ViewResults += EhViewResults;
			_view.ClearQuery += EhClearQuery;
			_view.TreeNodeMouseDoubleClick += EhTreeNodeMouseDoubleClick;
			_view.RelatedTablesRequired += EhRelatedTablesRequired;
			_view.HideTableChosen += EhHideTableChosen;

			_view.ShowTablesAllChosen += EhShowTablesAllChosen;

			_view.RelatedTableNameChosen += EhRelatedTableNameChosen;
		}

		private void DetachView()
		{
			_view.ChooseConnectionString -= EhChooseConnectionString;
			_view.GroupByChanged -= EhGroupByChanged;
			_view.ChooseProperties -= EhChooseProperties;
			_view.CheckSql -= EhCheckSql;
			_view.ViewResults -= EhViewResults;
			_view.ClearQuery -= EhClearQuery;
			_view.TreeNodeMouseDoubleClick -= EhTreeNodeMouseDoubleClick;
			_view.RelatedTablesRequired -= EhRelatedTablesRequired;
			_view.HideTableChosen -= EhHideTableChosen;

			_view.ShowTablesAllChosen -= EhShowTablesAllChosen;

			_view.RelatedTableNameChosen -= EhRelatedTableNameChosen;
		}

		private void EhRelatedTableNameChosen(string tableName)
		{
			var node = FindNode(tableName);
			if (node != null)
			{
				node.IsExpanded = true;
			}
		}

		private void EhShowTablesAllChosen()
		{
			UpdateTableTree();
		}

		private void EhHideTableChosen(NGTreeNode node)
		{
			if (null != node && node.Tag is System.Data.DataTable)
			{
				node.Remove();
			}
		}

		private void EhRelatedTablesRequired(NGTreeNode nd, List<string> resultingList)
		{
			resultingList.Clear();
			System.Data.DataTable dt = nd == null ? null : nd.Tag as System.Data.DataTable;

			if (null == dt)
				return;
			var list = new List<string>();
			foreach (System.Data.DataRelation dr in _builder.Schema.Relations)
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
					resultingList.Add(tableName);
				}
			}
		}

		private void EhTreeNodeMouseDoubleClick(NGTreeNode node)
		{
			if (null != node && (node.Tag is System.Data.DataColumn))
			{
				AddField(node.Tag);
			}
		}

		private void EhClearQuery()
		{
			if (true == Current.Gui.YesNoMessageBox("Are you sure you want to clear this query?", "Attention", false))
			{
				_builder.QueryFields.Clear();
			}
		}

		private void EhViewResults()
		{
			try
			{
				// get the data
				var da = new System.Data.OleDb.OleDbDataAdapter(SelectionStatement, ConnectionString);
				var dt = new System.Data.DataTable("Query");
				da.Fill(dt);

				// show the data
				using (var dlg = new DataPreviewController(dt))
				{
					Current.Gui.ShowDialog(dlg, "Preview data", false);
				}
			}
			catch (Exception x)
			{
				var msg = string.Format("Failed to retrieve data:\r\n{0}", x.Message);
				Current.Gui.ErrorMessageBox(msg, "Warning");
			}
		}

		private void EhCheckSql()
		{
			try
			{
				var da = new System.Data.OleDb.OleDbDataAdapter(SelectionStatement, ConnectionString);
				var dt = new System.Data.DataTable();
				da.FillSchema(dt, System.Data.SchemaType.Mapped);
				Current.Gui.InfoMessageBox(

						"The SQL syntax has been verified against the data source.",
						"Success");
			}
			catch (Exception x)
			{
				var msg = string.Format("Failed to retrieve the data:\r\n{0}", x.Message);
				Current.Gui.ErrorMessageBox(msg, "Warning");
			}
		}

		private void EhChooseProperties()
		{
			using (var dlg = new QueryPropertiesController(_builder))
			{
				if (Current.Gui.ShowDialog(dlg, "Properties", false))
				{
					_view.UpdateSqlDisplay(_builder.Sql, _builder.MissingJoins);
				}
			}
		}

		private void EhGroupByChanged(bool isGrouped)
		{
			// update sql
			_builder.GroupBy = isGrouped;
			_view.UpdateSqlDisplay(_builder.Sql, _builder.MissingJoins);

			// show/hide the GroupBy column on the grid
			_view.UpdateGridColumns(isGrouped);
		}

		private void EhChooseConnectionString()
		{
			ConnectionString = OleDbConnString.EditConnectionString(ConnectionString);
		}

		private void QueryFields_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
		{
			if (null != _view)
				_view.UpdateSqlDisplay(_builder.Sql, _builder.MissingJoins);
		}

		// update table tree to reflect new connection string
		private void UpdateTableTree()
		{
			// initialize table tree
			NGTreeNode rootNode = new NGTreeNodeWithImageIndex();
			var ndTables = new NGTreeNodeWithImageIndex { Text = Current.ResourceService.GetString("Gui.DataConnection.Tables"), ImageIndex = 0, SelectedImageIndex = 0 };
			var ndViews = new NGTreeNodeWithImageIndex { Text = Current.ResourceService.GetString("Gui.DataConnection.Views"), ImageIndex = 1, SelectedImageIndex = 1 };

			// populate using current schema
			if (Schema != null)
			{
				// populate the tree
				foreach (System.Data.DataTable dt in Schema.Tables)
				{
					// create new node, save table in tag property
					var node = new NGTreeNodeWithImageIndex { Text = dt.TableName };
					node.Tag = dt;

					// add new node to appropriate parent
					switch (OleDbSchema.GetTableType(dt))
					{
						case TableType.Table:
							ndTables.Nodes.Add(node);
							node.ImageIndex = node.SelectedImageIndex = 0;
							AddDataColumns(node, dt);
							break;

						case TableType.View:
							ndViews.Nodes.Add(node);
							node.ImageIndex = node.SelectedImageIndex = 1;
							AddDataColumns(node, dt);
							break;
					}
				}

				// add non-empty nodes to tree
				foreach (NGTreeNode nd in new NGTreeNode[] { ndTables, ndViews })
				{
					if (nd.Nodes.Count > 0)
					{
						nd.Text = string.Format("{0} ({1})", nd.Text, nd.Nodes.Count);
						rootNode.Nodes.Add(nd);
					}
				}

				// expand tables node
				ndTables.IsExpanded = true;

				_treeTableNodes = rootNode;
				if (null != _view)
					_view.SetTableTreeDataSource(_treeTableNodes);
			}
		}

		private void AddDataColumns(NGTreeNode node, System.Data.DataTable dt)
		{
			foreach (System.Data.DataColumn col in dt.Columns)
			{
				var field = new NGTreeNodeWithImageIndex { Text = col.ColumnName };
				field.Tag = col;
				field.ImageIndex = 3;
				field.SelectedImageIndex = 3;
				node.Nodes.Add(field);
			}
		}

		private void AddTable(System.Data.DataTable dt)
		{
			var field = new QueryField(dt);
			_builder.QueryFields.Add(field);
			SelectField(field);
		}

		private void AddColumn(System.Data.DataColumn dc)
		{
			var field = new QueryField(dc);
			_builder.QueryFields.Add(field);
			SelectField(field);
		}

		// add tables or columns to the sql statement
		private void AddField(object element)
		{
			var dt = element as System.Data.DataTable;
			if (dt != null)
			{
				AddTable(dt);
			}
			var dc = element as System.Data.DataColumn;
			if (dc != null)
			{
				AddColumn(dc);
			}
		}

		// select a field on the grid
		private void SelectField(QueryField field)
		{
			//var cm = BindingContext[_grid.DataSource] as CurrencyManager;
			//cm.Position = cm.List.IndexOf(field);
		}

		// find a node on the tree
		private NGTreeNode FindNode(string text)
		{
			return FindNode(_treeTableNodes, text);
		}

		private NGTreeNode FindNode(NGTreeNode parentNode, string text)
		{
			foreach (NGTreeNode node in parentNode.Nodes)
			{
				// check this node
				var dt = node.Tag as System.Data.DataTable;
				if (dt != null && dt.TableName == text)
				{
					return node;
				}

				// and check child nodes
				var child = FindNode(node, text);
				if (child != null)
				{
					return child;
				}
			}

			// not found...
			return null;
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
					DetachView();
				}

				_view = value as IQueryDesignerView;

				if (null != _view)
				{
					Initialize(false);
					AttachView();
				}
			}
		}

		public object ModelObject
		{
			get { return SelectionStatement; }
		}

		public void Dispose()
		{
			ViewObject = null;
		}

		public bool Apply()
		{
			return !string.IsNullOrEmpty(_builder.ConnectionString) && !string.IsNullOrEmpty(SelectionStatement);
		}
	}
}