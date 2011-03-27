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
