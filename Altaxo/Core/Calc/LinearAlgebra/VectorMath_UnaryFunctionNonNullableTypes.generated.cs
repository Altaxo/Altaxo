#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.LinearAlgebra
{
  // Unary functions not returning a vector, valid for all vector types

  public static partial class VectorMath
  {

// ******************************************* Unary functions not returning a vector, valid for all non-null vector types  ********************

// ******************************************** Definitions for double[] *******************************************



		/// <summary>
    /// Determines whether the given <paramref name="vector"/> contains any elements.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>True if the <paramref name="vector"/> contains any element; false otherwise, or if the vector is null.</returns>
    public static bool Any(this double[] vector)
    {
      return vector is not null && vector.Length > 0;
    }

    /// <summary>
    /// Determines whether any element of the <paramref name="vector"/> satisfies a condition.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="predicate">The condition to satisfy.</param>
    /// <returns>True if any element of the <paramref name="vector"/> satisfies the condition; false otherwise, or if the vector is null.</returns>
    public static bool Any(this double[] vector, Func<double, bool> predicate)
    {
      if (vector is not null)
      {
        for (int i = 0; i < vector.Length; ++i)
        {
          if (predicate(vector[i]))
            return true;
        }
      }
      return false;
    }

		 /// <summary>
    /// Determines whether any element of the <paramref name="vector"/> satisfies a condition.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="predicate">The condition to satisfy.</param>
    /// <param name="atIndex">The index of the first element that satisfied the condition; or -1 if no element satisfied the condition.</param>
    /// <returns>True if any element of the <paramref name="vector"/> satisfies the condition; false otherwise, or if the vector is null.</returns>
    public static bool Any(this double[] vector, Func<double, bool> predicate, out int atIndex)
    {
      if (vector is not null)
      {
        for (int i = 0; i < vector.Length; ++i)
        {
          if (predicate(vector[i]))
            {
						atIndex = i;
						return true;
						}
        }
      }
			atIndex = -1;
      return false;
    }

    /// <summary>Return the index of the first element with the maximum  value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the maximum value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMaxValue(this double[] vector)
    {
      int index = -1;
			int i;
      double max = double.NegativeInfinity;
      for (i = 0; i < vector.Length; ++i)
      {
        var test = vector[i];
        if (test >= max) // first, make the test for greater than or equal
        {
          index = i;
          max = test;
					break;
        }
      }
      for (++i; i < vector.Length; ++i)
      {
        var test = vector[i];
        if (test > max) // then, consider only values greater than
        {
          index = i;
          max = test;
        }
      }

      return index;
    }

		 /// <summary>Return the index of the first element with the maximum absolute value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the maximum absolute value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMaxAbsoluteValue(this double[] vector)
    {
      int index = -1;
			int i;
      var max = double.NegativeInfinity;
      for (i = 0; i < vector.Length; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test >= max) // first, make the test for greater than or equal
        {
          index = i;
          max = test;
					break;
        }
      }
      for (++i; i < vector.Length; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test > max) // then, consider only values greater than
        {
          index = i;
          max = test;
        }
      }

      return index;
    }

		/// <summary>Return the index of the first element with the minimum value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the minimum value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMinValue(this double[] vector)
    {
      int index = -1;
      int i;
      double min = double.PositiveInfinity;
      for (i = 0; i < vector.Length; ++i)
      {
        var test = vector[i];
        if (test <= min) // first, make the test for less than or equal
        {
          index = i;
          min = test;
					break;
        }
      }
      for (++i; i < vector.Length; ++i)
      {
        var test = vector[i];
        if (test < min) // then, consider only values less than
        {
          index = i;
          min = test;
        }
      }

      return index;
    }

		/// <summary>Return the index of the first element with the minimum absolute value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the minimum absolute value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMinAbsoluteValue(this double[] vector)
    {
      int index = -1;
      int i;
      var min = double.PositiveInfinity;
      for (i = 0; i < vector.Length; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test <= min) // first, make the test for less than or equal
        {
          index = i;
          min = test;
					break;
        }
      }
      for (++i; i < vector.Length; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test < min) // then, consider only values less than
        {
          index = i;
          min = test;
        }
      }

      return index;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing or decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <param name="isDecreasing">On return, this argument is set to true if the sequence is strictly decreasing. If increasing, this argument is set to false.</param>
    /// <returns>True if the sequence is strictly increasing or decreasing.</returns>
    public static bool IsStrictlyIncreasingOrDecreasing(this double[] vector, out bool isDecreasing)
    {
      isDecreasing = false;
      if (vector.Length == 0)
        return false;
      int sign = Math.Sign(vector[vector.Length - 1] - vector[0]);
      if (sign == 0)
        return false;

      isDecreasing = (sign < 0);

      for (int i = vector.Length - 1; i >= 1; --i)
        if (Math.Sign(vector[i] - vector[i - 1]) != sign)
          return false;

      return true;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing or decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly increasing or decreasing.</returns>
    public static bool IsStrictlyIncreasingOrDecreasing(this double[] vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing);
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly decreasing.</returns>
    public static bool IsStrictlyDecreasing(this double[] vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing) && isDecreasing;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly increasing.</returns>
    public static bool IsStrictlyIncreasing(this double[] vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing) && !isDecreasing;
    }


		/// <summary>
    /// Calculates the L1 norm of the vector (as the sum of the absolute values of the elements).
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>L1 norm of the vector (sum of the absolute values of the elements).</returns>
    public static double L1Norm(this double[] vector)
    {
      double sum = 0;
      for (int i = 0; i < vector.Length; ++i)
        sum += Math.Abs(vector[i]);

      return sum;
    }


		/// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
		/// <param name="vector">An input array of length n. </param>
		/// <param name="startIndex">The index of the first element in x to process.</param>
		/// <param name="count">A positive integer input variable of the number of elements to process.</param>
		/// <returns>The euclidian norm of the vector of length n, i.e. the square root of the sum of squares of the elements.</returns>
		/// <remarks>
		///     the euclidean norm is computed by accumulating the sum of
		///     squares in three different sums. the sums of squares for the
		///     small and large components are scaled so that no overflows
		///     occur. non-destructive underflows are permitted. underflows
		///     and overflows do not occur in the computation of the unscaled
		///     sum of squares for the intermediate components.
		///     the definitions of small, intermediate and large components
		///     depend on two constants, rdwarf and rgiant. the main
		///     restrictions on these constants are that rdwarf**2 not
		///     underflow and rgiant**2 not overflow. the constants
		///     given here are suitable for every known computer.
		///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
		/// </remarks>
		public static double L2Norm(this double[] vector, int startIndex, int count)
    {
      double sqr(double v) => (v * v);
      const double rdwarf = 3.834e-20;
      const double rgiant = 1.304e19;

      double ret_val = 0;
      double xabs;
      double x1max = 0, x3max = 0, s1 = 0, s2 = 0, s3 = 0;
      var agiant = rgiant / (double)count;

      for (int i = 0; i < count; ++i)
      {
        xabs = Math.Abs((double)vector[i + startIndex]);

        if (xabs > rgiant)
        {
          //sum for large components
          if (xabs <= x1max)
          {
            s1 += sqr(xabs / x1max);
          }
          else
          {
            s1 = 1 + s1 * sqr(x1max / xabs);
            x1max = xabs;
          }
        }
        else if (xabs <= rdwarf)
        {
          // sum for small components
          if (xabs <= x3max)
          {
            if (xabs != 0)
              s3 += sqr(xabs / x3max);
          }
          else
          {
            s3 = 1 + s3 * sqr(x3max / xabs);
            x3max = xabs;
          }
        }
        else
        {
          // sum for intermediate components
          s2 += sqr(xabs);
        }
      }

      // calculation of norm
      if (s1 == 0)
      {
        if (s2 == 0)
          ret_val = x3max * Math.Sqrt(s3);
        else if (s2 >= x3max)
          ret_val = Math.Sqrt(s2 * (1.0 + x3max / s2 * (x3max * s3)));
        else
          ret_val = Math.Sqrt(x3max * (s2 / x3max + x3max * s3));
      }
      else
      {
        ret_val = x1max * Math.Sqrt(s1 + s2 / x1max / x1max);
      }

      return ret_val;
    }

		/// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
    /// <param name="vector">An input array of length n. </param>
    /// <returns>The euclidian norm of the vector, i.e. the square root of the sum of squares of the elements.</returns>
    /// <remarks>
    ///     the euclidean norm is computed by accumulating the sum of
    ///     squares in three different sums. the sums of squares for the
    ///     small and large components are scaled so that no overflows
    ///     occur. non-destructive underflows are permitted. underflows
    ///     and overflows do not occur in the computation of the unscaled
    ///     sum of squares for the intermediate components.
    ///     the definitions of small, intermediate and large components
    ///     depend on two constants, rdwarf and rgiant. the main
    ///     restrictions on these constants are that rdwarf**2 not
    ///     underflow and rgiant**2 not overflow. the constants
    ///     given here are suitable for every known computer.
    ///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
    /// </remarks>
    public static double L2Norm(this double[] vector)
    {
      return L2Norm(vector, 0, vector.Length);
    }


		 /// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
    /// <param name="vector">An input array of length n. </param>
    /// <returns>The euclidian norm of the vector, i.e. the square root of the sum of squares of the elements.</returns>
    /// <remarks>
    ///     the euclidean norm is computed by accumulating the sum of
    ///     squares in three different sums. the sums of squares for the
    ///     small and large components are scaled so that no overflows
    ///     occur. non-destructive underflows are permitted. underflows
    ///     and overflows do not occur in the computation of the unscaled
    ///     sum of squares for the intermediate components.
    ///     the definitions of small, intermediate and large components
    ///     depend on two constants, rdwarf and rgiant. the main
    ///     restrictions on these constants are that rdwarf**2 not
    ///     underflow and rgiant**2 not overflow. the constants
    ///     given here are suitable for every known computer.
    ///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
    /// </remarks>
    public static double EuclideanNorm(this double[] vector)
    {
      return L2Norm(vector, 0, vector.Length);
    }


		/// <summary>
		/// Returns the L-infinity norm of the provided <paramref name="vector"/> (as is the maximum of the absolute value of the elements). If one
		/// of the elements of the vector is invalid, the return value is also invalid (for the floating point types).
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The L-infinity norm of the provided <paramref name="vector"/> (as is the maximum of the absolute value of the elements).</returns>
		public static double LInfinityNorm(this double[] vector)
		{
			double max = 0;
			int i;
			for (i = vector.Length -1; i>=0; --i)
			{
				var temp = Math.Abs(vector[i]);
				if (!(max>temp)) // if we have a double.NaN here, this expression is true and max <- double.NaN
				{
					max = temp;

					if(double.IsNaN(max))
						break;

				}
			}
			
			return max;
		}

			/// <summary>
		/// Returns the L-infinity norm of the difference of <paramref name="vector1"/> and <paramref name="vector2"/> (as is the maximum of the absolute value of the differences of the elements). If one
		/// of the elements of the vector is invalid, the return value is also invalid (for the floating point types).
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The second vector. Must have same length as the first vector.</param>
		/// <returns>The L-infinity norm of the element-wise differences of the provided vectors (as is the maximum of the absolute value of the differences).</returns>
		public static double LInfinityNorm(double[] vector1, double[] vector2)
		{
			if( vector1.Length != vector2.Length)
				throw new RankException("Length of vector 1 must match length of vector 2");

			double max = 0;
			int i;
			for (i = vector1.Length - 1; i >= 0; --i)
			{
				var temp = Math.Abs(vector1[i] - vector2[i]);
				if (!(max>temp)) // if we have a double.NaN here, this expression is true and max <- double.NaN
				{
					max = temp;

					if(double.IsNaN(max))
						break;

				}
			}
			
			return max;
		}

		///<summary>Compute the p Norm of this vector.</summary>
		/// <param name="vector">The vector.</param>
    ///<param name="p">Order of the norm.</param>
		///<returns>The p norm of the vector.</returns>
		///<remarks>p &gt; 0, if p &lt; 0, ABS(p) is used. If p = 0 or positive infinity, the infinity norm is returned.</remarks>
		public static double LpNorm(this double[] vector, double p)
		{
		if (p == 0 )
			{
        return LInfinityNorm(vector);
      }
			else if(p==1)
			{
				return L1Norm(vector);
			}
			else if(p == 2)
			{
				return L2Norm(vector);
			}

			if (p < 0)
			{
				p = -p;
			}
			double ret = 0;
			for (int i = vector.Length-1; i >=0; --i)
			{
				ret += System.Math.Pow(Math.Abs(vector[i]), p);
			}
			return (double)System.Math.Pow(ret, 1 / p);
		}

		


		/// <summary>
		/// Returns the maximum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The maximum of all elements in <paramref name="vector"/>.</returns>
		public static double Max(this double[] vector)
		{
			if (vector.Length == 0)
				throw new ArgumentException("Result undefined for vector of zero length", nameof(vector));

			double max = double.NegativeInfinity;
			double tmp;
			for (int i = 0; i < vector.Length; ++i)
			{
				tmp = vector[i];
				if (tmp > max)
					max = tmp;
			}
			return max;
		}

			/// <summary>
		/// Returns the maximum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="start">First element of the vector to include in the evaluation.</param>
		/// <param name="count">Number of elements of the vector to include in the evaluation.</param>
		/// <returns>The maximum of all elements in <paramref name="vector"/>.</returns>
		public static double Max(this double[] vector, int start, int count)
		{
			if (!(start >= 0))
				throw new ArgumentException("must be >=0", nameof(start));
			if (!(count > 0))
				throw new ArgumentException("Result undefined for count <=0", nameof(count));
			int end = start + count;
			if(!(end<=vector.Length))
				throw new ArgumentException("must be <= vector.count - start", nameof(count));

			double max = double.NegativeInfinity;
			double tmp;
			for (int i = start; i < end; ++i)
			{
				tmp = vector[i];
				if (tmp > max)
					max = tmp;
			}
			return max;
		}

		/// <summary>
		/// Returns the minimum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The minimum of all elements in <paramref name="vector"/>.</returns>
		public static double Min(this double[] vector)
		{
			if (vector.Length == 0)
				throw new ArgumentException("Result undefined for vector of zero length", nameof(vector));

			double min = double.PositiveInfinity;
			double tmp;
			for (int i = 0; i < vector.Length; ++i)
			{
				tmp = vector[i];
				if (tmp < min)
					min = tmp;
			}
			return min;
		}

		/// <summary>
		/// Returns the minimum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="start">First element of the vector to include in the evaluation.</param>
		/// <param name="count">Number of elements of the vector to include in the evaluation.</param>
		/// <returns>The minimum of all elements in <paramref name="vector"/>.</returns>
		public static double Min(this double[] vector, int start, int count)
		{
			if (!(start >= 0))
				throw new ArgumentException("must be >=0", nameof(start));
			if (!(count > 0))
				throw new ArgumentException("Result undefined for count <=0", nameof(count));
			int end = start + count;
			if(!(end<=vector.Length))
				throw new ArgumentException("must be <= vector.count - start", nameof(count));

			double min = double.PositiveInfinity;
			double tmp;
			for (int i = start; i < end; ++i)
			{
				tmp = vector[i];
				if (tmp < min)
					min = tmp;
			}
			return min;
		}


    /// <summary>
    /// Returns the sum of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The sum of all elements in <paramref name="vector"/>.</returns>
    public static double Sum(this double[] vector)
    {
      double sum = 0;
      for (int i = 0; i < vector.Length; ++i)
        sum += vector[i];

      return sum; 
    }


		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The average of all elements in <paramref name="vector"/>.</returns>
    public static double Average(this double[] vector)
    {
      double sum = 0;
      for (int i = 0; i < vector.Length; ++i)
        sum += vector[i];

      return sum / (double)vector.Length; 
    }

		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The average of all elements in <paramref name="vector"/>.</returns>
    public static double Mean(this double[] vector)
    {
			return Average(vector);
		}


		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>, as well as the variance (sum of squares of the mean centered values divided by length of the vector).
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The mean and variance of all elements in <paramref name="vector"/>.</returns>
    public static (double Mean, double Variance) MeanAndVariance(this double[] vector)
    {
			var mean = Mean(vector);

			double sum = 0;
      for (int i = 0; i < vector.Length; ++i)
        {
				var diff = vector[i] - mean;
				sum += (diff*diff);
				}

      return (mean, sum / (double)vector.Length);
		}

		/// <summary>
    /// Returns the kurtosis of the elements in <paramref name="vector"/>. The kurtosis is defined as
		/// kurtosis(X) = E{(X-µ)^4}/((E{(X-µ)²})².
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The kurtosis of the elements in <paramref name="vector"/>.</returns>
		public static double Kurtosis(this double[] vector)
    {
			var N = vector.Length;
			double sum = 0;
      for (int i = N-1; i>=0; --i)
        {
				sum += vector[i];
				}
			var mean = sum/N;

			double sumy2 = 0;
      double sumy4 = 0;
      for (int i = N-1; i>=0; --i)
        {
				var e = vector[i]-mean;
				var e2 = e*e;
				sumy2 += e2;
				sumy4 += e2*e2;
				}


      return N*sumy4/(sumy2*sumy2);
    }

		/// <summary>
    /// Returns the excess kurtosis of the elements in <paramref name="vector"/>. The excess kurtosis is defined as
		/// excesskurtosis(X) = E{X^4} - 3(E{X²})². 
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The excess kurtosis of the elements in <paramref name="vector"/>.</returns>
    public static double ExcessKurtosisOfNormalized(this double[] vector)
    {
      double sumy4 = 0;
			double sumy2 = 0;
      for (int i = 0; i < vector.Length; ++i)
        {
				var e = vector[i];
				var e2 = e*e;
				sumy2 += e2;
				sumy4 += e2*e2;
				}

			var N = vector.Length;

      return sumy4/N -3*RMath.Pow2(sumy2/N); 
    }


