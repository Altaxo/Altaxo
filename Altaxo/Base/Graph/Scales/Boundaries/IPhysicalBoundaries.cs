#region Copyright

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

#nullable enable
using System;

namespace Altaxo.Graph.Scales.Boundaries
{
  /// <summary>
  /// Provides an interface for tracking the physical boundaries of plot data.
  /// </summary>
  public interface IPhysicalBoundaries : Main.IDocumentLeafNode, ICloneable
  {
    /// <summary>
    /// Processes a single value from a numeric column <paramref name="col"/>[<paramref name="idx"/>].
    /// If the data value is inside the considered definition range of the values, the boundaries are
    /// updated and the number of items is increased by one. The function has to return true
    /// in this case. On the other hand, if the value is outside the range, the function has to
    /// return false.
    /// </summary>
    /// <param name="col">The numeric data column</param>
    /// <param name="idx">The index into this numeric column where the data value is located</param>
    /// <returns>True if data has contributed to the determination of the boundaries; otherwise, false.
    /// Please note that a return value of false does <b>not</b> necessarily mean that the data can not be plotted.
    /// For instance, a value of PositiveInfinity can be plotted on a linear axis as a peak, but it can not contribute to the determination of the boundaries.</returns>
    bool Add(Altaxo.Data.IReadableColumn col, int idx);

    /// <summary>
    /// Processes a single value.
    /// If the data value is inside the considered value range, the boundaries are
    /// updated and the number of items is increased by one. The function has to return true
    /// in this case. On the other hand, if the value is outside the range, the function has to
    /// return false.
    /// </summary>
    /// <param name="item">The data value.</param>
    /// <returns>True if data has contributed to the determination of the boundaries; otherwise, false.
    /// Please note that a return value of false does <b>not</b> necessarily mean that the data can not be plotted.
    /// For instance, a value of PositiveInfinity can be plotted on a linear axis as a peak, but it can not contribute to the determination of the boundaries.</returns>
    bool Add(Altaxo.Data.AltaxoVariant item);

    /// <summary>
    /// Resets the internal data to the initialized state.
    /// </summary>
    void Reset();

    /// <summary>
    /// Gets the number of items that were used to set the bounds.
    /// </summary>
    int NumberOfItems { get; }

    /// <summary>
    /// Gets a value indicating whether there are no data that can set the bounds.
    /// </summary>
    bool IsEmpty { get; }

    // double LowerBound { get { return minValue; } }
    // double UpperBound { get { return maxValue; } }

    /// <summary>
    /// Merges the boundaries of another object into this object.
    /// </summary>
    /// <param name="b">Another physical boundary object of the same type as this instance.</param>
    void Add(IPhysicalBoundaries b);
  }
}
