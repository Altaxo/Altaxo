#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable enable
using System;
using Clipper2Lib;

namespace Altaxo.Geometry.Int64_2D
{
  /// <summary>
  /// Represents a line segment defined by two 64-bit integer points in 2D space.
  /// </summary>
  public readonly struct Int64LineSegment
  {
    /// <summary>
    /// The first endpoint of the line segment.
    /// </summary>
    public readonly Point64 P0;
    /// <summary>
    /// The second endpoint of the line segment.
    /// </summary>
    public readonly Point64 P1;

    /// <summary>
    /// Initializes a new instance of the <see cref="Int64LineSegment"/> struct.
    /// </summary>
    /// <param name="p0">The first endpoint.</param>
    /// <param name="p1">The second endpoint.</param>
    public Int64LineSegment(Point64 p0, Point64 p1)
    {
      P0 = p0;
      P1 = p1;
    }

    /// <summary>
    /// Gets the Euclidean distance between two points.
    /// </summary>
    /// <param name="p0">The first point.</param>
    /// <param name="p1">The second point.</param>
    /// <returns>The distance between <paramref name="p0"/> and <paramref name="p1"/>.</returns>
    public static double GetDistance(Point64 p0, Point64 p1)
    {
      var dx = (double)(p1.X - p0.X);
      var dy = (double)(p1.Y - p0.Y);
      return Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Gets the cosine of the angle between the lines pivot-a and pivot-b.
    /// </summary>
    /// <param name="pivot">The pivot point.</param>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <returns>The cosine of the angle between the two lines.</returns>
    public static double GetCosOfAngle(Point64 pivot, Point64 a, Point64 b)
    {
      var rX = (double)(a.X - pivot.X);
      var rY = (double)(a.Y - pivot.Y);
      var sX = (double)(b.X - pivot.X);
      var sY = (double)(b.Y - pivot.Y);
      var z = (rX * sX + rY * sY) / Math.Sqrt((rX * rX + rY * rY) * (sX * sX + sY * sY));
      return z;
    }

    /// <summary>
    /// Gets the angle between the lines pivot-a and pivot-b.
    /// </summary>
    /// <param name="pivot">The pivot point.</param>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <param name="returnPositiveValueIf180Degrees">If both lines are antiparallel (180 degrees), the angle is not unique (can be -Pi or +Pi). If this parameter is set to true, then a positive angle (+Pi) will be returned; otherwise, a negative angle (-Pi) is returned.</param>
    /// <returns>The angle between the two lines in radians.</returns>
    public static double GetAngle(Point64 pivot, Point64 a, Point64 b, bool returnPositiveValueIf180Degrees)
    {
      var aX = (double)(a.X - pivot.X);
      var aY = (double)(a.Y - pivot.Y);
      var bX = (double)(b.X - pivot.X);
      var bY = (double)(b.Y - pivot.Y);
      var d1 = aX * bY - aY * bX;
      var d2 = aX * bX + aY * bY;

      if (d1 == 0 && d2 < 0 && returnPositiveValueIf180Degrees)
        return Math.PI;
      else
        return Math.Atan2(d1, d2);
    }
  }
}
