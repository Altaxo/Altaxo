#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
using System.Text;

namespace Altaxo.Drawing.ColorManagement
{
	/// <summary>
	/// Manages the set of colors for the application. This class has only a single instance (see <see cref="Instance"/>).
	/// </summary>
	public class ColorSetManager : Graph.IStyleListManager<IColorSet, NamedColor>
	{
		#region Inner classes

		public class ColorSetManagerEntry
		{
			public IColorSet ColorSet { get; private set; }
			public Main.ItemDefinitionLevel Level { get; private set; }
			public bool IsPlotColorSet { get; private set; }

			public ColorSetManagerEntry(IColorSet colorSet, Main.ItemDefinitionLevel level, bool isPlotColorSet)
			{
				ColorSet = colorSet;
				Level = level;
				IsPlotColorSet = isPlotColorSet;
			}
		}

		#endregion Inner classes

		/// <summary>
		/// Stores the only instance of this class.
		/// </summary>
		private static ColorSetManager _instance = new ColorSetManager();

		/// <summary>
		/// Stores all color sets in a dictionary. The key is the name of the colorset, the value is a struct consisting of the level and name of the color set.
		/// </summary>
		private SortedDictionary<string, ColorSetManagerEntry> _allLists = new SortedDictionary<string, ColorSetManagerEntry>();

		private IColorSet _builtinKnownColors;
		private IColorSet _builtinDarkPlotColors;

		private ColorSetManager()
		{
			_builtinKnownColors = NamedColors.Instance;
			_allLists.Add(_builtinKnownColors.Name, new ColorSetManagerEntry(_builtinKnownColors, Main.ItemDefinitionLevel.Builtin, false));

			_builtinDarkPlotColors = new ColorSet("PlotColorsDark", GetPlotColorsDark_Version0());
			_allLists.Add(_builtinDarkPlotColors.Name, new ColorSetManagerEntry(_builtinDarkPlotColors, Main.ItemDefinitionLevel.Builtin, true));
		}

		#region Buildin

		private static NamedColor[] GetPlotColorsDark_Version0() // Version 2012-09-10
		{
			return new NamedColor[]{
			NamedColors.Black,
			NamedColors.Red,
			NamedColors.Green,
			NamedColors.Blue,
			NamedColors.Magenta,
			NamedColors.Goldenrod,
			NamedColors.Coral
			};
		}

		#endregion Buildin

		/// <summary>
		/// Gets the (single) instance of this class.
		/// </summary>
		public static ColorSetManager Instance { get { return _instance; } }

		#region IStyleListManager interface

		public IEnumerable<string> GetAllListNames()
		{
			return _allLists.Keys;
		}

		public IEnumerable<Tuple<IColorSet, ItemDefinitionLevel>> GetListsWithLevel()
		{
			foreach (var entry in _allLists.Values)
				yield return new Tuple<IColorSet, ItemDefinitionLevel>(entry.ColorSet, entry.Level);
		}

		public bool ContainsList(string name)
		{
			return _allLists.ContainsKey(name);
		}

		public bool TryGetValue(string name, out IColorSet colorSet, out Main.ItemDefinitionLevel level, out bool isPlotColorSet)
		{
			ColorSetManagerEntry value;
			if (_allLists.TryGetValue(name, out value))
			{
				colorSet = value.ColorSet;
				level = value.Level;
				isPlotColorSet = value.IsPlotColorSet;
				return true;
			}
			else
			{
				colorSet = null;
				level = default(Main.ItemDefinitionLevel);
				isPlotColorSet = default(bool);
				return false;
			}
		}

		public IColorSet GetList(string name)
		{
			var value = _allLists[name];
			return value.ColorSet;
		}

		public IColorSet GetList(string name, out ItemDefinitionLevel level)
		{
			var value = _allLists[name];
			level = value.Level;
			return value.ColorSet;
		}

		public bool TryGetListByMembers(IEnumerable<NamedColor> symbols, out string nameOfExistingList)
		{
			foreach (var entry in _allLists)
			{
				if (entry.Value.ColorSet.IsStructuralEquivalentTo(symbols))
				{
					nameOfExistingList = entry.Key;
					return true;
				}
			}
			nameOfExistingList = null;
			return false;
		}

