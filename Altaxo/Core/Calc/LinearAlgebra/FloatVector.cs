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

using System;
using System.Collections;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
  ///<summary>
  /// Defines a Vector of floats.
  ///</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  [Serializable]
  sealed public class FloatVector
    : ICloneable, IFormattable, IEnumerable, ICollection, IList, IFloatVector
  {
    internal float[] data;

    ///<summary>Constructor for <c>FloatVector</c> with components set to zero</summary>
    ///<param name="length">Length of vector.</param>
    ///<exception cref="ArgumentException">Exception thrown if length parameter isn't positive</exception>
    public FloatVector(int length)
    {
      if (length < 1)
      {
        throw new ArgumentException("length must be positive.", "length");
      }
      data = new float[length];
    }

    ///<summary>Constructor for <c>FloatVector</c> with components set to a value</summary>
    ///<param name="length">Length of vector.</param>
    ///<param name="value"><c>float</c> value to set all components.</param>
    ///<exception cref="ArgumentException">Exception thrown if length parameter isn't positive</exception>
    public FloatVector(int length, float value)
    {
      if (length < 1)
      {
        throw new ArgumentException("length must be positive.", "length");
      }
      data = new float[length];
      for (int i = 0; i < data.Length; ++i)
      {
        data[i] = value;
      }
    }

    ///<summary>Constructor for <c>FloatVector</c> from <c>float</c> array</summary>
    ///<param name="values">Array of <c>float</c> to convert into <c>FloatVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'value' parameter.</exception>
    public FloatVector(float[] values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("Array alues cannot be null");
      }
      data = new float[values.Length];
      for (int i = 0; i < values.Length; ++i)
      {
        data[i] = values[i];
      }
    }

    ///<summary>Constructor for <c>FloatVector</c> from <c>IList</c></summary>
    ///<param name="values"><c>IList</c> to convert into <c>FloatVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'values' parameter.</exception>
    public FloatVector(IList values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("IList cannot be null");
      }
      data = new float[values.Count];
      for (int i = 0; i < values.Count; ++i)
      {
        data[i] = (float)values[i];
      }

    }

    ///<summary>Constructor for <c>FloatVector</c> to deep copy another <c>FloatVector</c></summary>
    ///<param name="src"><c>FloatVector</c> to deep copy into <c>FloatVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'src' parameter.</exception>
    public FloatVector(FloatVector src)
    {
      if (src == null)
      {
        throw new ArgumentNullException("FloatVector cannot be null");
      }
      data = new float[src.data.Length];
      Array.Copy(src.data, 0, data, 0, data.Length);
    }

    ///<summary>Return the length of the <c>FloatVector</c> variable</summary>
    ///<returns>The length.</returns>
    public int Length
    {
      get
      {
        return data.Length;
      }
    }

    ///<summary>Access a <c>FloatVector</c> element</summary>
    ///<param name="index">The element to access</param>
    ///<exception cref="IndexOutOfRangeException">Exception thrown in element accessed is out of the bounds of the vector.</exception>
    ///<returns>Returns a <c>float</c> vector element</returns>
    public float this[int index]
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

    ///<summary>Check if <c>FloatVector</c> variable is the same as another object</summary>
    ///<param name="obj"><c>obj</c> to compare present <c>FloatVector</c> to.</param>
    ///<returns>Returns true if the variable is the same as the <c>FloatVector</c> variable</returns>
    ///<remarks>The <c>obj</c> parameter is converted into a <c>FloatVector</c> variable before comparing with the current <c>DoubleVector</c>.</remarks>
    public override bool Equals(Object obj)
    {
      FloatVector vector = obj as FloatVector;
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

    ///<summary>Return the Hashcode for the <c>FloatVector</c></summary>
    ///<returns>The Hashcode representation of <c>FloatVector</c></returns>
    public override int GetHashCode()
    {
      return (int)this.GetNorm();
    }

    ///<summary>Retrieves a refernce to the public array.</summary>
    ///<returns>Reference to the public <c>float</c> array.</returns>
    public float[] GetInternalData()
    {
      return this.data;
    }

    ///<summary>Return <c>float</c> array of data from <c>FloatVector</c></summary>
    ///<returns><c>float</c> array with data.</returns>
    public float[] ToArray()
    {
      float[] ret = new float[data.Length];
      Array.Copy(data, ret, data.Length);
      return ret;
    }

    ///<summary>Returns a subvector of the <c>FloatVector</c></summary>
    ///<param name="startElement">Return data starting from this element.</param>
    ///<param name="endElement">Return data ending in this element.</param>
    ///<returns><c>FloatVector</c> a subvector of the reference vector</returns>
    ///<exception cref="ArgumentException">Exception thrown if <paramref>endElement</paramref> is greater than <paramref>startElement</paramref></exception>
    ///<exception cref="ArgumentOutOfRangeException">Exception thrown if input dimensions are out of the range of <c>FloatVector</c> dimensions</exception>
    public FloatVector GetSubVector(int startElement, int endElement)
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
      FloatVector ret = new FloatVector(n);
      for (int i = 0; i < n; i++)
      {
        ret[i] = this[i + startElement];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>FloatVector</c> from <c>float</c> array</summary>
    static public implicit operator FloatVector(float[] src)
    {
      return new FloatVector(src);
    }

    ///<summary>Implicit cast conversion to <c>FloatVector</c> from <c>float</c> array</summary>
    static public FloatVector ToFloatVector(float[] src)
    {
      return new FloatVector(src);
    }

    ///<summary>Explicit cast conversion to <c>FloatVector</c> from <c>DoubleVector</c></summary>
    static public explicit operator FloatVector(DoubleVector src)
    {
      if (src == null)
      {
        throw new ArgumentNullException("DoubleVector cannot be null");
      }
      FloatVector ret = new FloatVector(src.Length);
      // Can't use Array.Copy to implicitly copy from a double[] to a float[]
      for (int i = 0; i < src.Length; i++)
      {
        ret[i] = (float)src[i];
      }
      return ret;
    }

    ///<summary>Return the index of the absolute maximum element in the <c>FloatVector</c></summary>
    ///<returns>Index value of maximum element.</returns>
    public int GetAbsMaximumIndex()
    {
      return Blas.Imax.Compute(this.Length, this.data, 1);
    }

    ///<summary>Return the <c>float</c> value of the maximum element in the <c>FloatVector</c></summary>
    ///<returns><c>float</c> value of maximum element.</returns>
    public float GetAbsMaximum()
    {
      return this.data[Blas.Imax.Compute(this.Length, this.data, 1)];
    }

    ///<summary>Return the index of the minimum element in the <c>FloatVector</c></summary>
    ///<returns>Index value of minimum element.</returns>
    public int GetAbsMinimumIndex()
    {
      return Blas.Imin.Compute(this.Length, this.data, 1);
    }

    ///<summary>Return the <c>float</c> value of the minimum element in the <c>FloatVector</c></summary>
    ///<returns><c>float</c> value of minimum element.</returns>
    public float GetAbsMinimum()
    {
      return this.data[Blas.Imin.Compute(this.Length, this.data, 1)];
    }

    ///<summary>Clone (deep copy) of this <c>FloatVector</c> into another <c>FloatVector</c></summary>
    ///<param name="src"><c>FloatVector</c> to deepcopy this <c>FloatVector</c> into.</param>
    public void Copy(FloatVector src)
    {
      if (src == null)
      {
        throw new System.ArgumentNullException("FloatVector cannot be null.");
      }

      Blas.Copy.Compute(src.Length, src.data, 1, this.data, 1);
    }

    ///<summary>Swap data in this <c>FloatVector</c> with another <c>FloatVector</c></summary>
    ///<param name="src"><c>FloatVector</c> to swap data with.</param>
    public void Swap(FloatVector src)
    {
      if (src == null)
      {
        throw new System.ArgumentNullException("FloatVector cannot be null.");
      }

      Blas.Swap.Compute(src.Length, src.data, 1, this.data, 1);
    }

    ///<summary>Compute the dot product of this <c>FloatVector</c> with itself and return as <c>float</c></summary>
    ///<returns><c>float</c> results from x dot x.</returns>
    public float GetDotProduct()
    {
      return GetDotProduct(this);
    }

    ///<summary>Compute the dot product of this <c>FloatVector</c> x with another <c>FloatVector</c>  y and return as <c>float</c></summary>
    ///<param name="Y"><c>FloatVector</c> to dot product with this <c>FloatVector</c>.</param>
    ///<returns><c>float</c> results from x dot y.</returns>
    public float GetDotProduct(FloatVector Y)
    {
      return Blas.Dot.Compute(this.Length, this.data, 1, Y.data, 1);
    }

    ///<summary>Compute the dot product of this <c>FloatVector</c> with itself using extended precision and return as <c>float</c></summary>
    ///<param name="alpha">value added to the inner product.</param>
    ///<returns><c>float</c> results from x dot x.</returns>
    public double GetSDotProduct(float alpha)
    {
      return GetSDotProduct(alpha, this);
    }

    ///<summary>Compute the dot product of this <c>FloatVector</c> x with another <c>FloatVector</c> y and return as <c>float</c></summary>
    ///<param name="alpha">value added to the inner product.</param>
    ///<param name="Y"><c>FloatVector</c> to dot product with this <c>FloatVector</c>.</param>
    ///<returns><c>float</c> results from x dot y.</returns>
    public double GetSDotProduct(float alpha, FloatVector Y)
    {
      if (Y == null)
      {
        throw new System.ArgumentNullException("FloatVector cannot be null.");
      }
      return Blas.Sdot.Compute(this.Length, alpha, this.data, 1, Y.data, 1);
    }

    ///<summary>Compute the dot product of this <c>FloatVector</c> with itself using extended precision and return as <c>float</c></summary>
    ///<returns><c>float</c> results from x dot x.</returns>
    public double GetSDotProduct()
    {
      return GetSDotProduct(this);
    }

    ///<summary>Compute the dot product of this <c>FloatVector</c> x with another <c>FloatVector</c> y and return as <c>float</c></summary>
    ///<param name="Y"><c>FloatVector</c> to dot product with this <c>FloatVector</c>.</param>
    ///<returns><c>float</c> results from x dot y.</returns>
    public double GetSDotProduct(FloatVector Y)
    {
      if (Y == null)
      {
        throw new System.ArgumentNullException("FloatVector cannot be null.");
      }

      return Blas.Sdot.Compute(this.Length, this.data, 1, Y.data, 1);
    }

    ///<summary>Compute the Euclidean Norm ||x||_2 of this <c>FloatVector</c></summary>
    ///<returns><c>float</c> results from norm.</returns>
    public float GetNorm()
    {
      return Blas.Nrm2.Compute(this.Length, this.data, 1);
    }

    ///<summary>Compute the P Norm of this <c>FloatVector</c></summary>
    ///<returns><c>float</c> results from norm.</returns>
    ///<remarks>p &gt; 0, if p &lt; 0, ABS(p) is used. If p = 0, the infinity norm is returned.</remarks>
    public float GetNorm(double p)
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
      return (float)System.Math.Pow(ret, 1 / p);
    }

    ///<summary>Compute the Infinity Norm of this <c>FloatVector</c></summary>
    ///<returns><c>float</c> results from norm.</returns>
    public float GetInfinityNorm()
    {
      float ret = 0;
      for (int i = 0; i < data.Length; i++)
      {
        float tmp = System.Math.Abs(data[i]);
        if (tmp > ret)
        {
          ret = tmp;
        }
      }
      return ret;
    }
    ///<summary>Sum the components in this <c>FloatVector</c></summary>
    ///<returns><c>float</c> results from the summary of <c>FloatVector</c> components.</returns>
    public float GetSum()
    {
      float ret = 0;
      for (int i = 0; i < data.Length; ++i)
      {
        ret += data[i];
      }
      return ret;
    }

    ///<summary>Compute the absolute sum of the elements of this <c>FloatVector</c></summary>
    ///<returns><c>float</c> of absolute sum of the elements.</returns>
    public float GetSumMagnitudes()
    {
      return Blas.Asum.Compute(this.data.Length, this.data, 1);
    }

    ///<summary>Compute the sum y = alpha * x + y where y is this <c>FloatVector</c></summary>
    ///<param name="alpha"><c>float</c> value to scale this <c>FloatVector</c></param>
    ///<param name="X"><c>FloatVector</c> to add to alpha * this <c>FloatVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Axpy(float alpha, FloatVector X)
    {
      if (X == null)
      {
        throw new System.ArgumentNullException("FloatVector cannot be null.");
      }

      Blas.Axpy.Compute(this.data.Length, alpha, X.data, 1, this.data, 1);
    }

    ///<summary>Scale this <c>FloatVector</c> by a <c>float</c> scalar</summary>
    ///<param name="alpha"><c>float</c> value to scale this <c>FloatVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Scale(float alpha)
    {
      Blas.Scal.Compute(data.Length, alpha, this.data, 1);
    }


    ///<summary>Negate operator for <c>FloatVector</c></summary>
    ///<returns><c>FloatVector</c> with values to negate.</returns>
    public static FloatVector operator -(FloatVector rhs)
    {
      FloatVector ret = new FloatVector(rhs);
      Blas.Scal.Compute(ret.Length, -1, ret.data, 1);
      return ret;
    }

    ///<summary>Negate operator for <c>FloatVector</c></summary>
    ///<returns><c>FloatVector</c> with values to negate.</returns>
    public static FloatVector Negate(FloatVector rhs)
    {
      if (rhs == null)
      {
        throw new ArgumentNullException("rhs", "rhs cannot be null");
      }
      return -rhs;
    }

    ///<summary>Subtract a <c>FloatVector</c> from another <c>FloatVector</c></summary>
    ///<param name="lhs"><c>FloatVector</c> to subtract from.</param>
    ///<param name="rhs"><c>FloatVector</c> to subtract.</param>
    ///<returns><c>FloatVector</c> with results.</returns>
    public static FloatVector operator -(FloatVector lhs, FloatVector rhs)
    {
      FloatVector ret = new FloatVector(lhs);
      Blas.Axpy.Compute(ret.Length, -1, rhs.data, 1, ret.data, 1);
      return ret;
    }

    ///<summary>Subtract a <c>FloatVector</c> from another <c>FloatVector</c></summary>
    ///<param name="lhs"><c>FloatVector</c> to subtract from.</param>
    ///<param name="rhs"><c>FloatVector</c> to subtract.</param>
    ///<returns><c>FloatVector</c> with results.</returns>
    public static FloatVector Subtract(FloatVector lhs, FloatVector rhs)
    {
      return lhs - rhs;
    }

    ///<summary>Add a <c>FloatVector</c> to another <c>FloatVector</c></summary>
    ///<param name="lhs"><c>FloatVector</c> to add to.</param>
    ///<param name="rhs"><c>FloatVector</c> to add.</param>
    ///<returns><c>FloatVector</c> with results.</returns>
    public static FloatVector operator +(FloatVector lhs, FloatVector rhs)
    {
      FloatVector ret = new FloatVector(lhs);
      Blas.Axpy.Compute(ret.Length, 1, rhs.data, 1, ret.data, 1);
      return ret;
    }

    ///<summary>Add a <c>FloatVector</c> to another <c>FloatVector</c></summary>
    ///<param name="rhs"><c>FloatVector</c> to add.</param>
    ///<returns><c>FloatVector</c> with results.</returns>
    public static FloatVector operator +(FloatVector rhs)
    {
      return rhs;
    }

    ///<summary>Add a <c>FloatVector</c> to this<c>FloatVector</c></summary>
    ///<param name="vector"><c>FloatVector</c> to add.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null given as argument.</exception>
    public void Add(FloatVector vector)
    {
      if (vector == null)
      {
        throw new System.ArgumentNullException("FloatVector cannot be null.");
      }

      Blas.Axpy.Compute(this.Length, 1, vector.data, 1, this.data, 1);
    }

    ///<summary>Subtract a <c>FloatVector</c> from this<c>FloatVector</c></summary>
    ///<param name="vector"><c>FloatVector</c> to add.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null given as argument.</exception>
    public void Subtract(FloatVector vector)
    {
      if (vector == null)
      {
        throw new System.ArgumentNullException("FloatVector cannot be null.");
      }
      Blas.Axpy.Compute(this.Length, -1, vector.data, 1, this.data, 1);
    }

    ///<summary>Scale this <c>FloatVector</c> by a <c>float</c> scalar</summary>
    ///<param name="value"><c>float</c> value to scale this <c>FloatVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Multiply(float value)
    {
      this.Scale(value);
    }

    ///<summary>Divide this <c>FloatVector</c> by a <c>float</c> scalar</summary>
    ///<param name="value"><c>float</c> value to divide this <c>FloatVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Divide(float value)
    {
      this.Scale(1 / value);
    }

    ///<summary>Add a <c>FloatVector</c> to another <c>FloatVector</c></summary>
    ///<param name="lhs"><c>FloatVector</c> to add to.</param>
    ///<param name="rhs"><c>FloatVector</c> to add.</param>
    ///<returns><c>FloatVector</c> with results.</returns>
    public static FloatVector Add(FloatVector lhs, FloatVector rhs)
    {
      return lhs + rhs;
    }

    ///<summary>Multiply a <c>FloatVector</c> with another <c>FloatVector</c> as x*y^T</summary>
    ///<param name="lhs"><c>FloatVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>FloatVector</c> as right hand operand.</param>
    ///<returns><c>FloatMatrix</c> with results.</returns>
    public static FloatMatrix operator *(FloatVector lhs, FloatVector rhs)
    {
      FloatMatrix ret = new FloatMatrix(lhs.data.Length, rhs.data.Length);
#if MANAGED
      for( int i = 0; i < lhs.data.Length; i++)
      {
        for( int j = 0; j < rhs.data.Length; j++)
        {
          ret[i,j] = lhs.data[i]*rhs.data[j];
        }
      } 
#else
      Blas.Ger.Compute(Blas.Order.ColumnMajor, lhs.data.Length, rhs.data.Length, 1, lhs.data, 1, rhs.data, 1, ret.data, lhs.data.Length);
#endif
      return ret;
    }

    ///<summary>Multiply a <c>FloatVector</c> with another <c>FloatVector</c> as x*y^T</summary>
    ///<param name="lhs"><c>FloatVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>FloatVector</c> as right hand operand.</param>
    ///<returns><c>FloatMatrix</c> with results.</returns>
    public static FloatMatrix Multiply(FloatVector lhs, FloatVector rhs)
    {
      return lhs * rhs;
    }

    ///<summary>Multiply a <c>float</c> x with a <c>FloatVector</c> y as x*y</summary>
    ///<param name="lhs"><c>float</c> as left hand operand.</param>
    ///<param name="rhs"><c>FloatVector</c> as right hand operand.</param>
    ///<returns><c>FloatVector</c> with results.</returns>
    public static FloatVector operator *(float lhs, FloatVector rhs)
    {
      FloatVector ret = new FloatVector(rhs);
      Blas.Scal.Compute(ret.Length, lhs, ret.data, 1);
      return ret;
    }

    ///<summary>Multiply a <c>float</c> x with a <c>FloatVector</c> y as x*y</summary>
    ///<param name="lhs"><c>float</c> as left hand operand.</param>
    ///<param name="rhs"><c>FloatVector</c> as right hand operand.</param>
    ///<returns><c>FloatVector</c> with results.</returns>
    public static FloatVector Multiply(float lhs, FloatVector rhs)
    {
      return lhs * rhs;
    }

    ///<summary>Multiply a <c>FloatVector</c> x with a <c>float</c> y as x*y</summary>
    ///<param name="lhs"><c>FloatVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>float</c> as right hand operand.</param>
    ///<returns><c>FloatVector</c> with results.</returns>
    public static FloatVector operator *(FloatVector lhs, float rhs)
    {
      return rhs * lhs;
    }

    ///<summary>Multiply a <c>FloatVector</c> x with a <c>float</c> y as x*y</summary>
    ///<param name="lhs"><c>FloatVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>float</c> as right hand operand.</param>
    ///<returns><c>FloatVector</c> with results.</returns>
    public static FloatVector Multiply(FloatVector lhs, float rhs)
    {
      return lhs * rhs;
    }

    ///<summary>Divide a <c>FloatVector</c> x with a <c>float</c> y as x/y</summary>
    ///<param name="lhs"><c>FloatVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>float</c> as right hand operand.</param>
    ///<returns><c>FloatVector</c> with results.</returns>
    public static FloatVector operator /(FloatVector lhs, float rhs)
    {
      FloatVector ret = new FloatVector(lhs);
      Blas.Scal.Compute(ret.Length, 1 / rhs, ret.data, 1);
      return ret;
    }

    ///<summary>Divide a <c>FloatVector</c> x with a <c>float</c> y as x/y</summary>
    ///<param name="lhs"><c>FloatVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>float</c> as right hand operand.</param>
    ///<returns><c>FloatVector</c> with results.</returns>
    public static FloatVector Divide(FloatVector lhs, float rhs)
    {
      return lhs / rhs;
    }

    ///<summary>Clone (deep copy) a <c>FloatVector</c> variable</summary>
    public FloatVector Clone()
    {
      return new FloatVector(this);
    }

    // --- ICloneable Interface ---
    ///<summary>Clone (deep copy) a <c>FloatVector</c> variable</summary>
    Object ICloneable.Clone()
    {
      return this.Clone();
    }

    // --- IFormattable Interface ---

    ///<summary>A string representation of this <c>FloatVector</c>.</summary>
    ///<returns>The string representation of the value of <c>this</c> instance.</returns>
    public override string ToString()
    {
      return ToString(null, null);
    }

    ///<summary>A string representation of this <c>FloatVector</c>.</summary>
    ///<param name="format">A format specification.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by format.</returns>
    public string ToString(string format)
    {
      return ToString(format, null);
    }

    ///<summary>A string representation of this <c>FloatVector</c>.</summary>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by provider.</returns>
    public string ToString(IFormatProvider formatProvider)
    {
      return ToString(null, formatProvider);
    }

    ///<summary>A string representation of this <c>FloatVector</c>.</summary>
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
      return new FloatVectorEnumerator(this);
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

    ///<summary>Access a <c>FloatVector</c> element</summary>
    ///<param name="index">The element to access</param>
    ///<exception cref="ArgumentOutOfRangeException">Exception thrown in element accessed is out of the bounds of the vector.</exception>
    ///<returns>Returns a <c>ComplexDouble</c> vector element</returns>
    object IList.this[int index]
    {
      get { return (object)this[index]; }
      set { this[index] = (float)value; }
    }

    ///<summary>Add a new value to the end of the <c>FloatVector</c></summary>
    public int Add(object value)
    {
      float[] newdata = new float[data.Length + 1];
      int newpos = newdata.Length - 1;

      System.Array.Copy(data, newdata, data.Length);
      newdata[newpos] = (float)value;
      data = newdata;
      return newpos;
    }

    ///<summary>Set all values in the <c>floatVector</c> to zero </summary>
    public void Clear()
    {
      for (int i = 0; i < data.Length; i++)
        data[i] = 0;
    }

    ///<summary>Check if the any of the <c>DoubleVector</c> components equals a given <c>float</c></summary>
    public bool Contains(object value)
    {
      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] == (float)value)
          return true;
      }
      return false;
    }

    ///<summary>Return the index of the <c>xDoubleVector</c> for the first component that equals a given <c>float</c></summary>
    public int IndexOf(object value)
    {
      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] == (float)value)
          return i;
      }
      return -1;
    }

    ///<summary>Insert a <c>float</c> into the <c>FloatVector</c> at a given index</summary>
    public void Insert(int index, object value)
    {
      if (index > data.Length)
      {
        throw new System.ArgumentOutOfRangeException("index");
      }

      float[] newdata = new float[data.Length + 1];
      System.Array.Copy(data, newdata, index);
      newdata[index] = (float)value;
      System.Array.Copy(data, index, newdata, index + 1, data.Length - index);
      data = newdata;
    }

    ///<summary>Remove the first instance of a given <c>float</c> from the <c>FloatVector</c></summary>
    public void Remove(object value)
    {
      int index = this.IndexOf(value);

      if (index == -1)
        return;
      this.RemoveAt(index);
    }

    ///<summary>Remove the component of the <c>FloatVector</c> at a given index</summary>
    public void RemoveAt(int index)
    {
      if (index >= data.Length)
        throw new System.ArgumentOutOfRangeException("index");

      float[] newdata = new float[data.Length - 1];
      System.Array.Copy(data, newdata, index);
      if (index < data.Length)
        System.Array.Copy(data, index + 1, newdata, index, newdata.Length - index);
      data = newdata;
    }

    #region IROFloatVector Members

    public int LowerBound
    {
      get { return 0; }
    }

    public int UpperBound
    {
      get { return data.Length-1; }
    }

    #endregion

    #region Additions due to adoption to Altaxo

    ///<summary>Constructor for <c>FloatVector</c> to deep copy from a <see cref="IROFloatVector" /></summary>
    ///<param name="src"><c>Vector</c> to deep copy into <c>FloatVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'src' parameter.</exception>
    public FloatVector(IROFloatVector src)
    {
      if (src == null)
      {
        throw new ArgumentNullException("IROVector cannot be null");
      }
      if (src is FloatVector)
      {
        data = (float[])(((FloatVector)src).data.Clone());
      }
      else
      {
        data = new float[src.Length];
        for (int i = 0; i < src.Length; ++i)
        {
          data[i] = src[i];
        }
      }
    }

    /// <summary>
    /// Returns the column of a <see cref="IROFloatMatrix" /> as a new <c>FloatVector.</c>
    /// </summary>
    /// <param name="mat">The matrix to copy the column from.</param>
    /// <param name="col">Index of the column to copy from the matrix.</param>
    /// <returns>A new <c>FloatVector</c> with the same elements as the column of the given matrix.</returns>
    public static FloatVector GetColumn(IROFloatMatrix mat, int col)
    {
      FloatVector result = new FloatVector(mat.Rows);
      for (int i = 0; i < result.data.Length; ++i)
        result.data[i] = mat[i, col];

      return result;
    }

    /// <summary>
    /// Returns the column of a <see cref="IROFloatMatrix" /> as a new <c>float[]</c> array.
    /// </summary>
    /// <param name="mat">The matrix to copy the column from.</param>
    /// <param name="col">Index of the column to copy from the matrix.</param>
    /// <returns>A new array of <c>float</c> with the same elements as the column of the given matrix.</returns>
    public static float[] GetColumnAsArray(IROFloatMatrix mat, int col)
    {
      float[] result = new float[mat.Rows];
      for (int i = 0; i < result.Length; ++i)
        result[i] = mat[i, col];

      return result;
    }

    /// <summary>
    /// Returns the row of a <see cref="IROFloatMatrix" /> as a new <c>FloatVector.</c>
    /// </summary>
    /// <param name="mat">The matrix to copy the column from.</param>
    /// <param name="row">Index of the row to copy from the matrix.</param>
    /// <returns>A new <c>DoubleVector</c> with the same elements as the row of the given matrix.</returns>
    public static FloatVector GetRow(IROFloatMatrix mat, int row)
    {
      FloatVector result = new FloatVector(mat.Columns);
      for (int i = 0; i < result.data.Length; ++i)
        result.data[i] = mat[row, i];

      return result;
    }

    #endregion
  }
}



