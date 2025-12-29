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

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// This function increments x by direction*step as long as the function gets smaller. If it gets bigger, step is multiplied by -0.5.
  /// The method ends if two successive function evaluations give the same result.
  /// </summary>
  public class StupidLineSearch : LineSearchMethod
  {
    /// <summary>Initializes a new instance of the <see cref="StupidLineSearch"/> class.</summary>
    /// <param name="cost">The cost function to minimize.</param>
    public StupidLineSearch(ICostFunction cost)
    {
      costFunction_ = cost;
      endCriteria_ = new EndCriteria();
    }

    /// <inheritdoc/>
    public override Vector<double> Search(Vector<double> x, Vector<double> direction, double step)
    {
      var retx = x.Clone();
      double oldVal = FunctionEvaluation(retx);
      double newVal = oldVal;

      // First find the initial direction
      double valPos = FunctionEvaluation(retx + direction * step);
      double valNeg = FunctionEvaluation(retx - direction * step);
      if (valPos >= oldVal && valNeg < oldVal) // we reverse the direction only if the other direction really gives the smaller result
      {
        retx -= direction * step;
        oldVal = valNeg;
        step = -step;
      }
      else if (valPos < oldVal)
      {
        retx += direction * step;
        oldVal = valPos;
      }

      // now iterate
      for (; ; )
      {
        retx += direction * step;
        newVal = FunctionEvaluation(retx);

        if (newVal > oldVal)
        {
          step /= -2;
        }
        else if (!(newVal != oldVal))
        {
          break;
        }
        oldVal = newVal;
      }

      return retx;
    }
  }
}
