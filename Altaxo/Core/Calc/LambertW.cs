#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
  /// Provides static methods for evaluating branches of the Lambert W function for real arguments.
  /// </summary>
  /// <remarks>The Lambert W function, also known as the product logarithm, is the set of functions W(z)
  /// satisfying W(z) ⋅ e^{W(z)} = z. This class offers methods to compute the principal branch (W0) and the W_{-1}
  /// branch for real values of z. All methods are static and thread-safe.</remarks>
  public static class LambertW
  {
    /// <summary>
    /// Computes the principal branch (W0) of the Lambert W function for a given value.
    /// </summary>
    /// <remarks>The principal branch W0 is real-valued for z &gt;= -1/e. For values of z &lt; -1/e, the result may
    /// be complex or undefined. The method uses an initial approximation followed by iterative refinement to achieve
    /// the specified tolerance.</remarks>
    /// <param name="z">The input value for which to evaluate the principal branch of the Lambert W function.</param>
    /// <param name="maxIter">The maximum number of iterations to perform when refining the result. Must be positive. The default is 50.</param>
    /// <param name="tol">The convergence tolerance for the iterative refinement. Must be greater than zero. The default is 1e-12.</param>
    /// <returns>The value of the principal branch of the Lambert W function at the specified input.</returns>
    public static double W0(double z, int maxIter = 50, double tol = 1e-12)
    {
      if (!(z >= -1 / Math.E))
        throw new ArgumentOutOfRangeException(nameof(z), "Argument must be greater to or equal than 1/e");

      var d = z + 1 / Math.E;
      if (d <= Math.Sqrt(tol)) // if we are close to -1/e, then the following formula will suffice
      {
        double p = Math.Sqrt(2 * d * Math.E);
        return -1 + (((((769 / 17280d * p - 43 / 540d) * p + 11 / 72d) * p - 1 / 3d) * p + 1) * p);
      }

      // w is the start value for Newton-Raphson
      double w = z switch
      {
        > 3 => Math.Log(z) - Math.Log(Math.Log(z)), // asymtotic for big z
        >= 0 => z,                                  // simply z 
        _ => ((((-8 / 3d) * z + 1.5) * z - 1) * z + 1) * z // for negative valöues
      };

      return Newton(z, w, maxIter, tol);
    }

    /// <summary>
    /// Computes the W_{-1} branch of the Lambert W function for real arguments in the range -1/e %le; z &lt; 0.
    /// </summary>
    /// <remarks>The W_{-1} branch of the Lambert W function is defined only for real z in the interval -1/e &lt;=
    /// z &lt; 0. This method uses an iterative approach and may not converge for values of z very close to -1/e or for
    /// insufficiently large maxIter or loose tol values.</remarks>
    /// <param name="z">The input value for which to compute the W_{-1} branch. Must satisfy -1/e &lt;= z &lt; 0.</param>
    /// <param name="maxIter">The maximum number of iterations to use when refining the result. Must be positive. The default is 50.</param>
    /// <param name="tol">The convergence tolerance for the iterative solver. Must be positive. The default is 1e-12.</param>
    /// <returns>The value w such that w ⋅ e^{w} = z, where w is on the W_{-1} branch. Returns a real number for valid z in the
    /// range -1/e &lt;= z &lt; 0.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when z is outside the valid range of -1/e &lt;= z &lt; 0.</exception>
    public static double Wm1(double z, int maxIter = 50, double tol = 1e-12)
    {
      if (!(z < 0 && z >= -1 / Math.E))
        throw new ArgumentOutOfRangeException(nameof(z), "Nur für -1/e <= z < 0 definiert.");

      // Start value: asymptotic approximation for z close to 0
      double w = Math.Log(-z) - Math.Log(-Math.Log(-z));

      return w == -1 ? w : Newton(z, w, maxIter, tol);
    }

    /// <summary>
    /// Finds a root of the equation w * exp(w) = z using the Newton-Raphson method.
    /// </summary>
    /// <remarks>If the method does not converge within the specified number of iterations, the last computed
    /// value is returned. The accuracy of the result depends on the choice of the initial guess and the
    /// tolerance.</remarks>
    /// <param name="z">The value for which to solve the equation w * exp(w) = z.</param>
    /// <param name="w">The initial guess for the root.</param>
    /// <param name="maxIter">The maximum number of iterations to perform.</param>
    /// <param name="tol">The convergence tolerance. The iteration stops when the change between successive approximations is less than
    /// this value.</param>
    /// <returns>The computed root approximation for w such that w * exp(w) ≈ z.</returns>
    private static double Newton(double z, double w, int maxIter, double tol)
    {
      for (int i = 0; i < maxIter; i++)
      {
        double ew = Math.Exp(w);
        double f = w * ew - z;
        double fp = ew * (w + 1);
        double wNext = w - f / fp;

        if (Math.Abs(wNext - w) < tol)
          return wNext;

        w = wNext;
      }
      return w;
    }
  }
}
