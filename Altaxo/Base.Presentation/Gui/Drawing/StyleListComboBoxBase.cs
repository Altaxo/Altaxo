#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Altaxo.Gui.Drawing
{
	/// <summary>
	/// Base class to choose items from a style lists. See <see cref="IStyleList{T}"/>
	/// </summary>
	public abstract class StyleListComboBoxBase<TManager, TList, TItem> : UserControl
		where TManager : IStyleListManager<TList, TItem>
		where TList : IStyleList<TItem>
		where TItem : class, Altaxo.Main.IImmutable
	{
		#region Inner classes

		/// <summary>
		/// Special tree node for a style list. This tree node fills its child items only when it gets expanded.
		/// </summary>
		private class NGTreeNodeForTList : NGTreeNode
		{
			private StyleListComboBoxBase<TManager, TList, TItem> _parent;

			public NGTreeNodeForTList(StyleListComboBoxBase<TManager, TList, TItem> parent, TList itemList)
				: base(true)
			{
				_parent = parent;
				_tag = itemList;
				_text = itemList.Name;
			}

			protected override void LoadChildren()
			{
				base.LoadChildren();

				foreach (var c in (TList)_tag)
				{
					Nodes.Add(new NGTreeNode() { Text = _parent.GetDisplayName(c), Tag = c });
				}
			}
		}

		/// <summary>
		/// Selects the data template for the TreeView: either for a <see cref="TItem" />, for a <see cref="TList"/> or for another node.
		/// </summary>
		public class TreeViewDataTemplateSelector : DataTemplateSelector
		{
			private FrameworkElement _parent;
			private DataTemplate _TItemTemplate;
			private DataTemplate _TListTemplate;
			private DataTemplate _treeOtherTemplate;

			public TreeViewDataTemplateSelector(FrameworkElement ele)
			{
				_parent = ele;
			}

			public override DataTemplate SelectTemplate(object item, DependencyObject container)
			{
				NGTreeNode node = item as NGTreeNode;
				if (node != null)
				{
					if (node.Tag is TItem)
					{
						if (null == _TItemTemplate)
							_TItemTemplate = (DataTemplate)_parent.TryFindResource("TItemTemplate");
						if (null != _TItemTemplate)
							return _TItemTemplate;
					}
					else if (node.Tag is TList)
					{
						if (null == _TListTemplate)
							_TListTemplate = (DataTemplate)_parent.TryFindResource("TListTemplate");
						if (null != _TListTemplate)
							return _TListTemplate;
					}
					else
					{
						if (null == _treeOtherTemplate)
							_treeOtherTemplate = (DataTemplate)_parent.TryFindResource("TreeOtherTemplate");
						if (null != _treeOtherTemplate)
							return _treeOtherTemplate;
					}
				}

				return base.SelectTemplate(item, container);
			}
		}

		#endregion Inner classes

		/// <summary>Maximum number of items shown under "last used items".</summary>
		protected const int MaxNumberOfLastLocalUsedItems = 5;

		public static readonly DependencyProperty SelectedItemProperty;

		/// <summary>Gets access to the tree view, which shows the style lists.</summary>
		protected abstract TreeView GuiTreeView { get; }

		/// <summary>Gets access to the ComboBox, which shows the items of the current style list.</summary>
		protected abstract ComboBox GuiComboBox { get; }

		/// <summary>
		/// Gets or sets the selected item internally.
		/// </summary>
		/// <value>
		/// The item to get/set;
		/// </value>
		protected virtual TItem InternalSelectedItem { get { return SelectedItem; } set { SelectedItem = value; } }

		protected List<TItem> _lastLocalUsedItems = new List<TItem>();

		public event DependencyPropertyChangedEventHandler SelectedItemChanged;

		protected Action _viewEvent_SelectedItemChanged;

		/// <summary>
		/// The item list manager used here.
		/// </summary>
		protected TManager _styleListManager;

		/// <summary>
		/// Temporary storage for the selected value of the TreeView when the Popup is opened
		/// </summary>
		protected object _selectedFromTreeView;

		/// <summary>Data items for TreeView</summary>
		protected NGTreeNode _treeRootNode = new NGTreeNode();

		/// <summary>Filter string to filter the items in the ComboBox. The item names must start with the string stored here.</summary>
		protected string _filterString = string.Empty;

		/// <summary>Property that describes whether the TreeView dropdown is open.</summary>
		public static readonly DependencyProperty IsTreeDropDownOpenProperty;

		#region Constructors

		static StyleListComboBoxBase()
		{
			SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(TItem), typeof(StyleListComboBoxBase<TManager, TList, TItem>), new FrameworkPropertyMetadata(null, EhSelectedItemChanged, EhSelectedItemCoerce));
			IsTreeDropDownOpenProperty = DependencyProperty.Register(nameof(IsTreeDropDownOpen), typeof(bool), typeof(StyleListComboBoxBase<TManager, TList, TItem>), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(EhIsTreeDropDownOpenChanged)));
		}

		protected StyleListComboBoxBase(TManager manager)
		{
			_styleListManager = manager;
			_styleListManager.Changed += EhColorSetManager_Changed;
		}

		#endregion Constructors

		#region Helpers

		public virtual string GetDisplayName(TItem item)
		{
			return item.ToString();
		}

		#endregion Helpers

		#region Dependency property SelectedItem

		public TItem SelectedItem
		{
			get { return (TItem)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		private static void EhSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((StyleListComboBoxBase<TManager, TList, TItem>)obj).OnSelectedItemChanged(obj, args);
		}

		private static object EhSelectedItemCoerce(DependencyObject obj, object coerceValue)
		{
			var thiss = (StyleListComboBoxBase<TManager, TList, TItem>)obj;
			return thiss.InternalSelectedItemCoerce((TItem)coerceValue);
		}

		protected virtual void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var oldItem = (TItem)args.OldValue;
			var newItem = (TItem)args.NewValue;

			// make sure, that the item is part of the data items of the ComboBox
			if (_styleListManager.GetParentList(newItem) == null)
			{
				StoreAsLastUsedItem(_lastLocalUsedItems, newItem);
			}

			if (!object.ReferenceEquals(newItem, GuiComboBoxSelectedValue))
				this.UpdateComboBoxSourceSelection(newItem);

			if (!object.ReferenceEquals(_styleListManager.GetParentList(oldItem), _styleListManager.GetParentList(newItem)) && !object.ReferenceEquals(_styleListManager.GetParentList(newItem), GuiTreeView.SelectedValue))
				this.UpdateTreeViewSelection();

			SelectedItemChanged?.Invoke(obj, args);
			_viewEvent_SelectedItemChanged?.Invoke();
		}

		#endregion Dependency property SelectedItem

		#region Dependency property

		/// <summary>
		/// Coerce the selected item to fulfill certain requirements: (i) the item must still be a member of the ParentColorSet of this item.
		/// </summary>
		/// <param name="item">The item that fulfills the above stated requirements.</param>
		/// <returns></returns>
		protected virtual TItem InternalSelectedItemCoerce(TItem item)
		{
			return item;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the popup showing the TreeView is open.
		/// </summary>
		/// <value><c>True</c> if the TreeView popup is open; otherwise, <c>false</c>.</value>
		public bool IsTreeDropDownOpen
		{
			get { return (bool)GetValue(IsTreeDropDownOpenProperty); }
			set { SetValue(IsTreeDropDownOpenProperty, value); }
		}

		#endregion Dependency property

		#region Tree View

		/// <summary>
		/// Provides public access to the <see cref="DataTemplateSelector"/> that selected the data template for different nodes of the TreeView.
		/// </summary>
		public DataTemplateSelector TreeViewItemTemplateSelector
		{
			get
			{
				return new TreeViewDataTemplateSelector(this);
			}
		}

		#region Tree view event handlers

		private static void EhIsTreeDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var thiss = d as StyleListComboBoxBase<TManager, TList, TItem>;
			if (false == (bool)e.OldValue && true == (bool)e.NewValue) // Tree popup is just opened
				thiss.OnTreePopupOpened();
			else if (true == (bool)e.OldValue && false == (bool)e.NewValue) // Tree popup is just closed
				thiss.OnTreePopupClosed();
		}

		/// <summary>
		/// Called when the TreeView popup is opened.
		/// </summary>
		protected virtual void OnTreePopupOpened()
		{
			_selectedFromTreeView = GuiTreeView.SelectedValue;
			Mouse.Capture(this, CaptureMode.SubTree);
		}

		/// <summary>
		/// Closes the popup if it is open (meaning the mouse is captured) and the user clicked outside the popup.
		/// Known bug: if the user has clicked on an item inside the tree view, the mouse is no longer captured by the UserControl, but by a item in the TreeView. Thus, OnMouseDown is no longer called,
		/// and the popup will not close when the user then clicked outside the popup.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (true == IsTreeDropDownOpen && Mouse.Captured == this && e.OriginalSource == this)
			{
				IsTreeDropDownOpen = false;
			}
		}

		/// <summary>
		/// Called when the TreeView popup is closed.
		/// </summary>
		protected virtual void OnTreePopupClosed()
		{
			if (Mouse.Captured == this)
				Mouse.Capture(null);

			var oldSelection = _selectedFromTreeView;
			var newSelection = GuiTreeView.SelectedValue as NGTreeNode;
			_selectedFromTreeView = newSelection;
			if (null != newSelection && !newSelection.Equals(oldSelection))
			{
				if (newSelection.Tag is TItem)
				{
					InternalSelectedItem = (TItem)newSelection.Tag;
					return; // no need here to open the combobox after selection
				}
				else if (newSelection.Tag is TList)
				{
					var cset = (TList)newSelection.Tag;
					if (cset.Count > 0)
					{
						InternalSelectedItem = cset[0];
						// then open the real popup
						GuiComboBox.IsDropDownOpen = true;
						return;
					}
				}
				else
				{
					// search upwards until a colorset is found
					// if no set is found, there is no need to do anything here
				}
			}
		}

		/// <summary>
		/// Called when a tree view mouse double click occurs.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
		protected virtual void EhTreeViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			SetValue(IsTreeDropDownOpenProperty, false);
		}

		/// <summary>
		/// Called when the tree view received a KeyDown event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
		protected virtual void EhTreeViewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Escape)
			{
				SetValue(IsTreeDropDownOpenProperty, false);
			}
		}

		#endregion Tree view event handlers

		#region Tree view data handling

		private void EhColorSetManager_Changed()
		{
			UpdateTreeViewTreeNodes();
		}

		/// <summary>
		/// Updates/fills the data nodes used for the TreeView content.
		/// </summary>
		protected virtual void UpdateTreeViewTreeNodes()
		{
			var builtIn = new NGTreeNode() { Text = "Builtin", Tag = Altaxo.Main.ItemDefinitionLevel.Builtin };
			var app = new NGTreeNode() { Text = "Application", Tag = Altaxo.Main.ItemDefinitionLevel.Application };
			var user = new NGTreeNode() { Text = "User", Tag = Altaxo.Main.ItemDefinitionLevel.UserDefined };
			var proj = new NGTreeNode() { Text = "Project", Tag = Altaxo.Main.ItemDefinitionLevel.Project };

			foreach (var set in _styleListManager.GetEntryValues())
			{
				switch (set.Level)
				{
					case Altaxo.Main.ItemDefinitionLevel.Builtin:
						builtIn.Nodes.Add(new NGTreeNodeForTList(this, set.List));
						break;

					case Altaxo.Main.ItemDefinitionLevel.Application:
						app.Nodes.Add(new NGTreeNodeForTList(this, set.List));
						break;

					case Altaxo.Main.ItemDefinitionLevel.UserDefined:
						user.Nodes.Add(new NGTreeNodeForTList(this, set.List));
						break;

					case Altaxo.Main.ItemDefinitionLevel.Project:
						proj.Nodes.Add(new NGTreeNodeForTList(this, set.List));
						break;
				}
			}

			_treeRootNode.Nodes.Clear();
			_treeRootNode.Nodes.Add(builtIn);
			_treeRootNode.Nodes.Add(app);
			_treeRootNode.Nodes.Add(user);
			_treeRootNode.Nodes.Add(proj);
		}

		/// <summary>
		/// Updates the tree view node selection according to the currently selected item.
		/// </summary>
		protected virtual void UpdateTreeViewSelection()
		{
			var selectedItem = this.InternalSelectedItem;

			GuiTreeView.ItemsSource = null;
			_treeRootNode.FromHereToLeavesDo(node => { node.IsExpanded = false; node.IsSelected = false; }); // deselect and collapse all nodes

			if (_styleListManager.GetParentList(selectedItem) != null)
			{
				var colorSet = _styleListManager.GetParentList(selectedItem);
				Altaxo.Main.ItemDefinitionLevel level = Altaxo.Main.ItemDefinitionLevel.Project;

				_treeRootNode.FromHereToLeavesDo(node =>
				{
					if ((node.Tag is Altaxo.Main.ItemDefinitionLevel) && (Altaxo.Main.ItemDefinitionLevel)node.Tag == level)
					{
						node.IsExpanded = true; // expand the node the current item list belongs to (like "Builtin", "Application" etc.)
					}
					else if (node.Tag is IColorSet && object.ReferenceEquals(node.Tag, colorSet))
					{
						node.IsSelected = true; // select the node of the current item list
					}
				});
			};
			GuiTreeView.ItemsSource = _treeRootNode.Nodes;
		}

		#endregion Tree view data handling

		#endregion Tree View

		#region ComboBox

		/// <summary>
		/// Chooses the item list whose items are shown in the ComboBox.
		/// </summary>
		/// <returns>The item list whose items are shown in the ComboBox. Normally, the item list the currently selected items belongs to is shown in the ComboBox.
		/// If the currently selected item has no parent item list, the BuiltinDefault item list will be shown.</returns>
		protected virtual TList GetStyleListForComboBox()
		{
			var selectedItem = InternalSelectedItem;
			var parentList = _styleListManager.GetParentList(selectedItem);
			return parentList != null ? parentList : _styleListManager.BuiltinDefault;
		}

		#endregion ComboBox

		#region ComboBox data handling

		protected void UpdateComboBoxSourceSelection(TItem item)
		{
			if (null != item && object.ReferenceEquals(item, GuiComboBoxSelectedValue))
				return;

			_filterString = string.Empty;
			FillComboBoxWithFilteredItems(_filterString, false);
			GuiComboBoxSelectedValue = default(TItem); // make sure that combobox really changes the selected item, because it can not rely on Equals
			GuiComboBoxSelectedValue = item;
		}

		private List<object> _comboBoxSeparator1 = new List<object> { new Separator() { Name = "ThisIsASeparatorForTheComboBox", Tag = "Last used items" } };
		private List<object> _comboBoxSeparator2 = new List<object> { new Separator() { Name = "ThisIsASeparatorForTheComboBox", Tag = "Item list" } };

		/// <summary>
		/// Fills the combo box with items, whose name starts with a given <paramref name="filterString"/> .
		/// </summary>
		/// <param name="filterString">The filter string.</param>
		/// <param name="onlyIfItemsRemaining">If set to <c>false</c>, and no items match the filter criterium, the content of the ComboBox is left unchanged. Otherwise, even if no items match the filter criterium, the contents of the ComboBox is set to those items that match the criterium.</param>
		/// <returns><c>True</c> if at least one item match the filter criterium. <c>False</c> if no item match the criterium.</returns>
		protected virtual bool FillComboBoxWithFilteredItems(string filterString, bool onlyIfItemsRemaining)
		{
			List<object> lastUsed;

			List<object> separator = new List<object> { new Separator() { Name = "ThisIsASeparatorForTheComboBox", Tag = "Item list" } };

			lastUsed = GetFilteredList(_lastLocalUsedItems, filterString);

			var itemList = GetStyleListForComboBox();
			var known = GetFilteredList(itemList, filterString);

			if ((lastUsed.Count + known.Count) > 0 || !onlyIfItemsRemaining)
			{
				IEnumerable<object> source = null;

				if (lastUsed.Count > 0)
				{
					source = _comboBoxSeparator1.Concat(lastUsed);
				}
				if (known.Count > 0)
				{
					(_comboBoxSeparator2[0] as Separator).Tag = itemList.Name;
					if (source == null)
						source = _comboBoxSeparator2.Concat(known);
					else
						source = source.Concat(_comboBoxSeparator2).Concat(known);
				}
				GuiComboBox.ItemsSource = ConvertComboBoxSourceItems(source);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Function that converts the items intended to set the ItemsSource of the ComboBox to something else.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		protected virtual IEnumerable<object> ConvertComboBoxSourceItems(IEnumerable<object> source)
		{
			return source;
		}

		/// <summary>g
		/// Gets or sets the selected value of the <see cref="GuiComboBox"/>.
		/// If using an implementation a <see cref="ComboBox"/> that get/sets the items
		/// via another property that the SelectedValue property, you can override this function to get/set
		/// the selected values of this <see cref="ComboBox"/>.
		/// </summary>
		/// <value>
		/// The selected value of the <see cref="GuiComboBox"/>.
		/// </value>
		protected virtual TItem GuiComboBoxSelectedValue
		{
			get
			{
				return (TItem)GuiComboBox.SelectedValue;
			}
			set
			{
				GuiComboBox.SelectedValue = value;
			}
		}

		protected List<object> GetFilteredList(IList<TItem> originalList, string filterString)
		{
			var result = new List<object>();
			filterString = filterString.ToLowerInvariant();
			foreach (var item in originalList)
			{
				if (GetDisplayName(item).ToLowerInvariant().StartsWith(filterString))
					result.Add(item);
			}
			return result;
		}

		#endregion ComboBox data handling

		#region ComboBox event handling

		protected void EhComboBox_DropDownClosed(object sender, EventArgs e)
		{
			if (_filterString.Length > 0)
			{
				var selItem = GuiComboBoxSelectedValue;
				_filterString = string.Empty;
				FillComboBoxWithFilteredItems(_filterString, false);
				GuiComboBoxSelectedValue = selItem;
			}

			if (GuiComboBoxSelectedValue == null)
				GuiComboBoxSelectedValue = SelectedItem;
			else
				this.SelectedItem = GuiComboBoxSelectedValue;
		}

		#endregion ComboBox event handling

		#region Key processing of user control

		/// <summary>
		/// Handles the KeyDown event of the UserControl. Here, the filter string for the ComboBox content is updated according to the pressed key.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (this.GuiComboBox.IsDropDownOpen)
			{
				Key pressedKey = e.Key;
				string pressedString = new KeyConverter().ConvertToInvariantString(pressedKey);
				char pressedChar = pressedString.Length == 1 ? pressedString[0] : '\0';

				if (char.IsLetterOrDigit(pressedChar))
				{
					string filterString = _filterString + pressedChar;

					if (FillComboBoxWithFilteredItems(filterString, true))
						_filterString = filterString;
				}
				else if (pressedKey == Key.Delete || pressedKey == Key.Back)
				{
					if (_filterString.Length > 0)
					{
						_filterString = _filterString.Substring(0, _filterString.Length - 1);
						FillComboBoxWithFilteredItems(_filterString, false);
					}
				}
			}
			base.OnKeyDown(e);
		}

		#endregion Key processing of user control

		#region Context menus

		/// <summary>
		/// Stores an item as last used item in a list.
		/// </summary>
		/// <typeparam name="T">Type of the item to store.</typeparam>
		/// <param name="_listOfLocalLastUsedItems">The list of last used items.</param>
		/// <param name="lastUsedItem">The item to store.</param>
		protected void StoreAsLastUsedItem<T>(List<T> _listOfLocalLastUsedItems, T lastUsedItem)
		{
			if (_listOfLocalLastUsedItems.Contains(lastUsedItem))
				return;
			_listOfLocalLastUsedItems.Insert(0, lastUsedItem);
			// Trim local used items to maximum count
			for (int i = _listOfLocalLastUsedItems.Count - 1; i >= MaxNumberOfLastLocalUsedItems; --i)
				_listOfLocalLastUsedItems.RemoveAt(i);
		}

		protected virtual void EhShowStyleListManagerDialog(object sender, RoutedEventArgs e)
		{
			var itemList = _styleListManager.GetParentList(SelectedItem);
			if (null == itemList)
				itemList = _styleListManager.BuiltinDefault;

			var controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { itemList }, typeof(IMVCANController));

			if (null == controller || null == controller.ViewObject)
				return;

			if (Current.Gui.ShowDialog(controller, "Manage item lists", false))
			{
				itemList = (TList)controller.ModelObject;
				if (!object.ReferenceEquals(_styleListManager.GetParentList(SelectedItem), itemList))
					SelectedItem = itemList[0];
			}
		}

		#endregion Context menus

		#region Code to close the TreeView popup

		protected virtual void EhComboBox_DropDownOpened(object sender, EventArgs e)
		{
			if (IsTreeDropDownOpen)
				IsTreeDropDownOpen = false;
		}

		protected override void OnContextMenuClosing(ContextMenuEventArgs e)
		{
			base.OnContextMenuClosing(e);
			if (IsTreeDropDownOpen)
				IsTreeDropDownOpen = false;
		}

		/// <summary>If keyboard focus is lost from the user control, and is also not into the TreeView, the TreeView popup should be closed.</summary>
		/// <param name="e">Event args.</param>
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnIsKeyboardFocusWithinChanged(e);
			if (this.IsTreeDropDownOpen && !base.IsKeyboardFocusWithin && !GuiTreeView.IsKeyboardFocusWithin)
			{
				IsTreeDropDownOpen = false;
			}
		}

		#endregion Code to close the TreeView popup
	}
}