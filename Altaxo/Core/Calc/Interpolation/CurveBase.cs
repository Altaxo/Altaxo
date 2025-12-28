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
  /// Base for most interpolations.
  /// </summary>
  public abstract class CurveBase : IInterpolationCurve
  {
    private static readonly double[] _emptyDouble = new double[0];
    /// <summary>
    /// Represents the smallest number where 1+DBL_EPSILON is not equal to 1.
    /// </summary>
    public const double DBL_EPSILON = 2.2204460492503131e-016;

    /// <summary>Reference to the vector of the independent variable.</summary>
    protected IReadOnlyList<double> x = _emptyDouble;

    /// <summary>Reference to the vector of the dependent variable.</summary>
    protected IReadOnlyList<double> y = _emptyDouble;


    #region Helper functions

    /// <summary>
    /// Square of x.
    /// </summary>
    /// <param name="x">Argument.</param>
    /// <returns>The square of x.</returns>
    protected static double sqr(double x)
    {
      return x * x;
    }

    /// <summary>
    /// Return True if vectors have the same index range, False otherwise.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>True if both vectors have the same LowerBounds and the same UpperBounds.</returns>
    protected static bool MatchingIndexRange(IReadOnlyList<double> a, IReadOnlyList<double> b)
    {
      return a.Count == b.Count;
    }

    /// <summary>
    /// Determines whether the elements of <paramref name="a"/> are strictly monotonically increasing.
    /// </summary>
    /// <param name="a">The sequence to test. Must not be empty.</param>
    /// <returns><c>true</c> when each element is greater than its predecessor; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="a"/> is empty.</exception>
    protected static bool IsStrictlyMonotonicallyIncreasing(IReadOnlyList<double> a)
    {
      if (a.Count == 0)
        throw new ArgumentException("Array is empty", nameof(a));

      var previous = a[0];
      for (int i = 1; i < a.Count; ++i)
      {
        if (!(a[i] > previous))
          return false;
      }
      return true;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if <paramref name="a"/> is not strictly
    /// monotonically increasing. Also throws if the array is empty.
    /// If the array contains <see cref="double.NaN"/>, the comparison will fail and an exception
    /// will be thrown.
    /// </summary>
    /// <param name="a">The array to validate.</param>
    /// <param name="argumentName">Name of the argument to include in the exception.</param>
    /// <exception cref="ArgumentException">Thrown when the array is empty or not strictly increasing.</exception>
    protected static void ThrowIfIsNotStrictlyMonotonicallyIncreasing(IReadOnlyList<double> a, string argumentName)
    {
      if (a.Count == 0)
        throw new ArgumentException("Array is empty", nameof(a));

      var previous = a[0];
      for (int i = 1; i < a.Count; ++i)
      {
        if (!(a[i] > previous))
          throw new ArgumentException($"Array {argumentName} is not strictly monotonically increasing at index {i}. Element[{i - 1}]={previous}, Element[{i}]={a[i]}", argumentName);
        previous = a[i];
      }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if <paramref name="a"/> is not monotonically
    /// increasing (each element is greater than or equal to its predecessor). Also throws if
    /// the array is empty.
    /// </summary>
    /// <param name="a">The array to validate.</param>
    /// <param name="argumentName">Name of the argument to include in the exception.</param>
    /// <exception cref="ArgumentException">Thrown when the array is empty or not monotonically increasing.</exception>
    protected static void ThrowIfIsNotMonotonicallyIncreasing(IReadOnlyList<double> a, string argumentName)
    {
      if (a.Count == 0)
        throw new ArgumentException("Array is empty", nameof(a));

      var previous = a[0];
      for (int i = 1; i < a.Count; ++i)
      {
        if (!(a[i] >= previous))
          throw new ArgumentException($"Array {argumentName} is not monotonically increasing at index {i}. Element[{i - 1}]={previous}, Element[{i}]={a[i]}", argumentName);
        previous = a[i];
      }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> when the provided array contains NaN or infinite values.
    /// </summary>
    /// <param name="a">The array to check.</param>
    /// <param name="argumentName">Name of the argument to include in the exception.</param>
    /// <exception cref="ArgumentException">Thrown when an element is NaN or infinite.</exception>
    protected static void ThrowIfContainsNaNOrInfiniteValues(IReadOnlyList<double> a, string argumentName)
    {
      for (int i = 0; i < a.Count; ++i)
      {
        if (!(double.MinValue <= a[i] && a[i] <= double.MaxValue))
          throw new ArgumentException($"Array {argumentName} contains at least one invalid element at index {i}. Element[{i}]={a[i]}", argumentName);
      }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> when the provided array contains negative, NaN or infinite values.
    /// </summary>
    /// <param name="a">The array to check.</param>
    /// <param name="argumentName">Name of the argument to include in the exception.</param>
    /// <exception cref="ArgumentException">Thrown when an element is negative, NaN or infinite.</exception>
    protected static void ThrowIfContainsNegativeOrNaNOrInfiniteValues(IReadOnlyList<double> a, string argumentName)
    {
      for (int i = 0; i < a.Count; ++i)
      {
        if (!(0 <= a[i] && a[i] <= double.MaxValue))
          throw new ArgumentException($"Array {argumentName} contains at least one invalid element at index {i}. Element[{i}]={a[i]}", argumentName);
      }
    }

    #endregion Helper functions

    #region FindInterval

    /// <summary>
    /// Find index of largest element in the increasingly ordered vector x,
    /// which is smaller than u. If u is smaller than the smallest value in
    /// the vector then the lowest index minus one is returned.
    /// </summary>
    /// <param name="u">The value to search for.</param>
    /// <param name="x">Vector of (strictly increasing) x values.</param>
    /// <returns>The index i so that x[i]&lt;u&lt;=x[i+1]. If u is smaller than x[0] then -1 is returned.</returns>
    /// <remarks>
    /// A fast binary search is performed.
    /// Note, that the vector must be strictly increasing.
    /// </remarks>
    public static int FindInterval(double u, IReadOnlyList<double> x)
    {
      int i, j;

      int hi = x.Count - 1;
      if (u < x[0])
      {
        i = -1; // attention: return index below smallest index
      }
      else if (u >= x[hi])
      {
        i = hi; // attention: return highest index
      }
      else
      {
        i = 0;
        j = hi;
        do
        {
          int k = (i + j) / 2;
          if (u < x[k])
            j = k;
          else if (u >= x[k])
            i = k;
          else
            throw new ArithmeticException(string.Format("Either u or x[k] is NaN: u={0}, x[{1}]={2}", u, k, x[k]));
        } while (j > i + 1);
      }
      return i;
    }

    #endregion FindInterval

    /// <summary>
    /// Return the interpolation value P(u) for a piecewise cubic curve determined
    /// by the abscissa vector <paramref name="x"/>, the ordinate vector <paramref name="y"/>,
    /// and derivative coefficient vectors <paramref name="y1"/>, <paramref name="y2"/>, and <paramref name="y3"/>, using the Horner scheme.
    /// </summary>
    /// <param name="u">The abscissa value at which the interpolation is to be evaluated.</param>
    /// <param name="x">The vector (lo,hi) of data abscissa (must be strictly increasing).</param>
    /// <param name="y">The vectors (lo,hi) of ordinate</param>
    /// <param name="y1">Contains the 1st derivative y'(x(i)).</param>
    /// <param name="y2">Contains the 2nd derivative y''(x(i)).</param>
    /// <param name="y3">Contains the 3rd derivative y'''(x(i)).</param>
    /// <returns>The interpolated value P(u). If <paramref name="x"/> is empty, returns 0.0.</returns>
    /// <remarks><code>
    /// All vectors must have conformant dimenions.
    /// The abscissa x(i) values must be strictly increasing.
    ///
    ///
    /// This subroutine evaluates the function
    ///
    ///    P(u) = y(i) + dx * (y1(i) + dx * (y2(i) + dx * y3(i)))
    ///
    /// where  x(i) &lt;= u &lt; x(i+1) and dx = u - x(i), using Horner's rule
    ///
    ///    lo &lt;= i &lt;= hi is the index range of the vectors.
    ///    if  u &lt;  x(lo) then  i = lo  is used.
    ///    if  u &lt;= x(hi) then  i = hi  is used.
    ///
    ///    A fast binary search is performed to determine the proper interval.
    /// </code></remarks>
    public double CubicSplineHorner(double u,
      IReadOnlyList<double> x,
      IReadOnlyList<double> y,
      IReadOnlyList<double> y1,
      IReadOnlyList<double> y2,
      IReadOnlyList<double> y3)
    {
      // special case that there are no data. Return 0.0.
      if (x.Count == 0)
        return 0;

      int i = FindInterval(u, x);
      if (i < 0)
        i = 0;  // extrapolate to the left
      if (i == x.Count - 1)
        i--;   // extrapolate to the right

      double dx = u - x[i];
      return (y[i] + dx * (y1[i] + dx * (y2[i] + dx * y3[i])));
    }

    /// <summary>
    /// Return the first derivative P'(u) of the piecewise cubic curve evaluated using Horner's scheme.
    /// </summary>
    /// <param name="u">The abscissa value at which the derivative is to be evaluated.</param>
    /// <param name="x">The vector (lo,hi) of data abscissa (must be strictly increasing).</param>
    /// <param name="y">The vectors (lo,hi) of ordinate (not used for derivative calculation but kept for signature compatibility).</param>
    /// <param name="y1">Contains the 1st derivative y'(x(i)).</param>
    /// <param name="y2">Contains the 2nd derivative y''(x(i)).</param>
    /// <param name="y3">Contains the 3rd derivative y'''(x(i)).</param>
    /// <returns>The value of the first derivative at <paramref name="u"/>. If <paramref name="x"/> is empty, returns 0.0.</returns>
    public double CubicSplineHorner1stDerivative(double u,
    IReadOnlyList<double> x,
    IReadOnlyList<double> y,
    IReadOnlyList<double> y1,
    IReadOnlyList<double> y2,
    IReadOnlyList<double> y3)
    {
      // special case that there are no data. Return 0.0.
      if (x.Count == 0)
        return 0.0;

      int i = FindInterval(u, x);
      if (i < 0)
        i = 0;  // extrapolate to the left
      if (i == x.Count - 1)
        i--;   // extrapolate to the right
      double dx = u - x[i];
      return (y1[i] + dx * (2 * y2[i] + dx * 3 * y3[i]));
    }

    /// <summary>
    /// Calculate the spline coefficients y2(i) and y3(i) for a natural cubic
    /// spline, given the abscissa x(i), the ordinate y(i), and the 1st
    /// derivative y1(i).
    /// </summary>
    /// <param name="x">The vector (lo,hi) of data abscissa (must be strictly increasing).</param>
    /// <param name="y">The vector (lo,hi) of ordinate.</param>
    /// <param name="y1">The vector containing the 1st derivative y'(x(i)).</param>
    /// <param name="y2">Output: the spline coefficients y2(i).</param>
    /// <param name="y3">Output: the spline coefficients y3(i).</param>
    /// <remarks><code>
    /// The spline interpolation can then be evaluated using Horner's rule
    ///
    ///      P(u) = y(i) + dx * (y1(i) + dx * (y2(i) + dx * y3(i)))
    ///
    /// where  x(i) &lt;= u &lt; x(i+1) and dx = u - x(i).
    /// </code></remarks>
    public void CubicSplineCoefficients(
      IReadOnlyList<double> x,
      IReadOnlyList<double> y,
      IReadOnlyList<double> y1,
      Vector<double> y2,
      Vector<double> y3)
    {
      int hi = x.Count - 1;

      for (int i = 0; i < hi; i++)
      {
        double h = x[i + 1] - x[i],
          mi = (y[i + 1] - y[i]) / h;
        y2[i] = (3 * mi - 2 * y1[i] - y1[i + 1]) / h;
        y3[i] = (y1[i] + y1[i + 1] - 2 * mi) / (h * h);
      }

      y2[hi] = y3[hi] = 0.0;
    }

    /// <summary>
    /// Interpolates a curve using abcissa x and ordinate y.
    /// </summary>
    /// <param name="x">The vector of abscissa values.</param>
    /// <param name="y">The vector of ordinate values.</param>
    public abstract void Interpolate(IReadOnlyList<double> x, IReadOnlyList<double> y);

    /// <summary>
    /// Get the abscissa value in dependence on parameter u.
    /// </summary>
    /// <param name="u">Curve parameter.</param>
    /// <returns>The abscissa value.</returns>
    public abstract double GetXOfU(double u);

    /// <summary>
    /// Gets the ordinate value on dependence on parameter u.
    /// </summary>
    /// <param name="u">Curve parameter.</param>
    /// <returns>The ordinate value.</returns>
    public abstract double GetYOfU(double u);

    /// <summary>
    /// Curve length parametrization. Returns the accumulated "distances"
    /// between the points (x(i),y(i)) and (x(i+1),y(i+1)) in t(i+1)
    /// for i = lo ... hi. t(lo) = 0.0 always.
    /// </summary>
    /// <param name="x">The vector of abscissa values.</param>
    /// <param name="y">The vector of ordinate values.</param>
    /// <param name="t">Output: the vector of "distances".</param>
    /// <param name="parametrization">The parametrization rule to apply.</param>
    /// <remarks><code>
    /// The way of parametrization is controlled by the parameter parametrization.
    /// Parametrizes curve length using:
    ///
    ///    |dx| + |dy|       if  parametrization = Norm1
    ///    sqrt(dx^2+dy^2)   if  parametrization = Norm2
    ///    (dx^2+dy^2)       if  parametrization = SqrNorm2
    ///
    /// Parametrization using Norm2 usually gives the best results.
    /// </code></remarks>
    public virtual void Parametrize(IReadOnlyList<double> x, IReadOnlyList<double> y, IVector<double> t, Parametrization parametrization)
    {
      const int lo = 0;
      int hi = x.Count - 1;
      int i;

      switch (parametrization)
      {
        case Parametrization.Norm1:
          for (i = lo + 1, t[lo] = 0.0; i <= hi; i++)
            t[i] = t[i - 1] + Math.Abs(x[i] - x[i - 1]) + Math.Abs(y[i] - y[i - 1]);
          break;

        case Parametrization.Norm2:
          for (i = lo + 1, t[lo] = 0.0; i <= hi; i++)
            t[i] = t[i - 1] + RMath.Hypot(x[i] - x[i - 1], y[i] - y[i - 1]);
          break;

        case Parametrization.SqrNorm2:
          for (i = lo + 1, t[lo] = 0.0; i <= hi; i++)
            t[i] = t[i - 1] + sqr(x[i] - x[i - 1]) + sqr(y[i] - y[i - 1]);
          break;

        default:
          throw new System.ArgumentException("illegal value for parametrization method");
      }
    }

    #region GetResolution and DrawCurve

    /*

        //----------------------------------------------------------------------------//
        //
        // int MpCurveBase::GetResolution (const Scene& scene,
        //             double x1, double y1,
        //           double x2, double y2) const
        //
        // Calculate the number of intermediate points neccessary to get a
        // smooth appearance when drawing a line segment between (x1,y1) and (x2,y2).
        //
        //----------------------------------------------------------------------------//

        int GetResolution (IScene scene,
          double x1, double y1, double x2, double y2)
      {
      int r = scene.curve.resolution + 2;
      Pixel2D p1( scene.Map(x1,y1)), p2(scene.Map(x2,y2) );
      return 1 + int( (r + abs(p2.px-p1.px) + abs(p2.py-p1.py)) / (2 * r) );
    }

    */

    /// <summary>
    /// This function has to provide the points that are necessary between (x1,y1) and (x2,y2)
    /// to get a smooth curve.
    /// </summary>
    public delegate int ResolutionFunction(double x1, double y1, double x2, double y2);

    /// <summary>
    /// This function serves as a sink for the calculated points of a curve.
    /// </summary>
    public delegate void PointSink(double x, double y, bool bLastPoint);

    /// <summary>
    /// Get curve points to draw an interpolation curve between the abscissa values xlo and xhi.
    /// It calls the virtual methods MpCurveBase::GetXOfU() and GetYOfU() to obtain the
    /// interpolation values. Note, that before method DrawCurve() can be called
    /// the method Interpolate() must have been called. Otherwise, not interpolation
    /// is available.
    /// </summary>
    /// <param name="xlo">Lower bound of the drawing range.</param>
    /// <param name="xhi">Upper bound of the drawing range.</param>
    /// <param name="getresolution">A delegate that must provide the points necessary to draw a smooth curve between to points.</param>
    /// <param name="setpoint">A delegate which is called with each calculated point. Can be used to draw the curve. </param>
    public void GetCurvePoints(double xlo, double xhi, ResolutionFunction getresolution, PointSink setpoint)
    {
      // nothing to draw if zero or one element
      if (x.Count < 2)
        return;

      // Find index of the element in the abscissa vector x, that is smaller
      // than the lower (upper) value xlo (xhi) of the drawing range. If xlo is
      // smaller than the lowest abscissa value the lowest index minus one is
      // returned.
      int i_lo = FindInterval(xlo, x),
        i_hi = FindInterval(xhi, x);

      // Interpolation values for the boundaries of the drawing range [xlo,xhi]
      double ylo = GetYOfU(xlo),
        yhi = GetYOfU(xhi);

      int k;
      double x0, t, delta;

      setpoint(xlo, ylo, false);
      k = getresolution(xlo, ylo, x[i_lo + 1], y[i_lo + 1]);
      delta = (x[i_lo + 1] - xlo) / k;
      for (int j = 0; j < k; j++)
      {
        t = xlo + j * delta;
        setpoint(GetXOfU(t), GetYOfU(t), false);
      }

      for (int i = i_lo + 1; i < i_hi; i++)
      {
        x0 = x[i];
        k = getresolution(x0, y[i], x[i + 1], y[i + 1]);
        delta = (x[i + 1] - x0) / k;
        for (int j = 0; j < k; j++)
        {
          t = x0 + j * delta;
          setpoint(GetXOfU(t), GetYOfU(t), false);
        }
      }

      x0 = x[i_hi];
      k = getresolution(x0, y[i_hi], xhi, yhi);
      delta = (xhi - x0) / k;
      for (int j = 0; j < k; j++)
      {
        t = x0 + j * delta;
        setpoint(GetXOfU(t), GetYOfU(t), false);
      }

      // don't forget last point
      setpoint(xhi, yhi, true);
    }

    #endregion GetResolution and DrawCurve
  }
}
