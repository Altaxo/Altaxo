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

using Altaxo.Drawing;
using Altaxo.Main;

namespace Altaxo.Science.Signals
{
  /// <summary>
  /// Options for the step evaluation tool mouse handler.
  /// </summary>
  public record FourPointStepEvaluationToolMouseHandlerOptions : IImmutable
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
    /// Gets the pen that is used for drawing the lines.
    /// </summary>
    public PenX LinePen { get; init; } = new PenX(NamedColors.LightBlue, 2);

    #region Serialization

    /// <summary>
    /// 2024-12-23 V0
    /// </summary>
    /// <seealso cref="Serialization.Xml.IXmlSerializationSurrogate" />
    [Serialization.Xml.XmlSerializationSurrogateFor(typeof(FourPointStepEvaluationToolMouseHandlerOptions), 0)]
    public class SerializationSurrogate0 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FourPointStepEvaluationToolMouseHandlerOptions)obj;

        info.AddValue("UseRegressionForLeftAndRightLine", s.UseRegressionForLeftAndRightLine);
        info.AddValue("MiddleRegressionLowerLevel", s.MiddleRegressionLevels.LowerLevel);
        info.AddValue("MiddleRegressionUpperLevel", s.MiddleRegressionLevels.UpperLevel);
        info.AddValue("MiddleLineOverlap", s.MiddleLineOverlap);
        info.AddValue("LinePen", s.LinePen);
      }

      public object Deserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var useRegressionForLeftAndRightLine = info.GetBoolean("UseRegressionForLeftAndRightLine");
        var middleRegressionLowerLevel = info.GetDouble("MiddleRegressionLowerLevel");
        var middleRegressionUpperLevel = info.GetDouble("MiddleRegressionUpperLevel");
        var middleLineOverlap = info.GetDouble("MiddleLineOverlap");
        var linePen = info.GetValue<PenX>("LinePen", null);

        return new FourPointStepEvaluationToolMouseHandlerOptions()
        {
          UseRegressionForLeftAndRightLine = useRegressionForLeftAndRightLine,
          MiddleRegressionLevels = (middleRegressionLowerLevel, middleRegressionUpperLevel),
          MiddleLineOverlap = middleLineOverlap,
          LinePen = linePen,
        };
      }
    }
    #endregion

  }
}
