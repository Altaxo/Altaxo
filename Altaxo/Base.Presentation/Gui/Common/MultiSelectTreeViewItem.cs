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
using System.Collections.ObjectModel;

namespace Altaxo.Gui.Common
{
	public class MultiSelectTreeViewItem : TreeViewItem
	{


		public bool AreAllChildsSelected
		{
			get { return (bool)GetValue(AreAllChildsSelectedProperty); }
			set { SetValue(AreAllChildsSelectedProperty, value); }
		}

		// Using a DependencyProperty as the backing store for AreAllChildsSelected.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty AreAllChildsSelectedProperty =
				DependencyProperty.Register("AreAllChildsSelected", typeof(bool), typeof(MultiSelectTreeViewItem), new UIPropertyMetadata(false));



		#region Properties

		/// <summary>
		/// Get the UI Parent Control of this node.
		/// </summary>
		public ItemsControl ParentItemsControl
		{
			get
			{
				return ItemsControl.ItemsControlFromItemContainer(this);
			}
		}

		/// <summary>
		/// Get the MultiSelectTreeView in which this node is hosted in.
		/// Null value means that this node is not hosted into a MultiSelectTreeView control.
		/// </summary>
		public MultiSelectTreeView ParentMultiSelectTreeView
		{
			get
			{
				for (ItemsControl container = this.ParentItemsControl; container != null; container = ItemsControl.ItemsControlFromItemContainer(container))
				{
					MultiSelectTreeView view = container as MultiSelectTreeView;
					if (view != null)
					{
						return view;
					}
				}

				return null;
			}
		}

		/// <summary>
		/// Get the Parent MultiSelectTreeViewItem of this node.
		/// Remark: Null value means that this node is hosted into a control (e.g. MultiSelectTreeView).
		/// </summary>
		public MultiSelectTreeViewItem ParentMultiSelectTreeViewItem
		{
			get
			{
				return (this.ParentItemsControl as MultiSelectTreeViewItem);
			}
		}
		#endregion

		#region Constructors

		static MultiSelectTreeViewItem()
		{
			// DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectTreeViewItem), new FrameworkPropertyMetadata(typeof(MultiSelectTreeViewItem)));
		}
		#endregion

		#region Overrides
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new MultiSelectTreeViewItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is MultiSelectTreeViewItem;
		}


		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
				
			var tw = ParentMultiSelectTreeView;
			if (null!=tw)
			{
				tw.OnViewItemMouseDown(this,e);
			}

