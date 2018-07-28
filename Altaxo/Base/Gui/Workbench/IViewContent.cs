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

using Altaxo.Main.Services;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Interface for the viewmodel part of a document that is shown in the document area of the workbench.
  /// </summary>
  public interface IViewContent : IWorkbenchContent, ICanBeDirty
  {
    System.Windows.Input.ICommand CloseCommand { get; }

    /// <summary>
    /// The tooltip that will be shown when you hover the mouse over the title
    /// </summary>
    string InfoTip
    {
      get;
    }

    /// <summary>
    /// Builds an <see cref="INavigationPoint"/> for the current position.
    /// </summary>
    INavigationPoint BuildNavPoint();

    /// <summary>
    /// Announces that this view content is about to be disposed very soon. The view content should remain passiv (e.g. should not react to events any more),
    /// but should not release its resources yet (this is done later in <see cref="IDisposable.Dispose"/>).
    /// </summary>
    void SetDisposeInProgress();

    bool IsDisposed { get; }

    event EventHandler Disposed;

    /// <summary>
    /// Gets if the view content is read-only (can be saved only when choosing another file name).
    /// </summary>
    bool IsReadOnly { get; }

    /// <summary>
    /// Gets if the view content is view-only (cannot be saved at all).
    /// </summary>
    bool IsViewOnly { get; }

    /// <summary>
    /// Gets whether this view content should be closed when the solution is closed.
    /// </summary>
    bool CloseWithSolution { get; }
  }
}
