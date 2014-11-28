#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui;
using Altaxo.Gui.Common;
using Altaxo.Serialization.Clipboard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	public interface ILineScatterLayerContentsViewEventSink
	{
		void EhView_DataAvailableBeforeExpand(NGTreeNode node);

		void EhView_ContentsDoubleClick(NGTreeNode selNode);

		void AvailableItems_PutDataToPlotItems();

		void PlotItems_MoveUpSelected();

		void PlotItems_MoveDownSelected();

		void PlotItems_GroupClick();

		void PlotItems_UngroupClick();

		void PlotItems_EditRangeClick();

		void PlotItem_Open();

		void PlotItems_Copy();

		void PlotItems_Cut();

		bool PlotItems_CanPaste();

		void PlotItems_Paste();

		bool PlotItems_CanDelete();

		void PlotItems_Delete();

		void PlotItems_ShowRangeChanged(bool showRange);

		bool PlotItems_CanStartDrag(IEnumerable items);

		void PlotItems_StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove);

		void PlotItems_DragEnded(bool isCopy, bool isMove);

		void PlotItems_DragCancelled();

		void PlotItems_DropCanAcceptData(object data, NGTreeNode targetItem, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool canCopy, out bool canMove, out bool itemIsSwallowingData);

		void PlotItems_Drop(object data, NGTreeNode nGTreeNode, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove);

		bool AvailableItems_CanStartDrag(IEnumerable items);

		void AvailableItems_StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove);

		void AvailableItems_DragEnded(bool isCopy, bool isMove);

		void AvailableItems_DragCancelled();
	}

	public interface ILineScatterLayerContentsView
	{
		/// <summary>
		/// Get/sets the controller of this view.
		/// </summary>
		ILineScatterLayerContentsViewEventSink Controller { get; set; }

		IEnumerable<object> PlotItemsSelected { get; }

		IEnumerable<object> AvailableItemsSelected { get; }

		/// <summary>
		/// Initializes the treeview of available data with content.
		/// </summary>
		/// <param name="nodes"></param>
		void InitializeAvailableItems(NGTreeNodeCollection nodes);

		/// <summary>
		/// Initializes the content list box by setting the items.
		/// </summary>
		/// <param name="items">Collection of items.</param>
		void InitializePlotItems(NGTreeNodeCollection items);

		bool ShowRange { set; }
	}

	#endregion Interfaces

	/// <summary>
	/// Controls the content of a <see cref="PlotItemCollection" />
	/// </summary>
	[UserControllerForObject(typeof(PlotItemCollection))]
	[ExpectedTypeOfView(typeof(ILineScatterLayerContentsView))]
	public class LineScatterLayerContentsController : ILineScatterLayerContentsViewEventSink, IMVCANController
	{
		protected ILineScatterLayerContentsView _view;
		protected PlotItemCollection _doc;
		protected PlotItemCollection _originalDoc;

		private NGTreeNode _plotItemsRootNode = new NGTreeNode();
		private NGTreeNodeCollection _plotItemsTree;
		private NGTreeNode _availableItemsRootNode = new NGTreeNode();

		private bool _isDirty = false;
		private UseDocument _useDocument;

		public UseDocument UseDocumentCopy { set { _useDocument = value; } }

		private bool _showRange = false;

		public LineScatterLayerContentsController()
		{
		}

		/// <summary>
		/// Initialize the controller with the document. If successfull, the function has to return true.
		/// </summary>
		/// <param name="args">The arguments neccessary to create the controller. Normally, the first argument is the document, the second can be the parent of the document and so on.</param>
		/// <returns>True if successfull, else false.</returns>
		public bool InitializeDocument(params object[] args)
		{
			if (args == null || args.Length == 0 || !(args[0] is PlotItemCollection))
				return false;
			_originalDoc = (PlotItemCollection)args[0];

			if (_useDocument == UseDocument.Copy)
				_doc = _originalDoc.Clone();
			else
				_doc = _originalDoc;

			_plotItemsTree = _plotItemsRootNode.Nodes;
			Initialize(true);
			return true;
		}

		public void SetDirty()
		{
			_isDirty = true;
		}

		public void Initialize(bool initData)
		{
			if (_doc == null)
				throw new ApplicationException("Doc was not set before!");

			// now fill the tree view  with all plot associations currently inside
			if (initData)
			{
				// Plot items
				_plotItemsTree.Clear();
				PlotItemsToTree(_plotItemsRootNode, _doc);

				// available items
				SingleColumnChoiceController.AddAllTableNodes(_availableItemsRootNode);

				_isDirty = false;
			}

			// Available Items
			if (null != _view)
			{
				_view.InitializePlotItems(_plotItemsTree);

				_view.InitializeAvailableItems(this._availableItemsRootNode.Nodes);

				_view.ShowRange = _showRange;
			}
		}

		private NGTreeNode[] PlotItemsSelected
		{
			get
			{
				if (null != _view)
					return _view.PlotItemsSelected.OfType<NGTreeNode>().ToArray();
				else
					return new NGTreeNode[0];
			}
		}

		private void AvailableItems_ClearSelection()
		{
			_availableItemsRootNode.ClearSelectionRecursively();
		}

		public void PlotItems_ShowRangeChanged(bool showRange)
		{
			var oldValue = _showRange;
			_showRange = showRange;

			if (oldValue != _showRange)
			{
				_plotItemsTree.Clear();
				PlotItemsToTree(_plotItemsRootNode, _doc);
			}
		}

		private string GetNameOfItem(IGPlotItem item)
		{
			if (item is PlotItemCollection)
			{
				return "PlotGroup";
			}
			else if (item != null && item is PlotItem)
			{
				string name = item.GetName(2);
				if (_showRange && item is XYColumnPlotItem)
				{
					var pi1 = item as XYColumnPlotItem;
					return string.Format("{0} ({1}-{2})", name, pi1.Data.PlotRangeStart, pi1.Data.PlotRangeEnd);
				}
				else
				{
					return name;
				}
			}
			else
			{
				return string.Empty;
			}
		}

		private void PlotItemsToTree(NGTreeNode node, PlotItemCollection picoll)
		{
			foreach (IGPlotItem pa in picoll)
			{
				if (pa is PlotItemCollection) // if this is a plot item collection
				{
					// add only one item to the list box, namely a PLCon group item with
					// all the members of that group
					NGTreeNode grpNode = new NGTreeNode();
					grpNode.Text = GetNameOfItem(pa);
					grpNode.Tag = pa;
					grpNode.IsExpanded = true;
					node.Nodes.Add(grpNode);
					// add all the items in the group also to the list of added items
					PlotItemsToTree(grpNode, (PlotItemCollection)pa);
				}
				else // else if the item is not in a plot group
				{
					NGTreeNode toAdd = new NGTreeNode();
					toAdd.Text = GetNameOfItem(pa);
					toAdd.Tag = pa;
					toAdd.IsExpanded = true;
					node.Nodes.Add(toAdd);
				}
			}
		}

		private void TreeToPlotItems(NGTreeNode rootnode, PlotItemCollection picoll)
		{
			picoll.ClearPlotItems(); // do not clear group styles here, otherwise group styles would not be applied
			foreach (NGTreeNode node in rootnode.Nodes)
			{
				IGPlotItem item = (IGPlotItem)node.Tag;
				if (item is PlotItemCollection) // if this is a plot item collection
					TreeToPlotItems(node, (PlotItemCollection)item);

				picoll.Add(item);
			}
		}

		private void PutSelectedPlotItemsToTemporaryDocumentForClipboard(NGTreeNode[] selNodes, PlotItemCollection picoll)
		{
			picoll.ClearPlotItems();
			foreach (NGTreeNode node in selNodes)
			{
				IGPlotItem item = (IGPlotItem)node.Tag;
				if (item is PlotItemCollection) // if this is a plot item collection
					TreeToPlotItems(node, (PlotItemCollection)item);

				picoll.Add(item);
			}
		}

		private static XYColumnPlotItem FindFirstXYColumnPlotItem(PlotItemCollection coll)
		{
			// search in our document for the first plot item that is XYColumnPlotItem,
			// we need this as template style
			foreach (IGPlotItem pi in coll)
			{
				if (pi is PlotItemCollection)
				{
					XYColumnPlotItem result = FindFirstXYColumnPlotItem(pi as PlotItemCollection);
					if (result != null)
						return result;
				}
				else if (pi is XYColumnPlotItem)
				{
					return pi as XYColumnPlotItem;
				}
			}
			return null;
		}

		private IGPlotItem CreatePlotItem(string tablename, string columnname)
		{
			if (string.IsNullOrEmpty(tablename) || string.IsNullOrEmpty(columnname))
				return null;

			// create a new plotassociation from the column
			// first, get the y column from table and name
			DataTable tab = Current.Project.DataTableCollection[tablename];
			if (null != tab)
			{
				DataColumn ycol = tab[columnname];
				if (null != ycol)
				{
					DataColumn xcol = tab.DataColumns.FindXColumnOf(ycol);

					// search in our document for the first plot item that is an XYColumnPlotItem,
					// we need this as template style
					XYColumnPlotItem templatePlotItem = FindFirstXYColumnPlotItem(_doc);
					G2DPlotStyleCollection templatePlotStyle;
					if (null != templatePlotItem)
					{
						templatePlotStyle = templatePlotItem.Style.Clone();
					}
					else // there is no item that can be used as template
					{
						int numRows = ycol.Count;
						if (null != xcol)
							numRows = Math.Min(numRows, xcol.Count);
						if (numRows < 100)
						{
							templatePlotStyle = new G2DPlotStyleCollection(LineScatterPlotStyleKind.LineAndScatter, _doc.GetPropertyContext());
						}
						else
						{
							templatePlotStyle = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line, _doc.GetPropertyContext());
						}
					}

					XYColumnPlotItem result;
					if (null == xcol)
						result = new XYColumnPlotItem(new XYColumnPlotData(new Altaxo.Data.IndexerColumn(), ycol), templatePlotStyle);
					else
						result = new XYColumnPlotItem(new XYColumnPlotData(xcol, ycol), templatePlotStyle);

					return result;
				}
			}
			return null;
		}

		private IGPlotItem CreatePlotItem(Altaxo.Data.DataColumn ycol)
		{
			if (null == ycol)
				return null;

			DataTable tab = DataTable.GetParentDataTableOf(ycol);
			if (null == tab)
				return null;

			DataColumn xcol = tab.DataColumns.FindXColumnOf(ycol);

			// search in our document for the first plot item that is an XYColumnPlotItem,
			// we need this as template style
			XYColumnPlotItem templatePlotItem = FindFirstXYColumnPlotItem(_doc);
			G2DPlotStyleCollection templatePlotStyle;
			if (null != templatePlotItem)
			{
				templatePlotStyle = templatePlotItem.Style.Clone();
			}
			else // there is no item that can be used as template
			{
				int numRows = ycol.Count;
				if (null != xcol)
					numRows = Math.Min(numRows, xcol.Count);
				if (numRows < 100)
				{
					templatePlotStyle = new G2DPlotStyleCollection(LineScatterPlotStyleKind.LineAndScatter, _doc.GetPropertyContext());
				}
				else
				{
					templatePlotStyle = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line, _doc.GetPropertyContext());
				}
			}

			XYColumnPlotItem result;
			if (null == xcol)
				result = new XYColumnPlotItem(new XYColumnPlotData(new Altaxo.Data.IndexerColumn(), ycol), templatePlotStyle);
			else
				result = new XYColumnPlotItem(new XYColumnPlotData(xcol, ycol), templatePlotStyle);

			return result;
		}

		#region ILineScatterLayerContentsController Members

		public void EhView_DataAvailableBeforeExpand(NGTreeNode node)
		{
			DataTable dt = Current.Project.DataTableCollection[node.Text];
			if (null != dt)
			{
				node.Nodes.Clear();
				NGTreeNode[] toadd = new NGTreeNode[dt.DataColumns.ColumnCount];
				for (int i = 0; i < toadd.Length; i++)
				{
					toadd[i] = new NGTreeNode(dt[i].Name);
				}
				node.Nodes.AddRange(toadd);
			}
		}

		/// <summary>
		/// Puts the selected data columns into the plot content.
		/// </summary>
		public void AvailableItems_PutDataToPlotItems()
		{
			var selNodes = _view.AvailableItemsSelected.OfType<NGTreeNode>();

			var columnsAlreadyProcessed = new HashSet<Altaxo.Data.DataColumn>();

			var validNodes = NGTreeNode.NodesWithoutSelectedChilds(selNodes);

			// first, put the selected node into the list, even if it is not checked
			foreach (NGTreeNode sn in validNodes)
			{
				var dataCol = sn.Tag as Altaxo.Data.DataColumn;
				if (null != dataCol && !columnsAlreadyProcessed.Contains(dataCol))
				{
					columnsAlreadyProcessed.Add(dataCol);
					CreatePlotItemNodeAndAddAtEndOfTree(dataCol);
				}
				else if (sn is SingleColumnChoiceController.TableNode)
				{
					var rangeNode = sn as SingleColumnChoiceController.TableNode;
					var coll = rangeNode.Collection;
					var firstCol = rangeNode.FirstColumn;
					var columnCount = rangeNode.ColumnCount;

					for (int i = firstCol; i < firstCol + columnCount; ++i)
					{
						dataCol = coll[i];
						if (coll.GetColumnKind(i) == ColumnKind.V && !columnsAlreadyProcessed.Contains(dataCol)) // add only value columns as plot items
						{
							columnsAlreadyProcessed.Add(dataCol);
							CreatePlotItemNodeAndAddAtEndOfTree(dataCol);
						}
					}
				}
			}

			_view.InitializePlotItems(_plotItemsTree);
			AvailableItems_ClearSelection();
			SetDirty();
		}

		private NGTreeNode CreatePlotItemNode(DataColumn dataCol)
		{
			IGPlotItem newItem = this.CreatePlotItem(dataCol);
			if (null != newItem)
			{
				_doc.Add(newItem);
				NGTreeNode newNode = new NGTreeNode();
				newNode.Text = newItem.GetName(2);
				newNode.Tag = newItem;
				return newNode;
			}
			else
			{
				return null;
			}
		}

		private void CreatePlotItemNodeAndAddAtEndOfTree(DataColumn dataCol)
		{
			var node = CreatePlotItemNode(dataCol);
			if (null != node)
				_plotItemsTree.Add(node);
		}

		public void PlotItems_MoveUpSelected()
		{
			var selNodes = PlotItemsSelected;
			if (selNodes.Length != 0)
			{
				// move the selected items upwards in the list
				ContentsListBox_MoveUpDown(-1, selNodes);
				SetDirty();
			}
		}

		public void PlotItems_MoveDownSelected()
		{
			var selNodes = PlotItemsSelected;
			if (selNodes.Length != 0)
			{
				// move the selected items downwards in the list
				ContentsListBox_MoveUpDown(1, selNodes);
				SetDirty();
			}
		}

		public void ContentsListBox_MoveUpDown(int iDelta, NGTreeNode[] selNodes)
		{
			if (NGTreeNode.HaveSameParent(selNodes))
			{
				NGTreeNode.MoveUpDown(iDelta, selNodes);
				TreeToPlotItems(_plotItemsRootNode, _doc);
				_view.InitializePlotItems(this._plotItemsTree);
				SetDirty();
			}
		}

		public void ContentsListBox_MoveUpDown(int iDelta, int[] selidxs)
		{
			int i;

			if (iDelta != 1 && iDelta != -1)
				return;

			if (iDelta == -1) // move one position upwards
			{
				if (selidxs[0] == 0) // if the first item is selected, we can't move upwards
				{
					return;
				}

				for (i = 0; i < selidxs.Length; i++)
				{
					NGTreeNode helpSeg;
					int iSeg = selidxs[i];

					helpSeg = _plotItemsTree[iSeg - 1];
					_plotItemsTree[iSeg - 1] = _plotItemsTree[iSeg];
					_plotItemsTree[iSeg] = helpSeg;
				}
			} // end if iDelta==-1
			else if (iDelta == 1) // move one position down
			{
				if (selidxs[selidxs.Length - 1] == _plotItemsTree.Count - 1)    // if last item is selected, we can't move downwards
				{
					return;
				}

				for (i = selidxs.Length - 1; i >= 0; i--)
				{
					NGTreeNode helpSeg;
					int iSeg = selidxs[i];

					helpSeg = _plotItemsTree[iSeg + 1];
					_plotItemsTree[iSeg + 1] = _plotItemsTree[iSeg];
					_plotItemsTree[iSeg] = helpSeg;
				}
			} // end if iDelta==1

			TreeToPlotItems(_plotItemsRootNode, _doc);
		}

		/// <summary>
		/// Group the selected nodes.
		/// </summary>
		public void PlotItems_GroupClick()
		{
			var selNodes = PlotItemsSelected;

			// retrieve the selected items
			if (selNodes.Length < 2)
				return; // we cannot group anything if no or only one item is selected

			// look, if one of the selected items is a plot group
			// if found, use this group and add the remaining items to this
			int foundindex = -1;
			for (int i = 0; i < selNodes.Length; i++)
			{
				if (selNodes[i].Tag is PlotItemCollection)
				{
					foundindex = i;
					break;
				}
			}

			// if a group was found use this to add the remaining items
			if (foundindex >= 0)
			{
				for (int i = 0; i < selNodes.Length; i++)
					if (i != foundindex)
					{
						selNodes[i].Remove();
						selNodes[foundindex].Nodes.Add(selNodes[i]);
					}
			}
			else // if we found no group to add to, we have to create a new group
			{
				NGTreeNode newNode = new NGTreeNode();
				newNode.Tag = new PlotItemCollection();
				newNode.Text = "PlotGroup";

				// now add the remaining selected items to the found group
				for (int i = 0; i < selNodes.Length; i++)
				{
					NGTreeNode node = selNodes[i];
					if (node.Nodes.Count > 0) // if it is a group, add the members of the group to avoid more than one recursion
					{
						while (node.Nodes.Count > 0)
						{
							NGTreeNode addnode = node.Nodes[0];
							addnode.Remove();
							newNode.Nodes.Add(addnode);
						}
					}
					else // item to add is not a group
					{
						node.Remove();
						newNode.Nodes.Add(node);
					}
				} // end for
				_plotItemsRootNode.Nodes.Add(newNode);
			}
			// now all items are in the new group

			TreeToPlotItems(_plotItemsRootNode, _doc);
			// so update the list box:
			_view.InitializePlotItems(this._plotItemsTree);

			SetDirty();
		}

		public void PlotItems_UngroupClick()
		{
			var selNodes = PlotItemsSelected;

			// retrieve the selected items
			if (selNodes.Length < 1)
				return; // we cannot ungroup anything if nothing selected

			selNodes = NGTreeNode.FilterIndependentNodes(selNodes);

			for (int i = 0; i < selNodes.Length; i++)
			{
				if (selNodes[i].Nodes.Count == 0 && selNodes[i].ParentNode != null && selNodes[i].ParentNode.ParentNode != null)
				{
					NGTreeNode parent = selNodes[i].ParentNode;
					NGTreeNode grandParent = parent.ParentNode;
					selNodes[i].Remove();
					grandParent.Nodes.Add(selNodes[i]);

					if (parent.Nodes.Count == 0)
						parent.Remove();
				}
				else if (selNodes[i].Nodes.Count > 0 && selNodes[i].ParentNode != null)
				{
					NGTreeNode parent = selNodes[i].ParentNode;
					while (selNodes[i].Nodes.Count > 0)
					{
						NGTreeNode no = selNodes[i].Nodes[0];
						no.Remove();
						parent.Nodes.Add(no);
					}
					selNodes[i].Remove();
				}
			} // end for

			TreeToPlotItems(_plotItemsRootNode, _doc);
			_view.InitializePlotItems(_plotItemsTree);
			SetDirty();
		}

		public void EhView_ContentsDoubleClick(NGTreeNode selNode)
		{
			IGPlotItem pi = selNode.Tag as IGPlotItem;
			if (null != pi)
			{
				if (pi is PlotItemCollection)
				{
					// show not the dialog for PlotItemCollection, but only those for the group styles into that collection
					Current.Gui.ShowDialog(new object[] { ((PlotItemCollection)pi).GroupStyles }, pi.Name);
				}
				else
				{
					object piAsObject = pi;
					Current.Gui.ShowDialog(ref piAsObject, pi.GetName(2), true);
					pi = (IGPlotItem)piAsObject;
				}
			}

			// now set a new name of this node
			selNode.Text = GetNameOfItem(pi);
			selNode.Tag = pi;
		}

		public void PlotItems_EditRangeClick()
		{
			var selNodes = PlotItemsSelected;

			if (selNodes.Length == 0)
				return;
			int minRange, maxRange;
			if (!GetMinimumMaximumPlotRange(selNodes, out minRange, out maxRange))
				return;
			var range = new Altaxo.Collections.ContiguousNonNegativeIntegerRange(minRange, maxRange - minRange);

			if (!Current.Gui.ShowDialog(ref range, "Edit plot range for selected items", false))
				return;

			foreach (NGTreeNode node in selNodes)
			{
				var pi = node.Tag as XYColumnPlotItem;
				if (pi == null)
					continue;
				pi.Data.PlotRangeStart = range.Start;
				pi.Data.PlotRangeLength = range.Count;
			}
		}

		private bool GetMinimumMaximumPlotRange(IEnumerable<NGTreeNode> selNodes, out int minRange, out int maxRange)
		{
			minRange = int.MaxValue;
			maxRange = int.MinValue;
			bool result = false;

			foreach (NGTreeNode node in selNodes)
			{
				var pi = node.Tag as XYColumnPlotItem;
				if (pi == null)
					continue;
				minRange = Math.Min(minRange, pi.Data.PlotRangeStart);
				maxRange = Math.Max(maxRange, pi.Data.PlotRangeEnd);
				result = true;
			}
			return result;
		}

		public void PlotItem_Open()
		{
			var selNodes = PlotItemsSelected;

			if (selNodes.Length == 1)
				EhView_ContentsDoubleClick(selNodes[0]);
		}

		public bool PlotItems_CanDelete()
		{
			var anySelected = null != _plotItemsRootNode.TakeFromHereToFirstLeaves(false).Where(node => node.IsSelected).FirstOrDefault();
			return anySelected;
		}

		public void PlotItems_Delete()
		{
			var selNodes = PlotItemsSelected;
			foreach (var node in selNodes)
				node.Remove();
		}

		public void PlotItems_Copy()
		{
			var selNodes = PlotItemsSelected;

			PlotItemCollection coll = new PlotItemCollection();

			PutSelectedPlotItemsToTemporaryDocumentForClipboard(selNodes, coll);

			ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.Gdi.Plot.PlotItemCollection.AsXml", coll);
		}

		public void PlotItems_Cut()
		{
			var selNodes = PlotItemsSelected;
			this.PlotItems_Copy();
			foreach (var node in selNodes)
				node.Remove();
		}

		public bool PlotItems_CanPaste()
		{
			object o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Gdi.Plot.PlotItemCollection.AsXml");
			PlotItemCollection coll = o as PlotItemCollection;
			return null != coll;
		}

		public void PlotItems_Paste()
		{
			object o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Gdi.Plot.PlotItemCollection.AsXml");
			PlotItemCollection coll = o as PlotItemCollection;
			// if at this point obj is a memory stream, you probably have forgotten the deserialization constructor of the class you expect to deserialize here
			if (null != coll)
			{
				/*
				foreach (IGPlotItem item in coll)
          {
              _doc.Add(item);
              NGTreeNode newNode = new NGTreeNode();
              newNode.Text = item.GetName(2);
              newNode.Tag = item;
              _plotItemsTree.Add(newNode);
          }
				*/

				foreach (IGPlotItem item in coll)
				{
					_doc.Add(item); // it is formally neccessary to add the plot items to the doc, since some functions only work when the parent layer of the items is available
				}

				PlotItemsToTree(_plotItemsRootNode, coll);

				_view.InitializePlotItems(_plotItemsTree);
				SetDirty();
			}
		}

		#endregion ILineScatterLayerContentsController Members

		#region IApplyController Members

		public bool Apply()
		{
			//			if (!this._isDirty)
			//			return true; // not dirty - so no need to apply something

			_originalDoc.ClearPlotItems(); // first, clear all Plot items
			TreeToPlotItems(_plotItemsRootNode, _originalDoc);

			if (_useDocument == UseDocument.Copy)
				_doc = _originalDoc.Clone();
			Initialize(true); // Reload the applied contents to make sure it is synchronized

			return true; // all ok
		}

		#endregion IApplyController Members

		#region Drag/drop support

		#region Plot items

		private static bool AreAllNodesFromSameLevel(IEnumerable<NGTreeNode> selNodes)
		{
			int? level = null;
			foreach (var node in selNodes)
			{
				if (null == level)
					level = node.Level;

				if (level != node.Level)
					return false;
			}
			return null == level ? false : true;
		}

		public bool PlotItems_CanStartDrag(IEnumerable items)
		{
			return NGTreeNode.AreAllNodesFromSameLevel(items.OfType<NGTreeNode>());
		}

		public void PlotItems_StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove)
		{
			data = new List<NGTreeNode>(items.OfType<NGTreeNode>());
			canCopy = true;
			canMove = true;
		}

		public void PlotItems_DragEnded(bool isCopy, bool isMove)
		{
		}

		public void PlotItems_DragCancelled()
		{
		}

		public void PlotItems_DropCanAcceptData(object data, NGTreeNode targetItem, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool canCopy, out bool canMove, out bool itemIsSwallowingData)
		{
			if (!(data is IEnumerable<NGTreeNode>))
			{
				canCopy = false;
				canMove = false;
				itemIsSwallowingData = false;
				return;
			}

			if (targetItem != null && targetItem.Tag is PlotItemCollection)
			{
				canCopy = true;
				canMove = true;
				itemIsSwallowingData = true;
			}
			else
			{
				canCopy = true;
				canMove = true;
				itemIsSwallowingData = false;
			}
		}

		public void PlotItems_Drop(object data, NGTreeNode targetNode, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove)
		{
			isMove = false;
			isCopy = false;

			bool canTargetSwallowNodes = null != targetNode && targetNode.Tag is PlotItemCollection;

			Action<NGTreeNode> AddNodeToTree;

			int actualInsertIndex; // is updated every time the following delegate is called
			NGTreeNodeCollection parentNodeCollectionOfTargetNode = null;
			if (canTargetSwallowNodes)
			{
				AddNodeToTree = node => targetNode.Nodes.Add(node);
			}
			else if (targetNode == null)
			{
				AddNodeToTree = node => _plotItemsRootNode.Nodes.Add(node);
			}
			else // Add as sibling of the target node
			{
				int idx = targetNode.Index;
				if (idx < 0) // No parent node,
				{
					AddNodeToTree = node => targetNode.Nodes.Add(node);
				}
				else
				{
					if (insertPosition.HasFlag(Gui.Common.DragDropRelativeInsertPosition.AfterTargetItem))
						idx = targetNode.Index + 1;

					actualInsertIndex = idx;
					parentNodeCollectionOfTargetNode = targetNode.ParentNode.Nodes;
					AddNodeToTree = node => parentNodeCollectionOfTargetNode.Insert(actualInsertIndex++, node); // the incrementation is to support dropping of multiple items, they must be dropped at increasing indices
				}
			}

			if (data is IEnumerable<NGTreeNode>)
			{
				var dummyNodes = new List<NGTreeNode>();
				foreach (var node in (IEnumerable<NGTreeNode>)data)
				{
					if (node.Tag is Altaxo.Data.DataColumn)
					{
						isMove = false;
						isCopy = true;

						var newNode = CreatePlotItemNode((Altaxo.Data.DataColumn)node.Tag);
						AddNodeToTree(newNode);
					}
					else if (node.Tag is IGPlotItem)
					{
						isMove = true;
						isCopy = false;

						var dummyNode = new NGTreeNode();
						dummyNodes.Add(dummyNode);

						node.ReplaceBy(dummyNode); // instead of removing the old node from the tree, we replace it by a dummy. In this way we retain the position of all nodes in the tree, so that the insert index is valid during the whole drop operation
						AddNodeToTree(node);
					}
				}
				// now, after the drop is complete, we can remove the dummy nodes
				foreach (var dummy in dummyNodes)
					dummy.Remove();
			}
		}

		#endregion Plot items

		#region Available items

		public bool AvailableItems_CanStartDrag(IEnumerable items)
		{
			var selNodes = items.OfType<NGTreeNode>();
			var selNotAllowedNodes = selNodes.Where(node => !(node.Tag is Altaxo.Data.DataColumn));

			var isAnythingSelected = selNodes.FirstOrDefault() != null;
			var isAnythingForbiddenSelected = selNotAllowedNodes.FirstOrDefault() != null;

			// to start a drag, all selected nodes must be on the same level
			return isAnythingSelected && !isAnythingForbiddenSelected;
		}

		public void AvailableItems_StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove)
		{
			data = new List<NGTreeNode>(items.OfType<NGTreeNode>().Where(node => (node.IsSelected && node.Tag is Altaxo.Data.DataColumn)));
			canCopy = true;
			canMove = false;
		}

		public void AvailableItems_DragEnded(bool isCopy, bool isMove)
		{
		}

		public void AvailableItems_DragCancelled()
		{
		}

		#endregion Available items

		#endregion Drag/drop support

		#region IMVCController Members

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
					_view.Controller = null;
				}

				_view = value as ILineScatterLayerContentsView;

				if (null != _view)
				{
					Initialize(false); // set only the view elements, dont't initialize the variables
					_view.Controller = this;
				}
			}
		}

		public object ModelObject
		{
			get { return this._originalDoc; }
		}

		public void Dispose()
		{
		}

		#endregion IMVCController Members
	}
}