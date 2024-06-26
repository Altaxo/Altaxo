﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Vector of integer elements.
  /// </summary>
  public class IntegerVector
  {
    protected static readonly int[] _emptyIntArray = new int[0];
    protected int[] x = _emptyIntArray;
    protected int len = 0;

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
      for (int i = len - 1; i >= 0; --i)
        x[i] = val;
    }

    /// <summary>
    /// Clears all elements and deletes the underlying array.
    /// </summary>
    public void Clear()
    {
      x = _emptyIntArray;
      len = 0;
    }

    /// <summary>
    /// Resizes the vector. Previosly stored data are lost.
    /// </summary>
    /// <param name="length">New length.</param>
    [MemberNotNull(nameof(x))]
    public void Resize(int length)
    {
      if (x is null || length >= x.Length)
      {
        x = new int[length];
      }
      len = length;
    }
  }
}
