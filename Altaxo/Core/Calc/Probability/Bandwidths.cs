#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Provides bandwidth selection rules (primarily for kernel density estimation).
  /// </summary>
  public static class Bandwidths
  {
    /// <summary>
    /// Computes the normal reference bandwidth (R's <c>bw.nrd0</c>) for kernel density estimation.
    /// </summary>
    /// <param name="x">The sample values.</param>
    /// <returns>The selected bandwidth.</returns>
    /// <exception cref="ArgumentException">Thrown when fewer than 2 data points are provided.</exception>
    public static double Nrd0(IReadOnlyList<double> x)
    {
      if (x.Count < 2)
        throw new ArgumentException("need at least 2 data points");

      double hi = Statistics.StandardDeviation(x);
      double lo = Math.Min(hi, Statistics.InterQuartileRange(x) / 1.34);  // qnorm(.75) - qnorm(.25) = 1.34898
      if (lo.IsNaN())
      {
        lo = hi;
        if (lo.IsNaN())
        {
          lo = Math.Abs(x[0]);
          if (lo.IsNaN())
            lo = 1;
        }
      }

      return 0.9 * lo * Math.Pow(x.Count, (-0.2));
    }
  }
}
