#region Copyright

/////////////////////////////////////////////////////////////////////////////
//
// BSD 3-Clause License
//
// Copyright(c) 2015-16, Jan Karger(Steven Kirk)
//
// All rights reserved.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

#if NET35
using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
#endif

namespace GongSolutions.Wpf.DragDrop.Utilities
{
  public static class ItemsControlExtensions
  {
    /// <summary>
    /// Stores information about classes derived from <see cref="ItemsControl"/> that are neccessary for the drag/drop operations.
    /// </summary>
    private class ItemsControlInformation
    {
      /// <summary>The type of items control this information applies to.</summary>
      internal Type ItemsControlType;

      /// <summary>The type of items container which is used by the <see cref="ItemsControl"/>. Example: a <see cref="ListView"/> uses <see cref="ListViewItem"/>s.</summary>
      internal Type ItemContainerType;

      /// <summary>Function that determines if multiple items can be selected in the items control. For some items controls, that depends on the selection mode. The argument is the <see cref="ItemsControl"/>. If the return value is <c>true</c>, multiple items can be selected.</summary>
      internal Func<ItemsControl, bool> CanSelectMultipleItems;

      /// <summary>Function that retrieves the currently selected items from the items control. The argument is the items control. The return value is the enumeration of the currently selected items (<b>not</b> the item containers!).</summary>
      internal Func<ItemsControl, IEnumerable> GetSelectedItems;

      /// <summary>Function that determines whether the provided item is selected in the items control. 1st arg is the items control, 2nd arg is the item (not the item's container!). The return value is <c>true</c> if the item is selected in the items control.</summary>
      internal Func<ItemsControl, object, bool> GetIsItemSelected;

      /// <summary>Action that sets the selection status of an item. 1st arg is the items control, 2nd arg is the item, and 3rd arg is the selection status. If the selection status is <c>true</c>, the item must be selected in the items control. If the selection status is <c>false</c>, the item must be deselected in the items control. If the items control provides no means to set the selected item(s) programmatically, set this argument (the function itself) to <c>null</c>.</summary>
      internal Action<ItemsControl, object, bool> SetIsItemSelected;

      /// <summary>
      /// Function to get the orientation of the items in the items control. If <c>null</c> is provided for the function, the internal
      /// calculated value for the orientation is used. If the function is provided, but the return value of the function is <c>null</c>,
      /// the orientation is considered undefined (as for instance for the TreeView). In this case the distance between the items is calculated from x and y of their positions.
      /// </summary>
      internal Func<ItemsControl, System.Windows.Controls.Orientation?> GetOrientation;
    }

    /// <summary>
    /// Dictionary to store information about different types of <see cref="ItemsControl"/> classes. Key is the type of <see cref="ItemsControl"/>, value is the information about that type of <see cref="ItemsControl"/>.
    /// </summary>
    private static Dictionary<Type, ItemsControlInformation> _itemsControlRegistrationDict;

    static ItemsControlExtensions()
    {
      _itemsControlRegistrationDict = new Dictionary<Type, ItemsControlInformation>();

      RegisterWellKnownItemsControlTypes();
    }

    /// <summary>
    /// Determines whether the provided items control type is already registered.
    /// </summary>
    /// <param name="itemsControlType">Type of the items control.</param>
    /// <returns><c>True</c> if the provided items control type is already registered; otherwise <c>false</c>.</returns>
    public static bool IsItemsControlTypeRegistered(System.Type itemsControlType)
    {
      return _itemsControlRegistrationDict.ContainsKey(itemsControlType);
    }

