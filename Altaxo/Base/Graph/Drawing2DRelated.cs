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
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph
{
	/// <summary>
	/// Drawing2DRelated contains static methods related to mathematics and helper
	/// functions for classed from the System.Drawing namespace.
	/// </summary>
	public static class Drawing2DRelated
	{

		/// <summary>
		/// Calculates the distance between two points.
		/// </summary>
		/// <param name="p1">First point.</param>
		/// <param name="p2">Second point.</param>
		/// <returns>The distance between points p1 and p2.</returns>
		public static double Distance(PointD2D p1, PointD2D p2)
		{
			double x = p1.X - p2.X;
			double y = p1.Y - p2.Y;
			return Math.Sqrt(x * x + y * y);
		}

		/// <summary>
		/// Calculates the squared distance between a finite line and a point.
		/// </summary>
		/// <param name="point">The location of the point.</param>
		/// <param name="lineOrg">The location of the line origin.</param>
		/// <param name="lineEnd">The location of the line end.</param>
		/// <returns>The squared distance between the line (threated as having a finite length) and the point.</returns>
		public static double SquareDistanceLineToPoint(PointD2D point, PointD2D lineOrg, PointD2D lineEnd)
		{
			var linex = lineEnd.X - lineOrg.X;
			var liney = lineEnd.Y - lineOrg.Y;
			var pointx = point.X - lineOrg.X;
			var pointy = point.Y - lineOrg.Y;

			var rsquare = linex * linex + liney * liney;
			var xx = linex * pointx + liney * pointy;
			if (xx <= 0) // the point is located before the line, so use
			{         // the distance of the line origin to the point
				return pointx * pointx + pointy * pointy;
			}
			else if (xx >= rsquare) // the point is located after the line, so use
			{                   // the distance of the line end to the point
				pointx = point.X - lineEnd.X;
				pointy = point.Y - lineEnd.Y;
				return pointx * pointx + pointy * pointy;
			}
			else // the point is located in the middle of the line, use the
			{     // distance from the line to the point
				var yy = liney * pointx - linex * pointy;
				return yy * yy / rsquare;
			}
		}

		/// <summary>
		/// Determines whether or not a given point (<c>point</c>) is into a <c>distance</c> to a finite line, that is spanned between
		/// two points <c>lineOrg</c> and <c>lineEnd</c>.
		/// </summary>
		/// <param name="point">Point under test.</param>
		/// <param name="distance">Distance.</param>
		/// <param name="lineOrg">Starting point of the line.</param>
		/// <param name="lineEnd">End point of the line.</param>
		/// <returns>True if the distance between point <c>point</c> and the line between <c>lineOrg</c> and <c>lineEnd</c> is less or equal to <c>distance</c>.</returns>
		public static bool IsPointIntoDistance(PointD2D point, double distance, PointD2D lineOrg, PointD2D lineEnd)
		{
			// first a quick test if the point is far outside the circle
			// that is spanned from the middle of the line and has at least
			// a radius of half of the line length plus the distance
			var xm = (lineOrg.X + lineEnd.X) / 2;
			var ym = (lineOrg.Y + lineEnd.Y) / 2;
			var r = Math.Abs(lineOrg.X - xm) + Math.Abs(lineOrg.Y - ym) + distance;
			if (Math.Max(Math.Abs(point.X - xm), Math.Abs(point.Y - ym)) > r)
				return false;
			else
				return SquareDistanceLineToPoint(point, lineOrg, lineEnd) <= distance * distance;

		}
	}
}
