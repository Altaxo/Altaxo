#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.Com
{
	using UnmanagedApi.Ole32;

	/// <summary>
	/// Helper class to easily implement the IEnum Com interface. All members of this interface are included here, but since Com can not handle generic types,
	/// you have to derive an non-generic class from this class (and implement Clone).
	/// </summary>
	/// <typeparam name="T">Type of the items in the enumeration.</typeparam>
	public class ManagedEnumBase<T> : IDisposable
	{
		private IEnumerable<T> _enumeration;
		private IEnumerator<T> _enumerator;

		public ManagedEnumBase(IEnumerable<T> enumeration)
		{
			_enumeration = enumeration;
			_enumerator = enumeration.GetEnumerator();
		}

		public ManagedEnumBase(ManagedEnumBase<T> from)
		{
			CopyFrom(from);
		}

		protected virtual void CopyFrom(ManagedEnumBase<T> from)
		{
			this._enumeration = from._enumeration;
			this._enumerator = this._enumeration.GetEnumerator();
		}

		/// <summary>
		/// Resets the state of enumeration.
		/// </summary>
		/// <returns>S_OK</returns>
		public int Reset()
		{
			_enumerator.Reset();
			return ComReturnValue.S_OK; // S_OK
		}

		/// <summary>
		/// Skips the number of elements requested.
		/// </summary>
		/// <param name="celt">The number of elements to skip.</param>
		/// <returns>If there are not enough remaining elements to skip, returns S_FALSE. Otherwise, S_OK is returned.</returns>
		public int Skip(int celt)
		{
			for (int i = celt - 1; i >= 0; --i)
			{
				if (!_enumerator.MoveNext())
					return ComReturnValue.S_FALSE;
			}
			return ComReturnValue.S_OK; // S_OK
		}

		/// <summary>
		/// Retrieves the next elements from the enumeration.
		/// </summary>
		/// <param name="celt">The number of elements to retrieve.</param>
		/// <param name="rgelt">An array to receive the formats requested.</param>
		/// <param name="pceltFetched">An array to receive the number of element fetched.</param>
		/// <returns>If the fetched number of formats is the same as the requested number, S_OK is returned.
		/// There are several reasons S_FALSE may be returned: (1) The requested number of elements is less than
		/// or equal to zero. (2) The rgelt parameter equals null. (3) There are no more elements to enumerate.
		/// (4) The requested number of elements is greater than one and pceltFetched equals null or does not
		/// have at least one element in it. (5) The number of fetched elements is less than the number of
		/// requested elements.</returns>
		public int Next(int celt, T[] rgelt, int[] pceltFetched)
		{
			// Start with zero fetched, in case we return early
			if (pceltFetched != null && pceltFetched.Length > 0)
				pceltFetched[0] = 0;

			// Short circuit if they didn't request any elements, or didn't
			// provide room in the return array, or there are not more elements
			// to enumerate.
			if (celt <= 0 || rgelt == null || _enumerator == null)
				return ComReturnValue.S_FALSE; // S_FALSE

			// If the number of requested elements is not one, then we must
			// be able to tell the caller how many elements were fetched.
			if ((pceltFetched == null || pceltFetched.Length < 1) && celt != 1)
				return ComReturnValue.S_FALSE; // S_FALSE

			// If the number of elements in the return array is too small, we
			// throw. This is not a likely scenario, hence the exception.
			if (rgelt.Length < celt)
				throw new ArgumentException("The number of elements in the return array is less than the number of elements requested");

			// Fetch the elements.
			int fetchedCount = 0;
			while (fetchedCount < celt && _enumerator.MoveNext())
			{
				rgelt[fetchedCount++] = _enumerator.Current;
			}

			// Return the number of elements fetched
			if (pceltFetched != null && pceltFetched.Length > 0)
				pceltFetched[0] = fetchedCount;

			return (fetchedCount == celt) ? ComReturnValue.S_OK : ComReturnValue.S_FALSE; // S_OK : S_FALSE
		}

		public void Dispose()
		{
			if (null != _enumerator)
			{
				_enumerator.Dispose();
				_enumerator = null;
			}
			_enumeration = null;
		}
	}
}