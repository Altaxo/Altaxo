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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections.Operations
{
  /// <summary>
  /// Given a sequence of numeric values that will be added to this instance, the algorithm keeps track of the minimum value of the last <c>numberOfItems</c> added values.
  /// The name of the algorithm is ascending minima algorithm, one of the algorithms in the class of "minimum on a sliding window algorithms".
  /// </summary>
  public class MinimumOnSlidingWindow<T>
  {
    #region Item

    private struct Bucket
    {
      /// <summary>Value of this bucket.</summary>
      public T Value;

      /// <summary>Number of generation, when this bucket can be considered as expired, and must be removed from the collection.</summary>
      public int ExpireGeneration;
    }

    #endregion Item

    /// <summary>Counter that is incremented each time an element is added</summary>
    private int _generation;

    /// <summary>Array of Bucket structs storing the value and the generation when this value will become invalid.</summary>
    private Bucket[] _items;

    /// <summary>Index of the bucket with the lowest value in the bucket array.</summary>
    private int _minItemIdx;

    /// <summary>Index of the bucket that was the last added value in the array.</summary>
    private int _lastItemIdx;

    private Func<T, T, int> _comparer;

    /// <summary>Initializes a new instance of the <see cref="MinimumOnSlidingWindow&lt;T&gt;"/> class.</summary>
    /// <param name="numberOfItems">The number of items N. The algorithm evaluates the minimum of the last N items that where added to this instance.</param>
    /// <param name="startValue">The start value. This is the first entry to add to the instance. Thus, the <see cref="MinimumValue"/> always return a valid value.</param>
    public MinimumOnSlidingWindow(int numberOfItems, T startValue)
    {
      _comparer = Comparer<T>.Default.Compare;
      _items = new Bucket[numberOfItems];
      _items[_minItemIdx].ExpireGeneration = _generation + _items.Length;
      _items[_minItemIdx].Value = startValue;
      ++_generation;
    }

    /// <summary>Gets the current minimum value of the window.</summary>
    public T MinimumValue
    {
      get
      {
        return _items[_minItemIdx].Value;
      }
    }

    /// <summary>Removes the expired element from this window. Note: normally this is done when you use the <see cref="Add"/> function, thus there is no need to call this function separately.
    /// When the minimum item is the item that is expired now, then this function will remove this item from the collection and sets the current minimum to the next greater item.</summary>
    public void Remove()
    {
      if (_items[_minItemIdx].ExpireGeneration == _generation)
      {
        _minItemIdx++;
        if (_minItemIdx >= _items.Length)
          _minItemIdx = 0;
      }
    }

    /// <summary>Adds the specified value to the window, and removes the item that is now expired from the window.</summary>
    /// <param name="val">The val.</param>
    public void Add(T val)
    {
      if (_items[_minItemIdx].ExpireGeneration == _generation)
      {
        _minItemIdx++;
        if (_minItemIdx >= _items.Length)
          _minItemIdx = 0;
      }

      if (_comparer(val, _items[_minItemIdx].Value) <= 0)
      {
        _items[_minItemIdx].Value = val;
        _items[_minItemIdx].ExpireGeneration = _generation + _items.Length;
        _lastItemIdx = _minItemIdx;
      }
      else
      {
        while (_comparer(val, _items[_lastItemIdx].Value) <= 0)
        {
          if (_lastItemIdx == 0)
            _lastItemIdx = _items.Length;
          --_lastItemIdx;
        }
        ++_lastItemIdx;
        if (_lastItemIdx == _items.Length)
          _lastItemIdx = 0;

        _items[_lastItemIdx].Value = val;
        _items[_lastItemIdx].ExpireGeneration = _generation + _items.Length;
      }
      ++_generation;
    }

    /// <summary>Gets the sliding minimum of an enumeration.</summary>
    /// <param name="list">The enumeration to enumerate through</param>
    /// <param name="windowWidth">Width of the sliding window.</param>
    /// <returns>An enumeration. Each value is the minimum of the <paramref name="windowWidth"/> values (including the current one) of the original enumeration.</returns>
    public static IEnumerable<T> GetSlidingMinimum(IEnumerable<T> list, int windowWidth)
    {
      var window = (list.FirstOrDefault() is { } firstVal) ? new MinimumOnSlidingWindow<T>(windowWidth, firstVal) : null;
      if (!(window is null))
      {
        foreach (var val in list)
        {
          window.Add(val);
          yield return window.MinimumValue;
        }
      }
    }
  }
}
