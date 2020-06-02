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
  public enum OscillatoryTerm { Cosine, Sine };

  /// <summary>
  /// QAWO adaptive integration for oscillatory functions
  /// </summary>
  /// <remarks>
  /// The QAWO algorithm is designed for integrands with an oscillatory factor, sin(wx) or
  /// cos(wx). In order to work efficiently the algorithm requires a table of Chebyshev moments
  /// which must be pre-computed with calls to the functions below.
  /// This function uses an adaptive algorithm to compute the integral of f over (a, b) with
  /// the weight function sin(wx) or cos(wx) defined by the table wf:
  /// <code>
  ///            b                                   b
  /// I = Integral dx f(x) sin(wt)   or   I = Integral dx f(x) cos(wx)
  ///            a                                   a
  /// </code>
  /// The results are extrapolated using the epsilon-algorithm to accelerate the convergence
  /// of the integral. The function returns the final approximation from the extrapolation,
  /// result, and an estimate of the absolute error, abserr. The subintervals and their
  /// results are stored in the memory provided by workspace. The maximum number
  /// of subintervals is given by limit, which may not exceed the allocated size of the
  /// workspace.
  /// Those subintervals with "large" widths d where dw &gt; 4 are computed using a 25-point
  /// Clenshaw-Curtis integration rule, which handles the oscillatory behavior. Subintervals
  /// with a "small" widths where dw &lt; 4 are computed using a 15-point Gauss-Kronrod
  /// integration.
  /// <para>Ref.: Gnu Scientific library reference manual (<see href="http://www.gnu.org/software/gsl/" />)</para>
  /// </remarks>
  public class QawoIntegration : IntegrationBase
  {
    #region offical C# interface

    protected static int _defaultOscTableLength = 20;
    protected bool _debug;
    protected gsl_integration_workspace? _workSpace;
    protected gsl_integration_qawo_table? _qawoTable;

    /// <summary>
    /// Creates an instance of this integration class with a default integration rule and default debug flag setting.
    /// </summary>
    public QawoIntegration()
      : this(DefaultDebugFlag)
    {
    }

    /// <summary>
    /// Creates an instance of this integration class with specified integration rule and specified debug flag setting.
    /// </summary>
    /// <param name="debug">Setting of the debug flag for this instance. If the integration fails or the specified accuracy
    /// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
    /// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
    public QawoIntegration(bool debug)
    {
      _debug = debug;
    }

    public GSL_ERROR?
     Integrate(Func<double, double> f,
     double a, double b,
     OscillatoryTerm oscTerm,
     double omega,
     double epsabs, double epsrel, int limit,
     out double result, out double abserr)
    {
      return Integrate(f, a, b, oscTerm, omega, epsabs, epsrel, limit, _debug, out result, out abserr);
    }

    public GSL_ERROR?
      Integrate(Func<double, double> f,
      double a, double b,
      OscillatoryTerm oscTerm,
      double omega,
      double epsabs, double epsrel, int limit,
      bool debug,
      out double result, out double abserr)
    {
      if (null == _workSpace || limit > _workSpace.limit)
        _workSpace = new gsl_integration_workspace(limit);
      if (null == _qawoTable)
      {
        _qawoTable = new gsl_integration_qawo_table(omega, b - a, oscTerm == OscillatoryTerm.Cosine ? gsl_integration_qawo_enum.GSL_INTEG_COSINE : gsl_integration_qawo_enum.GSL_INTEG_SINE, _defaultOscTableLength);
      }
      else
      {
        _qawoTable.set(omega, b - a, oscTerm == OscillatoryTerm.Cosine ? gsl_integration_qawo_enum.GSL_INTEG_COSINE : gsl_integration_qawo_enum.GSL_INTEG_SINE);
      }

      return gsl_integration_qawo(f, a, epsabs, epsrel, limit, _workSpace, _qawoTable, out result, out abserr, debug);
    }

    public static GSL_ERROR?
    Integration(Func<double, double> f,
          double a, double b,
      OscillatoryTerm oscTerm,
     double omega,
          double epsabs, double epsrel,
          int limit,
          out double result, out double abserr,
          ref object tempStorage
          )
    {
      var algo = tempStorage as QawoIntegration;
      if (null == algo)
        tempStorage = algo = new QawoIntegration();
      return algo.Integrate(f, a, b, oscTerm, omega, epsabs, epsrel, limit, out result, out abserr);
    }

    #endregion offical C# interface

    protected enum gsl_integration_qawo_enum { GSL_INTEG_COSINE, GSL_INTEG_SINE };

    protected class gsl_integration_qawo_table
    {
      public int n;
      public double omega;
      public double L;
      public double par;
      public gsl_integration_qawo_enum sine;
      public double[] chebmo;

      public
        gsl_integration_qawo_table(double omega, double L,
                                  gsl_integration_qawo_enum sine,
                                  int n)
      {
        if (n == 0)
        {
          throw new ArgumentOutOfRangeException("table length n must be positive integer");
        }

        chebmo = new double[25 * n];
        this.n = n;
        this.sine = sine;
        this.omega = omega;
        this.L = L;
        par = 0.5 * omega * L;

        /* precompute the moments */

        {
          int i;
          double scale = 1.0;

          for (i = 0; i < this.n; i++)
          {
            compute_moments(par * scale, chebmo, 25 * i);
            scale *= 0.5;
          }
        }
      }

      public void set(double omega, double L, gsl_integration_qawo_enum sine)
      {
        this.omega = omega;
        this.sine = sine;
        this.L = L;
        par = 0.5 * omega * L;

        /* recompute the moments */

        {
          int i;
          double scale = 1.0;

          for (i = 0; i < n; i++)
          {
            compute_moments(par * scale, chebmo, 25 * i);
            scale *= 0.5;
          }
        }

        return; //GSL_SUCCESS;
      }

      public void set_length(double L)
      {
        /* return immediately if the length is the same as the old length */

        if (L == this.L)
          return; // GSL_SUCCESS;

        /* otherwise reset the table and compute the new parameters */

        this.L = L;
        par = 0.5 * omega * L;

        /* recompute the moments */

        {
          int i;
          double scale = 1.0;

          for (i = 0; i < n; i++)
          {
            compute_moments(par * scale, chebmo, 25 * i);
            scale *= 0.5;
          }
        }

        return; // GSL_SUCCESS;
      }

      private static void
 compute_moments(double par, double[] chebmo, int chebmostart)
      {
        double[] v = new double[28], d = new double[25], d1 = new double[25], d2 = new double[25];

        const int noeq = 25;

        double par2 = par * par;
        double par4 = par2 * par2;
        double par22 = par2 + 2.0;

        double sinpar = Math.Sin(par);
        double cospar = Math.Cos(par);

        int i;

        /* compute the chebyschev moments with respect to cosine */

        double ac = 8 * cospar;
        double asi = 24 * par * sinpar;

        v[0] = 2 * sinpar / par;
        v[1] = (8 * cospar + (2 * par2 - 8) * sinpar / par) / par2;
        v[2] = (32 * (par2 - 12) * cospar
                + (2 * ((par2 - 80) * par2 + 192) * sinpar) / par) / par4;

        if (Math.Abs(par) <= 24)
        {
          /* compute the moments as the solution of a boundary value
                         problem using the asyptotic expansion as an endpoint */

          double an2, ass, asap;
          double an = 6;
          int k;

          for (k = 0; k < noeq - 1; k++)
          {
            an2 = an * an;
            d[k] = -2 * (an2 - 4) * (par22 - 2 * an2);
            d2[k] = (an - 1) * (an - 2) * par2;
            d1[k + 1] = (an + 3) * (an + 4) * par2;
            v[k + 3] = asi - (an2 - 4) * ac;
            an = an + 2.0;
          }

          an2 = an * an;

          d[noeq - 1] = -2 * (an2 - 4) * (par22 - 2 * an2);
          v[noeq + 2] = asi - (an2 - 4) * ac;
          v[3] = v[3] - 56 * par2 * v[2];

          ass = par * sinpar;
          asap = (((((210 * par2 - 1) * cospar - (105 * par2 - 63) * ass) / an2
                    - (1 - 15 * par2) * cospar + 15 * ass) / an2
                   - cospar + 3 * ass) / an2
                  - cospar) / an2;
          v[noeq + 2] = v[noeq + 2] - 2 * asap * par2 * (an - 1) * (an - 2);

          dgtsl(noeq, d1, d, d2, LinearAlgebra.VectorMath.ToVector(v, 3, v.Length - 3));
        }
        else
        {
          /* compute the moments by forward recursion */
          int k;
          double an = 4;

          for (k = 3; k < 13; k++)
          {
            double an2 = an * an;
            v[k] = ((an2 - 4) * (2 * (par22 - 2 * an2) * v[k - 1] - ac)
                    + asi - par2 * (an + 1) * (an + 2) * v[k - 2])
              / (par2 * (an - 1) * (an - 2));
            an = an + 2.0;
          }
        }

        for (i = 0; i < 13; i++)
        {
          chebmo[chebmostart + 2 * i] = v[i];
        }

        /* compute the chebyschev moments with respect to sine */

        v[0] = 2 * (sinpar - par * cospar) / par2;
        v[1] = (18 - 48 / par2) * sinpar / par2 + (-2 + 48 / par2) * cospar / par;

        ac = -24 * par * cospar;
        asi = -8 * sinpar;

        if (Math.Abs(par) <= 24)
        {
          /* compute the moments as the solution of a boundary value
                         problem using the asyptotic expansion as an endpoint */

          int k;
          double an2, ass, asap;
          double an = 5;

          for (k = 0; k < noeq - 1; k++)
          {
            an2 = an * an;
            d[k] = -2 * (an2 - 4) * (par22 - 2 * an2);
            d2[k] = (an - 1) * (an - 2) * par2;
            d1[k + 1] = (an + 3) * (an + 4) * par2;
            v[k + 2] = ac + (an2 - 4) * asi;
            an = an + 2.0;
          }

          an2 = an * an;

          d[noeq - 1] = -2 * (an2 - 4) * (par22 - 2 * an2);
          v[noeq + 1] = ac + (an2 - 4) * asi;
          v[2] = v[2] - 42 * par2 * v[1];

          ass = par * cospar;
          asap = (((((105 * par2 - 63) * ass - (210 * par2 - 1) * sinpar) / an2
                    + (15 * par2 - 1) * sinpar
                    - 15 * ass) / an2 - sinpar - 3 * ass) / an2 - sinpar) / an2;
          v[noeq + 1] = v[noeq + 1] - 2 * asap * par2 * (an - 1) * (an - 2);

          dgtsl(noeq, d1, d, d2, LinearAlgebra.VectorMath.ToVector(v, 2, v.Length - 2));
        }
        else
        {
          /* compute the moments by forward recursion */
          int k;
          double an = 3;
          for (k = 2; k < 12; k++)
          {
            double an2 = an * an;
            v[k] = ((an2 - 4) * (2 * (par22 - 2 * an2) * v[k - 1] + asi)
                    + ac - par2 * (an + 1) * (an + 2) * v[k - 2])
              / (par2 * (an - 1) * (an - 2));
            an = an + 2.0;
          }
        }

        for (i = 0; i < 12; i++)
        {
          chebmo[chebmostart + 2 * i + 1] = v[i];
        }
      }

      private static GSL_ERR
dgtsl(int n, double[] c, double[] d, double[] e, LinearAlgebra.IVector<double> b)
      {
        /* solves a tridiagonal matrix A x = b

                     c[1 .. n - 1]   subdiagonal of the matrix A
                     d[0 .. n - 1]   diagonal of the matrix A
                     e[0 .. n - 2]   superdiagonal of the matrix A

                     b[0 .. n - 1]   right hand side, replaced by the solution vector x */

        int k;

        c[0] = d[0];

        if (n == 0)
        {
          return GSL_ERR.GSL_SUCCESS;
        }

        if (n == 1)
        {
          b[0] = b[0] / d[0];
          return GSL_ERR.GSL_SUCCESS;
        }

        d[0] = e[0];
        e[0] = 0;
        e[n - 1] = 0;

        for (k = 0; k < n - 1; k++)
        {
          int k1 = k + 1;

          if (Math.Abs(c[k1]) >= Math.Abs(c[k]))
          {
            {
              double t = c[k1];
              c[k1] = c[k];
              c[k] = t;
            };
            {
              double t = d[k1];
              d[k1] = d[k];
              d[k] = t;
            };
            {
              double t = e[k1];
              e[k1] = e[k];
              e[k] = t;
            };
            {
              double t = b[k1];
              b[k1] = b[k];
              b[k] = t;
            };
          }

          if (c[k] == 0)
          {
            return GSL_ERR.GSL_FAILURE;
          }

          {
            double t = -c[k1] / c[k];

            c[k1] = d[k1] + t * d[k];
            d[k1] = e[k1] + t * e[k];
            e[k1] = 0;
            b[k1] = b[k1] + t * b[k];
          }
        }

        if (c[n - 1] == 0)
        {
          return GSL_ERR.GSL_FAILURE;
        }

        b[n - 1] = b[n - 1] / c[n - 1];

        b[n - 2] = (b[n - 2] - d[n - 2] * b[n - 1]) / c[n - 2];

        for (k = n; k > 2; k--)
        {
          int kb = k - 3;
          b[kb] = (b[kb] - d[kb] * b[kb + 1] - e[kb] * b[kb + 2]) / c[kb];
        }

        return GSL_ERR.GSL_SUCCESS;
      }
    }

    /* integration/qawo.c
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

    protected static GSL_ERROR?
    gsl_integration_qawo(Func<double, double> f,
                          double a,
                          double epsabs, double epsrel,
                          int limit,
                          gsl_integration_workspace workspace,
                          gsl_integration_qawo_table wf,
                          out double result, out double abserr,
                            bool bDebug)
    {
      double area, errsum;
      double res_ext, err_ext;
      double tolerance;

      double ertest = 0;
      double error_over_large_intervals = 0;
      double reseps = 0, abseps = 0, correc = 0;
      int ktmin = 0;
      int roundoff_type1 = 0, roundoff_type2 = 0, roundoff_type3 = 0;
      int error_type = 0;
      bool error_type2 = false;

      int iteration = 0;

      bool positive_integrand = false;
      bool extrapolate = false;
      bool extall = false;
      bool disallow_extrapolation = false;

      var table = new extrapolation_table();

      double b = a + wf.L;
      double abs_omega = Math.Abs(wf.omega);

      /* Initialize results */

      workspace.initialise(a, b);

      result = 0;
      abserr = 0;

      if (limit > workspace.limit)
      {
        return new GSL_ERROR("iteration limit exceeds available workspace", GSL_ERR.GSL_EINVAL, bDebug);
      }

      /* Test on accuracy */

      if (epsabs <= 0 && (epsrel < 50 * GSL_CONST.GSL_DBL_EPSILON || epsrel < 0.5e-28))
      {
        return new GSL_ERROR("tolerance cannot be acheived with given epsabs and epsrel",
                   GSL_ERR.GSL_EBADTOL, bDebug);
      }

      /* Perform the first integration */

      qc25f(f, a, b, wf, 0, out var result0, out var abserr0, out var resabs0, out var resasc0);

      workspace.set_initial_result(result0, abserr0);

      tolerance = Math.Max(epsabs, epsrel * Math.Abs(result0));

      if (abserr0 <= 100 * GSL_CONST.GSL_DBL_EPSILON * resabs0 && abserr0 > tolerance)
      {
        result = result0;
        abserr = abserr0;

        return new GSL_ERROR("cannot reach tolerance because of roundoff error on first attempt", GSL_ERR.GSL_EROUND, bDebug);
      }
      else if ((abserr0 <= tolerance && abserr0 != resasc0) || abserr0 == 0.0)
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

      /* Initialization */

      table.initialise_table();

      if (0.5 * abs_omega * Math.Abs(b - a) <= 2)
      {
        table.append_table(result0);
        extall = true;
      }

      area = result0;
      errsum = abserr0;

      res_ext = result0;
      err_ext = GSL_CONST.GSL_DBL_MAX;

      positive_integrand = test_positivity(result0, resabs0);

      iteration = 1;

      do
      {
        int current_level;
        double a1, b1, a2, b2;
        double area12 = 0;
        double error12 = 0;
        double last_e_i;

        /* Bisect the subinterval with the largest error estimate */

        workspace.retrieve(out var a_i, out var b_i, out var r_i, out var e_i);

        current_level = workspace.level[workspace.i] + 1;

        if (current_level >= wf.n)
        {
          error_type = -1; /* exceeded limit of table */
          break;
        }

        a1 = a_i;
        b1 = 0.5 * (a_i + b_i);
        a2 = b1;
        b2 = b_i;

        iteration++;

        qc25f(f, a1, b1, wf, current_level, out var area1, out var error1, out var resabs1, out var resasc1);
        qc25f(f, a2, b2, wf, current_level, out var area2, out var error2, out var resabs2, out var resasc2);

        area12 = area1 + area2;
        error12 = error1 + error2;
        last_e_i = e_i;

        /* Improve previous approximations to the integral and test for
                     accuracy.

                     We write these expressions in the same way as the original
                     QUADPACK code so that the rounding errors are the same, which
                     makes testing easier. */

        errsum = errsum + error12 - e_i;
        area = area + area12 - r_i;

        tolerance = Math.Max(epsabs, epsrel * Math.Abs(area));

        if (resasc1 != error1 && resasc2 != error2)
        {
          double delta = r_i - area12;

          if (Math.Abs(delta) <= 1.0e-5 * Math.Abs(area12) && error12 >= 0.99 * e_i)
          {
            if (!extrapolate)
            {
              roundoff_type1++;
            }
            else
            {
              roundoff_type2++;
            }
          }
          if (iteration > 10 && error12 > e_i)
          {
            roundoff_type3++;
          }
        }

        /* Test for roundoff and eventually set error flag */

        if (roundoff_type1 + roundoff_type2 >= 10 || roundoff_type3 >= 20)
        {
          error_type = 2;       /* round off error */
        }

        if (roundoff_type2 >= 5)
        {
          error_type2 = true;
        }

        /* set error flag in the case of bad integrand behaviour at
                     a point of the integration range */

        if (subinterval_too_small(a1, a2, b2))
        {
          error_type = 4;
        }

        /* append the newly-created intervals to the list */

        workspace.update(a1, b1, area1, error1, a2, b2, area2, error2);

        if (errsum <= tolerance)
        {
          goto compute_result;
        }

        if (0 != error_type)
        {
          break;
        }

        if (iteration >= limit - 1)
        {
          error_type = 1;
          break;
        }

        /* set up variables on first iteration */

        if (iteration == 2 && extall)
        {
          error_over_large_intervals = errsum;
          ertest = tolerance;
          table.append_table(area);
          continue;
        }

        if (disallow_extrapolation)
        {
          continue;
        }

        if (extall)
        {
          error_over_large_intervals += -last_e_i;

          if (current_level < workspace.maximum_level)
          {
            error_over_large_intervals += error12;
          }

          if (extrapolate)
            goto label70;
        }

        if (workspace.large_interval())
        {
          continue;
        }

        if (extall)
        {
          extrapolate = true;
          workspace.nrmax = 1;
        }
        else
        {
          /* test whether the interval to be bisected next is the
                         smallest interval. */
          int i = workspace.i;
          double width = workspace.blist[i] - workspace.alist[i];

          if (0.25 * Math.Abs(width) * abs_omega > 2)
            continue;

          extall = true;
          error_over_large_intervals = errsum;
          ertest = tolerance;
          continue;
        }

label70:
        if (!error_type2 && error_over_large_intervals > ertest)
        {
          if (workspace.increase_nrmax())
            continue;
        }

        /* Perform extrapolation */

        table.append_table(area);

        if (table.n < 3)
        {
          workspace.reset_nrmax();
          extrapolate = false;
          error_over_large_intervals = errsum;
          continue;
        }

        table.qelg(out reseps, out abseps);

        ktmin++;

        if (ktmin > 5 && err_ext < 0.001 * errsum)
        {
          error_type = 5;
        }

        if (abseps < err_ext)
        {
          ktmin = 0;
          err_ext = abseps;
          res_ext = reseps;
          correc = error_over_large_intervals;
          ertest = Math.Max(epsabs, epsrel * Math.Abs(reseps));
          if (err_ext <= ertest)
            break;
        }

        /* Prepare bisection of the smallest interval. */

        if (table.n == 1)
        {
          disallow_extrapolation = true;
        }

        if (error_type == 5)
        {
          break;
        }

        /* work on interval with largest error */

        workspace.reset_nrmax();
        extrapolate = false;
        error_over_large_intervals = errsum;
      }
      while (iteration < limit);

      result = res_ext;
      abserr = err_ext;

      if (err_ext == GSL_CONST.GSL_DBL_MAX)
        goto compute_result;

      if (0 != error_type || error_type2)
      {
        if (error_type2)
        {
          err_ext += correc;
        }

        if (error_type == 0)
          error_type = 3;

        if (result != 0 && area != 0)
        {
          if (err_ext / Math.Abs(res_ext) > errsum / Math.Abs(area))
            goto compute_result;
        }
        else if (err_ext > errsum)
        {
          goto compute_result;
        }
        else if (area == 0.0)
        {
          goto return_error;
        }
      }

      /*  Test on divergence. */

      {
        double max_area = Math.Max(Math.Abs(res_ext), Math.Abs(area));

        if (!positive_integrand && max_area < 0.01 * resabs0)
          goto return_error;
      }

      {
        double ratio = res_ext / area;

        if (ratio < 0.01 || ratio > 100 || errsum > Math.Abs(area))
          error_type = 6;
      }

      goto return_error;

compute_result:

      result = workspace.sum_results();
      abserr = errsum;

return_error:

      if (error_type > 2)
        error_type--;

      if (error_type == 0)
      {
        return null; // GSL_SUCCESS;
      }
      else if (error_type == 1)
      {
        return new GSL_ERROR("number of iterations was insufficient", GSL_ERR.GSL_EMAXITER, bDebug);
      }
      else if (error_type == 2)
      {
        return new GSL_ERROR("cannot reach tolerance because of roundoff error",
                   GSL_ERR.GSL_EROUND, bDebug);
      }
      else if (error_type == 3)
      {
        return new GSL_ERROR("bad integrand behavior found in the integration interval",
                   GSL_ERR.GSL_ESING, bDebug);
      }
      else if (error_type == 4)
      {
        return new GSL_ERROR("roundoff error detected in extrapolation table", GSL_ERR.GSL_EROUND, bDebug);
      }
      else if (error_type == 5)
      {
        return new GSL_ERROR("integral is divergent, or slowly convergent", GSL_ERR.GSL_EDIVERGE, bDebug);
      }
      else if (error_type == -1)
      {
        return new GSL_ERROR("exceeded limit of trigonometric table", GSL_ERR.GSL_ETABLE, bDebug);
      }
      else
      {
        return new GSL_ERROR("could not integrate function", GSL_ERR.GSL_EFAILED, bDebug);
      }
    }

    #region QC25f

    /* integration/qc25f.c
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

    private struct fn_fourier_params
    {
      public Func<double, double> function;
      public double omega;
    }

    private static void
    qc25f(Func<double, double> f, double a, double b,
           gsl_integration_qawo_table wf, int level,
           out double result, out double abserr, out double resabs, out double resasc)
    {
      double center = 0.5 * (a + b);
      double half_length = 0.5 * (b - a);
      double omega = wf.omega;

      double par = omega * half_length;

      if (Math.Abs(par) < 2)
      {
        Func<double, double> weighted_function;
        fn_fourier_params fn_params;

        fn_params.function = f;
        fn_params.omega = omega;

        if (wf.sine == gsl_integration_qawo_enum.GSL_INTEG_SINE)
        {
          weighted_function = delegate (double t)
          { return fn_sin(t, fn_params); };
        }
        else
        {
          weighted_function = delegate (double t)
          { return fn_cos(t, fn_params); };
        }

        QK15.Integration(weighted_function, a, b, out result, out abserr,
                              out resabs, out resasc);

        return;
      }
      else
      {
        int momentix;
        double[] cheb12 = new double[13];
        double[] cheb24 = new double[25];
        double result_abs, res12_cos, res12_sin, res24_cos, res24_sin;
        double est_cos, est_sin;
        double c, s;
        int i;

        Qcheb.Approximation(f, a, b, cheb12, cheb24);

        if (level >= wf.n)
        {
          /* table overflow should not happen, check before calling */
          new GSL_ERROR("table overflow in internal function", GSL_ERR.GSL_ESANITY, true);
          throw new ArithmeticException("table overflow in internal function"); //
        }

        /* obtain moments from the table */

        momentix = 25 * level;

        res12_cos = cheb12[12] * wf.chebmo[momentix + 12];
        res12_sin = 0;

        for (i = 0; i < 6; i++)
        {
          int k = 10 - 2 * i;
          res12_cos += cheb12[k] * wf.chebmo[momentix + k];
          res12_sin += cheb12[k + 1] * wf.chebmo[momentix + k + 1];
        }

        res24_cos = cheb24[24] * wf.chebmo[momentix + 24];
        res24_sin = 0;

        result_abs = Math.Abs(cheb24[24]);

        for (i = 0; i < 12; i++)
        {
          int k = 22 - 2 * i;
          res24_cos += cheb24[k] * wf.chebmo[momentix + k];
          res24_sin += cheb24[k + 1] * wf.chebmo[momentix + k + 1];
          result_abs += Math.Abs(cheb24[k]) + Math.Abs(cheb24[k + 1]);
        }

        est_cos = Math.Abs(res24_cos - res12_cos);
        est_sin = Math.Abs(res24_sin - res12_sin);

        c = half_length * Math.Cos(center * omega);
        s = half_length * Math.Sin(center * omega);

        if (wf.sine == gsl_integration_qawo_enum.GSL_INTEG_SINE)
        {
          result = c * res24_sin + s * res24_cos;
          abserr = Math.Abs(c * est_sin) + Math.Abs(s * est_cos);
        }
        else
        {
          result = c * res24_cos - s * res24_sin;
          abserr = Math.Abs(c * est_cos) + Math.Abs(s * est_sin);
        }

        resabs = result_abs * half_length;
        resasc = GSL_CONST.GSL_DBL_MAX;

        return;
      }
    }

    private static double fn_sin(double x, fn_fourier_params p)
    {
      Func<double, double> f = p.function;
      double w = p.omega;
      double wx = w * x;
      double sinwx = Math.Sin(wx);
      return f(x) * sinwx;
    }

    private static double fn_cos(double x, fn_fourier_params p)
    {
      Func<double, double> f = p.function;
      double w = p.omega;
      double wx = w * x;
      double coswx = Math.Cos(wx);
      return f(x) * coswx;
    }

    #endregion QC25f
  }
}
