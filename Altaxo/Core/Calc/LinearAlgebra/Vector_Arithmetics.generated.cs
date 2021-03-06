﻿#region Copyright

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

#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;

namespace Altaxo.Calc.LinearAlgebra
{


// ******************************************* Unary functions not returning a vector, valid for all non-null vector types  ********************

// ******************************************** Definitions for Double *******************************************

	public partial class DoubleVector : Vector<Double> 
	{

		#region Constructors

		/// <summary>
		/// Constructor for an empty vector, i.e. a vector with no elements.
		/// </summary>
		public DoubleVector()
		{
		}

		///<summary>Constructor with components set to the default value.</summary>
		///<param name="length">Length of vector.</param>
		///<exception cref="ArgumentException">Exception thrown if length parameter isn't positive</exception>
		public DoubleVector(int length) : base(length)
		{
		}

		///<summary>Constructor with elements set to a value.</summary>
		///<param name="length">Length of vector.</param>
		///<param name="value">Value to set all elements with.</param>
		///<exception cref="ArgumentException">Exception thrown if length parameter isn't positive</exception>
		public DoubleVector(int length, Double value) : base(length, value)
		{
		}

		///<summary>Constructor for <c>FloatVector</c> to deep copy another <c>FloatVector</c></summary>
		///<param name="src"><c>FloatVector</c> to deep copy into <c>FloatVector</c>.</param>
		///<exception cref="ArgumentNullException">Exception thrown if null passed as 'src' parameter.</exception>
		public DoubleVector(Vector<Double> src) : base(src)
		{
		}

		///<summary>Constructor from an array</summary>
		///<param name="values">Array of values. The array is not used directly. Instead the elements of the array are copied to the vector.</param>
		///<exception cref="ArgumentNullException">Exception thrown if null passed as 'value' parameter.</exception>
		public DoubleVector(Double[] values) : base(values)
		{
		}

		///<summary>Constructor from an array</summary>
		///<param name="values">Array of values. The array is not used directly. Instead the elements of the array are copied to the vector.</param>
		///<exception cref="ArgumentNullException">Exception thrown if null passed as 'value' parameter.</exception>
		public DoubleVector(IReadOnlyList<Double> values) : base(values)
		{
		}

		///<summary>Constructor from an <see cref="IList"/></summary>
		///<param name="values"><c>IList</c> use as source for the elements of the vector.</param>
		///<exception cref="ArgumentNullException">Exception thrown if null passed as 'values' parameter.</exception>
		public DoubleVector(IList values) : base(values)
		{
		}

		

		/// <summary>
		/// Creates a vector, that is a wrapper of the provided array. The array is used directly.
		/// </summary>
		/// <param name="values">Array of values. This array is used directly in the returned vector!</param>
		/// <returns>Vector that is a wrapper for the provided array.</returns>
		public static new DoubleVector AsWrapperFor(Double[] values)
		{
			return new DoubleVector() { _array = values ?? throw new ArgumentNullException(nameof(values)) };
		}

		/// <summary>
		/// Creates a vector with the given length, all elements initialized to zero.
		/// </summary>
		/// <param name="lengthOfVector">Length of the vector.</param>
		/// <returns>vector with the given length, all elements initialized to zero.</returns>
		public static DoubleVector Zeros(int lengthOfVector)
		{
			return new DoubleVector(lengthOfVector);
		}

		///<summary>Returns a subvector of this vector.</summary>
		///<param name="startElement">Return data starting from this element.</param>
		///<param name="endElement">Return data ending in this element.</param>
		///<returns>A subvector of this vector.</returns>
		///<exception cref="ArgumentException">Exception thrown if <paramref>endElement</paramref> is greater than <paramref>startElement</paramref></exception>
		///<exception cref="ArgumentOutOfRangeException">Exception thrown if input dimensions are out of the range of <c>FloatVector</c> dimensions</exception>
		public new DoubleVector GetSubVector(int startElement, int endElement)
		{
			if (startElement > endElement)
			{
				throw new ArgumentException("The starting element must be less that the ending element.");
			}

			if (startElement < 0 || endElement < 0 || startElement >= this.Length || endElement >= this.Length)
			{
				throw new ArgumentException("startElement and startElement must be greater than or equal to zero, endElement must be less than Length, and endElement must be less than Length.");
			}

			int n = endElement - startElement + 1;
			var result = new  DoubleVector(n);
			for (int i = 0; i < n; i++)
			{
				result[i] = this[i + startElement];
			}
			return result;
		}

		public new  DoubleVector Clone()
		{
			return new  DoubleVector(this);
		}

		#endregion Constructors

		#region Norm

		/// <summary>
    /// Calculates the L1 norm of the vector (as the sum of the absolute values of the elements).
    /// </summary>
    /// <returns>L1 norm of the vector (sum of the absolute values of the elements).</returns>
    public Double L1Norm
		{
			get
			{
				return VectorMath.L1Norm(this._array);
			}
		}

		/// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
		/// <returns>The euclidian norm of the vector , i.e. the square root of the sum of squares of the elements.</returns>
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
		public double L2Norm
		{
			get
			{
				return VectorMath.L2Norm(this._array);
			}
		}

			///<summary>Gets vector's Euclidean norm</summary>
		public double EuclideanNorm
		{
			get
			{
				return VectorMath.L2Norm(this._array);
			}
		}

		/// <summary>Gets L-infinity norm of the vector (which is the maximum
		/// of the absolute values of the elements.</summary>
		public Double LInfinityNorm
		{
			get
			{
				return VectorMath.LInfinityNorm(this._array);
			}
		}

