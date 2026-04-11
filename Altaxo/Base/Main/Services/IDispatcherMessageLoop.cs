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
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Represents a thread running a message loop. This is the non-GUI part of the interface. The GUI interface
  /// will contain a dispatcher object, and additional possibilities to set the priority.
  /// </summary>
  public interface IDispatcherMessageLoop : ISynchronizeInvoke
  {
    /// <summary>
    /// Gets the thread that runs this message loop.
    /// </summary>
    Thread Thread { get; }

    /// <summary>
    /// Gets the synchronization context corresponding to this message loop.
    /// </summary>
    SynchronizationContext SynchronizationContext { get; }

    /// <summary>
    /// Gets the <see cref="ISynchronizeInvoke"/> implementation corresponding to this message loop.
    /// </summary>
    ISynchronizeInvoke SynchronizingObject { get; }

    /// <summary>
    /// Gets whether the current thread is the same as the thread running this message loop.
    /// </summary>
    /// <remarks><c>CheckAccess() = !InvokeRequired</c></remarks>
    /// <returns><see langword="true"/> if the current thread runs this message loop; otherwise, <see langword="false"/>.</returns>
    bool CheckAccess();

    /// <summary>
    /// Throws an exception if the current thread is different from the thread running this message loop.
    /// </summary>
    void VerifyAccess();

    #region InvokeIfRequired (Actions)

    /// <summary>
    /// Invokes the specified callback on the message loop and waits for its completion.
    /// If the current thread is the thread running the message loop, executes the callback
    /// directly without pumping the message loop.
    /// </summary>
    /// <param name="callback">The callback to execute.</param>
    void InvokeIfRequired(Action callback);

    /// <summary>
    /// Executes an action synchronously on the message loop thread.
    /// </summary>
    /// <typeparam name="T">The type of the action argument.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg">The argument of the action.</param>
    void InvokeIfRequired<T>(Action<T> action, T arg);

    /// <summary>
    /// Executes an action synchronously on the message loop thread.
    /// </summary>
    /// <typeparam name="T1">The type of the first action argument.</typeparam>
    /// <typeparam name="T2">The type of the second action argument.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The first argument of the action.</param>
    /// <param name="arg2">The second argument of the action.</param>
    void InvokeIfRequired<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2);

    /// <summary>
    /// Executes an action synchronously on the message loop thread.
    /// </summary>
    /// <typeparam name="T1">The type of the first action argument.</typeparam>
    /// <typeparam name="T2">The type of the second action argument.</typeparam>
    /// <typeparam name="T3">The type of the third action argument.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The 1st argument of the action.</param>
    /// <param name="arg2">The 2nd argument of the action.</param>
    /// <param name="arg3">The 3rd argument of the action.</param>
    void InvokeIfRequired<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// Executes an action synchronously on the message loop thread.
    /// </summary>
    /// <typeparam name="T1">The type of the first action argument.</typeparam>
    /// <typeparam name="T2">The type of the second action argument.</typeparam>
    /// <typeparam name="T3">The type of the third action argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth action argument.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The first argument of the action.</param>
    /// <param name="arg2">The second argument of the action.</param>
    /// <param name="arg3">The 3rd argument of the action.</param>
    /// <param name="arg4">The 4th argument of the action.</param>
    void InvokeIfRequired<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    /// <summary>
    /// Executes an action synchronously on the message loop thread.
    /// </summary>
    /// <typeparam name="T1">The type of the first action argument.</typeparam>
    /// <typeparam name="T2">The type of the second action argument.</typeparam>
    /// <typeparam name="T3">The type of the third action argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth action argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth action argument.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The first argument of the action.</param>
    /// <param name="arg2">The second argument of the action.</param>
    /// <param name="arg3">The 3rd argument of the action.</param>
    /// <param name="arg4">The 4th argument of the action.</param>
    /// <param name="arg5">The 5th argument of the action.</param>
    void InvokeIfRequired<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    #endregion InvokeIfRequired (Actions)

    #region InvokeIfRequired (Functions)

    /// <summary>
    /// Invokes the specified callback, waits for its completion, and returns the result.
    /// If the current thread is the thread running the message loop, executes the callback
    /// directly without pumping the message loop.
    /// </summary>
    /// <typeparam name="T">The type of the callback result.</typeparam>
    /// <param name="callback">The callback to execute.</param>
    /// <returns>The callback result.</returns>
    T InvokeIfRequired<T>(Func<T> callback);

    /// <summary>
    /// Evaluates a function synchronously on the message loop thread.
    /// </summary>
    /// <typeparam name="T">The type of the function argument.</typeparam>
    /// <typeparam name="TResult">The type of function result.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="arg">The 1st function argument.</param>
    /// <returns>The result of the function evaluation.</returns>
    TResult InvokeIfRequired<T, TResult>(Func<T, TResult> function, T arg);

    /// <summary>
    /// Evaluates a function synchronously on the message loop thread.
    /// </summary>
    /// <typeparam name="T1">The type of the first function argument.</typeparam>
    /// <typeparam name="T2">The type of the second function argument.</typeparam>
    /// <typeparam name="TResult">The type of the function result.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="arg1">The first function argument.</param>
    /// <param name="arg2">The second function argument.</param>
    /// <returns>The result of the function evaluation.</returns>
    TResult InvokeIfRequired<T1, T2, TResult>(Func<T1, T2, TResult> function, T1 arg1, T2 arg2);

    /// <summary>
    /// Evaluates a function synchronously on the message loop thread.
    /// </summary>
    /// <typeparam name="T1">The type of the first function argument.</typeparam>
    /// <typeparam name="T2">The type of the second function argument.</typeparam>
    /// <typeparam name="T3">The type of the third function argument.</typeparam>
    /// <typeparam name="TResult">The type of the function result.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="arg1">The first function argument.</param>
    /// <param name="arg2">The second function argument.</param>
    /// <param name="arg3">The third function argument.</param>
    /// <returns>The result of the function evaluation.</returns>
    TResult InvokeIfRequired<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function, T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// Evaluates a function synchronously on the message loop thread.
    /// </summary>
    /// <typeparam name="T1">The type of the first function argument.</typeparam>
    /// <typeparam name="T2">The type of the second function argument.</typeparam>
    /// <typeparam name="T3">The type of the third function argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth function argument.</typeparam>
    /// <typeparam name="TResult">The type of the function result.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="arg1">The first function argument.</param>
    /// <param name="arg2">The second function argument.</param>
    /// <param name="arg3">The third function argument.</param>
    /// <param name="arg4">The fourth function argument.</param>
    /// <returns>The result of the function evaluation.</returns>
    TResult InvokeIfRequired<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    /// <summary>
    /// Evaluates a function synchronously on the message loop thread.
    /// </summary>
    /// <typeparam name="T1">The type of the first function argument.</typeparam>
    /// <typeparam name="T2">The type of the second function argument.</typeparam>
    /// <typeparam name="T3">The type of the third function argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth function argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth function argument.</typeparam>
    /// <typeparam name="TResult">The type of the function result.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="arg1">The first function argument.</param>
    /// <param name="arg2">The second function argument.</param>
    /// <param name="arg3">The third function argument.</param>
    /// <param name="arg4">The fourth function argument.</param>
    /// <param name="arg5">The fifth function argument.</param>
    /// <returns>The result of the function evaluation.</returns>
    TResult InvokeIfRequired<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    /// <summary>
    /// Evaluates a function synchronously on the message loop thread.
    /// </summary>
    /// <typeparam name="T1">The type of the first function argument.</typeparam>
    /// <typeparam name="T2">The type of the second function argument.</typeparam>
    /// <typeparam name="T3">The type of the third function argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth function argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth function argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth function argument.</typeparam>
    /// <typeparam name="TResult">The type of the function result.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="arg1">The first function argument.</param>
    /// <param name="arg2">The second function argument.</param>
    /// <param name="arg3">The third function argument.</param>
    /// <param name="arg4">The fourth function argument.</param>
    /// <param name="arg5">The fifth function argument.</param>
    /// <param name="arg6">The sixth function argument.</param>
    /// <returns>The result of the function evaluation.</returns>
    TResult InvokeIfRequired<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    /// <summary>
    /// Evaluates a function synchronously on the message loop thread.
    /// </summary>
    /// <typeparam name="T1">The type of the first function argument.</typeparam>
    /// <typeparam name="T2">The type of the second function argument.</typeparam>
    /// <typeparam name="T3">The type of the third function argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth function argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth function argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth function argument.</typeparam>
    /// <typeparam name="T7">The type of the seventh function argument.</typeparam>
    /// <typeparam name="TResult">The type of the function result.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="arg1">The first function argument.</param>
    /// <param name="arg2">The second function argument.</param>
    /// <param name="arg3">The third function argument.</param>
    /// <param name="arg4">The fourth function argument.</param>
    /// <param name="arg5">The fifth function argument.</param>
    /// <param name="arg6">The sixth function argument.</param>
    /// <param name="arg7">The seventh function argument.</param>
    /// <returns>The result of the function evaluation.</returns>
    TResult InvokeIfRequired<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

    #endregion InvokeIfRequired (Functions)

    #region InvokeAndForget

    /// <summary>
    /// Invokes the specified callback on the message loop.
    /// </summary>
    /// <remarks>
    /// This method does not wait for the callback complete execution.
    /// If this method is used on the main thread; the message loop must be pumped before the callback gets to run.
    /// If the callback causes an exception, it is left unhandled.
    /// </remarks>
    /// <param name="callback">The callback to execute.</param>
    void InvokeAndForget(Action callback);

    /// <summary>
    /// Executes an action asynchronously on the message loop thread without waiting.
    /// </summary>
    /// <typeparam name="T">The type of the action argument.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg">The argument of the action.</param>
    void InvokeAndForget<T>(Action<T> action, T arg);

    /// <summary>
    /// Executes an action asynchronously on the message loop thread without waiting.
    /// </summary>
    /// <typeparam name="T1">The type of the first action argument.</typeparam>
    /// <typeparam name="T2">The type of the second action argument.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The first argument of the action.</param>
    /// <param name="arg2">The second argument of the action.</param>
    void InvokeAndForget<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2);

    /// <summary>
    /// Executes an action asynchronously on the message loop thread without waiting.
    /// </summary>
    /// <typeparam name="T1">The type of the first action argument.</typeparam>
    /// <typeparam name="T2">The type of the second action argument.</typeparam>
    /// <typeparam name="T3">The type of the third action argument.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The 1st argument of the action.</param>
    /// <param name="arg2">The 2nd argument of the action.</param>
    /// <param name="arg3">The 3rd argument of the action.</param>
    void InvokeAndForget<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// Executes an action asynchronously on the message loop thread without waiting.
    /// </summary>
    /// <typeparam name="T1">The type of the first action argument.</typeparam>
    /// <typeparam name="T2">The type of the second action argument.</typeparam>
    /// <typeparam name="T3">The type of the third action argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth action argument.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The first argument of the action.</param>
    /// <param name="arg2">The second argument of the action.</param>
    /// <param name="arg3">The 3rd argument of the action.</param>
    /// <param name="arg4">The 4th argument of the action.</param>
    void InvokeAndForget<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    /// <summary>
    /// Executes an action asynchronously on the message loop thread without waiting.
    /// </summary>
    /// <typeparam name="T1">The type of the first action argument.</typeparam>
    /// <typeparam name="T2">The type of the second action argument.</typeparam>
    /// <typeparam name="T3">The type of the third action argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth action argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth action argument.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="arg1">The first argument of the action.</param>
    /// <param name="arg2">The second argument of the action.</param>
    /// <param name="arg3">The 3rd argument of the action.</param>
    /// <param name="arg4">The 4th argument of the action.</param>
    /// <param name="arg5">The 5th argument of the action.</param>
    void InvokeAndForget<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    #endregion InvokeAndForget

    #region InvokeLaterAndForget (Actions)

    /// <summary>
    /// Waits <paramref name="delay"/>, then executes <paramref name="method"/> on the message loop thread.
    /// </summary>
    /// <param name="delay">The delay before executing the method.</param>
    /// <param name="method">The method to execute.</param>
    void InvokeLaterAndForget(TimeSpan delay, Action method);

    #endregion InvokeLaterAndForget (Actions)

    #region InvokeAsync

    /// <summary>
    /// Invokes the specified callback on the message loop.
    /// </summary>
    /// <param name="callback">The callback to execute.</param>
    /// <returns>Returns a task that is signalled when the execution of the callback is completed.</returns>
    /// <remarks>
    /// If the callback method causes an exception; the exception gets stored in the task object.
    /// </remarks>
    Task InvokeAsync(Action callback);

    /// <inheritdoc see="InvokeAsync(Action)"/>
    Task<T> InvokeAsync<T>(Func<T> callback);

    #endregion InvokeAsync
  }
}
