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
using System.Xml;
using Altaxo.Calc.Interpolation;

namespace Altaxo.Science.Spectroscopy.BaselineEstimation
{
  /// <summary>
  /// This class detrends a spectrum. This is done by fitting a smoothing spline through the spectrum.
  /// Then the spectral data that lies significantly above the spline are excluded from the spline.
  /// The process is repeated until the resulting curve changes no more.
  /// </summary>
  /// <remarks>Reference: developed by D. Lellinger (currently, no paper is available).</remarks>
  public abstract record SSProbBase : Main.IImmutable
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

    public bool IsAveragingSpanInXUnits { get; init; }

    private double _averagingSpan = 0;
    public double AveragingSpan
    {
      get => _averagingSpan;
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("AveragingSpan must be a value >= 0", nameof(AveragingSpan));

        _averagingSpan = value;
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
      var originalX = xArray.ToArray();
      var originalY = yArray.ToArray();

      var spline = GetSpline(InterpolationFunctionOptions, NumberOfFeatures, originalX.Length)
                    .Interpolate(originalX, originalY);

      var subtractedY = new double[originalX.Length];
      var probabilities = new double[originalY.Length];

      var isParticipating = new bool[originalX.Length];
      var participatingX = new List<double>();
      var participatingY = new List<double>();


      var previousSigmasAndPoints = new HashSet<(double SigmaEstimate, int participatingPoints)>();

      for (int iteration = 0; iteration < 1000; ++iteration)
      {
        for (int i = 0; i < subtractedY.Length; ++i)
        {
          subtractedY[i] = originalY[i] - spline.GetYOfX(originalX[i]);
        }

        // a first estimate of sigma is calculated by calculating the mean deviation of the subtracted
        // values from zero. If the baseline goes through a noise signal, then this is the sigma
        // Note that only the points are taken into account, which are part of the baseline
        var sigmaEstimate = GetMeanDeviationFromZero(subtractedY, isParticipating);

        if (participatingY.Count == 0)
          participatingY.AddRange(subtractedY);

        // a second estimate is calculated from the noise itself (offset free)
        // there is an option in the setting how to determine this noise level
        double sigmaEstimate2;
        if (AveragingSpan == 0)
        {
          sigmaEstimate2 = Altaxo.Science.Signals.SignalMath.GetNoiseLevelEstimate(participatingY.ToArray(), 5);
        }
        else
        {
          // in some Raman spectra, there is an amplitude modulation caused by interferences in the glass fiber
          // although this isn't noise, often is needs to be considered as noise
          // thats why we make another estimate of noise, in which we fit a shifting linear line with Savitzky-Golay
          // and estimate the noise from the deviation of the points with that line
          int numberOfLinePoints;
          if (!IsAveragingSpanInXUnits)
          {
            numberOfLinePoints = (int)Math.Max(3, AveragingSpan);
          }
          else
          {
            double avgIncrement = Math.Abs(originalX[0] - originalX[^1]) / (originalX.Length - 1);
            numberOfLinePoints = (int)Math.Max(3, AveragingSpan / avgIncrement);
          }

          if (participatingY.Count > numberOfLinePoints)
          {
            sigmaEstimate2 = Altaxo.Science.Signals.SignalMath.GetNoiseLevelEstimate(participatingY.ToArray(), numberOfLinePoints, 1);
          }
          else
          {
            sigmaEstimate2 = 0;
          }
        }

        System.Diagnostics.Debug.WriteLine($"Sigma (Stufe {iteration}): {sigmaEstimate}; sigmaEst2={sigmaEstimate2}");

        // finally, for our estimate for sigma we use the maximum value of the two estimates
        sigmaEstimate = Math.Max(sigmaEstimate, sigmaEstimate2);

        // this is the stop criterion: if the pair of sigmaEstimate and number of participating points has
        // already occured before, we stop the iteration
        if (previousSigmasAndPoints.Contains((sigmaEstimate, participatingY.Count)))
        {
          break; // has occured before, thus stop the iteration
        }
        else
        {
          previousSigmasAndPoints.Add((sigmaEstimate, participatingY.Count));
        }

        // now, estimate the probabilities that a point belongs to the noise (i.e. the smaller the probability, the more it belongs to a peak)
        // here, we not simple calculate the probabilities on a point by point base
        // instead, when we have a series of more than one consecutive point that is positive, we calculate the probability
        // for this series of positive points to occur
        int positiveSequenceLength = 0;
        double sumPositiveValues = 0;
        for (int i = 0; i < originalY.Length; ++i)
        {
          double y = subtractedY[i];
          if (y <= 0) // the point is below the baseline, thus reset the counter and sum
          {
            positiveSequenceLength = 0;
            sumPositiveValues = 0;
            probabilities[i] = 1;
          }
          else // the point is above the baseline (i.e., it is positive)
          {
            positiveSequenceLength++;
            sumPositiveValues += y;

            // we calculate the probability, that the sum of positive values is within the noise level
            double p = Math.Exp(-0.5 * sumPositiveValues * sumPositiveValues / (sigmaEstimate * sigmaEstimate * positiveSequenceLength)); // Wahrscheinlichkeit für die Summe

            // if the sequence has more than one point,
            // we also calculate the probability without the first point, and if it is even lower,
            // we use that probability
            if (positiveSequenceLength > 1)
            {
              double ss = sumPositiveValues - subtractedY[i - positiveSequenceLength + 1];
              int nn = positiveSequenceLength - 1;
              double p2 = Math.Exp(-0.5 * ss * ss / (sigmaEstimate * sigmaEstimate * nn)); // Wahrscheinlichkeit für die Summe
              if (p2 < p)
              {
                p = p2;
                sumPositiveValues = ss;
                positiveSequenceLength = nn;
              }
            }
            probabilities[i] = p;
          }
        }
        // in order to make the probabilities symmetrical for a peak, we do the same thing as above,
        // but now from high to low index 
        positiveSequenceLength = 0;
        sumPositiveValues = 0;
        for (int i = originalY.Length - 1; i >= 0; --i)
        {
          double y = subtractedY[i];

          if (y <= 0)
          {
            positiveSequenceLength = 0;
            sumPositiveValues = 0;
            probabilities[i] = 1;
          }
          else
          {
            positiveSequenceLength++;
            sumPositiveValues += y;

            double p = Math.Exp(-0.5 * sumPositiveValues * sumPositiveValues / (sigmaEstimate * sigmaEstimate * positiveSequenceLength)); // 
                                                                                                                                          // wir nehmen dann die kleinste Wahrscheinlichkeit
            if (positiveSequenceLength > 1)
            {
              double ss = sumPositiveValues - subtractedY[i + positiveSequenceLength - 1];
              int nn = positiveSequenceLength - 1;
              double p2 = Math.Exp(-0.5 * ss * ss / (sigmaEstimate * sigmaEstimate * nn)); // Wahrscheinlichkeit für die Summe
              if (p2 < p)
              {
                p = p2;
                sumPositiveValues = ss;
                positiveSequenceLength = nn;
              }
            }
            if (p < probabilities[i]) // if the probability is lower than in the forward run, we use the new probability
            {
              probabilities[i] = p;
            }
          }
        }
        // we now exclude all points that belong to a peak (points with very low probability belonging to noise)
        // the probability threshold is calculated so that only every 100 evaluations we will have a false positive
        double probabilityThreshold = 1E-2 / originalY.Length;

        participatingX.Clear();
        participatingY.Clear();
        for (int i = 0; i < originalY.Length; ++i)
        {
          bool participate = probabilities[i] > probabilityThreshold;
          isParticipating[i] = participate;
          if (participate)
          {
            participatingX.Add(originalX[i]);
            participatingY.Add(originalY[i]);
          }
        }

        if (participatingX.Count < 5) // we need at least 5 points for the smoothing spline
        {
          break;
        }

        var numberOfEffectiveSplinePoints = participatingX.Count;  // GetNumberOfEffectivePoints(listX);
        System.Diagnostics.Debug.WriteLine($"NumberOfEffectivePoint = {numberOfEffectiveSplinePoints}");
        spline = GetSpline(InterpolationFunctionOptions, NumberOfFeatures, numberOfEffectiveSplinePoints)
                            .Interpolate(participatingX.ToArray(), participatingY.ToArray());
      }

      // finally, calculate the resulting baseline from the last spline
      for (int i = 0; i < originalX.Length; i++)
      {
        resultingBaseline[i] = spline.GetYOfX(originalX[i]);
      }
    }


