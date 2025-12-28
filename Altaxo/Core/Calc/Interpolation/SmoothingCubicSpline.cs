#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
  /// Options for a smoothing cubic spline (<see cref="SmoothingCubicSpline"/>).
  /// </summary>
  public record SmoothingCubicSplineOptions : IInterpolationFunctionOptions
  {


    private double _errorStandardDeviation = -1;

    /// <summary>
    /// Determines how the smoothness of the spline is specified, together with the <see cref="Smoothness"/> value.
    /// </summary>
    public SmoothnessSpecification SmoothnessSpecifiedBy { get; init; } = SmoothnessSpecification.Direct;

    /// <summary>
    /// Gets the smoothness parameter.
    /// Must be in the interval [0, <see cref="double.PositiveInfinity"/>], where a value of <c>0</c> means
    /// no smoothing (evaluation of a cubic spline), while a value of <see cref="double.PositiveInfinity"/>
    /// means evaluation of a regression.
    /// </summary>
    /// <remarks>
    /// The <see cref="SmoothingCubicSplineBase.SmoothingParameter"/> is calculated by
    /// <c>SmoothingParameter = Smoothness / (1 + Smoothness)</c>.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is negative.</exception>
    public double Smoothness
    {
      get => field;
      init
      {
        if (!(value >= 0 && value <= double.PositiveInfinity))
          throw new ArgumentOutOfRangeException(nameof(Smoothness));

        field = value;
      }
    } = 1;


    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>
    /// 2022-08-14 Initial version.
    /// </remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Calc.Interpolation.SmoothingCubicSplineOptions", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SmoothingCubicSplineOptions)obj;
        info.AddValue("Smoothness", s.Smoothness);
        info.AddValue("ErrorStandardDeviation", s._errorStandardDeviation);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var smoothing = info.GetDouble("Smoothness");
        var stddev = info.GetDouble("ErrorStandardDeviation");
        return new SmoothingCubicSplineOptions() { Smoothness = smoothing, _errorStandardDeviation = stddev };
      }
    }

    /// <summary>
    /// XML serialization surrogate (version 1).
    /// </summary>
    /// <remarks>
    /// 2025-09-26 Add smoothness specification.
    /// </remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SmoothingCubicSplineOptions), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SmoothingCubicSplineOptions)obj;
        info.AddValue("Smoothness", s.Smoothness);
        info.AddEnum("SmoothnessSpecifiedBy", s.SmoothnessSpecifiedBy);
        info.AddValue("ErrorStandardDeviation", s._errorStandardDeviation);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var smoothing = info.GetDouble("Smoothness");
        var smoothnessSpec = info.GetEnum<SmoothnessSpecification>("SmoothnessSpecifiedBy");
        var stddev = info.GetDouble("ErrorStandardDeviation");
        return new SmoothingCubicSplineOptions() { Smoothness = smoothing, SmoothnessSpecifiedBy = smoothnessSpec, _errorStandardDeviation = stddev };
      }
    }

    #endregion

    /// <inheritdoc/>
    public IInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev = null)
    {
      var spline = new SmoothingCubicSpline() { Smoothness = Smoothness, SmoothnessSpecifiedBy = SmoothnessSpecifiedBy, ErrorStandardDeviation = _errorStandardDeviation, CombineNeighbouringPoints = true, };
      if (yStdDev is null)
        spline.Interpolate(xvec, yvec);
      else
        spline.Interpolate(xvec, yvec, _errorStandardDeviation, yStdDev);
      return spline;
    }

    /// <inheritdoc/>
    IInterpolationCurve IInterpolationCurveOptions.Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev)
    {
      return Interpolate(xvec, yvec, yStdDev);
    }
  }


  /// <summary>
  /// Calculates a smoothing cubic spline whose smoothness is determined by the <see cref="Smoothness"/> property.
  /// </summary>
  public class SmoothingCubicSpline : SmoothingCubicSplineBase, IInterpolationFunction
  {
    /// <summary>
    /// Gets or sets the smoothness parameter.
    /// Must be in the interval [0, <see cref="double.PositiveInfinity"/>], where a value of <c>0</c> means
    /// no smoothing (evaluation of a cubic spline), while a value of <see cref="double.PositiveInfinity"/>
    /// means evaluation of a regression.
    /// </summary>
    /// <remarks>
    /// The <see cref="SmoothingCubicSplineBase.SmoothingParameter"/> is calculated by
    /// <c>SmoothingParameter = Smoothness / (1 + Smoothness)</c>.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is negative.</exception>
    public double Smoothness
    {
      get
      {
        return field;
      }
      set
      {
        if (!(0 <= value))
          throw new ArgumentOutOfRangeException("Must be a value >=0", nameof(value));
        field = value;
      }
    } = 1;

    /// <summary>
    /// Determines how the smoothness of the spline is specified, together with the <see cref="Smoothness"/> value.
    /// </summary>
    public SmoothnessSpecification SmoothnessSpecifiedBy { get; set; } = SmoothnessSpecification.Direct;

    /// <summary>
    /// Creates an instance of <see cref="SmoothingCubicSpline"/> with a default value for <see cref="Smoothness"/> of 1.
    /// </summary>
    public SmoothingCubicSpline()
    {
      _standardDeviation = -1.0; // unknown standard deviation
    }

    /// <summary>
    /// Computes a smoothness value in internal units based on the specified interpretation.
    /// </summary>
    /// <param name="smoothnessValue">The user-specified smoothness value.</param>
    /// <param name="smoothnessSpecifiedBy">Specifies how <paramref name="smoothnessValue"/> should be interpreted.</param>
    /// <param name="x">The x-values of the data points.</param>
    /// <param name="numberOfPoints">The number of data points.</param>
    /// <returns>The modified smoothness value in internal units.</returns>
    public static double GetModifiedSmoothnessValue(double smoothnessValue, SmoothnessSpecification smoothnessSpecifiedBy, double[] x, double numberOfPoints)
    {
      double GetSmoothnessFromPointsPerPeriod(double pointsPerPeriod)
      {
        pointsPerPeriod *= pointsPerPeriod;
        pointsPerPeriod *= pointsPerPeriod;
        return 0.00219146043349633 * pointsPerPeriod;
      }

      var modifiedSmoothnessValue = smoothnessSpecifiedBy switch
      {
        SmoothnessSpecification.ByNumberOfPoints => GetSmoothnessFromPointsPerPeriod(smoothnessValue),// smoothnessValue is number of points in a feature span
        SmoothnessSpecification.ByNumberOfFeatures => GetSmoothnessFromPointsPerPeriod(numberOfPoints / smoothnessValue),// smoothnessValue is number of features
        SmoothnessSpecification.ByXSpan => GetSmoothnessFromPointsPerPeriod(smoothnessValue / Science.Signals.SignalMath.GetMeanIncrement(x)),// smoothnessValue is the x-span of one feature
        SmoothnessSpecification.Direct => smoothnessValue,// the smoothness value is used directly
        _ => throw new NotImplementedException($"SmoothnessSpecification {smoothnessSpecifiedBy} is not implemented here!")
      };
      return modifiedSmoothnessValue;
    }

    /// <inheritdoc/>
    protected override void InterpolationKernel(
      double[] x,
      double[] f,
      double[] df,
      int n,
      double[] y,
      double[][] C,
      int ic,
      double var,
      int job,
      double[] se,
      double[][] WK0,
      double[][] WK1,
      double[] WK2,
      double[] WK3,
      out int ier)
    {
      ier = 0;
      double[] stat = new double[6];
      double p, q;
      double avh;
      double avar;
      double avdf;
      double gf1;
      int i;

      // Now adjust the smoothness value according to the specification
      double r1 = GetModifiedSmoothnessValue(Smoothness, SmoothnessSpecifiedBy, x, n);

      spint1(x, out avh, f, df, out avdf, n, y, C, ic, WK0, WK1, ref ier);

      avar = var;
      if (var > 0)
        avar = var * avdf * avdf;

      /* Calculate spline coefficients */
      spfit1(x, avh, df, n, r1, out p, out q, out gf1, avar, stat, y, C, ic, WK0, WK1, WK2, WK3);
      spcof1(x, avh, f, df, n, p, q, y, C, ic, WK2, WK3);

      /* Optionally calculate standard error estimates */
      if (var < 0)
      {
        avar = stat[5];
        var = avar / (avdf * avdf);
      }
      if (job == 1)
        sperr1(x, avh, df, n, WK0, p, avar, se);

      /* Unscale df */
      for (i = 0; i < n; i++) df[i] = df[i] * avdf;

      /* Put statistics in wk */
      WK0[0][0] = stat[0];
      WK0[1][0] = stat[1];
      WK0[2][0] = stat[2];
      WK1[0][0] = stat[3];
      WK1[1][0] = stat[4];
      WK2[0] = stat[5] / (avdf * avdf);
      WK3[0] = avdf * avdf;
    }


  }
}
