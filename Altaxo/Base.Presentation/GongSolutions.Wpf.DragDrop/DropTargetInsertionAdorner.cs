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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#if NET35
using GongSolutions.Wpf.DragDrop.Utilities;
#endif

namespace GongSolutions.Wpf.DragDrop
{
  public class DropTargetInsertionAdorner : DropTargetAdorner
  {
    public DropTargetInsertionAdorner(UIElement adornedElement)
      : base(adornedElement)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      var itemsControl = DropInfo.VisualTarget as ItemsControl;

      if (itemsControl is not null)
      {
        // Get the position of the item at the insertion index. If the insertion point is
        // to be after the last item, then get the position of the last item and add an
        // offset later to draw it at the end of the list.
        ItemsControl itemParent;

        if (DropInfo.VisualTargetItem is not null)
        {
          itemParent = ItemsControl.ItemsControlFromItemContainer(DropInfo.VisualTargetItem);
        }
        else
        {
          itemParent = itemsControl;
        }

        var index = Math.Min(DropInfo.InsertIndex, itemParent.Items.Count - 1);

        var lastItemInGroup = false;
        var targetGroup = DropInfo.TargetGroup;
        if (targetGroup is not null && targetGroup.IsBottomLevel && DropInfo.InsertPosition.HasFlag(RelativeInsertPosition.AfterTargetItem))
        {
          var indexOf = targetGroup.Items.IndexOf(DropInfo.TargetItem);
          lastItemInGroup = indexOf == targetGroup.ItemCount - 1;
          if (lastItemInGroup)
          {
            index--;
          }
        }

        var itemContainer = (UIElement)itemParent.ItemContainerGenerator.ContainerFromIndex(index);

        if (itemContainer is not null)
        {
          var itemRect = new Rect(itemContainer.TranslatePoint(new Point(), AdornedElement), itemContainer.RenderSize);
          Point point1, point2;
          double rotation = 0;

          if (DropInfo.VisualTargetOrientation == Orientation.Vertical)
          {
            if (DropInfo.InsertIndex == itemParent.Items.Count || lastItemInGroup)
            {
              itemRect.Y += itemContainer.RenderSize.Height;
            }

            point1 = new Point(itemRect.X, itemRect.Y);
            point2 = new Point(itemRect.Right, itemRect.Y);
          }
          else
          {
            var itemRectX = itemRect.X;

            if (DropInfo.VisualTargetFlowDirection == FlowDirection.LeftToRight && DropInfo.InsertIndex == itemParent.Items.Count)
            {
              itemRectX += itemContainer.RenderSize.Width;
            }
            else if (DropInfo.VisualTargetFlowDirection == FlowDirection.RightToLeft && DropInfo.InsertIndex != itemParent.Items.Count)
            {
              itemRectX += itemContainer.RenderSize.Width;
            }

            point1 = new Point(itemRectX, itemRect.Y);
            point2 = new Point(itemRectX, itemRect.Bottom);
            rotation = 90;
          }

          drawingContext.DrawLine(m_Pen, point1, point2);
          DrawTriangle(drawingContext, point1, rotation);
          DrawTriangle(drawingContext, point2, 180 + rotation);
        }
      }
    }

    private void DrawTriangle(DrawingContext drawingContext, Point origin, double rotation)
    {
      drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
      drawingContext.PushTransform(new RotateTransform(rotation));

      drawingContext.DrawGeometry(m_Pen.Brush, null, m_Triangle);

      drawingContext.Pop();
      drawingContext.Pop();
    }

    static DropTargetInsertionAdorner()
    {
      // Create the pen and triangle in a static constructor and freeze them to improve performance.
      const int triangleSize = 5;

      m_Pen = new Pen(Brushes.Gray, 2);
      m_Pen.Freeze();

      var firstLine = new LineSegment(new Point(0, -triangleSize), false);
      firstLine.Freeze();
      var secondLine = new LineSegment(new Point(0, triangleSize), false);
      secondLine.Freeze();

      var figure = new PathFigure { StartPoint = new Point(triangleSize, 0) };
      figure.Segments.Add(firstLine);
      figure.Segments.Add(secondLine);
      figure.Freeze();

      m_Triangle = new PathGeometry();
      m_Triangle.Figures.Add(figure);
      m_Triangle.Freeze();
    }

    private static readonly Pen m_Pen;
    private static readonly PathGeometry m_Triangle;
  }
}
