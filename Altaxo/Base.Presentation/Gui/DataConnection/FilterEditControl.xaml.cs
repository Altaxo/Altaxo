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
  /// Interaction logic for FilterEditControl.xaml
  /// </summary>
  public partial class FilterEditControl : UserControl, IFilterEditView
  {
    public event Action SimpleUpdated;

    public event Action IntervalUpdated;

    public event Action ClearAll;

    public FilterEditControl()
    {
      InitializeComponent();
    }

    private void EhSimple_OperatorSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_cmbOperator);
      var ev = SimpleUpdated;
      if (null != ev)
      {
        ev();
      }
    }

    private void _simpleChangedText(object sender, TextChangedEventArgs e)
    {
      var ev = SimpleUpdated;
      if (null != ev)
      {
        ev();
      }
    }

    private void _betweenChanged(object sender, TextChangedEventArgs e)
    {
      var ev = IntervalUpdated;
      if (null != ev)
      {
        ev();
      }
    }

    private void _btnClear_Click(object sender, RoutedEventArgs e)
    {
      var ev = ClearAll;
      if (null != ev)
      {
        ev();
      }
    }

    public void SetValueText(string txt)
    {
      _value.Content = txt;
    }

    public string SingleValueText
    {
      get
      {
        return _txtValue.Text;
      }
      set
      {
        _txtValue.Text = value;
      }
    }

    public string IntervalFromText
    {
      get
      {
        return _txtFrom.Text;
      }
      set
      {
        _txtFrom.Text = value;
      }
    }

    public string intervalToText
    {
      get
      {
        return _txtTo.Text;
      }
      set
      {
        _txtTo.Text = value;
      }
    }

    public void SetOperatorChoices(Collections.SelectableListNodeList list)
    {
      GuiHelper.Initialize(_cmbOperator, list);
    }
  }
}
