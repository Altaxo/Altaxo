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

namespace Altaxo.Science.Spectroscopy.PeakFitting
{
  /// <summary>
  /// Fits all peaks that were found together in one single fitting function.
  /// Then, in order to get the variances for each single peak, each of the peaks it fitted again.
  /// </summary>
  public record PeakFittingTogetherWithSeparateVariances : PeakFittingTogether, IPeakFitting
  {
    #region Serialization

    #region Version 0

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.PeakFitting.PeakFittingTogetherWithSeparateVariances", 0)]
    public new class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.PeakFitting.PeakFittingTogetherWithSeparateVariances", 1)]
    public new class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

    #region Version 2

    /// <summary>
    /// 2022-08-06 V1: Added FitWidthScalingFactor
    /// 2023-04-11 V2: Added IsMinimalFWHMValueInXUnits and MinimalFWHMValue
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakFittingTogetherWithSeparateVariances), 2)]
    public new class SerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingTogetherWithSeparateVariances)obj;
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

        return new PeakFittingTogetherWithSeparateVariances()
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

    protected override List<PeakDescription> GetPeakDescriptionList(
      double[] xArray,
      double[] yArray,
      IEnumerable<PeakSearching.PeakDescription> peakDescriptions,
      IFitFunctionPeak fitFunc,
      int numberOfParametersPerPeak,
      Dictionary<PeakSearching.PeakDescription, PeakDescription> dictionaryOfNotFittedPeaks,
      List<(int FirstPoint, int LastPoint, double maximalXDistanceLocal, double minimalXValue, double maximalXValue, PeakSearching.PeakDescription Description)> peakParam,
      IReadOnlyList<double?>? lowerBounds,
      IReadOnlyList<double?>? upperBounds,
      QuickNonlinearRegression fit,
      NonlinearMinimizationResult globalFitResult,
      CancellationToken cancellationToken)
    {
      var param = globalFitResult.MinimizingPoint.ToArray();

      // in order to get the variances for each single peak separately:
      // 1.0 cut x and y to the area around that peak
      // 1.1 make the parameters fixed, which were fixed in the global fit (because they had reached a boundary).
      // 1.2 call fit with maximumNumberOfIterations=0, this will not fit, but only evaluate the result

      int idx = 0;
      var isFixed = Enumerable.Repeat(true, param.Length).ToArray();
      var parameterTemp = new double[param.Length];
      var parametersSeparate = new double[param.Length]; // Array to accomodate the parameter variances evaluated for each peak separately
      var standardErrorsSeparate = new double[param.Length]; // Array to accomodate the parameter variances evaluated for each peak separately
      var covariancesSeparate = new Matrix<double>[param.Length]; // Array of matrices that holds the covariances of each peak separately
      var sumChiSquareSeparate = new double[param.Length];
      var sigmaSquareSeparate = new double[param.Length];
      foreach (var (first, last, maxXDistance, minimalXValue, maximalXValue, description) in peakParam)
      {
        if (dictionaryOfNotFittedPeaks.ContainsKey(description))
          continue;

        int len = last - first + 1;
        var xCut = new double[len];
        var yCut = new double[len];
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

    public override string ToString()
    {
      return base.ToString();
    }
  }
}
