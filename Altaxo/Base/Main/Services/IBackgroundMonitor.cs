#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Allows a thread to report text to a receiver. Additionally, the thread can look to the property <see cref="CancelledByUser" />, and
  /// if it is <c>true</c>, return in a safe way.
  /// </summary>
  public interface IBackgroundMonitor
  {
    /// <summary>
    /// True when we should send a string what we do currently
    /// </summary>
    bool ShouldReport { get; }

    /// <summary>
    /// Sets a text that the user can read.
    /// </summary>
    /// <param name="text"></param>
    void Report(string text);

    /// <summary>
    /// Gets the report text.
    /// </summary>
    string ReportText { get; }

    /// <summary>
    /// Returns true if the activity was cancelled by the user
    /// </summary>
    bool CancelledByUser { get; }
    
  }

  public interface IExternalDrivenBackgroundMonitor : IBackgroundMonitor
  {
    /// <summary>
    /// Can be set True when the watching instance (for instance a dialog) should report the text.
    /// </summary>
    new bool ShouldReport { set; }

    /// <summary>
    /// Can be set True if the activity should be cancelled.
    /// </summary>
    new bool CancelledByUser { set; }
  }

  public class DummyBackgroundMonitor : IBackgroundMonitor
  {
    #region IBackgroundMonitor Members

    public bool ShouldReport
    {
      get
      {
        return false;
      }
    }

    public void Report(string text)
    {
      
    }

    public string ReportText 
    {
      get 
      {
        return string.Empty;
      }
    }

    public bool CancelledByUser
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
    bool _cancelledByUser;

    #region IBackgroundMonitor Members

    public virtual bool ShouldReport
    {
      get
      {
        return _shouldReport;
      }
      set
      {
        _shouldReport |= value;
      }
    }

    public void Report(string text)
    {
      _shouldReport = false;
      _reportText = text;
    }

    public string ReportText
    {
      get { return _reportText; }
    }

    public bool CancelledByUser
    {
      get
      {
        return _cancelledByUser;
      }
      set 
      {
        _cancelledByUser |= value;
      }
    }

    #endregion

  }

  public class ExternalDrivenTimeReportMonitor : ExternalDrivenBackgroundMonitor
  {
    DateTime _timeBegin = DateTime.Now;

    public override bool ShouldReport
    {
      get
      {
        return _shouldReport;
      }
      set
      {
        _shouldReport |= value;

        if(_shouldReport)
          Report("Busy ... " + (DateTime.Now - _timeBegin).ToString());
      }
    }
  }

  public class TimedBackgroundMonitor : IBackgroundMonitor
  {
    System.Timers.Timer _timer = new System.Timers.Timer(200);
    bool _shouldReport;
    bool _cancelledByUser;
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

    public bool ShouldReport
    {
      get
      {
        return _shouldReport;
      }
    }

    public void Report(string text)
    {
      _shouldReport = false;
      _reportText = text;
      
    }

    public string ReportText
    {
      set { _reportText = value; }
      get { return _reportText; }
    }

    public bool CancelledByUser
    {
      get
      {
        return _cancelledByUser;
      }
      set
      {
        _cancelledByUser = value;
      }
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
