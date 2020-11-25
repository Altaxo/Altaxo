#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  /// <summary>
  /// Interaction logic for GraphicItemsControl.xaml
  /// </summary>
  public partial class GraphicItemsControl : UserControl, IGraphicItemsView
  {
    public event Action? SelectedItemsUp;

    public event Action? SelectedItemsDown;

    public event Action? SelectedItemsRemove;

    public GraphicItemsControl()
    {
      InitializeComponent();
    }

    private void EhItemsSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiItemsList);
    }

    public Collections.SelectableListNodeList ItemsList
    {
      set
      {
        GuiHelper.Initialize(_guiItemsList, value);
      }
    }

    private void EhSelectedItemsUp_Click(object sender, RoutedEventArgs e)
    {
      if (SelectedItemsUp is not null)
        SelectedItemsUp();
    }

    private void EhSelectedItemsDown_Click(object sender, RoutedEventArgs e)
    {
      if (SelectedItemsDown is not null)
        SelectedItemsDown();
    }

    private void EhSelectedItemsRemove_Click(object sender, RoutedEventArgs e)
    {
      if (SelectedItemsRemove is not null)
        SelectedItemsRemove();
    }
  }
}
