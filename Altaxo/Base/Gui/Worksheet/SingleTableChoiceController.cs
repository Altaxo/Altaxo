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

using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Main;
using System;
using System.Collections.Generic;

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

	#endregion SingleColumnChoice document

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

	#endregion interfaces

	[UserControllerForObject(typeof(SingleTableChoice))]
	[ExpectedTypeOfView(typeof(ISingleTreeViewItemChoiceView))]
	public class SingleColumnChoiceController : IMVCANController
	{
		#region My private nodes

		private class TableNode : NGTreeNode
		{
			public TableNode(DataTable table)
				: base(false)
			{
				Text = table.ShortName;
				Tag = table;
			}
		}

		private class StructureNode : NGTreeNode
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

		#endregion My private nodes

		private ISingleTreeViewItemChoiceView _view;
		private SingleTableChoice _doc;

		private DataTable _selectedTable = null;
		private NGTreeNode _rootNode = new NGTreeNode();

		public bool InitializeDocument(params object[] args)
		{
			if (null == args || 0 == args.Length || !(args[0] is SingleTableChoice))
				return false;

			_doc = (SingleTableChoice)args[0];
			Initialize(true);
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
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

			if (_view != null)
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
				var parentNode = ProjectFolders.AddFolderNodeRecursively<StructureNode>(table.FolderName, folderDict);
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
				if (null != result)
					return result;
			}

			return result;
		}

		private NGTreeNode FindNode(NGTreeNodeCollection nodecoll, string txt)
		{
			foreach (var nd in nodecoll)
				if (nd.Text == txt)
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

		public void Dispose()
		{
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply(bool disposeController)
		{
			if (_selectedTable != null)
			{
				_doc.SelectedTable = _selectedTable;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public bool Revert(bool disposeController)
		{
			return false;
		}

		#endregion IApplyController Members

		#region ISingleColumnChoiceViewEventSink Members

		protected NGTreeNode GetRootNode(NGTreeNode node)
		{
			while (node.ParentNode != null)
				node = node.ParentNode;

			return node;
		}

		#endregion ISingleColumnChoiceViewEventSink Members
	}
}