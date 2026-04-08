#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Altaxo.Gui.Behaviors
{
  /// <summary>
  /// Behavior that helps binding the usual <see cref="ApplicationCommands"/> to commands in the viewmodel.
  /// The most used commands (copy, cut, paste, delete) can be bound directly. Other commands (must be a command from <see cref="ApplicationCommands"/>)
  /// can be bound using <see cref="CommandsToBindProperty"/>.
  /// </summary>
  public class CommandBindingBehavior
  {
    #region CommandsToBind

    /// <summary>
    /// Identifies the attached property that stores application commands to bind to view-model commands.
    /// </summary>
    public static readonly DependencyProperty CommandsToBindProperty = DependencyProperty.RegisterAttached(
        "CommandsToBind",
        typeof(IEnumerable<(string CommandName, System.Windows.Input.ICommand Command)>),
        typeof(CommandBindingBehavior),
        new PropertyMetadata(EhCommandsToBindChanged));

    /// <summary>
    /// Gets the commands configured for the specified framework element.
    /// </summary>
    public static object GetCommandsToBind(FrameworkElement frameworkElement)
    {
      return frameworkElement.GetValue(CommandsToBindProperty);
    }

    /// <summary>
    /// Sets the commands configured for the specified framework element.
    /// </summary>
    public static void SetCommandsToBind(FrameworkElement frameworkElement, object value)
    {
      frameworkElement.SetValue(CommandsToBindProperty, value);
    }

    private static void EhCommandsToBindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is FrameworkElement thiss)
      {
        // remove the old commands
        if (e.OldValue is IEnumerable<(string CommandName, ICommand Command)> oldColl)
        {
          foreach (var (CommandName, _) in oldColl)
          {
            if (GetApplicationCommand(CommandName) is ICommand cmdApp)
            {
              Remove(thiss.CommandBindings, cmdApp);
            }
          }
        }

        // add the new commands
        if (e.NewValue is IEnumerable<(string CommandName, ICommand Command)> newColl)
        {
          foreach (var (CommandName, Command) in newColl)
          {
            if (GetApplicationCommand(CommandName) is ICommand cmdApp)
            {
              var wrapper = new CommandWrapper(Command);
              var cmdBinding = new CommandBinding(cmdApp, wrapper.EhExecuted, wrapper.EhCanExecute);
            }
          }
        }
      }
    }

    #endregion

    #region Copy

    /// <summary>
    /// Identifies the attached property that binds <see cref="ApplicationCommands.Copy"/>.
    /// </summary>
    public static readonly DependencyProperty CopyProperty = DependencyProperty.RegisterAttached(
       "Copy",
       typeof(ICommand),
       typeof(CommandBindingBehavior),
       new PropertyMetadata((d,e) => EhBoundCommandChanged(ApplicationCommands.Copy, d, e)));

    /// <summary>
    /// Gets the command bound to <see cref="ApplicationCommands.Copy"/>.
    /// </summary>
    public static object GetCopy(FrameworkElement frameworkElement)
    {
      return frameworkElement.GetValue(CopyProperty);
    }

    /// <summary>
    /// Sets the command bound to <see cref="ApplicationCommands.Copy"/>.
    /// </summary>
    public static void SetCopy(FrameworkElement frameworkElement, object value)
    {
      frameworkElement.SetValue(CopyProperty, value);
    }

    #endregion

    #region Cut

    /// <summary>
    /// Identifies the attached property that binds <see cref="ApplicationCommands.Cut"/>.
    /// </summary>
    public static readonly DependencyProperty CutProperty = DependencyProperty.RegisterAttached(
       "Cut",
       typeof(ICommand),
       typeof(CommandBindingBehavior),
       new PropertyMetadata((d, e) => EhBoundCommandChanged(ApplicationCommands.Cut, d, e)));

    /// <summary>
    /// Gets the command bound to <see cref="ApplicationCommands.Cut"/>.
    /// </summary>
    public static object GetCut(FrameworkElement frameworkElement)
    {
      return frameworkElement.GetValue(CutProperty);
    }

    /// <summary>
    /// Sets the command bound to <see cref="ApplicationCommands.Cut"/>.
    /// </summary>
    public static void SetCut(FrameworkElement frameworkElement, object value)
    {
      frameworkElement.SetValue(CutProperty, value);
    }

    #endregion

    #region Paste

    /// <summary>
    /// Identifies the attached property that binds <see cref="ApplicationCommands.Paste"/>.
    /// </summary>
    public static readonly DependencyProperty PasteProperty = DependencyProperty.RegisterAttached(
       "Paste",
       typeof(ICommand),
       typeof(CommandBindingBehavior),
       new PropertyMetadata((d, e) => EhBoundCommandChanged(ApplicationCommands.Paste, d, e)));

    /// <summary>
    /// Gets the command bound to <see cref="ApplicationCommands.Paste"/>.
    /// </summary>
    public static object GetPaste(FrameworkElement frameworkElement)
    {
      return frameworkElement.GetValue(PasteProperty);
    }

    /// <summary>
    /// Sets the command bound to <see cref="ApplicationCommands.Paste"/>.
    /// </summary>
    public static void SetPaste(FrameworkElement frameworkElement, object value)
    {
      frameworkElement.SetValue(PasteProperty, value);
    }

    #endregion

    #region Delete

    /// <summary>
    /// Identifies the attached property that binds <see cref="ApplicationCommands.Delete"/>.
    /// </summary>
    public static readonly DependencyProperty DeleteProperty = DependencyProperty.RegisterAttached(
       "Delete",
       typeof(ICommand),
       typeof(CommandBindingBehavior),
       new PropertyMetadata((d, e) => EhBoundCommandChanged(ApplicationCommands.Delete, d, e)));

    /// <summary>
    /// Gets the command bound to <see cref="ApplicationCommands.Delete"/>.
    /// </summary>
    public static object GetDelete(FrameworkElement frameworkElement)
    {
      return frameworkElement.GetValue(DeleteProperty);
    }

    /// <summary>
    /// Sets the command bound to <see cref="ApplicationCommands.Delete"/>.
    /// </summary>
    public static void SetDelete(FrameworkElement frameworkElement, object value)
    {
      frameworkElement.SetValue(DeleteProperty, value);
    }

    #endregion

    #region Helpers

    private static void EhBoundCommandChanged(ICommand cmdBoundTo, DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is FrameworkElement thiss)
      {
        if (e.OldValue is ICommand cmdOld)
        {
          Remove(thiss.CommandBindings, cmdBoundTo);
        }
        if (e.NewValue is ICommand cmdNew)
        {
          var wrapper = new CommandWrapper(cmdNew);
          thiss.CommandBindings.Add(new CommandBinding(cmdBoundTo, wrapper.EhExecuted, wrapper.EhCanExecute));
        }
      }
    }

    static bool Remove(CommandBindingCollection coll, ICommand toDelete)
    {
      for(int i=coll.Count-1;i>=0;--i)
      {
        if(coll[i].Command == toDelete)
        {
          coll.RemoveAt(i);
          return true;
        }
      }
      return false;
    }

    static ICommand? GetApplicationCommand(string commandName)
    {
      var prop = typeof(ApplicationCommands).GetProperty(commandName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
      return prop?.GetValue(null, null) as ICommand;
    }

    private class CommandWrapper
    {
      WeakReference _wrappedCommand;
      public CommandWrapper(ICommand wrappedCommand)
      {
        _wrappedCommand = new WeakReference(wrappedCommand ?? throw new ArgumentNullException(nameof(wrappedCommand)));
      }

      public void EhCanExecute(object sender, CanExecuteRoutedEventArgs e)
      {
        if (_wrappedCommand.Target is ICommand cmd)
        {
          e.CanExecute = cmd.CanExecute(e.Parameter);
          e.Handled = true;
        }
      }

      public void EhExecuted(object sender, ExecutedRoutedEventArgs e)
      {
        if (_wrappedCommand.Target is ICommand cmd)
        {
          cmd.Execute(e.Parameter);
          e.Handled = true;
        }
      }
    }

    #endregion
  }
}
