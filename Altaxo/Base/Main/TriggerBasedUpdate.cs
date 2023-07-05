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
using System.Threading;
using Altaxo.Main.Services;
using Altaxo.Threading;

namespace Altaxo.Main
{
  /// <summary>
  /// Manages trigger based update operations. See remarks for details.
  /// </summary>
  /// <remarks>
  /// An update is triggered by calling the <see cref="Trigger"/> function. The update is not executed immediately. Instead, depending on five timing parameters set in this instance,
  /// it is delayed by a certain amount of time.
  /// <para>
  /// Five parameters control the behaviour of trigger base update operations:
  /// </para>
  /// <list type="list">
  /// <item>MinimumWaitingTimeAfterFirstTrigger (default: Zero): designates the minimum time interval that should at least be waited after the first trigger (after an update) and the next update operation.</item>
  /// <item>MaximumWaitingTimeAfterFirstTrigger (default: Infinity): designates the maximum waiting time after the first trigger (after an update) occured. If this time has elapsed, a new update operation will be executed.</item>
  /// <item>MinimumWaitingTimeAfterLastTrigger (default: Zero): designates the time interval that at least should be waited between the latest occured trigger (after an update) and the next update operation.</item>
  /// <item>MinimumWaitingTimeAfterUpdate (default: Zero) designates the minimum time that must be waited after an update operation was executed. This parameter has the highest priority.</item>
  /// <item>MaximumWaitingTimeAfterUpdate (default: Infinity): designates the maximum time that must be waited after an update operation was executed. Normally infinity. If set to another value, it can be used to 'poll' the update operation.</item>
  /// </list>
  /// </remarks>
  public class TriggerBasedUpdate : DisposableBase
  {
    private TimeSpan _minimumWaitingTimeAfterFirstTrigger = TimeSpan.Zero;
    private TimeSpan _minimumWaitingTimeAfterLastTrigger = TimeSpan.Zero;
    private TimeSpan _maximumWaitingTimeAfterFirstTrigger = TimeSpan.MaxValue;
    private TimeSpan _minimumWaitingTimeAfterUpdate = TimeSpan.Zero;
    private TimeSpan _maximumWaitingTimeAfterUpdate = TimeSpan.MaxValue;

    Interlockable<(TimeSpan TimeOfFirstTrigger, TimeSpan TimeOfLastTrigger)?> _triggerTimes = new(null);

    private TimeSpan _timeOfLastDueTime = TimeSpan.MinValue;
    private TimeSpan _timeOfNextDueTime;

    public Action? _updateAction;

    /// <summary>Reference to the timer queue.</summary>
    private readonly ITimerQueue _timerQueue;

    /// <summary>Token used to identify the items in the timer queue</summary>
    private readonly object _timerToken = new object();

    /// <summary>Lock used to synchronize access to the update function and to the recalculation of the next due time.</summary>
    private readonly ReaderWriterLockSlim _dueTimeAndQueueLock = new ReaderWriterLockSlim();

    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="TriggerBasedUpdate"/> class. After creating this instance, set some of the timing parameters, and then subscribe to the <see cref="UpdateAction"/> event. This will activate the instance.
    /// </summary>
    /// <param name="queue">The underlying queue (in most cases you should use <see cref="Current.TimerQueue"/>.</param>
    /// <exception cref="System.ArgumentNullException">queue</exception>
    public TriggerBasedUpdate(ITimerQueue queue)
    {
      _timerQueue = queue ?? throw new ArgumentNullException(nameof(queue));
    }

    #region Properties

    public event Action UpdateAction
    {
      add
      {
        if (_isDisposed)
          throw new ObjectDisposedException(GetType().ToString());

        var oldValue = _updateAction;
        _updateAction += value;
        if (oldValue is null)
          EnsureUpdatedDueTime();
      }
      remove
      {
        _updateAction -= value;
      }
    }

    /// <summary>
    /// MinimumWaitingTimeAfterFirstTrigger (default: Zero): designates the minimum time interval that should at least be waited after the first trigger (after an update) and the next update operation.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">MinimumWaitingTimeAfterFirstTrigger must not be negative!</exception>
    public TimeSpan MinimumWaitingTimeAfterFirstTrigger
    {
      get
      {
        return _minimumWaitingTimeAfterFirstTrigger;
      }
      set
      {
        if (value < TimeSpan.Zero)
          throw new ArgumentOutOfRangeException("MinimumWaitingTimeAfterFirstTrigger must not be negative!");

        var oldValue = _minimumWaitingTimeAfterFirstTrigger;
        _minimumWaitingTimeAfterFirstTrigger = value;

        if (oldValue != value)
          EnsureUpdatedDueTime();
      }
    }

