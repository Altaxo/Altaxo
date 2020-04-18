#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

#nullable enable

namespace Altaxo.Collections
{
  /// <summary>
  /// Implements an collection of ascending integers, that are stored in a list of ranges.
  /// </summary>
  public class AscendingIntegerRangeCollection : IEnumerable<int>
  {
    private const int AndMaskToEvenValue = ~1;
    private const int OrMaskToOddValue = 1;

    /// <summary>
    /// The _range array.
    /// </summary>
    private int[] _rangeArray;

    /// <summary>
    /// Current number of elements in <see cref="_rangeArray"/>.
    /// </summary>
    private int _rangeArrayCount;

    /// <summary>
    /// Array of cumulative sum of number of elements in the ranges.
    /// Element at index [0] contains the number of elements in range[0] (i.e. 1+to-from),
    /// Element at index [1] contains number of elements in range[0] plus number of elements in range[1],
    /// Element at index [2] contains number of elements in ranges[0..2] and so on.
    /// Last element (at index RangeCount-1) contains sum of number of elements in all ranges.
    /// </summary>
    private int[] _indexCache;

    /// <summary>
    /// Designates the first index in array <see cref="_indexCache"/> that contains invalid data.
    /// If range data are changed, the <see cref="_indexCacheInvalidFrom"/> is calculated simply
    /// by dividing the index of the range data by 2 (or using the shift operator) and take the minimum with the current value.
    /// </summary>
    private int _indexCacheInvalidFrom;

    /// <summary>Permanent proxy for the range collection</summary>
    private RangeCollectionProxy? _collectionOfRanges;

    public AscendingIntegerRangeCollection()
    {
      _rangeArray = new int[8];
      _indexCache = new int[4];
    }

    /// <summary>
    /// Gets the number of ranges in this collection.
    /// </summary>
    /// <value>
    /// Number of ranges in the collection.
    /// </value>
    public int RangeCount
    {
      get
      {
        return _rangeArrayCount >> 1;
      }
    }

    /// <summary>
    /// Gets the ranges of the collection
    /// </summary>
    /// <value>
    /// The ranges.
    /// </value>
    public IEnumerable<ContiguousIntegerRange> Ranges
    {
      get
      {
        if (_collectionOfRanges is null)
          _collectionOfRanges = new RangeCollectionProxy(this);
        return _collectionOfRanges;
      }
    }

    #region public functions

    /// <summary>
    /// Adds the specified element. Detail: The element is added as a range with a start value equal to element and a count of 1.
    /// </summary>
    /// <param name="element">The element to add.</param>
    public void Add(int element)
    {
      InternalAdd(element, element);
    }

    /// <summary>
    /// Removes the specified element.
    /// </summary>
    /// <param name="element">The element to remove.</param>
    public void Remove(int element)
    {
      InternalRemove(element, element);
    }

    /// <summary>
    /// Adds a range that is specified by a first value and by a last value (inclusive).
    /// </summary>
    /// <param name="first">First element of the range to add.</param>
    /// <param name="lastInclusive">Last element of the range (inclusive, i.e. this element is included in the range).</param>
    /// <exception cref="System.ArgumentOutOfRangeException">Argument 'to' has to be greater than or equal to argument 'from'</exception>
    public void AddRangeByFirstAndLastInclusive(int first, int lastInclusive)
    {
      if (lastInclusive < first)
        throw new ArgumentOutOfRangeException("Argument 'to' has to be greater than or equal to argument 'from'");

      InternalAdd(first, lastInclusive);
    }

    /// <summary>
    /// Removes all elements that belong to the provided range that is specified by a first value and by a last value (inclusive).
    /// </summary>
    /// <param name="first">First element of the range to remove.</param>
    /// <param name="lastInclusive">Last element of the range (inclusive, i.e. this element is included in the range).</param>
    /// <exception cref="System.ArgumentOutOfRangeException">Argument 'to' has to be greater than or equal to argument 'from'</exception>
    public void RemoveRangeByFirstAndLastInclusive(int first, int lastInclusive)
    {
      if (lastInclusive < first)
        throw new ArgumentOutOfRangeException("Argument 'to' has to be greater than or equal to argument 'from'");

      InternalRemove(first, lastInclusive);
    }

