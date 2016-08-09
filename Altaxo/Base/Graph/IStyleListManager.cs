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
		IEnumerable<string> GetAllListNames();

		TList GetList(string name);

		bool ContainsList(string name);

		/// <summary>
		/// Try to register the provided list.
		/// </summary>
		/// <param name="level">The level on which this list is defined.</param>
		/// <param name="instance">The new list which is tried to register.</param>
		/// <param name="storedList">On return, this is the list which is either registered, or is an already registed list with exactly the same elements.</param>
		/// <returns>True if the list was new and thus was added to the collection; false if the list has already existed.</returns>
		bool TryRegisterList(Main.ItemDefinitionLevel level, TList instance, out TList storedList);

		bool TryGetListByMembers(IEnumerable<T> symbols, out string nameOfExistingList);

		TList CreateNewList(string name, IEnumerable<T> symbols, bool registerNewList, Main.ItemDefinitionLevel level);
	}
}