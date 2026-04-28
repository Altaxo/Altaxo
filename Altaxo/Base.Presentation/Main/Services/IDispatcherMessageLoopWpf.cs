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
  /// <summary>
  /// Represents a thread running a message loop.
  /// </summary>
  [GlobalService("MainThread", FallbackImplementation = typeof(FakeMessageLoop))]
  public interface IDispatcherMessageLoopWpf : IDispatcherMessageLoop
  {
    /// <summary>
    /// Gets the dispatcher for this message loop.
    /// </summary>
    Dispatcher Dispatcher { get; }

    #region InvokeIfRequired (Actions)

    /// <summary>
    /// Invokes an action on the dispatcher with the specified priority.
    /// </summary>
    /// <param name="callback">The action to execute.</param>
    /// <param name="priority">The dispatcher priority.</param>
    void InvokeIfRequired(Action callback, DispatcherPriority priority);

    /// <summary>
    /// Invokes an action on the dispatcher with the specified priority.
    /// </summary>
    /// <param name="callback">The action to execute.</param>
    /// <param name="priority">The dispatcher priority.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    void InvokeIfRequired(Action callback, DispatcherPriority priority, CancellationToken cancellationToken);

    #endregion InvokeIfRequired (Actions)

    #region InvokeIfRequired (Functions)

    /// <summary>
    /// Invokes a function on the dispatcher with the specified priority.
    /// </summary>
    /// <typeparam name="T">The callback return type.</typeparam>
    /// <param name="callback">The function to execute.</param>
    /// <param name="priority">The dispatcher priority.</param>
    /// <returns>The function result.</returns>
    T InvokeIfRequired<T>(Func<T> callback, DispatcherPriority priority);

    /// <summary>
    /// Invokes a function on the dispatcher with the specified priority.
    /// </summary>
    /// <typeparam name="T">The callback return type.</typeparam>
    /// <param name="callback">The function to execute.</param>
    /// <param name="priority">The dispatcher priority.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The function result.</returns>
    T InvokeIfRequired<T>(Func<T> callback, DispatcherPriority priority, CancellationToken cancellationToken);

    #endregion InvokeIfRequired (Functions)

    #region InvokeAndForget (Actions)

    /// <summary>
    /// Invokes an action on the dispatcher without waiting.
    /// </summary>
    /// <param name="callback">The action to execute.</param>
    /// <param name="priority">The dispatcher priority.</param>
    void InvokeAndForget(Action callback, DispatcherPriority priority);

    #endregion InvokeAndForget (Actions)

    #region InvokeAsync

    /// <summary>
    /// Invokes an action asynchronously with the specified priority.
    /// </summary>
    /// <param name="callback">The action to execute.</param>
    /// <param name="priority">The dispatcher priority.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InvokeAsync(Action callback, DispatcherPriority priority);

    /// <summary>
    /// Invokes an action asynchronously with the specified priority.
    /// </summary>
    /// <param name="callback">The action to execute.</param>
    /// <param name="priority">The dispatcher priority.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InvokeAsync(Action callback, DispatcherPriority priority, CancellationToken cancellationToken);

    /// <summary>
    /// Invokes a function asynchronously with the specified priority.
    /// </summary>
    /// <typeparam name="T">The callback return type.</typeparam>
    /// <param name="callback">The function to execute.</param>
    /// <param name="priority">The dispatcher priority.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<T> InvokeAsync<T>(Func<T> callback, DispatcherPriority priority);

    /// <summary>
    /// Invokes a function asynchronously with the specified priority.
    /// </summary>
    /// <typeparam name="T">The callback return type.</typeparam>
    /// <param name="callback">The function to execute.</param>
    /// <param name="priority">The dispatcher priority.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<T> InvokeAsync<T>(Func<T> callback, DispatcherPriority priority, CancellationToken cancellationToken);

    #endregion InvokeAsync
  }
}
