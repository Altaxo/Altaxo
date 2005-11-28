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


namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Vector of integer elements.
  /// </summary>
  public class IntegerVector
  {
    protected int[] x;
    protected int lo = 0;
    protected int hi = -1;


    /// <summary>
    /// Element accessor.
    /// </summary>
    public int this[int i]
    {
      get { return x[i]; }
      set { x[i] = value; }
    }

    /// <summary>
    /// Sets all elements to the provided value.
    /// </summary>
    /// <param name="val">The value all elements are set to.</param>
    public void SetAllElementsTo(int val)
    {
      for(int i=lo; i<=hi; i++)
        x[i] = val;
    }

    /// <summary>
    /// Clears all elements and deletes the underlying array.
    /// </summary>
    public void Clear()
    {
      x=null;
      lo=0;
      hi=-1;
    }

    /// <summary>
    /// Resizes the vector. Previosly stored data are lost.
    /// </summary>
    /// <param name="lo">New lower bound (first valid index).</param>
    /// <param name="hi">New upper bound (last valid index).</param>
    public void Resize(int lo, int hi)
    {
      if(x==null || hi>=x.Length)
      {
        x = new int[hi+1];
      }
      this.lo = lo;
      this.hi = hi;
    }
  }


}
