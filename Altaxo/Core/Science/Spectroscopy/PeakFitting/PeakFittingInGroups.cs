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
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Science.Signals;

namespace Altaxo.Science.Spectroscopy.PeakFitting
{
  /// <summary>
  /// Groups the peaks that were found by using a minimal separation factor (based on the FWHM of the peaks).
  /// Then fits the peak groups.
  /// </summary>
  public record PeakFittingInGroups : PeakFittingBase, IPeakFitting
  {
    private double _minimalGroupSeparationFWHMFactor = 3;

    public double MinimalGroupSeparationFWHMFactor
    {
      get
      {
        return _minimalGroupSeparationFWHMFactor;
      }
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("Value has to be >= 0", nameof(MinimalGroupSeparationFWHMFactor));

        _minimalGroupSeparationFWHMFactor = value;
      }
    }

    public double _maximalRelativeAmplitudeInfluence = double.PositiveInfinity;

    /// <summary>
    /// Gets the maximal relative amplitude influence. If we have two neighbouring peaks, the one peak should not influence the amplitude
    /// at the most close point of the other peak (that is included in the fit) by more that this value times the expected height of the other peak at this point.
    /// </summary>
    public double MaximalRelativeAmplitudeInfluence
    {
      get
      {
        return _maximalRelativeAmplitudeInfluence;
      }
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException("Value has to be > 0", nameof(MaximalRelativeAmplitudeInfluence));

        _maximalRelativeAmplitudeInfluence = value;
      }
    }

    private int _minimalOrderOfBaselinePolynomial = -1;
    /// <summary>
    /// Gets or sets the minimal order of the polynomial that is used for the baseline of each group.
    /// The minimal order is applied if the group only contains one peak. As it contains more peaks,
    /// the order is increased, until the <see cref="MaximalOrderOfBaselinePolynomial"/> is reached if the
    /// group contains <see cref="NumberOfPeaksAtMaximalOrderOfBaselinePolynomial"/> peaks.
    /// </summary>
    /// <value>
    /// The minimal order of the baseline polynomial.
    /// </value>
    public int MinimalOrderOfBaselinePolynomial
    {
      get { return _minimalOrderOfBaselinePolynomial; }
      init
      {
        _minimalOrderOfBaselinePolynomial = Math.Max(-1, value);
      }
    }

    private int _maximalOrderOfBaselinePolynomial = -1;

    /// <summary>
    /// Gets or sets the maximal order of the polynomial that is used for the baseline of each group.
    /// If the group only contains one peak, then the <see cref="MinimalOrderOfBaselinePolynomial"/> is applied. As it contains more peaks,
    /// the order is increased, until the <see cref="MaximalOrderOfBaselinePolynomial"/> is reached if the
    /// group contains <see cref="NumberOfPeaksAtMaximalOrderOfBaselinePolynomial"/> peaks.
    /// </summary>
    /// <value>
    /// The maximal order of the baseline polynomial.
    /// </value>
    public int MaximalOrderOfBaselinePolynomial
    {
      get { return _maximalOrderOfBaselinePolynomial; }
      init
      {
        _maximalOrderOfBaselinePolynomial = Math.Max(-1, value);
      }
    }

    private int _numberOfPeaksAtMaximalOrderOfBaselinePolynomial = 3;

    /// <summary>
    /// Gets or sets the maximal order of the polynomial that is used for the baseline of each group.
    /// If the group only contains one peak, then the <see cref="MinimalOrderOfBaselinePolynomial"/> is applied. As it contains more peaks,
    /// the order is increased, until the <see cref="MaximalOrderOfBaselinePolynomial"/> is reached if the
    /// group contains <see cref="NumberOfPeaksAtMaximalOrderOfBaselinePolynomial"/> peaks.
    /// </summary>
    /// <value>
    /// The number of peaks in the group at which the maximal order of the baseline polynomial is reached.
    /// </value>
    public int NumberOfPeaksAtMaximalOrderOfBaselinePolynomial
    {
      get { return _numberOfPeaksAtMaximalOrderOfBaselinePolynomial; }
      init
      {
        _numberOfPeaksAtMaximalOrderOfBaselinePolynomial = Math.Max(2, value);
      }
    }

    public bool IsEvaluatingSeparateVariances { get; set; }

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2024-03-25 V1: initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakFittingInGroups), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingInGroups)obj;
        info.AddValue("FitFunction", s.FitFunction);
        info.AddValue("FitWidthScalingFactor", s.FitWidthScalingFactor);
        info.AddValue("IsMinimalFWHMValueInXUnits", s.IsMinimalFWHMValueInXUnits);
        info.AddValue("MinimalFWHMValue", s.MinimalFWHMValue);
        info.AddValue("MinimalGroupSeparationFWHMFactor", s.MinimalGroupSeparationFWHMFactor);
        info.AddValue("MaximalRelativeAmplitudeInfluence", s.MaximalRelativeAmplitudeInfluence);
        info.AddValue("EvaluateSeparateVariances", s.IsEvaluatingSeparateVariances);
        info.AddValue("MinOrderOfBaselinePolynomial", s.MinimalOrderOfBaselinePolynomial);
        info.AddValue("MaxOrderOfBaselinePolynomial", s.MaximalOrderOfBaselinePolynomial);
        info.AddValue("NumberOfPeaksAtMaxOrderOfBaselinePolynomial", s.NumberOfPeaksAtMaximalOrderOfBaselinePolynomial);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var fitFunction = info.GetValue<IFitFunctionPeak>("FitFunction", null);
        var fitWidthScaling = info.GetDouble("FitWidthScalingFactor");
        var isMinimalFWHMValueInXUnits = info.GetBoolean("IsMinimalFWHMValueInXUnits");
        var minimalFWHMValue = info.GetDouble("MinimalFWHMValue");
        var minimalGroupSeparationFWHMFactor = info.GetDouble("MinimalGroupSeparationFWHMFactor");
        var maximalRelativeAmplitudeInfluence = info.GetDouble("MaximalRelativeAmplitudeInfluence");
        var isEvaluatingSeparateVariances = info.GetBoolean("EvaluateSeparateVariances");
        var minOrderOfBaselinePolynomial = info.GetInt32("MinOrderOfBaselinePolynomial");
        var maxOrderOfBaselinePolynomial = info.GetInt32("MaxOrderOfBaselinePolynomial");
        var numberOfPeaksAtMaxOrderOfBaselinePolynomial = info.GetInt32("NumberOfPeaksAtMaxOrderOfBaselinePolynomial");

        return new PeakFittingInGroups()
        {
          FitFunction = fitFunction,
          MinimalOrderOfBaselinePolynomial = minOrderOfBaselinePolynomial,
          MaximalOrderOfBaselinePolynomial = maxOrderOfBaselinePolynomial,
          NumberOfPeaksAtMaximalOrderOfBaselinePolynomial = numberOfPeaksAtMaxOrderOfBaselinePolynomial,
          FitWidthScalingFactor = fitWidthScaling,
          IsMinimalFWHMValueInXUnits = isMinimalFWHMValueInXUnits,
          MinimalFWHMValue = minimalFWHMValue,
          MinimalGroupSeparationFWHMFactor = minimalGroupSeparationFWHMFactor,
          MaximalRelativeAmplitudeInfluence = maximalRelativeAmplitudeInfluence,
          IsEvaluatingSeparateVariances = isEvaluatingSeparateVariances,
        };
      }
    }

    #endregion

    #endregion

    public (
      double[] x,
      double[] y,
      int[]? regions,
      IReadOnlyList<(IReadOnlyList<PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> peakFittingResults
    ) Execute(double[] xArray, double[] yArray, int[]? regions, IReadOnlyList<(IReadOnlyList<PeakSearching.PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> peakDescriptions, CancellationToken cancellationToken)
    {
      var peakFitDescriptions = new List<(IReadOnlyList<PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)>();
      foreach (var (peakDesc, start, end) in peakDescriptions)
      {
        var subX = new double[end - start];
        var subY = new double[end - start];
        Array.Copy(xArray, start, subX, 0, end - start);
        Array.Copy(yArray, start, subY, 0, end - start);
        var result = Execute(subX, subY, peakDesc, cancellationToken);
        peakFitDescriptions.Add((result, start, end));
      }

      return (xArray, yArray, regions, peakFitDescriptions);
    }

    private static bool Intersects((double Lower, double Upper) x, (double Lower, double Upper) y)
    {
      return y.Upper > x.Lower && x.Upper > y.Lower;
    }

    public List<List<PeakSearching.PeakDescription>> GroupPeaks(IEnumerable<PeakSearching.PeakDescription> peaks)
    {
      var result = new List<(List<PeakSearching.PeakDescription> Descriptions, double Lower, double Upper)>();


      var peaksAndRanges = peaks.Select(p =>
      {
        var fwhm = GaussAmplitude.GetFWHMFromWidthAndRelativeHeight(p.WidthValue, p.RelativeHeightOfWidthDetermination);
        return (Peak: p, Lower: p.PositionValue - MinimalGroupSeparationFWHMFactor * fwhm, Upper: p.PositionValue + MinimalGroupSeparationFWHMFactor * fwhm);
      }).ToList();

      while (peaksAndRanges.Count > 0)
      {
        var group = new List<(PeakSearching.PeakDescription Peak, double Lower, double Upper)>();
        group.Add(peaksAndRanges[^1]);
        var currentRange = (peaksAndRanges[^1].Lower, peaksAndRanges[^1].Upper);
        peaksAndRanges.RemoveAt(peaksAndRanges.Count - 1);

        int groupCountBefore;
        do
        {
          groupCountBefore = group.Count;
          for (int i = peaksAndRanges.Count - 1; i >= 0; --i)
          {
            if (Intersects(currentRange, (peaksAndRanges[i].Lower, peaksAndRanges[i].Upper)))
            {
              currentRange = (Math.Min(currentRange.Lower, peaksAndRanges[i].Lower), Math.Max(currentRange.Upper, peaksAndRanges[i].Upper));
              group.Add(peaksAndRanges[i]);
              peaksAndRanges.RemoveAt(i);
            }
          }
        } while (group.Count != groupCountBefore);

        group.Sort((x, y) => Comparer<double>.Default.Compare(x.Peak.PositionValue, y.Peak.PositionValue));
        result.Add((group.Select(ge => ge.Peak).ToList(), currentRange.Lower, currentRange.Upper));
      }

      result.Sort((x, y) => Comparer<double>.Default.Compare(x.Lower, y.Lower));
      return result.Select(l => l.Descriptions).ToList();
    }

    /// <inheritdoc/>
    public List<PeakDescription> Execute(double[] xArray, double[] yArray, IEnumerable<PeakSearching.PeakDescription> peakDescriptions, CancellationToken cancellationToken)
    {
      var result = new List<PeakDescription>();

      var groups = GroupPeaks(peakDescriptions);
      foreach (var group in groups)
      {
        if (cancellationToken.IsCancellationRequested)
        { break; }

        var list = ExecuteForOneGroup(xArray, yArray, group, cancellationToken);
        result.AddRange(list);
      }

      return result;
    }

    /// <summary>Executes peak fitting for peaks in one group</summary>
    /// <param name="xArray"></param>
    public List<PeakDescription> ExecuteForOneGroup(double[] xArray, double[] yArray, IEnumerable<PeakSearching.PeakDescription> peakDescriptions, CancellationToken cancellationToken)
    {
      var fitFunc = FitFunction.WithNumberOfTerms(1);
      int numberOfParametersPerPeak = fitFunc.NumberOfParameters;

      var xyValues = new HashSet<(double X, double Y)>();
      var paramList = new List<double>();
      var dictionaryOfNotFittedPeaks = new Dictionary<PeakSearching.PeakDescription, PeakFitting.PeakDescription>();
      var peakParam = new List<(int FirstPoint, int LastPoint, double maximalXDistanceLocal, PeakSearching.PeakDescription Description)>();



      int numberOfPeaks = 0;
      foreach (var description in peakDescriptions)
      {
        int first = SignalMath.GetIndexOfXInAscendingArray(xArray, description.PositionValue - FitWidthScalingFactor * description.WidthValue / 2, false);
        int last = SignalMath.GetIndexOfXInAscendingArray(xArray, description.PositionValue + FitWidthScalingFactor * description.WidthValue / 2, true);
        int len = last - first + 1;
        if (len < numberOfParametersPerPeak)
        {
          dictionaryOfNotFittedPeaks.Add(description, new PeakDescription() { SearchDescription = description, Notes = "Width too small for fitting", FirstFitPoint = first, LastFitPoint = last });
          continue;
        }

        for (int i = first; i <= last; ++i)
          xyValues.Add((xArray[i], yArray[i]));

        var xPosition = description.PositionValue;
        var xWidth = description.WidthValue;

        var paras = fitFunc.GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(description.Prominence, xPosition, xWidth, description.RelativeHeightOfWidthDetermination);
        if (paras.Length != numberOfParametersPerPeak)
        {
          throw new InvalidProgramException();
        }
        foreach (var p in paras)
        {
          paramList.Add(p);
        }
        var (_, maxXDistance, _, _) = SignalMath.GetMinimalAndMaximalProperties(new ReadOnlySpan<double>(xArray, first, last - first + 1));
        peakParam.Add((first, last, maxXDistance, description));
        ++numberOfPeaks;
      }

      if (numberOfPeaks == 0) // no peaks could be fitted 
      {
        return dictionaryOfNotFittedPeaks.Values.ToList();
      }

      var orderOfBaselinePolynomial = numberOfPeaks >= NumberOfPeaksAtMaximalOrderOfBaselinePolynomial ? MaximalOrderOfBaselinePolynomial :
                                      (int)Math.Round(MinimalOrderOfBaselinePolynomial + (MaximalOrderOfBaselinePolynomial - MinimalOrderOfBaselinePolynomial) * ((numberOfPeaks - 1.0) / (NumberOfPeaksAtMaximalOrderOfBaselinePolynomial - 1.0)));

      var xCut = new double[xyValues.Count];
      var yCut = new double[xyValues.Count];
      int idx = 0;
      foreach (var xy in xyValues)
      {
        xCut[idx] = xy.X;
        yCut[idx] = xy.Y;
        idx++;
      }

      var (minimalXDistanceGlobal, maximalXDistanceGlobal, minimalXValueGlobal, maximalXValueGlobal) =
        SignalMath.GetMinimalAndMaximalProperties(xCut);

      // add parameters for baseline polynomial
      for (int i = 0; i <= orderOfBaselinePolynomial; ++i)
      {
        paramList.Add(0.0);
      }

      var param = paramList.ToArray();
      fitFunc = FitFunction.WithNumberOfTerms(param.Length / numberOfParametersPerPeak).WithOrderOfBaselinePolynomial(orderOfBaselinePolynomial);

      IReadOnlyList<double?>? lowerBounds, upperBounds;
      if (IsMinimalFWHMValueInXUnits)
      {
        // if the minimal FWHM value is given in x-units, 
        // then we can use it directly to specify the lower boundary
        (lowerBounds, upperBounds) = fitFunc.GetParameterBoundariesForPositivePeaks(
          minimalPosition: minimalXValueGlobal - 32 * (maximalXValueGlobal - minimalXValueGlobal),
          maximalPosition: maximalXValueGlobal + 32 * (maximalXValueGlobal - minimalXValueGlobal),
          minimalFWHM: Math.Max(MinimalFWHMValue, minimalXDistanceGlobal / 2d),
          maximalFWHM: (maximalXValueGlobal - minimalXValueGlobal) * 32d
          );
      }
      else
      {
        // if the minimal FWHM value is given in points, we use the maximalXDistance between the points
        // in the range of the peak (thus the local maximalXDistance, not the global maximalXDistance)
        var lowerBoundsArr = new double?[param.Length];
        var upperBoundsArr = new double?[param.Length];
        lowerBounds = lowerBoundsArr;
        upperBounds = upperBoundsArr;

        for (int i = 0; i < peakParam.Count; ++i)
        {
          var peakP = peakParam[i];
          var (localLowerBounds, localUpperBounds) = FitFunction.GetParameterBoundariesForPositivePeaks(
            minimalPosition: minimalXValueGlobal - 32 * (maximalXValueGlobal - minimalXValueGlobal),
            maximalPosition: maximalXValueGlobal + 32 * (maximalXValueGlobal - minimalXValueGlobal),
            minimalFWHM: Math.Max(MinimalFWHMValue * peakP.maximalXDistanceLocal, minimalXDistanceGlobal / 2d),
            maximalFWHM: (maximalXValueGlobal - minimalXValueGlobal) * 32d);
          if (localLowerBounds is not null)
          {
            VectorMath.Copy(localLowerBounds, 0, lowerBoundsArr, i * numberOfParametersPerPeak, numberOfParametersPerPeak);
          }
          if (localUpperBounds is not null)
          {
            VectorMath.Copy(localUpperBounds, 0, upperBoundsArr, i * numberOfParametersPerPeak, numberOfParametersPerPeak);
          }
        }
      }

      // clamp parameters in order to meet lowerBounds
      if (lowerBounds is not null)
      {
        for (int i = 0; i < param.Length; ++i)
        {
          var lb = lowerBounds[i];
          if (lb.HasValue && param[i] < lb.Value)
            param[i] = lb.Value;
        }
      }

      // clamp parameters in order to meet upperBounds
      if (upperBounds is not null)
      {
        for (int i = 0; i < param.Length; ++i)
        {
          var ub = upperBounds[i];
          if (ub.HasValue && param[i] > ub.Value)
            param[i] = ub.Value;
        }
      }

      // add parameters

      var fit = new QuickNonlinearRegression(fitFunc);

      // In the first stage of the global fitting, we
      // fix the positions (peak positions are always 2nd parameter)
      // this is because the positions tend to run away as long as the other parameters
      // are far from their fitted values
      var isFixed = new bool[param.Length];
      for (int i = 0; i < isFixed.Length; ++i)
        isFixed[i] = (0 != (i % numberOfParametersPerPeak)); // fix everything but the amplitude

      var globalFitResult = fit.Fit(xCut, yCut, param, lowerBounds, upperBounds, null, isFixed, cancellationToken);
      param = globalFitResult.MinimizingPoint.ToArray();

      // In the second stage of the global fitting, we
      // now leave all parameters free
      globalFitResult = fit.Fit(xCut, yCut, param, lowerBounds, upperBounds, null, null, cancellationToken);
      param = globalFitResult.MinimizingPoint.ToArray();
      var isFixedByUserOrBoundaries = globalFitResult.IsFixedByUserOrBoundaries;


      if (!IsEvaluatingSeparateVariances)
      {

        var list = GetPeakDescriptionList(xArray, yArray, peakDescriptions, fitFunc, numberOfParametersPerPeak, dictionaryOfNotFittedPeaks, peakParam, lowerBounds, upperBounds, fit, globalFitResult, cancellationToken);
        return list;
      }
      else // we calculate separate variances for each peak
      {
        var list = new List<PeakDescription>();
        fit = new QuickNonlinearRegression(fitFunc)
        {
          MaximumNumberOfIterations = 0
        };

        isFixed = Enumerable.Repeat(true, param.Length).ToArray();
        var parameterTemp = new double[param.Length];
        var parametersSeparate = new double[param.Length]; // Array to accomodate the parameter variances evaluated for each peak separately

        for (int i = 0, j = 0; i < numberOfPeaks; ++i, j += numberOfParametersPerPeak)
        {
          var parametersForThisPeak = new double[numberOfParametersPerPeak]; // fresh array, because it becomes part of the result!
          Array.Copy(param, j, parametersForThisPeak, 0, numberOfParametersPerPeak);

          var (position, area, height, fwhm) = fitFunc.GetPositionAreaHeightFWHMFromSinglePeakParameters(parametersForThisPeak);

          var firstX = position - FitWidthScalingFactor * fwhm / 2;
          var lastX = position + FitWidthScalingFactor * fwhm / 2;
          int first = SignalMath.GetIndexOfXInAscendingArray(xArray, firstX, roundUp: false);
          int last = SignalMath.GetIndexOfXInAscendingArray(xArray, lastX, roundUp: true);
          int len = last - first + 1;
          xCut = new double[len];
          yCut = new double[len];
          Array.Copy(xArray, first, xCut, 0, len);
          Array.Copy(yArray, first, yCut, 0, len);

          // Save parameter
          Array.Copy(param, 0, parameterTemp, 0, param.Length);

          // unfix our set of parameters
          for (int k = 0; k < numberOfParametersPerPeak; k++)
          {
            isFixed[j + k] = isFixedByUserOrBoundaries[j + k];
          }

          var localFitResult = fit.Fit(xCut, yCut, param, null, null, null, isFixed, cancellationToken);

          // fix again our set of parameters
          for (int k = 0; k < numberOfParametersPerPeak; k++)
          {
            isFixed[j + k] = true;
          }

          // Restore parameter
          Array.Copy(parameterTemp, 0, param, 0, param.Length);

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
              WidthPixels = SignalMath.GetIndexOfXInAscendingArray(xArray, position + 0.5 * fwhm, roundUp: null) - SignalMath.GetIndexOfXInAscendingArray(xArray, position - 0.5 * fwhm, roundUp: null),
              PositionIndex = SignalMath.GetIndexOfXInAscendingArray(xArray, position, roundUp: null),
            },
            FitFunction = fitFunc,
            FirstFitPoint = first,
            LastFitPoint = last,
            FirstFitPosition = xCut[0],
            LastFitPosition = xCut[^1],
            FitFunctionParameter = param,
            PeakParameter = parametersForThisPeak, // save because it was freshly allocated in this loop
            Notes = string.Empty,
            PeakParameterCovariances = covMatrix,
            SumChiSquare = localFitResult.ModelInfoAtMinimum.Value,
            SigmaSquare = localFitResult.ModelInfoAtMinimum.Value / (localFitResult.ModelInfoAtMinimum.DegreeOfFreedom + 1),
          };
          list.Add(desc);
        }
        return list;
      }
    }

    protected virtual List<PeakDescription> GetPeakDescriptionList(
      double[] xArray,
      double[] yArray,
      IEnumerable<PeakSearching.PeakDescription> peakDescriptions,
      IFitFunctionPeak fitFunc,
      int numberOfParametersPerPeak,
      Dictionary<PeakSearching.PeakDescription, PeakDescription> dictionaryOfNotFittedPeaks,
      List<(int FirstPoint, int LastPoint, double maximalXDistanceLocal, PeakSearching.PeakDescription Description)> peakParam,
      IReadOnlyList<double?>? lowerBounds,
      IReadOnlyList<double?>? upperBounds,
      QuickNonlinearRegression fit,
      NonlinearMinimizationResult globalFitResult,
      CancellationToken cancellationToken)
    {
      var param = globalFitResult.MinimizingPoint.ToArray();
      var list = new List<PeakFitting.PeakDescription>();
      int idx = 0;
      foreach (var description in peakDescriptions)
      {
        if (dictionaryOfNotFittedPeaks.TryGetValue(description, out var fitDescription))
        {
          list.Add(fitDescription);
        }
        else
        {
          var localCov = globalFitResult.Covariance.SubMatrix(idx * numberOfParametersPerPeak, numberOfParametersPerPeak, idx * numberOfParametersPerPeak, numberOfParametersPerPeak);

          list.Add(new PeakDescription
          {
            SearchDescription = description,
            FirstFitPoint = peakParam[idx].FirstPoint,
            LastFitPoint = peakParam[idx].LastPoint,
            FirstFitPosition = xArray[peakParam[idx].FirstPoint],
            LastFitPosition = xArray[peakParam[idx].LastPoint],
            PeakParameter = globalFitResult.MinimizingPoint.Skip(idx * numberOfParametersPerPeak).Take(numberOfParametersPerPeak).ToArray(),
            PeakParameterCovariances = localCov,
            FitFunction = fitFunc,
            FitFunctionParameter = globalFitResult.MinimizingPoint.ToArray(),
            SumChiSquare = globalFitResult.ModelInfoAtMinimum.Value,
            SigmaSquare = globalFitResult.ModelInfoAtMinimum.Value / (globalFitResult.ModelInfoAtMinimum.DegreeOfFreedom + 1),
          });
          ++idx;
        }
      }

      return list;
    }

    public override string ToString()
    {
      return $"{this.GetType().Name} Func={FitFunction} FitWidth={FitWidthScalingFactor} MinFWHM={MinimalFWHMValue}{(IsMinimalFWHMValueInXUnits ? 'X' : 'P')}";
    }
  }
}
