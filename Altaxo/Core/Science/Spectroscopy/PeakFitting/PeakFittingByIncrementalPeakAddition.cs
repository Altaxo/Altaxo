#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Threading;
using Altaxo.Calc;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.FitFunctions.Probability;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Science.Spectroscopy.PeakFitting
{
  public record PeakFittingByIncrementalPeakAddition : IPeakFitting
  {
    private IFitFunctionPeak _fitFunction = new VoigtAreaParametrizationNu();

    /// <summary>
    /// Gets /sets the fit function to use.
    /// </summary>
    /// <value>
    /// The fit function.
    /// </value>
    public IFitFunctionPeak FitFunction
    {
      get { return _fitFunction; }
      init { _fitFunction = value ?? throw new ArgumentNullException(nameof(FitFunction)); }
    }

    private int _orderOfBaselinePolynomial = 1;
    /// <summary>
    /// Gets or sets the order of the polynomial that is used for the baseline.
    /// </summary>
    /// <value>
    /// The baseline order.
    /// </value>
    public int OrderOfBaselinePolynomial
    {
      get { return _orderOfBaselinePolynomial; }
      init
      {
        _orderOfBaselinePolynomial = Math.Max(-1, value);
      }
    }

    private int _maximumNumberOfPeaks = 40;

    public int MaximumNumberOfPeaks
    {
      get { return _maximumNumberOfPeaks; }
      init
      {
        _maximumNumberOfPeaks = Math.Max(1, value);
      }
    }

    private double _minimalRelativeHeight = 1E-3;

    /// <summary>
    /// Gets/sets the minimal relative height. The addition of new peaks is stopped
    /// if the fitting residual falls below this value.
    /// </summary>
    /// <value>
    /// Minimal relative height of peaks to be added.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">Must be &gt;=0, nameof(MinimalRelativeHeight)</exception>
    public double MinimalRelativeHeight
    {
      get => _minimalRelativeHeight;
      init
      {
        if (!(_minimalRelativeHeight >= 0))
        {
          throw new ArgumentOutOfRangeException("Must be >=0", nameof(MinimalRelativeHeight));
        }

        _minimalRelativeHeight = value;

      }
    }

    private double _minimalSignalToNoiseRatio = 5;

    /// <summary>
    /// Gets/sets the minimal signal-to-noise ratio. The addition of new peaks is stopped
    /// if the ratio of the highest remaining peak with respect to the noise level falls below this value.
    /// </summary>
    /// <value>
    /// Minimal signal-to-noise ratio of peaks to be added.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">Must be &gt;=0, nameof(MinimalRelativeHeight)</exception>
    public double MinimalSignalToNoiseRatio
    {
      get => _minimalSignalToNoiseRatio;
      init
      {
        if (!(_minimalSignalToNoiseRatio >= 0))
        {
          throw new ArgumentOutOfRangeException("Must be >=0", nameof(MinimalSignalToNoiseRatio));
        }

        _minimalSignalToNoiseRatio = value;

      }
    }

    /// <summary>
    /// Gets / sets the scaling factor of the fit width. This value, when set to a finite value, determines the width around a peak,
    /// that is used to calculate the parameter errors of that peak (the width around the peak is calculated using this number times the FWHM value of the peak).
    /// </summary>
    private double _fitWidthScalingFactor = double.PositiveInfinity;

    /// <summary>
    /// Gets / sets the scaling factor of the fit width. This value, when set to a finite value, determines the width around a peak,
    /// that is used to calculate the parameter errors of that peak (the width around the peak is calculated using this number times the FWHM value of the peak).
    /// </summary>
    public double FitWidthScalingFactor
    {
      get
      {
        return _fitWidthScalingFactor;
      }
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException("Factor has to be > 0", nameof(FitWidthScalingFactor));

        _fitWidthScalingFactor = value;
      }
    }

    #region Serialization

    #region Version 0

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakFittingByIncrementalPeakAddition), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingByIncrementalPeakAddition)obj;
        info.AddValue("FitFunction", s.FitFunction);
        info.AddValue("OrderOfBaselinePolynomial", s.OrderOfBaselinePolynomial);
        info.AddValue("MaximumNumberOfPeaks", s.MaximumNumberOfPeaks);
        info.AddValue("MinimalRelativeHeight", s.MinimalRelativeHeight);
        info.AddValue("MinimalSignalToNoiseRatio", s.MinimalSignalToNoiseRatio);
        info.AddValue("FitWidthScalingFactor", s.FitWidthScalingFactor);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var fitFunction = info.GetValue<IFitFunctionPeak>("FitFunction", null);
        var orderOfBaselinePolynomial = info.GetInt32("OrderOfBaselinePolynomial");
        var maximumNumberOfPeaks = info.GetInt32("MaximumNumberOfPeaks");
        var minimalRelativeHeight = info.GetDouble("MinimalRelativeHeight");
        var minimalSignalToNoiseRatio = info.GetDouble("MinimalSignalToNoiseRatio");
        var fitWidthScalingFactor = info.GetDouble("FitWidthScalingFactor");


        return new PeakFittingByIncrementalPeakAddition()
        {
          FitFunction = fitFunction,
          OrderOfBaselinePolynomial = orderOfBaselinePolynomial,
          MaximumNumberOfPeaks = maximumNumberOfPeaks,
          MinimalRelativeHeight = minimalRelativeHeight,
          MinimalSignalToNoiseRatio = minimalSignalToNoiseRatio,
          FitWidthScalingFactor = fitWidthScalingFactor,
        };
      }
    }

    #endregion

    #endregion


    /// <inheritdoc/>
    public
      (
      double[] x,
      double[] y,
      int[]? regions,
      IReadOnlyList<(IReadOnlyList<PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> peakFittingResults
      ) Execute(double[] xArray, double[] yArray, int[]? regions, IReadOnlyList<(IReadOnlyList<PeakSearching.PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> peakDescriptions, CancellationToken cancellationToken)
    {
      var peakFitDescriptions = new List<(IReadOnlyList<PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)>();
      var yResult = (double[])yArray.Clone();
      foreach (var (peakDesc, start, end) in peakDescriptions)
      {
        cancellationToken.ThrowIfCancellationRequested();
        var subX = new double[end - start];
        var subY = new double[end - start];
        Array.Copy(xArray, start, subX, 0, end - start);
        Array.Copy(yArray, start, subY, 0, end - start);
        var result = Execute(subX, subY, peakDesc, cancellationToken);
        Array.Copy(subY, 0, yResult, start, end - start); // copy yArray back, the baseline now subtracted
        peakFitDescriptions.Add((result, start, end));
      }
      return (xArray, yResult, regions, peakFitDescriptions);
    }

    public List<PeakDescription> Execute(double[] xArray, double[] yArray, IEnumerable<PeakSearching.PeakDescription> peakDescriptions, CancellationToken cancellationToken)
    {
      // First, deduce some characteristics from the x-values
      double minimalX = double.PositiveInfinity;
      double maximalX = double.NegativeInfinity;
      double minimalY = double.PositiveInfinity;
      double maximalY = double.NegativeInfinity;
      double minIncrement = double.PositiveInfinity;

      // estimate properties of x and y arrays
      for (int i = 0; i < xArray.Length; ++i)
      {
        var x = xArray[i];
        minimalX = Math.Min(minimalX, x);
        maximalX = Math.Max(maximalX, x);
        if (i > 0)
        {
          minIncrement = Math.Min(minIncrement, Math.Abs(x - xArray[i - 1]));
        }

        var y = yArray[i];
        minimalY = Math.Min(minimalY, y);
        maximalY = Math.Max(maximalY, y);
      }
      var spanX = maximalX - minimalX;
      var spanY = maximalY - minimalY;

      // estimate the noise level
      double[] noiseArray = new double[yArray.Length - 2];
      for (int i = 2; i < yArray.Length; ++i)
      {
        noiseArray[i - 2] = Math.Abs(yArray[i - 1] - 0.5 * (yArray[i] + yArray[i - 2]));
      }
      Array.Sort(noiseArray);
      var noiseLevel = noiseArray[noiseArray.Length / 2] * 1.22; // take the 50% percentile as noise level

      var fitFunctionWithOneTerm = FitFunction.WithNumberOfTerms(1).WithOrderOfBaselinePolynomial(-1);
      int numberOfParametersPerPeak = fitFunctionWithOneTerm.NumberOfParameters;

      var boundariesForOnePeak = fitFunctionWithOneTerm.GetParameterBoundariesForPositivePeaks(
         minimalPosition: minimalX,
         maximalPosition: maximalX,
         minimalFWHM: minIncrement,
         maximalFWHM: spanX);

      var fitFunction = fitFunctionWithOneTerm.WithOrderOfBaselinePolynomial(OrderOfBaselinePolynomial);


      double[] yRest = (double[])yArray.Clone();

      var lowerBounds = new List<double?>(Enumerable.Repeat<double?>(null, OrderOfBaselinePolynomial + 1));
      var upperBounds = new List<double?>(Enumerable.Repeat<double?>(null, OrderOfBaselinePolynomial + 1));
      double[] previousGuess = new double[OrderOfBaselinePolynomial + 1];

      for (int numberOfTerms = 1; numberOfTerms <= MaximumNumberOfPeaks; ++numberOfTerms)
      {


        var idxMax = yRest.IndexOfMaxValue();
        var yMax = yRest[idxMax];

        if (yMax < Math.Abs(MinimalRelativeHeight * spanY))
        {
          break; // maximum value of residual is below minimal relative height
        }

        if (yMax < Math.Abs(noiseLevel * MinimalSignalToNoiseRatio))
        {
          break; // maximum value of residual is below required signal-to-noise level
        }

        int? idxHalf = null;
        for (int j = 1; j < yRest.Length; ++j)
        {
          if ((idxMax - j) >= 0 && RMath.IsInIntervalCC(0.5 * yMax, yRest[idxMax - j], yRest[idxMax - j + 1]))
          {
            idxHalf = idxMax - j;
            break;
          }
          if ((idxMax + j) < yRest.Length && RMath.IsInIntervalCC(0.5 * yMax, yRest[idxMax + j], yRest[idxMax + j - 1]))
          {
            idxHalf = idxMax + j;
            break;
          }
        }

        if (idxHalf is null)
          throw new InvalidOperationException("We found no half width ");

        double hwhm = Math.Abs(xArray[idxMax] - xArray[idxHalf.Value]);
        // Current.Console.WriteLine($"Stage[{numberOfTerms}]: idxMax={idxMax}, yMax={yMax}, X={srcX[idxMax]}, idxH={idxHalf}, yIdxH={yRest[idxHalf.Value]}, hwhm ={hwhm}");

        fitFunction = fitFunction.WithNumberOfTerms(numberOfTerms);

        var fit = new QuickNonlinearRegression(fitFunction)
        {
          MaximumNumberOfIterations = Math.Min(Math.Max(100, numberOfTerms * 10), 200),
          StepTolerance = 1E-7,
          MinimalRSSImprovement = 1E-4
        };

        double[] initialGuess = new double[OrderOfBaselinePolynomial + 1 + numberOfParametersPerPeak * numberOfTerms];
        bool[] paramsFixed = new bool[initialGuess.Length];
        for (int i = numberOfParametersPerPeak; i < paramsFixed.Length; ++i)
        {
          paramsFixed[i] = true; // alle Parameter außer für den ersten Peak auf Fixed setzen
        }


        // Insert the boundaries for the next peak to fit (note that we have to insert them in inverse order)
        for (int i = numberOfParametersPerPeak - 1; i >= 0; --i)
        {
          lowerBounds.Insert(0, boundariesForOnePeak.LowerBounds?[i]);
          upperBounds.Insert(0, boundariesForOnePeak.UpperBounds?[i]);
        }

        // Copy the parameters from the previous fit to the end of the array
        Array.Copy(previousGuess, 0, initialGuess, numberOfParametersPerPeak, previousGuess.Length);

        // Copy the parameters from the initial guess always to the start of the array
        Array.Copy(fitFunction.GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(yMax, xArray[idxMax], 2 * hwhm, 0.5), initialGuess, numberOfParametersPerPeak);
        var fitResult1 = fit.Fit(xArray, yArray, initialGuess, lowerBounds, upperBounds, null, paramsFixed, cancellationToken);
        double sumChiSquareFirst = fitResult1.ModelInfoAtMinimum.Value;


        // now all parameters are free to vary
        for (int i = 0; i < paramsFixed.Length; ++i)
        {
          paramsFixed[i] = false;
        }

        // Copy fit parameters back to initialGuess array
        for (int i = 0; i < numberOfParametersPerPeak; ++i)
        {
          initialGuess[i] = fitResult1.MinimizingPoint[i];
        }

        // now perform the second fit, now with all parameters free to vary
        var fitResult2 = fit.Fit(xArray, yArray, initialGuess, lowerBounds, upperBounds, null, paramsFixed, cancellationToken);
        double sumChiSquareSecond = fitResult2.ModelInfoAtMinimum.Value;
        // Current.Console.WriteLine($"SumChiSquare First={sumChiSquareFirst}, second={sumChiSquareSecond}");


        previousGuess = fitResult2.MinimizingPoint.ToArray();

        // calculate remaining (original signal minus fit function)
        fitFunction.Evaluate(
          MatrixMath.ToROMatrixWithOneColumn(xArray),
          fitResult2.MinimizingPoint,
          VectorMath.ToVector(yRest),
          null);
        VectorMath.AddScaled(yArray, yRest, -1, yRest); // yRest now contains the rest


        if (cancellationToken.IsCancellationRequested)
          break;
      } // end of loop of adding more and more peaks


      // finally, first, subtract the baseline
      for (int i = 0; i < yArray.Length; ++i)
      {
        double x = xArray[i];
        double sum = 0;
        for (int j = previousGuess.Length - 1, k = 0; k <= OrderOfBaselinePolynomial; --j, ++k)
        {
          sum *= x;
          sum += previousGuess[j];
        }
        yArray[i] -= sum;
      }

      {
        // By definition, the baseline subtraction is part of the preprocessing,
        // even if it is done here as part of the fitting process
        // this means, that the final fit function (and the corresponding data) must not contain the baseline
        fitFunction = fitFunction.WithOrderOfBaselinePolynomial(-1); // get rid of the baseline
        var tempArray = new double[previousGuess.Length - (OrderOfBaselinePolynomial + 1)];
        Array.Copy(previousGuess, 0, tempArray, 0, tempArray.Length); // shrink the parameter array to not include the baseline parameters
        previousGuess = tempArray;
      }

      // and then, summarize the peak descriptions in a list
      var result = new List<PeakDescription>();
      for (int i = 0; i < fitFunction.NumberOfTerms; ++i)
      {
        var peakParameters = VectorMath.ToROVector(previousGuess, i * numberOfParametersPerPeak, numberOfParametersPerPeak);
        var (position, area, height, fwhm) = fitFunction.GetPositionAreaHeightFWHMFromSinglePeakParameters(peakParameters);

        var desc = new PeakDescription()
        {
          SearchDescription = new PeakSearching.PeakDescription
          {
            AbsoluteHeightOfWidthDetermination = double.NaN,
            RelativeHeightOfWidthDetermination = double.NaN,
            Prominence = double.NaN,
            Height = height,
            PositionValue = position,
            WidthValue = fwhm,
            WidthPixels = 0,
            PositionIndex = 0,
          },
          FitFunction = fitFunction,
          FirstFitPoint = 0,
          LastFitPoint = xArray.Length - 1,
          FirstFitPosition = xArray[0],
          LastFitPosition = xArray[^1],
          FitFunctionParameter = previousGuess,
          PeakParameter = peakParameters.ToArray(),
          Notes = string.Empty,
          PeakParameterCovariances = null,
          SigmaSquare = double.NaN,
          SumChiSquare = double.NaN,

        };

        result.Add(desc);
      }

      // Sort the result by ascending position
      result.Sort((x, y) => Comparer<double>.Default.Compare(x.SearchDescription.PositionValue, y.SearchDescription.PositionValue));

      return result;

    }
  }
}
