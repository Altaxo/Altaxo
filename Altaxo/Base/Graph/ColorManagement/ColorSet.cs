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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.ColorManagement
{
	public class ColorSet : System.Collections.ObjectModel.ObservableCollection<NamedColor>, IColorSet
	{
		protected readonly string _name;
		protected readonly ColorSetLevel _colorSetLevel;
		private bool _isPlotColorSet;
		private DateTime _creationDate;

		protected Dictionary<string, int> _nameToIndexDictionary;
		protected Dictionary<AxoColor, int> _colorToIndexDictionary;
		protected Dictionary<ColorNameKey, int> _namecolorToIndexDictionary;

		/// <summary>Original name of the color set (name as it was in deserialized content).</summary>
		protected readonly string _originalName;

		/// <summary>Original level of the color set (level as it was in deserialized content).</summary>
		protected readonly ColorSetLevel _originalColorSetLevel;

		/// <summary>
		/// Used to suppress Changed events when a bulk of changes has to be made.
		/// </summary>
		private Main.TemporaryDisabler _bulkChanger;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorSet), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ColorSet)obj;
				info.AddValue("Name", s._originalName);
				info.AddEnum("Level", s._originalColorSetLevel);
				info.AddValue("CreationDate", s._creationDate);
				info.AddValue("IsPlotColorSet", s._isPlotColorSet);

				info.CreateArray("Colors", s.Count);

				foreach (NamedColor c in s)
				{
					info.CreateElement("e");
					info.AddAttributeValue("Name", c.Name);
					info.SetNodeContent(c.Color.ToInvariantString());
					info.CommitElement();
				}

				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				string colorSetName = info.GetString("Name");
				var colorSetLevel = (ColorManagement.ColorSetLevel)info.GetEnum("Level", typeof(ColorManagement.ColorSetLevel));
				var creationDate = info.GetDateTime("CreationDate");
				var isPlotColorSet = info.GetBoolean("IsPlotColorSet");

				int count = info.OpenArray("Colors");
				var colors = new NamedColor[count];
				for (int i = 0; i < count; ++i)
				{
					string name = info.GetStringAttribute("Name");
					string cvalue = info.GetString("e");
					colors[i] = new NamedColor(AxoColor.FromInvariantString(cvalue), name);
				}

				info.CloseArray(count);

				return ColorSetManager.Instance.GetDeserializedColorSet(colorSetName, colorSetLevel, creationDate, isPlotColorSet, colors);
			}
		}

		#endregion Serialization

		/// <summary>
		/// Creates a new collection of plot colors with a given name. The initial items will be copied from another plot color collection.
		/// </summary>
		/// <param name="name">Name of this plot color collection.</param>
		/// <param name="colorSetLevel">Hierarchie level of this color set.</param>
		public ColorSet(string name, ColorSetLevel colorSetLevel)
			: this(name, colorSetLevel, name, colorSetLevel, null)
		{
		}

		/// <summary>
		/// Creates a new collection of plot colors with a given name. The initial items will be copied from another plot color collection.
		/// </summary>
		/// <param name="name">Name of this plot color collection.</param>
		/// <param name="colorSetLevel">Hierarchie level of this color set.</param>
		/// <param name="basedOn">Another plot color collection from which to copy the initial items.</param>
		public ColorSet(string name, ColorSetLevel colorSetLevel, IEnumerable<NamedColor> basedOn)
			: this(name, colorSetLevel, name, colorSetLevel, basedOn)
		{
		}

		/// <summary>
		/// Creates a new collection of plot colors with a given name. The initial items will be copied from another plot color collection.
		/// The direct use of this constructor is intended mainly after deserialization, when the name and/or level deviates from the original name or level, because such a name-level combination is already present in the color set manager.
		/// </summary>
		/// <param name="name">Name of this plot color collection.</param>
		/// <param name="colorSetLevel">Hierarchie level of this color set.</param>
		/// <param name="originalName">Original name of the color set (name as it was in deserialized content).</param>
		/// <param name="originalColorSetLevel">Original level of the color set (level as it was in deserialized content).</param>
		/// <param name="basedOn">Another plot color collection from which to copy the initial items.</param>
		protected internal ColorSet(string name, ColorSetLevel colorSetLevel, string originalName, ColorSetLevel originalColorSetLevel, IEnumerable<NamedColor> basedOn)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentOutOfRangeException("Argument name must not be null or empty!");
			if (string.IsNullOrEmpty(originalName))
				throw new ArgumentOutOfRangeException("Argument originalName must not be null or empty!");
			if (colorSetLevel == ColorSetLevel.Builtin)
				throw new ArgumentOutOfRangeException("Argument colorSetLevel must not be 'ColorSetLevel.Builtin'. Please use a separate class for a new built-in color set");
			if (originalColorSetLevel == ColorSetLevel.Builtin)
				throw new ArgumentOutOfRangeException("Argument originalColorSetLevel must not be 'ColorSetLevel.Builtin'. Please use a separate class for a new built-in color set");

			_creationDate = DateTime.UtcNow;
			_bulkChanger = new Main.TemporaryDisabler(BuildSideDictionaries);

			_name = name;
			_colorSetLevel = colorSetLevel;
			_originalName = originalName;
			_originalColorSetLevel = originalColorSetLevel;

			if (null != basedOn)
			{
				using (var updateToken = _bulkChanger.SuspendGetToken())
				{
					foreach (var item in basedOn)
						Add(new NamedColor(item, this));
				}
			}
		}

		private void BuildSideDictionaries()
		{
			if (null == _nameToIndexDictionary)
				_nameToIndexDictionary = new Dictionary<string, int>();
			else
				_nameToIndexDictionary.Clear();

			if (null == _colorToIndexDictionary)
				_colorToIndexDictionary = new Dictionary<AxoColor, int>();
			else
				_colorToIndexDictionary.Clear();

			if (null == _namecolorToIndexDictionary)
				_namecolorToIndexDictionary = new Dictionary<ColorNameKey, int>();
			else
				_namecolorToIndexDictionary.Clear();

			for (int i = this.Count - 1; i >= 0; --i)
			{
				var c = this[i];
				_nameToIndexDictionary[c.Name] = i;
				_colorToIndexDictionary[c.Color] = i;
				_namecolorToIndexDictionary[new ColorNameKey(c.Color, c.Name)] = i;
			}
		}

		/// <summary>
		/// Inserts an <see cref="NamedColor"/> item at the specified position.
		/// </summary>
		/// <param name="index">The insertion position.</param>
		/// <param name="item">The color item to insert.</param>
		protected override void InsertItem(int index, NamedColor item)
		{
			base.InsertItem(index, new NamedColor(item, this));
			if (_bulkChanger.IsNotSuspended)
				BuildSideDictionaries();
		}

		/// <summary>
		/// Sets the item at the specified position.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="item">The item.</param>
		protected override void SetItem(int index, NamedColor item)
		{
			base.SetItem(index, new NamedColor(item, this));
			if (_bulkChanger.IsNotSuspended)
				BuildSideDictionaries();
		}

		/// <summary>
		/// Removes the item.
		/// </summary>
		/// <param name="index">The index.</param>
		protected override void RemoveItem(int index)
		{
			base.RemoveItem(index);
			if (_bulkChanger.IsNotSuspended)
				BuildSideDictionaries();
		}

		/// <summary>
		/// Clears all items in the set.
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();
			if (_bulkChanger.IsNotSuspended)
				BuildSideDictionaries();
		}

		/// <summary>
		/// Moves the item from one position to another position.
		/// </summary>
		/// <param name="oldIndex">The old index.</param>
		/// <param name="newIndex">The new index.</param>
		protected override void MoveItem(int oldIndex, int newIndex)
		{
			base.MoveItem(oldIndex, newIndex);
			if (_bulkChanger.IsNotSuspended)
				BuildSideDictionaries();
		}

		/// <summary>
		/// Gets the name of this collection of plot colors (without prefix like 'Application', 'Project' etc.).
		/// Note that the name can not be changed. To change the name, create a new plot color collection and copy the items to it.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Copies all color items to an array.
		/// </summary>
		/// <returns>Array with all colors in this set.</returns>
		public NamedColor[] ToArray()
		{
			return this.ToArray();
		}

		/// <summary>
		/// Gets the hierarchy level of the color set (see <see cref="ColorSetLevel"/>).
		/// </summary>
		public ColorSetLevel Level
		{
			get { return _colorSetLevel; }
		}

		/// <summary>
		/// Tries to find a color with a given name.
		/// </summary>
		/// <param name="colorName">The name of the color to find.</param>
		/// <param name="namedColor">If the color is contained in this collection, the color with the given name is returned. Otherwise, the result is undefined.</param>
		/// <returns>
		///   <c>True</c>, if the color with the given name is contained in this collection. Otherwise, <c>false</c> is returned.
		/// </returns>
		public bool TryGetValue(string colorName, out NamedColor namedColor)
		{
			int idx;
			if (_nameToIndexDictionary.TryGetValue(colorName, out idx))
			{
				namedColor = this[idx];
				return true;
			}
			else
			{
				namedColor = default(NamedColor);
				return false;
			}
		}

		/// <summary>
		/// Tries to find a color with a given color value.
		/// </summary>
		/// <param name="colorValue">The color to find.</param>
		/// <param name="namedColor">If the color is contained in this collection, the color with the given name is returned. Otherwise, the result is undefined.</param>
		/// <returns>
		///   <c>True</c>, if the color with the given name is contained in this collection. Otherwise, <c>false</c> is returned.
		/// </returns>
		public bool TryGetValue(AxoColor colorValue, out NamedColor namedColor)
		{
			int idx;
			if (_colorToIndexDictionary.TryGetValue(colorValue, out idx))
			{
				namedColor = this[idx];
				return true;
			}
			else
			{
				namedColor = default(NamedColor);
				return false;
			}
		}

		/// <summary>
		/// Tries to find a color with the color and the name of the given named color.
		/// </summary>
		/// <param name="colorValue">The color value to find.</param>
		/// <param name="colorName">The color name to find.</param>
		/// <param name="namedColor">On return, if a color with the same color value and name is found in the collection, that color is returned.</param>
		/// <returns>
		///   <c>True</c>, if the color with the given color value and name is found in this collection. Otherwise, <c>false</c> is returned.
		/// </returns>
		public bool TryGetValue(AxoColor colorValue, string colorName, out NamedColor namedColor)
		{
			int idx;
			if (_namecolorToIndexDictionary.TryGetValue(new ColorNameKey(colorValue, colorName), out idx))
			{
				namedColor = this[idx];
				return true;
			}
			else
			{
				namedColor = default(NamedColor);
				return false;
			}
		}

		/// <summary>
		/// Get the index of the given color in the ColorSet.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <returns>The index of the color in the set. If the color is not found in the set, a negative value is returned.</returns>
		public new virtual int IndexOf(NamedColor color)
		{
			int idx;
			if (_namecolorToIndexDictionary.TryGetValue(new ColorNameKey(color.Color, color.Name), out idx))
			{
				return idx;
			}
			else
			{
				return -1;
			}
		}

		/// <summary>
		/// Get the index of the given color in the ColorSet.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <returns>The index of the color in the set. If the color is not found in the set, a negative value is returned.</returns>
		public virtual int IndexOf(AxoColor color)
		{
			int idx;
			if (_colorToIndexDictionary.TryGetValue(color, out idx))
			{
				return idx;
			}
			else
			{
				return -1;
			}
		}

		/// <summary>
		/// Adds a color with the specified color value and name to the collection.
		/// </summary>
		/// <param name="colorValue">The color value.</param>
		/// <param name="name">The name of the color.</param>
		/// <returns>
		/// The freshly added named color with the color value and name provided by the arguments.
		/// </returns>
		public NamedColor Add(AxoColor colorValue, string name)
		{
			var result = new NamedColor(colorValue, name);
			Add(result);
			return result;
		}

		/// <summary>
		/// Gets a value indicating whether this instance is used as a plot color set.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is a plot color set; otherwise, <c>false</c>.
		/// </value>
		public bool IsPlotColorSet
		{
			get { return _isPlotColorSet; }
		}

		/// <summary>
		/// Declares the this set as plot color set. This decision is not reversable.
		/// </summary>
		public void DeclareThisSetAsPlotColorSet()
		{
			_isPlotColorSet = true;
		}

		/// <summary>
		/// Determines whether this color set has the same colors (matching by name and color value, and index) as another set.
		/// </summary>
		/// <param name="other">The other set to compare with.</param>
		/// <returns>
		///   <c>true</c> if this set has the same colors as the other set; otherwise, <c>false</c>.
		/// </returns>
		public bool HasSameContentAs(IList<NamedColor> other)
		{
			return BuiltinColorSet.HaveSameNamedColors(this, other);
		}
	}
}