			Keyboard.Focus(this); // neccessary because the element needs the keyboard focus in order to position the selection with the keyboard
		}

	



		protected override void OnKeyDown(KeyEventArgs e)
		{
			try
			{
				MultiSelectTreeViewItem itemToSelect = null;

				if (e.Key == Key.Left)
				{
					this.IsExpanded = false;
					e.Handled = true;
				}
				else if (e.Key == Key.Right)
				{
					this.IsExpanded = true;
					e.Handled = true;
				}
				else if (e.Key == Key.Up)
				{
					// In this case we need to select the last child of the last expandend node of
					// - the previous at the same level (if this index node is NOT 0)
					// - the parent node (if this index node is 0)

					int currentNodeIndex = this.ParentItemsControl.ItemContainerGenerator.IndexFromContainer(this);

					if (currentNodeIndex == 0)
					{
						itemToSelect = this.ParentMultiSelectTreeViewItem;
					}
					else
					{
						MultiSelectTreeViewItem tmp = null;
						tmp = GetPreviousNodeAtSameLevel(this);
						itemToSelect = GetLastVisibleChildNodeOf(tmp);
					}
					e.Handled = true;
				}
				else if (e.Key == Key.Down)
				{
					// In this case we need to select:
					// - the first child node (if this node is expanded)
					// - the next at the same level (if this not the last child)
					// - the next at the same level of the parent node (if this is the last child)

					if (this.IsExpanded && this.Items.Count > 0)
					{ // Select first Child
						itemToSelect = this.ItemContainerGenerator.ContainerFromIndex(0) as MultiSelectTreeViewItem;
					}
					else
					{
						itemToSelect = GetNextNodeAtSameLevel(this);

						if (itemToSelect == null) // current node has no subsequent node at the same level
						{
							MultiSelectTreeViewItem tmp = this.ParentMultiSelectTreeViewItem;

							while (itemToSelect == null && tmp != null) // searhing for the first parent that has a subsequent node at the same level
							{
								itemToSelect = GetNextNodeAtSameLevel(tmp);
								tmp = tmp.ParentMultiSelectTreeViewItem;
							}

						}
					}
					e.Handled = true;
				}

				if (itemToSelect != null)
				{
					itemToSelect.Focus();
					itemToSelect.IsSelected = true;
					ParentMultiSelectTreeView.OnItemClicked(itemToSelect);
				}
			}
			catch (Exception) { /* Silently ignore */ }
		}


		protected override void OnSelected(RoutedEventArgs e)
		{
			var parent = ParentMultiSelectTreeView;
			if(null!=parent)
				parent.OnSelectionStateChanged(this, true);

			base.OnSelected(e);
		}

		protected override void OnUnselected(RoutedEventArgs e)
		{
			var parent = ParentMultiSelectTreeView;
			if(null!=parent)
				parent.OnSelectionStateChanged(this, false);

			base.OnUnselected(e);
		}


		#endregion

		#region Methods
		/// <summary>
		/// Retrieve the last displayed child node of the given one.
		/// </summary>
		/// <param name="item">The node starting with you want to retrieve the last visible node.</param>
		/// <returns>The last child node that is displayed, or the node itself in case it is not expanded.</returns>
		public static MultiSelectTreeViewItem GetLastVisibleChildNodeOf(MultiSelectTreeViewItem item)
		{
			MultiSelectTreeViewItem lastVisibleNode = item;

			// Retrieving last child of last expanded node
			while (lastVisibleNode != null && lastVisibleNode.Items.Count > 0 && lastVisibleNode.IsExpanded)
				lastVisibleNode = lastVisibleNode.ItemContainerGenerator.ContainerFromIndex(lastVisibleNode.Items.Count - 1) as MultiSelectTreeViewItem;

			return lastVisibleNode;
		}

		/// <summary>
		/// Retrieve the previous node that is at the same level.
		/// </summary>
		/// <param name="item">The node starting with you want to retrieve the previous one.</param>
		/// <returns>Null if there is no previous node at the same level.</returns>
		public static MultiSelectTreeViewItem GetPreviousNodeAtSameLevel(MultiSelectTreeViewItem item)
		{
			if (item == null)
				return null;

			MultiSelectTreeViewItem previousNodeAtSameLevel = null;

			ItemsControl parentControl = item.ParentItemsControl;
			if (parentControl != null)
			{
				int index = parentControl.ItemContainerGenerator.IndexFromContainer(item);
				if (index != 0) // if this is not the last item
				{
					previousNodeAtSameLevel = parentControl.ItemContainerGenerator.ContainerFromIndex(index - 1) as MultiSelectTreeViewItem;
				}
			}

			return previousNodeAtSameLevel;
		}

		/// <summary>
		/// Retrieve the subsequent node that is at the same level.
		/// </summary>
		/// <param name="item">The node starting with you want to retrieve the subsequent one.</param>
		/// <returns>Null if there is no subsequent node at the same level.</returns>
		public static MultiSelectTreeViewItem GetNextNodeAtSameLevel(MultiSelectTreeViewItem item)
		{
			if (item == null)
				return null;

			MultiSelectTreeViewItem nextNodeAtSameLevel = null;

			ItemsControl parentControl = item.ParentItemsControl;
			if (parentControl != null)
			{
				int index = parentControl.ItemContainerGenerator.IndexFromContainer(item);
				if (index != parentControl.Items.Count - 1) // if this is not the last item
				{
					nextNodeAtSameLevel = parentControl.ItemContainerGenerator.ContainerFromIndex(index + 1) as MultiSelectTreeViewItem;
				}
			}

			return nextNodeAtSameLevel;
		}

		/// <summary>
		/// Select child nodes of a parent node between two indices.
		/// </summary>
		/// <param name="parent">The parent of the nodes to select.</param>
		/// <param name="idx1">Index of the first child node to select.</param>
		/// <param name="idx2">Index of the second child node to select.</param>
		public static void SelectAllChildNodesBetweenIndices(ItemsControl parent, int idx1, int idx2)
		{
			for (int i = idx1; i <= idx2; ++i)
			{
				var node = parent.ItemContainerGenerator.ContainerFromIndex(i) as MultiSelectTreeViewItem;
				if (null != node)
				{
					node.IsSelected = true;
				}
			}
		}

		public static void SelectAllChildNodesBetweenIndexAndEnd(ItemsControl parent, int idx)
		{
			SelectAllChildNodesBetweenIndices(parent, idx, parent.Items.Count - 1);
		}

		public static void SelectAllChildNodesBetweenStartAndIndex(ItemsControl parent, int idx)
		{
			SelectAllChildNodesBetweenIndices(parent, 0, idx);
		}

		/// <summary>
		/// Select all nodes between the two nodes given in the arguments. The two nodes might be given in arbitrary order and at arbitrary levels.
		/// </summary>
		/// <param name="item1">One node.</param>
		/// <param name="item2">The other node.</param>
		/// <param name="includingIndirectChildNodes"></param>
		public static void SelectAllNodesInbetween(MultiSelectTreeViewItem item1, MultiSelectTreeViewItem item2, bool includingIndirectChildNodes)
		{
			int level1 = item1.GetDepth();
			int level2 = item2.GetDepth();

			// swap item1 and item2, so that item2 is the item with the higher level
			if (level1 > level2)
			{
				var hi = item1;
				var hl = level1;
				item1 = item2;
				level1 = level2;
				item2 = hi;
				level2 = hl;
			}

			// get the parent node of item2, that is on the same level than item1
			var item2OnLevel1 = item2; // 
			for (int l = level2 - 1; l >= level1; --l)
				item2OnLevel1 = (MultiSelectTreeViewItem)item2OnLevel1.ParentItemsControl;

			// to determine which node is the first node and which the last node, find
			// the common anchestor of both nodes

			var it1 = item1;
			var it2 = item2OnLevel1;
			while (it1.ParentItemsControl != it2.ParentItemsControl)
			{
				it1 = it1.ParentItemsControl as MultiSelectTreeViewItem;
				it2 = it2.ParentItemsControl as MultiSelectTreeViewItem;
			}

			// now we can determine which node is first and which last
			// if both are identical, then item1 is the first item, since
			// parent nodes comes first compared to child nodes


			var commonParent = it1.ParentItemsControl;
			int idx1 = commonParent.ItemContainerGenerator.IndexFromContainer(it1);
			int idx2 = commonParent.ItemContainerGenerator.IndexFromContainer(it2);
			bool item1IsFirst = idx1 <= idx2;

			// Swap so that item1 gets the first item
			if (!item1IsFirst)
			{
				var hi = item1;
				var hl = level1;
				var hh = it1;
				var hx = idx1;

				item1 = item2;
				level1 = level2;
				it1 = it2;
				idx1 = idx2;

				item2 = hi;
				level2 = hl;
				it2 = hh;
				idx2 = hx;
			}


			// if item1 and item2 share the same anchestor, select from item1 to items
			if (item1.ParentItemsControl == commonParent && item2.ParentItemsControl == commonParent)
			{
				SelectAllChildNodesBetweenIndices(commonParent, idx1, idx2);
			}
			else
			{
				// first go down to the common anchestor, and then up
				var current = item1;
				current.IsSelected = true;
				var parent = current.ParentItemsControl;
				while (parent != commonParent)
				{
					int idx = parent.ItemContainerGenerator.IndexFromContainer(current);
					SelectAllChildNodesBetweenIndexAndEnd(parent, idx + 1);
					current = parent as MultiSelectTreeViewItem;
					parent = current.ParentItemsControl;
				}

				// now select all items between idx1+1 and idx2-1 (including all childs)
				SelectAllChildNodesBetweenIndices(commonParent, idx1 + 1, idx2 - 1);

				// last select from item2 downwards to the common anchestor
				current = item2;
				parent = current.ParentItemsControl;
				while (parent != commonParent)
				{
					int idx = parent.ItemContainerGenerator.IndexFromContainer(current);
					SelectAllChildNodesBetweenStartAndIndex(parent, idx);
					current = parent as MultiSelectTreeViewItem;
					parent = current.ParentItemsControl;
				}
				current.IsSelected = true;

			}

		}

		/// <summary>
		/// Unselect all children recursively, and finally, also this node.
		/// </summary>
		public void UnselectAllChildren()
		{
			int count = Items.Count;
			for (int i = 0; i < count; ++i)
			{
				var child = (MultiSelectTreeViewItem)this.ItemContainerGenerator.ContainerFromIndex(i);
				child.UnselectAllChildren();
			}

			if (this.IsSelected)
			{
				this.IsSelected = false;
				//ParentMultiSelectTreeView.OnSelectionChanges(this);
			}
		}

		/// <summary>
		/// Select select all children recursively, and finally, also this node.
		/// </summary>
		public void SelectAllChildren()
		{
			if (this.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
			{
				int count = Items.Count;
				for (int i = 0; i < count; ++i)
				{
					var child = (MultiSelectTreeViewItem)this.ItemContainerGenerator.ContainerFromIndex(i);
					child.SelectAllChildren();
				}
			}


			if (!this.IsSelected)
			{
				this.IsSelected = true;
				//ParentMultiSelectTreeView.OnSelectionChanges(this);
			}
		}

		/// <summary>
		/// Get the node depth. The one or more root nodes have a node depth of zero by definition.
		/// </summary>
		/// <returns>The node depth of the node.</returns>
		public int GetDepth()
		{
			var parent = this.ParentMultiSelectTreeViewItem;
			int result = 0;
			while (parent != null)
			{
				++result;
				parent = parent.ParentMultiSelectTreeViewItem;
			}

			return result;
		}
		#endregion
	}

}