// ******************************************** Definitions for float[] *******************************************



		/// <summary>
    /// Determines whether the given <paramref name="vector"/> contains any elements.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>True if the <paramref name="vector"/> contains any element; false otherwise, or if the vector is null.</returns>
    public static bool Any(this float[] vector)
    {
      return vector is not null && vector.Length > 0;
    }

    /// <summary>
    /// Determines whether any element of the <paramref name="vector"/> satisfies a condition.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="predicate">The condition to satisfy.</param>
    /// <returns>True if any element of the <paramref name="vector"/> satisfies the condition; false otherwise, or if the vector is null.</returns>
    public static bool Any(this float[] vector, Func<float, bool> predicate)
    {
      if (vector is not null)
      {
        for (int i = 0; i < vector.Length; ++i)
        {
          if (predicate(vector[i]))
            return true;
        }
      }
      return false;
    }

		 /// <summary>
    /// Determines whether any element of the <paramref name="vector"/> satisfies a condition.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="predicate">The condition to satisfy.</param>
    /// <param name="atIndex">The index of the first element that satisfied the condition; or -1 if no element satisfied the condition.</param>
    /// <returns>True if any element of the <paramref name="vector"/> satisfies the condition; false otherwise, or if the vector is null.</returns>
    public static bool Any(this float[] vector, Func<float, bool> predicate, out int atIndex)
    {
      if (vector is not null)
      {
        for (int i = 0; i < vector.Length; ++i)
        {
          if (predicate(vector[i]))
            {
						atIndex = i;
						return true;
						}
        }
      }
			atIndex = -1;
      return false;
    }

    /// <summary>Return the index of the first element with the maximum  value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the maximum value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMaxValue(this float[] vector)
    {
      int index = -1;
			int i;
      float max = float.NegativeInfinity;
      for (i = 0; i < vector.Length; ++i)
      {
        var test = vector[i];
        if (test >= max) // first, make the test for greater than or equal
        {
          index = i;
          max = test;
					break;
        }
      }
      for (++i; i < vector.Length; ++i)
      {
        var test = vector[i];
        if (test > max) // then, consider only values greater than
        {
          index = i;
          max = test;
        }
      }

      return index;
    }

		 /// <summary>Return the index of the first element with the maximum absolute value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the maximum absolute value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMaxAbsoluteValue(this float[] vector)
    {
      int index = -1;
			int i;
      var max = float.NegativeInfinity;
      for (i = 0; i < vector.Length; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test >= max) // first, make the test for greater than or equal
        {
          index = i;
          max = test;
					break;
        }
      }
      for (++i; i < vector.Length; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test > max) // then, consider only values greater than
        {
          index = i;
          max = test;
        }
      }

      return index;
    }

		/// <summary>Return the index of the first element with the minimum value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the minimum value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMinValue(this float[] vector)
    {
      int index = -1;
      int i;
      float min = float.PositiveInfinity;
      for (i = 0; i < vector.Length; ++i)
      {
        var test = vector[i];
        if (test <= min) // first, make the test for less than or equal
        {
          index = i;
          min = test;
					break;
        }
      }
      for (++i; i < vector.Length; ++i)
      {
        var test = vector[i];
        if (test < min) // then, consider only values less than
        {
          index = i;
          min = test;
        }
      }

      return index;
    }

		/// <summary>Return the index of the first element with the minimum absolute value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the minimum absolute value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMinAbsoluteValue(this float[] vector)
    {
      int index = -1;
      int i;
      var min = float.PositiveInfinity;
      for (i = 0; i < vector.Length; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test <= min) // first, make the test for less than or equal
        {
          index = i;
          min = test;
					break;
        }
      }
      for (++i; i < vector.Length; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test < min) // then, consider only values less than
        {
          index = i;
          min = test;
        }
      }

      return index;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing or decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <param name="isDecreasing">On return, this argument is set to true if the sequence is strictly decreasing. If increasing, this argument is set to false.</param>
    /// <returns>True if the sequence is strictly increasing or decreasing.</returns>
    public static bool IsStrictlyIncreasingOrDecreasing(this float[] vector, out bool isDecreasing)
    {
      isDecreasing = false;
      if (vector.Length == 0)
        return false;
      int sign = Math.Sign(vector[vector.Length - 1] - vector[0]);
      if (sign == 0)
        return false;

      isDecreasing = (sign < 0);

      for (int i = vector.Length - 1; i >= 1; --i)
        if (Math.Sign(vector[i] - vector[i - 1]) != sign)
          return false;

      return true;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing or decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly increasing or decreasing.</returns>
    public static bool IsStrictlyIncreasingOrDecreasing(this float[] vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing);
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly decreasing.</returns>
    public static bool IsStrictlyDecreasing(this float[] vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing) && isDecreasing;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly increasing.</returns>
    public static bool IsStrictlyIncreasing(this float[] vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing) && !isDecreasing;
    }


		/// <summary>
    /// Calculates the L1 norm of the vector (as the sum of the absolute values of the elements).
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>L1 norm of the vector (sum of the absolute values of the elements).</returns>
    public static float L1Norm(this float[] vector)
    {
      float sum = 0;
      for (int i = 0; i < vector.Length; ++i)
        sum += Math.Abs(vector[i]);

      return sum;
    }


		/// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
		/// <param name="vector">An input array of length n. </param>
		/// <param name="startIndex">The index of the first element in x to process.</param>
		/// <param name="count">A positive integer input variable of the number of elements to process.</param>
		/// <returns>The euclidian norm of the vector of length n, i.e. the square root of the sum of squares of the elements.</returns>
		/// <remarks>
		///     the euclidean norm is computed by accumulating the sum of
		///     squares in three different sums. the sums of squares for the
		///     small and large components are scaled so that no overflows
		///     occur. non-destructive underflows are permitted. underflows
		///     and overflows do not occur in the computation of the unscaled
		///     sum of squares for the intermediate components.
		///     the definitions of small, intermediate and large components
		///     depend on two constants, rdwarf and rgiant. the main
		///     restrictions on these constants are that rdwarf**2 not
		///     underflow and rgiant**2 not overflow. the constants
		///     given here are suitable for every known computer.
		///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
		/// </remarks>
		public static double L2Norm(this float[] vector, int startIndex, int count)
    {
      double sqr(double v) => (v * v);
      const double rdwarf = 3.834e-20;
      const double rgiant = 1.304e19;

      double ret_val = 0;
      double xabs;
      double x1max = 0, x3max = 0, s1 = 0, s2 = 0, s3 = 0;
      var agiant = rgiant / (double)count;

      for (int i = 0; i < count; ++i)
      {
        xabs = Math.Abs((double)vector[i + startIndex]);

        if (xabs > rgiant)
        {
          //sum for large components
          if (xabs <= x1max)
          {
            s1 += sqr(xabs / x1max);
          }
          else
          {
            s1 = 1 + s1 * sqr(x1max / xabs);
            x1max = xabs;
          }
        }
        else if (xabs <= rdwarf)
        {
          // sum for small components
          if (xabs <= x3max)
          {
            if (xabs != 0)
              s3 += sqr(xabs / x3max);
          }
          else
          {
            s3 = 1 + s3 * sqr(x3max / xabs);
            x3max = xabs;
          }
        }
        else
        {
          // sum for intermediate components
          s2 += sqr(xabs);
        }
      }

      // calculation of norm
      if (s1 == 0)
      {
        if (s2 == 0)
          ret_val = x3max * Math.Sqrt(s3);
        else if (s2 >= x3max)
          ret_val = Math.Sqrt(s2 * (1.0 + x3max / s2 * (x3max * s3)));
        else
          ret_val = Math.Sqrt(x3max * (s2 / x3max + x3max * s3));
      }
      else
      {
        ret_val = x1max * Math.Sqrt(s1 + s2 / x1max / x1max);
      }

      return ret_val;
    }

		/// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
    /// <param name="vector">An input array of length n. </param>
    /// <returns>The euclidian norm of the vector, i.e. the square root of the sum of squares of the elements.</returns>
    /// <remarks>
    ///     the euclidean norm is computed by accumulating the sum of
    ///     squares in three different sums. the sums of squares for the
    ///     small and large components are scaled so that no overflows
    ///     occur. non-destructive underflows are permitted. underflows
    ///     and overflows do not occur in the computation of the unscaled
    ///     sum of squares for the intermediate components.
    ///     the definitions of small, intermediate and large components
    ///     depend on two constants, rdwarf and rgiant. the main
    ///     restrictions on these constants are that rdwarf**2 not
    ///     underflow and rgiant**2 not overflow. the constants
    ///     given here are suitable for every known computer.
    ///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
    /// </remarks>
    public static double L2Norm(this float[] vector)
    {
      return L2Norm(vector, 0, vector.Length);
    }


		 /// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
    /// <param name="vector">An input array of length n. </param>
    /// <returns>The euclidian norm of the vector, i.e. the square root of the sum of squares of the elements.</returns>
    /// <remarks>
    ///     the euclidean norm is computed by accumulating the sum of
    ///     squares in three different sums. the sums of squares for the
    ///     small and large components are scaled so that no overflows
    ///     occur. non-destructive underflows are permitted. underflows
    ///     and overflows do not occur in the computation of the unscaled
    ///     sum of squares for the intermediate components.
    ///     the definitions of small, intermediate and large components
    ///     depend on two constants, rdwarf and rgiant. the main
    ///     restrictions on these constants are that rdwarf**2 not
    ///     underflow and rgiant**2 not overflow. the constants
    ///     given here are suitable for every known computer.
    ///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
    /// </remarks>
    public static double EuclideanNorm(this float[] vector)
    {
      return L2Norm(vector, 0, vector.Length);
    }


		/// <summary>
		/// Returns the L-infinity norm of the provided <paramref name="vector"/> (as is the maximum of the absolute value of the elements). If one
		/// of the elements of the vector is invalid, the return value is also invalid (for the floating point types).
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The L-infinity norm of the provided <paramref name="vector"/> (as is the maximum of the absolute value of the elements).</returns>
		public static float LInfinityNorm(this float[] vector)
		{
			float max = 0;
			int i;
			for (i = vector.Length -1; i>=0; --i)
			{
				var temp = Math.Abs(vector[i]);
				if (!(max>temp)) // if we have a double.NaN here, this expression is true and max <- double.NaN
				{
					max = temp;

					if(float.IsNaN(max))
						break;

				}
			}
			
			return max;
		}

			/// <summary>
		/// Returns the L-infinity norm of the difference of <paramref name="vector1"/> and <paramref name="vector2"/> (as is the maximum of the absolute value of the differences of the elements). If one
		/// of the elements of the vector is invalid, the return value is also invalid (for the floating point types).
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The second vector. Must have same length as the first vector.</param>
		/// <returns>The L-infinity norm of the element-wise differences of the provided vectors (as is the maximum of the absolute value of the differences).</returns>
		public static float LInfinityNorm(float[] vector1, float[] vector2)
		{
			if( vector1.Length != vector2.Length)
				throw new RankException("Length of vector 1 must match length of vector 2");

			float max = 0;
			int i;
			for (i = vector1.Length - 1; i >= 0; --i)
			{
				var temp = Math.Abs(vector1[i] - vector2[i]);
				if (!(max>temp)) // if we have a double.NaN here, this expression is true and max <- double.NaN
				{
					max = temp;

					if(float.IsNaN(max))
						break;

				}
			}
			
			return max;
		}

		///<summary>Compute the p Norm of this vector.</summary>
		/// <param name="vector">The vector.</param>
    ///<param name="p">Order of the norm.</param>
		///<returns>The p norm of the vector.</returns>
		///<remarks>p &gt; 0, if p &lt; 0, ABS(p) is used. If p = 0 or positive infinity, the infinity norm is returned.</remarks>
		public static double LpNorm(this float[] vector, double p)
		{
		if (p == 0 )
			{
        return LInfinityNorm(vector);
      }
			else if(p==1)
			{
				return L1Norm(vector);
			}
			else if(p == 2)
			{
				return L2Norm(vector);
			}

			if (p < 0)
			{
				p = -p;
			}
			double ret = 0;
			for (int i = vector.Length-1; i >=0; --i)
			{
				ret += System.Math.Pow(Math.Abs(vector[i]), p);
			}
			return (float)System.Math.Pow(ret, 1 / p);
		}

		


		/// <summary>
		/// Returns the maximum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The maximum of all elements in <paramref name="vector"/>.</returns>
		public static float Max(this float[] vector)
		{
			if (vector.Length == 0)
				throw new ArgumentException("Result undefined for vector of zero length", nameof(vector));

			float max = float.NegativeInfinity;
			float tmp;
			for (int i = 0; i < vector.Length; ++i)
			{
				tmp = vector[i];
				if (tmp > max)
					max = tmp;
			}
			return max;
		}

			/// <summary>
		/// Returns the maximum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="start">First element of the vector to include in the evaluation.</param>
		/// <param name="count">Number of elements of the vector to include in the evaluation.</param>
		/// <returns>The maximum of all elements in <paramref name="vector"/>.</returns>
		public static float Max(this float[] vector, int start, int count)
		{
			if (!(start >= 0))
				throw new ArgumentException("must be >=0", nameof(start));
			if (!(count > 0))
				throw new ArgumentException("Result undefined for count <=0", nameof(count));
			int end = start + count;
			if(!(end<=vector.Length))
				throw new ArgumentException("must be <= vector.count - start", nameof(count));

			float max = float.NegativeInfinity;
			float tmp;
			for (int i = start; i < end; ++i)
			{
				tmp = vector[i];
				if (tmp > max)
					max = tmp;
			}
			return max;
		}

		/// <summary>
		/// Returns the minimum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The minimum of all elements in <paramref name="vector"/>.</returns>
		public static float Min(this float[] vector)
		{
			if (vector.Length == 0)
				throw new ArgumentException("Result undefined for vector of zero length", nameof(vector));

			float min = float.PositiveInfinity;
			float tmp;
			for (int i = 0; i < vector.Length; ++i)
			{
				tmp = vector[i];
				if (tmp < min)
					min = tmp;
			}
			return min;
		}

		/// <summary>
		/// Returns the minimum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="start">First element of the vector to include in the evaluation.</param>
		/// <param name="count">Number of elements of the vector to include in the evaluation.</param>
		/// <returns>The minimum of all elements in <paramref name="vector"/>.</returns>
		public static float Min(this float[] vector, int start, int count)
		{
			if (!(start >= 0))
				throw new ArgumentException("must be >=0", nameof(start));
			if (!(count > 0))
				throw new ArgumentException("Result undefined for count <=0", nameof(count));
			int end = start + count;
			if(!(end<=vector.Length))
				throw new ArgumentException("must be <= vector.count - start", nameof(count));

			float min = float.PositiveInfinity;
			float tmp;
			for (int i = start; i < end; ++i)
			{
				tmp = vector[i];
				if (tmp < min)
					min = tmp;
			}
			return min;
		}


    /// <summary>
    /// Returns the sum of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The sum of all elements in <paramref name="vector"/>.</returns>
    public static float Sum(this float[] vector)
    {
      float sum = 0;
      for (int i = 0; i < vector.Length; ++i)
        sum += vector[i];

      return sum; 
    }


		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The average of all elements in <paramref name="vector"/>.</returns>
    public static double Average(this float[] vector)
    {
      float sum = 0;
      for (int i = 0; i < vector.Length; ++i)
        sum += vector[i];

      return sum / (double)vector.Length; 
    }

		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The average of all elements in <paramref name="vector"/>.</returns>
    public static double Mean(this float[] vector)
    {
			return Average(vector);
		}


		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>, as well as the variance (sum of squares of the mean centered values divided by length of the vector).
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The mean and variance of all elements in <paramref name="vector"/>.</returns>
    public static (double Mean, double Variance) MeanAndVariance(this float[] vector)
    {
			var mean = Mean(vector);

			double sum = 0;
      for (int i = 0; i < vector.Length; ++i)
        {
				var diff = vector[i] - mean;
				sum += (diff*diff);
				}

      return (mean, sum / (double)vector.Length);
		}

		/// <summary>
    /// Returns the kurtosis of the elements in <paramref name="vector"/>. The kurtosis is defined as
		/// kurtosis(X) = E{(X-µ)^4}/((E{(X-µ)²})².
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The kurtosis of the elements in <paramref name="vector"/>.</returns>
		public static double Kurtosis(this float[] vector)
    {
			var N = vector.Length;
			double sum = 0;
      for (int i = N-1; i>=0; --i)
        {
				sum += vector[i];
				}
			var mean = sum/N;

			double sumy2 = 0;
      double sumy4 = 0;
      for (int i = N-1; i>=0; --i)
        {
				var e = vector[i]-mean;
				var e2 = e*e;
				sumy2 += e2;
				sumy4 += e2*e2;
				}


      return N*sumy4/(sumy2*sumy2);
    }

		/// <summary>
    /// Returns the excess kurtosis of the elements in <paramref name="vector"/>. The excess kurtosis is defined as
		/// excesskurtosis(X) = E{X^4} - 3(E{X²})². 
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The excess kurtosis of the elements in <paramref name="vector"/>.</returns>
    public static double ExcessKurtosisOfNormalized(this float[] vector)
    {
      double sumy4 = 0;
			double sumy2 = 0;
      for (int i = 0; i < vector.Length; ++i)
        {
				var e = vector[i];
				var e2 = e*e;
				sumy2 += e2;
				sumy4 += e2*e2;
				}

			var N = vector.Length;

      return sumy4/N -3*RMath.Pow2(sumy2/N); 
    }