    public void Export(XmlWriter writer)
    {
      writer.WriteStartElement("SSProb");
      writer.WriteElementString("NumberOfFeatures", XmlConvert.ToString(_numberOfFeatures));
      writer.WriteEndElement();
    }

    public override string ToString()
    {
      return $"{this.GetType().Name} NumberOfFeatures={NumberOfFeatures}";
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

    protected static double GetSmoothnessValueForCubicSpline(double numberOfFeatures, double numberOfPoints)
    {
      var pointsPerFeature = numberOfPoints / numberOfFeatures;
      pointsPerFeature *= pointsPerFeature;
      pointsPerFeature *= pointsPerFeature;
      return 0.00219146043349633 * pointsPerFeature;
    }


    protected static double GetMeanDeviationFromZero(IReadOnlyList<double> arr, IReadOnlyList<bool> crit)
    {
      var sum = 0d;
      int N = 0;
      for (int i = 0; i < arr.Count; ++i)
      {
        if (crit[i])
        {
          sum += arr[i] * arr[i];
          ++N;
        }
      }

      return Math.Sqrt(sum / N);
    }

    protected static double GetNumberOfEffectivePoints(IReadOnlyList<double> x)
    {
      double maxInterval = double.MinValue;
      double min = x[0];
      double max = x[0];
      for (int i = 1; i < x.Count; ++i)
      {
        maxInterval = Math.Max(maxInterval, Math.Abs(x[i] - x[i - 1]));
        min = Math.Min(min, x[i]);
        max = Math.Max(max, x[i]);
      }

      return ((max - min) / maxInterval) + 1;

    }
  }

