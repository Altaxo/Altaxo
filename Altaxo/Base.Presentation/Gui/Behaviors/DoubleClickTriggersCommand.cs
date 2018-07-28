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
        if (e.NewValue != null)
          control.AddHandler(Control.MouseDoubleClickEvent, new RoutedEventHandler(EhDoubleClickEventHandler));
        else
          control.RemoveHandler(Control.MouseDoubleClickEvent, new RoutedEventHandler(EhDoubleClickEventHandler));
      }
      else if (d is UIElement fre)
      {
        if (e.NewValue != null)
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

      if (null != command)
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

          if (null != command)
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
