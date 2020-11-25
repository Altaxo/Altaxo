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
		/// Elementwise application of a function to each element of a vector. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="function">The function to apply to every element.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(double[] src1, Func<double, double> function, double[] result)
		{
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i]);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(double[] src1, double[] src2, Func<double, double, double> function, double[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i]);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(double[] src1, double[] src2,  double[] src3, Func<double, double, double, double> function, double[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != src3.Length)
				throw new RankException("Mismatch of dimensions of src1 and src3");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], src3[i]);
		}

		// -------------------- Map with one auxillary parameter -------------------------------------

		/// <summary>
		/// Elementwise application of a function to each element of a vector. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st arg is the element of vector src1, 2nd argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(double[] src1, T1 parameter1, Func<double, T1, double> function, double[] result)
		{
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], parameter1);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(double[] src1, double[] src2, T1 parameter1, Func<double, double, T1, double> function, double[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], parameter1);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3, 4th argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(double[] src1, double[] src2,  double[] src3, T1 parameter1, Func<double, double, double, T1, double> function, double[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != src3.Length)
				throw new RankException("Mismatch of dimensions of src1 and src3");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], src3[i], parameter1);
		}



		/// <summary>
		/// Elementwise application of a function to each element of a vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element, 2nd argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may be the same instance as the source vector.</param>
		public static void MapIndexed(double[] src1, Func<int, double, double> function, double[] result)
		{
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(i, src1[i]);
		}

	

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void MapIndexed(double[] src1, double[] src2, Func<int, double, double, double> function, double[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(i, src1[i], src2[i]);
		}


		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3, 4th argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void MapIndexed(double[] src1, double[] src2, double[] src3, Func<int, double, double, double, double> function, double[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(i, src1[i], src2[i], src3[i]);
		}


    /// <summary>
    /// Adds (elementwise) two vectors a and b and stores the result in c. All vectors must have the same length.
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="c">The resulting vector.</param>
    public static void Add(double[] a, double[] b, double[] c)
    {
      if (a.Length != b.Length)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Length != b.Length)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Length - 1; i >= 0; --i)
        c[i] = a[i] + b[i];
    }


    /// <summary>
    /// Adds (elementwise) two vectors a and (b scaled with scaleb) and stores the result in c, i.e. c = a + b * scaleb. All vectors must have the same length.
    /// The vectors a or b may be identical (the same instance) as c. 
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="scaleb">Scale factor for vector b.</param>
    /// <param name="c">The resulting vector calculated as a + b * scaleb.</param>
    public static void AddScaled(double[] a, double[] b, double scaleb, double[] c)
    {
      if (a.Length != b.Length)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Length != b.Length)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Length - 1; i >= 0; --i)
        c[i] = a[i] + b[i] * scaleb;
    }



		/// <summary>
		/// Returns true if and only if both vectors contain the same elements. Both vectors must have the same length.
		/// </summary>
		/// <param name="vector1">First vector.</param>
		/// <param name="vector2">Second vector.</param>
		/// <returns>True if both vectors contain the same elements.</returns>
		public static bool AreValuesEqual(double[] vector1, double[] vector2)
		{
			if (vector1.Length != vector2.Length)
				throw new ArgumentException("Length of x and y are unequal");

			for (int i = vector1.Length - 1; i >= 0; --i)
			{
				if (!(vector1[i] == vector2[i]))
				{
					return false;
				}
			}
			return true;
		}


		/// <summary>Performs linear interpolation between two vectors at specified points.</summary>
		/// <param name="t">Scalar parameter of interpolation</param>
		/// <param name="t0">Scalar parameter corresponding to first vector v0.</param>
		/// <param name="v0">Vector at first parameter point.</param>
		/// <param name="t1">Scalar parameter corresponding to the second vector v1.</param>
		/// <param name="v1">Vector at second parameter point.</param>
		/// <param name="destinationVector">Intepolated vector value at point <paramref name="t"/></param>
		public static void Lerp(double t, double t0, double[] v0, double t1, double[] v1, double[] destinationVector)
		{
			if(v0 is null)
				throw new ArgumentNullException(nameof(v0));
			if(v1 is null)
				throw new ArgumentNullException(nameof(v1));
			if(destinationVector is null)
				throw new ArgumentNullException(nameof(destinationVector));
			if (v0.Length != v1.Length)
        throw new ArgumentException("Length of vectors v0 and v1 unequal");
      if (destinationVector.Length != v0.Length)
        throw new ArgumentException("Length of vectors destinationVector and v0 unequal");

			for(int i=v0.Length -1; i>=0; --i)
				{
				destinationVector[i] = (double)((v0[i] * (t1 - t) + v1[i] * (t - t0)) / (t1 - t0));
				}
		}


     /// <summary>
    /// Multiplies (elementwise) two vectors a and b and stores the result in c. All vectors must have the same length.
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="c">The resulting vector.</param>
    public static void Multiply(double[] a, double[] b, double[] c)
    {
      if (a.Length != b.Length)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Length != b.Length)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Length - 1; i >= 0; --i)
        c[i] = a[i] * b[i];
    }

    /// <summary>
    /// Gives the parallel maximum of vector a and b. The first element of the resulting vector
    /// is the maximum of the first element of a and the first element of b. The second element of the
    /// resulting vector is the maximum of the second element of a and the second element of b, and so on.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <param name="result">The resulting vector.</param>
		/// <returns>The same instance as provided by result. Each element result[i] is the maximum of a[i] and b[i].</returns>
    public static double[] MaxOf(double[] a, double[] b, double[] result)
    {
      if (a.Length != b.Length)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (result.Length != b.Length)
        throw new ArgumentException("Length of vectors a and result unequal");

      for (int i = result.Length - 1; i >= 0; --i)
      {
        result[i] = Math.Max(a[i], b[i]);
      }

			return result;
    }


		/// <summary>
		/// Creates a new vector, whose elements are the maximum of the elements of a given input vector and a given number.
		/// </summary>
		/// <param name="vector1">Input vector.</param>
		/// <param name="scalar">Number.</param>
		/// <param name="result">Provide a vector whose elements are filled with the result. If null is provided, then a new vector will be allocated and returned.</param>
		/// <returns>The same instance as provided by result. Each element r[i] is the maximum of v[i] and b.</returns>
		public static double[] MaxOf(this double[] vector1, double scalar, double[] result)
		{
			if (vector1.Length != result.Length)
				throw new ArgumentException("vector1 and result must have the same length");

			for (int i = vector1.Length; i>=0; --i)
				result[i] = Math.Max(vector1[i], scalar);

			return result;
		}


		/// <summary>
		/// Returns the sum of squared differences of the elements of xarray and yarray.
		/// </summary>
		/// <param name="vector1">The first array.</param>
		/// <param name="vector2">The other array.</param>
		/// <returns>The sum of squared differences all elements of xarray and yarray.</returns>
		public static double SumOfSquaredDifferences(double[] vector1, double[] vector2)
		{
			if (vector1.Length != vector2.Length)
				throw new ArgumentException("Length of vector1 is different from length of vector2");

			double sum = 0;
			double tmp;
			for (int i = 0; i < vector1.Length; i++)
			{
				tmp = (double)vector1[i] - vector2[i];
				sum += tmp * tmp;
			}

			return sum;
		}

        /// <summary>
		/// Returns the Euclidean distance of two vectors, i.e. the L2-norm of the difference of the two vectors.
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The other vector.</param>
		/// <returns>The Euclidean distance of vector1 and vector2.</returns>
		public static double EuclideanDistance(double[] vector1, double[] vector2)
		{
			return Math.Sqrt(SumOfSquaredDifferences(vector1, vector2));
		}



		/// <summary>
		/// Returns the dot product of vector1 and vector2.
		/// </summary>
		/// <param name="vector1">The first array.</param>
		/// <param name="vector2">The other array.</param>
		/// <returns>The dot product (sum of vector1[i]*vector2[i].</returns>
		public static double DotProduct(double[] vector1, double[] vector2)
		{
			if (vector1.Length != vector2.Length)
				throw new ArgumentException("Length of vector1 is different from length of vector2");

			double sum = 0;
			for (int i = vector1.Length-1; i>=0; --i)
			{
				sum += ((double)vector1[i]) * vector2[i];
			}

			return sum;
		}


