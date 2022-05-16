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

namespace Altaxo.Science.Spectroscopy.PeakFitting
{
  /// <summary>
  /// Interface to peak searching methods.
  /// </summary>
  public interface IPeakFitting
  {
    /// <summary>
    /// Executes the normalization algorithm.
    /// </summary>
    /// <param name="xArray">The array of x-values.</param>
    /// <param name="yArray">The array of y-values.</param>
    /// <param name="peakDescriptions">Description of the peaks (output of peak searching algorithms, see <see cref="PeakSearching.IPeakSearching"/>).</param>
    /// <returns>The results of the peak fitting.</returns>
    IPeakFittingResult Execute(double[] xArray, double[] yArray, IEnumerable<PeakSearching.PeakDescription> peakDescriptions);
  }

  /// <summary>
  /// Interface to the results of peak searching algorithms.
  /// </summary>
  public interface IPeakFittingResult
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
    public PeakSearching.PeakDescription SearchDescription { get; init; }

    public string Notes { get; init; } = string.Empty;

    /// <summary>The peak position as index of the array.</summary>
    public double PositionIndex { get; init; }

    public double PositionInXUnits { get; init; }
    public double PositionInXUnitsVariance { get; init; }

    /// <summary>The height parameter.</summary>
    public double Height { get; init; }

    /// <summary>The height variance.</summary>
    public double HeightVariance { get; init; }

    /// <summary>The width parameter.</summary>
    public double Width { get; init; }

    /// <summary>The width variance.</summary>
    public double WidthVariance { get; init; }


    public double Area { get; init; }

    public PeakFitFunctions.FunctionWrapper? FitFunction { get; init; }
  }
}
