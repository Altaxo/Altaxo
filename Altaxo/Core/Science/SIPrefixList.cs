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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Science
{
	public interface ISIPrefixList : IEnumerable<SIPrefix>
	{
		int Count { get; }
		SIPrefix TryGetPrefixFromShortCut(string shortCut);
		bool ContainsNonePrefixOnly { get; }
	}


	public class SIPrefixList : ISIPrefixList
	{
		Dictionary<string, SIPrefix> _shortCutDictionary;

		public SIPrefixList(IEnumerable<SIPrefix> from)
		{
			_shortCutDictionary = new Dictionary<string, SIPrefix>();
			foreach (var e in from)
				_shortCutDictionary.Add(e.ShortCut, e);
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
