// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Scrolling related helpers.
  /// </summary>
  public static class ScrollUtils
  {
    /// <summary>
    /// Searches VisualTree of given object for a ScrollViewer.
    /// </summary>
    /// <param name="o">The dependency object whose visual tree is searched.</param>
    /// <returns>The ScrollViewer of the object if found; otherwise, null.</returns>
    public static ScrollViewer? GetScrollViewer(this DependencyObject o)
    {
      var scrollViewer = o as ScrollViewer;
      if (scrollViewer is not null)
      {
        return scrollViewer;
      }

      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
      {
        var child = VisualTreeHelper.GetChild(o, i);
        var result = GetScrollViewer(child);
        if (result is not null)
        {
          return result;
        }
      }
      return null;
    }

    /// <summary>
    /// Scrolls ScrollViewer up by given offset.
    /// </summary>
    /// <param name="scrollViewer">The scroll viewer which receives the command.</param>
    /// <param name="offset">Offset by which to scroll up. Should be positive.</param>
    public static void ScrollUp(this ScrollViewer scrollViewer, double offset)
    {
      ScrollUtils.ScrollByVerticalOffset(scrollViewer, -offset);
    }

    /// <summary>
    /// Scrolls ScrollViewer down by given offset.
    /// </summary>
    /// <param name="scrollViewer">The scroll viewer which receives the command.</param>
    /// <param name="offset">Offset by which to scroll down. Should be positive.</param>
    public static void ScrollDown(this ScrollViewer scrollViewer, double offset)
    {
      ScrollUtils.ScrollByVerticalOffset(scrollViewer, offset);
    }

    /// <summary>
    /// Scrolls ScrollViewer by given vertical offset.
    /// </summary>
    /// <param name="scrollViewer">The scroll viewer that is scrolled.</param>
    /// <param name="offset">The vertical offset.</param>
    public static void ScrollByVerticalOffset(this ScrollViewer scrollViewer, double offset)
    {
      scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset);
    }

    /// <summary>
    /// Synchronizes the scroll position of the target with the source.
    /// </summary>
    /// <param name="target">The target scroll viewer.</param>
    /// <param name="source">The source scroll viewer.</param>
    /// <param name="option">The synchronization mode.</param>
    /// <param name="proportional"><see langword="true"/> to synchronize proportionally; otherwise, use the raw offset.</param>
    public static void SynchronizeScroll(this ScrollViewer target, ScrollViewer source, ScrollSyncOption option, bool proportional = true)
    {
      if (source is null)
        throw new ArgumentNullException("source");
      if (target is null)
        throw new ArgumentNullException("target");
      double newScrollOffset;
      switch (option)
      {
        case ScrollSyncOption.Vertical:
          if (proportional)
            newScrollOffset = source.VerticalOffset / source.ScrollableHeight * target.ScrollableHeight;
          else
            newScrollOffset = source.VerticalOffset;
          target.ScrollToVerticalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
          break;

        case ScrollSyncOption.Horizontal:
          if (proportional)
            newScrollOffset = source.HorizontalOffset / source.ScrollableWidth * target.ScrollableWidth;
          else
            newScrollOffset = source.HorizontalOffset;
          target.ScrollToHorizontalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
          break;

        case ScrollSyncOption.VerticalToHorizontal:
          if (proportional)
            newScrollOffset = source.VerticalOffset / source.ScrollableHeight * target.ScrollableWidth;
          else
            newScrollOffset = source.VerticalOffset;
          target.ScrollToHorizontalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
          break;

        case ScrollSyncOption.HorizontalToVertical:
          if (proportional)
            newScrollOffset = source.HorizontalOffset / source.ScrollableWidth * target.ScrollableHeight;
          else
            newScrollOffset = source.HorizontalOffset;
          target.ScrollToVerticalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
          break;

        case ScrollSyncOption.Both:
          if (proportional)
            newScrollOffset = source.VerticalOffset / source.ScrollableHeight * target.ScrollableHeight;
          else
            newScrollOffset = source.VerticalOffset;
          target.ScrollToVerticalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
          if (proportional)
            newScrollOffset = source.HorizontalOffset / source.ScrollableWidth * target.ScrollableWidth;
          else
            newScrollOffset = source.HorizontalOffset;
          target.ScrollToHorizontalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
          break;

        case ScrollSyncOption.BothInterchanged:
          if (proportional)
            newScrollOffset = source.VerticalOffset / source.ScrollableHeight * target.ScrollableWidth;
          else
            newScrollOffset = source.VerticalOffset;
          target.ScrollToHorizontalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
          if (proportional)
            newScrollOffset = source.HorizontalOffset / source.ScrollableWidth * target.ScrollableHeight;
          else
            newScrollOffset = source.HorizontalOffset;
          target.ScrollToVerticalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
          break;

        default:
          throw new Exception("Invalid value for ScrollSyncOption");
      }
    }
  }
}
