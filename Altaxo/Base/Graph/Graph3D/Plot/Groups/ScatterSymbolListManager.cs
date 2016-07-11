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

using Altaxo.Graph.Graph3D.Plot.Styles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Plot.Groups
{
	public class ScatterSymbolListManager
	{
		private static ScatterSymbolListManager _instance = new ScatterSymbolListManager();

		public ScatterSymbolList BuiltinDefault { get; private set; }

		/// <summary>
		/// Dictionary of all existing lists. Key is the list name. Value is a tuple, whose boolean entry designates whether this is
		/// a buildin or user list (false) or a project list (true).
		/// </summary>
		private Dictionary<string, Tuple<bool, ScatterSymbolList>> _allLists = new Dictionary<string, Tuple<bool, ScatterSymbolList>>();

		public static ScatterSymbolListManager Instance
		{
			get
			{
				return _instance;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));
				_instance = value;
			}
		}

		protected ScatterSymbolListManager()
		{
			Current.ProjectService.ProjectClosed += EhProjectClosed;

			var defaultList = new ScatterSymbolList("BuiltinDefault", new IScatterSymbol[] {
				new Styles.ScatterSymbols.Cube(),
					new Styles.ScatterSymbols.Sphere(),
					new Styles.ScatterSymbols.TetrahedronUp(),
					new Styles.ScatterSymbols.TetrahedronDown()
			});

			BuiltinDefault = defaultList;

			_allLists.Add(defaultList.Name, new Tuple<bool, ScatterSymbolList>(false, defaultList));
		}

		public IEnumerable<string> GetAllListNames()
		{
			return _allLists.Keys;
		}

		public ScatterSymbolList GetList(string name)
		{
			return _allLists[name].Item2;
		}

		/// <summary>
		/// Called when the current project is closed. Removes all those list which are project lists.
		/// </summary>
		private void EhProjectClosed(object sender, Main.ProjectEventArgs e)
		{
			var namesToRemove = new List<string>(_allLists.Where(entry => entry.Value.Item1 == true).Select(entry => entry.Key));
			foreach (var name in namesToRemove)
				_allLists.Remove(name);
		}

		/// <summary>
		/// Try to register the provided list.
		/// </summary>
		/// <param name="instance">The new list which is tried to register.</param>
		/// <param name="storedList">On return, this is the list which is either registered, or is an already registed list with exactly the same elements.</param>
		/// <returns>True if the list was new and thus was added to the collection; false if the list has already existed.</returns>
		public bool TryRegisterInstance(ScatterSymbolList instance, out ScatterSymbolList storedList)
		{
			string nameOfExistingGroup;
			if (TryGetGroupByMembers(instance.Items, out nameOfExistingGroup)) // if a group with such a list already exist
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
					storedList = new ScatterSymbolList(GetUnusedName(instance.Name), instance.Items);
					_allLists.Add(storedList.Name, new Tuple<bool, ScatterSymbolList>(false, storedList));
					return true;
				}
				else // name is not in use
				{
					storedList = instance;
					_allLists.Add(instance.Name, new Tuple<bool, ScatterSymbolList>(false, instance));
					return true;
				}
			}
		}

		private string GetUnusedName(string usedName)
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

		public bool TryGetGroupByMembers(IEnumerable<IScatterSymbol> symbols, out string nameOfExistingGroup)
		{
			foreach (var entry in _allLists)
			{
				if (ScatterSymbolList.AreListsStructuralEquivalent(symbols, entry.Value.Item2.Items))
				{
					nameOfExistingGroup = entry.Key;
					return true;
				}
			}
			nameOfExistingGroup = null;
			return false;
		}

		public bool Contains(string name)
		{
			return _allLists.ContainsKey(name);
		}

		private static bool IsStructuralEquivalent(IEnumerable<IScatterSymbol> list1, IEnumerable<IScatterSymbol> list2)
		{
			using (var e1 = list1.GetEnumerator())
			{
				using (var e2 = list2.GetEnumerator())
				{
					bool f1 = e1.MoveNext();
					bool f2 = e2.MoveNext();

					while (f1 && f2)
					{
						if (!e1.Current.Equals(e2.Current))
							return false;
						f1 = e1.MoveNext();
						f2 = e2.MoveNext();
					}

					return f1 == false && f2 == false;
				}
			}
		}
	}
}