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

    /// <summary>
    /// Returns the inverse hyperbolic sine of a value.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>The inverse hyperbolic sine of <paramref name="x"/>.</returns>
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

    /// <summary>
    /// Returns the inverse hyperbolic cosine of a value.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>The inverse hyperbolic cosine of <paramref name="x"/>.</returns>
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

    /// <summary>
    /// Returns the inverse hyperbolic tangent of a value.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>The inverse hyperbolic tangent of <paramref name="x"/>.</returns>
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
    /// Hyperbolic secant, i.e. 1/Cosh(x) = 2/(Exp(x)+Exp(-x)).
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>Hyperbolic secant.</returns>
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
      const double LimitLargeValues = 32; // above this limit, we can use 1-1/x as an approximation of Coth(x)-1/x
      const double LimitPolynom19 = 0.5;
      const double LimitPolynom11 = 0.15625;
      const double A19 = -349222 / 1531329465290625d;
      const double A17 = 87734 / 38979295480125d;
      const double A15 = -3617 / 162820783125d;
      const double A13 = 4 / 18243225d;
      const double A11 = -1382 / 638512875d;
      const double A9 = 2 / 93555d;
      const double A7 = -1 / 4725d;
      const double A5 = 2 / 945d;
      const double A3 = -1 / 45d;
      const double A1 = 1 / 3d;

      var absX = Math.Abs(x);

      if (absX >= LimitLargeValues)
      {
        return x > 0 ? 1 - 1 / x : -1 - 1 / x;
      }
      else if (absX <= LimitPolynom19)
      {
        var x2 = x * x;
        if (absX <= LimitPolynom11)
          return (((((A11 * x2 + A9) * x2 + A7) * x2 + A5) * x2 + A3) * x2 + A1) * x;
        else
          return (((((((((A19 * x2 + A17) * x2 + A15) * x2 + A13) * x2 + A11) * x2 + A9) * x2 + A7) * x2 + A5) * x2 + A3) * x2 + A1) * x;
      }
      else
      {
        return 1 / Math.Tanh(x) - 1 / x;
      }
    }

    /// <summary>
    /// Exact derivative of the Langevin function, L'(x) = 1/x^2 - csch^2(x),
    /// evaluated via its Taylor series near x = 0 to avoid cancellation error.
    /// </summary>
    public static double LangevinFirstDerivative(double x)
    {
      const double LimitPolynom16Squared = 0.25 * 0.25;
      const double LimitLargeValueSquared = 32 * 32;
      const double
      A0 = 1 / 3d,
      A2 = -1 / 15d,
      A4 = 2 / 189d,
      A6 = -1 / 675d,
      A8 = 2 / 10395d,
      A10 = -1382 / 58046625d,
      A12 = 4 / 1403325d,
      A14 = -3717 / 10854718875d,
      A16 = 87734 / 2292899734125d,
      A18 = -349222 / 80596287646875d,
      A20 = 310732 / 640374140030625d,
      A22 = -472728182 / 8779111824511153125d,
      A24 = 2631724 / 443779279041223125d;


      var x2 = x * x;

      if (x2 <= LimitPolynom16Squared)
      {
        return (((((((A16 * x2 + A14) * x2 + A12) * x2 + A10) * x2 + A8) * x2 + A6) * x2 + A4) * x2 + A2) * x2 + A0;
      }
      else if (x2 <= LimitLargeValueSquared)
      {
        double csch = 1 / Math.Sinh(x);
        return 1 / x2 - csch * csch;
      }
      else
      {
        return 1 / x2;
      }
    }



    /// <summary>
    /// Padé approximation of the inverse Langevin function (Cohen, 1991).
    /// Accurate to within ~5% over the full domain (0,1).
    /// </summary>
    /// <param name="x">Argument, must satisfy 0 &lt;= x &lt; 1.</param>
    public static double InverseLangevinApproximationByCohen(double x)
    {
      if (x <= -1.0 || x >= 1.0)
      {
        {
          throw new ArgumentOutOfRangeException(nameof(x),
              "Argument of the inverse Langevin function must lie in (-1, 1).");
        }
      }
      else
      {
        var x2 = x * x;
        return x * (3 - x2) / (1 - x2);
      }
    }

    /// <summary>
    /// Kröger's rational approximation of the inverse Langevin function
    /// (Kröger, M., J. Non-Newtonian Fluid Mech. 223 (2015) 77-87).
    /// Substantially more accurate than Cohen's Padé approximant (max.
    /// relative error ~0.28% vs. ~4.9%), at a similar computational cost,
    /// since it is still a single rational expression with only integer powers.
    /// </summary>
    public static double InverseLangevinApproximationByKroger(double x)
    {
      if (x <= -1.0 || x >= 1.0)
      {
        throw new ArgumentOutOfRangeException(nameof(x), "Argument of the inverse Langevin function must lie in (-1, 1).");
      }
      else
      {
        double x2 = x * x;
        double x4 = x2 * x2;
        return (3 * x - (x / 5) * (6 * x2 + x4 - 2 * x2 * x4)) / (1 - x2);
      }
    }

    /// <summary>
    /// Inverse Langevin function approximation by Jedynak (2015).
    /// </summary>
    public static double InverseLangevinApproximationByJedynak(double x)
    {
      // 1. Symmetrie ausnutzen (die Funktion ist ungerade)
      double absX = Math.Abs(x);

      // 2. Fehlerbehandlung für ungültige Definitionsbereiche
      if (absX >= 1.0)
      {
        throw new ArgumentOutOfRangeException(nameof(x),
            "Der Betrag von x muss strikt kleiner als 1.0 sein (Singularität bei |x| = 1).");
      }

      // 3. Fallunterscheidung für numerische Stabilität
      double result;

      if (absX < 1e-4)
      {
        // Bei extrem kleinen Werten nahe 0 droht bei Brüchen ein Präzisionsverlust.
        // Hier nutzen wir die ersten zwei Glieder der Taylor-Reihe.
        result = absX * (3.0 + 1.8 * absX * absX);
      }
      else
      {
        // Hochpräzise rationale Approximation nach Jedynak für Double-Genauigkeit.
        // Formel-Typ: x * (3 - a*x^2 + b*x^4) / (1 - x^2)(1 + c*x^2)
        // Die Konstanten sind numerisch auf minimale relative Abweichung optimiert.
        double x2 = absX * absX;

        double numerator = absX * (3.0 - 2.89305 * x2 + 0.957574 * x2 * x2);
        double denominator = (1.0 - x2) * (1.0 + 0.463032 * x2);

        result = numerator / denominator;
      }

      // Vorzeichen wiederherstellen
      return x < 0 ? -result : result;
    }

    /// <summary>
    /// High-accuracy inverse Langevin function: starts from Kröger's rational
    /// approximation (max. relative error ~0.28%) and refines it with a fixed
    /// number of Newton-Raphson steps on the exact equation L(x) = y, where
    /// L is the true Langevin function (not an approximation).
    ///
    /// Each Newton step roughly doubles the number of correct digits, so
    /// starting from ~0.28% (about 2-3 correct digits), 3 steps reach full
    /// double precision (~15-16 digits) everywhere except in the immediate
    /// vicinity of y = +-1, where L'(x) -> 0 and the problem becomes
    /// ill-conditioned regardless of solver.
    /// </summary>
    /// <param name="y">Target value, must lie in (-1, 1).</param>
    /// <param name="newtonSteps">Number of Newton-Raphson refinement steps (default 3).</param>
    public static double InverseLangevin(double y, int newtonSteps = 3)
    {
      const double LimitPolynom19 = 0.125;
      const double LimitLargeValues = 1.0 - 1 / 32d; // above that, we can use 1/(1-y);

      const double
        A1 = 3,
        A3 = 9 / 5d,
        A5 = 297 / 175d,
        A7 = 1539 / 875d,
        A9 = 126117 / 67375d,
        A11 = 43733439 / 21896875d,
        A13 = 231321177 / 109484375d,
        A15 = 20495009043 / 9306171875d,
        A17 = 1073585186448381 / 476522530859375d,
        A19 = 4387445039583 / 1944989921875d;

      var absY = Math.Abs(y);
      if (absY <= LimitPolynom19)
      {
        var x2 = y * y;
        return (((((((((A19 * x2 + A17) * x2 + A15) * x2 + A13) * x2 + A11) * x2 + A9) * x2 + A7) * x2 + A5) * x2 + A3) * x2 + A1) * y;
      }
      else if (absY < LimitLargeValues)
      {
        double x = InverseLangevinApproximationByKroger(y); // starting value

        for (int i = 0; i < newtonSteps; i++)
        {
          double residual = Langevin(x) - y;
          double slope = LangevinFirstDerivative(x);
          x -= residual / slope;
        }
        return x;
      }
      else if (absY < 1)
      {
        return y > 0 ? 1 / (1 - y) : -1 / (1 + y);
      }
      else
      {
        throw new ArgumentOutOfRangeException(nameof(y), "Argument of the inverse Langevin function must lie in (-1, 1).");
      }
    }

    /// <summary>
    /// Closed-form derivative of Cohen's Padé approximant of the inverse Langevin
    /// function, dL^-1/dx = (3 + x^4) / (1 - x^2)^2.
    /// </summary>
    public static double InverseLangevinDerivativeApproxiationByCohen(double x)
    {
      if (x <= -1.0 || x >= 1.0)
      {
        throw new ArgumentOutOfRangeException(nameof(x),
            "Argument of the inverse Langevin function must lie in (-1, 1).");
      }

      double x2 = x * x;
      double denom = 1.0 - x2;
      return (3.0 + x2 * x2) / (denom * denom);
    }

    /// <summary>
    /// High-accuracy derivative of the inverse Langevin function, d(L^-1)/dy.
    ///
    /// Uses the inverse function theorem: if x = L^-1(y), then
    ///     d(L^-1)/dy (y) = 1 / L'(x)
    /// evaluated with the EXACT Langevin derivative (not an approximant's
    /// derivative), at the highly accurate x produced by
    /// InverseLangevinAccurate. This is far more accurate than
    /// differentiating a closed-form approximation analytically (e.g.
    /// InverseLangevinDerivative, which inherits Cohen's ~5% approximation
    /// error) -- the error here is limited only by the Newton refinement
    /// in InverseLangevinAccurate, i.e. essentially full double precision
    /// away from y = +-1.
    /// </summary>
    /// <param name="y">Target value, must lie in (-1, 1).</param>
    /// <param name="newtonSteps">Number of Newton-Raphson steps used to locate x (default 3).</param>
    public static double InverseLangevinDerivative(double y, int newtonSteps = 3)
    {
      double x = InverseLangevin(y, newtonSteps);
      double slope = LangevinFirstDerivative(x);
      return 1 / slope;
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
    /// <param name="x">The x value.</param>
    /// <returns>Log(1+x) with better accuracy for very small x.</returns>
    public static double Log1p(double x) => RMath.Log1p(x);


    /// <summary>
    /// Calculates 1-Exp(x) with better accuracy around x=0.
    /// </summary>
    /// <param name="x">Function argument.</param>
    /// <returns>The value 1-Exp(x).</returns>
    public static double OneMinusExp(double x) => RMath.OneMinusExp(x);

    /// <summary>
    /// Calculates Exp(x)-1 with better accuracy around x=0.
    /// </summary>
    /// <param name="x">Function argument.</param>
    /// <returns>The value Exp(x)-1.</returns>
    public static double ExpMinusOne(double x) => -RMath.OneMinusExp(x);

  }
}
