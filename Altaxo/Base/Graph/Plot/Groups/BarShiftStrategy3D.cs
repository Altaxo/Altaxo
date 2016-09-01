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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
	/// <summary>
	/// Designates how to shift the bars that belong to a group (i.e. have the same independent variables x and y).
	/// </summary>
	public enum BarShiftStrategy3D
	{
		/// <summary>Shift the bars first in x-direction from left to right. If the user defined maximum number of bars in x-direction is reached, the bars are shifted in y-direction from front to back.</summary>
		ManualFirstXThenY = 0,

		/// <summary>Shift the bars first in y-direction from front to back. If the user defined maximum number of bars in y-direction is reached, the bars are shifted in x-direction from left to right.</summary>
		ManualFirstYThenX = 1,

		/// <summary>Try to make the number of bars in x and y direction the same (square root of the total number). Shift the bars first in x-direction from left to right. If the destination number of bars in x-direction is reached, the bars are shifted in y-direction from front to back.</summary>
		UniformFirstXThenY = 2,

		/// <summary>Try to make the number of bars in x and y direction the same (square root of the total number). Shift the bars first in y-direction from front to back. If the destination number of bars in y-direction is reached, the bars are shifted in x-direction from left to right.</summary>
		UniformFirstYThenX = 3
	}
}