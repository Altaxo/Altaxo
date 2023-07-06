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
  public partial class ExternalDrivenBackgroundMonitor
  {
    public class Reporter : IProgressReporter
    {
      protected ExternalDrivenBackgroundMonitor Root { get; init; }

      protected Reporter? Parent { get; init; }

      public int Level { get; init; }

      public CancellationToken CancellationToken { get; init; }

      public CancellationToken CancellationTokenHard { get; init; }

      /// <summary>The fraction of work the parent has assigned to this subtask.</summary>
      protected double FractionOfWorkOfParent { get; init; }

      public string TaskName { get; init; }

      /// <summary>
      /// The sub task dictionary. Key is the subtask. Value is the amount of work from the latest report of the subtask.
      /// </summary>
      private Dictionary<IProgressReporter, double> _subTaskDictionary;

      private double _currentWorkDoneInSubtasks;

      private double _currentWorkDoneHere;

      private OperationStatus _status;


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

      public IProgressReporter GetSubTask(double fractionOfWork)
      {
        return CreateSubTask(fractionOfWork, CancellationToken, CancellationTokenHard, TaskName);
      }

      public IProgressReporter GetSubTask(double fractionOfWork, CancellationToken cancellationTokenSoft, CancellationToken cancellationTokenHard)
      {
        return CreateSubTask(fractionOfWork, cancellationTokenSoft, cancellationTokenHard, TaskName);
      }

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


      public bool ShouldReportNow
      {
        get
        {
          return Root.ShouldReportNow;
        }
      }

      public bool CancellationPending
      {
        get
        {
          return CancellationToken.IsCancellationRequested || CancellationTokenHard.IsCancellationRequested;
        }
      }


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

      public void Report((string text, double progressFraction) value)
      {
        ReportProgress(value.text, value.progressFraction);
      }

      public void Report(double value)
      {
        ReportProgress(null!, value);
      }

      public void Report(string text)
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
          p.EhSubTaskReport(this, Level, progress, text, statusNow);
        }
        else
        {
          Root.EhSubTaskReport(this, Level, progress, text, statusNow);
        }
      }
      public void ReportProgress(string text)
      {
        Report(text);
      }

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

      public void Dispose()
      {
        Report(FractionOfWorkOfParent);
      }
    }
  }

}
