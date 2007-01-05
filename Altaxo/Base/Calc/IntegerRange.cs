#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc
{
  public class IntegerRange : ICloneable
  {
    protected int _first;
    protected int _count;

    public IntegerRange()
    {
    }

    protected IntegerRange(int first, int count)
    {
      _first = first;
      _count = count;
      EnsureValidity();
    }

    public void CopyFrom(IntegerRange from)
    {
      _first = from._first;
      _count = from._count;
      EnsureValidity();
    }

    protected virtual void EnsureValidity()
    {
      if (_count < 0)
        throw new ArgumentOutOfRangeException("count", "Argument 'count' has to be positive");
    }

    public static IntegerRange NewFromFirstAndCount(int first, int count)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException("count", "Count must be a positive integer");
     
      return new IntegerRange(first, count);
    }

    public static IntegerRange NewFromFirstToInfinity(int first)
    {
      return new IntegerRange(first, int.MaxValue); 
    }

    static public IntegerRange NewFromFirstAndEnd(int first, int end)
    {
      return new IntegerRange(first,end-first);
    }

    static public IntegerRange NewFromFirstAndLast(int first, int last)
    {
      return new IntegerRange(first, 1 + (last - first));
    }

    public int First
    {
      get { return _first; }
    }
    public int Count
    {
      get { return _count; }
    }
    public int Last
    {
      get
      {
        if ((int.MaxValue - _count) < (_first-1))
          return int.MaxValue;
        else
          return _first + _count - 1; 
      }
    }
    public int End
    {
      get
      {
        if ((int.MaxValue - _count) < (_first ))
          return int.MaxValue;
        else
          return _first + _count; 
      }
    }

    public bool IsInfinite
    {
      get
      {
        return _count == int.MaxValue;
      }
    }

    #region ICloneable Members

    public virtual object Clone()
    {
      return new IntegerRange(_first, _count);
    }

    #endregion
  }
}