// ******************************************** Definitions for float[] *******************************************

		/// <summary>
		/// Elementwise application of a function to each element of a vector. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="function">The function to apply to every element.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(float[] src1, Func<float, float> function, float[] result)
		{
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i]);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(float[] src1, float[] src2, Func<float, float, float> function, float[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i]);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(float[] src1, float[] src2,  float[] src3, Func<float, float, float, float> function, float[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != src3.Length)
				throw new RankException("Mismatch of dimensions of src1 and src3");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], src3[i]);
		}

		// -------------------- Map with one auxillary parameter -------------------------------------

		/// <summary>
		/// Elementwise application of a function to each element of a vector. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st arg is the element of vector src1, 2nd argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(float[] src1, T1 parameter1, Func<float, T1, float> function, float[] result)
		{
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], parameter1);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(float[] src1, float[] src2, T1 parameter1, Func<float, float, T1, float> function, float[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], parameter1);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3, 4th argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(float[] src1, float[] src2,  float[] src3, T1 parameter1, Func<float, float, float, T1, float> function, float[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != src3.Length)
				throw new RankException("Mismatch of dimensions of src1 and src3");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], src3[i], parameter1);
		}



		/// <summary>
		/// Elementwise application of a function to each element of a vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element, 2nd argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may be the same instance as the source vector.</param>
		public static void MapIndexed(float[] src1, Func<int, float, float> function, float[] result)
		{
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(i, src1[i]);
		}

	

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void MapIndexed(float[] src1, float[] src2, Func<int, float, float, float> function, float[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(i, src1[i], src2[i]);
		}


		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3, 4th argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void MapIndexed(float[] src1, float[] src2, float[] src3, Func<int, float, float, float, float> function, float[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(i, src1[i], src2[i], src3[i]);
		}


    /// <summary>
    /// Adds (elementwise) two vectors a and b and stores the result in c. All vectors must have the same length.
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="c">The resulting vector.</param>
    public static void Add(float[] a, float[] b, float[] c)
    {
      if (a.Length != b.Length)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Length != b.Length)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Length - 1; i >= 0; --i)
        c[i] = a[i] + b[i];
    }


    /// <summary>
    /// Adds (elementwise) two vectors a and (b scaled with scaleb) and stores the result in c, i.e. c = a + b * scaleb. All vectors must have the same length.
    /// The vectors a or b may be identical (the same instance) as c. 
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="scaleb">Scale factor for vector b.</param>
    /// <param name="c">The resulting vector calculated as a + b * scaleb.</param>
    public static void AddScaled(float[] a, float[] b, float scaleb, float[] c)
    {
      if (a.Length != b.Length)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Length != b.Length)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Length - 1; i >= 0; --i)
        c[i] = a[i] + b[i] * scaleb;
    }



		/// <summary>
		/// Returns true if and only if both vectors contain the same elements. Both vectors must have the same length.
		/// </summary>
		/// <param name="vector1">First vector.</param>
		/// <param name="vector2">Second vector.</param>
		/// <returns>True if both vectors contain the same elements.</returns>
		public static bool AreValuesEqual(float[] vector1, float[] vector2)
		{
			if (vector1.Length != vector2.Length)
				throw new ArgumentException("Length of x and y are unequal");

			for (int i = vector1.Length - 1; i >= 0; --i)
			{
				if (!(vector1[i] == vector2[i]))
				{
					return false;
				}
			}
			return true;
		}


		/// <summary>Performs linear interpolation between two vectors at specified points.</summary>
		/// <param name="t">Scalar parameter of interpolation</param>
		/// <param name="t0">Scalar parameter corresponding to first vector v0.</param>
		/// <param name="v0">Vector at first parameter point.</param>
		/// <param name="t1">Scalar parameter corresponding to the second vector v1.</param>
		/// <param name="v1">Vector at second parameter point.</param>
		/// <param name="destinationVector">Intepolated vector value at point <paramref name="t"/></param>
		public static void Lerp(double t, double t0, float[] v0, double t1, float[] v1, float[] destinationVector)
		{
			if(v0 is null)
				throw new ArgumentNullException(nameof(v0));
			if(v1 is null)
				throw new ArgumentNullException(nameof(v1));
			if(destinationVector is null)
				throw new ArgumentNullException(nameof(destinationVector));
			if (v0.Length != v1.Length)
        throw new ArgumentException("Length of vectors v0 and v1 unequal");
      if (destinationVector.Length != v0.Length)
        throw new ArgumentException("Length of vectors destinationVector and v0 unequal");

			for(int i=v0.Length -1; i>=0; --i)
				{
				destinationVector[i] = (float)((v0[i] * (t1 - t) + v1[i] * (t - t0)) / (t1 - t0));
				}
		}


     /// <summary>
    /// Multiplies (elementwise) two vectors a and b and stores the result in c. All vectors must have the same length.
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="c">The resulting vector.</param>
    public static void Multiply(float[] a, float[] b, float[] c)
    {
      if (a.Length != b.Length)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Length != b.Length)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Length - 1; i >= 0; --i)
        c[i] = a[i] * b[i];
    }

    /// <summary>
    /// Gives the parallel maximum of vector a and b. The first element of the resulting vector
    /// is the maximum of the first element of a and the first element of b. The second element of the
    /// resulting vector is the maximum of the second element of a and the second element of b, and so on.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <param name="result">The resulting vector.</param>
		/// <returns>The same instance as provided by result. Each element result[i] is the maximum of a[i] and b[i].</returns>
    public static float[] MaxOf(float[] a, float[] b, float[] result)
    {
      if (a.Length != b.Length)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (result.Length != b.Length)
        throw new ArgumentException("Length of vectors a and result unequal");

      for (int i = result.Length - 1; i >= 0; --i)
      {
        result[i] = Math.Max(a[i], b[i]);
      }

			return result;
    }


		/// <summary>
		/// Creates a new vector, whose elements are the maximum of the elements of a given input vector and a given number.
		/// </summary>
		/// <param name="vector1">Input vector.</param>
		/// <param name="scalar">Number.</param>
		/// <param name="result">Provide a vector whose elements are filled with the result. If null is provided, then a new vector will be allocated and returned.</param>
		/// <returns>The same instance as provided by result. Each element r[i] is the maximum of v[i] and b.</returns>
		public static float[] MaxOf(this float[] vector1, float scalar, float[] result)
		{
			if (vector1.Length != result.Length)
				throw new ArgumentException("vector1 and result must have the same length");

			for (int i = vector1.Length; i>=0; --i)
				result[i] = Math.Max(vector1[i], scalar);

			return result;
		}


		/// <summary>
		/// Returns the sum of squared differences of the elements of xarray and yarray.
		/// </summary>
		/// <param name="vector1">The first array.</param>
		/// <param name="vector2">The other array.</param>
		/// <returns>The sum of squared differences all elements of xarray and yarray.</returns>
		public static double SumOfSquaredDifferences(float[] vector1, float[] vector2)
		{
			if (vector1.Length != vector2.Length)
				throw new ArgumentException("Length of vector1 is different from length of vector2");

			double sum = 0;
			double tmp;
			for (int i = 0; i < vector1.Length; i++)
			{
				tmp = (double)vector1[i] - vector2[i];
				sum += tmp * tmp;
			}

			return sum;
		}

        /// <summary>
		/// Returns the Euclidean distance of two vectors, i.e. the L2-norm of the difference of the two vectors.
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The other vector.</param>
		/// <returns>The Euclidean distance of vector1 and vector2.</returns>
		public static double EuclideanDistance(float[] vector1, float[] vector2)
		{
			return Math.Sqrt(SumOfSquaredDifferences(vector1, vector2));
		}



		/// <summary>
		/// Returns the dot product of vector1 and vector2.
		/// </summary>
		/// <param name="vector1">The first array.</param>
		/// <param name="vector2">The other array.</param>
		/// <returns>The dot product (sum of vector1[i]*vector2[i].</returns>
		public static double DotProduct(float[] vector1, float[] vector2)
		{
			if (vector1.Length != vector2.Length)
				throw new ArgumentException("Length of vector1 is different from length of vector2");

			double sum = 0;
			for (int i = vector1.Length-1; i>=0; --i)
			{
				sum += ((double)vector1[i]) * vector2[i];
			}

			return sum;
		}


