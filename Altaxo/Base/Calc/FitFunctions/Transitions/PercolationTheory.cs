using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.FitFunctions.Transitions
{
  public class PercolationTheory
  {
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
