#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Windows.Input;
using Altaxo.Collections;

namespace Altaxo.Gui.DataConnection
{
  /// <summary>
  /// Interaction logic for EntireTableQueryControl.xaml
  /// </summary>
  public partial class EntireTableQueryControl : UserControl, IEntireTableQueryView
  {
    public event Action ViewResults;

    /// <summary>
    /// Occurs when the selected tree node of the schema tree changed.
    /// </summary>
    public event Action SelectedSchemaNodeChanged;

    public EntireTableQueryControl()
    {
      InitializeComponent();
    }

    private void EhViewResults_Click(object sender, RoutedEventArgs e)
    {
      var ev = ViewResults;
      if (ev is not null)
      {
        ev();
      }
    }

    private void _treeTables_DoubleClick(object sender, MouseButtonEventArgs e)
    {
      var ev = ViewResults;
      if (ev is not null)
        ev();
    }

    private void EhTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      var ev = SelectedSchemaNodeChanged;
      if (ev is not null)
        ev();
    }

    private void EhTreeViewItem_PreviewRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      var twi = sender as TreeViewItem;
      if (twi is not null)
        twi.IsSelected = true;
    }

    private static IndexToImageConverter _treeImageConverter;

    public static IValueConverter TreeImageConverter
    {
      get
      {
        if (_treeImageConverter is null)
        {
          _treeImageConverter = new IndexToImageConverter(
              new string[]{
                            "Icons.16x16.DataConnection.Table",
                            "Icons.16x16.DataConnection.View",
                            "Icons.16x16.DataConnection.Procedure",
                            "Icons.16x16.DataConnection.Column",
                          });
        }
        return _treeImageConverter;
      }
    }

    public void SetTreeSource(NGTreeNode rootNode)
    {
      _treeTables.ItemsSource = rootNode.Nodes;
    }
  }
}
