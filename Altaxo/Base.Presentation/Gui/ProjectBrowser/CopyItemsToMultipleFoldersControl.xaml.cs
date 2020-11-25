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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.ProjectBrowser
{
  /// <summary>
  /// Interaction logic for CopyItemsToMultipleFolderControl.xaml
  /// </summary>
  public partial class CopyItemsToMultipleFolderControl : UserControl, ICopyItemsToMultipleFolderView
  {
    public event Action? CopySelectedFolderNames;

    public event Action? UnselectAllFolders;

    public CopyItemsToMultipleFolderControl()
    {
      InitializeComponent();
    }

    public void InitializeFolderTree(Collections.NGTreeNode rootNode)
    {
      _guiProjectFolders.ItemsSource = new Altaxo.Collections.NGTreeNode("", new[] { rootNode }).Nodes;
    }

    public bool RelocateReferences
    {
      get
      {
        return _guiRelocateReferences.IsChecked == true;
      }
      set
      {
        _guiRelocateReferences.IsChecked = value;
      }
    }

    public bool OverwriteExistingItems
    {
      get
      {
        return _guiOverwriteExistingItems.IsChecked == true;
      }
      set
      {
        _guiOverwriteExistingItems.IsChecked = value;
      }
    }

    public string AdditionalFoldersLineByLine
    {
      get { return _guiAdditionalFolders.Text; }
    }

    private void EhCommand_CopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = _guiProjectFolders.SelectedTreeViewItems is not null && _guiProjectFolders.SelectedTreeViewItems.Count > 0;
      e.Handled = true;
    }

    private void EhCommand_CopyExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      var ev = CopySelectedFolderNames;
      if (ev is not null)
        ev();
    }

    private void EhCommand_DeleteCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = _guiProjectFolders.SelectedTreeViewItems is not null && _guiProjectFolders.SelectedTreeViewItems.Count > 0;
      e.Handled = true;
    }

    private void EhCommand_DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      var ev = UnselectAllFolders;
      if (ev is not null)
        ev();
    }
  }
}
