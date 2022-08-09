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
using System;
using System.Threading;

namespace Altaxo
{
  /// <summary>
  /// Allows a thread to report text to a receiver. Additionally, the thread can look to the property <see cref="CancellationPending" />, and
  /// if it is <c>true</c>, return in a safe way.
  /// </summary>
  public interface IProgressReporter : IProgress<double>, IProgress<string>, IProgress<(string text, double progressFraction)>, IDisposable
  {
    /// <summary>
    /// Sets the amount of work already done within this task.
    /// Always uses a scale from 0 to 1 local to this progress reporter.
    /// </summary>
    double Progress { get; set; }

    /// <summary>
    /// Sets the operation status.
    ///
    /// Note: the status of the whole operation is the most severe status of all nested monitors.
    /// The more severe value persists even if the child monitor gets disposed.
    /// </summary>
    OperationStatus Status { get; set; }

    /// <summary>
    /// Creates a nested task.
    /// </summary>
    /// <param name="workAmount">The amount of work this sub-task performs in relation to the work of this task.
    /// That means, this parameter is used as a scaling factor for work performed within the subtask.</param>
    /// <returns>A new progress monitor representing the sub-task.
    /// Multiple child progress monitors can be used at once; even concurrently on multiple threads.</returns>
    IProgressReporter CreateSubTask(double workAmount);

    /// <summary>
    /// Creates a nested task.
    /// </summary>
    /// <param name="workAmount">The amount of work this sub-task performs in relation to the work of this task.
    /// That means, this parameter is used as a scaling factor for work performed within the subtask.</param>
    /// <param name="cancellationTokenSoft">
    /// A cancellation token that can be used to cancel the sub-task, typically with incomplete, but not corrupted result.
    /// Note: cancelling the main task will not cancel the sub-task.
    /// </param>
    /// <param name="cancellationTokenHard">
    /// A cancellation token that can be used to hard cancel the sub-task, typically with corrupted result.
    /// Note: cancelling the main task will cancel the sub-task.
    /// </param>
    /// <returns>A new progress monitor representing the sub-task.
    /// Multiple child progress monitors can be used at once; even concurrently on multiple threads.</returns>
    IProgressReporter CreateSubTask(double workAmount, CancellationToken cancellationTokenSoft, CancellationToken cancellationTokenHard);

    /// <summary>
    /// Gets/Sets the name to show while the task is active.
    /// </summary>
    string TaskName { get; set; }

    /// <summary>
    /// Gets the cancellation token (soft).
    /// Typical use of the soft cancellation token is to interrupt some work, without compromising the result (the result typically is incomplete, but not corrupted).
    /// </summary>
    /// <seealso cref="CancellationTokenHard"/>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets the cancellation token (hard).
    /// Typical use of the hard cancellation token is to abort some work, with corrupting the result. If the result is not corrupted, then use the <seealso cref="CancellationToken"/>.
    /// </summary>
    /// <seealso cref="CancellationToken"/>
    CancellationToken CancellationTokenHard { get; }


    /// <summary>
    /// True if we should report the progress now.
    /// </summary>
    bool ShouldReportNow { get; }

    /// <summary>
    /// Reports the progress as a text string.
    /// </summary>
    /// <param name="text">Report text</param>
    void ReportProgress(string text);

    /// <summary>
    /// Reports the progress as a text string.
    /// </summary>
    /// <param name="text">Report text</param>
    /// <param name="progressValue">The progress as fraction (0..1).</param>
    void ReportProgress(string text, double progressValue);

    /// <summary>
    /// Returns true if the activity was cancelled by the user. The script has to check this value periodically. If it is set to true, the script should return.
    /// </summary>
    bool CancellationPending { get; }
  }

  /// <summary>
  /// Represents the status of a operation with progress monitor.
  /// </summary>
  public enum OperationStatus : byte
  {
    /// <summary>
    /// Everything is normal.
    /// </summary>
    Normal,

    /// <summary>
    /// There was at least one warning.
    /// </summary>
    Warning,

    /// <summary>
    /// There was at least one error.
    /// </summary>
    Error
  }
}
