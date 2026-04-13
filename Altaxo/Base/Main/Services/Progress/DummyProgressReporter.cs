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

#nullable enable
using System.Threading;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Dummy implementation of <see cref="IProgressReporter"/>. Does nothing.
  /// </summary>
  /// <seealso cref="Altaxo.IProgressReporter" />
  public class DummyProgressReporter
    : IProgressReporter
  {
    /// <summary>
    /// Gets the shared dummy reporter instance.
    /// </summary>
    public static DummyProgressReporter Instance { get; } = new DummyProgressReporter();

    private DummyProgressReporter() { }

    #region IBackgroundMonitor Members

    /// <inheritdoc/>
    public bool ShouldReportNow
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public void ReportProgress(string text)
    {
    }

    /// <inheritdoc/>
    public void ReportProgress(string text, double progressValue)
    {
    }

    /// <inheritdoc/>
    public IProgressReporter GetSubTask(double workAmount)
    {
      return this;
    }

    /// <inheritdoc/>
    public IProgressReporter CreateSubTask(double workAmount, CancellationToken cancellationToken)
    {
      return this;
    }

    /// <inheritdoc/>
    public void Report(double value)
    {
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public IProgressReporter GetSubTask(double workAmount, CancellationToken cancellationTokenSoft, CancellationToken cancellationTokenHard)
    {
      return this;
    }

    /// <inheritdoc/>
    public void Report(string value)
    {

    }

    /// <inheritdoc/>
    public void Report((string text, double progressFraction) value)
    {

    }

    /// <inheritdoc/>
    public void ReportStatus(OperationStatus status)
    {
    }

    /// <inheritdoc/>
    public string ReportText
    {
      get
      {
        return string.Empty;
      }
    }

    /// <inheritdoc/>
    public bool CancellationPending
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public string TaskName { get; set; } = string.Empty;

    /// <inheritdoc/>
    public CancellationToken CancellationToken => CancellationToken.None;

    /// <inheritdoc/>
    public CancellationToken CancellationTokenHard => CancellationToken.None;

    #endregion IBackgroundMonitor Members
  }
}
