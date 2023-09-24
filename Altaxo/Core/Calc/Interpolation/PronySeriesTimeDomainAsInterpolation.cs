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
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Science.Signals;

namespace Altaxo.Calc.Interpolation
{
  public class PronySeriesTimeDomainAsInterpolation : IInterpolationFunction
  {
    bool _isRelaxation;
    bool _isMinMaxAutomatic;
    double _xMinimum, _xMaximum;
    int _numberOfPoints;
    double _pointsPerDecade = 1;
    public bool IsRelaxation { get; }
    public bool AllowNegativePronyCoefficients { get; }

    public bool UseIntercept { get; }

    public double RegularizationParameter { get; }

    private object? _fit;


    public PronySeriesTimeDomainAsInterpolation()
    {
    }

    public PronySeriesTimeDomainAsInterpolation(double xmin, double xmax, int numberOfPoints, bool isRelaxation, bool allowNegativePronyCoefficients, bool useIntercept, double regularizationParameter)
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
      AllowNegativePronyCoefficients = allowNegativePronyCoefficients;
      UseIntercept = useIntercept;
      RegularizationParameter = regularizationParameter;
      _xMinimum = xmin;
      _xMaximum = xmax;
      _numberOfPoints = numberOfPoints;

    }

    public PronySeriesTimeDomainAsInterpolation(int numberOfPoints, double pointsPerDecade, bool isRelaxation, bool allowNegativePronyCoefficients, bool useIntercept, double regularizationParameter)
    {
      if (!(pointsPerDecade > 0))
        throw new ArgumentOutOfRangeException("Must be >=0", nameof(pointsPerDecade));

      _isMinMaxAutomatic = true;
      _isRelaxation = isRelaxation;
      AllowNegativePronyCoefficients = allowNegativePronyCoefficients;
      UseIntercept = useIntercept;
      RegularizationParameter = regularizationParameter;
      _numberOfPoints = numberOfPoints;
      _pointsPerDecade = pointsPerDecade;
    }

    public void Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec)
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
        _fit = PronySeriesRelaxation.EvaluateTimeDomain(xvec, yvec, workingXMinimum, workingXMaximum, workingNumberOfPoints, withIntercept: UseIntercept, regularizationLambda: 0, allowNegativeCoefficients: AllowNegativePronyCoefficients);
      }
      else
      {
        _fit = PronySeriesRetardation.EvaluateTimeDomain(xvec, yvec, workingXMinimum, workingXMaximum, workingNumberOfPoints, withIntercept: UseIntercept, withFlowTerm: false, regularizationLambda: 0, allowNegativeCoefficients: AllowNegativePronyCoefficients);
      }
    }

    public double GetYOfX(double x)
    {
      if (_fit is null)
        throw new InvalidOperationException($"Results not available yet - please execute an interpolation first");

      return _isRelaxation ? ((PronySeriesRelaxationResult)_fit).GetTimeDomainYOfTime(x) : ((PronySeriesRetardationResult)_fit).GetTimeDomainYOfTime(x);
    }

    public double GetYOfU(double u)
    {
      return GetYOfX(u);
    }

    public double GetXOfU(double u)
    {
      return u;
    }
  }
}
