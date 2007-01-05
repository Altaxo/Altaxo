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
  public class PositiveIntegerRange : IntegerRange
  {
    protected PositiveIntegerRange()
    {
    }
    protected PositiveIntegerRange(int first, int count)
      : base(first,count)
    {
    }

    protected override void EnsureValidity()
    {
      if (_count < 0)
        throw new ArgumentOutOfRangeException("count", "Member '_count' has to be positive");
      if (_first < 0)
        throw new ArgumentOutOfRangeException("first", "Member '_first' has to be positive");
    }

    public override object Clone()
    {
      return new PositiveIntegerRange(_first, _count);
    }

    public static new PositiveIntegerRange NewFromFirstAndCount(int first, int count)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException("count", "Argument 'count' must be a positive integer");
      if (first < 0)
        throw new ArgumentOutOfRangeException("first", "Argument 'first' must be a positive integer");

      return new PositiveIntegerRange(first, count);
    }

    public static new PositiveIntegerRange NewFromFirstToInfinity(int first)
    {
      if (first < 0)
        throw new ArgumentOutOfRangeException("first", "Argument 'first' must be a positive integer");

      return new PositiveIntegerRange(first, int.MaxValue);
    }

    static public new PositiveIntegerRange NewFromFirstAndEnd(int first, int end)
    {
      if (first < 0)
        throw new ArgumentOutOfRangeException("first", "Argument 'first' must be a positive integer");
      if(end<first)
        throw new ArgumentOutOfRangeException("end", "Argument 'end' has to be greater or equal to argument 'first'.");

      return new PositiveIntegerRange(first, end - first);
    }

    static public new PositiveIntegerRange NewFromFirstAndLast(int first, int last)
    {
      if (first < 0)
        throw new ArgumentOutOfRangeException("first", "Argument 'first' must be a positive integer");
      if ((last+1) < first)
        throw new ArgumentOutOfRangeException("end", "Argument 'last' has to be greater or equal than argument 'first'.");

      return new PositiveIntegerRange(first, 1 + (last - first));
    }
  }
}
