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
  /// Represents the boundaries of an axis.
  /// </summary>
  [Serializable]
  public abstract class AbstractPhysicalBoundaries : Main.SuspendableDocumentLeafNodeWithSingleAccumulatedData<BoundariesChangedEventArgs>, IPhysicalBoundaries
  {
    protected int _numberOfItems = 0;

    [NonSerialized]
    protected int _savedNumberOfItems; // stores the number of items when events are disabled

    public AbstractPhysicalBoundaries()
    {
      _numberOfItems = 0;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The boundary object to copy from.</param>
    public AbstractPhysicalBoundaries(AbstractPhysicalBoundaries from)
    {
      _numberOfItems = from._numberOfItems;
    }

    #region IPhysicalBoundaries Members

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
    public abstract bool Add(Altaxo.Data.IReadableColumn col, int idx);

    /// <summary>
    /// Processes a single value.
    /// If the data value is inside the considered value range, the boundaries are
    /// updated and the number of items is increased by one. The function has to return true
    /// in this case. On the other hand, if the value is outside the range, the function has to
    /// return false.
    /// </summary>
    /// <param name="item">The data value.</param>
    /// <returns>True if data is in the tracked range, false if the data is not in the tracked range.</returns>
    public abstract bool Add(Altaxo.Data.AltaxoVariant item);

    public virtual void Reset()
    {
      _numberOfItems = 0;
    }

    public int NumberOfItems
    {
      get
      {
        return _numberOfItems;
      }
    }

    public virtual bool IsEmpty
    {
      get
      {
        return _numberOfItems == 0;
      }
    }

    /// <summary>
    /// Merges another boundary object into this one here.
    /// </summary>
    /// <param name="b">The other boundary object.</param>
    public abstract void Add(IPhysicalBoundaries b);

    #endregion IPhysicalBoundaries Members

    #region ICloneable Members

    public abstract object Clone();

    #endregion ICloneable Members

    #region Changed event handling

    protected override void AccumulateChangeData(object? sender, EventArgs e)
    {
      var eAsBCEA = e as BoundariesChangedEventArgs;
      if (null == eAsBCEA)
        throw new ArgumentOutOfRangeException(string.Format("Argument e should be of type {0}, but is {1}", typeof(BoundariesChangedEventArgs), e.GetType()));

      if (null == _accumulatedEventData)
        _accumulatedEventData = eAsBCEA;
      else
        _accumulatedEventData.Add(eAsBCEA);
    }

    #endregion Changed event handling
  }
}
