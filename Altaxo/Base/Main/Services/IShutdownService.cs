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
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Service that is responsible for managing the IDE shutdown. Furthermore it can intercept the shutdown if shutdown is not currently possible.
  /// </summary>
  public interface IShutdownService
  {
    /// <summary>
    /// Gets a value indicating whether the application is closing.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the application closing; otherwise, <c>false</c>.
    /// </value>
    bool IsApplicationClosing { get; }

    /// <summary>
    /// Attemps to close the IDE.
    /// </summary>
    /// <remarks>
    /// <para>This method:</para>
    /// <list type="items">
    /// <item>Checks if <see cref="PreventShutdown"/> was called and abort the shutdown if it was.</item>
    /// <item>Prompts the user to save the open files. The user has the option to cancel the shutdown at that point.</item>
    /// <item>Closes the solution.</item>
    /// <item>Signals the <see cref="ShutdownToken"/>.</item>
    /// <item>Disposes pads</item>
    /// <item>Waits for background tasks (<see cref="AddBackgroundTask"/>) to finish.</item>
    /// <item>Disposes services</item>
    /// <item>Saves the PropertyService</item>
    /// </list>
    /// <para>
    /// This method must be called on the main thread.
    /// </para>
    /// </remarks>
    bool Shutdown();

    /// <summary>
    /// Occurs when the project was already closed and the rest of the shutdown procedure starts to happen. This should be implemented
    /// as weak event in order to prevent garbage collection of item that subscribe to this event.
    /// </summary>
    event EventHandler Closed;

    /// <summary>
    /// Prevents shutdown with the following reason.
    /// Dispose the returned value to allow shutdown again.
    /// </summary>
    /// <param name="reason">The reason, will be displayed to the user.</param>
    /// <exception cref="InvalidOperationException">Shutdown is already in progress</exception>
    /// <remarks>This method is thread-safe.</remarks>
    IDisposable PreventShutdown(string reason);

    /// <summary>
    /// Gets the current reason that prevents shutdown.
    /// If there isn't any reason, returns null.
    /// If there are multiple reasons, only one of them is returned.
    /// </summary>
    /// <remarks>This property is thread-safe.</remarks>
    string CurrentReasonPreventingShutdown { get; }

    /// <summary>
    /// Gets a cancellation token that gets signalled when the application is shutting down.
    ///
    /// This cancellation token may be used to stop background calculations.
    /// </summary>
    /// <remarks>This property is thread-safe.</remarks>
    CancellationToken ShutdownToken { get; }

    void SignalShutdownToken();

    /// <summary>
    /// Gets a cancellation token that gets signalled a couple of seconds after the ShutdownToken.
    ///
    /// This cancellation token may be used to stop background calculations that should run
    /// for a limited time after the application is closed (e.g. saving state in caches
    /// - work that should better run even though we're shutting down, but shouldn't take too long either)
    /// </summary>
    /// <remarks>This property is thread-safe.</remarks>
    CancellationToken DelayedShutdownToken { get; }

    /// <summary>
    /// Adds a background task on which the application should wait on shutdown.
    ///
    /// Use this method for tasks that asynchronously write state to disk and should not be
    /// interrupted by the applicaiton closing down.
    /// </summary>
    /// <remarks>This method is thread-safe.</remarks>
    void AddBackgroundTask(Task task);

    /// <summary>
    /// Execute the closing procedure, and maybe prevent the workbench from closing by setting Cancel to true in the event args.
    /// </summary>
    /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
    void OnClosing(System.ComponentModel.CancelEventArgs e);
  }
}
