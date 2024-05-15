﻿#region Copyright

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
using Altaxo.Collections;
using Altaxo.Science.Signals;

namespace Altaxo.Science.Spectroscopy.BaselineEstimation
{
  /// <summary>
  /// This class detrends a spectrum. This is done by fitting a smoothing spline through the spectrum.
  /// Then the spectral data that lies significantly above the spline are excluded from the spline.
  /// The process is repeated until the resulting curve changes no more.
  /// </summary>
  /// <remarks>Reference: developed by D. Lellinger (currently, no paper is available).</remarks>
  public abstract partial record SSProbBase : Main.IImmutable
  {
    /// <summary>
    /// Determines how the smoothness of the spline is specified, together with <see cref="SmoothnessValue"/>.
    /// </summary>
    public SmoothnessSpecification SmoothnessSpecifiedBy { get; init; } = SmoothnessSpecification.ByNumberOfFeatures;


    private double _smoothnessValue = 15;

    /// <summary>
    /// Determines the smoothness of the spline. This value has different meaning depending on the value of <see cref="SmoothnessSpecifiedBy"/>.
    /// </summary>
    public double SmoothnessValue
    {
      get => _smoothnessValue;
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException("Number of features must be a value > 0", nameof(SmoothnessValue));

        _smoothnessValue = value;
      }
    }

    private Altaxo.Calc.Interpolation.IInterpolationFunctionOptions _interpolationFunctionOptions = new Altaxo.Calc.Interpolation.SmoothingCubicSplineOptions();

    public Altaxo.Calc.Interpolation.IInterpolationFunctionOptions InterpolationFunctionOptions
    {
      get => _interpolationFunctionOptions;
      init => _interpolationFunctionOptions = value ?? throw new ArgumentNullException(nameof(InterpolationFunctionOptions));
    }

    private double _relativeProbabilityThreshold = 1E-5;

    public double RelativeProbabilityThreshold
    {
      get => _relativeProbabilityThreshold;
      set
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException(nameof(RelativeProbabilityThreshold), "Must be > 0");
        if (!(value <= 1))
          throw new ArgumentOutOfRangeException(nameof(RelativeProbabilityThreshold), "Must be <= 1");

        _relativeProbabilityThreshold = value;
      }
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

      // probability threshold - it is choosen thus, that there is no false positive for all the points x all iteration x some curves
      double probabilityThreshold = _relativeProbabilityThreshold / originalX.Length;

      double meanXIncrement = SignalMath.GetMeanIncrement(originalX);

      var spline = GetSpline(InterpolationFunctionOptions, SmoothnessSpecifiedBy, SmoothnessValue, originalX.Length, meanXIncrement)
                    .Interpolate(originalX, originalY);

      var subtractedY = new double[originalX.Length];
      var probabilities = new double[originalY.Length];

      var isParticipating = new bool[originalX.Length].FillWith(true);
      var participatingX = new List<double>();
      var participatingY = new List<double>();

      // store the previous combinations of the estimate of sigma, and the number of participating points
      // in order to have a criterion to end the iterations
      var previousSigmasAndPoints = new HashSet<(double SigmaEstimate, int participatingPoints)>();

