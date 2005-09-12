#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// A vector class which provides direct access to the underlaying data array.
  /// </summary>
  public class DoubleVector : IVector, ICloneable
  {
    protected double[] x;
    protected int lo = 0;
    protected int hi = -1;

    /// <summary>
    /// Creates an empty vector.
    /// </summary>
    public DoubleVector()
    {
    }


    /// <summary>
    /// Creates a vector with a given length. The lower boundary is set to 0.
    /// </summary>
    /// <param name="length">Length of the vector.</param>
    public DoubleVector(int length)
    {
      this.Resize(0, length-1);
    }

    /// <summary>
    /// Creates a vector with lower boundary and upper boundary. The length will be 1+upperBound-lowerBound.
    /// </summary>
    /// <param name="lowerBound">Lower boundary (first valid index).</param>
    /// <param name="upperBound">Upper boundary (last valid index).</param>
    public DoubleVector(int lowerBound, int upperBound)
    {
      this.Resize(lowerBound,upperBound);
    }

    public DoubleVector(DoubleVector from)
    {
      this.lo = from.lo;
      this.hi = from.hi;
      this.x = (double[])from.x.Clone();
    }

    
    /// <summary>
    /// Element access.
    /// </summary>
    public double this[int i]
    {
      get { return x[i-lo]; }
      set { x[i-lo] = value; }
    }

    public double[] data
    {
      get
      {
        return x;
      }
    }
   
    /// <summary>
    /// Lower boundary (first valid index).
    /// </summary>
    public int LowerBound 
    {
      get
      {
        return lo; 
      }
    }
   
    /// <summary>
    /// Upper boundary (last valid index).
    /// </summary>
    public int UpperBound 
    {
      get
      {
        return hi;
      } 
    }

    /// <summary>
    /// Empties the vector and clears the underlying array.
    /// </summary>
    public void Clear()
    {
      x=null;
      lo=0;
      hi=-1;
    }

    /// <summary>
    /// Resizes the vector. The previous element data are lost.
    /// </summary>
    /// <param name="lo">New lower boundary of the vector (first valid index).</param>
    /// <param name="hi">New upper boundary of the vector (last valid index).</param>
    public void Resize(int lo, int hi)
    {
      if(x==null || (hi-lo)>=x.Length)
      {
        x = new double[hi-lo+1];
      }
      this.lo = lo;
      this.hi = hi;
    }

    /// <summary>
    /// Number of vector elements.
    /// </summary>
    public int Length // change this later to length property
    {
      get { return hi-lo+1; }
    }

    /// <summary>
    /// Resizes the vector to the same boundaries as the provided vector and copies the elements from it.
    /// </summary>
    /// <param name="a">The vector to copy the data from.</param>
    public void CopyFrom(IROVector a)
    {
      Resize(a.LowerBound,a.UpperBound);
      for(int i=lo;i<=hi;i++)
        x[i-lo] = a[i];
    }

    ///<summary>Return <c>double</c> array of data from <c>DoubleVector</c></summary>
    ///<returns><c>double</c> array with data.</returns>
    public double[] ToArray()
    {
      double[] ret = new double[x.Length];
      Array.Copy(x, ret, x.Length);
      return ret;
    }

    /// <summary>
    /// Direct access to the underlying data array. Use this with care!
    /// </summary>
    /// <returns>The underlying data array.</returns>
    public double[] Store()
    {
      return x;
    }

    ///<summary>Compute the Euclidean Norm ||x||_2 of this <c>DoubleVector</c></summary>
    ///<returns><c>double</c> results from norm.</returns>
    public double GetNorm()
    {
      return VectorMath.GetNorm(this.x);
    }

    #region from DnA

    ///<summary>Constructor for <c>DoubleVector</c> from <c>double</c> array</summary>
    ///<param name="values">Array of <c>double</c> to convert into <c>DoubleVector</c>.</param>
    ///<exception cref="ArgumentNullException">Exception thrown if null passed as 'value' parameter.</exception>
    public DoubleVector(double[] values)
    {
      if (values == null)
      {
        throw new ArgumentNullException("Array cannot be null");
      }
      x = (double[])values.Clone();
      this.hi = x.Length - 1;
    }

    ///<summary>Implicit cast conversion to <c>DoubleVector</c> from <c>double</c> array.
    ///The constructed <c>DoubleVector</c> is using the <c>src</c> array directly!</summary>
    static public implicit operator DoubleVector(double[] src)
    {
      DoubleVector ret = new DoubleVector();
      ret.x = src;
      ret.hi = src.Length - 1;
      return ret;
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
			dnA.Math.Blas.Scal.Compute(ret.Length,lhs, ret.data,1);
#endif
      return ret;
    }
    #endregion

    #region ICloneable Members

    object ICloneable.Clone()
    {
      return new DoubleVector(this);
    }
    public DoubleVector Clone()
    {
      return new DoubleVector(this);
    }

    #endregion
}

  
}