// ******************************************** Definitions for int[] *******************************************

		/// <summary>
		/// Elementwise application of a function to each element of a vector. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="function">The function to apply to every element.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(int[] src1, Func<int, int> function, int[] result)
		{
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i]);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(int[] src1, int[] src2, Func<int, int, int> function, int[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i]);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(int[] src1, int[] src2,  int[] src3, Func<int, int, int, int> function, int[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != src3.Length)
				throw new RankException("Mismatch of dimensions of src1 and src3");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], src3[i]);
		}

		// -------------------- Map with one auxillary parameter -------------------------------------

		/// <summary>
		/// Elementwise application of a function to each element of a vector. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st arg is the element of vector src1, 2nd argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(int[] src1, T1 parameter1, Func<int, T1, int> function, int[] result)
		{
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], parameter1);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(int[] src1, int[] src2, T1 parameter1, Func<int, int, T1, int> function, int[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], parameter1);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3, 4th argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(int[] src1, int[] src2,  int[] src3, T1 parameter1, Func<int, int, int, T1, int> function, int[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != src3.Length)
				throw new RankException("Mismatch of dimensions of src1 and src3");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], src3[i], parameter1);
		}



		/// <summary>
		/// Elementwise application of a function to each element of a vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element, 2nd argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may be the same instance as the source vector.</param>
		public static void MapIndexed(int[] src1, Func<int, int, int> function, int[] result)
		{
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(i, src1[i]);
		}

	

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void MapIndexed(int[] src1, int[] src2, Func<int, int, int, int> function, int[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(i, src1[i], src2[i]);
		}


		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3, 4th argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void MapIndexed(int[] src1, int[] src2, int[] src3, Func<int, int, int, int, int> function, int[] result)
		{
			if (src1.Length != src2.Length)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Length != result.Length)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Length - 1; i >= 0; --i)
				result[i] = function(i, src1[i], src2[i], src3[i]);
		}


    /// <summary>
    /// Adds (elementwise) two vectors a and b and stores the result in c. All vectors must have the same length.
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="c">The resulting vector.</param>
    public static void Add(int[] a, int[] b, int[] c)
    {
      if (a.Length != b.Length)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Length != b.Length)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Length - 1; i >= 0; --i)
        c[i] = a[i] + b[i];
    }


    /// <summary>
    /// Adds (elementwise) two vectors a and (b scaled with scaleb) and stores the result in c, i.e. c = a + b * scaleb. All vectors must have the same length.
    /// The vectors a or b may be identical (the same instance) as c. 
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="scaleb">Scale factor for vector b.</param>
    /// <param name="c">The resulting vector calculated as a + b * scaleb.</param>
    public static void AddScaled(int[] a, int[] b, int scaleb, int[] c)
    {
      if (a.Length != b.Length)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Length != b.Length)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Length - 1; i >= 0; --i)
        c[i] = a[i] + b[i] * scaleb;
    }



		/// <summary>
		/// Returns true if and only if both vectors contain the same elements. Both vectors must have the same length.
		/// </summary>
		/// <param name="vector1">First vector.</param>
		/// <param name="vector2">Second vector.</param>
		/// <returns>True if both vectors contain the same elements.</returns>
		public static bool AreValuesEqual(int[] vector1, int[] vector2)
		{
			if (vector1.Length != vector2.Length)
				throw new ArgumentException("Length of x and y are unequal");

			for (int i = vector1.Length - 1; i >= 0; --i)
			{
				if (!(vector1[i] == vector2[i]))
				{
					return false;
				}
			}
			return true;
		}


		/// <summary>Performs linear interpolation between two vectors at specified points.</summary>
		/// <param name="t">Scalar parameter of interpolation</param>
		/// <param name="t0">Scalar parameter corresponding to first vector v0.</param>
		/// <param name="v0">Vector at first parameter point.</param>
		/// <param name="t1">Scalar parameter corresponding to the second vector v1.</param>
		/// <param name="v1">Vector at second parameter point.</param>
		/// <param name="destinationVector">Intepolated vector value at point <paramref name="t"/></param>
		public static void Lerp(double t, double t0, int[] v0, double t1, int[] v1, int[] destinationVector)
		{
			if(v0 is null)
				throw new ArgumentNullException(nameof(v0));
			if(v1 is null)
				throw new ArgumentNullException(nameof(v1));
			if(destinationVector is null)
				throw new ArgumentNullException(nameof(destinationVector));
			if (v0.Length != v1.Length)
        throw new ArgumentException("Length of vectors v0 and v1 unequal");
      if (destinationVector.Length != v0.Length)
        throw new ArgumentException("Length of vectors destinationVector and v0 unequal");

			for(int i=v0.Length -1; i>=0; --i)
				{
				destinationVector[i] = (int)((v0[i] * (t1 - t) + v1[i] * (t - t0)) / (t1 - t0));
				}
		}


     /// <summary>
    /// Multiplies (elementwise) two vectors a and b and stores the result in c. All vectors must have the same length.
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="c">The resulting vector.</param>
    public static void Multiply(int[] a, int[] b, int[] c)
    {
      if (a.Length != b.Length)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Length != b.Length)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Length - 1; i >= 0; --i)
        c[i] = a[i] * b[i];
    }

    /// <summary>
    /// Gives the parallel maximum of vector a and b. The first element of the resulting vector
    /// is the maximum of the first element of a and the first element of b. The second element of the
    /// resulting vector is the maximum of the second element of a and the second element of b, and so on.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <param name="result">The resulting vector.</param>
		/// <returns>The same instance as provided by result. Each element result[i] is the maximum of a[i] and b[i].</returns>
    public static int[] MaxOf(int[] a, int[] b, int[] result)
    {
      if (a.Length != b.Length)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (result.Length != b.Length)
        throw new ArgumentException("Length of vectors a and result unequal");

      for (int i = result.Length - 1; i >= 0; --i)
      {
        result[i] = Math.Max(a[i], b[i]);
      }

			return result;
    }


		/// <summary>
		/// Creates a new vector, whose elements are the maximum of the elements of a given input vector and a given number.
		/// </summary>
		/// <param name="vector1">Input vector.</param>
		/// <param name="scalar">Number.</param>
		/// <param name="result">Provide a vector whose elements are filled with the result. If null is provided, then a new vector will be allocated and returned.</param>
		/// <returns>The same instance as provided by result. Each element r[i] is the maximum of v[i] and b.</returns>
		public static int[] MaxOf(this int[] vector1, int scalar, int[] result)
		{
			if (vector1.Length != result.Length)
				throw new ArgumentException("vector1 and result must have the same length");

			for (int i = vector1.Length; i>=0; --i)
				result[i] = Math.Max(vector1[i], scalar);

			return result;
		}


		/// <summary>
		/// Returns the sum of squared differences of the elements of xarray and yarray.
		/// </summary>
		/// <param name="vector1">The first array.</param>
		/// <param name="vector2">The other array.</param>
		/// <returns>The sum of squared differences all elements of xarray and yarray.</returns>
		public static double SumOfSquaredDifferences(int[] vector1, int[] vector2)
		{
			if (vector1.Length != vector2.Length)
				throw new ArgumentException("Length of vector1 is different from length of vector2");

			double sum = 0;
			double tmp;
			for (int i = 0; i < vector1.Length; i++)
			{
				tmp = (double)vector1[i] - vector2[i];
				sum += tmp * tmp;
			}

			return sum;
		}

        /// <summary>
		/// Returns the Euclidean distance of two vectors, i.e. the L2-norm of the difference of the two vectors.
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The other vector.</param>
		/// <returns>The Euclidean distance of vector1 and vector2.</returns>
		public static double EuclideanDistance(int[] vector1, int[] vector2)
		{
			return Math.Sqrt(SumOfSquaredDifferences(vector1, vector2));
		}



		/// <summary>
		/// Returns the dot product of vector1 and vector2.
		/// </summary>
		/// <param name="vector1">The first array.</param>
		/// <param name="vector2">The other array.</param>
		/// <returns>The dot product (sum of vector1[i]*vector2[i].</returns>
		public static double DotProduct(int[] vector1, int[] vector2)
		{
			if (vector1.Length != vector2.Length)
				throw new ArgumentException("Length of vector1 is different from length of vector2");

			double sum = 0;
			for (int i = vector1.Length-1; i>=0; --i)
			{
				sum += ((double)vector1[i]) * vector2[i];
			}

			return sum;
		}


