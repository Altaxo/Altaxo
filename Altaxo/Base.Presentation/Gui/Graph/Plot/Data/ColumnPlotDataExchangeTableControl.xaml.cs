#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Graph.Plot.Data
{
  using System.Collections;
  using System.IO;
  using System.Windows.Data;
  using System.Windows.Input;
  using System.Windows.Markup;
  using System.Windows.Media;
  using System.Xml;
  using Altaxo.Collections;
  using Common;
  using GongSolutions.Wpf.DragDrop;
  using Graph.Plot.Data;

  /// <summary>
  /// Interaction logic for ColumnPlotDataControl.xaml
  /// </summary>
  public partial class ColumnPlotDataExchangeTableControl : UserControl, IColumnPlotDataExchangeTableView
  {
    public event Action SelectedTableChanged;

    public event Action SelectedMatchingTableChanged;

    public ColumnPlotDataExchangeTableControl()
    {
      InitializeComponent();
    }

    private void EhTables_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
    {
      if (null != SelectedTableChanged)
      {
        GuiHelper.SynchronizeSelectionFromGui(_cbTables);
        SelectedTableChanged?.Invoke();
      }
    }

    private void EhMatchingTables_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiMatchingTables);
      SelectedMatchingTableChanged?.Invoke();
    }

    public void AvailableTables_Initialize(SelectableListNodeList items)
    {
      GuiHelper.Initialize(_cbTables, items);
    }

    public void MatchingTables_Initialize(SelectableListNodeList items)
    {
      GuiHelper.InitializeDeselectable(_guiMatchingTables, items);
    }

    public void Diagnostics_Initialize(int numberOfPlotItems, int numberOfSuccessfullyChangedColumns, int numberOfUnsuccessfullyChangedColumns)
    {
      string text1, text2, text3;

      if (0 == numberOfPlotItems)
        text1 = "- No plot items with exchanged tables";
      else if (1 == numberOfPlotItems)
        text1 = "- One plot item with an exchanged table";
      else
        text1 = string.Format(Altaxo.Settings.GuiCulture.Instance, "- {0} plot items with exchanged tables", numberOfPlotItems);

      if (0 == numberOfSuccessfullyChangedColumns)
        text2 = null;
      else if (1 == numberOfSuccessfullyChangedColumns)
        text2 = "- One successfully changed column";
      else
        text2 = string.Format(Altaxo.Settings.GuiCulture.Instance, "- {0} successfully changed columns", numberOfSuccessfullyChangedColumns);

      if (0 == numberOfUnsuccessfullyChangedColumns)
        text3 = null;
      else if (1 == numberOfUnsuccessfullyChangedColumns)
        text3 = "- One column could not be replaced!";
      else
        text3 = string.Format(Altaxo.Settings.GuiCulture.Instance, "- {0} columns could not be replaced!", numberOfUnsuccessfullyChangedColumns);

      _guiDiagnosticsNumberOfPlotItems.Text = text1;
      _guiDiagnosticsNumberOfSuccessfullyChangedColumns.Text = text2;
      _guiDiagnosticsNumberOfUnsuccessfullyChangedColumns.Text = text3;

      _guiDiagnosticsNumberOfSuccessfullyChangedColumns.Visibility = text2 == null ? Visibility.Hidden : Visibility.Visible;
      _guiDiagnosticsNumberOfUnsuccessfullyChangedColumns.Visibility = text3 == null ? Visibility.Hidden : Visibility.Visible;
    }
  }
}
