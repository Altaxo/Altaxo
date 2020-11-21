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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Altaxo.AddInItems;

namespace Altaxo.Gui.AddInItems
{
  internal sealed class MenuCheckBox : CoreMenuItem
  {
    private ICheckableMenuCommand cmd;

    // We need to keep the reference to the event handler around
    // because the IsCheckedChanged event may be a weak event
    private EventHandler isCheckedChangedHandler;

    public MenuCheckBox(UIElement inputBindingOwner, Codon codon, object caller, IReadOnlyCollection<ICondition> conditions)
      : base(codon, caller, conditions)
    {
      Command = CommandWrapper.CreateCommand(codon, conditions);
      CommandParameter = caller;

      cmd = CommandWrapper.Unwrap(Command) as ICheckableMenuCommand;
      if (cmd is not null)
      {
        isCheckedChangedHandler = cmd_IsCheckedChanged;
        cmd.IsCheckedChanged += isCheckedChangedHandler;
        IsChecked = cmd.IsChecked(caller);
      }

      if (!string.IsNullOrEmpty(codon.Properties["shortcut"]))
      {
        KeyGesture kg = MenuService.ParseShortcut(codon.Properties["shortcut"]);
        MenuCommand.AddGestureToInputBindingOwner(inputBindingOwner, kg, Command, null);
        InputGestureText = MenuService.GetDisplayStringForShortcut(kg);
      }
    }

    private void cmd_IsCheckedChanged(object? sender, EventArgs e)
    {
      IsChecked = cmd.IsChecked(_caller);
    }
  }
}
