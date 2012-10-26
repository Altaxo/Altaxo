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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.ColorManagement
{
	/// <summary>
	/// Manages the set of colors for the application. This class has only a single instance (see <see cref="Instance"/>).
	/// </summary>
	public class ColorSetManager
	{
		#region Inner classes

		/// <summary>
		/// Structure that stores <see cref="ColorSetLevel"/> and name of a color set. This is used as key value in the internal dictionaries.
		/// </summary>
		[System.ComponentModel.ImmutableObject(true)]
		protected struct ColorSetKey : IEquatable<ColorSetKey>, IComparable<ColorSetKey>
		{
			ColorSetLevel _level;
			string _name;

			public ColorSetKey(ColorSetLevel colorSetLevel, string colorSetName)
			{
				if (string.IsNullOrEmpty(colorSetName))
					throw new ArgumentOutOfRangeException("colorSetName is null or is empty");

				_level = colorSetLevel;
				_name = colorSetName;
			}

			public ColorSetLevel Level { get { return _level; } }
			public string Name { get { return _name; } }

			public override int GetHashCode()
			{
				return _level.GetHashCode() + _name.GetHashCode();
			}

			public bool Equals(ColorSetKey other)
			{
				return this._level == other._level && 0 == string.Compare(this._name, other._name);
			}

			public override bool Equals(object obj)
			{
				if (obj is ColorSetKey)
				{
					var other = (ColorSetKey)obj;
					return this._level == other._level && 0 == string.Compare(this._name, other._name);
				}
				return false;
			}



			public int CompareTo(ColorSetKey other)
			{
				int result;
				result = Comparer<int>.Default.Compare((int)this._level, (int)other._level);
				if (0 != result)
					return result;
				else
					return string.Compare(this._name, other._name);
			}
		}

		#endregion

		/// <summary>
		/// Stores the only instance of this class.
		/// </summary>
		static ColorSetManager _instance = new ColorSetManager();


		/// <summary>
		/// Stores all color sets in a dictionary. The key is a compound key, consisting of the level and name of the color set.
		/// </summary>
		SortedDictionary<ColorSetKey, IColorSet> _colorSetCollection = new SortedDictionary<ColorSetKey, IColorSet>();

		IColorSet _builtinKnownColors;
		IColorSet _builtinDarkPlotColors;

		private ColorSetManager()
		{
			_builtinKnownColors = NamedColors.Instance;
			_builtinDarkPlotColors = BuiltinDarkPlotColorSet.Instance;
			this.Add(_builtinDarkPlotColors);
			this.Add(_builtinKnownColors);
		}

		/// <summary>
		/// Gets the (single) instance of this class.
		/// </summary>
		public static ColorSetManager Instance { get { return _instance; } }

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

		/// <summary>
		/// Adds the specified color set to the color manager.
		/// </summary>
		/// <param name="plotColors">The color set to add.</param>
		public void Add(IColorSet plotColors)
		{
			IColorSet existing;
			var key = new ColorSetKey(plotColors.Level, plotColors.Name);
			if (_colorSetCollection.TryGetValue(key, out existing) && !object.ReferenceEquals(existing, plotColors))
				throw new ArgumentException(string.Format("Try to add a plot color collection <<{0}>>, but another collection with the same name is already present", plotColors.Name));

			_colorSetCollection.Add(key, plotColors);
		}

		/// <summary>
		/// Adds a range of color sets to the manager.
		/// </summary>
		/// <param name="sets">The sets.</param>
		public void AddRange(IEnumerable<IColorSet> sets)
		{
			foreach (var s in sets)
				Add(s);
		}

		/// <summary>
		/// Determines whether the manager contains a color set with the given <see cref="ColorSetLevel"/> and name.
		/// </summary>
		/// <param name="level">The color set level.</param>
		/// <param name="name">The color set name.</param>
		/// <returns>
		///   <c>true</c> if the manager contains a set with the given <see cref="ColorSetLevel"/> and name; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(ColorSetLevel level, string name)
		{
			return _colorSetCollection.ContainsKey(new ColorSetKey(level,name));
		}

		/// <summary>
		/// Gets the <see cref="Altaxo.Graph.ColorManagement.IColorSet"/> with the specified level and name.
		/// </summary>
		public IColorSet this[ColorSetLevel level, string name]
		{
			get
			{
				return _colorSetCollection[new ColorSetKey(level,name)];
			}
		}


		/// <summary>
		/// Tries to get the <see cref="Altaxo.Graph.ColorManagement.IColorSet"/> with the specified level and name.
		/// </summary>
		/// <param name="level">The color set level.</param>
		/// <param name="name">The color set name.</param>
		/// <param name="colorSet">On return, if the color set with given level and name was found, contains the found color set.</param>
		/// <returns><c>True</c> if a color set with the specified level and name was found in the manager.</returns>
		public bool TryGetValue(ColorSetLevel level, string name, out IColorSet colorSet)
		{
			return _colorSetCollection.TryGetValue(new ColorSetKey(level, name), out colorSet);
		}

		/// <summary>
		/// Enumerates through all color sets in this manager.
		/// </summary>
		/// <returns></returns>
    public IEnumerable<IColorSet> GetAllColorSets()
    {
      foreach (var entry in _colorSetCollection)
      {
        yield return entry.Value;
      }
    }


		#region Deserialization of colors

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


		public NamedColor GetDeserializedColorFromLevelAndSetName(AxoColor colorValue, string colorName, ColorSetLevel colorSetLevel, string colorSetName)
		{
			IColorSet foundSet;
			NamedColor foundColor;

			if (TryGetValue(colorSetLevel, colorSetName, out foundSet)) // if a set with the give name and level was found
			{
				if (foundSet.TryGetValue(colorName, out foundColor) && colorValue.Equals(foundColor.Color)) // if the color is known by this name, and the color value matches
					return foundColor;                                                                  // then return this found color
				if (foundSet.TryGetValue(colorValue, out foundColor)) // if only the color value matches, 
					return foundColor;                            // then return the found color, even if it has another name than the deserialized color

				// set was found, but color is not therein -> if we are at the project level, then simply add the color. If we are at the Application or User level, we must re-entrance this function with the project level
				if (colorSetLevel == ColorSetLevel.Project)
				{
					if (!foundSet.IsReadOnly)
					{
						return foundSet.Add(colorValue, colorName);
					}
					else // this set on the project level is readonly -> this would mean we have to create another set on the project level
					{
						// here we simply try all names by adding a number from 2 to infinity
						for (int i = 2; i < int.MaxValue; ++i)
						{
							string newSetName = colorSetName + i.ToString(System.Globalization.CultureInfo.InvariantCulture);
							if (!TryGetValue(colorSetLevel, newSetName, out foundSet))
								foundSet = new ColorSet(newSetName, colorSetLevel);

							if (foundSet.IsReadOnly)
								continue;
							else
								return foundSet.Add(colorValue, colorName);
						}
						throw new InvalidOperationException("All set names already in use");
					}
				}
				else // a set was found, but color was not therein, and the set level was not project level -> we re-entrance the function with project level
				{
					return GetDeserializedColorFromLevelAndSetName(colorValue, colorName, ColorSetLevel.Project, colorSetName); // re-entrance of this function with project level
				}
			}
			else // the color set with the given name and level was not found
			{
				// what we can do here: if we are already on the project level, we create a new color set with the given name and store the color therein
				if (ColorSetLevel.Project == colorSetLevel)
				{
					foundSet = new ColorSet(colorSetName, ColorSetLevel.Project);
					Add(foundSet);
					return foundSet.Add(colorValue, colorName);
				}
				else
				{
					colorSetName += string.Format("_(from{0})", System.Enum.GetName(typeof(ColorSetLevel), colorSetLevel));
					return GetDeserializedColorFromLevelAndSetName(colorValue, colorName, ColorSetLevel.Project, colorSetName);
				}
			}
		}

		

		#endregion

    #region Deserialization of color sets

    public IColorSet GetDeserializedColorSet(string colorSetName, ColorSetLevel colorSetLevel, DateTime creationDate, bool isPlotColorSet, IList<NamedColor> set)
    {
      // the given color set can have three levels:
      // Application: if an equal color set on Application level is found in Altaxo, use this instead. Otherwise, when an equal color set is found on user level, use that. Else, if an equal color set is found on project level, use that.
      //							Else, create a new color set on project level.
      // User:				Same procedure as above
      // Project:			Same procedure as above

      foreach (var builtinSet in _colorSetCollection.Values.Where(x => x.Level == ColorSetLevel.Builtin))
      {
        if (builtinSet.HasSameContentAs(set) && (isPlotColorSet==false || builtinSet.IsPlotColorSet))
          return builtinSet;
      }
      foreach (var appSet in _colorSetCollection.Values.Where(x => x.Level == ColorSetLevel.Application))
      {
        if (appSet.HasSameContentAs(set) && (isPlotColorSet == false || appSet.IsPlotColorSet))
          return appSet;
      }
      foreach (var userSet in _colorSetCollection.Values.Where(x => x.Level == ColorSetLevel.UserDefined))
      {
        if (userSet.HasSameContentAs(set) && (isPlotColorSet == false || userSet.IsPlotColorSet))
          return userSet;
      }
      foreach (var projSet in _colorSetCollection.Values.Where(x => x.Level == ColorSetLevel.Project))
      {
        if (projSet.HasSameContentAs(set) && (isPlotColorSet == false || projSet.IsPlotColorSet))
          return projSet;
      }

      // no such set found, then we should include the set at the project level
      // find an available name at the project level

      var newSet = new ColorSet(FindAvailableName(colorSetName, ColorSetLevel.Project), ColorSetLevel.Project, set);
      if (isPlotColorSet)
        newSet.DeclareThisSetAsPlotColorSet();

      this.Add(newSet);

      return newSet;
    }

    /// <summary>
    /// Finds an available name that can be used for a new color set, at the given color set level.
    /// </summary>
    /// <param name="name">The proposed name. Must not be null or empty.</param>
    /// <param name="level">The color set level.</param>
    /// <returns>A name (either the given name or a name with an appended number), that can be used for a new color set in this collection.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown if name is null or empty.</exception>
    public string FindAvailableName(string name, ColorSetLevel level)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentOutOfRangeException("name is null or empty");

      if (!_colorSetCollection.Keys.Contains(new ColorSetKey(level, name)))
        return name;
      // else try to append a number to the name
      int firstIdx;
      if (name[name.Length - 1] == ')' && (firstIdx = name.LastIndexOf('(')) > 0)
      {
        int number;
        if(int.TryParse(name.Substring(firstIdx+1,name.Length-firstIdx-2),System.Globalization.NumberStyles.Integer,System.Globalization.CultureInfo.InvariantCulture, out number))
          name = name.Substring(firstIdx);
      }

     for(int n=2;;++n)
     {
       string result = string.Format(System.Globalization.CultureInfo.InvariantCulture,"{0}({1})",name,n);
       if(!_colorSetCollection.Keys.Contains(new ColorSetKey(level, result)))
         return result;
     }
    }

    #endregion
  }


}