		/// <summary>
		/// Try to register the provided list.
		/// </summary>
		/// <param name="level">The level on which this list is defined.</param>
		/// <param name="instance">The new list which is tried to register.</param>
		/// <param name="storedList">On return, this is the list which is either registered, or is an already registed list with exactly the same elements.</param>
		/// <returns>True if the list was new and thus was added to the collection; false if the list has already existed.</returns>
		public bool TryRegisterList(Main.ItemDefinitionLevel level, IColorSet instance, out IColorSet storedList)
		{
			string nameOfExistingGroup;
			if (TryGetListByMembers(instance, out nameOfExistingGroup)) // if a group with such a list already exist
			{
				if (nameOfExistingGroup != instance.Name) // if it has the same list, but a different name, do nothing at all
				{
					storedList = _allLists[nameOfExistingGroup].ColorSet;
					return false;
				}
				else // if it has the same list, and the same name, even better, nothing is left to be done
				{
					storedList = _allLists[nameOfExistingGroup].ColorSet;
					return false;
				}
			}
			else // a group with such members don't exist currently
			{
				if (_allLists.ContainsKey(instance.Name)) // but name is already in use
				{
					storedList = (IColorSet)instance.WithName(GetUnusedName(instance.Name));
					_allLists.Add(storedList.Name, new ColorSetManagerEntry(storedList, level, false));
					return true;
				}
				else // name is not in use
				{
					storedList = instance;
					_allLists.Add(instance.Name, new ColorSetManagerEntry(instance, level, false));
					return true;
				}
			}
		}

		public IColorSet CreateNewList(string name, IEnumerable<NamedColor> symbols, bool registerNewList, Main.ItemDefinitionLevel level)
		{
			var newList = new ColorSet(name, symbols);
			IColorSet outList = newList;
			if (registerNewList)
			{
				TryRegisterList(level, newList, out outList);
			}
			return outList;
		}

		protected virtual string GetUnusedName(string usedName)
		{
			if (string.IsNullOrEmpty(usedName))
				throw new ArgumentNullException(nameof(usedName));

			if (!_allLists.ContainsKey(usedName))
				return usedName;

			int i;
			for (i = usedName.Length - 1; i >= 0; --i)
			{
				if (!char.IsDigit(usedName[i]))
					break;
			}

			int numberOfDigits = usedName.Length - (i + 1);

			if (0 == numberOfDigits)
			{
				return GetUnusedName(usedName + "0");
			}
			else
			{
				int number = int.Parse(usedName.Substring(i + 1), System.Globalization.NumberStyles.Any);
				string formatString = "N" + numberOfDigits.ToString(System.Globalization.CultureInfo.InvariantCulture);
				return GetUnusedName(usedName.Substring(0, i + 1) + (number + 1).ToString(formatString, System.Globalization.CultureInfo.InvariantCulture));
			}
		}

		#endregion IStyleListManager interface

		/// <summary>
		/// Gets the builtin set of known colors.
		/// </summary>
		public IColorSet BuiltinKnownColors
		{
			get
			{
				return _builtinKnownColors;
			}
		}

		/// <summary>
		/// Gets the builtin set of dark plot colors.
		/// </summary>
		public IColorSet BuiltinDarkPlotColors
		{
			get
			{
				return _builtinDarkPlotColors;
			}
		}

		public bool IsPlotColorSet(IColorSet colorSet)
		{
			if (null == colorSet)
				return false;

			ColorSetManagerEntry value;
			if (_allLists.TryGetValue(colorSet.Name, out value))
				return value.IsPlotColorSet;

			return false;
		}

		public void DeclareAsPlotColorList(IColorSet colorSet)
		{
			if (null == colorSet)
				throw new ArgumentNullException(nameof(colorSet));
			if (!_allLists.ContainsKey(colorSet.Name))
				throw new ArgumentException("Provided ColorSet is not registered in ColorSetManager", nameof(colorSet));

			var value = _allLists[colorSet.Name];
			if (!value.IsPlotColorSet)
				_allLists[colorSet.Name] = new ColorSetManagerEntry(colorSet, value.Level, true);
		}

