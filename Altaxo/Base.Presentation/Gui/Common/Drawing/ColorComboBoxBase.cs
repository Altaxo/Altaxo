#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
using Altaxo.Gui.Drawing.ColorManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Base class for <see cref="ColorComboBox"/> and <see cref="BrushComboBox"/>, which let the user choose between colors of different color sets.
	/// </summary>
	public abstract class ColorComboBoxBase : UserControl
	{
		#region Inner classes

		/// <summary>
		/// Special tree node for a color set. This tree node fills its child items only when it gets expanded.
		/// </summary>
		private class NGTreeNodeForColorSet : NGTreeNode
		{
			public NGTreeNodeForColorSet(IColorSet colorSet)
				: base(true)
			{
				_tag = colorSet;
				_text = colorSet.Name;
			}

			protected override void LoadChildren()
			{
				base.LoadChildren();

				foreach (var c in (IColorSet)_tag)
				{
					Nodes.Add(new NGTreeNode() { Text = c.Name, Tag = c });
				}
			}
		}

		/// <summary>
		/// Selects the data template for the TreeView: either for a <see cref="NamedColor"/>, for a <see cref="IColorSet"/> or for another node.
		/// </summary>
		public class TreeViewDataTemplateSelector : DataTemplateSelector
		{
			private FrameworkElement _parent;
			private DataTemplate _namedColorTemplate;
			private DataTemplate _colorSetTemplate;
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
					if (node.Tag is NamedColor)
					{
						if (null == _namedColorTemplate)
							_namedColorTemplate = (DataTemplate)_parent.TryFindResource("NamedColorTemplate");
						if (null != _namedColorTemplate)
							return _namedColorTemplate;
					}
					else if (node.Tag is IColorSet)
					{
						if (null == _colorSetTemplate)
							_colorSetTemplate = (DataTemplate)_parent.TryFindResource("ColorSetTemplate");
						if (null != _colorSetTemplate)
							return _colorSetTemplate;
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

		/// <summary>Maximum number of colors shown under "last used colors" (or "last used brushes").</summary>
		protected const int MaxNumberOfLastLocalUsedColors = 5;

		/// <summary>Gets access to the tree view, which shows the color sets.</summary>
		protected abstract TreeView GuiTreeView { get; }

		/// <summary>Gets access to the ComboBox, which shows the colors or brushes of the current color set.</summary>
		protected abstract ComboBox GuiComboBox { get; }

		/// <summary>
		/// Gets or sets the selected color internally.
		/// </summary>
		/// <value>
		/// The color to get/set;
		/// </value>
		protected abstract NamedColor InternalSelectedColor { get; set; }

		/// <summary>
		/// Fills the combo box with items, whose name starts with a given <paramref name="filterString"/> .
		/// </summary>
		/// <param name="filterString">The filter string.</param>
		/// <param name="onlyIfItemsRemaining">If set to <c>false</c>, and no items match the filter criterium, the content of the ComboBox is left unchanged. Otherwise, even if no items match the filter criterium, the contents of the ComboBox is set to those items that match the criterium.</param>
		/// <returns><c>True</c> if at least one item match the filter criterium. <c>False</c> if no item match the criterium.</returns>
		protected abstract bool FillComboBoxWithFilteredItems(string filterString, bool onlyIfItemsRemaining);

		/// <summary>
		/// The color set manager used here.
		/// </summary>
		private ColorSetManager _colorSetManager;

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

		/// <summary>Poperty that describes whether only plot colors should be shown in the TreeView and the ComboBox.</summary>
		public static readonly DependencyProperty ShowPlotColorsOnlyProperty;

		#region Constructors

		static ColorComboBoxBase()
		{
			IsTreeDropDownOpenProperty = DependencyProperty.Register("IsTreeDropDownOpen", typeof(bool), typeof(ColorComboBoxBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(EhIsTreeDropDownOpenChanged)));
			ShowPlotColorsOnlyProperty = DependencyProperty.Register("ShowPlotColorsOnly", typeof(bool), typeof(ColorComboBoxBase), new FrameworkPropertyMetadata(false, EhShowPlotColorsOnlyChanged));
		}

		protected ColorComboBoxBase()
		{
			_colorSetManager = ColorSetManager.Instance;
			_colorSetManager.Changed += EhColorSetManager_ListAdded;
		}

		#endregion Constructors

		#region Dependency property

		/// <summary>
		/// Gets or sets a value indicating whether to show only plot colors and plot color sets in the ComboBox and the TreeView.
		/// </summary>
		/// <value><c>True</c> if only plot colors are shown; otherwise, <c>false</c>.</value>
		public bool ShowPlotColorsOnly
		{
			get { return (bool)GetValue(ShowPlotColorsOnlyProperty); }
			set { SetValue(ShowPlotColorsOnlyProperty, value); }
		}

		private static void EhShowPlotColorsOnlyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			((ColorComboBoxBase)obj).OnShowPlotColorsOnlyChanged(obj, e);
		}

		/// <summary>
		/// Called when the <see cref="ShowPlotColorsOnly"/> property has changed.
		/// </summary>
		/// <param name="obj">Sender of this event.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void OnShowPlotColorsOnlyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (true == (bool)e.NewValue && false == (bool)e.OldValue) // show only plot colors
			{
				// if the current color is a plot color, we can leave everything as it is, except that we must update the tree view nodes
				if (InternalSelectedColor.ParentColorSet != null && ColorSetManager.Instance.IsPlotColorSet(InternalSelectedColor.ParentColorSet))
				{
					UpdateTreeViewTreeNodes();
					UpdateTreeViewSelection();
				}
				else
				{
					UpdateTreeViewTreeNodes();
					InternalSelectedColor = InternalSelectedColorCoerce(InternalSelectedColor);
				}
			}
			else if (false == (bool)e.NewValue && true == (bool)e.OldValue) // show all color sets
			{
				// we can leave the currently selected color as it is, we only need to update the items source for the tree view
				UpdateTreeViewTreeNodes();
				UpdateTreeViewSelection();
			}
		}

		/// <summary>
		/// Coerce the selected color to fulfill certain requirements: (i) the color must still be a member of the ParentColorSet of this color, and (ii) if the <see cref="ShowPlotColorsOnly"/>
		/// property is set to <c>true</c>, the color must be a member of a plot color set.
		/// </summary>
		/// <param name="color">The color that fulfills the above stated requirements.</param>
		/// <returns></returns>
		protected virtual NamedColor InternalSelectedColorCoerce(NamedColor color)
		{
			color = color.CoerceParentColorSetToNullIfNotMember();

			if (this.ShowPlotColorsOnly && (color.ParentColorSet == null || false == ColorSetManager.Instance.IsPlotColorSet(color.ParentColorSet)))
			{
				return ColorSetManager.Instance.BuiltinDarkPlotColors[0];
			}
			return color;
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
			var thiss = d as ColorComboBoxBase;
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
				if (newSelection.Tag is NamedColor)
				{
					InternalSelectedColor = (NamedColor)newSelection.Tag;
					return; // no need here to open the combobox after selection
				}
				else if (newSelection.Tag is IColorSet)
				{
					IColorSet cset = (IColorSet)newSelection.Tag;
					if (cset.Count > 0)
					{
						InternalSelectedColor = cset[0];
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

		private void EhColorSetManager_ListAdded()
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

			bool showPlotColorsOnly = this.ShowPlotColorsOnly;

			foreach (var set in _colorSetManager.GetEntryValues())
			{
				if (showPlotColorsOnly && !set.IsPlotColorSet)
					continue;

				switch (set.Level)
				{
					case Altaxo.Main.ItemDefinitionLevel.Builtin:
						builtIn.Nodes.Add(new NGTreeNodeForColorSet(set.List));
						break;

					case Altaxo.Main.ItemDefinitionLevel.Application:
						app.Nodes.Add(new NGTreeNodeForColorSet(set.List));
						break;

					case Altaxo.Main.ItemDefinitionLevel.UserDefined:
						user.Nodes.Add(new NGTreeNodeForColorSet(set.List));
						break;

					case Altaxo.Main.ItemDefinitionLevel.Project:
						proj.Nodes.Add(new NGTreeNodeForColorSet(set.List));
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
		/// Updates the tree view node selection according to the currently selected color.
		/// </summary>
		protected virtual void UpdateTreeViewSelection()
		{
			var selColor = this.InternalSelectedColor;

			GuiTreeView.ItemsSource = null;
			_treeRootNode.FromHereToLeavesDo(node => { node.IsExpanded = false; node.IsSelected = false; }); // deselect and collapse all nodes

			if (selColor.ParentColorSet != null)
			{
				var colorSet = selColor.ParentColorSet;
				Altaxo.Main.ItemDefinitionLevel level = Altaxo.Main.ItemDefinitionLevel.Project;

				bool isPlotColorSet = false;
				ColorSetManagerEntryValue colorSetEntry;
				if (selColor.ParentColorSet != null && ColorSetManager.Instance.TryGetList(selColor.ParentColorSet.Name, out colorSetEntry))
				{
					isPlotColorSet = colorSetEntry.IsPlotColorSet;
				}

				_treeRootNode.FromHereToLeavesDo(node =>
				{
					if ((node.Tag is Altaxo.Main.ItemDefinitionLevel) && (Altaxo.Main.ItemDefinitionLevel)node.Tag == level)
					{
						node.IsExpanded = true; // expand the node the current color set belongs to (like "Builtin", "Application" etc.)
					}
					else if (node.Tag is IColorSet && object.ReferenceEquals(node.Tag, colorSet))
					{
						node.IsSelected = true; // select the node of the current color set
					}
				});
			};
			GuiTreeView.ItemsSource = _treeRootNode.Nodes;
		}

		#endregion Tree view data handling

		#endregion Tree View

		#region ComboBox

		/// <summary>
		/// Chooses the color set whose colors are shown in the ComboBox.
		/// </summary>
		/// <returns>The color set whose colors are shown in the ComboBox. Normally, the color set the currently selected color belongs to is shown in the ComboBox.
		/// If the currently selected color has no parent color set, the 'Builtin/KnownColors' color set will be shown.</returns>
		protected virtual IColorSet GetColorSetForComboBox()
		{
			NamedColor selColor = InternalSelectedColor;
			if (selColor.ParentColorSet != null)
				return selColor.ParentColorSet;
			else
				return NamedColors.Instance;
		}

		#endregion ComboBox

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
		/// Shows the custom color dialog (if the <see cref="ShowPlotColorsOnly"/> property is set to <c>true</c>, this command will be ignored.
		/// </summary>
		/// <param name="sender">The sender of this event.</param>
		/// <param name="newColor">The user selected new color.</param>
		/// <returns><c>True</c> if the user has chosen a new color. Otherwise, <c>false</c>.</returns>
		protected virtual bool InternalShowCustomColorDialog(object sender, out NamedColor newColor)
		{
			if (ShowPlotColorsOnly)
			{
				newColor = InternalSelectedColor;
				return false;
			}
			/*
			var SelectedWpfColor = GuiHelper.ToWpf(InternalSelectedColor);
			ColorController ctrl = new ColorController(SelectedWpfColor);
			ctrl.ViewObject = new ColorPickerControl(SelectedWpfColor);
			*/
			var ctrl = new ColorModelController();
			ctrl.InitializeDocument(InternalSelectedColor);
			Current.Gui.FindAndAttachControlTo(ctrl);
			if (Current.Gui.ShowDialog(ctrl, "Select a color", false))
			{
				newColor = (NamedColor)ctrl.ModelObject;
				return true;
			}
			else
			{
				newColor = InternalSelectedColor;
				return false;
			}
		}

		/// <summary>
		/// Choose an opacity from the context menu. The opacity value in percent must be delived as a string in the Tag property of the sender, which normally is a context menu item.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="newNamedColor">If the return value is <c>true</c>, contains the new color with the chosen opacity value.</param>
		/// <returns><c>True</c>, if a new color with the given opacity value could be created.</returns>
		protected virtual bool InternalChooseOpacityFromContextMenu(object sender, out NamedColor newNamedColor)
		{
			if (ShowPlotColorsOnly)
			{
				newNamedColor = InternalSelectedColor;
				return false;
			}

			var opacityInPercent = int.Parse((string)((MenuItem)sender).Tag, System.Globalization.CultureInfo.InvariantCulture);
			newNamedColor = InternalSelectedColor.NewWithOpacityInPercent(opacityInPercent);
			return true;
		}

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
			// Trim local used colors to maximum count
			for (int i = _listOfLocalLastUsedItems.Count - 1; i >= MaxNumberOfLastLocalUsedColors; --i)
				_listOfLocalLastUsedItems.RemoveAt(i);
		}

		protected virtual void EhShowColorSetManagerDialog(object sender, RoutedEventArgs e)
		{
			var listController = new ColorSetController();
			listController.InitializeDocument(InternalSelectedColor.ParentColorSet);
			Current.Gui.FindAndAttachControlTo(listController);

			if (Current.Gui.ShowDialog(listController, "Manage color sets", false))
			{
				var colorSetChosen = (IColorSet)listController.ModelObject;

				if (object.ReferenceEquals(InternalSelectedColor.ParentColorSet, colorSetChosen))
					return; // nothing has changed

				if (ShowPlotColorsOnly && object.ReferenceEquals(colorSetChosen, ColorSetManager.Instance.BuiltinKnownColors))
				{
					Current.Gui.ErrorMessageBox(string.Format("The color set '{0}' is not admitted to be used as plot color set. Please choose another color set, or derive a new color set from '{0}'.", ColorSetManager.Instance.BuiltinKnownColors.Name), "ColorSet not admitted");
					return;
				}

				if (ShowPlotColorsOnly)
				{
					ColorSetManager.Instance.DeclareAsPlotColorList(colorSetChosen);
				}

				// we choose the first color from the new color set
				InternalSelectedColor = colorSetChosen[0];
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