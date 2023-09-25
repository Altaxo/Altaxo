#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  using System.Collections;


  /// <summary>
  /// The data to construct multiple shift curves with a common set of shift factors consist of multiple <see cref="ShiftCurveCollection"/>s.
  /// </summary>
  public class ShiftCurveCollections : IReadOnlyList<ShiftCurveCollection>
  {
    ShiftCurveCollection[] _inner;

    public ShiftCurveCollections(IEnumerable<ShiftCurveCollection> data)
    {
      _inner = data.ToArray();
    }

    public ShiftCurveCollection this[int index] => ((IReadOnlyList<ShiftCurveCollection>)_inner)[index];

    public int Count => ((IReadOnlyCollection<ShiftCurveCollection>)_inner).Count;

    public IEnumerator<ShiftCurveCollection> GetEnumerator()
    {
      return ((IEnumerable<ShiftCurveCollection>)_inner).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _inner.GetEnumerator();
    }
  }
}

