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
using System.Windows;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// This is the basic interface to the application's workbench.
  /// </summary>
  [GlobalService("Workbench")]
  public interface IWorkbenchEx : IWorkbench
  {
    /// <summary>
    /// A collection in which all opened primary view contents are saved.
    /// </summary>
    ICollection<IViewContent> PrimaryViewContents
    {
      get;
    }

    /// <summary>
    /// Gets whether this application is the active application in Windows.
    /// </summary>
    bool IsActiveWindow
    {
      get;
    }

    /// <summary>
    /// Initializes the workbench.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Inserts a new <see cref="IViewContent"/> object in the workspace and switches to the new view.
    /// </summary>
    void ShowView(IViewContent content);

    /// <summary>
    /// Inserts a new <see cref="IViewContent"/> object in the workspace.
    /// </summary>
    void ShowView(IViewContent content, bool switchToOpenedView);

    /// <summary>
    /// Activates the specified pad.
    /// </summary>
    void ActivatePad(PadDescriptor content);

    /// <summary>
    /// Returns a pad from a specific type.
    /// </summary>
    PadDescriptor GetPad(Type type);

    /// <summary>
    /// 	Closes all views related to current solution.
    /// </summary>
    /// <returns>
    /// 	True if all views were closed properly, false if closing was aborted.
    /// </returns>
    bool CloseAllSolutionViews(bool force);

    /// <summary>
    /// Gets/Sets the name of the current layout configuration.
    /// Setting this property causes the current layout to be saved, and the specified layout to be loaded.
    /// </summary>
    string CurrentLayoutConfiguration { get; set; }

    /// <summary>
    /// Is called, when a workbench view was opened
    /// </summary>
    /// <example>
    /// WorkbenchSingleton.WorkbenchCreated += delegate {
    /// 	WorkbenchSingleton.Workbench.ViewOpened += ...;
    /// };
    /// </example>
    event EventHandler<ViewContentEventArgs> ViewOpened;

    /// <summary>
    /// Is called, when a workbench view was closed
    /// </summary>
    event EventHandler<ViewContentEventArgs> ViewClosed;
  }
}
