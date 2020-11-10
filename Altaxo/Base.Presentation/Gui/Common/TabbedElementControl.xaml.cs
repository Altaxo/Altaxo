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
  /// Interaction logic for TabbedElementControl.xaml
  /// </summary>
  public partial class TabbedElementControl : UserControl, ITabbedElementView
  {
    public TabbedElementControl()
    {
      InitializeComponent();
    }

    public void ClearTabs()
    {
      foreach (TabItem item in _tabControl.Items)
      {
        item.LostFocus -= item_LostFocus;
        item.GotFocus -= item_GotFocus;
        item.Content = null;
      }

      _tabControl.Items.Clear();
    }

    public void AddTab(string title, object view)
    {
      var item = new TabItem
      {
        Header = title,

        Content = (UIElement)view
      };

      item.LostFocus += item_LostFocus;
      item.GotFocus += item_GotFocus;

      _tabControl.Items.Add(item);
    }

    private void item_GotFocus(object sender, RoutedEventArgs e)
    {
      if (ChildControl_Entered is not null)
        ChildControl_Entered(sender, EventArgs.Empty);
    }

    private void item_LostFocus(object sender, RoutedEventArgs e)
    {
      if (ChildControl_Validated is not null)
        ChildControl_Validated(sender, EventArgs.Empty);
    }

    public void BringTabToFront(int index)
    {
      _tabControl.SelectedIndex = index;
      var selItem = _tabControl.SelectedItem as TabItem;
      if (selItem is not null)
        selItem.Focus();
    }

    public event EventHandler? ChildControl_Entered;

    public event EventHandler? ChildControl_Validated;
  }
}
