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

#region Acknowledgements

// The following code was translated using Matpack sources (http://www.matpack.de) (Author B.Gammel)
// Original MatPack-1.7.3\Source\mpcurvebase.h
//                               mpcurvebase.cc
//                               mpfcspline.h
//                               mpfcspline.cc
//                               mpaspline.h
//                               mpaspline.cc
//                               mpbspline.h
//                               mpbspline.cc
//                               mpcspline.h
//                               mpcspline.cc
//                               mprspline.h
//                               mprspline.cc
//                               mpespline.h
//                               mpespline.cc
//                               mppolyinterpol.h
//                               mppolyinterpol.cc
//                               mpratinterpol.h
//                               mpratinterpol.cc
//                               mpgcvspline.h
//                               mpgcvspline.cc

#endregion Acknowledgements

using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Implements polynomial interpolation using the Aitken-Neville tableau for single-valued interpolation functions.
  /// </summary>
  public class PolynomialInterpolation : CurveBase, IInterpolationFunction
  {
    /// <summary>
    /// Working coefficients for the forward branch of the Aitken-Neville tableau.
    /// </summary>
    protected Vector<double> C = CreateVector.Dense<double>(0);

    /// <summary>
    /// Working coefficients for the backward branch of the Aitken-Neville tableau.
    /// </summary>
    protected Vector<double> D = CreateVector.Dense<double>(0);

    //----------------------------------------------------------------------------//
    //
    // int MpPolynomialInterpolation::Interpolate (const Vector &x, const Vector &y)
    //
    //
    //----------------------------------------------------------------------------//

    /// <inheritdoc/>
    public override void Interpolate(IReadOnlyList<double> x, IReadOnlyList<double> y)
    {
      // check input parameters

      if (!MatchingIndexRange(x, y))
        throw new ArgumentException("index range mismatch of vectors");

      // link original data vectors into base class
      base.x = x;
      base.y = y;

      // Empty data vectors - free auxiliary storage
      if (x.Count == 0)
      {
        C.Clear();
        D.Clear();
      }
    }

    /// <inheritdoc/>
    public override double GetXOfU(double u)
    {
      return u;
    }

    /// <inheritdoc/>
    public double GetYOfX(double x)
    {
      return GetYOfU(x);
    }

    //----------------------------------------------------------------------------//
    //
    // double MpPolynomialInterpolation::GetYOfU (double u)
    //
    // Polynomial interpolation using the Aitken-Neville tableaux.
    // The returned y is the value that corresponds to the value of
    // the polynomial y = P(x) of degree n = hi-lo
    // that interpolates the data points (x(i),y(i)), lo <= i <= hi.
    // In the special case of empty data vectors (x,y), a value of 0.0 is returned.
    //
    //----------------------------------------------------------------------------//

    /// <inheritdoc/>
    /// <remarks>
    /// Uses the Aitken-Neville tableau to evaluate the interpolating polynomial and returns 0.0 when no data points exist.
    /// </remarks>
    public override double GetYOfU(double u)
    {
      // special case that there are no data. Return 0.0.
      if (x.Count == 0)
        return 0.0;

      const int lo = 0;
      int hi = x.Count - 1;

      // allocate (resize) auxiliary vectors - the resize method has the property
      // that no (de-)allocation is done, if the size of the vector is not changed.
      // Thus there is no overhead if GetYOfU() is called many times with the
      // same vectors, for instance, if a whole curve is drawn.
      if (C.Count != x.Count)
      {
        C = CreateVector.Dense<double>(x.Count);
        D = CreateVector.Dense<double>(x.Count);
      }

      C.SetValues(y);        // initialize // TODO original was C = D = *y; check Vector if this is a copy operation
      D.SetValues(y);

      int pos = lo;     // find position of closest abscissa value
      double delta = Math.Abs(u - x[lo]), delta2;
      for (int i = lo; i <= hi; i++)
        if ((delta2 = Math.Abs(u - x[i])) < delta)
        {
          delta = delta2;
          pos = i;
        }

      double dy, yy = C[pos--];   // initial approximation

      for (int m = 1; m <= (hi - lo); m++)
      {
        for (int i = lo; i <= hi - m; i++)
        {
          double h1 = x[i] - u,
            h2 = x[i + m] - u,
            CD = C[i + 1] - D[i],
            denom;
          if ((denom = h1 - h2) == 0.0)
            throw new ArgumentException("two abscissa values are identical");
          denom = CD / denom;
          C[i] = denom * h1;
          D[i] = denom * h2;
        }
        yy += (dy = (2 * pos - lo + 1 < (hi - m) ? C[pos + 1] : D[pos--]));
      }

      return yy;      // return value
    }
  }
}
