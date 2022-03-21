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

#endregion Copyright

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
  using System.ComponentModel;
  using System.Windows.Input;
  using Altaxo.Data;
  using Altaxo.Graph.Gdi;
  using Altaxo.Gui.Common;
  using Altaxo.Main;

  /// <summary>
  /// Holds all information that is neccessary to replace the tables used as data source for the plot items in graphs by other tables with the same structure.
  /// </summary>
  public class ExchangeTablesOfPlotItemsDocument
  {
    private List<Altaxo.Graph.Gdi.Plot.IGPlotItem> _itemsToChange = new List<Altaxo.Graph.Gdi.Plot.IGPlotItem>();

    private Dictionary<AbsoluteDocumentPath, DataTable> _tablesToChange = new Dictionary<AbsoluteDocumentPath, DataTable>();

    /// <summary>Returns a list with the plot items for which to change the underlying tables.</summary>
    public List<Altaxo.Graph.Gdi.Plot.IGPlotItem> PlotItemsToChange
    {
      get { return _itemsToChange; }
    }

    /// <summary>Gets or sets dictionary with the tables to change and their replacement tables.</summary>
    /// <value>The replacement dictionary. Key is the <see cref="AbsoluteDocumentPath"/> to the old table (thus non-existing tables can also be keys!). The values are the replacement tables.</value>
    public Dictionary<AbsoluteDocumentPath, DataTable> TablesToChange
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
    private void CollectPlotItemsForGraph(Altaxo.Graph.Gdi.GraphDocument doc)
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
    private void CollectDataTableFromProxyVisit(IProxy proxy, object owner, string propertyName)
    {
      if (proxy.IsEmpty)
      {
      }
      else if (proxy.DocumentObject() is Altaxo.Data.DataColumn dataColumn)
      {
        var table = Altaxo.Data.DataTable.GetParentDataTableOf(dataColumn);
        if (table is not null)
        {
          var tablePath = AbsoluteDocumentPath.GetAbsolutePath(table);
          if (!_tablesToChange.ContainsKey(tablePath))
            _tablesToChange.Add(tablePath, null);
        }
      }
      else if (proxy.DocumentObject() is Altaxo.Data.DataColumnCollection dataColumnCollection)
      {
        var table = Altaxo.Data.DataTable.GetParentDataTableOf(dataColumnCollection);
        if (table is not null)
        {
          var tablePath = AbsoluteDocumentPath.GetAbsolutePath(table);
          if (!_tablesToChange.ContainsKey(tablePath))
            _tablesToChange.Add(tablePath, null);
        }
      }
      else if (proxy.DocumentObject() is DataTable table)
      {
        var tablePath = AbsoluteDocumentPath.GetAbsolutePath(table);
        if (!_tablesToChange.ContainsKey(tablePath))
          _tablesToChange.Add(tablePath, null);

      }
      else if ((proxy is Altaxo.Data.INumericColumnProxy) || (proxy is Altaxo.Data.IReadableColumnProxy))
      {
        var path = proxy.DocumentPath();
        if (path.Count >= 2 && path.StartsWith(AbsoluteDocumentPath.GetPath(Current.Project.DataTableCollection, int.MaxValue)))
        {
          var tablePath = path.SubPath(0, 2);
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
    private void ExchangeTablesProxyVisit(IProxy proxy, object owner, string propertyName)
    {
      Altaxo.Data.DataTable substituteTable;

      if (proxy.IsEmpty)
      {
      }
      else if (proxy.DocumentObject() is Altaxo.Data.DataColumn dataColumn)
      {
        var table = Altaxo.Data.DataTable.GetParentDataTableOf(dataColumn);
        if (table is not null)
        {
          var tablePath = AbsoluteDocumentPath.GetAbsolutePath(table);
          if (_tablesToChange.TryGetValue(tablePath, out substituteTable) && substituteTable is not null)
          {
            proxy.ReplacePathParts(tablePath, AbsoluteDocumentPath.GetAbsolutePath(substituteTable), (IDocumentLeafNode)owner);
          }
        }
      }
      else if (proxy.DocumentObject() is Altaxo.Data.DataColumnCollection)
      {
        throw new NotImplementedException();
      }
      else if (proxy.DocumentObject() is DataTable)
      {
        throw new NotImplementedException();
      }
      else if ((proxy is Altaxo.Data.INumericColumnProxy) || (proxy is Altaxo.Data.IReadableColumnProxy))
      {
        var path = proxy.DocumentPath();
        var tableCollectionPath = AbsoluteDocumentPath.GetAbsolutePath(Current.Project.DataTableCollection);
        if (path.Count >= 2 && path[0] == tableCollectionPath[0])
        {
          var tablePath = path.SubPath(0, 2);
          if (_tablesToChange.TryGetValue(tablePath, out substituteTable) && substituteTable is not null)
          {
            proxy.ReplacePathParts(tablePath, AbsoluteDocumentPath.GetAbsolutePath(substituteTable), (IDocumentLeafNode)owner);
          }
        }
      }
    }
  }

  /// <summary>
  /// Interface to the Gui view that shows the dialog in which the user can exchange the underlying tables of plot items.
  /// </summary>
  public interface IExchangeTablesOfPlotItemsView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controls the exchange of the underlying tables of plotitems by other, user selectable tables.
  /// </summary>
  [UserControllerForObject(typeof(ExchangeTablesOfPlotItemsDocument))]
  [ExpectedTypeOfView(typeof(IExchangeTablesOfPlotItemsView))]
  public class ExchangeTablesOfPlotItemsController : MVCANControllerEditImmutableDocBase<ExchangeTablesOfPlotItemsDocument, IExchangeTablesOfPlotItemsView>
  {
    #region Inner classes

    private class MyXTableListNode : SelectableListNodeWithParent
    {
      private DataTable _newTable;
      private string _previewTableName;

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
          if (_previewTableName is not null)
            return _previewTableName;
          else if (_newTable is not null)
            return _newTable.Name;
          else
            return "";
        }
      }

      public override string Text2
      {
        get
        {
          if (_previewTableName is not null)
          {
            if (Current.Project.DataTableCollection.Contains(_previewTableName))
              return "preview: OK";
            else
              return "preview: missing!";
          }
          else if (_newTable is not null)
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

    #endregion Inner classes



    /// <summary>The common substring that is displayed in the text box, thus the user can modify it.</summary>
    private string _userModifiedCommonSubstring;



    /// <summary>List with the names of all tables in the current project.</summary>
    private string[] _namesOfAllTables;

    private List<string> _listOfSelectedTableNames = new List<string>();

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public ExchangeTablesOfPlotItemsController()
    {
      CmdApplyReplacementForCommonSubstring = new RelayCommand(EhApplySubstringReplacement);
      CmdChooseFolder = new RelayCommand(EhChooseFolderForSelectedItems);
      CmdChooseTable = new RelayCommand(EhChooseTableForSelectedItems);
    }

    #region Bindings


    public ICommand CmdApplyReplacementForCommonSubstring { get; }

    public ICommand CmdChooseTable { get; }

    public ICommand CmdChooseFolder { get; }

    private MultipleSelectableListNodeList _tableList;

    /// <summary>List of the tables that are referenced by the plots, the plot items etc.
    /// First column is 'old table' and should be bound to the 'Text' property. Second column is the new table name and should be bound to 'Text1' property. Third column is the status and should be bound to 'Text2' property.
    /// </summary>
    public MultipleSelectableListNodeList TableList
    {
      get => _tableList;
      set
      {
        if (!(_tableList == value))
        {
          if (_tableList is { } oldC)
            oldC.SelectedItemsChanged -= EhTableSelectionChanged;

          _tableList = value;

          if (_tableList is { } newC)
            newC.SelectedItemsChanged += EhTableSelectionChanged;

          OnPropertyChanged(nameof(TableList));
        }
      }
    }


    private ItemsController<string> _listOfCommonSubstrings;

    /// <summary>Initializes the list of common substrings.
    /// The list. There is only one column which should be bound to the <see cref="P:Altaxo.Collections.SelectableListNode.Text"/> property.
    ///  List of common substrings of the currently selected items in <see cref="TableList"/>.
    ///</summary>
    public ItemsController<string> ListOfCommonSubstrings
    {
      get => _listOfCommonSubstrings;
      set
      {
        if (!(_listOfCommonSubstrings == value))
        {
          _listOfCommonSubstrings = value;
          OnPropertyChanged(nameof(ListOfCommonSubstrings));
        }
      }
    }

    private ItemsController<string> _listOfReplacementCandidates;

    /// <summary>List of possible replacement candidates for the currently selected common substring in <see cref="_commonSubstringsList"/>.</summary>
    public ItemsController<string> ListOfReplacementCandidates
    {
      get => _listOfReplacementCandidates;
      set
      {
        if (!(_listOfReplacementCandidates == value))
        {
          _listOfReplacementCandidates = value;
          OnPropertyChanged(nameof(ListOfReplacementCandidates));
        }
      }
    }


    private string _replacementCandidateText;

    public string ReplacementCandidateText
    {
      get => _replacementCandidateText;
      set
      {
        if (!(_replacementCandidateText == value))
        {
          _replacementCandidateText = value;
          OnPropertyChanged(nameof(ReplacementCandidateText));
        }
      }
    }

    private string _commonSubstringText;

    /// <summary>Gets or sets the common substring text. The user should be able to edit this text in a text box.</summary>
    public string CommonSubstringText
    {
      get => _commonSubstringText;
      set
      {
        if (!(_commonSubstringText == value))
        {
          _commonSubstringText = value;
          OnPropertyChanged(nameof(CommonSubstringText));
          EhCommonSubstringTextChanged();
        }
      }
    }

    private bool _isCommonSubstringOperationsVisible;

    public bool IsCommonSubstringOperationsVisible
    {
      get => _isCommonSubstringOperationsVisible;
      set
      {
        if (!(_isCommonSubstringOperationsVisible == value))
        {
          _isCommonSubstringOperationsVisible = value;
          OnPropertyChanged(nameof(IsCommonSubstringOperationsVisible));
          EhCommonSubstringPanelVisibilityChanged();
        }
      }
    }

    private bool _searchCommonSubstringCharacterwise;

    public bool SearchCommonSubstringCharacterwise
    {
      get => _searchCommonSubstringCharacterwise;
      set
      {
        if (!(_searchCommonSubstringCharacterwise == value))
        {
          _searchCommonSubstringCharacterwise = value;
          OnPropertyChanged(nameof(SearchCommonSubstringCharacterwise));
          EhSearchCommonSubstringsCharacterWiseChanged();
        }
      }
    }

    public bool SearchCommonSubstringSubfolderwise
    {
      get => !_searchCommonSubstringCharacterwise;
      set => SearchCommonSubstringCharacterwise = !value;
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var tableList = new MultipleSelectableListNodeList();
        foreach (var tablePath in _doc.TablesToChange.Keys)
        {
          tableList.Add(new MyXTableListNode(tablePath[tablePath.Count - 1], tablePath, false));
        }
        TableList = tableList;

        _listOfSelectedTableNames = new List<string>();
        ListOfReplacementCandidates = new ItemsController<string>(new SelectableListNodeList(), EhListOfSubstringReplacementCandidatesSelectionChanged);
         ListOfCommonSubstrings = new ItemsController<string>(new SelectableListNodeList(), EhListOfCommonSubstringsSelectionChanged);
      }
      
    }

    #region User action events

    /// <summary>Called when the user wants to select a single table as replacement for the selected tables.</summary>
    private void EhChooseTableForSelectedItems()
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
    private void EhChooseFolderForSelectedItems()
    {
      var selection = new Altaxo.Gui.Main.SingleFolderChoice();

      if (Current.Gui.ShowDialog(ref selection, "Choose new folder which contains similar tables", false))
      {
        var newFolder = selection.SelectedFolder;
        foreach (var entry in _tableList.Where(x => x.IsSelected))
        {
          var tableToChange = (AbsoluteDocumentPath)entry.Tag;
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
    private void EhApplySubstringReplacement()
    {
      foreach (MyXTableListNode node in _tableList)
      {
        if (node.PreviewTableName is not null && Current.Project.DataTableCollection.Contains(node.PreviewTableName))
        {
          node.NewTable = Current.Project.DataTableCollection[node.PreviewTableName];
          node.PreviewTableName = null;
        }
      }

      _tableList.ClearSelectionsAll();
    }

    #endregion User action events

    #region Other events

    /// <summary>Called when the visibility of the substring replacement panel changed.</summary>
    private void EhCommonSubstringPanelVisibilityChanged()
    {
      if (IsCommonSubstringOperationsVisible)
      {
        EhTableSelectionChanged();
      }
      else
      {
        InvalidateAllCommonSubstringData();
      }
    }

    /// <summary>Called when the choice, whether to search character-wise or subfolder-wise for the longest common substring, has changed.</summary>
    private void EhSearchCommonSubstringsCharacterWiseChanged()
    {
      EhTableSelectionChanged();
    }


    private void EhTableSelectionChanged(object sender, PropertyChangedEventArgs e)
    {
      if(e.PropertyName == nameof(MultipleSelectableListNodeList.SelectedItems))
      {
        EhTableSelectionChanged();
      }
    }
    /// <summary>Called when the selection in the table list has changed.</summary>
    private void EhTableSelectionChanged()
    {
      if (IsCommonSubstringOperationsVisible)
      {
        // invalidate all that corresponds to the common substring business
        InvalidateAllCommonSubstringData();

        UpdateListOfSelectedTableNames();
        if (_listOfSelectedTableNames.Count >= 2)
        {
          // we can found common substring if there are at least two items selected
          if (SearchCommonSubstringCharacterwise)
            UpdateListOfCommonSubstringsCharacterWise();
          else
            UpdateListOfCommonSubstringsSubfolderWise();

          if (ListOfCommonSubstrings.SelectedValue is not null)
          {
            _userModifiedCommonSubstring = ListOfCommonSubstrings.SelectedValue;
            CommonSubstringText = _userModifiedCommonSubstring;
          }

          UpdateListOfSubstringReplacementCandidates(_userModifiedCommonSubstring);
        }
      }
    }

    /// <summary>Called when the selection in the list of common substrings has changed.</summary>
    private void EhListOfCommonSubstringsSelectionChanged(string selectedValue)
    {
      if (ListOfCommonSubstrings.SelectedValue is not null)
      {
        _userModifiedCommonSubstring = ListOfCommonSubstrings.SelectedValue;
        CommonSubstringText = _userModifiedCommonSubstring;
      }

      UpdateListOfSubstringReplacementCandidates(_userModifiedCommonSubstring);
    }

    /// <summary>Called when the user changed the common substring text.</summary>
    private void EhCommonSubstringTextChanged()
    {
      string substring = CommonSubstringText;
      bool isModified = false;
      if (string.IsNullOrEmpty(substring))
      {
        if (ListOfCommonSubstrings.SelectedValue is not null)
        {
          _userModifiedCommonSubstring = ListOfCommonSubstrings.SelectedValue;
          CommonSubstringText = _userModifiedCommonSubstring;
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
      }
    }

    /// <summary>If the selection in the list of substring replacement candiates changed, then show a preview of the new table names in the table list.</summary>
    private void EhListOfSubstringReplacementCandidatesSelectionChanged(string selectedValue)
    {
      if(ListOfReplacementCandidates.SelectedItem is not null)
      {
        ReplacementCandidateText = ListOfReplacementCandidates.SelectedItem.ToString();
      }
      else
      {
        ReplacementCandidateText = null;
      }

      var commonSubstringNode = ListOfCommonSubstrings.SelectedItem;
      if (commonSubstringNode is null || string.IsNullOrEmpty(_userModifiedCommonSubstring))
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

      string replacementString = ListOfReplacementCandidates.SelectedValue;
      if (replacementString is null)
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

    #endregion Other events

    /// <summary>Invalidates all data concerning common substring search and replacement.</summary>
    private void InvalidateAllCommonSubstringData()
    {
      _userModifiedCommonSubstring = null;
      _listOfSelectedTableNames.Clear();
      ListOfCommonSubstrings.Items.Clear();
      ListOfReplacementCandidates.Items.Clear();

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
          var path = (AbsoluteDocumentPath)node.Tag;
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
        ListOfCommonSubstrings.Items.Clear();
        foreach (var pos in lcs.GetSubstringPositionsCommonToTheNumberOfWords(_listOfSelectedTableNames.Count))
        {
          var first = pos.FirstPosition;
          var commonString = _listOfSelectedTableNames[first.WordIndex].Substring(first.Start, first.Count);
          ListOfCommonSubstrings.Items.Add(new SelectableListNode(commonString, pos, false));
        }
        ListOfCommonSubstrings.SelectedItem = ListOfCommonSubstrings.Items[0];
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
      var words = new List<string[]>();

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
        ListOfCommonSubstrings.Items.Clear();
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
          ListOfCommonSubstrings.Items.Add(new SelectableListNode(commonString, poss, false));
        }
        ListOfCommonSubstrings.SelectedItem = ListOfCommonSubstrings.Items[0];
      }
    }

    /// <summary>
    /// Updates the list of substring replacement candidates for a given substring position. For the substring that is currently selected in <see cref="_commonSubstringsList"/> it evaluates
    /// the possible replacement candidates for this substrings and filles the list <see cref="_substringReplacementCandidatesList"/> with the result.
    /// </summary>
    /// <param name="maybeShortendedSubstring">The maybe shortended substring.</param>
    private void UpdateListOfSubstringReplacementCandidates(string maybeShortendedSubstring)
    {
      ListOfReplacementCandidates.Items.Clear();
      var commonSubstringNode = ListOfCommonSubstrings.SelectedItem;
      if (string.IsNullOrEmpty(maybeShortendedSubstring))
        return;

      // now find all substrings, that, when replacing the common substring of the selected tables, will give new valid table names for all (!) selected tables
      if (_namesOfAllTables is null)
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
          if (name.StartsWith(first) && name.EndsWith(last) && remainingLength >= 0)
            allReplacementStringsForThisTable.Add(name.Substring(first.Length, remainingLength));
        }
        if (allReplacementStrings is null)
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
        ListOfReplacementCandidates.Items.Add(new SelectableListNode(s, s, false));
      if (ListOfReplacementCandidates.Items.Count > 0)
        ListOfReplacementCandidates.SelectedItem = ListOfReplacementCandidates.Items[0];
    }

    /// <summary>Called when the user input has to be applied to the document being controlled. Returns true if Apply is successfull.</summary>
    /// <returns>True if the apply was successfull, otherwise false.</returns>
    public override bool Apply(bool disposeController)
    {
      _doc.TablesToChange.Clear();
      foreach (MyXTableListNode entry in _tableList)
      {
        if (entry.NewTable is not null)
        {
          var tableToChange = (AbsoluteDocumentPath)entry.Tag;
          _doc.TablesToChange[tableToChange] = entry.NewTable;
        }
      }
      _doc.ApplyTableExchanges();
      return ApplyEnd(true, disposeController);
    }
  }
}