    /// <summary>
    /// Registers the items control type.
    /// </summary>
    /// <typeparam name="TItemsControl">The type of the items control.</typeparam>
    /// <typeparam name="TItemsContainer">The type of the items container the items control is using.</typeparam>
    /// <param name="CanSelectMultipleItems">Function that determines if multiple items can be selected in the items control. For some items controls, that depends on the selection mode. If the return value is <c>true</c>, multiple items can be selected.</param>
    /// <param name="GetSelectedItems">Function that retrieves the currently selected items from the items control. The argument is the items control.</param>
    /// <param name="GetIsItemSelected">Function that determines whether the provided item is selected in the items control. 1st arg is the items control, 2nd arg is the item. The return value is <c>true</c> if the item is selected in the items control.</param>
    /// <param name="SetIsItemSelected">Action that sets the selection status of an item. 1st arg is the items control, 2nd arg is the item, and 3rd arg is the selection status. If the selection status is <c>true</c>, the item must be selected in the items control. If the selection status is <c>false</c>, the item must be deselected in the items control. If the items control provides no means to set the selected item(s) programmatically, set this argument to <c>null</c>.</param>
    /// <param name="GetOrientation">Function that gets the orientation of the items with respect to each other. Lists items are usually oriented vertically, but this can be changed. Because TreeViewItems have no specific orientation with respect to each other, the return value should be <c>null</c> in this case.</param>
    public static void RegisterItemsControl<TItemsControl, TItemsContainer>(
      Func<TItemsControl, bool> CanSelectMultipleItems,
      Func<TItemsControl, IEnumerable> GetSelectedItems,
      Func<TItemsControl, object, bool> GetIsItemSelected,
      Action<TItemsControl, object, bool> SetIsItemSelected,
      Func<TItemsControl, System.Windows.Controls.Orientation?> GetOrientation) where TItemsControl : ItemsControl
    {
      RegisterItemsControl(
        typeof(TItemsControl),
        typeof(TItemsContainer),
        itemsControl => CanSelectMultipleItems((TItemsControl)itemsControl),
        itemsControl => GetSelectedItems((TItemsControl)itemsControl),
        (itemsControl, o) => GetIsItemSelected((TItemsControl)itemsControl, o),
        (itemsControl, o, b) => SetIsItemSelected((TItemsControl)itemsControl, o, b),
        GetOrientation is null ? (Func<ItemsControl, System.Windows.Controls.Orientation?>)null : (itemsControl => GetOrientation((TItemsControl)itemsControl))
        );
    }

    /// <summary>
    /// Registers the items control.
    /// </summary>
    /// <param name="itemsControlType">Type of the items control.</param>
    /// <param name="itemsContainerType">Type of the items container the items control is using for its items.</param>
    /// <param name="CanSelectMultipleItems">Function that determines if multiple items can be selected in the items control. For some items controls, that depends on the selection mode. If the return value is <c>true</c>, multiple items can be selected.</param>
    /// <param name="GetSelectedItems">Function that retrieves the currently selected items from the items control. The argument is the items control.</param>
    /// <param name="GetIsItemSelected">Function that determines whether the provided item is selected in the items control. 1st arg is the items control, 2nd arg is the item. The return value is <c>true</c> if the item is selected in the items control.</param>
    /// <param name="SetIsItemSelected">Action that sets the selection status of an item. 1st arg is the items control, 2nd arg is the item, and 3rd arg is the selection status. If the selection status is <c>true</c>, the item must be selected in the items control. If the selection status is <c>false</c>, the item must be deselected in the items control. If the items control provides no means to set the selected item(s) programmatically, set this argument to <c>null</c>.</param>
    /// <param name="GetOrientation">Function that gets the orientation of the items with respect to each other. Lists items are usually oriented vertically, but this can be changed. Because TreeViewItems have no specific orientation with respect to each other, the return value should be <c>null</c> in this case.</param>
    /// <exception cref="System.ArgumentException">The provided items control type is not derived from ItemsControl.</exception>
    public static void RegisterItemsControl(
      System.Type itemsControlType,
      System.Type itemsContainerType,
      Func<ItemsControl, bool> CanSelectMultipleItems,
      Func<ItemsControl, IEnumerable> GetSelectedItems,
      Func<ItemsControl, object, bool> GetIsItemSelected,
      Action<ItemsControl, object, bool> SetIsItemSelected,
      Func<ItemsControl, System.Windows.Controls.Orientation?> GetOrientation)
    {
      if (!typeof(ItemsControl).IsAssignableFrom(itemsControlType))
        throw new ArgumentException("The provided items control type is not derived from ItemsControl.");

      var info = new ItemsControlInformation
      {
        ItemsControlType = itemsControlType,
        ItemContainerType = itemsContainerType,
        CanSelectMultipleItems = CanSelectMultipleItems,
        GetSelectedItems = GetSelectedItems,
        GetIsItemSelected = GetIsItemSelected,
        SetIsItemSelected = SetIsItemSelected,
        GetOrientation = GetOrientation
      };

      if (!_itemsControlRegistrationDict.ContainsKey(itemsControlType))
      {
        _itemsControlRegistrationDict.Add(itemsControlType, info);
      }
      else
      {
        _itemsControlRegistrationDict[itemsControlType] = info;
      }
    }

