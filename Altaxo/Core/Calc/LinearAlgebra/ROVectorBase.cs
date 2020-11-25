#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
  public abstract class ROVectorBase<T> : IROVector<T>
  {
    public abstract T this[int index] { get; set; }

    public abstract int Count { get; }

    public int Length { get { return Count; } }

    public int IndexOf(T item)
    {
      if (!(item is null))
      {
        var cnt = Count;
        for (int i = 0; i < Count; ++i)
          if (item.Equals(this[i]))
            return i;
      }
      return -1;
    }

    public bool Contains(T item)
    {
      if (!(item is null))
      {
        var cnt = Count;
        for (int i = 0; i < Count; ++i)
          if (item.Equals(this[i]))
            return true;
      }
      return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      if (array is null)
        throw new ArgumentNullException(nameof(array));
      if (arrayIndex < 0)
        throw new ArgumentOutOfRangeException(nameof(arrayIndex), "arrayIndex is < 0");
      var cnt = Count;
      if (!(arrayIndex + cnt <= array.Length))
        throw new ArgumentOutOfRangeException("Array too small for the provided data.");

      for (int i = 0; i < cnt; ++i)
        array[i + arrayIndex] = this[i];
    }

    public IEnumerator<T> GetEnumerator()
    {
      var cnt = Count;
      for (int i = 0; i < cnt; ++i)
        yield return this[i];
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      var cnt = Count;
      for (int i = 0; i < cnt; ++i)
        yield return this[i];
    }
  }
}