		///<summary>Compute the p Norm of this vector.</summary>
    ///<param name="p">Order of the norm.</param>
		///<returns>The p norm of this vector.</returns>
		///<remarks>p &gt; 0, if p &lt; 0, Abs(p) is used. If p = 0, the infinity norm is returned.</remarks>
		public double LpNorm(double p)
		{
			return VectorMath.LpNorm(this._array, p);
		}

		#endregion


		///<summary>Return the index of the absolute maximum element in the vector.</summary>
		///<returns>Index of the element with the maximum absolute value.</returns>
		public int GetAbsMaximumIndex()
		{
			return Blas.Imax.Compute(this.Length, this._array, 1);
		}

		///<summary>Return the <c>float</c> value of the maximum element in the vector.</summary>
		///<returns>Value (not the absolute value!) of the element with the maximum absolute value.</returns>
		public Double GetAbsMaximum()
		{
			return this._array[Blas.Imax.Compute(this.Length, this._array, 1)];
		}

		///<summary>Return the index of the minimum element in the vector.</summary>
		///<returns>Index of the element with the minimum absolute value.</returns>
		public int GetAbsMinimumIndex()
		{
			return Blas.Imin.Compute(this.Length, this._array, 1);
		}

		///<summary>Return the <c>float</c> value of the minimum element in the vector.</summary>
		///<returns>Value (not the absolute value!) of the element with the minimum absolute value.</returns>
		public Double GetAbsMinimum()
		{
			return this._array[Blas.Imin.Compute(this.Length, this._array, 1)];
		}

		///<summary>Compute the dot product of this <c>DoubleVector</c> with itself and return as <c>double</c></summary>
		///<returns><c>double</c> results from x dot x.</returns>
		public Double GetDotProduct()
		{
			return GetDotProduct(this);
		}

		///<summary>Compute the dot product of this <c>DoubleVector</c>  x with another <c>DoubleVector</c>  y and return as <c>double</c></summary>
		///<param name="Y"><c>DoubleVector</c> to dot product with this <c>DoubleVector</c>.</param>
		///<returns><c>double</c> results from x dot y.</returns>
		public Double GetDotProduct( DoubleVector Y)
		{
			if (Y is null)
			{
				throw new ArgumentNullException(nameof(Y));
			}
			return Blas.Dot.Compute(this.Length, this._array, 1, Y._array, 1);
		}

	

		///<summary>Sum the components in this <c>DoubleVector</c></summary>
		///<returns><c>double</c> results from the summary of <c>DoubleVector</c> components.</returns>
		public Double GetSum()
		{
			Double ret = 0;
			for (int i = 0; i < _array.Length; ++i)
			{
				ret += _array[i];
			}
			return ret;
		}

		///<summary>Compute the absolute sum of the elements of this <c>DoubleVector</c></summary>
		///<returns><c>double</c> of absolute sum of the elements.</returns>
		public Double GetSumMagnitudes()
		{
			return Blas.Asum.Compute(this._array.Length, this._array, 1);
		}

		///<summary>Compute the sum y = alpha * x + y where y is this <c>DoubleVector</c></summary>
		///<param name="alpha"><c>double</c> value to scale this <c>DoubleVector</c></param>
		///<param name="X"><c>DoubleVector</c> to add to alpha * this <c>DoubleVector</c></param>
		///<remarks>Results of computation replace data in this variable</remarks>
		public void Axpy(Double alpha, DoubleVector X)
		{
			if (X is null)
			{
				throw new ArgumentNullException(nameof(X));
			}
			Blas.Axpy.Compute(_array.Length, alpha, X._array, 1, this._array, 1);
		}

		///<summary>Scale this <c>DoubleVector</c> by a <c>double</c> scalar</summary>
		///<param name="alpha"><c>double</c> value to scale this <c>DoubleVector</c></param>
		///<remarks>Results of computation replace data in this variable</remarks>
		public void Scale(Double alpha)
		{
			Blas.Scal.Compute(_array.Length, alpha, _array, 1);
		}

		///<summary>Negate operator for <c>DoubleVector</c></summary>
		///<returns><c>DoubleVector</c> with values to negate.</returns>
		public static DoubleVector operator -(DoubleVector rhs)
		{
			var ret = new DoubleVector(rhs);
			Blas.Scal.Compute(ret.Length, -1, ret._array, 1);
			return ret;
		}

		///<summary>Negate operator for <c>DoubleVector</c></summary>
		///<returns><c>DoubleVector</c> with values to negate.</returns>
		public static DoubleVector Negate(DoubleVector rhs)
		{
			if (rhs is null)
			{
				throw new ArgumentNullException(nameof(rhs));
			}
			return -rhs;
		}

		///<summary>Subtract a <c>DoubleVector</c> from another <c>DoubleVector</c></summary>
		///<param name="lhs"><c>DoubleVector</c> to subtract from.</param>
		///<param name="rhs"><c>DoubleVector</c> to subtract.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static DoubleVector operator -(DoubleVector lhs, DoubleVector rhs)
		{
			var ret = new DoubleVector(lhs);
			Blas.Axpy.Compute(ret.Length, -1, rhs._array, 1, ret._array, 1);
			return ret;
		}

		///<summary>Subtract a <c>DoubleVector</c> from another <c>DoubleVector</c></summary>
		///<param name="lhs"><c>DoubleVector</c> to subtract from.</param>
		///<param name="rhs"><c>DoubleVector</c> to subtract.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static DoubleVector Subtract(DoubleVector lhs, DoubleVector rhs)
		{
			return lhs - rhs;
		}

