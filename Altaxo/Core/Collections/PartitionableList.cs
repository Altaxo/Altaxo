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

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
	/// <summary>
	/// List of items of class T. From this list partitions can be created, which basically are views inside this list, which have certain selection criterions. See remarks for details.
	/// </summary>
	/// <typeparam name="T">Type of the items in the list.</typeparam>
	/// <remarks>
	/// Say you have two classes, A and B, both having the base class T. Then you can create a PartionableList, holding items of class A as well as items of class B. 
	/// From this list you then can create a partition, i.e. a view, that shows only items of class A, and another partition, that shows only items of class B.
	/// These partitions support all list operations, including deletion, insertion, movement, and setting of items. These list operations are propagated to
	/// the main list. Of course, it will cause an exception if you try to insert items of class B into the partition of class A (and vice versa). When you set
	/// or insert an item in a partition, that item has to fulfill the selection criterion for the particular partition.
	/// </remarks>
	public partial class PartitionableList<T> : System.Collections.ObjectModel.ObservableCollection<T>
	{
		/// <summary>Contains all partial views that were created for this instance and are still alive.</summary>
		protected LinkedList<WeakReference> _partialViews = new LinkedList<WeakReference>();

		/// <summary>
		/// The partial views that need to notify after modifications that they have changed.
		/// </summary>
		protected HashSet<PartialViewBase> _partialViewsToNotify = new HashSet<PartialViewBase>();

		/// <summary>
		/// Defines an action that is executed before an item is inserted. The 1st argument is the item to insert.
		/// </summary>
		protected Action<T> _actionBeforeInsertion;

		#region Constructors

		public PartitionableList()
		{
		}

		public PartitionableList(Action<T> actionBeforeInsertion)
		{
			_actionBeforeInsertion = actionBeforeInsertion;
		}

		#endregion

		#region Partial view creation

		/// <summary>
		/// Creates a partial view. A partial view is a view of the list of items of the original collection, that fulfill a certain condition.
		/// </summary>
		/// <param name="selectionCriterium">
		/// The selection condition. 
		/// If this function returns <c>true</c> on an item of the original collection, that item is member of the partial view.
		/// Note that during the lifetime of the partition, the behaviour of the selection criterion must not change.
		/// </param>
		/// <returns>List of items of the original collection, that fullfils the selection condition. The returned list is automatically updated, when the original collection changed or when
		/// the user changed the partial view itself.</returns>
		public IObservableList<T> CreatePartialView(Func<T, bool> selectionCriterium)
		{
			var result = new PartialView<T>(this, selectionCriterium);
			_partialViews.AddLast(new WeakReference(result));
			return result;
		}

		/// <summary>
		/// Creates a partial view. A partial view is a view of the list of items of the original collection, that fulfill a certain condition.
		/// </summary>
		/// <param name="selectionCriterium">
		/// The selection condition. 
		/// If this function returns <c>true</c> on an item of the original collection, that item is member of the partial view.
		/// Note that during the lifetime of the partition, the behaviour of the selection criterion must not change.
		/// </param>
		/// <param name="actionBeforeInsertion">Action that is executed on an item before it is inserted.</param>
		/// <returns>List of items of the original collection, that fullfils the selection condition. The returned list is automatically updated, when the original collection changed or when
		/// the user changed the partial view itself.</returns>
		public IObservableList<T> CreatePartialView(Func<T, bool> selectionCriterium, Action<T> actionBeforeInsertion)
		{
			var result = new PartialView<T>(this, selectionCriterium, actionBeforeInsertion);
			_partialViews.AddLast(new WeakReference(result));
			return result;
		}


		/// <summary>
		/// Creates the partial view that consists of all elements in the original collection that are of type M.
		/// </summary>
		/// <typeparam name="M">Type of the elements of the partial view.</typeparam>
		/// <returns>View of all elements of type M of the original collection.</returns>
		public IObservableList<M> CreatePartialViewOfType<M>() where M : T
		{
			var result = new PartialView<M>(this, x => x is M);
			_partialViews.AddLast(new WeakReference(result));
			return result;
		}

		/// <summary>
		/// Creates the partial view that consists of all elements in the original collection that are of type M.
		/// </summary>
		/// <typeparam name="M">Type of the elements of the partial view.</typeparam>
		/// <param name="actionBeforeInsertion">Action that is executed on an item before it is inserted.</param>
		/// <returns>View of all elements of type M of the original collection.</returns>
		public IObservableList<M> CreatePartialViewOfType<M>(Action<M> actionBeforeInsertion) where M : T
		{
			var result = new PartialView<M>(this, x => x is M, actionBeforeInsertion);
			_partialViews.AddLast(new WeakReference(result));
			return result;
		}


		/// <summary>
		/// Creates the partial view that consisist of elements of type M that fullfil a given condition.
		/// </summary>
		/// <typeparam name="M">Type of the elements of the partial view.</typeparam>
		/// <param name="selectionCriterium">The selection criterium.</param>
		/// <returns></returns>
		public IObservableList<M> CreatePartialViewOfType<M>(Func<M, bool> selectionCriterium) where M : T
		{
			var result = new PartialView<M>(this, x => (x is M) && selectionCriterium((M)x));
			_partialViews.AddLast(new WeakReference(result));
			return result;
		}

		/// <summary>
		/// Creates the partial view that consisist of elements of type M that fullfil a given condition.
		/// </summary>
		/// <typeparam name="M">Type of the elements of the partial view.</typeparam>
		/// <param name="selectionCriterium">The selection criterium.</param>
		/// <returns></returns>
		public IObservableList<M> CreatePartialViewOfType<M>(Func<M, bool> selectionCriterium, Action<M> actionBeforeInsertion) where M : T
		{
			var result = new PartialView<M>(this, x => (x is M) && selectionCriterium((M)x), actionBeforeInsertion);
			_partialViews.AddLast(new WeakReference(result));
			return result;
		}

		#endregion



		/// <summary>
		/// Notifies the partial views that have changed after each operation.
		/// </summary>
		protected void NotifyPartialViewsThatHaveChanged()
		{
			foreach (var pv in _partialViewsToNotify)
				pv.OnNotifyCollectionChanged();

			_partialViewsToNotify.Clear();
		}

		#region List change operation overrides

		protected override void ClearItems()
		{
			var node = _partialViews.First;
			while (null != node)
			{
				var pv = node.Value.Target as PartialViewBase;
				if (null != pv)
				{
					if (pv._itemIndex.Count > 0)
						_partialViewsToNotify.Add(pv);

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

			NotifyPartialViewsThatHaveChanged();
		}

		protected override void InsertItem(int index, T item)
		{
			if (null != _actionBeforeInsertion)
				_actionBeforeInsertion(item);

			base.InsertItem(index, item);

			for (var node = _partialViews.First; null != node; node = node.Next)
			{
				var pv = node.Value.Target as PartialViewBase;
				if (null != pv)
				{
					// Note: there is no need to notify the partial view if only the item indices were adjusted
					var itemIndex = pv._itemIndex;
					int i;
					for (i = itemIndex.Count - 1; i >= 0 && itemIndex[i] >= index; --i)
						++itemIndex[i]; // adjust item indices

					if (pv._selectionCriterium(item))
					{
						itemIndex.Insert(i + 1, index);
						_partialViewsToNotify.Add(pv); // but of course when the item is inserted into the partial view, then the partial view must be notified
					}
				}
				else
				{
					var oldNode = node;
					_partialViews.Remove(node);
				}
			}

			NotifyPartialViewsThatHaveChanged();
		}

		protected override void MoveItem(int oldIndex, int newIndex)
		{
			for (var node = _partialViews.First; null != node; node = node.Next)
			{
				var pv = node.Value.Target as PartialViewBase;
				if (null != pv)
				{
					var itemIndex = pv._itemIndex;
					int i, j;
					int oIdx = FindIndexOfItemGreaterThanOrEqualTo(itemIndex, oldIndex);
					bool isItemIncluded = oIdx < itemIndex.Count && itemIndex[oIdx] == oldIndex;

					if (oldIndex < newIndex) // item is moved to higher index
					{
						if (isItemIncluded)
						{
							for (i = oIdx, j = oIdx + 1; j < itemIndex.Count && itemIndex[j] < newIndex; ++i, ++j)
								itemIndex[i] = itemIndex[j] - 1;
							itemIndex[i] = newIndex;
							_partialViewsToNotify.Add(pv);
						}
						else // item is not included - we only need to adjust the item indices
						{
							for (i = oIdx; i < itemIndex.Count && itemIndex[i] < newIndex; ++i)
								--itemIndex[i];
						}
					}
					else // item is moved to lower index
					{
						if (isItemIncluded)
						{
							for (i = oIdx, j = oIdx - 1; j >= 0 && itemIndex[j] >= newIndex; --i, --j)
								itemIndex[i] = itemIndex[j] + 1;
							itemIndex[i] = newIndex;
							_partialViewsToNotify.Add(pv);
						}
						else // item is not included - we only need to adjust the item indices
						{
							for (i = oIdx; i >= 0 && itemIndex[i] >= newIndex; --i)
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

			NotifyPartialViewsThatHaveChanged();
		}

		protected override void RemoveItem(int index)
		{
			for (var node = _partialViews.First; null != node; node = node.Next)
			{
				var pv = node.Value.Target as PartialViewBase;
				if (null != pv)
				{
					// Note: there is no need to notify the partial view if only the item indices were adjusted
					var itemIndex = pv._itemIndex;
					int i;
					for (i = itemIndex.Count - 1; i >= 0 && itemIndex[i] > index; --i)
						--itemIndex[i];

					if (i >= 0 && itemIndex[i] == index)
					{
						itemIndex.RemoveAt(i);
						_partialViewsToNotify.Add(pv); // but of course when the item is deleted into the partial view, then the partial view must be notified
					}
				}
				else
				{
					var oldNode = node;
					_partialViews.Remove(node);
				}
			}

			base.RemoveItem(index);

			NotifyPartialViewsThatHaveChanged();
		}

		protected override void SetItem(int index, T item)
		{
			if (null != _actionBeforeInsertion)
				_actionBeforeInsertion(item);

			base.SetItem(index, item);

			for (var node = _partialViews.First; null != node; node = node.Next)
			{
				var pv = node.Value.Target as PartialViewBase;
				if (null != pv)
				{
					bool isIncluded = pv._selectionCriterium(item);
					var itemIndex = pv._itemIndex;
					int i = FindIndexOfItemGreaterThanOrEqualTo(itemIndex, index);
					if (i < itemIndex.Count)
					{
						if (itemIndex[i] == index)
						{
							if (!isIncluded)
								itemIndex.RemoveAt(i);
						}
						else
						{
							if (isIncluded)
								itemIndex.Insert(i, index);
						}
					}

					if (isIncluded) // if the item fulfills the selection criterion,
						_partialViewsToNotify.Add(pv); // the partial view must be notified in any case
				}
				else
				{
					var oldNode = node;
					_partialViews.Remove(node);
				}
			}
			NotifyPartialViewsThatHaveChanged();
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

			while (lowerValue < value && value < upperValue && lowerIndex < upperIndex)
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

		#endregion List change operation overrides
	}
}