#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.Collections
{
	/// <summary>
	/// Extensions to the IEnumerable interface.
	/// </summary>
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Takes all elements of the enumeration except the last element.
		/// </summary>
		/// <typeparam name="T">Type of the elements of the enumeration</typeparam>
		/// <param name="org">The original enumeration.</param>
		/// <returns>An enumeration that has all elements of the original enumeration, except the last one.</returns>
		/// <exception cref="System.ArgumentNullException">The original enumeration was <c>null</c>.</exception>
		public static IEnumerable<T> TakeAllButLast<T>(this IEnumerable<T> org)
		{
			if (null == org)
				throw new ArgumentNullException("org");

			using (var it = org.GetEnumerator())
			{
				if (it.MoveNext())
				{
					var p = it.Current;
					while (it.MoveNext())
					{
						yield return p;
						p = it.Current;
					}
				}
			}
		}

		/// <summary>
		/// Determines whether the specified enumeration is empty.
		/// </summary>
		/// <typeparam name="T">Type of the elements of the enumeration.</typeparam>
		/// <param name="org">The enumeration to test.</param>
		/// <returns>
		///   <c>true</c> if the specified org is empty; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">The enumeration to test is <c>null</c>.</exception>
		public static bool IsEmpty<T>(this IEnumerable<T> org)
		{
			if (null == org)
				throw new ArgumentNullException("org");

			bool result;
			using (var it = org.GetEnumerator())
			{
				result = !it.MoveNext();
			}
			return result;
		}

		/// <summary>
		/// Gets the element of a IEnumerabe that evaluates by means of a conversion function to the maximal value.
		/// This is different from Select(x => conversion(x)).Max() insofar as it not returns the maximum value, but the original element x which converts to the maximum value.
		/// </summary>
		/// <typeparam name="T">Type of the elements of the enumeration.</typeparam>
		/// <typeparam name="M">Type of the value that is used to compare the elements of the sequence.</typeparam>
		/// <param name="org">The enumeration to consider.</param>
		/// <param name="conversion">Conversion function that converts each element (type: <typeparamref name="T"/>) of the sequence to a value (of type <typeparamref name="M"/> that is comparable.</param>
		/// <returns>This element of the sequence, which by the provided conversion function evaluates to the maximum value.</returns>
		/// <exception cref="System.InvalidOperationException">The provided enumeration is empty. Thus it is not possible to evaluate the maximum.</exception>
		public static T MaxElement<T, M>(this IEnumerable<T> org, Func<T, M> conversion) where M : IComparable<M>
		{
			using (var en = org.GetEnumerator())
			{
				if (!en.MoveNext())
					throw new InvalidOperationException("The provided enumeration is empty. Thus it is not possible to evaluate the maximum.");
				var maxEle = en.Current;
				var max = conversion(maxEle);

				while (en.MoveNext())
				{
					var ot = conversion(en.Current);
					if (max.CompareTo(ot) < 0)
					{
						maxEle = en.Current;
						max = ot;
					}
				}

				return maxEle;
			}
		}

		/// <summary>
		/// Evaluates the minimum of a enumeration of elements, or returns a default value if the series is empty.
		/// </summary>
		/// <typeparam name="T">Type of element</typeparam>
		/// <param name="seq">The enumeration.</param>
		/// <param name="defaultValue">The default value that is returned if the enumeration is empty.</param>
		/// <returns>The minimum of of all elements, or the <paramref name="defaultValue"/> if the series is empty.</returns>
		public static T MinOrDefault<T>(this IEnumerable<T> seq, T defaultValue) where T : IComparable<T>
		{
			using (var en = seq.GetEnumerator())
			{
				if (!en.MoveNext())
					return defaultValue;

				var min = en.Current;
				while (en.MoveNext())
				{
					if (min.CompareTo(en.Current) > 0)
					{
						min = en.Current;
					}
				}
				return min;
			}
		}

		/// <summary>
		/// Evaluates the maximum of a enumeration of elements, or returns a default value if the series is empty.
		/// </summary>
		/// <typeparam name="T">Type of element</typeparam>
		/// <param name="seq">The enumeration.</param>
		/// <param name="defaultValue">The default value that is returned if the enumeration is empty.</param>
		/// <returns>The maximum of of all elements, or the <paramref name="defaultValue"/> if the series is empty.</returns>
		public static T MaxOrDefault<T>(this IEnumerable<T> seq, T defaultValue) where T : IComparable<T>
		{
			using (var en = seq.GetEnumerator())
			{
				if (!en.MoveNext())
					return defaultValue;

				var max = en.Current;
				while (en.MoveNext())
				{
					if (max.CompareTo(en.Current) < 0)
					{
						max = en.Current;
					}
				}
				return max;
			}
		}

		/// <summary>
		/// Evaluates the maximum of a enumeration of elements, or returns a default value if the series is empty.
		/// </summary>
		/// <typeparam name="T">Type of element</typeparam>
		/// <typeparam name="M">Type of comparison value that results from the element.</typeparam>
		/// <param name="seq">The enumeration of elements.</param>
		/// <param name="conversion">Conversion function which converts the type <typeparamref name="T"/> of the original sequence into a type <typeparamref name="M"/> which is used to determine the maximum value.</param>
		/// <param name="defaultValue">The default value that is returned if the enumeration is empty.</param>
		/// <returns>The maximum of of all comparison values of the elements, or the <paramref name="defaultValue"/> if the series is empty.</returns>
		public static M MaxOrDefault<T, M>(this IEnumerable<T> seq, Func<T, M> conversion, M defaultValue) where M : IComparable<M>
		{
			using (var en = seq.GetEnumerator())
			{
				if (!en.MoveNext())
					return defaultValue;

				var max = conversion(en.Current);
				while (en.MoveNext())
				{
					var curVal = conversion(en.Current);
					if (max.CompareTo(curVal) < 0)
					{
						max = curVal;
					}
				}
				return max;
			}
		}

		/// <summary>
		/// Executes an action for each element of the sequence.
		/// </summary>
		/// <typeparam name="T">Type of element.</typeparam>
		/// <param name="seq">The element sequence.</param>
		/// <param name="action">The action to execute for each element.</param>
		public static void ForEachDo<T>(this IEnumerable<T> seq, Action<T> action)
		{
			foreach (var element in seq)
				action(element);
		}

		/// <summary>
		/// Takes all elements of a list, starting from index <paramref name="upperIndexInclusive"/> down to the index <paramref name="lowerIndexInclusive"/>.
		/// </summary>
		/// <typeparam name="T">Type of list elements</typeparam>
		/// <param name="list">The list of elements.</param>
		/// <param name="upperIndexInclusive">The upper index (inclusive).</param>
		/// <param name="lowerIndexInclusive">The lower index (inclusive).</param>
		/// <returns></returns>
		public static IEnumerable<T> TakeFromUpperIndexInclusiveDownToLowerIndexInclusive<T>(this IList<T> list, int upperIndexInclusive, int lowerIndexInclusive)
		{
			if (null == list)
				throw new ArgumentNullException("list");
			if (!(upperIndexInclusive >= lowerIndexInclusive))
				throw new ArgumentException("upperIndexInclusive should be >= lowerIndexInclusive");

			for (int i = upperIndexInclusive; i >= lowerIndexInclusive; --i)
				yield return list[i];
		}

		/// <summary>
		/// Takes all elements of a list, starting from index <paramref name="upperIndexExclusive"/> - 1 down to the index <paramref name="lowerIndexInclusive"/>.
		/// </summary>
		/// <typeparam name="T">Type of list elements</typeparam>
		/// <param name="list">The list of elements.</param>
		/// <param name="upperIndexExclusive">The upper index (exclusive).</param>
		/// <param name="lowerIndexInclusive">The lower index (inclusive).</param>
		/// <returns>All elements of a list, starting from index <paramref name="upperIndexExclusive"/> - 1 down to the index <paramref name="lowerIndexInclusive"/>.</returns>
		public static IEnumerable<T> TakeFromUpperIndexExclusiveDownToLowerIndexInclusive<T>(this IList<T> list, int upperIndexExclusive, int lowerIndexInclusive)
		{
			if (null == list)
				throw new ArgumentNullException(nameof(list));
			if (lowerIndexInclusive < 0)
				throw new ArgumentOutOfRangeException(nameof(lowerIndexInclusive) + " should be >= 0");
			if (!(upperIndexExclusive >= lowerIndexInclusive))
				throw new ArgumentException(nameof(upperIndexExclusive) + " should be >= " + nameof(lowerIndexInclusive));

			for (int i = upperIndexExclusive - 1; i >= lowerIndexInclusive; --i)
				yield return list[i];
		}
	}
}