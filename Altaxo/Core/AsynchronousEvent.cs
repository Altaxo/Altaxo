#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Altaxo
{
  /// <summary>
  /// Asynchronous event with no arguments. The event handlers can be executed either parallel or serial.
  /// All methods of this class are thread safe.
  /// </summary>
  public class AsynchronousEvent
  {
    object _locker = new();
    HashSet<Func<CancellationToken, Task>> _list = new();

    /// <summary>
    /// Subscribes the eventHandler to the event.
    /// </summary>
    /// <param name="eventHandler">The event handler.</param>
    public void Subscribe(Func<CancellationToken, Task> eventHandler)
    {
      if (eventHandler is not null)
      {
        lock (_locker)
        {
          _list.Add(eventHandler);
        }
      }
    }

    /// <summary>
    /// Unsubscribes the eventHandler from the event.
    /// </summary>
    /// <param name="eventHandler">The event handler.</param>
    /// <returns>
    /// True if the event was removed; otherwise, false.
    /// </returns>
    public bool Unsubscribe(Func<CancellationToken, Task> eventHandler)
    {
      if (eventHandler is not null)
      {
        lock (_locker)
        {
          return _list.Remove(eventHandler);
        }
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Invokes all event handlers in parallel.
    /// </summary>
    /// <returns>A task that can be awaited for.</returns>
    public Task InvokeParallel(CancellationToken token)
    {
      Func<CancellationToken, Task>[] array;
      lock (_locker)
      {
        array = _list.ToArray();
      }
      if (array.Length == 0)
      {
        return Task.CompletedTask;
      }

      var tasks = new ConcurrentBag<Task>();
      Parallel.ForEach(array, (f) => tasks.Add(f.Invoke(token)));
      return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Invokes the event handlers in serial order. If one or more event handlers fail, the other event handlers are executed nevertheless.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <exception cref="System.AggregateException"></exception>
    public async Task InvokeSerial(CancellationToken token)
    {
      Func<CancellationToken, Task>[] array;
      lock (_locker)
      {
        array = _list.ToArray();
      }

      if (array.Length ==0)
      {
        return;
      }
      
        // Make sure that if one or more handler are faulty, the other handlers are executed nevertheless
        var exceptionList = new List<Exception>();
        for (int i = 0; i < array.Length; ++i)
        {
          token.ThrowIfCancellationRequested();
          try
          {
            await array[i](token).ConfigureAwait(false);
          }
          catch (TaskCanceledException)
          {
            throw;
          }
          catch (Exception ex)
          {
            exceptionList.Add(ex);
          }
        }
        if (exceptionList.Count == 1)
          throw exceptionList[0];
        else if (exceptionList.Count > 1)
          throw new AggregateException(exceptionList);
      
    }
  }
}
