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
  public abstract record SNIP_Base
  {
    protected const double sqrt2 = 1.41421356237;
    protected const bool _roundUp = false;

    protected double _halfWidth = 15;
    protected bool _isHalfWidthInXUnits;
    protected int _numberOfRegularStages = 40;

    /// <summary>
    /// Half of the width of the averaging window. This value should be set to
    /// roughly the FWHM (full width half maximum) of the broadest peak in the spectrum.
    /// </summary>
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
    /// Gets a value indicating whether the half width is given in x-axis units.
    /// </summary>
    /// <value>If true, the <see cref="HalfWidth"/> value is given in x-axis units; otherwise (false) the <see cref="HalfWidth"/> is given in points.</value>
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
    /// Gets or sets the number of regular iterations. Default is 40.
    /// </summary>
    /// <value>
    /// The number of regular iterations.
    /// </value>
    /// <exception cref="System.ArgumentOutOfRangeException">Number of iterations must be at least one. - NumberOfRegularStages</exception>
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
    /// Executes the algorithm with the provided spectrum.
    /// </summary>
    /// <param name="xArray">The x values of the spectral values.</param>
    /// <param name="yArray">The array of spectral values.</param>
    /// <param name="result">The location where the baseline corrected spectrum should be stored.</param>
    /// <returns>The evaluated background of the provided spectrum.</returns>
    public virtual void Execute(ReadOnlySpan<double> xArray, ReadOnlySpan<double> yArray, Span<double> result)
    {
      var srcY = new double[yArray.Length];
      var tmpY = new double[yArray.Length];
      yArray.CopyTo(srcY);

      var stat = GetStatisticsOfInterPointDistance(xArray);
      if (_isHalfWidthInXUnits && 0.5 * (stat.Max - stat.Min) / stat.Max > 1.0 / xArray.Length)
      {
        // if the interpoint distant is not uniform, we need to use the algorithm with locally calculated half width
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
    /// Executes the algorithm to find the baseline with the provided spectrum. This method is specialized for (almost) equally spaced x-values, thus the half width can be given in points.
    /// </summary>
    /// <param name="x">The x values of the spectral values.</param>
    /// <param name="srcY">The array of spectral values.</param>
    /// <param name="tmpY">A temporary working array of the same length than <paramref name="srcY"/>.</param>
    /// <param name="w">The half width in points. The value must be equal to or greater than 1.</param>
    /// <param name="result">The location where the baseline corrected spectrum should be stored.</param>
    /// <returns>The evaluated baseline of the provided spectrum.</returns>
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
    /// Executes the algorithm with the provided spectrum. This method is specialized for not equally spaced x-values, and the half width given in x-units.
    /// The half width in points is calculated for each point individually.
    /// </summary>
    /// <param name="x">The x values of the spectral values.</param>
    /// <param name="srcY">The array of spectral values.</param>
    /// <param name="tmpY">A temporary working array of the same length than <paramref name="srcY"/>.</param>
    /// <param name="result">The location where the baseline corrected spectrum should be stored.</param>
    /// <returns>The evaluated background of the provided spectrum.</returns>
    protected virtual void EvaluateBaselineWithLocalHalfWidth(ReadOnlySpan<double> x, double[] srcY, double[] tmpY, Span<double> result)
    {
      int last = srcY.Length - 1;
      var w = new (int left, int right)[srcY.Length];

      var halfWidth = HalfWidth;
      var wmax = _roundUp ? CalculateHalfWidthInPointsLocallyRoundUp(x, HalfWidth, w) :
                            CalculateHalfWidthInPointsLocallyRoundDown(x, HalfWidth, w);

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

        if (iStage < 0 && wmax <= 1)
          break;

        (tmpY, srcY) = (srcY, tmpY);
      }

      srcY.CopyTo(result);
    }



    /// <summary>
    /// Given the half width in x-axis unit, the half width in points (to the left and right) is calculated for every point in array x.
    /// </summary>
    /// <param name="x">The array of x-values.</param>
    /// <param name="halfWidthInXUnits">The half width in x units.</param>
    /// <param name="w">On returns, contains the half width in points for every point in array x. The half width is given to the left and to the right of each point.</param>
    /// <returns>The maximal half width (left and right) of all points.</returns>
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
    /// Given the half width in x-axis unit, the half width in points (to the left and right) is calculated for every point in array x.
    /// </summary>
    /// <param name="x">The array of x-values.</param>
    /// <param name="halfWidthInXUnits">The half width in x units.</param>
    /// <param name="w">On returns, contains the half width in points for every point in array x. The half width is given to the left and to the right of each point.</param>
    /// <returns>The maximal half width (left and right) of all points.</returns>
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
