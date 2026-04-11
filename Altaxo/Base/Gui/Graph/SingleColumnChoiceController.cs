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
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Main;

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Provides the view contract for <see cref="SingleColumnChoiceController"/>.
  /// </summary>
  public interface ISingleColumnChoiceView : IDataContextAwareView
  {
  }

  #region SingleColumnChoice document

  /// <summary>
  /// Represents the document model for choosing a single column.
  /// </summary>
  public class SingleColumnChoice
  {
    /// <summary>
    /// Gets or sets the selected column.
    /// </summary>
    public DataColumn SelectedColumn;
    /// <summary>
    /// Gets or sets the environment object.
    /// </summary>
    public object Environment;
    /// <summary>
    /// Gets or sets a value indicating whether only numeric columns are allowed.
    /// </summary>
    public bool AllowOnlyNumericColumns;
  }

  #endregion SingleColumnChoice document


  /// <summary>
  /// Provides controller logic for choosing a single data column.
  /// </summary>
  [UserControllerForObject(typeof(SingleColumnChoice))]
  [ExpectedTypeOfView(typeof(ISingleColumnChoiceView))]
  public class SingleColumnChoiceController : MVCANControllerEditImmutableDocBase<SingleColumnChoice, ISingleColumnChoiceView>
  {
    #region My private nodes

    /// <summary>
    /// Represents a table node that can lazily expose its columns.
    /// </summary>
    internal class TableNode : NGTreeNode
    {
      private const int MaxNumberOfColumnsInOneNode = 100;
      private DataColumnCollection _collection;
      private int _firstColumn;
      private int _columnCount;

      /// <summary>
      /// Initializes a new instance of the <see cref="TableNode"/> class for a complete table.
      /// </summary>
      /// <param name="table">The source table.</param>
      public TableNode(DataTable table)
        : base(true)
      {
        _collection = table.DataColumns;
        _firstColumn = 0;
        _columnCount = table.DataColumns.ColumnCount;
        Text = table.ShortName;
        Tag = table;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="TableNode"/> class for a contiguous range of columns.
      /// </summary>
      /// <param name="coll">The source column collection.</param>
      /// <param name="firstColumn">The index of the first column in the range.</param>
      /// <param name="columnCount">The number of columns in the range.</param>
      public TableNode(DataColumnCollection coll, int firstColumn, int columnCount)
        : base(true)
      {
        _firstColumn = firstColumn;
        _columnCount = columnCount;
        _collection = coll;
        Text = string.Format("Cols {0}-{1}", firstColumn, firstColumn + columnCount - 1);
      }

      /// <summary>
      /// Gets or sets the underlying column collection.
      /// </summary>
      public DataColumnCollection Collection
      {
        get { return _collection; }
        set { _collection = value; }
      }

      /// <summary>
      /// Gets or sets the index of the first column represented by this node.
      /// </summary>
      public int FirstColumn
      {
        get { return _firstColumn; }
        set { _firstColumn = value; }
      }

      /// <summary>
      /// Gets or sets the number of columns represented by this node.
      /// </summary>
      public int ColumnCount
      {
        get { return _columnCount; }
        set { _columnCount = value; }
      }

      /// <inheritdoc />
      protected override void LoadChildren()
      {
        DataColumnCollection coll = _collection;
        Nodes.Clear();
        int nextColumn = Math.Min(_firstColumn + _columnCount, coll.ColumnCount);

        if (_columnCount <= MaxNumberOfColumnsInOneNode) // If number is low enough, expand to the data columns directly
        {
          for (int i = _firstColumn; i < nextColumn; ++i)
            Nodes.Add(new NGTreeNode() { Text = coll.GetColumnName(i), Tag = coll[i] });
        }
        else // if the number of data columns is too high to be directly shown, we create intermediate nodes
        {
          // calculate the number of nodes to be shown
          int numNodes = (int)Math.Ceiling(_columnCount / (double)MaxNumberOfColumnsInOneNode);
          numNodes = Math.Min(MaxNumberOfColumnsInOneNode, numNodes);
          int colsInOneNode = MaxNumberOfColumnsInOneNode;
          for (; colsInOneNode * numNodes < _columnCount; colsInOneNode *= MaxNumberOfColumnsInOneNode)
            ; // Multiply with a multiple of MaxNumberOfColumnsInOneNode until it fits

          int first = _firstColumn;
          int remaining = nextColumn - _firstColumn;
          for (int i = 0; i < numNodes && remaining > 0; ++i)
          {
            Nodes.Add(new TableNode(coll, first, Math.Min(remaining, colsInOneNode)));
            remaining -= colsInOneNode;
            first += colsInOneNode;
          }
        }
      }
    }

    #endregion My private nodes


    private DataColumn _selectedColumn = null;

    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleColumnChoiceController"/> class.
    /// </summary>
    /// <param name="doc">The document model.</param>
    public SingleColumnChoiceController(SingleColumnChoice doc)
    {
      CmdSelectedItemChanged = new RelayCommand(EhView_AfterSelectNode);
      _doc = doc;
      Initialize(true);
    }

    #region Bindings

    /// <summary>
    /// Gets the command that is executed when the selected item changes.
    /// </summary>
    public ICommand CmdSelectedItemChanged { get; }

    private NGTreeNode _rootNode = new NGTreeNode();

    /// <summary>
    /// Gets or sets the root tree node.
    /// </summary>
    public NGTreeNode RootNode
    {
      get => _rootNode;
      set
      {
        if (!(_rootNode == value))
        {
          _rootNode = value;
          OnPropertyChanged(nameof(RootNode));
        }
      }
    }


    #endregion

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      if (initData)
      {
        if (_doc.Environment is Altaxo.Gui.Graph.Gdi.Viewing.IGraphController)
        {
          NGTreeNode node = null;
          node = new NGTreeNode(true) { Text = "Graph", Tag = _doc.Environment };
          _rootNode.Nodes.Add(node);
        }

        var tableCollectionNode = new NGTreeNode(true) { Text = "Tables", Tag = Current.Project.DataTableCollection };
        _rootNode.Nodes.Add(tableCollectionNode);

        AddAllTableNodes(tableCollectionNode);

        DataTable selectedTable = null;
        if (_doc.SelectedColumn is not null)
          selectedTable = DataTable.GetParentDataTableOf(_doc.SelectedColumn);

        if (selectedTable is not null)
        {
          var selTableNode = FindTableNode(tableCollectionNode, selectedTable);
          if (selTableNode is not null)
            selTableNode.IsExpanded = true;

          if (selTableNode is not null && _doc.SelectedColumn is not null)
          {
            var selColumnNode = FindColumnNode(selTableNode, _doc.SelectedColumn);
            if (selColumnNode is not null)
              selColumnNode.IsSelected = true;
          }
        }
      }
    }

    /// <summary>
    /// Adds all table nodes below the specified table collection node.
    /// </summary>
    /// <param name="tableCollectionNode">The table collection node.</param>
    public static void AddAllTableNodes(NGTreeNode tableCollectionNode)
    {
      // Create a dictionary of folders to TreeNodes relation
      var folderDict = new Dictionary<string, NGTreeNode>
      {
        { ProjectFolder.RootFolderName, tableCollectionNode } // add the root folder node to the dictionary
      };

      tableCollectionNode.Nodes.Clear();
      foreach (var table in Current.Project.DataTableCollection)
      {
        var parentNode = ProjectFolders.AddFolderNodeRecursively(table.FolderName, folderDict);
        var node = new TableNode(table);
        parentNode.Nodes.Add(node);
      }
    }

    private NGTreeNode FindTableNode(NGTreeNode tableCollectionNode, DataTable table)
    {
      NGTreeNode result = null;

      foreach (NGTreeNode node in tableCollectionNode.Nodes)
        if (object.ReferenceEquals(node.Tag, table))
        {
          result = node;
          return result;
        }

      foreach (NGTreeNode node in tableCollectionNode.Nodes)
      {
        result = FindTableNode(node, table);
        if (result is not null)
          return result;
      }

      return result;
    }

    private NGTreeNode FindColumnNode(NGTreeNode tableNode, DataColumn column)
    {
      NGTreeNode result = null;
      foreach (NGTreeNode node in tableNode.Nodes)
      {
        if (object.ReferenceEquals(node.Tag, column))
        {
          return node;
        }
        else if (node.HasChilds)
        {
          result = FindColumnNode(node, column);
          if (result is not null)
            return result;
        }
      }

      return null;
    }

    private NGTreeNode FindNode(NGTreeNodeCollection nodecoll, string txt)
    {
      foreach (var nd in nodecoll)
        if (nd.Text == txt)
          return nd;

      return null;
    }

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      if (_selectedColumn is not null)
      {
        _doc.SelectedColumn = _selectedColumn;
        return ApplyEnd(true, disposeController);
      }

      return ApplyEnd(false, disposeController);
    }

    /// <summary>
    /// Handles the selection change after a node was selected.
    /// </summary>
    public void EhView_AfterSelectNode()
    {
      var node = _rootNode.FirstSelectedNode;

      if (node.Tag is DataColumn dc)
        _selectedColumn = dc;
      else
        _selectedColumn = null;
    }
  }
}
