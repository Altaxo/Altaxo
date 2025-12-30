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
using System.Xml;
using Altaxo.Calc;
using Altaxo.Calc.Interpolation;
using Altaxo.Science.Signals;

namespace Altaxo.Science.Spectroscopy.BaselineEstimation
{
  /// <summary>
  /// Detrends a spectrum by fitting a smoothing spline through the spectrum.
  /// Points above the spline are reduced in amplitude towards the spline, iteratively, until the curve converges.
  /// </summary>
  /// <remarks>
  /// Reference: Y. Xu, P. Du, R. Senger, J. Robertson, J. Pirkle, Applied Spectroscopy 2021 Vol 75 (1) 34-45.
  /// </remarks>
  public abstract record ISREABase : Main.IImmutable
  {
    /// <summary>
    /// Determines how the smoothness of the spline is specified, together with <see cref="SmoothnessValue"/>.
    /// </summary>
    public SmoothnessSpecification SmoothnessSpecifiedBy
    {
      get => field;
      init
      {
        if (value == SmoothnessSpecification.Direct)
          throw new ArgumentException("Direct specification of smoothness is not supported in SSProb.");

        field = value;
      }
    } = SmoothnessSpecification.ByNumberOfFeatures;

    /// <summary>
    /// Determines the smoothness of the spline.
    /// The meaning of this value depends on <see cref="SmoothnessSpecifiedBy"/>.
    /// </summary>
    public double SmoothnessValue
    {
      get => field;
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException("SmoothnessValue must be a value > 0", nameof(SmoothnessValue));

        field = value;
      }
    } = 15;

    private Altaxo.Calc.Interpolation.IInterpolationFunctionOptions _interpolationFunctionOptions = new Altaxo.Calc.Interpolation.SmoothingCubicSplineOptions();

    /// <summary>
    /// Gets the interpolation function options used to create the smoothing spline.
    /// </summary>
    public Altaxo.Calc.Interpolation.IInterpolationFunctionOptions InterpolationFunctionOptions
    {
      get => _interpolationFunctionOptions;
      init => _interpolationFunctionOptions = value ?? throw new ArgumentNullException(nameof(InterpolationFunctionOptions));
    }


    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var yBaseline = new double[y.Length];
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        var xSpan = new ArraySegment<double>(x, start, end - start);
        var ySpan = new ArraySegment<double>(y, start, end - start);
        var yBaselineSpan = new ArraySegment<double>(yBaseline, start, end - start);
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


    /// <inheritdoc/>
    public void Execute(ReadOnlySpan<double> xArray, ReadOnlySpan<double> yArray, Span<double> resultingBaseline)
    {
      double stopThreshold = 1E-4;
      stopThreshold *= stopThreshold;

      var regionlength = yArray.Length;

      var xx = xArray.ToArray();
      var yy = yArray.ToArray();
      double meanXIncrement = SignalMath.GetMeanIncrement(xx);

      var yprev = new double[yy.Length];
      var spline = GetSpline(InterpolationFunctionOptions, SmoothnessSpecifiedBy, SmoothnessValue, yArray.Length, meanXIncrement);
      IInterpolationFunction interpolation = null!;
      double sumysqr = 0;
      for (int i = 0; i < yy.Length; ++i)
      {
        sumysqr += yy[i] * yy[i];
      }
      double sumsqrdiff = sumysqr;

      for (int iteration = 0; iteration < 1000 && (sumsqrdiff / sumysqr) >= stopThreshold; ++iteration)
      {
        (yy, yprev) = (yprev, yy);
        interpolation = spline.Interpolate(xx, yprev);
        sumsqrdiff = 0;
        for (int i = 0; i < regionlength; i++)
        {
          var yspline = interpolation.GetYOfX(xArray[i]);
          var diff = yprev[i] - yspline;
          if (diff > 0)
          {
            yy[i] = yspline + Math.Sqrt(Math.Sqrt(diff));
            sumsqrdiff += RMath.Pow2(yy[i] - yprev[i]);
          }
          else
          {
            yy[i] = yprev[i];
          }
        }
      }

      // the result is the spline
      for (int i = 0; i < yy.Length; i++)
      {
        resultingBaseline[i] = interpolation.GetYOfX(xArray[i]);
      }
    }


    /// <summary>
    /// Writes this instance to XML.
    /// </summary>
    /// <param name="writer">The XML writer to write to.</param>
    public void Export(XmlWriter writer)
    {
      writer.WriteStartElement("ISREA");
      writer.WriteElementString("NumberOfKnots", XmlConvert.ToString(SmoothnessValue));
      writer.WriteEndElement();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"{this.GetType().Name} NumberOfKnots={SmoothnessValue}";
    }

    /// <summary>
    /// Gets spline interpolation options in which the smoothing amount is set according to the provided parameters.
    /// </summary>
    /// <param name="interpolation">The spline interpolation options.</param>
    /// <param name="smoothnessSpecifiedBy">Determines how the smoothness is specified.</param>
    /// <param name="smoothnessValue">The smoothness value.</param>
    /// <param name="numberOfArrayPoints">The number of points in the spline curve.</param>
    /// <param name="meanXIncrement">Mean increment of the x-values of the spectrum.</param>
    /// <returns>The spline options with the smoothing value set according to the parameters.</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    protected static IInterpolationFunctionOptions GetSpline(IInterpolationFunctionOptions interpolation, SmoothnessSpecification smoothnessSpecifiedBy, double smoothnessValue, double numberOfArrayPoints, double meanXIncrement)
    {
      switch (interpolation)
      {
        case SmoothingCubicSplineOptions csp:
          {
            double pointsPerPeriod = smoothnessSpecifiedBy switch
            {
              SmoothnessSpecification.ByNumberOfFeatures => numberOfArrayPoints / smoothnessValue, // smoothness value is NumberOfFeatures
              SmoothnessSpecification.ByNumberOfPoints => smoothnessValue, // smoothness value is period in points
              SmoothnessSpecification.ByXSpan => (smoothnessValue / meanXIncrement), // smoothness value is period in x-units
              _ => throw new NotImplementedException()
            };
            pointsPerPeriod *= pointsPerPeriod;
            pointsPerPeriod *= pointsPerPeriod;
            return csp with { Smoothness = 0.00219146043349633 * pointsPerPeriod };
          }
        default:
          throw new NotImplementedException($"Does not know how to set the smoothing value for spline {interpolation?.GetType()}");
      }
    }
  }

  /// <inheritdoc/>
  public record ISREA : ISREABase, IBaselineEstimation
  {
    #region Serialization

    /// <summary>
    /// 2024-04-16 V0 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ISREA), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ISREA)obj;
        info.AddValue("Interpolation", s.InterpolationFunctionOptions);
        info.AddEnum("SmoothnessSpecifiedBy", s.SmoothnessSpecifiedBy);
        info.AddValue("SmoothnessValue", s.SmoothnessValue);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var interpolation = info.GetValue<IInterpolationFunctionOptions>("Interpolation", parent);
        var smoothnessSpecifiedBy = info.GetEnum<SmoothnessSpecification>("SmoothnessSpecifiedBy");
        var numberOfFeatures = info.GetDouble("SmoothnessValue");

        return new ISREA()
        {
          InterpolationFunctionOptions = interpolation,
          SmoothnessSpecifiedBy = smoothnessSpecifiedBy,
          SmoothnessValue = numberOfFeatures,
        };
      }
    }
    #endregion
  }
}

