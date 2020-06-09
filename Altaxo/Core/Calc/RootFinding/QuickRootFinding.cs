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

namespace Altaxo.Calc.RootFinding
{
  /// <summary>
  /// Provides static methods for quick and dirty root finding without instantiating a class.
  /// </summary>
  public class QuickRootFinding
  {
    /// <summary>
    /// Find the bracket of a root, i.e. values for x0 and x1, so that ysearch is inbetween f(x0) and f(x1). This is done be extension of the interval [x0,x1] either
    /// to the left or the right side or both.
    /// </summary>
    /// <param name="func">The function used to evaluate the function values.</param>
    /// <param name="ysearch">The value to find.</param>
    /// <param name="x0">Starting parameter of x0, at the end the lower value of the bracket interval.</param>
    /// <param name="x1">Starting parameter of x1, at the end the upper value of the bracket interval.</param>
    /// <returns>True if a bracket interval was found. If such an interval could not be found, the return value is false.</returns>
    public static bool BracketRootByExtensionOnly(Func<double, double> func, double ysearch, ref double x0, ref double x1)
    {
      if (!(x0 != x1))
        return false;

      if (x0 > x1) // make sure that x0<x1
      {
        double xh = x0;
        x0 = x1;
        x1 = xh;
      }

      double y0 = func(x0);
      if (double.IsNaN(y0))
        return false;

      double y1 = func(x1);
      if (double.IsNaN(y1))
        return false;

      if (y0 == ysearch || y1 == ysearch)
        return true;

      for (; ; )
      {
        if (y0 < y1) // increasing
        {
          if (y0 <= ysearch && ysearch <= y1)
          {
            return true;
          }
          else if (y0 > ysearch)
          {
            // extend the interval in the direction of x0
            double oldx0 = x0;
            x0 -= x1 - x0;
            y0 = func(x0);
            if (!(x0 != oldx0) || double.IsNaN(y0))
              return false;
          }
          else if (y1 < ysearch)
          {
            // extend the interval in the direction of x1
            double oldx1 = x1;
            x1 += x1 - x0;
            y1 = func(x1);
            if (!(x1 != oldx1) || double.IsNaN(y1))
              return false;
          }
          else
          {
            return false; // something else happend, for instance some of the value is infinite
          }
        }
        else if (y0 > y1)
        {
          if (y1 <= ysearch && ysearch <= y0)
          {
            return true;
          }
          else if (y0 < ysearch)
          {
            // extend the interval in the direction of x0
            double oldx0 = x0;
            x0 -= x1 - x0;
            y0 = func(x0);
            if (!(x0 != oldx0) || double.IsNaN(y0))
              return false;
          }
          else if (y1 > ysearch)
          {
            // extend the interval in the direction of x1
            double oldx1 = x1;
            x1 += x1 - x0;
            y1 = func(x1);
            if (!(x1 != oldx1) || double.IsNaN(y1))
              return false;
          }
          else
          {
            return false; // something else happend, for instance some of the value is infinite
          }
        }
        else // both values are equal
        {
          // extend the interval in both directions
          double oldx0 = x0;
          double oldx1 = x1;

          x0 -= oldx1 - oldx0;
          x1 += oldx1 - oldx0;
          y0 = func(x0);
          y1 = func(x1);
          if (!(x0 != oldx0) || !(x1 != oldx1))
            return false;
          if (double.IsNaN(y0) || double.IsNaN(y1))
            return false;
        }
      }
    }

    #region Brents algorithm

    // Gnu Scientific Library 1.10
    /* roots/brent.c
        *
        * Copyright (C) 1996, 1997, 1998, 1999, 2000, 2007 Reid Priedhorsky, Brian Gough
        *
        * This program is free software; you can redistribute it and/or modify
        * it under the terms of the GNU General Public License as published by
        * the Free Software Foundation; either version 3 of the License, or (at
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

    /* brent.c -- brent root finding algorithm */

    private struct brent_state_t
    {
      public double a, b, c, d, e;
      public double fa, fb, fc;
    }

    private static GSL_ERROR?
    brent_init(Func<double, double> f, double x_lower, double x_upper, out double root, out brent_state_t state)
    {
      double f_lower, f_upper;

      root = 0.5 * (x_lower + x_upper);

      f_lower = f(x_lower);
      f_upper = f(x_upper);

      state.a = x_lower;
      state.fa = f_lower;

      state.b = x_upper;
      state.fb = f_upper;

      state.c = x_upper;
      state.fc = f_upper;

      state.d = x_upper - x_lower;
      state.e = x_upper - x_lower;

      if ((f_lower < 0.0 && f_upper < 0.0) || (f_lower > 0.0 && f_upper > 0.0))
      {
        return new GSL_ERROR("endpoints do not straddle y=0", GSL_ERR.GSL_EINVAL, false);
      }

      return null;
    }