    /// <summary>
    /// Finds the items control information. If the type of items control is not registered, the base class types of that type are successivly tried. If neither the type itself nor one of the base class types
    /// are found in the registration, <c>null</c> is returned.
    /// </summary>
    /// <param name="itemsControl">The items control for which to find information.</param>
    /// <returns>The information about that type, or <c>null</c> if no information is found.</returns>
    private static ItemsControlInformation FindItemsControlInformation(ItemsControl itemsControl)
    {

      Type itemsControlType = itemsControl.GetType();
      if (_itemsControlRegistrationDict.TryGetValue(itemsControlType, out var info))
        return info;

      while (itemsControlType != typeof(ItemsControl))
      {
        itemsControlType = itemsControlType.BaseType;
        if (_itemsControlRegistrationDict.TryGetValue(itemsControlType, out info))
          return info;
      }
      return null; // nothing found
    }

    /// <summary>
    /// Registers the information for the <see cref="ItemsControl"/> types that come with the .NET framework.
    /// </summary>
    private static void RegisterWellKnownItemsControlTypes()
    {
      // ListBox
      RegisterItemsControl<ListBox, ListBoxItem>(
        (itemsControl) => itemsControl.SelectionMode != SelectionMode.Single,
        (itemsControl) => { if (itemsControl.SelectionMode != SelectionMode.Single) return itemsControl.SelectedItems; else return Enumerable.Repeat(itemsControl.SelectedItem, 1); },
        (listBox, item) => listBox.SelectedItems.Contains(item),
        (listBox, item, isSelected) =>
        {
          if (isSelected)
          {
            if (listBox.SelectionMode != SelectionMode.Single)
            {
              listBox.SelectedItems.Add(item);
            }
            else
            {
              listBox.SelectedItem = item;
            }
          }
          else
          {
            listBox.SelectedItems.Remove(item);
          }
        },
        null // GetOrientation -> use internal function to get the orientation
        );

      // ListView
      RegisterItemsControl<ListView, ListViewItem>(
        (itemsControl) => itemsControl.SelectionMode != SelectionMode.Single,
        (itemsControl) => { if (itemsControl.SelectionMode != SelectionMode.Single) return itemsControl.SelectedItems; else return Enumerable.Repeat(itemsControl.SelectedItem, 1); },
        (listView, item) => listView.SelectedItems.Contains(item),
        (listView, item, isSelected) =>
        {
          if (isSelected)
          {
            if (listView.SelectionMode != SelectionMode.Single)
            {
              listView.SelectedItems.Add(item);
            }
            else
            {
              listView.SelectedItem = item;
            }
          }
          else
          {
            listView.SelectedItems.Remove(item);
          }
        },
        null // GetOrientation -> use internal function to get the orientation
        );

      // DataGrid
      RegisterItemsControl<DataGrid, DataGridRow>(
      (dataGrid) => dataGrid.SelectionMode != DataGridSelectionMode.Single,
      (dataGrid) => { if (dataGrid.SelectionMode != DataGridSelectionMode.Single) return dataGrid.SelectedItems; else return Enumerable.Repeat(dataGrid.SelectedItem, 1); },
      (dataGrid, item) => dataGrid.SelectionMode != DataGridSelectionMode.Single ? dataGrid.SelectedItems.Contains(item) : dataGrid.SelectedItem == item,
      (dataGrid, item, isSelected) =>
      {
        if (dataGrid.SelectionMode != DataGridSelectionMode.Single)
        {
          if (isSelected)
            dataGrid.SelectedItems.Add(item);
          else
            dataGrid.SelectedItems.Remove(item);
        }
        else
        {
          dataGrid.SelectedItem = isSelected ? item : null;
        }
      },
      null // GetOrientation -> use internal function to get the orientation
      );

      // TreeView
      RegisterItemsControl<TreeView, TreeViewItem>(
        (itemsControl) => false,
        (itemsControl) => Enumerable.Repeat(itemsControl.SelectedItem, 1),
        (itemsControl, o) => itemsControl.SelectedItem == o,
        (treeView, item, isSelected) => ((TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(item)).IsSelected = isSelected,
        (itemsControl) => null // GetOrientation returns null because TreeViewItems are neither oriented horizontally nor vertically
        );
    }

    public static CollectionViewGroup FindGroup(this ItemsControl itemsControl, Point position)
    {
      var element = itemsControl.InputHitTest(position) as DependencyObject;

      if (element is not null)
      {
        var groupItem = element.GetVisualAncestor<GroupItem>();

        if (groupItem is not null)
        {
          return groupItem.Content as CollectionViewGroup;
        }
      }

      return null;
    }

    public static bool CanSelectMultipleItems(this ItemsControl itemsControl)
    {
      var info = FindItemsControlInformation(itemsControl);
      if (info is not null)
      {
        return info.CanSelectMultipleItems(itemsControl);
      }
      else if (itemsControl is MultiSelector)
      {
        // The CanSelectMultipleItems property is protected. Use reflection to
        // get its value anyway.
        return (bool)itemsControl.GetType()
                                 .GetProperty("CanSelectMultipleItems", BindingFlags.Instance | BindingFlags.NonPublic)
                                 .GetValue(itemsControl, null);
      }
      else if (itemsControl is ListBox)
      {
        return ((ListBox)itemsControl).SelectionMode != SelectionMode.Single;
      }
      else
      {
        return false;
      }
    }

    public static UIElement GetItemContainer(this ItemsControl itemsControl, UIElement child)
    {
      var itemType = GetItemContainerType(itemsControl, out var isItemContainer);

      if (itemType is not null)
      {
        return isItemContainer
                 ? (UIElement)child.GetVisualAncestor(itemType, itemsControl)
                 : (UIElement)child.GetVisualAncestor(itemType);
      }

      return null;
    }

    public static UIElement GetItemContainerAt(this ItemsControl itemsControl, Point position)
    {
      var inputElement = itemsControl.InputHitTest(position);
      var uiElement = inputElement as UIElement;

      if (uiElement is not null)
      {
        return GetItemContainer(itemsControl, uiElement);
      }

      return null;
    }

    public static UIElement GetItemContainerAt(this ItemsControl itemsControl, Point position,
                                               Orientation searchDirection)
    {
      var itemContainerType = GetItemContainerType(itemsControl, out var isItemContainer);

      Geometry hitTestGeometry;

      if (typeof(TreeViewItem).IsAssignableFrom(itemContainerType))
      {
        hitTestGeometry = new LineGeometry(new Point(0, position.Y), new Point(itemsControl.RenderSize.Width, position.Y));
      }
      else
      {
        switch (searchDirection)
        {
          case Orientation.Horizontal:
            hitTestGeometry = new LineGeometry(new Point(0, position.Y), new Point(itemsControl.RenderSize.Width, position.Y));
            break;

          case Orientation.Vertical:
            hitTestGeometry = new LineGeometry(new Point(position.X, 0), new Point(position.X, itemsControl.RenderSize.Height));
            break;

          default:
            throw new ArgumentException("Invalid value for searchDirection");
        }
      }

      var hits = new List<DependencyObject>();

      VisualTreeHelper.HitTest(itemsControl, null,
                               result =>
                               {
                                 var itemContainer = isItemContainer
                                                       ? result.VisualHit.GetVisualAncestor(itemContainerType, itemsControl)
                                                       : result.VisualHit.GetVisualAncestor(itemContainerType);
                                 if (itemContainer is not null && !hits.Contains(itemContainer) && ((UIElement)itemContainer).IsVisible == true)
                                 {
                                   hits.Add(itemContainer);
                                 }
                                 return HitTestResultBehavior.Continue;
                               },
                               new GeometryHitTestParameters(hitTestGeometry));

      return GetClosest(itemsControl, hits, position, searchDirection);
    }

    public static Type GetItemContainerType(this ItemsControl itemsControl, out bool isItemContainer)
    {
      ItemsControlInformation info = FindItemsControlInformation(itemsControl);
      // determines if the itemsControl is not a ListView, ListBox or TreeView
      isItemContainer = false;

      if (info is not null)
      {
        return info.ItemContainerType;
      }

      if (typeof(DataGrid).IsAssignableFrom(itemsControl.GetType()))
      {
        return typeof(DataGridRow);
      }

      // There is no safe way to get the item container type for an ItemsControl.
      // First hard-code the types for the common ItemsControls.
      //if (itemsControl.GetType().IsAssignableFrom(typeof(ListView)))
      if (typeof(ListView).IsAssignableFrom(itemsControl.GetType()))
      {
        return typeof(ListViewItem);
      }
      //if (itemsControl.GetType().IsAssignableFrom(typeof(ListBox)))
      else if (typeof(ListBox).IsAssignableFrom(itemsControl.GetType()))
      {
        return typeof(ListBoxItem);
      }
      //else if (itemsControl.GetType().IsAssignableFrom(typeof(TreeView)))
      else if (typeof(TreeView).IsAssignableFrom(itemsControl.GetType()))
      {
        return typeof(TreeViewItem);
      }

      // Otherwise look for the control's ItemsPresenter, get it's child panel and the first
      // child of that *should* be an item container.
      //
      // If the control currently has no items, we're out of luck.
      if (itemsControl.Items.Count > 0)
      {
        var itemsPresenters = itemsControl.GetVisualDescendents<ItemsPresenter>();

        foreach (var itemsPresenter in itemsPresenters)
        {
          var panel = VisualTreeHelper.GetChild(itemsPresenter, 0);
          var itemContainer = VisualTreeHelper.GetChildrenCount(panel) > 0
                                ? VisualTreeHelper.GetChild(panel, 0)
                                : null;

          // Ensure that this actually *is* an item container by checking it with
          // ItemContainerGenerator.
          if (itemContainer is not null &&
              itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer) != -1)
          {
            isItemContainer = true;
            return itemContainer.GetType();
          }
        }
      }

      return null;
    }

