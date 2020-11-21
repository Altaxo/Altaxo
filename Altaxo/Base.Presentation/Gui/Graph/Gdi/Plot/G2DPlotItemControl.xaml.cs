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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Gdi.Plot
{
  /// <summary>
  /// Interaction logic for G2DPlotItemControl.xaml
  /// </summary>
  public partial class G2DPlotItemControl : UserControl, IG2DPlotItemView
  {
    public G2DPlotItemControl()
    {
      InitializeComponent();
    }

    private void EhTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.OriginalSource == _tabControl)
      {
        TabItem newItem = null;
        TabItem oldItem = null;

        if (e.AddedItems.Count > 0)
          newItem = (TabItem)e.AddedItems[0];
        if (e.RemovedItems.Count > 0)
          oldItem = (TabItem)e.RemovedItems[0];

        SelectedPage_Changed?.Invoke(this, new Altaxo.Main.InstanceChangedEventArgs(oldItem?.Content, newItem?.Content));
      }
    }

    #region IG2DPlotItemView

    public void ClearTabs()
    {
      // decouple controls from the tabitems
      foreach (TabItem tabItem in _tabControl.Items)
        tabItem.Content = null;
      _tabControl.Items.Clear();
    }

    public void AddTab(string title, object view)
    {
      var tabItem = new TabItem() { Header = title, Content = view };
      _tabControl.Items.Add(tabItem);
    }

    public void BringTabToFront(int index)
    {
      _tabControl.SelectedIndex = index;
    }

    public event EventHandler<Altaxo.Main.InstanceChangedEventArgs>? SelectedPage_Changed;

    public void SetPlotStyleView(object view)
    {
      _plotStyleCollectionControlHost.Content = (UIElement)view;
    }

    public void SetPlotGroupCollectionView(object view)
    {
      _plotGroupCollectionControlHost.Content = (UIElement)view;
    }

    #endregion IG2DPlotItemView
  }
}
