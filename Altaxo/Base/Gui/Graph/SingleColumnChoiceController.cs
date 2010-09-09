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
    void Initialize(NGTreeNode[] nodes);
    void SelectNode(NGTreeNode node);
    void ExpandNode(NGTreeNode node);
    void InitializeNewNodes(NGTreeNode[] nodes);
    void EhNodesCleared(NGTreeNodeCollection nodes);
  }

  public interface ISingleColumnChoiceViewEventSink
  {
    void EhView_BeforeExpand(NGTreeNode node);
    void EhView_AfterSelectNode(NGTreeNode node);
  }

  #endregion

  [UserControllerForObject(typeof(SingleColumnChoice))]
  [ExpectedTypeOfView(typeof(ISingleColumnChoiceView))]
  public class SingleColumnChoiceController : IMVCAController, ISingleColumnChoiceViewEventSink
  {
    ISingleColumnChoiceView _view;
    SingleColumnChoice _doc;

    DataColumn _selectedColumn = null;

    public SingleColumnChoiceController(SingleColumnChoice doc)
    {
      _doc = doc;
    }

    public void Initialize()
    {
      if(_view!=null)
      {
        var arr = new List<NGTreeNode>();
        NGTreeNode node = null;
				if (_doc.Environment is Altaxo.Gui.Graph.Viewing.IGraphController)
        {
          node = new NGTreeNode("Graph",new NGTreeNode[1]{new NGTreeNode()});
          node.Tag = _doc.Environment;
          arr.Add(node);
        }
        var tablenode = new NGTreeNode("Tables",new NGTreeNode[1]{new NGTreeNode()});
        tablenode.Tag = Current.Project.DataTableCollection;
        arr.Add(tablenode);

        _view.Initialize(arr.ToArray());

        DataTable selectedTable = null;
        if (_doc.SelectedColumn != null)
          selectedTable = DataTable.GetParentDataTableOf(_doc.SelectedColumn);

        if (null != selectedTable)
        {
          this.EhView_BeforeExpand(tablenode);
          _view.ExpandNode(tablenode);

          NGTreeNode found = FindNode(tablenode.Nodes,selectedTable.Name);
          if (found!=null)
          {
            NGTreeNode selectedTableNode = found;
            this.EhView_BeforeExpand(selectedTableNode);
            _view.ExpandNode(selectedTableNode);

            found = FindNode(selectedTableNode.Nodes,_doc.SelectedColumn.Name);

            if (found != null)
            {
              _view.SelectNode(found);
            }

          }
        }
          
      }


      
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
        
        Initialize();

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


    public void EhView_BeforeExpand(NGTreeNode node)
    {
      var rootNode = GetRootNode(node);

      if(rootNode.Tag is DataTableCollection)
      {
        if(node.Tag is DataTableCollection)
        {
          _view.EhNodesCleared(node.Nodes);
          node.Nodes.Clear();
          NGTreeNode[] toadd = new NGTreeNode[Current.Project.DataTableCollection.Count];

          int i=0;
          foreach(DataTable table in Current.Project.DataTableCollection)
          {
            toadd[i] = new NGTreeNode(table.Name, new NGTreeNode[1]{new NGTreeNode()});
            toadd[i].Tag = table;
            i++;
          }
          node.Nodes.AddRange(toadd);
          _view.InitializeNewNodes(toadd);
          return;
        }
        else if(node.Tag is DataTable)
        {
          DataTable dt = (DataTable)node.Tag;
          _view.EhNodesCleared(node.Nodes);
          node.Nodes.Clear();
          NGTreeNode[] toadd = new NGTreeNode[dt.DataColumns.ColumnCount];
          for(int i=0;i<toadd.Length;i++)
          {
            toadd[i] = new NGTreeNode(dt[i].Name);
            toadd[i].Tag = dt[i];
          }
          node.Nodes.AddRange(toadd);
          _view.InitializeNewNodes(toadd);
          return;
        }
      }
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
