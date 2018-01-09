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
using Xceed.Wpf.AvalonDock.Layout;
using System.Windows;

namespace Xceed.Wpf.AvalonDock.Controls
{
    public class LayoutDocumentItem : LayoutItem
    {
        LayoutDocument _document;
        internal LayoutDocumentItem()
        {

        }

        internal override void Attach(LayoutContent model)
        {
            _document = model as LayoutDocument;
            base.Attach(model);
        }

        protected override void Close()
        {
          if( (_document.Root != null) && (_document.Root.Manager != null) )
          {
            var dockingManager = _document.Root.Manager;
            dockingManager._ExecuteCloseCommand( _document );
          }
        }

        #region Description

        /// <summary>
        /// Description Dependency Property
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(LayoutDocumentItem),
                new FrameworkPropertyMetadata((string)null,
                    new PropertyChangedCallback(OnDescriptionChanged)));

        /// <summary>
        /// Gets or sets the Description property.  This dependency property 
        /// indicates the description to display for the document item.
        /// </summary>
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Description property.
        /// </summary>
        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutDocumentItem)d).OnDescriptionChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Description property.
        /// </summary>
        protected virtual void OnDescriptionChanged(DependencyPropertyChangedEventArgs e)
        {
            _document.Description = (string)e.NewValue;
        }

        #endregion

        internal override void Detach()
        {
            _document = null;
            base.Detach();
        }
    }
}
