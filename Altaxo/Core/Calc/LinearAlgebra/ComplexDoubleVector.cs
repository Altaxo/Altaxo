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
  sealed public class ComplexDoubleVector
    : ICloneable, IFormattable, IEnumerable, ICollection, IList, IComplexDoubleVector
  {
    internal Complex[] data;

    ///<summary>Constructor for <c>ComplexDoubleVector</c> with components set to zero</summary>
    ///<param name="length">Length of vector.</param>
    ///<exception cref="ArgumentException">Exception thrown if length parameter isn't positive</exception>
    public ComplexDoubleVector(int length)
    {
      if (length < 1)
      {
        throw new ArgumentException("length must be positive.", "length");
      }
      data = new Complex[length];
    }

    ///<summary>Constructor for <c>ComplexDoubleVector</c> with components set to a value</summary>
    ///<param name="length">Length of vector.</param>
    ///<param name="value"><c>Complex</c> value to set all components.</param>
    ///<exception cref="ArgumentException">Exception thrown if length parameter isn't positive</exception>
    public ComplexDoubleVector(int length, Complex value)
    {
      if (length < 1)
      {
        throw new ArgumentException("length must be positive.", "length");
      }
      data = new Complex[length];
      for (int i = 0; i < data.Length; ++i)
      {
        data[i] = value;
      }
    }

    ///<summary>Constructor for <c>ComplexDoubleVector</c> from <c>Complex</c> array</summary>
    ///<param name="values">Array of <c>Complex</c> to convert into <c>ComplexDoubleVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'value' parameter.</exception>
    public ComplexDoubleVector(Complex[] values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("Array cannot be null");
      }
      data = new Complex[values.Length];
      for (int i = 0; i < values.Length; ++i)
      {
        data[i] = values[i];
      }
    }


    ///<summary>Constructor for <c>ComplexDoubleVector</c> from <c>IList</c></summary>
    ///<param name="values"><c>IList</c> to convert into <c>ComplexDoubleVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'values' parameter.</exception>
    public ComplexDoubleVector(IList values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("IList cannot be null");
      }
      data = new Complex[values.Count];
      for (int i = 0; i < values.Count; ++i)
      {
        data[i] = (Complex)values[i];
      }
    }

    ///<summary>Constructor for <c>ComplexDoubleVector</c> to deep copy another <c>ComplexDoubleVector</c></summary>
    ///<param name="src"><c>ComplexDoubleVector</c> to deep copy into <c>ComplexDoubleVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'src' parameter.</exception>
    public ComplexDoubleVector(ComplexDoubleVector src)
    {
      if (src == null)
      {
        throw new ArgumentNullException("ComplexDoubleVector cannot be null");
      }
      data = new Complex[src.data.Length];
      Array.Copy(src.data, 0, data, 0, data.Length);
    }

    ///<summary>Constructor for <c>ComplexDoubleVector</c> to deep copy from a <see cref="IROComplexDoubleVector" /></summary>
    ///<param name="src"><c>ComplexDoubleVector</c> to deep copy into <c>ComplexDoubleVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'src' parameter.</exception>
    public ComplexDoubleVector(IROComplexDoubleVector src)
    {
      if (src == null)
      {
        throw new ArgumentNullException("IROComplexDoubleVector cannot be null");
      }
      if (src is ComplexDoubleVector)
      {
        data = (Complex[])(((ComplexDoubleVector)src).data.Clone());
      }
      else
      {
        data = new Complex[src.Length];
        for (int i = 0; i < src.Length; ++i)
        {
          data[i] = src[i];
        }
      }
    }

    ///<summary>Return the length of the <c>ComplexDoubleVector</c> variable</summary>
    ///<returns>The length.</returns>
    public int Length
    {
      get
      {
        return data.Length;
      }
    }

    ///<summary>Return a <c>DoubleVector</c> with the real components of the <c>ComplexDoubleVector</c></summary>
    ///<returns><c>DoubleVector</c> of real components</returns>
    public DoubleVector Real
    {
      get
      {
        DoubleVector returnvector = new DoubleVector(this.Length);
        for (int i = 0; i < this.Length; i++)
          returnvector[i] = this[i].Real;
        return returnvector;
      }
    }

    ///<summary>Return a <c>DoubleVector</c> with the imaginary components of the <c>ComplexDoubleVector</c></summary>
    ///<returns><c>DoubleVector</c> of imaginary components</returns>
    public DoubleVector Imag
    {
      get
      {
        DoubleVector returnvector = new DoubleVector(this.Length);
        for (int i = 0; i < this.Length; i++)
          returnvector[i] = this[i].Imag;
        return returnvector;
      }
    }

    ///<summary>Return the Hashcode for the <c>ComplexDoubleVector</c></summary>
    ///<returns>The Hashcode representation of <c>ComplexDoubleVector</c></returns>
    public override int GetHashCode()
    {
      return (int)this.GetNorm();
    }

    ///<summary>Check if <c>ComplexDoubleVector</c> variable is the same as another object</summary>
    ///<param name="obj"><c>obj</c> to compare present <c>ComplexDoubleVector</c> to.</param>
    ///<returns>Returns true if the variable is the same as the <c>ComplexDoubleVector</c> variable</returns>
    ///<remarks>The <c>obj</c> parameter is converted into a <c>ComplexDoubleVector</c> variable before comparing with the current <c>ComplexDoubleVector</c>.</remarks>
    public override bool Equals(Object obj)
    {
      ComplexDoubleVector vector = obj as ComplexDoubleVector;
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

    ///<summary>Retrieves a refernce to the public array.</summary>
    ///<returns>Reference to the public <c>Complex</c> array.</returns>
    public Complex[] GetInternalData()
    {
      return this.data;
    }

    ///<summary>Return <c>Complex</c> array of data from <c>ComplexDoubleVector</c></summary>
    ///<returns><c>Complex</c> array with data.</returns>
    public Complex[] ToArray()
    {
      Complex[] ret = new Complex[data.Length];
      Array.Copy(data, 0, ret, 0, data.Length);
      return ret;
    }

    ///<summary>Returns a subvector of the <c>ComplexDoubleVector</c></summary>
    ///<param name="startElement">Return data starting from this element.</param>
    ///<param name="endElement">Return data ending in this element.</param>
    ///<returns><c>ComplexDoubleVector</c> a subvector of the reference vector</returns>
    ///<exception cref="ArgumentException">Exception thrown if <paramref>endElement</paramref> is greater than <paramref>startElement</paramref></exception>
    ///<exception cref="ArgumentOutOfRangeException">Exception thrown if input dimensions are out of the range of <c>ComplexDoubleVector</c> dimensions</exception>
    public ComplexDoubleVector GetSubVector(int startElement, int endElement)
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
      ComplexDoubleVector ret = new ComplexDoubleVector(n);
      for (int i = 0; i < n; i++)
      {
        ret[i] = this[i + startElement];
      }
      return ret;
    }


    ///<summary>Implicit cast conversion to <c>ComplexDoubleVector</c> from <c>float</c> array</summary>
    static public implicit operator ComplexDoubleVector(float[] src)
    {
      if (src == null)
      {
        return null;
      }
      ComplexDoubleVector ret = new ComplexDoubleVector(src.Length);
      for (int i = 0; i < src.Length; ++i)
      {
        ret.data[i] = src[i];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>ComplexDoubleVector</c> from <c>float</c> array</summary>
    static public ComplexDoubleVector ToComplexDoubleVector(float[] src)
    {
      if (src == null)
      {
        return null;
      }
      ComplexDoubleVector ret = new ComplexDoubleVector(src.Length);
      for (int i = 0; i < src.Length; ++i)
      {
        ret.data[i] = src[i];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>ComplexDoubleVector</c> from <c>double</c> array</summary>
    static public implicit operator ComplexDoubleVector(double[] src)
    {
      if (src == null)
      {
        return null;
      }
      ComplexDoubleVector ret = new ComplexDoubleVector(src.Length);
      for (int i = 0; i < src.Length; ++i)
      {
        ret.data[i] = src[i];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>ComplexDoubleVector</c> from <c>double</c> array</summary>
    static public ComplexDoubleVector ToComplexDoubleVector(double[] src)
    {
      if (src == null)
      {
        return null;
      }
      ComplexDoubleVector ret = new ComplexDoubleVector(src.Length);
      for (int i = 0; i < src.Length; ++i)
      {
        ret.data[i] = src[i];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>ComplexDoubleVector</c> from <c>ComplexFloat</c> array</summary>
    ///<returns><c>Complex</c> array with data from <c>ComplexFloat</c> array.</returns>
    static public implicit operator ComplexDoubleVector(ComplexFloat[] src)
    {
      if (src == null)
      {
        return null;
      }
      ComplexDoubleVector ret = new ComplexDoubleVector(src.Length);
      for (int i = 0; i < src.Length; ++i)
      {
        ret.data[i] = src[i];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>ComplexDoubleVector</c> from <c>ComplexFloat</c> array</summary>
    ///<returns><c>Complex</c> array with data from <c>ComplexFloat</c> array.</returns>
    static public ComplexDoubleVector ToComplexDoubleVector(ComplexFloat[] src)
    {
      if (src == null)
      {
        return null;
      }
      ComplexDoubleVector ret = new ComplexDoubleVector(src.Length);
      for (int i = 0; i < src.Length; ++i)
      {
        ret.data[i] = src[i];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>ComplexDoubleVector</c> from <c>Complex</c> array</summary>
    ///<returns><c>Complex</c> array with data from <c>Complex</c> array.</returns>
    static public implicit operator ComplexDoubleVector(Complex[] src)
    {
      return new ComplexDoubleVector(src);
    }

    ///<summary>Implicit cast conversion to <c>ComplexDoubleVector</c> from <c>Complex</c> array</summary>
    ///<returns><c>Complex</c> array with data from <c>Complex</c> array.</returns>
    static public ComplexDoubleVector ToComplexDoubleVector(Complex[] src)
    {
      return new ComplexDoubleVector(src);
    }

    ///<summary>Implicit cast conversion to <c>ComplexDoubleVector</c> from <c>DoubleVector</c></summary>
    static public implicit operator ComplexDoubleVector(DoubleVector src)
    {
      double[] temp = src.ToArray();
      ComplexDoubleVector ret = new ComplexDoubleVector(temp.Length);
      for (int i = 0; i < temp.Length; ++i)
      {
        ret.data[i] = temp[i];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>ComplexDoubleVector</c> from <c>DoubleVector</c></summary>
    static public ComplexDoubleVector ToComplexDoubleVector(DoubleVector src)
    {
      double[] temp = src.ToArray();
      ComplexDoubleVector ret = new ComplexDoubleVector(temp.Length);
      for (int i = 0; i < temp.Length; ++i)
      {
        ret.data[i] = temp[i];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>ComplexDoubleVector</c> from <c>FloatVector</c></summary>
    static public implicit operator ComplexDoubleVector(FloatVector src)
    {
      float[] temp = src.ToArray();
      ComplexDoubleVector ret = new ComplexDoubleVector(temp.Length);
      for (int i = 0; i < temp.Length; ++i)
      {
        ret.data[i] = temp[i];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>ComplexDoubleVector</c> from <c>FloatVector</c></summary>
    static public ComplexDoubleVector ToComplexDoubleVector(FloatVector src)
    {
      float[] temp = src.ToArray();
      ComplexDoubleVector ret = new ComplexDoubleVector(temp.Length);
      for (int i = 0; i < temp.Length; ++i)
      {
        ret.data[i] = temp[i];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>ComplexDoubleVector</c> from <c>ComplexFloatVector</c></summary>
    static public implicit operator ComplexDoubleVector(ComplexFloatVector src)
    {
      ComplexFloat[] temp = src.ToArray();
      ComplexDoubleVector ret = new ComplexDoubleVector(temp.Length);
      for (int i = 0; i < temp.Length; ++i)
      {
        ret.data[i] = temp[i];
      }
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>ComplexDoubleVector</c> from <c>ComplexFloatVector</c></summary>
    static public ComplexDoubleVector ToComplexDoubleVector(ComplexFloatVector src)
    {
      ComplexFloat[] temp = src.ToArray();
      ComplexDoubleVector ret = new ComplexDoubleVector(temp.Length);
      for (int i = 0; i < temp.Length; ++i)
      {
        ret.data[i] = temp[i];
      }
      return ret;
    }

    ///<summary>Return the index of the absolute maximum element in the <c>ComplexDoubleVector</c></summary>
    ///<returns>Index value of maximum element.</returns>
    public int GetAbsMaximumIndex()
    {
      return Blas.Imax.Compute(this.Length,this.data, 1);
    }

    ///<summary>Return the <c>Complex</c> value of the absolute maximum element in the <c>ComplexDoubleVector</c></summary>
    ///<returns><c>Complex</c> value of maximum element.</returns>
    public Complex GetAbsMaximum()
    {
      return this.data[GetAbsMaximumIndex()];
    }

    ///<summary>Return the index of the absolute minimum element in the <c>ComplexDoubleVector</c></summary>
    ///<returns>Index value of minimum element.</returns>
    public int GetAbsMinimumIndex()
    {
      return Blas.Imin.Compute(this.Length, this.data, 1);
    }

    ///<summary>Return the <c>Complex</c> value of the absolute minimum element in the <c>ComplexDoubleVector</c></summary>
    ///<returns><c>Complex</c> value of minimum element.</returns>
    public Complex GetAbsMinimum()
    {
      return this.data[Blas.Imin.Compute(this.Length, this.data, 1)];
    }

    ///<summary>Clone (deep copy) of this <c>ComplexDoubleVector</c> into another <c>ComplexDoubleVector</c></summary>
    ///<param name="src"><c>ComplexDoubleVector</c> to deepcopy this <c>ComplexDoubleVector</c> into.</param>
    public void Copy(ComplexDoubleVector src)
    {
      if (src == null)
      {
        throw new System.ArgumentNullException("ComplexDoubleVector cannot be null.");
      }
      Blas.Copy.Compute(src.Length, src.data, 1, this.data, 1);
    }

    ///<summary>Swap data in this <c>ComplexDoubleVector</c> with another <c>ComplexDoubleVector</c></summary>
    ///<param name="src"><c>ComplexDoubleVector</c> to swap data with.</param>
    public void Swap(ComplexDoubleVector src)
    {
      if (src == null)
      {
        throw new System.ArgumentNullException("ComplexDoubleVector cannot be null.");
      }
      Blas.Swap.Compute(src.Length, src.data, 1, this.data, 1);
    }

    ///<summary>Compute the complex scalar product x^Tx and return as <c>Complex</c></summary>
    ///<returns><c>Complex</c> results from x^Tx.</returns>
    public Complex GetDotProduct()
    {
      return GetDotProduct(this);
    }

    ///<summary>Compute the complex scalar product x^Ty and return as <c>Complex</c></summary>
    ///<param name="Y"><c>ComplexDoubleVector</c> to act as y operand in x^Ty.</param>
    ///<returns><c>Complex</c> results from x^Ty.</returns>
    public Complex GetDotProduct(ComplexDoubleVector Y)
    {
      if (Y == null)
      {
        throw new System.ArgumentNullException("ComplexDoubleVector cannot be null.");
      }
      return Blas.Dotu.Compute(this.Length, this.data, 1, Y.data, 1);
    }

    ///<summary>Compute the complex conjugate scalar product x^Hx and return as <c>Complex</c></summary>
    ///<returns><c>Complex</c> results from x^Hx.</returns>
    public Complex GetConjugateDotProduct()
    {
      return GetConjugateDotProduct(this);
    }

    ///<summary>Compute the complex conjugate scalar product x^Hy and return as <c>Complex</c></summary>
    ///<param name="Y"><c>ComplexDoubleVector</c> to act as y operand in x^Hy.</param>
    ///<returns><c>Complex</c> results from x^Hy.</returns>
    public Complex GetConjugateDotProduct(ComplexDoubleVector Y)
    {
      if (Y == null)
      {
        throw new System.ArgumentNullException("ComplexDoubleVector cannot be null.");
      }
      return Blas.Dotc.Compute(this.Length, this.data, 1, Y.data, 1);
    }

    ///<summary>Compute the Euclidean Norm ||x||_2 of this <c>ComplexDoubleVector</c></summary>
    ///<returns><c>double</c> results from norm.</returns>
    public double GetNorm()
    {
      return Blas.Nrm2.Compute(this.Length, this.data, 1);
    }

    ///<summary>Compute the P Norm of this <c>ComplexDoubleVector</c></summary>
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
        ret += System.Math.Pow(ComplexMath.Absolute(data[i]), p);
      }
      return System.Math.Pow(ret, 1 / p);
    }

    ///<summary>Compute the Infinity Norm of this <c>ComplexDoubleVector</c></summary>
    ///<returns><c>double</c> results from norm.</returns>
    public double GetInfinityNorm()
    {
      double ret = 0;
      for (int i = 0; i < data.Length; i++)
      {
        double tmp = ComplexMath.Absolute(data[i]);
        if (tmp > ret)
        {
          ret = tmp;
        }
      }
      return ret;
    }

    ///<summary>Sum the components in this <c>ComplexDoubleVector</c></summary>
    ///<returns><c>Complex</c> results from the summary of <c>ComplexDoubleVector</c> components.</returns>
    public Complex GetSum()
    {
      Complex ret = 0;
      for (int i = 0; i < data.Length; ++i)
      {
        ret += data[i];
      }
      return ret;
    }

    ///<summary>Compute the absolute sum of the elements of this <c>ComplexDoubleVector</c></summary>
    ///<returns><c>double</c> of absolute sum of the elements.</returns>
    public double GetSumMagnitudes()
    {
      return Blas.Asum.Compute(this.data.Length, this.data, 1);
    }

    ///<summary>Compute the sum y = alpha * x + y where y is this <c>ComplexDoubleVector</c></summary>
    ///<param name="alpha"><c>Complex</c> value to scale this <c>ComplexDoubleVector</c></param>
    ///<param name="X"><c>ComplexDoubleVector</c> to add to alpha * this <c>ComplexDoubleVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Axpy(Complex alpha, ComplexDoubleVector X)
    {
      if (X == null)
      {
        throw new System.ArgumentNullException("ComplexDoubleVector cannot be null.");
      }
      Blas.Axpy.Compute(data.Length, alpha, X.data, 1, this.data, 1);
    }

    ///<summary>Scale this <c>ComplexDoubleVector</c> by a <c>double</c> scalar</summary>
    ///<param name="alpha"><c>double</c> value to scale this <c>ComplexDoubleVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Scale(double alpha)
    {
      Blas.Scal.Compute(data.Length, alpha, data, 1);
    }

    ///<summary>Scale this <c>ComplexDoubleVector</c> by a <c>Complex</c> scalar</summary>
    ///<param name="alpha"><c>Complex</c> value to scale this <c>ComplexDoubleVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Scale(Complex alpha)
    {
      Blas.Scal.Compute(data.Length, alpha, data, 1);
    }

    ///<summary>Negate operator for <c>ComplexDoubleVector</c></summary>
    ///<returns><c>ComplexDoubleVector</c> with values to negate.</returns>
    public static ComplexDoubleVector operator -(ComplexDoubleVector rhs)
    {
      ComplexDoubleVector ret = new ComplexDoubleVector(rhs);
      Blas.Scal.Compute(ret.Length, -1, ret.data, 1);
      return ret;
    }

    ///<summary>Negate operator for <c>ComplexDoubleVector</c></summary>
    ///<returns><c>ComplexDoubleVector</c> with values to negate.</returns>
    public static ComplexDoubleVector Negate(ComplexDoubleVector rhs)
    {
      if (rhs == null)
      {
        throw new ArgumentNullException("rhs", "rhs cannot be null");
      }
      return -rhs;
    }

    ///<summary>Subtract a <c>ComplexDoubleVector</c> from another <c>ComplexDoubleVector</c></summary>
    ///<param name="lhs"><c>ComplexDoubleVector</c> to subtract from.</param>
    ///<param name="rhs"><c>ComplexDoubleVector</c> to subtract.</param>
    ///<returns><c>ComplexDoubleVector</c> with results.</returns>
    public static ComplexDoubleVector operator -(ComplexDoubleVector lhs, ComplexDoubleVector rhs)
    {
      ComplexDoubleVector ret = new ComplexDoubleVector(lhs);
      Blas.Axpy.Compute(ret.Length, -1, rhs.data, 1, ret.data, 1);
      return ret;
    }

    ///<summary>Subtract a <c>ComplexDoubleVector</c> from another <c>ComplexDoubleVector</c></summary>
    ///<param name="lhs"><c>ComplexDoubleVector</c> to subtract from.</param>
    ///<param name="rhs"><c>ComplexDoubleVector</c> to subtract.</param>
    ///<returns><c>ComplexDoubleVector</c> with results.</returns>
    public static ComplexDoubleVector Subtract(ComplexDoubleVector lhs, ComplexDoubleVector rhs)
    {
      return lhs - rhs;
    }

    ///<summary>Add a <c>ComplexDoubleVector</c> to another <c>ComplexDoubleVector</c></summary>
    ///<param name="lhs"><c>ComplexDoubleVector</c> to add to.</param>
    ///<param name="rhs"><c>ComplexDoubleVector</c> to add.</param>
    ///<returns><c>ComplexDoubleVector</c> with results.</returns>
    public static ComplexDoubleVector operator +(ComplexDoubleVector lhs, ComplexDoubleVector rhs)
    {
      ComplexDoubleVector ret = new ComplexDoubleVector(lhs);
      Blas.Axpy.Compute(ret.Length, 1, rhs.data, 1, ret.data, 1);
      return ret;
    }

    ///<summary>Add a <c>ComplexDoubleVector</c> to another <c>ComplexDoubleVector</c></summary>
    ///<param name="rhs"><c>ComplexDoubleVector</c> to add.</param>
    ///<returns><c>ComplexDoubleVector</c> with results.</returns>
    public static ComplexDoubleVector operator +(ComplexDoubleVector rhs)
    {
      return rhs;
    }

    ///<summary>Add a <c>ComplexDoubleVector</c> to this<c>ComplexDoubleVector</c></summary>
    ///<param name="vector"><c>ComplexDoubleVector</c> to add.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null given as argument.</exception>
    public void Add(ComplexDoubleVector vector)
    {
      if (vector == null)
      {
        throw new System.ArgumentNullException("ComplexDoubleVector cannot be null.");
      }
      Blas.Axpy.Compute(this.Length, 1, vector.data, 1, this.data, 1);
    }

    ///<summary>Subtract a <c>ComplexDoubleVector</c> from this<c>ComplexDoubleVector</c></summary>
    ///<param name="vector"><c>ComplexDoubleVector</c> to add.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null given as argument.</exception>
    public void Subtract(ComplexDoubleVector vector)
    {
      if (vector == null)
      {
        throw new System.ArgumentNullException("ComplexDoubleVector cannot be null.");
      }
      Blas.Axpy.Compute(this.Length, -1, vector.data, 1, this.data, 1);
    }

    ///<summary>Scale this <c>ComplexDoubleVector</c> by a <c>Complex</c> scalar</summary>
    ///<param name="value"><c>Complex</c> value to scale this <c>ComplexDoubleVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Multiply(Complex value)
    {
      this.Scale(value);
    }

    ///<summary>Divide this <c>ComplexDoubleVector</c> by a <c>Complex</c> scalar</summary>
    ///<param name="value"><c>Complex</c> value to divide this <c>ComplexDoubleVector</c></param>
    ///<remarks>Results of computation replace data in this variable</remarks>
    public void Divide(Complex value)
    {
      this.Scale(1.0 / value);
    }

    ///<summary>Add a <c>ComplexDoubleVector</c> to another <c>ComplexDoubleVector</c></summary>
    ///<param name="lhs"><c>ComplexDoubleVector</c> to add to.</param>
    ///<param name="rhs"><c>ComplexDoubleVector</c> to add.</param>
    ///<returns><c>ComplexDoubleVector</c> with results.</returns>
    public static ComplexDoubleVector Add(ComplexDoubleVector lhs, ComplexDoubleVector rhs)
    {
      return lhs + rhs;
    }

    ///<summary>Multiply a <c>ComplexDoubleVector</c> with another <c>ComplexDoubleVector</c> as x*y^T</summary>
    ///<param name="lhs"><c>ComplexDoubleVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>ComplexDoubleVector</c> as right hand operand.</param>
    ///<returns><c>ComplexDoubleVector</c> with results.</returns>
    public static ComplexDoubleMatrix operator *(ComplexDoubleVector lhs, ComplexDoubleVector rhs)
    {
      ComplexDoubleMatrix ret = new ComplexDoubleMatrix(lhs.data.Length, rhs.data.Length);
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

    ///<summary>Multiply a <c>ComplexDoubleVector</c> with another <c>ComplexDoubleVector</c> as x*y^T</summary>
    ///<param name="lhs"><c>ComplexDoubleVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>ComplexDoubleVector</c> as right hand operand.</param>
    ///<returns><c>ComplexDoubleVector</c> with results.</returns>
    public static ComplexDoubleMatrix Multiply(ComplexDoubleVector lhs, ComplexDoubleVector rhs)
    {
      return lhs * rhs;
    }

    ///<summary>Multiply a <c>Complex</c> x with a <c>ComplexDoubleVector</c> y as x*y</summary>
    ///<param name="lhs"><c>Complex</c> as left hand operand.</param>
    ///<param name="rhs"><c>ComplexDoubleVector</c> as right hand operand.</param>
    ///<returns><c>ComplexDoubleVector</c> with results.</returns>
    public static ComplexDoubleVector operator *(Complex lhs, ComplexDoubleVector rhs)
    {
      ComplexDoubleVector ret = new ComplexDoubleVector(rhs);
      Blas.Scal.Compute(ret.Length, lhs, ret.data, 1);
      return ret;
    }

    ///<summary>Multiply a <c>Complex</c> x with a <c>ComplexDoubleVector</c> y as x*y</summary>
    ///<param name="lhs"><c>Complex</c> as left hand operand.</param>
    ///<param name="rhs"><c>ComplexDoubleVector</c> as right hand operand.</param>
    ///<returns><c>ComplexDoubleVector</c> with results.</returns>
    public static ComplexDoubleVector Multiply(Complex lhs, ComplexDoubleVector rhs)
    {
      return lhs * rhs;
    }

    ///<summary>Multiply a <c>ComplexDoubleVector</c> x with a <c>Complex</c> y as x*y</summary>
    ///<param name="lhs"><c>ComplexDoubleVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>Complex</c> as right hand operand.</param>
    ///<returns><c>ComplexDoubleVector</c> with results.</returns>
    public static ComplexDoubleVector operator *(ComplexDoubleVector lhs, Complex rhs)
    {
      return rhs * lhs;
    }

    ///<summary>Multiply a <c>ComplexDoubleVector</c> x with a <c>Complex</c> y as x*y</summary>
    ///<param name="lhs"><c>ComplexDoubleVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>Complex</c> as right hand operand.</param>
    ///<returns><c>ComplexDoubleVector</c> with results.</returns>
    public static ComplexDoubleVector Multiply(ComplexDoubleVector lhs, Complex rhs)
    {
      return lhs * rhs;
    }

    ///<summary>Divide a <c>ComplexDoubleVector</c> x with a <c>Complex</c> y as x/y</summary>
    ///<param name="lhs"><c>ComplexDoubleVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>Complex</c> as right hand operand.</param>
    ///<returns><c>ComplexDoubleVector</c> with results.</returns>
    public static ComplexDoubleVector operator /(ComplexDoubleVector lhs, Complex rhs)
    {
      ComplexDoubleVector ret = new ComplexDoubleVector(lhs);
      Blas.Scal.Compute(ret.Length, 1 / rhs, ret.data, 1);
      return ret;
    }

    ///<summary>Divide a <c>ComplexDoubleVector</c> x with a <c>Complex</c> y as x/y</summary>
    ///<param name="lhs"><c>ComplexDoubleVector</c> as left hand operand.</param>
    ///<param name="rhs"><c>Complex</c> as right hand operand.</param>
    ///<returns><c>ComplexDoubleVector</c> with results.</returns>
    public static ComplexDoubleVector Divide(ComplexDoubleVector lhs, Complex rhs)
    {
      return lhs / rhs;
    }

    ///<summary>Clone (deep copy) a <c>ComplexDoubleVector</c> variable</summary>
    public ComplexDoubleVector Clone()
    {
      return new ComplexDoubleVector(this);
    }

    // --- ICloneable Interface ---
    ///<summary>Clone (deep copy) a <c>ComplexDoubleVector</c> variable</summary>
    Object ICloneable.Clone()
    {
      return this.Clone();
    }

    // --- IFormattable Interface ---

    ///<summary>A string representation of this <c>ComplexDoubleVector</c>.</summary>
    ///<returns>The string representation of the value of <c>this</c> instance.</returns>
    public override string ToString()
    {
      return ToString(null, null);
    }

    ///<summary>A string representation of this <c>ComplexDoubleVector</c>.</summary>
    ///<param name="format">A format specification.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by format.</returns>
    public string ToString(string format)
    {
      return ToString(format, null);
    }

    ///<summary>A string representation of this <c>ComplexDoubleVector</c>.</summary>
    ///<param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
    ///<returns>The string representation of the value of <c>this</c> instance as specified by provider.</returns>
    public string ToString(IFormatProvider formatProvider)
    {
      return ToString(null, formatProvider);
    }

    ///<summary>A string representation of this <c>ComplexDoubleVector</c>.</summary>
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
      return new ComplexDoubleVectorEnumerator(this);
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
    public void CopyTo(Complex[] array, int index)
    {
      this.data.CopyTo(array, index);
    }
    void ICollection.CopyTo(Array array, int index)
    {
      this.CopyTo((Complex[])array, index);
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

    ///<summary>Access a <c>ComplexDoubleVector</c> element</summary>
    ///<param name="index">The element to access</param>
    ///<exception cref="IndexOutOfRangeException">Exception thrown in element accessed is out of the bounds of the vector.</exception>
    ///<returns>Returns a <c>Complex</c> vector element</returns>
    public Complex this[int index]
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
    object IList.this[int index]
    {
      get { return (object)this[index]; }
      set { this[index] = (Complex)value; }
    }

    ///<summary>Add a new value to the end of the <c>ComplexDoubleVector</c></summary>
    public int Add(object value)
    {
      Complex[] newdata = new Complex[data.Length + 1];
      int newpos = newdata.Length - 1;

      System.Array.Copy(data, newdata, data.Length);
      newdata[newpos] = (Complex)value;
      data = newdata;
      return newpos;
    }

    ///<summary>Set all values in the <c>ComplexDoubleVector</c> to zero </summary>
    public void Clear()
    {
      for (int i = 0; i < data.Length; i++)
        data[i] = 0;
    }

    ///<summary>Check if the any of the <c>ComplexDoubleVector</c> components equals a given <c>Complex</c></summary>
    public bool Contains(object value)
    {
      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] == (Complex)value)
          return true;
      }
      return false;
    }

    ///<summary>Return the index of the <c>ComplexDoubleVector</c> for the first component that equals a given <c>Complex</c></summary>
    public int IndexOf(object value)
    {
      for (int i = 0; i < data.Length; i++)
      {
        if (data[i] == (Complex)value)
          return i;
      }
      return -1;
    }

    ///<summary>Insert a <c>Complex</c> into the <c>ComplexDoubleVector</c> at a given index</summary>
    public void Insert(int index, object value)
    {
      if (index > data.Length)
      {
        throw new System.ArgumentOutOfRangeException("index");
      }

      Complex[] newdata = new Complex[data.Length + 1];
      System.Array.Copy(data, newdata, index);
      newdata[index] = (Complex)value;
      System.Array.Copy(data, index, newdata, index + 1, data.Length - index);
      data = newdata;
    }

    ///<summary>Remove the first instance of a given <c>Complex</c> from the <c>ComplexDoubleVector</c></summary>
    public void Remove(object value)
    {
      int index = this.IndexOf(value);

      if (index == -1)
        return;
      this.RemoveAt(index);
    }

    ///<summary>Remove the component of the <c>ComplexDoubleVector</c> at a given index</summary>
    public void RemoveAt(int index)
    {
      if (index >= data.Length)
        throw new System.ArgumentOutOfRangeException("index");

      Complex[] newdata = new Complex[data.Length - 1];
      System.Array.Copy(data, newdata, index);
      if (index < data.Length)
        System.Array.Copy(data, index + 1, newdata, index, newdata.Length - index);
      data = newdata;
    }


    #region IROComplexDoubleVector Members

    public int LowerBound
    {
      get { return 0; }
    }

    public int UpperBound
    {
      get { return data.Length-1; }
    }

    #endregion

    #region Additions due to adoption

    /// <summary>
    /// Returns the column of a <see cref="IROComplexDoubleMatrix" /> as a new <c>ComplexDoubleVector.</c>
    /// </summary>
    /// <param name="mat">The matrix to copy the column from.</param>
    /// <param name="col">Number of column to copy from the matrix.</param>
    /// <returns>A new <c>ComplexDoubleVector</c> with the same elements as the column of the given matrix.</returns>
    public static ComplexDoubleVector GetColumn(IROComplexDoubleMatrix mat, int col)
    {
      ComplexDoubleVector result = new ComplexDoubleVector(mat.Rows);
      for(int i=0;i<result.data.Length;++i)
        result.data[i] = mat[i,col];
      
      return result;
    }

    /// <summary>
    /// Returns the column of a <see cref="IROComplexDoubleMatrix" /> as a new <c>Complex[]</c> array.
    /// </summary>
    /// <param name="mat">The matrix to copy the column from.</param>
    /// <param name="col">Number of column to copy from the matrix.</param>
    /// <returns>A new array of <c>Complex</c> with the same elements as the column of the given matrix.</returns>
    public static Complex[] GetColumnAsArray(IROComplexDoubleMatrix mat, int col)
    {
      Complex[] result = new Complex[mat.Rows];
      for(int i=0;i<result.Length;++i)
        result[i] = mat[i,col];

      return result;
    }

    /// <summary>
    /// Returns the row of a <see cref="IROComplexDoubleMatrix" /> as a new <c>ComplexDoubleVector.</c>
    /// </summary>
    /// <param name="mat">The matrix to copy the column from.</param>
    /// <param name="row">Number of row to copy from the matrix.</param>
    /// <returns>A new <c>ComplexDoubleVector</c> with the same elements as the row of the given matrix.</returns>
    public static ComplexDoubleVector GetRow(IROComplexDoubleMatrix mat, int row)
    {
      ComplexDoubleVector result = new ComplexDoubleVector(mat.Columns);
      for(int i=0;i<result.data.Length;++i)
        result.data[i] = mat[row,i];
      
      return result;
    }

    #endregion
  }
}
