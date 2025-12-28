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
  /// Adaptive integration with known singular points.
  /// </summary>
  /// <remarks>
  /// This class applies the adaptive integration algorithm QAGS taking account of the
  /// user-supplied locations of singular points. The array pts of length npts should contain
  /// the endpoints of the integration ranges defined by the integration region and locations
  /// of the singularities. For example, to integrate over the region (a, b) with break-points
  /// at x1, x2, x3 (where a &lt; x1 &lt; x2 &lt; x3 &lt; b) the following pts array should be used:
  /// <code>
  /// pts[0] = a
  /// pts[1] = x1
  /// pts[2] = x2
  /// pts[3] = x3
  /// pts[4] = b
  ///  </code>
  /// with npts = 5.
  /// If you know the locations of the singular points in the integration region then this
  /// routine will be faster than QAGS.
  /// <para>Ref.: Gnu Scientific library reference manual (<see href="http://www.gnu.org/software/gsl/" />)</para>
  /// </remarks>
  public class QagpIntegration : IntegrationBase
  {
    #region offical C# interface

    private bool _debug;
    private gsl_integration_workspace? _workSpace;
    private gsl_integration_rule _integrationRule;

    /// <summary>
    /// Returns the default integration rule used for this class.
    /// </summary>
    public static gsl_integration_rule DefaultIntegrationRule
    {
      get
      {
        return new QK21().Integrate;
      }
    }

    /// <summary>
    /// Creates an instance of this integration class with a default integration rule and default debug flag setting.
    /// </summary>
    public QagpIntegration()
      : this(DefaultIntegrationRule, DefaultDebugFlag)
    {
    }

    /// <summary>
    /// Creates an instance of this integration class with a default integration rule and specified debug flag setting.
    /// </summary>
    /// <param name="debug">Setting of the debug flag for this instance. If the integration fails or the specified accuracy
    /// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
    /// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
    public QagpIntegration(bool debug)
      : this(DefaultIntegrationRule, debug)
    {
    }

    /// <summary>
    /// Creates an instance of this integration class with specified integration rule and specified debug flag setting.
    /// </summary>
    /// <param name="integrationRule">Integration rule used for integration.</param>
    /// <param name="debug">Setting of the debug flag for this instance. If the integration fails or the specified accuracy
    /// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
    /// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
    public QagpIntegration(gsl_integration_rule integrationRule, bool debug)
    {
      _integrationRule = integrationRule;
      _debug = debug;
    }

    /// <summary>
    /// Adaptive integration with known singular points.
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="pts">Range of integration including the known singular points, see remarks here: <see cref="QagpIntegration"/></param>
    /// <param name="npts">Number of valid points in the array pts. Must be less or equal the size of the array pts.</param>
    /// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
    /// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
    /// <param name="limit">Maximum number of subintervals used for integration.</param>
    /// <param name="integrationRule">Integration rule used for integration (only for this function call).</param>
    /// <param name="debug">Setting of the debug flag (only for this function call). If the integration fails or the specified accuracy
    /// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
    /// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
    /// <param name="result">On return, contains the integration result.</param>
    /// <param name="abserr">On return, contains the absolute error of integration.</param>
    /// <returns>Null if successfull, otherwise the appropriate error code.</returns>
    public GSL_ERROR? Integrate(Func<double, double> f,
       double[] pts, int npts,
       double epsabs, double epsrel, int limit,
       gsl_integration_rule integrationRule, bool debug,
       out double result, out double abserr)
    {
      if (_workSpace is null || limit > _workSpace.limit)
        _workSpace = new gsl_integration_workspace(limit);

      return qagp(f, pts, npts, epsabs, epsrel, limit, _workSpace, out result, out abserr, integrationRule, debug);
    }

    /// <summary>
    /// Adaptive integration with known singular points using the integration rule and debug setting given in the constructor.
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="pts">Range of integration including the known singular points, see remarks here: <see cref="QagpIntegration"/></param>
    /// <param name="npts">Number of valid points in the array pts. Must be less or equal the size of the array pts.</param>
    /// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
    /// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
    /// <param name="limit">Maximum number of subintervals used for integration.</param>
    /// <param name="result">On return, contains the integration result.</param>
    /// <param name="abserr">On return, contains the absolute error of integration.</param>
    /// <returns>Null if successfull, otherwise the appropriate error code.</returns>
    public GSL_ERROR? Integrate(Func<double, double> f,
          double[] pts, int npts,
          double epsabs, double epsrel, int limit,
          out double result, out double abserr)
    {
      return Integrate(f, pts, npts, epsabs, epsrel, limit, _integrationRule, _debug, out result, out abserr);
    }

    /// <summary>
    /// Adaptive integration with known singular points.
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="pts">Range of integration including the known singular points, see remarks here: <see cref="QagpIntegration"/></param>
    /// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
    /// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
    /// <param name="limit">Maximum number of subintervals used for integration.</param>
    /// <param name="integrationRule">Integration rule used for integration (only for this function call).</param>
    /// <param name="debug">Setting of the debug flag (only for this function call). If the integration fails or the specified accuracy
    /// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
    /// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
    /// <param name="result">On return, contains the integration result.</param>
    /// <param name="abserr">On return, contains the absolute error of integration.</param>
    /// <returns>Null if successfull, otherwise the appropriate error code.</returns>
    public GSL_ERROR? Integrate(Func<double, double> f,
      double[] pts,
      double epsabs, double epsrel, int limit,
      gsl_integration_rule integrationRule, bool debug,
      out double result, out double abserr)
    {
      return Integrate(f, pts, pts.Length, epsabs, epsrel, limit, integrationRule, debug, out result, out abserr);
    }

    /// <summary>
    /// Adaptive integration with known singular points using the integration rule and debug setting given in the constructor.
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="pts">Range of integration including the known singular points, see remarks here: <see cref="QagpIntegration"/></param>
    /// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
    /// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
    /// <param name="limit">Maximum number of subintervals used for integration.</param>
    /// <param name="result">On return, contains the integration result.</param>
    /// <param name="abserr">On return, contains the absolute error of integration.</param>
    /// <returns>Null if successfull, otherwise the appropriate error code.</returns>
    public GSL_ERROR? Integrate(Func<double, double> f,
      double[] pts,
      double epsabs, double epsrel, int limit,
      out double result, out double abserr)
    {
      return Integrate(f, pts, pts.Length, epsabs, epsrel, limit, _integrationRule, _debug, out result, out abserr);
    }

    /// <summary>
    /// Adaptive integration with known singular points.
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="pts">Range of integration including the known singular points, see remarks here: <see cref="QagpIntegration"/></param>
    /// <param name="npts">Number of valid points in the array pts. Must be less or equal the size of the array pts.</param>
    /// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
    /// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
    /// <param name="limit">Maximum number of subintervals used for integration.</param>
    /// <param name="integrationRule">Integration rule used for integration (only for this function call).</param>
    /// <param name="debug">Setting of the debug flag (only for this function call). If the integration fails or the specified accuracy
    /// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
    /// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
    /// <param name="result">On return, contains the integration result.</param>
    /// <param name="abserr">On return, contains the absolute error of integration.</param>
    /// <param name="tempStorage">Provides a temporary storage object that you can reuse for repeating function calls.</param>
    /// <returns>Null if successfull, otherwise the appropriate error code.</returns>
    public static GSL_ERROR?
    Integration(Func<double, double> f,
          double[] pts, int npts,
          double epsabs, double epsrel,
          int limit,
          gsl_integration_rule integrationRule, bool debug,
          out double result, out double abserr,
          ref object? tempStorage)
    {
      var algo = tempStorage as QagpIntegration;
      if (algo is null)
        tempStorage = algo = new QagpIntegration(integrationRule, debug);
      return algo.Integrate(f, pts, npts, epsabs, epsrel, limit, integrationRule, debug, out result, out abserr);
    }

    /// <summary>
    /// Adaptive integration with known singular points using default settings for integration rule and debugging.
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="pts">Range of integration including the known singular points, see remarks here: <see cref="QagpIntegration"/></param>
    /// <param name="npts">Number of valid points in the array pts. Must be less or equal the size of the array pts.</param>
    /// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
    /// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
    /// <param name="limit">Maximum number of subintervals used for integration.</param>
    /// <param name="result">On return, contains the integration result.</param>
    /// <param name="abserr">On return, contains the absolute error of integration.</param>
    /// <param name="tempStorage">Provides a temporary storage object that you can reuse for repeating function calls.</param>
    /// <returns>Null if successfull, otherwise the appropriate error code.</returns>
    public static GSL_ERROR?
    Integration(Func<double, double> f,
          double[] pts, int npts,
          double epsabs, double epsrel,
          int limit,
          out double result, out double abserr,
          ref object? tempStorage
          )
    {
      var algo = tempStorage as QagpIntegration;
      if (algo is null)
        tempStorage = algo = new QagpIntegration();
      return algo.Integrate(f, pts, npts, epsabs, epsrel, limit, out result, out abserr);
    }

    /// <summary>
    /// Adaptive integration with known singular points.
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="pts">Range of integration including the known singular points, see remarks here: <see cref="QagpIntegration"/></param>
    /// <param name="npts">Number of valid points in the array pts. Must be less or equal the size of the array pts.</param>
    /// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
    /// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
    /// <param name="limit">Maximum number of subintervals used for integration.</param>
    /// <param name="integrationRule">Integration rule used for integration (only for this function call).</param>
    /// <param name="debug">Setting of the debug flag (only for this function call). If the integration fails or the specified accuracy
    /// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
    /// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
    /// <param name="result">On return, contains the integration result.</param>
    /// <param name="abserr">On return, contains the absolute error of integration.</param>
    /// <returns>Null if successfull, otherwise the appropriate error code.</returns>
    public static GSL_ERROR?
   Integration(Func<double, double> f,
     double[] pts, int npts,
     double epsabs, double epsrel,
     int limit,
      gsl_integration_rule integrationRule, bool debug,
     out double result, out double abserr
     )
    {
      object? tempStorage = null;
      return Integration(f, pts, npts, epsabs, epsrel, limit, integrationRule, debug, out result, out abserr, ref tempStorage);
    }

    /// <summary>
    /// Adaptive integration with known singular points using default settings for integration rule and debugging.
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="pts">Range of integration including the known singular points, see remarks here: <see cref="QagpIntegration"/></param>
    /// <param name="npts">Number of valid points in the array pts. Must be less or equal the size of the array pts.</param>
    /// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
    /// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
    /// <param name="limit">Maximum number of subintervals used for integration.</param>
    /// <param name="result">On return, contains the integration result.</param>
    /// <param name="abserr">On return, contains the absolute error of integration.</param>
    /// <returns>Null if successfull, otherwise the appropriate error code.</returns>
    public static GSL_ERROR?
    Integration(Func<double, double> f,
      double[] pts, int npts,
      double epsabs, double epsrel,
      int limit,
      out double result, out double abserr
      )
    {
      object? tempStorage = null;
      return Integration(f, pts, npts, epsabs, epsrel, limit, out result, out abserr, ref tempStorage);
    }

    #endregion offical C# interface

    /* integration/qagp.c
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
    /// Internal implementation of the QAGP adaptive integrator.
    /// </summary>
    /// <param name="f">The integrand to evaluate.</param>
    /// <param name="pts">Array of points defining the integration range and known singular points.</param>
    /// <param name="npts">Number of valid entries in <paramref name="pts"/> (should be at least 2).</param>
    /// <param name="epsabs">Absolute error tolerance.</param>
    /// <param name="epsrel">Relative error tolerance.</param>
    /// <param name="limit">Maximum number of subintervals allowed for the algorithm.</param>
    /// <param name="workspace">Workspace used to manage subintervals and error estimates.</param>
    /// <param name="result">On return, contains the computed integral value.</param>
    /// <param name="abserr">On return, contains the estimated absolute error.</param>
    /// <param name="q">Quadrature rule used for subinterval evaluations.</param>
    /// <param name="bDebug">When true, detailed errors include debug information.</param>
    /// <returns>Null on success, otherwise a <see cref="GSL_ERROR"/> describing the failure.</returns>
    private static GSL_ERROR?
    qagp(Func<double, double> f,
          double[] pts, int npts,
          double epsabs, double epsrel,
          int limit,
          gsl_integration_workspace workspace,
          out double result, out double abserr,
          gsl_integration_rule q, bool bDebug)
    {
      double area, errsum;
      double res_ext, err_ext;
      double result0, abserr0, resabs0;
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
      bool disallow_extrapolation = false;

      var table = new extrapolation_table();

      int nint = npts - 1; /* number of intervals */

      int[] ndin = workspace.level; /* temporarily alias ndin to level */

      int i;

      /* Initialize results */

      result = 0;
      abserr = 0;

      /* Test on validity of parameters */

      if (limit > workspace.limit)
      {
        return new GSL_ERROR("iteration limit exceeds available workspace", GSL_ERR.GSL_EINVAL, bDebug);
      }

      if (npts > workspace.limit)
      {
        return new GSL_ERROR("npts exceeds size of workspace", GSL_ERR.GSL_EINVAL, bDebug);
      }

      if (epsabs <= 0 && (epsrel < 50 * GSL_CONST.GSL_DBL_EPSILON || epsrel < 0.5e-28))
      {
        return new GSL_ERROR("tolerance cannot be acheived with given epsabs and epsrel",
                   GSL_ERR.GSL_EBADTOL, bDebug);
      }

      /* Check that the integration range and break points are an
                 ascending sequence */

      for (i = 0; i < nint; i++)
      {
        if (pts[i + 1] < pts[i])
        {
          return new GSL_ERROR("points are not in an ascending sequence", GSL_ERR.GSL_EINVAL, bDebug);
        }
      }

      /* Perform the first integration */

      result0 = 0;
      abserr0 = 0;
      resabs0 = 0;

      workspace.initialise(0.0, 0.0);

      for (i = 0; i < nint; i++)
      {
        double a1 = pts[i];
        double b1 = pts[i + 1];

        q(f, a1, b1, out var area1, out var error1, out var resabs1, out var resasc1);

        result0 = result0 + area1;
        abserr0 = abserr0 + error1;
        resabs0 = resabs0 + resabs1;

        workspace.append_interval(a1, b1, area1, error1);

        if (error1 == resasc1 && error1 != 0.0)
        {
          ndin[i] = 1;
        }
        else
        {
          ndin[i] = 0;
        }
      }

      /* Compute the initial error estimate */

      errsum = 0.0;

      for (i = 0; i < nint; i++)
      {
        if (0 != ndin[i])
        {
          workspace.elist[i] = abserr0;
        }

        errsum = errsum + workspace.elist[i];
      }

      for (i = 0; i < nint; i++)
      {
        workspace.level[i] = 0;
      }

      /* Sort results into order of decreasing error via the indirection
                 array order[] */

      workspace.sort_results();

      /* Test on accuracy */

      tolerance = Math.Max(epsabs, epsrel * Math.Abs(result0));

      if (abserr0 <= 100 * GSL_CONST.GSL_DBL_EPSILON * resabs0 && abserr0 > tolerance)
      {
        result = result0;
        abserr = abserr0;

        return new GSL_ERROR("cannot reach tolerance because of roundoff error on first attempt", GSL_ERR.GSL_EROUND, bDebug);
      }
      else if (abserr0 <= tolerance)
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
      table.append_table(result0);

      area = result0;

      res_ext = result0;
      err_ext = GSL_CONST.GSL_DBL_MAX;

      error_over_large_intervals = errsum;
      ertest = tolerance;

      positive_integrand = test_positivity(result0, resabs0);

      iteration = nint - 1;

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

        a1 = a_i;
        b1 = 0.5 * (a_i + b_i);
        a2 = b1;
        b2 = b_i;

        iteration++;

        q(f, a1, b1, out var area1, out var error1, out var resabs1, out var resasc1);
        q(f, a2, b2, out var area2, out var error2, out var resabs2, out var resasc2);

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

          if (i > 10 && error12 > e_i)
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

        if (disallow_extrapolation)
        {
          continue;
        }

        error_over_large_intervals += -last_e_i;

        if (current_level < workspace.maximum_level)
        {
          error_over_large_intervals += error12;
        }

        if (!extrapolate)
        {
          /* test whether the interval to be bisected next is the
                         smallest interval. */
          if (workspace.large_interval())
            continue;

          extrapolate = true;
          workspace.nrmax = 1;
        }

        /* The smallest interval has the largest error.  Before
                     bisecting decrease the sum of the errors over the larger
                     intervals (error_over_large_intervals) and perform
                     extrapolation. */

        if (!error_type2 && error_over_large_intervals > ertest)
        {
          if (workspace.increase_nrmax())
            continue;
        }

        /* Perform extrapolation */

        table.append_table(area);

        if (table.n < 3)
        {
          goto skip_extrapolation;
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

skip_extrapolation:

        workspace.reset_nrmax();
        extrapolate = false;
        error_over_large_intervals = errsum;
      }
      while (iteration < limit);

      result = res_ext;
      abserr = err_ext;

      if (err_ext == GSL_CONST.GSL_DBL_MAX)
        goto compute_result;

      if ((0 != error_type) || error_type2)
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
  }
}
