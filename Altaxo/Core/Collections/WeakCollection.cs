﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;

namespace Altaxo.Collections
{
  /// <summary>
  /// A collection that allows its elements to be garbage-collected (unless there are other
  /// references to the elements). Elements will disappear from the collection when they are
  /// garbage-collected.
  ///
  /// The WeakCollection is not thread-safe, not even for read-only access!
  /// No methods may be called on the WeakCollection while it is enumerated, not even a Contains or
  /// creating a second enumerator.
  /// The WeakCollection does not preserve any order among its contents; the ordering may be different each
  /// time the collection is enumerated.
  ///
  /// Since items may disappear at any time when they are garbage collected, this class
  /// cannot provide a useful implementation for Count and thus cannot implement the ICollection interface.
  /// </summary>
  public class WeakCollection<T> : IEnumerable<T> where T : class
  {
    private readonly List<WeakReference> innerList = new List<WeakReference>();

    /// <summary>
    /// Adds an element to the collection. Runtime: O(n).
    /// </summary>
    public void Add(T item)
    {
      if (item is null)
        throw new ArgumentNullException("item");
      CheckNoEnumerator();
      if (innerList.Count == innerList.Capacity || (innerList.Count % 32) == 31)
        innerList.RemoveAll(delegate (WeakReference r)
        { return !r.IsAlive; });
      innerList.Add(new WeakReference(item));
    }

    /// <summary>
    /// Removes all elements from the collection. Runtime: O(n).
    /// </summary>
    public void Clear()
    {
      innerList.Clear();
      CheckNoEnumerator();
    }

    /// <summary>
    /// Removes an element from the collection. Returns true if the item is found and removed,
    /// false when the item is not found.
    /// Runtime: O(n).
    /// </summary>
    public bool Remove(T item)
    {
      if (item is null)
        return false;
      CheckNoEnumerator();
      var comparer = EqualityComparer<T>.Default;
      for (int i = 0; i < innerList.Count;)
      {
        if (innerList[i].Target is T element)
        {
          if (comparer.Equals(element, item))
          {
            RemoveAt(i);
            return true;
          }
          else
          {
            i++;
          }
        }
        else
        {
          RemoveAt(i);
        }
      }
      return false;
    }

    private void RemoveAt(int i)
    {
      int lastIndex = innerList.Count - 1;
      innerList[i] = innerList[lastIndex];
      innerList.RemoveAt(lastIndex);
    }

    private bool hasEnumerator;

    private void CheckNoEnumerator()
    {
      if (hasEnumerator)
        throw new InvalidOperationException("The WeakCollection is already being enumerated, it cannot be modified at the same time. Ensure you dispose the first enumerator before modifying the WeakCollection.");
    }

    /// <summary>
    /// Enumerates the collection.
    /// Each MoveNext() call on the enumerator is O(1), thus the enumeration is O(n).
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
      if (hasEnumerator)
        throw new InvalidOperationException("The WeakCollection is already being enumerated, it cannot be enumerated twice at the same time. Ensure you dispose the first enumerator before using another enumerator.");
      try
      {
        hasEnumerator = true;
        for (int i = 0; i < innerList.Count;)
        {
          if (innerList[i].Target is T element)
          {
            yield return element;
            i++;
          }
          else
          {
            RemoveAt(i);
          }
        }
      }
      finally
      {
        hasEnumerator = false;
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