    /// <summary>
    /// MaximumWaitingTimeAfterFirstTrigger (default: Infinity): designates the maximum waiting time after the first trigger (after an update) occured. If this time has elapsed, a new update operation will be executed.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">MaximumWaitingTimeAfterFirstTrigger must not be negative!</exception>
    public TimeSpan MaximumWaitingTimeAfterFirstTrigger
    {
      get
      {
        return _maximumWaitingTimeAfterFirstTrigger;
      }
      set
      {
        if (value < TimeSpan.Zero)
          throw new ArgumentOutOfRangeException("MaximumWaitingTimeAfterFirstTrigger must not be negative!");

        var oldValue = _maximumWaitingTimeAfterFirstTrigger;
        _maximumWaitingTimeAfterFirstTrigger = value;

        if (oldValue != value)
          EnsureUpdatedDueTime();
      }
    }

    /// <summary>
    /// MinimumWaitingTimeAfterLastTrigger (default: Zero): designates the time interval that at least should be waited between the latest occured trigger (after an update) and the next update operation.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">MinimumWaitingTimeAfterLastTrigger must not be negative!</exception>
    public TimeSpan MinimumWaitingTimeAfterLastTrigger
    {
      get
      {
        return _minimumWaitingTimeAfterLastTrigger;
      }
      set
      {
        if (value < TimeSpan.Zero)
          throw new ArgumentOutOfRangeException("MinimumWaitingTimeAfterLastTrigger must not be negative!");

        var oldValue = _minimumWaitingTimeAfterLastTrigger;
        _minimumWaitingTimeAfterLastTrigger = value;

        if (oldValue != value)
          EnsureUpdatedDueTime();
      }
    }

    /// <summary>
    /// MinimumWaitingTimeAfterUpdate (default: Zero) designates the minimum time that must be waited after an update operation was executed. This parameter has the highest priority.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">MinimumWaitingTimeAfterUpdate must not be negative!</exception>
    public TimeSpan MinimumWaitingTimeAfterUpdate
    {
      get
      {
        return _minimumWaitingTimeAfterUpdate;
      }
      set
      {
        if (value < TimeSpan.Zero)
          throw new ArgumentOutOfRangeException("MinimumWaitingTimeAfterUpdate must not be negative!");

        var oldValue = _minimumWaitingTimeAfterUpdate;
        _minimumWaitingTimeAfterUpdate = value;

        if (oldValue != value)
          EnsureUpdatedDueTime();
      }
    }

    /// <summary>
    /// MaximumWaitingTimeAfterUpdate (default: Infinity): designates the maximum time that must be waited after an update operation was executed. Normally infinity. If set to another value, it can be used to 'poll' the update operation.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">MinimumWaitingTimeAfterUpdate must not be negative!</exception>
    public TimeSpan MaximumWaitingTimeAfterUpdate
    {
      get
      {
        return _maximumWaitingTimeAfterUpdate;
      }
      set
      {
        if (value < TimeSpan.Zero)
          throw new ArgumentOutOfRangeException("MinimumWaitingTimeAfterUpdate must not be negative!");

        var oldValue = _maximumWaitingTimeAfterUpdate;
        _maximumWaitingTimeAfterUpdate = value;

        if (oldValue != value)
          EnsureUpdatedDueTime();
      }
    }

    #endregion Properties

    /// <summary>
    /// Triggers an update operation. The time the update operation is executed after this trigger depends on the parameters set in this instance.
    /// </summary>
    public void Trigger()
    {
      if (_isDisposed)
        throw new ObjectDisposedException(GetType().ToString());

      var currTime = _timerQueue.CurrentTime;

      _triggerTimes.Modify(tt => tt.HasValue ? (tt.Value.TimeOfFirstTrigger, currTime) : (currTime, currTime));
      EnsureUpdatedDueTime();
    }

    private void EnsureUpdatedDueTime()
    {
      if (_updateAction is not null)
      {
        var triggerTimes = _triggerTimes.Value;
        if (_dueTimeAndQueueLock.TryEnterWriteLock(0)) // calculate a new due time only if no update is currently in progress. If update is in progress, the timer queue is updated at the end of the update anyway
        {
          try
          {
            InternalCalculateDueTimeAndUpdateTimerQueueNoLock(triggerTimes);
          }
          finally
          {
            _dueTimeAndQueueLock.ExitWriteLock();
          }
        }
      }
    }

