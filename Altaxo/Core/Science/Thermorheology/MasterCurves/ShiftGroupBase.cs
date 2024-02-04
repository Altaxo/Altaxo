#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
  /// A collection of multiple x-y curves (see <see cref="ShiftCurve{T}"/>) that will finally form one master curve.
  /// </summary>
  public class ShiftGroupBase<T> : IReadOnlyList<ShiftCurve<T>>
  {
    protected ShiftCurve<T>[] _inner;

    /// <summary>
    /// Determines how to shift the x values: either by factor or by offset. Use offset if the original data are already logarithmized.
    /// </summary>
    public ShiftXBy XShiftBy { get; }

    /// <summary>Logarithmize x values before adding to the interpolation curve. (Only for interpolation).</summary>
    public bool LogarithmizeXForInterpolation { get; }

    /// <summary>Logarithmize y values before adding to the interpolation curve. (Only for interpolation).</summary>
    public bool LogarithmizeYForInterpolation { get; }

    /// <summary>
    /// Gets the fitting weight, a number number &gt; 0.
    /// </summary>
    public double FittingWeight { get; }

    /// <summary>
    /// <summary>
    /// Initializes a new instance of the <see cref="ShiftGroup"/> class.
    /// </summary>
    /// <param name="data">Collection of multiple x-y curves that will finally form one master curve.</param>
    /// <param name="xShiftBy">Shift method, either additive or multiplicative.</param>
    /// <param name="fitWeight">The weight with which to participate in the fit. Has to be &gt; 0.</param>
    /// <param name="logarithmizeXForInterpolation">If true, the x-values are logarithmized prior to participating in the interpolation function.</param>
    /// <param name="logarithmizeYForInterpolation">If true, the y-values are logartihmized prior to participating in the interpolation function.</param>
    public ShiftGroupBase(IEnumerable<ShiftCurve<T>> data, ShiftXBy xShiftBy, double fitWeight, bool logarithmizeXForInterpolation, bool logarithmizeYForInterpolation)
    {
      _inner = data.ToArray();
      XShiftBy = xShiftBy;
      FittingWeight = fitWeight;
      LogarithmizeXForInterpolation = logarithmizeXForInterpolation;
      LogarithmizeYForInterpolation = logarithmizeYForInterpolation;
    }

    /// <inheritdoc/>
    public ShiftCurve<T> this[int index] => ((IReadOnlyList<ShiftCurve<T>>)_inner)[index];

    /// <inheritdoc/>
    public int Count => ((IReadOnlyCollection<ShiftCurve<T>>)_inner).Count;

    /// <inheritdoc/>
    public IEnumerator<ShiftCurve<T>> GetEnumerator()
    {
      return ((IEnumerable<ShiftCurve<T>>)_inner).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _inner.GetEnumerator();
    }
  }
}

