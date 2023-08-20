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
using System.Diagnostics.CodeAnalysis;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Provides the infrastructure to handle generalized code navigation.
  /// </summary>
  /// <remarks>
  /// <para>This service is not limited to navigating code; rather, it
  /// integrates with extendable <see cref="IViewContent"/>
  /// interface so that each window has the opportunity to implement a
  /// contextually appropriate navigation scheme.</para>
  /// <para>The default scheme, <see cref="DefaultNavigationPoint"/>, is
  /// created automatically in the <see cref="AbstractViewContent"/>
  /// implementation.  This scheme supports the basic function of logging a
  /// filename and returning to that file's default view.</para>
  /// <para>The default text editor provides a slightly more sophisticated
  /// scheme, that logs filename and line number.</para>
  /// <para>To implement your own navigation scheme, implement
  /// <see cref="IViewContent"/> or derive from
  /// <see cref="AbstractViewContent"/> and override the
  /// <see cref="IViewContent.BuildNavPoint">BuildNavigationPoint</see>
  /// method.</para>
  /// <para>
  /// <i>History logic based in part on Orlando Curioso's <i>Code Project</i> article:
  /// <see href="http://www.codeproject.com/cs/miscctrl/WinFormsHistory.asp">
  /// Navigational history (go back/forward) for WinForms controls</see></i>
  /// </para>
  /// </remarks>
  public sealed class NavigationService
  {
    #region Private members

    private static LinkedList<INavigationPoint> _history = new LinkedList<INavigationPoint>();
    private static LinkedListNode<INavigationPoint>? _currentNode;
    private static int _loggingSuspendedCount;
    private static bool _serviceInitialized;

    #endregion Private members

    /// <summary>
    /// Keeps .NET compiler from autogenerating a public constructor that breaks an FxCop rule #CA1053
    /// </summary>
    private NavigationService()
    {
    }

    /// <summary>
    /// Initializes the NavigationService.
    /// </summary>
    /// <remarks>Must be called after the Workbench has been initialized and after the ProjectService has been initialized.</remarks>
    /// <exception cref="InvalidOperationException">The <see cref="IWorkbenchEx"/> has not yet been initialized and <see cref="IWorkbenchEx"></see> is <value>null</value></exception>
    public static void InitializeService()
    {
      var workbench = Altaxo.Current.GetService<IWorkbenchEx>();

      if (!_serviceInitialized)
      {
        if (workbench is null)
        {
          throw new InvalidOperationException("Initializing the NavigationService requires that the WorkbenchSingleton has already created a Workbench.");
        }
        // trap changes in the secondary tab via the workbench's ActiveViewContentChanged event
        workbench.ActiveViewContentChanged += ActiveViewContentChanged;

        Altaxo.Current.GetRequiredService<IFileService>().FileRenamed += FileService_FileRenamed;
        _serviceInitialized = true;
      }
    }

    /// <summary>
    /// Unloads the <see cref="NavigationService"/>
    /// </summary>
    public static void Unload()
    {
      // perform any necessary cleanup
      HistoryChanged = null;
      ClearHistory(true);
      _serviceInitialized = false;
    }

    #region Public Properties

    /// <summary>
    /// <b>true</b> if we can navigate back to a previous point; <b>false</b>
    /// if there are no points in the history.
    /// </summary>
    public static bool CanNavigateBack
    {
      get { return _currentNode != _history.First && _currentNode is not null; }
    }

    /// <summary>
    /// <b>true</b> if we can navigate forwards to a point prevously left
    /// via navigating backwards; <b>false</b> if all the points in the
    /// history are in the "past".
    /// </summary>
    public static bool CanNavigateForwards
    {
      get { return _currentNode != _history.Last && _currentNode is not null; }
    }

    /// <summary>
    /// Gets the number of points in the navigation history.
    /// </summary>
    /// <remarks>
    /// <b>Note:</b> jumping forwards or backwards requires at least
    /// two points in the history; otherwise navigating has no meaning.
    /// </remarks>
    public static int Count
    {
      get { return _history.Count; }
    }

    /// <summary>
    /// Gets or sets the "current" position as tracked by the service.
    /// </summary>
    [MaybeNull]
    public static INavigationPoint? CurrentPosition
    {
      get
      {
        return _currentNode?.Value;
      }
      set
      {
        Log(value);
      }
    }

    /// <summary>
    /// <b>true</b> when the service is logging points; <b>false</b> when
    /// logging is suspended.
    /// </summary>
    public static bool IsLogging
    {
      get { return _loggingSuspendedCount == 0; }
    }

    #endregion Public Properties

    #region Public Methods

    // TODO: FxCop says "find another way to do this" (ReviewVisibleEventHandlers)
    // we'd have to ask each point that cares to subscribe to the appropriate event
    // listeners in their respective IViewContent implementation's underlying models.
    public static void ContentChanging(object sender, EventArgs e)
    {
      foreach (INavigationPoint p in _history)
      {
        p.ContentChanging(sender, e);
      }
    }

    /*		static void Log(IWorkbenchWindow window)
{
    if (window==null) return;
    Log(window.ActiveViewContent);
}
 */

    /// <summary>
    /// Asks an <see cref="IViewContent"/> implementation to build an
    /// <see cref="INavigationPoint"/> and then logs it.
    /// </summary>
    /// <param name="vc"></param>
    public static void Log(IViewContent vc)
    {
      if (vc is null)
        return;
      Log(vc.BuildNavPoint());
    }

    /// <summary>
    /// Adds an <see cref="INavigationPoint"/> to the history.
    /// </summary>
    /// <param name="pointToLog">The point to store.</param>
    public static void Log(INavigationPoint? pointToLog)
    {
      if (_loggingSuspendedCount > 0)
      {
        return;
      }
      LogInternal(pointToLog);
    }

    /// <summary>
    /// Adds an <see cref="INavigationPoint"/> to the history.
    /// </summary>
    /// <param name="p">The <see cref="INavigationPoint"/> to add.</param>
    /// <remarks>
    /// Refactoring this out of Log() allows the NavigationService
    /// to call this and ensure it will work regardless of the
    /// requested state of loggingSuspended, as in
    /// <see cref="ClearHistory()"/> where we want to log
    /// the current position after clearing the
    /// history.
    /// </remarks>
    private static void LogInternal(INavigationPoint? p)
    {
      if (p is null
              || string.IsNullOrEmpty(p.FileName)
           )
      {
        return;
      }
      if (_currentNode is null)
      {
        _currentNode = _history.AddFirst(p);
      }
      else if (p.Equals(_currentNode.Value))
      {
        // replace it
        _currentNode.Value = p;
      }
      else
      {
        _currentNode = _history.AddAfter(_currentNode, p);
      }
      OnHistoryChanged();
    }

    /// <summary>
    /// Gets a <see cref="List{T}"/> of the <see cref="INavigationPoint">INavigationPoints</see> that
    /// are currently in the collection.
    /// </summary>
    public static ICollection<INavigationPoint> Points
    {
      get
      {
        return new List<INavigationPoint>(_history);
      }
    }

    /// <summary>
    /// Clears the navigation history (except for the current position).
    /// </summary>
    public static void ClearHistory()
    {
      ClearHistory(false);
    }

    /// <summary>
    /// Clears the navigation history and optionally clears the current position.
    /// </summary>
    /// <param name="clearCurrentPosition">Do we clear the current position as well as the rest of the history?</param>
    /// <remarks>
    /// <para>The current position is often used to "seed" the next history to ensure
    /// that the first significant movement after clearing the history allows
    /// us to jump "back" immediately.</para>
    /// <para>Remembering the current position across requests to clear the history
    /// does not always make sense, however, such as when a solution is closing,
    /// hence the ability to explicitly control it's retention.</para>
    /// </remarks>
    public static void ClearHistory(bool clearCurrentPosition)
    {
      var currentPosition = CurrentPosition;
      _history.Clear();
      _currentNode = null;
      if (!clearCurrentPosition)
      {
        LogInternal(currentPosition);
      }
      OnHistoryChanged();
    }

    /// <summary>
    /// Navigates to an <see cref="INavigationPoint"/> that is an arbitrary
    /// number of points away from the <see cref="CurrentPosition"/>.
    /// </summary>
    /// <param name="delta">Number of points to move; negative deltas move
    /// backwards while positive deltas move forwards through the history.</param>
    public static void Go(int delta)
    {
      if (0 == delta)
      {
        // no movement required
        return;
      }
      else if (0 > delta)
      {
        // move backwards
        while (0 > delta && _currentNode != _history.First && _currentNode is not null)
        {
          _currentNode = _currentNode.Previous;
          delta++;
        }
      }
      else
      {
        // move forwards
        while (0 < delta && _currentNode != _history.Last && _currentNode is not null)
        {
          _currentNode = _currentNode.Next;
          delta--;
        }
      }

      SyncViewWithModel();
    }

    /// <summary>
    /// Jump to a specific <see cref="INavigationPoint"/> in the history;
    /// if the point is not in the history, we log it internally, regardless
    /// of whether logging is currently suspended or not.
    /// </summary>
    /// <param name="target">The <see cref="INavigationPoint"/> to jump</param>
    public static void Go(INavigationPoint? target)
    {
      if (target is null)
      {
        return;
      }

      var targetNode = _history.Find(target);
      if (targetNode is not null)
      {
        _currentNode = targetNode;
      }
      else
      {
        Current.Log.ErrorFormatted("Logging additional point: {0}", target);
        LogInternal(target);
      }

      SyncViewWithModel();
    }

    /// <summary>
    /// Navigates the view (i.e. the workbench) to whatever
    /// <see cref="INavigationPoint"/> is the current
    /// position in the internal model.
    /// </summary>
    /// <remarks>Factoring this out of code that manipulates
    /// the history allows to make multiple changes to the
    /// history while only updating the view once we are
    /// finished.</remarks>
    private static void SyncViewWithModel()
    {
      SuspendLogging();
      if (CurrentPosition is not null)
      {
        CurrentPosition.JumpTo();
      }
      ResumeLogging();
    }

    /// <summary>
    /// Suspends logging of navigation so that we don't log intermediate points
    /// while opening a file, for example.
    /// </summary>
    /// <remarks>
    /// Suspend/resume statements are incremental, i.e. you must call
    /// <see cref="ResumeLogging"/> once for each time you call
    /// <see cref="SuspendLogging"/> to resume logging.
    /// </remarks>
    public static void SuspendLogging()
    {
      Current.Log.Debug("NavigationService -- suspend logging");
      System.Threading.Interlocked.Increment(ref _loggingSuspendedCount);
    }

    /// <summary>
    /// Resumes logging after suspending it via <see cref="SuspendLogging"/>.
    /// </summary>
    public static void ResumeLogging()
    {
      Current.Log.Debug("NavigationService -- resume logging");
      if (System.Threading.Interlocked.Decrement(ref _loggingSuspendedCount) < 0)
      {
        System.Threading.Interlocked.Increment(ref _loggingSuspendedCount);
        Current.Log.Warn("NavigationService -- ResumeLogging called without corresponding SuspendLogging");
      }
    }

    #endregion Public Methods

    // the following code is not covered by Unit tests as i wasn't sure
    // how to test code triggered by the user interacting with the workbench

    #region event trapping

    /// <summary>
    /// Respond to changes in the <see cref="IWorkbench.ActiveViewContent">
    /// ActiveViewContent</see> by logging the new <see cref="IViewContent"/>.
    /// </summary>
    private static void ActiveViewContentChanged(object? sender, EventArgs e)
    {
      var vc = Altaxo.Current.GetRequiredService<IWorkbench>().ActiveViewContent;
      if (vc is null)
        return;
      Current.Log.Debug($"NavigationService\n\tActiveViewContent: {vc.Title}\n\t          Subview: {vc.Title}");
      Log(vc);
    }

    /// <summary>
    /// Respond to changes in filenames by updating points in the history
    /// to reflect the change.
    /// </summary>
    /// <param name="sender"/>
    /// <param name="e"><see cref="FileRenameEventArgs"/> describing
    /// the file rename.</param>
    private static void FileService_FileRenamed(object? sender, FileRenameEventArgs e)
    {
      foreach (INavigationPoint p in _history)
      {
        if (p.FileName.Equals(e.SourceFile))
        {
          p.FileNameChanged(e.TargetFile);
        }
      }
    }

    /// <summary>
    /// Responds to the ProjectService's SolutionClosed event.
    /// </summary>
    private static void ProjectService_SolutionClosed(object sender, EventArgs e)
    {
      NavigationService.ClearHistory(true);
    }

    #endregion event trapping

    #region Public Events

    /// <summary>
    /// Fires whenever the navigation history has changed.
    /// </summary>
    public static event System.EventHandler? HistoryChanged;

    /// <summary>
    /// Used internally to call the <see cref="HistoryChanged"/> event delegates.
    /// </summary>
    private static void OnHistoryChanged()
    {
      HistoryChanged?.Invoke(NavigationService.CurrentPosition, EventArgs.Empty);
      Current.Gui.InvalidateRequerySuggested();
    }

    #endregion Public Events
  }
}
