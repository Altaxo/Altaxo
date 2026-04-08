// Copyright(c) 2004, Ernst Hairer

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Ode.Obsolete.Dopri
{
  #region Interface

  /// <summary>
  /// Defines a callback that receives intermediate solution values during DOPRI integration.
  /// </summary>
  public interface ISOLOUT
  {
    /// <summary>
    /// Handles an integration output step.
    /// </summary>
    /// <param name="NR">The current step number.</param>
    /// <param name="XOLD">The previous integration point.</param>
    /// <param name="X">The current integration point.</param>
    /// <param name="Y">The solution vector.</param>
    /// <param name="offset_y">The offset into <paramref name="Y"/>.</param>
    /// <param name="N">The number of equations.</param>
    /// <param name="CON">The continuous output coefficients.</param>
    /// <param name="offset_con">The offset into <paramref name="CON"/>.</param>
    /// <param name="ICOMP">The component selection array.</param>
    /// <param name="offset_icomp">The offset into <paramref name="ICOMP"/>.</param>
    /// <param name="ND">The number of dense output components.</param>
    /// <param name="RPAR">User-supplied real parameters.</param>
    /// <param name="offset_rpar">The offset into <paramref name="RPAR"/>.</param>
    /// <param name="IPAR">User-supplied integer parameter.</param>
    /// <param name="IRTRN">Return flag used to influence further integration.</param>
    void Run(int NR, double XOLD, double X, double[] Y, int offset_y, int N, double[] CON, int offset_con
             , int[] ICOMP, int offset_icomp, int ND, double[] RPAR, int offset_rpar, int IPAR, int IRTRN);
  }

  #endregion Interface

}
