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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Interaction logic for EnumFlagControl.xaml
  /// </summary>
  public partial class EnumFlagControl : UserControl, IEnumFlagView
  {
    private SelectableListNodeList _choices;

    public EnumFlagControl()
    {
      InitializeComponent();
    }

    public void Initialize(SelectableListNodeList choices)
    {
      _choices = choices;
      _listView.ItemsSource = _choices;
    }

    public void Initialize(Enum value)
    {
      _choices = new SelectableListNodeList(value);
      _listView.ItemsSource = _choices;
    }

    public int SelectedValue
    {
      get
      {
        int sum = 0;

        if (_choices is not null)
        {
          foreach (var item in _choices)
          {
            if (item.IsSelected)
              sum |= (int)(item.Tag);
          }
        }

        return sum;
      }
    }
  }
}
