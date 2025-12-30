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

namespace Altaxo.Science.Spectroscopy
{
  /// <summary>
  /// Helper methods for working with region arrays.
  /// </summary>
  public static class RegionHelper
  {
    /// <summary>
    /// Gets the region ranges.
    /// </summary>
    /// <param name="regions">
    /// The regions. Each element designates the start index of a new region.
    /// It is not necessary to set the first point of the array to zero. If <see langword="null"/> or an empty array is provided,
    /// the full range is returned.
    /// </param>
    /// <param name="arrayLength">Length of the array.</param>
    /// <returns>An enumeration of regions as tuples of start index and end index (exclusive).</returns>
    public static IEnumerable<(int Start, int End)> GetRegionRanges(int[]? regions, int arrayLength)
    {
      if (regions is null || regions.Length == 0)
      {
        yield return (0, arrayLength);
      }
      else
      {
        if (regions[0] != 0)
        {
          yield return (0, regions[0]);
        }
        for (int i = 1; i < regions.Length; i++)
        {
          yield return (regions[i - 1], regions[i]);
        }
        if (regions[regions.Length - 1] < arrayLength)
        {
          yield return (regions[regions.Length - 1], arrayLength);
        }
      }
    }


    /// <summary>
    /// Tries to identify spectral regions based on the provided spectral x-values.
    /// An end of a region is recognized when the gap between two consecutive x-values is ten times larger
    /// than the previous gap, or if the sign of the gap changes.
    /// This method fails if a spectral region contains only a single point (since no gap value can be obtained then).
    /// (In this case, almost all spectral correction methods also fail.)
    /// </summary>
    /// <param name="xvalues">The vector of x-values for the spectra (wavelengths, frequencies, etc.).</param>
    /// <returns>
    /// The array of regions. Each element in the array is the start index of a new region in <paramref name="xvalues"/>.
    /// </returns>
    public static int[] IdentifyRegions(IReadOnlyList<double> xvalues)
    {
      var list = new List<int>();

      int len = xvalues.Count;

      for (int i = 0; i < len - 2; i++)
      {
        double gap = Math.Abs(xvalues[i + 1] - xvalues[i]);
        double nextgap = Math.Abs(xvalues[i + 2] - xvalues[i + 1]);
        if (gap != 0 && (Math.Sign(gap) == -Math.Sign(nextgap) || Math.Abs(nextgap) > 10 * Math.Abs(gap)))
        {
          list.Add(i + 2);
          i++;
        }
      }

      return list.ToArray();
    }

    /// <summary>
    /// Normalizes a list of region start indices.
    /// </summary>
    /// <param name="regions">
    /// The regions list. Each element designates the start index of a new region.
    /// </param>
    /// <param name="length">The length of the underlying data array.</param>
    /// <returns>
    /// A normalized regions array, or <see langword="null"/> if no regions are present or normalization results in a single full-length region.
    /// </returns>
    public static int[]? NormalizeRegions(List<int> regions, int length)
    {
      if (regions is null)
        return null;
      if (regions.Count == 0)
        return null;
      if (length < 2)
        return null;
      if (regions.Count == 1 && regions[0] == length)
        return null;
      if (regions[^1] == length)
        regions.RemoveAt(regions.Count - 1);
      return regions.ToArray();
    }
  }
}
