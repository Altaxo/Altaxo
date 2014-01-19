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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Com
{
	/// <summary>
	/// Helper class used to avoid deadlocks if the Gui thread should be invoked multiple times from nested cross-thread calls.
	/// </summary>
	public class GuiThreadStack
	{
		private class InvokeData
		{
			public Task @Task;
			public Action GuiAction;
			public System.Threading.AutoResetEvent @Event;
		}

		private ConcurrentStack<InvokeData> _invokerQueue = new ConcurrentStack<InvokeData>();

		private object _syncObject = new object();

		private Func<bool> _isInvokeRequiredForGuiThread;
		private Action<Action> _invokeGuiThread;

		public GuiThreadStack(Func<bool> IsInvokeRequiredForGuiThread, Action<Action> InvokeGuiThread)
		{
			if (null == IsInvokeRequiredForGuiThread)
				throw new ArgumentNullException("IsInvokeRequiredForGuiThread is null, but it is mandatory.");
			if (null == InvokeGuiThread)
				throw new ArgumentNullException("InvokeGuiThread is null, but it is mandatory.");

			_isInvokeRequiredForGuiThread = IsInvokeRequiredForGuiThread;
			_invokeGuiThread = InvokeGuiThread;
		}

		/// <summary>
		/// Invokes the GUI thread (blocking). The special feature is here that there is no deadlock even if another action executed in the Gui tasks waits for completion, supposed
		/// that it has used the <see cref="FromGuiThreadExecute"/> method.
		/// </summary>
		/// <param name="action">The action to be executed in the Gui thread.</param>
		public void InvokeGuiThread(Action action)
		{
			if (!_isInvokeRequiredForGuiThread())
			{
				// no invoke required because we are already in the Gui thread
				action();
				return;
			}

			InvokeData data;
			System.Threading.AutoResetEvent myEvent = null;
			lock (_syncObject)
			{
				if (_invokerQueue.TryPeek(out data))
				{
					lock (data)
					{
						data.GuiAction = action;
						data.Event = (myEvent = new System.Threading.AutoResetEvent(false));
					}
				}
			}

			if (null != myEvent) // we pushed the action to the stack, thus we now must wait for the event to be set after the action is executed
			{
				myEvent.WaitOne();
			}
			else // TryPeek has returned no data, this we invoke directly
			{
				_invokeGuiThread(action);
			}
		}

		/// <summary>
		/// Executes an action <paramref name="action"/> in a separate task (blocking). If the action (or a child task) wants to invoke the Gui thread, this is possible without deadlock.
		/// This procedure must be called only from the Gui thread. The procedure returns when the action has been finished.
		/// </summary>
		/// <param name="action">The action. Please not that the action is executed in a thread other than the Gui thread.</param>
		/// <exception cref="System.InvalidOperationException">Is thrown if this procedure is called from a thread that is not the Gui thread.</exception>
		public void FromGuiThreadExecute(Action action)
		{
			// we assume that this is the Gui thread
			if (_isInvokeRequiredForGuiThread())
				throw new InvalidOperationException("This procedure must be called from the Gui thread only.");

			InvokeData data = new InvokeData() { Task = new Task(action) };
			_invokerQueue.Push(data);
			data.Task.Start();

			Action guiAction;
			System.Threading.AutoResetEvent myEvent;
			for (; ; )
			{
				lock (data)
				{
					guiAction = data.GuiAction;
					myEvent = data.Event;
					data.GuiAction = null;
					data.Event = null;
				}
				if (null != guiAction)
				{
					guiAction();
					myEvent.Set();
				}
				if (data.Task.Wait(1))
					break;
			}

			guiAction = null;
			lock (_syncObject)
			{
				if (_invokerQueue.TryPop(out data))
				{
					lock (data)
					{
						guiAction = data.GuiAction;
						myEvent = data.Event;
						data.GuiAction = null;
						data.Event = null;
					}
				}
			}
			if (null != guiAction)
			{
				guiAction();
				myEvent.Set();
			}

			if (null != data.Task.Exception)
				throw data.Task.Exception;
		}
	}
}