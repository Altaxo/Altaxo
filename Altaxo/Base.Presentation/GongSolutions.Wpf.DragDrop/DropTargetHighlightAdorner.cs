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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop
{
  public class DropTargetHighlightAdorner : DropTargetAdorner
  {
    public DropTargetHighlightAdorner(UIElement adornedElement)
      : base(adornedElement)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      var visualTargetItem = DropInfo.VisualTargetItem;
      if (visualTargetItem is not null)
      {
        var rect = Rect.Empty;

        var tvItem = visualTargetItem as TreeViewItem;
        if (tvItem is not null && VisualTreeHelper.GetChildrenCount(tvItem) > 0)
        {
          var descendant = VisualTreeHelper.GetDescendantBounds(tvItem);
          rect = new Rect(tvItem.TranslatePoint(new Point(), AdornedElement), new Size(descendant.Width + 4, tvItem.ActualHeight));
        }
        if (rect.IsEmpty)
        {
          rect = new Rect(visualTargetItem.TranslatePoint(new Point(), AdornedElement), VisualTreeHelper.GetDescendantBounds(visualTargetItem).Size);
        }
        drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Gray, 2), rect, 2, 2);
      }
    }
  }
}
