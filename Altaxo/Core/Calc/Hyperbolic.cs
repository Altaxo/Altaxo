#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc
{
  /// <summary>
  /// Hyperbolic functions.
  /// </summary>
  public static class Hyperbolic
  {

    /// <summary>
    /// Hyperbolic sine, i.e. (Exp(x)-Exp(-x))/2.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>Hyperbolic sine.</returns>
    public static double Sinh(double x)
    {
      const double Limit = 0.135732d;
      const double A9 = 1 / 362880d;
      const double A7 = 1 / 5040d;
      const double A5 = 1 / 120d;
      const double A3 = 1 / 6d;

      if (Math.Abs(x) > Limit)
      {
        return (Math.Exp(x) - Math.Exp(-x)) * 0.5;
      }
      else
      {
        var x2 = x * x;
        return ((((A9 * x2 + A7) * x2 + A5) * x2 + A3) * x2 + 1) * x;
      }
    }

    public static double Asinh(double x) => RMath.Asinh(x);

    /// <summary>
    /// Hyperbolic cosine, i.e. (Exp(x)+Exp(-x))/2.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>Hyperbolic cosine.</returns>
    public static double Cosh(double x)
    {
      return 0.5 * (Math.Exp(x) + Math.Exp(-x));
    }

    public static double Acosh(double x) => RMath.Acosh(x);


    /// <summary>
    /// Hyperbolic tangent, i.e. Sinh(x)/Cosh(x).
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>Hyperbolic tangent.</returns>
    public static double Tanh(double x)
    {
      return Sinh(x) / Cosh(x);
    }

    public static double Atanh(double x) => RMath.Atanh(x);


    /// <summary>
    /// Hyperbolic cotangent, i.e. Cosh(x)/Sinh(x).
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>Hyperbolic cotangent.</returns>
    public static double Coth(double x)
    {
      return Cosh(x) / Sinh(x);
    }

    /// <summary>
    /// Hyperbolic cosecant, i.e. 1/Cosh(x) = 2/(Exp(x)+Exp(-x)).
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>Hyperbolic cosecant.</returns>
    public static double Sech(double x)
    {
      return 2 / (Math.Exp(x) + Math.Exp(-x));
    }

    /// <summary>
    /// Hyperbolic cosecant, i.e. 1/Sinh(x) = 2/(Exp(x)-Exp(-x)).
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>Hyperbolic cosecant.</returns>
    public static double Csch(double x)
    {
      return 1 / Sinh(x);
    }



    /// <summary>
    /// Hyperbolic cosecant, multiplied with the argument <paramref name="x"/>, i.e. x*Csch(x) = x/Sinh(x).
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>Hyperbolic cosecant, multiplied with the argument <paramref name="x"/>.</returns>
    public static double CschTimesX(double x)
    {
      const double Limit = 0.148547;
      const double A10 = -73 / 3421440d;
      const double A8 = 127 / 604800d;
      const double A6 = -31 / 15120d;
      const double A4 = 7 / 360d;
      const double A2 = -1 / 6d;

      if (Math.Abs(x) > Limit)
      {
        return x / Sinh(x);
      }
      else
      {
        var x2 = x * x;
        return ((((A10 * x2 + A8) * x2 + A6) * x2 + A4) * x2 + A2) * x2 + 1;
      }
    }

    /// <summary>
    /// Langevin function, which is defined as Coth(x)-1/x.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>Langevin function Coth(x)-1/x.</returns>
    public static double Langevin(double x)
    {
      const double Limit = 0.162453766880893;
      const double A11 = -1382 / 638512875d;
      const double A9 = 2 / 93555d;
      const double A7 = -1 / 4725d;
      const double A5 = 2 / 945d;
      const double A3 = -1 / 45d;
      const double A1 = 1 / 3d;


      if (Math.Abs(x) > Limit)
      {
        return (Math.Exp(x) + Math.Exp(-x)) / (Math.Exp(x) - Math.Exp(-x)) - 1 / x;
      }
      else
      {
        var x2 = x * x;
        return (((((A11 * x2 + A9) * x2 + A7) * x2 + A5) * x2 + A3) * x2 + A1) * x;
      }
    }

    /// <summary>
    /// Calculates [Exp(a x)-Exp(b x)]/[Exp(x)-Exp(-x)].
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <param name="a">The first prefactor.</param>
    /// <param name="b">The second prefactor.</param>
    /// <returns>The function f(x,a,b) = [Exp(a x)-Exp(b x)]/[Exp(x)-Exp(-x)].</returns>
    public static double SinhAxBxTimesCschX(double x, double a, double b)
    {
      const double Limit = 0.0980690159972; // Limit for x in 9th order series 1-Exp(x) to maintain rel accuracy 2^-52
      const double A9 = 1 / 362880d;
      const double A8 = 1 / 40320d;
      const double A7 = 1 / 5040d;
      const double A6 = 1 / 720d;
      const double A5 = 1 / 120d;
      const double A4 = 1 / 24d;
      const double A3 = 1 / 6d;
      const double A2 = 1 / 2d;
      const double A1 = 1d;

      var xx = (b - a) * x;
      if (Math.Abs(xx) > Limit)
      {
        return (Math.Exp(a * x) * (1 - Math.Exp((b - a) * x))) * 0.5 / Sinh(x);
      }
      else
      {
        // Series expansion of 1-Exp(xx)
        var p = -((((((((A9 * xx + A8) * xx + A7) * xx + A6) * xx + A5) * xx + A4) * xx + A3) * xx + A2) * xx + A1) * xx;
        return Math.Exp(a * x) * p * 0.5 / Sinh(x);
      }
    }

    /// <summary>
    /// Calculates the natural logarithm of 1+x with better accuracy for very small x.
    /// </summary>
    /// <param name="x">The x value</param>
    /// <returns>Log(1+x) with better accuracy for very small x.</returns>
    public static double Log1p(double x) => RMath.Log1p(x);


    /// <summary>
    /// Calculates 1-Exp(x) with better accuracy around x=0.
    /// </summary>
    /// <param name="x">Function argument</param>
    /// <returns>The value 1-Exp(x)</returns>
    public static double OneMinusExp(double x) => RMath.OneMinusExp(x);

    /// <summary>
    /// Calculates Exp(x)-1 with better accuracy around x=0.
    /// </summary>
    /// <param name="x">Function argument</param>
    /// <returns>The value Exp(x)-1</returns>
    public static double ExpMinusOne(double x) => -RMath.OneMinusExp(x);

  }
}
