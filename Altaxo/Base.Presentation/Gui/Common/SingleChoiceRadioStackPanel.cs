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
using System.Windows;
using System.Windows.Controls;
using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
  public class SingleChoiceRadioStackPanel : StackPanel
  {
    public event Action SelectionChanged;

    private SelectableListNodeList _choices;

    public void Initialize(SelectableListNodeList choices)
    {
      _choices = choices;
      Children.Clear();
      foreach (var choice in _choices)
      {
        var rb = new RadioButton
        {
          Content = choice.Text,
          Tag = choice,
          IsChecked = choice.IsSelected
        };
        rb.Checked += EhRadioButtonChecked;

        if (Orientation == System.Windows.Controls.Orientation.Horizontal)
          rb.Margin = new Thickness(3, 0, 3, 0);
        else
          rb.Margin = new Thickness(0, 3, 0, 3);

        Children.Add(rb);
      }
    }

    private void EhRadioButtonChecked(object sender, RoutedEventArgs e)
    {
      var rb = (RadioButton)sender;
      var node = rb.Tag as SelectableListNode;
      if (node != null)
      {
        _choices.ClearSelectionsAll();
        node.IsSelected = true == rb.IsChecked;
      }

      if (null != SelectionChanged)
        SelectionChanged();
    }
  }
}
