#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Interaction logic for MultiChildControl.xaml
  /// </summary>
  public partial class MultiChildControl : UserControl, IMultiChildView
  {
    /// <summary>Event fired when one of the child controls is entered.</summary>
    public event EventHandler ChildControlEntered;

    /// <summary>Event fired when one of the child controls is validated.</summary>
    public event EventHandler ChildControlValidated;

    public MultiChildControl()
    {
      InitializeComponent();
    }

    public void InitializeBegin()
    {
    }

    public void InitializeEnd()
    {
    }

    public void InitializeLayout(bool horizontalLayout)
    {
      _stackPanel.Orientation = horizontalLayout ? Orientation.Horizontal : Orientation.Vertical;
    }

    public void InitializeDescription(string value)
    {
      _lblDescription.Content = value;
    }

    public void InitializeChilds(ViewDescriptionElement[] childs, int initialFocusedChild)
    {
      foreach (UIElement uiEle in _stackPanel.Children)
      {
        uiEle.GotFocus -= EhUIElement_GotFocus;
        uiEle.LostFocus -= EhUIElement_LostFocus;
      }

      _stackPanel.Children.Clear();
      foreach (var child in childs)
      {
        UIElement uiEle;

        uiEle = (UIElement)child.View;

        uiEle.GotFocus += EhUIElement_GotFocus;
        uiEle.LostFocus += EhUIElement_LostFocus;

        if (!string.IsNullOrEmpty(child.Title))
        {
          var gbox = new GroupBox
          {
            Header = child.Title,
            Content = uiEle
          };
          uiEle = gbox;
        }

        _stackPanel.Children.Add(uiEle);
      }

      _stackPanel.Children[initialFocusedChild].Focus();
    }

    private void EhUIElement_LostFocus(object sender, RoutedEventArgs e)
    {
      if (ChildControlValidated is not null)
        ChildControlValidated(this, EventArgs.Empty);
    }

    private void EhUIElement_GotFocus(object sender, RoutedEventArgs e)
    {
      if (ChildControlEntered is not null)
        ChildControlEntered(this, EventArgs.Empty);
    }
  }
}
