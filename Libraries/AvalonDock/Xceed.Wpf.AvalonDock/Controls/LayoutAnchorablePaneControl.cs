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
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
  public class LayoutAnchorablePaneControl : TabControl, ILayoutControl//, ILogicalChildrenContainer
  {
    static LayoutAnchorablePaneControl()
    {
      FocusableProperty.OverrideMetadata( typeof( LayoutAnchorablePaneControl ), new FrameworkPropertyMetadata( false ) );
    }

    public LayoutAnchorablePaneControl( LayoutAnchorablePane model )
    {
      if( model == null )
        throw new ArgumentNullException( "model" );

      _model = model;

      SetBinding( ItemsSourceProperty, new Binding( "Model.Children" ) { Source = this } );
      SetBinding( FlowDirectionProperty, new Binding( "Model.Root.Manager.FlowDirection" ) { Source = this } );

      this.LayoutUpdated += new EventHandler( OnLayoutUpdated );
    }

    void OnLayoutUpdated( object sender, EventArgs e )
    {
      var modelWithAtcualSize = _model as ILayoutPositionableElementWithActualSize;
      modelWithAtcualSize.ActualWidth = ActualWidth;
      modelWithAtcualSize.ActualHeight = ActualHeight;
    }

    LayoutAnchorablePane _model;

    public ILayoutElement Model
    {
      get
      {
        return _model;
      }
    }

    protected override void OnGotKeyboardFocus( System.Windows.Input.KeyboardFocusChangedEventArgs e )
    {
      if( ( _model != null ) && ( _model.SelectedContent != null ) )
      {
        _model.SelectedContent.IsActive = true;
      }

      base.OnGotKeyboardFocus( e );
    }

    protected override void OnMouseLeftButtonDown( System.Windows.Input.MouseButtonEventArgs e )
    {
      base.OnMouseLeftButtonDown( e );

      if( !e.Handled && ( _model != null ) && ( _model.SelectedContent != null ) )
      {
        _model.SelectedContent.IsActive = true;
      }
    }

    protected override void OnMouseRightButtonDown( System.Windows.Input.MouseButtonEventArgs e )
    {
      base.OnMouseRightButtonDown( e );

      if( !e.Handled && ( _model != null ) && ( _model.SelectedContent != null ) )
      {
        _model.SelectedContent.IsActive = true;
      }
    }

  }

}
