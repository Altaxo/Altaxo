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

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo
{
	/// <summary>
	/// Helps to copy instances, preferably by using <see cref="Altaxo.Main.ICopyFrom"/>, or by using <see cref="ICloneable"/> interface.
	/// </summary>
	public static class CopyHelper
	{
		/// <summary>Copies an instance of an immutable class. This can be done simply by assigning <paramref name="from"/> to <paramref name="to"/>.</summary>
		/// <typeparam name="T">The type of the instance to copy.</typeparam>
		/// <param name="to">The variable to copy to.</param>
		/// <param name="from">The instance that was copied.</param>
		public static void CopyImmutable<T>(ref T to, T from) where T : Main.IImmutable
		{
			to = from;
		}

		/// <summary>Copies an instance.</summary>
		/// <typeparam name="T">The type of the instance to copy.</typeparam>
		/// <param name="to">The variable to copy to.</param>
		/// <param name="from">The instance that was copied.</param>
		public static void Copy<T>(ref T to, T from) where T : ICloneable
		{
			Main.ICopyFrom toc;

			if (object.ReferenceEquals(to, from))
			{
			}
			else if (from == null)
			{
				to = default(T);
			}
			else if (to == null)
			{
				to = (T)from.Clone();
			}
			else if (null != (toc = (to as Main.ICopyFrom)) && to.GetType() == from.GetType())
			{
				toc.CopyFrom(from);
			}
			else
			{
				to = (T)from.Clone();
			}
		}

		/// <summary>Gets a copy of an instance, either by using <see cref="Altaxo.Main.ICopyFrom"/> or <see cref="ICloneable"/> interface.</summary>
		/// <typeparam name="T">The type of the instance to copy.</typeparam>
		/// <param name="to">The value of the variable to copy to.</param>
		/// <param name="from">The instance to copy from.</param>
		/// <returns>The copied instance. It might be the same instance as provided in <paramref name="to"/>, if the interface <see cref="Altaxo.Main.ICopyFrom"/> was used for copying.
		/// If the <see cref="ICloneable"/> interface was used for copying, the returned instance is different from <paramref name="to"/>.</returns>
		public static T GetCopy<T>(T to, T from) where T : ICloneable
		{
			Copy(ref to, from);
			return to;
		}

		/// <summary>
		/// Gets the members of the input enumeration cloned as output enumeration.
		/// </summary>
		/// <typeparam name="T">Type of the enumeration members.</typeparam>
		/// <param name="toClone">Input enumeration.</param>
		/// <returns>Output enumeration with cloned members of the input enumeration.</returns>
		public static IEnumerable<T> GetEnumerationMembersCloned<T>(IEnumerable<T> toClone) where T : ICloneable
		{
			foreach (var e in toClone)
			{
				if (null == e)
					yield return default(T);
				else
					yield return (T)e.Clone();
			}
		}
	}
}