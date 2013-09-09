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
	/// <summary>
	/// Supports movement of selected items up/down in a list.
	/// </summary>
	public static class ListExtensions
	{
		/// <summary>
		/// Exchange the positions of two items in a list.
		/// </summary>
		/// <typeparam name="T">Type of the list items.</typeparam>
		/// <param name="list">List to operate with.</param>
		/// <param name="i">Position of the first item.</param>
		/// <param name="j">Position of the second item.</param>
		static public void ExchangePositions<T>(this IList<T> list, int i, int j)
		{
			if (i == j)
				return;
			if (i < 0)
				throw new ArgumentException("i<0");
			if (j < 0)
				throw new ArgumentException("j<0");
			if (i >= list.Count)
				throw new ArgumentException("i>=Count");
			if (j >= list.Count)
				throw new ArgumentException("j>=Count");

			var item_i = list[i];
			list[i] = list[j];
			list[j] = item_i;
		}

		/// <summary>
		/// Moves a item to another list position. All items inbetween the interval <paramref name="originalIndex"/> and <paramref name="destinationIndex"/> will slip by one position (except the item at <paramref name="originalIndex"/>,
		/// which will of course move to <paramref name="destinationIndex"/>.
		/// </summary>
		/// <typeparam name="T">Type of the list items.</typeparam>
		/// <param name="list">List to operate with.</param>
		/// <param name="originalIndex">Original position of the  item.</param>
		/// <param name="destinationIndex">Destination position of the item.</param>
		static public void MoveItemToIndex<T>(this IList<T> list, int originalIndex, int destinationIndex)
		{
			if (originalIndex == destinationIndex)
				return;
			if (originalIndex < 0)
				throw new ArgumentException("originalIndex<0");
			if (destinationIndex < 0)
				throw new ArgumentException("DestinationIndex<0");
			if (originalIndex >= list.Count)
				throw new ArgumentException("originalIndex>=Count");
			if (destinationIndex >= list.Count)
				throw new ArgumentException("destinationIndex>=Count");

			var oc = list as System.Collections.ObjectModel.ObservableCollection<T>; // special case list is an observable collection, which has an own Move operation
			if (null != oc)
			{
				oc.Move(originalIndex, destinationIndex);
				return;
			}


			var item_i = list[originalIndex];

			if (destinationIndex > originalIndex)
			{
				for (int k = originalIndex; k < destinationIndex; ++k)
					list[k] = list[k + 1];
			}
			else
			{
				for (int k = originalIndex; k > destinationIndex; --k)
					list[k] = list[k - 1];
			}

			list[destinationIndex] = item_i;
		}



		/// <summary>
		/// Moves the selected items towards higher indices (for steps &gt; 0) or lower indices (for steps &lt; 0).
		/// </summary>
		/// <typeparam name="T">Type of list item.</typeparam>
		/// <param name="list">List to operate with.</param>
		/// <param name="IsSelected">Function that determines for each item index if it is selected or not.</param>
		/// <param name="steps">Number of steps to move. A positive value moves the items towards higher indices, a negative value towards lower indices.</param>
		public static void MoveSelectedItems<T>(this IList<T> list, Func<int, bool> IsSelected, int steps)
		{
			if (steps < 0)
				MoveSelectedItemsTowardsLowerIndices(list, IsSelected, -steps);
			else
				MoveSelectedItemsTowardsHigherIndices(list, IsSelected, steps);
		}

		/// <summary>
		/// Return the number of steps that selected items can be moved towards lower indices. The selected item with the lowest index determines that value.
		/// </summary>
		/// <typeparam name="T">Type of list item.</typeparam>
		/// <param name="list">List to operate onto.</param>
		/// <param name="IsSelected">Function that determines for each item index if it is selected or not.</param>
		/// <returns>The number of steps that all selected items can be moved towards lower indices, so that the selected item with the lowest index is moved to index 0.</returns>
		public static int GetPossibleStepsToMoveTowardsLowerIndices<T>(this IList<T> list, Func<int, bool> IsSelected)
		{
			int first = -1;
			// find out the first index that is selected
			for (int i = 0; i < list.Count; ++i)
			{
				if (IsSelected(i))
				{
					first = i;
					break;
				}
			}

			if (first < 0)
				return 0;
			else
				return first;
		}

		/// <summary>
		/// Return the number of steps that selected items can be moved towards higher indices. The selected item with the highest index determines that value.
		/// </summary>
		/// <typeparam name="T">Type of list item.</typeparam>
		/// <param name="list">List to operate onto.</param>
		/// <param name="IsSelected">Function that determines for each item index if it is selected or not.</param>
		/// <returns>The number of steps that all selected items can be moved towards higher indices, so that the selected item with the highest index is moved to the end of the list (at index list.Count-1).</returns>
		public static int GetPossibleStepsToMoveTowardsHigherIndices<T>(this IList<T> list, Func<int, bool> IsSelected)
		{
			int last = -1;
			// find out the last index that is selected
			for (int i = list.Count - 1; i >= 0; --i)
			{
				if (IsSelected(i))
				{
					last = i;
					break;
				}
			}

			if (last < 0)
				return 0;
			else
				return list.Count - 1 - last;

		}

		/// <summary>
		/// Moves the selected item so that the selected item with the formerly lowest index is afterwards at the start of the list (at index 0).
		/// </summary>
		/// <typeparam name="T">Type of list item.</typeparam>
		/// <param name="list">List to operate with.</param>
		/// <param name="IsSelected">Function that determines for each item index if it is selected or not.</param>
		public static void MoveSelectedItemsToMinimumIndex<T>(this IList<T> list, Func<int, bool> IsSelected)
		{
			int steps = GetPossibleStepsToMoveTowardsLowerIndices(list, IsSelected);
			MoveSelectedItemsTowardsLowerIndices(list, IsSelected, steps);
		}

		/// <summary>
		/// Moves the selected item so that the selected item with the formerly highest index is afterwards at the end of the list (at index list.Count-1).
		/// </summary>
		/// <typeparam name="T">Type of list item.</typeparam>
		/// <param name="list">List to operate with.</param>
		/// <param name="IsSelected">Function that determines for each item index if it is selected or not.</param>
		public static void MoveSelectedItemsToMaximumIndex<T>(this IList<T> list, Func<int, bool> IsSelected)
		{
			int steps = GetPossibleStepsToMoveTowardsHigherIndices(list, IsSelected);
			MoveSelectedItemsTowardsHigherIndices(list, IsSelected, steps);
		}

		/// <summary>
		/// Moves the selected items towards lower indices.
		/// </summary>
		/// <typeparam name="T">Type of list item.</typeparam>
		/// <param name="list">List to operate with.</param>
		/// <param name="isSelected">Function that determines for each item index if it is selected or not.</param>
		/// <param name="steps">Number of steps to move. Has to be a positive value.</param>
		public static void MoveSelectedItemsTowardsLowerIndices<T>(this IList<T> list, Func<int, bool> isSelected, int steps)
		{
			if (steps < 0)
				throw new ArgumentOutOfRangeException("steps have to be greater or equal than zero");

			if (0 == steps || list.Count == 0)
				return;

			for (int i = 0; i < steps; ++i)
			{
				if (isSelected(i))
					return;
			}

			for (int i = steps; i < list.Count; i++)
			{
				if (isSelected(i))
					MoveItemToIndex(list, i, i - steps);
			}
		}

		/// <summary>
		/// Moves the selected items towards higher indices.
		/// </summary>
		/// <typeparam name="T">Type of list item.</typeparam>
		/// <param name="list">List to operate with.</param>
		/// <param name="isSelected">Function that determines for each item index if it is selected or not.</param>
		/// <param name="steps">Number of steps to move. Has to be a positive value.</param>

		public static void MoveSelectedItemsTowardsHigherIndices<T>(this IList<T> list, Func<int, bool> isSelected, int steps)
		{
			if (steps < 0)
				throw new ArgumentOutOfRangeException("steps have to be greater or equal than zero");

			if (0 == steps || list.Count == 0)
				return;

			for (int i = list.Count - 1; i >= list.Count - steps; --i)
			{
				if (isSelected(i))
					return;
			}

			for (int i = list.Count - 1 - steps; i >= 0; --i)
			{
				if (isSelected(i))
					MoveItemToIndex(list, i, i + steps);
			}

		}

		/// <summary>
		/// Gets the index the of first item in <paramref name="list"/> that fulfills the predicate <paramref name="predicate"/>
		/// </summary>
		/// <typeparam name="T">Type of items in the list</typeparam>
		/// <param name="list">The list.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns>Index the of first item in <paramref name="list"/> that fulfills the predicate, or a value of -1 if no such item could be found.</returns>
		public static int IndexOfFirst<T>(this IList<T> list, Func<T, bool> predicate)
		{
			for (int i = 0; i < list.Count; ++i)
				if (predicate(list[i]))
					return i;
			return -1;
		}

	}
}
