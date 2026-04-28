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

#nullable disable warnings
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
  /// Interaction logic for Form1.xaml
  /// </summary>
  public partial class OleDbDataQueryControl : UserControl, IOleDbDataQueryView
  {
    /// <inheritdoc/>
    public event Action? SelectedTabChanged;

    /// <inheritdoc/>
    public event Action? CmdChooseConnectionStringFromDialog;

    /// <inheritdoc/>
    public event Action? ConnectionStringSelectedFromList;

    /// <inheritdoc/>
    public event Action<string>? ConnectionStringChangedByUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="OleDbDataQueryControl"/> class.
    /// </summary>
    public OleDbDataQueryControl()
    {
      InitializeComponent();
    }

    private static IndexToImageConverter _treeImageConverter;

    [IconResource] const string IconResource_Table = "Icons.16x16.DataConnection.Table";
    [IconResource] const string IconResource_View = "Icons.16x16.DataConnection.View";
    [IconResource] const string IconResource_Procedure = "Icons.16x16.DataConnection.Procedure";
    [IconResource] const string IconResource_Column = "Icons.16x16.DataConnection.Column";

    /// <summary>
    /// Gets an image converter for schema tree nodes.
    /// </summary>
    public static IValueConverter TreeImageConverter
    {
      get
      {
        if (_treeImageConverter is null)
        {
          _treeImageConverter = new IndexToImageConverter(
              new string[]{
                            IconResource_Table,
                            IconResource_View,
                            IconResource_Procedure,
                            IconResource_Column,
                          });
        }
        return _treeImageConverter;
      }
    }

    /// <inheritdoc/>
    public void SetConnectionListSource(SelectableListNodeList list, string currentValue)
    {
      GuiHelper.Initialize(_cmbConnString, list);
      _cmbConnString.Text = currentValue;
    }

    private void EhConnectionStringSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_cmbConnString);
      var ev = ConnectionStringSelectedFromList;
      if (ev is not null)
      {
        ev();
      }
    }

    private void EhConnStringKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        var ev = ConnectionStringChangedByUser;
        if (ev is not null)
        {
          e.Handled = true;
          ev(_cmbConnString.Text);
        }
      }
    }

    /// <summary>
    /// Switches the view to the table tab.
    /// </summary>
    public void ShowTableTabItem()
    {
      _tab.SelectedIndex = 0;
    }

    /// <summary>
    /// Switches the view to the SQL text tab.
    /// </summary>
    public void ShowSqlTextTabItem()
    {
      _tab.SelectedIndex = 1;
    }

    private void _tab_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_tab);

      var ev = SelectedTabChanged;
      if (ev is not null)
        ev();
    }

    // pick a new connection
    private void _btnConnPicker_Click(object sender, RoutedEventArgs e)
    {
      var ev = CmdChooseConnectionStringFromDialog;
      if (ev is not null)
        ev();
    }

    /// <inheritdoc/>
    public void SetWaitCursor()
    {
      Cursor = Cursors.Wait;
    }

    /// <inheritdoc/>
    public void SetNormalCursor()
    {
      Cursor = Cursors.Arrow;
    }

    /// <inheritdoc/>
    public void SetTabItemsSource(SelectableListNodeList tabItems)
    {
      GuiHelper.Initialize(_tab, tabItems);
    }

    /// <inheritdoc/>
    public void SetConnectionStatus(bool isValidConnectionSource)
    {
      _guiConnectionInvalid.Visibility = isValidConnectionSource ? Visibility.Collapsed : System.Windows.Visibility.Visible;
    }
  }
}
