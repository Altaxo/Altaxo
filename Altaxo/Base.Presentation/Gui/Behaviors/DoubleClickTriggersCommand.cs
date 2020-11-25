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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Altaxo.Gui.Behaviors
{
  public class DoubleClickTriggersCommand
  {
    #region IsHandled property

    public static readonly DependencyProperty IsHandledProperty = DependencyProperty.RegisterAttached(
        "IsHandled",
        typeof(bool),
        typeof(DoubleClickTriggersCommand),
        new FrameworkPropertyMetadata(true));

    public static bool GetIsHandled(FrameworkElement frameworkElement)
    {
      return (bool)frameworkElement.GetValue(IsHandledProperty);
    }

    public static void SetIsHandled(FrameworkElement frameworkElement, bool observe)
    {
      frameworkElement.SetValue(IsHandledProperty, observe);
    }

    #endregion IsHandled property

    #region Command

    public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
    "Command",
    typeof(ICommand),
    typeof(DoubleClickTriggersCommand),
    new FrameworkPropertyMetadata(OnCommandChanged)
    );

    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is Control control)
      {
        if (e.NewValue is not null)
          control.AddHandler(Control.MouseDoubleClickEvent, new RoutedEventHandler(EhDoubleClickEventHandler));
        else
          control.RemoveHandler(Control.MouseDoubleClickEvent, new RoutedEventHandler(EhDoubleClickEventHandler));
      }
      else if (d is UIElement fre)
      {
        if (e.NewValue is not null)
          fre.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(EhPreviewMouseLeftButtonDownEventHandler));
        else
          fre.RemoveHandler(Control.PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(EhPreviewMouseLeftButtonDownEventHandler));
      }
    }

    public static ICommand GetCommand(FrameworkElement frameworkElement)
    {
      return (ICommand)frameworkElement.GetValue(CommandProperty);
    }

    public static void SetCommand(FrameworkElement frameworkElement, ICommand value)
    {
      frameworkElement.SetValue(CommandProperty, value);
    }

    #endregion Command

    #region Command parameter

    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
    "CommandParameter",
    typeof(object),
    typeof(DoubleClickTriggersCommand)
    );

    public static object GetCommandParameter(FrameworkElement frameworkElement)
    {
      return frameworkElement.GetValue(CommandParameterProperty);
    }

    public static void SetCommandParameter(FrameworkElement frameworkElement, object value)
    {
      frameworkElement.SetValue(CommandParameterProperty, value);
    }

    #endregion Command parameter

    #region Event handlers

    private static void EhDoubleClickEventHandler(object sender, RoutedEventArgs e)
    {
      var command = GetCommand((FrameworkElement)sender);
      var commandParameter = GetCommandParameter((FrameworkElement)sender);

      if (command is not null)
      {
        if (command.CanExecute(commandParameter))
          command.Execute(commandParameter);

        e.Handled = GetIsHandled((FrameworkElement)sender);
      }
    }

    private static void EhPreviewMouseLeftButtonDownEventHandler(object sender, RoutedEventArgs e)
    {
      if (e is MouseButtonEventArgs mbe)
      {
        if (mbe.ClickCount >= 2)
        {
          var command = GetCommand((FrameworkElement)sender);
          var commandParameter = GetCommandParameter((FrameworkElement)sender);

          if (command is not null)
          {
            if (command.CanExecute(commandParameter))
              command.Execute(commandParameter);

            e.Handled = GetIsHandled((FrameworkElement)sender);
          }
        }
      }
    }

    #endregion Event handlers
  }
}
