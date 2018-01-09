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
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;

namespace Xceed.Wpf.AvalonDock.Controls
{
    public class LayoutGridResizerControl : Thumb
    {
        static LayoutGridResizerControl()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(typeof(LayoutGridResizerControl)));
            HorizontalAlignmentProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
            VerticalAlignmentProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
            BackgroundProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(Brushes.Transparent));
            IsHitTestVisibleProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(true, null));
        }


        #region BackgroundWhileDragging

        /// <summary>
        /// BackgroundWhileDragging Dependency Property
        /// </summary>
        public static readonly DependencyProperty BackgroundWhileDraggingProperty =
            DependencyProperty.Register("BackgroundWhileDragging", typeof(Brush), typeof(LayoutGridResizerControl),
                new FrameworkPropertyMetadata((Brush)Brushes.Black));

        /// <summary>
        /// Gets or sets the BackgroundWhileDragging property.  This dependency property 
        /// indicates ....
        /// </summary>
        public Brush BackgroundWhileDragging
        {
            get { return (Brush)GetValue(BackgroundWhileDraggingProperty); }
            set { SetValue(BackgroundWhileDraggingProperty, value); }
        }

        #endregion

        #region OpacityWhileDragging

        /// <summary>
        /// OpacityWhileDragging Dependency Property
        /// </summary>
        public static readonly DependencyProperty OpacityWhileDraggingProperty =
            DependencyProperty.Register("OpacityWhileDragging", typeof(double), typeof(LayoutGridResizerControl),
                new FrameworkPropertyMetadata((double)0.5));

        /// <summary>
        /// Gets or sets the OpacityWhileDragging property.  This dependency property 
        /// indicates ....
        /// </summary>
        public double OpacityWhileDragging
        {
            get { return (double)GetValue(OpacityWhileDraggingProperty); }
            set { SetValue(OpacityWhileDraggingProperty, value); }
        }

        #endregion
    }
}
