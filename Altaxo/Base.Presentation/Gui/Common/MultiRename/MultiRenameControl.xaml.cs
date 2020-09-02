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
using System.Windows.Controls;

namespace Altaxo.Gui.Common.MultiRename
{
  /// <summary>
  /// Interaction logic for MultiRenameControl.xaml
  /// </summary>
  public partial class MultiRenameControl : UserControl, IMultiRenameView
  {
    public MultiRenameControl()
    {
      InitializeComponent();
    }

    private void EhRenameTextChanged(object sender, TextChangedEventArgs e)
    {
      if (RenameStringTemplateChanged is not null)
        RenameStringTemplateChanged();
    }

    public void InitializeItemListColumns(string[] columnHeaders)
    {
      GuiHelper.InitializeListViewColumnsAndBindToListNode(_guiItemList, columnHeaders);
    }

    public void InitializeItemListItems(Collections.ListNodeList list)
    {
      _guiItemList.ItemsSource = null;
      _guiItemList.ItemsSource = list;
    }

    public void InitializeAvailableShortcuts(Collections.ListNodeList list)
    {
      _guiAvailableShortcuts.ItemsSource = null;
      _guiAvailableShortcuts.ItemsSource = list;
    }

    public string RenameStringTemplate
    {
      get
      {
        return _guiRenameText.Text;
      }
      set
      {
        _guiRenameText.Text = value;
      }
    }

    public event Action RenameStringTemplateChanged;

    /// <summary>
    /// Sets a value indicating whether the button to choose the base directory should be visible.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the button to choose the base directory is visible; otherwise, <c>false</c>.
    /// </value>
    public bool IsBaseDirectoryButtonVisible
    {
      set
      {
        _guiChooseBaseDirectory.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
      }
    }

    public event Action<string> BaseDirectoryChosen;

    private void EhChooseBaseDirectory(object sender, System.Windows.RoutedEventArgs e)
    {
      var dlg = new System.Windows.Forms.FolderBrowserDialog();
      dlg.ShowNewFolderButton = true;
      dlg.Description = "Choose base folder";
      if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        BaseDirectoryChosen?.Invoke(dlg.SelectedPath);
      }
    }
  }
}
