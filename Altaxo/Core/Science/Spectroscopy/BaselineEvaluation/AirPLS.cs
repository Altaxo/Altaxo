#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using Altaxo.Science.Spectroscopy.BaselineEstimation;

namespace Altaxo.Science.Spectroscopy.BaselineEvaluation
{
  /// <inheritdoc/>
  public record AirPLS : AirPLSBase, IBaselineEvaluation
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="AirPLS"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AirPLS), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AirPLS)obj;
        info.AddValue("Lambda", s.Lambda);
        info.AddValue("ScaleLambdaWithXUnits", s.ScaleLambdaWithXUnits);
        info.AddValue("TerminationRatio", s.TerminationRatio);
        info.AddValue("Order", s.Order);
        info.AddValue("MaxNumberOfIterations", s.MaximumNumberOfIterations);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var lambda = info.GetDouble("Lambda");
        var scaleLambdaWithXUnits = info.GetBoolean("ScaleLambdaWithXUnits");
        var terminationRatio = info.GetDouble("TerminationRatio");
        var order = info.GetInt32("Order");
        var maxNumberOfIterations = info.GetInt32("MaxNumberOfIterations");

        return o is null ? new AirPLS
        {
          Lambda = lambda,
          ScaleLambdaWithXUnits = scaleLambdaWithXUnits,
          TerminationRatio = terminationRatio,
          Order = order,
          MaximumNumberOfIterations = maxNumberOfIterations,
        } :
          ((AirPLS)o) with
          {
            Lambda = lambda,
            ScaleLambdaWithXUnits = scaleLambdaWithXUnits,
            TerminationRatio = terminationRatio,
            Order = order,
            MaximumNumberOfIterations = maxNumberOfIterations,
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
