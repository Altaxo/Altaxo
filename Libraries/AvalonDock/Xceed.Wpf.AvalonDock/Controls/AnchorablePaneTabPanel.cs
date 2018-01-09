﻿/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
    public class AnchorablePaneTabPanel : Panel
    {
        public AnchorablePaneTabPanel()
        {
            FlowDirection = System.Windows.FlowDirection.LeftToRight;
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            double totWidth = 0;
            double maxHeight = 0;
            var visibleChildren = Children.Cast<UIElement>().Where(ch => ch.Visibility != System.Windows.Visibility.Collapsed);
            foreach (FrameworkElement child in visibleChildren)
            {
                child.Measure(new Size(double.PositiveInfinity, availableSize.Height));
                totWidth += child.DesiredSize.Width;
                maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
            }

            if (totWidth > availableSize.Width)
            {
                double childFinalDesideredWidth = availableSize.Width / visibleChildren.Count();
                foreach (FrameworkElement child in visibleChildren)
                {
                    child.Measure(new Size(childFinalDesideredWidth, availableSize.Height));
                }
            }

            return new Size(Math.Min(availableSize.Width, totWidth), maxHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var visibleChildren = Children.Cast<UIElement>().Where(ch => ch.Visibility != System.Windows.Visibility.Collapsed);


            double finalWidth = finalSize.Width;
            double desideredWidth = visibleChildren.Sum(ch => ch.DesiredSize.Width);
            double offsetX = 0.0;

            if (finalWidth > desideredWidth)
            {
                foreach (FrameworkElement child in visibleChildren)
                {
                    double childFinalWidth = child.DesiredSize.Width ;
                    child.Arrange(new Rect(offsetX, 0, childFinalWidth, finalSize.Height));

                    offsetX += childFinalWidth;
                }
            }
            else
            {
                double childFinalWidth = finalWidth / visibleChildren.Count();
                foreach (FrameworkElement child in visibleChildren)
                {
                    child.Arrange(new Rect(offsetX, 0, childFinalWidth, finalSize.Height));

                    offsetX += childFinalWidth;
                }   
            }

            return finalSize;
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed &&
                LayoutAnchorableTabItem.IsDraggingItem())
            {
                var contentModel = LayoutAnchorableTabItem.GetDraggingItem().Model as LayoutAnchorable;
                var manager = contentModel.Root.Manager;
                LayoutAnchorableTabItem.ResetDraggingItem();

                manager.StartDraggingFloatingWindowForContent(contentModel);
            }

            base.OnMouseLeave(e);
        }
    }
}
