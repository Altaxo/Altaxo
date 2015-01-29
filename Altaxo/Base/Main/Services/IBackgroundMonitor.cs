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

#endregion Copyright

using System;

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

		/// <summary>Gets the progress as fraction. If you are not able to calculate the progress, this function should return <see cref="System.Double.NaN"/>.</summary>
		/// <returns>The progress as fraction value [0..1], or <see cref="System.Double.NaN"/>.</returns>
		double GetProgressFraction();
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

		public void ReportProgress(string text, double progressFraction)
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

		#endregion IBackgroundMonitor Members
	}

	public class ExternalDrivenBackgroundMonitor : IExternalDrivenBackgroundMonitor
	{
		protected bool _shouldReport;
		private string _reportText;
		private bool _hasFreshReportText;
		private double _progressFraction = double.NaN;

		private bool _cancellationPending;

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

		public void ReportProgress(string text, double progressFraction)
		{
			_shouldReport = false;
			_reportText = text;
			_hasFreshReportText = true;
			_progressFraction = progressFraction;
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

		public double GetProgressFraction()
		{
			return _progressFraction;
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

		#endregion IBackgroundMonitor Members
	}

	public class ExternalDrivenTimeReportMonitor : ExternalDrivenBackgroundMonitor
	{
		private DateTime _timeBegin = DateTime.Now;

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
		private System.Timers.Timer _timer = new System.Timers.Timer(200);
		private bool _shouldReport;
		private bool _cancellationPending;
		private string _reportText;
		private double _progressFraction = double.NaN;

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

		public void ReportProgress(string text, double progressFraction)
		{
			_shouldReport = false;
			_reportText = text;
			_progressFraction = progressFraction;
		}

		public string ReportText
		{
			set { _reportText = value; }
			get { return _reportText; }
		}

		public double ProgressFraction
		{
			get { return _progressFraction; }
			set { _progressFraction = value; }
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

		#endregion IBackgroundMonitor Members

		private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			_shouldReport = true;
			if (this.Elapsed != null)
				this.Elapsed(sender, e);
		}
	}
}