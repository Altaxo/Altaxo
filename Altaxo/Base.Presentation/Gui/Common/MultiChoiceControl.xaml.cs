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
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Interaction logic for MultiChoiceControl.xaml
  /// </summary>
  public partial class MultiChoiceControl : UserControl, IMultiChoiceView
  {
    public MultiChoiceControl()
    {
      InitializeComponent();
    }

    public void InitializeDescription(string value)
    {
      _edDescription.Text = value;
    }

    public void InitializeColumnNames(string[] colNames)
    {
      if (_lvItems.View is null)
        _lvItems.View = new GridView();

      var gv = (GridView)_lvItems.View;

      gv.Columns.Clear();

      int colNo = -1;
      foreach (var colName in colNames)
      {
        ++colNo;

        var gvCol = new GridViewColumn() { Header = colName };
        var binding = new Binding(colNo == 0 ? "Text " : "Text" + colNo.ToString());
        gvCol.DisplayMemberBinding = binding;
        gv.Columns.Add(gvCol);
      }
    }

    public void InitializeList(Collections.SelectableListNodeList list)
    {
      _lvItems.ItemsSource = list;
    }
  }
}
