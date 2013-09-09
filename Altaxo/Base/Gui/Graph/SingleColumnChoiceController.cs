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

using Altaxo.Main;
using Altaxo.Gui;
using Altaxo.Data;
using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
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

  #endregion

  #region interfaces

  public interface ISingleColumnChoiceView
  {
    ISingleColumnChoiceViewEventSink Controller { get; set; }
    /// <summary>
    /// Initializes the treeview of available data with content.
    /// </summary>
    /// <param name="nodes"></param>
    void Initialize(NGTreeNodeCollection nodes);
  }

  public interface ISingleColumnChoiceViewEventSink
  {
    void EhView_AfterSelectNode(NGTreeNode node);
  }

  #endregion

  [UserControllerForObject(typeof(SingleColumnChoice))]
  [ExpectedTypeOfView(typeof(ISingleColumnChoiceView))]
  public class SingleColumnChoiceController : IMVCAController, ISingleColumnChoiceViewEventSink
	{
		#region My private nodes

		internal class TableNode : NGTreeNode
		{
			const int MaxNumberOfColumnsInOneNode = 100;
			DataColumnCollection _collection;
			int _firstColumn;
			int _columnCount;

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
				int nextColumn = Math.Min(_firstColumn+_columnCount,coll.ColumnCount);

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
					for (; colsInOneNode * numNodes < _columnCount; colsInOneNode *= MaxNumberOfColumnsInOneNode) ; // Multiply with a multiple of MaxNumberOfColumnsInOneNode until it fits

					int first=_firstColumn;
					int remaining = nextColumn - _firstColumn;
					for (int i = 0; i < numNodes && remaining>0; ++i )
					{
						Nodes.Add(new TableNode(coll, first, Math.Min(remaining, colsInOneNode)));
						remaining -= colsInOneNode;
						first += colsInOneNode;
					}
				}
			}
		}

		#endregion

		ISingleColumnChoiceView _view;
    SingleColumnChoice _doc;

    DataColumn _selectedColumn = null;
		NGTreeNode _rootNode = new NGTreeNode();

    public SingleColumnChoiceController(SingleColumnChoice doc)
    {
      _doc = doc;
			Initialize(true);
    }

    public void Initialize(bool initData)
    {
			if (initData)
			{
				if (_doc.Environment is Altaxo.Gui.Graph.Viewing.IGraphController)
				{
					NGTreeNode node = null;
					node = new NGTreeNode(true) { Text = "Graph", Tag = _doc.Environment };
					_rootNode.Nodes.Add(node);
				}

				var tableCollectionNode = new NGTreeNode(true) { Text="Tables", Tag=Current.Project.DataTableCollection };
				_rootNode.Nodes.Add(tableCollectionNode);

				AddAllTableNodes(tableCollectionNode);


				DataTable selectedTable = null;
				if (_doc.SelectedColumn != null)
					selectedTable = DataTable.GetParentDataTableOf(_doc.SelectedColumn);

				if (null != selectedTable)
				{
					var selTableNode = FindTableNode(tableCollectionNode, selectedTable);
					if(selTableNode != null)
						selTableNode.IsExpanded = true;

					if (null != selTableNode && null != _doc.SelectedColumn)
					{
						var selColumnNode = FindColumnNode(selTableNode, _doc.SelectedColumn);
						if(null!=selColumnNode)
							selColumnNode.IsSelected = true;
					}
				}
			}

      if(_view!=null)
      {
        _view.Initialize(_rootNode.Nodes);
      }
    }


		public static void AddAllTableNodes(NGTreeNode tableCollectionNode)
		{
			// Create a dictionary of folders to TreeNodes relation
			var folderDict = new Dictionary<string, NGTreeNode>();
			folderDict.Add(ProjectFolder.RootFolderName, tableCollectionNode); // add the root folder node to the dictionary

			tableCollectionNode.Nodes.Clear();
			foreach (var table in Current.Project.DataTableCollection)
			{
				var parentNode = ProjectFolders.AddFolderNodeRecursively(table.FolderName, folderDict);
				var node = new TableNode(table);
				parentNode.Nodes.Add(node);
			}
		}


		NGTreeNode FindTableNode(NGTreeNode tableCollectionNode, DataTable table)
		{
			NGTreeNode result=null;

			foreach(NGTreeNode node in tableCollectionNode.Nodes)
				if(object.ReferenceEquals(node.Tag,table))
				{
					result=node;
					return result;
				}

			foreach(NGTreeNode node in tableCollectionNode.Nodes)
			{
				result = FindTableNode(node, table);
				if(null!=result)
					return result;
			}

			return result;
		}


		NGTreeNode FindColumnNode(NGTreeNode tableNode, DataColumn column)
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
					if (null != result)
						return result;
				}
			}

			return null;
		}

    private NGTreeNode FindNode(NGTreeNodeCollection nodecoll, string txt)
    {
      foreach(var nd in nodecoll)
        if(nd.Text==txt)
          return nd;

      return null;
    }



    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        
        return _view;
      }
      set
      {
        if(_view!=null)
          _view.Controller = null;

        _view = value as ISingleColumnChoiceView;
        
        Initialize(false);

        if(_view!=null)
          _view.Controller = this;
      }
    }

    public object ModelObject
    {
      get
      {
        return _doc;
      }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      if(_selectedColumn != null)
      {
        _doc.SelectedColumn = _selectedColumn;
        return true;
      }

      return false;
    }

    #endregion

    #region ISingleColumnChoiceViewEventSink Members

    protected NGTreeNode GetRootNode(NGTreeNode node)
    {
      while(node.ParentNode!=null)
        node = node.ParentNode;

      return node;
    }

    public void EhView_AfterSelectNode(NGTreeNode node)
    {
      if(node.Tag is DataColumn)
        _selectedColumn = (DataColumn)node.Tag;
      else
        _selectedColumn = null;
    }

    #endregion
  }

}