      for (int iteration = 0; iteration < 1000; ++iteration)
      {
        for (int i = 0; i < subtractedY.Length; ++i)
        {
          subtractedY[i] = originalY[i] - spline.GetYOfX(originalX[i]);
        }
        if (participatingY.Count == 0)
        {
          participatingY.AddRange(subtractedY);
        }

        // a first estimate of sigma is calculated by calculating the mean deviation of the subtracted
        // values from zero. If the baseline goes through a noise signal, then this is the sigma
        // Note that only the points are taken into account, which are part of the baseline
        var sigmaEstimate = GetMeanDeviationFromZero(subtractedY, isParticipating);

#if SSProbDiagnostics
        System.Diagnostics.Debug.WriteLine($"Sigma (Stufe {iteration}): {sigmaEstimate}");
#endif
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
        // Remember that the sum of two normal distributed values with sigma is also normal distributed with Sqrt(2)*sigma; a sum of 3 values is distributed with Sqrt(3)*sigma etc.
        // IMPORTANT: the correct formula for the probability that a positive value x belong to noise would be
        // p(x) = Erfc(x/(sigma*Sqrt(2)))
        // Since calculation of Erfc is costly, it is approximated with a function. The approximation is not very accurate, but sufficient for this purpose
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
            double p = ProbabilityThatXIsNoise(sumPositiveValues, sigmaEstimate, positiveSequenceLength); // Probability that the sum of the values is noise

            // if the sequence has more than one point,
            // we also calculate the probability without the first point, and if it is even lower,
            // we use that probability
            if (positiveSequenceLength > 1)
            {
              double sumPositiveValuesWithoutFirst = sumPositiveValues - subtractedY[i - positiveSequenceLength + 1];
              int positiveSequenceLengthWithoutFirst = positiveSequenceLength - 1;
              double p2 = ProbabilityThatXIsNoise(sumPositiveValuesWithoutFirst, sigmaEstimate, positiveSequenceLengthWithoutFirst); // Probability that the sum of values without the first term is noise
              if (p2 < p)
              {
                p = p2;
                sumPositiveValues = sumPositiveValuesWithoutFirst;
                positiveSequenceLength = positiveSequenceLengthWithoutFirst;
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

            double p = ProbabilityThatXIsNoise(sumPositiveValues, sigmaEstimate, positiveSequenceLength); // Probability for the sum

            if (positiveSequenceLength > 1)
            {
              double sumPositiveValuesWithoutFirst = sumPositiveValues - subtractedY[i + positiveSequenceLength - 1];
              int positiveSequenceLengthWithoutFirst = positiveSequenceLength - 1;
              double p2 = ProbabilityThatXIsNoise(sumPositiveValuesWithoutFirst, sigmaEstimate, positiveSequenceLengthWithoutFirst); // Probability for the sum without the first term
              if (p2 < p)
              {
                p = p2;
                sumPositiveValues = sumPositiveValuesWithoutFirst;
                positiveSequenceLength = positiveSequenceLengthWithoutFirst;
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

#if SSProbDiagnostics
        System.Diagnostics.Debug.WriteLine($"NumberOfEffectivePoint = {numberOfEffectiveSplinePoints}");
#endif

        spline = GetSpline(InterpolationFunctionOptions, SmoothnessSpecifiedBy, SmoothnessValue, numberOfEffectiveSplinePoints, meanXIncrement)
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
      writer.WriteElementString("NumberOfFeatures", XmlConvert.ToString(_smoothnessValue));
      writer.WriteEndElement();
    }

    public override string ToString()
    {
      return $"{this.GetType().Name} NumberOfFeatures={SmoothnessValue}";
    }

    /// <summary>
    /// Gets the smoothing spline (options) for which the smoothing amount is set to the value given in the options.
    /// </summary>
    /// <param name="interpolation">The spline interpolation options.</param>
    /// <param name="smoothnessSpecifiedBy">Determines how the smoothness is specified.</param>
    /// <param name="smoothnessValue">The smoothness value. The smoothness depends on this value and how the smoothness is specified.</param>
    /// <param name="numberOfArrayPoints">The number of points in the spline curve.</param>
    /// <param name="meanXIncrement">Mean increment of the x-values of the spectrum.</param>
    /// <returns>The same spline, but now with the smoothing value set according to the parameters.</returns>
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

    /// <summary>
    /// Approximation for the probability that the value x belongs to the noise level. Attention: x has to be positive (this is not tested!).
    /// This function is an approximation for Erfc(x/(sigma*Sqrt(2)).
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="sigma">The sigma of the noise.</param>
    /// <param name="numberOfAddedPoints">The parameter <paramref name="x"/>can contain of a single value, or of a sum of consecutive values.
    /// The number represents the number of values that were added up. Provide 1  if x is a single value.</param>
    /// <returns>Approximation for the Erfc function.</returns>
    protected static double ProbabilityThatXIsNoise(double x, double sigma, int numberOfAddedPoints)
    {
      x /= sigma * Math.Sqrt(numberOfAddedPoints);
      return Math.Exp(-0.5 * x * x) / (1 + x);
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
  }

  /// <inheritdoc/>
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
        info.AddEnum("SmoothnessSpecifiedBy", s.SmoothnessSpecifiedBy);
        info.AddValue("SmoothnessValue", s.SmoothnessValue);
        info.AddValue("Probability", s.RelativeProbabilityThreshold);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var interpolation = info.GetValue<IInterpolationFunctionOptions>("Interpolation", parent);
        var smoothnessSpecifiedBy = info.GetEnum<SmoothnessSpecification>("SmoothnessSpecifiedBy");
        var numberOfFeatures = info.GetDouble("SmoothnessValue");
        var probabilityThreshold = info.GetDouble("Probability");
        return new SSProb()
        {
          InterpolationFunctionOptions = interpolation,
          SmoothnessSpecifiedBy = smoothnessSpecifiedBy,
          SmoothnessValue = numberOfFeatures,
          RelativeProbabilityThreshold = probabilityThreshold,
        };
      }
    }
    #endregion
  }
}

