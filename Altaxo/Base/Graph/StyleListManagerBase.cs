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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	/// <summary>
	/// Implements a basic manager for style lists.
	/// </summary>
	/// <typeparam name="TList">Type of the list of style items.</typeparam>
	/// <typeparam name="T">Type of the style item in the lists.</typeparam>
	/// <seealso cref="Altaxo.Graph.IStyleListManager{TList, T}" />
	public abstract class StyleListManagerBase<TList, T> : IStyleListManager<TList, T> where TList : IStyleList<T> where T : Main.IImmutable
	{
		/// <summary>
		/// Dictionary of all existing lists. Key is the list name. Value is a tuple, whose boolean entry designates whether this is
		/// a buildin or user list (false) or a project list (true).
		/// </summary>
		private Dictionary<string, Tuple<Main.ItemDefinitionLevel, TList>> _allLists = new Dictionary<string, Tuple<Main.ItemDefinitionLevel, TList>>();

		public TList BuiltinDefault { get; private set; }

		protected StyleListManagerBase(TList builtinDefaultList)
		{
			BuiltinDefault = builtinDefaultList;
			_allLists.Add(BuiltinDefault.Name, new Tuple<Main.ItemDefinitionLevel, TList>(Main.ItemDefinitionLevel.Builtin, BuiltinDefault));
		}

		public IEnumerable<string> GetAllListNames()
		{
			return _allLists.Keys;
		}

		public TList GetList(string name)
		{
			return _allLists[name].Item2;
		}

		/// <summary>
		/// Called when the current project is closed. Removes all those list which are project lists.
		/// </summary>
		protected virtual void EhProjectClosed(object sender, Main.ProjectEventArgs e)
		{
			var namesToRemove = new List<string>(_allLists.Where(entry => entry.Value.Item1 == Main.ItemDefinitionLevel.Project).Select(entry => entry.Key));
			foreach (var name in namesToRemove)
				_allLists.Remove(name);
		}

		/// <summary>
		/// Try to register the provided list.
		/// </summary>
		/// <param name="level">The level on which this list is defined.</param>
		/// <param name="instance">The new list which is tried to register.</param>
		/// <param name="storedList">On return, this is the list which is either registered, or is an already registed list with exactly the same elements.</param>
		/// <returns>True if the list was new and thus was added to the collection; false if the list has already existed.</returns>
		public bool TryRegisterList(Main.ItemDefinitionLevel level, TList instance, out TList storedList)
		{
			string nameOfExistingGroup;
			if (TryGetListByMembers(instance, out nameOfExistingGroup)) // if a group with such a list already exist
			{
				if (nameOfExistingGroup != instance.Name) // if it has the same list, but a different name, do nothing at all
				{
					storedList = _allLists[nameOfExistingGroup].Item2;
					return false;
				}
				else // if it has the same list, and the same name, even better, nothing is left to be done
				{
					storedList = _allLists[nameOfExistingGroup].Item2;
					return false;
				}
			}
			else // a group with such members don't exist currently
			{
				if (_allLists.ContainsKey(instance.Name)) // but name is already in use
				{
					storedList = (TList)instance.WithName(GetUnusedName(instance.Name));
					_allLists.Add(storedList.Name, new Tuple<Main.ItemDefinitionLevel, TList>(level, storedList));
					return true;
				}
				else // name is not in use
				{
					storedList = instance;
					_allLists.Add(instance.Name, new Tuple<Main.ItemDefinitionLevel, TList>(level, instance));
					return true;
				}
			}
		}

		protected virtual string GetUnusedName(string usedName)
		{
			int i;
			for (i = usedName.Length - 1; i >= 0; --i)
			{
				if (!char.IsDigit(usedName[i]))
					break;
			}

			int numberOfDigits = usedName.Length - (i + 1);

			if (0 == numberOfDigits)
			{
				return usedName + "0";
			}
			else
			{
				int number = int.Parse(usedName.Substring(i + 1), System.Globalization.NumberStyles.Any);
				string formatString = "N" + numberOfDigits.ToString(System.Globalization.CultureInfo.InvariantCulture);
				return usedName.Substring(0, i + 1) + (number + 1).ToString(formatString, System.Globalization.CultureInfo.InvariantCulture);
			}
		}

		public bool TryGetListByMembers(IEnumerable<T> symbols, out string nameOfExistingList)
		{
			foreach (var entry in _allLists)
			{
				if (entry.Value.Item2.IsStructuralEquivalentTo(symbols))
				{
					nameOfExistingList = entry.Key;
					return true;
				}
			}
			nameOfExistingList = null;
			return false;
		}

		public bool ContainsList(string name)
		{
			return _allLists.ContainsKey(name);
		}

		public abstract TList CreateNewList(string name, IEnumerable<T> symbols, bool registerNewList, ItemDefinitionLevel level);
	}
}