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

#nullable enable
using System;
using System.Collections.Generic;

namespace Altaxo.Geometry
{
  /// <summary>
  /// A straight line in 2D space.
  /// </summary>
  public struct LineD2D
  {
    private PointD2D _p0;
    private PointD2D _p1;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineD2D"/> struct.
    /// </summary>
    /// <param name="p0">The starting point of the line.</param>
    /// <param name="p1">The end point of the line.</param>
    public LineD2D(PointD2D p0, PointD2D p1)
    {
      _p0 = p0;
      _p1 = p1;
    }

    /// <summary>
    /// Returns a new line with <see cref="P0"/> set to the provided value.
    /// </summary>
    /// <param name="p0">The value for <see cref="P0"/>.</param>
    /// <returns>A new line with <see cref="P0"/> set to the provided value.</returns>
    public LineD2D WithP0(PointD2D p0)
    {
      return new LineD2D(p0, P1);
    }

    /// <summary>
    /// Returns a new line with <see cref="P1"/> set to the provided value.
    /// </summary>
    /// <param name="p1">The value for <see cref="P1"/>.</param>
    /// <returns>A new line with <see cref="P1"/> set to the provided value.</returns>
    public LineD2D WithP1(PointD2D p1)
    {
      return new LineD2D(P0, p1);
    }

    /// <summary>
    /// Gets the starting point of the line.
    /// </summary>
    /// <value>
    /// The starting point of the line.
    /// </value>
    public PointD2D P0 { get { return _p0; } }

    /// <summary>
    /// Gets the end point of the line.
    /// </summary>
    /// <value>
    /// The end point of the line.
    /// </value>
    public PointD2D P1 { get { return _p1; } }

    /// <summary>
    /// Gets the vector from <see cref="P0"/> to <see cref="P1"/>.
    /// </summary>
    /// <value>
    /// The vector from <see cref="P0"/> to <see cref="P1"/>.
    /// </value>
    public VectorD2D Vector { get { return (VectorD2D)(_p1 - _p0); } }

    /// <summary>
    /// Gets the length of the line.
    /// </summary>
    /// <value>
    /// The length of the line.
    /// </value>
    public double Length { get { return ((VectorD2D)(_p1 - _p0)).Length; } }

    /// <summary>
    /// Gets the vector from <see cref="P0"/> to <see cref="P1"/>.
    /// </summary>
    /// <value>
    /// The vector from <see cref="P0"/> to <see cref="P1"/>.
    /// </value>
    public VectorD2D LineVector { get { return (VectorD2D)(_p1 - _p0); } }

    /// <summary>
    /// Gets the normalized vector from <see cref="P0"/> to <see cref="P1"/>.
    /// </summary>
    /// <value>
    /// The normalized vector from <see cref="P0"/> to <see cref="P1"/>.
    /// </value>
    public VectorD2D LineVectorNormalized { get { return VectorD2D.CreateNormalized(_p1.X - _p0.X, _p1.Y - _p0.Y); } }

    /// <summary>
    /// Gets the points of the line as enumeration.
    /// </summary>
    /// <returns>Enumeration of the two points of this line.</returns>
    public IEnumerable<PointD2D> GetPoints()
    {
      yield return _p0;
      yield return _p1;
    }

    /// <summary>
    /// Gets the point at this line from a relative value.
    /// </summary>
    /// <param name="relativeValue">The relative value. If 0, the start point <see cref="P0"/> is returned. If 1, the end point <see cref="P1"/> is returned.</param>
    /// <returns>The point at the specified relative value along the line.</returns>
    public PointD2D GetPointAtLineFromRelativeValue(double relativeValue)
    {
      if (relativeValue == 0)
      {
        return _p0;
      }
      else if (relativeValue == 1)
      {
        return _p1;
      }
      else
      {
        return new PointD2D(
        (1 - relativeValue) * _p0.X + (relativeValue) * _p1.X,
        (1 - relativeValue) * _p0.Y + (relativeValue) * _p1.Y);
      }
    }

    /// <summary>
    /// Calculates the intersection point of two lines.
    /// </summary>
    /// <param name="l0">First line.</param>
    /// <param name="l1">Second line.</param>
    /// <returns>The intersection point of the two lines, or null if no intersection point exists (lines are parallel).</returns>
    public static PointD2D? Intersection(LineD2D l0, LineD2D l1)
    {
      var determinant = (l0.P0.Y - l0.P1.Y) * (l1.P0.X - l1.P1.X) - (l0.P0.X - l0.P1.X) * (l1.P0.Y - l1.P1.Y);

      if (determinant == 0 || double.IsNaN(determinant))
        return null; // lines are parallel

      var t = l0.P0.X * (l1.P1.Y - l1.P0.Y) + l0.P0.Y * (l1.P0.X - l1.P1.X) - l1.P0.X * l1.P1.Y + l1.P0.Y * l1.P1.X;
      t /= determinant;

      return l0.GetPointAtLineFromRelativeValue(t);
    }

    /// <summary>
    /// Creates a new line, which is a rotated version of this line (rotation around a given center point).
    /// </summary>
    /// <param name="center">The center point of the rotation.</param>
    /// <param name="angleInRadian">The angle in radian.</param>
    /// <returns>Rotated version of this line (rotated by angle around the center point).</returns>
    public LineD2D WithRotationRadian(PointD2D center, double angleInRadian)
    {
      var m = Matrix3x2.NewRotationRadian(center, angleInRadian);
      return new LineD2D(m.Transform(P0), m.Transform(P1));
    }

    /// <summary>
    /// Returns a line with opposite direction compared to this line.
    /// </summary>
    /// <returns>Line with the opposite direction.</returns>
    public LineD2D Reversed()
    {
      return new LineD2D(P1, P0);
    }

    /// <summary>
    /// Translate a line by adding a vector to both start and end point.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="translation">The vector to add.</param>
    /// <returns>
    /// The translated line.
    /// </returns>
    public static LineD2D operator +(LineD2D line, VectorD2D translation)
    {
      return new LineD2D(line.P0 + translation, line.P1 + translation);
    }

    /// <summary>
    /// Translate a line by subtracting a vector from both start and end point.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="translation">The vector to subtract.</param>
    /// <returns>
    /// The translated line.
    /// </returns>
    public static LineD2D operator -(LineD2D line, VectorD2D translation)
    {
      return new LineD2D(line.P0 - translation, line.P1 - translation);
    }

    /// <summary>
    /// Transforms a line with the matrix m.
    /// </summary>
    /// <param name="m">The transformation matrix.</param>
    /// <param name="l">The line to transform.</param>
    /// <returns>The transformed line.</returns>
    public static LineD2D Transform(Matrix3x2 m, LineD2D l)
    {
      return new LineD2D(m.Transform(l.P0), m.Transform(l.P1));
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return FormattableString.Invariant($"{nameof(LineD2D)}(({P0.X}, {P0.Y}); ({P1.X}, {P1.Y}))");
    }
  }
}