		///<summary>Add a <c>DoubleVector</c> to another <c>DoubleVector</c></summary>
		///<param name="lhs"><c>DoubleVector</c> to add to.</param>
		///<param name="rhs"><c>DoubleVector</c> to add.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static DoubleVector operator +(DoubleVector lhs, DoubleVector rhs)
		{
			var ret = new DoubleVector(lhs);
			Blas.Axpy.Compute(ret.Length, 1, rhs._array, 1, ret._array, 1);
			return ret;
		}

		///<summary>Add a <c>DoubleVector</c> to another <c>DoubleVector</c></summary>
		///<param name="rhs"><c>DoubleVector</c> to add.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static DoubleVector operator +(DoubleVector rhs)
		{
			return rhs;
		}

		///<summary>Add a <c>DoubleVector</c> to this<c>DoubleVector</c></summary>
		///<param name="vector"><c>DoubleVector</c> to add.</param>
		///<exception cref="ArgumentNullException">Exception thrown if null given as argument.</exception>
		public void Add(DoubleVector vector)
		{
			if (vector is null)
			{
				throw new System.ArgumentNullException(nameof(vector));
			}
			Blas.Axpy.Compute(this.Length, 1, vector._array, 1, this._array, 1);
		}

		///<summary>Adds a scaled <c>DoubleVector</c> to this<c>DoubleVector</c></summary>
		///<param name="vector"><c>DoubleVector</c> to add.</param>
		///<param name="scale">Factor that is used to scale the vector, before it is added to this vector.</param>
		///<exception cref="ArgumentNullException">Exception thrown if null given as argument.</exception>
		public void AddScaled(DoubleVector vector, Double scale)
		{
			if (vector is null)
			{
				throw new System.ArgumentNullException(nameof(vector));
			}
			Blas.Axpy.Compute(this.Length, scale, vector._array, 1, this._array, 1);
		}


		///<summary>Subtract a <c>DoubleVector</c> from this<c>DoubleVector</c></summary>
		///<param name="vector"><c>DoubleVector</c> to add.</param>
		///<exception cref="ArgumentNullException">Exception thrown if null given as argument.</exception>
		public void Subtract(DoubleVector vector)
		{
			if (vector is null)
			{
				throw new System.ArgumentNullException(nameof(vector));
			}
			Blas.Axpy.Compute(this.Length, -1, vector._array, 1, this._array, 1);
		}

		///<summary>Scale this <c>DoubleVector</c> by a <c>double</c> scalar</summary>
		///<param name="value"><c>double</c> value to scale this <c>DoubleVector</c></param>
		///<remarks>Results of computation replace data in this variable</remarks>
		public void Multiply(Double value)
		{
			this.Scale(value);
		}

		///<summary>Divide this <c>DoubleVector</c> by a <c>double</c> scalar</summary>
		///<param name="value"><c>double</c> value to divide this <c>DoubleVector</c></param>
		///<remarks>Results of computation replace data in this variable</remarks>
		public void Divide(Double value)
		{
			this.Scale(1 / value);
		}

		///<summary>Add a <c>DoubleVector</c> to another <c>DoubleVector</c></summary>
		///<param name="lhs"><c>DoubleVector</c> to add to.</param>
		///<param name="rhs"><c>DoubleVector</c> to add.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static DoubleVector Add(DoubleVector lhs, DoubleVector rhs)
		{
			return lhs + rhs;
		}

		///<summary>Multiply a <c>double</c> x with a <c>DoubleVector</c> y as x*y</summary>
		///<param name="lhs"><c>double</c> as left hand operand.</param>
		///<param name="rhs"><c>DoubleVector</c> as right hand operand.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static DoubleVector operator *(Double lhs, DoubleVector rhs)
		{
			var ret = new DoubleVector(rhs);
#if MANAGED
			for (int i = 0; i < rhs._array.Length; i++)
				ret._array[i] = lhs * rhs._array[i];
#else
      Blas.Scal.Compute(ret.Length,lhs, ret.data,1);
#endif
			return ret;
		}

		///<summary>Multiply a <c>double</c> x with a <c>DoubleVector</c> y as x*y</summary>
		///<param name="lhs"><c>double</c> as left hand operand.</param>
		///<param name="rhs"><c>DoubleVector</c> as right hand operand.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static DoubleVector Multiply(Double lhs, DoubleVector rhs)
		{
			return lhs * rhs;
		}

		///<summary>Multiply a <c>DoubleVector</c> x with a <c>double</c> y as x*y</summary>
		///<param name="lhs"><c>DoubleVector</c> as left hand operand.</param>
		///<param name="rhs"><c>double</c> as right hand operand.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static DoubleVector operator *(DoubleVector lhs, Double rhs)
		{
			return rhs * lhs;
		}

		///<summary>Multiply a <c>DoubleVector</c> x with a <c>double</c> y as x*y</summary>
		///<param name="lhs"><c>DoubleVector</c> as left hand operand.</param>
		///<param name="rhs"><c>double</c> as right hand operand.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static DoubleVector Multiply(DoubleVector lhs, Double rhs)
		{
			return lhs * rhs;
		}

		///<summary>Divide a <c>DoubleVector</c> x with a <c>double</c> y as x/y</summary>
		///<param name="lhs"><c>DoubleVector</c> as left hand operand.</param>
		///<param name="rhs"><c>double</c> as right hand operand.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static DoubleVector operator /(DoubleVector lhs, Double rhs)
		{
			var ret = new DoubleVector(lhs);
#if MANAGED
			for (int i = 0; i < lhs._array.Length; i++)
				ret[i] = lhs._array[i] / rhs;
#else
      Blas.Scal.Compute(ret.Length, 1/rhs, ret.data,1);
#endif
			return ret;
		}

