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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// Interface to a group style that needs to shift the items, e.g. the <see cref="BarSizePosition3DGroupStyle"/>. The shift is independet on the row index of the underlying data.
  /// </summary>
  public interface IShiftLogicalXYGroupStyle
  {
    /// <summary>
    /// Gets a value indicating whether the shift is constant, i.e. independent on the row index, or not.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is constant; otherwise, <c>false</c>.
    /// </value>
    bool IsConstant { get; }

    /// <summary>
    /// Get the logical shift values applied to the items. Use this function if <see cref="IsConstant"/> returns true.
    /// </summary>
    /// <param name="logicalShiftX">The logical shift x applied to the items.</param>
    /// <param name="logicalShiftY">The logical shift y applied to the items.</param>
    void Apply(out double logicalShiftX, out double logicalShiftY);

    /// <summary>
    /// Get the logical shift values applied to the items. Use this function if <see cref="IsConstant"/> returns false.
    /// </summary>
    /// <param name="logicalShiftX">The function to get the logical shift x applied to the items. Parameter is the original row index of the item.</param>
    /// <param name="logicalShiftY">The function to get the logical shift y applied to the items. Parameter is the original row index of the item.</param>
    void Apply(out Func<int, double> logicalShiftX, out Func<int, double> logicalShiftY);
  }
}
