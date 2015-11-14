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

namespace Altaxo.Drawing.ColorManagement
{
	/// <summary>
	///
	/// </summary>
	public abstract class BuiltinColorSet : IColorSet
	{
		protected List<NamedColor> _innerList;

		protected readonly string _name;
		protected Dictionary<string, int> _nameToIndexDictionary;
		protected Dictionary<AxoColor, int> _colorToIndexDictionary;
		protected Dictionary<ColorNameKey, int> _colornameToIndexDictionary;

		#region Serialization

		// Here, no serialization is intended to do.
		// This is because it should be forced that every built-in ColorSet
		// implements its own deserialization, so that, if the colors in the color set change,
		// a new serialization version of the color set is created. Old versions should be deserialized also,
		// but of course with the original colors.

		#endregion Serialization

		/// <summary>
		/// Initializes a new instance of the <see cref="BuiltinColorSet"/> class with the provided name and the colors.
		/// </summary>
		/// <param name="name">The name of the color set.</param>
		/// <param name="colorSet">The colors the color set contains.</param>
		protected BuiltinColorSet(string name, IEnumerable<NamedColor> colorSet)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("name is null or empty");

			_name = name;

			_innerList = new List<NamedColor>();
			foreach (NamedColor c in colorSet)
				_innerList.Add(new NamedColor(c, this));

			BuildSideDictionaries();
		}

		/// <summary>
		/// Build all dictionaries for fast acess of colors by name, by color value and by combined color value and name.
		/// </summary>
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

			if (null == _colornameToIndexDictionary)
				_colornameToIndexDictionary = new Dictionary<ColorNameKey, int>();
			else
				_colornameToIndexDictionary.Clear();

