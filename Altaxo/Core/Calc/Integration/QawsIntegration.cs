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
using System.Text;

namespace Altaxo.Calc.Integration
{
  /// <summary>
  /// Adaptive integration for singular functions.
  /// </summary>
  /// <remarks>
  /// The QAWS algorithm is designed for integrands with algebraic-logarithmic singularities at
  /// the end-points of an integration region. In order to work efficiently the algorithm requires
  /// a precomputed table of Chebyshev moments.
  /// <para>Ref.: Gnu Scientific library reference manual (<see href="http://www.gnu.org/software/gsl/" />)</para>
  /// /// </remarks>
  internal class QawsIntegration : IntegrationBase
  {
    #region qaws.c

    /* integration/qaws.c
 *
 * Copyright (C) 1996, 1997, 1998, 1999, 2000 Brian Gough
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or (at
 * your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

    private static GSL_ERROR
    gsl_integration_qaws(Func<double, double> f,
                          double a, double b,
                          gsl_integration_qaws_table t,
                          double epsabs, double epsrel,
                          int limit,
                          gsl_integration_workspace workspace,
                          out double result, out double abserr, bool bDebug)
    {
      double area, errsum;
      double result0, abserr0;
      double tolerance;
      int iteration = 0;
      int roundoff_type1 = 0, roundoff_type2 = 0, error_type = 0;

      /* Initialize results */

      workspace.initialise(a, b);

      result = 0;
      abserr = 0;

      if (limit > workspace.limit)
      {
        return new GSL_ERROR("iteration limit exceeds available workspace", GSL_ERR.GSL_EINVAL, bDebug);
      }

      if (b <= a)
      {
        return new GSL_ERROR("limits must form an ascending sequence, a < b", GSL_ERR.GSL_EINVAL, bDebug);
      }

      if (epsabs <= 0 && (epsrel < 50 * GSL_CONST.GSL_DBL_EPSILON || epsrel < 0.5e-28))
      {
        return new GSL_ERROR("tolerance cannot be acheived with given epsabs and epsrel",
                   GSL_ERR.GSL_EBADTOL, bDebug);
      }

      /* perform the first integration */

      {
        double a1 = a;
        double b1 = 0.5 * (a + b);
        double a2 = b1;
        double b2 = b;

        qc25s(f, a, b, a1, b1, t, out var area1, out var error1, out var err_reliable1);
        qc25s(f, a, b, a2, b2, t, out var area2, out var error2, out var err_reliable2);

        if (error1 > error2)
        {
          workspace.append_interval(a1, b1, area1, error1);
          workspace.append_interval(a2, b2, area2, error2);
        }
        else
        {
          workspace.append_interval(a2, b2, area2, error2);
          workspace.append_interval(a1, b1, area1, error1);
        }

        result0 = area1 + area2;
        abserr0 = error1 + error2;
      }

      /* Test on accuracy */

      tolerance = Math.Max(epsabs, epsrel * Math.Abs(result0));

      /* Test on accuracy, use 0.01 relative error as an extra safety
                 margin on the first iteration (ignored for subsequent iterations) */

      if (abserr0 < tolerance && abserr0 < 0.01 * Math.Abs(result0))
      {
        result = result0;
        abserr = abserr0;

        return null; // GSL_SUCCESS;
      }
      else if (limit == 1)
      {
        result = result0;
        abserr = abserr0;

        return new GSL_ERROR("a maximum of one iteration was insufficient", GSL_ERR.GSL_EMAXITER, bDebug);
      }

      area = result0;
      errsum = abserr0;

      iteration = 2;

