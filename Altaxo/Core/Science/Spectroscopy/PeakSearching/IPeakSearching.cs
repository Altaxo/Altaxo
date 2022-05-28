#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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


using System.Collections.Generic;

namespace Altaxo.Science.Spectroscopy.PeakSearching
{
  /// <summary>
  /// Interface to peak searching methods.
  /// </summary>
  public interface IPeakSearching
  {
    /// <summary>
    /// Executes the peak searching algorithm.
    /// </summary>
    /// <param name="input">The input array.</param>
    /// <returns>The results of the peak searching.</returns>
    IPeakSearchingResult Execute(double[] input);
  }

  /// <summary>
  /// Interface to the results of peak searching algorithms.
  /// </summary>
  public interface IPeakSearchingResult
  {
    /// <summary>
    /// Gets the peak descriptions.
    /// </summary>
    public IReadOnlyList<PeakDescription> PeakDescriptions { get; }
  }

  /// <summary>
  /// Description of one peak
  /// </summary>
  public record PeakDescription
  {
    /// <summary>The peak position as index of the array.</summary>
    public double PositionIndex { get; init; }

    /// <summary>The peak prominence.</summary>
    public double Prominence { get; init; }

    /// <summary>The peak's total heigth.</summary>
    public double Height { get; init; }


    /// <summary>The peak width.</summary>
    public double Width { get; init; }

    /// <summary>The relative heigth of the peak that was used to measure
    /// the width.</summary>
    public double RelativeHeightOfWidthDetermination { get; init; }

    public double AbsoluteHeightOfWidthDetermination { get; init; }

  }
}
