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
using System.Linq;
using System.Threading;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.FitFunctions.Probability;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Science.Spectroscopy.PeakSearching;

namespace Altaxo.Science.Spectroscopy.PeakFitting.MultipleSpectra
{
  public record PeakFittingOfMultipleSpectraByIncrementalPeakAddition
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
    /// Gets/sets the minimal relative height (relative to the maximum of the y-span of all spectra).
    /// The addition of new peaks is stopped if the fitting residual falls below this value.
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
    /// <exception cref="ArgumentOutOfRangeException">Factor has to be >= 0 - FitWidthScalingFactor</exception>
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

    private double _minimalFWHMValue;
    public double MinimalFWHMValue
    {
      get
      {
        return _minimalFWHMValue;
      }
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("Value has to be >= 0", nameof(MinimalFWHMValue));

        _minimalFWHMValue = value;
      }
    }

    public bool IsMinimalFWHMValueInXUnits { get; init; } = true;

    /// <summary>
    /// Gets a list of fixed peak positions. While the designated positions are fixed and will not participate in the fitting process,
    /// the designated FWHM values are intended for calculation of the initial peak parameter values and to calculate the parameter boundaries.
    /// </summary>
    public IReadOnlyList<(double Position, double InitialFWHMValue, double? MinimalFWHMValue, double? MaximalFWHMValue)> FixedPeakPositions { get; init; } = [];


    public PeakAdditionOrder PeakAdditionOrder { get; init; } = PeakAdditionOrder.Height;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakFittingOfMultipleSpectraByIncrementalPeakAddition), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingOfMultipleSpectraByIncrementalPeakAddition)obj;
        info.AddValue("FitFunction", s.FitFunction);
        info.AddValue("OrderOfBaselinePolynomial", s.OrderOfBaselinePolynomial);
        info.AddValue("MaximumNumberOfPeaks", s.MaximumNumberOfPeaks);
        info.AddValue("MinimalRelativeHeight", s.MinimalRelativeHeight);
        info.AddValue("MinimalSignalToNoiseRatio", s.MinimalSignalToNoiseRatio);
        info.AddValue("IsMinimalFWHMValueInXUnits", s.IsMinimalFWHMValueInXUnits);
        info.AddValue("MinimalFWHMValue", s.MinimalFWHMValue);
        info.AddValue("PrunePeaksSumChiSquareFactor", s.PrunePeaksSumChiSquareFactor);
        info.AddValue("FitWidthScalingFactor", s.FitWidthScalingFactor);
        info.AddEnum("PeakAdditionOrder", s.PeakAdditionOrder);
        info.CreateArray("FixedPeakPositions", s.FixedPeakPositions.Count);
        {
          for (int i = 0; i < s.FixedPeakPositions.Count; ++i)
          {
            info.CreateElement("e");
            info.AddValue("Position", s.FixedPeakPositions[i].Position);
            info.AddValue("InitialFWHMValue", s.FixedPeakPositions[i].InitialFWHMValue);
            info.AddValue("MinimalFWHMValue", s.FixedPeakPositions[i].MinimalFWHMValue);
            info.AddValue("MaximalFWHMValue", s.FixedPeakPositions[i].MaximalFWHMValue);
            info.CommitElement();
          }
        }
        info.CommitArray(); // FixedPeakPositions
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var fitFunction = info.GetValue<IFitFunctionPeak>("FitFunction", null);
        var orderOfBaselinePolynomial = info.GetInt32("OrderOfBaselinePolynomial");
        var maximumNumberOfPeaks = info.GetInt32("MaximumNumberOfPeaks");
        var minimalRelativeHeight = info.GetDouble("MinimalRelativeHeight");
        var minimalSignalToNoiseRatio = info.GetDouble("MinimalSignalToNoiseRatio");
        var isMinimalFWHMValueInXUnits = info.GetBoolean("IsMinimalFWHMValueInXUnits");
        var minimalFWHMValue = info.GetDouble("MinimalFWHMValue");
        var prunePeaksSumChiSquareFactor = info.GetDouble("PrunePeaksSumChiSquareFactor");
        var fitWidthScalingFactor = info.GetNullableDouble("FitWidthScalingFactor");
        var peakAdditionOrder = info.GetEnum("PeakAdditionOrder", typeof(PeakAdditionOrder));

        var count = info.OpenArray("FixedPeakPositions");
        var fixedPeaks = new (double Position, double InitialFWHMValue, double? MinimalFWHMValue, double? MaximalFWHMValue)[count];
        {
          for (int i = 0; i < count; ++i)
          {
            info.OpenElement();
            var pos = info.GetDouble("Position");
            var inifwhm = info.GetDouble("InitialFWHMValue");
            var minfwhm = info.GetNullableDouble("MinimalFWHMValue");
            var maxfwhm = info.GetNullableDouble("MaximalFWHMValue");
            fixedPeaks[i] = (pos, inifwhm, minfwhm, maxfwhm);
            info.CloseElement();
          }
        }
        info.CloseArray(count); // FixedPeakPositions

        return new PeakFittingOfMultipleSpectraByIncrementalPeakAddition()
        {
          FitFunction = fitFunction,
          OrderOfBaselinePolynomial = orderOfBaselinePolynomial,
          MaximumNumberOfPeaks = maximumNumberOfPeaks,
          MinimalRelativeHeight = minimalRelativeHeight,
          MinimalSignalToNoiseRatio = minimalSignalToNoiseRatio,
          IsMinimalFWHMValueInXUnits = isMinimalFWHMValueInXUnits,
          MinimalFWHMValue = minimalFWHMValue,
          PrunePeaksSumChiSquareFactor = prunePeaksSumChiSquareFactor,
          FitWidthScalingFactor = fitWidthScalingFactor,
          FixedPeakPositions = fixedPeaks,
        };
      }
    }

    #endregion

    #endregion


    /// <summary>
    /// Executes the peak fitting algorithm.
    /// </summary>
    /// <param name="spectra">The list of spectra. Each spectrum consists of an x-array and an y-array.</param>
    /// <param name="cancellationToken">The token to cancel the algorithm.</param>
    /// <param name="cancellationTokenHard">The token to hard cancel the algorithm</param>
    /// <returns>A list of peak descriptions, sorted by position ascending.</returns>
    public MultipleSpectraPeakFittingResult Execute(IReadOnlyList<(double[] xArray, double[] yArray)> spectra, CancellationToken cancellationToken, CancellationToken cancellationTokenHard, IProgress<double>? numericalProgress = null, IProgress<string> textualProgress = null)
    {
      var fitFunctionWithOneTerm = FitFunction.WithNumberOfTerms(1).WithOrderOfBaselinePolynomial(-1);
      int numberOfSpectra = spectra.Count;
      int numberOfParametersPerPeakLocal = fitFunctionWithOneTerm.NumberOfParameters;
      int numberOfParametersPerPeakGlobal = numberOfParametersPerPeakLocal - 1 + numberOfSpectra;

      // First, deduce some characteristics from the x-values
      var minimalX = double.PositiveInfinity;
      var maximalX = double.NegativeInfinity;
      var minIncrement = double.PositiveInfinity;

      // estimate properties of x and y arrays
      int totalNumberOfPoints = 0;
      double spanY = 0;
      foreach (var spectrum in spectra)
      {
        var minimalY = double.PositiveInfinity;
        var maximalY = double.NegativeInfinity;
        var len = Math.Min(spectrum.xArray.Length, spectrum.yArray.Length);
        for (var i = 0; i < len; ++i)
        {
          var x = spectrum.xArray[i];
          minimalX = Math.Min(minimalX, x);
          maximalX = Math.Max(maximalX, x);
          if (i > 0)
          {
            minIncrement = Math.Min(minIncrement, Math.Abs(x - spectrum.xArray[i - 1]));
          }

          var y = spectrum.yArray[i];
          minimalY = Math.Min(minimalY, y);
          maximalY = Math.Max(maximalY, y);
        }
        totalNumberOfPoints += len;
        spanY = Math.Max(spanY, maximalY - minimalY);
      }
      var spanX = maximalX - minimalX;

      var minimalFWHM = IsMinimalFWHMValueInXUnits && MinimalFWHMValue > 0 ? MinimalFWHMValue : Math.Max(1, MinimalFWHMValue) * minIncrement;
      var maximalFWHM = spanX;


      // make one big array of x and y values
      var xGlobal = new double[totalNumberOfPoints];
      var yGlobal = new double[totalNumberOfPoints];
      totalNumberOfPoints = 0;
      var startIndicesOfSpectra = new int[spectra.Count];
      {
        int idxSpectrum = 0;
        foreach (var spectrum in spectra)
        {
          var len = Math.Min(spectrum.xArray.Length, spectrum.yArray.Length);
          startIndicesOfSpectra[idxSpectrum] = totalNumberOfPoints;
          for (int i = 0; i < len; ++i)
          {
            xGlobal[totalNumberOfPoints + i] = spectrum.xArray[i];
            yGlobal[totalNumberOfPoints + i] = spectrum.yArray[i];
          }

          ++idxSpectrum;
          totalNumberOfPoints += len;
        }
      }

      int GetLengthOfSpectrum(int i)
      {
        return i + 1 < numberOfSpectra ? startIndicesOfSpectra[i + 1] - startIndicesOfSpectra[i] : totalNumberOfPoints - startIndicesOfSpectra[i];
      }

      var yRest = (double[])yGlobal.Clone();

      double[] previousGuess = new double[OrderOfBaselinePolynomial + 1];
      NonlinearMinimizationResult? fitResult2 = null;

      // determine the noise level
      var noiseStatistics = new Altaxo.Calc.Regression.QuickStatistics();
      for (int iSpectrum = 0; iSpectrum < numberOfSpectra; ++iSpectrum)
      {
        var noiseSpectrum_i = Altaxo.Science.Signals.SignalMath.GetNoiseLevelEstimate(spectra[iSpectrum].yArray, 3);
        noiseStatistics.Add(noiseSpectrum_i);
      }
      var noiseLevel = noiseStatistics.Mean;

      var boundariesForOnePeak = fitFunctionWithOneTerm.GetParameterBoundariesForPositivePeaks(
        minimalPosition: minimalX,
        maximalPosition: maximalX,
        minimalFWHM: IsMinimalFWHMValueInXUnits && MinimalFWHMValue > 0 ? MinimalFWHMValue : Math.Max(1, MinimalFWHMValue) * minIncrement,
        maximalFWHM: spanX);

      var prohibitedPeaks = new Dictionary<int, (int countDown, int numberOfPushs)>(); // Dictionary of prohibited peaks, key is the point index, value is the number of times this peak was evaluated as prohibited
      double minimalPeakHeightForSearching = Math.Max(Math.Abs(MinimalRelativeHeight * spanY), Math.Abs(noiseLevel * MinimalSignalToNoiseRatio));


      // ************************************************************
      // Fit the baseline first, and optionally, the fixed peaks
      // ************************************************************
      var fitFunction = fitFunctionWithOneTerm.WithNumberOfTerms(FixedPeakPositions.Count).WithOrderOfBaselinePolynomial(OrderOfBaselinePolynomial);
      var fitFunctionGlobal = new FitFunctionMultipleSpectraSeparatePeakHeightsSeparateBaseline(fitFunction, startIndicesOfSpectra);
      var fit = new QuickNonlinearRegression(fitFunctionGlobal)
      {
        MaximumNumberOfIterations = 200,
        StepTolerance = 1E-7,
        MinimalRSSImprovement = 1E-4
      };
      var lowerBounds = new List<double?>();
      var upperBounds = new List<double?>();
      var tempInitialGuessList = new List<double>();
      var tempInitialFixedList = new List<bool>();
      for (int idxPeak = 0; idxPeak < FixedPeakPositions.Count; ++idxPeak)
      {
        var fixedPeak = FixedPeakPositions[idxPeak];
        var fixedPeakBoundaries = fitFunctionWithOneTerm.GetParameterBoundariesForPositivePeaks(
        minimalPosition: minimalX,
        maximalPosition: maximalX,
        minimalFWHM: fixedPeak.MinimalFWHMValue.HasValue ? Math.Max(minimalFWHM, fixedPeak.MinimalFWHMValue.Value) : minimalFWHM,
        maximalFWHM: fixedPeak.MaximalFWHMValue.HasValue ? Math.Min(maximalFWHM, fixedPeak.MaximalFWHMValue.Value) : maximalFWHM);
        var peakParam = fitFunctionWithOneTerm.GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(0, fixedPeak.Position, fixedPeak.InitialFWHMValue, 0.5);

        for (int idxSpectrum = 0; idxSpectrum < numberOfSpectra; ++idxSpectrum)
        {
          // add the amplitude boundaries for each spectrum
          lowerBounds.Add(fixedPeakBoundaries.LowerBounds?[0]);
          upperBounds.Add(fixedPeakBoundaries.UpperBounds?[0]);
          tempInitialGuessList.Add(peakParam[0]);
          tempInitialFixedList.Add(false);
        }
        for (int j = 1; j < numberOfParametersPerPeakLocal; ++j)
        {
          // add the other parameter boundaries
          lowerBounds.Add(fixedPeakBoundaries.LowerBounds?[j]);
          upperBounds.Add(fixedPeakBoundaries.UpperBounds?[j]);
          tempInitialGuessList.Add(peakParam[j]);
          tempInitialFixedList.Add(j == 1); // the position (j==1) is fixed, all other parameters can vary
        }
      }
      // Add lower and upper bounds for the baseline parameters
      for (int idxSpectrum = 0; idxSpectrum < numberOfSpectra; ++idxSpectrum)
      {
        for (int j = 0; j <= OrderOfBaselinePolynomial; ++j)
        {
          lowerBounds.Add(null);
          upperBounds.Add(null);
          tempInitialGuessList.Add(0);
          tempInitialFixedList.Add(false);
        }
      }
      var initialGuess = tempInitialGuessList.ToArray();
      var isFixed = tempInitialFixedList.ToArray();

      if (initialGuess.Length > 0)
      {
        fitFunctionGlobal.SetAllPeakParametersExceptFirstPeakToFixed(false, FixedPeakPositions.Count); // all baseline parameters can vary
        fitResult2 = fit.Fit(xGlobal, yGlobal, initialGuess, lowerBounds, upperBounds, null, isFixed, cancellationTokenHard);
        previousGuess = fitResult2.MinimizingPoint.ToArray();

        // calculate remaining (original signal minus fit function)
        fitFunctionGlobal.Evaluate(
          MatrixMath.ToROMatrixWithOneColumn(xGlobal),
          previousGuess,
          VectorMath.ToVector(yRest),
          null);
        VectorMath.AddScaled(yGlobal, yRest, -1, yRest); // yRest now contains the rest
      }

      for (int numberOfTerms = 1 + FixedPeakPositions.Count; numberOfTerms <= MaximumNumberOfPeaks; ++numberOfTerms)
      {
        // search in all spectra for peaks
        var peakList = new List<(int Position, double Height, double Width, int idxSpectrum)>();
        for (int iSpectrum = 0; iSpectrum < numberOfSpectra; ++iSpectrum)
        {
          var pf = new PeakFinder();
          pf.SetRelativeHeight(0.5);
          pf.SetWidth(0.0);
          pf.SetHeight(minimalPeakHeightForSearching);
          pf.Execute(new ArraySegment<double>(yRest, startIndicesOfSpectra[iSpectrum], GetLengthOfSpectrum(iSpectrum)));
          for (int i = 0; i < pf.PeakPositions.Length; i++)
          {
            peakList.Add((pf.PeakPositions[i], pf.PeakHeights![i], pf.Widths![i], iSpectrum));
          }
        }
        // peakList now contains the peaks of all spectra, now sort them by height (descending), so that the highest peak is first
        switch (PeakAdditionOrder)
        {
          case PeakAdditionOrder.Height:
            peakList.Sort((x, y) => Comparer<double>.Default.Compare(y.Height, x.Height)); // Sort peaks by height descending
            break;
          case PeakAdditionOrder.Area:
            peakList.Sort((x, y) => Comparer<double>.Default.Compare(y.Height * y.Width, x.Height * x.Width)); // Sort peaks by height descending
            break;
          case PeakAdditionOrder.SquaredHeightTimesWidth:
            peakList.Sort((x, y) => Comparer<double>.Default.Compare(y.Height * y.Height * y.Width, x.Height * x.Height * x.Width)); // Sort peaks by height descending
            break;
          default:
            throw new NotImplementedException($"Unknown peak addition order: {PeakAdditionOrder}");
        }

        var thispeak = peakList.FirstOrDefault(peak => !prohibitedPeaks.TryGetValue(peak.Position, out var prohibitionLevel) || prohibitionLevel.countDown == 0);

        if (thispeak.Height == 0)
        {
          break; // no more peaks to fit
        }

        var idxMax = thispeak.Position;
        var yMax = thispeak.Height;
        var fwhm = 0.5 * Math.Abs(PeakSearchingNone.GetWidthValue(spectra[thispeak.idxSpectrum].xArray, thispeak.Position - 0.5 * thispeak.Width, thispeak.Position, thispeak.Position + 0.5 * thispeak.Width));
        var xPos = spectra[thispeak.idxSpectrum].xArray[idxMax];

        fitFunction = fitFunction.WithNumberOfTerms(numberOfTerms);

        fitFunctionGlobal = new FitFunctionMultipleSpectraSeparatePeakHeightsSeparateBaseline(fitFunction, startIndicesOfSpectra);

        fit = new QuickNonlinearRegression(fitFunctionGlobal)
        {
          MaximumNumberOfIterations = Math.Min(Math.Max(100, numberOfTerms * 10), 200),
          StepTolerance = 1E-7,
          MinimalRSSImprovement = 1E-4
        };

        initialGuess = new double[numberOfTerms * numberOfParametersPerPeakGlobal + numberOfSpectra * (OrderOfBaselinePolynomial + 1)];
        bool[] paramsFixed = new bool[initialGuess.Length];

        // all Parameters exept that for the first peak are set to fixed
        // we do this directly in the fit wrapper, not here.
        fitFunctionGlobal.SetAllPeakParametersExceptFirstPeakToFixed(true, FixedPeakPositions.Count);

        for (int i = numberOfParametersPerPeakGlobal; i < paramsFixed.Length; ++i)
        {
          paramsFixed[i] = true; // alle Parameter außer für den ersten Peak auf Fixed setzen
        }

        // Insert the boundaries for the next peak to fit (note that we have to insert them in inverse order)
        for (int i = numberOfParametersPerPeakLocal - 1; i >= 0; --i)
        {
          lowerBounds.Insert(0, boundariesForOnePeak.LowerBounds?[i]);
          upperBounds.Insert(0, boundariesForOnePeak.UpperBounds?[i]);
        }
        // in the previous for loop we have only added the bounds for one amplitude, now we have to add the boundaries for the other amplitudes
        for (int i = 0; i < numberOfSpectra - 1; ++i)
        {
          lowerBounds.Insert(0, boundariesForOnePeak.LowerBounds?[0]);
          upperBounds.Insert(0, boundariesForOnePeak.UpperBounds?[0]);
        }

        // Copy the parameters from the initial guess always to the start of the array
        var peakParam = fitFunction.GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(yMax, xPos, fwhm, 0.5);
        if (peakParam[2] < boundariesForOnePeak.LowerBounds[2])
          peakParam[2] = boundariesForOnePeak.LowerBounds[2].Value;
        else if (peakParam[2] > boundariesForOnePeak.UpperBounds[2])
          peakParam[2] = boundariesForOnePeak.UpperBounds[2].Value;

        // Copy the parameters from the previous fit to the end of the array
        Array.Copy(previousGuess, 0, initialGuess, initialGuess.Length - previousGuess.Length, previousGuess.Length);
        // now copy the new peak parameters at the beginning of the array (note that no amplitude is set)
        Array.Copy(peakParam, 1, initialGuess, numberOfSpectra, numberOfParametersPerPeakLocal - 1);

        var fitResult1 = fit.Fit(xGlobal, yGlobal, initialGuess, lowerBounds, upperBounds, null, paramsFixed, cancellationTokenHard);

        double sumChiSquareFirst = fitResult1.ModelInfoAtMinimum.Value;

        // if the amplitude of the new peak is zero on all spectra, then this peak has a peculiar shape,
        // we set this peak on a list of prohibited peaks

        bool isAmplitudeZeroForAllSpectra = true;
        for (int i = 0; i < numberOfSpectra; i++)
        {
          isAmplitudeZeroForAllSpectra &= (0 == fitResult1.MinimizingPoint[0]);
        }

        if (isAmplitudeZeroForAllSpectra)
        {
          if (prohibitedPeaks.TryGetValue(idxMax, out var val))
          {
            prohibitedPeaks[idxMax] = (2 * (val.numberOfPushs + 1), val.numberOfPushs + 1);
          }
          else
          {
            prohibitedPeaks.Add(idxMax, (2, 1));
          }

          // Decrease the number of terms, and remove again the lower and upper bounds
          --numberOfTerms;
          for (int i = 0; i < numberOfParametersPerPeakGlobal; i++)
          {
            lowerBounds.RemoveAt(0);
            upperBounds.RemoveAt(0);
          }
          continue;
        }

        foreach (var key in prohibitedPeaks.Keys.ToArray())
        {
          var val = prohibitedPeaks[key];
          prohibitedPeaks[key] = (Math.Max(0, val.countDown - 1), val.numberOfPushs);
        }

        // now all parameters are free to vary, except the fixed positions
        Array.Clear(paramsFixed, 0, paramsFixed.Length);
        for (int idxPeak = numberOfTerms - FixedPeakPositions.Count; idxPeak < numberOfTerms; ++idxPeak)
        {
          paramsFixed[idxPeak * numberOfParametersPerPeakGlobal + numberOfSpectra] = true;
        }
        fitFunctionGlobal.SetAllPeakParametersExceptFirstPeakToFixed(false, FixedPeakPositions.Count);

        // Copy fit parameters back to initialGuess array
        for (int i = 0; i < initialGuess.Length; ++i)
        {
          initialGuess[i] = fitResult1.MinimizingPoint[i];
        }

        // now perform the second fit, now with all parameters free to vary
        fitResult2 = fit.Fit(xGlobal, yGlobal, initialGuess, lowerBounds, upperBounds, null, paramsFixed, cancellationTokenHard);
        double sumChiSquareSecond = fitResult2.ModelInfoAtMinimum.Value;
        // Current.Console.WriteLine($"SumChiSquare First={sumChiSquareFirst}, second={sumChiSquareSecond}");

        previousGuess = fitResult2.MinimizingPoint.ToArray();

        // calculate remaining (original signal minus fit function)
        fitFunctionGlobal.Evaluate(
          MatrixMath.ToROMatrixWithOneColumn(xGlobal),
          fitResult2.MinimizingPoint,
          VectorMath.ToVector(yRest),
          null);
        VectorMath.AddScaled(yGlobal, yRest, -1, yRest); // yRest now contains the rest


        if (numericalProgress is { } numericalProgessNotNull)
        {
          numericalProgress.Report((numberOfTerms + 1) / (double)MaximumNumberOfPeaks);
        }

        if (textualProgress is { } textualProgessNotNull)
        {
          textualProgress.Report($"PeakFittingOfMultipleSpectra: {numberOfTerms + 1} of {MaximumNumberOfPeaks} fitted.");
        }

        if (cancellationToken.IsCancellationRequested)
          break;

      }

      // *************************************************
      // Prune Peaks
      // *************************************************

      // *************************************************
      // assemble the fit results
      // *************************************************

      var listOfPeaks = new List<PeakDescription>();
      var parameter = fitResult2.MinimizingPoint.ToArray();
      var numberOfPeaks = (fitResult2.MinimizingPoint.Count - (OrderOfBaselinePolynomial + 1) * numberOfSpectra) / (numberOfParametersPerPeakGlobal);
      fitFunction = fitFunction.WithNumberOfTerms(numberOfPeaks);

      var result = new MultipleSpectraPeakFittingResult
      {
        PeakDescriptions = listOfPeaks,
        FitFunction = fitFunction,
        NumberOfParametersPerPeak = numberOfParametersPerPeakLocal,
        NumberOfSpectra = numberOfSpectra,
        ParametersGlobal = parameter,
        XGlobal = xGlobal,
        YGlobal = yGlobal,
        StartIndicesOfSpectra = startIndicesOfSpectra,
        CovariancesGlobal = fitResult2.Covariance
      };

      for (int idxPeak = 0; idxPeak < numberOfPeaks; ++idxPeak)
      {
        var offset = idxPeak * numberOfParametersPerPeakGlobal;
        var peakAmplitudes = new double[numberOfSpectra];
        Array.Copy(parameter, offset, peakAmplitudes, 0, peakAmplitudes.Length);

        var peakParameter = new double[numberOfParametersPerPeakLocal - 1];
        Array.Copy(parameter, offset + numberOfSpectra, peakParameter, 0, peakParameter.Length);

        var pd = new PeakDescription()
        {
          Parent = result,
          OriginalPeakIndex = idxPeak,
        };

        listOfPeaks.Add(pd);
      }

      // sort the result list by position
      // note that when we sort the peaks, all the rest must also be sorted, thus
      listOfPeaks.Sort((x, y) => Comparer<double>.Default.Compare(x.Position, y.Position));




      return result;
    }
  }
}
