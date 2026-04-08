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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace GongSolutions.Wpf.DragDrop
{
  /// <summary>
  /// Provides the default implementation for validating and applying drop operations.
  /// </summary>
  public class DefaultDropHandler : IDropTarget
  {
    /// <inheritdoc/>
    public virtual void DragOver(IDropInfo dropInfo)
    {
      if (CanAcceptData(dropInfo))
      {
        dropInfo.Effects = DragDropEffects.Copy;
        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
      }
    }

    /// <summary>
    /// Handles the drop operation and inserts the dragged data into the target collection.
    /// </summary>
    /// <param name="dropInfo">The drop information.</param>
    public virtual void Drop(IDropInfo dropInfo)
    {
      var insertIndex = dropInfo.InsertIndex;
      var destinationList = GetList(dropInfo.TargetCollection);
      var data = ExtractData(dropInfo.Data);

      if (dropInfo.DragInfo.VisualSource == dropInfo.VisualTarget)
      {
        var sourceList = GetList(dropInfo.DragInfo.SourceCollection);

        foreach (var o in data)
        {
          var index = sourceList.IndexOf(o);

          if (index != -1)
          {
            sourceList.RemoveAt(index);

            if (sourceList == destinationList && index < insertIndex)
            {
              --insertIndex;
            }
          }
        }
      }

      foreach (var o in data)
      {
        destinationList.Insert(insertIndex++, o);
      }
    }

    /// <summary>
    /// Determines whether the specified drop target can accept the dragged data.
    /// </summary>
    /// <param name="dropInfo">The current drop information.</param>
    /// <returns><see langword="true"/> if the target can accept the data; otherwise, <see langword="false"/>.</returns>
    public static bool CanAcceptData(IDropInfo dropInfo)
    {
      if (dropInfo is null || dropInfo.DragInfo is null)
      {
        return false;
      }

      if (dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection)
      {
        return GetList(dropInfo.TargetCollection) is not null;
      }
      else if (dropInfo.DragInfo.SourceCollection is ItemCollection)
      {
        return false;
      }
      else if (dropInfo.TargetCollection is null)
      {
        return false;
      }
      else
      {
        if (TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data))
        {
          return !IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
        }
        else
        {
          return false;
        }
      }
    }

    /// <summary>
    /// Extracts the enumerable sequence of dragged items from the supplied drag payload.
    /// </summary>
    /// <param name="data">The drag payload.</param>
    /// <returns>An enumerable sequence of dragged items.</returns>
    public static IEnumerable ExtractData(object data)
    {
      if (data is IEnumerable && !(data is string))
      {
        return (IEnumerable)data;
      }
      else
      {
        return Enumerable.Repeat(data, 1);
      }
    }

    /// <summary>
    /// Gets the writable list backing the specified enumerable target collection.
    /// </summary>
    /// <param name="enumerable">The enumerable collection to inspect.</param>
    /// <returns>The writable list if available; otherwise, <see langword="null"/>.</returns>
    public static IList GetList(IEnumerable enumerable)
    {
      if (enumerable is ICollectionView)
      {
        return ((ICollectionView)enumerable).SourceCollection as IList;
      }
      else
      {
        return enumerable as IList;
      }
    }

    /// <summary>
    /// Determines whether the target item is a child of the source item.
    /// </summary>
    /// <param name="targetItem">The potential child item.</param>
    /// <param name="sourceItem">The potential parent item.</param>
    /// <returns><see langword="true"/> if the target item is a child of the source item; otherwise, <see langword="false"/>.</returns>
    protected static bool IsChildOf(UIElement targetItem, UIElement sourceItem)
    {
      var parent = ItemsControl.ItemsControlFromItemContainer(targetItem);

      while (parent is not null)
      {
        if (parent == sourceItem)
        {
          return true;
        }

        parent = ItemsControl.ItemsControlFromItemContainer(parent);
      }

      return false;
    }

    /// <summary>
    /// Determines whether the dragged data is compatible with the target collection type.
    /// </summary>
    /// <param name="target">The target collection.</param>
    /// <param name="data">The dragged data.</param>
    /// <returns><see langword="true"/> if the data can be inserted into the target collection; otherwise, <see langword="false"/>.</returns>
    protected static bool TestCompatibleTypes(IEnumerable target, object data)
    {
      TypeFilter filter = (t, o) =>
      {
        return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
      };

      var enumerableInterfaces = target.GetType().FindInterfaces(filter, null);
      var enumerableTypes = from i in enumerableInterfaces select i.GetGenericArguments().Single();

      if (enumerableTypes.Count() > 0)
      {
        var dataType = TypeUtilities.GetCommonBaseClass(ExtractData(data));
        return enumerableTypes.Any(t => t.IsAssignableFrom(dataType));
      }
      else
      {
        return target is IList;
      }
    }
  }
}