// ******************************************** Definitions for IReadOnlyList<double> *******************************************

		/// <summary>
		/// Elementwise application of a function to each element of a vector. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="function">The function to apply to every element.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(IReadOnlyList<double> src1, Func<double, double> function, IVector<double> result)
		{
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(src1[i]);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(IReadOnlyList<double> src1, IReadOnlyList<double> src2, Func<double, double, double> function, IVector<double> result)
		{
			if (src1.Count != src2.Count)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i]);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(IReadOnlyList<double> src1, IReadOnlyList<double> src2,  IReadOnlyList<double> src3, Func<double, double, double, double> function, IVector<double> result)
		{
			if (src1.Count != src2.Count)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Count != src3.Count)
				throw new RankException("Mismatch of dimensions of src1 and src3");
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], src3[i]);
		}

		// -------------------- Map with one auxillary parameter -------------------------------------

		/// <summary>
		/// Elementwise application of a function to each element of a vector. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st arg is the element of vector src1, 2nd argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(IReadOnlyList<double> src1, T1 parameter1, Func<double, T1, double> function, IVector<double> result)
		{
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(src1[i], parameter1);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(IReadOnlyList<double> src1, IReadOnlyList<double> src2, T1 parameter1, Func<double, double, T1, double> function, IVector<double> result)
		{
			if (src1.Count != src2.Count)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], parameter1);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3, 4th argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(IReadOnlyList<double> src1, IReadOnlyList<double> src2,  IReadOnlyList<double> src3, T1 parameter1, Func<double, double, double, T1, double> function, IVector<double> result)
		{
			if (src1.Count != src2.Count)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Count != src3.Count)
				throw new RankException("Mismatch of dimensions of src1 and src3");
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], src3[i], parameter1);
		}



		/// <summary>
		/// Elementwise application of a function to each element of a vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element, 2nd argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may be the same instance as the source vector.</param>
		public static void MapIndexed(IReadOnlyList<double> src1, Func<int, double, double> function, IVector<double> result)
		{
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(i, src1[i]);
		}

	

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void MapIndexed(IReadOnlyList<double> src1, IReadOnlyList<double> src2, Func<int, double, double, double> function, IVector<double> result)
		{
			if (src1.Count != src2.Count)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(i, src1[i], src2[i]);
		}


		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3, 4th argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void MapIndexed(IReadOnlyList<double> src1, IReadOnlyList<double> src2, IReadOnlyList<double> src3, Func<int, double, double, double, double> function, IVector<double> result)
		{
			if (src1.Count != src2.Count)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(i, src1[i], src2[i], src3[i]);
		}


    /// <summary>
    /// Adds (elementwise) two vectors a and b and stores the result in c. All vectors must have the same length.
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="c">The resulting vector.</param>
    public static void Add(IReadOnlyList<double> a, IReadOnlyList<double> b, IVector<double> c)
    {
      if (a.Count != b.Count)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Count != b.Count)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Count - 1; i >= 0; --i)
        c[i] = a[i] + b[i];
    }


    /// <summary>
    /// Adds (elementwise) two vectors a and (b scaled with scaleb) and stores the result in c, i.e. c = a + b * scaleb. All vectors must have the same length.
    /// The vectors a or b may be identical (the same instance) as c. 
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="scaleb">Scale factor for vector b.</param>
    /// <param name="c">The resulting vector calculated as a + b * scaleb.</param>
    public static void AddScaled(IReadOnlyList<double> a, IReadOnlyList<double> b, double scaleb, IVector<double> c)
    {
      if (a.Count != b.Count)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Count != b.Count)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Count - 1; i >= 0; --i)
        c[i] = a[i] + b[i] * scaleb;
    }



		/// <summary>
		/// Returns true if and only if both vectors contain the same elements. Both vectors must have the same length.
		/// </summary>
		/// <param name="vector1">First vector.</param>
		/// <param name="vector2">Second vector.</param>
		/// <returns>True if both vectors contain the same elements.</returns>
		public static bool AreValuesEqual(IReadOnlyList<double> vector1, IReadOnlyList<double> vector2)
		{
			if (vector1.Count != vector2.Count)
				throw new ArgumentException("Length of x and y are unequal");

			for (int i = vector1.Count - 1; i >= 0; --i)
			{
				if (!(vector1[i] == vector2[i]))
				{
					return false;
				}
			}
			return true;
		}


		/// <summary>Performs linear interpolation between two vectors at specified points.</summary>
		/// <param name="t">Scalar parameter of interpolation</param>
		/// <param name="t0">Scalar parameter corresponding to first vector v0.</param>
		/// <param name="v0">Vector at first parameter point.</param>
		/// <param name="t1">Scalar parameter corresponding to the second vector v1.</param>
		/// <param name="v1">Vector at second parameter point.</param>
		/// <param name="destinationVector">Intepolated vector value at point <paramref name="t"/></param>
		public static void Lerp(double t, double t0, IReadOnlyList<double> v0, double t1, IReadOnlyList<double> v1, IVector<double> destinationVector)
		{
			if(v0 is null)
				throw new ArgumentNullException(nameof(v0));
			if(v1 is null)
				throw new ArgumentNullException(nameof(v1));
			if(destinationVector is null)
				throw new ArgumentNullException(nameof(destinationVector));
			if (v0.Count != v1.Count)
        throw new ArgumentException("Length of vectors v0 and v1 unequal");
      if (destinationVector.Count != v0.Count)
        throw new ArgumentException("Length of vectors destinationVector and v0 unequal");

			for(int i=v0.Count -1; i>=0; --i)
				{
				destinationVector[i] = (double)((v0[i] * (t1 - t) + v1[i] * (t - t0)) / (t1 - t0));
				}
		}


     /// <summary>
    /// Multiplies (elementwise) two vectors a and b and stores the result in c. All vectors must have the same length.
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="c">The resulting vector.</param>
    public static void Multiply(IReadOnlyList<double> a, IReadOnlyList<double> b, IVector<double> c)
    {
      if (a.Count != b.Count)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Count != b.Count)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Count - 1; i >= 0; --i)
        c[i] = a[i] * b[i];
    }

    /// <summary>
    /// Gives the parallel maximum of vector a and b. The first element of the resulting vector
    /// is the maximum of the first element of a and the first element of b. The second element of the
    /// resulting vector is the maximum of the second element of a and the second element of b, and so on.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <param name="result">The resulting vector.</param>
		/// <returns>The same instance as provided by result. Each element result[i] is the maximum of a[i] and b[i].</returns>
    public static IVector<double> MaxOf(IReadOnlyList<double> a, IReadOnlyList<double> b, IVector<double> result)
    {
      if (a.Count != b.Count)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (result.Count != b.Count)
        throw new ArgumentException("Length of vectors a and result unequal");

      for (int i = result.Count - 1; i >= 0; --i)
      {
        result[i] = Math.Max(a[i], b[i]);
      }

			return result;
    }


		/// <summary>
		/// Creates a new vector, whose elements are the maximum of the elements of a given input vector and a given number.
		/// </summary>
		/// <param name="vector1">Input vector.</param>
		/// <param name="scalar">Number.</param>
		/// <param name="result">Provide a vector whose elements are filled with the result. If null is provided, then a new vector will be allocated and returned.</param>
		/// <returns>The same instance as provided by result. Each element r[i] is the maximum of v[i] and b.</returns>
		public static IVector<double> MaxOf(this IReadOnlyList<double> vector1, double scalar, IVector<double> result)
		{
			if (vector1.Count != result.Count)
				throw new ArgumentException("vector1 and result must have the same length");

			for (int i = vector1.Count; i>=0; --i)
				result[i] = Math.Max(vector1[i], scalar);

			return result;
		}


		/// <summary>
		/// Returns the sum of squared differences of the elements of xarray and yarray.
		/// </summary>
		/// <param name="vector1">The first array.</param>
		/// <param name="vector2">The other array.</param>
		/// <returns>The sum of squared differences all elements of xarray and yarray.</returns>
		public static double SumOfSquaredDifferences(IReadOnlyList<double> vector1, IReadOnlyList<double> vector2)
		{
			if (vector1.Count != vector2.Count)
				throw new ArgumentException("Length of vector1 is different from length of vector2");

			double sum = 0;
			double tmp;
			for (int i = 0; i < vector1.Count; i++)
			{
				tmp = (double)vector1[i] - vector2[i];
				sum += tmp * tmp;
			}

			return sum;
		}

        /// <summary>
		/// Returns the Euclidean distance of two vectors, i.e. the L2-norm of the difference of the two vectors.
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The other vector.</param>
		/// <returns>The Euclidean distance of vector1 and vector2.</returns>
		public static double EuclideanDistance(IReadOnlyList<double> vector1, IReadOnlyList<double> vector2)
		{
			return Math.Sqrt(SumOfSquaredDifferences(vector1, vector2));
		}



		/// <summary>
		/// Returns the dot product of vector1 and vector2.
		/// </summary>
		/// <param name="vector1">The first array.</param>
		/// <param name="vector2">The other array.</param>
		/// <returns>The dot product (sum of vector1[i]*vector2[i].</returns>
		public static double DotProduct(IReadOnlyList<double> vector1, IReadOnlyList<double> vector2)
		{
			if (vector1.Count != vector2.Count)
				throw new ArgumentException("Length of vector1 is different from length of vector2");

			double sum = 0;
			for (int i = vector1.Count-1; i>=0; --i)
			{
				sum += ((double)vector1[i]) * vector2[i];
			}

			return sum;
		}