      do
      {
        double a1, b1, a2, b2;
        double area12 = 0;
        double error12 = 0;

        /* Bisect the subinterval with the largest error estimate */

        workspace.retrieve(out var a_i, out var b_i, out var r_i, out var e_i);

        a1 = a_i;
        b1 = 0.5 * (a_i + b_i);
        a2 = b1;
        b2 = b_i;

        qc25s(f, a, b, a1, b1, t, out var area1, out var error1, out var err_reliable1);
        qc25s(f, a, b, a2, b2, t, out var area2, out var error2, out var err_reliable2);

        area12 = area1 + area2;
        error12 = error1 + error2;

        errsum += (error12 - e_i);
        area += area12 - r_i;

        if (err_reliable1 && err_reliable2)
        {
          double delta = r_i - area12;

          if (Math.Abs(delta) <= 1.0e-5 * Math.Abs(area12) && error12 >= 0.99 * e_i)
          {
            roundoff_type1++;
          }
          if (iteration >= 10 && error12 > e_i)
          {
            roundoff_type2++;
          }
        }

        tolerance = Math.Max(epsabs, epsrel * Math.Abs(area));

        if (errsum > tolerance)
        {
          if (roundoff_type1 >= 6 || roundoff_type2 >= 20)
          {
            error_type = 2;   /* round off error */
          }

          /* set error flag in the case of bad integrand behaviour at
                         a point of the integration range */

          if (subinterval_too_small(a1, a2, b2))
          {
            error_type = 3;
          }
        }

        workspace.update(a1, b1, area1, error1, a2, b2, area2, error2);

        workspace.retrieve(out a_i, out b_i, out r_i, out e_i);

        iteration++;
      }
      while (iteration < limit && 0 == error_type && errsum > tolerance);

      result = workspace.sum_results();
      abserr = errsum;

      if (errsum <= tolerance)
      {
        return null; // GSL_SUCCESS;
      }
      else if (error_type == 2)
      {
        return new GSL_ERROR("roundoff error prevents tolerance from being achieved",
                   GSL_ERR.GSL_EROUND, bDebug);
      }
      else if (error_type == 3)
      {
        return new GSL_ERROR("bad integrand behavior found in the integration interval",
                   GSL_ERR.GSL_ESING, bDebug);
      }
      else if (iteration == limit)
      {
        return new GSL_ERROR("maximum number of subdivisions reached", GSL_ERR.GSL_EMAXITER, bDebug);
      }
      else
      {
        return new GSL_ERROR("could not integrate function", GSL_ERR.GSL_EFAILED, bDebug);
      }
    }

    #endregion qaws.c

    #region qc25s.c

