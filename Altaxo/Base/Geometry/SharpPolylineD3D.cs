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

		public double TotalLineLength
		{
			get
			{
				double sum = 0;
				for (int i = 1; i < _points.Length; ++i)
					sum += (_points[i] - _points[i - 1]).Length;

				return sum;
			}
		}

		public IPolylineD3D ShortenedBy(RADouble marginAtStart, RADouble marginAtEnd)
		{
			if (_points.Length < 2)
				return null;

			double totLength = TotalLineLength;

			double a1 = marginAtStart.IsAbsolute ? marginAtStart.Value : marginAtStart.Value * totLength;
			double a2 = marginAtEnd.IsAbsolute ? marginAtEnd.Value : marginAtEnd.Value * totLength;

			if (!((a1 + a2) < totLength))
				return null;

			PointD3D? p0 = null;
			PointD3D? p1 = null;
			int i0 = 0;
			int i1 = 0;

			if (a1 <= 0)
			{
				p0 = PointD3D.Interpolate(_points[0], _points[1], a1 / totLength);
			}
			else
			{
				double sum = 0, prevSum = 0;
				for (int i = 1; i < _points.Length; ++i)
				{
					sum += (_points[i] - _points[i - 1]).Length;
					if (!(sum < a1))
					{
						p0 = PointD3D.Interpolate(_points[i - 1], _points[i], (a1 - prevSum) / (sum - prevSum));
						i0 = p0 != _points[i] ? i : i + 1;
						break;
					}
					prevSum = sum;
				}
			}

			if (a2 <= 0)
			{
				p1 = PointD3D.Interpolate(_points[_points.Length - 2], _points[_points.Length - 1], 1 - a2 / totLength);
			}
			else
			{
				double sum = 0, prevSum = 0;
				for (int i = _points.Length - 2; i >= 0; --i)
				{
					sum += (_points[i] - _points[i + 1]).Length;
					if (!(sum < a2))
					{
						p1 = PointD3D.Interpolate(_points[i + 1], _points[i], (a1 - prevSum) / (sum - prevSum));
						i1 = p1 != _points[i] ? i : i - 1;
						break;
					}
					prevSum = sum;
				}
			}

			if (p0.HasValue && p1.HasValue)
			{
				var plist = new List<PointD3D>();
				plist.Add(p0.Value);
				for (int i = i0; i <= i1; ++i)
					plist.Add(_points[i]);
				plist.Add(p1.Value);
				return new SharpPolylineD3D(plist.ToArray());
			}
			else
			{
				return null;
			}
		}
	}
}