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
using System.Windows.Forms;

using Altaxo.Gui;
using Altaxo.Data;

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
    void Initialize(TreeNode[] nodes);

    void SelectNode(TreeNode node);
  }

  public interface ISingleColumnChoiceViewEventSink
  {
    void EhView_BeforeExpand(TreeNode node);
    void EhView_AfterSelectNode(TreeNode node);
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
        System.Collections.ArrayList arr = new System.Collections.ArrayList();
        TreeNode node = null;
        if(_doc.Environment is Altaxo.Graph.GUI.GraphController)
        {
          node = new TreeNode("Graph",new TreeNode[1]{new TreeNode()});
          node.Tag = _doc.Environment;
          arr.Add(node);
        }
        TreeNode tablenode = new TreeNode("Tables",new TreeNode[1]{new TreeNode()});
        tablenode.Tag = Current.Project.DataTableCollection;
        arr.Add(tablenode);

        _view.Initialize((TreeNode[])arr.ToArray(typeof(TreeNode)));

        DataTable selectedTable = null;
        if (_doc.SelectedColumn != null)
          selectedTable = DataTable.GetParentDataTableOf(_doc.SelectedColumn);

        if (null != selectedTable)
        {
          this.EhView_BeforeExpand(tablenode);
          tablenode.Expand();

          TreeNode found = FindNode(tablenode.Nodes,selectedTable.Name);
          if (found!=null)
          {
            TreeNode selectedTableNode = found;
            this.EhView_BeforeExpand(selectedTableNode);
            selectedTableNode.Expand();

            found = FindNode(selectedTableNode.Nodes,_doc.SelectedColumn.Name);

            if (found != null)
            {
              _view.SelectNode(found);
            }

          }
        }
          
      }


      
    }

    private TreeNode FindNode(TreeNodeCollection nodecoll, string txt)
    {
      foreach(TreeNode nd in nodecoll)
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

    protected TreeNode GetRootNode(TreeNode node)
    {
      while(node.Parent!=null)
        node = node.Parent;

      return node;
    }


    public void EhView_BeforeExpand(TreeNode node)
    {
      TreeNode rootNode = GetRootNode(node);

      if(rootNode.Tag is DataTableCollection)
      {
        if(node.Tag is DataTableCollection)
        {
          node.Nodes.Clear();
          TreeNode[] toadd = new TreeNode[Current.Project.DataTableCollection.Count];

          int i=0;
          foreach(DataTable table in Current.Project.DataTableCollection)
          {
            toadd[i] = new TreeNode(table.Name, new TreeNode[1]{new TreeNode()});
            toadd[i].Tag = table;
            i++;
          }
          node.Nodes.AddRange(toadd);
          return;
        }
        else if(node.Tag is DataTable)
        {
        

          Data.DataTable dt = (DataTable)node.Tag;
          node.Nodes.Clear();
          TreeNode[] toadd = new TreeNode[dt.DataColumns.ColumnCount];
          for(int i=0;i<toadd.Length;i++)
          {
            toadd[i] = new TreeNode(dt[i].Name);
            toadd[i].Tag = dt[i];
          }
          node.Nodes.AddRange(toadd);
          return;
        }
      }
    }

    public void EhView_AfterSelectNode(TreeNode node)
    {
      if(node.Tag is DataColumn)
        _selectedColumn = (DataColumn)node.Tag;
      else
        _selectedColumn = null;
    }

    #endregion
  }

}
