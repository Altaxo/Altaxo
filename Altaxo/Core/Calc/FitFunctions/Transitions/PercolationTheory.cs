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

#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.FitFunctions.Transitions
{
  /// <summary>
  /// Utility class providing the simple percolation theory transition expression.
  /// </summary>
  public class PercolationTheory
  {
    /// <summary>
    /// Evaluates the basic percolation transition formula. Returns <c>double.NaN</c> when <paramref name="p"/> equals <paramref name="pc"/>.
    /// </summary>
    /// <param name="p">Occupation probability in [0,1].</param>
    /// <param name="y0">Left amplitude.</param>
    /// <param name="y1">Right amplitude.</param>
    /// <param name="pc">Critical threshold in [0,1].</param>
    /// <param name="s">Left exponent.</param>
    /// <param name="t">Right exponent.</param>
    /// <returns>The transitioned value according to percolation theory or <c>double.NaN</c> if p == pc.</returns>
    public static double Transition(double p, double y0, double y1, double pc, double s, double t)
    {
      if (p < pc)
        return y0 * Math.Pow((pc - p) / pc, -s);
      else if (p > pc)
        return y1 * Math.Pow((p - pc) / (1 - pc), t);
      else
        return double.NaN;
    }
  }
}
