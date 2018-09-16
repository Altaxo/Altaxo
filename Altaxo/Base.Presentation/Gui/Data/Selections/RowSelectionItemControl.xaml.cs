#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Collections;

namespace Altaxo.Gui.Data.Selections
{
  /// <summary>
  /// Interaction logic for RowSelectionItemControl.xaml
  /// </summary>
  public partial class RowSelectionItemControl : Grid
  {
    private FrameworkElement _selectionDetailControl;

    public RowSelectionItemControl()
    {
      InitializeComponent();
    }

    public RowSelectionItemControl(SelectableListNodeList rowSelectionTypes, object detailsControl)
    {
      InitializeComponent();

      GuiHelper.Initialize(_guiRowSelectionType, rowSelectionTypes);
      RowSelectionDetailControl = detailsControl;
    }

    public object RowSelectionDetailControl
    {
      set
      {
        if (null != _selectionDetailControl)
        {
          Children.Remove(_selectionDetailControl);
        }

        _selectionDetailControl = value as FrameworkElement;

        if (null != _selectionDetailControl)
        {
          _selectionDetailControl.SetValue(Grid.ColumnProperty, 1);
          Children.Add(_selectionDetailControl);
        }
      }
    }

    public int IndentationLevel
    {
      set
      {
        Margin = new Thickness(value * 10, 0, 0, 0);
      }
    }

    public object SelectedSelection
    {
      get
      {
        return _guiRowSelectionType.SelectedValue;
      }
    }
  }
}
