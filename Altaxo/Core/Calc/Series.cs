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

// The following code was translated using Matpack sources (http://www.matpack.de) (Author B.Gammel)

using System;

namespace Altaxo.Calc
{
  /// <summary>
  /// Contains series calculations.
  /// </summary>
  public class SeriesMP
  {
    /// <summary>
    /// Represents the smallest number where <c>1 + DBL_EPSILON</c> is not equal to 1.
    /// </summary>
    /// <remarks>
    /// In the IEEE 754 representation, this constant is <c>2^-52</c>.
    /// </remarks>
    private const double DBL_EPSILON = 2.2204460492503131e-016;

    #region initds

    /// <summary>
    /// Initializes an orthogonal series so that the number of terms needed to achieve the requested accuracy is returned.
    /// </summary>
    /// <param name="os">Double-precision array of <paramref name="nos"/> coefficients in an orthogonal series.</param>
    /// <param name="nos">Number of coefficients in <paramref name="os"/>.</param>
    /// <param name="eta">
    /// The requested accuracy of the series. Typically, this is chosen to be one-tenth of machine precision.
    /// </param>
    /// <returns>The number of terms necessary to ensure the error is no larger than <paramref name="eta"/>.</returns>
    /// <remarks>
    /// This is a translation from the Fortran version of SLATEC, FNLIB, CATEGORY C3A2, REVISION 900315,
    /// originally written by Fullerton W. (LANL) to C++.
    /// </remarks>
    public static int initds(double[] os, int nos, double eta)
    {
      if (nos < 1)
        throw new ArgumentException("Number of coefficients is less than 1");

      int i = 0;
      double err = 0.0;
      for (int ii = 1; ii <= nos; ii++)
      {
        i = nos - ii;
        err += Math.Abs(os[i]);
        if (err > eta)
          break;
      }

      if (i == nos)
        throw new ArgumentException("Chebyshev series too short for specified accuracy");

      return i;
    }

    #endregion initds

    #region dcsevl

    /// <summary>
    /// Evaluates the <paramref name="n"/>-term Chebyshev series <paramref name="cs"/> at <paramref name="x"/>.
    /// </summary>
    /// <param name="x">Value at which the series is to be evaluated.</param>
    /// <param name="cs">
    /// Array of <paramref name="n"/> terms of a Chebyshev series. In evaluating the series, only half the first coefficient is summed.
    /// </param>
    /// <param name="n">Number of terms in array <paramref name="cs"/>.</param>
    /// <returns>The value of the <paramref name="n"/>-term Chebyshev series <paramref name="cs"/> at <paramref name="x"/>.</returns>
    /// <remarks>
    /// References:
    ///
    /// R. Broucke, Ten subroutines for the manipulation of Chebyshev series,
    /// Algorithm 446, Communications of the A.C.M. 16 (1973), pp. 254-256.
    ///
    /// L. Fox and I. B. Parker, Chebyshev Polynomials in Numerical Analysis,
    /// Oxford University Press, 1968, page 56.
    ///
    /// This is a translation from the Fortran version of SLATEC, FNLIB, CATEGORY C3A2, REVISION 920501,
    /// originally written by Fullerton W. (LANL) to C++.
    /// </remarks>
    public static double dcsevl(double x, double[] cs, int n)
    {
#if DEBUG
      if (n < 1)
        throw new ArgumentException("Number of terms <= 0");
      if (n > 1000)
        throw new ArgumentException("Number of terms > 1000");
      if (Math.Abs(x) > DBL_EPSILON + 1.0)
        throw new ArgumentException("X outside the interval (-1,+1)");
#endif

      double b0 = 0.0, b1 = 0.0, b2 = 0.0, twox = x * 2;
      for (int i = 1; i <= n; i++)
      {
        b2 = b1;
        b1 = b0;
        b0 = twox * b1 - b2 + cs[n - i];
      }

      return (b0 - b2) * 0.5;
    }

    #endregion dcsevl
  }
}
