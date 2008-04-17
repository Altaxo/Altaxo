using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.FitFunctions.Transitions
{
  public class FermiDiracTransition
  {
    /// <summary>
    /// Returns a value which is 1 for p=0 and is 0 for p=1.
    /// </summary>
    /// <param name="p">Argument (between 0 and 1).</param>
    /// <param name="pc">Location of the transition (between 0 and 1).</param>
    /// <param name="w">Parameter determing the width of the transition.</param>
    /// <returns>A value between 1 (for p=0) and 0 (for p=1).</returns>
    /// <remarks>
    /// The original formula was y(p)=1/(1+Exp(b*(p-pc)).
    /// This formula has the disadvantage that it is not 1 for p=0 nor 0 for p=1. Thus we use
    /// the modified formula 
    /// core(p)=(y(p)-y(1))/(y(0)-y(1)) with the definition above for y(p). Additionally, instead of b, we use w=1/b, because
    /// w is directly related to the width of the transition.
    /// </remarks>
    public static double Core(double p, double pc, double w)
    {
      double b = 1 / w;
      double A = Math.Exp(b * (p - pc));
      double B = Math.Exp(b * (1 - pc));
      double C = Math.Exp(b * (0 - pc));
      return ((B - A)/(B - C))*((1 + C)/(1 + A));
    }


    /// <summary>
    /// Provides a linear scaled transition y = y1+(y0-y1)*Core(...).
    /// </summary>
    /// <param name="y0"></param>
    /// <param name="y1"></param>
    /// <param name="p"></param>
    /// <param name="pc"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static double LinearScaledTransition(double p, double y0, double y1, double pc, double w)
    {
      double core = Core(p, pc, w);
      return y1 + (y0 - y1) * core;
    }


    /// <summary>
    /// Provides a logarithmically scaled transition lg(y) = lg(y1)+(lg(y0)-lg(y1))*Core(...).
    /// </summary>
    /// <param name="y0"></param>
    /// <param name="y1"></param>
    /// <param name="p"></param>
    /// <param name="pc"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static double LogarithmicScaledTransition(double p, double y0, double y1, double pc, double w)
    {
      double core = Core(p, pc, w);
      return Math.Pow(y0, core) * Math.Pow(y1, 1 - core);
    }



  }
}