			for (int i = _innerList.Count - 1; i >= 0; --i)
			{
				var c = _innerList[i];
				_nameToIndexDictionary[c.Name] = i;
				_colorToIndexDictionary[c.Color] = i;
				_colornameToIndexDictionary[new ColorNameKey(c.Color, c.Name)] = i;
			}
		}

		#region IPlotColorSet

		/// <summary>
		/// Gets the name of the color set (without the prefix 'builtin').
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets the hierarchy level of the color set (see <see cref="ColorSetLevel"/>). For this class and derived classes, it is always <see cref="ColorSetLevel.Builtin"/>.
		/// </summary>
		public ColorSetLevel Level
		{
			get { return ColorSetLevel.Builtin; }
		}

		/// <summary>
		/// Not allowed for a builtin color set. Calling this function will throw an <see cref="InvalidOperationException"/>.
		/// </summary>
		/// <param name="item">Not used.</param>
		public void Add(NamedColor item)
		{
			throw new InvalidOperationException("This is a read-only collection");
		}

		/// <summary>
		/// Not allowed for a builtin color set. Calling this function will throw an <see cref="InvalidOperationException"/>.
		/// </summary>
		/// <param name="colorValue">Not used.</param>
		/// <param name="name">Not used.</param>
		/// <returns>Not used.</returns>
		public NamedColor Add(AxoColor colorValue, string name)
		{
			throw new InvalidOperationException("This is a read-only collection");
		}

		/// <summary>
		/// Not allowed for a builtin color set. Calling this function will throw an <see cref="InvalidOperationException"/>.
		/// </summary>
		public void Clear()
		{
			throw new InvalidOperationException("This is a read-only collection");
		}

		/// <summary>
		/// Determines whether this set contains the specified color (a color that matches the color value and the name).
		/// </summary>
		/// <param name="item">The color.</param>
		/// <returns><c>True</c> if the set contains the a color with the same color value and name as the specified color; otherwise, <c>false</c>.</returns>
		public bool Contains(NamedColor item)
		{
			return _colornameToIndexDictionary.ContainsKey(new ColorNameKey(item.Color, item.Name));
		}

		/// <summary>
		/// Copies the color set to an array, starting at the specified index.
		/// </summary>
		/// <param name="array">The array to store the colors.</param>
		/// <param name="arrayIndex">Index of the array to store the first item.</param>
		public void CopyTo(NamedColor[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Gets the number of colors in this color set.
		/// </summary>
		public int Count
		{
			get { return _innerList.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is read only. This is always <c>True</c> for a builtin color set like this.
		/// </summary>
		/// <value>
		/// 	Always <c>true</c>.
		/// </value>
		public bool IsReadOnly
		{
			get { return true; }
		}

		/// <summary>
		/// Not allowed for a builtin color set. Calling this function will throw an <see cref="InvalidOperationException"/>.
		/// </summary>
		/// <param name="item">Unused.</param>
		/// <returns>Unused.</returns>
		public bool Remove(NamedColor item)
		{
			throw new InvalidOperationException("This is a read-only collection");
		}

		/// <summary>
		/// Returns an enumerator that iterates through the colors of the color set.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<NamedColor> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the colors of the color set.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _innerList.GetEnumerator();
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
		/// Gets the index of the <see cref="NamedColor"/> item in the set.
		/// </summary>
		/// <param name="item">The item to look for.</param>
		/// <returns>The index of the specified color in the color set. If the color is not found in the set, a negative value is returned.</returns>
		public int IndexOf(NamedColor item)
		{
			int idx;
			if (_colornameToIndexDictionary.TryGetValue(new ColorNameKey(item.Color, item.Name), out idx))
				return idx;
			else
				return -1;
		}

		/// <summary>
		/// Not allowed for a builtin color set. Calling this function will throw an <see cref="InvalidOperationException"/>.
		/// </summary>
		/// <param name="index">Unused.</param>
		/// <param name="item">Unused.</param>
		public void Insert(int index, NamedColor item)
		{
			throw new InvalidOperationException("This is a read-only collection");
		}

		/// <summary>
		/// Not allowed for a builtin color set. Calling this function will throw an <see cref="InvalidOperationException"/>.
		/// </summary>
		/// <param name="index">Unused.</param>
		public void RemoveAt(int index)
		{
			throw new InvalidOperationException("This is a read-only collection");
		}

		/// <summary>
		/// Gets the <see cref="Altaxo.Graph.NamedColor"/> at the specified index. The set operation is not allowed for a builtin color set and will throw an <see cref="InvalidOperationException"/>.
		/// </summary>
		public NamedColor this[int index]
		{
			get
			{
				return _innerList[index];
			}
			set
			{
				throw new InvalidOperationException("This is a read-only collection");
			}
		}

		#endregion IPlotColorSet

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
				namedColor = _innerList[idx];
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
		/// <c>True</c>, if the color with the given name is contained in this collection. Otherwise, <c>false</c> is returned.
		/// </returns>
		public bool TryGetValue(AxoColor colorValue, out NamedColor namedColor)
		{
			int idx;
			if (_colorToIndexDictionary.TryGetValue(colorValue, out idx))
			{
				namedColor = _innerList[idx];
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
			if (_colornameToIndexDictionary.TryGetValue(new ColorNameKey(colorValue, colorName), out idx))
			{
				namedColor = _innerList[idx];
				return true;
			}
			else
			{
				namedColor = default(NamedColor);
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is used as a plot color set.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is a plot color set; otherwise, <c>false</c>.
		/// </value>
		public abstract bool IsPlotColorSet { get; }

		/// <summary>
		/// Not allowed for a builtin color set. Will throw an <see cref="InvalidOperationException"/>.
		/// </summary>
		public void DeclareThisSetAsPlotColorSet()
		{
			if (!IsPlotColorSet)
				throw new InvalidOperationException("This is a built-in color set that can not be declared as plot color set");
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
			return HaveSameNamedColors(this, other);
		}

		private static System.Security.Cryptography.MD5CryptoServiceProvider _md5Evaluator = new System.Security.Cryptography.MD5CryptoServiceProvider();
		private static System.Text.UTF8Encoding _textEncoder = new UTF8Encoding();

		/// <summary>
		/// Computes from a set of named colors an MD5 hash value. The hash value considers both the name and the value of the colors (but not the set the color belongs to).
		/// </summary>
		/// <param name="colors">The enumeration of named colors.</param>
		/// <returns>An hash value that is unique to this enumeration of colors.</returns>
		public static byte[] ComputeNamedColorHash(IEnumerable<NamedColor> colors)
		{
			var stb = new StringBuilder();
			foreach (var color in colors)
			{
				stb.Append(color.Name);
				stb.Append(color.Color.ToInvariantString());
			}

			if (stb.Length == 0)                 // if the enumeration is empty
				stb.Append("EmptyAltaxoColorSet"); // then use this string to compute the hash

			var bytes = _textEncoder.GetBytes(stb.ToString());
			return _md5Evaluator.ComputeHash(bytes);
		}

		public static bool HaveSameNamedColors(IColorSet first, IList<NamedColor> other)
		{
			if (first.Count != other.Count)
				return false;
			int len = first.Count;

			for (int i = 0; i < len; ++i)
				if (first[i].Name != other[i].Name)
					return false;

			for (int i = 0; i < len; ++i)
				if (first[i].Color != other[i].Color)
					return false;

			return true;
		}
	}
}