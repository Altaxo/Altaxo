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

#nullable disable warnings
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// A fake IMessageLoop implementation that always has <c>InvokeRequired=false</c> and synchronously invokes
  /// the callback passed to <c>InvokeIfRequired</c>.
  /// </summary>
  internal sealed class FakeMessageLoop : IDispatcherMessageLoopWpf
  {
    public Thread Thread
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public Dispatcher Dispatcher
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public SynchronizationContext SynchronizationContext
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public System.ComponentModel.ISynchronizeInvoke SynchronizingObject
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public bool InvokeRequired
    {
      get { return false; }
    }

    public bool CheckAccess()
    {
      return true;
    }

    public void VerifyAccess()
    {
    }

    #region InvokeIfRequired (Actions)

    public void InvokeIfRequired(Action callback)
    {
      callback();
    }

    /// <summary>
    /// Executes an action synchronously with the GUI.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg">The argument of the action.</param>
    public void InvokeIfRequired<T>(Action<T> action, T arg)
    {
      action(arg);
    }

    /// <summary>
    /// Executes an action synchronously with the GUI.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The first argument of the action.</param>
    /// <param name="arg2">The second argument of the action.</param>
    public void InvokeIfRequired<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
    {
      action(arg1, arg2);
    }

    /// <summary>
    /// Executes an action synchronously with the GUI.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The 1st argument of the action.</param>
    /// <param name="arg2">The 2nd argument of the action.</param>
    /// <param name="arg3">The 3rd argument of the action.</param>
    public void InvokeIfRequired<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
    {
      action(arg1, arg2, arg3);
    }

    /// <summary>
    /// Executes an action synchronously with the GUI.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The first argument of the action.</param>
    /// <param name="arg2">The second argument of the action.</param>
    /// <param name="arg3">The 3rd argument of the action.</param>
    /// <param name="arg4">The 4th argument of the action.</param>
    public void InvokeIfRequired<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
      action(arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Executes an action synchronously with the Gui.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The first argument of the action.</param>
    /// <param name="arg2">The second argument of the action.</param>
    /// <param name="arg3">The 3rd argument of the action.</param>
    /// <param name="arg4">The 4th argument of the action.</param>
    /// <param name="arg5">The 5th argument of the action.</param>
    public void InvokeIfRequired<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
      action(arg1, arg2, arg3, arg4, arg5);
    }

    public void InvokeIfRequired(Action callback, DispatcherPriority priority)
    {
      callback();
    }

    public void InvokeIfRequired(Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
    {
      callback();
    }

    #endregion InvokeIfRequired (Actions)

    #region InvokeIfRequired (Functions)

    public T InvokeIfRequired<T>(Func<T> callback)
    {
      return callback();
    }

    /// <summary>
    /// Evaluates a function synchronously with the Gui.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult">The type of function result.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="arg">The 1st function argument.</param>
    /// <returns>The result of the function evaluation.</returns>
    public TResult InvokeIfRequired<T, TResult>(Func<T, TResult> function, T arg)
    {
      return function(arg);
    }

    public TResult InvokeIfRequired<T1, T2, TResult>(Func<T1, T2, TResult> function, T1 arg1, T2 arg2)
    {
      return function(arg1, arg2);
    }

    public TResult InvokeIfRequired<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function, T1 arg1, T2 arg2, T3 arg3)
    {
      return function(arg1, arg2, arg3);
    }

    public TResult InvokeIfRequired<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
      return function(arg1, arg2, arg3, arg4);
    }

    public TResult InvokeIfRequired<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
      return function(arg1, arg2, arg3, arg4, arg5);
    }

    public TResult InvokeIfRequired<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    {
      return function(arg1, arg2, arg3, arg4, arg5, arg6);
    }

    public TResult InvokeIfRequired<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    {
      return function(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    public T InvokeIfRequired<T>(Func<T> callback, DispatcherPriority priority)
    {
      return callback();
    }

    public T InvokeIfRequired<T>(Func<T> callback, DispatcherPriority priority, CancellationToken cancellationToken)
    {
      return callback();
    }

    #endregion InvokeIfRequired (Functions)

    #region InvokeAndForget (Actions)

    public void InvokeAndForget(Action callback)
    {
      callback();
    }

    /// <summary>
    /// Executes an action synchronously with the Gui without waiting.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg">The argument of the action.</param>
    public void InvokeAndForget<T>(Action<T> action, T arg)
    {
      action(arg);
    }

    /// <summary>
    /// Executes an action synchronously with the Gui without waiting.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The first argument of the action.</param>
    /// <param name="arg2">The second argument of the action.</param>
    public void InvokeAndForget<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
    {
      action(arg1, arg2);
    }

    /// <summary>
    /// Executes an action synchronously with the Gui without waiting.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The 1st argument of the action.</param>
    /// <param name="arg2">The 2nd argument of the action.</param>
    /// <param name="arg3">The 3rd argument of the action.</param>
    public void InvokeAndForget<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
    {
      action(arg1, arg2, arg3);
    }

    /// <summary>
    /// Executes an action synchronously with the Gui without waiting.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The first argument of the action.</param>
    /// <param name="arg2">The second argument of the action.</param>
    /// <param name="arg3">The 3rd argument of the action.</param>
    /// <param name="arg4">The 4th argument of the action.</param>
    public void InvokeAndForget<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
      action(arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Executes an action synchronously with the Gui without waiting.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The first argument of the action.</param>
    /// <param name="arg2">The second argument of the action.</param>
    /// <param name="arg3">The 3rd argument of the action.</param>
    /// <param name="arg4">The 4th argument of the action.</param>
    /// <param name="arg5">The 5th argument of the action.</param>
    public void InvokeAndForget<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
      action(arg1, arg2, arg3, arg4, arg5);
    }

    public void InvokeAndForget(Action callback, DispatcherPriority priority)
    {
      throw new NotImplementedException();
    }

    #endregion InvokeAndForget (Actions)

    #region InvokeLaterAndForget (Actions)

    public void InvokeLaterAndForget(TimeSpan delay, Action method)
    {
      throw new NotImplementedException();
    }

    #endregion InvokeLaterAndForget (Actions)

    #region InvokeAsync

    public Task InvokeAsync(Action callback)
    {
      throw new NotImplementedException();
    }

    public Task InvokeAsync(Action callback, DispatcherPriority priority)
    {
      throw new NotImplementedException();
    }

    public Task InvokeAsync(Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<T> InvokeAsync<T>(Func<T> callback)
    {
      throw new NotImplementedException();
    }

    public Task<T> InvokeAsync<T>(Func<T> callback, DispatcherPriority priority)
    {
      throw new NotImplementedException();
    }

    public Task<T> InvokeAsync<T>(Func<T> callback, DispatcherPriority priority, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    #endregion InvokeAsync

    #region ISynchronizeInvoke

    public IAsyncResult BeginInvoke(Delegate method, object[] args)
    {
      throw new NotImplementedException();
    }

    public object EndInvoke(IAsyncResult result)
    {
      throw new NotImplementedException();
    }

    public object Invoke(Delegate method, object[] args)
    {
      throw new NotImplementedException();
    }

    #endregion ISynchronizeInvoke
  }
}
