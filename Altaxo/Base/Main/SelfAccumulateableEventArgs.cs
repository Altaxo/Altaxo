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

namespace Altaxo.Main
{
	/// <summary>
	/// Base class for all event args that can accumulate state. They can accumulate even instances of themself, by calling the <see cref="Add"/> function.
	/// Overrides for <see cref="GetHashCode"/> and <see cref="Equals"/> ensure that only a single instance is contained in a HashSet.
	/// </summary>
	public abstract class SelfAccumulateableEventArgs : EventArgs
	{
		/// <summary>
		/// Adds the specified event args e.
		/// </summary>
		/// <param name="e">The <see cref="Altaxo.Main.SelfAccumulateableEventArgs"/> instance containing the event data.</param>
		public abstract void Add(SelfAccumulateableEventArgs e);

		/// <summary>
		/// Override to ensure that only one instance of <see cref="SelfAccumulateableEventArgs"/> is contained in the accumulated event args collection.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			return this.GetType().GetHashCode();
		}

		/// <summary>
		/// Override to ensure that only one instance of <see cref="SelfAccumulateableEventArgs"/> is contained in the accumulated event args collection.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (null == obj)
				return false;
			if (this.GetType() != obj.GetType())
				return false;

			return true;
		}
	}
}