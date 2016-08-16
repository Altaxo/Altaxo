#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
	/// Manager for style lists. Usually only a single instance of this manager should exist in the application.
	/// </summary>
	/// <typeparam name="TList">The type of the style list.</typeparam>
	/// <typeparam name="T">The type of the style</typeparam>
	public interface IStyleListManager<TList, T> where TList : IStyleList<T> where T : Main.IImmutable
	{
		/// <summary>
		/// Gets the names of all entries (styles) in the list.
		/// </summary>
		/// <returns>The names of all entries (styles) in the list.</returns>
		IEnumerable<string> GetAllListNames();

		/// <summary>
		/// Gets all lists together with their definition level.
		/// </summary>
		/// <returns>The lists together with their definition level.</returns>
		IEnumerable<Tuple<TList, Main.ItemDefinitionLevel>> GetListsWithLevel();

		/// <summary>
		/// Determines whether the specified name contains a list (style) with the provided name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>
		///   <c>true</c> if the specified name contains a list (style) with the provided name; otherwise, <c>false</c>.
		/// </returns>
		bool ContainsList(string name);

		/// <summary>
		/// Gets the list (style) with the provided name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The list (style) with the provided name.</returns>
		TList GetList(string name);

		/// <summary>
		/// Gets the list (style) with the provided name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="level">Returns the definition level of the list.</param>
		/// <returns>The list (style) with the provided name.</returns>
		TList GetList(string name, out Main.ItemDefinitionLevel level);

		bool TryGetListByMembers(IEnumerable<T> symbols, out string nameOfExistingList);

		/// <summary>
		/// Try to register the provided list.
		/// </summary>
		/// <param name="level">The level on which this list is defined.</param>
		/// <param name="instance">The new list which is tried to register.</param>
		/// <param name="storedList">On return, this is the list which is either registered, or is an already registed list with exactly the same elements.</param>
		/// <returns>True if the list was new and thus was added to the collection; false if the list has already existed.</returns>
		bool TryRegisterList(Main.ItemDefinitionLevel level, TList instance, out TList storedList);

		TList CreateNewList(string name, IEnumerable<T> symbols, bool registerNewList, Main.ItemDefinitionLevel level);
	}
}