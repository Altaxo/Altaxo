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
  /// Fits all peaks that were found together in one single fitting function.
  /// </summary>
  public record PeakFittingTogether : PeakFittingBase, IPeakFitting
  {

    #region Serialization

    #region Version 0

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.PeakFitting.PeakFittingTogether", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingTogether)obj;
        info.AddValue("FitFunction", s.FitFunction);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var fitFunction = info.GetValue<IFitFunctionPeak>("FitFunction", null); return new PeakFittingTogether()
        {
          FitFunction = fitFunction,
        };
      }
    }

    #endregion

    #region Version 1

    /// <summary>
    /// 2022-08-06 Added FitWidthScalingFactor
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.PeakFitting.PeakFittingTogether", 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingTogether)obj;
        info.AddValue("FitFunction", s.FitFunction);
        info.AddValue("FitWidthScalingFactor", s.FitWidthScalingFactor);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var fitFunction = info.GetValue<IFitFunctionPeak>("FitFunction", null);
        var fitWidthScaling = info.GetDouble("FitWidthScalingFactor");
        return new PeakFittingTogether()
        {
          FitFunction = fitFunction,
          FitWidthScalingFactor = fitWidthScaling,
        };
      }
    }

    #endregion

    #region Version 2

    /// <summary>
    /// 2022-08-06 V1: Added FitWidthScalingFactor
    /// 2023-04-11 V2: Added IsMinimalFWHMValueInXUnits and MinimalFWHMValue
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakFittingTogether), 2)]
    public class SerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingTogether)obj;
        info.AddValue("FitFunction", s.FitFunction);
        info.AddValue("FitWidthScalingFactor", s.FitWidthScalingFactor);
        info.AddValue("IsMinimalFWHMValueInXUnits", s.IsMinimalFWHMValueInXUnits);
        info.AddValue("MinimalFWHMValue", s.MinimalFWHMValue);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var fitFunction = info.GetValue<IFitFunctionPeak>("FitFunction", null);
        var fitWidthScaling = info.GetDouble("FitWidthScalingFactor");
        var isMinimalFWHMValueInXUnits = info.GetBoolean("IsMinimalFWHMValueInXUnits");
        var minimalFWHMValue = info.GetDouble("MinimalFWHMValue");

        return new PeakFittingTogether()
        {
          FitFunction = fitFunction,
          FitWidthScalingFactor = fitWidthScaling,
          IsMinimalFWHMValueInXUnits = isMinimalFWHMValueInXUnits,
          MinimalFWHMValue = minimalFWHMValue,
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

    /// <inheritdoc/>
    public List<PeakDescription> Execute(double[] xArray, double[] yArray, IEnumerable<PeakSearching.PeakDescription> peakDescriptions, CancellationToken cancellationToken)
    {
      var fitFunc = FitFunction.WithNumberOfTerms(1);
      int numberOfParametersPerPeak = fitFunc.NumberOfParameters;

      var xyValues = new HashSet<(double X, double Y)>();
      var paramList = new List<double>();
      var dictionaryOfNotFittedPeaks = new Dictionary<PeakSearching.PeakDescription, PeakFitting.PeakDescription>();
      var peakParam = new List<(int FirstPoint, int LastPoint, double maximalXDistanceLocal, PeakSearching.PeakDescription Description)>();


      foreach (var description in peakDescriptions)
      {
        int first = SignalMath.GetIndexOfXInAscendingArray(xArray, description.PositionValue - FitWidthScalingFactor * description.WidthValue / 2, false);
        int last = SignalMath.GetIndexOfXInAscendingArray(xArray, description.PositionValue + FitWidthScalingFactor * description.WidthValue / 2, true);

        //int first = (int)Math.Max(0, Math.Floor(description.PositionIndex - FitWidthScalingFactor * description.WidthPixels / 2));
        //int last = (int)Math.Min(xArray.Length - 1, Math.Ceiling(description.PositionIndex + FitWidthScalingFactor * description.WidthPixels / 2));
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
      }

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

      var param = paramList.ToArray();
      fitFunc = FitFunction.WithNumberOfTerms(param.Length / numberOfParametersPerPeak);

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
      var list = GetPeakDescriptionList(xArray, yArray, peakDescriptions, fitFunc, numberOfParametersPerPeak, dictionaryOfNotFittedPeaks, peakParam, lowerBounds, upperBounds, fit, globalFitResult, cancellationToken);
      return list;
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
