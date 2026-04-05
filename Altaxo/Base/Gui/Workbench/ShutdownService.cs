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

#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Main;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Coordinates the multi-stage shutdown of the application.
  /// </summary>
  public class ShutdownService : IShutdownService
  {
    /// <inheritdoc/>
    public bool IsApplicationClosing { get; private set; }
    private CancellationTokenSource _shutdownCancellationTokenSource = new CancellationTokenSource();
    private CancellationTokenSource _delayedShutdownCancellationTokenSource = new CancellationTokenSource();
    private List<string> _reasonsPreventingShutdown = new List<string>();
    private WeakDelegate<EventHandler> _closedEvent = new WeakDelegate<EventHandler>();

    private int _outstandingBackgroundTasks;
    private ManualResetEventSlim _backgroundTaskEvent = new ManualResetEventSlim(true);

    /// <summary>
    /// Occurs when the project is already closed and the rest of the shutdown procedure starts to happen.
    /// This is a weak event in order to prevent garbage collection of item that subscribe to this event.
    /// The event is raised during stage 3 <see cref="OnClosingStage3_SignalShutdownToken"/>.
    /// </summary>
    public event EventHandler? Closed
    {
      add
      {
        _closedEvent.Combine(value);
      }
      remove
      {
        _closedEvent.Remove(value);
      }
    }

    /// <summary>
    /// Occurs when the project is already closed and the rest of the shutdown procedure starts to happen.
    /// This is a weak event in order to prevent garbage collection of item that subscribe to this event.
    /// The event is raised during stage 3 <see cref="OnClosingStage3_SignalShutdownToken"/>. The tasks are executed
    /// in parallel on thread-pool threads, thus make sure that the event handlers are thread-safe.
    /// </summary>
    public WeakAsynchronousEvent ClosedAsync { get; } = new WeakAsynchronousEvent();

    /// <inheritdoc/>
    public CancellationToken ShutdownToken
    {
      get { return _shutdownCancellationTokenSource.Token; }
    }

    /// <inheritdoc/>
    public CancellationToken DelayedShutdownToken
    {
      get { return _delayedShutdownCancellationTokenSource.Token; }
    }

    /// <inheritdoc/>
    public void SignalShutdownToken()
    {
      _shutdownCancellationTokenSource.Cancel();
      _delayedShutdownCancellationTokenSource.CancelAfter(2000);
    }

    /// <inheritdoc/>
    public bool Shutdown()
    {
      Current.Workbench.Close();
      return Current.Workbench is null;
    }

    #region PreventShutdown

    /// <inheritdoc/>
    public IDisposable PreventShutdown(string reason)
    {
      lock (_reasonsPreventingShutdown)
      {
        _reasonsPreventingShutdown.Add(reason);
      }
      return new CallbackOnDispose(
          delegate
          {
            lock (_reasonsPreventingShutdown)
            {
              _reasonsPreventingShutdown.Remove(reason);
            }
          });
    }

    /// <summary>
    /// Gets the current reason that prevents shutdown.
    /// If there isn't any reason, returns null.
    /// If there are multiple reasons, only one of them is returned.
    /// </summary>
    /// <remarks>
    /// This property is thread-safe.
    /// </remarks>
    public string? CurrentReasonPreventingShutdown
    {
      get
      {
        lock (_reasonsPreventingShutdown)
        {
          return _reasonsPreventingShutdown.FirstOrDefault();
        }
      }
    }

    #endregion PreventShutdown

    #region Background Tasks

    /// <inheritdoc/>
    public void AddBackgroundTask(Task task)
    {
      _backgroundTaskEvent.Reset();
      Interlocked.Increment(ref _outstandingBackgroundTasks);
      task.ContinueWith(
          delegate
          {
            if (Interlocked.Decrement(ref _outstandingBackgroundTasks) == 0)
            {
              _backgroundTaskEvent.Set();
            }
          });
    }

    /// <summary>
    /// Waits until all registered background tasks have completed.
    /// </summary>
    internal void WaitForBackgroundTasks()
    {
      if (!_backgroundTaskEvent.IsSet)
      {
        Altaxo.Current.Log.Info("Waiting for background tasks to finish...");
        _backgroundTaskEvent.Wait();
        Altaxo.Current.Log.Info("Background tasks have finished.");
      }
    }

    #endregion Background Tasks

    #region Closing Stages

    /// <summary>
    /// Executes the multi-stage application shutdown.
    /// </summary>
    public void OnClosing(CancelEventArgs e)
    {
      var propertyService = Current.PropertyService;
      IsApplicationClosing = true;

      OnClosingStage1_ReasonsPreventingShutdown(e);
      if (e.Cancel)
      {
        IsApplicationClosing = false;
        return;
      }

      OnClosingStage2_CloseProject(e);
      if (e.Cancel)
      {
        IsApplicationClosing = false;
        return;
      }

      IsApplicationClosing = true;
      OnClosingStage3_SignalShutdownToken();
      OnClosingStage4_CloseAllViews();
      OnClosingStage5_DisposePads();
      OnClosingStage6_StopComServer();
      OnClosingStage7_DisposeProject();

      // The following task could (should?) run _after_ the workbench has closed (and not during closing)
      OnClosingStage8_WaitForBackgroundTasks();
      OnClosingStage9_DisposeServices();
      OnClosingStage10_PropertyServiceSaveToFile(propertyService);
    }

    /// <summary>
    /// Executes shutdown stage 1, which checks whether shutdown is currently blocked.
    /// </summary>
    /// <param name="e">The cancel event arguments used to abort shutdown.</param>
    protected virtual void OnClosingStage1_ReasonsPreventingShutdown(CancelEventArgs e)
    {
      if (CurrentReasonPreventingShutdown is not null)
      {
        Current.MessageService.ShowMessage(CurrentReasonPreventingShutdown);
        e.Cancel = true;
      }
    }

    /// <summary>
    /// Executes shutdown stage 2, which closes the current project if necessary.
    /// </summary>
    /// <param name="e">The cancel event arguments used to abort shutdown.</param>
    protected virtual void OnClosingStage2_CloseProject(CancelEventArgs e)
    {
      Current.Workbench.SaveCompleteWorkbenchStateAndLayoutInPropertyService();

      var projectService = Altaxo.Current.IProjectService;
      if (projectService is not null && (false == (Current.ComManager?.IsInEmbeddedMode ?? false)))
      {
        if (projectService.CurrentProject?.IsDirty == true)
          projectService.AskForSavingOfProject(e); // Save the project, but leave it open
      }
    }

    /// <summary>
    /// Executes shutdown stage 3, which signals the shutdown tokens and raises shutdown events.
    /// </summary>
    protected virtual void OnClosingStage3_SignalShutdownToken()
    {
      SignalShutdownToken();
      _closedEvent.Target?.Invoke(this, EventArgs.Empty);
      ClosedAsync.InvokeParallel(CancellationToken.None).Wait();
    }

    /// <summary>
    /// Executes shutdown stage 4, which closes all views.
    /// </summary>
    protected virtual void OnClosingStage4_CloseAllViews()
    {
      Current.Workbench.CloseAllViews();
    }

    /// <summary>
    /// Executes shutdown stage 5, which disposes all pads.
    /// </summary>
    protected virtual void OnClosingStage5_DisposePads()
    {
      foreach (var pad in Current.Workbench.PadContentCollection)
      {
        pad.Dispose();
      }
    }

    /// <summary>
    /// Executes shutdown stage 6, which stops the local COM server.
    /// </summary>
    protected virtual void OnClosingStage6_StopComServer()
    {
      Current.ComManager?.StopLocalServer();
    }

    /// <summary>
    /// Executes shutdown stage 7, which disposes the current project.
    /// </summary>
    protected virtual void OnClosingStage7_DisposeProject()
    {
      Altaxo.Current.IProjectService?.DisposeProjectAndSetToNull();
    }

    /// <summary>
    /// Executes shutdown stage 8, which waits for background tasks to complete.
    /// </summary>
    protected virtual void OnClosingStage8_WaitForBackgroundTasks()
    {
      WaitForBackgroundTasks();
    }

    /// <summary>
    /// Executes shutdown stage 9, which disposes registered services.
    /// </summary>
    protected virtual void OnClosingStage9_DisposeServices()
    {
      Current.DisposeServicesAll();
    }

    /// <summary>
    /// Executes shutdown stage 10, which saves the property service to persistent storage.
    /// </summary>
    /// <param name="propertyService">The property service to save.</param>
    protected virtual void OnClosingStage10_PropertyServiceSaveToFile(IPropertyService propertyService)
    {
      propertyService.Save();
    }

    #endregion Closing Stages
  }
}
