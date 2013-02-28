#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2013 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 3 of the License, or
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
	public partial class PartitionableList<T> : System.Collections.ObjectModel.ObservableCollection<T>
	{
		private class PartialView<M> : IList<M>
		{
			PartitionableList<M> _collection;
			public Func<M, bool> _selectionCriterium;
			public List<int> _itemIndex;


			protected internal PartialView(PartitionableList<M> list, Func<M, bool> selectionCriterium)
			{
				_collection = list;
				_selectionCriterium = selectionCriterium;
				_itemIndex = new List<int>();
			}


			/// <summary>
			/// Finds the item in the list that is equal to <paramref name="value"/>.
			/// </summary>
			/// <param name="value">The value to found.</param>
			/// <param name="indexFound">On return, contains the index of the item in <see cref="_itemIndex"/> that is equal to contains <paramref name="value"/> (if such an item is found). Otherwise, contains the index of the first item which is greater than <paramref name="value"/>.</param>
			/// <returns>True if <see cref="_itemIndex"/> contains an item equal to <paramref name="value"/>. Otherwise, the return value is <c>false</c>.</returns>
			public bool TryFindIndexOfItemGreaterThanOrEqualTo(int value, out int indexFound)
			{
				int upperIndex = _itemIndex.Count - 1;
				if (upperIndex < 0)
				{
					indexFound = 0;
					return false;
				}

				int lowerIndex = 0;
				int upperValue = _itemIndex[upperIndex];
				int lowerValue = _itemIndex[lowerIndex];

				while (lowerValue < value && value < upperValue && lowerIndex < upperIndex)
				{
					int middleIndex = upperIndex - ((upperIndex - lowerIndex) / 2);
					int middleValue = _itemIndex[middleIndex];
					if (middleValue < value)
					{
						lowerIndex = middleIndex;
						lowerValue = middleValue;
					}
					else
					{
						upperIndex = middleIndex;
						upperValue = middleValue;
					}
				}

				if (value == upperValue)
				{
					indexFound = upperIndex;
					return true;
				}
				else
				{
					indexFound = upperIndex + 1;
					return false;
				}
			}

			#region IList implementations

			public int IndexOf(M item)
			{
				for (int i = 0; i < _itemIndex.Count; ++i)
				{
					if (object.Equals(item, _collection[_itemIndex[i]]))
						return i;
				}
				return -1;
			}

			public void Insert(int index, M item)
			{
				if (index < 0 && index > _itemIndex.Count)
					throw new ArgumentOutOfRangeException("index");
				if (!_selectionCriterium(item))
					throw new ArgumentException("item to insert does not fulfill the selection criterion");

				int insertPoint;

				if (_itemIndex.Count == 0)
				{
					insertPoint = 0;
				}
				if (index == _itemIndex.Count)
				{
					if (_itemIndex.Count == 0)
						insertPoint = 0;
					else
						insertPoint = _itemIndex[_itemIndex.Count - 1] + 1;
				}
				else
				{
					insertPoint = _itemIndex[index];
				}
				_collection.Insert(insertPoint, item);
			}

			public void RemoveAt(int index)
			{
				int j = _itemIndex[index];
				_collection.RemoveAt(j);
			}

			public M this[int index]
			{
				get
				{
					return _collection[_itemIndex[index]];
				}
				set
				{
					if (!_selectionCriterium(value))
						throw new ArgumentException("item does not fulfill the selection criterion");

					_collection[_itemIndex[index]] = value;
				}
			}

			public void Add(M item)
			{
				if (!_selectionCriterium(item))
					throw new ArgumentException("item to insert does not fulfill the selection criterion");

				if (_itemIndex.Count == 0)
				{
					_collection.Add(item);
				}
				else
				{
					_collection.Insert(_itemIndex[_itemIndex.Count - 1] + 1, item);
				}
			}

			public void Clear()
			{
				if (0 == _itemIndex.Count)
					return;
				else if (1 == _itemIndex.Count)
					_collection.RemoveAt(_itemIndex[0]);
				else
				{
					// don't use locks here, because after every RemoveAt the _itemIndex is updated
					while (_itemIndex.Count > 0)
					{

						int j = _itemIndex[_itemIndex.Count - 1];
						_collection.RemoveAt(j);
					}
				}
			}

			public bool Contains(M item)
			{
				for (int i = 0; i < _itemIndex.Count; ++i)
				{
					if (object.Equals(item, _collection[_itemIndex[i]]))
						return true;
				}
				return false;
			}

			public void CopyTo(M[] array, int arrayIndex)
			{
				for (int i = 0; i < _itemIndex.Count; ++i)
					array[i + arrayIndex] = _collection[_itemIndex[i]];
			}

			public int Count
			{
				get { return _itemIndex.Count; }
			}

			public bool IsReadOnly
			{
				get { return false; }
			}

			public bool Remove(M item)
			{
				int i = IndexOf(item);
				if (i < 0)
				{
					return false;
				}
				else
				{
					RemoveAt(i);
					return true;
				}
			}

			public IEnumerator<M> GetEnumerator()
			{
				foreach (int j in _itemIndex)
					yield return _collection[j];
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				foreach (int j in _itemIndex)
					yield return _collection[j];
			}

			#endregion

		}
	}
}