		/// <summary>
		/// Gets the <see cref="IColorSet"/> with the specified level and name.
		/// </summary>
		public IColorSet this[string name]
		{
			get
			{
				return _allLists[name].ColorSet;
			}
		}

		/// <summary>
		/// Enumerates through all color sets in this manager.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IColorSet> GetAllColorSets()
		{
			foreach (var entry in _allLists)
			{
				yield return entry.Value.ColorSet;
			}
		}

		/// <summary>
		/// Enumerates through all color sets in this manager.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ColorSetManagerEntry> GetAllColorSetsWithLevelAndPlotColorStatus()
		{
			foreach (var entry in _allLists)
			{
				yield return entry.Value;
			}
		}

		#region Deserialization of colors

		public bool TryFindColorSetContaining(AxoColor color, out IColorSet value)
		{
			NamedColor namedColor;

			foreach (Main.ItemDefinitionLevel level in Enum.GetValues(typeof(Main.ItemDefinitionLevel)))
			{
				foreach (var entry in _allLists)
				{
					if (entry.Value.Level != level)
						continue;

					if (entry.Value.ColorSet.TryGetValue(color, out namedColor))
					{
						value = entry.Value.ColorSet;
						return true;
					}
				}
			}

			value = null;
			return false;
		}

		public bool TryFindColorSetContaining(AxoColor colorValue, string colorName, out IColorSet value)
		{
			NamedColor namedColor;

			foreach (Main.ItemDefinitionLevel level in Enum.GetValues(typeof(Main.ItemDefinitionLevel)))
			{
				foreach (var entry in _allLists)
				{
					if (entry.Value.Level != level)
						continue;

					if (entry.Value.ColorSet.TryGetValue(colorValue, colorName, out namedColor))
					{
						value = entry.Value.ColorSet;
						return true;
					}
				}
			}

			value = null;
			return false;
		}

		public NamedColor GetDeserializedColorWithNoSet(AxoColor color, string name)
		{
			// test if it is a standard color
			NamedColor foundColor;
			if (_builtinKnownColors.TryGetValue(name, out foundColor) && color.Equals(foundColor.Color)) // if the color is known by this name, and the color value matches
				return foundColor; // then return this found color

			if (_builtinKnownColors.TryGetValue(color, out foundColor)) // if only the color value matches, then return the found color, even if it has another name than the deserialized color
				return foundColor;

			return new NamedColor(color, name); // if it is not a known color, then return the color without a color set as parent
		}

		public NamedColor GetDeserializedColorFromBuiltinSet(AxoColor color, string colorName, IColorSet builtinColorSet)
		{
			return new NamedColor(color, colorName, builtinColorSet);
		}

		public NamedColor GetDeserializedColorFromLevelAndSetName(AxoColor colorValue, string colorName, string colorSetName)
		{
			ColorSetManagerEntry foundSet;
			NamedColor foundColor;

			if (_allLists.TryGetValue(colorSetName, out foundSet)) // if a set with the give name and level was found
			{
				if (foundSet.ColorSet.TryGetValue(colorName, out foundColor) && colorValue.Equals(foundColor.Color)) // if the color is known by this name, and the color value matches
					return foundColor;                                                                  // then return this found color
				if (foundSet.ColorSet.TryGetValue(colorValue, out foundColor)) // if only the color value matches,
					return foundColor;                            // then return the found color, even if it has another name than the deserialized color

				// set was found, but color is not therein -> return a color without set (or use the first set where the color could be found
				IColorSet cset;
				TryFindColorSetContaining(colorValue, colorName, out cset);
				var result = new NamedColor(colorValue, colorName, cset);
				return result;
			}
			else // the color set with the given name was not found by name
			{
				IColorSet cset;
				TryFindColorSetContaining(colorValue, colorName, out cset);
				var result = new NamedColor(colorValue, colorName, cset);
				return result;
			}
		}

		#endregion Deserialization of colors
	}
}