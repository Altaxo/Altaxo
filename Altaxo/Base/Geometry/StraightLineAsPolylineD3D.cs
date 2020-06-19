#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Geometry
{
  public class StraightLineAsPolylineD3D : IPolylineD3D, IList<PointD3D>
  {
    private PointD3D _p0, _p1;

    public StraightLineAsPolylineD3D(PointD3D p0, PointD3D p1)
    {
      _p0 = p0;
      _p1 = p1;
    }

    public int Count
    {
      get
      {
        return 2;
      }
    }

    public PointD3D GetPoint(int idx)
    {
      switch (idx)
      {
        case 0:
          return _p0;

        case 1:
          return _p1;

        default:
          throw new ArgumentOutOfRangeException(nameof(idx));
      }
    }

    public IList<PointD3D> Points
    {
      get
      {
        return this;
      }
    }

    public bool IsTransitionFromIdxToNextIdxSharp(int idx)
    {
      return true;
    }

    public double TotalLineLength
    {
      get
      {
        return (_p0 - _p1).Length;
      }
    }

    public IPolylineD3D ShortenedBy(RADouble marginAtStart, RADouble marginAtEnd)
    {
      double totLength = TotalLineLength;

      double r1 = marginAtStart.IsAbsolute ? marginAtStart.Value / totLength : marginAtStart.Value;
      double r2 = marginAtEnd.IsAbsolute ? marginAtEnd.Value / totLength : marginAtEnd.Value;

      if (!((r1 + r2) < 1))
        return null;

      return new StraightLineAsPolylineD3D(PointD3D.Interpolate(_p0, _p1, r1), PointD3D.Interpolate(_p1, _p0, r2));
    }

    #region IList<PointD3D>

    public bool IsReadOnly
    {
      get
      {
        return true;
      }
    }

    public PointD3D this[int index]
    {
      get
      {
        switch (index)
        {
          case 0:
            return _p0;

          case 1:
            return _p1;

          default:
            throw new IndexOutOfRangeException();
        }
      }

      set
      {
        throw new InvalidOperationException("This list is readonly!");
      }
    }

    public int IndexOf(PointD3D item)
    {
      throw new NotImplementedException();
    }

    public void Insert(int index, PointD3D item)
    {
      throw new InvalidOperationException("This list is readonly!");
    }

    public void RemoveAt(int index)
    {
      throw new InvalidOperationException("This list is readonly!");
    }

    public void Add(PointD3D item)
    {
      throw new InvalidOperationException("This list is readonly!");
    }

    public void Clear()
    {
      throw new InvalidOperationException("This list is readonly!");
    }

    public bool Contains(PointD3D item)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
    public void CopyTo(PointD3D[] array, int arrayIndex)
    {
      array[0 + arrayIndex] = _p0;
      array[1 + arrayIndex] = _p1;
    }

    public bool Remove(PointD3D item)
    {
      throw new InvalidOperationException("This list is readonly!");
    }

    public IEnumerator<PointD3D> GetEnumerator()
    {
      yield return _p0;
      yield return _p1;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      yield return _p0;
      yield return _p1;
    }

    #endregion IList<PointD3D>
  }
}
