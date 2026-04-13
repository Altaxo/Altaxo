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

using Altaxo.Main;

namespace Altaxo.Gui.Workbench.Commands
{
  /// <summary>
  /// Selects the next open window.
  /// </summary>
  public class SelectNextWindow : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();

      var thisWindow = (parameter as IViewContent) ?? workbench.ActiveViewContent;
      if (thisWindow is not null)
      {
        int index = workbench.ViewContentCollection.IndexOf(thisWindow);
        workbench.ViewContentCollection[(index + 1) % workbench.ViewContentCollection.Count].IsSelected = true;
      }
    }
  }

  /// <summary>
  /// Selects the previous open window.
  /// </summary>
  public class SelectPrevWindow : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();
      var thisWindow = (parameter as IViewContent) ?? workbench.ActiveViewContent;
      if (thisWindow is not null)
      {
        int index = workbench.ViewContentCollection.IndexOf(thisWindow);
        workbench.ViewContentCollection[(index + workbench.ViewContentCollection.Count - 1) % workbench.ViewContentCollection.Count].IsSelected = true;
      }
    }
  }

  /// <summary>
  /// Closes all open windows.
  /// </summary>
  public class CloseAllWindows : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();
      workbench.CloseAllViews();
    }
  }

  /// <summary>
  /// Closes the current file tab.
  /// </summary>
  public class CloseFileTab : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();
      var thisWindow = (parameter as IViewContent) ?? workbench.ActiveViewContent;
      if (thisWindow is not null)
      {
        workbench.CloseContent(thisWindow);
      }
    }
  }

  /// <summary>
  /// Closes all file tabs except the current one.
  /// </summary>
  public class CloseAllButThisFileTab : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();
      var thisWindow = (parameter as IViewContent) ?? workbench.ActiveViewContent;
      if (thisWindow is not null)
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

  /// <summary>
  /// Deletes the document associated with the current file tab.
  /// </summary>
  public class DeleteThisFileTab : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();
      var thisWindow = (parameter as IViewContent) ?? workbench.ActiveViewContent;
      if (thisWindow is not null)
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
