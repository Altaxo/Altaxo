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

using Altaxo.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	public class StyleListBase<T> : IStyleList<T> where T : Main.IImmutable // TODO NET45 replace IList with IReadonlyList
	{
		protected string _name;
		protected IList<T> _list;

		#region Serialization

		protected StyleListBase(string name, List<T> listToTakeDirectly, Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
			_name = name;
			_list = listToTakeDirectly;
		}

		#endregion Serialization

		public StyleListBase(string name, IEnumerable<T> symbols)
		{
			_name = name;
			_list = new List<T>(symbols);
			if (_list.Count == 0)
				throw new ArgumentException("Provided enumeration is emtpy", nameof(symbols));
		}

		public string Name { get { return _name; } }

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
				var result = (StyleListBase<T>)this.MemberwiseClone();
				result._name = name;
				return result;
			}
		}

		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

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

		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			throw new InvalidOperationException("List is a read-only list");
		}

		public void RemoveAt(int index)
		{
			throw new InvalidOperationException("List is a read-only list");
		}

		public void Add(T item)
		{
			throw new InvalidOperationException("List is a read-only list");
		}

		public void Clear()
		{
			throw new InvalidOperationException("List is a read-only list");
		}

		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			throw new InvalidOperationException("List is a read-only list");
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		#region Structural comparison

		public static bool AreListsStructuralEquivalent(IList<T> l1, IList<T> l2) // TODO NET45 Replace with IReadonlyList
		{
			if (l1 == null || l2 == null)
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

		public bool IsStructuralEquivalentTo(IEnumerable<T> l1) // TODO NET45 Replace with IReadonlyList
		{
			if (l1 == null)
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