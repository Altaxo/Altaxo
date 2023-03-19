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
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.FitFunctions.Probability;

namespace Altaxo.Science.Spectroscopy.PeakFitting
{
  public record class PeakFittingBase
  {
    private IFitFunctionPeak _fitFunction = new VoigtAreaParametrizationNu();

    /// <summary>
    /// Gets /sets the fit function to use.
    /// </summary>
    /// <value>
    /// The fit function.
    /// </value>
    public IFitFunctionPeak FitFunction
    {
      get { return _fitFunction; }
      init { _fitFunction = value ?? throw new ArgumentNullException(nameof(FitFunction)); }
    }

    /// <summary>
    /// Gets / sets the scaling factor of the fit width. To calculate the fit width, the scaling factor is
    /// multiplied by the FullWidthHalfMaximum of the peak.
    /// </summary>
    private double _fitWidthScalingFactor = 2;

    /// <summary>
    /// Gets / sets the scaling factor of the fit width. To calculate the fit width, the scaling factor is
    /// multiplied by the FullWidthHalfMaximum of the peak.
    /// </summary>
    public double FitWidthScalingFactor
    {
      get
      {
        return _fitWidthScalingFactor;
      }
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException("Factor has to be > 0", nameof(FitWidthScalingFactor));

        _fitWidthScalingFactor = value;
      }
    }



    /// <summary>
    /// Gets the minimal and maximal properties of an array of x-values.
    /// </summary>
    /// <param name="array">The array of x values.</param>
    /// <returns>The (absolute value) of the minimal distance between two consecutive data points, the maximal distance, the minimal value and the maximal value of the array.</returns>
    protected static (double minimalDistance, double maximalDistance, double minimalValue, double maximalValue) GetMinimalAndMaximalProperties(IEnumerable<double> array)
    {
      double min = double.PositiveInfinity;
      double max = double.NegativeInfinity;
      double minDist = double.PositiveInfinity;
      double previousX = double.NaN;
      foreach (var x in array)
      {
        var dist = Math.Abs(x - previousX);

        if (dist > 0 && dist < minDist)
        {
          minDist = dist;
        }

        min = Math.Min(min, x);
        max = Math.Max(max, x);
        previousX = x;
      }

      return (minDist, max - min, min, max);
    }



  }
}
