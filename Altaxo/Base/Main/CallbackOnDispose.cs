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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Main
{
  /// <summary>
  /// Invokes an action when it is disposed.
  /// </summary>
  /// <remarks>
  /// This class ensures the callback is invoked at most once,
  /// even when Dispose is called on multiple threads.
  /// </remarks>
  public sealed class CallbackOnDispose : IDisposable
  {
    private Action _action;

    /// <summary>
    /// Initializes a new instance of the <see cref="CallbackOnDispose"/> class.
    /// </summary>
    /// <param name="action">The action to invoke when this instance is disposed.</param>
    public CallbackOnDispose(Action action)
    {
      _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      Interlocked.Exchange(ref _action, null!)?.Invoke();
    }

  }

  /// <summary>
  /// This class is used to prevent stack overflows by representing a busy flag
  /// that prevents reentrance when another call is running.
  /// However, using a simple <c>bool busy</c> is not thread-safe, so we use a
  /// thread-static BusyManager.
  /// </summary>
  internal static class BusyManager
  {
    /// <summary>
    /// Represents a disposable busy-state token.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible",
      Justification = "Should always be used with 'var'")]
    public struct BusyLock : IDisposable
    {
      /// <summary>
      /// Gets a failed busy lock.
      /// </summary>
      public static readonly BusyLock Failed = new BusyLock(null);

      private readonly List<object>? _objectList;

      /// <summary>
      /// Initializes a new instance of the <see cref="BusyLock"/> struct.
      /// </summary>
      /// <param name="objectList">The stack of active objects, or <see langword="null"/> for a failed lock.</param>
      internal BusyLock(List<object>? objectList)
      {
        this._objectList = objectList;
      }

      /// <summary>
      /// Gets a value indicating whether the lock was acquired successfully.
      /// </summary>
      public bool Success
      {
        get { return _objectList is not null; }
      }

      /// <summary>
      /// Releases the busy lock.
      /// </summary>
      public void Dispose()
      {
        if (_objectList is not null)
        {
          _objectList.RemoveAt(_objectList.Count - 1);
        }
      }
    }

    [ThreadStatic] private static List<object>? _activeObjects;

    /// <summary>
    /// Enters the busy state for the specified object.
    /// </summary>
    /// <param name="obj">The object to mark as busy.</param>
    /// <returns>A lock token that releases the busy state when disposed.</returns>
    public static BusyLock Enter(object obj)
    {
      var activeObjects = _activeObjects;
      if (activeObjects is null)
        activeObjects = _activeObjects = new List<object>();
      for (int i = 0; i < activeObjects.Count; i++)
      {
        if (activeObjects[i] == obj)
          return BusyLock.Failed;
      }
      activeObjects.Add(obj);
      return new BusyLock(activeObjects);
    }
  }
}
