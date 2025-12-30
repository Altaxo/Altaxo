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

using System;
using Altaxo.Calc.Regression;

namespace Altaxo.Science.Spectroscopy.BaselineEstimation
{
  /// <summary>
  /// Base type for SNIP (Statistics-sensitive Non-linear Iterative Procedure) baseline estimation methods.
  /// </summary>
  public abstract record SNIP_Base
  {
    protected const double sqrt2 = 1.41421356237;
    protected const bool _roundUp = false;

    protected double _halfWidth = 15;
    protected bool _isHalfWidthInXUnits;
    protected int _numberOfRegularStages = 40;

    /// <summary>
    /// Gets half of the width of the averaging window.
    /// </summary>
    /// <remarks>
    /// This value should be set to roughly the FWHM (full width at half maximum) of the broadest peak in the spectrum.
    /// </remarks>
    public double HalfWidth
    {
      get { return _halfWidth; }
      init
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException("Value must be >=0", nameof(HalfWidth));
        _halfWidth = value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether <see cref="HalfWidth"/> is specified in x-axis units.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if <see cref="HalfWidth"/> is given in x-axis units; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsHalfWidthInXUnits
    {
      get
      {
        return _isHalfWidthInXUnits;
      }
      init
      {
        _isHalfWidthInXUnits = value;
      }
    }


    /// <summary>
    /// Gets the number of regular iterations.
    /// The default is 40.
    /// </summary>
    /// <value>The number of regular iterations.</value>
    /// <exception cref="System.ArgumentOutOfRangeException">Number of iterations must be at least one.</exception>
    public int NumberOfRegularIterations
    {
      get { return _numberOfRegularStages; }
      init
      {
        if (value < 1)
          throw new ArgumentOutOfRangeException("Number of iterations must be at least one.", nameof(NumberOfRegularIterations));
        _numberOfRegularStages = value;
      }
    }

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var yBaseline = new double[y.Length];
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        var xSpan = new ReadOnlySpan<double>(x, start, end - start);
        var ySpan = new ReadOnlySpan<double>(y, start, end - start);
        var yBaselineSpan = new Span<double>(yBaseline, start, end - start);
        Execute(xSpan, ySpan, yBaselineSpan);
      }

      // subtract baseline
      var yy = new double[y.Length];
      for (int i = 0; i < y.Length; i++)
      {
        yy[i] = y[i] - yBaseline[i];
      }

      return (x, yy, regions);
    }

    /// <summary>
    /// Executes the algorithm for the provided spectrum and writes the estimated baseline into <paramref name="result"/>.
    /// </summary>
    /// <param name="xArray">The x-values of the spectrum.</param>
    /// <param name="yArray">The y-values of the spectrum.</param>
    /// <param name="result">The destination span to which the estimated baseline is written.</param>
    public virtual void Execute(ReadOnlySpan<double> xArray, ReadOnlySpan<double> yArray, Span<double> result)
    {
      var srcY = new double[yArray.Length];
      var tmpY = new double[yArray.Length];
      yArray.CopyTo(srcY);

      var stat = GetStatisticsOfInterPointDistance(xArray);
      if (_isHalfWidthInXUnits && 0.5 * (stat.Max - stat.Min) / stat.Max > 1.0 / xArray.Length)
      {
        // if the interpoint distance is not uniform, we need to use the algorithm with locally calculated half width
        EvaluateBaselineWithLocalHalfWidth(xArray, srcY, tmpY, result);
        return;
      }
      else
      {
        int w;

        if (_isHalfWidthInXUnits)
        {
          w = _roundUp ? Math.Max(1, (int)Math.Ceiling(Math.Abs(_halfWidth / stat.Mean))) :
                         Math.Max(1, (int)Math.Floor(Math.Abs(_halfWidth / stat.Mean)));
        }
        else
        {
          w = _roundUp ? Math.Max(1, (int)Math.Ceiling(_halfWidth)) :
                         Math.Max(1, (int)Math.Floor(_halfWidth));
        }

        EvaluateBaselineWithConstantHalfWidth(xArray, srcY, tmpY, w, result);
      }
    }