    private static GSL_ERROR?
brent_iterate(ref brent_state_t state, Func<double, double> f, out double root, ref double x_lower, ref double x_upper)
    {
      double tol, m;

      bool ac_equal = false;

      double a = state.a, b = state.b, c = state.c;
      double fa = state.fa, fb = state.fb, fc = state.fc;
      double d = state.d, e = state.e;

      if ((fb < 0 && fc < 0) || (fb > 0 && fc > 0))
      {
        ac_equal = true;
        c = a;
        fc = fa;
        d = b - a;
        e = b - a;
      }

      if (Math.Abs(fc) < Math.Abs(fb))
      {
        ac_equal = true;
        a = b;
        b = c;
        c = a;
        fa = fb;
        fb = fc;
        fc = fa;
      }

      tol = 0.5 * DoubleConstants.DBL_EPSILON * Math.Abs(b);
      m = 0.5 * (c - b);

      if (fb == 0)
      {
        root = b;
        x_lower = b;
        x_upper = b;

        return null;
      }

      if (Math.Abs(m) <= tol)
      {
        root = b;

        if (b < c)
        {
          x_lower = b;
          x_upper = c;
        }
        else
        {
          x_lower = c;
          x_upper = b;
        }

        return null;
      }

      if (Math.Abs(e) < tol || Math.Abs(fa) <= Math.Abs(fb))
      {
        d = m;            /* use bisection */
        e = m;
      }
      else
      {
        double p, q, r;   /* use inverse cubic interpolation */
        double s = fb / fa;

        if (ac_equal)
        {
          p = 2 * m * s;
          q = 1 - s;
        }
        else
        {
          q = fa / fc;
          r = fb / fc;
          p = s * (2 * m * q * (q - r) - (b - a) * (r - 1));
          q = (q - 1) * (r - 1) * (s - 1);
        }

        if (p > 0)
        {
          q = -q;
        }
        else
        {
          p = -p;
        }

        if (2 * p < Math.Min(3 * m * q - Math.Abs(tol * q), Math.Abs(e * q)))
        {
          e = d;
          d = p / q;
        }
        else
        {
          /* interpolation failed, fall back to bisection */

          d = m;
          e = m;
        }
      }

      a = b;
      fa = fb;

      if (Math.Abs(d) > tol)
      {
        b += d;
      }
      else
      {
        b += (m > 0 ? +tol : -tol);
      }

      fb = f(b);

      state.a = a;
      state.b = b;
      state.c = c;
      state.d = d;
      state.e = e;
      state.fa = fa;
      state.fb = fb;
      state.fc = fc;

      /* Update the best estimate of the root and bounds on each
                 iteration */

      root = b;

      if ((fb < 0 && fc < 0) || (fb > 0 && fc > 0))
      {
        c = a;
      }

      if (b < c)
      {
        x_lower = b;
        x_upper = c;
      }
      else
      {
        x_lower = c;
        x_upper = b;
      }

      return null;
    }

    public static double ByBrentsAlgorithm(Func<double, double> f, double x0, double x1)
    {
      return ByBrentsAlgorithm(f, x0, x1, 0, DoubleConstants.DBL_EPSILON);
    }

    public static double ByBrentsAlgorithm(Func<double, double> f, double x0, double x1, double epsabs, double epsrel)
    {
      if (null == ByBrentsAlgorithm(f, x0, x1, epsabs, epsrel, out var root))
        return root;
      else
        return double.NaN;
    }

    public static GSL_ERROR? ByBrentsAlgorithm(Func<double, double> f, double x0, double x1, double epsabs, double epsrel, out double root)
    {
      GSL_ERROR? err;
      err = brent_init(f, x0, x1, out root, out var state);
      if (null != err)
        return err;
      do
      {
        err = brent_iterate(ref state, f, out root, ref x0, ref x1);
        if (null != err)
          return err;
        err = gsl_root_test_interval(x0, x1, epsabs, epsrel);
      } while (err == GSL_ERROR.CONTINUE);
      return err;
    }

    #endregion Brents algorithm

    private static GSL_ERROR? gsl_root_test_interval(double x_lower, double x_upper, double epsabs, double epsrel)
    {
      double abs_lower = Math.Abs(x_lower);
      double abs_upper = Math.Abs(x_upper);

      double min_abs, tolerance;

      if (epsrel < 0.0)
        return new GSL_ERROR("relative tolerance is negative", GSL_ERR.GSL_EBADTOL, false);

      if (epsabs < 0.0)
        return new GSL_ERROR("absolute tolerance is negative", GSL_ERR.GSL_EBADTOL, false);

      if (x_lower > x_upper)
        return new GSL_ERROR("lower bound larger than upper bound", GSL_ERR.GSL_EINVAL, false);

      if ((x_lower > 0.0 && x_upper > 0.0) || (x_lower < 0.0 && x_upper < 0.0))
      {
        min_abs = Math.Min(abs_lower, abs_upper);
      }
      else
      {
        min_abs = 0;
      }

      tolerance = epsabs + epsrel * min_abs;

      if (Math.Abs(x_upper - x_lower) <= tolerance)
        return null;

      return GSL_ERROR.CONTINUE;
    }
  }
}
