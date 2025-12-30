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

using System;
using Altaxo.Calc.Interpolation;
using Altaxo.Science.Spectroscopy.BaselineEstimation;

namespace Altaxo.Science.Spectroscopy.BaselineEvaluation
{
  /// <inheritdoc/>
  public record SSProb : SSProbBase, IBaselineEvaluation
  {
    #region Serialization

    /// <summary>
    /// 2024-04-16 V0 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SSProb), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SSProb)obj;
        info.AddValue("Interpolation", s.InterpolationFunctionOptions);
        info.AddEnum("SmoothnessSpecifiedBy", s.SmoothnessSpecifiedBy);
        info.AddValue("SmoothnessValue", s.SmoothnessValue);
        info.AddValue("Probability", s.RelativeProbabilityThreshold);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var interpolation = info.GetValue<IInterpolationFunctionOptions>("Interpolation", parent);
        var smoothnessSpecifiedBy = info.GetEnum<SmoothnessSpecification>("SmoothnessSpecifiedBy");
        var numberOfFeatures = info.GetDouble("SmoothnessValue");
        var probabilityThreshold = info.GetDouble("Probability");
        return new SSProb()
        {
          InterpolationFunctionOptions = interpolation,
          SmoothnessSpecifiedBy = smoothnessSpecifiedBy,
          SmoothnessValue = numberOfFeatures,
          RelativeProbabilityThreshold = probabilityThreshold,
        };
      }
    }
    #endregion

    /// <inheritdoc/>
    public new (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var yBaseline = new double[y.Length];
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        var xSpan = new ReadOnlySpan<double>(x, start, end - start);
        var ySpan = new ReadOnlySpan<double>(y, start, end - start);
        var yBaselineSpan = new Span<double>(yBaseline, start, end - start);
        Execute(xSpan, ySpan, yBaselineSpan);
      }

      return (x, yBaseline, regions);
    }
  }
}
