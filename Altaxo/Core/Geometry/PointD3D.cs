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

namespace Altaxo.Geometry
{
  /// <summary>
  /// Represents a point with values of type Double in 3D space.
  /// </summary>
  public struct PointD3D
  {
    /// <summary>
    /// Gets the x component of this point.
    /// </summary>
    /// <value>
    /// The x  component of this point..
    /// </value>
    public double X { get; private set; }

    /// <summary>
    /// Gets the y component of this point.
    /// </summary>
    /// <value>
    /// The y  component of this point..
    /// </value>
    public double Y { get; private set; }

    /// <summary>
    /// Gets the z component of this point.
    /// </summary>
    public double Z { get; private set; }

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="PointD3D"/>.
    /// V1: 2015-11-16 initial version.
    /// V2: 2023-01-14 Move from assembly AltaxoBase to AltaxoCore
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Geometry.PointD3D", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PointD3D), 2)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PointD3D)obj;
        info.AddValue("X", s.X);
        info.AddValue("Y", s.Y);
        info.AddValue("Z", s.Z);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var x = info.GetDouble("X");
        var y = info.GetDouble("Y");
        var z = info.GetDouble("Z");
        return new PointD3D(x, y, z);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="PointD3D"/> struct.
    /// </summary>
    /// <param name="x">The x component.</param>
    /// <param name="y">The y component.</param>
    /// <param name="z">The z component.</param>
    public PointD3D(double x, double y, double z)
    {
      X = x;
      Y = y;
      Z = z;
    }

