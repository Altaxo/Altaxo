#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
  /// Options for a peak evaluation that uses 4 points on a curve.
  /// The outer two points on the curve define a straight line under the peak.
  /// The two inner points designate the minimal x and maximal x values to start and stop the integration.
  /// Furthermore, it is assumed that the peak maximum is located between the two inner points.
  /// </summary>
  public record FourPointPeakEvaluationOptions : Main.IImmutable
  {

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
    /// 2024-01-06 V0
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FourPointPeakEvaluationOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FourPointPeakEvaluationOptions)obj;

        info.AddValue("IncludeOriginalPointsInOutput", s.IncludeOriginalPointsInOutput);
        info.AddValue("IndexLeftOuter", s.IndexLeftOuter);
        info.AddValue("IndexLeftInner", s.IndexLeftInner);
        info.AddValue("IndexRightInner", s.IndexRightInner);
        info.AddValue("IndexRightOuter", s.IndexRightOuter);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var includeOriginalPointsInOutput = info.GetBoolean("IncludeOriginalPointsInOutput");
        var indexLeftOuter = info.GetDouble("IndexLeftOuter");
        var indexLeftInner = info.GetDouble("IndexLeftInner");
        var indexRightInner = info.GetDouble("IndexRightInner");
        var indexRightOuter = info.GetDouble("IndexRightOuter");

        return new FourPointPeakEvaluationOptions()
        {
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
