#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

namespace Altaxo.Geometry
{
	/// <summary>
	/// A straight line in 3D space.
	/// </summary>
	public struct LineD3D
	{
		private PointD3D _p0;
		private PointD3D _p1;

		/// <summary>
		/// Initializes a new instance of the <see cref="LineD3D"/> struct.
		/// </summary>
		/// <param name="p0">The starting point of the line.</param>
		/// <param name="p1">The end point of the line.</param>
		public LineD3D(PointD3D p0, PointD3D p1)
		{
			_p0 = p0;
			_p1 = p1;
		}

		/// <summary>
		/// Gets the starting point of the line.
		/// </summary>
		/// <value>
		/// The starting point of the line.
		/// </value>
		public PointD3D P0 { get { return _p0; } }

		/// <summary>
		/// Gets the end point of the line.
		/// </summary>
		/// <value>
		/// The end point of the line.
		/// </value>
		public PointD3D P1 { get { return _p1; } }
	}
}