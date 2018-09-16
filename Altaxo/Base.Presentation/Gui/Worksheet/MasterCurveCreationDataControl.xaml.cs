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
using Altaxo.Collections;

namespace Altaxo.Gui.Worksheet
{
  /// <summary>
  /// Interaction logic for MasterCurveCreationDataControl.xaml
  /// </summary>
  public partial class MasterCurveCreationDataControl : UserControl, IMasterCurveCreationDataView
  {
    private List<MasterCurveCreationDataColumnControl> _columns = new List<MasterCurveCreationDataColumnControl>();

    public MasterCurveCreationDataControl()
    {
      InitializeComponent();
    }

    private void AddRemoveGroups(int numberOfGroups)
    {
      if (_columns.Count > numberOfGroups)
      {
        for (int i = _columns.Count - 1; i >= numberOfGroups; i--)
        {
          _dataGrid.ColumnDefinitions.RemoveAt(_dataGrid.ColumnDefinitions.Count - 1);
          _dataGrid.Children.Remove(_columns[i]);
          _columns.RemoveAt(i);
        }
      }
      else if (_columns.Count < numberOfGroups)
      {
        for (int i = _columns.Count; i < numberOfGroups; i++)
        {
          var ele = new MasterCurveCreationDataColumnControl();
          ele.SetValue(Grid.ColumnProperty, i + 1);
          ele.SetValue(Grid.RowProperty, 0);
          _columns.Add(ele);
          _dataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
          _dataGrid.Children.Add(ele);
        }
      }
    }

    public void InitializeListData(List<SelectableListNodeList> list)
    {
      AddRemoveGroups(list.Count);

      for (int srcGroupIdx = 0; srcGroupIdx < list.Count; srcGroupIdx++)
      {
        var srcGroup = list[srcGroupIdx];
        var lb = _columns[srcGroupIdx];
        GuiHelper.Initialize(lb.ItemList, srcGroup);
      }
    }
  }
}
