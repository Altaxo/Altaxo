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
using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Gui.Workbench
{
  public class ShutdownService : IShutdownService
  {
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
    public event EventHandler Closed
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

    public CancellationToken ShutdownToken
    {
      get { return _shutdownCancellationTokenSource.Token; }
    }

    public CancellationToken DelayedShutdownToken
    {
      get { return _delayedShutdownCancellationTokenSource.Token; }
    }

    public void SignalShutdownToken()
    {
      _shutdownCancellationTokenSource.Cancel();
      _delayedShutdownCancellationTokenSource.CancelAfter(2000);
    }

    public bool Shutdown()
    {
      Current.Workbench.Close();
      return Current.Workbench == null;
    }

    #region PreventShutdown

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

    public string CurrentReasonPreventingShutdown
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

    protected virtual void OnClosingStage1_ReasonsPreventingShutdown(CancelEventArgs e)
    {
      if (CurrentReasonPreventingShutdown != null)
      {
        Current.MessageService.ShowMessage(CurrentReasonPreventingShutdown);
        e.Cancel = true;
      }
    }

    protected virtual void OnClosingStage2_CloseProject(CancelEventArgs e)
    {
      Current.Workbench.SaveCompleteWorkbenchStateAndLayoutInPropertyService();

      var projectService = Altaxo.Current.IProjectService;
      if (null != projectService && (false == (Current.ComManager?.IsInEmbeddedMode ?? false)))
      {
        if (projectService.CurrentProject.IsDirty)
          projectService.AskForSavingOfProject(e); // Save the project, but leave it open
      }
    }

    protected virtual void OnClosingStage3_SignalShutdownToken()
    {
      SignalShutdownToken();
      _closedEvent.Target?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnClosingStage4_CloseAllViews()
    {
      Current.Workbench.CloseAllViews();
    }

    protected virtual void OnClosingStage5_DisposePads()
    {
      foreach (var pad in Current.Workbench.PadContentCollection)
      {
        pad.Dispose();
      }
    }

    protected virtual void OnClosingStage6_StopComServer()
    {
      Current.ComManager?.StopLocalServer();
    }

    protected virtual void OnClosingStage7_DisposeProject()
    {
      Altaxo.Current.IProjectService?.DisposeProjectAndSetToNull();
    }

    protected virtual void OnClosingStage8_WaitForBackgroundTasks()
    {
      this.WaitForBackgroundTasks();
    }

    protected virtual void OnClosingStage9_DisposeServices()
    {
      Current.DisposeServicesAll();
    }

    protected virtual void OnClosingStage10_PropertyServiceSaveToFile(IPropertyService propertyService)
    {
      propertyService.Save();
    }

    #endregion Closing Stages
  }
}
