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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Graph;

namespace Altaxo.Drawing
{
  /// <summary>
  /// Provides a read-only base implementation for immutable style lists.
  /// </summary>
  /// <typeparam name="T">The type of the style items.</typeparam>
  public class StyleListBase<T> : IStyleList<T> where T : Main.IImmutable
  {
    /// <summary>
    /// The name of the style list.
    /// </summary>
    protected string _name;
    /// <summary>
    /// The items stored in the style list.
    /// </summary>
    protected IList<T> _list;

    #region Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="StyleListBase{T}"/> class for deserialization.
    /// </summary>
    /// <param name="name">The list name.</param>
    /// <param name="listToTakeDirectly">The list instance to use directly as storage.</param>
    /// <param name="info">The deserialization information.</param>
    protected StyleListBase(string name, List<T> listToTakeDirectly, Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      _name = name;
      _list = listToTakeDirectly;
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="StyleListBase{T}"/> class.
    /// </summary>
    /// <param name="name">The list name.</param>
    /// <param name="symbols">The items to include in the list.</param>
    public StyleListBase(string name, IEnumerable<T> symbols)
    {
      _name = name;
      _list = new List<T>(symbols);
      if (_list.Count == 0)
        throw new ArgumentException("Provided enumeration is emtpy", nameof(symbols));
    }

    /// <inheritdoc />
    public string Name { get { return _name; } }

    /// <inheritdoc />
    public IStyleList<T> WithName(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof(name) + " is null or empty");

      if (_name == name)
      {
        return this;
      }
      else
      {
        var result = (StyleListBase<T>)MemberwiseClone();
        result._name = name;
        return result;
      }
    }

    /// <inheritdoc />
    public int Count
    {
      get
      {
        return _list.Count;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this list is read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get
      {
        return true;
      }
    }

    /// <inheritdoc />
    public T this[int index]
    {
      get
      {
        return _list[index];
      }

      set
      {
        throw new InvalidOperationException("List is a read-only list");
      }
    }

    /// <summary>
    /// Returns the index of the specified item.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    /// <returns>The zero-based index of the item, or -1 if the item was not found.</returns>
    public int IndexOf(T item)
    {
      return _list.IndexOf(item);
    }

    /// <summary>
    /// Throws because the list is immutable.
    /// </summary>
    /// <param name="index">The insertion index.</param>
    /// <param name="item">The item to insert.</param>
    public void Insert(int index, T item)
    {
      throw new InvalidOperationException("List is a read-only list");
    }

    /// <summary>
    /// Throws because the list is immutable.
    /// </summary>
    /// <param name="index">The index of the item to remove.</param>
    public void RemoveAt(int index)
    {
      throw new InvalidOperationException("List is a read-only list");
    }

    /// <summary>
    /// Throws because the list is immutable.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Add(T item)
    {
      throw new InvalidOperationException("List is a read-only list");
    }

    /// <summary>
    /// Throws because the list is immutable.
    /// </summary>
    public void Clear()
    {
      throw new InvalidOperationException("List is a read-only list");
    }

    /// <summary>
    /// Determines whether the list contains the specified item.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    /// <returns><c>true</c> if the item is contained in the list; otherwise, <c>false</c>.</returns>
    public bool Contains(T item)
    {
      return _list.Contains(item);
    }

    /// <summary>
    /// Copies the elements of the list to an array.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The zero-based destination index.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
      _list.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Throws because the list is immutable.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>This method does not return normally.</returns>
    public bool Remove(T item)
    {
      throw new InvalidOperationException("List is a read-only list");
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
      return _list.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _list.GetEnumerator();
    }

    #region Structural comparison

    /// <summary>
    /// Determines whether two lists are structurally equivalent.
    /// </summary>
    /// <param name="l1">The first list.</param>
    /// <param name="l2">The second list.</param>
    /// <returns><c>true</c> if both lists contain equal items in the same order; otherwise, <c>false</c>.</returns>
    public static bool AreListsStructuralEquivalent(IReadOnlyList<T> l1, IReadOnlyList<T> l2)
    {
      if (l1 is null || l2 is null)
        return false;

      if (l1.Count != l2.Count)
        return false;

      for (int i = l1.Count - 1; i >= 0; --i)
      {
        if (!l1[i].Equals(l2[i]))
          return false;
      }

      return true;
    }

    /// <inheritdoc />
    public bool IsStructuralEquivalentTo(IEnumerable<T> l1)
    {
      if (l1 is null)
        return false;

      var l2 = this;

      int i = 0;
      int len2 = l2.Count;
      foreach (var item1 in l1)
      {
        if (i >= len2)
          return false;

        if (!item1.Equals(l2[i]))
          return false;
        ++i;
      }

      if (i != l2.Count)
        return false;

      return true;
    }

    #endregion Structural comparison
  }
}
