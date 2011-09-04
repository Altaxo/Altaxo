#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
#endregion


using System;
using Altaxo;

namespace Altaxo.Main.Services
{
	/// <summary>
	/// Interface for the other site, i.e. the site that reads the progress and bring it to display.
	/// </summary>
	public interface IProgressMonitor
	{
		/// <summary>
		/// Indicates that new report text has arrived that was not displayed yet.
		/// </summary>
		bool HasReportText { get; }
		/// <summary>
		/// Gets the report text. When called, the function has to reset the <see cref="HasReportText"/> flag.
		/// </summary>
		string GetReportText();
	}
 

  public interface IExternalDrivenBackgroundMonitor : IProgressReporter, IProgressMonitor
  {
    /// <summary>
    /// Sets the <see cref="IProgressReporter.ShouldReportNow"/> flag to <c>True</c> to indicate that the worker thread should report its progress.
    /// </summary>
		void SetShouldReportNow();

    /// <summary>
    /// Sets the <see cref="IProgressReporter.CancellationPending"/> flag to <c>True</c> to indicate that the worker thread should cancel its activity.
    /// </summary>
		void SetCancellationPending();
  }

  public class DummyBackgroundMonitor : IProgressReporter
  {
    #region IBackgroundMonitor Members

    public bool ShouldReportNow
    {
      get
      {
        return false;
      }
    }

    public void ReportProgress(string text)
    {
      
    }

    public string ReportText 
    {
      get 
      {
        return string.Empty;
      }
    }

    public bool CancellationPending
    {
      get
      {
        
        return false;
      }
    }

    #endregion

  }

  public class ExternalDrivenBackgroundMonitor : IExternalDrivenBackgroundMonitor
  {
    protected bool _shouldReport;
    string _reportText;
		bool _hasFreshReportText;

    bool _cancellationPending;

    #region IBackgroundMonitor Members

    public virtual bool ShouldReportNow
    {
      get
      {
        return _shouldReport;
      }
    }

		public virtual void SetShouldReportNow()
		{
			_shouldReport = true;
		}

    public void ReportProgress(string text)
    {
      _shouldReport = false;
      _reportText = text;
			_hasFreshReportText = true;
    }


		public bool HasReportText
		{
			get { return _hasFreshReportText; }
		}

    public string GetReportText()
    {
				_hasFreshReportText = false;
				return _reportText; 
    }

    public bool CancellationPending
    {
      get
      {
        return _cancellationPending;
      }
    }

		public void SetCancellationPending()
		{
			_cancellationPending = true;
		}

    #endregion

  }

  public class ExternalDrivenTimeReportMonitor : ExternalDrivenBackgroundMonitor
  {
    DateTime _timeBegin = DateTime.Now;

    public override bool ShouldReportNow
    {
      get
      {
        return _shouldReport;
      }
     
    }

		public override void SetShouldReportNow()
		{
			_shouldReport = true;

			if (_shouldReport)
				ReportProgress("Busy ... " + (DateTime.Now - _timeBegin).ToString());
		}
  }

  public class TimedBackgroundMonitor : IProgressReporter
  {
    System.Timers.Timer _timer = new System.Timers.Timer(200);
    bool _shouldReport;
    bool _cancellationPending;
    string _reportText;

    public event System.Timers.ElapsedEventHandler Elapsed;

    public TimedBackgroundMonitor()
    {
      _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
    }

    public void Start()
    {
      _timer.Start();
    }

    public void Stop()
    {
      _timer.Stop();
    }

    public System.ComponentModel.ISynchronizeInvoke SynchronizingObject
    {
      get { return _timer.SynchronizingObject; }
      set { _timer.SynchronizingObject = value; }
    }

    #region IBackgroundMonitor Members

    public bool ShouldReportNow
    {
      get
      {
        return _shouldReport;
      }
    }

    public void ReportProgress(string text)
    {
      _shouldReport = false;
      _reportText = text;
      
    }

    public string ReportText
    {
      set { _reportText = value; }
      get { return _reportText; }
    }

    public bool CancellationPending
    {
      get
      {
        return _cancellationPending;
      }
    }

		public void SetCancellationPending()
		{
			_cancellationPending = true;
		}

    #endregion

    private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      _shouldReport = true;
      if(this.Elapsed!=null)
        this.Elapsed(sender,e);
    }
  }

}
