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
  /// <summary>
  /// Attached behavior that executes a command when a control is double-clicked.
  /// </summary>
  public class DoubleClickTriggersCommand
  {
    #region IsHandled property

    /// <summary>
    /// Identifies the attached property that controls whether the double-click event is marked as handled.
    /// </summary>
    public static readonly DependencyProperty IsHandledProperty = DependencyProperty.RegisterAttached(
        "IsHandled",
        typeof(bool),
        typeof(DoubleClickTriggersCommand),
        new FrameworkPropertyMetadata(true));

    /// <summary>
    /// Gets a value indicating whether the handled flag is set for the specified framework element.
    /// </summary>
    public static bool GetIsHandled(FrameworkElement frameworkElement)
    {
      return (bool)frameworkElement.GetValue(IsHandledProperty);
    }

    /// <summary>
    /// Sets a value indicating whether the double-click event is marked as handled for the specified framework element.
    /// </summary>
    public static void SetIsHandled(FrameworkElement frameworkElement, bool observe)
    {
      frameworkElement.SetValue(IsHandledProperty, observe);
    }

    #endregion IsHandled property

    #region Command

    /// <summary>
    /// Identifies the attached property that stores the command to execute.
    /// </summary>
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

    /// <summary>
    /// Gets the command associated with the specified framework element.
    /// </summary>
    public static ICommand GetCommand(FrameworkElement frameworkElement)
    {
      return (ICommand)frameworkElement.GetValue(CommandProperty);
    }

    /// <summary>
    /// Sets the command associated with the specified framework element.
    /// </summary>
    public static void SetCommand(FrameworkElement frameworkElement, ICommand value)
    {
      frameworkElement.SetValue(CommandProperty, value);
    }

    #endregion Command

    #region Command parameter

    /// <summary>
    /// Identifies the attached property that stores the command parameter.
    /// </summary>
    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
    "CommandParameter",
    typeof(object),
    typeof(DoubleClickTriggersCommand)
    );

    /// <summary>
    /// Gets the command parameter associated with the specified framework element.
    /// </summary>
    public static object GetCommandParameter(FrameworkElement frameworkElement)
    {
      return frameworkElement.GetValue(CommandParameterProperty);
    }

    /// <summary>
    /// Sets the command parameter associated with the specified framework element.
    /// </summary>
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
