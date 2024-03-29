﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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

namespace Altaxo.Gui.Workbench
{
  [Flags]
  public enum WindowState
  {
    None = 0,
    Untitled = 1,
    Dirty = 2,
    ViewOnly = 4
  }

  /// <summary>
  /// Tests the window state of the active workbench window.
  /// </summary>
  public class ActiveWindowStateConditionEvaluator : IConditionEvaluator
  {
    public bool IsValid(object? caller, Condition condition)
    {
      var workbench = Altaxo.Current.GetRequiredService<Workbench.IWorkbenchEx>();

      var activeWorkbenchWindow = workbench.ActiveViewContent;
      if (activeWorkbenchWindow is null)
      {
        return false;
      }

      WindowState windowState = condition.Properties.Get("windowstate", WindowState.None);
      WindowState nowindowState = condition.Properties.Get("nowindowstate", WindowState.None);

      bool isWindowStateOk = false;
      if (windowState != WindowState.None)
      {
        if ((windowState & WindowState.Dirty) > 0)
        {
          isWindowStateOk |= activeWorkbenchWindow.IsDirty;
        }
        if ((windowState & WindowState.Untitled) > 0)
        {
          isWindowStateOk |= IsUntitled(activeWorkbenchWindow as IFileViewContent);
        }
        if ((windowState & WindowState.ViewOnly) > 0)
        {
          isWindowStateOk |= IsViewOnly(activeWorkbenchWindow);
        }
      }
      else
      {
        isWindowStateOk = true;
      }

      if (nowindowState != WindowState.None)
      {
        if ((nowindowState & WindowState.Dirty) > 0)
        {
          isWindowStateOk &= !activeWorkbenchWindow.IsDirty;
        }

        if ((nowindowState & WindowState.Untitled) > 0)
        {
          isWindowStateOk &= !IsUntitled(activeWorkbenchWindow as IFileViewContent);
        }

        if ((nowindowState & WindowState.ViewOnly) > 0)
        {
          isWindowStateOk &= !IsViewOnly(activeWorkbenchWindow);
        }
      }
      return isWindowStateOk;
    }

    private static bool IsUntitled(IFileViewContent? viewContent)
    {
      if (viewContent is null)
        return false;
      OpenedFile file = viewContent.PrimaryFile;
      if (file is null)
        return false;
      else
        return file.IsUntitled;
    }

    private static bool IsViewOnly(IViewContent viewContent)
    {
      return viewContent is not null && viewContent.IsViewOnly;
    }
  }
}
