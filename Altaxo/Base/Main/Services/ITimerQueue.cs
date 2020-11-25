#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Interface for a timer queue. Users of this queue can add items, update the due time of already enqueued items, or remove items. The actions that are triggered when the due time
  /// is reached are run asynchronously in a thread pool task.
  /// </summary>
  public interface ITimerQueue
  {
    /// <summary>
    /// Gets the current time of the underlying high resolution clock. This is a relative value, e.g. relative to the start time of the clock.
    /// </summary>
    /// <value>
    /// The current time value.
    /// </value>
    TimeSpan CurrentTime { get; }

    /// <summary>
    /// Determines whether the queue contains an item identified by the provided token.
    /// </summary>
    /// <param name="token">The token that identifies the item.</param>
    /// <returns><c>True</c> if the queue contains the item, otherwise <c>false</c></returns>
    bool Contains(object token);

    /// <summary>
    /// Adds (if not already present) or updates (if already present) the item identified by the provided <paramref name="token"/>.
    /// </summary>
    /// <param name="token">The token that identifies the item.</param>
    /// <param name="time">The due time.</param>
    /// <param name="value">The action to perform when the due time is reached. 1st parameter is the token, 2nd parameter is the due time.</param>
    /// <returns><c>True</c> if the item was added. <c>False</c> if the already present item was updated.</returns>
    bool AddOrUpdate(object token, TimeSpan time, Action<object, TimeSpan> value);

    /// <summary>
    /// Tries to adds the item identified by the provided <paramref name="token"/>.
    /// </summary>
    /// <param name="token">The token to identify the item.</param>
    /// <param name="time">The due time.</param>
    /// <param name="action">The action to perform when the due time is reached. 1st parameter is the token, 2nd parameter is the due time.</param>
    /// <returns></returns>
    bool TryAdd(object token, TimeSpan time, Action<object, TimeSpan> action);

    /// <summary>
    /// Tries to update the time value of an already enqueued item.
    /// </summary>
    /// <param name="token">The token to identify the item.</param>
    /// <param name="time">The new due time value.</param>
    /// <returns><c>True</c> if the item's due time could be changed. <c>False</c> if the item was not present in the queue.</returns>
    bool TryUpdateTime(object token, TimeSpan time);

    /// <summary>
    /// Tries to remove an item from the queue.
    /// </summary>
    /// <param name="token">The token to identify the item.</param>
    /// <param name="dueTime">If the removal was successfull, contains the due time value of the removed item.</param>
    /// <returns><c>True</c> if the item could be successfully removed. <c>False</c> if the item was not present in the queue.</returns>
    bool TryRemove(object token, out TimeSpan dueTime);

    /// <summary>
    /// Tries to remove an item from the queue.
    /// </summary>
    /// <param name="token">The token to identify the item.</param>
    /// <returns><c>True</c> if the item could be successfully removed. <c>False</c> if the item was not present in the queue.</returns>
    bool TryRemove(object token);
  }
}
