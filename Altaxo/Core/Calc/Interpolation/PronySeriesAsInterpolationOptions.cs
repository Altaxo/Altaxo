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

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Options for a polynomial regression used as interpolation method (<see cref=PronySeriesRetardationAsInterpolationOptions"/>).
  /// </summary>
  public record PronySeriesAsInterpolationOptions : IInterpolationFunctionOptions
  {
    bool _isMinMaxAutomatic;
    double _xmin, _xmax;
    int _numberOfPoints;
    double _pointsPerDecade = 1;

    public bool IsRelaxation { get; init; }

    public bool AllowNegativePronyCoefficients { get; init; }

    public bool UseIntercept { get; init; }

    public double RegularizationParameter { get; init; }

    #region Serialization

    /// <summary>
    /// 2023-06-16 initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PronySeriesAsInterpolationOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PronySeriesAsInterpolationOptions)obj;

        info.AddValue("IsRelaxation", s.IsRelaxation);
        info.AddValue("UseIntercept", s.UseIntercept);
        info.AddValue("AllowNegativePronyCoefficients", s.AllowNegativePronyCoefficients);
        info.AddValue("RegularizationParameter", s.RegularizationParameter);
        info.AddValue("NumberOfPoints", s._numberOfPoints);
        info.AddValue("PointsPerDecade", s._pointsPerDecade);
        info.AddValue("IsMinMaxAutomatic", s._isMinMaxAutomatic);

        if (!s._isMinMaxAutomatic)
        {
          info.AddValue("XMin", s._xmin);
          info.AddValue("XMax", s._xmax);
        }
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var isRelaxation = info.GetBoolean("IsRelaxation");
        var useIntercept = info.GetBoolean("UseIntercept");
        var allowNegCoef = info.GetBoolean("AllowNegativePronyCoefficients");
        var regularization = info.GetDouble("RegularizationParameter");
        var numberOfPoints = info.GetInt32("NumberOfPoints");
        var pointsPerDecade = info.GetDouble("PointsPerDecade");
        var isMinMaxAutomatic = info.GetBoolean("IsMinMaxAutomatic");
        double xmin = double.NaN, xmax = double.NaN;

        if (!isMinMaxAutomatic)
        {
          xmin = info.GetDouble("XMin");
          xmax = info.GetDouble("XMax");
        }

        return new PronySeriesAsInterpolationOptions()
        {
          IsRelaxation = isRelaxation,
          UseIntercept = useIntercept,
          AllowNegativePronyCoefficients = allowNegCoef,
          RegularizationParameter = regularization,
          NumberOfPoints = numberOfPoints,
          _pointsPerDecade = pointsPerDecade,
          _isMinMaxAutomatic = isMinMaxAutomatic,
          XMinimum = xmin,
          XMaximum = xmax,
        };
      }
    }

    #endregion


    public PronySeriesAsInterpolationOptions WithSpecifiedMinMaxAndNumberOfPoints(double xmin, double xmax, int numberOfPoints)
    {
      return this with
      {
        _isMinMaxAutomatic = false,
        _xmin = xmin,
        _xmax = xmax,
        _numberOfPoints = numberOfPoints,
        _pointsPerDecade = 0,
      };
    }

    public PronySeriesAsInterpolationOptions RetardationWithAutomaticMinMaxAndNumberPerDecade(double pointsPerDecade)
    {
      return this with
      {
        _isMinMaxAutomatic = true,
        _pointsPerDecade = pointsPerDecade,
        _xmin = _xmax = double.NaN,
        _numberOfPoints = int.MaxValue,
      };
    }

    public PronySeriesAsInterpolationOptions WithAutomaticMinMaxAndNumberOfPoints(int numberOfPoints)
    {
      return this with
      {
        _isMinMaxAutomatic = true,
        _numberOfPoints = numberOfPoints,
        _pointsPerDecade = 0,
        _xmin = _xmax = double.NaN,
      };
    }

    public PronySeriesAsInterpolationOptions WithAutomaticMinMaxAndPointsPerDecadeAndMaxNumberOfPoints(double pointsPerDecade, int numberOfPoints)
    {
      return this with
      {
        _isMinMaxAutomatic = true,
        _numberOfPoints = numberOfPoints,
        _pointsPerDecade = pointsPerDecade,
        _xmin = _xmax = double.NaN,
      };
    }



    public double XMinimum
    {
      get => _xmin;
      set
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException(nameof(XMinimum));
        _xmin = value;
      }
    }

    public double XMaximum
    {
      get => _xmax;
      set
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException(nameof(XMaximum));
        _xmax = value;
      }
    }

    public int NumberOfPoints
    {
      get => _numberOfPoints;
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException(nameof(NumberOfPoints));
        _numberOfPoints = value;
      }
    }

    /// <inheritdoc/>
    public IInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev = null)
    {


      var pr = _isMinMaxAutomatic ?
                new PronySeriesTimeDomainAsInterpolation(_numberOfPoints, _pointsPerDecade, isRelaxation: IsRelaxation, allowNegativePronyCoefficients: AllowNegativePronyCoefficients, useIntercept: UseIntercept, regularizationParameter: RegularizationParameter) :
                new PronySeriesTimeDomainAsInterpolation(_xmin, _xmax, _numberOfPoints, isRelaxation: IsRelaxation, allowNegativePronyCoefficients: AllowNegativePronyCoefficients, useIntercept: UseIntercept, regularizationParameter: RegularizationParameter);
      pr.Interpolate(xvec, yvec);
      return pr;
    }

    IInterpolationCurve IInterpolationCurveOptions.Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev)
    {
      return Interpolate(xvec, yvec, yStdDev);
    }
  }
}
