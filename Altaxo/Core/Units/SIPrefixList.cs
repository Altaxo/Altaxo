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
using Altaxo.Calc;

#nullable enable

namespace Altaxo.Units
{
  /// <summary>
  /// Represents a list of known <see cref="SIPrefix"/>es.
  /// </summary>
  /// <seealso cref="Altaxo.Units.ISIPrefixList" />
  public class SIPrefixList : ISIPrefixList
  {
    private Dictionary<string, SIPrefix> _shortCutDictionary;
    private Dictionary<int, SIPrefix> _exponentDictionary;
    private int[] _allExponentsSorted;

    #region Serialization
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SIPrefixList), 0)]
  /// <summary>
  /// XML serialization surrogate for <see cref="SIPrefixList"/> (version 0).
  /// Handles custom serialization and deserialization of the outer type.
  /// </summary>
  public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    /// <summary>
    /// Serializes the specified <see cref="SIPrefixList"/> instance into the provided
    /// <see cref="Altaxo.Serialization.Xml.IXmlSerializationInfo"/>.
    /// </summary>
    /// <param name="obj">The object to serialize (expected to be a <see cref="SIPrefixList"/>).</param>
    /// <param name="info">The serialization info where values should be written.</param>
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      var s = (SIPrefixList)obj;

      info.CreateArray("PrefixList", s.Count);
      foreach (var prefix in s)
        info.AddValue("e", prefix);
      info.CommitArray();
    }

    /// <summary>
    /// Deserializes an instance of <see cref="SIPrefixList"/> from the provided
    /// <see cref="Altaxo.Serialization.Xml.IXmlDeserializationInfo"/> and returns the reconstructed object.
    /// </summary>
    /// <param name="o">An optional existing object instance (ignored).</param>
    /// <param name="info">The deserialization info to read values from.</param>
    /// <param name="parent">The parent object in the object graph (may be <c>null</c>).</param>
    /// <returns>A new <see cref="SIPrefixList"/> instance created from the serialized data.</returns>
    public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
    {
      int count = info.OpenArray("PrefixList");

      var list = new SIPrefix[count];
      for (int i = 0; i < count; ++i)
        list[i] = (SIPrefix)info.GetValue("e", parent);
      info.CloseArray(count);

      return new SIPrefixList(list);
    }
  }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="SIPrefixList"/> class.
    /// </summary>
    /// <param name="from">An enumeration of prefixes that should be the content of this list.</param>
    /// <exception cref="ArgumentNullException">from</exception>
    public SIPrefixList(IEnumerable<SIPrefix> from)
    {
      if (from is null)
        throw new ArgumentNullException(nameof(from));

      var prefixes = from.ToArray();
      Array.Sort(prefixes, (a, b) => Comparer<int>.Default.Compare(a.Exponent, b.Exponent));

      _allExponentsSorted = prefixes.Select(p => p.Exponent).ToArray();
      _shortCutDictionary = new Dictionary<string, SIPrefix>();
      _exponentDictionary = new Dictionary<int, SIPrefix>();
      foreach (var e in prefixes)
      {
        _shortCutDictionary.Add(e.ShortCut, e);
        _exponentDictionary.Add(e.Exponent, e);
      }
    }

    /// <summary>
    /// Gets the number of prefixes in this list.
    /// </summary>
    /// <value>
    /// Number of prefixes in this list.
    /// </value>
    public int Count
    {
      get { return _shortCutDictionary.Count; }
    }

    /// <summary>
    /// Return true if the collection contains exactly one element, which is the prefix <see cref="SIPrefix.None"/>.
    /// </summary>
    public bool ContainsNonePrefixOnly
    {
      get
      {
        return _shortCutDictionary.Count == 1 && _shortCutDictionary.ContainsKey("");
      }
    }

    /// <summary>
    /// Try the get a prefix, given its shortcut. Example: given the string 'n', this function will return the prefix <see cref="SIPrefix.Nano" />.
    /// </summary>
    /// <param name="shortCut">The short cut.</param>
    /// <returns>
    /// The prefix with the given shortcut. If no such prefix exist, the function will return null.
    /// </returns>
    /// <exception cref="ArgumentNullException">shortCut</exception>
    public SIPrefix? TryGetPrefixFromShortCut(string shortCut)
    {
      if (string.IsNullOrEmpty(shortCut))
        throw new ArgumentNullException(nameof(shortCut));

      return _shortCutDictionary.TryGetValue(shortCut, out var result) ? result : null;
    }

    /// <summary>
    /// Gets the prefix from the given exponent. Example: given an exponent of 9, this call will return the <see cref="SIPrefix.Giga"/> with a remaining factor of 1.
    /// </summary>
    /// <param name="exponent">The exponent.</param>
    /// <returns>The prefix, and a remaining factor. If the exponent is greater than or equal to the exponent of the smallest Prefix (<see cref="SIPrefix.SmallestPrefix"/>), the
    /// remaining factor is greater then or equal to 1.</returns>
    public (SIPrefix prefix, double remainingFactor) GetPrefixFromExponent(int exponent)
    {
      if (_exponentDictionary.TryGetValue(exponent, out SIPrefix? result))
      {
        return (result, 1);
      }

      // if it is not in the dictionary, then it also not is in the array, thus the bitwise complement must always be positive
      int idx = ~Array.BinarySearch(_allExponentsSorted, exponent);

      if (idx >= _allExponentsSorted.Length)
      {
        result = _exponentDictionary[_allExponentsSorted[_allExponentsSorted.Length - 1]];
      }
      else
      {
        result = _exponentDictionary[_allExponentsSorted[idx]];
      }

      return (result, RMath.Pow(10, exponent - result.Exponent));
    }

    /// <summary>
    /// Returns an enumerator that iterates through all known prefixes.
    /// </summary>
    /// <returns>
    /// An enumerator that iterates through all known prefixes.
    /// </returns>
    public IEnumerator<SIPrefix> GetEnumerator()
    {
      return _shortCutDictionary.Values.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through all known prefixes.
    /// </summary>
    /// <returns>
    /// An enumerator that iterates through all known prefixes.
    /// </returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _shortCutDictionary.Values.GetEnumerator();
    }
  }
}
