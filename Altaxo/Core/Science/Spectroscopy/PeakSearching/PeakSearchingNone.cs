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

using System.Collections.Generic;
using Altaxo.Calc;

namespace Altaxo.Science.Spectroscopy.PeakSearching
{
  public class PeakSearchingNone : IPeakSearching
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakSearchingNone), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new PeakSearchingNone();
      }
    }
    #endregion

    (
      double[] x,
      double[] y,
      int[]? regions,
      IReadOnlyList<(IReadOnlyList<PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> peakSearchResults
    ) IPeakSearching.Execute(double[] x, double[] y, int[]? regions)
    {
      return (x, y, regions, new List<(IReadOnlyList<PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)>());
    }

    /// <summary>
    /// Gets the width value from the fractional indices of the left side, the peak position, and the right side.
    /// </summary>
    /// <param name="x">The x array.</param>
    /// <param name="leftIdx">Index of the left side.</param>
    /// <param name="middleIdx">Index of the peak position.</param>
    /// <param name="rightIdx">Index of the right side.</param>
    /// <returns>The width value. Attention: can be negative if the x array is sorted descending.</returns>
    public static double GetWidthValue(double[] x, double leftIdx, double middleIdx, double rightIdx)
    {
      if (IsInRange(x, leftIdx) && IsInRange(x, rightIdx))
      {
        return RMath.InterpolateLinear(rightIdx, x) - RMath.InterpolateLinear(leftIdx, x);
      }
      else if (IsInRange(x, middleIdx) && IsInRange(x, leftIdx))
      {
        return 2 * (RMath.InterpolateLinear(middleIdx, x) - RMath.InterpolateLinear(leftIdx, x));
      }
      else if (IsInRange(x, rightIdx) && IsInRange(x, middleIdx))
      {
        return 2 * (RMath.InterpolateLinear(rightIdx, x) - RMath.InterpolateLinear(middleIdx, x));
      }
      else
      {
        // Try to interpolate over the full range of x
        return (x[x.Length - 1] - x[0]) * (rightIdx - leftIdx) / (double)(x.Length - 1);
      }
    }

    /// <summary>
    /// Determines whether the fractional index is in the range of the x-array, so that it can be converted to an x-value.
    /// </summary>
    /// <param name="x">The x array.</param>
    /// <param name="idx">The fractional index into the x-array.</param>
    /// <returns>
    ///   <c>true</c> if the fractional index is in range; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsInRange(double[] x, double idx)
    {
      return idx >= 0 && idx <= x.Length - 1;
    }
  }
}
