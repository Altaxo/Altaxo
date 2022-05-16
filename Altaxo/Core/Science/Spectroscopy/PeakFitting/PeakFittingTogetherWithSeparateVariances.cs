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
using Altaxo.Calc;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Science.Spectroscopy.PeakFitting
{
  /// <summary>
  /// Fits all peaks that were found together in one single fitting function.
  /// Then, in order to get the variances for each single peak, each of the peaks it fitted again.
  /// </summary>
  public record PeakFittingTogetherWithSeparateVariances : IPeakFitting
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakFittingTogetherWithSeparateVariances), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingTogetherWithSeparateVariances)obj;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var minimalProminence = info.GetNullableDouble("MinimalProminence");
        return new PeakFittingTogetherWithSeparateVariances()
        {
        };
      }
    }
    #endregion

    /// <inheritdoc/>
    public IPeakFittingResult Execute(double[] xArray, double[] yArray, IEnumerable<PeakSearching.PeakDescription> peakDescriptions)
    {
      var fitFunc = new PeakFitFunctions.Gauss(1);
      int numberOfParametersPerPeak = 3;

      var xyValues = new HashSet<(double X, double Y)>();
      var paramList  = new List<double>();
      var dictionaryOfNotFittedPeaks = new Dictionary<PeakSearching.PeakDescription, PeakFitting.PeakDescription>();

      foreach (var description in peakDescriptions)
      {
        int first = (int)Math.Max(0, description.PositionIndex - description.Width / 2);
        int last = (int)Math.Min(xArray.Length - 1, description.PositionIndex + description.Width / 2);
        int len = last - first + 1;
        if (len < numberOfParametersPerPeak)
        {
          dictionaryOfNotFittedPeaks.Add(description, new PeakDescription() { Notes = "Width too small for fitting" });
          continue;
        }

        for (int i = first; i<= last; ++i)
          xyValues.Add((xArray[i], yArray[i]));

        var xPosition = RMath.InterpolateLinear(description.PositionIndex, xArray);
        var xWidth = Math.Abs(RMath.InterpolateLinear(description.PositionIndex + 0.5 * description.Width, xArray) -
          RMath.InterpolateLinear(description.PositionIndex - 0.5 * description.Width, xArray));

        paramList.Add(description.Height);
        paramList.Add(xPosition);
        paramList.Add(fitFunc.GetWidthParameterFromWidthAtRelativeHeight(xWidth, description.AbsoluteHeightOfWidthDetermination / description.Height));

      }

      var xCut = new double[xyValues.Count];
      var yCut = new double[xyValues.Count];
      int idx = 0;
      foreach(var xy in xyValues)
      {
        xCut[idx] = xy.X;
        yCut[idx] = xy.Y;
        idx++;
      }

      var param = paramList.ToArray();
      fitFunc = new PeakFitFunctions.Gauss(param.Length/3);
      var fit = new QuickNonlinearRegression(fitFunc);
      param = fit.Fit(xCut, yCut, param);
      var fitFunctionWrapper = new PeakFitFunctions.FunctionWrapper(fitFunc, param);

      // in order to get the variances for each single peak separately:
      // 1.  fit again, but
      // 1.1 for each peak, only vary the parameters of that peak; fix all other parameters
      // 1.2 do not use all x-values, but only the x-values around that peak

      idx=0;
      var isFixed = Enumerable.Repeat(true, param.Length).ToArray();
      var parameterTemp = new double[param.Length];
      var variancesSeparate = new double[param.Length]; // Array to accomodate the parameter variances evaluated for each peak separately
      foreach (var description in peakDescriptions)
      {
        if (dictionaryOfNotFittedPeaks.ContainsKey(description))
          continue;

        int first = (int)Math.Max(0, description.PositionIndex - description.Width / 2);
        int last = (int)Math.Min(xArray.Length - 1, description.PositionIndex + description.Width / 2);
        int len = last - first + 1;
        xCut = new double[len];
        yCut = new double[len];
        Array.Copy(xArray, first, xCut, 0, len);
        Array.Copy(yArray, first, yCut, 0, len);

        var xPosition = RMath.InterpolateLinear(description.PositionIndex, xArray);
        var xWidth = Math.Abs(RMath.InterpolateLinear(description.PositionIndex + 0.5 * description.Width, xArray) -
          RMath.InterpolateLinear(description.PositionIndex - 0.5 * description.Width, xArray));

        // Save parameter
        Array.Copy(param, 0, parameterTemp, 0, param.Length);
        // unfix our set of parameters
        for (int i = 0; i < numberOfParametersPerPeak; i++)
          isFixed[idx + i] = false;
        fit.Fit(xCut, yCut, param, isFixed);
        // fix again our set of parameters
        for (int i = 0; i < numberOfParametersPerPeak; i++)
          isFixed[idx + i] = true;
        // Restore parameter
        Array.Copy(parameterTemp, 0, param, 0, param.Length);

        for(int i = 0; i < numberOfParametersPerPeak; i++)
          variancesSeparate[idx + i] = fit.ParameterVariances[idx + i];

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
            Height = param[idx + 0],
            HeightVariance = variancesSeparate[idx + 0],
            PositionInXUnits = param[idx + 1],
            PositionInXUnitsVariance = variancesSeparate[idx + 1],
            Width = param[idx + 2],
            WidthVariance = variancesSeparate[idx + 2],
            FitFunction = fitFunctionWrapper,
          }); ;
          idx += numberOfParametersPerPeak;
        }
      }

      return new Result()
      {
        PeakDescriptions = list
      };
    }

    #region Result

    class Result : IPeakFittingResult
    {
      IReadOnlyList<PeakDescription> _description = new PeakDescription[0];

      public IReadOnlyList<PeakDescription> PeakDescriptions
      {
        get => _description;
        init => _description = value ?? throw new ArgumentNullException();
      }


    }
    #endregion
  }
}
