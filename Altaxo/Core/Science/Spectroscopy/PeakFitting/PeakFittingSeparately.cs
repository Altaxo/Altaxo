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
  /// Executes area normalization : y' = (y-min)/(mean), in which min and mean are the minimal and the mean values of the array.
  /// </summary>
  /// <seealso cref="Altaxo.Science.Spectroscopy.Normalization.INormalization" />
  public record PeakFittingSeparately : IPeakFitting
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakFittingSeparately), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingSeparately)obj;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var minimalProminence = info.GetNullableDouble("MinimalProminence");
        return new PeakFittingSeparately()
        {
        };
      }
    }
    #endregion

    /// <inheritdoc/>
    public IPeakFittingResult Execute(double[] xArray, double[] yArray, IEnumerable<PeakSearching.PeakDescription> peakDescriptions)
    {
      var fitFunc = new PeakFitFunctions.Gauss(1);

      var list = new List<PeakFitting.PeakDescription>();

      foreach (var description in peakDescriptions)
      {
        int first = (int)Math.Max(0, description.PositionIndex - description.Width / 2);
        int last = (int)Math.Min(xArray.Length - 1, description.PositionIndex + description.Width / 2);
        int len = last - first + 1;
        if (len < 3)
        {
          list.Add(new PeakDescription() { SearchDescription = description, Notes = "Width too small for fitting" });
          continue;
        }

        var xCut = new double[len];
        var yCut = new double[len];
        Array.Copy(xArray, first, xCut, 0, len);
        Array.Copy(yArray, first, yCut, 0, len);

        var xPosition = RMath.InterpolateLinear(description.PositionIndex, xArray);
        var xWidth = Math.Abs(RMath.InterpolateLinear(description.PositionIndex + 0.5 * description.Width, xArray) -
          RMath.InterpolateLinear(description.PositionIndex - 0.5 * description.Width, xArray));


        var param = new double[3];
        param[0] = description.Height;
        param[1] = xPosition;
        param[2] = xWidth;

        var fit = new QuickNonlinearRegression(fitFunc);
        param = fit.Fit(xCut, yCut,param);

        list.Add(new PeakDescription
        {
          SearchDescription = description,
          Height = param[0],
          PositionInXUnits = param[1],
          Width = param[2]
        });
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
