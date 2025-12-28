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
using Altaxo.Collections;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Provides rational interpolation through a set of support points.
  /// </summary>
  /// <remarks>
  /// The implementation is based on Matpack sources (see acknowledgements) and computes a rational
  /// interpolant represented in continued-fraction form.
  /// </remarks>
  public class RationalInterpolation : CurveBase, IInterpolationFunction
  {
    protected Vector<double> xr = CreateVector.Dense<double>(0);
    protected Vector<double> yr = CreateVector.Dense<double>(0);
    protected Vector<int> m = CreateVector.Dense<int>(0);
    protected int num;
    protected double epsilon;

    /// <summary>
    /// Initializes a new instance of the `RationalInterpolation` class.
    /// </summary>
    public RationalInterpolation()
    {
      num = 2;
      epsilon = DBL_EPSILON;
    }

    //----------------------------------------------------------------------------//
    //
    // int MpRationalInterpolation::Interpolate (const Vector &x, const Vector &y)
    //
    // Description:
    // ------------
    //
    //  Calculates a rational interpolation
    //  polynomial with numerator degree N and denominator
    //  degree D which passes through the n given points
    //  (x[i],y[i]). In the unique solution the denominator
    //  degree D is determined by the relation
    //
    //        D = n - 1 - N
    //
    //  for the given values of n and N.
    //
    //  The required precision "double epsilon" should be set before calling
    //  this function. Use function SetPrecision (double eps) for this purpose.
    //
    // Arguments:
    // ----------
    //
    //    Vector& x
    //    Vector& y
    //
    //  The x- and y- values (x[i],y[i]) which are to be
    //  interpolated. The vectors must have the same index
    //  range i = lo,...,hi. This means n = hi-lo+1 values.
    //
    // Return values:
    // --------------
    //
    //   0  everything is ok
    //
    //   1  Interpolation function doesn't exist. You should try a numerator
    //      degree N > (n - 1) / 2
    //
    //   2  Number of points still to interpolate and degree of numerator
    //      polynomial N < 0. You should try to change the numerator degree.
    //
    //   3  Degree of denominator polynomial < 0. You should try to change
    //      the numerator degree.
    //
    //   4  Not all points have been used in the interpolation. You should
    //      try to change the numerator degree.
    //
    //
    // Reference:
    // ----------
    //
    //   H. Werner, A reliable and numerically stable program for rational
    //   interpolation of Lagrange data, Computing, Vol 31, 269 (1983).
    //
    //   H. Werner, R. Schaback, Praktische Mathematik II, Springer,
    //   Berlin, Heidelberg, New York, 1972, 2. Aufl. 1979.
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

      // Empty data vectors - free auxilliary storage
      if (x.Count == 0)
      {
        xr.Clear();
        yr.Clear();
        m.Clear();
        return;
      }

      const int lo = 0;
      int hi = x.Count - 1;

      if (xr.Count != x.Count)
      {
        xr = CreateVector.Dense<double>(x.Count);
        yr = CreateVector.Dense<double>(x.Count);
        m = CreateVector.Dense<int>(x.Count);
      }

      int i, j, j1, denom, nend;
      double xj, yj, x2, y2;

      int n = x.Count - 1;

      if (n < 1)
        throw new ArgumentException(string.Format("less than two points where given ({0})", n + 1));

      if (num <= 0)
        throw new ArgumentException("numerator degree must be positive");

      if (num > n)
        throw new ArgumentException(string.Format("degree of numerator polynomial ({0}) was greater or equal to the number of data points ({1})",
          num, n + 1));

      for (i = lo; i < hi; i++)
        for (j = i + 1; j <= hi; j++)
          if (x[i] == x[j])
            throw new ArgumentException(string.Format("two equal x values at ({0}) and ({1})",
              i, j));

      // precision limit
      epsilon = Math.Max(epsilon, 128.0 * DBL_EPSILON);

      // allocate auxilliary storage
      var z = CreateVector.Dense<double>(hi);

      // copy original values
      xr.SetValues(x);
      yr.SetValues(y);

      // initialize M to 1
      m.FillWith(1);

      nend = hi;
      denom = n - num; // degree of denominator polynomial

      if (num < denom)
      {
        for (i = lo; i <= hi; i++)
        {
          if (yr[i] != 0.0)
            yr[i] = 1.0 / yr[i];
          else
            throw new ArgumentException("Interpolation function does not exist"); // interpolation function doesn't exist
        }
        m[hi] = 0;
        j = num;
        num = denom;
        denom = j;
      }

      while (nend > lo)
      {
        for (i = 1; i <= num - denom; i++)
        {
          xj = xr[nend];
          yj = yr[nend];
          for (j = lo; j < nend; j++)
            yr[j] = (yr[j] - yj) / (xr[j] - xj);
          --nend;
        }

        if (nend < lo && denom < 0)
          throw new InvalidOperationException("Denominator is < 0");

        if (nend > lo)
        {
          ymin(nend, out xj, out yj, xr, yr);

          j1 = lo;
          for (j = lo; j < nend; j++)
          {
            y2 = yr[j] - yj;
            x2 = xr[j] - xj;
            if (Math.Abs(y2) <= Math.Abs(x2) * epsilon)
              z[j1++] = xr[j];
            else
            {
              yr[j - j1 + lo] = x2 / y2;
              xr[j - j1 + lo] = xr[j];
            }
          }
          for (j = lo; j < j1; j++)
          {
            xr[nend - 1] = z[j];
            yr[nend - 1] = 0.0;
            for (i = lo; i < nend; i++)
              yr[i] *= xr[i] - xr[nend];
            --nend;
          }
          if (nend > lo)
          {
            m[--nend] = 0;
            num = denom;
            denom = nend - num;
          }
          if (denom < 0 && nend < lo)
            throw new InvalidOperationException("Degree of denominator polynomial is < 0"); // degree of denominator polynomial < 0
        }
      }

      y2 = Math.Abs(yr[hi]);
      for (i = lo; i < hi; i++)
        y2 += Math.Abs(yr[i]);
      for (i = lo; i <= hi; i++)
      {
        x2 = GetYOfU(x[i]);
        if (Math.Abs(x2 - y[i]) > n * epsilon * y2)
          throw new InvalidOperationException("Not all points have been used"); // not all points have been used
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

    private static readonly double RootMax = Math.Sqrt(double.MaxValue);

    /// <inheritdoc/>
    public override double GetYOfU(double u)
    {
      const double SquareEps = DBL_EPSILON * DBL_EPSILON;

      const int lo = 0;
      int hi = yr.Count - 1;

      double val = yr[lo];
      for (int i = lo + 1; i <= hi; i++)
      {
        if (m[i - 1] != 0)
          val = yr[i] + (u - xr[i]) * val;
        else if (Math.Abs(val) > SquareEps)
          val = yr[i] + (u - xr[i]) / val;
        else
          return RootMax;
      }

      if (m[hi] != 0)
        return val;
      else if (Math.Abs(val) > SquareEps)
        return 1.0 / val;
      else
        return RootMax;
    }

    /// <summary>
    /// Gets or sets the precision parameter used to decide whether two values are treated as equal
    /// during the interpolation algorithm.
    /// </summary>
    public double Precision
    {
      set { epsilon = value; }
      get { return epsilon; }
    }

    /// <summary>
    /// Gets or sets the degree of the numerator polynomial used for the rational interpolation.
    /// </summary>
    public int NumeratorDegree
    {
      set { num = value; }
      get { return num; }
    }

    //----------------------------------------------------------------------------//
    //
    // static void ymin (int nend, double& xj, double& yj, Vector& x, Vector& y)
    //
    // local auxilliary function
    //
    //----------------------------------------------------------------------------//

    /// <summary>
    /// Finds the element of <paramref name="y"/> with minimal absolute value within the range
    /// <c>[0, nend]</c> and swaps it with the element at <paramref name="nend"/> in both
    /// <paramref name="x"/> and <paramref name="y"/>.
    /// </summary>
    /// <param name="nend">Upper bound (inclusive) of the range to search.</param>
    /// <param name="xj">Receives the selected abscissa value.</param>
    /// <param name="yj">Receives the selected ordinate value.</param>
    /// <param name="x">Abscissa working vector.</param>
    /// <param name="y">Ordinate working vector.</param>
    private static void ymin(int nend, out double xj, out double yj, Vector<double> x, Vector<double> y)
    {
      int j;
      yj = y[nend];
      j = nend;
      for (int k = 0; k < nend; k++)
        if (Math.Abs(y[k]) < Math.Abs(yj))
        {
          j = k;
          yj = y[j];
        }
      xj = x[j];
      x[j] = x[nend];
      x[nend] = xj;
      y[j] = y[nend];
      y[nend] = yj;
    }
  }
}
