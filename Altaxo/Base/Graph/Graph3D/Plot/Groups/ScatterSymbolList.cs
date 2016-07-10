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
	public class ScatterSymbolList : Main.IImmutable, IList<IScatterSymbol> // TODO NET45 replace IList with IReadonlyList
	{
		private string _name;
		private List<IScatterSymbol> _list;

		public static ScatterSymbolList BuiltinDefault { get; private set; }

		#region Serialization

		private ScatterSymbolList(string name, List<IScatterSymbol> listToTakeDirectly, Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
			_name = name;
			_list = listToTakeDirectly;
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScatterSymbolList), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ScatterSymbolList)obj;
				info.AddValue("Name", s._name);
				info.CreateArray("Elements", s._list.Count);
				foreach (var ele in s._list)
					info.AddValue("e", ele);
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				string name = info.GetString("Name");
				int count = info.OpenArray("Elements");
				var list = new List<IScatterSymbol>(count);
				for (int i = 0; i < count; ++i)
					list.Add((IScatterSymbol)info.GetValue("e", null));
				info.CloseArray(count);

				var result = new ScatterSymbolList(name, list);
				ScatterSymbolList.RegisterInstance(result);
				return result;
			}
		}

		#endregion Serialization

		public string Name { get { return _name; } }

		public ScatterSymbolList(string name, IEnumerable<IScatterSymbol> symbols)
		{
			_name = name;
			_list = new List<IScatterSymbol>(symbols);
		}

		public int Count { get { return _list.Count; } }

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public IScatterSymbol this[int index]
		{
			get
			{
				return ((IList<IScatterSymbol>)_list)[index];
			}

			set
			{
				throw new InvalidOperationException("List is readonly");
			}
		}

		public int IndexOf(IScatterSymbol item)
		{
			return ((IList<IScatterSymbol>)_list).IndexOf(item);
		}

		public void Insert(int index, IScatterSymbol item)
		{
			throw new InvalidOperationException("List is readonly");
		}

		public void RemoveAt(int index)
		{
			((IList<IScatterSymbol>)_list).RemoveAt(index);
		}

		public void Add(IScatterSymbol item)
		{
			throw new InvalidOperationException("List is readonly");
		}

		public void Clear()
		{
			throw new InvalidOperationException("List is readonly");
		}

		public bool Contains(IScatterSymbol item)
		{
			return ((IList<IScatterSymbol>)_list).Contains(item);
		}

		public void CopyTo(IScatterSymbol[] array, int arrayIndex)
		{
			((IList<IScatterSymbol>)_list).CopyTo(array, arrayIndex);
		}

		public bool Remove(IScatterSymbol item)
		{
			throw new InvalidOperationException("List is readonly");
		}

		public IEnumerator<IScatterSymbol> GetEnumerator()
		{
			return ((IEnumerable<IScatterSymbol>)_list).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<IScatterSymbol>)_list).GetEnumerator();
		}

		#region Static and global

		static ScatterSymbolList()
		{
			Current.ProjectService.ProjectClosed += EhProjectClosed;

			var defaultList = new ScatterSymbolList("Builtin/Default", new IScatterSymbol[] {
				new Styles.ScatterSymbols.Cube(),
					new Styles.ScatterSymbols.Sphere(),
					new Styles.ScatterSymbols.TetrahedronUp(),
					new Styles.ScatterSymbols.TetrahedronDown()
			});

			BuiltinDefault = defaultList;

			_allLists.Add(defaultList.Name, new Tuple<bool, ScatterSymbolList>(false, defaultList));
		}

		/// <summary>
		/// Dictionary of all existing lists. Key is the list name. Value is a tuple, whose boolean entry designates whether this is
		/// a buildin or user list (false) or a project list (true).
		/// </summary>
		private static Dictionary<string, Tuple<bool, ScatterSymbolList>> _allLists = new Dictionary<string, Tuple<bool, ScatterSymbolList>>();

		public static IEnumerable<string> GetAllListNames()
		{
			return _allLists.Keys;
		}

		public static ScatterSymbolList GetList(string name)
		{
			return _allLists[name].Item2;
		}

		/// <summary>
		/// Called when the current project is closed. Removes all those list which are project lists.
		/// </summary>
		private static void EhProjectClosed(object sender, Main.ProjectEventArgs e)
		{
			var namesToRemove = new List<string>(_allLists.Where(entry => entry.Value.Item1 == true).Select(entry => entry.Key));
			foreach (var name in namesToRemove)
				_allLists.Remove(name);
		}

		private static void RegisterInstance(ScatterSymbolList instance)
		{
			string nameOfExistingGroup;
			if (TryGetGroupByMembers(instance._list, out nameOfExistingGroup)) // if a group with such a list already exist
			{
				if (nameOfExistingGroup != instance.Name) // if it has the same list, but a different name, do nothing at all
				{
				}
				else // if it has the same list, and the same name, even better, nothing is left to be done
				{
				}
			}
			else // a group with such members don't exist currently
			{
				if (_allLists.ContainsKey(instance.Name)) // but name is already in use
				{
					var newInstance = new ScatterSymbolList(GetUnusedName(instance.Name), instance);
					_allLists.Add(newInstance.Name, new Tuple<bool, ScatterSymbolList>(false, newInstance));
				}
				else // name is not in use
				{
					_allLists.Add(instance.Name, new Tuple<bool, ScatterSymbolList>(false, instance));
				}
			}
		}

		private static string GetUnusedName(string usedName)
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

		public static bool TryGetGroupByMembers(IEnumerable<IScatterSymbol> symbols, out string nameOfExistingGroup)
		{
			foreach (var entry in _allLists)
			{
				if (IsStructuralEquivalent(symbols, entry.Value.Item2))
				{
					nameOfExistingGroup = entry.Key;
					return true;
				}
			}
			nameOfExistingGroup = null;
			return false;
		}

		public static bool Contains(string name)
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

		#endregion Static and global
	}
}