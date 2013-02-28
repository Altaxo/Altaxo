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
	/// Hierarchy level of the color set.
	/// </summary>
	public enum ColorSetLevel
	{
		/// <summary>The color set is built-in, i.e. hard coded in the source code of Altaxo.</summary>
		Builtin = 0,
		/// <summary>The color set is defined on the application level, i.e. for instance in an .addin file.</summary>
		Application = 1,
		/// <summary>The color set is defined on the user level. Those sets are usually stored in the user's profile.</summary>
		UserDefined = 2,
		/// <summary>The color set is defined on the project level.</summary>
		Project = 3 
	}


	/// <summary>
	/// Set of colors with a given (<see cref="Name"/>) and <see cref="ColorSetLevel"/>. This set can be writeable or not (see <see cref="P:IList{T}.IsReadOnly"/>).
	/// </summary>
	public interface IColorSet : IList<NamedColor>
	{
		/// <summary>Gets the name of the color set (without any prefix like 'Builtin', 'Application' etc.).</summary>
		string Name { get; }

		/// <summary>Gets the hierarchy level of the color set (see <see cref="ColorSetLevel"/>).</summary>
		ColorSetLevel Level { get; }

		/// <summary>
		/// Gets a value indicating whether this instance is used as a plot color set.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is a plot color set; otherwise, <c>false</c>.
		/// </value>
		bool IsPlotColorSet { get; }


		/// <summary>
		/// Declares the this set as plot color set. This decision is not reversable.
		/// </summary>
		void DeclareThisSetAsPlotColorSet();

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

		/// <summary>
		/// Adds a color with the specified color value and name to the collection.
		/// </summary>
		/// <param name="colorValue">The color value.</param>
		/// <param name="name">The name of the color.</param>
		/// <returns>The freshly added named color with the color value and name provided by the arguments.</returns>
		NamedColor Add(AxoColor colorValue, string name);

    /// <summary>
    /// Determines whether this color set has the same colors (matching by name and color value, and index) as another set.
    /// </summary>
    /// <param name="other">The other set to compare with.</param>
    /// <returns>
    ///   <c>true</c> if this set has the same colors as the other set; otherwise, <c>false</c>.
    /// </returns>
    bool HasSameContentAs(IList<NamedColor> other);
	}
}
