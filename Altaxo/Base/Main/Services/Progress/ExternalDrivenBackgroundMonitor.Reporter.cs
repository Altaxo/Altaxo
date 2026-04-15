#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Threading;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Provides monitoring support for externally driven background operations.
  /// </summary>
  public partial class ExternalDrivenBackgroundMonitor
  {
    /// <summary>
    /// Reports progress to an <see cref="ExternalDrivenBackgroundMonitor"/> and supports hierarchical subtasks.
    /// </summary>
    public class Reporter : IProgressReporter
    {
      /// <summary>
      /// Gets the root monitor that receives the aggregated progress.
      /// </summary>
      protected ExternalDrivenBackgroundMonitor Root { get; init; }

      /// <summary>
      /// Gets the parent reporter, or <c>null</c> if this is the root reporter.
      /// </summary>
      protected Reporter? Parent { get; init; }

      /// <summary>
      /// Gets the nesting level of this reporter.
      /// </summary>
      public int Level { get; init; }

      /// <inheritdoc/>
      public CancellationToken CancellationToken { get; init; }

      /// <summary>
      /// Gets the hard-cancellation token.
      /// </summary>
      public CancellationToken CancellationTokenHard { get; init; }

      /// <summary>The fraction of work the parent has assigned to this subtask.</summary>
      protected double FractionOfWorkOfParent { get; init; }

      /// <inheritdoc/>
      public string TaskName { get; init; }

      /// <summary>
      /// The sub task dictionary. Key is the subtask. Value is the amount of work from the latest report of the subtask.
      /// </summary>
      private Dictionary<IProgressReporter, double> _subTaskDictionary;

      private double _currentWorkDoneInSubtasks;

      private double _currentWorkDoneHere;

      private OperationStatus _status;


      /// <summary>
      /// Initializes a new instance of the <see cref="Reporter"/> class.
      /// </summary>
      /// <param name="root">The root background monitor.</param>
      /// <param name="parent">The parent reporter, if any.</param>
      /// <param name="level">The nesting level of this reporter.</param>
      /// <param name="fractionOfWorkOfParent">The fraction of the parent's work represented by this reporter.</param>
      /// <param name="taskName">The task name.</param>
      /// <param name="cancellationToken">The soft-cancellation token.</param>
      /// <param name="cancellationTokenHard">The hard-cancellation token.</param>
      public Reporter(
        ExternalDrivenBackgroundMonitor root,
        Reporter? parent,
        int level,
        double fractionOfWorkOfParent,
        string taskName,
        CancellationToken cancellationToken,
        CancellationToken cancellationTokenHard)
      {
        _subTaskDictionary = new Dictionary<IProgressReporter, double>();

        Root = root;
        Parent = parent;
        Level = level;
        FractionOfWorkOfParent = fractionOfWorkOfParent;
        TaskName = taskName;
        CancellationToken = cancellationToken;
        CancellationTokenHard = cancellationTokenHard;
      }

      /// <inheritdoc/>
      public IProgressReporter GetSubTask(double workAmount)
      {
        return CreateSubTask(workAmount, CancellationToken, CancellationTokenHard, TaskName);
      }

      /// <summary>
      /// Creates a subtask with explicit cancellation tokens.
      /// </summary>
      /// <param name="workAmount">The fraction of work represented by the subtask.</param>
      /// <param name="cancellationTokenSoft">The soft-cancellation token.</param>
      /// <param name="cancellationTokenHard">The hard-cancellation token.</param>
      /// <returns>A reporter for the created subtask.</returns>
      public IProgressReporter GetSubTask(double workAmount, CancellationToken cancellationTokenSoft, CancellationToken cancellationTokenHard)
      {
        return CreateSubTask(workAmount, cancellationTokenSoft, cancellationTokenHard, TaskName);
      }

      /// <summary>
      /// Creates a named subtask.
      /// </summary>
      /// <param name="fractionOfWork">The fraction of work represented by the subtask.</param>
      /// <param name="cancellationTokenSoft">The soft-cancellation token.</param>
      /// <param name="cancellationTokenHard">The hard-cancellation token.</param>
      /// <param name="taskName">The task name.</param>
      /// <returns>A reporter for the created subtask.</returns>
      public IProgressReporter CreateSubTask(double fractionOfWork, CancellationToken cancellationTokenSoft, CancellationToken cancellationTokenHard, string taskName)
      {
        var subTask = new Reporter(
          Root,
          this,
          Level + 1,
          fractionOfWork,
          taskName
,
          cancellationTokenSoft,
          cancellationTokenHard);


        lock (_subTaskDictionary)
        {
          _subTaskDictionary.Add(subTask, 0);
        }

        return subTask;
      }

      /// <summary>
      /// Is called from the child subtasks. 
      /// </summary>
      /// <param name="subtask">The subtask (is the immediate child of this task).</param>
      /// <param name="level">The level. This value can be higher than the level of the immediate child (if a child of the child task is reporting).</param>
      /// <param name="progressValue">The progress value.</param>
      /// <param name="text">The text.</param>
      /// <param name="status">The reported operation status.</param>
      protected void EhSubTaskReport(Reporter subtask, int level, double progressValue, string? text, OperationStatus status)
      {
        double differenceWork;
        double progress;
        OperationStatus statusNow;
        lock (_subTaskDictionary)
        {
          var previousWork = _subTaskDictionary[subtask];
          differenceWork = progressValue - previousWork;
          _subTaskDictionary[subtask] = progressValue;
          _currentWorkDoneInSubtasks += differenceWork; // must be done with thread safety
          progress = FractionOfWorkOfParent * Math.Min(1, Math.Max(0, _currentWorkDoneHere + _currentWorkDoneInSubtasks));
          statusNow = (OperationStatus)Math.Max((byte)status, (byte)_status);
        }

        if (Parent is { } p)
        {
          p.EhSubTaskReport(this, level, progress, text, statusNow);
        }
        else
        {
          Root.EhSubTaskReport(this, level, progress, text, statusNow);
        }
      }


      /// <inheritdoc/>
      public bool ShouldReportNow
      {
        get
        {
          return Root.ShouldReportNow;
        }
      }

      /// <inheritdoc/>
      public bool CancellationPending
      {
        get
        {
          return CancellationToken.IsCancellationRequested || CancellationTokenHard.IsCancellationRequested;
        }
      }


      /// <inheritdoc/>
      public void ReportProgress(string text, double progressValue)
      {
        double progress;
        OperationStatus statusNow;
        lock (_subTaskDictionary)
        {
          _currentWorkDoneHere = progressValue;
          progress = FractionOfWorkOfParent * Math.Min(1, Math.Max(0, _currentWorkDoneHere + _currentWorkDoneInSubtasks));
          statusNow = _status;
        }

        if (Parent is { } p)
        {
          p.EhSubTaskReport(this, Level, progress, text, statusNow);
        }
        else
        {
          Root.EhSubTaskReport(this, Level, progress, text, statusNow);
        }
      }

      /// <inheritdoc/>
      public void Report((string text, double progressFraction) value)
      {
        ReportProgress(value.text, value.progressFraction);
      }

      /// <inheritdoc/>
      public void Report(double value)
      {
        ReportProgress(null!, value);
      }

      /// <inheritdoc/>
      public void Report(string value)
      {
        double progress;
        OperationStatus statusNow;
        lock (_subTaskDictionary)
        {
          progress = FractionOfWorkOfParent * Math.Min(1, Math.Max(0, _currentWorkDoneHere + _currentWorkDoneInSubtasks));
          statusNow = _status;
        }

        if (Parent is { } p)
        {
          p.EhSubTaskReport(this, Level, progress, value, statusNow);
        }
        else
        {
          Root.EhSubTaskReport(this, Level, progress, value, statusNow);
        }
      }
      /// <inheritdoc/>
      public void ReportProgress(string text)
      {
        Report(text);
      }

      /// <inheritdoc/>
      public void ReportStatus(OperationStatus status)
      {
        OperationStatus statusNow;
        double progress;
        lock (_subTaskDictionary)
        {
          progress = FractionOfWorkOfParent * Math.Min(1, Math.Max(0, _currentWorkDoneHere + _currentWorkDoneInSubtasks));
          _status = statusNow = (OperationStatus)Math.Max((byte)_status, (byte)status);
        }

        if (Parent is { } p)
        {
          p.EhSubTaskReport(this, Level, progress, null, statusNow);
        }
        else
        {
          Root.EhSubTaskReport(this, Level, progress, null, statusNow);
        }
      }

      /// <inheritdoc/>
      public void Dispose()
      {
        Report(FractionOfWorkOfParent);
      }
    }
  }

}
