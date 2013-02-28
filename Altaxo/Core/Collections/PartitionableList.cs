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
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
	public partial class PartitionableList<T> : System.Collections.ObjectModel.ObservableCollection<T>
	{
		/// <summary>Contains all partial views that were created for this instance and are still alive.</summary>
		LinkedList<WeakReference> _partialViews = new LinkedList<WeakReference>();

		/// <summary>
		/// Creates a partial view. A partial view is a view of the list of items of the original collection, that fulfill a certain condition.
		/// </summary>
		/// <param name="selectionCriterium">The selection condition. If this function returns <c>true</c> on an item of the original collection, that item is member of the partial view.</param>
		/// <returns>List of items of the original collection, that fullfils the selection condition. The returned list is automatically updated, when the original collection changed or when
		/// the user changed the partial view itself.</returns>
		public IList<T> CreatePartialView(Func<T, bool> selectionCriterium)
		{
			var result = new PartialView<T>(this, selectionCriterium);
			_partialViews.AddLast(new WeakReference(result));
			return result;
		}


		#region List change operation overrides

		protected override void ClearItems()
		{
			var node = _partialViews.First;
			while (null != node)
			{
				var pv = node.Value.Target as PartialView<T>;
				if (null != pv)
				{
					pv._itemIndex.Clear();
					node = node.Next;
				}
				else
				{
					var oldNode = node;
					_partialViews.Remove(node);
					node = node.Next;
				}
			}

			base.ClearItems();
		}

		protected override void InsertItem(int index, T item)
		{
			base.InsertItem(index, item);

			for (var node = _partialViews.First; null != node; node = node.Next)
			{
				var pv = node.Value.Target as PartialView<T>;
				if (null != pv)
				{
					var itemIndex = pv._itemIndex;
					int i;
					for (i = itemIndex.Count - 1; i >= 0 && itemIndex[i] >= index; --i)
						++itemIndex[i];

					if (pv._selectionCriterium(item))
						itemIndex.Insert(i + 1, index);
				}
				else
				{
					var oldNode = node;
					_partialViews.Remove(node);
				}

				node = node.Next;
			}
		}

		protected override void MoveItem(int oldIndex, int newIndex)
		{
			for (var node = _partialViews.First; null != node; node = node.Next)
			{
				var pv = node.Value.Target as PartialView<T>;
				if (null != pv)
				{
					var itemIndex = pv._itemIndex;
					int i,j;
					int oIdx = FindIndexOfItemGreaterThanOrEqualTo(itemIndex, oldIndex);
					bool isItemIncluded = oIdx<itemIndex.Count && itemIndex[oIdx] == oldIndex;

					if (oldIndex < newIndex) // item is moved to higher index 
					{
						if(isItemIncluded)
						{
						for(i=oIdx, j=oIdx+1; j<itemIndex.Count && itemIndex[j]<newIndex;++i,++j)
							itemIndex[i] = itemIndex[j]-1;
						itemIndex[i] = newIndex;
						}
						else
						{
						for(i=oIdx; i<itemIndex.Count && itemIndex[i]<newIndex;++i)
							--itemIndex[i];
						}
					}
					else // item is moved to lower index 
					{
						if(isItemIncluded)
						{
							for(i=oIdx, j=oIdx-1; j>=0 && itemIndex[j]>=newIndex;--i,--j)
							itemIndex[i] = itemIndex[j]+1;
						itemIndex[i] = newIndex;
						}
						else
						{
							for(i=oIdx; i>=0 && itemIndex[i]>=newIndex;--i)
							++itemIndex[i];
						}
					}
				}
				else
				{
					var oldNode = node;
					_partialViews.Remove(node);
				}
			}
		
			base.MoveItem(oldIndex, newIndex);
		}

		protected override void RemoveItem(int index)
		{
			for (var node = _partialViews.First; null != node; node = node.Next)
			{
				var pv = node.Value.Target as PartialView<T>;
				if (null != pv)
				{
					var itemIndex = pv._itemIndex;
					int i;
					for (i = itemIndex.Count - 1; i >= 0 && itemIndex[i] > index; --i)
						--itemIndex[i];

					if (i >= 0 && itemIndex[i] == index)
						itemIndex.RemoveAt(i);
				}
				else
				{
					var oldNode = node;
					_partialViews.Remove(node);
				}
			}

			base.RemoveItem(index);
		}

		protected override void SetItem(int index, T item)
		{
			base.SetItem(index, item);

			for (var node = _partialViews.First; null != node; node = node.Next)
			{
				var pv = node.Value.Target as PartialView<T>;
				if (null != pv)
				{
					var itemIndex = pv._itemIndex;
					int i = FindIndexOfItemGreaterThanOrEqualTo(itemIndex, index);
					if (i < itemIndex.Count)
					{
						if (itemIndex[i] == index)
						{
							if (!pv._selectionCriterium(item))
								itemIndex.RemoveAt(i);
						}
						else
						{
							if (pv._selectionCriterium(item))
								itemIndex.Insert(i, index);
						}
					}
				}
				else
				{
					var oldNode = node;
					_partialViews.Remove(node);
				}
			}

		}

		/// <summary>
		/// Finds the item in the list that than or equal to.
		/// </summary>
		/// <param name="itemIndex">List with the ordered list of item indices.</param>
		/// <param name="value">The index to find.</param>
		/// <returns></returns>
		private static int FindIndexOfItemGreaterThanOrEqualTo(IList<int> itemIndex, int value)
		{
			int upperIndex = itemIndex.Count - 1;
			if (upperIndex < 0)
				return 0;

			int lowerIndex = 0;
			int upperValue = itemIndex[upperIndex];
			int lowerValue = itemIndex[lowerIndex];

			while (lowerValue < value && value < upperValue && lowerIndex<upperIndex)
			{
				int middleIndex = upperIndex - ((upperIndex - lowerIndex) / 2);
				int middleValue = itemIndex[middleIndex];
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
			return value > upperValue ? upperIndex + 1 : upperIndex;
		}

		#endregion



	}
}
