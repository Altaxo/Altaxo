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
	public interface IConnectionMainView
	{
		/// <summary>Sets the cursor inside the control to the wait cursor.</summary>
		void SetWaitCursor();

		/// <summary>Sets the cursor inside the control to the normal (arrow) cursor.</summary>
		void SetNormalCursor();

		/// <summary>
		/// Sets the source for the connection combo box.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="currentValue">Current value of the connection string.</param>
		void SetConnectionListSource(SelectableListNodeList list, string currentValue);

		void SetConnectionStatus(bool isValidConnectionSource);

		void SetTabItemsSource(SelectableListNodeList tabItems);

		/// <summary>Fires when the currently selected tab page changed.</summary>
		event Action SelectedTabChanged;

		/// <summary>Fires when the uses wants to create a new data connection string.</summary>
		event Action CmdChooseConnectionStringFromDialog;

		/// <summary>Fires when the user selects another connection string in the connection string combobox. The argument is the selected or newly entered connection string.</summary>
		event Action ConnectionStringSelectedFromList;

		/// <summary>
		/// Occurs when the user enters a new connection string an then presses enter.
		/// </summary>
		event Action<string> ConnectionStringChangedByUser;
	}

	[ExpectedTypeOfView(typeof(IConnectionMainView))]
	public class ConnectionMainController : IMVCAController
	{
		private static SelectableListNodeList _staticConnectionStringList = new SelectableListNodeList();

		private IConnectionMainView _view;

		// current connection string and corresponding schema
		private string _connectionString;

		private bool _isConnectionStringValid;

		private string _selectionStatement;

		private OleDbSchema _schema;

		private SelectableListNodeList _connectionStringList;

		/// <summary>
		/// The list of tab items, the tag of each item is the controller used for this tab.
		/// </summary>
		private SelectableListNodeList _tabItemList;

		// max number of records shown on the preview dialog
		private const int MAX_PREVIEW_RECORDS = 5000;

		private const int _cmbConnStringMaxDropDownItems = 10;

		private EntireTableQueryController _entireTableQueryController;
		private QueryDesignerController _queryDesignerController;
		private ArbitrarySqlQueryController _arbitrarySqlQueryController;
		private IMVCAController _currentlySelectedController;

		public ConnectionMainController(string sqlStatement, string connectionString)
		{
			_entireTableQueryController = new EntireTableQueryController();
			_queryDesignerController = new QueryDesignerController();
			_arbitrarySqlQueryController = new ArbitrarySqlQueryController();
			_currentlySelectedController = _arbitrarySqlQueryController;
			_selectionStatement = sqlStatement;

			_connectionStringList = new SelectableListNodeList(_staticConnectionStringList);
			ConnectionString = connectionString;

			Initialize(true);
		}

		public ConnectionMainController(OleDbDataQuery query)
			: this(query.SelectionStatement, query.ConnectionString)
		{
		}

		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
			protected set
			{
				if (string.IsNullOrEmpty(value))
					return;

				var oldValue = _connectionString;
				var oldValueValid = _isConnectionStringValid;

				_connectionString = value;

				if (_connectionString != oldValue)
				{
					_isConnectionStringValid = IsConnectionStringValid(_connectionString);
				}

				if (oldValue != _connectionString || oldValueValid != _isConnectionStringValid)
					OnConnectionStringChanged();
			}
		}

		/// <summary>
		/// Gets a SQL statement that corresponds to the element that
		/// is currently selected (table, view, stored procedure, or
		/// explicit sql statement).
		/// </summary>
		public string SelectionStatement
		{
			get
			{
				return _selectionStatement;
			}
		}

		public void Initialize(bool initData)
		{
			if (initData)
			{
				_tabItemList = new SelectableListNodeList();

				_entireTableQueryController.ConnectionString = ConnectionString;
				_currentlySelectedController = _entireTableQueryController;
				_tabItemList.Add(new SelectableListNode("Single table", _entireTableQueryController, object.ReferenceEquals(_entireTableQueryController, _currentlySelectedController)));

				_queryDesignerController.ConnectionString = ConnectionString;
				_tabItemList.Add(new SelectableListNode("Query builder", _queryDesignerController, object.ReferenceEquals(_queryDesignerController, _currentlySelectedController)));

				_arbitrarySqlQueryController.ConnectionString = ConnectionString;
				_arbitrarySqlQueryController.SelectionStatement = _selectionStatement;
				_tabItemList.Add(new SelectableListNode("Arbitrary Sql statement", _arbitrarySqlQueryController, object.ReferenceEquals(_arbitrarySqlQueryController, _currentlySelectedController)));
			}
			if (null != _view)
			{
				_view.SetConnectionListSource(_connectionStringList, ConnectionString);
				_view.SetConnectionStatus(_isConnectionStringValid);

				if (null == _entireTableQueryController.ViewObject)
					Current.Gui.FindAndAttachControlTo(_entireTableQueryController);

				if (null == _queryDesignerController.ViewObject)
					Current.Gui.FindAndAttachControlTo(_queryDesignerController);

				if (null == _arbitrarySqlQueryController.ViewObject)
					Current.Gui.FindAndAttachControlTo(_arbitrarySqlQueryController);

				_view.SetTabItemsSource(_tabItemList);
			}
		}

		private bool IsConnectionStringValid(string connString)
		{
			if (string.IsNullOrEmpty(connString))
				return false;

			// get schema for the new connection string
			_schema = OleDbSchema.GetSchema(connString);

			return _schema != null;
		}

		private static int InsertConnectionStringAtBeginningOfList(SelectableListNodeList list, string connString)
		{
			// look for item in the list
			var index = list.IndexOfFirst(x => connString == (string)x.Tag);

			// add good values to the list
			if (index < 0) // was not before in the list
			{
				list.ClearSelectionsAll();
				list.Insert(0, new SelectableListNode(connString, connString, true));
				index = 0; // inserted at index 0
			}
			else if (index > 0) // was in the list before
			{
				list.ClearSelectionsAll();
				list.RemoveAt(index);
				list.Insert(0, new SelectableListNode(connString, connString, true));
				index = 0;
			}

			// trim list
			while (list.Count > _cmbConnStringMaxDropDownItems)
			{
				list.RemoveAt(list.Count - 1);
			}

			return index;
		}

		private static void RemoveConnectionStringFromList(SelectableListNodeList list, string connString)
		{
			// look for item in the list
			var index = list.IndexOfFirst(x => connString == (string)x.Tag);
			if (index >= 0)
				list.RemoveAt(index);
		}

		/// <summary>
		/// Called whenever the connection string has changed.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>True if the current connection string has changed.</returns>
		private void OnConnectionStringChanged()
		{
			int index;

			// handle good connection strings
			if (_isConnectionStringValid)
			{
				index = InsertConnectionStringAtBeginningOfList(_connectionStringList, ConnectionString);
			}
			else  // _schema is null, thus handle bad connection strings
			{
				index = -1;
				RemoveConnectionStringFromList(_connectionStringList, ConnectionString);
			}

			var controllerConnectionString = _isConnectionStringValid ? _connectionString : string.Empty;
			_entireTableQueryController.ConnectionString = controllerConnectionString;
			_queryDesignerController.ConnectionString = controllerConnectionString;
			_arbitrarySqlQueryController.ConnectionString = controllerConnectionString;

			if (null != _view)
			{
				_view.SetConnectionListSource(_connectionStringList, _connectionString);
				_view.SetConnectionStatus(_isConnectionStringValid);
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
					DetachView();
				}

				_view = value as IConnectionMainView;

				if (null != _view)
				{
					Initialize(false);
					AttachView();
				}
			}
		}

		private void AttachView()
		{
			_view.SelectedTabChanged += EhSelectedTabChanged;
			_view.CmdChooseConnectionStringFromDialog += EhChooseConnection;
			_view.ConnectionStringSelectedFromList += EhConnectionStringSelectedFromList;
			_view.ConnectionStringChangedByUser += EhConnectionStringChangedByUser;
		}

		private void DetachView()
		{
			_view.SelectedTabChanged -= EhSelectedTabChanged;
			_view.CmdChooseConnectionStringFromDialog -= EhChooseConnection;
			_view.ConnectionStringSelectedFromList -= EhConnectionStringSelectedFromList;
			_view.ConnectionStringChangedByUser -= EhConnectionStringChangedByUser;
		}

		private void EhSelectedTabChanged()
		{
			var applyResult = false;
			var oldController = _currentlySelectedController;
			if (null != oldController)
				applyResult = oldController.Apply();

			_currentlySelectedController = _tabItemList.FirstSelectedNode == null ? null : _tabItemList.FirstSelectedNode.Tag as IMVCAController;

			if (_currentlySelectedController is ArbitrarySqlQueryController && applyResult == true)
			{
				if (oldController is QueryDesignerController)
					_arbitrarySqlQueryController.SelectionStatement = _queryDesignerController.SelectionStatement;
				else if (oldController is EntireTableQueryController)
					_arbitrarySqlQueryController.SelectionStatement = _entireTableQueryController.SelectionStatement;
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
			var newConnString = OleDbConnString.EditConnectionString(connString);

			if (string.IsNullOrEmpty(newConnString))
				return;

			ConnectionString = newConnString;
		}

		private void EhConnectionStringSelectedFromList()
		{
			var node = _connectionStringList.FirstSelectedNode;
			if (node != null)
				ConnectionString = (string)node.Tag;
		}

		private void EhConnectionStringChangedByUser(string newConnectionString)
		{
			ConnectionString = newConnectionString;
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
			bool result = false;

			var node = _tabItemList.FirstSelectedNode;
			_currentlySelectedController = node == null ? null : node.Tag as IMVCAController;

			if (null != _currentlySelectedController)
			{
				result = _currentlySelectedController.Apply();
			}

			if (result)
			{
				if (_currentlySelectedController == _entireTableQueryController)
					_selectionStatement = _entireTableQueryController.SelectionStatement;
				else if (_currentlySelectedController == _queryDesignerController)
					_selectionStatement = _queryDesignerController.SelectionStatement;
				else if (_currentlySelectedController == _arbitrarySqlQueryController)
					_selectionStatement = _arbitrarySqlQueryController.SelectionStatement;
			}
			else
			{
				return false;
			}

			if (_isConnectionStringValid)
				InsertConnectionStringAtBeginningOfList(_staticConnectionStringList, ConnectionString);

			return !string.IsNullOrEmpty(_connectionString) && !string.IsNullOrEmpty(_selectionStatement);
		}
	}
}