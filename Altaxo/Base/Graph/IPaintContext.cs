#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

namespace Altaxo.Graph
{
	/// <summary>
	/// Interface to a paint context that must be used by the objects to be painted to store temporary object valid only during painting.
	/// </summary>
	public interface IPaintContext
	{
		/// <summary>
		/// Adds a specified object under a specified key.
		/// </summary>
		/// <param name="key">The key (usually the owner of the value).</param>
		/// <param name="value">The value.</param>
		void AddValue(object key, object value);

		/// <summary>
		/// Gets an object stored under a specified key (usually the owner).
		/// </summary>
		/// <typeparam name="T">Type of the value.</typeparam>
		/// <param name="key">The key.</param>
		/// <returns>The value. An exception will be thrown if the specified key does not exist or a value with another type than the specified type is stored under the key.</returns>
		T GetValue<T>(object key);

		/// <summary>
		/// Gets an object stored under a specified key (usually the owner). If the value is not available, the default value of the specified type is returned.
		/// </summary>
		/// <typeparam name="T">Type of the value.</typeparam>
		/// <param name="key">The key.</param>
		/// <returns>The value, or the default value. An exception will be thrown if the specified key does not exist or a value with another type than the specified type is stored under the key.</returns>
		T GetValueOrDefault<T>(object key);

		void PushHierarchicalValue<T>(string name, T value);

		T PopHierarchicalValue<T>(string name);

		T GetHierarchicalValue<T>(string name);
	}
}