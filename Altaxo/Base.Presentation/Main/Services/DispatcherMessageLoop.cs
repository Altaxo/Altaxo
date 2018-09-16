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
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Altaxo.Main.Services
{
  public sealed class DispatcherMessageLoop : IDispatcherMessageLoopWpf
  {
    private readonly Dispatcher _dispatcher;
    private readonly SynchronizationContext _synchronizationContext;

    public DispatcherMessageLoop(Dispatcher dispatcher, SynchronizationContext synchronizationContext)
    {
      _dispatcher = dispatcher;
      _synchronizationContext = synchronizationContext;
    }

    /// <inheritdoc/>
    public Thread Thread
    {
      get { return _dispatcher.Thread; }
    }

    /// <inheritdoc/>
    public Dispatcher Dispatcher
    {
      get { return _dispatcher; }
    }

    /// <inheritdoc/>
    public SynchronizationContext SynchronizationContext
    {
      get { return _synchronizationContext; }
    }

    /// <inheritdoc/>
    public ISynchronizeInvoke SynchronizingObject
    {
      get { return this; }
    }

    /// <inheritdoc/>
    public bool InvokeRequired
    {
      get { return !_dispatcher.CheckAccess(); }
    }

    /// <inheritdoc/>
    public bool CheckAccess()
    {
      return _dispatcher.CheckAccess();
    }

    /// <inheritdoc/>
    public void VerifyAccess()
    {
      _dispatcher.VerifyAccess();
    }

    #region InvokeIfRequired (Actions)

    /// <inheritdoc/>
    public void InvokeIfRequired(Action callback)
    {
      if (_dispatcher.CheckAccess())
        callback();
      else
        _dispatcher.Invoke(callback);
    }

    /// <summary>
    /// Executes an action synchronously with the GUI.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg">The argument of the action.</param>
    public void InvokeIfRequired<T>(Action<T> action, T arg)
    {
      if (_dispatcher.CheckAccess())
        action(arg);
      else
        _dispatcher.Invoke(action, new object[] { arg });
    }

    /// <summary>
    /// Executes an action synchronously with the GUI.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The first argument of the action.</param>
    /// <param name="arg2">The second argument of the action.</param>
    public void InvokeIfRequired<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
    {
      if (_dispatcher.CheckAccess())
        action(arg1, arg2);
      else
        _dispatcher.Invoke(action, new object[] { arg1, arg2 });
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
      if (_dispatcher.CheckAccess())
        action(arg1, arg2, arg3);
      else
        _dispatcher.Invoke(action, new object[] { arg1, arg2, arg3 });
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
      if (_dispatcher.CheckAccess())
        action(arg1, arg2, arg3, arg4);
      else
        _dispatcher.Invoke(action, new object[] { arg1, arg2, arg3, arg4 });
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
      if (_dispatcher.CheckAccess())
        action(arg1, arg2, arg3, arg4, arg5);
      else
        _dispatcher.Invoke(action, new object[] { arg1, arg2, arg3, arg4, arg5 });
    }

    /// <inheritdoc/>
    public void InvokeIfRequired(Action callback, DispatcherPriority priority)
    {
      if (_dispatcher.CheckAccess())
        callback();
      else
        _dispatcher.Invoke(callback, priority);
    }

    /// <inheritdoc/>
    public void InvokeIfRequired(Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
    {
      if (_dispatcher.CheckAccess())
        callback();
      else
        _dispatcher.Invoke(callback, priority, cancellationToken);
    }

    #endregion InvokeIfRequired (Actions)

    #region InvokeIfRequired (Functions)

    /// <inheritdoc/>
    public T InvokeIfRequired<T>(Func<T> callback)
    {
      if (_dispatcher.CheckAccess())
        return callback();
      else
        return _dispatcher.Invoke(callback);
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
      if (_dispatcher.CheckAccess())
        return function(arg);
      else
        return (TResult)_dispatcher.Invoke(function, new object[] { arg });
    }

    /// <inheritdoc/>
    public TResult InvokeIfRequired<T1, T2, TResult>(Func<T1, T2, TResult> function, T1 arg1, T2 arg2)
    {
      if (_dispatcher.CheckAccess())
        return function(arg1, arg2);
      else
        return (TResult)_dispatcher.Invoke(function, new object[] { arg1, arg2 });
    }

    /// <inheritdoc/>
    public TResult InvokeIfRequired<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function, T1 arg1, T2 arg2, T3 arg3)
    {
      if (_dispatcher.CheckAccess())
        return function(arg1, arg2, arg3);
      else
        return (TResult)_dispatcher.Invoke(function, new object[] { arg1, arg2, arg3 });
    }

    /// <inheritdoc/>
    public TResult InvokeIfRequired<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
      if (_dispatcher.CheckAccess())
        return function(arg1, arg2, arg3, arg4);
      else
        return (TResult)_dispatcher.Invoke(function, new object[] { arg1, arg2, arg3, arg4 });
    }

    /// <inheritdoc/>
    public TResult InvokeIfRequired<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
      if (_dispatcher.CheckAccess())
        return function(arg1, arg2, arg3, arg4, arg5);
      else
        return (TResult)_dispatcher.Invoke(function, new object[] { arg1, arg2, arg3, arg4, arg5 });
    }

    /// <inheritdoc/>
    public TResult InvokeIfRequired<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    {
      if (_dispatcher.CheckAccess())
        return function(arg1, arg2, arg3, arg4, arg5, arg6);
      else
        return (TResult)_dispatcher.Invoke(function, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
    }

    /// <inheritdoc/>
    public TResult InvokeIfRequired<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    {
      if (_dispatcher.CheckAccess())
        return function(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
      else
        return (TResult)_dispatcher.Invoke(function, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
    }

    /// <inheritdoc/>
    public T InvokeIfRequired<T>(Func<T> callback, DispatcherPriority priority)
    {
      if (_dispatcher.CheckAccess())
        return callback();
      else
        return _dispatcher.Invoke(callback, priority);
    }

    /// <inheritdoc/>
    public T InvokeIfRequired<T>(Func<T> callback, DispatcherPriority priority, CancellationToken cancellationToken)
    {
      if (_dispatcher.CheckAccess())
        return callback();
      else
        return _dispatcher.Invoke(callback, priority, cancellationToken);
    }

    #endregion InvokeIfRequired (Functions)

    #region InvokeAndForget (Actions)

    /// <inheritdoc/>
    public void InvokeAndForget(Action callback)
    {
      _dispatcher.BeginInvoke(callback);
    }

    /// <summary>
    /// Executes an action synchronously with the Gui without waiting.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg">The argument of the action.</param>
    public void InvokeAndForget<T>(Action<T> action, T arg)
    {
      _dispatcher.BeginInvoke(action, new object[] { arg });
    }

    /// <summary>
    /// Executes an action synchronously with the Gui without waiting.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The first argument of the action.</param>
    /// <param name="arg2">The second argument of the action.</param>
    public void InvokeAndForget<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
    {
      _dispatcher.BeginInvoke(action, new object[] { arg1, arg2 });
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
      _dispatcher.BeginInvoke(action, new object[] { arg1, arg2, arg3 });
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
      _dispatcher.BeginInvoke(action, new object[] { arg1, arg2, arg3, arg4 });
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
      _dispatcher.BeginInvoke(action, new object[] { arg1, arg2, arg3, arg4, arg5 });
    }

    /// <inheritdoc/>
    public void InvokeAndForget(Action callback, DispatcherPriority priority)
    {
      _dispatcher.BeginInvoke(callback, priority);
    }

    #endregion InvokeAndForget (Actions)

    #region InvokeLaterAndForget (Actions)

    /// <inheritdoc/>
    public async void InvokeLaterAndForget(TimeSpan delay, Action method)
    {
      await Task.Delay(delay).ConfigureAwait(false);
      InvokeAndForget(method);
    }

    #endregion InvokeLaterAndForget (Actions)

    #region InvokeAsync

    /// <inheritdoc/>
    public Task InvokeAsync(Action callback)
    {
      return _dispatcher.InvokeAsync(callback).Task;
    }

    /// <inheritdoc/>
    public Task InvokeAsync(Action callback, DispatcherPriority priority)
    {
      return _dispatcher.InvokeAsync(callback, priority).Task;
    }

    /// <inheritdoc/>
    public Task InvokeAsync(Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
    {
      return _dispatcher.InvokeAsync(callback, priority, cancellationToken).Task;
    }

    /// <inheritdoc/>
    public Task<T> InvokeAsync<T>(Func<T> callback)
    {
      return _dispatcher.InvokeAsync(callback).Task;
    }

    /// <inheritdoc/>
    public Task<T> InvokeAsync<T>(Func<T> callback, DispatcherPriority priority)
    {
      return _dispatcher.InvokeAsync(callback, priority).Task;
    }

    /// <inheritdoc/>
    public Task<T> InvokeAsync<T>(Func<T> callback, DispatcherPriority priority, CancellationToken cancellationToken)
    {
      return _dispatcher.InvokeAsync(callback, priority, cancellationToken).Task;
    }

    #endregion InvokeAsync

    #region ISynchronizeInvoke

    /// <inheritdoc/>
    IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
    {
      return _dispatcher.BeginInvoke(method, args).Task;
    }

    /// <inheritdoc/>
    object ISynchronizeInvoke.EndInvoke(IAsyncResult result)
    {
      return ((Task<object>)result).Result;
    }

    /// <inheritdoc/>
    object ISynchronizeInvoke.Invoke(Delegate method, object[] args)
    {
      return _dispatcher.Invoke(method, args);
    }

    #endregion ISynchronizeInvoke
  }
}
