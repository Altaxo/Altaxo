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
    IFitFunctionPeak _fitFunction = new VoigtAreaParametrizationNu();

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

    int _orderOfBaselinePolynomial = 1;
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

    int _maximumNumberOfPeaks = 40;

    public int MaximumNumberOfPeaks
    {
      get { return _maximumNumberOfPeaks; }
      init
      {
          _maximumNumberOfPeaks= Math.Max(1, value);
      }
    }

    /// <inheritdoc/>
    public IReadOnlyList<(IReadOnlyList<PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> Execute(double[] xArray, double[] yArray, IReadOnlyList<(IReadOnlyList<PeakSearching.PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> peakDescriptions, CancellationToken cancellationToken)
    {
      var peakFitDescriptions = new List<(IReadOnlyList<PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)>();
      foreach (var (peakDesc, start, end) in peakDescriptions)
      {
        cancellationToken.ThrowIfCancellationRequested();
        var subX = new double[end - start];
        var subY = new double[end - start];
        Array.Copy(xArray, start, subX, 0, end - start);
        Array.Copy(yArray, start, subY, 0, end - start);
        var result = Execute(subX, subY, peakDesc, cancellationToken);
        peakFitDescriptions.Add((result, start, end));
      }
      return peakFitDescriptions;
    }

    public List<PeakDescription> Execute(double[] xArray, double[] yArray, IEnumerable<PeakSearching.PeakDescription> peakDescriptions, CancellationToken cancellationToken)
    {
      // First, deduce some characteristics from the x-values
      double minimalX = double.PositiveInfinity;
      double maximalX = double.NegativeInfinity;
      double minIncrement = double.PositiveInfinity;

      for (int i = 0; i < xArray.Length; ++i)
      {
        var x = xArray[i];
        minimalX = Math.Min(minimalX, x);
        maximalX = Math.Max(maximalX, x);
        if (i > 0)
        {
          minIncrement = Math.Min(minIncrement, Math.Abs(x - xArray[i - 1]));
        }
      }
      var spanX = maximalX - minimalX;


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

        if (yMax <= 0)
        {
          // Break because yMax is below 0
          break;
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
        for (int i = numberOfParametersPerPeak-1; i>=0; --i)
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
      }

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


      return result;

    }
  }
}
