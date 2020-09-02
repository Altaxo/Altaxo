// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Altaxo;
using Altaxo.AddInItems;
using Altaxo.Collections;

namespace Altaxo.Gui.AddInItems
{
  public sealed class CommandWrapper : ICommand
  {
    private bool _commandCreated;
    private ICommand _addInCommand;
    private readonly IReadOnlyCollection<ICondition> _conditions;
    private readonly Codon _codon;

    /// <summary>
    /// Maintains a weak collection of CanExecute handlers as long as there is no command created. When creating the command
    /// the CanExecute handler of this collection are bound to the command, and this collection is set to null.
    /// </summary>
    private WeakCollection<EventHandler> _canExecuteChangedHandlersToRegisterOnCommand;

    /// <summary>
    /// A delegate that is set by the host application, and gets executed to create link commands.
    /// </summary>
    public static Func<string, ICommand> LinkCommandCreator { get; set; }

    /// <summary>
    /// A delegate that is set by the host application, and gets executed to create well-known commands.
    /// </summary>
    public static Func<string, ICommand> WellKnownCommandCreator { get; set; }

    public static Action<EventHandler> RegisterConditionRequerySuggestedHandler { get; set; }
    public static Action<EventHandler> UnregisterConditionRequerySuggestedHandler { get; set; }

    /// <summary>
    /// Creates a lazy command.
    /// </summary>
    public static ICommand CreateLazyCommand(Codon codon, IReadOnlyCollection<ICondition> conditions)
    {
      if (codon.Properties["loadclasslazy"] == "false")
      {
        // if lazy loading was explicitly disabled, create the actual command now
        return CreateCommand(codon, conditions);
      }
      if (codon.Properties.Contains("command") && !codon.Properties.Contains("loadclasslazy"))
      {
        // If we're using the 'command=' syntax, this is most likely a built-in command
        // where lazy loading isn't useful (and hurts if CanExecute is used).
        // Don't use lazy loading unless loadclasslazy is set explicitly.
        return CreateCommand(codon, conditions);
      }
      // Create the wrapper that lazily loads the actual command.
      return new CommandWrapper(codon, conditions);
    }

    /// <summary>
    /// Creates a non-lazy command.
    /// </summary>
    public static ICommand CreateCommand(Codon codon, IReadOnlyCollection<ICondition> conditions)
    {
      ICommand command = CreateCommand(codon);
      if (command is not null && conditions.Count == 0)
        return command;
      else
        return new CommandWrapper(command, conditions);
    }

    public static ICommand Unwrap(ICommand command)
    {
      if (command is CommandWrapper w)
      {
        w.EnsureCommandCreated();
        return w._addInCommand;
      }
      else
      {
        return command;
      }
    }

    private static ICommand CreateCommand(Codon codon)
    {
      ICommand command = null;
      if (codon.Properties.Contains("command"))
      {
        string commandName = codon.Properties["command"];
        if (WellKnownCommandCreator is not null)
        {
          command = WellKnownCommandCreator(commandName);
        }
        if (command is null)
        {
          command = GetCommandFromStaticProperty(codon.AddIn, commandName);
        }
        if (command is null)
        {
          Current.MessageService.ShowError("Could not find command '" + commandName + "'.");
        }
      }
      else if (codon.Properties.Contains("link"))
      {
        if (LinkCommandCreator is null)
          throw new NotSupportedException("MenuCommand.LinkCommandCreator is not set, cannot create LinkCommands.");
        command = LinkCommandCreator(codon.Properties["link"]);
      }
      else
      {
        command = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
      }
      return command;
    }

    private static ICommand GetCommandFromStaticProperty(AddIn addIn, string commandName)
    {
      int pos = commandName.LastIndexOf('.');
      if (pos > 0)
      {
        string className = commandName.Substring(0, pos);
        string propertyName = commandName.Substring(pos + 1);
        Type classType = addIn.FindType(className);
        if (classType is not null)
        {
          PropertyInfo p = classType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
          if (p is not null)
            return (ICommand)p.GetValue(null, null);
          FieldInfo f = classType.GetField(propertyName, BindingFlags.Public | BindingFlags.Static);
          if (f is not null)
            return (ICommand)f.GetValue(null);
        }
      }
      return null;
    }

    private CommandWrapper(Codon codon, IReadOnlyCollection<ICondition> conditions)
    {
      _codon = codon;
      _conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
      _canExecuteChangedHandlersToRegisterOnCommand = new WeakCollection<EventHandler>();
    }

    private CommandWrapper(ICommand command, IReadOnlyCollection<ICondition> conditions)
    {
      _addInCommand = command;
      _conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
      _commandCreated = true;
    }

    private void EnsureCommandCreated()
    {
      if (!_commandCreated)
      {
        _commandCreated = true;
        _addInCommand = CreateCommand(_codon);
        if (_canExecuteChangedHandlersToRegisterOnCommand is not null)
        {
          var handlers = _canExecuteChangedHandlersToRegisterOnCommand.ToArray();
          _canExecuteChangedHandlersToRegisterOnCommand = null;

          foreach (var handler in handlers)
          {
            if (_addInCommand is not null)
              _addInCommand.CanExecuteChanged += handler;
            // Creating the command potentially changes the CanExecute state, so we should raise the event handlers once:
            handler(this, EventArgs.Empty);
          }
        }
      }
    }

    public event EventHandler CanExecuteChanged
    {
      add
      {
        if (value is null)
          return;
        if (_conditions.Count > 0 && RegisterConditionRequerySuggestedHandler is not null)
          RegisterConditionRequerySuggestedHandler(value);

        if (_addInCommand is not null)
          _addInCommand.CanExecuteChanged += value;
        else if (_canExecuteChangedHandlersToRegisterOnCommand is not null)
          _canExecuteChangedHandlersToRegisterOnCommand.Add(value);
      }
      remove
      {
        if (value is null)
          return;
        if (_conditions.Count > 0 && UnregisterConditionRequerySuggestedHandler is not null)
          UnregisterConditionRequerySuggestedHandler(value);

        if (_addInCommand is not null)
          _addInCommand.CanExecuteChanged -= value;
        else if (_canExecuteChangedHandlersToRegisterOnCommand is not null)
          _canExecuteChangedHandlersToRegisterOnCommand.Remove(value);
      }
    }

    public void Execute(object parameter)
    {
      EnsureCommandCreated();
      if (CanExecute(parameter))
      {
        _addInCommand.Execute(parameter);
      }
    }

    public bool CanExecute(object parameter)
    {
      if (Condition.GetFailedAction(_conditions, parameter) != ConditionFailedAction.Nothing)
        return false;
      if (!_commandCreated)
        return true;
      return _addInCommand is not null && _addInCommand.CanExecute(parameter);
    }
  }
}
