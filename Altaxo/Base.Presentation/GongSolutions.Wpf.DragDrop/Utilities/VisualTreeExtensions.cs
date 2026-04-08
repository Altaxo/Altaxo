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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
  /// <summary>
  /// Provides helper methods for navigating the WPF visual tree.
  /// </summary>
  public static class VisualTreeExtensions
  {
    internal static DependencyObject FindVisualTreeRoot(this DependencyObject d)
    {
      var current = d;
      var result = d;

      while (current is not null)
      {
        result = current;
        if (current is Visual || current is Visual3D)
        {
          break;
        }
        else
        {
          // If we're in Logical Land then we must walk
          // up the logical tree until we find a
          // Visual/Visual3D to get us back to Visual Land.
          current = LogicalTreeHelper.GetParent(current);
        }
      }

      return result;
    }

    /// <summary>
    /// Gets the first visual ancestor of the specified type.
    /// </summary>
    /// <typeparam name="T">The ancestor type.</typeparam>
    /// <param name="d">The starting dependency object.</param>
    /// <returns>The matching ancestor, or <see langword="null"/> if none was found.</returns>
    public static T GetVisualAncestor<T>(this DependencyObject d) where T : class
    {
      var item = VisualTreeHelper.GetParent(d.FindVisualTreeRoot());

      while (item is not null)
      {
        var itemAsT = item as T;
        if (itemAsT is not null)
        {
          return itemAsT;
        }
        item = VisualTreeHelper.GetParent(item);
      }

      return null;
    }

    /// <summary>
    /// Gets the first visual ancestor that matches the specified runtime type.
    /// </summary>
    /// <param name="d">The starting dependency object.</param>
    /// <param name="type">The ancestor type to match.</param>
    /// <returns>The matching ancestor, or <see langword="null"/> if none was found.</returns>
    public static DependencyObject GetVisualAncestor(this DependencyObject d, Type type)
    {
      var item = VisualTreeHelper.GetParent(d.FindVisualTreeRoot());

      while (item is not null && type is not null)
      {
        if (item.GetType() == type || item.GetType().IsSubclassOf(type))
        {
          return item;
        }
        item = VisualTreeHelper.GetParent(item);
      }

      return null;
    }

    /// <summary>
    /// find the visual ancestor by type and go through the visual tree until the given itemsControl will be found
    /// </summary>
    public static DependencyObject GetVisualAncestor(this DependencyObject d, Type type, ItemsControl itemsControl)
    {
      var item = VisualTreeHelper.GetParent(d.FindVisualTreeRoot());
      DependencyObject lastFoundItemByType = null;

      while (item is not null && type is not null)
      {
        if (item == itemsControl)
        {
          return lastFoundItemByType;
        }
        if (item.GetType() == type || item.GetType().IsSubclassOf(type))
        {
          lastFoundItemByType = item;
        }
        item = VisualTreeHelper.GetParent(item);
      }

      return lastFoundItemByType;
    }

    /// <summary>
    /// Gets the first visual descendent of the specified type.
    /// </summary>
    /// <typeparam name="T">The descendent type.</typeparam>
    /// <param name="d">The starting dependency object.</param>
    /// <returns>The matching descendent, or <see langword="null"/> if none was found.</returns>
    public static T GetVisualDescendent<T>(this DependencyObject d) where T : DependencyObject
    {
      return d.GetVisualDescendents<T>().FirstOrDefault();
    }

    /// <summary>
    /// Enumerates all visual descendents of the specified type.
    /// </summary>
    /// <typeparam name="T">The descendent type.</typeparam>
    /// <param name="d">The starting dependency object.</param>
    /// <returns>An enumeration of matching descendents.</returns>
    public static IEnumerable<T> GetVisualDescendents<T>(this DependencyObject d) where T : DependencyObject
    {
      var childCount = VisualTreeHelper.GetChildrenCount(d);

      for (var n = 0; n < childCount; n++)
      {
        var child = VisualTreeHelper.GetChild(d, n);

        if (child is T)
        {
          yield return (T)child;
        }

        foreach (var match in GetVisualDescendents<T>(child))
        {
          yield return match;
        }
      }

      yield break;
    }
  }
}
