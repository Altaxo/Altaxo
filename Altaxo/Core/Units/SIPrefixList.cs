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

using Altaxo.Calc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Units
{
  public interface ISIPrefixList : IEnumerable<SIPrefix>
  {
    int Count { get; }

    SIPrefix TryGetPrefixFromShortCut(string shortCut);

    bool ContainsNonePrefixOnly { get; }
  }

  public class SIPrefixList : ISIPrefixList
  {
    private Dictionary<string, SIPrefix> _shortCutDictionary;
    private Dictionary<int, SIPrefix> _exponentDictionary;
    private int[] _allExponentsSorted;

    public SIPrefixList(IEnumerable<SIPrefix> from)
    {
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

    public SIPrefix TryGetPrefixFromShortCut(string shortCut)
    {
      SIPrefix result;
      if (_shortCutDictionary.TryGetValue(shortCut, out result))
        return result;
      else
        return null;
    }

    public (SIPrefix prefix, double remainingFactor) GetPrefixFromExponent(int exponent)
    {
      if (_exponentDictionary.TryGetValue(exponent, out SIPrefix result))
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

    public IEnumerator<SIPrefix> GetEnumerator()
    {
      return _shortCutDictionary.Values.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _shortCutDictionary.Values.GetEnumerator();
    }
  }
}
