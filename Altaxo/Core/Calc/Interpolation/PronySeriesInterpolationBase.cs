#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using Complex64 = System.Numerics.Complex;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Base class of the options for Prony series interpolation, both in the time domain as well as in the frequency domain.
  /// </summary>
  public record PronySeriesInterpolationBase
  {
    /// <summary>
    /// Gets the minimum and maximum x values to be used. If this property is null, then the minimum and maximum x is determined automatically.
    /// In time domain, the x values are times. In frequency domain, the x values are frequencies (frequencies, not circular frequencies!).
    /// </summary>
    public (double xMinimum, double xMaximum)? XMinimumMaximum { get; init; }

    /// <summary>
    /// If <see cref="PointsPerDecade"/> is 0, this property specifies a fixed number of Prony terms.
    /// Else, if <see cref="PointsPerDecade"/> is &gt; 0, this property specifies the maximum number of Prony terms.
    /// </summary>
    public int NumberOfPoints { get; init; } = int.MaxValue;

    /// <summary>
    /// Gets the number of Prony terms per decade. If this value is &lt;=0, the property <see cref="NumberOfPoints"/> specifiy a fixed number of Prony terms.
    /// Else, if this property is &gt; 0, it specify the number of Prony terms per decade, and <see cref="NumberOfPoints"/> only specifies the maximum number of Prony terms.
    /// </summary>
    public double PointsPerDecade { get; init; } = 2;

    /// <summary>
    /// If true, the Prony terms model a relaxation process, i.e. a modulus, where the real part increases with frequency.
    /// If false, the Prony terms model a retardation process, i.e. a susceptibility, where the real part decreases with frequency.
    /// </summary>
    public bool IsRelaxation { get; init; }

    /// <summary>
    /// If true, besides of the Prony terms, additionally an intercept is fitted to the data.
    /// </summary>
    public bool UseIntercept { get; init; }

    /// <summary>
    /// If true, also negative Prony coefficients are allowed. The default value is false. 
    /// </summary>
    public bool AllowNegativePronyCoefficients { get; init; }

    /// <summary>
    /// Gets /sets the regularization parameter that controls the smoothing
    /// of the resulting curve. The higher the parameter, the smoother the resulting curve will be.
    /// </summary>
    public double RegularizationParameter { get; init; }

    public PronySeriesInterpolationBase WithSpecifiedXMinimumMaximumAndFixedNumberOfPoints(double xmin, double xmax, int numberOfPoints)
    {
      return this with
      {
        XMinimumMaximum = (xmin, xmax),
        NumberOfPoints = numberOfPoints,
        PointsPerDecade = 0,
      };
    }

    public PronySeriesInterpolationBase RetardationWithAutomaticXMinimumMaximumAndNumberOfPointsPerDecade(double pointsPerDecade)
    {
      if (!(pointsPerDecade > 0))
        throw new ArgumentOutOfRangeException(nameof(pointsPerDecade));

      return this with
      {
        XMinimumMaximum = null,
        NumberOfPoints = int.MaxValue,
        PointsPerDecade = pointsPerDecade,
      };
    }

    public PronySeriesInterpolationBase WithAutomaticXMinimumMaximumAndFixedNumberOfPoints(int numberOfPoints)
    {
      return this with
      {
        XMinimumMaximum = null,
        NumberOfPoints = numberOfPoints,
        PointsPerDecade = 0,
      };
    }

    public PronySeriesInterpolationBase WithAutomaticXMinimumMaximumAndNumberOfPointsPerDecadeAndMaximumNumberOfPoints(double pointsPerDecade, int numberOfPoints)
    {
      return this with
      {
        XMinimumMaximum = null,
        NumberOfPoints = numberOfPoints,
        PointsPerDecade = pointsPerDecade,
      };
    }

    protected (double workingXMinimum, double workingXMaximum, int workingNumberOfPoints) GetWorkingXMinMaxNumberOfPoints(IReadOnlyList<double> xvec)
    {
      double workingXMinimum, workingXMaximum;
      var workingNumberOfPoints = NumberOfPoints;

      if (XMinimumMaximum is { } minmax)
      {
        workingXMinimum = minmax.xMinimum;
        workingXMaximum = minmax.xMaximum;

        if (!(workingXMaximum > workingXMinimum))
          throw new InvalidOperationException($"Error with manually fixed x-minimum/x-maximum values of Prony interpolation. X-max should be greater than x-min, but is: xmin={workingXMinimum}, xmax={workingXMaximum}");
      }
      else // min max done automatically
      {
        var xmin = xvec.Min(x => x > 0 ? x : null);
        if (!xmin.HasValue)
          throw new ArgumentException("Prony interpolation: the x-array does not contain positive elements", nameof(xvec));

        var xmax = xvec.Max(x => x > 0 ? x : null);
        if (!xmax.HasValue)
          throw new ArgumentException("Prony interpolation: The x-array does not contain positive elements", nameof(xvec));

        if (xmin.Value == xmax.Value)
        {
          workingXMinimum = workingXMaximum = xmin.Value;
          workingNumberOfPoints = 1;
        }
        else
        {
          workingXMinimum = xmin.Value;
          workingXMaximum = xmax.Value;
          if (PointsPerDecade > 0)
          {
            workingNumberOfPoints = (int)(Math.Ceiling(Math.Log10(xmax.Value / xmin.Value) * PointsPerDecade) + 1);
            workingNumberOfPoints = Math.Min(workingNumberOfPoints, NumberOfPoints);
          }
        }
      }

      return (workingXMinimum, workingXMaximum, workingNumberOfPoints);
    }


    protected class InterpolationResultDoubleWrapper : IInterpolationFunction
    {
      Func<double, double> YOfX { get; }

      public InterpolationResultDoubleWrapper(Func<double, double> yOfX)
      {
        YOfX = yOfX;
      }

      public double GetXOfU(double u)
      {
        return u;
      }

      public double GetYOfU(double u)
      {
        return YOfX(u);
      }

      public double GetYOfX(double x)
      {
        return YOfX(x);
      }

      public void Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec)
      {
        throw new NotImplementedException();
      }
    }
    protected class InterpolationResultComplexWrapper : IComplexInterpolationFunction
    {
      Func<double, Complex64> YOfX { get; }

      public InterpolationResultComplexWrapper(Func<double, Complex64> yOfX)
      {
        YOfX = yOfX;
      }

      public double GetXOfU(double u)
      {
        return u;
      }

      public Complex64 GetYOfU(double u)
      {
        return YOfX(u);
      }

      public Complex64 GetYOfX(double x)
      {
        return YOfX(x);
      }

      public void Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec)
      {
        throw new NotImplementedException();
      }
    }
  }
}