// ******************************************** Definitions for int[] *******************************************



		/// <summary>
    /// Determines whether the given <paramref name="vector"/> contains any elements.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>True if the <paramref name="vector"/> contains any element; false otherwise, or if the vector is null.</returns>
    public static bool Any(this int[] vector)
    {
      return vector is not null && vector.Length > 0;
    }

    /// <summary>
    /// Determines whether any element of the <paramref name="vector"/> satisfies a condition.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="predicate">The condition to satisfy.</param>
    /// <returns>True if any element of the <paramref name="vector"/> satisfies the condition; false otherwise, or if the vector is null.</returns>
    public static bool Any(this int[] vector, Func<int, bool> predicate)
    {
      if (vector is not null)
      {
        for (int i = 0; i < vector.Length; ++i)
        {
          if (predicate(vector[i]))
            return true;
        }
      }
      return false;
    }

		 /// <summary>
    /// Determines whether any element of the <paramref name="vector"/> satisfies a condition.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="predicate">The condition to satisfy.</param>
    /// <param name="atIndex">The index of the first element that satisfied the condition; or -1 if no element satisfied the condition.</param>
    /// <returns>True if any element of the <paramref name="vector"/> satisfies the condition; false otherwise, or if the vector is null.</returns>
    public static bool Any(this int[] vector, Func<int, bool> predicate, out int atIndex)
    {
      if (vector is not null)
      {
        for (int i = 0; i < vector.Length; ++i)
        {
          if (predicate(vector[i]))
            {
						atIndex = i;
						return true;
						}
        }
      }
			atIndex = -1;
      return false;
    }

    /// <summary>Return the index of the first element with the maximum  value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the maximum value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMaxValue(this int[] vector)
    {
      int index = -1;
			int i;
      int max = int.MinValue;
      for (i = 0; i < vector.Length; ++i)
      {
        var test = vector[i];
        if (test >= max) // first, make the test for greater than or equal
        {
          index = i;
          max = test;
					break;
        }
      }
      for (++i; i < vector.Length; ++i)
      {
        var test = vector[i];
        if (test > max) // then, consider only values greater than
        {
          index = i;
          max = test;
        }
      }

      return index;
    }

		 /// <summary>Return the index of the first element with the maximum absolute value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the maximum absolute value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMaxAbsoluteValue(this int[] vector)
    {
      int index = -1;
			int i;
      var max = (long)int.MinValue;
      for (i = 0; i < vector.Length; ++i)
      {
        var test = Math.Abs((long)vector[i]);
        if (test >= max) // first, make the test for greater than or equal
        {
          index = i;
          max = test;
					break;
        }
      }
      for (++i; i < vector.Length; ++i)
      {
        var test = Math.Abs((long)vector[i]);
        if (test > max) // then, consider only values greater than
        {
          index = i;
          max = test;
        }
      }

      return index;
    }

		/// <summary>Return the index of the first element with the minimum value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the minimum value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMinValue(this int[] vector)
    {
      int index = -1;
      int i;
      int min = int.MaxValue;
      for (i = 0; i < vector.Length; ++i)
      {
        var test = vector[i];
        if (test <= min) // first, make the test for less than or equal
        {
          index = i;
          min = test;
					break;
        }
      }
      for (++i; i < vector.Length; ++i)
      {
        var test = vector[i];
        if (test < min) // then, consider only values less than
        {
          index = i;
          min = test;
        }
      }

      return index;
    }

		/// <summary>Return the index of the first element with the minimum absolute value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the minimum absolute value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMinAbsoluteValue(this int[] vector)
    {
      int index = -1;
      int i;
      var min = (long)int.MaxValue;
      for (i = 0; i < vector.Length; ++i)
      {
        var test = Math.Abs((long)vector[i]);
        if (test <= min) // first, make the test for less than or equal
        {
          index = i;
          min = test;
					break;
        }
      }
      for (++i; i < vector.Length; ++i)
      {
        var test = Math.Abs((long)vector[i]);
        if (test < min) // then, consider only values less than
        {
          index = i;
          min = test;
        }
      }

      return index;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing or decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <param name="isDecreasing">On return, this argument is set to true if the sequence is strictly decreasing. If increasing, this argument is set to false.</param>
    /// <returns>True if the sequence is strictly increasing or decreasing.</returns>
    public static bool IsStrictlyIncreasingOrDecreasing(this int[] vector, out bool isDecreasing)
    {
      isDecreasing = false;
      if (vector.Length == 0)
        return false;
      int sign = Math.Sign(vector[vector.Length - 1] - vector[0]);
      if (sign == 0)
        return false;

      isDecreasing = (sign < 0);

      for (int i = vector.Length - 1; i >= 1; --i)
        if (Math.Sign(vector[i] - vector[i - 1]) != sign)
          return false;

      return true;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing or decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly increasing or decreasing.</returns>
    public static bool IsStrictlyIncreasingOrDecreasing(this int[] vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing);
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly decreasing.</returns>
    public static bool IsStrictlyDecreasing(this int[] vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing) && isDecreasing;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly increasing.</returns>
    public static bool IsStrictlyIncreasing(this int[] vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing) && !isDecreasing;
    }


		/// <summary>
    /// Calculates the L1 norm of the vector (as the sum of the absolute values of the elements).
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>L1 norm of the vector (sum of the absolute values of the elements).</returns>
    public static long L1Norm(this int[] vector)
    {
      long sum = 0;
      for (int i = 0; i < vector.Length; ++i)
        sum += Math.Abs((long)vector[i]);

      return sum;
    }


		/// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
		/// <param name="vector">An input array of length n. </param>
		/// <param name="startIndex">The index of the first element in x to process.</param>
		/// <param name="count">A positive integer input variable of the number of elements to process.</param>
		/// <returns>The euclidian norm of the vector of length n, i.e. the square root of the sum of squares of the elements.</returns>
		/// <remarks>
		///     the euclidean norm is computed by accumulating the sum of
		///     squares in three different sums. the sums of squares for the
		///     small and large components are scaled so that no overflows
		///     occur. non-destructive underflows are permitted. underflows
		///     and overflows do not occur in the computation of the unscaled
		///     sum of squares for the intermediate components.
		///     the definitions of small, intermediate and large components
		///     depend on two constants, rdwarf and rgiant. the main
		///     restrictions on these constants are that rdwarf**2 not
		///     underflow and rgiant**2 not overflow. the constants
		///     given here are suitable for every known computer.
		///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
		/// </remarks>
		public static double L2Norm(this int[] vector, int startIndex, int count)
    {
      double sqr(double v) => (v * v);
      const double rdwarf = 3.834e-20;
      const double rgiant = 1.304e19;

      double ret_val = 0;
      double xabs;
      double x1max = 0, x3max = 0, s1 = 0, s2 = 0, s3 = 0;
      var agiant = rgiant / (double)count;

      for (int i = 0; i < count; ++i)
      {
        xabs = Math.Abs((double)vector[i + startIndex]);

        if (xabs > rgiant)
        {
          //sum for large components
          if (xabs <= x1max)
          {
            s1 += sqr(xabs / x1max);
          }
          else
          {
            s1 = 1 + s1 * sqr(x1max / xabs);
            x1max = xabs;
          }
        }
        else if (xabs <= rdwarf)
        {
          // sum for small components
          if (xabs <= x3max)
          {
            if (xabs != 0)
              s3 += sqr(xabs / x3max);
          }
          else
          {
            s3 = 1 + s3 * sqr(x3max / xabs);
            x3max = xabs;
          }
        }
        else
        {
          // sum for intermediate components
          s2 += sqr(xabs);
        }
      }

      // calculation of norm
      if (s1 == 0)
      {
        if (s2 == 0)
          ret_val = x3max * Math.Sqrt(s3);
        else if (s2 >= x3max)
          ret_val = Math.Sqrt(s2 * (1.0 + x3max / s2 * (x3max * s3)));
        else
          ret_val = Math.Sqrt(x3max * (s2 / x3max + x3max * s3));
      }
      else
      {
        ret_val = x1max * Math.Sqrt(s1 + s2 / x1max / x1max);
      }

      return ret_val;
    }

		/// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
    /// <param name="vector">An input array of length n. </param>
    /// <returns>The euclidian norm of the vector, i.e. the square root of the sum of squares of the elements.</returns>
    /// <remarks>
    ///     the euclidean norm is computed by accumulating the sum of
    ///     squares in three different sums. the sums of squares for the
    ///     small and large components are scaled so that no overflows
    ///     occur. non-destructive underflows are permitted. underflows
    ///     and overflows do not occur in the computation of the unscaled
    ///     sum of squares for the intermediate components.
    ///     the definitions of small, intermediate and large components
    ///     depend on two constants, rdwarf and rgiant. the main
    ///     restrictions on these constants are that rdwarf**2 not
    ///     underflow and rgiant**2 not overflow. the constants
    ///     given here are suitable for every known computer.
    ///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
    /// </remarks>
    public static double L2Norm(this int[] vector)
    {
      return L2Norm(vector, 0, vector.Length);
    }


		 /// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
    /// <param name="vector">An input array of length n. </param>
    /// <returns>The euclidian norm of the vector, i.e. the square root of the sum of squares of the elements.</returns>
    /// <remarks>
    ///     the euclidean norm is computed by accumulating the sum of
    ///     squares in three different sums. the sums of squares for the
    ///     small and large components are scaled so that no overflows
    ///     occur. non-destructive underflows are permitted. underflows
    ///     and overflows do not occur in the computation of the unscaled
    ///     sum of squares for the intermediate components.
    ///     the definitions of small, intermediate and large components
    ///     depend on two constants, rdwarf and rgiant. the main
    ///     restrictions on these constants are that rdwarf**2 not
    ///     underflow and rgiant**2 not overflow. the constants
    ///     given here are suitable for every known computer.
    ///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
    /// </remarks>
    public static double EuclideanNorm(this int[] vector)
    {
      return L2Norm(vector, 0, vector.Length);
    }


		/// <summary>
		/// Returns the L-infinity norm of the provided <paramref name="vector"/> (as is the maximum of the absolute value of the elements). If one
		/// of the elements of the vector is invalid, the return value is also invalid (for the floating point types).
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The L-infinity norm of the provided <paramref name="vector"/> (as is the maximum of the absolute value of the elements).</returns>
		public static long LInfinityNorm(this int[] vector)
		{
			long max = 0;
			int i;
			for (i = vector.Length -1; i>=0; --i)
			{
				var temp = Math.Abs((long)vector[i]);
				if (!(max>temp)) // if we have a double.NaN here, this expression is true and max <- double.NaN
				{
					max = temp;


				}
			}
			
			return max;
		}

			/// <summary>
		/// Returns the L-infinity norm of the difference of <paramref name="vector1"/> and <paramref name="vector2"/> (as is the maximum of the absolute value of the differences of the elements). If one
		/// of the elements of the vector is invalid, the return value is also invalid (for the floating point types).
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The second vector. Must have same length as the first vector.</param>
		/// <returns>The L-infinity norm of the element-wise differences of the provided vectors (as is the maximum of the absolute value of the differences).</returns>
		public static long LInfinityNorm(int[] vector1, int[] vector2)
		{
			if( vector1.Length != vector2.Length)
				throw new RankException("Length of vector 1 must match length of vector 2");

			long max = 0;
			int i;
			for (i = vector1.Length - 1; i >= 0; --i)
			{
				var temp = Math.Abs((long)vector1[i] - (long)vector2[i]);
				if (!(max>temp)) // if we have a double.NaN here, this expression is true and max <- double.NaN
				{
					max = temp;


				}
			}
			
			return max;
		}

		///<summary>Compute the p Norm of this vector.</summary>
		/// <param name="vector">The vector.</param>
    ///<param name="p">Order of the norm.</param>
		///<returns>The p norm of the vector.</returns>
		///<remarks>p &gt; 0, if p &lt; 0, ABS(p) is used. If p = 0 or positive infinity, the infinity norm is returned.</remarks>
		public static double LpNorm(this int[] vector, double p)
		{
		if (p == 0 )
			{
        return LInfinityNorm(vector);
      }
			else if(p==1)
			{
				return L1Norm(vector);
			}
			else if(p == 2)
			{
				return L2Norm(vector);
			}

			if (p < 0)
			{
				p = -p;
			}
			double ret = 0;
			for (int i = vector.Length-1; i >=0; --i)
			{
				ret += System.Math.Pow(Math.Abs((long)vector[i]), p);
			}
			return (long)System.Math.Pow(ret, 1 / p);
		}

		


		/// <summary>
		/// Returns the maximum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The maximum of all elements in <paramref name="vector"/>.</returns>
		public static int Max(this int[] vector)
		{
			if (vector.Length == 0)
				throw new ArgumentException("Result undefined for vector of zero length", nameof(vector));

			int max = int.MinValue;
			int tmp;
			for (int i = 0; i < vector.Length; ++i)
			{
				tmp = vector[i];
				if (tmp > max)
					max = tmp;
			}
			return max;
		}

			/// <summary>
		/// Returns the maximum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="start">First element of the vector to include in the evaluation.</param>
		/// <param name="count">Number of elements of the vector to include in the evaluation.</param>
		/// <returns>The maximum of all elements in <paramref name="vector"/>.</returns>
		public static int Max(this int[] vector, int start, int count)
		{
			if (!(start >= 0))
				throw new ArgumentException("must be >=0", nameof(start));
			if (!(count > 0))
				throw new ArgumentException("Result undefined for count <=0", nameof(count));
			int end = start + count;
			if(!(end<=vector.Length))
				throw new ArgumentException("must be <= vector.count - start", nameof(count));

			int max = int.MinValue;
			int tmp;
			for (int i = start; i < end; ++i)
			{
				tmp = vector[i];
				if (tmp > max)
					max = tmp;
			}
			return max;
		}

		/// <summary>
		/// Returns the minimum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The minimum of all elements in <paramref name="vector"/>.</returns>
		public static int Min(this int[] vector)
		{
			if (vector.Length == 0)
				throw new ArgumentException("Result undefined for vector of zero length", nameof(vector));

			int min = int.MaxValue;
			int tmp;
			for (int i = 0; i < vector.Length; ++i)
			{
				tmp = vector[i];
				if (tmp < min)
					min = tmp;
			}
			return min;
		}

		/// <summary>
		/// Returns the minimum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="start">First element of the vector to include in the evaluation.</param>
		/// <param name="count">Number of elements of the vector to include in the evaluation.</param>
		/// <returns>The minimum of all elements in <paramref name="vector"/>.</returns>
		public static int Min(this int[] vector, int start, int count)
		{
			if (!(start >= 0))
				throw new ArgumentException("must be >=0", nameof(start));
			if (!(count > 0))
				throw new ArgumentException("Result undefined for count <=0", nameof(count));
			int end = start + count;
			if(!(end<=vector.Length))
				throw new ArgumentException("must be <= vector.count - start", nameof(count));

			int min = int.MaxValue;
			int tmp;
			for (int i = start; i < end; ++i)
			{
				tmp = vector[i];
				if (tmp < min)
					min = tmp;
			}
			return min;
		}


    /// <summary>
    /// Returns the sum of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The sum of all elements in <paramref name="vector"/>.</returns>
    public static long Sum(this int[] vector)
    {
      long sum = 0;
      for (int i = 0; i < vector.Length; ++i)
        sum += vector[i];

      return sum; 
    }


		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The average of all elements in <paramref name="vector"/>.</returns>
    public static double Average(this int[] vector)
    {
      long sum = 0;
      for (int i = 0; i < vector.Length; ++i)
        sum += vector[i];

      return sum / (double)vector.Length; 
    }

		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The average of all elements in <paramref name="vector"/>.</returns>
    public static double Mean(this int[] vector)
    {
			return Average(vector);
		}


		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>, as well as the variance (sum of squares of the mean centered values divided by length of the vector).
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The mean and variance of all elements in <paramref name="vector"/>.</returns>
    public static (double Mean, double Variance) MeanAndVariance(this int[] vector)
    {
			var mean = Mean(vector);

			double sum = 0;
      for (int i = 0; i < vector.Length; ++i)
        {
				var diff = vector[i] - mean;
				sum += (diff*diff);
				}

      return (mean, sum / (double)vector.Length);
		}

		/// <summary>
    /// Returns the kurtosis of the elements in <paramref name="vector"/>. The kurtosis is defined as
		/// kurtosis(X) = E{(X-µ)^4}/((E{(X-µ)²})².
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The kurtosis of the elements in <paramref name="vector"/>.</returns>
		public static double Kurtosis(this int[] vector)
    {
			var N = vector.Length;
			double sum = 0;
      for (int i = N-1; i>=0; --i)
        {
				sum += vector[i];
				}
			var mean = sum/N;

			double sumy2 = 0;
      double sumy4 = 0;
      for (int i = N-1; i>=0; --i)
        {
				var e = vector[i]-mean;
				var e2 = e*e;
				sumy2 += e2;
				sumy4 += e2*e2;
				}


      return N*sumy4/(sumy2*sumy2);
    }

		/// <summary>
    /// Returns the excess kurtosis of the elements in <paramref name="vector"/>. The excess kurtosis is defined as
		/// excesskurtosis(X) = E{X^4} - 3(E{X²})². 
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The excess kurtosis of the elements in <paramref name="vector"/>.</returns>
    public static double ExcessKurtosisOfNormalized(this int[] vector)
    {
      double sumy4 = 0;
			double sumy2 = 0;
      for (int i = 0; i < vector.Length; ++i)
        {
				var e = vector[i];
				var e2 = e*e;
				sumy2 += e2;
				sumy4 += e2*e2;
				}

			var N = vector.Length;

      return sumy4/N -3*RMath.Pow2(sumy2/N); 
    }


// ******************************************** Definitions for IReadOnlyList<double> *******************************************



		/// <summary>
    /// Determines whether the given <paramref name="vector"/> contains any elements.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>True if the <paramref name="vector"/> contains any element; false otherwise, or if the vector is null.</returns>
    public static bool Any(this IReadOnlyList<double> vector)
    {
      return vector is not null && vector.Count > 0;
    }

    /// <summary>
    /// Determines whether any element of the <paramref name="vector"/> satisfies a condition.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="predicate">The condition to satisfy.</param>
    /// <returns>True if any element of the <paramref name="vector"/> satisfies the condition; false otherwise, or if the vector is null.</returns>
    public static bool Any(this IReadOnlyList<double> vector, Func<double, bool> predicate)
    {
      if (vector is not null)
      {
        for (int i = 0; i < vector.Count; ++i)
        {
          if (predicate(vector[i]))
            return true;
        }
      }
      return false;
    }

		 /// <summary>
    /// Determines whether any element of the <paramref name="vector"/> satisfies a condition.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="predicate">The condition to satisfy.</param>
    /// <param name="atIndex">The index of the first element that satisfied the condition; or -1 if no element satisfied the condition.</param>
    /// <returns>True if any element of the <paramref name="vector"/> satisfies the condition; false otherwise, or if the vector is null.</returns>
    public static bool Any(this IReadOnlyList<double> vector, Func<double, bool> predicate, out int atIndex)
    {
      if (vector is not null)
      {
        for (int i = 0; i < vector.Count; ++i)
        {
          if (predicate(vector[i]))
            {
						atIndex = i;
						return true;
						}
        }
      }
			atIndex = -1;
      return false;
    }

    /// <summary>Return the index of the first element with the maximum  value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the maximum value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMaxValue(this IReadOnlyList<double> vector)
    {
      int index = -1;
			int i;
      double max = double.NegativeInfinity;
      for (i = 0; i < vector.Count; ++i)
      {
        var test = vector[i];
        if (test >= max) // first, make the test for greater than or equal
        {
          index = i;
          max = test;
					break;
        }
      }
      for (++i; i < vector.Count; ++i)
      {
        var test = vector[i];
        if (test > max) // then, consider only values greater than
        {
          index = i;
          max = test;
        }
      }

      return index;
    }

		 /// <summary>Return the index of the first element with the maximum absolute value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the maximum absolute value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMaxAbsoluteValue(this IReadOnlyList<double> vector)
    {
      int index = -1;
			int i;
      var max = double.NegativeInfinity;
      for (i = 0; i < vector.Count; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test >= max) // first, make the test for greater than or equal
        {
          index = i;
          max = test;
					break;
        }
      }
      for (++i; i < vector.Count; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test > max) // then, consider only values greater than
        {
          index = i;
          max = test;
        }
      }

      return index;
    }

		/// <summary>Return the index of the first element with the minimum value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the minimum value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMinValue(this IReadOnlyList<double> vector)
    {
      int index = -1;
      int i;
      double min = double.PositiveInfinity;
      for (i = 0; i < vector.Count; ++i)
      {
        var test = vector[i];
        if (test <= min) // first, make the test for less than or equal
        {
          index = i;
          min = test;
					break;
        }
      }
      for (++i; i < vector.Count; ++i)
      {
        var test = vector[i];
        if (test < min) // then, consider only values less than
        {
          index = i;
          min = test;
        }
      }

      return index;
    }

		/// <summary>Return the index of the first element with the minimum absolute value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the minimum absolute value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMinAbsoluteValue(this IReadOnlyList<double> vector)
    {
      int index = -1;
      int i;
      var min = double.PositiveInfinity;
      for (i = 0; i < vector.Count; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test <= min) // first, make the test for less than or equal
        {
          index = i;
          min = test;
					break;
        }
      }
      for (++i; i < vector.Count; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test < min) // then, consider only values less than
        {
          index = i;
          min = test;
        }
      }

      return index;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing or decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <param name="isDecreasing">On return, this argument is set to true if the sequence is strictly decreasing. If increasing, this argument is set to false.</param>
    /// <returns>True if the sequence is strictly increasing or decreasing.</returns>
    public static bool IsStrictlyIncreasingOrDecreasing(this IReadOnlyList<double> vector, out bool isDecreasing)
    {
      isDecreasing = false;
      if (vector.Count == 0)
        return false;
      int sign = Math.Sign(vector[vector.Count - 1] - vector[0]);
      if (sign == 0)
        return false;

      isDecreasing = (sign < 0);

      for (int i = vector.Count - 1; i >= 1; --i)
        if (Math.Sign(vector[i] - vector[i - 1]) != sign)
          return false;

      return true;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing or decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly increasing or decreasing.</returns>
    public static bool IsStrictlyIncreasingOrDecreasing(this IReadOnlyList<double> vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing);
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly decreasing.</returns>
    public static bool IsStrictlyDecreasing(this IReadOnlyList<double> vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing) && isDecreasing;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly increasing.</returns>
    public static bool IsStrictlyIncreasing(this IReadOnlyList<double> vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing) && !isDecreasing;
    }


		/// <summary>
    /// Calculates the L1 norm of the vector (as the sum of the absolute values of the elements).
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>L1 norm of the vector (sum of the absolute values of the elements).</returns>
    public static double L1Norm(this IReadOnlyList<double> vector)
    {
      double sum = 0;
      for (int i = 0; i < vector.Count; ++i)
        sum += Math.Abs(vector[i]);

      return sum;
    }


		/// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
		/// <param name="vector">An input array of length n. </param>
		/// <param name="startIndex">The index of the first element in x to process.</param>
		/// <param name="count">A positive integer input variable of the number of elements to process.</param>
		/// <returns>The euclidian norm of the vector of length n, i.e. the square root of the sum of squares of the elements.</returns>
		/// <remarks>
		///     the euclidean norm is computed by accumulating the sum of
		///     squares in three different sums. the sums of squares for the
		///     small and large components are scaled so that no overflows
		///     occur. non-destructive underflows are permitted. underflows
		///     and overflows do not occur in the computation of the unscaled
		///     sum of squares for the intermediate components.
		///     the definitions of small, intermediate and large components
		///     depend on two constants, rdwarf and rgiant. the main
		///     restrictions on these constants are that rdwarf**2 not
		///     underflow and rgiant**2 not overflow. the constants
		///     given here are suitable for every known computer.
		///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
		/// </remarks>
		public static double L2Norm(this IReadOnlyList<double> vector, int startIndex, int count)
    {
      double sqr(double v) => (v * v);
      const double rdwarf = 3.834e-20;
      const double rgiant = 1.304e19;

      double ret_val = 0;
      double xabs;
      double x1max = 0, x3max = 0, s1 = 0, s2 = 0, s3 = 0;
      var agiant = rgiant / (double)count;

      for (int i = 0; i < count; ++i)
      {
        xabs = Math.Abs((double)vector[i + startIndex]);

        if (xabs > rgiant)
        {
          //sum for large components
          if (xabs <= x1max)
          {
            s1 += sqr(xabs / x1max);
          }
          else
          {
            s1 = 1 + s1 * sqr(x1max / xabs);
            x1max = xabs;
          }
        }
        else if (xabs <= rdwarf)
        {
          // sum for small components
          if (xabs <= x3max)
          {
            if (xabs != 0)
              s3 += sqr(xabs / x3max);
          }
          else
          {
            s3 = 1 + s3 * sqr(x3max / xabs);
            x3max = xabs;
          }
        }
        else
        {
          // sum for intermediate components
          s2 += sqr(xabs);
        }
      }

      // calculation of norm
      if (s1 == 0)
      {
        if (s2 == 0)
          ret_val = x3max * Math.Sqrt(s3);
        else if (s2 >= x3max)
          ret_val = Math.Sqrt(s2 * (1.0 + x3max / s2 * (x3max * s3)));
        else
          ret_val = Math.Sqrt(x3max * (s2 / x3max + x3max * s3));
      }
      else
      {
        ret_val = x1max * Math.Sqrt(s1 + s2 / x1max / x1max);
      }

      return ret_val;
    }

		/// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
    /// <param name="vector">An input array of length n. </param>
    /// <returns>The euclidian norm of the vector, i.e. the square root of the sum of squares of the elements.</returns>
    /// <remarks>
    ///     the euclidean norm is computed by accumulating the sum of
    ///     squares in three different sums. the sums of squares for the
    ///     small and large components are scaled so that no overflows
    ///     occur. non-destructive underflows are permitted. underflows
    ///     and overflows do not occur in the computation of the unscaled
    ///     sum of squares for the intermediate components.
    ///     the definitions of small, intermediate and large components
    ///     depend on two constants, rdwarf and rgiant. the main
    ///     restrictions on these constants are that rdwarf**2 not
    ///     underflow and rgiant**2 not overflow. the constants
    ///     given here are suitable for every known computer.
    ///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
    /// </remarks>
    public static double L2Norm(this IReadOnlyList<double> vector)
    {
      return L2Norm(vector, 0, vector.Count);
    }


		 /// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
    /// <param name="vector">An input array of length n. </param>
    /// <returns>The euclidian norm of the vector, i.e. the square root of the sum of squares of the elements.</returns>
    /// <remarks>
    ///     the euclidean norm is computed by accumulating the sum of
    ///     squares in three different sums. the sums of squares for the
    ///     small and large components are scaled so that no overflows
    ///     occur. non-destructive underflows are permitted. underflows
    ///     and overflows do not occur in the computation of the unscaled
    ///     sum of squares for the intermediate components.
    ///     the definitions of small, intermediate and large components
    ///     depend on two constants, rdwarf and rgiant. the main
    ///     restrictions on these constants are that rdwarf**2 not
    ///     underflow and rgiant**2 not overflow. the constants
    ///     given here are suitable for every known computer.
    ///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
    /// </remarks>
    public static double EuclideanNorm(this IReadOnlyList<double> vector)
    {
      return L2Norm(vector, 0, vector.Count);
    }


		/// <summary>
		/// Returns the L-infinity norm of the provided <paramref name="vector"/> (as is the maximum of the absolute value of the elements). If one
		/// of the elements of the vector is invalid, the return value is also invalid (for the floating point types).
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The L-infinity norm of the provided <paramref name="vector"/> (as is the maximum of the absolute value of the elements).</returns>
		public static double LInfinityNorm(this IReadOnlyList<double> vector)
		{
			double max = 0;
			int i;
			for (i = vector.Count -1; i>=0; --i)
			{
				var temp = Math.Abs(vector[i]);
				if (!(max>temp)) // if we have a double.NaN here, this expression is true and max <- double.NaN
				{
					max = temp;

					if(double.IsNaN(max))
						break;

				}
			}
			
			return max;
		}

			/// <summary>
		/// Returns the L-infinity norm of the difference of <paramref name="vector1"/> and <paramref name="vector2"/> (as is the maximum of the absolute value of the differences of the elements). If one
		/// of the elements of the vector is invalid, the return value is also invalid (for the floating point types).
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The second vector. Must have same length as the first vector.</param>
		/// <returns>The L-infinity norm of the element-wise differences of the provided vectors (as is the maximum of the absolute value of the differences).</returns>
		public static double LInfinityNorm(IReadOnlyList<double> vector1, IReadOnlyList<double> vector2)
		{
			if( vector1.Count != vector2.Count)
				throw new RankException("Length of vector 1 must match length of vector 2");

			double max = 0;
			int i;
			for (i = vector1.Count - 1; i >= 0; --i)
			{
				var temp = Math.Abs(vector1[i] - vector2[i]);
				if (!(max>temp)) // if we have a double.NaN here, this expression is true and max <- double.NaN
				{
					max = temp;

					if(double.IsNaN(max))
						break;

				}
			}
			
			return max;
		}

		///<summary>Compute the p Norm of this vector.</summary>
		/// <param name="vector">The vector.</param>
    ///<param name="p">Order of the norm.</param>
		///<returns>The p norm of the vector.</returns>
		///<remarks>p &gt; 0, if p &lt; 0, ABS(p) is used. If p = 0 or positive infinity, the infinity norm is returned.</remarks>
		public static double LpNorm(this IReadOnlyList<double> vector, double p)
		{
		if (p == 0 )
			{
        return LInfinityNorm(vector);
      }
			else if(p==1)
			{
				return L1Norm(vector);
			}
			else if(p == 2)
			{
				return L2Norm(vector);
			}

			if (p < 0)
			{
				p = -p;
			}
			double ret = 0;
			for (int i = vector.Count-1; i >=0; --i)
			{
				ret += System.Math.Pow(Math.Abs(vector[i]), p);
			}
			return (double)System.Math.Pow(ret, 1 / p);
		}

		


		/// <summary>
		/// Returns the maximum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The maximum of all elements in <paramref name="vector"/>.</returns>
		public static double Max(this IReadOnlyList<double> vector)
		{
			if (vector.Count == 0)
				throw new ArgumentException("Result undefined for vector of zero length", nameof(vector));

			double max = double.NegativeInfinity;
			double tmp;
			for (int i = 0; i < vector.Count; ++i)
			{
				tmp = vector[i];
				if (tmp > max)
					max = tmp;
			}
			return max;
		}

			/// <summary>
		/// Returns the maximum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="start">First element of the vector to include in the evaluation.</param>
		/// <param name="count">Number of elements of the vector to include in the evaluation.</param>
		/// <returns>The maximum of all elements in <paramref name="vector"/>.</returns>
		public static double Max(this IReadOnlyList<double> vector, int start, int count)
		{
			if (!(start >= 0))
				throw new ArgumentException("must be >=0", nameof(start));
			if (!(count > 0))
				throw new ArgumentException("Result undefined for count <=0", nameof(count));
			int end = start + count;
			if(!(end<=vector.Count))
				throw new ArgumentException("must be <= vector.count - start", nameof(count));

			double max = double.NegativeInfinity;
			double tmp;
			for (int i = start; i < end; ++i)
			{
				tmp = vector[i];
				if (tmp > max)
					max = tmp;
			}
			return max;
		}

		/// <summary>
		/// Returns the minimum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The minimum of all elements in <paramref name="vector"/>.</returns>
		public static double Min(this IReadOnlyList<double> vector)
		{
			if (vector.Count == 0)
				throw new ArgumentException("Result undefined for vector of zero length", nameof(vector));

			double min = double.PositiveInfinity;
			double tmp;
			for (int i = 0; i < vector.Count; ++i)
			{
				tmp = vector[i];
				if (tmp < min)
					min = tmp;
			}
			return min;
		}

		/// <summary>
		/// Returns the minimum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="start">First element of the vector to include in the evaluation.</param>
		/// <param name="count">Number of elements of the vector to include in the evaluation.</param>
		/// <returns>The minimum of all elements in <paramref name="vector"/>.</returns>
		public static double Min(this IReadOnlyList<double> vector, int start, int count)
		{
			if (!(start >= 0))
				throw new ArgumentException("must be >=0", nameof(start));
			if (!(count > 0))
				throw new ArgumentException("Result undefined for count <=0", nameof(count));
			int end = start + count;
			if(!(end<=vector.Count))
				throw new ArgumentException("must be <= vector.count - start", nameof(count));

			double min = double.PositiveInfinity;
			double tmp;
			for (int i = start; i < end; ++i)
			{
				tmp = vector[i];
				if (tmp < min)
					min = tmp;
			}
			return min;
		}


    /// <summary>
    /// Returns the sum of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The sum of all elements in <paramref name="vector"/>.</returns>
    public static double Sum(this IReadOnlyList<double> vector)
    {
      double sum = 0;
      for (int i = 0; i < vector.Count; ++i)
        sum += vector[i];

      return sum; 
    }


		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The average of all elements in <paramref name="vector"/>.</returns>
    public static double Average(this IReadOnlyList<double> vector)
    {
      double sum = 0;
      for (int i = 0; i < vector.Count; ++i)
        sum += vector[i];

      return sum / (double)vector.Count; 
    }

		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The average of all elements in <paramref name="vector"/>.</returns>
    public static double Mean(this IReadOnlyList<double> vector)
    {
			return Average(vector);
		}


		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>, as well as the variance (sum of squares of the mean centered values divided by length of the vector).
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The mean and variance of all elements in <paramref name="vector"/>.</returns>
    public static (double Mean, double Variance) MeanAndVariance(this IReadOnlyList<double> vector)
    {
			var mean = Mean(vector);

			double sum = 0;
      for (int i = 0; i < vector.Count; ++i)
        {
				var diff = vector[i] - mean;
				sum += (diff*diff);
				}

      return (mean, sum / (double)vector.Count);
		}

		/// <summary>
    /// Returns the kurtosis of the elements in <paramref name="vector"/>. The kurtosis is defined as
		/// kurtosis(X) = E{(X-µ)^4}/((E{(X-µ)²})².
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The kurtosis of the elements in <paramref name="vector"/>.</returns>
		public static double Kurtosis(this IReadOnlyList<double> vector)
    {
			var N = vector.Count;
			double sum = 0;
      for (int i = N-1; i>=0; --i)
        {
				sum += vector[i];
				}
			var mean = sum/N;

			double sumy2 = 0;
      double sumy4 = 0;
      for (int i = N-1; i>=0; --i)
        {
				var e = vector[i]-mean;
				var e2 = e*e;
				sumy2 += e2;
				sumy4 += e2*e2;
				}


      return N*sumy4/(sumy2*sumy2);
    }

		/// <summary>
    /// Returns the excess kurtosis of the elements in <paramref name="vector"/>. The excess kurtosis is defined as
		/// excesskurtosis(X) = E{X^4} - 3(E{X²})². 
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The excess kurtosis of the elements in <paramref name="vector"/>.</returns>
    public static double ExcessKurtosisOfNormalized(this IReadOnlyList<double> vector)
    {
      double sumy4 = 0;
			double sumy2 = 0;
      for (int i = 0; i < vector.Count; ++i)
        {
				var e = vector[i];
				var e2 = e*e;
				sumy2 += e2;
				sumy4 += e2*e2;
				}

			var N = vector.Count;

      return sumy4/N -3*RMath.Pow2(sumy2/N); 
    }


