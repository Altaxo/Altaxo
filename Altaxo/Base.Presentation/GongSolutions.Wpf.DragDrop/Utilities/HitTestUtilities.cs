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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#if NET35
using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
#else

using System.Windows.Controls.Primitives;

#endif

namespace GongSolutions.Wpf.DragDrop.Utilities
{
  /// <summary>
  /// Provides helper methods for hit-testing drag and drop targets.
  /// </summary>
  public static class HitTestUtilities
  {
    /// <summary>
    /// Determines whether the hit-tested element is of the specified type and visible.
    /// </summary>
    /// <typeparam name="T">The expected UI element type.</typeparam>
    /// <param name="sender">The visual to test.</param>
    /// <param name="elementPosition">The hit-test position.</param>
    /// <returns><see langword="true"/> if a visible element of the specified type is hit; otherwise, <see langword="false"/>.</returns>
    public static bool HitTest4Type<T>(object sender, Point elementPosition) where T : UIElement
    {
      var uiElement = GetHitTestElement4Type<T>(sender, elementPosition);
      return uiElement is not null && uiElement.Visibility == Visibility.Visible;
    }

    /// <summary>
    /// Gets the hit-tested element of the specified type.
    /// </summary>
    /// <typeparam name="T">The expected UI element type.</typeparam>
    /// <param name="sender">The visual to test.</param>
    /// <param name="elementPosition">The hit-test position.</param>
    /// <returns>The matching element, or <see langword="null"/> if none was found.</returns>
    private static T GetHitTestElement4Type<T>(object sender, Point elementPosition) where T : UIElement
    {
      var visual = sender as Visual;
      if (visual is null)
      {
        return null;
      }
      var hit = VisualTreeHelper.HitTest(visual, elementPosition);
      if (hit is null)
      {
        return null;
      }
      var uiElement = hit.VisualHit.GetVisualAncestor<T>();
      return uiElement;
    }

    /// <summary>
    /// Determines whether the hit-test position is over a grid view column header.
    /// </summary>
    /// <param name="sender">The visual to test.</param>
    /// <param name="elementPosition">The hit-test position.</param>
    /// <returns><see langword="true"/> if the position is over a visible grid view column header; otherwise, <see langword="false"/>.</returns>
    public static bool HitTest4GridViewColumnHeader(object sender, Point elementPosition)
    {
      if (sender is ListView)
      {
        // no drag&drop for column header
        var columnHeader = GetHitTestElement4Type<GridViewColumnHeader>(sender, elementPosition);
        if (columnHeader is not null && (columnHeader.Role == GridViewColumnHeaderRole.Floating || columnHeader.Visibility == Visibility.Visible))
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Determines whether the hit-test position is over a data grid element that should suppress drag and drop.
    /// </summary>
    /// <param name="sender">The visual to test.</param>
    /// <param name="elementPosition">The hit-test position.</param>
    /// <returns><see langword="true"/> if drag and drop should be suppressed; otherwise, <see langword="false"/>.</returns>
    public static bool HitTest4DataGridTypes(object sender, Point elementPosition)
    {
      if (sender is DataGrid)
      {
        // no drag&drop for column header
        var columnHeader = GetHitTestElement4Type<DataGridColumnHeader>(sender, elementPosition);
        if (columnHeader is not null && columnHeader.Visibility == Visibility.Visible)
        {
          return true;
        }
        // no drag&drop for row header
        var rowHeader = GetHitTestElement4Type<DataGridRowHeader>(sender, elementPosition);
        if (rowHeader is not null && rowHeader.Visibility == Visibility.Visible)
        {
          return true;
        }
        // drag&drop only for data grid row
        var dataRow = GetHitTestElement4Type<DataGridRow>(sender, elementPosition);
        return dataRow is null;
      }
      return false;
    }

    /// <summary>
    /// Determines whether the hit-test position is over a data grid element that should suppress drag-over handling.
    /// </summary>
    /// <param name="sender">The visual to test.</param>
    /// <param name="elementPosition">The hit-test position.</param>
    /// <returns><see langword="true"/> if drag-over handling should be suppressed; otherwise, <see langword="false"/>.</returns>
    public static bool HitTest4DataGridTypesOnDragOver(object sender, Point elementPosition)
    {
      if (sender is DataGrid)
      {
        // no drag&drop on column header
        var columnHeader = GetHitTestElement4Type<DataGridColumnHeader>(sender, elementPosition);
        if (columnHeader is not null && columnHeader.Visibility == Visibility.Visible)
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// thx to @osicka from issue #84
    ///
    /// e.g. original source is part of a popup (e.g. ComboBox drop down), the hit test needs to be done on the original source.
    /// Because the popup is not attached to the visual tree of the sender.
    /// This function test this by looping back from the original source to the sender and if it didn't end up at the sender stopped the drag.
    /// </summary>
    /// <summary>
    /// Determines whether the original source is outside the sender's visual tree.
    /// </summary>
    /// <param name="sender">The visual to test.</param>
    /// <param name="e">The mouse event arguments.</param>
    /// <returns><see langword="true"/> if the original source is not part of the sender; otherwise, <see langword="false"/>.</returns>
    public static bool IsNotPartOfSender(object sender, MouseButtonEventArgs e)
    {
      var visual = e.OriginalSource as Visual;
      if (visual is null)
      {
        return false;
      }
      var hit = VisualTreeHelper.HitTest(visual, e.GetPosition((IInputElement)visual));

      if (hit is null)
      {
        return false;
      }
      else
      {
        var depObj = e.OriginalSource as DependencyObject;
        if (depObj is null)
        {
          return false;
        }
        var item = VisualTreeHelper.GetParent(depObj.FindVisualTreeRoot());
        //var item = VisualTreeHelper.GetParent(e.OriginalSource as DependencyObject);

        while (item is not null && item != sender)
        {
          item = VisualTreeHelper.GetParent(item);
        }
        return item != sender;
      }
    }
  }
}
