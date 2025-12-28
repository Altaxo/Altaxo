#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or (at
//    your option) any later version.
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
  /// Adaptive integration.
  /// </summary>
  /// <remarks>
  /// The QAG algorithm is a simple adaptive integration procedure. The integration region is
  /// divided into subintervals, and on each iteration the subinterval with the largest estimated
  /// error is bisected. This reduces the overall error rapidly, as the subintervals become concentrated
  /// around local difficulties in the integrand. These subintervals are managed by a
  /// gsl_integration_workspace struct, which handles the memory for the subinterval ranges,
  /// results and error estimates.
  /// <para>Ref.: GNU Scientific Library reference manual (<see href="http://www.gnu.org/software/gsl/" />)</para>
  /// </remarks>
  public class QagIntegration : IntegrationBase
  {
    #region qag.c

    /* integration/qag.c
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

    /// <summary>
    /// Core implementation of the QAG adaptive integration algorithm.
    /// Mirrors the behavior of the original QUADPACK/GSL implementation for adaptive bisection
    /// and error control over the interval [a,b].
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="a">Lower integration limit.</param>
    /// <param name="b">Upper integration limit.</param>
    /// <param name="epsabs">Absolute error tolerance. Set to zero to rely on relative tolerance.</param>
    /// <param name="epsrel">Relative error tolerance. Set to zero to rely on absolute tolerance.</param>
    /// <param name="limit">Maximum number of subintervals allowed (workspace size).</param>
    /// <param name="workspace">Workspace used to manage subintervals and store intermediate results.</param>
    /// <param name="result">On return, contains the integration result when successful.</param>
    /// <param name="abserr">On return, contains the estimated absolute error of the result.</param>
    /// <param name="q">Integration rule (quadrature) used for local interval approximations.</param>
    /// <param name="bDebug">Debug flag that controls whether detailed errors throw exceptions.</param>
    /// <returns>Null if successful; otherwise a <see cref="GSL_ERROR"/> describing the error.</returns>
    private static GSL_ERROR?
    qag(Func<double, double> f,
         double a, double b,
         double epsabs, double epsrel,
         int limit,
         gsl_integration_workspace workspace,
         out double result, out double abserr,
         gsl_integration_rule q,
         bool bDebug)
    {
      double area, errsum;
      double tolerance;
      int iteration = 0;
      int roundoff_type1 = 0, roundoff_type2 = 0, error_type = 0;

      double round_off;

      /* Initialize results */

      workspace.initialise(a, b);

      result = 0;
      abserr = 0;

      if (limit > workspace.limit)
      {
        return new GSL_ERROR("iteration limit exceeds available workspace", GSL_ERR.GSL_EINVAL, true);
      }

      if (epsabs <= 0 && (epsrel < 50 * GSL_CONST.GSL_DBL_EPSILON || epsrel < 0.5e-28))
      {
        return new GSL_ERROR("tolerance cannot be acheived with given epsabs and epsrel",
                   GSL_ERR.GSL_EBADTOL, true);
      }

      /* perform the first integration */

      q(f, a, b, out var result0, out var abserr0, out var resabs0, out var resasc0);

      workspace.set_initial_result(result0, abserr0);

      /* Test on accuracy */

      tolerance = Math.Max(epsabs, epsrel * Math.Abs(result0));

      /* need IEEE rounding here to match original quadpack behavior */

      round_off = GSL_COERCE_DBL(50 * GSL_CONST.GSL_DBL_EPSILON * resabs0);

      if (abserr0 <= round_off && abserr0 > tolerance)
      {
        result = result0;
        abserr = abserr0;

        return new GSL_ERROR("cannot reach tolerance because of roundoff error on first attempt", GSL_ERR.GSL_EROUND, bDebug);
      }
      else if ((abserr0 <= tolerance && abserr0 != resasc0) || abserr0 == 0.0)
      {
        result = result0;
        abserr = abserr0;

        return null; //GSL_SUCCESS;
      }
      else if (limit == 1)
      {
        result = result0;
        abserr = abserr0;

        return new GSL_ERROR("a maximum of one iteration was insufficient", GSL_ERR.GSL_EMAXITER, bDebug);
      }

      area = result0;
      errsum = abserr0;

      iteration = 1;

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

        q(f, a1, b1, out var area1, out var error1, out var resabs1, out var resasc1);
        q(f, a2, b2, out var area2, out var error2, out var resabs2, out var resasc2);

        area12 = area1 + area2;
        error12 = error1 + error2;

        errsum += (error12 - e_i);
        area += area12 - r_i;

        if (resasc1 != error1 && resasc2 != error2)
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
        return null; //GSL_SUCCESS;
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

    #endregion qag.c
  }
}
