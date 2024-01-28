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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Science.Signals;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Base class of the options for Prony series interpolation.
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


    protected PronySeriesFrequencyDomainAsInterpolation GetInterpolation()
    {
      PronySeriesFrequencyDomainAsInterpolation result;

      if (XMinimumMaximum.HasValue)
      {
        result = new PronySeriesFrequencyDomainAsInterpolation(XMinimumMaximum.Value.xMinimum, XMinimumMaximum.Value.xMaximum, NumberOfPoints, IsRelaxation, AllowNegativePronyCoefficients, UseIntercept, RegularizationParameter);
      }
      else
      {
        result = new PronySeriesFrequencyDomainAsInterpolation(NumberOfPoints, PointsPerDecade, IsRelaxation, AllowNegativePronyCoefficients, UseIntercept, RegularizationParameter);
      }
      return result;
    }

  }

  public record PronySeriesComplexInterpolation : PronySeriesInterpolationBase, IComplexInterpolation
  {
    public IComplexInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<Complex> yvec, IReadOnlyList<Complex>? yStdDev = null)
    {
      var result = GetInterpolation();
      result.Interpolate(xvec, yvec.Select(x => x.Real).ToArray(), yvec.Select(y => y.Imaginary).ToArray());
      return result;
    }

    public IComplexInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yreal, IReadOnlyList<double> yimaginary)
    {
      var result = GetInterpolation();
      result.Interpolate(xvec, yreal, yimaginary);
      return result;

    }
  }

  public class PronySeriesFrequencyDomainAsInterpolation : IInterpolationFunction, IComplexInterpolationFunction
  {
    bool _isRelaxation;
    bool _isMinMaxAutomatic;
    double _xMinimum, _xMaximum;
    int _numberOfPoints;
    double _pointsPerDecade = 1;
    public bool IsRelaxation { get; }

    public bool UseIntercept { get; }

    public double RegularizationParameter { get; }

    private object? _fit;


    public PronySeriesFrequencyDomainAsInterpolation()
    {
    }

    public PronySeriesFrequencyDomainAsInterpolation(double xmin, double xmax, int numberOfPoints, bool isRelaxation, bool allowNegativePronyCoefficients, bool useIntercept, double regularizationParameter)
    {
      if (!(xmin > 0))
        throw new ArgumentOutOfRangeException("Must be >=0", nameof(xmin));

      if (!(xmax >= xmin))
        throw new ArgumentOutOfRangeException("Must be > xmin", nameof(xmin));

      if (numberOfPoints == 1 && !(xmin == xmax))
        throw new ArgumentOutOfRangeException("If the number of points is 1, xmin must be equal to xmax");

      if (!(numberOfPoints > 0))
        throw new ArgumentOutOfRangeException("Must be > 0", nameof(numberOfPoints));

      _isMinMaxAutomatic = false;
      _isRelaxation = isRelaxation;
      UseIntercept = useIntercept;
      RegularizationParameter = regularizationParameter;
      _xMinimum = xmin;
      _xMaximum = xmax;
      _numberOfPoints = numberOfPoints;

    }

    public PronySeriesFrequencyDomainAsInterpolation(int numberOfPoints, double pointsPerDecade, bool isRelaxation, bool allowNegativePronyCoefficients, bool useIntercept, double regularizationParameter)
    {
      if (!(pointsPerDecade > 0))
        throw new ArgumentOutOfRangeException("Must be >=0", nameof(pointsPerDecade));

      _isMinMaxAutomatic = true;
      _isRelaxation = isRelaxation;
      UseIntercept = useIntercept;
      RegularizationParameter = regularizationParameter;
      _numberOfPoints = numberOfPoints;
      _pointsPerDecade = pointsPerDecade;
    }

    public void Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec)
    {
      Interpolate(xvec, yvec, null);
    }

    public void Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yimvec)
    {
      var workingXMinimum = _xMinimum;
      var workingXMaximum = _xMaximum;
      var workingNumberOfPoints = _numberOfPoints;

      if (_isMinMaxAutomatic)
      {
        var xmin = xvec.Min(x => x > 0 ? x : null);
        if (!xmin.HasValue)
          throw new ArgumentException("The array does not contain positive elements", nameof(xvec));

        var xmax = xvec.Max(x => x > 0 ? x : null);
        if (!xmax.HasValue)
          throw new ArgumentException("The array does not contain positive elements", nameof(xvec));

        if (xmin.Value == xmax.Value)
        {
          workingXMinimum = workingXMaximum = xmin.Value;
          workingNumberOfPoints = 1;
        }
        else
        {
          workingXMinimum = xmin.Value;
          workingXMaximum = xmax.Value;
          if (_pointsPerDecade > 0)
          {
            workingNumberOfPoints = (int)(Math.Ceiling(Math.Log10(xmax.Value / xmin.Value) / _pointsPerDecade) + 1);
            workingNumberOfPoints = Math.Min(workingNumberOfPoints, _numberOfPoints);
          }
        }
      }

      if (_isRelaxation)
      {
        _fit = PronySeriesRelaxation.EvaluateFrequencyDomain(
          xvec,
          isCircularFrequency: false,
          yvec,
           yimvec,
          workingXMinimum,
          workingXMaximum,
          workingNumberOfPoints,
          withIntercept: UseIntercept,
          regularizationLambda: 0
          );
      }
      else
      {
        _fit = PronySeriesRetardation.EvaluateFrequencyDomain(
          xvec,
          isCircularFrequency: false,
          yvec,
          yarrIm: yimvec,
          workingXMinimum,
          workingXMaximum,
          workingNumberOfPoints,
          withIntercept: UseIntercept,
          withFlowTerm: false,
          isRelativePermittivitySpectrum: false,
          regularizationLambda: 0);
      }
    }

    public double GetYOfX(double x)
    {
      return GetYRealOfX(x);
    }

    public double GetYRealOfX(double x)
    {
      if (_fit is null)
        throw new InvalidOperationException($"Results not available yet - please execute an interpolation first");

      return _isRelaxation ?
             ((PronySeriesRelaxationResult)_fit).GetFrequencyDomainYOfFrequency(x).Real :
             ((PronySeriesRetardationResult)_fit).GetFrequencyDomainYOfFrequency(x).Real;
    }

    public double GetYImaginaryOfX(double x)
    {
      if (_fit is null)
        throw new InvalidOperationException($"Results not available yet - please execute an interpolation first");

      return _isRelaxation ?
             ((PronySeriesRelaxationResult)_fit).GetFrequencyDomainYOfFrequency(x).Imaginary :
             ((PronySeriesRetardationResult)_fit).GetFrequencyDomainYOfFrequency(x).Imaginary;
    }




    public double GetYOfU(double u)
    {
      return GetYOfX(u);
    }

    public double GetXOfU(double u)
    {
      return u;
    }

    Complex IComplexInterpolationFunction.GetYOfX(double x)
    {
      if (_fit is null)
        throw new InvalidOperationException($"Results not available yet - please execute an interpolation first");

      return _isRelaxation ?
             ((PronySeriesRelaxationResult)_fit).GetFrequencyDomainYOfFrequency(x) :
             ((PronySeriesRetardationResult)_fit).GetFrequencyDomainYOfFrequency(x);
    }
  }
}