    /// <summary>
    /// Called by the timer queue if our timer has elapsed.
    /// </summary>
    protected virtual void EhTimerElapsed(object timerQueueToken, TimeSpan dueTime)
    {
      if (_isDisposed && _timerQueue is { } tq1)
      {
        tq1.TryRemove(_timerToken);
        return;
      }

      var updateAction = _updateAction;
      if (updateAction is null)
        return;

      Exception? exception = null;

      // Set the write lock to:
      // 1). Avoid execution of the action more than once at a time
      // 2.) Triggers coming during execution can update the trigger time, but should not update the due times, since
      // that is done at the end of this sequence
      _dueTimeAndQueueLock.EnterWriteLock();
      try
      {
        _triggerTimes.Value = null;
        try
        {
          updateAction(); // Do the update
        }
        catch (Exception ex)
        {
          exception = ex;
        }

        // ------ Trigger events below this line must certainly trigger a due time update ----------------------------

        _timeOfLastDueTime = _timerQueue.CurrentTime;
        for (; !_isDisposed;)
        {
          var triggerTimes = _triggerTimes.Value;
          // note: if a trigger comes exactly in this moment, it is lost, because it will not cause an update to the timer queue due to the lock
          InternalCalculateDueTimeAndUpdateTimerQueueNoLock(triggerTimes);
          if (triggerTimes == _triggerTimes.Value) // if the trigger times have changed meanwhile, then calculate again
            break;
        }

        if (_isDisposed && _timerQueue is { } tq2)
        {
          tq2.TryRemove(_timerToken);
        }
      }
      finally
      {
        _dueTimeAndQueueLock.ExitWriteLock();
      }

      if (exception is not null)
        throw new Exception("Exception during execution of the trigger based update action", exception);
    }

    /// <summary>
    /// Calculates the due time of the next update operation.
    /// </summary>
    /// <returns>Due time of the next update operation. </returns>
    protected TimeSpan InternalGetDueTimeNoLock((TimeSpan TimeOfFirstTrigger, TimeSpan TimeOfLastTrigger)? triggerTimes)
    {
      TimeSpan minDueTime = TimeSpan.MinValue;
      TimeSpan maxDueTime = TimeSpan.MaxValue;
      TimeSpan dueTime;
      if (triggerTimes.HasValue)
      {
        dueTime = triggerTimes.Value.TimeOfLastTrigger + _minimumWaitingTimeAfterLastTrigger;
        minDueTime = Max(minDueTime, dueTime);

        dueTime = triggerTimes.Value.TimeOfFirstTrigger + _minimumWaitingTimeAfterFirstTrigger;
        minDueTime = Max(minDueTime, dueTime);

        if (_maximumWaitingTimeAfterFirstTrigger < TimeSpan.MaxValue)
        {
          dueTime = triggerTimes.Value.TimeOfFirstTrigger + _maximumWaitingTimeAfterFirstTrigger;
          maxDueTime = Min(maxDueTime, dueTime);
        }
      }

      dueTime = _timeOfLastDueTime + _minimumWaitingTimeAfterUpdate;
      minDueTime = Max(minDueTime, dueTime);

      if (_maximumWaitingTimeAfterUpdate < TimeSpan.MaxValue)
      {
        dueTime = _timeOfLastDueTime + _maximumWaitingTimeAfterUpdate;
        maxDueTime = Min(maxDueTime, dueTime);
      }

      if (triggerTimes.HasValue)
      {
        // when notified, the maximum due time always wins over the minimum waiting time
        dueTime = Min(minDueTime, maxDueTime);
        return dueTime;
      }
      else
      {
        // when not notified, only the max time counts
        return maxDueTime;
      }
    }

    /// <summary>
    /// Calculates the due time of the next update operation,
    /// and updates the timer queue with the new value of the due time.
    /// </summary>
    private void InternalCalculateDueTimeAndUpdateTimerQueueNoLock((TimeSpan TimeOfFirstTrigger, TimeSpan TimeOfLastTrigger)? triggerTimes)
    {
      var oldDueTime = _timeOfNextDueTime;
      _timeOfNextDueTime = InternalGetDueTimeNoLock(triggerTimes);

      if (oldDueTime != _timeOfNextDueTime && _timeOfNextDueTime != TimeSpan.MaxValue)
      {
        _timerQueue.AddOrUpdate(_timerToken, _timeOfNextDueTime, EhTimerElapsed);
      }
    }

    private static TimeSpan Max(TimeSpan x, TimeSpan y)
    {
      return x > y ? x : y;
    }

    private static TimeSpan Min(TimeSpan x, TimeSpan y)
    {
      return x < y ? x : y;
    }

    protected override void Dispose(bool disposing)
    {
      if (!_isDisposed)
      {
        _updateAction = null;
        _isDisposed = true;

        // In order to avoid dead locks when disposed is called from the Gui thread,
        // we try to remove the token without lock
        // should it be added again in the fill thread, it is removed again at the end of this thread
        _timerQueue.TryRemove(_timerToken);
        _dueTimeAndQueueLock.Dispose();
      }
    }
  }
}
