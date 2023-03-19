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
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Science.Signals;

namespace Altaxo.Science.Spectroscopy.PeakFitting
{
  /// <summary>
  /// Fits all peaks that were found together in one single fitting function.
  /// Then, in order to get the variances for each single peak, each of the peaks it fitted again.
  /// </summary>
  public record PeakFittingTogetherWithSeparateVariances : PeakFittingBase, IPeakFitting
  {
    #region Serialization

    #region Version 0

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.PeakFitting.PeakFittingTogetherWithSeparateVariances", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingTogetherWithSeparateVariances)obj;
        info.AddValue("FitFunction", s.FitFunction);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var fitFunction = info.GetValue<IFitFunctionPeak>("FitFunction", null); return new PeakFittingTogetherWithSeparateVariances()
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
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakFittingTogetherWithSeparateVariances), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingTogetherWithSeparateVariances)obj;
        info.AddValue("FitFunction", s.FitFunction);
        info.AddValue("FitWidthScalingFactor", s.FitWidthScalingFactor);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var fitFunction = info.GetValue<IFitFunctionPeak>("FitFunction", null);
        var fitWidthScaling = info.GetDouble("FitWidthScalingFactor");
        return new PeakFittingTogetherWithSeparateVariances()
        {
          FitFunction = fitFunction,
          FitWidthScalingFactor = fitWidthScaling,
        };
      }
    }

    #endregion


    #endregion


    public
      (
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
      var peakParam = new List<(int FirstPoint, int LastPoint, PeakSearching.PeakDescription Description)>();


      foreach (var description in peakDescriptions)
      {
        int first = SignalMath.GetIndexOfXInAscendingArray(xArray, description.PositionValue - FitWidthScalingFactor * description.WidthValue / 2, false);
        int last = SignalMath.GetIndexOfXInAscendingArray(xArray, description.PositionValue + FitWidthScalingFactor * description.WidthValue / 2, true);

        //int first = (int)Math.Max(0, Math.Floor(description.PositionIndex - FitWidthScalingFactor * description.WidthPixels / 2));
        //int last = (int)Math.Min(xArray.Length - 1, Math.Ceiling(description.PositionIndex + FitWidthScalingFactor * description.WidthPixels / 2));
        int len = last - first + 1;
        if (len < numberOfParametersPerPeak)
        {
          dictionaryOfNotFittedPeaks.Add(description, new PeakDescription() { Notes = "Width too small for fitting" });
          continue;
        }

        for (int i = first; i <= last; ++i)
          xyValues.Add((xArray[i], yArray[i]));

        var xPosition = description.PositionValue;
        var xWidth = description.WidthValue;

        var paras = fitFunc.GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(description.Prominence, xPosition, xWidth, description.RelativeHeightOfWidthDetermination);
        if (paras.Length != numberOfParametersPerPeak)
          throw new InvalidProgramException();
        foreach (var p in paras)
          paramList.Add(p);
        peakParam.Add((first, last, description));

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

      var (minimalXDistance, maximalXDistance, minimalXValue, maximalXValue) = GetMinimalAndMaximalProperties(xCut);
      var param = paramList.ToArray();
      fitFunc = FitFunction.WithNumberOfTerms(param.Length / numberOfParametersPerPeak);
      var (lowerBounds, upperBounds) = fitFunc.GetParameterBoundariesForPositivePeaks(
        minimalPosition: minimalXValue - 32 * maximalXDistance,
        maximalPosition: maximalXValue + 32 * maximalXDistance,
        minimalFWHM: minimalXDistance / 2d,
        maximalFWHM: maximalXDistance * 32d
        );

      var fit = new QuickNonlinearRegression(fitFunc);

      // In the first stage of the global fitting, we
      // fix the positions (peak positions are always 2nd parameter)
      // this is because the positions tend to run away as long as the other parameters
      // are far from their fitted values
      var isFixed = new bool[param.Length];
      for (int i = 1; i < isFixed.Length; i += numberOfParametersPerPeak)
        isFixed[i] = true;
      var globalFitResult = fit.Fit(xCut, yCut, param, lowerBounds, upperBounds, null, isFixed, cancellationToken);
      param = globalFitResult.MinimizingPoint.ToArray();

      // In the second stage of the global fitting, we
      // now leave all parameters free
      globalFitResult = fit.Fit(xCut, yCut, param, lowerBounds, upperBounds, null, null, cancellationToken);
      param = globalFitResult.MinimizingPoint.ToArray();
      var fitFunctionWrapper = new PeakFitFunctions.FunctionWrapper(fitFunc, param);

      // in order to get the variances for each single peak separately:
      // 1.0 cut x and y to the area around that peak
      // 1.1 make the parameters fixed, which were fixed in the global fit (because they had reached a boundary).
      // 1.2 call fit with maximumNumberOfIterations=0, this will not fit, but only evaluate the result

      idx = 0;
      isFixed = Enumerable.Repeat(true, param.Length).ToArray();
      var parameterTemp = new double[param.Length];
      var parametersSeparate = new double[param.Length]; // Array to accomodate the parameter variances evaluated for each peak separately
      var standardErrorsSeparate = new double[param.Length]; // Array to accomodate the parameter variances evaluated for each peak separately
      var covariancesSeparate = new Matrix<double>[param.Length]; // Array of matrices that holds the covariances of each peak separately
      var sumChiSquareSeparate = new double[param.Length];
      var sigmaSquareSeparate = new double[param.Length];
      foreach (var (first, last, description) in peakParam)
      {
        if (dictionaryOfNotFittedPeaks.ContainsKey(description))
          continue;

        int len = last - first + 1;
        xCut = new double[len];
        yCut = new double[len];
        Array.Copy(xArray, first, xCut, 0, len);
        Array.Copy(yArray, first, yCut, 0, len);

        // Save parameter
        Array.Copy(param, 0, parameterTemp, 0, param.Length);
        // unfix our set of parameters
        for (int i = 0; i < numberOfParametersPerPeak; i++)
        {
          isFixed[idx + i] = globalFitResult.IsFixedByUserOrBoundaries[idx + i];
        }
        fit.MaximumNumberOfIterations = 0; // set maximum number of iterations to 0. This causes only to evaluate the results. The parameters will be not changed.
        var localFitResult = fit.Fit(xCut, yCut, param, lowerBounds, upperBounds, null, isFixed, cancellationToken);
        // fix again our set of parameters
        for (int i = 0; i < numberOfParametersPerPeak; i++)
          isFixed[idx + i] = true;
        // Restore parameter
        Array.Copy(parameterTemp, 0, param, 0, param.Length);

        for (int i = 0; i < numberOfParametersPerPeak; i++)
        {
          parametersSeparate[idx + i] = localFitResult.MinimizingPoint[idx + i];
          standardErrorsSeparate[idx + i] = localFitResult.StandardErrors is null ? 0 : localFitResult.StandardErrors[idx + i];
        }

        // extract the covariance matrix
        var covMatrix = CreateMatrix.Dense<double>(numberOfParametersPerPeak, numberOfParametersPerPeak);
        for (int i = 0; i < numberOfParametersPerPeak; i++)
          for (int j = 0; j < numberOfParametersPerPeak; ++j)
            covMatrix[i, j] = localFitResult.Covariance is null ? 0 : localFitResult.Covariance[idx + i, idx + j];
        covariancesSeparate[idx / numberOfParametersPerPeak] = covMatrix;
        sumChiSquareSeparate[idx / numberOfParametersPerPeak] = localFitResult.ModelInfoAtMinimum.Value;
        sigmaSquareSeparate[idx / numberOfParametersPerPeak] = localFitResult.ModelInfoAtMinimum.Value / (localFitResult.ModelInfoAtMinimum.DegreeOfFreedom + 1);

        idx += numberOfParametersPerPeak;
      }

      var list = new List<PeakFitting.PeakDescription>();

      idx = 0;
      foreach (var description in peakDescriptions)
      {
        if (dictionaryOfNotFittedPeaks.TryGetValue(description, out var fitDescription))
        {
          list.Add(fitDescription);
        }
        else
        {
          list.Add(new PeakDescription
          {
            SearchDescription = description,
            FirstFitPoint = peakParam[idx].FirstPoint,
            LastFitPoint = peakParam[idx].LastPoint,
            FirstFitPosition = xArray[peakParam[idx].FirstPoint],
            LastFitPosition = xArray[peakParam[idx].LastPoint],
            PeakParameter = parametersSeparate.Skip(idx * numberOfParametersPerPeak).Take(numberOfParametersPerPeak).ToArray(),
            PeakParameterCovariances = covariancesSeparate[idx],
            FitFunction = fitFunc,
            FitFunctionParameter = (double[])param.Clone(),
            SumChiSquare = sumChiSquareSeparate[idx],
            SigmaSquare = sigmaSquareSeparate[idx],
          });
          ++idx;
        }
      }

      return list;
    }
  }
}
