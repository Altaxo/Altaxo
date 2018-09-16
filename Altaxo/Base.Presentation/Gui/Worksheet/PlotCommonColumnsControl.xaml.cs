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
  /// Interaction logic for PlotCommonColumnsControl.xaml
  /// </summary>
  public partial class PlotCommonColumnsControl : UserControl, IPlotCommonColumnsView
  {
    public PlotCommonColumnsControl()
    {
      InitializeComponent();
    }

    public bool UseCurrentXColumn
    {
      get
      {
        return true == _guiUseXColumnCurrent.IsChecked;
      }
      set
      {
        _guiUseXColumnCurrent.IsChecked = value;
        _guiUseXColumnUserDefined.IsChecked = !value;
        _guiCommonXColumn.IsEnabled = !value;
      }
    }

    public void InitializeXCommonColumns(SelectableListNodeList list)
    {
      GuiHelper.Initialize(_guiCommonXColumn, list);
    }

    public void InitializeYCommonColumns(SelectableListNodeList list)
    {
      GuiHelper.Initialize(_guiCommonYColumns, list);
    }

    private void EhUseCurrentXColumnChecked(object sender, RoutedEventArgs e)
    {
      _guiCommonXColumn.IsEnabled = false;
    }

    private void EhUseUserDefinedXColumnChecked(object sender, RoutedEventArgs e)
    {
      _guiCommonXColumn.IsEnabled = true;
    }

    private void EhXColumnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiCommonXColumn);
    }

    private void EhYColumnsSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiCommonYColumns);
    }
  }
}
