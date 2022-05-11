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
using System.Linq;

namespace Altaxo.Science.Spectroscopy.SpikeRemoval
{
  public record SpikeRemovalByPeakElimination : ISpikeRemoval
  {
    public int MaximalWidth { get; init; } = 1;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpikeRemovalByPeakElimination), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpikeRemovalByPeakElimination)obj;
        info.AddValue("MaximalWidth", s.MaximalWidth);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var maximalWidth = info.GetInt32("MaximalWidth");

        return o is null ? new SpikeRemovalByPeakElimination
        {
          MaximalWidth = maximalWidth,
        } :
          ((SpikeRemovalByPeakElimination)o) with
          {
            MaximalWidth = maximalWidth,
          };
      }
    }
    #endregion

    public double[] Execute(double[] data)
    {
      var yArray = (double[])data.Clone();

      double noiseLevel = 0;
      for (int i = 2; i < data.Length; ++i)
        noiseLevel += Math.Abs(data[i - 1] - 0.5 * (data[i] + data[i - 2]));
      noiseLevel /= data.Length - 2;

      var peakFinder = new PeakSearching.PeakFinder();
      peakFinder.SetWidth((0, MaximalWidth+0.25));
      peakFinder.SetRelativeHeight(0.5);
      peakFinder.SetProminence(10*noiseLevel); // only in order to trigger the calculation of the peak height
      peakFinder.Execute(data);

      if (peakFinder.PeakPositions.Length > 0)
      {
        var heights = (double[])peakFinder.Prominences!.Clone();
        var indices = Enumerable.Range(0, heights.Length).ToArray();
        Array.Sort(heights, indices); // Sort heights and the indices

        // now consider the highest peaks that fullfill the peak width criterion
        for (int j = indices.Length - 1; j >= Math.Max(0,indices.Length - 10); --j)
        {
          var idx = indices[j];

          var leftPos = (int)Math.Max(0, Math.Floor(peakFinder.PeakPositions[idx] - peakFinder.Widths![idx] / 2.0));
          var rightPos = (int)Math.Min(data.Length - 1, Math.Ceiling(peakFinder.PeakPositions[idx] + peakFinder.Widths![idx] / 2.0));


          // interpolate data
          for (int k = leftPos + 1; k < rightPos; ++k)
          {
            var r = (k - leftPos) / (double)(rightPos - leftPos);
            var y = r * data[rightPos] + (1 - r) * data[leftPos]; // interpolate the base line from left to right
            yArray[k] = y; // replace the values in yArray with the baseline
          }
        }
      }

      return yArray;
    }
  }
}
