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

using Altaxo.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Geometry
{
	/// <summary>
	/// Implementation of a closed polygon in 2D space.
	/// </summary>
	public class PolygonD2D
	{
		protected PointD2D[] _points;

		protected HashSet<PointD2D> _sharpPoints;

		protected bool? _isHole;

		/// <summary>
		/// Gets or sets the parent of this polygon.
		/// </summary>
		/// <value>
		/// The parent.
		/// </value>
		public PolygonD2D Parent { get; set; }

		public PolygonD2D(PointD2D[] points, HashSet<PointD2D> sharpPoints)
		{
			_points = points;
			_sharpPoints = sharpPoints;
		}

		public PolygonD2D(PolygonD2D template, double scale)
		{
			_points = template._points.Select(p => new PointD2D(p.X * scale, p.Y * scale)).ToArray();
			_sharpPoints = new HashSet<PointD2D>(template._sharpPoints.Select(p => new PointD2D(p.X * scale, p.Y * scale)));
		}

		/// <summary>
		/// Gets the points that form the closed polygon.
		/// </summary>
		/// <value>
		/// The points.
		/// </value>
		public PointD2D[] Points { get { return _points; } }

		/// <summary>
		/// Gets the points of the polygon which are sharp points. Points of the polygon which are not in this set are considered to be soft points.
		/// </summary>
		/// <value>
		/// The sharp points.
		/// </value>
		public HashSet<PointD2D> SharpPoints { get { return _sharpPoints; } }

		/// <summary>
		/// Gets or sets a value indicating whether this polygon is a hole. In this case it belongs to another, outer, polygon.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is a hole; otherwise, <c>false</c>.
		/// </value>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool IsHole
		{
			get
			{
				if (!_isHole.HasValue)
				{
					throw new NotImplementedException();
					//return PolygonHelper.GetPolygonArea(_points) > 0;
				}
				else
				{
					return _isHole.Value;
				}
			}
			set
			{
				_isHole = value;
			}
		}
	}
}