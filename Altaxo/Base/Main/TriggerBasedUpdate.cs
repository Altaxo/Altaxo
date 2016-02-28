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

using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Altaxo.Main
{
	internal class Interlockable<T>
	{
		private T _value;

		public Interlockable(T t)
		{
			_value = t;
		}

		public T Value
		{
			get
			{
				lock (this)
					return _value;
			}
			set
			{
				lock (this)
					_value = value;
			}
		}

		public T Exchange(T newValue)
		{
			lock (this)
			{
				var oldValue = _value;
				_value = newValue;
				return oldValue;
			}
		}

		public static implicit operator T(Interlockable<T> t)
		{
			return t.Value;
		}

		public static explicit operator Interlockable<T>(T t) // Explicite because otherwise unintended assignment of a new instance of Interlockable can happen
		{
			return new Interlockable<T>(t);
		}
	}

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

		private bool _isNotified;
		private TimeSpan _timeOfLastUpdate = TimeSpan.MinValue;
		private TimeSpan _timeOfFirstTrigger;
		private TimeSpan _timeOfLastTrigger;
		private TimeSpan _timeOfNextUpdate;

		public Action _updateAction;

		/// <summary>Reference to the timer queue.</summary>
		private readonly ITimerQueue _timerQueue;

		/// <summary>Token used to identify the items in the timer queue</summary>
		private readonly object _timerToken = new object();

		/// <summary>Lock used to synchronize access to the trigger times <see cref="_isNotified"/>, <see cref="_timeOfFirstTrigger"/>, <see cref="_timeOfLastTrigger"/></summary>
		private readonly object _triggerTimeChangeLock = new object();

		/// <summary>Lock used to synchronize access to the update function and to the recalculation of the next due time.</summary>
		private readonly ReaderWriterLockSlim _updateLock = new ReaderWriterLockSlim();

		private bool _isDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="TriggerBasedUpdate"/> class. After creating this instance, set some of the timing parameters, and then subscribe to the <see cref="UpdateAction"/> event. This will activate the instance.
		/// </summary>
		/// <param name="queue">The underlying queue (in most cases you should use <see cref="Current.TimerQueue"/>.</param>
		/// <exception cref="System.ArgumentNullException">queue</exception>
		public TriggerBasedUpdate(ITimerQueue queue)
		{
			if (null == queue)
				throw new ArgumentNullException("queue");

			_timerQueue = queue;
		}

		#region Properties

		public event Action UpdateAction
		{
			add
			{
				if (_isDisposed)
					throw new ObjectDisposedException(this.GetType().ToString());

				var oldValue = _updateAction;
				_updateAction += value;
				if (null == oldValue)
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
				throw new ObjectDisposedException(this.GetType().ToString());

			var currTime = _timerQueue.CurrentTime;

			lock (_triggerTimeChangeLock)
			{
				if (!_isNotified)
				{
					_isNotified = true;
					_timeOfFirstTrigger = currTime;
				}
				_timeOfLastTrigger = currTime;
			}

			EnsureUpdatedDueTime();
		}

		private void EnsureUpdatedDueTime()
		{
			if (null == _updateAction)
				return;

			if (_updateLock.TryEnterWriteLock(0)) // calculate a new due time only if no update is currently in progress. If update is in progress, the timer queue is updated at the end of the update anyway
			{
				InternalCalculateDueTimeAndUpdateTimerQueueNoLock();
				_updateLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Executes the update operation.
		/// </summary>
		protected virtual void OnUpdate(object timerQueueToken, TimeSpan dueTime)
		{
			var updateAction = _updateAction;
			if (null == updateAction)
				return;

			Exception exception = null;

			_updateLock.EnterWriteLock();

			lock (_triggerTimeChangeLock)
			{
				_isNotified = false;
			}
			// ------ Notification events below this line must certainly trigger a due time update ----------------------------

			try
			{
				updateAction(); // Do the update
			}
			catch (Exception ex)
			{
				exception = ex;
			}

			_timeOfLastUpdate = _timerQueue.CurrentTime;

			lock (_triggerTimeChangeLock) // ----- Notification time can not be changed below this line ---------------
			{
				InternalCalculateDueTimeAndUpdateTimerQueueNoLock();

				_updateLock.ExitWriteLock(); // Note that the UpdateLock must be released __before__ the NotificationLock. If the other way around, a notification taking place inbetween the release of NotificationLock and UpdateLock could cause TryEnter(UpdateLock) in (see function Notification) to fail, and the TimerQueue is not updated in this case
			} // ----- Notification time can be changed again -----------------

			if (null != exception)
				throw new Exception("Exception during execution of the trigger based update action", exception);
		}

		/// <summary>
		/// Calculates the due time of the next update operation.
		/// </summary>
		/// <returns>Due time of the next update operation. </returns>
		protected TimeSpan InternalGetDueTimeNoLock()
		{
			TimeSpan minDueTime = TimeSpan.MinValue;
			TimeSpan maxDueTime = TimeSpan.MaxValue;
			TimeSpan dueTime;
			if (_isNotified)
			{
				dueTime = _timeOfLastTrigger + _minimumWaitingTimeAfterLastTrigger;
				minDueTime = Max(minDueTime, dueTime);

				dueTime = _timeOfFirstTrigger + _minimumWaitingTimeAfterFirstTrigger;
				minDueTime = Max(minDueTime, dueTime);

				if (_maximumWaitingTimeAfterFirstTrigger < TimeSpan.MaxValue)
				{
					dueTime = _timeOfFirstTrigger + _maximumWaitingTimeAfterFirstTrigger;
					maxDueTime = Min(maxDueTime, dueTime);
				}
			}

			dueTime = _timeOfLastUpdate + _minimumWaitingTimeAfterUpdate;
			minDueTime = Max(minDueTime, dueTime);

			if (_maximumWaitingTimeAfterUpdate < TimeSpan.MaxValue)
			{
				dueTime = _timeOfLastUpdate + _maximumWaitingTimeAfterUpdate;
				maxDueTime = Min(maxDueTime, dueTime);
			}

			if (_isNotified)
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
		/// Calculates the due time of the next update operation, and updates the timer queue with the new value of the due time.
		/// </summary>
		private void InternalCalculateDueTimeAndUpdateTimerQueueNoLock()
		{
			var oldDueTime = _timeOfNextUpdate;
			_timeOfNextUpdate = InternalGetDueTimeNoLock();

			if (oldDueTime != _timeOfNextUpdate && _timeOfNextUpdate != TimeSpan.MaxValue)
			{
				_timerQueue.AddOrUpdate(_timerToken, _timeOfNextUpdate, OnUpdate);
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

				_updateLock.EnterWriteLock();
				_timerQueue.TryRemove(_timerToken);
				_updateLock.ExitWriteLock();

				_updateLock.Dispose();
			}
			_isDisposed = true;
		}
	}
}