// ******************************************** Definitions for IReadOnlyList<float> *******************************************



		/// <summary>
    /// Determines whether the given <paramref name="vector"/> contains any elements.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>True if the <paramref name="vector"/> contains any element; false otherwise, or if the vector is null.</returns>
    public static bool Any(this IReadOnlyList<float> vector)
    {
      return vector is not null && vector.Count > 0;
    }

    /// <summary>
    /// Determines whether any element of the <paramref name="vector"/> satisfies a condition.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="predicate">The condition to satisfy.</param>
    /// <returns>True if any element of the <paramref name="vector"/> satisfies the condition; false otherwise, or if the vector is null.</returns>
    public static bool Any(this IReadOnlyList<float> vector, Func<float, bool> predicate)
    {
      if (vector is not null)
      {
        for (int i = 0; i < vector.Count; ++i)
        {
          if (predicate(vector[i]))
            return true;
        }
      }
      return false;
    }

		 /// <summary>
    /// Determines whether any element of the <paramref name="vector"/> satisfies a condition.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="predicate">The condition to satisfy.</param>
    /// <param name="atIndex">The index of the first element that satisfied the condition; or -1 if no element satisfied the condition.</param>
    /// <returns>True if any element of the <paramref name="vector"/> satisfies the condition; false otherwise, or if the vector is null.</returns>
    public static bool Any(this IReadOnlyList<float> vector, Func<float, bool> predicate, out int atIndex)
    {
      if (vector is not null)
      {
        for (int i = 0; i < vector.Count; ++i)
        {
          if (predicate(vector[i]))
            {
						atIndex = i;
						return true;
						}
        }
      }
			atIndex = -1;
      return false;
    }

    /// <summary>Return the index of the first element with the maximum  value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the maximum value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMaxValue(this IReadOnlyList<float> vector)
    {
      int index = -1;
			int i;
      float max = float.NegativeInfinity;
      for (i = 0; i < vector.Count; ++i)
      {
        var test = vector[i];
        if (test >= max) // first, make the test for greater than or equal
        {
          index = i;
          max = test;
					break;
        }
      }
      for (++i; i < vector.Count; ++i)
      {
        var test = vector[i];
        if (test > max) // then, consider only values greater than
        {
          index = i;
          max = test;
        }
      }

      return index;
    }

		 /// <summary>Return the index of the first element with the maximum absolute value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the maximum absolute value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMaxAbsoluteValue(this IReadOnlyList<float> vector)
    {
      int index = -1;
			int i;
      var max = float.NegativeInfinity;
      for (i = 0; i < vector.Count; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test >= max) // first, make the test for greater than or equal
        {
          index = i;
          max = test;
					break;
        }
      }
      for (++i; i < vector.Count; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test > max) // then, consider only values greater than
        {
          index = i;
          max = test;
        }
      }

      return index;
    }

		/// <summary>Return the index of the first element with the minimum value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the minimum value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMinValue(this IReadOnlyList<float> vector)
    {
      int index = -1;
      int i;
      float min = float.PositiveInfinity;
      for (i = 0; i < vector.Count; ++i)
      {
        var test = vector[i];
        if (test <= min) // first, make the test for less than or equal
        {
          index = i;
          min = test;
					break;
        }
      }
      for (++i; i < vector.Count; ++i)
      {
        var test = vector[i];
        if (test < min) // then, consider only values less than
        {
          index = i;
          min = test;
        }
      }

      return index;
    }

		/// <summary>Return the index of the first element with the minimum absolute value in a vector</summary>
		/// <param name="vector">The input vector.</param>
		/// <returns>The index of the first element with the minimum absolute value. Returns -1 if the vector is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMinAbsoluteValue(this IReadOnlyList<float> vector)
    {
      int index = -1;
      int i;
      var min = float.PositiveInfinity;
      for (i = 0; i < vector.Count; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test <= min) // first, make the test for less than or equal
        {
          index = i;
          min = test;
					break;
        }
      }
      for (++i; i < vector.Count; ++i)
      {
        var test = Math.Abs(vector[i]);
        if (test < min) // then, consider only values less than
        {
          index = i;
          min = test;
        }
      }

      return index;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing or decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <param name="isDecreasing">On return, this argument is set to true if the sequence is strictly decreasing. If increasing, this argument is set to false.</param>
    /// <returns>True if the sequence is strictly increasing or decreasing.</returns>
    public static bool IsStrictlyIncreasingOrDecreasing(this IReadOnlyList<float> vector, out bool isDecreasing)
    {
      isDecreasing = false;
      if (vector.Count == 0)
        return false;
      int sign = Math.Sign(vector[vector.Count - 1] - vector[0]);
      if (sign == 0)
        return false;

      isDecreasing = (sign < 0);

      for (int i = vector.Count - 1; i >= 1; --i)
        if (Math.Sign(vector[i] - vector[i - 1]) != sign)
          return false;

      return true;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing or decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly increasing or decreasing.</returns>
    public static bool IsStrictlyIncreasingOrDecreasing(this IReadOnlyList<float> vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing);
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly decreasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly decreasing.</returns>
    public static bool IsStrictlyDecreasing(this IReadOnlyList<float> vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing) && isDecreasing;
    }

		/// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing.
    /// </summary>
    /// <param name="vector">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly increasing.</returns>
    public static bool IsStrictlyIncreasing(this IReadOnlyList<float> vector)
    {
      return IsStrictlyIncreasingOrDecreasing(vector, out var isDecreasing) && !isDecreasing;
    }


		/// <summary>
    /// Calculates the L1 norm of the vector (as the sum of the absolute values of the elements).
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>L1 norm of the vector (sum of the absolute values of the elements).</returns>
    public static float L1Norm(this IReadOnlyList<float> vector)
    {
      float sum = 0;
      for (int i = 0; i < vector.Count; ++i)
        sum += Math.Abs(vector[i]);

      return sum;
    }


		/// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
		/// <param name="vector">An input array of length n. </param>
		/// <param name="startIndex">The index of the first element in x to process.</param>
		/// <param name="count">A positive integer input variable of the number of elements to process.</param>
		/// <returns>The euclidian norm of the vector of length n, i.e. the square root of the sum of squares of the elements.</returns>
		/// <remarks>
		///     the euclidean norm is computed by accumulating the sum of
		///     squares in three different sums. the sums of squares for the
		///     small and large components are scaled so that no overflows
		///     occur. non-destructive underflows are permitted. underflows
		///     and overflows do not occur in the computation of the unscaled
		///     sum of squares for the intermediate components.
		///     the definitions of small, intermediate and large components
		///     depend on two constants, rdwarf and rgiant. the main
		///     restrictions on these constants are that rdwarf**2 not
		///     underflow and rgiant**2 not overflow. the constants
		///     given here are suitable for every known computer.
		///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
		/// </remarks>
		public static double L2Norm(this IReadOnlyList<float> vector, int startIndex, int count)
    {
      double sqr(double v) => (v * v);
      const double rdwarf = 3.834e-20;
      const double rgiant = 1.304e19;

      double ret_val = 0;
      double xabs;
      double x1max = 0, x3max = 0, s1 = 0, s2 = 0, s3 = 0;
      var agiant = rgiant / (double)count;

      for (int i = 0; i < count; ++i)
      {
        xabs = Math.Abs((double)vector[i + startIndex]);

        if (xabs > rgiant)
        {
          //sum for large components
          if (xabs <= x1max)
          {
            s1 += sqr(xabs / x1max);
          }
          else
          {
            s1 = 1 + s1 * sqr(x1max / xabs);
            x1max = xabs;
          }
        }
        else if (xabs <= rdwarf)
        {
          // sum for small components
          if (xabs <= x3max)
          {
            if (xabs != 0)
              s3 += sqr(xabs / x3max);
          }
          else
          {
            s3 = 1 + s3 * sqr(x3max / xabs);
            x3max = xabs;
          }
        }
        else
        {
          // sum for intermediate components
          s2 += sqr(xabs);
        }
      }

      // calculation of norm
      if (s1 == 0)
      {
        if (s2 == 0)
          ret_val = x3max * Math.Sqrt(s3);
        else if (s2 >= x3max)
          ret_val = Math.Sqrt(s2 * (1.0 + x3max / s2 * (x3max * s3)));
        else
          ret_val = Math.Sqrt(x3max * (s2 / x3max + x3max * s3));
      }
      else
      {
        ret_val = x1max * Math.Sqrt(s1 + s2 / x1max / x1max);
      }

      return ret_val;
    }

		/// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
    /// <param name="vector">An input array of length n. </param>
    /// <returns>The euclidian norm of the vector, i.e. the square root of the sum of squares of the elements.</returns>
    /// <remarks>
    ///     the euclidean norm is computed by accumulating the sum of
    ///     squares in three different sums. the sums of squares for the
    ///     small and large components are scaled so that no overflows
    ///     occur. non-destructive underflows are permitted. underflows
    ///     and overflows do not occur in the computation of the unscaled
    ///     sum of squares for the intermediate components.
    ///     the definitions of small, intermediate and large components
    ///     depend on two constants, rdwarf and rgiant. the main
    ///     restrictions on these constants are that rdwarf**2 not
    ///     underflow and rgiant**2 not overflow. the constants
    ///     given here are suitable for every known computer.
    ///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
    /// </remarks>
    public static double L2Norm(this IReadOnlyList<float> vector)
    {
      return L2Norm(vector, 0, vector.Count);
    }


		 /// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
    /// <param name="vector">An input array of length n. </param>
    /// <returns>The euclidian norm of the vector, i.e. the square root of the sum of squares of the elements.</returns>
    /// <remarks>
    ///     the euclidean norm is computed by accumulating the sum of
    ///     squares in three different sums. the sums of squares for the
    ///     small and large components are scaled so that no overflows
    ///     occur. non-destructive underflows are permitted. underflows
    ///     and overflows do not occur in the computation of the unscaled
    ///     sum of squares for the intermediate components.
    ///     the definitions of small, intermediate and large components
    ///     depend on two constants, rdwarf and rgiant. the main
    ///     restrictions on these constants are that rdwarf**2 not
    ///     underflow and rgiant**2 not overflow. the constants
    ///     given here are suitable for every known computer.
    ///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
    /// </remarks>
    public static double EuclideanNorm(this IReadOnlyList<float> vector)
    {
      return L2Norm(vector, 0, vector.Count);
    }


		/// <summary>
		/// Returns the L-infinity norm of the provided <paramref name="vector"/> (as is the maximum of the absolute value of the elements). If one
		/// of the elements of the vector is invalid, the return value is also invalid (for the floating point types).
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The L-infinity norm of the provided <paramref name="vector"/> (as is the maximum of the absolute value of the elements).</returns>
		public static float LInfinityNorm(this IReadOnlyList<float> vector)
		{
			float max = 0;
			int i;
			for (i = vector.Count -1; i>=0; --i)
			{
				var temp = Math.Abs(vector[i]);
				if (!(max>temp)) // if we have a double.NaN here, this expression is true and max <- double.NaN
				{
					max = temp;

					if(float.IsNaN(max))
						break;

				}
			}
			
			return max;
		}

			/// <summary>
		/// Returns the L-infinity norm of the difference of <paramref name="vector1"/> and <paramref name="vector2"/> (as is the maximum of the absolute value of the differences of the elements). If one
		/// of the elements of the vector is invalid, the return value is also invalid (for the floating point types).
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The second vector. Must have same length as the first vector.</param>
		/// <returns>The L-infinity norm of the element-wise differences of the provided vectors (as is the maximum of the absolute value of the differences).</returns>
		public static float LInfinityNorm(IReadOnlyList<float> vector1, IReadOnlyList<float> vector2)
		{
			if( vector1.Count != vector2.Count)
				throw new RankException("Length of vector 1 must match length of vector 2");

			float max = 0;
			int i;
			for (i = vector1.Count - 1; i >= 0; --i)
			{
				var temp = Math.Abs(vector1[i] - vector2[i]);
				if (!(max>temp)) // if we have a double.NaN here, this expression is true and max <- double.NaN
				{
					max = temp;

					if(float.IsNaN(max))
						break;

				}
			}
			
			return max;
		}

		///<summary>Compute the p Norm of this vector.</summary>
		/// <param name="vector">The vector.</param>
    ///<param name="p">Order of the norm.</param>
		///<returns>The p norm of the vector.</returns>
		///<remarks>p &gt; 0, if p &lt; 0, ABS(p) is used. If p = 0 or positive infinity, the infinity norm is returned.</remarks>
		public static double LpNorm(this IReadOnlyList<float> vector, double p)
		{
		if (p == 0 )
			{
        return LInfinityNorm(vector);
      }
			else if(p==1)
			{
				return L1Norm(vector);
			}
			else if(p == 2)
			{
				return L2Norm(vector);
			}

			if (p < 0)
			{
				p = -p;
			}
			double ret = 0;
			for (int i = vector.Count-1; i >=0; --i)
			{
				ret += System.Math.Pow(Math.Abs(vector[i]), p);
			}
			return (float)System.Math.Pow(ret, 1 / p);
		}

		


		/// <summary>
		/// Returns the maximum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The maximum of all elements in <paramref name="vector"/>.</returns>
		public static float Max(this IReadOnlyList<float> vector)
		{
			if (vector.Count == 0)
				throw new ArgumentException("Result undefined for vector of zero length", nameof(vector));

			float max = float.NegativeInfinity;
			float tmp;
			for (int i = 0; i < vector.Count; ++i)
			{
				tmp = vector[i];
				if (tmp > max)
					max = tmp;
			}
			return max;
		}

			/// <summary>
		/// Returns the maximum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="start">First element of the vector to include in the evaluation.</param>
		/// <param name="count">Number of elements of the vector to include in the evaluation.</param>
		/// <returns>The maximum of all elements in <paramref name="vector"/>.</returns>
		public static float Max(this IReadOnlyList<float> vector, int start, int count)
		{
			if (!(start >= 0))
				throw new ArgumentException("must be >=0", nameof(start));
			if (!(count > 0))
				throw new ArgumentException("Result undefined for count <=0", nameof(count));
			int end = start + count;
			if(!(end<=vector.Count))
				throw new ArgumentException("must be <= vector.count - start", nameof(count));

			float max = float.NegativeInfinity;
			float tmp;
			for (int i = start; i < end; ++i)
			{
				tmp = vector[i];
				if (tmp > max)
					max = tmp;
			}
			return max;
		}

		/// <summary>
		/// Returns the minimum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>The minimum of all elements in <paramref name="vector"/>.</returns>
		public static float Min(this IReadOnlyList<float> vector)
		{
			if (vector.Count == 0)
				throw new ArgumentException("Result undefined for vector of zero length", nameof(vector));

			float min = float.PositiveInfinity;
			float tmp;
			for (int i = 0; i < vector.Count; ++i)
			{
				tmp = vector[i];
				if (tmp < min)
					min = tmp;
			}
			return min;
		}

		/// <summary>
		/// Returns the minimum of the elements in <paramref name="vector"/>.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="start">First element of the vector to include in the evaluation.</param>
		/// <param name="count">Number of elements of the vector to include in the evaluation.</param>
		/// <returns>The minimum of all elements in <paramref name="vector"/>.</returns>
		public static float Min(this IReadOnlyList<float> vector, int start, int count)
		{
			if (!(start >= 0))
				throw new ArgumentException("must be >=0", nameof(start));
			if (!(count > 0))
				throw new ArgumentException("Result undefined for count <=0", nameof(count));
			int end = start + count;
			if(!(end<=vector.Count))
				throw new ArgumentException("must be <= vector.count - start", nameof(count));

			float min = float.PositiveInfinity;
			float tmp;
			for (int i = start; i < end; ++i)
			{
				tmp = vector[i];
				if (tmp < min)
					min = tmp;
			}
			return min;
		}


    /// <summary>
    /// Returns the sum of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The sum of all elements in <paramref name="vector"/>.</returns>
    public static float Sum(this IReadOnlyList<float> vector)
    {
      float sum = 0;
      for (int i = 0; i < vector.Count; ++i)
        sum += vector[i];

      return sum; 
    }


		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The average of all elements in <paramref name="vector"/>.</returns>
    public static double Average(this IReadOnlyList<float> vector)
    {
      float sum = 0;
      for (int i = 0; i < vector.Count; ++i)
        sum += vector[i];

      return sum / (double)vector.Count; 
    }

		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The average of all elements in <paramref name="vector"/>.</returns>
    public static double Mean(this IReadOnlyList<float> vector)
    {
			return Average(vector);
		}


		/// <summary>
    /// Returns the average (=sum/N) of the elements in <paramref name="vector"/>, as well as the variance (sum of squares of the mean centered values divided by length of the vector).
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The mean and variance of all elements in <paramref name="vector"/>.</returns>
    public static (double Mean, double Variance) MeanAndVariance(this IReadOnlyList<float> vector)
    {
			var mean = Mean(vector);

			double sum = 0;
      for (int i = 0; i < vector.Count; ++i)
        {
				var diff = vector[i] - mean;
				sum += (diff*diff);
				}

      return (mean, sum / (double)vector.Count);
		}

		/// <summary>
    /// Returns the kurtosis of the elements in <paramref name="vector"/>. The kurtosis is defined as
		/// kurtosis(X) = E{(X-µ)^4}/((E{(X-µ)²})².
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The kurtosis of the elements in <paramref name="vector"/>.</returns>
		public static double Kurtosis(this IReadOnlyList<float> vector)
    {
			var N = vector.Count;
			double sum = 0;
      for (int i = N-1; i>=0; --i)
        {
				sum += vector[i];
				}
			var mean = sum/N;

			double sumy2 = 0;
      double sumy4 = 0;
      for (int i = N-1; i>=0; --i)
        {
				var e = vector[i]-mean;
				var e2 = e*e;
				sumy2 += e2;
				sumy4 += e2*e2;
				}


      return N*sumy4/(sumy2*sumy2);
    }

		/// <summary>
    /// Returns the excess kurtosis of the elements in <paramref name="vector"/>. The excess kurtosis is defined as
		/// excesskurtosis(X) = E{X^4} - 3(E{X²})². 
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The excess kurtosis of the elements in <paramref name="vector"/>.</returns>
    public static double ExcessKurtosisOfNormalized(this IReadOnlyList<float> vector)
    {
      double sumy4 = 0;
			double sumy2 = 0;
      for (int i = 0; i < vector.Count; ++i)
        {
				var e = vector[i];
				var e2 = e*e;
				sumy2 += e2;
				sumy4 += e2*e2;
				}

			var N = vector.Count;

      return sumy4/N -3*RMath.Pow2(sumy2/N); 
    }


	} // class
} // namespace
