#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Windows.Input;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Command implementation that delegates execution to supplied callbacks.
  /// </summary>
  public class RelayCommand : ICommand
  {
    private Action<object> execute;
    private Func<object, bool> canExecute;

    /// <inheritdoc/>
    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    public RelayCommand(Action<object> execute) : this(execute, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    /// <param name="canExecute">The predicate that determines whether the command can execute.</param>
    public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
    {
      this.execute = execute;
      this.canExecute = canExecute;
    }

    /// <inheritdoc/>
    public bool CanExecute(object parameter)
    {
      return canExecute is null || canExecute(parameter);
    }

    /// <inheritdoc/>
    public void Execute(object parameter)
    {
      execute(parameter);
    }
  }

  /// <summary>
  /// Generic command implementation that delegates execution to supplied callbacks.
  /// </summary>
  /// <typeparam name="TArg">The command argument type.</typeparam>
  public class RelayCommand<TArg> : ICommand
  {
    private Action<TArg> execute;
    private Func<TArg, bool> canExecute;

    /// <inheritdoc/>
    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand{TArg}"/> class.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    public RelayCommand(Action<TArg> execute) : this(execute, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand{TArg}"/> class.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    /// <param name="canExecute">The predicate that determines whether the command can execute.</param>
    public RelayCommand(Action<TArg> execute, Func<TArg, bool> canExecute)
    {
      this.execute = execute;
      this.canExecute = canExecute;
    }

    /// <summary>
    /// Determines whether the command can execute with the specified argument.
    /// </summary>
    /// <param name="parameter">The command argument.</param>
    /// <returns><see langword="true"/> if the command can execute; otherwise, <see langword="false"/>.</returns>
    public bool CanExecute(TArg parameter)
    {
      return canExecute is null || canExecute(parameter);
    }

    /// <summary>
    /// Executes the command with the specified argument.
    /// </summary>
    /// <param name="parameter">The command argument.</param>
    public void Execute(TArg parameter)
    {
      execute(parameter);
    }

    void ICommand.Execute(object parameter)
    {
      if (parameter is TArg)
        Execute((TArg)parameter);
      else
        throw new ArgumentException(string.Format("Type {0} was expected, but it is type {1}", typeof(TArg), parameter?.GetType()), nameof(parameter));
    }

    bool ICommand.CanExecute(object parameter)
    {
      if (parameter is TArg)
        return CanExecute((TArg)parameter);
      else
        return false;
    }
  }
}
