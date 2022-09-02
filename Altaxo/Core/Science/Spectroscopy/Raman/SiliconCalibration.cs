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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Altaxo.Science.Spectroscopy.Raman
{
  /// <summary>
  /// Represents the results of a calibration of a Raman device with light from a Neon lamp.
  /// </summary>
  public class SiliconCalibration
  {

    #region Operational data

    /// <summary>
    /// The x-vales of the Neon measurement, converted to nm, presumed that the laser has the wavelength
    /// </summary>
    private double[]? _xPreprocessed;
    private double[]? _yPreprocessed;
    private List<PeakSearching.PeakDescription>? _peakSearchingDescriptions;
    private IReadOnlyList<PeakFitting.PeakDescription>? _peakFittingDescriptions;
    public double[]? XPreprocessed => _xPreprocessed;
    public double[]? YPreprocessed => _yPreprocessed;

    #endregion

    #region Result data

    public bool IsPeakFound { get; private set; }

    public double SiliconPeakPosition { get; private set; } = double.NaN;

    public double SiliconPeakPositionStdDev { get; private set; } = double.NaN;

    #endregion

    /// <summary>
    /// Finds the Silicon peak.
    /// </summary>
    /// <param name="options">The options used for calculation.</param>
    /// <param name="x">The x values of the measured Neon spectrum.</param>
    /// <param name="y">The y values of the measured Neon spectrum.</param>
    /// <param name="cancellationToken">Token used to cancel this task.</param>
    /// <returns>The position (as shift value) and position tolerance of the silicon peak.</returns>
    /// The returned value is null if no peaks could be matched.
    public (double Position, double PositionTolerance)?
    FindMatch(SiliconCalibrationOptions options, double[] x, double[] y, CancellationToken cancellationToken)
    {
      // make sure that peak fitting is activated
      if (options.PeakFindingOptions.PeakFitting is null || options.PeakFindingOptions.PeakFitting is PeakFitting.PeakFittingNone)
        throw new InvalidOperationException("Silicon calibration without peak fitting is not allowed (it gives an inaccurate result)");


      Array.Sort(x, y); // Sort x-axis ascending
      var peakOptions = options.PeakFindingOptions;
      (x, y, _) = peakOptions.Preprocessing.Execute(x, y, null);
      _xPreprocessed = x;
      _yPreprocessed = y;

      var peakSearchingResults = peakOptions.PeakSearching.Execute(x, y, null);
      _peakSearchingDescriptions = peakSearchingResults[0].PeakDescriptions.ToList();
      _peakSearchingDescriptions.Sort((a, b) => Comparer<double>.Default.Compare(a.PositionIndex, b.PositionIndex));

      var peakFitting = peakOptions.PeakFitting;
      var peakFittingDescriptions = peakFitting.Execute(x, y, peakSearchingResults, cancellationToken);
      _peakFittingDescriptions = peakFittingDescriptions[0].PeakDescriptions;

      // now look for the peak around 425 nm

      double boundarySearchLeft = options.GetOfficialShiftValue_Silicon_invcm() - options.RelativeShift_Tolerance_invcm;
      double boundarySearchRight = options.GetOfficialShiftValue_Silicon_invcm() + options.RelativeShift_Tolerance_invcm;

      var listOfCandidates = _peakFittingDescriptions.Where(d => d.FitFunction is not null && d.PositionAreaHeightFWHM.Position >= boundarySearchLeft && d.PositionAreaHeightFWHM.Position <= boundarySearchRight).ToList();
      listOfCandidates.Sort((x, y) => Comparer<double>.Default.Compare(y.PositionAreaHeightFWHM.Height, x.PositionAreaHeightFWHM.Height));

      if (listOfCandidates.Count == 0)
      {
        return null;
      }
      else
      {
        var para = listOfCandidates[0].FitFunction.GetPositionAreaHeightFWHMFromSinglePeakParameters(listOfCandidates[0].PeakParameter, listOfCandidates[0].PeakParameterCovariances);

        IsPeakFound = true;
        SiliconPeakPosition = para.Position;
        SiliconPeakPositionStdDev = para.PositionStdDev;
        return (para.Position, para.PositionStdDev);
      }
    }
  }
}
