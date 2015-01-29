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

using Altaxo.Calc.LinearAlgebra;
using System;

namespace Altaxo.Calc.Interpolation
{
	/// <summary>
	/// Represents an interpolation curve. The curve is parametrized using a parameter u. Because of the parametrization, it is possible that
	/// for some x values there can exist more than one corresponding y values.
	/// </summary>
	public interface IInterpolationCurve
	{
		/// <summary>
		/// Sets the interpolation data by providing values for x and y. Both vectors must be of equal length.
		/// </summary>
		/// <param name="xvec">Vector of x (independent) data.</param>
		/// <param name="yvec">Vector of y (dependent) data.</param>
		/// <returns></returns>
		int Interpolate(IROVector xvec, IROVector yvec);

		/// <summary>
		/// Returns a y value in dependence of a parameter u.
		/// </summary>
		/// <param name="u">The parameter.</param>
		/// <returns>The y value at the given value of u.</returns>
		double GetYOfU(double u);

		/// <summary>
		/// Returns a x value in dependence of a parameter u.
		/// </summary>
		/// <param name="u">The parameter.</param>
		/// <returns>The y value at the given value of u.</returns>
		double GetXOfU(double u);
	}

	/// <summary>
	/// Gives an interpolation function, i.e. for every given x, there is exactly one corresponding y value.
	/// </summary>
	public interface IInterpolationFunction : IInterpolationCurve
	{
		/// <summary>
		/// Returns the y value in dependence of a given x value.
		/// </summary>
		/// <param name="x">The x value (value of the independent variable).</param>
		/// <returns>The y value at the given x value.</returns>
		double GetYOfX(double x);
	}

	/// <summary>Condition how to manage the left and right boundary of a spline.</summary>
	public enum BoundaryConditions
	{
		/// <summary>natural boundaries, zero 2nd deriv.</summary>
		Natural = 0,

		/// <summary>finite differences for 1st derivatives</summary>
		FiniteDifferences,

		/// <summary>user supplied f'(x_lo), f'(x_hi)</summary>
		Supply1stDerivative,

		/// <summary>user supplied f''(x_lo), f''(x_hi)</summary>
		Supply2ndDerivative,

		/// <summary>periodic boundaries (NOT YET IMPLEMENTED)</summary>
		Periodic
	}

	/// <summary>curve parametrization methods</summary>
	public enum Parametrization
	{
		/// <summary>don't parametrize (default)</summary>
		No = 0,

		/// <summary>use sqrt(dx^2+dy^2)</summary>
		Norm2,

		/// <summary>use (dx^2+dy^2)</summary>
		SqrNorm2,

		/// <summary>use |dx| + |dy|</summary>
		Norm1
	}

	#region CurveBase

	/// <summary>
	/// Base for most interpolations.
	/// </summary>
	public abstract class CurveBase : IInterpolationCurve
	{
		/// <summary>
		/// Represents the smallest number where 1+DBL_EPSILON is not equal to 1.
		/// </summary>
		protected const double DBL_EPSILON = 2.2204460492503131e-016;

		/// <summary>Reference to the vector of the independent variable.</summary>
		protected IROVector x;

		/// <summary>Reference to the vector of the dependent variable.</summary>
		protected IROVector y;

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
		protected static bool MatchingIndexRange(IROVector a, IROVector b)
		{
			return a.Length == b.Length;
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
		public static int FindInterval(double u, IROVector x)
		{
			int i, j;

			int hi = x.Length - 1;
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
					if (u < x[k]) j = k;
					else if (u >= x[k]) i = k;
					else throw new ArithmeticException(string.Format("Either u or x[k] is NaN: u={0}, x[{1}]={2}", u, k, x[k]));
				} while (j > i + 1);
			}
			return i;
		}

		#endregion FindInterval