    public static Orientation GetItemsPanelOrientation(this ItemsControl itemsControl)
    {
      var itemsPresenter = itemsControl.GetVisualDescendent<ItemsPresenter>();

      if (itemsPresenter is not null)
      {
        var itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0);
        var orientationProperty = itemsPanel.GetType().GetProperty("Orientation", typeof(Orientation));

        if (orientationProperty is not null)
        {
          return (Orientation)orientationProperty.GetValue(itemsPanel, null);
        }
      }

      // Make a guess!
      return Orientation.Vertical;
    }

    public static FlowDirection GetItemsPanelFlowDirection(this ItemsControl itemsControl)
    {
      var itemsPresenter = itemsControl.GetVisualDescendent<ItemsPresenter>();

      if (itemsPresenter is not null)
      {
        var itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0);
        var flowDirectionProperty = itemsPanel.GetType().GetProperty("FlowDirection", typeof(FlowDirection));

        if (flowDirectionProperty is not null)
        {
          return (FlowDirection)flowDirectionProperty.GetValue(itemsPanel, null);
        }
      }

      // Make a guess!
      return FlowDirection.LeftToRight;
    }

    public static void SetSelectedItem(this ItemsControl itemsControl, object item)
    {
      if (itemsControl is MultiSelector)
      {
        ((MultiSelector)itemsControl).SelectedItem = null;
        ((MultiSelector)itemsControl).SelectedItem = item;
      }
      else if (itemsControl is ListBox)
      {
        ((ListBox)itemsControl).SelectedItem = null;
        ((ListBox)itemsControl).SelectedItem = item;
      }
      else if (itemsControl is TreeView)
      {
        // TODO: Select the TreeViewItem
        //((TreeView)itemsControl)
      }
      else if (itemsControl is Selector)
      {
        ((Selector)itemsControl).SelectedItem = null;
        ((Selector)itemsControl).SelectedItem = item;
      }
    }

    public static IEnumerable GetSelectedItems(this ItemsControl itemsControl)
    {
      ItemsControlInformation info = FindItemsControlInformation(itemsControl);

      if (info is not null)
      {
        return info.GetSelectedItems(itemsControl);
      }

      //if (itemsControl.GetType().IsAssignableFrom(typeof(MultiSelector)))
      else if (typeof(MultiSelector).IsAssignableFrom(itemsControl.GetType()))
      {
        return ((MultiSelector)itemsControl).SelectedItems;
      }
      else if (itemsControl is ListBox)
      {
        var listBox = (ListBox)itemsControl;

        if (listBox.SelectionMode == SelectionMode.Single)
        {
          return Enumerable.Repeat(listBox.SelectedItem, 1);
        }
        else
        {
          return listBox.SelectedItems;
        }
      }
      //else if (itemsControl.GetType().IsAssignableFrom(typeof(TreeView)))
      else if (typeof(TreeView).IsAssignableFrom(itemsControl.GetType()))
      {
        return Enumerable.Repeat(((TreeView)itemsControl).SelectedItem, 1);
      }
      //else if (itemsControl.GetType().IsAssignableFrom(typeof(Selector)))
      else if (typeof(Selector).IsAssignableFrom(itemsControl.GetType()))
      {
        return Enumerable.Repeat(((Selector)itemsControl).SelectedItem, 1);
      }
      else
      {
        return Enumerable.Empty<object>();
      }
    }

    public static bool GetItemSelected(this ItemsControl itemsControl, object item)
    {
      ItemsControlInformation info = FindItemsControlInformation(itemsControl);

      if (info is not null)
      {
        return info.GetIsItemSelected(itemsControl, item);
      }
      else if (itemsControl is MultiSelector)
      {
        return ((MultiSelector)itemsControl).SelectedItems.Contains(item);
      }
      else if (itemsControl is ListBox)
      {
        return ((ListBox)itemsControl).SelectedItems.Contains(item);
      }
      else if (itemsControl is TreeView)
      {
        return ((TreeView)itemsControl).SelectedItem == item;
      }
      else if (itemsControl is Selector)
      {
        return ((Selector)itemsControl).SelectedItem == item;
      }
      else
      {
        return false;
      }
    }

    public static void SetItemSelected(this ItemsControl itemsControl, object item, bool value)
    {
      var info = FindItemsControlInformation(itemsControl);

      if (info is not null && info.SetIsItemSelected is not null)
      {
        info.SetIsItemSelected(itemsControl, item, value);
      }
      else if (itemsControl is MultiSelector)
      {
        var multiSelector = (MultiSelector)itemsControl;

        if (value)
        {
          if (multiSelector.CanSelectMultipleItems())
          {
            multiSelector.SelectedItems.Add(item);
          }
          else
          {
            multiSelector.SelectedItem = item;
          }
        }
        else
        {
          multiSelector.SelectedItems.Remove(item);
        }
      }
      else if (itemsControl is ListBox)
      {
        var listBox = (ListBox)itemsControl;

        if (value)
        {
          if (listBox.SelectionMode != SelectionMode.Single)
          {
            listBox.SelectedItems.Add(item);
          }
          else
          {
            listBox.SelectedItem = item;
          }
        }
        else
        {
          listBox.SelectedItems.Remove(item);
        }
      }
    }

    private static UIElement GetClosest(ItemsControl itemsControl, List<DependencyObject> items,
                                        Point position, Orientation searchDirection)
    {
      //Console.WriteLine("GetClosest - {0}", itemsControl.ToString());

      UIElement closest = null;
      var closestDistance = double.MaxValue;
      var info = FindItemsControlInformation(itemsControl);

      Orientation? effectiveSearchDirection = searchDirection;
      if (info is not null && info.GetOrientation is not null)
        effectiveSearchDirection = info.GetOrientation(itemsControl);

      foreach (var i in items)
      {
        var uiElement = i as UIElement;

        if (uiElement is not null)
        {
          var p = uiElement.TransformToAncestor(itemsControl).Transform(new Point(0, 0));
          var distance = double.MaxValue;

          if (effectiveSearchDirection is null)
          {
            var xDiff = position.X - p.X;
            var yDiff = position.Y - p.Y;
            var hyp = Math.Sqrt(Math.Pow(xDiff, 2d) + Math.Pow(yDiff, 2d));
            distance = Math.Abs(hyp);
          }
          else
          {
            switch (effectiveSearchDirection.Value)
            {
              case Orientation.Horizontal:
                distance = Math.Abs(position.X - p.X);
                break;

              case Orientation.Vertical:
                distance = Math.Abs(position.Y - p.Y);
                break;
            }
          }

          if (distance < closestDistance)
          {
            closest = uiElement;
            closestDistance = distance;
          }
        }
      }

      return closest;
    }
  }
}
