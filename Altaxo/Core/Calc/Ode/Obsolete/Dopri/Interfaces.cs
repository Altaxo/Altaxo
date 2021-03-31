// Copyright(c) 2004, Ernst Hairer

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Ode.Obsolete.Dopri
{
  #region Interface

  public interface ISOLOUT
  {
    void Run(int NR, double XOLD, double X, double[] Y, int offset_y, int N, double[] CON, int offset_con
             , int[] ICOMP, int offset_icomp, int ND, double[] RPAR, int offset_rpar, int IPAR, int IRTRN);
  }

  #endregion Interface

}
