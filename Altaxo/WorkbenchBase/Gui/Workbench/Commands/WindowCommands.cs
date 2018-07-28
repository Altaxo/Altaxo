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
using Altaxo.Gui.AddInItems;
using Altaxo.Gui.Workbench;
using Altaxo.Main;

namespace Altaxo.Gui.Workbench.Commands
{
  public class SelectNextWindow : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();

      if (!(parameter is IViewContent thisWindow))
        thisWindow = workbench.ActiveViewContent;

      if (null != thisWindow)
      {
        int index = workbench.ViewContentCollection.IndexOf(workbench.ActiveViewContent);
        workbench.ViewContentCollection[(index + 1) % workbench.ViewContentCollection.Count].IsSelected = true;
      }
    }
  }

  public class SelectPrevWindow : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();
      if (!(parameter is IViewContent thisWindow))
        thisWindow = workbench.ActiveViewContent;

      if (null != thisWindow)
      {
        int index = workbench.ViewContentCollection.IndexOf(workbench.ActiveViewContent);
        workbench.ViewContentCollection[(index + workbench.ViewContentCollection.Count - 1) % workbench.ViewContentCollection.Count].IsSelected = true;
      }
    }
  }

  public class CloseAllWindows : SimpleCommand
  {
    public override void Execute(object _)
    {
      var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();
      workbench.CloseAllViews();
    }
  }

  public class CloseFileTab : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();
      if (!(parameter is IViewContent thisWindow))
        thisWindow = workbench.ActiveViewContent;

      if (thisWindow != null)
      {
        workbench.CloseContent(thisWindow);
      }
    }
  }

  public class CloseAllButThisFileTab : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();

      if (!(parameter is IViewContent thisWindow))
        thisWindow = workbench.ActiveViewContent;

      if (null != thisWindow)
      {
        for (int i = workbench.ViewContentCollection.Count - 1; i >= 0; --i)
        {
          var window = workbench.ViewContentCollection[i];
          if (window != thisWindow)
          {
            workbench.CloseContent(window);
          }
        }
      }
    }
  }

  public class DeleteThisFileTab : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();

      if (!(parameter is IViewContent thisWindow))
        thisWindow = workbench.ActiveViewContent;

      if (null != thisWindow)
      {
        var currentProjectService = Current.GetRequiredService<IProjectService>();

        if (thisWindow.ModelObject is Altaxo.Main.IProjectItem pi)
          currentProjectService.DeleteDocument(pi, false);
        else if (thisWindow.ModelObject is Altaxo.Main.IProjectItemPresentationModel pipm)
          currentProjectService.DeleteDocument(pipm.Document, false);
      }
    }
  }
}
