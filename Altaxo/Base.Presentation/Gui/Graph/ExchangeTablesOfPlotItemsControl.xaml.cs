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
using System.Windows.Input;

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Interaction logic for ExchangeTablesOfPlotItemsControl.xaml
  /// </summary>
  public partial class ExchangeTablesOfPlotItemsControl : UserControl, IExchangeTablesOfPlotItemsView
  {
    public event Action ChooseTableForSelectedItems;

    public event Action ChooseFolderForSelectedItems;

    public event Action TableSelectionChanged;

    public event Action ListOfCommonSubstringsSelectionChanged;

    public event Action ApplySubstringReplacement;

    public event Action CommonSubstringTextChanged;

    public event Action ListOfSubstringReplacementCandidatesSelectionChanged;

    public event Action SearchCommonSubstringsCharacterWiseChanged;

    public event Action CommonSubstringPanelVisibilityChanged;

    public ExchangeTablesOfPlotItemsControl()
    {
      InitializeComponent();
    }

    public void InitializeExchangeTableList(Collections.SelectableListNodeList list)
    {
      GuiHelper.Initialize(_guiTableList, list);
    }

    public void InitializeListOfCommonSubstrings(Collections.SelectableListNodeList list)
    {
      GuiHelper.Initialize(_guiListOfCommonSubstrings, list);
    }

    public void InitializeListOfReplacementCandidates(Collections.SelectableListNodeList list)
    {
      GuiHelper.Initialize(_guiListOfReplacementCandidates, list);
    }

    public string CommonSubstringText { get { return _guiCommonSubstring.Text; } set { _guiCommonSubstring.Text = value; } }

    private void EhTableList_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
    }

    private void EhChooseTable(object sender, RoutedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiTableList);
      if (null != ChooseTableForSelectedItems)
        ChooseTableForSelectedItems();
    }

    private void EhChooseFolder(object sender, RoutedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiTableList);
      if (null != ChooseFolderForSelectedItems)
        ChooseFolderForSelectedItems();
    }

    private void EhItemsSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiTableList);
      if (null != TableSelectionChanged)
      {
        TableSelectionChanged();
      }
    }

    private void EhListOfCommonSubstringsSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiListOfCommonSubstrings);
      if (null != ListOfCommonSubstringsSelectionChanged)
        ListOfCommonSubstringsSelectionChanged();
    }

    private void EhListOfReplacementCandidatesSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiListOfReplacementCandidates);

      if (null != _guiListOfReplacementCandidates.SelectedItem)
        _guiReplacementCandidate.Text = _guiListOfReplacementCandidates.SelectedItem.ToString();
      else
        _guiReplacementCandidate.Text = null;

      if (null != ListOfSubstringReplacementCandidatesSelectionChanged)
        ListOfSubstringReplacementCandidatesSelectionChanged();
    }

    private void EhApplyReplacementForCommonSubstring(object sender, RoutedEventArgs e)
    {
      if (null != ApplySubstringReplacement)
        ApplySubstringReplacement();
    }

    private void EhCommonSubstringTextChanged(object sender, TextChangedEventArgs e)
    {
      if (null != CommonSubstringTextChanged)
        CommonSubstringTextChanged();
    }

    public bool SearchCommonSubstringsCharacterWise
    {
      get
      {
        return true == _guiSearchCommonSubstringCharacterwise.IsChecked;
      }
      set
      {
        _guiSearchCommonSubstringCharacterwise.IsChecked = value;
        _guiSearchCommonSubstringSubfolderwise.IsChecked = !value;
      }
    }

    private void EhSearchCommonSubstringCharacterWiseChanged(object sender, RoutedEventArgs e)
    {
      if (SearchCommonSubstringsCharacterWiseChanged != null)
        SearchCommonSubstringsCharacterWiseChanged();
    }

    private void EhCommonSubstringOperations_VisibilityChanged(object sender, RoutedEventArgs e)
    {
      if (null != CommonSubstringPanelVisibilityChanged)
        CommonSubstringPanelVisibilityChanged();
    }

    public bool IsCommonSubstringPanelVisible
    {
      get { return _guiCommonSubstringPanel.IsExpanded; }
    }
  }
}
