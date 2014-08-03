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

using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Main.Services
{
	/// <summary>
	/// Queue that allows to trigger actions at specific times. This class is thread safe. When enqueuing items, you will get back a token, which can be used to question or change the item on the queue.
	/// </summary>
	public class TimerQueue : DisposableBase, Altaxo.Main.Services.ITimerQueue
	{
		private ConcurrentTokenizedPriorityQueue<TimeSpan, Action<object, TimeSpan>, object> _items;
		private IHighResolutionClock _clock;
		private AutoResetEvent _event;
		private bool _isDisposed;
		private Task _task;

		/// <summary>
		/// Initializes a new instance of the <see cref="TimerQueue"/> class.
		/// </summary>
		/// <param name="clock">The underlying high resolution clock.</param>
		/// <exception cref="System.ArgumentNullException">Argument clock is null</exception>
		public TimerQueue(IHighResolutionClock clock)
		{
			if (null == clock)
				throw new ArgumentNullException("clock");

			_clock = clock;
			_event = new AutoResetEvent(false);
			_items = new ConcurrentTokenizedPriorityQueue<TimeSpan, Action<object, TimeSpan>, object>();
			_task = Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
		}

		/// <summary>
		/// Gets the time that is elapsed during this timer is running.
		/// </summary>
		/// <value>
		/// The elapsed time.
		/// </value>
		public TimeSpan CurrentTime
		{
			get
			{
				return _clock.CurrentTime;
			}
		}

		/// <summary>
		/// Determines whether the item identified by the token is still in the queue.
		/// </summary>
		/// <param name="token">The token to identify the item (return value of <see cref="Enqueue"/>).</param>
		/// <returns><c>True</c> if the item is still in the queue; <c>false</c> otherwise.</returns>
		public bool Contains(object token)
		{
			return _items.ContainsToken(token);
		}

		/// <summary>
		/// Enqueues an item in the timer queue.
		/// </summary>
		/// <param name="time">The time when to trigger the action.</param>
		/// <param name="action">The action. Can be a single or a multicast delegate. First argument is the token for the timer queue, second argument is the due time.</param>
		/// <param name="token"></param>
		public bool TryAdd(object token, TimeSpan time, Action<object, TimeSpan> action)
		{
			if (_items.TryAdd(token, time, action))
			{
				_event.Set();
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool AddOrUpdate(object token, TimeSpan time, Action<object, TimeSpan> action)
		{
			var result = _items.AddOrUpdate(token, time, action);
			_event.Set();
			return result;
		}

		/// <summary>
		/// Tries  to change the due time of an item already in the queue.
		/// </summary>
		/// <param name="token">The token that identifies the item (return value of <see cref="Enqueue"/>).</param>
		/// <param name="time">The new due time.</param>
		/// <returns><c>True</c> if the change was sucessfull; <c>false</c> if the item was not in the queue.</returns>
		public bool TryUpdateTime(object token, TimeSpan time)
		{
			if (_items.TryUpdateKey(token, time))
			{
				_event.Set();
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Tries the remove an item identified by a token from the queue.
		/// </summary>
		/// <param name="token">The token to identify the item (return value of <see cref="Enqueue"/>).</param>
		/// <param name="dueTime">On success,  contains the removed item.</param>
		/// <returns><c>True</c> if the item could be sucessfully removed; <c>false</c> if the item was not in the queue.</returns>
		public bool TryRemove(object token, out  TimeSpan dueTime)
		{
			return _items.TryRemove(token, out dueTime);
		}

		/// <summary>
		/// Tries the remove an item identified by a token from the queue.
		/// </summary>
		/// <param name="token">The token to identify the item (return value of <see cref="Enqueue"/>).</param>
		/// <returns><c>True</c> if the item could be sucessfully removed; <c>false</c> if the item was not in the queue.</returns>
		public bool TryRemove(object token)
		{
			return _items.TryRemove(token);
		}

		private void Run()
		{
			for (; !_isDisposed; )
			{
				_event.WaitOne(); // wait for an enqueue event

				TimeSpan nextTimeOnQueue;
				while (_items.TryPeekKey(out nextTimeOnQueue))
				{
					var currTime = _clock.CurrentTime;
					if (nextTimeOnQueue <= currTime)
					{
						object token;
						TimeSpan dueTime;
						Action<object, TimeSpan> action;
						if (_items.TryDequeue(out dueTime, out action, out token))
						{
							if (null != action)
								Task.Factory.StartNew(() => action(token, dueTime)); // Execute the action in a new task
						}
					}
					else
					{
						_event.WaitOne(nextTimeOnQueue - currTime); // wait for the elapsing of time
					}
				}
			}
		}

		#region IDisposable

		protected override void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				_isDisposed = true;

				if (disposing)
				{
					lock (this)
					{
						_items.Clear();
						_isDisposed = true;
						_event.Set();
						_task.Wait();
						_event.Dispose();
						_task.Dispose();
					}
				}
			}
		}

		#endregion IDisposable
	}
}