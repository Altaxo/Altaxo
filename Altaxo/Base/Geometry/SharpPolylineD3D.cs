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

using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Geometry
{
	/// <summary>
	/// Represents a polyline, i.e. a line consisting of multiple line segments, described by their points. All joints are assumed to be sharp joints.
	/// </summary>
	public class SharpPolylineD3D : IPolylineD3D
	{
		private PointD3D[] _points;

		public SharpPolylineD3D(PointD3D[] points)
		{
			_points = points;
		}

		public PointD3D GetPoint(int idx)
		{
			return _points[idx];
		}

		/// <summary>
		/// Gets the number of points.
		/// </summary>
		/// <value>
		/// Number of points.
		/// </value>
		public int Count { get { return _points.Length; } }

		/// <summary>
		/// Gets the points of this polyline. No information is contained here whether the joints are sharp or soft.
		/// </summary>
		/// <value>
		/// The points that make out the polyline.
		/// </value>
		public IList<PointD3D> Points { get { return _points; } }

		public bool IsTransitionFromIdxToNextIdxSharp(int idx)
		{
			return true;
		}
	}
}