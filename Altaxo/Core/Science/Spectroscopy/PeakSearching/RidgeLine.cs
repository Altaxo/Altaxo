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

namespace Altaxo.Science.Spectroscopy.PeakSearching
{
  /// <summary>
  /// Represents a ridge line that is the output of the ridge line search in a Continuous Wavelet Transformation (Cwt) matrix.
  /// </summary>
  public class RidgeLine : List<(int Row, int Column, double CwtCoefficient)>
  {
    /// <summary>
    /// Gets the point at the lowest stage, i.e. at the lowest width. This is not neccessarily the stage 0, since the ridge line can end before reaching stage 0.
    /// </summary>
    /// <value>
    /// The point at the lowest stage.
    /// </value>
    public (int Row, int Column, double CwtCoefficient) PointAtLowestWidth => this[Count - 1];


    /// <summary>
    /// Gets the point where the CWT coefficient has the first time a local maximum (searching starts from stage0 (lowest width))
    /// </summary>
    /// <value>
    /// The point where the CWT coefficient has a local maximum. If no such point is found, the point with the maximal CWT coefficient is returned.
    /// </value>
    public (int Row, int Column, double CwtCoefficient) PointAtMaximalCwtCoefficient
    {
      get
      {
        var max = this[Count - 1].CwtCoefficient;
        for (int i = Count - 2; i >= 0; --i)
        {
          if (this[i].CwtCoefficient > max)
          {
            max = this[i].CwtCoefficient;
          }
          else
          {
            return this[i + 1];
          }
        }
        return this[0];
      }
    }

    /// <summary>
    /// Beginning from stage 0 (lowest width), a point is searched at which the Cwt coefficient has a local maximum. The parameter <paramref name="order"/> determines, how many points to the left
    /// and the right of the designates local maximum are taken into consideration.
    /// </summary>
    /// <param name="order">The order (must be at least 1). Number of points to the left and right of the designated maximum taken into consideration.</param>
    /// <returns>The first local maximum that is found. If no local maximum is found, the point at which the Cwt coefficient has its global maximum is returned.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">Order has to be >=1 - order</exception>
    public (int Row, int Column, double CwtCoefficient) GetPointAtMaximalCwtCoefficient(int order)
    {
      if (!(order >= 1))
        throw new ArgumentOutOfRangeException("Order has to be >=1", nameof(order));

      var lenM1 = Count - 1;

      int idxGlobalMax=0;
      double cwtGlobalMax = double.NegativeInfinity;
      for(int i=lenM1;i>=0;--i)
      {
        int j;
        for (j = order; j > 0; --j)
        {
          // make sure that to both sides the center Cwt coefficient is higher than the side coefficients
          // if not, break, so that j remains > 0
          if ((i + j) <= lenM1 && !(this[i].CwtCoefficient > this[i + j].CwtCoefficient))
            break;
          if ((i - j) >= 0 && !(this[i].CwtCoefficient > this[i - j].CwtCoefficient))
            break;
        }
        if(j==0) // found a local maximum, and return it
        {
          return this[i];
        }
        
        if (this[i].CwtCoefficient > cwtGlobalMax) // find global maximum
        {
          cwtGlobalMax = this[i].CwtCoefficient;
          idxGlobalMax = i;
        }
      }

      return this[idxGlobalMax];
    }

    // Some operators for filtering

    /// <summary>
    /// Gets a value indicating whether the ridge line starts at stage 0 (the stage with the lowest width).
    /// </summary>
    /// <value>
    ///   <c>true</c> if the ridge line starts at stage 0; otherwise, <c>false</c>.
    /// </value>
    public bool StartsAtStageZero => this[Count - 1].Row == 0;

    /// <summary>
    /// Gets a value indicating whether the ridge line has at least the provided length.
    /// </summary>
    /// <param name="minimalLength">Minimal required length of the ridge line.</param>
    /// <returns>True if the length of the ridge line is &gt;= <paramref name="minimalLength"/>; otherwise, false.</returns>
    public bool LengthIsAtLeast(int minimalLength) => Count >= minimalLength;

    /// <summary>
    /// Gets a value indicating whether the ridge line has at least a signal the provided signal-to-noise ratio.
    /// The signal-to-noise ratio is calculated at the point of the lowest stage (lowest width, see <see cref="PointAtLowestWidth"/>).
    /// </summary>
    /// <param name="noiseLevels">The noise levels along the x-axis. The array must have the same length than the length of the spectrum that was used to create the ridge line(s).</param>
    /// <param name="minimalSNR">The minimal required signal-to-noise ratio.</param>
    /// <returns>True if the signal-to-noise ratio is greater than or equal to <paramref name="minimalSNR"/>; otherwise, false.</returns>
    public bool SignalToNoiseRatioAtLowestWidthIsAtLeast(double[] noiseLevels, double minimalSNR) => GetSignalToNoiseRatioAtLowestWidth(noiseLevels) >= minimalSNR;

    /// <summary>
    /// Gets a value indicating whether the ridge line has at least a signal-to-noise ratio greater than or equal to the provided value.
    /// The signal-to-noise ratio is calculated at the point with the first maximum of the Cwt coefficient (see <see cref="PointAtMaximalCwtCoefficient"/>).
    /// </summary>
    /// <param name="noiseLevels">The noise levels along the x-axis. The array must have the same length than the length of the spectrum that was used to create the ridge line(s).</param>
    /// <param name="minimalSNR">The minimal required signal-to-noise ratio.</param>
    /// <param name="order">The order (must be at least 1). Number of points to the left and right of the designated maximum taken into consideration. See <see cref="GetPointAtMaximalCwtCoefficient(int)"/>.</param>
    /// <returns>True if the signal-to-noise ratio is greater than or equal to <paramref name="minimalSNR"/>; otherwise, false.</returns>
    public bool SignalToNoiseRatioAtMaximalCwtCoefficientIsAtLeast(double[] noiseLevels, double minimalSNR, int order) => GetSignalToNoiseRatioAtMaximalCwtCoefficient(noiseLevels, order) >= minimalSNR;


    /// <summary>
    /// Gets the signal-to-noise ratio at the point with the lowest stage (and therefore, the lowest width, see <see cref="PointAtLowestWidth"/>).
    /// </summary>
    /// <param name="noiseLevels">The noise levels along the x-axis. The array must have the same length than the length of the spectrum that was used to create the ridge line(s).</param>
    /// <returns>The signal-to-noise ratio at the point with the lowest stage (and therefore, the lowest width).</returns>
    public double GetSignalToNoiseRatioAtLowestWidth(double[] noiseLevels)
    {
      var m = PointAtLowestWidth;
      return m.CwtCoefficient / noiseLevels[m.Column];
    }

    /// <summary>
    /// Gets the signal-to-noise ratio at the point with the first maximum of the Cwt coefficient (see <see cref="PointAtMaximalCwtCoefficient"/>).
    /// </summary>
    /// <param name="noiseLevels">The noise levels along the x-axis. The array must have the same length than the length of the spectrum that was used to create the ridge line(s).</param>
    /// <param name="order">The order (must be at least 1). Number of points to the left and right of the designated maximum taken into consideration. See <see cref="GetPointAtMaximalCwtCoefficient(int)"/>.</param>
    /// <returns>The signal-to-noise ratio at the point with the first maximum of the Cwt coefficient</returns>
    public double GetSignalToNoiseRatioAtMaximalCwtCoefficient(double[] noiseLevels, int order)
    {
      var m = GetPointAtMaximalCwtCoefficient(order);
      return m.CwtCoefficient / noiseLevels[m.Column];
    }

  }

}

