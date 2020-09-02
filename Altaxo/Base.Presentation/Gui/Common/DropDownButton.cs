﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// A button that opens a drop-down menu when clicked.
  /// </summary>
  public class DropDownButton : ButtonBase
  {
    public static readonly DependencyProperty DropDownMenuProperty
      = DependencyProperty.Register("DropDownMenu", typeof(ContextMenu),
                                    typeof(DropDownButton), new FrameworkPropertyMetadata(null));

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    protected static readonly DependencyPropertyKey IsDropDownMenuOpenPropertyKey
      = DependencyProperty.RegisterReadOnly("IsDropDownMenuOpen", typeof(bool),
                                            typeof(DropDownButton), new FrameworkPropertyMetadata(false));

    public static readonly DependencyProperty IsDropDownMenuOpenProperty = IsDropDownMenuOpenPropertyKey.DependencyProperty;

    static DropDownButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));
    }

    public ContextMenu DropDownMenu
    {
      get { return (ContextMenu)GetValue(DropDownMenuProperty); }
      set { SetValue(DropDownMenuProperty, value); }
    }

    public bool IsDropDownMenuOpen
    {
      get { return (bool)GetValue(IsDropDownMenuOpenProperty); }
      protected set { SetValue(IsDropDownMenuOpenPropertyKey, value); }
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      if (DropDownMenu is not null && !IsDropDownMenuOpen)
      {
        DropDownMenu.Placement = PlacementMode.Bottom;
        DropDownMenu.PlacementTarget = this;
        DropDownMenu.IsOpen = true;
        DropDownMenu.Closed += DropDownMenu_Closed;
        IsDropDownMenuOpen = true;
      }
    }

    private void DropDownMenu_Closed(object sender, RoutedEventArgs e)
    {
      ((ContextMenu)sender).Closed -= DropDownMenu_Closed;
      IsDropDownMenuOpen = false;
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      if (!IsMouseCaptured)
      {
        e.Handled = true;
      }
    }
  }
}