		/// <summary>
		/// Return the interpolation value P(u) for a piecewise cubic curve determined
		/// by the abscissa vector x, the ordinate vector y, the 1st derivative
		/// vector y1, the 2nd derivative vector y2, and the 3rd derivative vector y3,
		/// using the Horner scheme.
		/// </summary>
		/// <param name="u">The abscissa value at which the interpolation is to be evaluated.</param>
		/// <param name="x">The vector (lo,hi) of data abscissa (must be strictly increasing).</param>
		/// <param name="y">The vectors (lo,hi) of ordinate</param>
		/// <param name="y1">contains the 1st derivative y'(x(i))</param>
		/// <param name="y2">contains the 2nd derivative y''(x(i))</param>
		/// <param name="y3">contains the 3rd derivative y'''(x(i))</param>
		/// <returns>P(u) = y(i) + dx * (y1(i) + dx * (y2(i) + dx * y3(i))).
		/// In the special case of empty data vectors (x,y) a value of 0.0 is returned.</returns>
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
			IROVector x,
			IROVector y,
			IROVector y1,
			IROVector y2,
			IROVector y3)
		{
			// special case that there are no data. Return 0.0.
			if (x.Length == 0)
				return 0;

			int i = FindInterval(u, x);
			if (i < 0)
				i = 0;  // extrapolate to the left
			if (i == x.Length - 1)
				i--;   // extrapolate to the right

			double dx = u - x[i];
			return (y[i] + dx * (y1[i] + dx * (y2[i] + dx * y3[i])));
		}

		public double CubicSplineHorner1stDerivative(double u,
		IROVector x,
		IROVector y,
		IROVector y1,
		IROVector y2,
		IROVector y3)
		{
			// special case that there are no data. Return 0.0.
			if (x.Length == 0) return 0.0;

			int i = FindInterval(u, x);
			if (i < 0) i = 0;  // extrapolate to the left
			if (i == x.Length - 1) i--;   // extrapolate to the right
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
		public void CubicSplineCoefficients(IROVector x,
			IROVector y,
			IROVector y1,
			IVector y2,
			IVector y3)
		{
			int hi = x.Length - 1;

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
		/// <returns></returns>
		public abstract int Interpolate(IROVector x, IROVector y);

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
		public virtual void Parametrize(IROVector x, IROVector y, IVector t, Parametrization parametrization)
		{
			const int lo = 0;
			int hi = x.Length - 1;
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
			if (x.Length < 2)
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

	#endregion CurveBase

	#region LinearInterpolation

	/// <summary>
	/// Contains static methods for linear interpolation of data.
	/// </summary>
	public class LinearInterpolation : IInterpolationFunction
	{
		#region Members

		private DoubleVector x, y;

		#endregion Members

		#region IInterpolationFunction Members

		public double GetYOfX(double xval)
		{
			int idx = CurveBase.FindInterval(xval, x);
			if (idx < 0)
				throw new ArgumentOutOfRangeException("xval is smaller than the interpolation x range");
			if (idx + 1 > x.Length)
				throw new ArgumentOutOfRangeException("xval is greater than the interpolation x range");

			if (idx + 1 == x.Length)
				idx--;

			return Interpolate(xval, x[idx], x[idx + 1], y[idx], y[idx + 1]);
		}

		#endregion IInterpolationFunction Members

		#region IInterpolationCurve Members

		public int Interpolate(IROVector xvec, IROVector yvec)
		{
			if (null == x)
				x = new DoubleVector();
			if (null == y)
				y = new DoubleVector();

			x.CopyFrom(xvec);
			y.CopyFrom(yvec);

			return 0;
		}

		public double GetYOfU(double u)
		{
			return GetYOfX(u);
		}

		public double GetXOfU(double u)
		{
			return u;
		}

		#endregion IInterpolationCurve Members

		#region Static methods

		public static int GetNextIndexOfValidPair(IROVector xcol, IROVector ycol, int sourceLength, int currentIndex)
		{
			for (int sourceIndex = currentIndex; sourceIndex < sourceLength; sourceIndex++)
			{
				if (!double.IsNaN(xcol[sourceIndex]) && !double.IsNaN(ycol[sourceIndex]))
					return sourceIndex;
			}

			return -1;
		}

		public static double Interpolate(double x, double x0, double x1, double y0, double y1)
		{
			double r = (x - x0) / (x1 - x0);
			return (1 - r) * y0 + r * y1;
		}

		public static string Interpolate(
			IROVector xcol,
			IROVector ycol,
			int sourceLength,
			double xstart, double xincrement, int numberOfValues,
			double yOutsideOfBounds,
			out double[] resultCol)
		{
			resultCol = new double[numberOfValues];

			int currentIndex = GetNextIndexOfValidPair(xcol, ycol, sourceLength, 0);
			if (currentIndex < 0)
				return "The two columns don't contain a valid pair of values";

			int nextIndex = GetNextIndexOfValidPair(xcol, ycol, sourceLength, currentIndex + 1);
			if (nextIndex < 0)
				return "The two columns contain only one valid pair of values, but at least two valid pairs are neccessary!";

			double x_current = xcol[currentIndex];
			double x_next = xcol[nextIndex];

			int resultIndex = 0;

			// handles values before interpolation range
			for (resultIndex = 0; resultIndex < numberOfValues; resultIndex++)
			{
				double x_result = xstart + resultIndex * xincrement;
				if (x_result >= x_current)
					break;

				resultCol[resultIndex] = yOutsideOfBounds;
			}

			// handle values in the interpolation range
			for (; resultIndex < numberOfValues; resultIndex++)
			{
				double x_result = xstart + resultIndex * xincrement;

			tryinterpolation:
				if (x_result >= x_current && x_result <= x_next)
				{
					resultCol[resultIndex] = Interpolate(x_result, x_current, x_next, ycol[currentIndex], ycol[nextIndex]);
				}
				else
				{
					currentIndex = nextIndex;
					x_current = x_next;
					nextIndex = GetNextIndexOfValidPair(xcol, ycol, sourceLength, currentIndex + 1);
					if (nextIndex < 0)
						break;

					x_next = xcol[nextIndex];
					goto tryinterpolation;
				}
			}

			// handle values behind the interplation range
			for (; resultIndex < numberOfValues; resultIndex++)
			{
				resultCol[resultIndex] = yOutsideOfBounds;
			}

			return null;
		}

		public static string Interpolate(
			IROVector xcol,
			IROVector ycol,
			int sourceLength,
			IROVector xnewsampling, int numberOfValues,
			double yOutsideOfBounds,
			out double[] resultCol)
		{
			resultCol = new double[numberOfValues];

			int currentIndex = GetNextIndexOfValidPair(xcol, ycol, sourceLength, 0);
			if (currentIndex < 0)
				return "The two columns don't contain a valid pair of values";

			int nextIndex = GetNextIndexOfValidPair(xcol, ycol, sourceLength, currentIndex + 1);
			if (nextIndex < 0)
				return "The two columns contain only one valid pair of values, but at least two valid pairs are neccessary!";

			double x_current = xcol[currentIndex];
			double x_next = xcol[nextIndex];

			int resultIndex = 0;

			// handles values before interpolation range
			for (resultIndex = 0; resultIndex < numberOfValues; resultIndex++)
			{
				double x_result = xnewsampling[resultIndex];
				if (x_result >= x_current)
					break;

				resultCol[resultIndex] = yOutsideOfBounds;
			}

			// handle values in the interpolation range
			for (; resultIndex < numberOfValues; resultIndex++)
			{
				double x_result = xnewsampling[resultIndex];

			tryinterpolation:
				if (x_result >= x_current && x_result <= x_next)
				{
					resultCol[resultIndex] = Interpolate(x_result, x_current, x_next, ycol[currentIndex], ycol[nextIndex]);
				}
				else
				{
					currentIndex = nextIndex;
					x_current = x_next;
					nextIndex = GetNextIndexOfValidPair(xcol, ycol, sourceLength, currentIndex + 1);
					if (nextIndex < 0)
						break;

					x_next = xcol[nextIndex];
					goto tryinterpolation;
				}
			}

			// handle values behind the interplation range
			for (; resultIndex < numberOfValues; resultIndex++)
			{
				resultCol[resultIndex] = yOutsideOfBounds;
			}

			return null;
		}

		#endregion Static methods
	}

	#endregion LinearInterpolation

	#region PolynomialRegressionAsInterpolation

	public class PolynomialRegressionAsInterpolation : IInterpolationFunction
	{
		private Regression.LinearFitBySvd _fit;

		public int RegressionOrder { get; set; }

		public PolynomialRegressionAsInterpolation()
		{
			RegressionOrder = 2;
		}

		public PolynomialRegressionAsInterpolation(int regressionOrder)
		{
			RegressionOrder = regressionOrder;
		}

		public int Interpolate(IROVector xvec, IROVector yvec)
		{
			IROVector err = VectorMath.GetConstantVector(1, yvec.Length);
			_fit = new Regression.LinearFitBySvd(xvec, yvec, err, xvec.Length, RegressionOrder + 1, Regression.LinearFitBySvd.GetPolynomialFunctionBase(RegressionOrder), 1E-6);
			return 0;
		}

		public double GetYOfX(double x)
		{
			double[] paras = _fit.Parameter;

			double result = 0;
			for (int i = paras.Length - 1; i >= 0; i--)
			{
				result *= x;
				result += paras[i];
			}
			return result;
		}

		public double GetYOfU(double u)
		{
			return GetYOfX(u);
		}

		public double GetXOfU(double u)
		{
			return u;
		}
	}

	#endregion PolynomialRegressionAsInterpolation

	#region FritschCarlsonCubicSpline

	/// <summary><para>
	/// Calculate the Fritsch-Carlson monotone cubic spline interpolation for the
	/// given abscissa vector x and ordinate vector y.
	/// All vectors must have conformant dimenions.
	/// The abscissa vector must be strictly increasing.
	/// </para>
	/// <para>
	/// The Fritsch-Carlson interpolation produces a neat monotone
	/// piecewise cubic curve, which is especially suited for the
	/// presentation of scientific data.
	/// This is the state of the art to create curves that preserve
	/// monotonicity, although it is not so well known as Akima's
	/// interpolation. The commonly used Akima interpolation doesn't
	/// produce so pleasant results.
	/// </para>
	/// <code>
	/// Reference:
	///    F.N.Fritsch,R.E.Carlson: Monotone Piecewise Cubic
	///    Interpolation, SIAM J. Numer. Anal. Vol 17, No. 2,
	///    April 1980
	///
	/// Copyright (C) 1991-1998 by Berndt M. Gammel
	/// Translated to C# by Dirk Lellinger.
	/// </code>
	/// </summary>
	public class FritschCarlsonCubicSpline : CurveBase, IInterpolationFunction
	{
		protected DoubleVector y1 = new DoubleVector();
		protected DoubleVector y2 = new DoubleVector();
		protected DoubleVector y3 = new DoubleVector();

		//----------------------------------------------------------------------------//
		//
		// int MpFritschCarlsonCubicSpline::Interpolate (const Vector &x, const Vector &y)
		//
		// Calculate the Fritsch-Carlson monotone cubic spline interpolation for the
		// given abscissa vector x and ordinate vector y.
		// All vectors must have conformant dimenions.
		// The abscissa vector must be strictly increasing.
		//
		// The Fritsch-Carlson interpolation produces a neat monotone
		// piecewise cubic curve, which is especially suited for the
		// presentation of scientific data.
		// This is the state of the art to create curves that preserve
		// monotonicity, although it is not so well known as Akima's
		// interpolation. The commonly used Akima interpolation doesn't
		// produce so pleasant results.
		//
		// Reference:
		//    F.N.Fritsch,R.E.Carlson: Monotone Piecewise Cubic
		//    Interpolation, SIAM J. Numer. Anal. Vol 17, No. 2,
		//    April 1980
		//
		// Copyright (C) 1991-1998 by Berndt M. Gammel
		//
		//----------------------------------------------------------------------------//

		public override int Interpolate(IROVector x, IROVector y)
		{
			// check input parameters

			if (!MatchingIndexRange(x, y))
				throw new ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;

			// Empty data vectors - free auxilliary storage
			if (x.Length == 0)
			{
				y1.Clear();
				y2.Clear();
				y3.Clear();
				return 0; // ok
			}

			int hi = x.Length - 1;
			int len = x.Length;

			// Resize the auxilliary vectors. Note, that there is no reallocation if the
			// vector already has the appropriate dimension.
			y1.Resize(len);
			y2.Resize(len);
			y3.Resize(len);

			if (x.Length == 1)
			{
				// default derivative is 0.0
				y1[0] = y2[0] = y3[0] = 0.0;
			}
			else if (x.Length == 2)
			{
				// set derivatives for a line
				y1[0] = y1[hi] = (y[hi] - y[0]) / (x[hi] - x[0]);
				y2[0] = y2[hi] =
					y3[0] = y3[hi] = 0.0;
			}
			else
			{ // three or more points
				// initial guess derivative vector
				y1[0] = deriv1(x, y, 1, -1);
				y1[hi] = deriv1(x, y, hi - 1, 1);
				for (int i = 1; i < hi; i++)
					y1[i] = deriv2(x, y, i);

				if (x.Length > 3)
				{
					// adjust derivatives at boundaries
					if (y1[0] * y1[1] < 0) y1[0] = 0;
					if (y1[hi] * y1[hi - 1] < 0) y1[hi] = 0;

					// adjustment of cubic interpolant
					fritsch(x, y, y1);
				}

				// calculate remaining spline coefficients y2(i) and y3(i)
				CubicSplineCoefficients(x, y, y1, y2, y3);
			}

			return 0; // ok
		}

		public override double GetXOfU(double u)
		{
			return u;
		}

		public override double GetYOfU(double u)
		{
			return CubicSplineHorner(u, x, y, y1, y2, y3);
		}

		public double GetYOfX(double u)
		{
			return CubicSplineHorner(u, x, y, y1, y2, y3);
		}

		public double GetY1stDerivativeOfX(double u)
		{
			return CubicSplineHorner1stDerivative(u, x, y, y1, y2, y3);
		}

		#region deriv1

		//-----------------------------------------------------------------------------//
		//
		// Initial derivatives at boundaries of data set using
		// quadratic Newton interpolation
		//
		//-----------------------------------------------------------------------------//

		private static double deriv1(IROVector x, IROVector y, int i, int sgn)
		{
			double di, dis, di2, his;
			int i1, i2;

			i1 = i + 1;
			i2 = i - 1;
			his = x[i1] - x[i2];
			dis = (y[i1] - y[i2]) / his;
			di = (y[i1] - y[i]) / (x[i1] - x[i]);
			di2 = (di - dis) / (x[i] - x[i2]);
			return dis + sgn * di2 * his;
		}

		#endregion deriv1

		//-----------------------------------------------------------------------------//
		//
		// Initial derivatives within data set using
		// quadratic Newton interpolation
		//
		//-----------------------------------------------------------------------------//

		private static double deriv2(IROVector x, IROVector y, int i)
		{
			double di0, di1, di2, hi0;
			int i1, i2;

			i1 = i + 1;
			i2 = i - 1;
			hi0 = x[i] - x[i2];
			di0 = (y[i] - y[i2]) / hi0;
			di1 = (y[i1] - y[i]) / (x[i1] - x[i]);
			di2 = (di1 - di0) / (x[i1] - x[i2]);
			return di0 + di2 * hi0;
		}

		//-----------------------------------------------------------------------------//
		//
		// Fritsch-Carlson iteration to adjust the monotone
		// cubic interpolant. The iteration converges with cubic order.
		//
		//-----------------------------------------------------------------------------//

		private static void fritsch(IROVector x, IROVector y, IVector d)
		{
			int i, i1;
			bool stop;
			double d1, r2, t;

			const int max_loop = 20; // should never happen! Note, that currently it
			// can happen when the curve is not strictly
			// monotone. In future this case should be handled
			// more gracefully without wasting CPU time.
			int loop = 0;

			do
			{
				stop = true;
				int hi = x.Length - 1;
				for (i = 0; i < hi; i++)
				{
					i1 = i + 1;
					d1 = (y[i1] - y[i]) / (x[i1] - x[i]);
					if (d1 == 0.0)
						d[i] = d[i1] = 0.0;
					else
					{
						t = d[i] / d1;
						r2 = t * t;
						t = d[i1] / d1;
						r2 += t * t;
						if (r2 > 9.0)
						{
							t = 3.0 / Math.Sqrt(r2);
							d[i] *= t;
							d[i1] *= t;
							stop = false;
						}
					}
				}
			} while (!stop && ++loop < max_loop);
		}
	}

	#endregion FritschCarlsonCubicSpline

	#region AkimaCubicSpline

	/// <summary>
	/// Akima cubic spline interpolation for the given abscissa
	/// vector x and ordinate vector y.
	/// All vectors must have conformant dimenions.
	/// The abscissa vector must be strictly increasing.
	/// </summary>
	public class AkimaCubicSpline : CurveBase, IInterpolationFunction
	{
		protected DoubleVector y1 = new DoubleVector();
		protected DoubleVector y2 = new DoubleVector();
		protected DoubleVector y3 = new DoubleVector();

		private double m(int i)
		{
			return ((y[i + 1] - y[i]) / (x[i + 1] - x[i]));
		}

		//----------------------------------------------------------------------------//
		//
		// int MpAkimaCubicSpline::Interpolate (const Vector &x, const Vector &y)
		//
		// Calculate the Akima cubic spline interpolation for the given abscissa
		// vector x and ordinate vector y.
		// All vectors must have conformant dimenions.
		// The abscissa vector must be strictly increasing.
		//----------------------------------------------------------------------------//

		public override int Interpolate(IROVector x, IROVector y)
		{
			// check input parameters

			if (!MatchingIndexRange(x, y))
				throw new ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;

			// Empty data vectors - free auxilliary storage
			if (x.Length == 0)
			{
				y1.Clear();
				y2.Clear();
				y3.Clear();
				return 0; // ok
			}

			const int lo = 0, lo1 = lo + 1, lo2 = lo + 2;
			int hi = x.Length - 1, hi1 = hi - 1, hi2 = hi - 2, hi3 = hi - 3;

			// Resize the auxilliary vectors. Note, that there is no reallocation if the
			// vectors already have the appropriate dimensions.
			y1.Resize(x.Length);
			y2.Resize(x.Length);
			y3.Resize(x.Length);

			if (x.Length == 1)
			{
				// default derivatives are 0.0
				y1[lo] = y2[lo] = y3[lo] = 0.0;
			}
			else if (x.Length == 2)
			{
				// set derivatives for a line
				y1[lo] = y1[hi] = (y[hi] - y[lo]) / (x[hi] - x[lo]);
				y2[lo] = y2[hi] =
					y3[lo] = y3[hi] = 0.0;
			}
			else
			{ // three or more elements - do Akima interpolation
				double num, den,
					m_m1, m_m2, m_p1, m_p2,
					x_m1, x_m2, x_p1, x_p2,
					y_m1, y_m2, y_p1, y_p2;

				// short form to save some typing
				// #define m(i) ((y(i+1)-y(i)) / (x(i+1)-x(i)))

				// interpolate the missing points
				x_m1 = x[lo] + x[lo1] - x[lo2];
				y_m1 = (x[lo] - x_m1) * (m(lo1) - 2 * m(lo)) + y[lo];
				m_m1 = (y[lo] - y_m1) / (x[lo] - x_m1);

				x_m2 = 2 * x[lo] - x[lo2];
				y_m2 = (x_m1 - x_m2) * (m(lo) - 2 * m_m1) + y_m1;
				m_m2 = (y_m1 - y_m2) / (x_m1 - x_m2);

				x_p1 = x[hi] + x[hi1] - x[hi2];
				y_p1 = (2 * m(hi1) - m(hi2)) * (x_p1 - x[hi]) + y[hi];
				m_p1 = (y_p1 - y[hi]) / (x_p1 - x[hi]);

				x_p2 = 2 * x[hi] - x[hi2];
				y_p2 = (2 * m_p1 - m(hi1)) * (x_p2 - x_p1) + y_p1;
				m_p2 = (y_p2 - y_p1) / (x_p2 - x_p1);

				// i = 0
				num = Math.Abs(m(lo1) - m(lo)) * m_m1 + Math.Abs(m_m1 - m_m2) * m(lo);
				den = Math.Abs(m(lo1) - m(lo)) + Math.Abs(m_m1 - m_m2);
				y1[lo] = (den != 0.0) ? num / den : 0.0;

				// i = 1
				if (x.Length > 3)
				{
					num = Math.Abs(m(lo2) - m(lo1)) * m(lo) + Math.Abs(m(lo) - m_m1) * m(lo1);
					den = Math.Abs(m(lo2) - m(lo1)) + Math.Abs(m(lo) - m_m1);
					y1[lo1] = (den != 0.0) ? num / den : 0.0;

					for (int i = lo2; i < hi1; i++)
					{
						double mip1 = m(i + 1),
							mi = m(i),
							mim1 = m(i - 1),
							mim2 = m(i - 2);
						num = Math.Abs(mip1 - mi) * mim1 + Math.Abs(mim1 - mim2) * mi;
						den = Math.Abs(mip1 - mi) + Math.Abs(mim1 - mim2);
						y1[i] = (den != 0.0) ? num / den : 0.0;
					}

					// i = n - 2
					num = Math.Abs(m_p1 - m(hi1)) * m(hi2) + Math.Abs(m(hi2) - m(hi3)) * m(hi1);
					den = Math.Abs(m_p1 - m(hi1)) + Math.Abs(m(hi2) - m(hi3));
					y1[hi1] = (den != 0.0) ? num / den : 0.0;
				}
				else
				{ // exactly three elements
					num = m(lo);
					den = (m(lo1) - num) / (x[lo2] - x[lo]);
					y1[lo1] = num + den * (x[lo1] - x[lo]);
				}

				// i = n - 1
				num = Math.Abs(m_p2 - m_p1) * m(hi1) + Math.Abs(m(hi1) - m(hi2)) * m_p1;
				den = Math.Abs(m_p2 - m_p1) + Math.Abs(m(hi1) - m(hi2));
				y1[hi] = (den != 0.0) ? num / den : 0.0;

				// calculate remaining spline coefficients y2(i) and y3(i)
				CubicSplineCoefficients(x, y, y1, y2, y3);
			}

			return 0; // ok
		}

		public override double GetXOfU(double u)
		{
			return u;
		}

		public override double GetYOfU(double u)
		{
			return CubicSplineHorner(u, x, y, y1, y2, y3);
		}

		public double GetYOfX(double u)
		{
			return CubicSplineHorner(u, x, y, y1, y2, y3);
		}

		public double GetY1stDerivativeOfX(double u)
		{
			return CubicSplineHorner1stDerivative(u, x, y, y1, y2, y3);
		}
	}

	#endregion AkimaCubicSpline

	#region BezierCubicSpline

	/// <summary>
	/// Calculate the Bezier cubic spline interpolation for the
	/// given abscissa vector x and ordinate vector y.
	/// All vectors must have conformant dimensions.
	/// </summary>
	/// <remarks>
	/// <code>
	///
	///
	///                              1   / -1.0  3.0 -3.0  1.0 \   / P1 \
	///  BSpline(t) = (t^3 t^2 t 1) --- |   3.0 -6.0  3.0  0.0  | |  P2  | = T M G
	///                              6  |  -3.0  0.0  3.0  0.0  | |  P3  |
	///                                  \  1.0  4.0  1.0  0.0 /   \ P4 /
	///
	///  T is the polynomial basis vector
	///  M is the basis matrix of the Bezier spline
	///  G is the geometry vector of the control points
	/// </code>
	/// </remarks>
	public class BezierCubicSpline : CurveBase
	{
		public override int Interpolate(IROVector x, IROVector y)
		{
			// verify index range
			if (!MatchingIndexRange(x, y))
				throw new ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;

			return 0; // ok
		}

		public override double GetXOfU(double t)
		{
			int hi = x.Length - 1;
			int i = FindInterval(t, x);

			if (i < 0 || i >= hi || hi == 1)
			{
				// linear extrapolation and interpolation for 2 points
				return t;
			}
			else if (0 == i)
			{
				i = 0;
				double u = (t - x[i]) / (x[i + 1] - x[i]),
					u2 = u * u,
					c1 = 1.0 + u * (u2 / 6.0 - 1.0),
					c2 = u * (1.0 - u2 / 3.0),
					c3 = u * u2 / 6.0;
				return c1 * x[i] + c2 * x[i + 1] + c3 * x[i + 2];
			}
			else if (i == hi - 1)
			{
				i = hi - 1;
				double u = (x[i + 1] - t) / (x[i + 1] - x[i]),
					u2 = u * u,
					c1 = 1.0 + u * (u2 / 6.0 - 1.0),
					c2 = u * (1.0 - u2 / 3.0),
					c3 = u * u2 / 6.0;
				return c1 * x[i + 1] + c2 * x[i] + c3 * x[i - 1];
			}
			else
			{
				double u = (t - x[i]) / (x[i + 1] - x[i]),
					u2 = u * u,
					u3 = 1.0 - u,
					u4 = u3 * u3,
					c1 = u3 * u4 / 6.0,
					c2 = u2 * (u / 2.0 - 1.0) + 4.0 / 6.0,
					c3 = u * (1.0 + u * u3) / 2.0 + 1.0 / 6.0,
					c4 = u * u2 / 6.0;
				return c1 * x[i - 1] + c2 * x[i] + c3 * x[i + 1] + c4 * x[i + 2];
			}
		}

		public override double GetYOfU(double t)
		{
			int hi = x.Length - 1;
			int i = FindInterval(t, x);

			if (i < 0 || hi == 1)
			{
				// linear extrapolation and interpolation for 2 points
				return y[0] + (t - x[0]) * (y[1] - y[0]) / (x[1] - x[0]);
			}
			else if (i == 0)
			{
				i = 0;
				double u = (t - x[i]) / (x[i + 1] - x[i]),
					u2 = u * u,
					c1 = 1.0 + u * (u2 / 6.0 - 1.0),
					c2 = u * (1.0 - u2 / 3.0),
					c3 = u * u2 / 6.0;
				return c1 * y[i] + c2 * y[i + 1] + c3 * y[i + 2];
			}
			else if (i >= hi)
			{
				// linear extrapolation
				return y[hi] + (t - x[hi]) * (y[hi] - y[hi - 1]) / (x[hi] - x[hi - 1]);
			}
			else if (i == hi - 1)
			{
				i = hi - 1;
				double u = (x[i + 1] - t) / (x[i + 1] - x[i]),
					u2 = u * u,
					c1 = 1.0 + u * (u2 / 6.0 - 1.0),
					c2 = u * (1.0 - u2 / 3.0),
					c3 = u * u2 / 6.0;
				return c1 * y[i + 1] + c2 * y[i] + c3 * y[i - 1];
			}
			else
			{
				double u = (t - x[i]) / (x[i + 1] - x[i]),
					u2 = u * u,
					u3 = 1.0 - u,
					u4 = u3 * u3,
					c1 = u3 * u4 / 6.0,
					c2 = u2 * (u / 2.0 - 1.0) + 4.0 / 6.0,
					c3 = u * (1.0 + u * u3) / 2.0 + 1.0 / 6.0,
					c4 = u * u2 / 6.0;
				return c1 * y[i - 1] + c2 * y[i] + c3 * y[i + 1] + c4 * y[i + 2];
			}
		}

		#region Scene drawing

#if IncludeScene

    //----------------------------------------------------------------------------//
//
//  compute a cubic Bezier polynomial for the points
//  (x[0],y[0])...(x[3],y[3]) and plot it from (x[1],y[1]) to (x[2],y[2])
//
//----------------------------------------------------------------------------//

    protected void Draw4 (Scene& scene, const double *x, const double *y, int &first)
{
  double u,u2,u3,u4,c1,c2,c3,c4,rx,ry;
  int n = GetResolution(scene,x[1],y[1],x[2],y[2]);

  for (int i = 0; i <= n; i++) {
    u  = (double) i / n;
    u2 = u*u;
    u3 = 1.0-u;
    u4 = u3*u3;

    c1 = u3*u4/6.0;
    c2 = u2*(u/2.0-1.0)+4.0/6.0;
    c3 = u*(1.0+u*u3)/2.0+1.0/6.0;
    c4 = u*u2/6.0;

    rx = c1*x[0]+c2*x[1]+c3*x[2]+c4*x[3];
    ry = c1*y[0]+c2*y[1]+c3*y[2]+c4*y[3];
    if (first) {
      scene.MoveTo(rx,ry);
      first = false;
    } else
      scene.LineTo(rx,ry);
  }
}

//----------------------------------------------------------------------------//
//
// void MpBezierCubicSpline::DrawClosedCurve (Scene &scene)
//
// Special method to draw a closed curve
//
//----------------------------------------------------------------------------//

void DrawClosedCurve (Scene &scene)
{
  if ( ! scene.IsOpen() )
    Matpack.Error("MpBezierCubicSpline::DrawClosedCurve: "
      "scene is not open for drawing");

  // get index range
  int lo = x->Lo(),
      hi = x->Hi();

  // nothing to draw - one point
  if ( lo >= hi ) return;

  // special case - two points
  if ( hi == lo+1 ) {
    scene.Line((*x)(lo),(*y)(lo),(*x)(hi),(*y)(hi));
    return;
  }

  // three or more points

  int first = true;

  double xx[6],yy[6];

  for (int i = 0; i <= 2; i++) {
    xx[i] = (*x)[hi-2+i];
    yy[i] = (*y)[hi-2+i];
  }

  for (int i = 3; i <= 5; i++) {
    xx[i] = (*x)[lo+i-3];
    yy[i] = (*y)[lo+i-3];
  }

  for (int i = lo; i <= hi-3; i++)
    Draw4(scene,&(*x)(i),&(*y)(i),first);

  for (int i = 0; i <= 2; i++)
    Draw4(scene,&xx[i],&yy[i],first);

  // flush graphics buffer
  scene.Flush();
}

#endif

		#endregion Scene drawing
	};

	#endregion BezierCubicSpline

	#region CardinalCubicSpline

	/// <summary>
	/// Calculate the Cardinal cubic spline interpolation for the
	/// given abscissa vector x and ordinate vector y.
	/// All vectors must have conformant dimensions.
	/// </summary>
	/// <remarks>
	/// <code>
	///
	///                              / -0.5  1.5 -1.5  0.5 \   / P1 \
	///  CSpline(t) = (t^3 t^2 t 1) |   1.0 -2.5  2.0 -0.5  | |  P2  | = T M G
	///                             |  -0.5  0.0  0.5  0.0  | |  P3  |
	///                              \  0.0  1.0  0.0  0.0 /   \ P4 /
	///
	///  T is the polynomial basis vector
	///  M is the basis matrix of the Cardinal spline
	///  G is the geometry vector of the control points
	///
	/// </code>
	/// </remarks>
	public class CardinalCubicSpline : CurveBase
	{
		public override int Interpolate(IROVector x, IROVector y)
		{
			// verify index range
			if (!MatchingIndexRange(x, y))
				throw new System.ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;

			return 0; // ok
		}

		//----------------------------------------------------------------------------//

		public override double GetXOfU(double t)
		{
			const int lo = 0;
			int hi = x.Length - 1;
			int i = FindInterval(t, x);

			if (i < lo || i >= hi || hi - lo == 1)
			{
				// linear extrapolation and interpolation for 2 points
				return t;
			}
			else if (i == lo)
			{
				i = lo;
				double u = (t - x[i]) / (x[i + 1] - x[i]),
					u2 = u * u,
					c1 = 1.0 + u * (-1.0 + u * (u - 1.0) / 2.0),
					c2 = u * (1.0 + u * (1.0 - u)),
					c3 = u2 * (u - 1.0) / 2.0;
				return c1 * x[i] + c2 * x[i + 1] + c3 * x[i + 2];
			}
			else if (i == hi - 1)
			{
				i = hi - 1;
				double u = (x[i + 1] - t) / (x[i + 1] - x[i]),
					u2 = u * u,
					c1 = 1.0 + u * (-1.0 + u * (u - 1.0) / 2.0),
					c2 = u * (1.0 + u * (1.0 - u)),
					c3 = u2 * (u - 1.0) / 2.0;
				return c1 * x[i + 1] + c2 * x[i] + c3 * x[i - 1];
			}
			else
			{
				double u = (t - x[i]) / (x[i + 1] - x[i]),
					u2 = u * u,
					u3 = 1.0 - u,
					u4 = u3 * u3,
					c1 = -u4 * u / 2.0,
					c2 = 1.0 + u2 * (3.0 * u - 5.0) / 2.0,
					c3 = u * (1.0 + u * (4.0 - 3.0 * u)) / 2.0,
					c4 = -u2 * u3 / 2.0;
				return c1 * x[i - 1] + c2 * x[i] + c3 * x[i + 1] + c4 * x[i + 2];
			}
		}

		public override double GetYOfU(double t)
		{
			const int lo = 0;
			int hi = x.Length - 1;
			int i = FindInterval(t, x);

			if (i < lo || hi - lo == 1)
			{
				// linear extrapolation and interpolation for 2 points
				return y[lo] + (t - x[lo]) * (y[lo + 1] - y[lo]) / (x[lo + 1] - x[lo]);
			}
			else if (i == lo)
			{
				i = lo;
				double u = (t - x[i]) / (x[i + 1] - x[i]),
					u2 = u * u,
					c1 = 1.0 + u * (-1.0 + u * (u - 1.0) / 2.0),
					c2 = u * (1.0 + u * (1.0 - u)),
					c3 = u2 * (u - 1.0) / 2.0;
				return c1 * y[i] + c2 * y[i + 1] + c3 * y[i + 2];
			}
			else if (i >= hi)
			{
				// linear extrapolation
				return y[hi] + (t - x[hi]) * (y[hi] - y[hi - 1]) / (x[hi] - x[hi - 1]);
			}
			else if (i == hi - 1)
			{
				i = hi - 1;
				double u = (x[i + 1] - t) / (x[i + 1] - x[i]),
					u2 = u * u,
					c1 = 1.0 + u * (-1.0 + u * (u - 1.0) / 2.0),
					c2 = u * (1.0 + u * (1.0 - u)),
					c3 = u2 * (u - 1.0) / 2.0;
				return c1 * y[i + 1] + c2 * y[i] + c3 * y[i - 1];
			}
			else
			{
				double u = (t - x[i]) / (x[i + 1] - x[i]),
					u2 = u * u,
					u3 = 1.0 - u,
					u4 = u3 * u3,
					c1 = -u4 * u / 2.0,
					c2 = 1.0 + u2 * (3.0 * u - 5.0) / 2.0,
					c3 = u * (1.0 + u * (4.0 - 3.0 * u)) / 2.0,
					c4 = -u2 * u3 / 2.0;
				return c1 * y[i - 1] + c2 * y[i] + c3 * y[i + 1] + c4 * y[i + 2];
			}
		}

		#region Scene Drawing

#if IncludeScene
  //----------------------------------------------------------------------------//
//
//  compute a cubic Cardinal polynomial for the points
//  (x[0],y[0])...(x[3],y[3]) and plot it from (x[1],y[1]) to (x[2],y[2])
//
//----------------------------------------------------------------------------//

void MpCardinalCubicSpline::Draw4 (Scene& scene,
           const double *x, const double *y, int &first)
{
  double u,u2,u3,u4,c1,c2,c3,c4,rx,ry;
  int n = GetResolution(scene,x[1],y[1],x[2],y[2]);

  for (int i = 0; i <= n; i++) {
    u  = (double) i / n;
    u2 = u*u;
    u3 = 1.0-u;
    u4 = u3*u3;

    c1 = -u4*u/2.0;
    c2 = 1.0+u2*(3.0*u-5.0)/2.0;
    c3 = u*(1.0+u*(4.0-3.0*u))/2.0;
    c4 = -u2*u3/2.0;

    rx = c1*x[0]+c2*x[1]+c3*x[2]+c4*x[3];
    ry = c1*y[0]+c2*y[1]+c3*y[2]+c4*y[3];
    if (first) {
      scene.MoveTo(rx,ry);
      first = false;
    } else
      scene.LineTo(rx,ry);
  }
}

//----------------------------------------------------------------------------//

void MpCardinalCubicSpline::DrawClosedCurve (Scene &scene)
{
  if ( ! scene.IsOpen() )
    Matpack.Error("MpCardinalCubicSpline::DrawClosedCurve: "
      "scene is not open for drawing");

  // get index range
  int lo = x->Lo(),
      hi = x->Hi();

  // nothing to draw - one point
  if ( lo >= hi ) return;

  // special case - two points
  if ( hi == lo+1 ) {
    scene.Line((*x)(lo),(*y)(lo),(*x)(hi),(*y)(hi));
    return;
  }

  // three or more points

  int first = true;

  double xx[6],yy[6];

  for (int i = 0; i <= 2; i++) {
    xx[i] = (*x)[hi-2+i];
    yy[i] = (*y)[hi-2+i];
  }

  for (int i = 3; i <= 5; i++) {
    xx[i] = (*x)[lo+i-3];
    yy[i] = (*y)[lo+i-3];
  }

  for (int i = lo; i <= hi-3; i++)
    Draw4(scene,&(*x)(i),&(*y)(i),first);

  for (int i = 0; i <= 2; i++)
    Draw4(scene,&xx[i],&yy[i],first);

  // flush graphics buffer
  scene.Flush();
}
#endif

		#endregion Scene Drawing
	}

	#endregion CardinalCubicSpline

	#region RationalCubicSpline

	/// <summary>
	/// This kind of generalized splines give much more pleasent results
	/// than cubic splines when interpolating, e.g., experimental data.
	/// A control parameter p can be used to tune the interpolation smoothly
	/// between cubic splines and a linear interpolation.
	/// But this doesn't mean smoothing of the data - the rational spline curve
	/// will still go through all data points.
	/// </summary>
	/// <remarks>
	/// <code>
	///
	/// The basis functions for rational cubic splines are
	///
	///   g1 = u
	///   g2 = t                     with   t = (x - x(i)) / (x(i+1) - x(i))
	///   g3 = u^3 / (p*t + 1)              u = 1 - t
	///   g4 = t^3 / (p*u + 1)
	///
	/// A rational spline with coefficients a(i),b(i),c(i),d(i) is determined by
	///
	///          f(i)(x) = a(i)*g1 + b(i)*g2 + c(i)*g3 + d(i)*g4
	///
	/// Choosing the smoothing parameter p:
	/// -----------------------------------
	///
	/// Use the method
	///
	///      void MpRationalCubicSpline::SetSmoothing (double smoothing)
	///
	/// to set the value of the smoothing paramenter. A value of p = 0
	/// for the smoothing parameter results in a standard cubic spline.
	/// A value of p with -1 &lt; p &lt; 0 results in "unsmoothing" that means
	/// overshooting oscillations. A value of p with p &gt; 0 gives increasing
	/// smoothness. p to infinity results in a linear interpolation. A value
	/// smaller or equal to -1.0 leads to an error.
	///
	///
	/// Choosing the boundary conditions:
	/// ---------------------------------
	///
	/// Use the method
	///
	///      void MpRationalCubicSpline::SetBoundaryConditions (int boundary,
	///                       double b1, double b2)
	///
	/// to set the boundary conditions. The following values are possible:
	///
	///      Natural
	///          natural boundaries, that means the 2nd derivatives are zero
	///          at both boundaries. This is the default value.
	///
	///      FiniteDifferences
	///          use  finite difference approximation for 1st derivatives.
	///
	///      Supply1stDerivative
	///          user supplied values for 1st derivatives are given in b1 and b2
	///          i.e. f'(x_lo) in b1
	///               f'(x_hi) in b2
	///
	///      Supply2ndDerivative
	///          user supplied values for 2nd derivatives are given in b1 and b2
	///          i.e. f''(x_lo) in b1
	///               f''(x_hi) in b2
	///
	///      Periodic
	///          periodic boundary conditions for periodic curves or functions.
	///          NOT YET IMPLEMENTED IN THIS VERSION.
	///
	///
	/// If the parameters b1,b2 are omitted the default value is 0.0.
	///
	///
	/// Input parameters:
	/// -----------------
	///
	///      Vector x(lo,hi)  The abscissa vector
	///      Vector y(lo,hi)  The ordinata vector
	///                       If the spline is not parametric then the
	///                       abscissa must be strictly monotone increasing
	///                       or decreasing!
	///
	///
	/// References:
	/// -----------
	///   Dr.rer.nat. Helmuth Spaeth,
	///   Spline-Algorithmen zur Konstruktion glatter Kurven und Flaechen,
	///   3. Auflage, R. Oldenburg Verlag, Muenchen, Wien, 1983.
	///
	///
	/// </code>
	/// </remarks>
	public class RationalCubicSpline : CurveBase, IInterpolationFunction
	{
		protected BoundaryConditions boundary;
		protected double p, r1, r2;
		protected DoubleVector dx = new DoubleVector();
		protected DoubleVector dy = new DoubleVector();
		protected DoubleVector a = new DoubleVector();
		protected DoubleVector b = new DoubleVector();
		protected DoubleVector c = new DoubleVector();
		protected DoubleVector d = new DoubleVector();

		//-----------------------------------------------------------------------------//
		//
		// static double deriv1 (const Vector &x, const Vector &y, int i, int sgn)
		//
		// Initial derivatives at boundaries of data set using
		// quadratic Newton interpolation
		//
		//-----------------------------------------------------------------------------//

		private static double deriv1(IROVector x, IROVector y, int i, int sgn)
		{
			if (x.Length <= 1)
				return 0.0;
			else if (x.Length == 2)
				return (y[y.Length - 1] - y[0]) / (x[x.Length - 1] - x[0]);
			else
			{
				int i1 = i + 1,
					i2 = i - 1;
				double his = x[i1] - x[i2],
					dis = (y[i1] - y[i2]) / his,
					di = (y[i1] - y[i]) / (x[i1] - x[i]),
					di2 = (di - dis) / (x[i] - x[i2]);
				return dis + sgn * di2 * his;
			}
		}

		/// <summary>
		/// Set the value of the smoothing paramenter. A value of p = 0
		/// for the smoothing parameter results in a standard cubic spline.
		/// A value of p with -1 &lt; p &lt; 0 results in "unsmoothing" that means
		/// overshooting oscillations. A value of p with p &gt; 0 gives increasing
		/// smoothness. p to infinity results in a linear interpolation. A value
		/// smaller or equal to -1.0 leads to an error.
		/// </summary>
		public double Smoothing
		{
			get
			{
				return p;
			}
			set
			{
				if (value > -1.0)
					p = value;
				else
					throw new ArgumentException("smoothing parameter must be greater than -1.0");
			}
		}

		/// <summary>
		/// Sets the boundary conditions.
		/// </summary>
		/// <param name="bnd"> The boundary condition. See remarks for the possible values.</param>
		/// <param name="b1"></param>
		/// <param name="b2"></param>
		/// <remarks>
		/// <code>
		///      Natural
		///          natural boundaries, that means the 2nd derivatives are zero
		///          at both boundaries. This is the default value.
		///
		///      FiniteDifferences
		///          use  finite difference approximation for 1st derivatives.
		///
		///      Supply1stDerivative
		///          user supplied values for 1st derivatives are given in b1 and b2
		///          i.e. f'(x_lo) in b1
		///               f'(x_hi) in b2
		///
		///      Supply2ndDerivative
		///          user supplied values for 2nd derivatives are given in b1 and b2
		///          i.e. f''(x_lo) in b1
		///               f''(x_hi) in b2
		///
		///      Periodic
		///          periodic boundary conditions for periodic curves or functions.
		///          NOT YET IMPLEMENTED IN THIS VERSION.
		/// </code>
		/// </remarks>
		public void SetBoundaryConditions(
			BoundaryConditions bnd,
			double b1,
			double b2)
		{
			boundary = bnd;
			r1 = b1;
			r2 = b2;
		}

		/// <summary>
		/// Gets the boundary condition and the two condition parameters.
		/// </summary>
		/// <param name="b1">First boundary condition parameter.</param>
		/// <param name="b2">Second boundary condition parameter.</param>
		/// <returns>The boundary condition.</returns>
		public BoundaryConditions GetBoundaryConditions(out double b1, out double b2)
		{
			b1 = r1;
			b2 = r2;
			return boundary;
		}

		/// <summary>
		/// Gets the boundary condition and the two condition parameters.
		/// </summary>
		/// <returns>The boundary condition.</returns>
		public BoundaryConditions GetBoundaryConditions()
		{
			return boundary;
		}

		/// <summary>
		/// Calculate difference vector dx(i) from vector x(i) and
		/// assure that x(i) is strictly monotone increasing or decreasing.
		/// Can be called with both arguments the same vector in order to
		/// do it inplace!
		/// </summary>
		/// <param name="x">Input vector.</param>
		/// <param name="dx">Output vector.</param>
		public static void Differences(IROVector x, IVector dx)
		{
			int sgn;
			double t;

			// get dimensions
			const int lo = 0;
			int hi = x.Length - 1;

			if (hi > lo)
			{
				if ((sgn = Math.Sign(x[lo + 1] - x[lo])) == 0)
					throw new ArgumentException("abscissa is not strictly monotone");

				for (int i = lo; i < hi; i++)
				{
					if (Math.Sign(t = x[i + 1] - x[i]) != sgn)
						throw new System.ArgumentException("abscissa is not strictly monotone");
					dx[i] = t;
				}
			}

			dx[hi] = 0;
		}

		/// <summary>
		/// Calculate inverse difference vector dx(i) from vector x(i) and
		/// assure that x(i) is strictly monotone increasing or decreasing.
		/// Can be called with both arguments the same vector in order to
		/// do it inplace!
		/// </summary>
		/// <param name="x">Input vector.</param>
		/// <param name="dx">Output vector.</param>
		public static void InverseDifferences(IROVector x, IVector dx)
		{
			int sgn;
			double t;

			// get dimensions
			const int lo = 0;
			int hi = x.Length - 1;

			if (hi > lo)
			{
				if ((sgn = Math.Sign(x[lo + 1] - x[lo])) == 0)
					throw new ArgumentException("abscissa is not strictly monotone");

				for (int i = lo; i < hi; i++)
				{
					if (Math.Sign(t = x[i + 1] - x[i]) != sgn)
						throw new ArgumentException("abscissa is not strictly monotone");
					dx[i] = 1.0 / t;
				}
			}

			dx[hi] = 0;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="p">smoothing parameter</param>
		/// <param name="dx">inverse abscissa difference vector</param>
		/// <param name="z">output parameter: coefficient vector SplineB1 and SplineB2</param>
		protected void SplineA(double p, IROVector dx, IVector z)
		{
			double h1, h2, p2;

			// get dimensions
			const int lo = 0;
			int hi = dx.Length - 1;

			// calculate vector z
			z[lo] = 0.0;
			h1 = dx[lo];
			p2 = 2.0 + p;
			for (int j = lo, k = lo + 1; k < hi; j = k++)
			{
				h2 = dx[k];
				z[k] = 1.0 / (p2 * (h1 + h2) - h1 * h1 * z[j]);
				h1 = h2;
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="p">smoothing parameter</param>
		/// <param name="dx">inverse abscissa difference vector</param>
		/// <param name="y">ordinata vector</param>
		/// <param name="y1">Input: 1st derivative vector with elements y1(lo) and y1(hi) supplied by the user.
		/// Output: 1st derivative vector with elements y1(i), i = lo+1...hi-1 calculated newly.</param>
		/// <param name="f">working vector</param>
		/// <param name="z">the coefficients computed by SplineA</param>
		protected void SplineB1(
			double p,
			IROVector dx,
			IROVector y,
			IVector y1, IVector f, IROVector z)
		{
			int j = 0, k;
			double h, h1 = 0.0, h2, r1 = 0.0, r2;

			// get dimensions
			const int lo = 0, lo1 = lo + 1;
			int hi = dx.Length - 1,
			hi1 = hi - 1;

			// calculate the derivative vector y1
			f[lo] = 0.0;
			double p3 = 3.0 + p;
			for (k = lo; k < hi; k++)
			{
				h2 = dx[k];
				r2 = p3 * h2 * h2 * (y[k + 1] - y[k]);
				if (k != lo)
				{
					h = r1 + r2;
					if (k == lo1) h -= h1 * y1[lo];
					if (k == hi1) h -= h2 * y1[hi];
					f[k] = z[k] * (h - h1 * f[j]);
				}
				j = k;
				h1 = h2;
				r1 = r2;
			}
			if (hi - lo < 1) return;
			y1[hi1] = f[hi1];
			int n2 = hi - 2;
			for (j = lo + 1; j <= n2; j++)
			{
				k = hi - j;
				y1[k] = f[k] - z[k] * dx[k] * y1[k + 1];
			}
		}

		//----------------------------------------------------------------------------//
		//
		// void MpRationalCubicSpline::SplineC1 (double p,
		//              const Vector& x, const Vector& dx,
		//              const Vector& y, const Vector& y1,
		//              Vector &a, Vector &b, Vector &c, Vector &d)
		//
		// Calculates the spline coefficients a(i), b(i), c(i), d(i) for a spline
		// with given 1st derivative. It uses the coefficients calculated by
		// SplineA and SplineB1.
		//
		//----------------------------------------------------------------------------//

		protected void SplineC1(double p,
			IROVector x, IROVector dx,
			IROVector y, IROVector y1,
			IVector a, IVector b, IVector c, IVector d)
		{
			// get dimensions
			const int lo = 0;
			int hi = x.Length - 1;

			// auxilliaries
			double p2 = 2.0 + p,
				p3 = 3.0 + p,
				p4 = 1.0 / (p2 * p2 - 1.0);

			// calculate spline coefficients
			for (int i = lo; i < hi; i++)
			{
				double dy = y[i + 1] - y[i];
				c[i] = (p3 * dy - p2 * y1[i] / dx[i] - y1[i + 1] / dx[i]) * p4;
				d[i] = (-p3 * dy + y1[i] / dx[i] + p2 * y1[i + 1] / dx[i]) * p4;
				a[i] = y[i] - c[i];
				b[i] = y[i + 1] - d[i];
			}
			a[hi] = b[hi] = c[hi] = d[hi] = 0.0;
		}

		//----------------------------------------------------------------------------//
		//
		// void MpRationalCubicSpline::SplineB2 (double p,
		//                 const Vector& dx, const Vector& y,
		//                 Vector& y2, Vector& f, const Vector& z)
		//
		//  input paramaters:    p  smoothing parameter
		//                      dx  abscissa difference vector
		//                       y  ordinata vector
		//                       z  the coefficients computed by SplineA
		//                       f  working vector
		//                      y2  2nd derivative vector with elements y2(lo) and y2(hi)
		//                          supplied by the user.
		//
		//  output parameters:  y2  2nd derivative vector with elements
		//                          y2(i), i = lo+1...hi-1 calculated newly
		//
		//----------------------------------------------------------------------------//

		protected void SplineB2(double p,
			IROVector dx, IROVector y,
			IVector y2, IVector f, IROVector z)
		{
			int j = 0, k;
			double h, h1 = 0.0, h2, r1 = 0.0, r2;

			// get dimensions
			const int lo = 0, lo1 = lo + 1;
			int hi = dx.Length - 1,
				hi1 = hi - 1;

			// calculate the derivative vector y2
			f[lo] = 0.0;
			double pp = 2.0 * p * (3.0 + p) + 6.0;
			for (k = lo; k < hi; k++)
			{
				h2 = dx[k];
				r2 = pp * (y[k + 1] - y[k]) / h2;
				if (k != lo)
				{
					h = r2 - r1;
					if (k == lo1) h -= h1 * y2[lo];
					if (k == hi1) h -= h2 * y2[hi];
					f[k] = z[k] * (h - h1 * f[j]);
				}
				j = k;
				h1 = h2;
				r1 = r2;
			}
			if (hi - lo < 1) return;
			y2[hi1] = f[hi1];
			int n2 = hi - 2;
			for (j = lo + 1; j <= n2; j++)
			{
				k = hi - j;
				y2[k] = f[k] - z[k] * dx[k] * y2[k + 1];
			}
		}

		//----------------------------------------------------------------------------//
		//
		// void MpRationalCubicSpline::SplineC2 (double p,
		//                 const Vector& x, const Vector& dx,
		//                 const Vector& y, const Vector& y2,
		//                 Vector &a, Vector &b, Vector &c, Vector &d)
		//
		// Calculates the spline coefficients a(i), b(i), c(i), d(i) for a spline
		// with given 2nd derivative. It uses the coefficients calculated by
		// SplineA and SplineB2.
		//
		//----------------------------------------------------------------------------//

		private void SplineC2(double p,
			IROVector x, IROVector dx,
			IROVector y, IROVector y2,
			IVector a, IVector b, IVector c, IVector d)
		{
			// get dimensions
			const int lo = 0;
			int hi = x.Length - 1;

			// auxilliaries
			double pp = 0.5 / (p * (3.0 + p) + 3.0);

			// calculate spline coefficients
			for (int i = lo; i < hi; i++)
			{
				double h = pp * sqr(dx[i]);
				c[i] = h * y2[i];
				d[i] = h * y2[i + 1];
				a[i] = y[i] - c[i];
				b[i] = y[i + 1] - d[i];
			}
			a[hi] = b[hi] = c[hi] = d[hi] = 0.0;
		}

		public RationalCubicSpline()
		{
			boundary = BoundaryConditions.Natural;
			p = 0.0;
		}

		//----------------------------------------------------------------------------//
		//
		// int MpRationalCubicSpline::Interpolate (const Vector &x, const Vector &y)
		//
		// Rational Cubic Spline Interpolation:
		// ------------------------------------
		//
		// This kind of generalized splines give much more pleasent results
		// than cubic splines when interpolating, e.g., experimental data.
		// A control parameter p can be used to tune the interpolation smoothly
		// between cubic splines and a linear interpolation.
		// But this doesn't mean smoothing of the data - the rational spline curve
		// will still go through all data points.
		//
		// The basis functions for rational cubic splines are
		//
		//   g1 = u
		//   g2 = t                     with   t = (x - x(i)) / (x(i+1) - x(i))
		//   g3 = u^3 / (p*t + 1)              u = 1 - t
		//   g4 = t^3 / (p*u + 1)
		//
		// A rational spline with coefficients a(i),b(i),c(i),d(i) is determined by
		//
		//          f(i)(x) = a(i)*g1 + b(i)*g2 + c(i)*g3 + d(i)*g4
		//
		//
		// Choosing the smoothing parameter p:
		// -----------------------------------
		//
		// Use the method
		//
		//      void MpRationalCubicSpline::SetSmoothing (double smoothing)
		//
		// to set the value of the smoothing paramenter. A value of p = 0
		// for the smoothing parameter results in a standard cubic spline.
		// A value of p with -1 < p < 0 results in "unsmoothing" that means
		// overshooting oscillations. A value of p with p > 0 gives increasing
		// smoothness. p to infinity results in a linear interpolation. A value
		// smaller or equal to -1.0 leads to an error.
		//
		//
		// Choosing the boundary conditions:
		// ---------------------------------
		//
		// Use the method
		//
		//      void MpRationalCubicSpline::SetBoundaryConditions (int boundary,
		//                       double b1, double b2)
		//
		// to set the boundary conditions. The following values are possible:
		//
		//      Natural
		//          natural boundaries, that means the 2nd derivatives are zero
		//          at both boundaries. This is the default value.
		//
		//      FiniteDifferences
		//          use  finite difference approximation for 1st derivatives.
		//
		//      Supply1stDerivative
		//          user supplied values for 1st derivatives are given in b1 and b2
		//          i.e. f'(x_lo) in b1
		//               f'(x_hi) in b2
		//
		//      Supply2ndDerivative
		//          user supplied values for 2nd derivatives are given in b1 and b2
		//          i.e. f''(x_lo) in b1
		//               f''(x_hi) in b2
		//
		//      Periodic
		//          periodic boundary conditions for periodic curves or functions.
		//          NOT YET IMPLEMENTED IN THIS VERSION.
		//
		//
		// If the parameters b1,b2 are omitted the default value is 0.0.
		//
		//
		// Input parameters:
		// -----------------
		//
		//      Vector x(lo,hi)  The abscissa vector
		//      Vector y(lo,hi)  The ordinata vector
		//                       If the spline is not parametric then the
		//                       abscissa must be strictly monotone increasing
		//                       or decreasing!
		//
		//
		// References:
		// -----------
		//   Dr.rer.nat. Helmuth Spaeth,
		//   Spline-Algorithmen zur Konstruktion glatter Kurven und Flaechen,
		//   3. Auflage, R. Oldenburg Verlag, Muenchen, Wien, 1983.
		//
		//
		//----------------------------------------------------------------------------//

		public override int Interpolate(IROVector x, IROVector y)
		{
			// check input parameters

			if (!MatchingIndexRange(x, y))
				throw new ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;

			// Empty data vectors - free auxilliary storage
			if (x.Length == 0)
			{
				dx.Clear();
				dy.Clear();
				a.Clear();
				b.Clear();
				c.Clear();
				d.Clear();
				return 0; // ok
			}

			const int lo = 0;
			int hi = x.Length - 1;

			dx.Resize(x.Length);  // abscissa difference vector
			dy.Resize(x.Length);  // vector of derivatives
			a.Resize(x.Length);   // spline coefficients
			b.Resize(x.Length);   // spline coefficients
			c.Resize(x.Length);   // spline coefficients
			d.Resize(x.Length);   // spline coefficients

			if (boundary == BoundaryConditions.FiniteDifferences || boundary == BoundaryConditions.Supply1stDerivative)
			{
				if (boundary == BoundaryConditions.FiniteDifferences)
				{
					// finite differences (quadratic Newton interpolation)
					dy[lo] = deriv1(x, y, lo + 1, -1);
					dy[hi] = deriv1(x, y, hi - 1, 1);
				}
				else
				{  // the 1st derivatives are supplied by the user
					// user supplied data
					dy[lo] = r1;
					dy[hi] = r2;
				}

				// start the calculation
				InverseDifferences(x, dx);       // inverse difference vector
				SplineA(p, dx, a);      // a used as working vector
				SplineB1(p, dx, y, dy, b, a);    // a and b used as working vector
				SplineC1(p, x, dx, y, dy, a, b, c, d);  // spline coeff. returned in a,b,c,d
			}
			else if (boundary == BoundaryConditions.Natural || boundary == BoundaryConditions.Supply2ndDerivative)
			{
				if (boundary == BoundaryConditions.Natural)
				{
					// 2nd derivatives are zero
					dy[lo] = dy[hi] = 0.0;
				}
				else
				{  // the 2nd derivatives are supplied by the user
					// user supplied data
					dy[lo] = r1;
					dy[hi] = r2;
				}

				// start the calculation
				Differences(x, dx);              // difference vector
				SplineA(p, dx, a);      // a used as working vector
				SplineB2(p, dx, y, dy, b, a);    // a and b used as working vector
				SplineC2(p, x, dx, y, dy, a, b, c, d);  // spline coeff. returned in a,b,c,d
			}
			else if (boundary == BoundaryConditions.Periodic)
			{
				throw new NotImplementedException("PERIODIC BOUNDARIES NOT YET IMPLEMENTED");
			}

			return 0; // ok
		}

		public override double GetXOfU(double u)
		{
			return u;
		}

		public double GetYOfX(double u)
		{
			return GetYOfU(u);
		}

		public override double GetYOfU(double u)
		{
			// special case that there are no data. Return 0.0.
			if (x.Length == 0) return 0.0;

			int i = FindInterval(u, x);

			if (i < 0)
			{     // extrapolation
				i++;
				double dx = u - x[i],
					h = x[i + 1] - x[i],
					y0 = a[i] + c[i],
					y1 = (b[i] - a[i] - (3 + p) * c[i]) / h,
					y2 = 2 * (p * p + 3 * p + 3) * c[i] / (h * h);
				return y0 + dx * (y1 + dx * y2);
			}
			else if (i == x.Length - 1)
			{ // extrapolation
				i--;
				double dx = u - x[i + 1],
					h = x[i + 1] - x[i],
					y0 = b[i] + d[i],
					y1 = (b[i] - a[i] + (3 + p) * d[i]) / h,
					y2 = 2 * (p * p + 3 * p + 3) * d[i] / (h * h);
				return y0 + dx * (y1 + dx * y2);
			}
			else
			{       // interpolation
				double t = (u - x[i]) / (x[i + 1] - x[i]),
					v = 1.0 - t;
				return a[i] * v + b[i] * t + c[i] * v * v * v / (p * t + 1) + d[i] * t * t * t / (p * v + 1);
			}
		}
	}

	#endregion RationalCubicSpline

	#region ExponentialSpline

	/// <summary>
	/// Exponential Splines.
	/// </summary>
	/// <remarks>
	/// <code>
	/// References:
	/// -----------
	/// (1) D.G. Schweikert, "An Interpolation Curve using a Spline in Tension"
	///     J. Math. Physics, 45, pp 312-317 (1966).
	/// (2) Dr.rer.nat. Helmuth Spaeth,
	///     "Spline-Algorithmen zur Konstruktion glatter Kurven und Flaechen",
	///     3. Auflage, R. Oldenburg Verlag, Muenchen, Wien, 1983.
	/// (3) A. K. Cline, Commun. of the ACM, 17, 4, pp 218-223 (Apr 1974).
	/// (4) This algorithm is also implemented in the Unix spline tool by
	///     James R. Van Zandt (jrv@mitre-bedford), 1985.
	/// </code>
	/// </remarks>

	public class ExponentialSpline : CurveBase, IInterpolationFunction
	{
		protected BoundaryConditions boundary;
		protected double sigma, r1, r2;
		protected DoubleVector y1 = new DoubleVector();
		protected DoubleVector tmp = new DoubleVector();

		public ExponentialSpline()
		{
			boundary = BoundaryConditions.FiniteDifferences;
			sigma = 1.0;
		}

		public override int Interpolate(IROVector x, IROVector y)
		{
			// check input parameters

			if (!MatchingIndexRange(x, y))
				throw new ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;

			// Empty data vectors - free auxilliary storage
			if (x.Length == 0)
			{
				y1.Clear();
				tmp.Clear();
				return 0; // ok
			}

			const int lo = 0;
			int hi = x.Length - 1,
				n = x.Length;

			y1.Resize(n);    // spline coefficients

			if (n == 1)
			{
				y1[lo] = 0.0;
				return 0; // ok
			}

			tmp.Resize(n);   // temporary

			double slp1 = 0.0, slpn = 0.0;
			double dels, delx1, delx2, delx12, deln, delnm1, delnn, c1, c2, c3,
				diag1, diag2, diagin, dx1, dx2 = 0.0, exps, sigmap, sinhs, sinhin,
				slpp1, slppn = 0.0, spdiag;

			delx1 = x[lo + 1] - x[lo];
			dx1 = (y[lo + 1] - y[lo]) / delx1;

			slpp1 = dx1;  // to get y1(lo) = 0 in unspecified cases
			if (boundary == BoundaryConditions.Supply1stDerivative)
				slpp1 = slp1;
			else if (n != 2)
			{
				delx2 = x[lo + 2] - x[lo + 1];
				delx12 = x[lo + 2] - x[lo];
				c1 = -(delx12 + delx1) / delx12 / delx1;
				c2 = delx12 / delx1 / delx2;
				c3 = -delx1 / delx12 / delx2;
				slpp1 = c1 * y[lo] + c2 * y[lo + 1] + c3 * y[lo + 2];
			}
			else
			{
				y1[lo] = y1[lo + 1] = 0.0;
			}

			if (boundary == BoundaryConditions.Supply1stDerivative)
				slppn = slpn;
			else if (n != 2)
			{
				deln = x[hi] - x[hi - 1];
				delnm1 = x[hi - 1] - x[hi - 2];
				delnn = x[hi] - x[hi - 2];
				c1 = (delnn + deln) / delnn / deln;
				c2 = -delnn / deln / delnm1;
				c3 = deln / delnn / delnm1;
				slppn = c3 * y[hi - 2] + c2 * y[hi - 1] + c1 * y[hi];
			}
			else
			{
				y1[lo] = y1[lo + 1] = 0.0;
			}

			// denormalize tension factor
			sigmap = Math.Abs(sigma) * (n - 1) / (x[hi] - x[lo]);

			// set up right hand side and tridiagonal system for y1 and perform forward
			// elimination
			dels = sigmap * delx1;
			exps = Math.Exp(dels);
			sinhs = 0.5 * (exps - 1.0 / exps);
			sinhin = 1.0 / (delx1 * sinhs);
			diag1 = sinhin * (dels * 0.5 * (exps + 1.0 / exps) - sinhs);
			diagin = 1.0 / diag1;
			y1[lo] = diagin * (dx1 - slpp1);
			spdiag = sinhin * (sinhs - dels);
			tmp[lo] = diagin * spdiag;
			if (n != 2)
			{
				for (int i = lo + 1; i <= hi - 1; i++)
				{
					delx2 = x[i + 1] - x[i];
					dx2 = (y[i + 1] - y[i]) / delx2;
					dels = sigmap * delx2;
					exps = Math.Exp(dels);
					sinhs = 0.5 * (exps - 1.0 / exps);
					sinhin = 1.0 / (delx2 * sinhs);
					diag2 = sinhin * (dels * (0.5 * (exps + 1.0 / exps)) - sinhs);
					diagin = 1.0 / (diag1 + diag2 - spdiag * tmp[i - 1]);
					y1[i] = diagin * (dx2 - dx1 - spdiag * y1[i - 1]);
					spdiag = sinhin * (sinhs - dels);
					tmp[i] = diagin * spdiag;
					dx1 = dx2;
					diag1 = diag2;
				}
			}

			diagin = 1.0 / (diag1 - spdiag * tmp[hi - 1]);

			// the expression below does not make sense if n == 2
			if (n != 2)
				y1[hi] = diagin * (slppn - dx2 - spdiag * y1[hi - 1]);

			// perform back substitution
			for (int i = hi - 1; i >= lo; i--)
				y1[i] -= tmp[i] * y1[i + 1];

			return 0; // ok
		}

		public override double GetXOfU(double u)
		{
			return u;
		}

		public double GetYOfX(double x)
		{
			return GetYOfU(x);
		}

		public override double GetYOfU(double u)
		{
			const int lo = 0;
			int hi = x.Length - 1,
				n = x.Length,
				i = lo + 1;

			// special cases
			if (n == 1) return y[lo];

			// search for x(i-1) <= u < x(i), lo < i <= hi
			while (i < hi && u >= x[i]) i++;
			while (i > lo + 1 && x[i - 1] > u) i--;

			double sigmap = Math.Abs(sigma) * (n - 1) / (x[hi] - x[lo]),
				del1 = u - x[i - 1],
				del2 = x[i] - u,
				dels = x[i] - x[i - 1],
				exps1 = Math.Exp(sigmap * del1),
				sinhd1 = 0.5 * (exps1 - 1.0 / exps1),
				exps = Math.Exp(sigmap * del2),
				sinhd2 = 0.5 * (exps - 1.0 / exps),
				exps2 = exps1 * exps,
				sinhs = 0.5 * (exps2 - 1.0 / exps2);

			return (y1[i] * sinhd1 + y1[i - 1] * sinhd2) / sinhs +
				((y[i] - y1[i]) * del1 + (y[i - 1] - y1[i - 1]) * del2) / dels;
		}

		public double Smoothing
		{
			get
			{
				return sigma;
			}
			set
			{
				if (value > 0.0)
					sigma = value;
				else
					throw new ArgumentException("smoothing parameter must be greater than 0.0");
			}
		}

		public BoundaryConditions BoundaryCondition
		{
			get { return boundary; }
			set { SetBoundaryConditions(value, 0, 0); }
		}

		public void SetBoundaryConditions(BoundaryConditions bnd, double b1, double b2)
		{
			// check boundary conditions argument
			if (bnd == BoundaryConditions.Supply1stDerivative || bnd == BoundaryConditions.FiniteDifferences)
			{
				boundary = bnd;
				r1 = b1;
				r2 = b2;
			}
			else
				throw new ArgumentException("Only Supply1stDerivative or FiniteDifferences boundary conditions");
		}

		public BoundaryConditions GetBoundaryConditions(out double b1, out double b2)
		{
			b1 = r1;
			b2 = r2;
			return boundary;
		}
	}

	#endregion ExponentialSpline

	#region PolynomialInterpolation

	public class PolynomialInterpolation : CurveBase, IInterpolationFunction
	{
		protected DoubleVector C = new DoubleVector();
		protected DoubleVector D = new DoubleVector();

		//----------------------------------------------------------------------------//
		//
		// int MpPolynomialInterpolation::Interpolate (const Vector &x, const Vector &y)
		//
		//
		//----------------------------------------------------------------------------//

		public override int Interpolate(IROVector x, IROVector y)
		{
			// check input parameters

			if (!MatchingIndexRange(x, y))
				throw new ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;

			// Empty data vectors - free auxilliary storage
			if (x.Length == 0)
			{
				C.Clear();
				D.Clear();
			}

			return 0; // ok
		}

		public override double GetXOfU(double u)
		{
			return u;
		}

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
		// the poynomial y = P(x) of degree n = hi-lo
		// that interpolates the data points (x(i),y(i)), lo <= i <= hi.
		// In the special case of empty data vectors (x,y) a value of 0.0 is returned.
		//
		//----------------------------------------------------------------------------//

		public override double GetYOfU(double u)
		{
			// special case that there are no data. Return 0.0.
			if (x.Length == 0) return 0.0;

			const int lo = 0;
			int hi = x.Length - 1;

			// allocate (resize) auxilliary vectors - the resize method has the property
			// that no (de-)allocation is done, if the size of the vector is not changed.
			// Thus there is no overhead if GetYOfU() is called many times with the
			// same vectors, for instance, if a whole curve is drawn.
			C.Resize(x.Length);
			D.Resize(x.Length);

			C.CopyFrom(y);        // initialize // TODO original was C = D = *y; check Vector if this is a copy operation
			D.CopyFrom(y);

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

	#endregion PolynomialInterpolation

	#region RationalInterpolation

	public class RationalInterpolation : CurveBase, IInterpolationFunction
	{
		protected DoubleVector xr = new DoubleVector();
		protected DoubleVector yr = new DoubleVector();
		protected IntegerVector m = new IntegerVector();
		protected int num;
		protected double epsilon;

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
		//  this function.  Use function SetPrecision (double eps) for this purpose.
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
		//   1  Interpolation function doesn't exist. You should  try a numerator
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
		//   H. Werner, A reliable and Numerically Stable Program for Rational
		//   Interpolation of Lagrange Data, Computing, Vol 31, 269 (1983).
		//
		//   H. Werner, R. Schaback, Praktische Mathematik II, Springer,
		//   Berlin, Heidelberg, New York, 1972, 2. Aufl. 1979.
		//
		//----------------------------------------------------------------------------//

		public override int Interpolate(IROVector x, IROVector y)
		{
			// check input parameters

			if (!MatchingIndexRange(x, y))
				throw new ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;

			// Empty data vectors - free auxilliary storage
			if (x.Length == 0)
			{
				xr.Clear();
				yr.Clear();
				m.Clear();
				return 0;
			}

			const int lo = 0;
			int hi = x.Length - 1;

			xr.Resize(x.Length);
			yr.Resize(x.Length);
			m.Resize(x.Length);

			int i, j, j1, denom, nend;
			double xj, yj, x2, y2;

			int n = x.Length - 1;

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
			DoubleVector z = new DoubleVector(lo, hi);

			// copy original values
			xr.CopyFrom(x);
			yr.CopyFrom(y);

			// initialize M to 1
			m.SetAllElementsTo(1);

			nend = hi;
			denom = n - num; // degree of denominator polynomial

			if (num < denom)
			{
				for (i = lo; i <= hi; i++)
				{
					if (yr[i] != 0.0)
						yr[i] = 1.0 / yr[i];
					else
						return 1; // interpolation function doesn't exist
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
					return 2;

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
						return 3; // degree of denominator polynomial < 0
				}
			}

			y2 = Math.Abs(yr[hi]);
			for (i = lo; i < hi; i++)
				y2 += Math.Abs(yr[i]);
			for (i = lo; i <= hi; i++)
			{
				x2 = this.GetYOfU(x[i]);
				if (Math.Abs(x2 - y[i]) > n * epsilon * y2)
					return 4; // not all points have been used
			}

			return 0; // ok
		}

		public override double GetXOfU(double u)
		{
			return u;
		}

		public double GetYOfX(double x)
		{
			return GetYOfU(x);
		}

		private static readonly double RootMax = Math.Sqrt(double.MaxValue);

		public override double GetYOfU(double u)
		{
			const double SquareEps = DBL_EPSILON * DBL_EPSILON;

			const int lo = 0;
			int hi = yr.Length - 1;

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

		public double Precision
		{
			set { epsilon = value; }
			get { return epsilon; }
		}

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

		private static void ymin(int nend, out double xj, out double yj, IVector x, IVector y)
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
			x[j] = x[nend]; x[nend] = xj;
			y[j] = y[nend]; y[nend] = yj;
		}
	}

	#endregion RationalInterpolation
}