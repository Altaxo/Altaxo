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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Science.Spectroscopy
{
  /// <summary>
  /// Class that helps with region arrays.
  /// </summary>
  public static class RegionHelper
  {
    /// <summary>
    /// Gets the region ranges.
    /// </summary>
    /// <param name="regions">The regions. Each element designates the start index of a new region.
    /// It is not neccessary to set the first point of the array to zero. If null or an empty array is provided,
    /// the full region is returned.</param>
    /// <param name="arrayLength">Length of the array.</param>
    /// <returns>Enumeration of regions as tuple of start index and end index (exclusive).</returns>
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
        for(int i=1;i<regions.Length;i++)
        {
          yield return (regions[i-1], regions[i]);
        }
        if (regions[regions.Length - 1] < arrayLength)
        {
          yield return (regions[regions.Length - 1], arrayLength);
        }
      }
    }
  }
}
