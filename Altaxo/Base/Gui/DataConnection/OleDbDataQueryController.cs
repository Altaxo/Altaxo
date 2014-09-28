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
	public interface IOleDbDataQueryView
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

	[ExpectedTypeOfView(typeof(IOleDbDataQueryView))]
	[UserControllerForObject(typeof(OleDbDataQuery))]
	public class OleDbDataQueryController : MVCANControllerBase<OleDbDataQuery, IOleDbDataQueryView>
	{
		#region Inner classes

		private struct StringValidIndicator
		{
			private AltaxoOleDbConnectionString _connectionString;
			private bool _isConnectionStringValid;

			public StringValidIndicator(AltaxoOleDbConnectionString connectionString, bool isValid)
			{
				_connectionString = connectionString;
				_isConnectionStringValid = isValid;
			}

			public AltaxoOleDbConnectionString ConnectionString { get { return _connectionString; } }

			public bool IsConnectionStringValid { get { return _isConnectionStringValid; } }
		}

		#endregion Inner classes

		private static SelectableListNodeList _staticConnectionStringList = new SelectableListNodeList();

		private StringValidIndicator _connectionStringValidIndicator = new StringValidIndicator();

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

		protected override void Initialize(bool initData)
		{
			if (null == _doc)
				throw new InvalidOperationException("Initialize called without setting the document beforehand");

			if (initData)
			{
				_entireTableQueryController = new EntireTableQueryController();
				_queryDesignerController = new QueryDesignerController();
				_arbitrarySqlQueryController = new ArbitrarySqlQueryController();
				_connectionStringList = new SelectableListNodeList(_staticConnectionStringList);

				var connectionString = _doc.ConnectionString;
				bool isConnectionStringValid = IsConnectionStringValid(ref connectionString);
				_connectionStringValidIndicator = new StringValidIndicator(connectionString, isConnectionStringValid);

				_tabItemList = new SelectableListNodeList();

				_currentlySelectedController = _arbitrarySqlQueryController;

				// Decide which tab to show - if entireTableName at the end is not null, then show the entire table tab
				string entireTableName = null;
				if (!string.IsNullOrEmpty(SelectionStatement))
				{
					var tokens = SelectionStatement.Split(new char[] { '\r', '\n', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
					if (tokens.Length == 4 && tokens[0].ToUpperInvariant() == "SELECT" && tokens[1].ToUpperInvariant() == "*" && tokens[2].ToUpperInvariant() == "FROM")
					{
						entireTableName = tokens[3];
						_currentlySelectedController = _entireTableQueryController;
					}
				}
				else
				{
					_currentlySelectedController = _entireTableQueryController;
				}

				_entireTableQueryController.TableName = entireTableName;
				if (isConnectionStringValid) _entireTableQueryController.ConnectionString = connectionString;
				_tabItemList.Add(new SelectableListNode("Single table", _entireTableQueryController, object.ReferenceEquals(_entireTableQueryController, _currentlySelectedController)));

				// Query designer controller
				if (isConnectionStringValid) _queryDesignerController.ConnectionString = connectionString;
				_tabItemList.Add(new SelectableListNode("Query builder", _queryDesignerController, object.ReferenceEquals(_queryDesignerController, _currentlySelectedController)));

				// Arbitrary SQL controller
				if (isConnectionStringValid) _arbitrarySqlQueryController.ConnectionString = connectionString;
				_arbitrarySqlQueryController.SelectionStatement = SelectionStatement;
				_tabItemList.Add(new SelectableListNode("Arbitrary Sql statement", _arbitrarySqlQueryController, object.ReferenceEquals(_arbitrarySqlQueryController, _currentlySelectedController)));

				ConnectionString = connectionString;
			}
			if (null != _view)
			{
				_view.SetConnectionListSource(_connectionStringList, ConnectionString.OriginalConnectionString);
				_view.SetConnectionStatus(_connectionStringValidIndicator.IsConnectionStringValid);

				if (null == _entireTableQueryController.ViewObject)
					Current.Gui.FindAndAttachControlTo(_entireTableQueryController);

				if (null == _queryDesignerController.ViewObject)
					Current.Gui.FindAndAttachControlTo(_queryDesignerController);

				if (null == _arbitrarySqlQueryController.ViewObject)
					Current.Gui.FindAndAttachControlTo(_arbitrarySqlQueryController);

				_view.SetTabItemsSource(_tabItemList);
			}
		}

		public AltaxoOleDbConnectionString ConnectionString
		{
			get
			{
				return _doc.ConnectionString;
			}
			protected set
			{
				if (null == value || value.IsEmpty)
					return;

				var oldValue = _doc.ConnectionString;
				var oldValueValid = _connectionStringValidIndicator.IsConnectionStringValid;

				_doc = _doc.WithConnectionString(value);

				if (_doc.ConnectionString != _connectionStringValidIndicator.ConnectionString)
				{
					var connString = _doc.ConnectionString;
					bool isConnStringValid = IsConnectionStringValid(ref connString);
					_doc = _doc.WithConnectionString(connString);
					_connectionStringValidIndicator = new StringValidIndicator(connString, isConnStringValid);
				}

				if (oldValue != _doc.ConnectionString || oldValueValid != _connectionStringValidIndicator.IsConnectionStringValid)
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
				return _doc.SelectionStatement;
			}
			set
			{
				_doc = _doc.WithSelectionStatement(value);
			}
		}

		private bool IsConnectionStringValid(ref AltaxoOleDbConnectionString connString)
		{
			string error;

			if (null == connString || connString.IsEmpty)
				return false;

			if (null == (error = CanEstablishConnectionWithConnectionString(connString.ConnectionStringWithTemporaryCredentials)))
				return true;

			for (; ; )
			{
				if (!Current.Gui.YesNoMessageBox(
					string.Format(
					"Could not connect to data base using the following connection string:\r\n" +
					"{0}\r\n" +
					"Error message:\r\n" +
					"{1}\r\n" +
					"Do you want to try again with different credentials?", connString, error),
					"Database connection failed", true))
					return false;

				var credentials = new LoginCredentials(string.Empty, string.Empty);
				try
				{
					credentials = connString.GetCredentials();
				}
				catch (Exception)
				{
				}
				if (!Current.Gui.ShowDialog(ref credentials, "New credentials for database connection", false))
					return false;

				connString = new AltaxoOleDbConnectionString(connString.OriginalConnectionString, credentials);

				if (null == (error = CanEstablishConnectionWithConnectionString(connString.ConnectionStringWithTemporaryCredentials)))
					return true;
			}
		}

		private string CanEstablishConnectionWithConnectionString(string connString)
		{
			try
			{
				using (var conn = new System.Data.OleDb.OleDbConnection(connString))
				{
					conn.Open();
					conn.Close();
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
			return null;
		}

		private static int InsertConnectionStringAtBeginningOfList(SelectableListNodeList list, AltaxoOleDbConnectionString connString)
		{
			// look for item in the list
			var index = list.IndexOfFirst(x => connString.OriginalConnectionString == ((AltaxoOleDbConnectionString)x.Tag).OriginalConnectionString);

			// add good values to the list
			if (index < 0) // was not before in the list
			{
				list.ClearSelectionsAll();
				list.Insert(0, new SelectableListNode(connString.OriginalConnectionString, connString, true));
				index = 0; // inserted at index 0
			}
			else if (index > 0) // was in the list before
			{
				list.ClearSelectionsAll();
				list.RemoveAt(index);
				list.Insert(0, new SelectableListNode(connString.OriginalConnectionString, connString, true));
				index = 0;
			}

			// trim list
			while (list.Count > _cmbConnStringMaxDropDownItems)
			{
				list.RemoveAt(list.Count - 1);
			}

			return index;
		}

		private static void RemoveConnectionStringFromList(SelectableListNodeList list, AltaxoOleDbConnectionString connString)
		{
			// look for item in the list
			var index = list.IndexOfFirst(x => connString.OriginalConnectionString == ((AltaxoOleDbConnectionString)x.Tag).OriginalConnectionString);
			if (index >= 0)
				list.RemoveAt(index);
		}

		/// <summary>
		/// Called whenever the connection string has changed.
		/// </summary>
		private void OnConnectionStringChanged()
		{
			int index;

			// handle good connection strings
			if (_connectionStringValidIndicator.IsConnectionStringValid)
			{
				index = InsertConnectionStringAtBeginningOfList(_connectionStringList, ConnectionString);
			}
			else  // _schema is null, thus handle bad connection strings
			{
				index = -1;
				RemoveConnectionStringFromList(_connectionStringList, ConnectionString);
			}

			var controllerConnectionString = _connectionStringValidIndicator.IsConnectionStringValid ? ConnectionString : AltaxoOleDbConnectionString.Empty;
			_entireTableQueryController.ConnectionString = controllerConnectionString;
			_queryDesignerController.ConnectionString = controllerConnectionString;
			_arbitrarySqlQueryController.ConnectionString = controllerConnectionString;

			if (null != _view)
			{
				_view.SetConnectionListSource(_connectionStringList, ConnectionString.OriginalConnectionString);
				_view.SetConnectionStatus(_connectionStringValidIndicator.IsConnectionStringValid);
			}
		}

		protected override void AttachView()
		{
			_view.SelectedTabChanged += EhSelectedTabChanged;
			_view.CmdChooseConnectionStringFromDialog += EhChooseConnection;
			_view.ConnectionStringSelectedFromList += EhConnectionStringSelectedFromList;
			_view.ConnectionStringChangedByUser += EhConnectionStringChangedByUser;
		}

		protected override void DetachView()
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
			AltaxoOleDbConnectionString axoConnString = null != connectionChoice ? (AltaxoOleDbConnectionString)connectionChoice.Tag : AltaxoOleDbConnectionString.Empty;

			var connString = axoConnString.OriginalConnectionString;
			if (string.IsNullOrEmpty(connString) || connString.IndexOf("provider=", StringComparison.OrdinalIgnoreCase) < 0)
			{
				connString = "Provider=SQLNCLI11.1;";
			}

			// let user change it
			var newConnString = OleDbConnString.EditConnectionString(connString);

			if (string.IsNullOrEmpty(newConnString))
				return;

			ConnectionString = new AltaxoOleDbConnectionString(newConnString, null);
		}

		private void EhConnectionStringSelectedFromList()
		{
			var node = _connectionStringList.FirstSelectedNode;
			if (node != null)
				ConnectionString = (AltaxoOleDbConnectionString)node.Tag;
		}

		private void EhConnectionStringChangedByUser(string newConnectionString)
		{
			ConnectionString = new AltaxoOleDbConnectionString(newConnectionString, null);
		}

		// issue a warning
		private void Warning(string format, params object[] args)
		{
			string msg = string.Format(format, args);
			Current.Gui.ErrorMessageBox(msg);
		}

		public override bool Apply()
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
					SelectionStatement = _entireTableQueryController.SelectionStatement;
				else if (_currentlySelectedController == _queryDesignerController)
					SelectionStatement = _queryDesignerController.SelectionStatement;
				else if (_currentlySelectedController == _arbitrarySqlQueryController)
					SelectionStatement = _arbitrarySqlQueryController.SelectionStatement;
			}
			else
			{
				return false;
			}

			if (_connectionStringValidIndicator.IsConnectionStringValid)
				InsertConnectionStringAtBeginningOfList(_staticConnectionStringList, ConnectionString);

			bool isValid = null != _doc.ConnectionString && !_doc.ConnectionString.IsEmpty && !string.IsNullOrEmpty(_doc.SelectionStatement);

			if (isValid && !object.ReferenceEquals(_doc, _originalDoc))
				CopyHelper.CopyImmutable(ref _originalDoc, _doc);

			return isValid;
		}
	}
}