		///<summary>Divide a <c>DoubleVector</c> x with a <c>double</c> y as x/y</summary>
		///<param name="lhs"><c>DoubleVector</c> as left hand operand.</param>
		///<param name="rhs"><c>double</c> as right hand operand.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static DoubleVector Divide(DoubleVector lhs, Double rhs)
		{
			return lhs / rhs;
		}


    /// <summary>Performs linear interpolation between two vectors at specified point</summary>
    /// <param name="t">Point of interpolation</param>
    /// <param name="t0">First time point</param>
    /// <param name="v0">Vector at first time point</param>
    /// <param name="t1">Second time point</param>
    /// <param name="v1">Vector at second time point</param>
    /// <param name="result">Resulting vector. If <c>null</c> is provided, a new vector will be allocated. The resulting vector is the return value.</param>
    /// <returns>Intepolated vector value at point <paramref name="t"/></returns>
    public static DoubleVector Lerp(Double t, Double t0, DoubleVector v0, Double t1, DoubleVector v1, DoubleVector? result = null)
    {
      if (v0 is null) 
        throw new ArgumentNullException(nameof(v0));
      if (v1 is null)
        throw new ArgumentNullException(nameof(v0));
      if (v1.Length != v0.Length)
        throw new ArgumentNullException(nameof(v1), $"Length of {nameof(v1)} does not match length of {nameof(v0)}");
      if (result is null)
        result = new DoubleVector(v0.Length);
      else if(result.Length != v0.Length)
        throw new ArgumentNullException(nameof(result), $"Length of {nameof(result)} does not match length of {nameof(v0)}");

      var len = v0.Length;

      var factor0 = (t1 - t);
      var factor1 = (t - t0);
      var scale = 1 / (t1 - t0);

      for (int i=0;i<len;++i)
      {
        result[i] = (factor0 * v0[i] + factor1 * v1[i]) * scale;
      }

      return result;
    }



		///<summary>A string representation of this <c>DoubleVector</c>.</summary>
		///<param name="format">A format specification.</param>
		///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
		///<returns>The string representation of the value of <c>this</c> instance as specified by format and provider.</returns>
		public override string ToString(string? format, IFormatProvider? formatProvider)
		{
			var sb = new System.Text.StringBuilder("Length: ");
			sb.Append(_array.Length).Append(System.Environment.NewLine);
			for (int i = 0; i < _array.Length; ++i)
			{
				sb.Append(_array[i].ToString(format, formatProvider));
				if (i != _array.Length - 1)
				{
					sb.Append(", ");
				}
			}
			return sb.ToString();
		}


		#region Matrix related operations

		/// <summary>
		/// Returns the column of a <see cref="IROMatrix{Double}" /> as a new <c>DoubleVector.</c>
		/// </summary>
		/// <param name="mat">The matrix to copy the column from.</param>
		/// <param name="col">Number of column to copy from the matrix.</param>
		/// <returns>A new <c>DoubleVector</c> with the same elements as the column of the given matrix.</returns>
		public static DoubleVector GetColumn(IROMatrix<Double> mat, int col)
		{
			var result = new DoubleVector(mat.RowCount);
			for (int i = 0; i < result._array.Length; ++i)
				result._array[i] = mat[i, col];

			return result;
		}

		/// <summary>
		/// Returns the column of a <see cref="IROMatrix{Double}" /> as a new <c>double[]</c> array.
		/// </summary>
		/// <param name="mat">The matrix to copy the column from.</param>
		/// <param name="col">Index of the column to copy from the matrix.</param>
		/// <returns>A new array of <c>double</c> with the same elements as the column of the given matrix.</returns>
		public static Double[] GetColumnAsArray(IROMatrix<Double> mat, int col)
		{
			var result = new Double[mat.RowCount];
			for (int i = 0; i < result.Length; ++i)
				result[i] = mat[i, col];

			return result;
		}

		/// <summary>
		/// Returns the row of a <see cref="IROMatrix{Double}" /> as a new <c>DoubleVector.</c>
		/// </summary>
		/// <param name="mat">The matrix to copy the column from.</param>
		/// <param name="row">Index of the row to copy from the matrix.</param>
		/// <returns>A new <c>DoubleVector</c> with the same elements as the row of the given matrix.</returns>
		public static DoubleVector GetRow(IROMatrix<Double> mat, int row)
		{
			var result = new DoubleVector(mat.ColumnCount);
			for (int i = 0; i < result._array.Length; ++i)
				result._array[i] = mat[row, i];

			return result;
		}


		///<summary>Multiply a <c>FloatVector</c> with another <c>FloatVector</c> as x*y^T</summary>
		///<param name="lhs"><c>FloatVector</c> as left hand operand.</param>
		///<param name="rhs"><c>FloatVector</c> as right hand operand.</param>
		///<returns><c>FloatMatrix</c> with results.</returns>
		public static DoubleMatrix operator *(DoubleVector lhs, DoubleVector rhs)
		{
			var ret = new DoubleMatrix(lhs._array.Length, rhs._array.Length);

			for (int i = 0; i < lhs._array.Length; i++)
			{
				for (int j = 0; j < rhs._array.Length; j++)
				{
					ret[i, j] = lhs._array[i] * rhs._array[j];
				}
			}
			return ret;
		}

			///<summary>Multiply a <c>FloatVector</c> with another <c>FloatVector</c> as x*y^T</summary>
		///<param name="lhs"><c>FloatVector</c> as left hand operand.</param>
		///<param name="rhs"><c>FloatVector</c> as right hand operand.</param>
		///<returns><c>FloatMatrix</c> with results.</returns>
		public static DoubleMatrix Multiply(DoubleVector lhs,DoubleVector rhs)
		{
			return lhs * rhs;
		}

