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
using System.Linq;
using Altaxo.AddInItems;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Tests if the current workbench window is a specified type or implements an interface.
  /// </summary>
  /// <attribute name="activeWindow">
  /// The fully qualified name of the type the active window should be or the
  /// interface name it should implement.
  /// "*" to test if any window is active.
  /// </attribute>
  /// <example title="Test if the current window is an text editor">
  /// &lt;Condition name="WindowActive" activeWindow="Altaxo.Gui.ITextEditor"&gt;
  /// </example>
  /// <example title="Test if any window is active">
  /// &lt;Condition name="WindowActive" activeWindow="*"&gt;
  /// </example>
  public class WindowActiveConditionEvaluator : IConditionEvaluator
  {
    public bool IsValid(object? caller, Condition condition)
    {
      IViewContent? activeViewContent = caller as IViewContent;
      if (activeViewContent is null) // active view content is probably coming from the data context of the menu
      {
        var workbench = Altaxo.Current.GetRequiredService<Workbench.IWorkbenchEx>();
        activeViewContent = workbench.ActiveViewContent; // else active view content is retrieved from the workbench
      }

      string activeWindow = condition.Properties["activewindow"];
      if (activeWindow == "*")
      {
        return activeViewContent is not null;
      }

      var activeWindowType = condition.AddIn.FindType(activeWindow);
      if (activeWindowType is null)
      {
        Current.Log.WarnFormatted("WindowActiveCondition: cannot find Type {0}", activeWindow);
        return false;
      }

      // ask the active view content, if it has a sub-content of the given window type
      if (activeViewContent?.GetService(activeWindowType) is not null)
        return true;

      if (activeViewContent is null)
        return false;

      Type? currentType = activeViewContent.GetType();
      if (currentType.FullName == activeWindow)
        return true;
      foreach (Type interf in currentType.GetInterfaces())
      {
        if (interf.FullName == activeWindow)
          return true;
      }
      while (currentType.BaseType is { } currentBaseType)
      {
        currentType = currentBaseType;
        if (currentType.FullName == activeWindow)
          return true;
      }
      return false;
    }
  }
}
