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

using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui
{
	/// <summary>
	/// Specifies constants that define which mouse button was pressed.
	/// </summary>
	[Flags]
	public enum AltaxoMouseButtons
	{
		/// <summary>No mouse button was pressed.</summary>
		None,

		/// <summary>The left mouse button was pressed.</summary>
		Left, //

		/// <summary>The middle mouse button was pressed. </summary>
		Middle,

		/// <summary>The right mouse button was pressed.</summary>
		Right,

		/// <summary>The first XButton was pressed.</summary>
		XButton1,

		/// <summary>The second XButton was pressed. </summary>
		XButton2,
	}

	/// <summary>Provides data for the MouseUp, MouseDown, and MouseMove events.</summary>
	public class AltaxoMouseEventArgs
	{
		/// <summary>Gets which mouse button was pressed.</summary>
		public AltaxoMouseButtons Button { get; set; }

		/// <summary>Gets the number of times the mouse button was pressed and released. </summary>
		public int Clicks { get; set; }

		/// <summary>Gets a signed count of the number of detents the mouse wheel has rotated. A detent is one notch of the mouse wheel. </summary>
		public int Delta { get; set; }

		/// <summary>Gets the x-coordinate of the mouse during the generating mouse event.</summary>
		public double X { get; set; }

		/// <summary>Gets the y-coordinate of the mouse during the generating mouse event. </summary>
		public double Y { get; set; }

		public PointD2D Position { get { return new PointD2D(X, Y); } set { X = value.X; Y = value.Y; } }
	}
}