		#endregion Matrix related operations

	} // end of class


// ******************************************** Definitions for Single *******************************************

	public partial class FloatVector : Vector<Single> 
	{

		#region Constructors

		/// <summary>
		/// Constructor for an empty vector, i.e. a vector with no elements.
		/// </summary>
		public FloatVector()
		{
		}

		///<summary>Constructor with components set to the default value.</summary>
		///<param name="length">Length of vector.</param>
		///<exception cref="ArgumentException">Exception thrown if length parameter isn't positive</exception>
		public FloatVector(int length) : base(length)
		{
		}

		///<summary>Constructor with elements set to a value.</summary>
		///<param name="length">Length of vector.</param>
		///<param name="value">Value to set all elements with.</param>
		///<exception cref="ArgumentException">Exception thrown if length parameter isn't positive</exception>
		public FloatVector(int length, Single value) : base(length, value)
		{
		}

		///<summary>Constructor for <c>FloatVector</c> to deep copy another <c>FloatVector</c></summary>
		///<param name="src"><c>FloatVector</c> to deep copy into <c>FloatVector</c>.</param>
		///<exception cref="ArgumentNullException">Exception thrown if null passed as 'src' parameter.</exception>
		public FloatVector(Vector<Single> src) : base(src)
		{
		}

		///<summary>Constructor from an array</summary>
		///<param name="values">Array of values. The array is not used directly. Instead the elements of the array are copied to the vector.</param>
		///<exception cref="ArgumentNullException">Exception thrown if null passed as 'value' parameter.</exception>
		public FloatVector(Single[] values) : base(values)
		{
		}

		///<summary>Constructor from an array</summary>
		///<param name="values">Array of values. The array is not used directly. Instead the elements of the array are copied to the vector.</param>
		///<exception cref="ArgumentNullException">Exception thrown if null passed as 'value' parameter.</exception>
		public FloatVector(IReadOnlyList<Single> values) : base(values)
		{
		}

		///<summary>Constructor from an <see cref="IList"/></summary>
		///<param name="values"><c>IList</c> use as source for the elements of the vector.</param>
		///<exception cref="ArgumentNullException">Exception thrown if null passed as 'values' parameter.</exception>
		public FloatVector(IList values) : base(values)
		{
		}

		

		/// <summary>
		/// Creates a vector, that is a wrapper of the provided array. The array is used directly.
		/// </summary>
		/// <param name="values">Array of values. This array is used directly in the returned vector!</param>
		/// <returns>Vector that is a wrapper for the provided array.</returns>
		public static new FloatVector AsWrapperFor(Single[] values)
		{
			return new FloatVector() { _array = values ?? throw new ArgumentNullException(nameof(values)) };
		}

		/// <summary>
		/// Creates a vector with the given length, all elements initialized to zero.
		/// </summary>
		/// <param name="lengthOfVector">Length of the vector.</param>
		/// <returns>vector with the given length, all elements initialized to zero.</returns>
		public static FloatVector Zeros(int lengthOfVector)
		{
			return new FloatVector(lengthOfVector);
		}

		///<summary>Returns a subvector of this vector.</summary>
		///<param name="startElement">Return data starting from this element.</param>
		///<param name="endElement">Return data ending in this element.</param>
		///<returns>A subvector of this vector.</returns>
		///<exception cref="ArgumentException">Exception thrown if <paramref>endElement</paramref> is greater than <paramref>startElement</paramref></exception>
		///<exception cref="ArgumentOutOfRangeException">Exception thrown if input dimensions are out of the range of <c>FloatVector</c> dimensions</exception>
		public new FloatVector GetSubVector(int startElement, int endElement)
		{
			if (startElement > endElement)
			{
				throw new ArgumentException("The starting element must be less that the ending element.");
			}

			if (startElement < 0 || endElement < 0 || startElement >= this.Length || endElement >= this.Length)
			{
				throw new ArgumentException("startElement and startElement must be greater than or equal to zero, endElement must be less than Length, and endElement must be less than Length.");
			}

			int n = endElement - startElement + 1;
			var result = new  FloatVector(n);
			for (int i = 0; i < n; i++)
			{
				result[i] = this[i + startElement];
			}
			return result;
		}

		public new  FloatVector Clone()
		{
			return new  FloatVector(this);
		}

		#endregion Constructors

		#region Norm

		/// <summary>
    /// Calculates the L1 norm of the vector (as the sum of the absolute values of the elements).
    /// </summary>
    /// <returns>L1 norm of the vector (sum of the absolute values of the elements).</returns>
    public Single L1Norm
		{
			get
			{
				return VectorMath.L1Norm(this._array);
			}
		}

		/// <summary>Given an n-vector x, this function calculates the euclidean norm of x.</summary>
		/// <returns>The euclidian norm of the vector , i.e. the square root of the sum of squares of the elements.</returns>
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
		public double L2Norm
		{
			get
			{
				return VectorMath.L2Norm(this._array);
			}
		}

			///<summary>Gets vector's Euclidean norm</summary>
		public double EuclideanNorm
		{
			get
			{
				return VectorMath.L2Norm(this._array);
			}
		}

		/// <summary>Gets L-infinity norm of the vector (which is the maximum
		/// of the absolute values of the elements.</summary>
		public Single LInfinityNorm
		{
			get
			{
				return VectorMath.LInfinityNorm(this._array);
			}
		}

