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
using System.Threading;

namespace Altaxo.Main.Services
{
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

    public IProgressReporter CreateSubTask(double workAmount)
    {
      return this;
    }

    public IProgressReporter CreateSubTask(double workAmount, CancellationToken cancellationToken)
    {
      return this;
    }

    public void Report(double value)
    {
    }

    public void Dispose()
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

    public double Progress
    {
      get
      {
        return 0;
      }
      set
      {
      }
    }

    public OperationStatus Status
    {
      get
      {
        return OperationStatus.Normal;
      }
      set
      {
      }
    }

    public string TaskName { get; set; }

    public CancellationToken CancellationToken { get; set; }

    #endregion IBackgroundMonitor Members
  }
}
