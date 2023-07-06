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
using System.ComponentModel;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Interface for the other site of a <see cref="IProgressReporter"/>, i.e. the site that reads the progress and bring it to display.
  /// </summary>
  public interface IProgressMonitor : INotifyPropertyChanged, IDisposable
  {
    /// <summary>
    /// Sets a flag that tries to interrupt the task softly. This will typically leave an incomplete, but not corrupted result.
    /// </summary>
    void SetCancellationPendingSoft();

    /// <summary>
    /// Sets a flag that tries to interrupt the task hardly. This will typically leave a corrupted result.
    /// </summary>
    void SetCancellationPendingHard();

    /// <summary>
    /// Gets the progress. If the value changes, it must be notified through the <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    double Progress { get; }

    /// <summary>
    /// Gets the text at level 0. If the value changes, it must be notified through the <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    string Text0 { get; }

    /// <summary>
    /// Gets a flag indicating whether this instance is disposed.
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// Gets the name of the task.
    /// </summary>
    string TaskName { get; }

    /// <summary>
    /// Gets the overall status of the task.
    /// </summary>
    OperationStatus Status { get; }
  }
}
