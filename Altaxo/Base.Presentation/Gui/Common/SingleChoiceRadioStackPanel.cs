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

    #region Dependency property

    public static readonly DependencyProperty ItemsSourceProperty =
    DependencyProperty.Register(
      nameof(ItemsSource),
      typeof(SelectableListNodeList),
      typeof(SingleChoiceRadioStackPanel),
      new FrameworkPropertyMetadata(EhItemsSourceChanged));

    /// <summary>
    /// Gets/sets the quantity. The quantity consist of a numeric value together with a unit.
    /// </summary>
    public SelectableListNodeList ItemsSource
    {
      get { var result = (SelectableListNodeList)GetValue(ItemsSourceProperty); return result; }
      set { SetValue(ItemsSourceProperty, value); }
    }

    private static void EhItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((SingleChoiceRadioStackPanel)obj).OnItemsSourceChanged(obj, args);
    }

    protected void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var list = args.NewValue as SelectableListNodeList;
      if (list is not null)
      {
        SelectedItem = list.FirstSelectedNode;
        Initialize(list);
      }
      else
      {
        SelectedItem = null;
      }
    }

    public static readonly DependencyProperty SelectedItemProperty =
   DependencyProperty.Register(
     nameof(SelectedItem),
     typeof(SelectableListNode),
     typeof(SingleChoiceRadioStackPanel),
     new FrameworkPropertyMetadata(EhSelectedItemChanged));

    /// <summary>
    /// Gets/sets the quantity. The quantity consist of a numeric value together with a unit.
    /// </summary>
    public SelectableListNode SelectedItem
    {
      get { var result = (SelectableListNode)GetValue(SelectedItemProperty); return result; }
      set { SetValue(SelectedItemProperty, value); }
    }

    private static void EhSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((SingleChoiceRadioStackPanel)obj).OnSelectedItemChanged(obj, args);
    }

    protected void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var list = ItemsSource;

      if (list is not null)
      {
        list.ClearSelectionsAll();
        if (args.NewValue is not null)
          list.SetSelection((node) => object.ReferenceEquals(node, args.NewValue));
      }
    }

    #endregion


    public void Initialize(SelectableListNodeList choices)
    {
      if (!object.ReferenceEquals(choices, ItemsSource))
      {
        ItemsSource = choices;
      }

      Children.Clear();
      if (choices is not null)
      {
        foreach (var choice in choices)
        {
          var rb = new RadioButton
          {
            Content = choice.Text,
            ToolTip = choice.Text0,
            Tag = choice,
            IsChecked = choice.IsSelected,

          };
          rb.Checked += EhRadioButtonChecked;

          if (Orientation == System.Windows.Controls.Orientation.Horizontal)
            rb.Margin = new Thickness(3, 0, 3, 0);
          else
            rb.Margin = new Thickness(0, 3, 0, 3);

          Children.Add(rb);
        }
      }
    }

    private void EhRadioButtonChecked(object sender, RoutedEventArgs e)
    {
      var rb = (RadioButton)sender;
      var node = rb.Tag as SelectableListNode;
      if (node is not null && (true == rb.IsChecked))
      {
        SelectedItem = node;
      }


      SelectionChanged?.Invoke();
    }
  }
}