    /// <summary>
    /// Executes the algorithm to estimate the baseline for the provided spectrum.
    /// This method is specialized for (almost) equally spaced x-values; thus the half width can be given in points.
    /// </summary>
    /// <param name="x">The x-values of the spectrum.</param>
    /// <param name="srcY">The y-values of the spectrum.</param>
    /// <param name="tmpY">A temporary working array of the same length as <paramref name="srcY"/>.</param>
    /// <param name="w">The half width in points. The value must be greater than or equal to 1.</param>
    /// <param name="result">The destination span to which the estimated baseline is written.</param>
    protected virtual void EvaluateBaselineWithConstantHalfWidth(ReadOnlySpan<double> x, double[] srcY, double[] tmpY, int w, Span<double> result)
    {
      int last = srcY.Length - 1;

      for (int iStage = _numberOfRegularStages - 1; ; --iStage)
      {
        if (iStage < 0)
        {
          w = Math.Min(w - 1, (int)(w / sqrt2));
          if (w < 1)
          {
            break;
          }
        }

        for (int i = 0; i <= last; i++)
        {
          var iLeft = i - w;
          var iRight = i + w;
          if (iLeft >= 0 && iRight <= last)
          {
            var xspan = x[iRight] - x[iLeft];
            var yMid = srcY[iLeft] * (x[iRight] - x[i]) / xspan + srcY[iRight] * (x[i] - x[iLeft]) / xspan;
            tmpY[i] = Math.Min(yMid, srcY[i]);
          }
          else
          {
            tmpY[i] = srcY[i];
          }
        }

        (tmpY, srcY) = (srcY, tmpY);
      }

      srcY.CopyTo(result);
    }

    /// <summary>
    /// Executes the algorithm using a locally calculated half width.
    /// This method is specialized for non-uniformly spaced x-values and a half width specified in x-units.
    /// The half width in points is calculated individually for each point.
    /// </summary>
    /// <param name="x">The x-values of the spectrum.</param>
    /// <param name="srcY">The y-values of the spectrum.</param>
    /// <param name="tmpY">A temporary working array of the same length as <paramref name="srcY"/>.</param>
    /// <param name="result">The destination span to which the estimated baseline is written.</param>
    protected virtual void EvaluateBaselineWithLocalHalfWidth(ReadOnlySpan<double> x, double[] srcY, double[] tmpY, Span<double> result)
    {
      int last = srcY.Length - 1;
      var w = new (int left, int right)[srcY.Length];

      var halfWidth = HalfWidth;
      var wmax = _roundUp ? CalculateHalfWidthInPointsLocallyRoundUp(x, HalfWidth, w) :
                            CalculateHalfWidthInPointsLocallyRoundDown(x, HalfWidth, w);

      var prev_wmax = int.MaxValue;
      var prev_prev_wmax = int.MaxValue;
      var prev_prev_prev_wmax = int.MaxValue;

      for (int iStage = _numberOfRegularStages - 1; ; --iStage)
      {
        if (iStage < 0)
        {
          halfWidth /= sqrt2;
          wmax = _roundUp ? CalculateHalfWidthInPointsLocallyRoundUp(x, halfWidth, w) :
                            CalculateHalfWidthInPointsLocallyRoundDown(x, halfWidth, w);
        }

        for (int i = 0; i <= last; i++)
        {
          var iLeft = i - w[i].left;
          var iRight = i + w[i].right;
          if (iLeft >= 0 && iRight <= last)
          {
            var xspan = x[iRight] - x[iLeft];
            var yMid = srcY[iLeft] * (x[iRight] - x[i]) / xspan + srcY[iRight] * (x[i] - x[iLeft]) / xspan;
            tmpY[i] = Math.Min(yMid, srcY[i]);
          }
          else
          {
            tmpY[i] = srcY[i];
          }
        }

        // the comparison with previous wmax values is because for input data that contain multiple values with the same x-value,
        // the wmax value does not drop down to 1, but to the number of same x-values.
        if (iStage < 0 && (wmax <= 1 || (wmax == prev_wmax && wmax == prev_prev_wmax && wmax == prev_prev_prev_wmax)))
          break;
        prev_prev_prev_wmax = prev_prev_wmax;
        prev_prev_wmax = prev_wmax;
        prev_wmax = wmax;

        (tmpY, srcY) = (srcY, tmpY);
      }

      srcY.CopyTo(result);
    }



