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
  public interface ISingleColumnChoiceView : IDataContextAwareView
  {
  }

  #region SingleColumnChoice document

  /// <summary>
  /// Summary description for SingleColumnChoice.
  /// </summary>
  public class SingleColumnChoice
  {
    public DataColumn SelectedColumn;
    public object Environment;
    public bool AllowOnlyNumericColumns;
  }

  #endregion SingleColumnChoice document

  [UserControllerForObject(typeof(SingleColumnChoice))]
  [ExpectedTypeOfView(typeof(ISingleColumnChoiceView))]
  public class SingleColumnChoiceController : MVCANControllerEditImmutableDocBase<SingleColumnChoice, ISingleColumnChoiceView>
  {
    #region My private nodes

    internal class TableNode : NGTreeNode
    {
      private const int MaxNumberOfColumnsInOneNode = 100;
      private DataColumnCollection _collection;
      private int _firstColumn;
      private int _columnCount;

      public TableNode(DataTable table)
        : base(true)
      {
        _collection = table.DataColumns;
        _firstColumn = 0;
        _columnCount = table.DataColumns.ColumnCount;
        Text = table.ShortName;
        Tag = table;
      }

      public TableNode(DataColumnCollection coll, int firstColumn, int columnCount)
        : base(true)
      {
        _firstColumn = firstColumn;
        _columnCount = columnCount;
        _collection = coll;
        Text = string.Format("Cols {0}-{1}", firstColumn, firstColumn + columnCount - 1);
      }

      public DataColumnCollection Collection
      {
        get { return _collection; }
        set { _collection = value; }
      }

      public int FirstColumn
      {
        get { return _firstColumn; }
        set { _firstColumn = value; }
      }

      public int ColumnCount
      {
        get { return _columnCount; }
        set { _columnCount = value; }
      }

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

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public SingleColumnChoiceController(SingleColumnChoice doc)
    {
      CmdSelectedItemChanged = new RelayCommand(EhView_AfterSelectNode);
      _doc = doc;
      Initialize(true);
    }

    #region Bindings

    public ICommand CmdSelectedItemChanged { get; }

    private NGTreeNode _rootNode = new NGTreeNode();

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

    public override bool Apply(bool disposeController)
    {
      if (_selectedColumn is not null)
      {
        _doc.SelectedColumn = _selectedColumn;
        return ApplyEnd(true, disposeController);
      }

      return ApplyEnd(false, disposeController);
    }

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
