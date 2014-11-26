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
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for XYPlotLayerContentsControl.xaml
	/// </summary>
	public partial class XYPlotLayerContentsControl : UserControl, ILineScatterLayerContentsView
	{
		private ILineScatterLayerContentsViewEventSink _controller;

		public XYPlotLayerContentsControl()
		{
			InitializeComponent();
		}

		private void EhCommand_CopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _guiPlotItemsTree.SelectedItems.Count > 0;
			e.Handled = true;
			object o = Keyboard.FocusedElement;
		}

		private void EhCommand_CopyExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Controller.EhView_CopyClipboard(SelectedNodes(this._guiPlotItemsTree));
			e.Handled = true;
		}

		private void EhCommand_PasteCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Controller.EhView_CanPasteFromClipboard();
			e.Handled = true;
		}

		private void EhCommand_PasteExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Controller.EhView_PasteClipboard();
			e.Handled = true;
		}

		private NGTreeNode[] SelectedNodes(Altaxo.Gui.Common.MultiSelectTreeView tree)
		{
			var result = tree.SelectedItems.OfType<NGTreeNode>().ToArray();
			NGTreeNode.SortByOrder(result);
			return result;
		}

		private void EhPutData_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_PutData(SelectedNodes(this._guiAvailableContent));
				this._guiPlotItemsTree.Focus();
			}
		}

		private void EhPullData_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_PullDataClick(SelectedNodes(this._guiPlotItemsTree));
			}
		}

		private void EhListSelUp_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_ListSelUpClick(SelectedNodes(this._guiPlotItemsTree));
				this._guiPlotItemsTree.Focus();
			}
		}

		private void EhListSelDown_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_SelDownClick(SelectedNodes(this._guiPlotItemsTree));
				this._guiPlotItemsTree.Focus();
			}
		}

		private void EhPlotAssociations_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_PlotAssociationsClick(SelectedNodes(this._guiPlotItemsTree));
				this._guiPlotItemsTree.Focus();
			}
		}

		private void EhGroup_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_GroupClick(SelectedNodes(this._guiPlotItemsTree));
				this._guiPlotItemsTree.Focus();
			}
		}

		private void EhUngroup_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_UngroupClick(SelectedNodes(this._guiPlotItemsTree));
				this._guiPlotItemsTree.Focus();
			}
		}

		private void EhEditRange_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
			{
				Controller.EhView_EditRangeClick(SelectedNodes(this._guiPlotItemsTree));
				this._guiPlotItemsTree.Focus();
			}
		}

		private void EhShowRange_CheckedChanged(object sender, RoutedEventArgs e)
		{
		}

		#region ILineScatterLayerContentsView

		public ILineScatterLayerContentsViewEventSink Controller
		{
			get
			{
				return _controller;
			}
			set
			{
				_controller = value;
			}
		}

		private Collections.NGTreeNodeCollection _dataAvailable;

		public void DataAvailable_Initialize(Collections.NGTreeNodeCollection nodes)
		{
			var oldItems = _dataAvailable;
			_dataAvailable = nodes;
			if (oldItems != _dataAvailable)
				_guiAvailableContent.ItemsSource = _dataAvailable;
		}

		public void DataAvailable_ClearSelection()
		{
			if (null != _dataAvailable)
				foreach (var n in _dataAvailable)
					n.ClearSelectionRecursively();
		}

		private Collections.NGTreeNodeCollection _layerContents;

		public void Contents_SetItems(Collections.NGTreeNodeCollection items)
		{
			var oldItems = _layerContents;
			_layerContents = items;
			if (oldItems != _layerContents)
				_guiPlotItemsTree.ItemsSource = _layerContents;
		}

		public void Contents_RemoveItems(Collections.NGTreeNode[] items)
		{
			foreach (NGTreeNode node in items)
				node.Remove();
		}

		public void Contents_SetSelected(int idx, bool bSelected)
		{
		}

		public void Contents_InvalidateItems(int idx1, int idx2)
		{
		}

		public object ControllerObject
		{
			get
			{
				return _controller;
			}
			set
			{
				_controller = value as ILineScatterLayerContentsViewEventSink;
			}
		}

		#endregion ILineScatterLayerContentsView

		private void EhItemMouseDoubleClick(object sender, EventArgs e)
		{
			if (null != Controller)
			{
				if (this._guiPlotItemsTree.SelectedItems.Count == 1)
				{
					Controller.EhView_ContentsDoubleClick(_guiPlotItemsTree.SelectedItems.First() as NGTreeNode);
				}
				this._guiPlotItemsTree.Focus();
			}
		}

		#region PlotItemTreeView Drag drop support

		private IDragSource _plotItemTreeDragHandler;

		public IDragSource PlotItemTreeDragHandler
		{
			get
			{
				if (null == _plotItemTreeDragHandler)
					_plotItemTreeDragHandler = new PlotItemTree_DragHandler(this);
				return _plotItemTreeDragHandler;
			}
		}

		public class PlotItemTree_DragHandler : IDragSource
		{
			private XYPlotLayerContentsControl _projectBrowseControl;

			public PlotItemTree_DragHandler(XYPlotLayerContentsControl ctrl)
			{
				_projectBrowseControl = ctrl;
			}

			public bool CanStartDrag(IDragInfo dragInfo)
			{
				return _projectBrowseControl._controller.PlotItemTree_CanStartDrag();
			}

			public void StartDrag(IDragInfo dragInfo)
			{
				object data;
				bool canCopy, canMove;
				_projectBrowseControl._controller.PlotItemTree_StartDrag(out data, out canCopy, out canMove);

				dragInfo.Effects = GuiHelper.ConvertCopyMoveToDragDropEffect(canCopy, canMove);
				dragInfo.Data = data;
			}

			public void Dropped(IDropInfo dropInfo, DragDropEffects effects)
			{
				bool isCopy, isMove;
				GuiHelper.ConvertDragDropEffectToCopyMove(effects, out isCopy, out isMove);

				_projectBrowseControl._controller.PlotItemTree_DragEnded(isCopy, isMove);
			}

			public void DragCancelled()
			{
				_projectBrowseControl._controller.PlotItemTree_DragCancelled();
			}
		}

		private IDropTarget _treeViewDropHandler;

		public IDropTarget PlotItemTreeDropHandler
		{
			get
			{
				if (null == _treeViewDropHandler)
					_treeViewDropHandler = new PlotItemTree_DropHandler(this);
				return _treeViewDropHandler;
			}
		}

		public class PlotItemTree_DropHandler : IDropTarget
		{
			private XYPlotLayerContentsControl _projectBrowseControl;

			public PlotItemTree_DropHandler(XYPlotLayerContentsControl ctrl)
			{
				_projectBrowseControl = ctrl;
			}

			public void DragOver(IDropInfo dropInfo)
			{
				DragDropEffects resultingEffect;
				Type adornerType;
				if (CanAcceptData(dropInfo, out resultingEffect, out adornerType))
				{
					dropInfo.Effects = resultingEffect;
					dropInfo.DropTargetAdorner = adornerType;
				}
			}

			protected bool CanAcceptData(IDropInfo dropInfo, out System.Windows.DragDropEffects resultingEffect, out Type adornerType)
			{
				bool canCopy, canMove, itemIsSwallowingData;
				_projectBrowseControl._controller.PlotItemTree_DropCanAcceptData(
					dropInfo.Data is System.Windows.IDataObject ? GuiHelper.ToAltaxo((System.Windows.IDataObject)dropInfo.Data) : dropInfo.Data,
					dropInfo.TargetItem as Altaxo.Collections.NGTreeNode,
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ControlKey),
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ShiftKey),
					out canCopy, out canMove, out itemIsSwallowingData);

				resultingEffect = GuiHelper.ConvertCopyMoveToDragDropEffect(canCopy, canMove);
				adornerType = itemIsSwallowingData ? DropTargetAdorners.Highlight : DropTargetAdorners.Insert;

				return canCopy | canMove;
			}

			public void Drop(IDropInfo dropInfo)
			{
				bool isCopy, isMove;
				_projectBrowseControl._controller.PlotItemTree_Drop(
					dropInfo.Data is System.Windows.IDataObject ? GuiHelper.ToAltaxo((System.Windows.IDataObject)dropInfo.Data) : dropInfo.Data,
					dropInfo.TargetItem as Altaxo.Collections.NGTreeNode,
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ControlKey),
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ShiftKey),
					out isCopy, out isMove);

				dropInfo.Effects = GuiHelper.ConvertCopyMoveToDragDropEffect(isCopy, isMove); // it is important to get back the resulting effect to dropInfo, because dropInfo informs the drag handler about the resulting effect, which can e.g. delete the items after a move operation
			}
		}

		#endregion PlotItemTreeView Drag drop support

		#region AvailableItemTree_DragHander

		private IDragSource _availableItemTreeDragHandler;

		public IDragSource AvailableItemTreeDragHandler
		{
			get
			{
				if (null == _availableItemTreeDragHandler)
					_availableItemTreeDragHandler = new AvailableItemTree_DragHandler(this);
				return _availableItemTreeDragHandler;
			}
		}

		public class AvailableItemTree_DragHandler : IDragSource
		{
			private XYPlotLayerContentsControl _projectBrowseControl;

			public AvailableItemTree_DragHandler(XYPlotLayerContentsControl ctrl)
			{
				_projectBrowseControl = ctrl;
			}

			public bool CanStartDrag(IDragInfo dragInfo)
			{
				return _projectBrowseControl._controller.AvailableItemTree_CanStartDrag();
			}

			public void StartDrag(IDragInfo dragInfo)
			{
				object data;
				bool canCopy, canMove;
				_projectBrowseControl._controller.AvailableItemTree_StartDrag(out data, out canCopy, out canMove);

				dragInfo.Effects = GuiHelper.ConvertCopyMoveToDragDropEffect(canCopy, canMove);
				dragInfo.Data = data;
			}

			public void Dropped(IDropInfo dropInfo, DragDropEffects effects)
			{
				bool isCopy, isMove;
				GuiHelper.ConvertDragDropEffectToCopyMove(effects, out isCopy, out isMove);

				_projectBrowseControl._controller.AvailableItemTree_DragEnded(isCopy, isMove);
			}

			public void DragCancelled()
			{
				_projectBrowseControl._controller.AvailableItemTree_DragCancelled();
			}
		}

		#endregion AvailableItemTree_DragHander
	}
}