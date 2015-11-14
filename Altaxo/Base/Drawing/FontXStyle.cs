#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Drawing
{
	/// <summary>
	/// Font styles that are numerically compatible to the <see cref="System.Drawing.FontStyle"/> enumeration.
	/// </summary>
	[Flags]
	public enum FontXStyle
	{
		/// <summary>Normal text.</summary>
		Regular = 0,

		/// <summary>Bold text.</summary>
		Bold = 1,

		/// <summary>Italic text.</summary>
		Italic = 2,

		/// <summary>Underlined text.</summary>
		Underline = 4,

		/// <summary>Striked out text.</summary>
		Strikeout = 8
	}

	/// <summary>
	/// Designates whether for a given font the specified styles are available.
	/// </summary>
	[Flags]
	public enum FontStylePresence
	{
		/// <summary>No style is available at all.</summary>
		NoStyleAvailable = 0,

		/// <summary>A regular style is available for the font.</summary>
		RegularStyleAvailable = 1,

		/// <summary>The bold style is available for the font.</summary>
		BoldStyleAvailable = 2,

		/// <summary>The italic style is available for the font.</summary>
		ItalicStyleAvailable = 4,

		/// <summary>The bold and italic style is available for the font.</summary>
		BoldAndItalicStyleAvailable = 8
	};
}