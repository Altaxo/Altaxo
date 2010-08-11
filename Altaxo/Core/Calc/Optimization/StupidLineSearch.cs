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
    public StupidLineSearch(ICostFunction cost)
    {
      this.costFunction_ = cost;
      this.endCriteria_ = new EndCriteria();
    }

    public override LinearAlgebra.DoubleVector Search(LinearAlgebra.DoubleVector x, LinearAlgebra.DoubleVector direction, double step)
    {
      DoubleVector retx = new DoubleVector(x);
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
