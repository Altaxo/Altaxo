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
	/// Set of colors with a given (<see cref="Name"/>) and <see cref="ColorSetLevel"/>. This set can be writeable or not (see <see cref="P:IList{T}.IsReadOnly"/>).
	/// </summary>
	public interface IColorSet : Altaxo.Graph.IStyleList<NamedColor>
	{
		/// <summary>
		/// Tries to find a color with a given name.
		/// </summary>
		/// <param name="colorName">The name of the color to find.</param>
		/// <param name="namedColor">If the color is contained in this collection, the color with the given name is returned. Otherwise, the result is undefined.</param>
		/// <returns><c>True</c>, if the color with the given name is contained in this collection. Otherwise, <c>false</c> is returned.</returns>
		bool TryGetValue(string colorName, out NamedColor namedColor);

		/// <summary>
		/// Tries to find a color with a given color value.
		/// </summary>
		/// <param name="colorValue">The color to find.</param>
		/// <param name="namedColor">If the color is contained in this collection, the color with the given name is returned. Otherwise, the result is undefined.</param>
		/// <returns><c>True</c>, if the color with the given name is contained in this collection. Otherwise, <c>false</c> is returned.</returns>
		bool TryGetValue(AxoColor colorValue, out NamedColor namedColor);

		/// <summary>
		/// Tries to find a color with the color and the name of the given named color.
		/// </summary>
		/// <param name="colorValue">The color value to find.</param>
		/// <param name="colorName">The color name to find.</param>
		/// <param name="namedColor">On return, if a color with the same color value and name is found in the collection, that color is returned.</param>
		/// <returns><c>True</c>, if the color with the given color value and name is found in this collection. Otherwise, <c>false</c> is returned.</returns>
		bool TryGetValue(AxoColor colorValue, string colorName, out NamedColor namedColor);

		/// <summary>
		/// Get the indexes of a color with a given color value.
		/// </summary>
		/// <param name="colorValue">The color value.</param>
		/// <returns>The index of the color in this color set, if a such a color was found in the set. Otherwise, a negative value is returned.</returns>
		int IndexOf(AxoColor colorValue);
	}
}