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
  /// Defines a Vector of ComplexDoubles.
  ///</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  [Serializable]
  sealed public class ComplexFloatVector : ICloneable, IFormattable, IList, IComplexFloatVector
  {
    internal ComplexFloat[] data;

    ///<summary>Constructor for <c>ComplexFloatVector</c> with components set to zero</summary>
    ///<param name="length">Length of vector.</param>
    ///<exception cref="ArgumentException">Exception thrown if length parameter isn't positive</exception>
    public ComplexFloatVector(int length)
    {
      if (length < 1)
      {
        throw new ArgumentException("length must be positive.", "length");
      }
      data = new ComplexFloat[length];
    }

    ///<summary>Constructor for <c>ComplexFloatVector</c> with components set to a value</summary>
    ///<param name="length">Length of vector.</param>
    ///<param name="value"><c>ComplexFloat</c> value to set all components.</param>
    ///<exception cref="ArgumentException">Exception thrown if length parameter isn't positive</exception>
    public ComplexFloatVector(int length, ComplexFloat value)
    {
      if (length < 1)
      {
        throw new ArgumentException("length must be positive.", "length");
      }
      data = new ComplexFloat[length];
      for (int i = 0; i < data.Length; ++i)
      {
        data[i] = value;
      }
    }

    ///<summary>Constructor for <c>ComplexFloatVector</c> from <c>ComplexFloat</c> array</summary>
    ///<param name="values">Array of <c>ComplexFloat</c> to convert into <c>ComplexFloatVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'value' parameter.</exception>
    public ComplexFloatVector(ComplexFloat[] values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("Array cannot be null");
      }
      data = new ComplexFloat[values.Length];
      for (int i = 0; i < values.Length; ++i)
      {
        data[i] = values[i];
      }
    }

    ///<summary>Constructor for <c>ComplexFloatVector</c> from <c>IList</c></summary>
    ///<param name="values"><c>IList</c> to convert into <c>ComplexFloatVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'values' parameter.</exception>
    public ComplexFloatVector(IList values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("IList cannot be null");
      }
      data = new ComplexFloat[values.Count];
      for (int i = 0; i < values.Count; ++i)
      {
        data[i] = (ComplexFloat)values[i];
      }
    }

    ///<summary>Constructor for <c>ComplexFloatVector</c> to deep copy another <c>ComplexFloatVector</c></summary>
    ///<param name="src"><c>ComplexFloatVector</c> to deep copy into <c>ComplexFloatVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'src' parameter.</exception>
    public ComplexFloatVector(ComplexFloatVector src)
    {
      if (src == null)
      {
        throw new ArgumentNullException("ComplexFloatVector cannot be null");
      }
      data = new ComplexFloat[src.data.Length];
      Array.Copy(src.data, 0, data, 0, data.Length);
    }

    ///<summary>Return the length of the <c>ComplexFloatVector</c> variable</summary>
    ///<returns>The length.</returns>
    public int Length
    {
      get
      {
        return data.Length;
      }
    }

    ///<summary>Return a <c>FloatVector</c> with the real components of the <c>ComplexFloatVector</c></summary>
    ///<returns><c>FloatVector</c> of real components</returns>
    public FloatVector Real
    {
      get
      {
        FloatVector returnvector = new FloatVector(this.Length);
        for (int i = 0; i < this.Length; i++)
          returnvector[i] = this[i].Real;
        return returnvector;
      }
    }

    ///<summary>Return a <c>FloatVector</c> with the imaginary components of the <c>ComplexFloatVector</c></summary>
    ///<returns><c>FloatVector</c> of imaginary components</returns>
    public FloatVector Imag
    {
      get
      {
        FloatVector returnvector = new FloatVector(this.Length);
        for (int i = 0; i < this.Length; i++)
          returnvector[i] = this[i].Imag;
        return returnvector;
      }
    }

    ///<summary>Access a <c>ComplexFloatVector</c> element</summary>
    ///<param name="index">The element to access</param>
    ///<exception cref="IndexOutOfRangeException">Exception thrown in element accessed is out of the bounds of the vector.</exception>
    ///<returns>Returns a <c>ComplexFloat</c> vector element</returns>
    public ComplexFloat this[int index]
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

    ///<summary>Return the Hashcode for the <c>ComplexFloatVector</c></summary>
    ///<returns>The Hashcode representation of <c>ComplexFloatVector</c></returns>
    public override int GetHashCode()
    {
      return (int)this.GetNorm();
    }

    ///<summary>Check if <c>ComplexFloatVector</c> variable is the same as another object</summary>
    ///<param name="obj"><c>obj</c> to compare present <c>ComplexFloatVector</c> to.</param>
    ///<returns>Returns true if the variable is the same as the <c>ComplexFloatVector</c> variable</returns>
    ///<remarks>The <c>obj</c> parameter is converted into a <c>ComplexFloatVector</c> variable before comparing with the current <c>ComplexFloatVector</c>.</remarks>
    public override bool Equals(Object obj)
    {
      ComplexFloatVector vector = obj as ComplexFloatVector;
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

    ///<summary>Retrieves a refernce to the internal array.</summary>
    ///<returns>Reference to the internal <c>ComplexFloat</c> array.</returns>
    public ComplexFloat[] GetInternalData()
    {
      return this.data;
    }

    ///<summary>Return <c>ComplexFloat</c> array of data from <c>ComplexFloatVector</c></summary>
    ///<returns><c>ComplexFloat</c> array with data.</returns>
    public ComplexFloat[] ToArray()
    {
      ComplexFloat[] ret = new ComplexFloat[data.Length];
      Array.Copy(data, 0, ret, 0, data.Length);
      return ret;
    }

    ///<summary>Returns a subvector of the <c>ComplexFloatVector</c></summary>
    ///<param name="startElement">Return data starting from this element.</param>
    ///<param name="endElement">Return data ending in this element.</param>
    ///<returns><c>ComplexFloatVector</c> a subvector of the reference vector</returns>
    ///<exception cref="ArgumentException">Exception thrown if <paramref>endElement</paramref> is greater than <paramref>startElement</paramref></exception>
    ///<exception cref="ArgumentOutOfRangeException">Exception thrown if input dimensions are out of the range of <c>ComplexFloatVector</c> dimensions</exception>
    public ComplexFloatVector GetSubVector(int startElement, int endElement)
    {
      if (startElement > endElement)
      {
        throw new ArgumentException("The starting element must be less that the ending element.");
      }

      if (startElement < 0 || startElement >= this.Length)
      {
        throw new ArgumentOutOfRangeException("startElement");
      }
      if (endElement < 0 || endElement >= this.Length)
      {
        throw new ArgumentOutOfRangeException("endElement");
      }

      int n = endElement - startElement + 1;
      ComplexFloatVector ret = new ComplexFloatVector(n);
      for (int i = 0; i < n; i++)
      {
        ret[i] = this[i + startElement];
      }
      return ret;
    }


    ///<summary>Implicit cast conversion to <c>ComplexFloatVector</c> from <c>float</c> array</summary>
    static public implicit operator ComplexFloatVector(float[] src)
    {
      if (src == null)
      {
        return null;
      }
      ComplexFloatVector ret = new ComplexFloatVector(src.Length);
      for (int i = 0; i < src.Length; ++i)
      {
        ret.data[i] = src[i];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>ComplexFloatVector</c> from <c>float</c> array</summary>
    static public ComplexFloatVector ToComplexFloatVector(float[] src)
    {
      if (src == null)
      {
        return null;
      }
      ComplexFloatVector ret = new ComplexFloatVector(src.Length);
      for (int i = 0; i < src.Length; ++i)
      {
        ret.data[i] = src[i];
      }
      return ret;
    }

    ///<summary>Explicit cast conversion to <c>ComplexFloatVector</c> from <c>double</c> array</summary>
    static public explicit operator ComplexFloatVector(ComplexDoubleVector src)
    {
      if (src == null)
      {
        return null;
      }
      ComplexFloatVector ret = new ComplexFloatVector(src.Length);
      for (int i = 0; i < src.Length; ++i)
      {
        ret.data[i] = (ComplexFloat)src[i];
      }
      return ret;
    }

    ///<summary>Explicit cast conversion to <c>ComplexFloatVector</c> from <c>double</c> array</summary>
    static public ComplexFloatVector ToComplexFloatVector(ComplexDoubleVector src)
    {
      if (src == null)
      {
        return null;
      }
      ComplexFloatVector ret = new ComplexFloatVector(src.Length);
      for (int i = 0; i < src.Length; ++i)
      {
        ret.data[i] = (ComplexFloat)src[i];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>ComplexFloatVector</c> from <c>ComplexFloat</c> array</summary>
    static public implicit operator ComplexFloatVector(ComplexFloat[] src)
    {
      return new ComplexFloatVector(src);
    }

    ///<summary>Implicit cast conversion to <c>ComplexFloatVector</c> from <c>ComplexFloat</c> array</summary>
    static public ComplexFloatVector ToComplexFloatVector(ComplexFloat[] src)
    {
      return new ComplexFloatVector(src);
    }

    ///<summary>Implicit cast conversion to <c>ComplexFloatVector</c> from <c>FloatVector</c></summary>
    static public implicit operator ComplexFloatVector(FloatVector src)
    {
      float[] temp = src.ToArray();
      ComplexFloatVector ret = new ComplexFloatVector(temp.Length);
      for (int i = 0; i < temp.Length; ++i)
      {
        ret.data[i] = temp[i];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>ComplexFloatVector</c> from <c>FloatVector</c></summary>
    static public ComplexFloatVector ToComplexFloatVector(FloatVector src)
    {
      float[] temp = src.ToArray();
      ComplexFloatVector ret = new ComplexFloatVector(temp.Length);
      for (int i = 0; i < temp.Length; ++i)
      {
        ret.data[i] = temp[i];
      }
      return ret;
    }

    ///<summary>Return the index of the absolute maximum element in the <c>ComplexFloatVector</c></summary>
    ///<returns>Index value of maximum element.</returns>
    public int GetAbsMaximumIndex()
    {
      return Blas.Imax.Compute(this.Length, this.data, 1);
    }

    ///<summary>Return the <c>ComplexFloat</c> value of the absolute maximum element in the <c>ComplexFloatVector</c></summary>
    ///<returns><c>ComplexFloat</c> value of maximum element.</returns>
    public ComplexFloat GetAbsMaximum()
    {
      return this.data[Blas.Imax.Compute(this.Length, this.data, 1)];
    }

    ///<summary>Return the index of the absolute minimum element in the <c>ComplexFloatVector</c></summary>
    ///<returns>Index value of minimum element.</returns>
    public int GetAbsMinimumIndex()
    {
      return Blas.Imin.Compute(this.Length, this.data, 1);
    }

    ///<summary>Return the <c>ComplexFloat</c> value of the absolute minimum element in the <c>ComplexFloatVector</c></summary>
    ///<returns><c>ComplexFloat</c> value of minimum element.</returns>
    public ComplexFloat GetAbsMinimum()
    {
      return this.data[Blas.Imin.Compute(this.Length, this.data, 1)];
    }

    ///<summary>Clone (deep copy) of this <c>ComplexFloatVector</c> into another <c>ComplexFloatVector</c></summary>
    ///<param name="src"><c>ComplexFloatVector</c> to deepcopy this <c>ComplexFloatVector</c> into.</param>
    public void Copy(ComplexFloatVector src)
    {
      if (src == null)
      {
        throw new System.ArgumentNullException("CfVector cannot be null.");
      }

      Blas.Copy.Compute(src.Length, src.data, 1, this.data, 1);
    }

    ///<summary>Swap data in this <c>ComplexFloatVector</c> with another <c>ComplexFloatVector</c></summary>
    ///<param name="src"><c>ComplexFloatVector</c> to swap data with.</param>
    public void Swap(ComplexFloatVector src)
    {
      if (src == null)
      {
        throw new System.ArgumentNullException("CfVector cannot be null.");
      }
      Blas.Swap.Compute(src.Length, src.data, 1, this.data, 1);
    }

    ///<summary>Compute the complex scalar product x^Tx and return as <c>ComplexFloat</c></summary>
    ///<returns><c>ComplexFloat</c> results from x^Tx.</returns>
    public ComplexFloat GetDotProduct()
    {
      return GetDotProduct(this);
    }

    ///<summary>Compute the complex scalar product x^Ty and return as <c>ComplexFloat</c></summary>
    ///<param name="Y"><c>ComplexFloatVector</c> to act as y operand in x^Ty.</param>
    ///<returns><c>ComplexFloat</c> results from x^Ty.</returns>
    public ComplexFloat GetDotProduct(ComplexFloatVector Y)
    {
      if (Y == null)
      {
        throw new System.ArgumentNullException("ComplexFloatVector cannot be null.");
      }
      return Blas.Dotu.Compute(this.Length, this.data, 1, Y.data, 1);
    }

    ///<summary>Compute the complex conjugate scalar product x^Hx and return as <c>ComplexFloat</c></summary>
    ///<returns><c>ComplexFloat</c> results from x^Hx.</returns>
    public ComplexFloat GetConjugateDotProduct()
    {
      return GetConjugateDotProduct(this);
    }

    ///<summary>Compute the complex conjugate scalar product x^Hy and return as <c>ComplexFloat</c></summary>
    ///<param name="Y"><c>ComplexFloatVector</c> to act as y operand in x^Hy.</param>
    ///<returns><c>ComplexFloat</c> results from x^Hy.</returns>
    public ComplexFloat GetConjugateDotProduct(ComplexFloatVector Y)
    {
      if (Y == null)
      {
        throw new System.ArgumentNullException("ComplexFloatVector cannot be null.");
      }
      return Blas.Dotc.Compute(this.Length, this.data, 1, Y.data, 1);
    }

    ///<summary>Compute the Euclidean Norm ||x||_2 of this <c>ComplexFloatVector</c></summary>
    ///<returns><c>float</c> results from norm.</returns>
    public float GetNorm()
    {
      return Blas.Nrm2.Compute(this.Length, this.data, 1);
    }

    ///<summary>Compute the P Norm of this <c>ComplexFloatVector</c></summary>
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
        ret += System.Math.Pow(ComplexMath.Absolute(data[i]), p);
      }
      return (float)System.Math.Pow(ret, 1 / p);
    }

    ///<summary>Compute the Infinity Norm of this <c>ComplexFloatVector</c></summary>
    ///<returns><c>float</c> results from norm.</returns>
    public float GetInfinityNorm()
    {
      float ret = 0;
      for (int i = 0; i < data.Length; i++)
      {
        float tmp = ComplexMath.Absolute(data[i]);
        if (tmp > ret)
        {
          ret = tmp;
        }
      }
      return ret;
    }

    ///<summary>Sum the components in this <c>ComplexFloatVector</c></summary>
    ///<returns><c>ComplexFloat</c> results from the summary of <c>ComplexFloatVector</c> components.</returns>
    public ComplexFloat GetSum()
    {
      ComplexFloat ret = 0;
      for (int i = 0; i < data.Length; ++i)
      {
        ret += data[i];
      }
      return ret;
    }

    ///<summary>Compute the absolute sum of the elements of this <c>ComplexFloatVector</c></summary>
    ///<returns><c>float</c> of absolute sum of the elements.</returns>
    public float GetSumMagnitudes()
    {
      return Blas.Asum.Compute(this.data.Length, this.data, 1);
    }

    ///<summary>Compute the sum y = alpha * x + y where y is this <c>ComplexFloatVector</c></summary>
    ///<param name="alpha"><c>ComplexFloat</c> value to scale this <c>ComplexFloatVector</c></param>
    ///<param name="X"><c>ComplexFloatVector</c> to add to alpha * this <c>ComplexFloatVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Axpy(ComplexFloat alpha, ComplexFloatVector X)
    {
      if (X == null)
      {
        throw new System.ArgumentNullException("ComplexFloatVector cannot be null.");
      }
      Blas.Axpy.Compute(data.Length, alpha, X.data, 1, this.data, 1);
    }

    ///<summary>Scale this <c>ComplexFloatVector</c> by a <c>float</c> scalar</summary>
    ///<param name="alpha"><c>float</c> value to scale this <c>ComplexFloatVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Scale(float alpha)
    {
      Blas.Scal.Compute(data.Length, alpha, data, 1);
    }

    ///<summary>Scale this <c>ComplexFloatVector</c> by a <c>ComplexFloat</c> scalar</summary>
    ///<param name="alpha"><c>ComplexFloat</c> value to scale this <c>ComplexFloatVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Scale(ComplexFloat alpha)
    {
      Blas.Scal.Compute(data.Length, alpha, data, 1);
    }

    ///<summary>Negate operator for <c>ComplexFloatVector</c></summary>
    ///<returns><c>ComplexFloatVector</c> with values to negate.</returns>
    public static ComplexFloatVector operator -(ComplexFloatVector rhs)
    {
      ComplexFloatVector ret = new ComplexFloatVector(rhs);
      Blas.Scal.Compute(rhs.Length, -1, ret.data, 1);
      return ret;
    }

    ///<summary>Negate operator for <c>ComplexFloatVector</c></summary>
    ///<returns><c>ComplexFloatVector</c> with values to negate.</returns>
    public static ComplexFloatVector Negate(ComplexFloatVector rhs)
    {
      if (rhs == null)
      {
        throw new ArgumentNullException("rhs", "rhs cannot be null");
      }
      return -rhs;
    }

    ///<summary>Subtract a <c>ComplexFloatVector</c> from another <c>ComplexFloatVector</c></summary>
    ///<param name="lhs"><c>ComplexFloatVector</c> to subtract from.</param>
    ///<param name="rhs"><c>ComplexFloatVector</c> to subtract.</param>
    ///<returns><c>ComplexFloatVector</c> with results.</returns>
    public static ComplexFloatVector operator -(ComplexFloatVector lhs, ComplexFloatVector rhs)
    {
      ComplexFloatVector ret = new ComplexFloatVector(lhs);
      Blas.Axpy.Compute(ret.Length, -1, rhs.data, 1, ret.data, 1);
      return ret;
    }

    ///<summary>Subtract a <c>ComplexFloatVector</c> from another <c>ComplexFloatVector</c></summary>
    ///<param name="lhs"><c>ComplexFloatVector</c> to subtract from.</param>
    ///<param name="rhs"><c>ComplexFloatVector</c> to subtract.</param>
    ///<returns><c>ComplexFloatVector</c> with results.</returns>
    public static ComplexFloatVector Subtract(ComplexFloatVector lhs, ComplexFloatVector rhs)
    {
      return lhs - rhs;
    }

    ///<summary>Add a <c>ComplexFloatVector</c> to another <c>ComplexFloatVector</c></summary>
    ///<param name="lhs"><c>ComplexFloatVector</c> to add to.</param>
    ///<param name="rhs"><c>ComplexFloatVector</c> to add.</param>
    ///<returns><c>ComplexFloatVector</c> with results.</returns>
    public static ComplexFloatVector operator +(ComplexFloatVector lhs, ComplexFloatVector rhs)
    {
      ComplexFloatVector ret = new ComplexFloatVector(lhs);
      Blas.Axpy.Compute(ret.Length, 1, rhs.data, 1, ret.data, 1);
      return ret;
    }

    ///<summary>Add a <c>ComplexFloatVector</c> to another <c>ComplexFloatVector</c></summary>
    ///<param name="rhs"><c>ComplexFloatVector</c> to add.</param>
    ///<returns><c>ComplexFloatVector</c> with results.</returns>
    public static ComplexFloatVector operator +(ComplexFloatVector rhs)
    {
      return rhs;
    }

    ///<summary>Add a <c>ComplexFloatVector</c> to this<c>ComplexFloatVector</c></summary>
    ///<param name="vector"><c>ComplexFloatVector</c> to add.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null given as argument.</exception>
    public void Add(ComplexFloatVector vector)
    {
      if (vector == null)
      {
        throw new System.ArgumentNullException("ComplexFloatVector cannot be null.");
      }
      Blas.Axpy.Compute(this.Length, 1, vector.data, 1, this.data, 1);
    }

    ///<summary>Subtract a <c>ComplexFloatVector</c> from this<c>ComplexFloatVector</c></summary>
    ///<param name="vector"><c>ComplexFloatVector</c> to add.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null given as argument.</exception>
    public void Subtract(ComplexFloatVector vector)
    {
      if (vector == null)
      {
        throw new System.ArgumentNullException("ComplexFloatVector cannot be null.");
      }
      Blas.Axpy.Compute(this.Length, -1, vector.data, 1, this.data, 1);
    }

    ///<summary>Scale this <c>ComplexFloatVector</c> by a <c>ComplexFloat</c> scalar</summary>
    ///<param name="value"><c>ComplexFloat</c> value to scale this <c>ComplexFloatVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Multiply(ComplexFloat value)
    {
      this.Scale(value);
    }

    ///<summary>Divide this <c>ComplexFloatVector</c> by a <c>ComplexFloat</c> scalar</summary>
    ///<param name="value"><c>ComplexFloat</c> value to divide this <c>ComplexFloatVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Divide(ComplexFloat value)
    {
      this.Scale(1.0f / value);
    }

    ///<summary>Add a <c>ComplexFloatVector</c> to another <c>ComplexFloatVector</c></summary>
    ///<param name="lhs"><c>ComplexFloatVector</c> to add to.</param>
    ///<param name="rhs"><c>ComplexFloatVector</c> to add.</param>
    ///<returns><c>ComplexFloatVector</c> with results.</returns>
    public static ComplexFloatVector Add(ComplexFloatVector lhs, ComplexFloatVector rhs)
    {
      return lhs + rhs;
    }

    ///<summary>Multiply a <c>ComplexFloatVector</c> with another <c>ComplexFloatVector</c> as x*y^T</summary>
    ///<param name="lhs"><c>ComplexFloatVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>ComplexFloatVector</c> as right hand operand.</param>
    ///<returns><c>ComplexFloatVector</c> with results.</returns>
    public static ComplexFloatMatrix operator *(ComplexFloatVector lhs, ComplexFloatVector rhs)
    {
      ComplexFloatMatrix ret = new ComplexFloatMatrix(lhs.data.Length, rhs.data.Length);
#if MANAGED
      for( int i = 0; i < lhs.data.Length; i++)
      {
        for( int j = 0; j < rhs.data.Length; j++)
        {
          ret[i,j] = lhs.data[i]*rhs.data[j];
        }
      } 
#else
      Blas.Geru.Compute(Blas.Order.ColumnMajor, lhs.data.Length, rhs.data.Length, 1, lhs.data, 1, rhs.data, 1, ret.data, lhs.data.Length);
#endif
      return ret;
    }

    ///<summary>Multiply a <c>ComplexFloatVector</c> with another <c>ComplexFloatVector</c> as x*y^T</summary>
    ///<param name="lhs"><c>ComplexFloatVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>ComplexFloatVector</c> as right hand operand.</param>
    ///<returns><c>ComplexFloatVector</c> with results.</returns>
    public static ComplexFloatMatrix Multiply(ComplexFloatVector lhs, ComplexFloatVector rhs)
    {
      return lhs * rhs;
    }

    ///<summary>Multiply a <c>ComplexFloat</c> x with a <c>ComplexFloatVector</c> y as x*y</summary>
    ///<param name="lhs"><c>ComplexFloat</c> as left hand operand.</param>
    ///<param name="rhs"><c>ComplexFloatVector</c> as right hand operand.</param>
    ///<returns><c>ComplexFloatVector</c> with results.</returns>
    public static ComplexFloatVector operator *(ComplexFloat lhs, ComplexFloatVector rhs)
    {
      ComplexFloatVector ret = new ComplexFloatVector(rhs);
      Blas.Scal.Compute(ret.Length, lhs, ret.data, 1);
      return ret;
    }

    ///<summary>Multiply a <c>ComplexFloat</c> x with a <c>ComplexFloatVector</c> y as x*y</summary>
    ///<param name="lhs"><c>ComplexFloat</c> as left hand operand.</param>
    ///<param name="rhs"><c>ComplexFloatVector</c> as right hand operand.</param>
    ///<returns><c>ComplexFloatVector</c> with results.</returns>
    public static ComplexFloatVector Multiply(ComplexFloat lhs, ComplexFloatVector rhs)
    {
      return lhs * rhs;
    }

    ///<summary>Multiply a <c>ComplexFloatVector</c> x with a <c>ComplexFloat</c> y as x*y</summary>
    ///<param name="lhs"><c>ComplexFloatVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>ComplexFloat</c> as right hand operand.</param>
    ///<returns><c>ComplexFloatVector</c> with results.</returns>
    public static ComplexFloatVector operator *(ComplexFloatVector lhs, ComplexFloat rhs)
    {
      return rhs * lhs;
    }

    ///<summary>Multiply a <c>ComplexFloatVector</c> x with a <c>ComplexFloat</c> y as x*y</summary>
    ///<param name="lhs"><c>ComplexFloatVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>ComplexFloat</c> as right hand operand.</param>
    ///<returns><c>ComplexFloatVector</c> with results.</returns>
    public static ComplexFloatVector Multiply(ComplexFloatVector lhs, ComplexFloat rhs)
    {
      return lhs * rhs;
    }

    ///<summary>Divide a <c>ComplexFloatVector</c> x with a <c>ComplexFloat</c> y as x/y</summary>
    ///<param name="lhs"><c>ComplexFloatVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>ComplexFloat</c> as right hand operand.</param>
    ///<returns><c>ComplexFloatVector</c> with results.</returns>
    public static ComplexFloatVector operator /(ComplexFloatVector lhs, ComplexFloat rhs)
    {
      ComplexFloatVector ret = new ComplexFloatVector(lhs);
      Blas.Scal.Compute(ret.Length, 1.0f / rhs, ret.data, 1);
      return ret;
    }

    ///<summary>Divide a <c>ComplexFloatVector</c> x with a <c>ComplexFloat</c> y as x/y</summary>
    ///<param name="lhs"><c>ComplexFloatVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>ComplexFloat</c> as right hand operand.</param>
    ///<returns><c>ComplexFloatVector</c> with results.</returns>
    public static ComplexFloatVector Divide(ComplexFloatVector lhs, ComplexFloat rhs)
    {
      return lhs / rhs;
    }

    ///<summary>Clone (deep copy) a <c>ComplexFloatVector</c> variable</summary>
    public ComplexFloatVector Clone()
    {
      return new ComplexFloatVector(this);
    }

    // --- ICloneable Interface ---
    ///<summary>Clone (deep copy) a <c>ComplexFloatVector</c> variable</summary>
    Object ICloneable.Clone()
    {
      return this.Clone();
    }

    // --- IFormattable Interface ---

    ///<summary>A string representation of this <c>ComplexFloatVector</c>.</summary>
    ///<returns>The string representation of the value of <c>this</c> instance.</returns>
    public override string ToString()
    {
      return ToString(null, null);
    }

    ///<summary>A string representation of this <c>ComplexFloatVector</c>.</summary>
    ///<param name="format">A format specification.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by format.</returns>
    public string ToString(string format)
    {
      return ToString(format, null);
    }

    ///<summary>A string representation of this <c>ComplexFloatVector</c>.</summary>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by provider.</returns>
    public string ToString(IFormatProvider formatProvider)
    {
      return ToString(null, formatProvider);
    }

    ///<summary>A string representation of this <c>ComplexFloatVector</c>.</summary>
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
      return new ComplexFloatVectorEnumerator(this);
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

    ///<summary>Access a <c>ComplexFloatVector</c> element</summary>
    ///<param name="index">The element to access</param>
    ///<exception cref="ArgumentOutOfRangeException">Exception thrown in element accessed is out of the bounds of the vector.</exception>
    ///<returns>Returns a <c>ComplexDouble</c> vector element</returns>
    object IList.this[int index]
    {
      get { return (object)this[index]; }
      set { this[index] = (ComplexFloat)value; }
    }

    ///<summary>Add a new value to the end of the <c>ComplexFloatVector</c></summary>
    public int Add(object value)
    {
      ComplexFloat[] newdata = new ComplexFloat[data.Length + 1];
      int newpos = newdata.Length - 1;

      System.Array.Copy(data, newdata, data.Length);
      newdata[newpos] = (ComplexFloat)value;
      data = newdata;
      return newpos;
    }

    ///<summary>Set all values in the <c>ComplexFloatVector</c> to zero </summary>
    public void Clear()
    {
      for (int i = 0; i < data.Length; i++)
        data[i] = 0;
    }

    ///<summary>Check if the any of the <c>ComplexFloatVector</c> components equals a given <c>ComplexFloat</c></summary>
    public bool Contains(object value)
    {
      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] == (ComplexFloat)value)
          return true;
      }
      return false;
    }

    ///<summary>Return the index of the <c>ComplexFloatVector</c> for the first component that equals a given <c>ComplexFloat</c></summary>
    public int IndexOf(object value)
    {
      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] == (ComplexFloat)value)
          return i;
      }
      return -1;
    }

    ///<summary>Insert a <c>ComplexFloat</c> into the <c>ComplexFloatVector</c> at a given index</summary>
    public void Insert(int index, object value)
    {
      if (index > data.Length)
      {
        throw new System.ArgumentOutOfRangeException("index");
      }

      ComplexFloat[] newdata = new ComplexFloat[data.Length + 1];
      System.Array.Copy(data, newdata, index);
      newdata[index] = (ComplexFloat)value;
      System.Array.Copy(data, index, newdata, index + 1, data.Length - index);
      data = newdata;
    }

    ///<summary>Remove the first instance of a given <c>ComplexFloat</c> from the <c>ComplexFloatVector</c></summary>
    public void Remove(object value)
    {
      int index = this.IndexOf(value);

      if (index == -1)
        return;
      this.RemoveAt(index);
    }

    ///<summary>Remove the component of the <c>ComplexFloatVector</c> at a given index</summary>
    public void RemoveAt(int index)
    {
      if (index >= data.Length)
        throw new System.ArgumentOutOfRangeException("index");

      ComplexFloat[] newdata = new ComplexFloat[data.Length - 1];
      System.Array.Copy(data, newdata, index);
      if (index < data.Length)
        System.Array.Copy(data, index + 1, newdata, index, newdata.Length - index);
      data = newdata;
    }

    #region IROComplexFloatVector Members

    public int LowerBound
    {
      get { return 0; }
    }

    public int UpperBound
    {
      get { return data.Length - 1; }
    }

    #endregion

    #region Additions due to adoption to Altaxo

    ///<summary>Constructor for <c>ComplexDoubleVector</c> to deep copy from a <see cref="IROComplexDoubleVector" /></summary>
    ///<param name="src"><c>ComplexDoubleVector</c> to deep copy into <c>ComplexDoubleVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'src' parameter.</exception>
    public ComplexFloatVector(IROComplexFloatVector src)
    {
      if (src == null)
      {
        throw new ArgumentNullException("IROComplexFloatVector cannot be null");
      }
      if (src is ComplexFloatVector)
      {
        data = (ComplexFloat[]) (((ComplexFloatVector)src).data.Clone());
      }
      else
      {
        data = new ComplexFloat[src.Length];
        for (int i = 0; i < src.Length; ++i)
        {
          data[i] = src[i];
        }
      }
    }

    /// <summary>
    /// Returns the column of a <see cref="IROComplexFloatMatrix" /> as a new <c>ComplexFloatVector.</c>
    /// </summary>
    /// <param name="mat">The matrix to copy the column from.</param>
    /// <param name="col">Number of column to copy from the matrix.</param>
    /// <returns>A new <c>ComplexFloatVector</c> with the same elements as the column of the given matrix.</returns>
    public static ComplexFloatVector GetColumn(IROComplexFloatMatrix mat, int col)
    {
      ComplexFloatVector result = new ComplexFloatVector(mat.Rows);
      for (int i = 0; i < result.data.Length; ++i)
        result.data[i] = mat[i, col];

      return result;
    }

    /// <summary>
    /// Returns the column of a <see cref="IROComplexFloatMatrix" /> as a new <c>Complex[]</c> array.
    /// </summary>
    /// <param name="mat">The matrix to copy the column from.</param>
    /// <param name="col">Number of column to copy from the matrix.</param>
    /// <returns>A new array of <c>ComplexFloat</c> with the same elements as the column of the given matrix.</returns>
    public static ComplexFloat[] GetColumnAsArray(IROComplexFloatMatrix mat, int col)
    {
      ComplexFloat[] result = new ComplexFloat[mat.Rows];
      for (int i = 0; i < result.Length; ++i)
        result[i] = mat[i, col];

      return result;
    }

    /// <summary>
    /// Returns the row of a <see cref="IROComplexFloatMatrix" /> as a new <c>ComplexFloatVector.</c>
    /// </summary>
    /// <param name="mat">The matrix to copy the column from.</param>
    /// <param name="row">Number of row to copy from the matrix.</param>
    /// <returns>A new <c>ComplexFloatVector</c> with the same elements as the row of the given matrix.</returns>
    public static ComplexFloatVector GetRow(IROComplexFloatMatrix mat, int row)
    {
      ComplexFloatVector result = new ComplexFloatVector(mat.Columns);
      for (int i = 0; i < result.data.Length; ++i)
        result.data[i] = mat[row, i];

      return result;
    }

    #endregion
  }
}
