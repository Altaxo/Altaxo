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

using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Geometry
{
	/// <summary>
	/// Represents a polyline, i.e. a line consisting of multiple line segments. This class contains additional information whether the joints between the line segments
	/// are sharp or soft.
	/// </summary>
	public interface IPolylineD3D
	{
		PointD3D GetPoint(int idx);

		/// <summary>
		/// Gets the number of points.
		/// </summary>
		/// <value>
		/// Number of points.
		/// </value>
		int Count { get; }

		/// <summary>
		/// Gets the points of this polyline. No information is contained here whether the joints are sharp or soft.
		/// </summary>
		/// <value>
		/// The points that make out the polyline.
		/// </value>
		IEnumerable<PointD3D> Points { get; }

		bool IsTransitionFromIdxToNextIdxSharp(int idx);
	}
}