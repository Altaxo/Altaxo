#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
	using Altaxo.Data;
	using Altaxo.Main;
	using Altaxo.Graph.Gdi;

	/// <summary>
	/// Holds all information that is neccessary to replace the tables used as data source for the plot items in graphs by other tables with the same structure.
	/// </summary>
	public class ExchangeTablesOfPlotItemsDocument
	{
		List<Altaxo.Graph.Gdi.Plot.IGPlotItem> _itemsToChange = new List<Altaxo.Graph.Gdi.Plot.IGPlotItem>();

		Dictionary<DocumentPath, DataTable> _tablesToChange = new Dictionary<DocumentPath, DataTable>();


		/// <summary>Returns a list with the plot items for which to change the underlying tables.</summary>
		public List<Altaxo.Graph.Gdi.Plot.IGPlotItem> PlotItemsToChange
		{
			get { return _itemsToChange; }
		}

		/// <summary>Gets or sets dictionary with the tables to change and their replacement tables.</summary>
		/// <value>The replacement dictionary. Key is the <see cref="DocumentPath"/> to the old table (thus non-existing tables can also be keys!). The values are the replacement tables.</value>
		public Dictionary<DocumentPath, DataTable> TablesToChange
		{
			get { return _tablesToChange; }
			set { _tablesToChange = value; }
		}


		/// <summary>Creates an instance of this class from a single <see cref="Altaxo.Graph.Gdi.GraphDocument"/>.</summary>
		/// <param name="doc">The graph document.</param>
		/// <returns>An instance of this class. The graph document is analyzed and all underlying tables of the plot items of the graph document are collected.</returns>
		public static ExchangeTablesOfPlotItemsDocument CreateFromGraph(Altaxo.Graph.Gdi.GraphDocument doc)
		{
			var result = new ExchangeTablesOfPlotItemsDocument();
			doc.VisitDocumentReferences(result.CollectDataTableFromProxyVisit);
			result.CollectPlotItemsForGraph(doc);
			return result;
		}


		/// <summary>Creates an instance of this class from multiple <see cref="Altaxo.Graph.Gdi.GraphDocument"/>s.</summary>
		/// <param name="docs">The graph documents.</param>
		/// <returns>An instance of this class. The graph documents are analyzed and all underlying tables of the plot items of all graph documents are collected.</returns>
		public static ExchangeTablesOfPlotItemsDocument CreateFromGraphs(IEnumerable<Altaxo.Graph.Gdi.GraphDocument> docs)
		{
			var result = new ExchangeTablesOfPlotItemsDocument();
			foreach (var doc in docs)
			{
				doc.VisitDocumentReferences(result.CollectDataTableFromProxyVisit);
				result.CollectPlotItemsForGraph(doc);
			}


			return result;
		}

		/// <summary>Collects all plot items for a single <see cref="Altaxo.Graph.Gdi.GraphDocument"/>.</summary>
		/// <param name="doc">The graph document to collect the plot items from..</param>
		void CollectPlotItemsForGraph(Altaxo.Graph.Gdi.GraphDocument doc)
		{
			foreach (var layer in doc.RootLayer.TakeFromHereToFirstLeaves().OfType<XYPlotLayer>())
			{
				var pis = layer.PlotItems.Flattened;
				foreach (var pi in pis)
				{
					if (!_itemsToChange.Contains(pi))
						_itemsToChange.Add(pi);
				}
			}
		}


		/// <summary>Collects a underlying data table from a proxy.</summary>
		/// <param name="proxy">The proxy.</param>
		/// <param name="owner">The owner of the proxy.</param>
		/// <param name="propertyName">Name of the property in the owner class that will return the proxy.</param>
		void CollectDataTableFromProxyVisit(DocNodeProxy proxy, object owner, string propertyName)
		{
			if (proxy.IsEmpty)
			{
			}
			else if (proxy.DocumentObject is Altaxo.Data.DataColumn)
			{
				var table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)proxy.DocumentObject);
				if (table != null)
				{
					var tablePath = DocumentPath.GetAbsolutePath(table);
					if (!_tablesToChange.ContainsKey(tablePath))
						_tablesToChange.Add(tablePath, null);
				}
			}
			else if (proxy.DocumentObject is Altaxo.Data.DataColumnCollection)
			{
				var table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumnCollection)proxy.DocumentObject);
				if (table != null)
				{
					var tablePath = DocumentPath.GetAbsolutePath(table);
					if (!_tablesToChange.ContainsKey(tablePath))
						_tablesToChange.Add(tablePath, null);
				}
			}
			else if (proxy.DocumentObject is DataTable)
			{
				var table = proxy.DocumentObject as DataTable;
				if (table != null)
				{
					var tablePath = DocumentPath.GetAbsolutePath(table);
					if (!_tablesToChange.ContainsKey(tablePath))
						_tablesToChange.Add(tablePath, null);
				}
			}
			else if ((proxy is Altaxo.Data.NumericColumnProxy) || (proxy is Altaxo.Data.ReadableColumnProxy))
			{
				var path = proxy.DocumentPath;
				if (path.Count >= 2 && path.StartsWith(DocumentPath.GetPath(Current.Project.DataTableCollection, int.MaxValue)))
				{
					var tablePath = path.SubPath(2);
					if (!_tablesToChange.ContainsKey(tablePath))
						_tablesToChange.Add(tablePath, null);
				}
			}

		}


		/// <summary>Applies the table exchanges, i.e. exchanges in all collected references to data tables and data columns the old table by the replacement table.</summary>
		public void ApplyTableExchanges()
		{
			foreach (var pi in _itemsToChange)
			{
				pi.VisitDocumentReferences(ExchangeTablesProxyVisit);
			}
		}

		/// <summary>Exchanges the tables during a proxy visit.</summary>
		/// <param name="proxy">The proxy which contain a reference to another project item.</param>
		/// <param name="owner">The owner instance of the proxy.</param>
		/// <param name="propertyName">Name of the property in the owner instance that returns the proxy.</param>
		void ExchangeTablesProxyVisit(DocNodeProxy proxy, object owner, string propertyName)
		{
			Altaxo.Data.DataTable substituteTable;

			if (proxy.IsEmpty)
			{
			}
			else if (proxy.DocumentObject is Altaxo.Data.DataColumn)
			{
				var table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)proxy.DocumentObject);
				if (table != null)
				{
					var tablePath = DocumentPath.GetAbsolutePath(table);
					if (_tablesToChange.TryGetValue(tablePath, out substituteTable) && null != substituteTable)
					{

						proxy.ReplacePathParts(tablePath, DocumentPath.GetAbsolutePath(substituteTable));

					}
				}
			}
			else if (proxy.DocumentObject is Altaxo.Data.DataColumnCollection)
			{
				throw new NotImplementedException();
			}
			else if (proxy.DocumentObject is DataTable)
			{
				throw new NotImplementedException();
			}
			else if ((proxy is Altaxo.Data.NumericColumnProxy) || (proxy is Altaxo.Data.ReadableColumnProxy))
			{
				var path = proxy.DocumentPath;
				var tableCollectionPath = DocumentPath.GetAbsolutePath(Current.Project.DataTableCollection);
				if (path.Count >= 2 && path[0] == tableCollectionPath[0])
				{
					var tablePath = path.SubPath(2);
					if (_tablesToChange.TryGetValue(tablePath, out substituteTable) && null != substituteTable)
					{
						proxy.ReplacePathParts(tablePath, DocumentPath.GetAbsolutePath(substituteTable));
					}
				}
			}

		}

	}

	/// <summary>
	/// Interface to the Gui view that shows the dialog in which the user can exchange the underlying tables of plot items.
	/// </summary>
	public interface IExchangeTablesOfPlotItemsView
	{
		/// <summary>Initializes the exchange table list.</summary>
		/// <param name="list">The list. First colum is 'old table' and should be bound to the 'Text' property. Second column is the new table name and should be bound to 'Text1' property. Third column is the status and should be bound to 'Text2' property.</param>
		void InitializeExchangeTableList(SelectableListNodeList list);

		/// <summary>Initializes the list of common substrings.</summary>
		/// <param name="list">The list. There is only one column which should be bound to the <see cref="P:Altaxo.Collections.SelectableListNode.Text"/> property.</param>
		void InitializeListOfCommonSubstrings(Collections.SelectableListNodeList list);

		/// <summary>Initializes the list of replacement candidates.</summary>
		/// <param name="list">The list. There is only one column which should be bound to the <see cref="P:Altaxo.Collections.SelectableListNode.Text"/> property.</param>
		void InitializeListOfReplacementCandidates(Collections.SelectableListNodeList list);

		/// <summary>Gets or sets the common substring text. The user should be able to edit this text in a text box.</summary>
		/// <value>The common substring text.</value>
		string CommonSubstringText { get; set; }

		/// <summary>Normally, the common substring search is done in enties of subfolders. If the option is <c>true</c>, the search is done characterwise instead.</summary>
		/// <value>The value is <c>true</c> if the search should be carried out characterwise. If the value is <c>false</c>, the search is carried out in entities of subfolders instead.<c>false</c>.</value>
		bool SearchCommonSubstringsCharacterWise { get; set; }

		/// <summary>Gets a value indicating whether the common substring panel is visible. Only if it is visible, all the operations will be carried out concerning common substrings.</summary>
		/// <value>The value is <see langword="true"/> if this common substring panel is visible; otherwise, <see langword="false"/>.</value>
		bool IsCommonSubstringPanelVisible { get; }

		/// <summary>Fired when the user wants to choose another table for the selected tables.</summary>
		event Action ChooseTableForSelectedItems;
		/// <summary>Fired when the user wants to change the folder of the selected tables.</summary>
		event Action ChooseFolderForSelectedItems;
		/// <summary>Fired when the selection in the table list has changed.</summary>
		event Action TableSelectionChanged;
		/// <summary>Fired when the selection in the list of common substrings has changed.</summary>
		event Action ListOfCommonSubstringsSelectionChanged;
		/// <summary>Fired when the selection in the list of substring replacement candidates has changed.</summary>
		event Action ListOfSubstringReplacementCandidatesSelectionChanged;
		/// <summary>Fired when the choice, whether to search character-wise or subfolder-wise for the longest common substring, has changed.</summary>
		event Action SearchCommonSubstringsCharacterWiseChanged;
		/// <summary>Fired when the user wants to make the replacement of the common substring by the replacement candidate permanent.</summary>
		event Action ApplySubstringReplacement;
		/// <summary>Fired when the user changed the common substring text.</summary>
		event Action CommonSubstringTextChanged;
		/// <summary>Fired when the visibility of the substring replacement panel changed.</summary>
		event Action CommonSubstringPanelVisibilityChanged;
	}

	/// <summary>
	/// Controls the exchange of the underlying tables of plotitems by other, user selectable tables.
	/// </summary>
	[UserControllerForObject(typeof(ExchangeTablesOfPlotItemsDocument))]
	[ExpectedTypeOfView(typeof(IExchangeTablesOfPlotItemsView))]
	public class ExchangeTablesOfPlotItemsController : IMVCANController
	{
		#region Inner classes

		class MyXTableListNode : SelectableListNode
		{
			DataTable _newTable;
			string _previewTableName;

			public MyXTableListNode(string text, object tag, bool isSelected)
				: base(text, tag, isSelected)
			{
			}

			public DataTable NewTable
			{
				get { return _newTable; }
				set
				{
					var oldValue = _newTable;
					_newTable = value;
					if (!object.ReferenceEquals(_newTable, oldValue))
					{
						OnPropertyChanged("Text1");
					}
				}
			}

			public string PreviewTableName
			{
				get
				{
					return _previewTableName;
				}
				set
				{
					var oldValue = _previewTableName;
					_previewTableName = value;
					if (oldValue != value)
					{
						OnPropertyChanged("Text1");
						OnPropertyChanged("Text2");
					}
				}
			}

			public override string Text1
			{
				get
				{
					if (null != _previewTableName)
						return _previewTableName;
					else if (null != _newTable)
						return _newTable.Name;
					else
						return "";
				}
			}

			public override string Text2
			{
				get
				{
					if (null != _previewTableName)
					{
						if (Current.Project.DataTableCollection.Contains(_previewTableName))
							return "preview: OK";
						else
							return "preview: missing!";

					}
					else if (null != _newTable)
					{
						return "changed";
					}
					else
					{
						return "no change";
					}
				}
			}
		}



		#endregion

		IExchangeTablesOfPlotItemsView _view;
		ExchangeTablesOfPlotItemsDocument _doc;

		/// <summary>List of the tables that are referenced by the plots, the plot items etc.</summary>
		SelectableListNodeList _tableList;

		/// <summary>The common substring that is displayed in the text box, thus the user can modify it.</summary>
		string _userModifiedCommonSubstring;
		/// <summary>List of common substrings of the currently selected items in <see cref="_tableList"/>.</summary>
		SelectableListNodeList _commonSubstringsList;

		/// <summary>List of possible replacement candidates for the currently selected common substring in <see cref="_commonSubstringsList"/>.</summary>
		SelectableListNodeList _substringReplacementCandidatesList;

		/// <summary>List with the names of all tables in the current project.</summary>
		string[] _namesOfAllTables;

		List<string> _listOfSelectedTableNames = new List<string>();


		/// <summary>Initialize the controller with the document. If successfull, the function has to return true.</summary>
		/// <param name="args">The arguments neccessary to create the controller. Normally, the first argument is the document, the second can be the parent of the document and so on.</param>
		/// <returns>Returns <see langword="true"/> if successfull; otherwise <see langword="false"/>.</returns>
		public bool InitializeDocument(params object[] args)
		{
			if (null == args || 0 == args.Length || !(args[0] is ExchangeTablesOfPlotItemsDocument))
				return false;

			_doc = args[0] as ExchangeTablesOfPlotItemsDocument;
			Initialize(true);
			return true;
		}

		void Initialize(bool initData)
		{
			if (initData)
			{
				_tableList = new SelectableListNodeList();
				foreach (var tablePath in _doc.TablesToChange.Keys)
				{
					_tableList.Add(new MyXTableListNode(tablePath[tablePath.Count-1], tablePath, false));
				}

				_listOfSelectedTableNames = new List<string>();
				_substringReplacementCandidatesList = new SelectableListNodeList();
				_commonSubstringsList = new SelectableListNodeList();
			}
			if (null != _view)
			{
				_view.InitializeExchangeTableList(_tableList);
			}
		}

		#region User action events

		/// <summary>Called when the user wants to select a single table as replacement for the selected tables.</summary>
		void EhChooseTableForSelectedItems()
		{
			var tableSel = new Altaxo.Gui.Worksheet.SingleTableChoice();

			if (Current.Gui.ShowDialog(ref tableSel, "Choose new table for selected items", false))
			{
				var newTable = tableSel.SelectedTable;
				foreach (var entry in _tableList)
				{
					if (entry.IsSelected)
					{
						((MyXTableListNode)entry).NewTable = newTable;
					}
				}
			}
		}

		/// <summary>Occurs when the user wants to select a new folder name that replaces the old folder name of the selected tables.</summary>
		void EhChooseFolderForSelectedItems()
		{
			var selection = new Altaxo.Gui.Main.SingleFolderChoice();

			if (Current.Gui.ShowDialog(ref selection, "Choose new folder which contains similar tables", false))
			{
				var newFolder = selection.SelectedFolder;
				foreach (var entry in _tableList.Where(x => x.IsSelected))
				{
					var tableToChange = (DocumentPath)entry.Tag;
					// try to find a table of the same short name in the selected folder
					if (tableToChange.Count > 0)
					{
						string newTableName = tableToChange[tableToChange.Count - 1];
						string shortName = Altaxo.Main.ProjectFolder.GetNamePart(newTableName);
						newTableName = newFolder.Name + shortName;

						if (Current.Project.DataTableCollection.Contains(newTableName))
							((MyXTableListNode)entry).NewTable = Current.Project.DataTableCollection[newTableName];
					}
				}
			}
		}

		/// <summary>Occurs when the user wants to make the preview to the common substring replacement permanent.</summary>
		void EhApplySubstringReplacement()
		{
			foreach (MyXTableListNode node in _tableList)
			{
				if (node.PreviewTableName != null && Current.Project.DataTableCollection.Contains(node.PreviewTableName))
				{
					node.NewTable = Current.Project.DataTableCollection[node.PreviewTableName];
					node.PreviewTableName = null;
				}
			}

			_tableList.ClearSelectionsAll();
			_view.InitializeExchangeTableList(_tableList);
		}
		#endregion

		#region Other events

		/// <summary>Called when the visibility of the substring replacement panel changed.</summary>
		void EhCommonSubstringPanelVisibilityChanged()
		{
			if (_view.IsCommonSubstringPanelVisible)
			{
				EhTableSelectionChanged();
			}
			else
			{
				InvalidateAllCommonSubstringData();
			}
		}

		/// <summary>Called when the choice, whether to search character-wise or subfolder-wise for the longest common substring, has changed.</summary>
		void EhSearchCommonSubstringsCharacterWiseChanged()
		{
			EhTableSelectionChanged();
		}


		/// <summary>Called when the selection in the table list has changed.</summary>
		void EhTableSelectionChanged()
		{
			if (_view.IsCommonSubstringPanelVisible)
			{
				// invalidate all that corresponds to the common substring business
				InvalidateAllCommonSubstringData();

				UpdateListOfSelectedTableNames();
				if (_listOfSelectedTableNames.Count >= 2)
				{
					// we can found common substring if there are at least two items selected
					if (_view.SearchCommonSubstringsCharacterWise)
						UpdateListOfCommonSubstringsCharacterWise();
					else
						UpdateListOfCommonSubstringsSubfolderWise();

					_view.InitializeListOfCommonSubstrings(_commonSubstringsList);
					if (null != _commonSubstringsList.FirstSelectedNode)
					{
						_userModifiedCommonSubstring = _commonSubstringsList.FirstSelectedNode.Text;
						_view.CommonSubstringText = _userModifiedCommonSubstring;
					}

					UpdateListOfSubstringReplacementCandidates(_userModifiedCommonSubstring);
					_view.InitializeListOfReplacementCandidates(_substringReplacementCandidatesList);

				}
			}
		}

		/// <summary>Called when the selection in the list of common substrings has changed.</summary>
		void EhListOfCommonSubstringsSelectionChanged()
		{
			if (null != _commonSubstringsList.FirstSelectedNode)
			{
				_userModifiedCommonSubstring = _commonSubstringsList.FirstSelectedNode.Text;
				_view.CommonSubstringText = _userModifiedCommonSubstring;
			}

			UpdateListOfSubstringReplacementCandidates(_userModifiedCommonSubstring);
			_view.InitializeListOfReplacementCandidates(_substringReplacementCandidatesList);
		}

		/// <summary>Called when the user changed the common substring text.</summary>
		void EhCommonSubstringTextChanged()
		{
			string substring = _view.CommonSubstringText;
			bool isModified = false;
			if (string.IsNullOrEmpty(substring))
			{
				if (null != _commonSubstringsList.FirstSelectedNode)
				{
					_userModifiedCommonSubstring = _commonSubstringsList.FirstSelectedNode.Text;
					_view.CommonSubstringText = _userModifiedCommonSubstring;
					isModified = true;
				}
			}
			else if (substring != _userModifiedCommonSubstring)
			{
				_userModifiedCommonSubstring = substring;
				isModified = true;
			}

			if (isModified)
			{
				UpdateListOfSubstringReplacementCandidates(_userModifiedCommonSubstring);
				_view.InitializeListOfReplacementCandidates(_substringReplacementCandidatesList);
			}
		}


		/// <summary>If the selection in the list of substring replacement candiates changed, then show a preview of the new table names in the table list.</summary>
		void EhListOfSubstringReplacementCandidatesSelectionChanged()
		{
			var commonSubstringNode = _commonSubstringsList.FirstSelectedNode;
			if (null == commonSubstringNode || string.IsNullOrEmpty(_userModifiedCommonSubstring))
				return;
			var commonSubstring = (IEnumerable<Collections.Text.SubstringPosition>)(commonSubstringNode.Tag);
			string fullCommonSubstring = commonSubstring.First().GetCommonSubstring(_listOfSelectedTableNames);

			int startOffset; // position of the maybeShortenedSubstring into the currently selected common substring
			int substringLength;
			if (0 <= fullCommonSubstring.IndexOf(_userModifiedCommonSubstring))
			{
				startOffset = fullCommonSubstring.IndexOf(_userModifiedCommonSubstring);
				substringLength = _userModifiedCommonSubstring.Length;
			}
			else // maybeshortendstring is not part of the selected common substring
				return;

			string replacementString = _substringReplacementCandidatesList.FirstSelectedNode == null ? null : _substringReplacementCandidatesList.FirstSelectedNode.Tag as string;
			if (null == replacementString)
				return;

			var tableNodesSelected = new List<SelectableListNode>(_tableList.Where(x => x.IsSelected));

			foreach (var poss in commonSubstring)
			{
				var selectedTableName = _listOfSelectedTableNames[poss.WordIndex];
				var first = selectedTableName.Substring(0, poss.Start + startOffset);
				int lastStart = poss.Start + startOffset + substringLength;
				var last = selectedTableName.Substring(lastStart, selectedTableName.Length - lastStart);
				var newName = first + replacementString + last;
				((MyXTableListNode)tableNodesSelected[poss.WordIndex]).PreviewTableName = newName;
			}
		}

		#endregion

		/// <summary>Invalidates all data concerning common substring search and replacement.</summary>
		void InvalidateAllCommonSubstringData()
		{
			_userModifiedCommonSubstring = null;
			_listOfSelectedTableNames.Clear();
			_commonSubstringsList.Clear();
			_substringReplacementCandidatesList.Clear();

			foreach (MyXTableListNode node in _tableList)
				node.PreviewTableName = null;
		}

		/// <summary>Updates the (<see cref="_listOfSelectedTableNames">list of selected table names</see>).</summary>
		private void UpdateListOfSelectedTableNames()
		{
			_listOfSelectedTableNames.Clear();
			foreach (MyXTableListNode node in _tableList)
			{
				if (node.IsSelected)
				{
					var path = (DocumentPath)node.Tag;
					var name = path[path.Count - 1];
					_listOfSelectedTableNames.Add(name);
				}
			}
		}

		/// <summary>Evaluates substrings that are common to all (!) of the <see cref="_listOfSelectedTableNames"/> and updates the <see cref="_commonSubstringsList">list of common substrings</see> with the result.</summary>
		private void UpdateListOfCommonSubstringsCharacterWise()
		{
			var gsa = Altaxo.Collections.Text.GeneralizedSuffixArray.FromSeparateWords(_listOfSelectedTableNames, true);
			var lcs = new Altaxo.Collections.Text.LongestCommonSubstringA(gsa) { StoreVerboseResults = true };
			lcs.Evaluate();
			if (lcs.MaximumNumberOfWordsWithCommonSubstring == _listOfSelectedTableNames.Count)
			{

				_commonSubstringsList = new SelectableListNodeList();
				foreach (var pos in lcs.GetSubstringPositionsCommonToTheNumberOfWords(_listOfSelectedTableNames.Count))
				{
					var first = pos.FirstPosition;
					var commonString = _listOfSelectedTableNames[first.WordIndex].Substring(first.Start, first.Count);
					_commonSubstringsList.Add(new SelectableListNode(commonString, pos, false));
				}
				_commonSubstringsList[0].IsSelected = true;
			}
		}


		/// <summary>Splits the full name into parts (the parts are the subfolders, and if present, the short name of the item).</summary>
		/// <param name="fullName">The full name of the item.</param>
		/// <returns>The parts, i.e. all subfolders (every subfolder ends with a DirectorySeparatorChar), and if present, the last part is the item's short name.</returns>
		private string[] SplitNameIntoParts(string fullName)
		{
			var arr = fullName.Split(new char[] { ProjectFolder.DirectorySeparatorChar }, StringSplitOptions.None);
			for (int i = arr.Length - 2; i >= 0; --i)
				arr[i] += ProjectFolder.DirectorySeparatorChar;
			return arr;
		}

		/// <summary>Joins the name of the parts back to the full name. This is the inverse function of <see cref="SplitNameIntoParts"/>, with the exception that it is chooseable which parts to join.</summary>
		/// <param name="nameParts">The name parts.</param>
		/// <param name="start">Index of the first name part to join.</param>
		/// <param name="count">Number of name parts to join.</param>
		/// <returns></returns>
		private string JoinPartsToName(string[] nameParts, int start, int count)
		{
			return string.Join(string.Empty, nameParts, start, count);
		}

		/// <summary>Searches for common substrings in the selected table names. The character entity here is not a character from a string, but a name part, as created by <see cref="SplitNameIntoParts"/>.</summary>
		private void UpdateListOfCommonSubstringsSubfolderWise()
		{
			List<string[]> words = new List<string[]>();

			foreach (var tableName in _listOfSelectedTableNames)
			{
				var parts = SplitNameIntoParts(tableName);
				words.Add(parts);
			}
			var gsa = Altaxo.Collections.Text.GeneralizedSuffixArray.FromSeparateWords(words, true);
			var lcs = new Altaxo.Collections.Text.LongestCommonSubstringA(gsa) { StoreVerboseResults = true };
			lcs.Evaluate();
			if (lcs.MaximumNumberOfWordsWithCommonSubstring == _listOfSelectedTableNames.Count)
			{
				_commonSubstringsList = new SelectableListNodeList();
				foreach (var pos in lcs.GetSubstringPositionsCommonToTheNumberOfWords(_listOfSelectedTableNames.Count))
				{
					var first = pos.FirstPosition;
					var commonString = JoinPartsToName(words[first.WordIndex], first.Start, first.Count);
					var poss = new List<Altaxo.Collections.Text.SubstringPosition>();

					foreach (var entry in pos) // we have to convert our positions which ar subfolderWise into character-wise positions
					{
						var word = words[entry.WordIndex];
						var toStart = JoinPartsToName(word, 0, entry.Start);
						poss.Add(new Collections.Text.SubstringPosition(entry.WordIndex, toStart.Length, commonString.Length));
					}
					_commonSubstringsList.Add(new SelectableListNode(commonString, poss, false));
				}
				_commonSubstringsList[0].IsSelected = true;
			}
		}




		/// <summary>
		/// Updates the list of substring replacement candidates for a given substring position. For the substring that is currently selected in <see cref="_commonSubstringsList"/> it evaluates
		/// the possible replacement candidates for this substrings and filles the list <see cref="_substringReplacementCandidatesList"/> with the result.
		/// </summary>
		/// <param name="maybeShortendedSubstring">The maybe shortended substring.</param>
		private void UpdateListOfSubstringReplacementCandidates(string maybeShortendedSubstring)
		{
			_substringReplacementCandidatesList.Clear();
			var commonSubstringNode = _commonSubstringsList.FirstSelectedNode;
			if (string.IsNullOrEmpty(maybeShortendedSubstring))
				return;

			// now find all substrings, that, when replacing the common substring of the selected tables, will give new valid table names for all (!) selected tables
			if (null == _namesOfAllTables)
				_namesOfAllTables = Current.Project.DataTableCollection.GetSortedTableNames();
			var commonSubstringPositions = (IEnumerable<Collections.Text.SubstringPosition>)(commonSubstringNode.Tag);
			string fullCommonSubstring = commonSubstringPositions.First().GetCommonSubstring(_listOfSelectedTableNames);

			int startOffset = 0; // position of the maybeShortenedSubstring into the currently selected common substring
			int substringLength = maybeShortendedSubstring.Length;
			if (0 <= fullCommonSubstring.IndexOf(maybeShortendedSubstring)) // maybeShortenedSubstring is part of the fullCommonSubstring that was found by the longest common substring algorithm
			{
				startOffset = fullCommonSubstring.IndexOf(maybeShortendedSubstring);
			}
			else // maybeshortendstring is not part of the selected common substring, so we try to find the actual positions in the selected table names
			{
				var positions = new List<Collections.Text.SubstringPosition>();
				commonSubstringPositions = positions;
				for (int wordIndex = 0; wordIndex < _listOfSelectedTableNames.Count; ++wordIndex)
				{
					var selectedTableName = _listOfSelectedTableNames[wordIndex];
					int idx = selectedTableName.IndexOf(maybeShortendedSubstring);
					if (idx < 0)
						return;
					positions.Add(new Collections.Text.SubstringPosition(wordIndex, idx, maybeShortendedSubstring.Length));
				}
			}

			HashSet<string> allReplacementStrings = null;
			foreach (var poss in commonSubstringPositions)
			{
				var selectedTableName = _listOfSelectedTableNames[poss.WordIndex];
				var first = selectedTableName.Substring(0, poss.Start + startOffset);
				int lastStart = poss.Start + startOffset + substringLength;
				var last = selectedTableName.Substring(lastStart, selectedTableName.Length - lastStart);
				var allReplacementStringsForThisTable = new HashSet<string>();
				foreach (var name in _namesOfAllTables)
				{
					int remainingLength = name.Length - first.Length - last.Length;
					if (name.StartsWith(first) && name.EndsWith(last) && remainingLength>=0)
						allReplacementStringsForThisTable.Add(name.Substring(first.Length, remainingLength));
				}
				if (allReplacementStrings == null)
				{
					allReplacementStrings = allReplacementStringsForThisTable;
					allReplacementStrings.ExceptWith(new string[] { maybeShortendedSubstring }); // the own substring we have found we could exclude, it makes no sense to replace it by its own
				}
				else
				{
					allReplacementStrings.IntersectWith(allReplacementStringsForThisTable);
				}

				// if allReplacementStrings is empty here, we can break, since we will not found then a replacement string 
				if (0 == allReplacementStrings.Count)
					break;

			}



			var larr = allReplacementStrings.ToArray();
			Array.Sort(larr);
			foreach (var s in larr)
				_substringReplacementCandidatesList.Add(new SelectableListNode(s, s, false));
			if (_substringReplacementCandidatesList.Count > 0)
				_substringReplacementCandidatesList[0].IsSelected = true;
		}


		#region IMVCANController

		/// <summary>
		/// Not used here.
		/// </summary>
		public UseDocument UseDocumentCopy
		{
			set { }
		}

		/// <summary>Returns the Gui element that shows the model to the user.</summary>
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
					_view.ChooseTableForSelectedItems -= this.EhChooseTableForSelectedItems;
					_view.ChooseFolderForSelectedItems -= this.EhChooseFolderForSelectedItems;
					_view.TableSelectionChanged -= this.EhTableSelectionChanged;
					_view.ListOfCommonSubstringsSelectionChanged -= EhListOfCommonSubstringsSelectionChanged;
					_view.ListOfSubstringReplacementCandidatesSelectionChanged -= EhListOfSubstringReplacementCandidatesSelectionChanged;
					_view.SearchCommonSubstringsCharacterWiseChanged -= EhSearchCommonSubstringsCharacterWiseChanged;
					_view.ApplySubstringReplacement -= EhApplySubstringReplacement;
					_view.CommonSubstringTextChanged -= EhCommonSubstringTextChanged;
					_view.CommonSubstringPanelVisibilityChanged -= EhCommonSubstringPanelVisibilityChanged;
				}

				_view = value as IExchangeTablesOfPlotItemsView;

				if (null != _view)
				{
					Initialize(false);

					_view.ChooseTableForSelectedItems += this.EhChooseTableForSelectedItems;
					_view.ChooseFolderForSelectedItems += this.EhChooseFolderForSelectedItems;
					_view.TableSelectionChanged += this.EhTableSelectionChanged;
					_view.ListOfCommonSubstringsSelectionChanged += EhListOfCommonSubstringsSelectionChanged;
					_view.ListOfSubstringReplacementCandidatesSelectionChanged += EhListOfSubstringReplacementCandidatesSelectionChanged;
					_view.SearchCommonSubstringsCharacterWiseChanged += EhSearchCommonSubstringsCharacterWiseChanged;
					_view.ApplySubstringReplacement += EhApplySubstringReplacement;
					_view.CommonSubstringTextChanged += EhCommonSubstringTextChanged;
					_view.CommonSubstringPanelVisibilityChanged += EhCommonSubstringPanelVisibilityChanged;
				}
			}
		}


		/// <summary>Returns the model (document) that this controller manages.</summary>
		public object ModelObject
		{
			get { return _doc; }
		}

		/// <summary>Called when the user input has to be applied to the document being controlled. Returns true if Apply is successfull.</summary>
		/// <returns>True if the apply was successfull, otherwise false.</returns>
		public bool Apply()
		{
			_doc.TablesToChange.Clear();
			foreach (MyXTableListNode entry in _tableList)
			{
				if (null != entry.NewTable)
				{
					var tableToChange = (DocumentPath)entry.Tag;
					_doc.TablesToChange[tableToChange] = entry.NewTable;
				}
			}
			_doc.ApplyTableExchanges();
			return true;
		}
		#endregion
	}
}
