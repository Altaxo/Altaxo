using System;
using System.Threading;

namespace Altaxo.Com
{
	/// <summary>
	/// Summary description for GarbageCollection.
	/// </summary>
	internal class GarbageCollector
	{
		protected bool _isThreadContinued;
		protected bool _isGCWatchStopped;
		protected int _interval_ms;
		protected ManualResetEvent _eventThreadEnded;

		public GarbageCollector(int interval_ms)
		{
			_isThreadContinued = true;
			_isGCWatchStopped = false;
			_interval_ms = interval_ms;
			_eventThreadEnded = new ManualResetEvent(false);
		}

		public void GCWatch()
		{
#if COMLOGGING
			Debug.ReportInfo("GarbageCollection.GCWatch() is now running on another thread.");
#endif

			// Pause for a moment to provide a delay to make threads more apparent.
			while (IsThreadContinued())
			{
				GC.Collect();
				Thread.Sleep(_interval_ms);
			}

#if COMLOGGING
			Debug.ReportInfo("Going to call m_EventThreadEnded.Set().");
#endif
			_eventThreadEnded.Set();
		}

		protected bool IsThreadContinued()
		{
			lock (this)
			{
				return _isThreadContinued;
			}
		}

		public void StopThread()
		{
			lock (this)
			{
				_isThreadContinued = false;
			}
		}

		public void WaitForThreadToStop()
		{
#if COMLOGGING
			Debug.ReportInfo("WaitForThreadToStop().");
#endif
			_eventThreadEnded.WaitOne();
			_eventThreadEnded.Reset();

#if COMLOGGING
			Debug.ReportInfo("WaitForThreadToStop() exiting.");
#endif
		}
	}
}