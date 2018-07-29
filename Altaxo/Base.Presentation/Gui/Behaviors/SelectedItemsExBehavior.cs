#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Altaxo.Gui.Behaviors
{
  /// <summary>
  /// Attached behavior to extend ListBox/ListView by a new dependency property SelectedItemsEx, which is bindable (in contrast to SelectedItems)
  /// You can bind both to SelectedItemsEx, which gives you a IList of selected items, and/or to SelectionChangedCommand, which executes the command
  /// whenever the selection changed.
  /// </summary>
  /// <example>
  ///  /// Usage:
  /// <code>
  /// &lt;ListBox ....
  /// local:SelectedItemsExBehavior.IsEnabled="True"
  ///  local:SelectedItemsExBehavior.SelectedItemsEx="{Binding SelectedItems, Mode=OneWayToSource}"
  ///  local:SelectedItemsExBehavior.SelectionChangedCommand="{Binding SelectionChangedCommand}"
  /// </code>
  /// </example>
  public static class SelectedItemsExBehavior
  {
    #region IsEnabled property (used to attach behaviour)

    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled",
        typeof(bool),
        typeof(SelectedItemsExBehavior),
        new FrameworkPropertyMetadata(OnIsEnabledChanged));

    public static bool GetIsEnabled(ListBox frameworkElement)
    {
      return (bool)frameworkElement.GetValue(IsEnabledProperty);
    }

    public static void SetIsEnabled(ListBox frameworkElement, bool observe)
    {
      frameworkElement.SetValue(IsEnabledProperty, observe);
    }

    private static void OnIsEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      DetachBehavior(dependencyObject);

      if ((bool)e.NewValue)
      {
        AttachBehavior(dependencyObject);
      }
    }

    #endregion IsEnabled property (used to attach behaviour)

    #region Attach/Detach

    private static void AttachBehavior(DependencyObject dpo)
    {
      if (dpo is ListBox lb)
      {
        lb.SelectionChanged += EhSelectionChanged;
      }
    }

    private static void DetachBehavior(DependencyObject dpo)
    {
      if (dpo is ListBox lb)
      {
        lb.SelectionChanged -= EhSelectionChanged;
      }
    }

    private static void EhSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (sender is ListBox lb)
      {
        SetSelectedItemsEx(lb, lb.SelectedItems);
        var command = GetSelectionChangedCommand(lb);
        if (null != command)
        {
          command.Execute(lb.SelectionMode == SelectionMode.Single ? lb.SelectedItem : lb.SelectedItems);
        }
      }
    }

    #endregion Attach/Detach

    #region SelectedItemsExProperty

    public static readonly DependencyProperty SelectedItemsExProperty = DependencyProperty.RegisterAttached(
        "SelectedItemsEx",
        typeof(IList),
        typeof(SelectedItemsExBehavior),
        new PropertyMetadata(null, OnSelectedItemsExChanged));

    public static IList GetSelectedItemsEx(ListBox frameworkElement)
    {
      return (IList)frameworkElement.GetValue(SelectedItemsExProperty);
    }

    public static void SetSelectedItemsEx(ListBox frameworkElement, IList value)
    {
      frameworkElement.SetValue(SelectedItemsExProperty, value);
    }

    private static void OnSelectedItemsExChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion SelectedItemsExProperty

    #region SelectedItemsExProperty

    public static readonly DependencyProperty SelectionChangedCommandProperty = DependencyProperty.RegisterAttached(
        "SelectionChangedCommand",
        typeof(ICommand),
        typeof(SelectedItemsExBehavior),
        new PropertyMetadata(null));

    public static ICommand GetSelectionChangedCommand(ListBox frameworkElement)
    {
      return (ICommand)frameworkElement.GetValue(SelectionChangedCommandProperty);
    }

    public static void SetSelectionChangedCommand(ListBox frameworkElement, ICommand value)
    {
      frameworkElement.SetValue(SelectionChangedCommandProperty, value);
    }

    #endregion SelectedItemsExProperty
  }
}
