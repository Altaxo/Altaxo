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
#endregion

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
		/// <param name="org">The orgiginal enumeration.</param>
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
	}
}
