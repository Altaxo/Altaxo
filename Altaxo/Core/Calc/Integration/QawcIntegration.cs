using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Integration
{
  /// <summary>
  /// Adaptive integration for Cauchy principal values.
  /// </summary>
  /// <remarks>
  /// This function computes the Cauchy principal value of the integral of f over (a, b),
  /// with a singularity at c.
  /// The adaptive bisection algorithm of QAG is used, with modifications to ensure that
  /// subdivisions do not occur at the singular point x = c. When a subinterval contains
  /// the point x = c or is close to it then a special 25-point modified Clenshaw-Curtis rule
  /// is used to control the singularity. Further away from the singularity the algorithm
  /// uses an ordinary 15-point Gauss-Kronrod integration rule.
  /// <para>Ref.: Gnu Scientific library reference manual (<see href="http://www.gnu.org/software/gsl/" />)</para>
  /// </remarks>
  public class QawcIntegration : IntegrationBase
  {
		#region offical C# interface
    bool _debug;
    gsl_integration_workspace _workSpace;

    /// <summary>
    /// Creates an instance of this integration class with a default integration rule and default debug flag setting.
    /// </summary>
    public QawcIntegration()
      : this(DefaultDebugFlag)
    {
    }


    /// <summary>
    /// Creates an instance of this integration class with specified integration rule and specified debug flag setting.
    /// </summary>
    /// <param name="integrationRule">Integration rule used for integration.</param>
    /// <param name="debug">Setting of the debug flag for this instance. If the integration fails or the specified accuracy
    /// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
    /// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
    public QawcIntegration(bool debug)
    {
      _debug = debug;
    }
    
    public GSL_ERROR Integrate(ScalarFunctionDD f,
       double a, double b, double c,
       double epsabs, double epsrel, 
			 int limit,
       bool debug,
       out double result, out double abserr)
    {
      if (null == _workSpace || limit > _workSpace.limit)
        _workSpace = new gsl_integration_workspace(limit);

			return gsl_integration_qawc(f, a, b, c, epsabs, epsrel, limit, _workSpace, out result, out abserr, debug);
    }

    public GSL_ERROR Integrate(ScalarFunctionDD f,
				 double a, double b, double c,
          double epsabs, double epsrel,
					int limit,
          out double result, out double abserr)
    {
      return Integrate(f, a, b, c, epsabs, epsrel, limit, _debug, out result, out abserr);
    }


   
    public static GSL_ERROR
    Integration(ScalarFunctionDD f,
				 double a, double b, double c,
          double epsabs, double epsrel,
          int limit,
          bool debug,
          out double result, out double abserr,
          ref object tempStorage)
    {
      var algo = tempStorage as QawcIntegration;
      if (null == algo)
        tempStorage = algo = new QawcIntegration(debug);
      return algo.Integrate(f, a,b,c, epsabs, epsrel, limit, debug, out result, out abserr);
    }

    public static GSL_ERROR
    Integration(ScalarFunctionDD f,
					double a, double b, double c,
          double epsabs, double epsrel,
          int limit,
          out double result, out double abserr,
          ref object tempStorage
          )
    {
      var algo = tempStorage as QawcIntegration;
      if (null == algo)
        tempStorage = algo = new QawcIntegration();
      return algo.Integrate(f, a,b,c, epsabs, epsrel, limit, out result, out abserr);
    }


		public static GSL_ERROR
	 Integration(ScalarFunctionDD f,
				double a, double b, double c,
				 double epsabs, double epsrel,
				 int limit,
				 bool debug,
				 out double result, out double abserr)
		{
			object tempStorage = null;
			return Integration(f, a, b, c, epsabs, epsrel, limit, debug, out result, out abserr, ref tempStorage);
		}

		public static GSL_ERROR
		Integration(ScalarFunctionDD f,
					double a, double b, double c,
					double epsabs, double epsrel,
					int limit,
					out double result, out double abserr)
		{
			object tempStorage = null;
			return Integration(f, a, b, c, epsabs, epsrel, limit, out result, out abserr, ref tempStorage);
		}
    #endregion



    #region qawc.c
    /* integration/qawc.c
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



    static GSL_ERROR
    gsl_integration_qawc(ScalarFunctionDD f,
                          double a, double b, double c,
                          double epsabs, double epsrel,
                          int limit,
                          gsl_integration_workspace workspace,
                          out double result, out double abserr,
                          bool bDebug)
    {
      double area, errsum;
      double result0, abserr0;
      double tolerance;
      int iteration = 0;
      int roundoff_type1 = 0, roundoff_type2 = 0, error_type = 0;
      bool err_reliable;
      int sign = 1;
      double lower, higher;

      /* Initialize results */

      result = 0;
      abserr = 0;

      if (limit > workspace.limit)
      {
        return new GSL_ERROR("iteration limit exceeds available workspace", GSL_ERR.GSL_EINVAL, bDebug);
      }

      if (b < a)
      {
        lower = b;
        higher = a;
        sign = -1;
      }
      else
      {
        lower = a;
        higher = b;
      }

      workspace.initialise(lower, higher);

      if (epsabs <= 0 && (epsrel < 50 * GSL_CONST.GSL_DBL_EPSILON || epsrel < 0.5e-28))
      {
        return new GSL_ERROR("tolerance cannot be acheived with given epsabs and epsrel", GSL_ERR.GSL_EBADTOL, bDebug);
      }

      if (c == a || c == b)
      {
        return new GSL_ERROR("cannot integrate with singularity on endpoint", GSL_ERR.GSL_EINVAL, bDebug);
      }

      /* perform the first integration */

      qc25c(f, lower, higher, c, out result0, out abserr0, out err_reliable);

      workspace.set_initial_result(result0, abserr0);

      /* Test on accuracy, use 0.01 relative error as an extra safety
         margin on the first iteration (ignored for subsequent iterations) */

      tolerance = Math.Max(epsabs, epsrel * Math.Abs(result0));

      if (abserr0 < tolerance && abserr0 < 0.01 * Math.Abs(result0))
      {
        result = sign * result0;
        abserr = abserr0;

        return null; // GSL_SUCCESS;
      }
      else if (limit == 1)
      {
        result = sign * result0;
        abserr = abserr0;

        return new GSL_ERROR("a maximum of one iteration was insufficient", GSL_ERR.GSL_EMAXITER, bDebug);
      }

      area = result0;
      errsum = abserr0;

      iteration = 1;

      do
      {
        double a1, b1, a2, b2;
        double a_i, b_i, r_i, e_i;
        double area1 = 0, area2 = 0, area12 = 0;
        double error1 = 0, error2 = 0, error12 = 0;
        bool err_reliable1, err_reliable2;

        /* Bisect the subinterval with the largest error estimate */

        workspace.retrieve(out a_i, out b_i, out r_i, out e_i);

        a1 = a_i;
        b1 = 0.5 * (a_i + b_i);
        a2 = b1;
        b2 = b_i;

        if (c > a1 && c <= b1)
        {
          b1 = 0.5 * (c + b2);
          a2 = b1;
        }
        else if (c > b1 && c < b2)
        {
          b1 = 0.5 * (a1 + c);
          a2 = b1;
        }

        qc25c(f, a1, b1, c, out area1, out error1, out err_reliable1);
        qc25c(f, a2, b2, c, out area2, out error2, out err_reliable2);

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

      result = sign * workspace.sum_results();
      abserr = errsum;

      if (errsum <= tolerance)
      {
        return null; // GSL_SUCCESS;
      }
      else if (error_type == 2)
      {
        return new GSL_ERROR("roundoff error prevents tolerance from being achieved", GSL_ERR.GSL_EROUND, bDebug);
      }
      else if (error_type == 3)
      {
        return new GSL_ERROR("bad integrand behavior found in the integration interval", GSL_ERR.GSL_ESING, bDebug);
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
    #endregion
    #region qc25c.c
    /* integration/qc25c.c
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

    struct fn_cauchy_params
    {
      public ScalarFunctionDD function;
      public double singularity;
    }




    static void
    qc25c(ScalarFunctionDD f, double a, double b, double c,
           out double result, out double abserr, out bool err_reliable)
    {
      double cc = (2 * c - b - a) / (b - a);

      if (Math.Abs(cc) > 1.1)
      {
        double resabs, resasc;

        fn_cauchy_params fn_params;
        fn_params.function = f;
        fn_params.singularity = c;

        ScalarFunctionDD weighted_function = delegate(double t) { return fn_cauchy(t, fn_params); };


        QK15.Integration(weighted_function, a, b, out result, out abserr,
                              out resabs, out resasc);

        if (abserr == resasc)
        {
          err_reliable = false; // 0;
        }
        else
        {
          err_reliable = true; // 1;
        }

        return;
      }
      else
      {
        double[] cheb12 = new double[13];
        double[] cheb24 = new double[25];
        double[] moment = new double[25];
        double res12 = 0, res24 = 0;
        int i;
        Qcheb.Approximation(f, a, b, cheb12, cheb24);
        compute_moments(cc, moment);

        for (i = 0; i < 13; i++)
        {
          res12 += cheb12[i] * moment[i];
        }

        for (i = 0; i < 25; i++)
        {
          res24 += cheb24[i] * moment[i];
        }

        result = res24;
        abserr = Math.Abs(res24 - res12);
        err_reliable = false; // 0;

        return;
      }
    }

    static double
    fn_cauchy(double x, fn_cauchy_params p)
    {
      ScalarFunctionDD f = p.function;
      double c = p.singularity;
      return f(x) / (x - c);
    }

    static void
    compute_moments(double cc, double[] moment)
    {
      int k;

      double a0 = Math.Log(Math.Abs((1.0 - cc) / (1.0 + cc)));
      double a1 = 2 + a0 * cc;

      moment[0] = a0;
      moment[1] = a1;

      for (k = 2; k < 25; k++)
      {
        double a2;

        if ((k % 2) == 0)
        {
          a2 = 2.0 * cc * a1 - a0;
        }
        else
        {
          double km1 = k - 1.0;
          a2 = 2.0 * cc * a1 - a0 - 4.0 / (km1 * km1 - 1.0);
        }

        moment[k] = a2;

        a0 = a1;
        a1 = a2;
      }
    }

  #endregion

  }
}
