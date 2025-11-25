#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

#if false
using System;
using System.Numerics;
using Altaxo.Calc;

namespace Altaxo.Geometry.DoubleInterval
{
  /// <summary>
  /// Represents a point with values of type Double in 2D space.
  /// </summary>
  public struct PointDInterval2D<T> where T: unmanaged, IFloatingPoint<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
  {
    /// <summary>
    /// Gets the x component of this point.
    /// </summary>
    /// <value>
    /// The x  component of this point..
    /// </value>
    public Interval<T> X { get; private set; }

    /// <summary>
    /// Gets the y component of this point.
    /// </summary>
    /// <value>
    /// The y  component of this point..
    /// </value>
    public Interval<T> Y { get; private set; }

    public PointDInterval2D(Interval<T> x, Interval<T> y)
    {
      X = x;
      Y = y;
    }

    public PointDInterval2D(T x, T y)
    {
      X = Interval<T>.From(x);
      Y = Interval<T>.From(y);
    }

    public void Deconstruct(out Interval<T> x, out Interval<T> y)
    {
      x = X;
      y = Y;
    }

    /// <summary>
    /// Returns a new instance with <see cref="X"/> set to the provided value.
    /// </summary>
    /// <param name="newX">The new x.</param>
    /// <returns>New instance with <see cref="X"/> set to the provided value.</returns>
    public PointDInterval2D<T> WithX(T newX)
    {
      return new PointDInterval2D<T>(Interval<T>.From(newX), Y);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Y"/> set to the provided value.
    /// </summary>
    /// <param name="newY">The new x.</param>
    /// <returns>New instance with <see cref="Y"/> set to the provided value.</returns>
    public PointDInterval2D<T> WithY(T newY)
    {
      return new PointDInterval2D<T>(X, Interval<T>.From(newY));
    }

    /// <summary>
    /// Returns a new instance with <see cref="X"/> set to the X plus the provided value.
    /// </summary>
    /// <param name="addX">The value to add to <see cref="X"/>.</param>
    /// <returns>New instance with <see cref="X"/> set to the X plus the provided value.</returns>
    public PointDInterval2D<T> WithXPlus(T addX)
    {
      return new PointDInterval2D<T>(X + Interval<T>.From(addX), Y);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Y"/> set to the Y plus the provided value.
    /// </summary>
    /// <param name="addY">The value to add to <see cref="Y"/>.</param>
    /// <returns>New instance with <see cref="Y"/> set to the Y plus the provided value.</returns>
    public PointDInterval2D<T> WithYPlus(T addY)
    {
      return new PointDInterval2D<T>(X, Y + Interval<T>.From(addY));
    }

    public static PointDInterval2D<T> Empty
    {
      get
      {
        return new PointDInterval2D<T>(T.Zero, T.Zero);
      }
    }

    public bool IsEmpty
    {
      get
      {
        return T.Zero == X && T.Zero == Y;
      }
    }

    /// <summary>
    /// Gets a value indicating whether one of the members of this instance is <see cref="T.NaN"/>.
    /// </summary>
    /// <value>
    ///   <c>true</c> if one of the members of this instance is <see cref="T.NaN"/>; otherwise, <c>false</c>.
    /// </value>
    public bool IsNaN
    {
      get
      {
        return T.IsNaN(X.Min) || T.IsNaN(Y.Min);
      }
    }

    #region Operators

    public static PointDInterval2D<T> operator -(PointDInterval2D<T> p1, PointDInterval2D<T> p2)
    {
      return new PointDInterval2D<T>(p1.X - p2.X, p1.Y - p2.Y);
    }

    public static PointDInterval2D<T> operator -(PointDInterval2D<T> p1, VectorD2D p2)
    {
      return new PointDInterval2D<T>(p1.X - p2.X, p1.Y - p2.Y);
    }

    public static PointDInterval2D<T> operator +(PointDInterval2D<T> p1, PointDInterval2D<T> p2)
    {
      return new PointDInterval2D<T>(p1.X + p2.X, p1.Y + p2.Y);
    }

    public static PointDInterval2D<T> operator +(PointDInterval2D<T> p1, VectorD2D p2)
    {
      return new PointDInterval2D<T>(p1.X + p2.X, p1.Y + p2.Y);
    }

    public static PointDInterval2D<T> operator *(PointDInterval2D<T> p, T s)
    {
      return new PointDInterval2D<T>(p.X * s, p.Y * s);
    }

    public static PointDInterval2D<T> operator *(T s, PointDInterval2D<T> p)
    {
      return new PointDInterval2D<T>(p.X * s, p.Y * s);
    }

    public static PointDInterval2D<T> operator *(PointDInterval2D<T> p, PointDInterval2D<T> q)
    {
      return new PointDInterval2D<T>(p.X * q.X, p.Y * q.Y);
    }

    public static PointDInterval2D<T> operator /(PointDInterval2D<T> p, T s)
    {
      return new PointDInterval2D<T>(p.X / s, p.Y / s);
    }

    public static bool operator ==(PointDInterval2D<T> p, PointDInterval2D<T> q)
    {
      return p.X == q.X && p.Y == q.Y;
    }

    public static bool operator !=(PointDInterval2D<T> p, PointDInterval2D<T> q)
    {
      return !(p.X == q.X && p.Y == q.Y);
    }

    #endregion Operators

    #region Conversion operators

    public static explicit operator PointDInterval2D<T>(VectorD2D v)
    {
      return new PointDInterval2D<T>(v.X, v.Y);
    }

    #endregion Conversion operators

    public override int GetHashCode()
    {
      return X.GetHashCode() + 7 * Y.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
      if (obj is PointDInterval2D<T> q)
      {
        return X == q.X && Y == q.Y;
      }
      else
      {
        return false;
      }
    }

    public override string ToString()
    {
      return string.Format(System.Globalization.CultureInfo.InvariantCulture,
        "PointD2D({0}, {1})", X, Y);
    }

    /// <summary>
    /// Calculates the distance between this point and another point.
    /// </summary>
    /// <param name="p">Other point.</param>
    /// <returns>The distance between this point and point p.</returns>
    public Interval<T> DistanceTo(PointDInterval2D<T> p)
    {
      return Distance(this, p);
    }

    /// <summary>
    /// Calculates the squared distance between this point and another point.
    /// </summary>
    /// <param name="p">Other point.</param>
    /// <returns>The distance between this point and point p.</returns>
    public Interval<T> DistanceSquaredTo(PointDInterval2D<T> p)
    {
      return DistanceSquared(this, p);
    }

    public Interval<T> VectorLength
    {
      get
      {
        return (X * X + Y * Y).Sqrt();
      }
    }

    public Interval<T> VectorLengthSquared
    {
      get
      {
        return (X * X + Y * Y);
      }
    }

    public PointDInterval2D<T> GetXYFlipped()
    {
      return new PointDInterval2D<T>(Y, X);
    }

    public PointDInterval2D<T> Get90DegreeRotated()
    {
      return new PointDInterval2D<T>(-Y, X);
    }

    public PointDInterval2D<T> GetSignFlipped()
    {
      return new PointDInterval2D<T>(-X, -Y);
    }

    /// <summary>
    /// Returns a new <see cref="PointDInterval2D<T>"/> based on the existing one, but with both Abs(X) instead of X, and Abs(Y) instead of Y.
    /// </summary>
    /// <returns>New <see cref="PointDInterval2D<T>"/> based on the existing one, but with both Abs(X) instead of X, and Abs(Y) instead of Y.</returns>
    public PointDInterval2D<T> GetMemberwiseAbs()
    {
      return new PointDInterval2D<T>(X.Abs(), Y.Abs());
    }

    public PointDInterval2D<T> GetRotatedByDegree(T rotation, PointDInterval2D<T> pivot)
    {
      return GetRotatedByRad(rotation * Math.PI / 180, pivot);
    }

    public PointDInterval2D<T> GetRotatedByDegree(T rotation)
    {
      return GetRotatedByRad(rotation * Math.PI / 180, PointDInterval2D<T>.Empty);
    }

    public PointDInterval2D<T> GetRotatedByRad(T phi, PointDInterval2D<T> pivot)
    {
      var dx = X - pivot.X;
      var dy = Y - pivot.Y;

      if (phi != T.Zero)
      {
        var cosphi =  Math.Cos(phi);
        var sinphi = Math.Sin(phi);
        return new PointDInterval2D<T>(pivot.X + (dx * cosphi - dy * sinphi), pivot.Y + (dx * sinphi + dy * cosphi));
      }
      else
      {
        return new PointDInterval2D<T>(X, Y);
      }
    }

    public PointDInterval2D<T> GetRotatedByRad(T phi)
    {
      return GetRotatedByRad(phi, PointDInterval2D<T>.Empty);
    }

    public Interval<T> DotProduct(PointDInterval2D<T> q)
    {
      return DotProduct(this, q);
    }

    public static Interval<T> DotProduct(PointDInterval2D<T> p, PointDInterval2D<T> q)
    {
      return p.X * q.X + p.Y * q.Y;
    }

    public PointDInterval2D<T> GetNormalized()
    {
      var s = Interval<T>.One / VectorLength;
      return new PointDInterval2D<T>(X * s, Y * s);
    }

    /// <summary>
    /// Calculates the distance between two points.
    /// </summary>
    /// <param name="p1">First point.</param>
    /// <param name="p2">Second point.</param>
    /// <returns>The distance between points p1 and p2.</returns>
    public static Interval<T> Distance(PointDInterval2D<T> p1, PointDInterval2D<T> p2)
    {
      var x = p1.X - p2.X;
      var y = p1.Y - p2.Y;
      return (x * x + y * y).Sqrt();
    }

    /// <summary>
    /// Calculates the squared distance between two points.
    /// </summary>
    /// <param name="p1">First point.</param>
    /// <param name="p2">Second point.</param>
    /// <returns>The distance between points p1 and p2.</returns>
    public static Interval<T> DistanceSquared(PointDInterval2D<T> p1, PointDInterval2D<T> p2)
    {
      var x = p1.X - p2.X;
      var y = p1.Y - p2.Y;
      return (x * x + y * y);
    }

    /// <summary>
    /// Calculates the squared distance between a finite line and a point.
    /// </summary>
    /// <param name="point">The location of the point.</param>
    /// <param name="lineOrg">The location of the line origin.</param>
    /// <param name="lineEnd">The location of the line end.</param>
    /// <returns>The squared distance between the line (threated as having a finite length) and the point.</returns>
    public static Interval<T> SquareDistanceLineToPoint(PointDInterval2D<T> point, PointDInterval2D<T> lineOrg, PointDInterval2D<T> lineEnd)
    {
      var linex = lineEnd.X - lineOrg.X;
      var liney = lineEnd.Y - lineOrg.Y;
      var pointx = point.X - lineOrg.X;
      var pointy = point.Y - lineOrg.Y;

      var rsquare = linex * linex + liney * liney;
      var xx = linex * pointx + liney * pointy;
      if (xx <= Interval<T>.Zero) // the point is located before the line, so use
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
    public static bool IsPointIntoDistance(PointDInterval2D<T> point, Interval<T> distance, PointDInterval2D<T> lineOrg, PointDInterval2D<T> lineEnd)
    {
      // first a quick test if the point is far outside the circle
      // that is spanned from the middle of the line and has at least
      // a radius of half of the line length plus the distance
      var xm = (lineOrg.X + lineEnd.X) / 2;
      var ym = (lineOrg.Y + lineEnd.Y) / 2;
      var r = (lineOrg.X - xm).Abs() + (lineOrg.Y - ym).Abs() + distance;
      if (Interval<T>.Maxx((point.X - xm).Abs(), (point.Y - ym).Abs()) > r)
      {
        return false;
      }
      else
      {
        return SquareDistanceLineToPoint(point, lineOrg, lineEnd) <= distance * distance;
      }
    }
  }
}
#endif
