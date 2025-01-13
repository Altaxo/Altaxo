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

namespace Altaxo.Science.Signals
{
  /// <summary>
  /// Options for a step evaluation that uses 4 points on a curve. Two points on the curve define a left straight line, and two other points on the curve define a right straight line.
  /// The step should be located inbetween the two inner points. The step position is then evaluated by building a regression
  /// line of the curve between the inner points, but only from a certain level to another given leven (e.g., from 25% to 75% of the distance between left and right line).
  /// </summary>
  public record FourPointStepEvaluationOptions : Main.IImmutable
  {
    /// <summary>
    /// Gets a value indicating whether to use regression for the left and right section of the step.
    /// </summary>
    /// <value>
    /// If <c>true</c>, regression is used, i.e. all points between the left outer and inner point are used for the regression of the left line,
    /// and all points between the right inner point and right outer point are used for the left line.
    /// If <c>false</c>, only the two points (left outer and left inner) are used to form the left line. Likewise, only the two points (right inner and right outer) are used to form the right line.
    /// </value>
    public bool UseRegressionForLeftAndRightLine { get; init; }

    /// <summary>
    /// Gets the regression levels for the middle line. For instance, if the value is set to (0.25, 0.75), then all points between 25% and 75% distance from the left and right line are used
    /// for the regression of the middle line.
    /// </summary>
    /// <value>
    /// The regression limits. Both values must be between 0 and 1, and the first value must be smaller than the second value.
    /// </value>
    public (double LowerLevel, double UpperLevel) MiddleRegressionLevels { get; init; } = (0.25, 0.75);

    /// <summary>
    /// Gets the overlap of the middle line with the left and right line. The value is a fraction of the distance between the left and right line.
    /// </summary>
    public double MiddleLineOverlap { get; init; } = 0.1;


    /// <summary>
    /// Gets a value indicating whether the original (measured) data points should be included in the output.
    /// </summary>
    /// <value>
    /// <c>true</c> if the orignal data points should be included in the output; otherwise, <c>false</c>.
    /// </value>
    public bool IncludeOriginalPointsInOutput { get; init; }


    /// <summary>
    /// Gets the index of the first point of the left line.
    /// </summary>
    public double IndexLeftOuter { get; init; }

    /// <summary>
    /// Gets the index of the second point of the left line.
    /// </summary>
    public double IndexLeftInner { get; init; }

    /// <summary>
    /// Gets the index of the first point of the right line.
    /// </summary>
    public double IndexRightOuter { get; init; }

    /// <summary>
    /// Gets the index of the second point of the right line.
    /// </summary>
    public double IndexRightInner { get; init; }

    #region Serialization

    /// <summary>
    /// 2024-12-22 V0
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FourPointStepEvaluationOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FourPointStepEvaluationOptions)obj;

        info.AddValue("UseRegressionForLeftAndRightLine", s.UseRegressionForLeftAndRightLine);
        info.AddValue("MiddleRegressionLowerLevel", s.MiddleRegressionLevels.LowerLevel);
        info.AddValue("MiddleRegressionUpperLevel", s.MiddleRegressionLevels.UpperLevel);
        info.AddValue("MiddleLineOverlap", s.MiddleLineOverlap);
        info.AddValue("IncludeOriginalPointsInOutput", s.IncludeOriginalPointsInOutput);
        info.AddValue("IndexLeftOuter", s.IndexLeftOuter);
        info.AddValue("IndexLeftInner", s.IndexLeftInner);
        info.AddValue("IndexRightInner", s.IndexRightInner);
        info.AddValue("IndexRightOuter", s.IndexRightOuter);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var useRegressionForLeftAndRightLine = info.GetBoolean("UseRegressionForLeftAndRightLine");
        var middleRegressionLowerLevel = info.GetDouble("MiddleRegressionLowerLevel");
        var middleRegressionUpperLevel = info.GetDouble("MiddleRegressionUpperLevel");
        var middleLineOverlap = info.GetDouble("MiddleLineOverlap");
        var includeOriginalPointsInOutput = info.GetBoolean("IncludeOriginalPointsInOutput");
        var indexLeftOuter = info.GetDouble("IndexLeftOuter");
        var indexLeftInner = info.GetDouble("IndexLeftInner");
        var indexRightInner = info.GetDouble("IndexRightInner");
        var indexRightOuter = info.GetDouble("IndexRightOuter");

        return new FourPointStepEvaluationOptions()
        {
          UseRegressionForLeftAndRightLine = useRegressionForLeftAndRightLine,
          MiddleRegressionLevels = (middleRegressionLowerLevel, middleRegressionUpperLevel),
          MiddleLineOverlap = middleLineOverlap,
          IncludeOriginalPointsInOutput = includeOriginalPointsInOutput,
          IndexLeftOuter = indexLeftOuter,
          IndexLeftInner = indexLeftInner,
          IndexRightInner = indexRightInner,
          IndexRightOuter = indexRightOuter,
        };
      }
    }
    #endregion

  }
}
