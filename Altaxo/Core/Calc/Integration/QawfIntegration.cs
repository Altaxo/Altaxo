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
  /// QAWF adaptive integration for Fourier integrals.
  /// </summary>
  /// <remarks>
  /// This class attempts to compute a Fourier integral of the function f over the semiinfinite
  ///interval [a,+Infinity).
  /// <code>
  ///            +Infinity                           +Infinity
  /// I = Integral dx f(x) sin(wt)   or   I = Integral dx f(x) cos(wx)
  ///            a                                   a
  /// </code>
  ///The parameter w and choice of sin or cos is taken from the table wf (the length L
  ///can take any value, since it is overridden by this function to a value appropriate for
  ///the fourier integration). The integral is computed using the QAWO algorithm over
  ///each of the subintervals,
  /// <code>
  /// C1 = [a, a + c]
  /// C2 = [a + c, a + 2c]
  /// . . . = . . .
  /// Ck = [a + (k - 1)c, a + kc]
  /// </code>
  ///where c = (2floor(|w|) + 1)Pi/|w|. The width c is chosen to cover an odd number of
  ///periods so that the contributions from the intervals alternate in sign and are monotonically
  ///decreasing when f is positive and monotonically decreasing. The sum of this
  ///sequence of contributions is accelerated using the epsilon-algorithm.
  /// <para>Ref.: Gnu Scientific library reference manual (<see href="http://www.gnu.org/software/gsl/" />)</para>
  /// </remarks>
  public class QawfIntegration : QawoIntegration
  {
    #region offical C# interface

    private gsl_integration_workspace? _cycleWorkspace;

    /// <summary>
    /// Creates an instance of this integration class with a default integration rule and default debug flag setting.
    /// </summary>
    public QawfIntegration()
      : this(DefaultDebugFlag)
    {
    }

    /// <summary>
    /// Creates an instance of this integration class with specified integration rule and specified debug flag setting.
    /// </summary>
    /// <param name="debug">Setting of the debug flag for this instance. If the integration fails or the specified accuracy
    /// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
    /// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
    public QawfIntegration(bool debug)
      : base(debug)
    {
    }

    public GSL_ERROR?
     Integrate(Func<double, double> f,
     double a,
     OscillatoryTerm oscTerm,
     double omega,
     double epsabs, int limit,
     out double result, out double abserr)
    {
      return Integrate(f, a, oscTerm, omega, epsabs, limit, _debug, out result, out abserr);
    }

    public GSL_ERROR?
      Integrate(Func<double, double> f,
      double a,
      OscillatoryTerm oscTerm,
      double omega,
      double epsabs, int limit,
      bool debug,
      out double result, out double abserr)
    {
      if (_workSpace is null || limit > _workSpace.limit)
        _workSpace = new gsl_integration_workspace(limit);

      if (_cycleWorkspace is null || limit > _cycleWorkspace.limit)
        _cycleWorkspace = new gsl_integration_workspace(limit);

      if (_qawoTable is null)
      {
        _qawoTable = new gsl_integration_qawo_table(omega, 1, oscTerm == OscillatoryTerm.Cosine ? gsl_integration_qawo_enum.GSL_INTEG_COSINE : gsl_integration_qawo_enum.GSL_INTEG_SINE, _defaultOscTableLength);
      }
      else
      {
        _qawoTable.set(omega, 1, oscTerm == OscillatoryTerm.Cosine ? gsl_integration_qawo_enum.GSL_INTEG_COSINE : gsl_integration_qawo_enum.GSL_INTEG_SINE);
      }
      return gsl_integration_qawf(f, a, epsabs, limit, _workSpace, _cycleWorkspace, _qawoTable, out result, out abserr, debug);
    }

    public static GSL_ERROR?
    Integration(Func<double, double> f,
          double a,
      OscillatoryTerm oscTerm,
     double omega,
          double epsabs,
          int limit,
          out double result, out double abserr,
          ref object? tempStorage
          )
    {
      var algo = tempStorage as QawfIntegration;
      if (algo is null)
        tempStorage = algo = new QawfIntegration();
      return algo.Integrate(f, a, oscTerm, omega, epsabs, limit, out result, out abserr);
    }

    #endregion offical C# interface

    #region qawf.c

    /* integration/qawf.c
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

    private static GSL_ERROR?
    gsl_integration_qawf(Func<double, double> f,
                          double a,
                          double epsabs,
                          int limit,
                          gsl_integration_workspace workspace,
                          gsl_integration_workspace cycle_workspace,
                          gsl_integration_qawo_table wf,
                          out double result, out double abserr,
                          bool bDebug)
    {
      double area, errsum;
      double res_ext, err_ext;
      double correc, total_error = 0.0, truncation_error = 0;

      int ktmin = 0;
      int iteration = 0;

      var table = new extrapolation_table();

      double cycle;
      double omega = wf.omega;

      double p = 0.9;
      double factor = 1;
      double initial_eps, eps;
      int error_type = 0;

      /* Initialize results */

      workspace.initialise(a, a);

      result = 0;
      abserr = 0;

      if (limit > workspace.limit)
      {
        return new GSL_ERROR("iteration limit exceeds available workspace", GSL_ERR.GSL_EINVAL, bDebug);
      }

      /* Test on accuracy */

      if (epsabs <= 0)
      {
        return new GSL_ERROR("absolute tolerance epsabs must be positive", GSL_ERR.GSL_EBADTOL, bDebug);
      }

      if (omega == 0.0)
      {
        if (wf.sine == gsl_integration_qawo_enum.GSL_INTEG_SINE)
        {
          /* The function sin(w x) f(x) is always zero for w = 0 */

          result = 0;
          abserr = 0;

          return null; // GSL_SUCCESS;
        }
        else
        {
          /* The function cos(w x) f(x) is always f(x) for w = 0 */

          var status = QagiuIntegration.gsl_integration_qagiu(f, a, epsabs, 0.0,
                                              cycle_workspace.limit,
                                              cycle_workspace,
                                              out result, out abserr,
                                              QagiuIntegration.DefaultIntegrationRule, QagiuIntegration.DefaultDebugFlag);
          return status;
        }
      }

      if (epsabs > GSL_CONST.GSL_DBL_MIN / (1 - p))
      {
        eps = epsabs * (1 - p);
      }
      else
      {
        eps = epsabs;
      }

      initial_eps = eps;

      area = 0;
      errsum = 0;

      res_ext = 0;
      err_ext = GSL_CONST.GSL_DBL_MAX;
      correc = 0;

      cycle = (2 * Math.Floor(Math.Abs(omega)) + 1) * Math.PI / Math.Abs(omega);

      wf.set_length(cycle);

      table.initialise_table();

      for (iteration = 0; iteration < limit; iteration++)
      {
        double a1 = a + iteration * cycle;
        double b1 = a1 + cycle;

        double epsabs1 = eps * factor;

        var status = gsl_integration_qawo(f, a1, epsabs1, 0.0, limit,
                                           cycle_workspace, wf,
                                           out var area1, out var error1, false);

        workspace.append_interval(a1, b1, area1, error1);

        factor *= p;

        area = area + area1;
        errsum = errsum + error1;

        /* estimate the truncation error as 50 times the final term */

        truncation_error = 50 * Math.Abs(area1);

        total_error = errsum + truncation_error;

        if (total_error < epsabs && iteration > 4)
        {
          goto compute_result;
        }

        if (error1 > correc)
        {
          correc = error1;
        }

        if (status is not null)
        {
          eps = Math.Max(initial_eps, correc * (1.0 - p));
        }

        if (status is not null && total_error < 10 * correc && iteration > 3)
        {
          goto compute_result;
        }

        table.append_table(area);

        if (table.n < 2)
        {
          continue;
        }

        table.qelg(out var reseps, out var erreps);

        ktmin++;

        if (ktmin >= 15 && err_ext < 0.001 * total_error)
        {
          error_type = 4;
        }

        if (erreps < err_ext)
        {
          ktmin = 0;
          err_ext = erreps;
          res_ext = reseps;

          if (err_ext + 10 * correc <= epsabs)
            break;
          if (err_ext <= epsabs && 10 * correc >= epsabs)
            break;
        }
      }

      if (iteration == limit)
        error_type = 1;

      if (err_ext == GSL_CONST.GSL_DBL_MAX)
        goto compute_result;

      err_ext = err_ext + 10 * correc;

      result = res_ext;
      abserr = err_ext;

      if (error_type == 0)
      {
        return null; // GSL_SUCCESS;
      }

      if (res_ext != 0.0 && area != 0.0)
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

      if (error_type == 4)
      {
        err_ext = err_ext + truncation_error;
      }

      goto return_error;

compute_result:

      result = area;
      abserr = total_error;

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
        return new GSL_ERROR("roundoff error detected in the extrapolation table",
                   GSL_ERR.GSL_EROUND, bDebug);
      }
      else if (error_type == 5)
      {
        return new GSL_ERROR("integral is divergent, or slowly convergent",
                   GSL_ERR.GSL_EDIVERGE, bDebug);
      }
      else
      {
        return new GSL_ERROR("could not integrate function", GSL_ERR.GSL_EFAILED, bDebug);
      }
    }

    #endregion qawf.c
  }
}
