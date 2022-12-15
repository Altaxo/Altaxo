﻿#region Copyright

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
using Altaxo.Calc.Optimization;
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

    private int _maximumNumberOfPeaks = 50;

    public int MaximumNumberOfPeaks
    {
      get { return _maximumNumberOfPeaks; }
      init
      {
        _maximumNumberOfPeaks = Math.Max(1, value);
      }
    }

    private double _minimalRelativeHeight = 2.5E-3;

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

    private double _minimalSignalToNoiseRatio = 8;

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
    /// Gets / sets the scaling factor of the fit width. This value, when set, determines the width around a peak,
    /// that is used to calculate the parameter errors of that peak (the width around the peak is calculated using this number times the FWHM value of the peak).
    /// </summary>
    private double? _fitWidthScalingFactor;

    /// <summary>
    /// Gets / sets the scaling factor of the fit width. This value, when set, determines the width around a peak,
    /// that is used to calculate the parameter errors of that peak (the width around the peak is calculated using this number times the FWHM value of the peak).
    /// </summary>
    public double? FitWidthScalingFactor
    {
      get
      {
        return _fitWidthScalingFactor;
      }
      init
      {
        if (value.HasValue && !(value > 0))
          throw new ArgumentOutOfRangeException("Factor has to be > 0", nameof(FitWidthScalingFactor));

        _fitWidthScalingFactor = value;
      }
    }

    private double _prunePeaksSumChiSquareFactor = 0.1;
    /// <summary>
    /// Gets/inits a factor that will prune peaks based on their contribution
    /// to the sum of chi square.
    /// </summary>
    /// <value>
    /// Factor that will prune peaks based on their contribution
    /// to the sum of chi square
    /// </value>
    /// <exception cref="System.ArgumentOutOfRangeException">Factor has to be >= 0 - FitWidthScalingFactor</exception>
    /// <remarks>After the fitting of multiple peaks has been done, every one of the peaks will be left out
    /// of the fit, and it will be calculated, how much this will increase the sum of Chi². If
    /// the new SumChi² is less than final SumChi² x (1+<see cref="PrunePeaksSumChiSquareFactor"/>), that
    /// peak will not be included in the final result.</remarks>
    public double PrunePeaksSumChiSquareFactor
    {
      get
      {
        return _prunePeaksSumChiSquareFactor;
      }
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("Factor has to be >= 0", nameof(FitWidthScalingFactor));

        _prunePeaksSumChiSquareFactor = value;
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
        info.AddValue("PrunePeaksSumChiSquareFactor", s.PrunePeaksSumChiSquareFactor);
        info.AddValue("FitWidthScalingFactor", s.FitWidthScalingFactor);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var fitFunction = info.GetValue<IFitFunctionPeak>("FitFunction", null);
        var orderOfBaselinePolynomial = info.GetInt32("OrderOfBaselinePolynomial");
        var maximumNumberOfPeaks = info.GetInt32("MaximumNumberOfPeaks");
        var minimalRelativeHeight = info.GetDouble("MinimalRelativeHeight");
        var minimalSignalToNoiseRatio = info.GetDouble("MinimalSignalToNoiseRatio");
        var prunePeaksSumChiSquareFactor = info.GetDouble("PrunePeaksSumChiSquareFactor");
        var fitWidthScalingFactor = info.GetNullableDouble("FitWidthScalingFactor");


        return new PeakFittingByIncrementalPeakAddition()
        {
          FitFunction = fitFunction,
          OrderOfBaselinePolynomial = orderOfBaselinePolynomial,
          MaximumNumberOfPeaks = maximumNumberOfPeaks,
          MinimalRelativeHeight = minimalRelativeHeight,
          MinimalSignalToNoiseRatio = minimalSignalToNoiseRatio,
          PrunePeaksSumChiSquareFactor = prunePeaksSumChiSquareFactor,
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

      NonlinearMinimizationResult fitResult2 = null;

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
        fitResult2 = fit.Fit(xArray, yArray, initialGuess, lowerBounds, upperBounds, null, paramsFixed, cancellationToken);
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

      (bool wasPruned, fitFunction, previousGuess, var isFixedByUsersOrBoundaries) = PrunePeaksBasedOnSumChiSquare(PrunePeaksSumChiSquareFactor, xArray, yArray, fitFunction, previousGuess, fitResult2.IsFixedByUserOrBoundaries.ToArray(), numberOfParametersPerPeak);
      // if it was pruned, we need to evaluate the covariances again
      if (wasPruned)
      {
        var fit = new QuickNonlinearRegression(fitFunction) { MaximumNumberOfIterations = 0 }; // only evaluate the results
        fitResult2 = fit.Fit(xArray, yArray, previousGuess, null, null, null, isFixedByUsersOrBoundaries, cancellationToken);
      }

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
        yArray[i] -= sum; // Note: it is OK here to modify the yArray, because the calling routine has cloned it before
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

      var result = new List<PeakDescription>();

      if (FitWidthScalingFactor.HasValue && fitResult2 is not null)
      {
        var fit = new QuickNonlinearRegression(fitFunction)
        {
          MaximumNumberOfIterations = 0, // set maximum number of iterations to 0. This causes only to evaluate the results. The parameters will be not changed.
        };

        var isFixed = Enumerable.Repeat(true, previousGuess.Length).ToArray();
        var parameterTemp = new double[previousGuess.Length];
        var parametersSeparate = new double[previousGuess.Length]; // Array to accomodate the parameter variances evaluated for each peak separately

        for (int i = 0, j = 0; i < fitFunction.NumberOfTerms; ++i, j += numberOfParametersPerPeak)
        {
          var parametersForThisPeak = new double[numberOfParametersPerPeak]; // fresh array, because it becomes part of the result!
          Array.Copy(previousGuess, j, parametersForThisPeak, 0, numberOfParametersPerPeak);

          var (position, area, height, fwhm) = fitFunction.GetPositionAreaHeightFWHMFromSinglePeakParameters(parametersForThisPeak);

          var firstX = position - FitWidthScalingFactor.Value * fwhm / 2;
          var lastX = position + FitWidthScalingFactor.Value * fwhm / 2;
          int first = GetIndexOfXInAscendingArray(xArray, firstX, roundUp: false);
          int last = GetIndexOfXInAscendingArray(xArray, lastX, roundUp: true);
          int len = last - first + 1;
          var xCut = new double[len];
          var yCut = new double[len];
          Array.Copy(xArray, first, xCut, 0, len);
          Array.Copy(yArray, first, yCut, 0, len);

          // Save parameter
          Array.Copy(previousGuess, 0, parameterTemp, 0, previousGuess.Length);

          // unfix our set of parameters
          for (int k = 0; k < numberOfParametersPerPeak; k++)
          {
            isFixed[j + k] = isFixedByUsersOrBoundaries[j + k];
          }

          var localFitResult = fit.Fit(xCut, yCut, previousGuess, null, null, null, isFixed, cancellationToken);

          // fix again our set of parameters
          for (int k = 0; k < numberOfParametersPerPeak; k++)
          {
            isFixed[j + k] = true;
          }

          // Restore parameter
          Array.Copy(parameterTemp, 0, previousGuess, 0, previousGuess.Length);

          // extract the covariance matrix
          var covMatrix = CreateMatrix.Dense<double>(numberOfParametersPerPeak, numberOfParametersPerPeak);
          for (int k = 0; k < numberOfParametersPerPeak; k++)
          {
            for (int l = 0; l < numberOfParametersPerPeak; ++l)
            {
              covMatrix[k, l] = localFitResult.Covariance is null ? 0 : localFitResult.Covariance[j + k, j + l];
            }
          }

          var desc = new PeakDescription()
          {
            SearchDescription = new PeakSearching.PeakDescription
            {
              AbsoluteHeightOfWidthDetermination = 0.5 * height,
              RelativeHeightOfWidthDetermination = 0.5,
              Prominence = height,
              Height = height,
              PositionValue = position,
              WidthValue = fwhm,
              WidthPixels = GetIndexOfXInAscendingArray(xArray, position + 0.5 * fwhm, roundUp: null) - GetIndexOfXInAscendingArray(xArray, position - 0.5 * fwhm, roundUp: null),
              PositionIndex = GetIndexOfXInAscendingArray(xArray, position, roundUp: null),
            },
            FitFunction = fitFunction,
            FirstFitPoint = first,
            LastFitPoint = last,
            FirstFitPosition = xCut[0],
            LastFitPosition = xCut[^1],
            FitFunctionParameter = previousGuess,
            PeakParameter = parametersForThisPeak, // save because it was freshly allocated in this loop
            Notes = string.Empty,
            PeakParameterCovariances = covMatrix,
            SumChiSquare = localFitResult.ModelInfoAtMinimum.Value,
            SigmaSquare = localFitResult.ModelInfoAtMinimum.Value / (localFitResult.ModelInfoAtMinimum.DegreeOfFreedom + 1),
          };
          result.Add(desc);
        }
      }
      else if (fitResult2 is not null) // do not use the separate peaks to calculate fit errors etc.
      {
        // and then, summarize the peak descriptions in a list
        for (int i = 0, j = 0; i < fitFunction.NumberOfTerms; ++i, j += numberOfParametersPerPeak)
        {
          var peakParameters = VectorMath.ToROVector(previousGuess, i * numberOfParametersPerPeak, numberOfParametersPerPeak);
          var (position, area, height, fwhm) = fitFunction.GetPositionAreaHeightFWHMFromSinglePeakParameters(peakParameters);

          // extract the covariance matrix
          var covMatrix = CreateMatrix.Dense<double>(numberOfParametersPerPeak, numberOfParametersPerPeak);
          for (int k = 0; k < numberOfParametersPerPeak; k++)
          {
            for (int l = 0; l < numberOfParametersPerPeak; ++l)
            {
              covMatrix[k, l] = fitResult2.Covariance is null ? 0 : fitResult2.Covariance[j + k, j + l];
            }
          }

          var desc = new PeakDescription()
          {
            SearchDescription = new PeakSearching.PeakDescription
            {
              AbsoluteHeightOfWidthDetermination = 0.5 * height,
              RelativeHeightOfWidthDetermination = 0.5,
              Prominence = height,
              Height = height,
              PositionValue = position,
              WidthValue = fwhm,
              WidthPixels = GetIndexOfXInAscendingArray(xArray, position + 0.5 * fwhm, roundUp: null) - GetIndexOfXInAscendingArray(xArray, position - 0.5 * fwhm, roundUp: null),
              PositionIndex = GetIndexOfXInAscendingArray(xArray, position, roundUp: null),
            },
            FitFunction = fitFunction,
            FirstFitPoint = 0,
            LastFitPoint = xArray.Length - 1,
            FirstFitPosition = xArray[0],
            LastFitPosition = xArray[^1],
            FitFunctionParameter = previousGuess,
            PeakParameter = peakParameters.ToArray(),
            Notes = string.Empty,
            PeakParameterCovariances = covMatrix,
            SumChiSquare = fitResult2.ModelInfoAtMinimum.Value,
            SigmaSquare = fitResult2.ModelInfoAtMinimum.Value / (fitResult2.ModelInfoAtMinimum.DegreeOfFreedom + 1),
          };

          result.Add(desc);
        }
      }

      // Sort the result by ascending position
      result.Sort((x, y) => Comparer<double>.Default.Compare(x.SearchDescription.PositionValue, y.SearchDescription.PositionValue));

      return result;
    }

    /// <summary>
    /// Prunes the peaks based on the Sum of Chi².
    /// </summary>
    /// <param name="prunePeaksSumChiSquareFactor">The factor that determines how many peaks are pruned.
    /// Peaks are pruned as long as newChi²&lt;oldChi² x (1+<paramref name="prunePeaksSumChiSquareFactor"/>).</param>
    /// <param name="xArray">The x array.</param>
    /// <param name="yArray">The y array.</param>
    /// <param name="fitFunction">The fit function.</param>
    /// <param name="previousGuess">The fit function's parameter.</param>
    /// <param name="isFixedByUserOrBoundaries">Outcome of the fit that designates which parameters are fixed by the user or by boundary conditions.</param>
    /// <param name="numberOfParametersPerTerm">The number of parameters per peak.</param>
    /// <returns>A value that is true if peaks were pruned, the new fit function, the new parameter set, and the new array of fixed parameters.</returns>
    public static (bool wasPruned, IFitFunctionPeak fitFunction, double[] previousGuess, bool[] isFixedByUserOrBoundaries) PrunePeaksBasedOnSumChiSquare(double prunePeaksSumChiSquareFactor, double[] xArray, double[] yArray, IFitFunctionPeak fitFunction, double[] previousGuess, bool[] isFixedByUserOrBoundaries, int numberOfParametersPerTerm)
    {
      bool wasPruned = false;

      if (prunePeaksSumChiSquareFactor > 0)
      {
        var numberOfTerms = fitFunction.NumberOfTerms;
        var yFit = new double[yArray.Length];
        fitFunction.Evaluate(MatrixMath.ToROMatrixWithOneColumn(xArray), previousGuess, VectorMath.ToVector(yFit), null);
        var sumChi2 = VectorMath.SumOfSquaredDifferences(yArray, yFit);
        var maximalAcceptedSumChi2 = sumChi2 * (1 + prunePeaksSumChiSquareFactor);

        var newGuess = (double[])previousGuess.Clone();
        var arrayOfChiSquare = new List<(int idx, double sumChiSquare)>(numberOfTerms);

        for (int i = 0; i < numberOfTerms; ++i)
        {
          newGuess[i * numberOfParametersPerTerm] = 0; // set first parameter (amplitude or area) to zero
          fitFunction.Evaluate(MatrixMath.ToROMatrixWithOneColumn(xArray), newGuess, VectorMath.ToVector(yFit), null);
          arrayOfChiSquare.Add((i, VectorMath.SumOfSquaredDifferences(yArray, yFit)));
          newGuess[i * numberOfParametersPerTerm] = previousGuess[i * numberOfParametersPerTerm]; // set the amplitude back to its old value
        }

        // Sort the list ascending according to sumChiSquare
        arrayOfChiSquare.Sort((x, y) => Comparer<double>.Default.Compare(x.sumChiSquare, y.sumChiSquare));
        int lenLeftOut;
        for (lenLeftOut = 0; lenLeftOut < numberOfTerms; ++lenLeftOut)
        {
          var i = arrayOfChiSquare[lenLeftOut].idx;
          newGuess[i * numberOfParametersPerTerm] = 0; // set first parameter (amplitude or area) to zero
          fitFunction.Evaluate(MatrixMath.ToROMatrixWithOneColumn(xArray), newGuess, VectorMath.ToVector(yFit), null);
          var newChi2 = VectorMath.SumOfSquaredDifferences(yArray, yFit);
          if (newChi2 > maximalAcceptedSumChi2)
          {
            break;
          }
        }

        if (lenLeftOut > 0)
        {
          fitFunction = fitFunction.WithNumberOfTerms(fitFunction.NumberOfTerms - lenLeftOut);
          var prunedGuess = new double[fitFunction.NumberOfParameters]; // 
          var prunedIsFixed = new bool[fitFunction.NumberOfParameters];
          Array.Copy(previousGuess, lenLeftOut * numberOfParametersPerTerm, prunedGuess, 0, prunedGuess.Length); // copy the parameters at the end of the array- this are the baseline polynomial parameters
          Array.Copy(isFixedByUserOrBoundaries, lenLeftOut * numberOfParametersPerTerm, prunedIsFixed, 0, prunedGuess.Length); // copy the parameters at the end of the array- this are the baseline polynomial parameters

          for (int j = 0; j < numberOfTerms - lenLeftOut; ++j)
          {
            var i = arrayOfChiSquare[j + lenLeftOut].idx;
            Array.Copy(previousGuess, i * numberOfParametersPerTerm, prunedGuess, j * numberOfParametersPerTerm, numberOfParametersPerTerm); // copy parameter for each non-left-out peak separately
            Array.Copy(isFixedByUserOrBoundaries, i * numberOfParametersPerTerm, prunedIsFixed, j * numberOfParametersPerTerm, numberOfParametersPerTerm); // copy isFixed for each non-left-out peak separately)
          }

          previousGuess = prunedGuess;
          isFixedByUserOrBoundaries = prunedIsFixed;
          wasPruned = true;
        }
      }

      return (wasPruned, fitFunction, previousGuess, isFixedByUserOrBoundaries);
    }

    /// <summary>
    /// Gets the index of the provided value in an array of ascending elements.
    /// </summary>
    /// <param name="xArray">The x array.</param>
    /// <param name="x">The x value that is searched.</param>
    /// <param name="roundUp">If there is no exact match, and the parameter is false, the next lower index will be returned, else if true, the index with the higher value will be returned. If null, the index for which x is closest to the element will be returned..</param>
    /// <returns>For an exact match with x, the index of x in the array. Otherwise, either the index of a value lower than x (roundUp=false), higher than x (roundUp=true), or closest to x (roundUp=null).
    /// The return value is always a valid index into the provided array.
    /// </returns>
    public static int GetIndexOfXInAscendingArray(double[] xArray, double x, bool? roundUp)
    {
      int r = Array.BinarySearch(xArray, x);

      if (r >= 0) // found!
      {
        return r;
      }
      else // not found - result depends on the rounding parameter
      {
        r = ~r;
        var rm1 = Math.Max(0, r - 1);
        r = Math.Min(xArray.Length - 1, r);

        return roundUp switch
        {
          false => rm1,
          true => r,
          null => Math.Abs(x - xArray[rm1]) < Math.Abs(x - xArray[r]) ? rm1 : rm1,
        };
      }
    }

  }
}
