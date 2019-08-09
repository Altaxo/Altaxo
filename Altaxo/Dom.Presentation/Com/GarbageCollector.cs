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
using System.Threading;

namespace Altaxo.Com
{
  /// <summary>
  /// Keeps a thread alive that continually collects and disposes unused objects.
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
      ComDebug.ReportInfo("GarbageCollection.GCWatch() is now running on another thread.");

      // Pause for a moment to provide a delay to make threads more apparent.
      while (IsThreadContinued())
      {
        GC.Collect();
        Thread.Sleep(_interval_ms);
      }

      ComDebug.ReportInfo("Going to call m_EventThreadEnded.Set().");
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
      ComDebug.ReportInfo("WaitForThreadToStop().");
      _eventThreadEnded.WaitOne();
      _eventThreadEnded.Reset();

      ComDebug.ReportInfo("WaitForThreadToStop() exiting.");
    }
  }
}
