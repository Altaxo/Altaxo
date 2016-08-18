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

using Altaxo.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.ColorManagement
{
	public class ColorSet : IColorSet
	{
		private IList<NamedColor> _innerList;
		protected readonly string _name;

		protected Dictionary<string, int> _nameToIndexDictionary;
		protected Dictionary<AxoColor, int> _colorToIndexDictionary;
		protected Dictionary<ColorNameKey, int> _namecolorToIndexDictionary;

		#region Serialization

		/// <summary>
		/// 2015-11-15 Version 1 moved to Altaxo.Drawing.ColorManagement namespace.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ColorManagement.ColorSet", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Drawing.ColorManagement.ColorSet", 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version is not supported");
				/*
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
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				string colorSetName = info.GetString("Name");
				var colorSetLevel = (Altaxo.Main.ItemDefinitionLevel)info.GetEnum("Level", typeof(Altaxo.Main.ItemDefinitionLevel));
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

				IColorSet returnValue;
				ColorSetManager.Instance.TryRegisterList(new ColorSet(colorSetName, colors), Main.ItemDefinitionLevel.Project, out returnValue);
				return returnValue;
			}
		}

		/// <summary>
		/// 2016-08-15 Simplification and Immutability of ColorSet
		/// </summary>
		/// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorSet), 2)]
		private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ColorSet)obj;
				info.AddValue("Name", s._name);

				info.CreateArray("Colors", s._innerList.Count);

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

				int count = info.OpenArray("Colors");
				var colors = new NamedColor[count];
				for (int i = 0; i < count; ++i)
				{
					string name = info.GetStringAttribute("Name");
					string cvalue = info.GetString("e");
					colors[i] = new NamedColor(AxoColor.FromInvariantString(cvalue), name);
				}

				info.CloseArray(count);

				IColorSet returnValue;
				ColorSetManager.Instance.TryRegisterList(new ColorSet(colorSetName, colors), Main.ItemDefinitionLevel.Project, out returnValue);

				return returnValue;
			}
		}

		#endregion Serialization

		/// <summary>
		/// Creates a new collection of plot colors with a given name. The initial items will be copied from another plot color collection.
		/// </summary>
		/// <param name="name">Name of this plot color collection.</param>
		/// <param name="colors">The colors for this set.</param>
		public ColorSet(string name, IEnumerable<NamedColor> colors)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			_name = name;
			_innerList = new List<NamedColor>(colors.Select(c => new NamedColor(c.Color, c.Name, this)));
			BuildSideDictionaries();
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

			for (int i = this._innerList.Count - 1; i >= 0; --i)
			{
				var c = this._innerList[i];
				_nameToIndexDictionary[c.Name] = i;
				_colorToIndexDictionary[c.Color] = i;
				_namecolorToIndexDictionary[new ColorNameKey(c.Color, c.Name)] = i;
			}
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

		public int Count
		{
			get
			{
				return _innerList.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public NamedColor this[int index]
		{
			get
			{
				return _innerList[index];
			}

			set
			{
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Copies all color items to an array.
		/// </summary>
		/// <returns>Array with all colors in this set.</returns>
		public NamedColor[] ToArray()
		{
			return this._innerList.ToArray();
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
				namedColor = this._innerList[idx];
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
				namedColor = this._innerList[idx];
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
				namedColor = this._innerList[idx];
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
		public virtual int IndexOf(NamedColor color)
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

		public IStyleList<NamedColor> WithName(string name)
		{
			if (this.Name == name)
				return this;
			else
				return new ColorSet(name, this._innerList);
		}

		public bool IsStructuralEquivalentTo(IEnumerable<NamedColor> l1)
		{
			if (l1 == null)
				return false;

			var l2 = this;

			int i = 0;
			int len2 = l2._innerList.Count;
			foreach (var item1 in l1)
			{
				if (i >= len2)
					return false;

				if (!item1.EqualsInNameAndColor(l2._innerList[i]))
					return false;
				++i;
			}

			if (i != l2._innerList.Count)
				return false;

			return true;
		}

		public void Insert(int index, NamedColor item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public void Add(NamedColor item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(NamedColor item)
		{
			return _innerList.Contains(item);
		}

		public void CopyTo(NamedColor[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		public bool Remove(NamedColor item)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<NamedColor> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}
	}
}