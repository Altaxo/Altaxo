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
using System.Linq;
using Altaxo.Calc;
using Altaxo.Science.Signals;

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
        return (x[^1] - x[0]) * (rightIdx - leftIdx) / (double)(x.Length - 1);
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

    /// <summary>
    /// Combines the results of two list of PeakDescriptions into one list. Note that both lists must be already sorted by position!
    /// </summary>
    /// <param name="peaksRegular">The result of the regular peak search.</param>
    /// <param name="peaksEnhanced">The result of the peak search in the enhanced spectrum.</param>
    /// <returns>A list with the combined results.</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public static List<PeakDescription> CombineResults(List<PeakDescription> peaksRegular, List<PeakDescription> peaksEnhanced, double[] xRegular, double[] yRegular)
    {
      // the rules for combination are as follows:
      // if a peak from peaksRegular coincide with 2 or more peaks from peaksEnhanced, the peakRegular is replaced by the peaks from peakEnhanced
      // if a peak from peaksRegular coincide with only one peak from peaksEnhanced, the peak from peaksEnhanced is dropped, and the peak from peaksRegular is kept
      // non-coinciding peaks are both included in the resulting list

      var results = new List<PeakDescription>();

      for (int i = 0; i < peaksRegular.Count; ++i)
      {
        var peakDescription = peaksRegular[i];
        var coincidingPeaks = GetCoincidingPeaksEnhanced(peakDescription, peaksEnhanced);

        if (coincidingPeaks.Count >= 2)
        {
          for (int j = coincidingPeaks.Count - 1; j >= 0; --j)
          {
            results.Add(ConvertToRegularPeakDescription(coincidingPeaks[j].Desc, xRegular, yRegular));
            peaksEnhanced.RemoveAt(coincidingPeaks[j].Index);
          }
        }
        else if (coincidingPeaks.Count >= 1)
        {
          results.Add(peakDescription);
          peaksEnhanced.RemoveAt(coincidingPeaks[0].Index);
        }
        else // no coincidence - include the regular peak, maintain the enhanced peak for later inclusion
        {
          results.Add(peakDescription);
        }
      }
      // now add all remaining peaks from peaksEnhanced
      results.AddRange(peaksEnhanced.Select(pEh => ConvertToRegularPeakDescription(pEh, xRegular, yRegular)));

      // now sort by position
      results.Sort((p1, p2) => Comparer<double>.Default.Compare(p1.PositionValue, p2.PositionValue));
      return results;
    }




    /// <summary>
    /// Give a peak from the regular spectrum, this function finds the coinciding peaks of the peak search in the enhanced spectrum.
    /// </summary>
    /// <param name="peakRegular">The peak description of the regular peak.</param>
    /// <param name="peaksEnhanced">The peak descriptions of all peaks found in the enhanced spectrum.</param>
    /// <returns>A list of peaks from the enhanced spectrum, which may coincide with the peak from the regular spectrum.</returns>
    public static List<(PeakDescription Desc, int Index)> GetCoincidingPeaksEnhanced(PeakDescription peakRegular, IReadOnlyList<PeakDescription> peaksEnhanced)
    {
      var results = new List<(PeakDescription Desc, int Index)>();

      for (int i = 0; i < peaksEnhanced.Count; ++i)
      {
        var peakEnh = peaksEnhanced[i];
        if (DoPeaksCoincide(peakRegular, peakEnh))
        {
          results.Add((peakEnh, i));
        }
      }
      return results;
    }


    /// <summary>
    /// Determine if two peaks do coincide.
    /// </summary>
    /// <param name="peakRegular">The peak (from regular peak search).</param>
    /// <param name="peakEnhanced">Another peak (from enhanced peak search).</param>
    /// <returns>True if the two peaks do coincide; otherwise, false.</returns>
    public static bool DoPeaksCoincide(PeakDescription peakRegular, PeakDescription peakEnhanced)
    {
      return Altaxo.Calc.RMath.IsInIntervalCC(peakEnhanced.PositionValue, peakRegular.PositionValue - peakRegular.WidthValue / 2, peakRegular.PositionValue + peakRegular.WidthValue / 2);
    }

    /// <summary>
    /// Converts a peak description that was retrieved from an enhanced spectrum to a regular peak description.
    /// </summary>
    /// <param name="peakEnhanced">The peak description that was retrieved from the enhanced spectrum.</param>
    /// <param name="xRegular">The x-values of the regular spectrum.</param>
    /// <param name="yRegular">The y-values of the regular spectrum.</param>
    /// <returns>The peak description, converted to the regular spectrum domain (concerns position index, with in pixels, height, and prominence).</returns>
    public static PeakDescription ConvertToRegularPeakDescription(PeakDescription peakEnhanced, double[] xRegular, double[] yRegular)
    {
      var positionIndex = SignalMath.GetIndexOfXInAscendingArray(xRegular, peakEnhanced.PositionValue, null);
      var widthPixels = SignalMath.GetIndexOfXInAscendingArray(xRegular, peakEnhanced.PositionValue + peakEnhanced.WidthValue * 0.5, null) - SignalMath.GetIndexOfXInAscendingArray(xRegular, peakEnhanced.PositionValue - peakEnhanced.WidthValue * 0.5, null);
      var height = yRegular[positionIndex];
      var prominence = height;

      return peakEnhanced with
      {
        PositionIndex = positionIndex,
        WidthPixels = widthPixels,
        Height = height,
        Prominence = prominence,
      };
    }

  }
}