		///<summary>Compute the p Norm of this vector.</summary>
    ///<param name="p">Order of the norm.</param>
		///<returns>The p norm of this vector.</returns>
		///<remarks>p &gt; 0, if p &lt; 0, Abs(p) is used. If p = 0, the infinity norm is returned.</remarks>
		public double LpNorm(double p)
		{
			return VectorMath.LpNorm(this._array, p);
		}

		#endregion


		///<summary>Return the index of the absolute maximum element in the vector.</summary>
		///<returns>Index of the element with the maximum absolute value.</returns>
		public int GetAbsMaximumIndex()
		{
			return Blas.Imax.Compute(this.Length, this._array, 1);
		}

		///<summary>Return the <c>float</c> value of the maximum element in the vector.</summary>
		///<returns>Value (not the absolute value!) of the element with the maximum absolute value.</returns>
		public Single GetAbsMaximum()
		{
			return this._array[Blas.Imax.Compute(this.Length, this._array, 1)];
		}

		///<summary>Return the index of the minimum element in the vector.</summary>
		///<returns>Index of the element with the minimum absolute value.</returns>
		public int GetAbsMinimumIndex()
		{
			return Blas.Imin.Compute(this.Length, this._array, 1);
		}

		///<summary>Return the <c>float</c> value of the minimum element in the vector.</summary>
		///<returns>Value (not the absolute value!) of the element with the minimum absolute value.</returns>
		public Single GetAbsMinimum()
		{
			return this._array[Blas.Imin.Compute(this.Length, this._array, 1)];
		}

		///<summary>Compute the dot product of this <c>DoubleVector</c> with itself and return as <c>double</c></summary>
		///<returns><c>double</c> results from x dot x.</returns>
		public Single GetDotProduct()
		{
			return GetDotProduct(this);
		}

		///<summary>Compute the dot product of this <c>DoubleVector</c>  x with another <c>DoubleVector</c>  y and return as <c>double</c></summary>
		///<param name="Y"><c>DoubleVector</c> to dot product with this <c>DoubleVector</c>.</param>
		///<returns><c>double</c> results from x dot y.</returns>
		public Single GetDotProduct( FloatVector Y)
		{
			if (Y is null)
			{
				throw new ArgumentNullException(nameof(Y));
			}
			return Blas.Dot.Compute(this.Length, this._array, 1, Y._array, 1);
		}

	

		///<summary>Sum the components in this <c>DoubleVector</c></summary>
		///<returns><c>double</c> results from the summary of <c>DoubleVector</c> components.</returns>
		public Single GetSum()
		{
			Single ret = 0;
			for (int i = 0; i < _array.Length; ++i)
			{
				ret += _array[i];
			}
			return ret;
		}

		///<summary>Compute the absolute sum of the elements of this <c>DoubleVector</c></summary>
		///<returns><c>double</c> of absolute sum of the elements.</returns>
		public Single GetSumMagnitudes()
		{
			return Blas.Asum.Compute(this._array.Length, this._array, 1);
		}

		///<summary>Compute the sum y = alpha * x + y where y is this <c>DoubleVector</c></summary>
		///<param name="alpha"><c>double</c> value to scale this <c>DoubleVector</c></param>
		///<param name="X"><c>DoubleVector</c> to add to alpha * this <c>DoubleVector</c></param>
		///<remarks>Results of computation replace data in this variable</remarks>
		public void Axpy(Single alpha, FloatVector X)
		{
			if (X is null)
			{
				throw new ArgumentNullException(nameof(X));
			}
			Blas.Axpy.Compute(_array.Length, alpha, X._array, 1, this._array, 1);
		}

		///<summary>Scale this <c>DoubleVector</c> by a <c>double</c> scalar</summary>
		///<param name="alpha"><c>double</c> value to scale this <c>DoubleVector</c></param>
		///<remarks>Results of computation replace data in this variable</remarks>
		public void Scale(Single alpha)
		{
			Blas.Scal.Compute(_array.Length, alpha, _array, 1);
		}

		///<summary>Negate operator for <c>DoubleVector</c></summary>
		///<returns><c>DoubleVector</c> with values to negate.</returns>
		public static FloatVector operator -(FloatVector rhs)
		{
			var ret = new FloatVector(rhs);
			Blas.Scal.Compute(ret.Length, -1, ret._array, 1);
			return ret;
		}

		///<summary>Negate operator for <c>DoubleVector</c></summary>
		///<returns><c>DoubleVector</c> with values to negate.</returns>
		public static FloatVector Negate(FloatVector rhs)
		{
			if (rhs is null)
			{
				throw new ArgumentNullException(nameof(rhs));
			}
			return -rhs;
		}

		///<summary>Subtract a <c>DoubleVector</c> from another <c>DoubleVector</c></summary>
		///<param name="lhs"><c>DoubleVector</c> to subtract from.</param>
		///<param name="rhs"><c>DoubleVector</c> to subtract.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static FloatVector operator -(FloatVector lhs, FloatVector rhs)
		{
			var ret = new FloatVector(lhs);
			Blas.Axpy.Compute(ret.Length, -1, rhs._array, 1, ret._array, 1);
			return ret;
		}

		///<summary>Subtract a <c>DoubleVector</c> from another <c>DoubleVector</c></summary>
		///<param name="lhs"><c>DoubleVector</c> to subtract from.</param>
		///<param name="rhs"><c>DoubleVector</c> to subtract.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static FloatVector Subtract(FloatVector lhs, FloatVector rhs)
		{
			return lhs - rhs;
		}

		///<summary>Add a <c>DoubleVector</c> to another <c>DoubleVector</c></summary>
		///<param name="lhs"><c>DoubleVector</c> to add to.</param>
		///<param name="rhs"><c>DoubleVector</c> to add.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static FloatVector operator +(FloatVector lhs, FloatVector rhs)
		{
			var ret = new FloatVector(lhs);
			Blas.Axpy.Compute(ret.Length, 1, rhs._array, 1, ret._array, 1);
			return ret;
		}