    /// <summary>
    /// Given the half width in x-axis units, calculates the half width in points (to the left and right) for every point in <paramref name="x"/>.
    /// </summary>
    /// <param name="x">The array of x-values.</param>
    /// <param name="halfWidthInXUnits">The half width in x-units.</param>
    /// <param name="w">
    /// On return, contains the half width in points for every point in <paramref name="x"/>.
    /// The half width is given to the left and to the right of each point.
    /// </param>
    /// <returns>The maximum half width (left or right) over all points.</returns>
    public static int CalculateHalfWidthInPointsLocallyRoundUp(ReadOnlySpan<double> x, double halfWidthInXUnits, (int left, int right)[] w)
    {
      int hmax = 1;
      int hleft = 1, hright = 1;
      for (int i = x.Length - 1; i >= 0; i--, hleft--, hright++)
      {
        for (; hright > 1 && Math.Abs(x[i + hright - 1] - x[i]) >= halfWidthInXUnits; --hright) ;
        for (; hleft <= i && Math.Abs(x[i - hleft] - x[i]) < halfWidthInXUnits; ++hleft) ;
        hmax = Math.Max(hmax, Math.Max(hleft, hright));
        w[i] = (hleft, hright);
      }
      return hmax;
    }

    /// <summary>
    /// Given the half width in x-axis units, calculates the half width in points (to the left and right) for every point in <paramref name="x"/>.
    /// </summary>
    /// <param name="x">The array of x-values.</param>
    /// <param name="halfWidthInXUnits">The half width in x-units.</param>
    /// <param name="w">
    /// On return, contains the half width in points for every point in <paramref name="x"/>.
    /// The half width is given to the left and to the right of each point.
    /// </param>
    /// <returns>The maximum half width (left or right) over all points.</returns>
    public static int CalculateHalfWidthInPointsLocallyRoundDown(ReadOnlySpan<double> x, double halfWidthInXUnits, (int left, int right)[] w)
    {
      // Find a hypothetical point to the left and to the right
      double leftNextX = double.NaN;
      double rightNextX = double.NaN;
      for (int i = 1; i < x.Length; i++)
      {
        if (Math.Abs(x[i] - x[0]) > 0)
        {
          leftNextX = x[0] - (x[i] - x[0]) / i;
        }
      }
      for (int i = x.Length - 2; i >= 0; --i)
      {
        if (Math.Abs(x[i] - x[^1]) > 0)
        {
          rightNextX = x[^1] - (x[i] - x[^1]) / (x.Length - 1 - i);
        }
      }

      if (double.IsNaN(leftNextX) || double.IsNaN(rightNextX))
        throw new InvalidOperationException("The x-array does not contain varying values");



      int hmax = 1;
      int hleft = 1, hright = 1;
      for (int i = x.Length - 1, j = 0; i >= 0; i--, j++, hleft--, hright++)
      {
        for (; hright > 0 && Math.Abs((hright > j ? rightNextX : x[i + hright]) - x[i]) > halfWidthInXUnits; --hright) ;
        for (; hleft <= i && Math.Abs((hleft >= i ? leftNextX : x[i - hleft - 1]) - x[i]) <= halfWidthInXUnits; ++hleft) ;
        hright = Math.Max(1, hright);
        hleft = Math.Max(1, hleft);
        hmax = Math.Max(hmax, Math.Max(hleft, hright));
        w[i] = (hleft, hright);
      }


      return hmax;
    }

    /// <summary>
    /// Gets the statistics of the absolute distance between two subsequent points in the provided array.
    /// </summary>
    /// <param name="x">The x array.</param>
    /// <returns>The statistics (mean, average, min, max) of the distance (e.g. Math.Abs(x[i+1]-x[i])) of the points in the array.</returns>
    public static QuickStatistics GetStatisticsOfInterPointDistance(ReadOnlySpan<double> x)
    {
      var statistics = new QuickStatistics();
      for (int i = x.Length - 2; i >= 0; i--)
      {
        statistics.Add(Math.Abs(x[i] - x[i + 1]));
      }
      return statistics;
    }
  }
}