  /// <summary>
  /// This class detrends all spectra. This is done by fitting a polynomial to the spectrum (x value is simply the index of data point), and then
  /// subtracting the fit curve from the spectrum.
  /// The degree of the polynomial can be choosen between 0 (the mean is subtracted), 1 (a fitted straight line is subtracted).
  /// </summary>
  public record SSProb : SSProbBase, IBaselineEstimation
  {
    #region Serialization

    /// <summary>
    /// 2024-04-16 V0 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SSProb), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SSProb)obj;
        info.AddValue("Interpolation", s.InterpolationFunctionOptions);
        info.AddValue("NumberOfFeatures", s.NumberOfFeatures);
        info.AddValue("AveragingSpan", s.AveragingSpan);
        info.AddValue("IsAveragingSpanInXUnits", s.IsAveragingSpanInXUnits);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var interpolation = info.GetValue<IInterpolationFunctionOptions>("Interpolation", parent);
        var numberOfFeatures = info.GetDouble("NumberOfFeatures");
        var averagingSpan = info.GetDouble("AveragingSpan");
        var isAveragingSpanInXUnits = info.GetBoolean("IsAveragingSpanInXUnits");
        return new SSProb()
        {
          InterpolationFunctionOptions = interpolation,
          NumberOfFeatures = numberOfFeatures,
          AveragingSpan = averagingSpan,
          IsAveragingSpanInXUnits = isAveragingSpanInXUnits
        };
      }
    }
    #endregion
  }
}

