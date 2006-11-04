﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1584 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Description of ExtTreeView.
	/// </summary>
	public class ExtTreeView : TreeView
	{
		Dictionary<string, int> imageIndexTable = new Dictionary<string, int>();
		List<ExtTreeNode> cutNodes = new List<ExtTreeNode>();
		bool isSorted = true;
		
		/// <summary>
		/// Gets/Sets whether the ExtTreeView does its own sorting.
		/// </summary>
		public bool IsSorted {
			get {
				return isSorted;
			}
			set {
				isSorted = value;
			}
		}
		
		[Obsolete("Use IsSorted instead!")]
		public new bool Sorted {
			get {
				return base.Sorted;
			}
			set {
				base.Sorted = value;
			}
		}

		public List<ExtTreeNode> CutNodes {
			get {
				return cutNodes;
			}
		}
		
		// using TreeView.TreeViewNodeSorter will result in TreeNodeCollection
		// calling Sort() after every insertion. Therefore, we have to create
		// our own NodeSorter property.
		IComparer<TreeNode> nodeSorter = new ExtTreeViewComparer();

		public IComparer<TreeNode> NodeSorter {
			get {
				return nodeSorter;
			}
			set {
				nodeSorter = value;
			}
		}
		
		[Obsolete("Use NodeSorter instead!")]
		public new System.Collections.IComparer TreeViewNodeSorter {
			get {
				return base.TreeViewNodeSorter;
			}
			set {
				base.TreeViewNodeSorter = value;
			}
		}
		
		public ExtTreeView()
		{
			DrawMode      = TreeViewDrawMode.OwnerDrawText;
			HideSelection = false;
			AllowDrop     = true;
			ImageList newImageList = new ImageList();
			newImageList.ImageSize = new Size(16, 16);
			newImageList.ColorDepth = ColorDepth.Depth32Bit;
			this.ImageList = newImageList;
		}

		public new void Sort()
		{
			SortNodes(Nodes, true);
		}

		public void SortNodes(TreeNodeCollection nodes, bool recursive)
		{
			if (!isSorted) {
				return;
			}
			TreeNode[] nodeArray = new TreeNode[nodes.Count];
			nodes.CopyTo(nodeArray, 0);
			Array.Sort(nodeArray, nodeSorter);
			nodes.Clear();
			nodes.AddRange(nodeArray);

			if (recursive) {
				foreach (TreeNode childNode in nodeArray) {
					SortNodes(childNode.Nodes, true);
				}
			}
		}
		
		public void ClearCutNodes()
		{
			foreach (ExtTreeNode node in CutNodes) {
				node.DoPerformCut = false;
			}
			CutNodes.Clear();
		}
		
		public void Clear()
		{
			if (this.IsDisposed) {
				return;
			}
			TreeNode[] nodeArray = new TreeNode[Nodes.Count];
			Nodes.CopyTo(nodeArray, 0);
			Nodes.Clear();
			foreach (TreeNode node in nodeArray) {
				if (node is IDisposable) {
					((IDisposable)node).Dispose();
				}
			}
		}
		
		#region label editing
		
		public void StartLabelEdit(ExtTreeNode node)
		{
			if (node == null) {
				return;
			}
			
			if (node.CanLabelEdit) {
				node.EnsureVisible();
				SelectedNode = node;
				LabelEdit = true;
				node.BeforeLabelEdit();
				node.BeginEdit();
			}
		}
		
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (SelectedNode == null || !SelectedNode.IsEditing) {
				switch (keyData) {
					case Keys.F2:
						StartLabelEdit(SelectedNode as ExtTreeNode);
						break;
					case Keys.Delete:
						DeleteNode(SelectedNode as ExtTreeNode);
						break;
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		
		protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
		{
			base.OnAfterLabelEdit(e);
			LabelEdit    = false;
			e.CancelEdit = true;
			
			ExtTreeNode node = e.Node as ExtTreeNode;
			if (node != null && e.Label != null) {
				node.AfterLabelEdit(e.Label);
			}
			SortParentNodes(e.Node);
		}

		private void SortParentNodes(TreeNode treeNode)
		{
			TreeNode parent = treeNode.Parent;
			SortNodes((parent == null) ? Nodes : parent.Nodes, false);
		}
		#endregion
		bool inRefresh;
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			if (mouseClickNum == 2) {
				mouseClickNum = 0; // only intercept first occurrance, don't prevent expansion by ActivateItem on double click
				e.Cancel = true;
				return;
			}
			base.OnBeforeExpand(e);
			if (e.Node == null) {
				return;
			}
			try {
				if (e.Node is ExtTreeNode) {
					if (((ExtTreeNode)e.Node).IsInitialized == false) {
						if (!inRefresh) {
							inRefresh = true;
							BeginUpdate();
						}
					}
					((ExtTreeNode)e.Node).Expanding();
				}
				if (inRefresh) {
					SortNodes(e.Node.Nodes, false);
				}
			} catch (Exception ex) {
				// catch error to prevent corrupting the TreeView component
				MessageService.ShowError(ex);
			}
			if (e.Node.Nodes.Count == 0) {
				// when the node's subnodes have been removed by Expanding, AfterExpand is not called
				if (inRefresh) {
					inRefresh = false;
					EndUpdate();
				}
			}
		}
		
		protected override void OnAfterExpand(TreeViewEventArgs e)
		{
			base.OnAfterExpand(e);
			if (inRefresh) {
				inRefresh = false;
				EndUpdate();
			}
		}
		
		protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
		{
			if (mouseClickNum == 2) {
				mouseClickNum = 0; // only intercept first occurrance, don't prevent collapsing by ActivateItem on double click
				e.Cancel = true;
				return;
			}
			base.OnBeforeCollapse(e);
			if (e.Node is ExtTreeNode) {
				((ExtTreeNode)e.Node).Collapsing();
			}
		}
		
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			if (e.KeyChar == '\r') {
				ExtTreeNode node = SelectedNode as ExtTreeNode;
				if (node != null) {
					node.ActivateItem();
				}
				e.Handled = true;
			}
		}
		
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			ExtTreeNode node = GetNodeAt(e.Location) as ExtTreeNode;
			if (node != null) {
				node.ActivateItem();
			}
		}
		
		bool canClearSelection = true;
		
		/// <summary>
		/// Gets/Sets whether the user can clear the selection by clicking in the empty area.
		/// </summary>
		public bool CanClearSelection {
			get {
				return canClearSelection;
			}
			set {
				canClearSelection = value;
			}
		}
		
		int mouseClickNum; // 0 if mouse button is not pressed, otherwise click number (1=normal, 2=double click)
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			mouseClickNum = e.Clicks;
			base.OnMouseDown(e);
			TreeNode node = GetNodeAt(e.X, e.Y);
			if (node != null) {
				if (SelectedNode != node) {
					SelectedNode = node;
				}
			} else {
				if (canClearSelection) {
					SelectedNode = null;
				}
			}
		}
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			mouseClickNum = 0;
			base.OnMouseUp(e);
		}
		
		protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
		{
			// setting the context menu must be done by BeforeSelect because
			// AfterSelect is not called for the selection changes when a node is being deleted.
			base.OnBeforeSelect(e);
			ExtTreeNode node = e.Node as ExtTreeNode;
			if (node != null) {
				node.ContextMenuStrip = MenuService.CreateContextMenu(e.Node, node.ContextmenuAddinTreePath);
			}
		}
		
		protected override void OnAfterCheck(TreeViewEventArgs e)
		{
			base.OnAfterCheck(e);
			ExtTreeNode node = e.Node as ExtTreeNode;
			if (node != null) {
				node.CheckedChanged();
			}
		}
		
		protected override void OnDrawNode(DrawTreeNodeEventArgs e)
		{
			if (!inRefresh) {
				ExtTreeNode node = e.Node as ExtTreeNode;
				if (node != null && !node.DrawDefault) {
					node.Draw(e);
					e.DrawDefault = false;
				} else {
					if ((e.State & (TreeNodeStates.Selected | TreeNodeStates.Focused)) == TreeNodeStates.Selected) {
						// node is selected, but not focussed:
						// HACK: work around TreeView bug in OwnerDrawText mode:
						// overpaint blue selection with the correct gray selection
						e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
						e.Graphics.DrawString(e.Node.Text, this.Font, SystemBrushes.ControlText, e.Bounds.Location);
						e.DrawDefault = false;
					} else {
						e.DrawDefault = true;
					}
				}
			} else {
				e.DrawDefault = false;
			}
			base.OnDrawNode(e);
		}
		
		protected override void OnItemDrag(ItemDragEventArgs e)
		{
			base.OnItemDrag(e);
			ExtTreeNode node = e.Item as ExtTreeNode;
			if (node != null) {
				DataObject dataObject = node.DragDropDataObject;
				if (dataObject != null) {
					DoDragDrop(dataObject, DragDropEffects.All);
					SortParentNodes(node);
				}
			}
		}
		
		protected override void OnDragEnter(DragEventArgs e)
		{
			base.OnDragEnter(e);
			e.Effect = DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.None;
		}
		
		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);
			Point       clientcoordinate = PointToClient(new Point(e.X, e.Y));
			ExtTreeNode node             = GetNodeAt(clientcoordinate) as ExtTreeNode;
			
			if (node != null) {
				DragDropEffects effect = DragDropEffects.None;
				
				if ((e.KeyState & 8) > 0) { // CTRL key pressed.
					effect = DragDropEffects.Copy;
				} else {
					effect = DragDropEffects.Move;
				}
				e.Effect = node.GetDragDropEffect(e.Data, effect);
				
				if (e.Effect != DragDropEffects.None) {
					SelectedNode = node;
				}
			}
		}
		
		protected override void OnDragDrop(DragEventArgs e)
		{
			base.OnDragDrop(e);
			Point       clientcoordinate = PointToClient(new Point(e.X, e.Y));
			ExtTreeNode node             = GetNodeAt(clientcoordinate) as ExtTreeNode;
			
			if (node != null) {
				node.DoDragDrop(e.Data, e.Effect);
				SortParentNodes(node);
			}
		}
		
		public int GetImageIndexForImage(string image, bool performCutBitmap)
		{
			string imageKey = performCutBitmap ? (image + "_ghost") : image;
			if (!imageIndexTable.ContainsKey(imageKey)) {
				ImageList.Images.Add(performCutBitmap ? IconService.GetGhostBitmap(image) : IconService.GetBitmap(image));
				imageIndexTable[imageKey] = ImageList.Images.Count - 1;
				return ImageList.Images.Count - 1;
			}
			return imageIndexTable[imageKey];
		}
		
		void DeleteNode(ExtTreeNode node)
		{
			if (node == null) {
				return;
			}
			
			if (node.EnableDelete) {
				node.EnsureVisible();
				SelectedNode = node;
				node.Delete();
			}
		}
		
		#region Static methods for saving/restoring expanded state
		// example ViewStateString:
		// [Main[ICSharpCode.SharpDevelop[Src[Gui[Pads[ProjectBrowser[]]]]Services[]]]]
		// -> every node name is terminated by opening bracket
		// -> only expanded nodes are included in the view state string
		// -> after an opening bracket, an identifier or closing bracket must follow
		// -> after a closing bracket, an identifier or closing bracket must follow
		// -> nodes whose text contains '[' can not be saved
		public static string GetViewStateString(TreeView treeView)
		{
			if (treeView.Nodes.Count == 0) return "";
			StringBuilder b = new StringBuilder();
			WriteViewStateString(b, treeView.Nodes[0]);
			return b.ToString();
		}
		static void WriteViewStateString(StringBuilder b, TreeNode node)
		{
			b.Append('[');
			foreach (TreeNode subNode in node.Nodes) {
				if (subNode.IsExpanded && subNode.Text.IndexOf('[') < 0) {
					b.Append(subNode.Text);
					WriteViewStateString(b, subNode);
				}
			}
			b.Append(']');
		}
		public static void ApplyViewStateString(string viewState, TreeView treeView)
		{
			if (viewState.Length == 0)
				return;
			int i = 0;
			ApplyViewStateString(treeView.Nodes[0], viewState, ref i);
			System.Diagnostics.Debug.Assert(i == viewState.Length - 1);
		}
		static void ApplyViewStateString(TreeNode node, string viewState, ref int pos)
		{
			if (viewState[pos++] != '[')
				throw new ArgumentException("pos must point to '['");
			// expect an identifier or an closing bracket
			while (viewState[pos] != ']') {
				StringBuilder nameBuilder = new StringBuilder();
				char ch;
				while ((ch = viewState[pos++]) != '[') {
					nameBuilder.Append(ch);
				}
				pos -= 1; // go back to '[' character
				string nodeText = nameBuilder.ToString();
				// find the node in question
				TreeNode subNode = null;
				if (node != null) {
					foreach (TreeNode n in node.Nodes) {
						if (n.Text == nodeText) {
							subNode = n;
							break;
						}
					}
				}
				if (subNode != null) {
					subNode.Expand();
				}
				ApplyViewStateString(subNode, viewState, ref pos);
				// pos now points to the closing bracket of the inner view state
				pos += 1; // move to next character
			}
		}
		#endregion
	}
}
