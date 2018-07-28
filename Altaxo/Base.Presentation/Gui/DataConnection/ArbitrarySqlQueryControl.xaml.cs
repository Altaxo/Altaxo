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

namespace Altaxo.Gui.DataConnection
{
  /// <summary>
  /// Interaction logic for ArbitrarySqlQueryControl.xaml
  /// </summary>
  public partial class ArbitrarySqlQueryControl : UserControl, IArbitrarySqlQueryView
  {
    public event Action CheckSql;

    public event Action ViewResults;

    public event Action ClearQuery;

    public event Action SqlTextChanged;

    public ArbitrarySqlQueryControl()
    {
      InitializeComponent();
    }

    private void EhCheckSql_Click(object sender, RoutedEventArgs e)
    {
      var ev = CheckSql;
      if (null != ev)
      {
        ev();
      }
    }

    private void EhViewResults_Click(object sender, RoutedEventArgs e)
    {
      var ev = ViewResults;
      if (null != ev)
      {
        ev();
      }
    }

    private void EhClearQuery_Click(object sender, RoutedEventArgs e)
    {
      var ev = ClearQuery;
      if (null != ev)
      {
        ev();
      }
    }

    public string SqlText
    {
      get
      {
        return _txtSql.Text;
      }
      set
      {
        _txtSql.Text = value;
      }
    }

    public void UpdateStatus(bool isConnectionStringEmpty, bool isSelectionStatementEmpty)
    {
      _guiCheckSql.IsEnabled = !isConnectionStringEmpty && !isSelectionStatementEmpty;
      _guiClearQuery.IsEnabled = true;
      _guiViewResults.IsEnabled = !isConnectionStringEmpty && !isSelectionStatementEmpty;
    }

    private void EhSqlTextChanged(object sender, TextChangedEventArgs e)
    {
      var ev = SqlTextChanged;
      if (null != ev)
        ev();
    }
  }
}
