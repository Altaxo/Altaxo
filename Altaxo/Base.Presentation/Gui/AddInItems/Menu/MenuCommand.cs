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
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Altaxo.AddInItems;

namespace Altaxo.Gui.AddInItems
{
  internal class MenuCommand : CoreMenuItem
  {
    private readonly string ActivationMethod;

    public MenuCommand(UIElement inputBindingOwner, Codon codon, object caller, string activationMethod, IReadOnlyCollection<ICondition> conditions) : base(codon, caller, conditions)
    {
      ActivationMethod = activationMethod;
      Command = CommandWrapper.CreateLazyCommand(codon, conditions);
      //this.CommandParameter = caller;

      if ("true" == codon.Properties["usedatacontext"]?.ToLowerInvariant())
      {
        _useDataContext = true;

        var myBinding = new System.Windows.Data.Binding(nameof(DataContext))
        {
          Source = this
        };
        SetBinding(System.Windows.Controls.MenuItem.CommandParameterProperty, myBinding);
        // this.DataContextChanged += (s,e) => UpdateStatus(); // we can abstain from using this as long as the workbench calls UpdateStatus for all menus (especially the workbench's context menu)
      }
      else
      {
        CommandParameter = caller;
      }

      if (!string.IsNullOrEmpty(codon.Properties["shortcut"]))
      {
        KeyGesture kg = MenuService.ParseShortcut(codon.Properties["shortcut"]);
        AddGestureToInputBindingOwner(inputBindingOwner, kg, Command, GetFeatureName());
        InputGestureText = MenuService.GetDisplayStringForShortcut(kg);
      }
    }

    internal static void AddGestureToInputBindingOwner(UIElement inputBindingOwner, KeyGesture kg, System.Windows.Input.ICommand shortcutCommand, string featureName)
    {
      if (inputBindingOwner is not null && kg is not null && shortcutCommand is not null)
      {
        // wrap the shortcut command so that it can report to UDC with
        // different activation method
        if (!string.IsNullOrEmpty(featureName))
          shortcutCommand = new ShortcutCommandWrapper(shortcutCommand, featureName);
        // try to find an existing input binding which conflicts with this shortcut
        var existingInputBinding =
          (from InputBinding b in inputBindingOwner.InputBindings
           let gesture = b.Gesture as KeyGesture
           where gesture is not null
           && gesture.Key == kg.Key
           && gesture.Modifiers == kg.Modifiers
           select b
          ).FirstOrDefault();
        if (existingInputBinding is not null)
        {
          // modify the existing input binding to allow calling both commands
          existingInputBinding.Command = new AmbiguousCommandWrapper(existingInputBinding.Command, shortcutCommand);
        }
        else
        {
          inputBindingOwner.InputBindings.Add(new InputBinding(shortcutCommand, kg));
        }
      }
    }

    private string GetFeatureName()
    {
      string commandName = _codon.Properties["command"];
      if (string.IsNullOrEmpty(commandName))
      {
        return _codon.Properties["class"];
      }
      else
      {
        return commandName;
      }
    }

    private sealed class ShortcutCommandWrapper : System.Windows.Input.ICommand
    {
      private readonly System.Windows.Input.ICommand baseCommand;
      private readonly string featureName;

      public ShortcutCommandWrapper(System.Windows.Input.ICommand baseCommand, string featureName)
      {
        Debug.Assert(baseCommand is not null);
        Debug.Assert(featureName is not null);
        this.baseCommand = baseCommand;
        this.featureName = featureName;
      }

      public event EventHandler CanExecuteChanged
      {
        add { baseCommand.CanExecuteChanged += value; }
        remove { baseCommand.CanExecuteChanged -= value; }
      }

      public void Execute(object parameter)
      {
        baseCommand.Execute(parameter);
      }

      public bool CanExecute(object parameter)
      {
        return baseCommand.CanExecute(parameter);
      }
    }

    private sealed class AmbiguousCommandWrapper : System.Windows.Input.ICommand
    {
      private System.Windows.Input.ICommand first;
      private System.Windows.Input.ICommand second;

      public AmbiguousCommandWrapper(System.Windows.Input.ICommand first, System.Windows.Input.ICommand second)
      {
        this.first = first;
        this.second = second;
      }

      public event EventHandler CanExecuteChanged
      {
        add
        {
          first.CanExecuteChanged += value;
          second.CanExecuteChanged += value;
        }
        remove
        {
          first.CanExecuteChanged -= value;
          second.CanExecuteChanged -= value;
        }
      }

      public void Execute(object parameter)
      {
        if (first.CanExecute(parameter))
          first.Execute(parameter);
        else
          second.Execute(parameter);
      }

      public bool CanExecute(object parameter)
      {
        return first.CanExecute(parameter) || second.CanExecute(parameter);
      }
    }
  }
}
