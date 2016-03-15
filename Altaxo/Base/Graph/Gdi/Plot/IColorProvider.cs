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

namespace Altaxo.Graph.Gdi.Plot
{
	/// <summary>
	/// Interface to calculate a color out of a relative value that is normally
	/// between 0 and 1. Special colors should be used for values between 0, above 1, and for NaN.
	/// </summary>
	public interface IColorProvider : Main.IImmutable
	{
		/// <summary>
		/// Calculates a color from the provided relative value.
		/// </summary>
		/// <param name="relVal">Value used for color calculation. Normally between 0 and 1.</param>
		/// <returns>A color associated with the relative value.</returns>
		System.Drawing.Color GetColor(double relVal);
	}
}