		///<summary>Add a <c>DoubleVector</c> to another <c>DoubleVector</c></summary>
		///<param name="rhs"><c>DoubleVector</c> to add.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static FloatVector operator +(FloatVector rhs)
		{
			return rhs;
		}

		///<summary>Add a <c>DoubleVector</c> to this<c>DoubleVector</c></summary>
		///<param name="vector"><c>DoubleVector</c> to add.</param>
		///<exception cref="ArgumentNullException">Exception thrown if null given as argument.</exception>
		public void Add(FloatVector vector)
		{
			if (vector is null)
			{
				throw new System.ArgumentNullException(nameof(vector));
			}
			Blas.Axpy.Compute(this.Length, 1, vector._array, 1, this._array, 1);
		}

		///<summary>Adds a scaled <c>DoubleVector</c> to this<c>DoubleVector</c></summary>
		///<param name="vector"><c>DoubleVector</c> to add.</param>
		///<param name="scale">Factor that is used to scale the vector, before it is added to this vector.</param>
		///<exception cref="ArgumentNullException">Exception thrown if null given as argument.</exception>
		public void AddScaled(FloatVector vector, Single scale)
		{
			if (vector is null)
			{
				throw new System.ArgumentNullException(nameof(vector));
			}
			Blas.Axpy.Compute(this.Length, scale, vector._array, 1, this._array, 1);
		}


		///<summary>Subtract a <c>DoubleVector</c> from this<c>DoubleVector</c></summary>
		///<param name="vector"><c>DoubleVector</c> to add.</param>
		///<exception cref="ArgumentNullException">Exception thrown if null given as argument.</exception>
		public void Subtract(FloatVector vector)
		{
			if (vector is null)
			{
				throw new System.ArgumentNullException(nameof(vector));
			}
			Blas.Axpy.Compute(this.Length, -1, vector._array, 1, this._array, 1);
		}

		///<summary>Scale this <c>DoubleVector</c> by a <c>double</c> scalar</summary>
		///<param name="value"><c>double</c> value to scale this <c>DoubleVector</c></param>
		///<remarks>Results of computation replace data in this variable</remarks>
		public void Multiply(Single value)
		{
			this.Scale(value);
		}

		///<summary>Divide this <c>DoubleVector</c> by a <c>double</c> scalar</summary>
		///<param name="value"><c>double</c> value to divide this <c>DoubleVector</c></param>
		///<remarks>Results of computation replace data in this variable</remarks>
		public void Divide(Single value)
		{
			this.Scale(1 / value);
		}

		///<summary>Add a <c>DoubleVector</c> to another <c>DoubleVector</c></summary>
		///<param name="lhs"><c>DoubleVector</c> to add to.</param>
		///<param name="rhs"><c>DoubleVector</c> to add.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static FloatVector Add(FloatVector lhs, FloatVector rhs)
		{
			return lhs + rhs;
		}

		///<summary>Multiply a <c>double</c> x with a <c>DoubleVector</c> y as x*y</summary>
		///<param name="lhs"><c>double</c> as left hand operand.</param>
		///<param name="rhs"><c>DoubleVector</c> as right hand operand.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static FloatVector operator *(Single lhs, FloatVector rhs)
		{
			var ret = new FloatVector(rhs);
#if MANAGED
			for (int i = 0; i < rhs._array.Length; i++)
				ret._array[i] = lhs * rhs._array[i];
#else
      Blas.Scal.Compute(ret.Length,lhs, ret.data,1);
#endif
			return ret;
		}

		///<summary>Multiply a <c>double</c> x with a <c>DoubleVector</c> y as x*y</summary>
		///<param name="lhs"><c>double</c> as left hand operand.</param>
		///<param name="rhs"><c>DoubleVector</c> as right hand operand.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static FloatVector Multiply(Single lhs, FloatVector rhs)
		{
			return lhs * rhs;
		}

		///<summary>Multiply a <c>DoubleVector</c> x with a <c>double</c> y as x*y</summary>
		///<param name="lhs"><c>DoubleVector</c> as left hand operand.</param>
		///<param name="rhs"><c>double</c> as right hand operand.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static FloatVector operator *(FloatVector lhs, Single rhs)
		{
			return rhs * lhs;
		}

		///<summary>Multiply a <c>DoubleVector</c> x with a <c>double</c> y as x*y</summary>
		///<param name="lhs"><c>DoubleVector</c> as left hand operand.</param>
		///<param name="rhs"><c>double</c> as right hand operand.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static FloatVector Multiply(FloatVector lhs, Single rhs)
		{
			return lhs * rhs;
		}

		///<summary>Divide a <c>DoubleVector</c> x with a <c>double</c> y as x/y</summary>
		///<param name="lhs"><c>DoubleVector</c> as left hand operand.</param>
		///<param name="rhs"><c>double</c> as right hand operand.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static FloatVector operator /(FloatVector lhs, Single rhs)
		{
			var ret = new FloatVector(lhs);
#if MANAGED
			for (int i = 0; i < lhs._array.Length; i++)
				ret[i] = lhs._array[i] / rhs;
#else
      Blas.Scal.Compute(ret.Length, 1/rhs, ret.data,1);
#endif
			return ret;
		}

