#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
#endregion

/*
 * DoubleVector.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Collections;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
  ///<summary>
  /// Defines a Vector of doubles.
  ///</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  [Serializable]
  sealed public class DoubleVector : IVector, ICloneable, IFormattable, IEnumerable, ICollection, IList
  {
    internal double[] data;

    /// <summary>
    /// Creates an empty vector.
    /// </summary>
    public DoubleVector()
    {
      data = new double[0];
    }

    ///<summary>Constructor for <c>DoubleVector</c> with components set to zero</summary>
    ///<param name="length">Length of vector.</param>
    ///<exception cref="ArgumentException">Exception thrown if length parameter isn't positive</exception>
    public DoubleVector(int length)
    {
      if (length < 1)
      {
        throw new ArgumentException("Length must be positive.", "length");
      }
      data = new double[length];
    }

    ///<summary>Constructor for <c>DoubleVector</c> with components set to a value</summary>
    ///<param name="length">Length of vector.</param>
    ///<param name="value"><c>double</c> value to set all components.</param>
    ///<exception cref="ArgumentException">Exception thrown if length parameter isn't positive</exception>
    public DoubleVector(int length, double value)
    {
      if (length < 1)
      {
        throw new ArgumentException("length must be positive.", "length");
      }
      data = new double[length];
      for (int i = 0; i < data.Length; ++i)
      {
        data[i] = value;
      }
    }

    ///<summary>Constructor for <c>DoubleVector</c> from <c>double</c> array</summary>
    ///<param name="values">Array of <c>double</c> to convert into <c>DoubleVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'value' parameter.</exception>
    public DoubleVector(double[] values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("Array cannot be null");
      }
      data = new double[values.Length];
      for (int i = 0; i < values.Length; ++i)
      {
        data[i] = values[i];
      }
    }

    ///<summary>Constructor for <c>DoubleVector</c> from <c>IList</c></summary>
    ///<param name="values"><c>IList</c> to convert into <c>DoubleVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'values' parameter.</exception>
    public DoubleVector(IList values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("IList cannot be null");
      }
      data = new double[values.Count];
      for (int i = 0; i < values.Count; ++i)
      {
        data[i] = (double)values[i];
      }
    }

    ///<summary>Constructor for <c>DoubleVector</c> to deep copy another <c>DoubleVector</c></summary>
    ///<param name="src"><c>DoubleVector</c> to deep copy into <c>DoubleVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'src' parameter.</exception>
    public DoubleVector(DoubleVector src)
    {
      if (src == null)
      {
        throw new ArgumentNullException("DoubleVector cannot be null");
      }
      data = new double[src.data.Length];
      Array.Copy(src.data, 0, data, 0, data.Length);
    }

    /// <summary>
    /// Resizes the vector. The previous element data are lost.
    /// </summary>
    /// <param name="lo">New lower boundary of the vector (first valid index).</param>
    /// <param name="hi">New upper boundary of the vector (last valid index).</param>
    public void Resize(int lo, int hi)
    {
      if (lo != 0)
        throw new NotSupportedException("Lower bound value other than zero is not supported by DoubleVector class");

      if (data == null || (hi - lo) >= data.Length)
      {
        data = new double[hi - lo + 1];
      }
      //      this.lo = lo;
      //      this.hi = hi;
    }

    ///<summary>Return the length of the <c>DoubleVector</c> variable</summary>
    ///<returns>The length.</returns>
    public int Length
    {
      get
      {
        return data.Length;
      }
    }

   
    public int LowerBound
    {
      get
      {
        return 0;
      }
    }
    

    public int UpperBound
    {
      get
      {
        return data.Length-1;
      }
    }

    ///<summary>Access a <c>DoubleVector</c> element</summary>
    ///<param name="index">The element to access</param>
    ///<exception cref="IndexOutOfRangeException">Exception thrown in element accessed is out of the bounds of the vector.</exception>
    ///<returns>Returns a <c>double</c> vector element</returns>
    public double this[int index]
    {
      get
      {
        return data[index];
      }
      set
      {
        data[index] = value;
      }
    }

    ///<summary>Check if <c>DoubleVector</c> variable is the same as another object</summary>
    ///<param name="obj"><c>obj</c> to compare present <c>DoubleVector</c> to.</param>
    ///<returns>Returns true if the variable is the same as the <c>DoubleVector</c> variable</returns>
    ///<remarks>The <c>obj</c> parameter is converted into a <c>DoubleVector</c> variable before comparing with the current <c>DoubleVector</c>.</remarks>
    public override bool Equals(Object obj)
    {
      DoubleVector vector = obj as DoubleVector;
      if (vector == null)
      {
        return false;
      }

      if (this.data.Length != vector.data.Length)
      {
        return false;
      }

      for (int i = 0; i < this.data.Length; ++i)
      {
        if (this.data[i] != vector.data[i])
        {
          return false;
        }
      }
      return true;
    }

    ///<summary>Return the Hashcode for the <c>DoubleVector</c></summary>
    ///<returns>The Hashcode representation of <c>DoubleVector</c></returns>
    public override int GetHashCode()
    {
      return (int)this.GetNorm();
    }

    ///<summary>Retrieves a refernce to the public array.</summary>
    ///<returns>Reference to the public <c>double</c> array.</returns>
    public double[] GetInternalData()
    {
      return this.data;
    }

    ///<summary>Return <c>double</c> array of data from <c>DoubleVector</c></summary>
    ///<returns><c>double</c> array with data.</returns>
    public double[] ToArray()
    {
      double[] ret = new double[data.Length];
      Array.Copy(data, ret, data.Length);
      return ret;
    }

    ///<summary>Returns a subvector of the <c>DoubleVector</c></summary>
    ///<param name="startElement">Return data starting from this element.</param>
    ///<param name="endElement">Return data ending in this element.</param>
    ///<returns><c>DoubleVector</c> a subvector of the reference vector</returns>
    ///<exception cref="ArgumentException">Exception thrown if <paramref>endElement</paramref> is greater than <paramref>startElement</paramref></exception>
    ///<exception cref="ArgumentOutOfRangeException">Exception thrown if input dimensions are out of the range of <c>DoubleVector</c> dimensions</exception>
    public DoubleVector GetSubVector(int startElement, int endElement)
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
      DoubleVector ret = new DoubleVector(n);
      for (int i = 0; i < n; i++)
      {
        ret[i] = this[i + startElement];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>DoubleVector</c> from <c>FloatVector</c></summary>
    static public implicit operator DoubleVector(FloatVector src)
    {
      if (src == null)
      {
        return null;
      }
      DoubleVector ret = new DoubleVector(src.Length);
      Array.Copy(src.data, ret.data, src.Length);
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>DoubleVector</c> from <c>FloatVector</c></summary>
    static public DoubleVector ToDoubleVector(FloatVector src)
    {
      if (src == null)
      {
        return null;
      }
      DoubleVector ret = new DoubleVector(src.Length);
      Array.Copy(src.data, ret.data, src.Length);
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>DoubleVector</c> from <c>float</c> array</summary>
    static public implicit operator DoubleVector(float[] src)
    {
      if (src == null)
      {
        return null;
      }
      DoubleVector ret = new DoubleVector(src.Length);
      Array.Copy(src, ret.data, src.Length);
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>DoubleVector</c> from <c>float</c> array</summary>
    static public DoubleVector ToDoubleVector(float[] src)
    {
      if (src == null)
      {
        return null;
      }
      DoubleVector ret = new DoubleVector(src.Length);
      Array.Copy(src, ret.data, src.Length);
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>DoubleVector</c> from <c>double</c> array</summary>
    static public implicit operator DoubleVector(double[] src)
    {
      return new DoubleVector(src);
    }

    ///<summary>Implicit cast conversion to <c>DoubleVector</c> from <c>double</c> array</summary>
    static public DoubleVector ToDoubleVector(double[] src)
    {
      return new DoubleVector(src);
    }

    ///<summary>Return the index of the absolute maximum element in the <c>DoubleVector</c></summary>
    ///<returns>Index value of maximum element.</returns>
    public int GetAbsMaximumIndex()
    {
      return Blas.Imax.Compute(this.Length, this.data, 1);
    }

    ///<summary>Return the <c>double</c> value of the absolute maximum element in the <c>DoubleVector</c></summary>
    ///<returns><c>double</c> value of maximum element.</returns>
    public double GetAbsMaximum()
    {
      return this.data[Blas.Imax.Compute(this.Length, this.data, 1)];
    }

    ///<summary>Return the index of the absolute minimum element in the <c>DoubleVector</c></summary>
    ///<returns>Index value of minimum element.</returns>
    public int GetAbsMinimumIndex()
    {
      return Blas.Imin.Compute(this.Length, this.data, 1);
    }

    ///<summary>Return the <c>double</c> value of the absolute minimum element in the <c>DoubleVector</c></summary>
    ///<returns><c>double</c> value of minimum element.</returns>
    public double GetAbsMinimum()
    {
      return this.data[Blas.Imin.Compute(this.Length, this.data, 1)];
    }

    ///<summary>Clone (deep copy) of this <c>DoubleVector</c> into another <c>DoubleVector</c></summary>
    ///<param name="src"><c>DoubleVector</c> to deepcopy this <c>DoubleVector</c> into.</param>
    public void Copy(DoubleVector src)
    {
      if (src == null)
      {
        throw new ArgumentNullException("Vector cannot be null.");
      }
      Blas.Copy.Compute(src.Length, src.data, 1, this.data, 1);
    }

    ///<summary>Swap data in this <c>DoubleVector</c> with another <c>DoubleVector</c></summary>
    ///<param name="src"><c>DoubleVector</c> to swap data with.</param>
    public void Swap(DoubleVector src)
    {
      if (src == null)
      {
        throw new ArgumentNullException("Vector cannot be null.");
      }
      Blas.Swap.Compute(src.Length, src.data, 1, this.data, 1);
    }

    ///<summary>Compute the dot product of this <c>DoubleVector</c> with itself and return as <c>double</c></summary>
    ///<returns><c>double</c> results from x dot x.</returns>
    public double GetDotProduct()
    {
      return GetDotProduct(this);
    }

    ///<summary>Compute the dot product of this <c>DoubleVector</c>  x with another <c>DoubleVector</c>  y and return as <c>double</c></summary>
    ///<param name="Y"><c>DoubleVector</c> to dot product with this <c>DoubleVector</c>.</param>
    ///<returns><c>double</c> results from x dot y.</returns>
    public double GetDotProduct(DoubleVector Y)
    {
      if (Y == null)
      {
        throw new ArgumentNullException("Vector cannot be null.");
      }
      return Blas.Dot.Compute(this.Length, this.data, 1, Y.data, 1);
    }

    ///<summary>Compute the Euclidean Norm ||x||_2 of this <c>DoubleVector</c></summary>
    ///<returns><c>double</c> results from norm.</returns>
    public double GetNorm()
    {
      return Blas.Nrm2.Compute(this.Length, this.data, 1);
    }

    ///<summary>Compute the P Norm of this <c>DoubleVector</c></summary>
    ///<returns><c>double</c> results from norm.</returns>
    ///<remarks>p &gt; 0, if p &lt; 0, ABS(p) is used. If p = 0, the infinity norm is returned.</remarks>
    public double GetNorm(double p)
    {
      if (p == 0)
      {
        return GetInfinityNorm();
      }
      if (p < 0)
      {
        p = -p;
      }
      double ret = 0;
      for (int i = 0; i < data.Length; i++)
      {
        ret += System.Math.Pow(System.Math.Abs(data[i]), p);
      }
      return System.Math.Pow(ret, 1 / p);
    }

    ///<summary>Compute the Infinity Norm of this <c>DoubleVector</c></summary>
    ///<returns><c>double</c> results from norm.</returns>
    public double GetInfinityNorm()
    {
      double ret = 0;
      for (int i = 0; i < data.Length; i++)
      {
        double tmp = System.Math.Abs(data[i]);
        if (tmp > ret)
        {
          ret = tmp;
        }
      }
      return ret;
    }

    ///<summary>Sum the components in this <c>DoubleVector</c></summary>
    ///<returns><c>double</c> results from the summary of <c>DoubleVector</c> components.</returns>
    public double GetSum()
    {
      double ret = 0;
      for (int i = 0; i < data.Length; ++i)
      {
        ret += data[i];
      }
      return ret;
    }

    ///<summary>Compute the absolute sum of the elements of this <c>DoubleVector</c></summary>
    ///<returns><c>double</c> of absolute sum of the elements.</returns>
    public double GetSumMagnitudes()
    {
      return Blas.Asum.Compute(this.data.Length, this.data, 1);
    }

    ///<summary>Compute the sum y = alpha * x + y where y is this <c>DoubleVector</c></summary>
    ///<param name="alpha"><c>double</c> value to scale this <c>DoubleVector</c></param>
    ///<param name="X"><c>DoubleVector</c> to add to alpha * this <c>DoubleVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Axpy(double alpha, DoubleVector X)
    {
      if (X == null)
      {
        throw new ArgumentNullException("Vector cannot be null.");
      }
      Blas.Axpy.Compute(data.Length, alpha, X.data, 1, this.data, 1);
    }

    ///<summary>Scale this <c>DoubleVector</c> by a <c>double</c> scalar</summary>
    ///<param name="alpha"><c>double</c> value to scale this <c>DoubleVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Scale(double alpha)
    {
      Blas.Scal.Compute(data.Length, alpha, data, 1);
    }


    ///<summary>Negate operator for <c>DoubleVector</c></summary>
    ///<returns><c>DoubleVector</c> with values to negate.</returns>
    public static DoubleVector operator -(DoubleVector rhs)
    {
      DoubleVector ret = new DoubleVector(rhs);
      Blas.Scal.Compute(ret.Length, -1, ret.data, 1);
      return ret;
    }

    ///<summary>Negate operator for <c>DoubleVector</c></summary>
    ///<returns><c>DoubleVector</c> with values to negate.</returns>
    public static DoubleVector Negate(DoubleVector rhs)
    {
      if (rhs == null)
      {
        throw new ArgumentNullException("rhs", "rhs cannot be null");
      }
      return -rhs;
    }

    ///<summary>Subtract a <c>DoubleVector</c> from another <c>DoubleVector</c></summary>
    ///<param name="lhs"><c>DoubleVector</c> to subtract from.</param>
    ///<param name="rhs"><c>DoubleVector</c> to subtract.</param>
    ///<returns><c>DoubleVector</c> with results.</returns>
    public static DoubleVector operator -(DoubleVector lhs, DoubleVector rhs)
    {
      DoubleVector ret = new DoubleVector(lhs);
      Blas.Axpy.Compute(ret.Length, -1, rhs.data, 1, ret.data, 1);
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
      DoubleVector ret = new DoubleVector(lhs);
      Blas.Axpy.Compute(ret.Length, 1, rhs.data, 1, ret.data, 1);
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
      if (vector == null)
      {
        throw new System.ArgumentNullException("Vector cannot be null.");
      }
      Blas.Axpy.Compute(this.Length, 1, vector.data, 1, this.data, 1);
    }

    ///<summary>Subtract a <c>DoubleVector</c> from this<c>DoubleVector</c></summary>
    ///<param name="vector"><c>DoubleVector</c> to add.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null given as argument.</exception>
    public void Subtract(DoubleVector vector)
    {
      if (vector == null)
      {
        throw new System.ArgumentNullException("Vector cannot be null.");
      }
      Blas.Axpy.Compute(this.Length, -1, vector.data, 1, this.data, 1);
    }

    ///<summary>Scale this <c>DoubleVector</c> by a <c>double</c> scalar</summary>
    ///<param name="value"><c>double</c> value to scale this <c>DoubleVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Multiply(double value)
    {
      this.Scale(value);
    }

    ///<summary>Divide this <c>DoubleVector</c> by a <c>double</c> scalar</summary>
    ///<param name="value"><c>double</c> value to divide this <c>DoubleVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Divide(double value)
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

    ///<summary>Multiply a <c>DoubleVector</c> with another <c>DoubleVector</c> as x*y^T</summary>
    ///<param name="lhs"><c>DoubleVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>DoubleVector</c> as right hand operand.</param>
    ///<returns><c>DoubleMatrix</c> with results.</returns>
    public static DoubleMatrix operator *(DoubleVector lhs, DoubleVector rhs)
    {
      DoubleMatrix ret = new DoubleMatrix(lhs.data.Length, rhs.data.Length);
#if MANAGED
      for (int i = 0; i < lhs.data.Length; i++)
      {
        for (int j = 0; j < rhs.data.Length; j++)
        {
          ret.data[i][j] = lhs.data[i] * rhs.data[j];
        }
      }
#else
      Blas.Ger.Compute(Blas.Order.ColumnMajor, lhs.data.Length, rhs.data.Length,1,lhs.data,1,rhs.data,1,ret.data,lhs.data.Length);
#endif

      return ret;
    }

    ///<summary>Multiply a <c>DoubleVector</c> with another <c>DoubleVector</c> as x*y^T</summary>
    ///<param name="lhs"><c>DoubleVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>DoubleVector</c> as right hand operand.</param>
    ///<returns><c>DoubleMatrix</c> with results.</returns>
    public static DoubleMatrix Multiply(DoubleVector lhs, DoubleVector rhs)
    {
      return lhs * rhs;
    }

    ///<summary>Multiply a <c>double</c> x with a <c>DoubleVector</c> y as x*y</summary>
    ///<param name="lhs"><c>double</c> as left hand operand.</param>
    ///<param name="rhs"><c>DoubleVector</c> as right hand operand.</param>
    ///<returns><c>DoubleVector</c> with results.</returns>
    public static DoubleVector operator *(double lhs, DoubleVector rhs)
    {
      DoubleVector ret = new DoubleVector(rhs);
#if MANAGED
      for (int i = 0; i < rhs.data.Length; i++)
        ret.data[i] = lhs * rhs.data[i];
#else
      Blas.Scal.Compute(ret.Length,lhs, ret.data,1);
#endif
      return ret;
    }

    ///<summary>Multiply a <c>double</c> x with a <c>DoubleVector</c> y as x*y</summary>
    ///<param name="lhs"><c>double</c> as left hand operand.</param>
    ///<param name="rhs"><c>DoubleVector</c> as right hand operand.</param>
    ///<returns><c>DoubleVector</c> with results.</returns>
    public static DoubleVector Multiply(double lhs, DoubleVector rhs)
    {
      return lhs * rhs;
    }

    ///<summary>Multiply a <c>DoubleVector</c> x with a <c>double</c> y as x*y</summary>
    ///<param name="lhs"><c>DoubleVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>double</c> as right hand operand.</param>
    ///<returns><c>DoubleVector</c> with results.</returns>
    public static DoubleVector operator *(DoubleVector lhs, double rhs)
    {
      return rhs * lhs;
    }

    ///<summary>Multiply a <c>DoubleVector</c> x with a <c>double</c> y as x*y</summary>
    ///<param name="lhs"><c>DoubleVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>double</c> as right hand operand.</param>
    ///<returns><c>DoubleVector</c> with results.</returns>
    public static DoubleVector Multiply(DoubleVector lhs, double rhs)
    {
      return lhs * rhs;
    }

    ///<summary>Divide a <c>DoubleVector</c> x with a <c>double</c> y as x/y</summary>
    ///<param name="lhs"><c>DoubleVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>double</c> as right hand operand.</param>
    ///<returns><c>DoubleVector</c> with results.</returns>
    public static DoubleVector operator /(DoubleVector lhs, double rhs)
    {
      DoubleVector ret = new DoubleVector(lhs);
#if MANAGED
      for (int i = 0; i < lhs.data.Length; i++)
        ret[i] = lhs.data[i] / rhs;
#else
      Blas.Scal.Compute(ret.Length, 1/rhs, ret.data,1);
#endif
      return ret;
    }

    ///<summary>Divide a <c>DoubleVector</c> x with a <c>double</c> y as x/y</summary>
    ///<param name="lhs"><c>DoubleVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>double</c> as right hand operand.</param>
    ///<returns><c>DoubleVector</c> with results.</returns>
    public static DoubleVector Divide(DoubleVector lhs, double rhs)
    {
      return lhs / rhs;
    }

    ///<summary>Clone (deep copy) a <c>DoubleVector</c> variable</summary>
    public DoubleVector Clone()
    {
      return new DoubleVector(this);
    }

    // --- ICloneable Interface ---
    ///<summary>Clone (deep copy) a <c>DoubleVector</c> variable</summary>
    Object ICloneable.Clone()
    {
      return this.Clone();
    }

    // --- IFormattable Interface ---

    ///<summary>A string representation of this <c>DoubleVector</c>.</summary>
    ///<returns>The string representation of the value of <c>this</c> instance.</returns>
    public override string ToString()
    {
      return ToString(null, null);
    }

    ///<summary>A string representation of this <c>DoubleVector</c>.</summary>
    ///<param name="format">A format specification.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by format.</returns>
    public string ToString(string format)
    {
      return ToString(format, null);
    }

    ///<summary>A string representation of this <c>DoubleVector</c>.</summary>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by provider.</returns>
    public string ToString(IFormatProvider formatProvider)
    {
      return ToString(null, formatProvider);
    }

    ///<summary>A string representation of this <c>DoubleVector</c>.</summary>
    ///<param name="format">A format specification.</param>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by format and provider.</returns>
    public string ToString(string format, IFormatProvider formatProvider)
    {
      StringBuilder sb = new StringBuilder("Length: ");
      sb.Append(data.Length).Append(System.Environment.NewLine);
      for (int i = 0; i < data.Length; ++i)
      {
        sb.Append(data[i].ToString(format, formatProvider));
        if (i != data.Length - 1)
        {
          sb.Append(", ");
        }
      }
      return sb.ToString();
    }

    // --- IEnumerable Interface ---
    ///<summary> Return an IEnumerator </summary>
    public IEnumerator GetEnumerator()
    {
      return new DoubleVectorEnumerator(this);
    }

    // --- ICollection Interface ---
    ///<summary> Get the number of elements in the vector </summary>
    public int Count
    {
      get { return this.Length; }
    }
    int ICollection.Count
    {
      get { return this.Count; }
    }

    ///<summary> Get a boolean indicating whether the data storage method of this vector is thread-safe</summary>
    public bool IsSynchronized
    {
      get { return this.data.IsSynchronized; }
    }
    bool ICollection.IsSynchronized
    {
      get { return this.IsSynchronized; }
    }

    ///<summary> Get an object that can be used to synchronize the data storage method of this vector</summary>
    object ICollection.SyncRoot
    {
      get { return this.data.SyncRoot; }
    }

    ///<summary> Copy the components of this vector to an array </summary>
    public void CopyTo(Array array, int index)
    {
      this.data.CopyTo(array, index);
    }
    void ICollection.CopyTo(Array array, int index)
    {
      this.CopyTo(array, index);
    }

    // --- IList Interface ---

    ///<summary>Returns false indicating that the IList interface supports addition and removal of elements</summary>
    public bool IsFixedSize
    {
      get { return false; }
    }

    ///<summary>Returns false indicating that the IList interface supports addition, removal, and modification of elements</summary>
    public bool IsReadOnly
    {
      get { return false; }
    }

    ///<summary>Access a <c>DoubleVector</c> element</summary>
    ///<param name="index">The element to access</param>
    ///<exception cref="ArgumentOutOfRangeException">Exception thrown in element accessed is out of the bounds of the vector.</exception>
    ///<returns>Returns a <c>ComplexDouble</c> vector element</returns>
    object IList.this[int index]
    {
      get { return (object)this[index]; }
      set { this[index] = (double)value; }
    }

    ///<summary>Add a new value to the end of the <c>DoubleVector</c></summary>
    public int Add(object value)
    {
      double[] newdata = new double[data.Length + 1];
      int newpos = newdata.Length - 1;

      System.Array.Copy(data, newdata, data.Length);
      newdata[newpos] = (double)value;
      data = newdata;
      return newpos;
    }

    ///<summary>Set all values in the <c>DoubleVector</c> to zero </summary>
    public void Clear()
    {
      for (int i = 0; i < data.Length; i++)
        data[i] = 0;
    }

    ///<summary>Check if the any of the <c>DoubleVector</c> components equals a given <c>double</c></summary>
    public bool Contains(object value)
    {
      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] == (double)value)
          return true;
      }
      return false;
    }

    ///<summary>Return the index of the <c>xDoubleVector</c> for the first component that equals a given <c>double</c></summary>
    public int IndexOf(object value)
    {
      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] == (double)value)
          return i;
      }
      return -1;
    }

    ///<summary>Insert a <c>double</c> into the <c>DoubleVector</c> at a given index</summary>
    public void Insert(int index, object value)
    {
      if (index > data.Length)
      {
        throw new System.ArgumentOutOfRangeException("index");
      }

      double[] newdata = new double[data.Length + 1];
      System.Array.Copy(data, newdata, index);
      newdata[index] = (double)value;
      System.Array.Copy(data, index, newdata, index + 1, data.Length - index);
      data = newdata;
    }

    ///<summary>Remove the first instance of a given <c>double</c> from the <c>DoubleVector</c></summary>
    public void Remove(object value)
    {
      int index = this.IndexOf(value);

      if (index == -1)
        return;
      this.RemoveAt(index);
    }

    ///<summary>Remove the component of the <c>DoubleVector</c> at a given index</summary>
    public void RemoveAt(int index)
    {
      if (index >= data.Length)
        throw new System.ArgumentOutOfRangeException("index");

      double[] newdata = new double[data.Length - 1];
      System.Array.Copy(data, newdata, index);
      if (index < data.Length)
        System.Array.Copy(data, index + 1, newdata, index, newdata.Length - index);
      data = newdata;
    }

    /// <summary>
    /// Direct access to the underlying data array. Use this with care!
    /// </summary>
    /// <returns>The underlying data array.</returns>
    internal double[] Store()
    {
      return data;
    }

    /// <summary>
    /// Resizes the vector to the same boundaries as the provided vector and copies the elements from it.
    /// </summary>
    /// <param name="a">The vector to copy the data from.</param>
    public void CopyFrom(IROVector a)
    {
      Resize(a.LowerBound, a.UpperBound);
      int lo = a.LowerBound;
      for (int i = 0; i < data.Length; ++i)
        data[i] = a[i+lo];
    }

    #region Additions due to adoption to Altaxo

    ///<summary>Constructor for <c>DoubleVector</c> to deep copy from a <see cref="IROVector" /></summary>
    ///<param name="src"><c>Vector</c> to deep copy into <c>DoubleVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'src' parameter.</exception>
    public DoubleVector(IROVector src)
    {
      if (src == null)
      {
        throw new ArgumentNullException("IROVector cannot be null");
      }
      if (src is DoubleVector)
      {
        data = (double[]) (((DoubleVector)src).data.Clone());
      }
      else
      {
        data = new double[src.Length];
        for (int i = 0; i < src.Length; ++i)
        {
          data[i] = src[i];
        }
      }
    }

    /// <summary>
    /// Returns the column of a <see cref="IROMatrix" /> as a new <c>DoubleVector.</c>
    /// </summary>
    /// <param name="mat">The matrix to copy the column from.</param>
    /// <param name="col">Number of column to copy from the matrix.</param>
    /// <returns>A new <c>DoubleVector</c> with the same elements as the column of the given matrix.</returns>
    public static DoubleVector GetColumn(IROMatrix mat, int col)
    {
      DoubleVector result = new DoubleVector(mat.Rows);
      for (int i = 0; i < result.data.Length; ++i)
        result.data[i] = mat[i, col];

      return result;
    }

    /// <summary>
    /// Returns the column of a <see cref="IROMatrix" /> as a new <c>double[]</c> array.
    /// </summary>
    /// <param name="mat">The matrix to copy the column from.</param>
    /// <param name="col">Index of the column to copy from the matrix.</param>
    /// <returns>A new array of <c>double</c> with the same elements as the column of the given matrix.</returns>
    public static double[] GetColumnAsArray(IROMatrix mat, int col)
    {
      double[] result = new double[mat.Rows];
      for (int i = 0; i < result.Length; ++i)
        result[i] = mat[i, col];

      return result;
    }

    /// <summary>
    /// Returns the row of a <see cref="IROMatrix" /> as a new <c>DoubleVector.</c>
    /// </summary>
    /// <param name="mat">The matrix to copy the column from.</param>
    /// <param name="row">Index of the row to copy from the matrix.</param>
    /// <returns>A new <c>DoubleVector</c> with the same elements as the row of the given matrix.</returns>
    public static DoubleVector GetRow(IROMatrix mat, int row)
    {
      DoubleVector result = new DoubleVector(mat.Columns);
      for (int i = 0; i < result.data.Length; ++i)
        result.data[i] = mat[row, i];

      return result;
    }

    #endregion
  }
}



