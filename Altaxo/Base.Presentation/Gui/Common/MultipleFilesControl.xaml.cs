#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2019 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Interaction logic for RealFourierTransformation2DDataSourceControl.xaml
  /// </summary>
  public partial class MultipleFilesControl : UserControl, IMultipleFilesView
  {
    /// <inheritdoc/>
    public event Action? BrowseSelectedFileName;

    /// <inheritdoc/>
    public event Action? DeleteSelectedFileName;

    /// <inheritdoc/>
    public event Action? MoveUpSelectedFileName;

    /// <inheritdoc/>
    public event Action? MoveDownSelectedFileName;

    /// <inheritdoc/>
    public event Action? AddNewFileName;

    /// <inheritdoc/>
    public event Action? NewFileNameExclusively;

    /// <inheritdoc/>
    public event Action? SortFileNamesAscending;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleFilesControl"/> class.
    /// </summary>
    public MultipleFilesControl()
    {
      InitializeComponent();
    }

    /// <inheritdoc/>
    public Collections.SelectableListNodeList FileNames
    {
      set { GuiHelper.Initialize(_guiFileNames, value); }
    }

    private void EhFileNamesMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiFileNames);
      BrowseSelectedFileName?.Invoke();
    }

    private void EhRemoveFileName(object sender, RoutedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiFileNames);
      DeleteSelectedFileName?.Invoke();
    }

    private void EhAddNewFileName(object sender, RoutedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiFileNames);
      AddNewFileName?.Invoke();
    }

    private void EhNewFileNameExclusively(object sender, RoutedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiFileNames);
      NewFileNameExclusively?.Invoke();
    }

    private void EhMoveDownFileName(object sender, RoutedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiFileNames);
      MoveDownSelectedFileName?.Invoke();
    }

    private void EhMoveUpFileName(object sender, RoutedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiFileNames);
      MoveUpSelectedFileName?.Invoke();
    }

    private void EhSortFileNamesAscending(object sender, RoutedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiFileNames);
      SortFileNamesAscending?.Invoke();
    }
  }
}
