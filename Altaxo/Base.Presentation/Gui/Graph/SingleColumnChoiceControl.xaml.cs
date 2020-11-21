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
using System.Windows;
using System.Windows.Controls;
using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Interaction logic for SingleColumnChoiceControl.xaml
  /// </summary>
  [UserControlForController(typeof(ISingleColumnChoiceViewEventSink))]
  public partial class SingleColumnChoiceControl : UserControl, ISingleColumnChoiceView
  {
    private ISingleColumnChoiceViewEventSink _controller;

    public SingleColumnChoiceControl()
    {
      InitializeComponent();
    }

    private void _tvColumns_AfterSelect(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      if (_controller is not null)
      {
        var selitem = _tvColumns.SelectedItem as NGTreeNode;
        if (selitem is not null)
          _controller.EhView_AfterSelectNode(selitem);
      }
    }

    #region ISingleColumnChoiceView

    public ISingleColumnChoiceViewEventSink Controller
    {
      get
      {
        return _controller;
      }
      set
      {
        _controller = value;
      }
    }

    public void Initialize(Collections.NGTreeNodeCollection nodes)
    {
      _tvColumns.ItemsSource = nodes;
    }

    #endregion ISingleColumnChoiceView
  }
}
