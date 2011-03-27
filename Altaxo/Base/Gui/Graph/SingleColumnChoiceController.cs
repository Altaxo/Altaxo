#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

		class TableNode : NGTreeNode
		{
			public TableNode() : base(true) { }

			protected override void LoadChildren()
			{
				DataTable table = Tag as DataTable;
				if (null != table)
				{
					Nodes.Clear();
					for (int i = 0; i < table.DataColumnCount; ++i)
						Nodes.Add(new NGTreeNode() { Text = table.DataColumns.GetColumnName(i), Tag = table.DataColumns[i] });
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
						selColumnNode.IsSelected = true;
					}
				}
			}

      if(_view!=null)
      {
        _view.Initialize(_rootNode.Nodes);
      }
    }

		void AddAllTableNodes(NGTreeNode tableCollectionNode)
		{
			tableCollectionNode.Nodes.Clear();
			foreach (var table in Current.Project.DataTableCollection)
			{
				var node = new TableNode { Text = table.Name, Tag = table };
				tableCollectionNode.Nodes.Add(node);
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
			foreach (NGTreeNode node in tableNode.Nodes)
				if (object.ReferenceEquals(node.Tag, column))
				{
					return node;
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
      while(node.Parent!=null)
        node = node.Parent;

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