    /// <summary>
    /// Determines whether this collection contains the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><c>True</c> if the collection contains the element, otherwise <c>false</c>.</returns>
    public bool Contains(int element)
    {
      return Contains(element, out var _);
    }

    /// <summary>
    /// Determines whether this collection contains the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="rangeIndex">If the collection contains the element, this value is the index of the range that contains the element.</param>
    /// <returns><c>True</c> if the collection contains the element, otherwise <c>false</c>.</returns>
    public bool Contains(int element, out int rangeIndex)
    {
      rangeIndex = -1;
      if (0 == _rangeArrayCount)
        return false;

      var idx = Array.BinarySearch(_rangeArray, 0, _rangeArrayCount, element);

      if (idx >= 0) // range found directly, thus element is either FROM or TO of the range, in both cases it belongs to the range
      {
        rangeIndex = idx & AndMaskToEvenValue;
        return true;
      }
      else // element was not found directly
      {
        idx = ~idx;
        if (idx >= _rangeArrayCount) // element is behind the array, thus it is not included
        {
          return false;
        }
        else
        {
          int evenIdx = idx & AndMaskToEvenValue;
          if (idx == evenIdx) // element is less than FROM, thus it is between to ranges, and thus not included
          {
            return false;
          }
          else // element is less than TO, but greater than FROM, thus it is included in this range
          {
            rangeIndex = evenIdx;
            return true;
          }
        }
      }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<int> GetEnumerator()
    {
      var ranges = _rangeArray;
      for (int i = 0; i < _rangeArrayCount; i += 2)
      {
        var to = ranges[i + 1];
        for (int j = ranges[i]; j <= to; ++j)
        {
          yield return j;
        }
      }
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
    /// </returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary>
    /// Gets the number of elements in the collection.
    /// </summary>
    /// <value>
    /// The number of elements in the collection.
    /// </value>
    public int Count
    {
      get
      {
        var rangeCount = RangeCount;
        if (rangeCount > 0)
        {
          if (_indexCacheInvalidFrom < rangeCount)
            InternalUpdateIndexCache();
          return _indexCache[rangeCount - 1];
        }
        else
        {
          return 0;
        }
      }
    }

    /// <summary>
    /// Gets the <see cref="int"/> element with the specified index.
    /// </summary>
    /// <value>The element.</value>
    /// <param name="idx">The index of the element.</param>
    /// <returns>Element at the specified index.</returns>
    /// <exception cref="System.IndexOutOfRangeException">
    /// If idx is less than 0 or greater than or equal to <see cref="Count"/>.
    /// </exception>
    public int this[int idx]
    {
      get
      {
        if (idx < 0)
          throw new IndexOutOfRangeException("idx<0");

        var rangeCount = RangeCount;
        if (_indexCacheInvalidFrom < rangeCount)
          InternalUpdateIndexCache();

        int iFound = Array.BinarySearch(_indexCache, 0, rangeCount, idx);

        if (iFound >= 0)
        {
          return _rangeArray[2 * (iFound + 1)]; // note that the found index belongs to the start of the __next__ range
        }
        else
        {
          iFound = ~iFound;
          if (iFound >= rangeCount)
            throw new IndexOutOfRangeException("idx is behind last valid index");

          // the index now designates the segment itself
          var sumHere = iFound > 0 ? _indexCache[iFound - 1] : 0;
          if (!(idx >= sumHere))
            throw new InvalidProgramException();
          var rangeFrom = _rangeArray[iFound * 2];
          return rangeFrom + (idx - sumHere);
        }
      }
    }

    #endregion public functions

    #region Internal functions

    /// <summary>
    /// Adds a range specified by from (first element) and to (last inclusive element).
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    private void InternalAdd(int from, int to)
    {
      var fromM1 = from == int.MinValue ? int.MinValue : from - 1;
      var idx = Array.BinarySearch(_rangeArray, 0, _rangeArrayCount, fromM1);

      if (idx >= 0) // range found, the range starts at the even number
      {
        idx |= OrMaskToOddValue; // this is the range's TO value
        _rangeArray[idx] = Math.Max(_rangeArray[idx], to);
        _indexCacheInvalidFrom = Math.Min(idx >> 1, _indexCacheInvalidFrom);
        InternalCoalesceSubsequentRanges(idx);
      }
      else // fromM1 not found in the array
      {
        idx = ~idx;
        if (idx >= _rangeArrayCount) // behind the array
        {
          InternalAddRangeAtEndOfArray(from, to); // thus add a new range at the end of the array
        }
        else // valid idx, the element at index idx is larger than fromM1
        {
          int evenIdx = idx & AndMaskToEvenValue;
          if (idx == evenIdx) // we are located before a range, thus we look if we can coalesce with this range
          {
            var toP1 = to == int.MaxValue ? to : to + 1;
            if (toP1 >= _rangeArray[evenIdx]) // yes - we can coalesce!
            {
              _rangeArray[evenIdx] = Math.Min(_rangeArray[evenIdx], from);
              _rangeArray[evenIdx + 1] = Math.Max(_rangeArray[evenIdx + 1], to);
              _indexCacheInvalidFrom = Math.Min(idx >> 1, _indexCacheInvalidFrom);
              InternalCoalesceSubsequentRanges(evenIdx + 1);
            }
            else // we can not coalesce, thus we have to insert a new range
            {
              InternalInsertNewRange(evenIdx, from, to);
            }
          }
          else // we are located inside a range, because idx is odd
          {
            _rangeArray[idx] = Math.Max(_rangeArray[idx], to); // coalesce the TO values with this range
            _indexCacheInvalidFrom = Math.Min(idx >> 1, _indexCacheInvalidFrom);
            InternalCoalesceSubsequentRanges(idx);
          }
        }
      }
    }

    /// <summary>
    /// Removes a range specified by from (first element) and to (last inclusive element).
    /// </summary>
    /// <param name="rmvFrom">The first element to remove.</param>
    /// <param name="rmvTo">The last inclusive element to remove.</param>
    private void InternalRemove(int rmvFrom, int rmvTo)
    {
      var idx = Array.BinarySearch(_rangeArray, 0, _rangeArrayCount, rmvTo);

      var rmvFromM1 = rmvFrom == int.MinValue ? int.MinValue : rmvFrom - 1;
      var rmvToP1 = rmvTo == int.MaxValue ? int.MaxValue : rmvTo + 1;

      if (idx >= 0) // range found
      {
        int evenIdx = idx & AndMaskToEvenValue;
        if (idx == evenIdx) // remove TO is equal to range's FROM
        {
          // rmvTo is FROM of the range
          bool removeThisRange = (_rangeArray[evenIdx + 1] == _rangeArray[evenIdx]); // if the range was a single number (from==to), it is now empty and must be removed.
          _rangeArray[evenIdx] = rmvToP1; // set ranges from value to rmvTo+1

          PruneThisAndPriorRanges(removeThisRange ? evenIdx : evenIdx - 2, rmvFromM1, removeThisRange);
        }
        else // idx is odd, thus rmvTo is equal to TO of our range
        {
          PruneThisAndPriorRanges(evenIdx, rmvFromM1, false);
        }
      }
      else // rmvTo was not found directly
      {
        idx = ~idx;
        if (idx >= _rangeArrayCount) // rmvTo value  is behind the array
        {
          PruneThisAndPriorRanges(_rangeArrayCount - 2, rmvFromM1, false);
        }
        else
        {
          int evenIdx = idx & AndMaskToEvenValue;
          if (idx == evenIdx) // rmvTo value is located before a range, thus this range is not affected, but maybe the ranges before
          {
            PruneThisAndPriorRanges(evenIdx - 2, rmvFromM1, false);
          }
          else // rmvTo value is less than TO value of range, but greater than FROM value of range
          {
            if (!(rmvTo < _rangeArray[idx]))
              throw new InvalidProgramException(); // TO value to remove is less than TO value of range

            if (rmvFrom > _rangeArray[evenIdx]) // special case: our range will become splitted into two ranges
            {
              var originalRangeTo = _rangeArray[idx];
              _rangeArray[idx] = rmvFromM1;
              AddRangeByFirstAndLastInclusive(rmvToP1, originalRangeTo);
            }
            else // rmvFrom is equal to or lass than range.From
            {
              bool removeThisRange = (_rangeArray[evenIdx] == _rangeArray[evenIdx + 1] || rmvToP1 > _rangeArray[evenIdx + 1]); // first equality term is a special care for int.MaxValue
              _rangeArray[evenIdx] = rmvToP1;
              _indexCacheInvalidFrom = Math.Min(evenIdx >> 1, _indexCacheInvalidFrom);
              PruneThisAndPriorRanges(removeThisRange ? evenIdx : evenIdx - 2, rmvFromM1, removeThisRange);
            }
          }
        }
      }
    }

    /// <summary>
    /// When removing ranges, it may be neccessary not only to change the current range, but also to
    /// either delete or to change ranges prior to the current range.
    /// </summary>
    /// <param name="idx">The index into the <see cref="_rangeArray"/> where to start the pruning.</param>
    /// <param name="toFromValueM1">FROM value minus 1 of the range to remove.</param>
    /// <param name="deleteCurrentRange">If set to <c>true</c>, the current range is deleted without further consideration, and the pruning is immediately continued with the range prior to the current range.</param>
    private void PruneThisAndPriorRanges(int idx, int toFromValueM1, bool deleteCurrentRange)
    {
      if (!((idx & AndMaskToEvenValue) == idx))
        throw new InvalidProgramException();

      int startIndex = idx; // index of TO of the next range
      int nextIndex;
      for (nextIndex = deleteCurrentRange ? startIndex - 2 : startIndex; nextIndex >= 0; nextIndex -= 2)
      {
        if (_rangeArray[nextIndex + 1] <= toFromValueM1)
        {
          break;
        }

        if (_rangeArray[nextIndex] <= toFromValueM1)
        {
          _rangeArray[nextIndex + 1] = toFromValueM1;
          break;
        }
      }

      if (nextIndex < startIndex)
      {
        // remove all ranged from nextIndex+2 to startIndex+2
        Array.Copy(_rangeArray, startIndex + 2, _rangeArray, nextIndex + 2, _rangeArrayCount - startIndex - 2);
        _rangeArrayCount -= (startIndex - nextIndex);
        _indexCacheInvalidFrom = Math.Min((nextIndex + 2) >> 1, _indexCacheInvalidFrom);
      }
    }

    /// <summary>
    /// Starting from range at index <paramref name="idx"/>, looks to ranges with indices greater than <paramref name="idx"/>.
    /// If those ranges can be united with the range at index <paramref name="idx"/>, the range at index <paramref name="idx"/> is updated,
    /// and the now superflous ranges will be deleted.
    /// </summary>
    /// <param name="idx">The index.</param>
    private void InternalCoalesceSubsequentRanges(int idx)
    {
      if (!((idx | OrMaskToOddValue) == idx))
        throw new InvalidProgramException();
      if (idx + 1 >= _rangeArrayCount)
        return; // at the end of the array, thus nothing to do

      var coalescedToValue = _rangeArray[idx]; // TO value of the range in consideration

      int startIndex = idx + 1; // index of FROM of the next range
      int nextIndex;
      for (nextIndex = startIndex; nextIndex < _rangeArrayCount; nextIndex += 2)
      {
        int fromM1 = _rangeArray[nextIndex] - 1; // no need to test for int.MinimumValue, because this from value belongs to at least the second range, and this should always at least >= int.MinValue+2
        if (fromM1 <= coalescedToValue)
        {
          coalescedToValue = Math.Max(coalescedToValue, _rangeArray[nextIndex + 1]);
        }
        else
        {
          break;
        }
      }
      _rangeArray[idx] = coalescedToValue;
      _indexCacheInvalidFrom = Math.Min(idx >> 1, _indexCacheInvalidFrom);

      // we have to delete all ranges between startIdx+1 and i;
      if (nextIndex > startIndex)
      {
        Array.Copy(_rangeArray, nextIndex, _rangeArray, startIndex, _rangeArrayCount - nextIndex);
        _rangeArrayCount -= (nextIndex - startIndex);
        if (!(_rangeArrayCount >= 2 && (_rangeArrayCount % 2) == 0))
          throw new InvalidProgramException();
      }
    }

    /// <summary>
    /// Adds a single range at the end of the array (ensuring that there is enough space for doing so).
    /// </summary>
    /// <param name="from">Range FROM value.</param>
    /// <param name="to">Range TO value.</param>
    private void InternalAddRangeAtEndOfArray(int from, int to)
    {
      InternalEnsureEnoughSpace();

      _rangeArray[_rangeArrayCount++] = from;
      _rangeArray[_rangeArrayCount++] = to;
    }

    /// <summary>
    /// Inserts a single range at position index in the <see cref="_rangeArray"/>. Elements behind are shifted upwards by two positions to make space for the new range.
    /// </summary>
    /// <param name="atIndex">Position in the <see cref="_rangeArray"/> where to insert the new range.</param>
    /// <param name="from">Range FROM value.</param>
    /// <param name="to">Range TO value.</param>
    private void InternalInsertNewRange(int atIndex, int from, int to)
    {
      InternalEnsureEnoughSpace();
      Array.Copy(_rangeArray, atIndex, _rangeArray, atIndex + 2, _rangeArrayCount - atIndex);
      _rangeArrayCount += 2;

      _rangeArray[atIndex] = from;
      _rangeArray[atIndex + 1] = to;
      _indexCacheInvalidFrom = Math.Min(atIndex >> 1, _indexCacheInvalidFrom);
    }

    /// <summary>
    /// Ensures that there is enough space to accomodate at least one new range.
    /// </summary>
    private void InternalEnsureEnoughSpace()
    {
      if (_rangeArray.Length == _rangeArrayCount)
      {
        var newArray = new int[_rangeArray.Length * 2];
        Array.Copy(_rangeArray, newArray, _rangeArray.Length);
        _rangeArray = newArray;
      }
    }

    /// <summary>
    /// Updates the index cache, starting from <see cref="_indexCacheInvalidFrom"/> up to the highest range.
    /// </summary>
    private void InternalUpdateIndexCache()
    {
      int rangeCount = RangeCount;
      if (_indexCache.Length < rangeCount)
      {
        var newArr = new int[_rangeArray.Length / 2];
        Array.Copy(_indexCache, newArr, _indexCache.Length);
        _indexCache = newArr;
      }

      int sumOfItems = _indexCacheInvalidFrom > 0 ? _indexCache[_indexCacheInvalidFrom - 1] : 0;
      int i, j;
      for (i = _indexCacheInvalidFrom, j = 2 * _indexCacheInvalidFrom; i < rangeCount; ++i, j += 2)
      {
        sumOfItems += (_rangeArray[j + 1] - _rangeArray[j]) + 1;
        _indexCache[i] = sumOfItems;
      }
      _indexCacheInvalidFrom = i;
    }

    #endregion Internal functions

    #region Internal proxy classes

    /// <summary>
    /// Helper class that provides the range enumeration as a public interface.
    /// </summary>
    private class RangeCollectionProxy : IEnumerable<ContiguousIntegerRange>
    {
      private AscendingIntegerRangeCollection _parent;

      internal RangeCollectionProxy(AscendingIntegerRangeCollection parent)
      {
        _parent = parent;
      }

      public IEnumerator<ContiguousIntegerRange> GetEnumerator()
      {
        var rangeArray = _parent._rangeArray;
        var len = _parent._rangeArrayCount;
        for (int i = 0; i < len; i += 2)
          yield return ContiguousIntegerRange.FromFirstAndLastInclusive(rangeArray[i], rangeArray[i + 1]);
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        return GetEnumerator();
      }
    }

    #endregion Internal proxy classes
  }
}
