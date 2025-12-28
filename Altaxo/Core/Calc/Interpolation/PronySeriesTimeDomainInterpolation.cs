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

using System.Collections.Generic;
using Altaxo.Science.Signals;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Interpolation with a sum of Prony terms in the time domain, either a relaxation (a time-decreasing function, e.g. a time-dependent modulus),
  /// or a retardation (a time-increasing function, e.g. a time-dependent compliance).
  /// </summary>
  public record PronySeriesTimeDomainInterpolation : PronySeriesInterpolationBase, IInterpolationFunctionOptions
  {
    #region Serialization

    /// <summary>
    /// Serialization surrogate.
    /// </summary>
    /// <remarks>
    /// 2023-06-16 V0: initial version was <c>Altaxo.Calc.Interpolation.PronySeriesAsInterpolationOptions</c> (AltaxoCore).
    /// 2024-02-01 V1.
    /// </remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Calc.Interpolation.PronySeriesAsInterpolationOptions", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PronySeriesTimeDomainInterpolation), 1)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PronySeriesTimeDomainInterpolation)obj;

        info.AddValue("IsRelaxation", s.IsRelaxation);
        info.AddValue("UseIntercept", s.UseIntercept);
        info.AddValue("AllowNegativePronyCoefficients", s.AllowNegativePronyCoefficients);
        info.AddValue("RegularizationParameter", s.RegularizationParameter);
        info.AddValue("NumberOfPoints", s.NumberOfPoints);
        info.AddValue("PointsPerDecade", s.PointsPerDecade);
        info.AddValue("IsMinMaxAutomatic", !s.XMinimumMaximum.HasValue);

        if (s.XMinimumMaximum.HasValue)
        {
          info.AddValue("XMin", s.XMinimumMaximum.Value.xMinimum);
          info.AddValue("XMax", s.XMinimumMaximum.Value.xMaximum);
        }
      }

      /// <inheritdoc/>
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

        return new PronySeriesTimeDomainInterpolation()
        {
          IsRelaxation = isRelaxation,
          UseIntercept = useIntercept,
          AllowNegativePronyCoefficients = allowNegCoef,
          RegularizationParameter = regularization,
          NumberOfPoints = numberOfPoints,
          PointsPerDecade = pointsPerDecade,
          XMinimumMaximum = isMinMaxAutomatic ? null : (xmin, xmax),
        };
      }
    }

    #endregion

    /// <inheritdoc/>
    public IInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev = null)
    {
      var (workingXMinimum, workingXMaximum, workingNumberOfPoints) = GetWorkingXMinMaxNumberOfPoints(xvec);

      if (IsRelaxation)
      {
        var fit = PronySeriesRelaxation.EvaluateTimeDomain(xvec, yvec, workingXMinimum, workingXMaximum, workingNumberOfPoints, withIntercept: UseIntercept, regularizationLambda: 0, allowNegativeCoefficients: AllowNegativePronyCoefficients);
        return new InterpolationResultDoubleWrapper(fit.GetTimeDomainYOfTime);
      }
      else
      {
        var fit = PronySeriesRetardation.EvaluateTimeDomain(xvec, yvec, workingXMinimum, workingXMaximum, workingNumberOfPoints, withIntercept: UseIntercept, withFlowTerm: false, regularizationLambda: 0, allowNegativeCoefficients: AllowNegativePronyCoefficients);
        return new InterpolationResultDoubleWrapper(fit.GetTimeDomainYOfTime);
      }
    }

    /// <inheritdoc/>
    IInterpolationCurve IInterpolationCurveOptions.Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev)
    {
      return Interpolate(xvec, yvec, yStdDev);
    }
  }
}
