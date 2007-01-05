#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;

namespace Altaxo.Calc
{
  /// <summary>
  /// Provides methods for real numbers, that were forgotten by the <see cref="System.Math" /> class.
  /// </summary>
  public class RMath
  {

    #region Helper constants
    const double GSL_DBL_EPSILON = 2.2204460492503131e-16;
    const double GSL_SQRT_DBL_EPSILON =  1.4901161193847656e-08 ;

    const double M_LN2 = 0.69314718055994530941723212146 ;     // ln(2) 

    #endregion


    public static double Log1p(double x)
    {
      double y;
      y = 1 + x;
      return Math.Log(y) - ((y-1)-x)/y ;  /* cancels errors with IEEE arithmetic */
    }

    
    public static double Acosh(double x)
    {
      if (x > 1.0 / GSL_SQRT_DBL_EPSILON)
      {
        return Math.Log (x) + M_LN2;
      }
      else if (x > 2)
      {
        return Math.Log(2 * x - 1 / (Math.Sqrt (x * x - 1) + x));
      }
      else if (x > 1)
      {
        double t = x - 1;
        return Log1p (t + Math.Sqrt (2 * t + t * t));
      }
      else if (x == 1)
      {
        return 0;
      }
      else
      {
        return double.NaN;
      }
    }

    public static double Asinh(double x)
    {
      double a = Math.Abs (x);
      double s = (x < 0) ? -1 : 1;

      if (a > 1 / GSL_SQRT_DBL_EPSILON)
      {
        return s * (Math.Log (a) + M_LN2);
      }
      else if (a > 2)
      {
        return s * Math.Log (2 * a + 1 / (a + Math.Sqrt (a * a + 1)));
      }
      else if (a > GSL_SQRT_DBL_EPSILON)
      {
        double a2 = a * a;
        return s * Log1p (a + a2 / (1 + Math.Sqrt (1 + a2)));
      }
      else
      {
        return x;
      }
    }

    public static double Atanh(double x)
    {
      double a = Math.Abs (x);
      double s = (x < 0) ? -1 : 1;

      if (a > 1)
      {
        return double.NaN;
      }
      else if (a == 1)
      {
        return (x < 0) ? double.NegativeInfinity : double.PositiveInfinity;
      }
      else if (a >= 0.5)
      {
        return s * 0.5 * Log1p (2 * a / (1 - a));
      }
      else if (a > GSL_DBL_EPSILON)
      {
        return s * 0.5 * Log1p (2 * a + 2 * a * a / (1 - a));
      }
      else
      {
        return x;
      }
    }


    /// <summary>
    /// The standard hypot() function for two arguments taking care of overflows and zerodivides. 
    /// </summary>
    /// <param name="x">First argument.</param>
    /// <param name="y">Second argument.</param>
    /// <returns>Square root of the sum of x-square and y-square.</returns>
    public static double Hypot(double x, double y)
    {
      double xabs = Math.Abs(x) ;
      double yabs = Math.Abs(y) ;
      double min, max;

      if (xabs < yabs) 
      {
        min = xabs ;
        max = yabs ;
      } 
      else 
      {
        min = yabs ;
        max = xabs ;
      }

      if (min == 0) 
      {
        return max ;
      }

    {
      double u = min / max ;
      return max * Math.Sqrt (1 + u * u) ;
    }
    }


    /// <summary>
    /// Calculates x^2 (square of x).
    /// </summary>
    /// <param name="x">Argument.</param>
    /// <returns><c>x</c> squared.</returns>
    public static double Pow2(double x) { return x*x;   }
    public static double Pow3(double x) { return x*x*x; }
    public static double Pow4(double x) { double x2 = x*x;   return x2*x2;    }
    public static double Pow5(double x) { double x2 = x*x;   return x2*x2*x;  }
    public static double Pow6(double x) { double x2 = x*x;   return x2*x2*x2; }
    public static double Pow7(double x) { double x3 = x*x*x; return x3*x3*x;  }
    public static double Pow8(double x) { double x2 = x*x;   double x4 = x2*x2; return x4*x4; }
    public static double Pow9(double x) { double x3 = x*x*x; return x3*x3*x3; }


    /// <summary>
    /// Calculates x^n by repeated multiplications. The algorithm takes ld(n) multiplications.
    /// This algorithm can also be used with negative n.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static double Pow(double x, int n)
    {
      double value = 1.0;

      bool inverse = (n<0);
      if(n < 0) 
      {
        n = -n;
      }

      /* repeated squaring method 
       * returns 0.0^0 = 1.0, so continuous in x
       */
      do 
      {
        if(0 != (n & 1)) 
          value *= x;  /* for n odd */
        
        n >>= 1;
        x *= x;
      } while (n!=0);

      return inverse ? 1.0/value : value;
    }


  }
}
