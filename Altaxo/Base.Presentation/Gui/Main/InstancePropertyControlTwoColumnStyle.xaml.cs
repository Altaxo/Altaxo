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

namespace Altaxo.Gui.Main
{
  /// <summary>
  /// Shows a property view with two columns
  /// </summary>
  public partial class InstancePropertyControlTwoColumnStyle : UserControl, IInstancePropertyView
  {
    public InstancePropertyControlTwoColumnStyle()
    {
      InitializeComponent();
    }

    public void InitializeItems(Altaxo.Collections.ListNodeList list)
    {
      int rowsNeeded = (list.Count + 1) >> 1;

      int diff = rowsNeeded - _guiGrid.RowDefinitions.Count;
      for (int i = diff - 1; i >= 0; --i)
        _guiGrid.RowDefinitions.Add(new RowDefinition());

      _guiGrid.Children.Clear();

      int itemIdx = -1;
      foreach (var t in list)
      {
        itemIdx++;
        int column = itemIdx % 2;
        int row = itemIdx >> 1;

        var label = new Label() { Content = t.Text };
        label.SetValue(Grid.ColumnProperty, 2 * column);
        label.SetValue(Grid.RowProperty, row);
        _guiGrid.Children.Add(label);

        var uiElement = (FrameworkElement)t.Tag;
        uiElement.SetValue(Grid.ColumnProperty, 2 * column + 1);
        uiElement.SetValue(Grid.RowProperty, row);
        uiElement.Margin = new Thickness(4);
        _guiGrid.Children.Add(uiElement);
      }
    }
  }
}
