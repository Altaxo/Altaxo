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

namespace Altaxo.Science.Spectroscopy.BaselineEstimation
{
  /// <summary>
  /// This class detrends a spectrum. This is done by fitting a smoothing spline through the spectrum.
  /// Then the spectral data that lies above the spline are reduced in amplitude toward the spline.
  /// The process is repeated until the resulting curve changes no more.
  /// </summary>
  /// <remarks>
  /// Reference: Y.Xu, P.Du, R.Senger, J.Robertson, J.Pirkle, Applied Spectroscopy 2021 Vol 75 (1) 34-45
  /// </remarks>
  public abstract record ISREABase : Main.IImmutable
  {
    private double _numberOfFeatures = 15;

    public double NumberOfFeatures
    {
      get => _numberOfFeatures;
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException("Number of features must be a value > 0", nameof(NumberOfFeatures));

        _numberOfFeatures = value;
      }
    }

    private Altaxo.Calc.Interpolation.IInterpolationFunctionOptions _interpolationFunctionOptions = new Altaxo.Calc.Interpolation.SmoothingCubicSplineOptions();

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


    /// <summary>
    /// Executes the baseline estimation algorithm with the provided spectrum.
    /// </summary>
    /// <param name="xArray">The x values of the spectral values.</param>
    /// <param name="yArray">The array of spectral values.</param>
    /// <param name="resultingBaseline">The location to which the estimated baseline should be copied.</param>
    public void Execute(ReadOnlySpan<double> xArray, ReadOnlySpan<double> yArray, Span<double> resultingBaseline)
    {
      double stopThreshold = 1E-4;
      stopThreshold *= stopThreshold;

      var regionlength = yArray.Length;

      var xx = xArray.ToArray();
      var yy = yArray.ToArray();
      var yprev = new double[yy.Length];
      var spline = GetSpline(InterpolationFunctionOptions, NumberOfFeatures, yArray.Length);
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


    public void Export(XmlWriter writer)
    {
      writer.WriteStartElement("ISREA");
      writer.WriteElementString("NumberOfKnots", XmlConvert.ToString(_numberOfFeatures));
      writer.WriteEndElement();
    }

    public override string ToString()
    {
      return $"{this.GetType().Name} NumberOfKnots={NumberOfFeatures}";
    }

    /// <summary>
    /// Gets the smoothing spline (options) for which the smoothing amount is set to the value given in the options.
    /// </summary>
    /// <param name="interpolation">The spline interpolation options.</param>
    /// <param name="numberOfFeatures">The number of features. The less features, the smoother the spline will be.</param>
    /// <param name="numberOfPoints">The number of points in the spline curve.</param>
    /// <returns>The same spline, but now with the smoothing value set according to <paramref name="numberOfFeatures"/>.</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    protected static IInterpolationFunctionOptions GetSpline(IInterpolationFunctionOptions interpolation, double numberOfFeatures, double numberOfPoints)
    {
      switch (interpolation)
      {
        case SmoothingCubicSplineOptions csp:
          {
            var pointsPerFeature = numberOfPoints / numberOfFeatures;
            pointsPerFeature *= pointsPerFeature;
            pointsPerFeature *= pointsPerFeature;
            return csp with { Smoothness = 0.00219146043349633 * pointsPerFeature };
          }
        default:
          throw new NotImplementedException($"Does not know how to set the smoothing value for spline {interpolation?.GetType()}");
      }
    }
  }

  /// <summary>
  /// This class detrends all spectra. This is done by fitting a polynomial to the spectrum (x value is simply the index of data point), and then
  /// subtracting the fit curve from the spectrum.
  /// The degree of the polynomial can be choosen between 0 (the mean is subtracted), 1 (a fitted straight line is subtracted).
  /// </summary>
  public record ISREA : ISREABase, IBaselineEstimation
  {
    #region Serialization

    /// <summary>
    /// 2024-04-16 V0 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ISREA), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ISREA)obj;
        info.AddValue("Interpolation", s.InterpolationFunctionOptions);
        info.AddValue("NumberOfFeatures", s.NumberOfFeatures);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var interpolation = info.GetValue<IInterpolationFunctionOptions>("Interpolation", parent);
        var order = info.GetDouble("NumberOfFeatures");
        return new ISREA() { InterpolationFunctionOptions = interpolation, NumberOfFeatures = order };
      }
    }
    #endregion
  }
}