    /* integration/qc25s.c
 *
 * Copyright (C) 1996, 1997, 1998, 1999, 2000 Brian Gough
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or (at
 * your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

    private struct fn_qaws_params
    {
      public Func<double, double> function;
      public double a;
      public double b;
      public gsl_integration_qaws_table table;
    };

    private static void
    qc25s(Func<double, double> f, double a, double b, double a1, double b1,
           gsl_integration_qaws_table t,
           out double result, out double abserr, out bool err_reliable)
    {
      var fn_params = new fn_qaws_params
      {
        function = f,
        a = a,
        b = b,
        table = t
      };

      Func<double, double> weighted_function;

      if (a1 == a && (t.alpha != 0.0 || t.mu != 0))
      {
        double[] cheb12 = new double[13], cheb24 = new double[25];

        double factor = Math.Pow(0.5 * (b1 - a1), t.alpha + 1.0);

        weighted_function = delegate (double tt)
        { return fn_qaws_R(tt, fn_params); };

        Qcheb.Approximation(weighted_function, a1, b1, cheb12, cheb24);

        if (t.mu == 0)
        {
          double u = factor;

          compute_result(t.ri, cheb12, cheb24, out var res12, out var res24);

          result = u * res24;
          abserr = Math.Abs(u * (res24 - res12));
        }
        else
        {
          double u = factor * Math.Log(b1 - a1);
          double v = factor;

          compute_result(t.ri, cheb12, cheb24, out var res12a, out var res24a);
          compute_result(t.rg, cheb12, cheb24, out var res12b, out var res24b);

          result = u * res24a + v * res24b;
          abserr = Math.Abs(u * (res24a - res12a)) + Math.Abs(v * (res24b - res12b));
        }

        err_reliable = false;

        return;
      }
      else if (b1 == b && (t.beta != 0.0 || t.nu != 0))
      {
        double[] cheb12 = new double[13], cheb24 = new double[25];
        double factor = Math.Pow(0.5 * (b1 - a1), t.beta + 1.0);

        weighted_function = delegate (double tt)
        { return fn_qaws_L(tt, fn_params); };

        Qcheb.Approximation(weighted_function, a1, b1, cheb12, cheb24);

        if (t.nu == 0)
        {
          double u = factor;

          compute_result(t.rj, cheb12, cheb24, out var res12, out var res24);

          result = u * res24;
          abserr = Math.Abs(u * (res24 - res12));
        }
        else
        {
          double u = factor * Math.Log(b1 - a1);
          double v = factor;

          compute_result(t.rj, cheb12, cheb24, out var res12a, out var res24a);
          compute_result(t.rh, cheb12, cheb24, out var res12b, out var res24b);

          result = u * res24a + v * res24b;
          abserr = Math.Abs(u * (res24a - res12a)) + Math.Abs(v * (res24b - res12b));
        }

        err_reliable = false;

        return;
      }
      else
      {
        weighted_function = delegate (double tt)
        { return fn_qaws(tt, fn_params); };

        QK15.Integration(weighted_function, a1, b1, out result, out abserr,
                              out var resabs, out var resasc);

        if (abserr == resasc)
        {
          err_reliable = false;
        }
        else
        {
          err_reliable = true;
        }

        return;
      }
    }

    private static double
    fn_qaws(double x, fn_qaws_params p)
    {
      Func<double, double> f = p.function;
      gsl_integration_qaws_table t = p.table;

      double factor = 1.0;

      if (t.alpha != 0.0)
        factor *= Math.Pow(x - p.a, t.alpha);

      if (t.beta != 0.0)
        factor *= Math.Pow(p.b - x, t.beta);

      if (t.mu == 1)
        factor *= Math.Log(x - p.a);

      if (t.nu == 1)
        factor *= Math.Log(p.b - x);

      return factor * f(x);
    }

    private static double
    fn_qaws_L(double x, fn_qaws_params p)
    {
      Func<double, double> f = p.function;
      gsl_integration_qaws_table t = p.table;

      double factor = 1.0;

      if (t.alpha != 0.0)
        factor *= Math.Pow(x - p.a, t.alpha);

      if (t.mu == 1)
        factor *= Math.Log(x - p.a);

      return factor * f(x);
    }

    private static double
    fn_qaws_R(double x, fn_qaws_params p)
    {
      Func<double, double> f = p.function;
      gsl_integration_qaws_table t = p.table;

      double factor = 1.0;

      if (t.beta != 0.0)
        factor *= Math.Pow(p.b - x, t.beta);

      if (t.nu == 1)
        factor *= Math.Log(p.b - x);

      return factor * f(x);
    }

    private static void
    compute_result(double[] r, double[] cheb12, double[] cheb24,
                    out double result12, out double result24)
    {
      int i;
      double res12 = 0;
      double res24 = 0;

      for (i = 0; i < 13; i++)
      {
        res12 += r[i] * cheb12[i];
      }

      for (i = 0; i < 25; i++)
      {
        res24 += r[i] * cheb24[i];
      }

      result12 = res12;
      result24 = res24;
    }

    #endregion qc25s.c

    #region integration.h and momo.c

    /* Workspace for QAWS integrator */

    private class gsl_integration_qaws_table
    {
      public double alpha = 0;
      public double beta = 0;
      public int mu = 0;
      public int nu = 0;
      public double[] ri = new double[25];
      public double[] rj = new double[25];
      public double[] rg = new double[25];
      public double[] rh = new double[25];
    }

    #endregion integration.h and momo.c
  }
}
