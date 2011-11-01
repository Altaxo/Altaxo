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

using Altaxo.Gui;
using Altaxo.Data;
using Altaxo.Collections;

namespace Altaxo.Gui.Worksheet
{
  #region SingleColumnChoice document

  /// <summary>
  /// Summary description for SingleColumnChoice.
  /// </summary>
  public class SingleTableChoice
  {
		public DataTable SelectedTable { get; set; }
  }

  #endregion

  #region interfaces

  public interface ISingleTreeViewItemChoiceView
  {
    /// <summary>
    /// Initializes the treeview of available data with content.
    /// </summary>
    /// <param name="nodes"></param>
    void Initialize(NGTreeNodeCollection nodes);

		event Action<NGTreeNode> SelectionChanged;
  }

  #endregion

  [UserControllerForObject(typeof(SingleTableChoice))]
	[ExpectedTypeOfView(typeof(ISingleTreeViewItemChoiceView))]
  public class SingleColumnChoiceController : IMVCANController
	{
		#region My private nodes

		class TableNode : NGTreeNode
		{
			public TableNode(DataTable table)
				: base(false)
			{
				Text = table.ShortName;
				Tag = table;
			}
		}

		class StructureNode : NGTreeNode
		{
			public override bool IsSelected
			{
				get
				{
					return false;
				}
				set
				{
					base.IsSelected = false;
				}
			}
		}
		#endregion

		ISingleTreeViewItemChoiceView _view;
    SingleTableChoice _doc;

    DataTable _selectedTable = null;
		NGTreeNode _rootNode = new NGTreeNode();


		public bool  InitializeDocument(params object[] args)
{
 	if(null==args || 0==args.Length || !(args[0] is SingleTableChoice))
		return false;

			_doc = (SingleTableChoice)args[0];
			Initialize(true);
			return true;
}

public UseDocument  UseDocumentCopy
{
	set {  }
}

    public void Initialize(bool initData)
    {
			if (initData)
			{
				var tableCollectionNode = new StructureNode { Text = "Tables", Tag = Current.Project.DataTableCollection };

				_rootNode.Nodes.Add(tableCollectionNode);

				AddAllTableNodes(tableCollectionNode);

				tableCollectionNode.IsExpanded = true;

			

				if (null != _doc.SelectedTable)
				{
					var selTableNode = FindTableNode(tableCollectionNode, _doc.SelectedTable);
					if (selTableNode != null)
					{
						selTableNode.IsSelected = true;
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
			folderDict.Add(Main.ProjectFolder.RootFolderName, tableCollectionNode); // add the root folder node to the dictionary

			tableCollectionNode.Nodes.Clear();
			foreach (var table in Current.Project.DataTableCollection)
			{
				var parentNode = Main.ProjectFolders.AddFolderNodeRecursively<StructureNode>(table.FolderName, folderDict);
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

    private NGTreeNode FindNode(NGTreeNodeCollection nodecoll, string txt)
    {
      foreach(var nd in nodecoll)
        if(nd.Text==txt)
          return nd;

      return null;
    }


		public void EhSelectionChanged(NGTreeNode node)
		{
			if (node.Tag is DataTable)
				_selectedTable = (DataTable)node.Tag;
			else
				_selectedTable = null;
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
				if (_view != null)
				{
					_view.SelectionChanged -= this.EhSelectionChanged;
				}

				_view = value as ISingleTreeViewItemChoiceView;
        

				if (_view != null)
				{
					Initialize(false);

					_view.SelectionChanged += this.EhSelectionChanged;
				}
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
      if(_selectedTable != null)
      {
        _doc.SelectedTable = _selectedTable;
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

   

    #endregion
  

}

}
