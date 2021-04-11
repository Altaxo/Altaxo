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

namespace Altaxo.Gui.Common
{
  public class SingleChoiceRadioWrapPanel : WrapPanel
  {
    public event EventHandler? SelectionChanged;

    #region Dependency properties

    public static readonly DependencyProperty ItemsSourceProperty =
    DependencyProperty.Register(
      nameof(ItemsSource),
      typeof(SelectableListNodeList),
      typeof(SingleChoiceRadioWrapPanel),
      new FrameworkPropertyMetadata(EhItemsSourceChanged));

    /// <summary>
    /// Gets/sets the quantity. The quantity consist of a numeric value together with a unit.
    /// </summary>
    public SelectableListNodeList? ItemsSource
    {
      get { return (SelectableListNodeList)GetValue(ItemsSourceProperty); }
      set { SetValue(ItemsSourceProperty, value); }
    }

    private static void EhItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((SingleChoiceRadioWrapPanel)obj).OnItemsSourceChanged(obj, args);
    }

    /// <summary>
    /// Triggers the <see cref="SelectedQuantityChanged"/> event.
    /// </summary>
    /// <param name="obj">Dependency object (here: the control).</param>
    /// <param name="args">Property changed event arguments.</param>
    protected void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (args.NewValue is SelectableListNodeList newItemsSource)
      {
        SelectedItem = newItemsSource.FirstSelectedNode;
        Initialize(newItemsSource);
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
        typeof(SingleChoiceRadioWrapPanel),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, EhSelectedItemChanged));

    /// <summary>
    /// Gets/sets the quantity. The quantity consist of a numeric value together with a unit.
    /// </summary>
    public SelectableListNode? SelectedItem
    {
      get { return (SelectableListNode)GetValue(SelectedItemProperty); }
      set { SetValue(SelectedItemProperty, value); }
    }

    private static void EhSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((SingleChoiceRadioWrapPanel)obj).OnSelectedItemChanged(obj, args);
    }

    /// <summary>
    /// Triggers the <see cref="SelectedQuantityChanged"/> event.
    /// </summary>
    /// <param name="obj">Dependency object (here: the control).</param>
    /// <param name="args">Property changed event arguments.</param>
    protected void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion



    private SelectableListNodeList _choices;



    public void Initialize(SelectableListNodeList choices)
    {
      _choices = choices;
      if (!object.ReferenceEquals(_choices, ItemsSource))
      {
        ItemsSource = _choices;
      }

      Children.Clear();
      if (_choices is not null)
      {
        foreach (var choice in _choices)
        {
          var rb = new RadioButton
          {
            Content = choice.Text,
            ToolTip = choice.Text0,
            Tag = choice,
            IsChecked = choice.IsSelected,
            Margin = Orientation == Orientation.Horizontal ? new Thickness(3, 0, 3, 0) : new Thickness(0, 3, 0, 3),
          };
          rb.Checked += EhRadioButtonChecked;

          Children.Add(rb);
        }
      }
    }

    private void EhRadioButtonChecked(object sender, RoutedEventArgs e)
    {
      var rb = (RadioButton)sender;
      if (rb.Tag is SelectableListNode node)
      {
        _choices.ClearSelectionsAll();
        node.IsSelected = true == rb.IsChecked;

        if (node.IsSelected)
        {
          SelectedItem = node;
        }
      }
    }
  }
}
