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
    /// <param name="x">
    /// The x values of the spectrum. Can be <see langword="null"/> (then only the positions as indices are calculated).
    /// </param>
    /// <param name="y">The y values of the spectrum.</param>
    /// <param name="regions">
    /// The spectral regions. Can be <see langword="null"/> (if the array is one region). Each element in this array
    /// is the start index of a new spectral region.
    /// </param>
    /// <returns>
    /// The results of the peak searching.
    /// For each spectral region, a tuple containing the peak descriptions in that range and the start and end index (exclusive)
    /// of that range is returned.
    /// Please note that the peak descriptions contain position indices that are relative to the corresponding region (and thus
    /// not to the underlying spectral array).
    /// </returns>
    (
    double[] x,
    double[] y,
    int[]? regions,
    IReadOnlyList<(IReadOnlyList<PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> peakSearchResults
    ) Execute(double[] x, double[] y, int[]? regions);
  }

  /// <summary>
  /// Describes a single peak found by a peak searching algorithm.
  /// </summary>
  public record PeakDescription
  {
    /// <summary>
    /// Gets the peak position as an index within the spectral region.
    /// </summary>
    public double PositionIndex { get; init; }

    /// <summary>
    /// Gets the position value.
    /// If an x-axis was provided for the peak searching algorithm, this contains the x-value at the peak position;
    /// otherwise, it is the same value as <see cref="PositionIndex"/>.
    /// </summary>
    public double PositionValue { get; init; }

    /// <summary>
    /// Gets the peak prominence.
    /// </summary>
    public double Prominence { get; init; }

    /// <summary>
    /// Gets the peak's total height.
    /// </summary>
    public double Height { get; init; }


    /// <summary>
    /// Gets the peak width, in points.
    /// </summary>
    public double WidthPixels { get; init; }

    /// <summary>
    /// Gets the peak width, in x-units.
    /// </summary>
    public double WidthValue { get; init; }

    /// <summary>
    /// Gets the relative height of the peak that was used to measure the width.
    /// </summary>
    public double RelativeHeightOfWidthDetermination { get; init; }

    /// <summary>
    /// Gets the absolute height of the peak that was used to measure the width.
    /// </summary>
    public double AbsoluteHeightOfWidthDetermination { get; init; }
  }
}
