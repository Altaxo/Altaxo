using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace Altaxo.Com
{
	/// <summary>
	/// Thread that can be invoked, i.e. code can be executed using <see cref="Invoke"/> or <see cref="BeginInvoke"/> always from this thread. This is especially important
	/// for objects which are thread sensitive. These objects must be created and it's functions must be called always from the same thread.
	/// </summary>
	public class InvokeableThread
	{
		private class InvokeData
		{
			public Action @Action;
			public System.Threading.AutoResetEvent @Event;
			public Exception @Exception;
		}

		// this is the time after which the thread is looped around anyway (if something goes wrong, for instance if two _triggerEvent.Set calls are too close)
		// set this value to -1 if you trust the code
		private const int _safetyIntervalTime_msec = -1;

		private volatile bool _keepThreadRunning;

		private Thread _thread;

		private System.Threading.AutoResetEvent _triggeringEvent;

		/// <summary>
		/// The dispatcher for re-throwing exceptions. Normally, you should use the dispatcher of the MainWindow.
		/// </summary>
		private System.Windows.Threading.Dispatcher _dispatcherForReThrowingExceptions;

		private ConcurrentQueue<InvokeData> _invokerQueue = new ConcurrentQueue<InvokeData>();

		/// <summary>
		/// Initializes a new instance of the <see cref="InvokeableThread"/> class.
		/// </summary>
		/// <param name="name">The name of the thread to create.</param>
		/// <param name="dispatcherForReThrowingExceptions">The dispatcher for re throwing exceptions. Normally, you should use the dispatcher of the MainWindow.</param>
		public InvokeableThread(string name, System.Windows.Threading.Dispatcher dispatcherForReThrowingExceptions)
			: this(name, System.Threading.ThreadPriority.Normal, dispatcherForReThrowingExceptions)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvokeableThread"/> class.
		/// </summary>
		/// <param name="name">The name of the thread to create.</param>
		/// <param name="priority">The thread priority this thread should be executed with.</param>
		/// <param name="dispatcherForReThrowingExceptions">The dispatcher for re throwing exceptions. Normally, you should use the dispatcher of the MainWindow.</param>
		public InvokeableThread(string name, System.Threading.ThreadPriority priority, System.Windows.Threading.Dispatcher dispatcherForReThrowingExceptions)
		{
			_dispatcherForReThrowingExceptions = dispatcherForReThrowingExceptions;
			Start(name, priority);
		}

		~InvokeableThread()
		{
			// Finalizer calls Dispose(false)
			Dispose(false);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && null != _thread)
			{
				Stop();

				if (null != _triggeringEvent)
					_triggeringEvent.Dispose();

				_triggeringEvent = null;
				_thread = null;
			}
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources. If the thread is still running, it is stopped.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Starts the invokable thread with a specified priority. The <paramref name="name"/> argument is used for debugging purposes, it is assigned to the thread.
		/// </summary>
		/// <param name="name">The name of the thread (as it then appears in TaskManager).</param>
		/// <param name="priority">The priority of the thread.</param>
		protected void Start(string name, System.Threading.ThreadPriority priority)
		{
			_keepThreadRunning = true;
			_triggeringEvent = new AutoResetEvent(false);
			_thread = new Thread(EhThreadBody);
			_thread.IsBackground = true;
			_thread.Priority = priority;
			_thread.Name = name;
			_thread.Start();
		}

		/// <summary>
		/// Stops this instance, and disposes the underlying thread instance.
		/// </summary>
		public void Stop()
		{
			_keepThreadRunning = false;
			_triggeringEvent.Set();
		}

		/// <summary>
		/// Executes the provided action synchronously. This means that this function returns only after the provided action was executed.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		public void Invoke(Action action)
		{
			var evnt = new System.Threading.AutoResetEvent(false);
			var invokeData = new InvokeData { @Action = action, @Event = evnt };
			_invokerQueue.Enqueue(invokeData);
			_triggeringEvent.Set();

			evnt.WaitOne();

			if (invokeData.Exception != null)
				throw invokeData.Exception;
		}

		/// <summary>
		/// Executes the provided action asynchronously. This means that this function immediately returns, without waiting for the action to be executed.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		public void InvokeAsync(Action action)
		{
			_invokerQueue.Enqueue(new InvokeData() { @Action = action, @Event = null });
			_triggeringEvent.Set();
		}

		private void ProcessInvokerQueue()
		{
			InvokeData invokeData;
			while (_invokerQueue.TryDequeue(out invokeData))
			{
				if (invokeData.Action != null)
				{
					try
					{
						invokeData.Action();
					}
					catch (Exception ex)
					{
						invokeData.Exception = ex;
					}
				}
				if (invokeData.Event != null)
				{
					invokeData.Event.Set();
				}
			}
		}

		private void EhThreadBody()
		{
			while (_keepThreadRunning)
			{
				try
				{
					ProcessInvokerQueue();
					_triggeringEvent.WaitOne(_safetyIntervalTime_msec);
				}
				catch (Exception ex)
				{
					// the exception must be re-thrown on the Gui thread in order to be handled
					_dispatcherForReThrowingExceptions.Invoke((Action<Exception>)ReThrow, ex);
				}
			}

			// Process the invoker queue one last time
			ProcessInvokerQueue();

			_triggeringEvent.Dispose();
			_triggeringEvent = null;
			_thread = null;
		}

		private void ReThrow(Exception ex)
		{
			throw ex;
		}
	}
}