// ******************************************** Definitions for IReadOnlyList<float> *******************************************

		/// <summary>
		/// Elementwise application of a function to each element of a vector. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="function">The function to apply to every element.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(IReadOnlyList<float> src1, Func<float, float> function, IVector<float> result)
		{
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(src1[i]);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(IReadOnlyList<float> src1, IReadOnlyList<float> src2, Func<float, float, float> function, IVector<float> result)
		{
			if (src1.Count != src2.Count)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i]);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map(IReadOnlyList<float> src1, IReadOnlyList<float> src2,  IReadOnlyList<float> src3, Func<float, float, float, float> function, IVector<float> result)
		{
			if (src1.Count != src2.Count)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Count != src3.Count)
				throw new RankException("Mismatch of dimensions of src1 and src3");
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], src3[i]);
		}

		// -------------------- Map with one auxillary parameter -------------------------------------

		/// <summary>
		/// Elementwise application of a function to each element of a vector. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st arg is the element of vector src1, 2nd argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(IReadOnlyList<float> src1, T1 parameter1, Func<float, T1, float> function, IVector<float> result)
		{
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(src1[i], parameter1);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(IReadOnlyList<float> src1, IReadOnlyList<float> src2, T1 parameter1, Func<float, float, T1, float> function, IVector<float> result)
		{
			if (src1.Count != src2.Count)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], parameter1);
		}

		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3, 4th argument is the auxillary parameter1.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void Map<T1>(IReadOnlyList<float> src1, IReadOnlyList<float> src2,  IReadOnlyList<float> src3, T1 parameter1, Func<float, float, float, T1, float> function, IVector<float> result)
		{
			if (src1.Count != src2.Count)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Count != src3.Count)
				throw new RankException("Mismatch of dimensions of src1 and src3");
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(src1[i], src2[i], src3[i], parameter1);
		}



		/// <summary>
		/// Elementwise application of a function to each element of a vector.
		/// </summary>
		/// <param name="src1">The source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element, 2nd argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may be the same instance as the source vector.</param>
		public static void MapIndexed(IReadOnlyList<float> src1, Func<int, float, float> function, IVector<float> result)
		{
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(i, src1[i]);
		}

	

		/// <summary>
		/// Elementwise application of a function to corresponding elements of two vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void MapIndexed(IReadOnlyList<float> src1, IReadOnlyList<float> src2, Func<int, float, float, float> function, IVector<float> result)
		{
			if (src1.Count != src2.Count)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(i, src1[i], src2[i]);
		}


		/// <summary>
		/// Elementwise application of a function to corresponding elements of three vectors. The result is stored in another vector or in the same vector.
		/// </summary>
		/// <param name="src1">First source vector.</param>
		/// <param name="src2">Second source vector.</param>
		/// <param name="src3">Third source vector.</param>
		/// <param name="function">The function to apply to every element. 1st argument is the element from src1, 2nd argument is the element from src2, 3rd argument is the element from src3, 4th argument is the element's index.</param>
		/// <param name="result">The vector to store the results. This may or may not be the same instance as the source vector.</param>
		public static void MapIndexed(IReadOnlyList<float> src1, IReadOnlyList<float> src2, IReadOnlyList<float> src3, Func<int, float, float, float, float> function, IVector<float> result)
		{
			if (src1.Count != src2.Count)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.Count != result.Count)
				throw new RankException("Mismatch of dimensions of src1 and result");

			for (int i = src1.Count - 1; i >= 0; --i)
				result[i] = function(i, src1[i], src2[i], src3[i]);
		}


    /// <summary>
    /// Adds (elementwise) two vectors a and b and stores the result in c. All vectors must have the same length.
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="c">The resulting vector.</param>
    public static void Add(IReadOnlyList<float> a, IReadOnlyList<float> b, IVector<float> c)
    {
      if (a.Count != b.Count)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Count != b.Count)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Count - 1; i >= 0; --i)
        c[i] = a[i] + b[i];
    }


    /// <summary>
    /// Adds (elementwise) two vectors a and (b scaled with scaleb) and stores the result in c, i.e. c = a + b * scaleb. All vectors must have the same length.
    /// The vectors a or b may be identical (the same instance) as c. 
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="scaleb">Scale factor for vector b.</param>
    /// <param name="c">The resulting vector calculated as a + b * scaleb.</param>
    public static void AddScaled(IReadOnlyList<float> a, IReadOnlyList<float> b, float scaleb, IVector<float> c)
    {
      if (a.Count != b.Count)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Count != b.Count)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Count - 1; i >= 0; --i)
        c[i] = a[i] + b[i] * scaleb;
    }



		/// <summary>
		/// Returns true if and only if both vectors contain the same elements. Both vectors must have the same length.
		/// </summary>
		/// <param name="vector1">First vector.</param>
		/// <param name="vector2">Second vector.</param>
		/// <returns>True if both vectors contain the same elements.</returns>
		public static bool AreValuesEqual(IReadOnlyList<float> vector1, IReadOnlyList<float> vector2)
		{
			if (vector1.Count != vector2.Count)
				throw new ArgumentException("Length of x and y are unequal");

			for (int i = vector1.Count - 1; i >= 0; --i)
			{
				if (!(vector1[i] == vector2[i]))
				{
					return false;
				}
			}
			return true;
		}


		/// <summary>Performs linear interpolation between two vectors at specified points.</summary>
		/// <param name="t">Scalar parameter of interpolation</param>
		/// <param name="t0">Scalar parameter corresponding to first vector v0.</param>
		/// <param name="v0">Vector at first parameter point.</param>
		/// <param name="t1">Scalar parameter corresponding to the second vector v1.</param>
		/// <param name="v1">Vector at second parameter point.</param>
		/// <param name="destinationVector">Intepolated vector value at point <paramref name="t"/></param>
		public static void Lerp(double t, double t0, IReadOnlyList<float> v0, double t1, IReadOnlyList<float> v1, IVector<float> destinationVector)
		{
			if(v0 is null)
				throw new ArgumentNullException(nameof(v0));
			if(v1 is null)
				throw new ArgumentNullException(nameof(v1));
			if(destinationVector is null)
				throw new ArgumentNullException(nameof(destinationVector));
			if (v0.Count != v1.Count)
        throw new ArgumentException("Length of vectors v0 and v1 unequal");
      if (destinationVector.Count != v0.Count)
        throw new ArgumentException("Length of vectors destinationVector and v0 unequal");

			for(int i=v0.Count -1; i>=0; --i)
				{
				destinationVector[i] = (float)((v0[i] * (t1 - t) + v1[i] * (t - t0)) / (t1 - t0));
				}
		}


     /// <summary>
    /// Multiplies (elementwise) two vectors a and b and stores the result in c. All vectors must have the same length.
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="c">The resulting vector.</param>
    public static void Multiply(IReadOnlyList<float> a, IReadOnlyList<float> b, IVector<float> c)
    {
      if (a.Count != b.Count)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (c.Count != b.Count)
        throw new ArgumentException("Length of vectors a and c unequal");

      for (int i = c.Count - 1; i >= 0; --i)
        c[i] = a[i] * b[i];
    }

    /// <summary>
    /// Gives the parallel maximum of vector a and b. The first element of the resulting vector
    /// is the maximum of the first element of a and the first element of b. The second element of the
    /// resulting vector is the maximum of the second element of a and the second element of b, and so on.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <param name="result">The resulting vector.</param>
		/// <returns>The same instance as provided by result. Each element result[i] is the maximum of a[i] and b[i].</returns>
    public static IVector<float> MaxOf(IReadOnlyList<float> a, IReadOnlyList<float> b, IVector<float> result)
    {
      if (a.Count != b.Count)
        throw new ArgumentException("Length of vectors a and b unequal");
      if (result.Count != b.Count)
        throw new ArgumentException("Length of vectors a and result unequal");

      for (int i = result.Count - 1; i >= 0; --i)
      {
        result[i] = Math.Max(a[i], b[i]);
      }

			return result;
    }


		/// <summary>
		/// Creates a new vector, whose elements are the maximum of the elements of a given input vector and a given number.
		/// </summary>
		/// <param name="vector1">Input vector.</param>
		/// <param name="scalar">Number.</param>
		/// <param name="result">Provide a vector whose elements are filled with the result. If null is provided, then a new vector will be allocated and returned.</param>
		/// <returns>The same instance as provided by result. Each element r[i] is the maximum of v[i] and b.</returns>
		public static IVector<float> MaxOf(this IReadOnlyList<float> vector1, float scalar, IVector<float> result)
		{
			if (vector1.Count != result.Count)
				throw new ArgumentException("vector1 and result must have the same length");

			for (int i = vector1.Count; i>=0; --i)
				result[i] = Math.Max(vector1[i], scalar);

			return result;
		}


		/// <summary>
		/// Returns the sum of squared differences of the elements of xarray and yarray.
		/// </summary>
		/// <param name="vector1">The first array.</param>
		/// <param name="vector2">The other array.</param>
		/// <returns>The sum of squared differences all elements of xarray and yarray.</returns>
		public static double SumOfSquaredDifferences(IReadOnlyList<float> vector1, IReadOnlyList<float> vector2)
		{
			if (vector1.Count != vector2.Count)
				throw new ArgumentException("Length of vector1 is different from length of vector2");

			double sum = 0;
			double tmp;
			for (int i = 0; i < vector1.Count; i++)
			{
				tmp = (double)vector1[i] - vector2[i];
				sum += tmp * tmp;
			}

			return sum;
		}

        /// <summary>
		/// Returns the Euclidean distance of two vectors, i.e. the L2-norm of the difference of the two vectors.
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The other vector.</param>
		/// <returns>The Euclidean distance of vector1 and vector2.</returns>
		public static double EuclideanDistance(IReadOnlyList<float> vector1, IReadOnlyList<float> vector2)
		{
			return Math.Sqrt(SumOfSquaredDifferences(vector1, vector2));
		}



		/// <summary>
		/// Returns the dot product of vector1 and vector2.
		/// </summary>
		/// <param name="vector1">The first array.</param>
		/// <param name="vector2">The other array.</param>
		/// <returns>The dot product (sum of vector1[i]*vector2[i].</returns>
		public static double DotProduct(IReadOnlyList<float> vector1, IReadOnlyList<float> vector2)
		{
			if (vector1.Count != vector2.Count)
				throw new ArgumentException("Length of vector1 is different from length of vector2");

			double sum = 0;
			for (int i = vector1.Count-1; i>=0; --i)
			{
				sum += ((double)vector1[i]) * vector2[i];
			}

			return sum;
		}


	} // class
} // namespace