    /// <summary>
    /// Returns a new instance with <see cref="X"/> set to the provided value.
    /// </summary>
    /// <param name="newX">The new x.</param>
    /// <returns>New instance with <see cref="X"/> set to the provided value.</returns>
    public PointD3D WithX(double newX)
    {
      return new PointD3D(newX, Y, Z);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Y"/> set to the provided value.
    /// </summary>
    /// <param name="newY">The new x.</param>
    /// <returns>New instance with <see cref="Y"/> set to the provided value.</returns>
    public PointD3D WithY(double newY)
    {
      return new PointD3D(X, newY, Z);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Z"/> set to the provided value.
    /// </summary>
    /// <param name="newZ">The new z.</param>
    /// <returns>New instance with <see cref="Z"/> set to the provided value.</returns>
    public PointD3D WithZ(double newZ)
    {
      return new PointD3D(X, Y, newZ);
    }

    /// <summary>
    /// Returns a new instance with <see cref="X"/> set to the X plus the provided value.
    /// </summary>
    /// <param name="addX">The value to add to <see cref="X"/>.</param>
    /// <returns>New instance with <see cref="X"/> set to the X plus the provided value.</returns>
    public PointD3D WithXPlus(double addX)
    {
      return new PointD3D(X + addX, Y, Z);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Y"/> set to the Y plus the provided value.
    /// </summary>
    /// <param name="addY">The value to add to <see cref="Y"/>.</param>
    /// <returns>New instance with <see cref="Y"/> set to the Y plus the provided value.</returns>
    public PointD3D WithYPlus(double addY)
    {
      return new PointD3D(X, Y + addY, Z);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Z"/> set to the Z plus the provided value.
    /// </summary>
    /// <param name="addZ">The value to add to <see cref="Z"/>.</param>
    /// <returns>New instance with <see cref="Z"/> set to the Z plus the provided value.</returns>
    public PointD3D WithZPlus(double addZ)
    {
      return new PointD3D(X, Y, Z + addZ);
    }

    /// <summary>
    /// Gets a 2D point with X and Y the same as this point, but without the z-component.
    /// </summary>
    /// <value>
    /// 2D point with X and Y the same as this point, but without the z-component.
    /// </value>
    public PointD2D PointD2DWithoutZ
    {
      get
      {
        return new PointD2D(X, Y);
      }
    }

    /// <summary>
    /// Gets an empty point (0,0,0).
    /// </summary>
    public static PointD3D Empty
    {
      get
      {
        return new PointD3D();
      }
    }

    /// <summary>
    /// Gets a value indicating whether this point is empty (all components are zero).
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return 0 == X && 0 == Y && 0 == Z;
      }
    }

    /// <summary>
    /// Gets a value indicating whether one of the members of this instance is <see cref="double.NaN"/>.
    /// </summary>
    public bool IsNaN
    {
      get
      {
        return double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Z);
      }
    }

    #region Operators

    /// <summary>
    /// Adds a vector to a point component-wise.
    /// </summary>
    public static PointD3D operator +(PointD3D a, VectorD3D b)
    {
      return new PointD3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    /// <summary>
    /// Adds a vector to a point component-wise.
    /// </summary>
    public static PointD3D operator +(VectorD3D b, PointD3D a)
    {
      return new PointD3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    /// <summary>
    /// Subtracts two points to get a vector.
    /// </summary>
    public static VectorD3D operator -(PointD3D a, PointD3D b)
    {
      return new VectorD3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    /// <summary>
    /// Subtracts a vector from a point component-wise.
    /// </summary>
    public static PointD3D operator -(PointD3D a, VectorD3D b)
    {
      return new PointD3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    /// <summary>
    /// Checks if two points are equal.
    /// </summary>
    public static bool operator ==(PointD3D a, PointD3D b)
    {
      return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    }

    /// <summary>
    /// Checks if two points are not equal.
    /// </summary>
    public static bool operator !=(PointD3D a, PointD3D b)
    {
      return !(a.X == b.X && a.Y == b.Y && a.Z == b.Z);
    }

    /// <summary>
    /// Converts a <see cref="VectorD3D"/> to a <see cref="PointD3D"/>.
    /// </summary>
    /// <param name="v">The vector to convert.</param>
    public static explicit operator PointD3D(VectorD3D v)
    {
      return new PointD3D(v.X, v.Y, v.Z);
    }

    #endregion Operators

    #region Other calculations

    /// <summary>
    /// Interpolates between the points <paramref name="p0"/> and <paramref name="p1"/>.
    /// </summary>
    /// <param name="p0">The first point.</param>
    /// <param name="p1">The second point.</param>
    /// <param name="r">Relative way between <paramref name="p0"/> and <paramref name="p1"/> (0..1).</param>
    /// <returns>Interpolation between <paramref name="p0"/> and <paramref name="p1"/>. The return value is <paramref name="p0"/> if <paramref name="r"/> is 0. The return value is <paramref name="p1"/>  if <paramref name="r"/> is 1.  </returns>
    public static PointD3D Interpolate(PointD3D p0, PointD3D p1, double r)
    {
      double or = 1 - r;
      return new PointD3D(or * p0.X + r * p1.X, or * p0.Y + r * p1.Y, or * p0.Z + r * p1.Z);
    }

    #endregion Other calculations

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      return obj is PointD3D from &&
             X == from.X && Y == from.Y && Z == from.Z;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return X.GetHashCode() + 7 * Y.GetHashCode() + 13 * Z.GetHashCode();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return string.Format(System.Globalization.CultureInfo.InvariantCulture,
        "PointD3D({0}, {1}, {2})", X, Y, Z);
    }

    /// <summary>
    /// Calculates the distance between two points.
    /// </summary>
    /// <param name="p1">First point.</param>
    /// <param name="p2">Second point.</param>
    /// <returns>The distance between points p1 and p2.</returns>
    public static double Distance(PointD3D p1, PointD3D p2)
    {
      var x = p1.X - p2.X;
      var y = p1.Y - p2.Y;
      var z = p1.Z - p2.Z;
      return Math.Sqrt(x * x + y * y + z * z);
    }

    /// <summary>
    /// Calculates the squared distance between two points.
    /// </summary>
    /// <param name="p1">First point.</param>
    /// <param name="p2">Second point.</param>
    /// <returns>The squared distance between points p1 and p2.</returns>
    public static double DistanceSquared(PointD3D p1, PointD3D p2)
    {
      var x = p1.X - p2.X;
      var y = p1.Y - p2.Y;
      var z = p1.Z - p2.Z;
      return (x * x + y * y + z * z);
    }

    /// <summary>
    /// Calculates the distance between this point and another point.
    /// </summary>
    /// <param name="p">Other point.</param>
    /// <returns>The distance between this point and point p.</returns>
    public double DistanceTo(PointD3D p)
    {
      return Distance(this, p);
    }

    /// <summary>
    /// Calculates the squared distance between this point and another point.
    /// </summary>
    /// <param name="p">Other point.</param>
    /// <returns>The squared distance between this point and point p.</returns>
    public double DistanceSquaredTo(PointD3D p)
    {
      return DistanceSquared(this, p);
    }

    /// <summary>
    /// Gets the vector length of this point.
    /// </summary>
    public double VectorLength
    {
      get
      {
        return Math.Sqrt(X * X + Y * Y + Z * Z);
      }
    }

    /// <summary>
    /// Gets the squared vector length of this point.
    /// </summary>
    public double VectorLengthSquared
    {
      get
      {
        return (X * X + Y * Y + Z * Z);
      }
    }

    /// <summary>
    /// Gets a normalized version of this point (length 1).
    /// </summary>
    /// <returns>A normalized version of this point.</returns>
    public PointD3D GetNormalized()
    {
      var s = 1 / VectorLength;
      return new PointD3D(X * s, Y * s, Z * s);
    }

    /// <summary>
    /// Calculates the dot product of this point and another point.
    /// </summary>
    /// <param name="q">Other point.</param>
    /// <returns>The dot product.</returns>
    public double DotProduct(PointD3D q)
    {
      return DotProduct(this, q);
    }

    /// <summary>
    /// Calculates the dot product of two points.
    /// </summary>
    /// <param name="p">First point.</param>
    /// <param name="q">Second point.</param>
    /// <returns>The dot product.</returns>
    public static double DotProduct(PointD3D p, PointD3D q)
    {
      return p.X * q.X + p.Y * q.Y + p.Z * q.Z;
    }

    /// <summary>
    /// Gets a new point with all components sign-flipped.
    /// </summary>
    /// <returns>A new point with all components sign-flipped.</returns>
    public PointD3D GetSignFlipped()
    {
      return new PointD3D(-X, -Y, -Z);
    }
  }
}
