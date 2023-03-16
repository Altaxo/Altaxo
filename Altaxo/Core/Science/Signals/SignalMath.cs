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
using System.Collections.Generic;

namespace Altaxo.Science.Signals
{
  public static class SignalMath
  {
    /// <summary>
    /// Gets the indices of the zero crossings of the signal.
    /// </summary>
    /// <param name="y">The y-values of the signal.</param>
    /// <param name="zeroCrossings">A list that can be re-used as return value.</param>
    /// <returns>List of indices, where zero crossing of the signal occurs.</returns>
    public static List<int> GetIndicesOfZeroCrossings(IReadOnlyList<double> y, List<int>? zeroCrossings = null)
    {
      if (y is null)
        throw new ArgumentNullException(nameof(y));

      if (zeroCrossings is null)
        zeroCrossings = new List<int>(y.Count / 4);
      else
        zeroCrossings.Clear();

      if (y.Count > 1)
      {
        int il = 0;
        var yl = Math.Sign(y[0]);
        for (int i = 1; i < y.Count; ++i)
        {
          var ym = Math.Sign(y[i]);
          if (ym == 0)
            continue;
          else if (ym != yl)
            zeroCrossings.Add((il + i) / 2);

          il = i;
          if (ym != 0)
          {
            yl = ym;
          }
        }
      }
      return zeroCrossings;
    }

    /// <summary>
    /// Gets the indices of the extrema (minima and maxima) of the signal
    /// </summary>
    /// <param name="y">The y-values of the signal.</param>
    /// <param name="indicesOfMinima">A list that can be recycled as the list of indices of minima. If null, a new list for the indices of minima will be allocated.</param>
    /// <param name="indicesOfMaxima">A list that can be recycled as the list of indices of maxima. If null, a new list for the indices of maxima will be allocated.</param>
    /// <returns>A tuple, which contains the list of indices of the minima and maxima of the signal. Note that the start and end of the signal can not be a minimum or maximum.</returns>
    public static (List<int> IndicesOfMinima, List<int> IndicesOfMaxima) GetIndicesOfExtrema(IReadOnlyList<double> y, List<int>? indicesOfMinima = null, List<int>? indicesOfMaxima = null)
    {
      if (y is null)
        throw new ArgumentNullException(nameof(y));

      if (indicesOfMinima is null)
        indicesOfMinima = new List<int>(y.Count / 4);
      else
        indicesOfMinima.Clear();

      if (indicesOfMaxima is null)
        indicesOfMaxima = new List<int>(y.Count / 4);
      else
        indicesOfMaxima.Clear();

      if (y.Count > 2)
      {
        int il = 1;
        var yl = Math.Sign(y[1] - y[0]);
        for (int i = 2; i < y.Count; ++i)
        {
          if (yl == 0)
          {
            continue;
          }

          var ym = Math.Sign(y[i] - y[i - 1]);
          if (ym == 0)
          {
            continue;
          }
          else if (ym != yl)
          {
            if (ym > 0)
              indicesOfMinima.Add((il + i - 1) / 2);
            else
              indicesOfMaxima.Add((il + i - 1) / 2);
          }

          il = i;
          yl = ym;
        }
      }
      return (indicesOfMinima, indicesOfMaxima);
    }
  }
}
