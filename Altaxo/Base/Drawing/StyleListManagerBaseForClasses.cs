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

using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Altaxo.Drawing
{
	/// <summary>
	/// Base class for style lists whose items are immutable class instances.
	/// </summary>
	/// <typeparam name="TList">The type of the list.</typeparam>
	/// <typeparam name="TItem">The type of the list item.</typeparam>
	/// <typeparam name="TListManagerEntry">The type of the list manager entry.</typeparam>
	/// <seealso cref="Altaxo.Drawing.StyleListManagerBase{TList, T, TListManagerEntry}" />
	public abstract class StyleListManagerBaseForClasses<TList, TItem, TListManagerEntry> : StyleListManagerBase<TList, TItem, TListManagerEntry>
		where TList : IStyleList<TItem>
		where TItem : class, Main.IImmutable
		where TListManagerEntry : StyleListManagerBaseEntryValue<TList, TItem>
	{
		private Dictionary<TItem, TList> _dictListEntryToList = new Dictionary<TItem, TList>();

		private class ReferenceEqualityComparer : IEqualityComparer<TItem>
		{
			public bool Equals(TItem x, TItem y)
			{
				return object.ReferenceEquals(x, y);
			}

			public int GetHashCode(TItem obj)
			{
				return RuntimeHelpers.GetHashCode(obj);
			}
		}

		public StyleListManagerBaseForClasses(Func<TList, ItemDefinitionLevel, TListManagerEntry> valueCreator, TList builtinDefaultList)
			:
			base(valueCreator, builtinDefaultList)
		{
			RebuildListEntryToListDictionary();
		}

		private void RebuildListEntryToListDictionary()
		{
			var dictListEntryToList = new Dictionary<TItem, TList>(new ReferenceEqualityComparer());
			// all currently present to the dictionary
			foreach (var entry in _allLists.Values)
				foreach (var instance in entry.List)
					dictListEntryToList.Add(instance, entry.List);

			_dictListEntryToList = dictListEntryToList;
		}

		protected override void OnListAdded(TList list, ItemDefinitionLevel level)
		{
			RebuildListEntryToListDictionary();
			base.OnListAdded(list, level);
		}

		protected override void OnListChanged(TList list, ItemDefinitionLevel level)
		{
			RebuildListEntryToListDictionary();
			base.OnListChanged(list, level);
		}

		/// <summary>
		/// Gets the parent list of an item, or null if no parent list is found.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>The parent list of an item, or null if no parent list is found.</returns>
		public override TList GetParentList(TItem item)
		{
			if (null == item)
				return default(TList);

			TList result;
			if (_dictListEntryToList.TryGetValue(item, out result))
				return result;
			else
				return default(TList);
		}
	}
}