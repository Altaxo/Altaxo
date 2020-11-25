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

namespace Altaxo.Calc.FitFunctions.Kinetics
{
  /// <summary>
  /// Represents solutions related to the differential equation y'=-k*y^n. For the direct solution of this equation, see <see cref="CoreSolution"/>.
  /// </summary>
  public class KineticsNthOrder
  {
    /// <summary>
    /// Represents the real solution of the nth order kinetic equation y'=-k*y^n with y[0]&gt;=0.
    /// </summary>
    /// <param name="t">Time.</param>
    /// <param name="y0">Starting value of y at t=0.</param>
    /// <param name="k">Kinetic constant.</param>
    /// <param name="order">The order (n in above formula) of the kinetics equation ( has to be nonnegative).</param>
    /// <returns>The solution if y'=-k*y^n, presuming that y0 is nonnegative.</returns>
    public static double CoreSolution(double t, double y0, double k, double order)
    {
      if (!(y0 >= 0))
        return double.NaN; // throw new ArgumentOutOfRangeException("y0 has to be nonnegative");

      if (order >= 1)
      {
        if (order == 1)
          return y0 * Math.Exp(-k * t);
        else
          return Math.Pow(Math.Pow(y0, 1 - order) + (order - 1) * k * t, 1 / (1 - order));
      }
      else // order<1
      {
        if (order == 0)
          return y0 - k * t;
        else if (order > 0)
          return Math.Pow(Math.Pow(y0, 1 - order) + (order - 1) * k * t, 1 / (1 - order));
        else
          return double.NaN; // throw new ArgumentOutOfRangeException("order has to be nonnegative");
      }
    }

    /// <summary>
    /// Represents the solution of a nth order kinetics to the problem of aggregation.
    /// </summary>
    /// <param name="t">Time.</param>
    /// <param name="p0">Volume fraction of aggregating species at time t=0, which is free (i.e. which is at this time not contained inside an aggragate).</param>
    /// <param name="pSample">Total volume fraction of aggregating species in the sample.</param>
    /// <param name="pInsideAggregate">Aggregates are assumed to contain a constant volume fraction of aggregating species. This parameter represents this constant.</param>
    /// <param name="k">Kinetic constant.</param>
    /// <param name="order">Order of the kinetics. Has to be equal or greater than 0.</param>
    /// <returns>The volume fraction of aggregates at time t. At time t=0, this value is <c>(pSample-p0)/pInsideAggregate</c>. For t going to infinity,
    /// this value tends to <c>pSample/pInsideAggregate</c>.</returns>
    /// <remarks>
    /// The kinetic equation for this problem (see <see cref="CoreSolution"/> is formulated with the number
    /// of free aggregating particles as variable x and the number of aggregating particels inside aggregates as the variable y.
    /// The solution was reformulated with volume fractions, using a new kinetic constant scaled by the volume of one aggregating particel.
    /// </remarks>
    public static double AgglomerateConcentrationFromP0AndPInsideAggregate(double t, double p0, double k, double order, double pSample, double pInsideAggregate)
    {
      return (pSample - p0 * CoreSolution(t, p0, k, order)) / pInsideAggregate;
    }

    /// <summary>
    /// Represents the solution of a nth order kinetics to the problem of aggregation.
    /// </summary>
    /// <param name="t">Time.</param>
    /// <param name="pA0">Volume fraction of aggregates at time t=0.</param>
    /// <param name="pSample">Total volume fraction of aggregating species in the sample.</param>
    /// <param name="pAInf">Volume fraction of aggregates at time t=Infinity.</param>
    /// <param name="k">Kinetic constant.</param>
    /// <param name="order">Order of the kinetics. Has to be equal or greater than 0.</param>
    /// <returns>The volume fraction of aggregates at time t. At time t=0, this value is <c>pA0</c>. For t going to infinity,
    /// this value tends to <c>pAInf</c>.</returns>
    /// <remarks>The provided volume fraction of aggregating species <c>pSample</c>is influencing only the rate. It is important only
    /// if you want to compare aggregation processes for sample with different content of aggregating species. If such a comparism is not neccessary,
    /// you can set <c>pSample</c> to 1.
    /// The kinetic equation for this problem (see <see cref="CoreSolution"/> is formulated with the number
    /// of free aggregating particles as variable x and the number of aggregating particels inside aggregates as the variable y.
    /// The solution was reformulated with volume fractions, using a new kinetic constant scaled by the volume of one aggregating particel.
    /// </remarks>
    public static double AgglomerateConcentrationFromPA0AndPAInf(double t, double pA0, double pAInf, double k, double order, double pSample)
    {
      double p0 = pSample * (1 - pA0 / pAInf);
      return (pAInf - (pAInf - pA0) * CoreSolution(t, p0, k, order));
    }
  }
}
