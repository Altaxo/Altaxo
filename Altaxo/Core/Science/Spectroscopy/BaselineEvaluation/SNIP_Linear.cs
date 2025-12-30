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

namespace Altaxo.Science.Spectroscopy.BaselineEvaluation
{
  /// <inheritdoc/>
  public record SNIP_Linear : Altaxo.Science.Spectroscopy.BaselineEstimation.SNIP_Base, IBaselineEvaluation
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="SNIP_Linear"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SNIP_Linear), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SNIP_Linear)obj;
        info.AddValue("HalfWidth", s.HalfWidth);
        info.AddValue("IsHalfWidthInXUnits", s.IsHalfWidthInXUnits);
        info.AddValue("NumberOfIterations", s.NumberOfRegularIterations);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var halfWidth = info.GetDouble("HalfWidth");
        var isHalfWidthInXUnits = info.GetBoolean("IsHalfWidthInXUnits");
        var numberOfIterations = info.GetInt32("NumberOfIterations");

        return o is null ? new SNIP_Linear
        {
          HalfWidth = halfWidth,
          IsHalfWidthInXUnits = isHalfWidthInXUnits,
          NumberOfRegularIterations = numberOfIterations
        } :
          ((SNIP_Linear)o) with
          {
            HalfWidth = halfWidth,
            IsHalfWidthInXUnits = isHalfWidthInXUnits,
            NumberOfRegularIterations = numberOfIterations
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

      // return the baseline itself
      return (x, yBaseline, regions);
    }


  }
}
