#region Original Copyright
// The original file is from GSL1.9/Integration/qags.c
/* integration/qags.c
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Integration
{
  public class GSL_CONST
  {
    public const double GSL_DBL_EPSILON = 2.2204460492503131e-16;
    public const double GSL_SQRT_DBL_EPSILON = 1.4901161193847656e-08;
    public const double GSL_ROOT3_DBL_EPSILON = 6.0554544523933429e-06;
    public const double GSL_ROOT4_DBL_EPSILON = 1.2207031250000000e-04;
    public const double GSL_ROOT5_DBL_EPSILON = 7.4009597974140505e-04;
    public const double GSL_ROOT6_DBL_EPSILON = 2.4607833005759251e-03;
    public const double GSL_LOG_DBL_EPSILON = (-3.6043653389117154e+01);

    public const double GSL_DBL_MIN = 2.2250738585072014e-308;
    public const double GSL_SQRT_DBL_MIN = 1.4916681462400413e-154;
    public const double GSL_ROOT3_DBL_MIN = 2.8126442852362996e-103;
    public const double GSL_ROOT4_DBL_MIN = 1.2213386697554620e-77;
    public const double GSL_ROOT5_DBL_MIN = 2.9476022969691763e-62;
    public const double GSL_ROOT6_DBL_MIN = 5.3034368905798218e-52;
    public const double GSL_LOG_DBL_MIN = (-7.0839641853226408e+02);

    public const double GSL_DBL_MAX = 1.7976931348623157e+308;
    public const double GSL_SQRT_DBL_MAX = 1.3407807929942596e+154;
    public const double GSL_ROOT3_DBL_MAX = 5.6438030941222897e+102;
    public const double GSL_ROOT4_DBL_MAX = 1.1579208923731620e+77;
    public const double GSL_ROOT5_DBL_MAX = 4.4765466227572707e+61;
    public const double GSL_ROOT6_DBL_MAX = 2.3756689782295612e+51;
    public const double GSL_LOG_DBL_MAX = 7.0978271289338397e+02;

    public const double GSL_FLT_EPSILON = 1.1920928955078125e-07;
    public const double GSL_SQRT_FLT_EPSILON = 3.4526698300124393e-04;
    public const double GSL_ROOT3_FLT_EPSILON = 4.9215666011518501e-03;
    public const double GSL_ROOT4_FLT_EPSILON = 1.8581361171917516e-02;
    public const double GSL_ROOT5_FLT_EPSILON = 4.1234622211652937e-02;
    public const double GSL_ROOT6_FLT_EPSILON = 7.0153878019335827e-02;
    public const double GSL_LOG_FLT_EPSILON = (-1.5942385152878742e+01);

    public const double GSL_FLT_MIN = 1.1754943508222875e-38;
    public const double GSL_SQRT_FLT_MIN = 1.0842021724855044e-19;
    public const double GSL_ROOT3_FLT_MIN = 2.2737367544323241e-13;
    public const double GSL_ROOT4_FLT_MIN = 3.2927225399135965e-10;
    public const double GSL_ROOT5_FLT_MIN = 2.5944428542140822e-08;
    public const double GSL_ROOT6_FLT_MIN = 4.7683715820312542e-07;
    public const double GSL_LOG_FLT_MIN = (-8.7336544750553102e+01);

    public const double GSL_FLT_MAX = 3.4028234663852886e+38;
    public const double GSL_SQRT_FLT_MAX = 1.8446743523953730e+19;
    public const double GSL_ROOT3_FLT_MAX = 6.9814635196223242e+12;
    public const double GSL_ROOT4_FLT_MAX = 4.2949672319999986e+09;
    public const double GSL_ROOT5_FLT_MAX = 5.0859007855960041e+07;
    public const double GSL_ROOT6_FLT_MAX = 2.6422459233807749e+06;
    public const double GSL_LOG_FLT_MAX = 8.8722839052068352e+01;

    public const double GSL_SFLT_EPSILON = 4.8828125000000000e-04;
    public const double GSL_SQRT_SFLT_EPSILON = 2.2097086912079612e-02;
    public const double GSL_ROOT3_SFLT_EPSILON = 7.8745065618429588e-02;
    public const double GSL_ROOT4_SFLT_EPSILON = 1.4865088937534013e-01;
    public const double GSL_ROOT5_SFLT_EPSILON = 2.1763764082403100e-01;
    public const double GSL_ROOT6_SFLT_EPSILON = 2.8061551207734325e-01;
    public const double GSL_LOG_SFLT_EPSILON = (-7.6246189861593985e+00);
  }


  public class gsl_integration_workspace
  {
    public int limit;
    int size;
    public int nrmax;
    public int i;
    public int maximum_level;
    public double[] alist;
    public double[] blist;
    double[] rlist;
    public double[] elist;
    int[] order;
    public int[] level;


    public gsl_integration_workspace(int n)
    {

      if (n == 0)
        throw new ArgumentOutOfRangeException("workspace length n must be positive integer");


      alist = new double[n];
      blist = new double[n];
      rlist = new double[n];
      elist = new double[n];
      order = new int[n];
      level = new int[n];
      size = 0;
      limit = n;
      maximum_level = 0;
    }

    public void initialise(double a, double b)
    {
      this.size = 0;
      this.nrmax = 0;
      this.i = 0;
      this.alist[0] = a;
      this.blist[0] = b;
      this.rlist[0] = 0.0;
      this.elist[0] = 0.0;
      this.order[0] = 0;
      this.level[0] = 0;

      this.maximum_level = 0;
    }

    public void reset_nrmax()
    {
      nrmax = 0;
      i = order[0];
    }

    public void set_initial_result(
                         double result, double error)
    {
      size = 1;
      rlist[0] = result;
      elist[0] = error;
    }

    public void
retrieve(out double a, out double b, out double r, out double e)
    {
      a = alist[i];
      b = blist[i];
      r = rlist[i];
      e = elist[i];
    }

    public bool large_interval()
    {
      if (level[i] < maximum_level)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /* integration/append.c
 * 
 * Copyright (C) 1996, 1997, 1998, 1999, 2000, 2001 Brian Gough
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

    public void append_interval(double a1, double b1, double area1, double error1)
    {
      int i_new = this.size;

      this.alist[i_new] = a1;
      this.blist[i_new] = b1;
      this.rlist[i_new] = area1;
      this.elist[i_new] = error1;
      this.order[i_new] = i_new;
      this.level[i_new] = 0;

      this.size++;
    }


    public bool increase_nrmax()
    {
      int k;
      int id = this.nrmax;
      int jupbnd;


      int last = this.size - 1;

      if (last > (1 + limit / 2))
      {
        jupbnd = limit + 1 - last;
      }
      else
      {
        jupbnd = last;
      }

      for (k = id; k <= jupbnd; k++)
      {
        int i_max = order[this.nrmax];

        this.i = i_max;

        if (level[i_max] < this.maximum_level)
        {
          return true;
        }

        this.nrmax++;

      }
      return false;
    }

    public double sum_results()
    {
      int n = this.size;

      int k;
      double result_sum = 0;

      for (k = 0; k < n; k++)
      {
        result_sum += rlist[k];
      }

      return result_sum;
    }


    public void update(double a1, double b1, double area1, double error1,
             double a2, double b2, double area2, double error2)
    {
      int i_max = this.i;
      int i_new = this.size;

      int new_level = this.level[i_max] + 1;

      /* append the newly-created intervals to the list */

      if (error2 > error1)
      {
        alist[i_max] = a2;        /* blist[maxerr] is already == b2 */
        rlist[i_max] = area2;
        elist[i_max] = error2;
        level[i_max] = new_level;

        alist[i_new] = a1;
        blist[i_new] = b1;
        rlist[i_new] = area1;
        elist[i_new] = error1;
        level[i_new] = new_level;
      }
      else
      {
        blist[i_max] = b1;        /* alist[maxerr] is already == a1 */
        rlist[i_max] = area1;
        elist[i_max] = error1;
        level[i_max] = new_level;

        alist[i_new] = a2;
        blist[i_new] = b2;
        rlist[i_new] = area2;
        elist[i_new] = error2;
        level[i_new] = new_level;
      }

      this.size++;

      if (new_level > this.maximum_level)
      {
        this.maximum_level = new_level;
      }

      qpsrt();
    }

    void qpsrt()
    {
      int last = this.size - 1;
      int limit = this.limit;


      double errmax;
      double errmin;
      int i, k, top;

      int i_nrmax = this.nrmax;
      int i_maxerr = order[i_nrmax];

      /* Check whether the list contains more than two error estimates */

      if (last < 2)
      {
        order[0] = 0;
        order[1] = 1;
        this.i = i_maxerr;
        return;
      }

      errmax = elist[i_maxerr];

      /* This part of the routine is only executed if, due to a difficult
         integrand, subdivision increased the error estimate. In the normal
         case the insert procedure should start after the nrmax-th largest
         error estimate. */

      while (i_nrmax > 0 && errmax > elist[order[i_nrmax - 1]])
      {
        order[i_nrmax] = order[i_nrmax - 1];
        i_nrmax--;
      }

      /* Compute the number of elements in the list to be maintained in
         descending order. This number depends on the number of
         subdivisions still allowed. */

      if (last < (limit / 2 + 2))
      {
        top = last;
      }
      else
      {
        top = limit - last + 1;
      }

      /* Insert errmax by traversing the list top-down, starting
         comparison from the element elist(order(i_nrmax+1)). */

      i = i_nrmax + 1;

      /* The order of the tests in the following line is important to
         prevent a segmentation fault */

      while (i < top && errmax < elist[order[i]])
      {
        order[i - 1] = order[i];
        i++;
      }

      order[i - 1] = i_maxerr;

      /* Insert errmin by traversing the list bottom-up */

      errmin = elist[last];

      k = top - 1;

      while (k > i - 2 && errmin >= elist[order[k]])
      {
        order[k + 1] = order[k];
        k--;
      }

      order[k + 1] = last;

      /* Set i_max and e_max */

      i_maxerr = order[i_nrmax];

      this.i = i_maxerr;
      this.nrmax = i_nrmax;
    }


    /* integration/ptsort.c
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



    public void sort_results()
    {
      int i;

      double[] elist = this.elist;
      int[] order = this.order;

      int nint = this.size;

      for (i = 0; i < nint; i++)
      {
        int i1 = order[i];
        double e1 = elist[i1];
        int i_max = i1;
        int j;

        for (j = i + 1; j < nint; j++)
        {
          int i2 = order[j];
          double e2 = elist[i2];

          if (e2 >= e1)
          {
            i_max = i2;
            e1 = e2;
          }
        }

        if (i_max != i1)
        {
          order[i] = order[i_max];
          order[i_max] = i1;
        }
      }

      this.i = order[0];
    }


  }


  public class extrapolation_table
  {
    public int n;
    public double[] rlist2 = new double[52];
    public int nres;
    public double[] res3la = new double[3];

    public void initialise_table()
    {
      n = 0;
      nres = 0;
    }

    public void
append_table(double y)
    {
      int n;
      n = this.n;
      this.rlist2[n] = y;
      this.n++;
    }

    public void
qelg(out double result, out double abserr)
    {
      double[] epstab = rlist2;
      int n = this.n - 1;

      double current = epstab[n];

      double absolute = GSL_CONST.GSL_DBL_MAX;
      double relative = 5 * GSL_CONST.GSL_DBL_EPSILON * Math.Abs(current);

      int newelm = n / 2;
      int n_orig = n;
      int n_final = n;
      int i;

      int nres_orig = this.nres;

      result = current;
      abserr = GSL_CONST.GSL_DBL_MAX;

      if (n < 2)
      {
        result = current;
        abserr = Math.Max(absolute, relative);
        return;
      }

      epstab[n + 2] = epstab[n];
      epstab[n] = GSL_CONST.GSL_DBL_MAX;

      for (i = 0; i < newelm; i++)
      {
        double res = epstab[n - 2 * i + 2];
        double e0 = epstab[n - 2 * i - 2];
        double e1 = epstab[n - 2 * i - 1];
        double e2 = res;

        double e1abs = Math.Abs(e1);
        double delta2 = e2 - e1;
        double err2 = Math.Abs(delta2);
        double tol2 = Math.Max(Math.Abs(e2), e1abs) * GSL_CONST.GSL_DBL_EPSILON;
        double delta3 = e1 - e0;
        double err3 = Math.Abs(delta3);
        double tol3 = Math.Max(e1abs, Math.Abs(e0)) * GSL_CONST.GSL_DBL_EPSILON;

        double e3, delta1, err1, tol1, ss;

        if (err2 <= tol2 && err3 <= tol3)
        {
          /* If e0, e1 and e2 are equal to within machine accuracy,
             convergence is assumed.  */

          result = res;
          absolute = err2 + err3;
          relative = 5 * GSL_CONST.GSL_DBL_EPSILON * Math.Abs(res);
          abserr = Math.Max(absolute, relative);
          return;
        }

        e3 = epstab[n - 2 * i];
        epstab[n - 2 * i] = e1;
        delta1 = e1 - e3;
        err1 = Math.Abs(delta1);
        tol1 = Math.Max(e1abs, Math.Abs(e3)) * GSL_CONST.GSL_DBL_EPSILON;

        /* If two elements are very close to each other, omit a part of
           the table by adjusting the value of n */

        if (err1 <= tol1 || err2 <= tol2 || err3 <= tol3)
        {
          n_final = 2 * i;
          break;
        }

        ss = (1 / delta1 + 1 / delta2) - 1 / delta3;

        /* Test to detect irregular behaviour in the table, and
           eventually omit a part of the table by adjusting the value of
           n. */

        if (Math.Abs(ss * e1) <= 0.0001)
        {
          n_final = 2 * i;
          break;
        }

        /* Compute a new element and eventually adjust the value of
           result. */

        res = e1 + 1 / ss;
        epstab[n - 2 * i] = res;

        {
          double error = err2 + Math.Abs(res - e2) + err3;

          if (error <= abserr)
          {
            abserr = error;
            result = res;
          }
        }
      }

      /* Shift the table */

      {
        int limexp = 50 - 1;

        if (n_final == limexp)
        {
          n_final = 2 * (limexp / 2);
        }
      }

      if (n_orig % 2 == 1)
      {
        for (i = 0; i <= newelm; i++)
        {
          epstab[1 + i * 2] = epstab[i * 2 + 3];
        }
      }
      else
      {
        for (i = 0; i <= newelm; i++)
        {
          epstab[i * 2] = epstab[i * 2 + 2];
        }
      }

      if (n_orig != n_final)
      {
        for (i = 0; i <= n_final; i++)
        {
          epstab[i] = epstab[n_orig - n_final + i];
        }
      }

      this.n = n_final + 1;

      if (nres_orig < 3)
      {
        res3la[nres_orig] = result;
        abserr = GSL_CONST.GSL_DBL_MAX;
      }
      else
      {                           /* Compute error estimate */
        abserr = (Math.Abs(result - res3la[2]) + Math.Abs(result - res3la[1])
                   + Math.Abs(result - res3la[0]));

        res3la[0] = res3la[1];
        res3la[1] = res3la[2];
        res3la[2] = result;
      }

      /* In QUADPACK the variable table->nres is incremented at the top of
         qelg, so it increases on every call. This leads to the array
         res3la being accessed when its elements are still undefined, so I
         have moved the update to this point so that its value more
         useful. */

      this.nres = nres_orig + 1;

      abserr = Math.Max(abserr, 5 * GSL_CONST.GSL_DBL_EPSILON * Math.Abs(result));

      return;
    }



  };


  public enum GSL_ERR
  {
    GSL_SUCCESS = 0,
    GSL_FAILURE = -1,
    GSL_CONTINUE = -2,  /* iteration has not converged */
    GSL_EDOM = 1,   /* input domain error, e.g sqrt(-1) */
    GSL_ERANGE = 2,   /* output range error, e.g. exp(1e100) */
    GSL_EFAULT = 3,   /* invalid pointer */
    GSL_EINVAL = 4,   /* invalid argument supplied by user */
    GSL_EFAILED = 5,   /* generic failure */
    GSL_EFACTOR = 6,   /* factorization failed */
    GSL_ESANITY = 7,   /* sanity check failed - shouldn't happen */
    GSL_ENOMEM = 8,   /* malloc failed */
    GSL_EBADFUNC = 9,   /* problem with user-supplied function */
    GSL_ERUNAWAY = 10,  /* iterative process is out of control */
    GSL_EMAXITER = 11,  /* exceeded max number of iterations */
    GSL_EZERODIV = 12,  /* tried to divide by zero */
    GSL_EBADTOL = 13,  /* user specified an invalid tolerance */
    GSL_ETOL = 14,  /* failed to reach the specified tolerance */
    GSL_EUNDRFLW = 15,  /* underflow */
    GSL_EOVRFLW = 16,  /* overflow  */
    GSL_ELOSS = 17,  /* loss of accuracy */
    GSL_EROUND = 18,  /* failed because of roundoff error */
    GSL_EBADLEN = 19,  /* matrix, vector lengths are not conformant */
    GSL_ENOTSQR = 20,  /* matrix not square */
    GSL_ESING = 21,  /* apparent singularity detected */
    GSL_EDIVERGE = 22,  /* integral or series is divergent */
    GSL_EUNSUP = 23,  /* requested feature is not supported by the hardware */
    GSL_EUNIMPL = 24,  /* requested feature not (yet) implemented */
    GSL_ECACHE = 25,  /* cache limit exceeded */
    GSL_ETABLE = 26,  /* table limit exceeded */
    GSL_ENOPROG = 27,  /* iteration is not making progress towards solution */
    GSL_ENOPROGJ = 28,  /* jacobian evaluations are not improving the solution */
    GSL_ETOLF = 29,  /* cannot reach the specified tolerance in F */
    GSL_ETOLX = 30,  /* cannot reach the specified tolerance in X */
    GSL_ETOLG = 31,  /* cannot reach the specified tolerance in gradient */
    GSL_EOF = 32   /* end of file */
  } ;


  public class GSL_ERROR
  {
    public string Message;
    public GSL_ERR Number;




    public GSL_ERROR(string message, GSL_ERR number, bool bDebug)
    {
      this.Message = message;
      this.Number = number;

      if (bDebug)
        throw new ArithmeticException(message);
    }
  }

  public class GSL_UTILS
  {
  }

  public delegate void gsl_integration_rule(
    ScalarFunctionDD f, double a, double b,
    out double result, out double abserr,
                                  out double defabs, out double resabs);
  public class IntegrationBase
  {
    protected static bool test_positivity(double result, double resabs)
    {
      bool status = (Math.Abs(result) >= (1 - 50 * GSL_CONST.GSL_DBL_EPSILON) * resabs);
      return status;
    }
    protected static bool subinterval_too_small(double a1, double a2, double b2)
    {
      const double e = GSL_CONST.GSL_DBL_EPSILON;
      const double u = GSL_CONST.GSL_DBL_MIN;

      double tmp = (1 + 100 * e) * (Math.Abs(a2) + 1000 * u);

      bool status = Math.Abs(a1) <= tmp && Math.Abs(b2) <= tmp;

      return status;
    }

   

   /* Define a rounding function which moves extended precision values
   out of registers and rounds them to double-precision. This should
   be used *sparingly*, in places where it is necessary to keep
   double-precision rounding for critical expressions while running in
   extended precision. For example, the following code should ensure
   exact equality, even when extended precision registers are in use,

      double q = GSL_COERCE_DBL(3.0/7.0) ;
      if (q == GSL_COERCE_DBL(3.0/7.0)) { ... } ;

   It carries a penalty even when the program is running in double
   precision mode unless you compile a separate version of the
   library with HAVE_EXTENDED_PRECISION_REGISTERS turned off. */


protected static double GSL_COERCE_DBL(double x) 
{
  return (x); 
}


   


    /* Main integration function */

    protected static GSL_ERROR
    qags(ScalarFunctionDD f,
          double a, double b,
          double epsabs, double epsrel,
          int limit,
          gsl_integration_workspace workspace,
          out double result, out double abserr,
          gsl_integration_rule q)
    {
      bool bDebug = true;

      double area, errsum;
      double res_ext, err_ext;
      double result0, abserr0, resabs0, resasc0;
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

      extrapolation_table table = new extrapolation_table();

      /* Initialize results */

      workspace.initialise(a, b);

      result = double.NaN;
      abserr = double.NaN;

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

      q(f, a, b, out result0, out abserr0, out resabs0, out resasc0);

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
      table.append_table(result0);

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
        double a_i, b_i, r_i, e_i;
        double area1 = 0, area2 = 0, area12 = 0;
        double error1 = 0, error2 = 0, error12 = 0;
        double resasc1, resasc2;
        double resabs1, resabs2;
        double last_e_i;

        /* Bisect the subinterval with the largest error estimate */

        workspace.retrieve(out a_i, out b_i, out r_i, out e_i);

        current_level = workspace.level[workspace.i] + 1;

        a1 = a_i;
        b1 = 0.5 * (a_i + b_i);
        a2 = b1;
        b2 = b_i;

        iteration++;

        q(f, a1, b1, out area1, out error1, out resabs1, out resasc1);
        q(f, a2, b2, out area2, out error2, out resabs2, out resasc2);

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

        if (iteration == 2)       /* set up variables on first iteration */
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

        if (!error_type2 && error_over_large_intervals > ertest)
        {
          if (workspace.increase_nrmax())
            continue;
        }

        /* Perform extrapolation */

        table.append_table(area);

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
      }

      /*  Test on divergence. */

      {
        double max_area = Math.Max(Math.Abs(res_ext), Math.Abs(area));

        if (!positive_integrand && max_area < 0.01 * resabs0)
          goto return_error;
      }

      {
        double ratio = res_ext / area;

        if (ratio < 0.01 || ratio > 100.0 || errsum > Math.Abs(area))
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
