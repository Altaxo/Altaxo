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

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Interface for the other site of a <see cref="IProgressReporter"/>, i.e. the site that reads the progress and bring it to display.
  /// </summary>
  public interface IProgressMonitor
  {
    /// <summary>
    /// Indicates that new report text has arrived that was not displayed yet.
    /// </summary>
    bool HasReportUpdate { get; }

    /// <summary>
    /// Gets the report update. When called, the function has to reset the <see cref="HasReportUpdate"/> flag.
    /// If you are not able to calculate the progress [0..1], this function should return <see cref="double.NaN"/>.
    /// </summary>
    (string text, double progressFraction) GetReportUpdate();


    /// <summary>
    /// Sets a flag that tries to interrupt the task softly. This will typically leave an incomplete, but not corrupted result.
    /// </summary>
    void SetCancellationPendingSoft();

    /// <summary>
    /// Sets a flag that tries to interrupt the task hardly. This will typically leave a corrupted result.
    /// </summary>
    void SetCancellationPendingHard();

  }
}
