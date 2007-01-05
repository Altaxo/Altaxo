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
using Altaxo.Serialization;

namespace Altaxo.Graph.Scales.Boundaries
{
  /// <summary>
  /// Provides an interface for tracking the physical
  /// boundaries of a plot association. Every plot association has two of these objects
  /// that help tracking the boundaries of X and Y axis
  /// </summary>
  public interface IPhysicalBoundaries : ICloneable
  {

    event BoundaryChangedHandler   BoundaryChanged;
    event ItemNumberChangedHandler NumberOfItemsChanged;

    /// <summary>
    /// Returns true if events are enabled
    /// </summary>
    bool EventsEnabled
    {
      get;
    }

    /// <summary>
    /// Starts an update process. This will suspend change events.
    /// </summary>
    void BeginUpdate();

    /// <summary>
    /// Ends an update process. This will re-enable change events if the suspend count reached zero.
    /// </summary>
    void EndUpdate();

  
    /// <summary>
    /// Processes a single value from a numeric column <paramref name="col"/>[<paramref name="idx"/>].
    /// If the data value is inside the considered value range, the boundaries are
    /// updated and the number of items is increased by one. The function has to return true
    /// in this case. On the other hand, if the value is outside the range, the function has to
    /// return false.
    /// </summary>
    /// <param name="col">The numeric data column</param>
    /// <param name="idx">The index into this numeric column where the data value is located</param>
    /// <returns>True if data is in the tracked range, false if the data is not in the tracked range.</returns>
    bool Add(Altaxo.Data.IReadableColumn col, int idx);

    /// <summary>
    /// Processes a single value.
    /// If the data value is inside the considered value range, the boundaries are
    /// updated and the number of items is increased by one. The function has to return true
    /// in this case. On the other hand, if the value is outside the range, the function has to
    /// return false.
    /// </summary>
    /// <param name="item">The data value.</param>
    /// <returns>True if data is in the tracked range, false if the data is not in the tracked range.</returns>
    bool Add(Altaxo.Data.AltaxoVariant item);

    /// <summary>
    /// Reset the internal data to the initialized state
    /// </summary>
    void Reset();

    /// <summary>
    /// Return the number of items that was used to set the bounds.
    /// </summary>
    int NumberOfItems  { get; }
    /// <summary>
    /// Returns true when there are no data that can set the bounds.
    /// </summary>
    bool IsEmpty { get; }
    
    // double LowerBound { get { return minValue; } }
    // double UpperBound { get { return maxValue; } }

    /// <summary>
    /// merged boundaries of another object into this object
    /// </summary>
    /// <param name="b">another physical boundary object of the same type as this</param>
    void Add(IPhysicalBoundaries b);
  }
}
