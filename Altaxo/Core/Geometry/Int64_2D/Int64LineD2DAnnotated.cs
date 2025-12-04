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
using System.Collections.Generic;
using Clipper2Lib;

namespace Altaxo.Geometry.Int64_2D
{
  /// <summary>
  /// Represents a line segment, using Points with Int64 coordinates. The points are annotated with integer numbers, which usually represents indices in a list or array.
  /// </summary>
  public readonly struct Int64LineD2DAnnotated
  {
    /// <summary>
    /// Start point of the line segment.
    /// </summary>
    public readonly Point64 P0;

    /// <summary>
    /// Gets the index that is associated with the start point <see cref="P0"/>.
    /// </summary>
    public readonly int I0;

    /// <summary>
    /// End point of the line segment.
    /// </summary>
    public readonly Point64 P1;

    /// <summary>
    /// Gets the index that is associated with the end point <see cref="P1"/>.
    /// </summary>
    public readonly int I1;

    /// <summary>
    /// Gets the length of the line segment. The length is calculated once in the constructor, so frequent use does not imply any computational penalty.
    /// </summary>
    public readonly double Length;

    /// <summary>
    /// Initializes a new instance of the <see cref="Int64LineD2DAnnotated"/> class.
    /// </summary>
    /// <param name="p0">The start point (including its index).</param>
    /// <param name="p1">The end point (including its index).</param>
    public Int64LineD2DAnnotated((Point64 point, int index) p0, (Point64 point, int index) p1)
    {
      P0 = p0.point;
      I0 = p0.index;
      P1 = p1.point;
      I1 = p1.index;

      var dx = (double)(P1.X - P0.X);
      var dy = (double)(P1.Y - P0.Y);
      Length = Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Gets the line segment as an <see cref="Int64LineSegment"/>.
    /// </summary>
    public Int64LineSegment Line
    {
      get
      {
        return new Int64LineSegment(P0, P1);
      }
    }

    /// <summary>
    /// Enumerates the two points <see cref="P0"/> and <see cref="P1"/>.
    /// </summary>
    public IEnumerable<Point64> Points
    {
      get
      {
        yield return P0;
        yield return P1;
      }
    }

    /// <summary>
    /// Gets either the point <see cref="P0"/> at index 0, or the point <see cref="P1"/> at index 1.
    /// </summary>
    /// <param name="idx">The index.</param>
    /// <returns>Either the point <see cref="P0"/> (idx==0), or the point <see cref="P1"/> (idx==1).</returns>
    /// <exception cref="IndexOutOfRangeException">Index out of range [0,1]</exception>
    public Point64 GetPoint(int idx)
    {
      switch (idx)
      {
        case 0:
          return P0;
        case 1:
          return P1;
        default:
          throw new IndexOutOfRangeException("Index out of range [0,1]");
      }
    }

    /// <summary>
    /// Gets either the start point's index <see cref="I0"/> at index 0, or the end point's index <see cref="I1"/> at index 1.
    /// </summary>
    /// <param name="idx">The retrieving index.</param>
    /// <returns>Either the start point's index <see cref="I0"/> (idx==0), or the end point's index <see cref="I1"/> (idx==1).</returns>
    /// <exception cref="IndexOutOfRangeException">Index out of range [0,1]</exception>
    public int GetIndex(int idx)
    {
      switch (idx)
      {
        case 0:
          return I0;
        case 1:
          return I1;
        default:
          throw new IndexOutOfRangeException("Index out of range [0,1]");
      }
    }

    /// <summary>
    /// Gets either the start point with associated index at idx 0, or the end point with associated index  at idx 1.
    /// </summary>
    /// <param name="idx">The retrieving index.</param>
    /// <returns>Either the start point with associated index  (idx==0), or the end point with associated index  (idx==1).</returns>
    /// <exception cref="IndexOutOfRangeException">Index out of range [0,1]</exception>
    public (Point64 point, int index) GetPointWithIndex(int idx)
    {
      switch (idx)
      {
        case 0:
          return (P0, I0);
        case 1:
          return (P1, I1);
        default:
          throw new IndexOutOfRangeException("Index out of range [0,1]");
      }
    }
  }

  /// <summary>
  /// Represents a rectangular bounding box using two <see cref="Point64"/> coordinates.
  /// </summary>
  public readonly struct Int64BoundingBox
  {
    /// <summary>
    /// The lower-left corner of the bounding box.
    /// </summary>
    public readonly Point64 P0;
    /// <summary>
    /// The upper-right corner of the bounding box.
    /// </summary>
    public readonly Point64 P1;

    /// <summary>
    /// Initializes a new instance of the <see cref="Int64BoundingBox"/> struct.
    /// </summary>
    /// <param name="p0">The lower-left corner.</param>
    /// <param name="p1">The upper-right corner.</param>
    /// <exception cref="ArgumentException">Thrown if p0 is not less than or equal to p1 in both coordinates.</exception>
    public Int64BoundingBox(Point64 p0, Point64 p1)
    {
      if (!(p0.X <= p1.X && p0.Y <= p1.Y))
      {
        throw new ArgumentException();
      }

      P0 = p0;
      P1 = p1;
    }

    /// <summary>
    /// Determines whether the specified point is within the bounding box.
    /// </summary>
    /// <param name="p">The point to test.</param>
    /// <returns>True if the point is within the bounding box; otherwise, false.</returns>
    public bool IsPointWithin(in Point64 p)
    {
      return
           p.X >= P0.X
        && p.Y >= P0.Y
        && p.X <= P1.X
        && p.Y <= P1.Y;
    }
  }
}
