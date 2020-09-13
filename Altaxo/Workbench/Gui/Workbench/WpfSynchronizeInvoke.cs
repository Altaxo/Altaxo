// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using Altaxo.Main;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Implements the ISynchronizeInvoke interface by using a WPF dispatcher
  /// to perform the cross-thread call.
  /// </summary>
  internal sealed class WpfSynchronizeInvoke : ISynchronizeInvoke
  {
    private readonly Dispatcher dispatcher;

    public WpfSynchronizeInvoke(Dispatcher dispatcher)
    {
      if (dispatcher is null)
        throw new ArgumentNullException("dispatcher");
      this.dispatcher = dispatcher;
    }

    public bool InvokeRequired
    {
      get
      {
        return !dispatcher.CheckAccess();
      }
    }

    public IAsyncResult BeginInvoke(Delegate method, object?[]? args)
    {
      DispatcherOperation op;
      if (args is null || args.Length == 0)
        op = dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
      else if (args.Length == 1)
        op = dispatcher.BeginInvoke(DispatcherPriority.Normal, method, args[0]);
      else
        op = dispatcher.BeginInvoke(DispatcherPriority.Normal, method, args[0], args.Splice(1));
      return new AsyncResult(op);
    }

    private sealed class AsyncResult : IAsyncResult
    {
      internal readonly DispatcherOperation _dispatcherOperation;
      private readonly object _lockObj = new object();
      private ManualResetEvent? _resetEvent;

      public AsyncResult(DispatcherOperation op)
      {
        this._dispatcherOperation = op;
      }

      public bool IsCompleted
      {
        get
        {
          return _dispatcherOperation.Status == DispatcherOperationStatus.Completed;
        }
      }

      public WaitHandle AsyncWaitHandle
      {
        get
        {
          lock (_lockObj)
          {
            if (_resetEvent is null)
            {
              _dispatcherOperation.Completed += op_Completed;
              _resetEvent = new ManualResetEvent(false);
              if (IsCompleted)
                _resetEvent.Set();
            }
            return _resetEvent;
          }
        }
      }

      private void op_Completed(object? sender, EventArgs e)
      {
        lock (_lockObj)
        {
          _resetEvent?.Set();
        }
      }

      public object? AsyncState
      {
        get { return null; }
      }

      public bool CompletedSynchronously
      {
        get { return false; }
      }
    }

    public object EndInvoke(IAsyncResult result)
    {
      var r = result as AsyncResult;
      if (r is null)
        throw new ArgumentException("result must be the return value of a WpfSynchronizeInvoke.BeginInvoke call!");
      r._dispatcherOperation.Wait();
      return r._dispatcherOperation.Result;
    }

    public object? Invoke(Delegate method, object?[]? args)
    {
      object? result = null;
      Exception? exception = null;
      dispatcher.Invoke(
          DispatcherPriority.Normal,
          (Action)delegate
          {
            try
            {
              result = method.DynamicInvoke(args);
            }
            catch (TargetInvocationException ex)
            {
              exception = ex.InnerException;
            }
          });
      // if an exception occurred, re-throw it on the calling thread
      if (exception is not null)
        throw new TargetInvocationException(exception);
      return result;
    }
  }
}
