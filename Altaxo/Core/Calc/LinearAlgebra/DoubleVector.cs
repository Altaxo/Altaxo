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
  public class DoubleVector : IVector
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
    /// Creates a vector with lower boundary and upper boundary. The length will be 1+upperBound-lowerBound.
    /// </summary>
    /// <param name="lowerBound">Lower boundary (first valid index).</param>
    /// <param name="upperBound">Upper boundary (last valid index).</param>
    public DoubleVector(int lowerBound, int upperBound)
    {
      this.Resize(lowerBound,upperBound);
    }

    /// <summary>
    /// Element access.
    /// </summary>
    public double this[int i]
    {
      get { return x[i-lo]; }
      set { x[i-lo] = value; }
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

    /// <summary>
    /// Direct access to the underlying data array. Use this with care!
    /// </summary>
    /// <returns>The underlying data array.</returns>
    public double[] Store()
    {
      return x;
    }
  }

  
}
