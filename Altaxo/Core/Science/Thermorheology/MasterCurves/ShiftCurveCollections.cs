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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// A set of <see cref="ShiftCurveCollection"/>s. The curves in these collections will be shifted with a common set of shift factors.
  /// For example, for complex data to be shifted, there will be two <see cref="ShiftCurveCollection"/>s, one for the real part, and one for the imaginary.
  /// The curves of the real part will finally form the master curve of the real part, and the curves of the imaginary part will finally
  /// form the master curve of the imaginary part.
  /// </summary>
  public class ShiftCurveCollections : IReadOnlyList<ShiftCurveCollection>
  {
    ShiftCurveCollection[] _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShiftCurveCollections"/> class.
    /// </summary>
    /// <param name="data">The set of <see cref="ShiftCurveCollection"/>s.</param>
    public ShiftCurveCollections(IEnumerable<ShiftCurveCollection> data)
    {
      _inner = data.ToArray();
    }

    /// <inheritdoc/>
    public ShiftCurveCollection this[int index] => ((IReadOnlyList<ShiftCurveCollection>)_inner)[index];

    /// <inheritdoc/>
    public int Count => ((IReadOnlyCollection<ShiftCurveCollection>)_inner).Count;

    /// <inheritdoc/>
    public IEnumerator<ShiftCurveCollection> GetEnumerator()
    {
      return ((IEnumerable<ShiftCurveCollection>)_inner).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _inner.GetEnumerator();
    }
  }
}

