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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for MultiSelectTreeView.xaml
	/// </summary>
	public partial class MultiSelectTreeView : System.Windows.Controls.ItemsControl
	{
		public class SelectedItemsCollection : ObservableCollection<MultiSelectTreeViewItem> { }

		private SelectedItemsCollection _selectedTreeViewItems = new SelectedItemsCollection();

		/// <summary>
		/// Fired when a mouse double click on any item occurs.
		/// </summary>
		public event EventHandler ItemMouseDoubleClick;

		public MultiSelectTreeView()
		{
			InitializeComponent();

			var ib = new KeyBinding(ApplicationCommands.Copy, Key.C, ModifierKeys.Control);
			this.InputBindings.Add(ib);
		}

		public enum SelectionModalities
		{
			SingleSelectionOnly,
			MultipleSelectionOnly,
			KeyboardModifiersMode
		}

		#region Properties

		private MultiSelectTreeViewItem _lastClickedItem = null;

		public SelectionModalities SelectionMode
		{
			get { return (SelectionModalities)GetValue(SelectionModeProperty); }
			set { SetValue(SelectionModeProperty, value); }
		}

		public static readonly DependencyProperty SelectionModeProperty =
				DependencyProperty.Register("SelectionMode", typeof(SelectionModalities), typeof(MultiSelectTreeView), new UIPropertyMetadata(SelectionModalities.KeyboardModifiersMode));

		public SelectedItemsCollection SelectedTreeViewItems
		{
			get { return _selectedTreeViewItems; }
		}

		public System.Collections.Generic.ICollection<object> SelectedItems
		{
			get
			{
				var _selectedItems = new Collection<object>();

				foreach (MultiSelectTreeViewItem item in _selectedTreeViewItems)
				{
					object o = item.DataContext;
					if (o.GetType().FullName == "MS.Internal.NamedObject") // TreeViewItems that were removed from the tree contain a DataContect object of type MS.Internal.NamedObject
						continue; // such items are not part of the tree anymore, and thus its datacontext does not belong to the selected objects collection

					_selectedItems.Add(o);
				}

				return _selectedItems;
			}
		}

		#endregion Properties

		#region Constructors

		static MultiSelectTreeView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectTreeView), new FrameworkPropertyMetadata(typeof(MultiSelectTreeView)));

			GongSolutions.Wpf.DragDrop.Utilities.ItemsControlExtensions.RegisterItemsControl<MultiSelectTreeView, MultiSelectTreeViewItem>(
				itemsControl => true,
				itemsControl => itemsControl.SelectedItems,
				(itemsControl, obj) => itemsControl.SelectedItems.Contains(obj),
				(itemsControl, item, isSelected) => ((MultiSelectTreeViewItem)itemsControl.ItemContainerGenerator.ContainerFromItem(item)).IsSelected = isSelected,
				itemsControl => null // GetOrientation returns null because TreeViewItems are neither oriented horizontally nor vertically
				);
		}

		#endregion Constructors

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new MultiSelectTreeViewItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is MultiSelectTreeViewItem;
		}

		protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
		{
			_selectedTreeViewItems.Clear();

			base.OnItemsSourceChanged(oldValue, newValue);
		}

		internal void OnSelectionStateChanged(MultiSelectTreeViewItem viewItem, bool newSelectionState)
		{
			if (true == newSelectionState)
				AddItemToSelection(viewItem);
			else
				RemoveItemFromSelection(viewItem);
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.C && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
			{
				//e.Handled = true;
			}
			base.OnPreviewKeyDown(e);
		}

		private bool _wasShiftPressedOnLastItemMouseDown = false;
		private bool _wasCtrlPressedOnLastItemMouseDown = false;

		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseDown(e);
			_wasShiftPressedOnLastItemMouseDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
			_wasCtrlPressedOnLastItemMouseDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
		}

		internal void OnViewItemMouseDown(MultiSelectTreeViewItem viewItem, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
			{
				bool isSelected = _selectedTreeViewItems.Contains(viewItem);
				if (!isSelected)
					OnItemClicked(viewItem);
				e.Handled = true;
			}
		}

		internal void OnViewItemMouseUp(MultiSelectTreeViewItem viewItem, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
			{
				bool isSelected = _selectedTreeViewItems.Contains(viewItem);
				if (isSelected && !(_wasShiftPressedOnLastItemMouseDown || _wasCtrlPressedOnLastItemMouseDown))
					OnItemClicked(viewItem);
				e.Handled = true;
			}
		}

		internal void OnViewItemMouseDoubleClick(MultiSelectTreeViewItem viewItem, MouseButtonEventArgs e)
		{
			OnItemDoubleClicked(viewItem);
			e.Handled = true;
		}

		/// <summary>
		/// Handels the situation when a item is clicked (either by mouse or by keyboard).
		/// </summary>
		/// <param name="item"></param>
		internal void OnItemClicked(MultiSelectTreeViewItem item)
		{
			if (item != null)
			{
				switch (this.SelectionMode)
				{
					case SelectionModalities.MultipleSelectionOnly:
						ManageCtrlSelection(item);
						break;

					case SelectionModalities.SingleSelectionOnly:
						ManageSingleSelection(item);
						break;

					case SelectionModalities.KeyboardModifiersMode:
						if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
						{
							ManageShiftSelection(item);
						}
						else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
						{
							ManageCtrlSelection(item);
						}
						else
						{
							ManageSingleSelection(item);
						}
						break;
				}
			}
		}

		protected internal void OnItemDoubleClicked(MultiSelectTreeViewItem item)
		{
			if (null != item && null != ItemMouseDoubleClick)
				ItemMouseDoubleClick(this, EventArgs.Empty);
		}

		#region Methods

		public void UnselectAll()
		{
			var selItemsCopy = _selectedTreeViewItems.ToArray(); // use a copy of the collection since IsSelected=false causes the removal of the item from the collection
			foreach (MultiSelectTreeViewItem item in selItemsCopy)
				item.IsSelected = false;

			_selectedTreeViewItems.Clear();
			_lastClickedItem = null;
		}

		public void SelectAll()
		{
			foreach (MultiSelectTreeViewItem item in Items)
				item.SelectAllChildren();
		}

		#endregion Methods

		#region Helper Methods

		private void AddItemToSelection(MultiSelectTreeViewItem newItem)
		{
			if (!_selectedTreeViewItems.Contains(newItem))
			{
				_selectedTreeViewItems.Add(newItem);
			}
		}

		private void RemoveItemFromSelection(MultiSelectTreeViewItem newItem)
		{
			if (_selectedTreeViewItems.Contains(newItem))
				_selectedTreeViewItems.Remove(newItem);
		}

		private void ManageSingleSelection(MultiSelectTreeViewItem viewItem)
		{
			UnselectAll();
			viewItem.IsSelected = true;
			_lastClickedItem = viewItem;
		}

		private void ManageCtrlSelection(MultiSelectTreeViewItem viewItem)
		{
			viewItem.IsSelected = !viewItem.IsSelected;
			_lastClickedItem = viewItem;
		}

		private void ManageShiftSelection(MultiSelectTreeViewItem viewItem)
		{
			if (_lastClickedItem == null)
				_lastClickedItem = Items[0] as MultiSelectTreeViewItem;

			if (_lastClickedItem == null)
				return;

			if (_lastClickedItem.ParentMultiSelectTreeView == null)
				return; // seems to be a disconnected item

			MultiSelectTreeViewItem.SelectAllNodesInbetween(_lastClickedItem, viewItem, true);
		}

		#endregion Helper Methods
	}
}