		///<summary>Divide a <c>DoubleVector</c> x with a <c>double</c> y as x/y</summary>
		///<param name="lhs"><c>DoubleVector</c> as left hand operand.</param>
		///<param name="rhs"><c>double</c> as right hand operand.</param>
		///<returns><c>DoubleVector</c> with results.</returns>
		public static FloatVector Divide(FloatVector lhs, Single rhs)
		{
			return lhs / rhs;
		}


    /// <summary>Performs linear interpolation between two vectors at specified point</summary>
    /// <param name="t">Point of interpolation</param>
    /// <param name="t0">First time point</param>
    /// <param name="v0">Vector at first time point</param>
    /// <param name="t1">Second time point</param>
    /// <param name="v1">Vector at second time point</param>
    /// <param name="result">Resulting vector. If <c>null</c> is provided, a new vector will be allocated. The resulting vector is the return value.</param>
    /// <returns>Intepolated vector value at point <paramref name="t"/></returns>
    public static FloatVector Lerp(Single t, Single t0, FloatVector v0, Single t1, FloatVector v1, FloatVector? result = null)
    {
      if (v0 is null) 
        throw new ArgumentNullException(nameof(v0));
      if (v1 is null)
        throw new ArgumentNullException(nameof(v0));
      if (v1.Length != v0.Length)
        throw new ArgumentNullException(nameof(v1), $"Length of {nameof(v1)} does not match length of {nameof(v0)}");
      if (result is null)
        result = new FloatVector(v0.Length);
      else if(result.Length != v0.Length)
        throw new ArgumentNullException(nameof(result), $"Length of {nameof(result)} does not match length of {nameof(v0)}");

      var len = v0.Length;

      var factor0 = (t1 - t);
      var factor1 = (t - t0);
      var scale = 1 / (t1 - t0);

      for (int i=0;i<len;++i)
      {
        result[i] = (factor0 * v0[i] + factor1 * v1[i]) * scale;
      }

      return result;
    }



		///<summary>A string representation of this <c>DoubleVector</c>.</summary>
		///<param name="format">A format specification.</param>
		///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
		///<returns>The string representation of the value of <c>this</c> instance as specified by format and provider.</returns>
		public override string ToString(string? format, IFormatProvider? formatProvider)
		{
			var sb = new System.Text.StringBuilder("Length: ");
			sb.Append(_array.Length).Append(System.Environment.NewLine);
			for (int i = 0; i < _array.Length; ++i)
			{
				sb.Append(_array[i].ToString(format, formatProvider));
				if (i != _array.Length - 1)
				{
					sb.Append(", ");
				}
			}
			return sb.ToString();
		}


		#region Matrix related operations

		/// <summary>
		/// Returns the column of a <see cref="IROMatrix{Single}" /> as a new <c>DoubleVector.</c>
		/// </summary>
		/// <param name="mat">The matrix to copy the column from.</param>
		/// <param name="col">Number of column to copy from the matrix.</param>
		/// <returns>A new <c>DoubleVector</c> with the same elements as the column of the given matrix.</returns>
		public static FloatVector GetColumn(IROMatrix<Single> mat, int col)
		{
			var result = new FloatVector(mat.RowCount);
			for (int i = 0; i < result._array.Length; ++i)
				result._array[i] = mat[i, col];

			return result;
		}

		/// <summary>
		/// Returns the column of a <see cref="IROMatrix{Single}" /> as a new <c>double[]</c> array.
		/// </summary>
		/// <param name="mat">The matrix to copy the column from.</param>
		/// <param name="col">Index of the column to copy from the matrix.</param>
		/// <returns>A new array of <c>double</c> with the same elements as the column of the given matrix.</returns>
		public static Single[] GetColumnAsArray(IROMatrix<Single> mat, int col)
		{
			var result = new Single[mat.RowCount];
			for (int i = 0; i < result.Length; ++i)
				result[i] = mat[i, col];

			return result;
		}

		/// <summary>
		/// Returns the row of a <see cref="IROMatrix{Single}" /> as a new <c>DoubleVector.</c>
		/// </summary>
		/// <param name="mat">The matrix to copy the column from.</param>
		/// <param name="row">Index of the row to copy from the matrix.</param>
		/// <returns>A new <c>DoubleVector</c> with the same elements as the row of the given matrix.</returns>
		public static FloatVector GetRow(IROMatrix<Single> mat, int row)
		{
			var result = new FloatVector(mat.ColumnCount);
			for (int i = 0; i < result._array.Length; ++i)
				result._array[i] = mat[row, i];

			return result;
		}


		///<summary>Multiply a <c>FloatVector</c> with another <c>FloatVector</c> as x*y^T</summary>
		///<param name="lhs"><c>FloatVector</c> as left hand operand.</param>
		///<param name="rhs"><c>FloatVector</c> as right hand operand.</param>
		///<returns><c>FloatMatrix</c> with results.</returns>
		public static FloatMatrix operator *(FloatVector lhs, FloatVector rhs)
		{
			var ret = new FloatMatrix(lhs._array.Length, rhs._array.Length);

			for (int i = 0; i < lhs._array.Length; i++)
			{
				for (int j = 0; j < rhs._array.Length; j++)
				{
					ret[i, j] = lhs._array[i] * rhs._array[j];
				}
			}
			return ret;
		}

			///<summary>Multiply a <c>FloatVector</c> with another <c>FloatVector</c> as x*y^T</summary>
		///<param name="lhs"><c>FloatVector</c> as left hand operand.</param>
		///<param name="rhs"><c>FloatVector</c> as right hand operand.</param>
		///<returns><c>FloatMatrix</c> with results.</returns>
		public static FloatMatrix Multiply(FloatVector lhs,FloatVector rhs)
		{
			return lhs * rhs;
		}

		#endregion Matrix related operations

	} // end of class



} // end of namespace
