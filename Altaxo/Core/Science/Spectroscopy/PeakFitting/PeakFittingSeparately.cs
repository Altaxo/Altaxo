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
using System.Threading;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Science.Spectroscopy.PeakFitting
{
  /// <summary>
  /// Executes area normalization : y' = (y-min)/(mean), in which min and mean are the minimal and the mean values of the array.
  /// </summary>
  /// <seealso cref="Altaxo.Science.Spectroscopy.Normalization.INormalization" />
  public record PeakFittingSeparately : PeakFittingBase, IPeakFitting
  {
    #region Serialization

    #region Version 0

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.PeakFitting.PeakFittingSeparately", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingSeparately)obj;
        info.AddValue("FitFunction", s.FitFunction);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var fitFunction = info.GetValue<IFitFunctionPeak>("FitFunction", null);
        return new PeakFittingSeparately()
        {
          FitFunction = fitFunction,
        };
      }
    }

    #endregion

    #region Version 1

    /// <summary>
    /// 2022-08-06 V1: Added FitWidthScalingFactor
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakFittingSeparately), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingSeparately)obj;
        info.AddValue("FitFunction", s.FitFunction);
        info.AddValue("FitWidthScalingFactor", s.FitWidthScalingFactor);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var fitFunction = info.GetValue<IFitFunctionPeak>("FitFunction", null);
        var fitWidthScaling = info.GetDouble("FitWidthScalingFactor");
        return new PeakFittingSeparately()
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
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakFittingSeparately), 2)]
    public class SerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingSeparately)obj;
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

        return new PeakFittingSeparately()
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
        cancellationToken.ThrowIfCancellationRequested();
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

      var (minimalXDistance, maximalXDistance, minimalXValue, maximalXValue) = GetMinimalAndMaximalProperties(xArray);

      var userMinimalFWHM = this.IsMinimalFWHMValueInXUnits ? MinimalFWHMValue : MinimalFWHMValue * minimalXDistance;

      var (lowerBounds, upperBounds) = fitFunc.GetParameterBoundariesForPositivePeaks(
       minimalPosition: minimalXValue - 32 * maximalXDistance,
       maximalPosition: maximalXValue + 32 * maximalXDistance,
       minimalFWHM: Math.Max(userMinimalFWHM, minimalXDistance / 2d),
       maximalFWHM: maximalXDistance * 32d
       );



      var list = new List<PeakFitting.PeakDescription>();

      foreach (var description in peakDescriptions)
      {
        int first = (int)Math.Max(0, Math.Floor(description.PositionIndex - FitWidthScalingFactor * description.WidthPixels / 2));
        int last = (int)Math.Min(xArray.Length - 1, Math.Ceiling(description.PositionIndex + FitWidthScalingFactor * description.WidthPixels / 2));
        int len = last - first + 1;
        if (len < numberOfParametersPerPeak)
        {
          list.Add(new PeakDescription() { SearchDescription = description, Notes = "Width too small for fitting" });
          continue;
        }

        var xCut = new double[len];
        var yCut = new double[len];
        Array.Copy(xArray, first, xCut, 0, len);
        Array.Copy(yArray, first, yCut, 0, len);

        var xPosition = description.PositionValue;
        double xWidth = description.WidthValue;

        var initialHeight = Math.Max(description.Height, description.Prominence);
        var initialRelativeHeight = (description.Prominence / initialHeight) * description.RelativeHeightOfWidthDetermination;
        var param = fitFunc.GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(initialHeight, xPosition, xWidth, initialRelativeHeight);

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
        try
        {
          var fitResult = fit.Fit(xCut, yCut, param, lowerBounds, upperBounds, null, null, cancellationToken);

          list.Add(new PeakDescription
          {
            SearchDescription = description,
            FirstFitPoint = first,
            LastFitPoint = last,
            FirstFitPosition = xArray[first],
            LastFitPosition = xArray[last],
            PeakParameter = fitResult.MinimizingPoint.ToArray(),
            PeakParameterCovariances = fitResult.Covariance,
            FitFunction = fitFunc,
            FitFunctionParameter = fitResult.MinimizingPoint.ToArray(),
            SumChiSquare = fitResult.ModelInfoAtMinimum.Value,
            SigmaSquare = fitResult.ModelInfoAtMinimum.Value / (fitResult.ModelInfoAtMinimum.DegreeOfFreedom + 1),
          });
        }
        catch (Exception ex)
        {
          list.Add(new PeakDescription
          {
            SearchDescription = description,
            Notes = ex.Message,
            FirstFitPoint = -1,
            LastFitPoint = -1,
            FirstFitPosition = double.NaN,
            LastFitPosition = double.NaN,
            SumChiSquare = double.NaN,
            SigmaSquare = double.NaN,
          });
        }
      }

      return list;
    }
  }
}
