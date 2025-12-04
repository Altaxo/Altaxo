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
  /// Represents a vector with members of type Double in 3D space.
  /// </summary>
  public struct VectorD3D : IEquatable<VectorD3D>
  {
    /// <summary>Gets the x component of this vector.</summary>
    public double X { get; private set; }
    /// <summary>Gets the y component of this vector.</summary>
    public double Y { get; private set; }
    /// <summary>Gets the z component of this vector.</summary>
    public double Z { get; private set; }

    #region Serialization

    /// <summary>
    /// 2015-11-16 initial version 0.
    /// V1: 2023-01-14 Move from assembly AltaxoBase to AltaxoCore
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Geometry.VectorD3D", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VectorD3D), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VectorD3D)obj;
        info.AddValue("X", s.X);
        info.AddValue("Y", s.Y);
        info.AddValue("Z", s.Z);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var x = info.GetDouble("X");
        var y = info.GetDouble("Y");
        var z = info.GetDouble("Z");
        return new VectorD3D(x, y, z);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorD3D"/> struct.
    /// </summary>
    /// <param name="x">The x component.</param>
    /// <param name="y">The y component.</param>
    /// <param name="z">The z component.</param>
    public VectorD3D(double x, double y, double z)
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
    public VectorD3D WithX(double newX)
    {
      return new VectorD3D(newX, Y, Z);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Y"/> set to the provided value.
    /// </summary>
    /// <param name="newY">The new y.</param>
    /// <returns>New instance with <see cref="Y"/> set to the provided value.</returns>
    public VectorD3D WithY(double newY)
    {
      return new VectorD3D(X, newY, Z);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Z"/> set to the provided value.
    /// </summary>
    /// <param name="newZ">The new z.</param>
    /// <returns>New instance with <see cref="Z"/> set to the provided value.</returns>
    public VectorD3D WithZ(double newZ)
    {
      return new VectorD3D(X, Y, newZ);
    }

    /// <summary>
    /// Gets the length of the vector.
    /// </summary>
    public double Length
    {
      get
      {
        return Math.Sqrt(X * X + Y * Y + Z * Z);
      }
    }

    /// <summary>
    /// Gets the squared length of the vector.
    /// </summary>
    public double SquareOfLength
    {
      get
      {
        return (X * X + Y * Y + Z * Z);
      }
    }

    /// <summary>
    /// Gets the normalized version of this vector (length 1).
    /// </summary>
    public VectorD3D Normalized { get { var s = 1 / Length; return new VectorD3D(s * X, s * Y, s * Z); } }

    /// <inheritdoc/>
    public bool Equals(VectorD3D other)
    {
      return X == other.X && Y == other.Y && Z == other.Z;
    }

    /// <summary>
    /// Gets an empty vector (0,0,0).
    /// </summary>
    public static VectorD3D Empty
    {
      get
      {
        return new VectorD3D();
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is empty, i.e. all elements are zero.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmpty { get { return 0 == X && 0 == Y && 0 == Z; } }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      return obj is VectorD3D from && X == from.X && Y == from.Y && Z == from.Z;
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
        "VectorD3D({0}, {1}, {2})", X, Y, Z);
    }

    #region operators

    /// <summary>
    /// Adds two vectors component-wise.
    /// </summary>
    public static VectorD3D operator +(VectorD3D a, VectorD3D b)
    {
      return new VectorD3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    /// <summary>
    /// Subtracts two vectors component-wise.
    /// </summary>
    public static VectorD3D operator -(VectorD3D a, VectorD3D b)
    {
      return new VectorD3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    /// <summary>
    /// Multiplies a vector by a scalar.
    /// </summary>
    public static VectorD3D operator *(VectorD3D a, double b)
    {
      return new VectorD3D(a.X * b, a.Y * b, a.Z * b);
    }

    /// <summary>
    /// Multiplies a scalar by a vector.
    /// </summary>
    public static VectorD3D operator *(double b, VectorD3D a)
    {
      return new VectorD3D(a.X * b, a.Y * b, a.Z * b);
    }

    /// <summary>
    /// Divides a vector by a scalar.
    /// </summary>
    public static VectorD3D operator /(VectorD3D a, double b)
    {
      return new VectorD3D(a.X / b, a.Y / b, a.Z / b);
    }

    /// <summary>
    /// Negates a vector.
    /// </summary>
    public static VectorD3D operator -(VectorD3D b)
    {
      return new VectorD3D(-b.X, -b.Y, -b.Z);
    }

    /// <summary>
    /// Checks if two vectors are equal.
    /// </summary>
    public static bool operator ==(VectorD3D a, VectorD3D b)
    {
      return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    }

    /// <summary>
    /// Checks if two vectors are not equal.
    /// </summary>
    public static bool operator !=(VectorD3D a, VectorD3D b)
    {
      return !(a.X == b.X && a.Y == b.Y && a.Z == b.Z);
    }

    /// <summary>
    /// Converts a <see cref="PointD3D"/> to a <see cref="VectorD3D"/>.
    /// </summary>
    /// <param name="v">The point to convert.</param>
    public static explicit operator VectorD3D(PointD3D v)
    {
      return new VectorD3D(v.X, v.Y, v.Z);
    }

    #endregion operators

    #region static functions

    /// <summary>
    /// Multiplies two vectors elementwise.
    /// </summary>
    public static VectorD3D MultiplicationElementwise(VectorD3D a, VectorD3D b)
    {
      return new VectorD3D(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    }

    /// <summary>
    /// Calculates the dot product of two vectors.
    /// </summary>
    public static double DotProduct(VectorD3D a, VectorD3D b)
    {
      return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }

    /// <summary>
    /// Creates a normalized version of the given vector.
    /// </summary>
    public static VectorD3D CreateNormalized(VectorD3D pt)
    {
      var ilen = 1 / pt.Length;
      return new VectorD3D(pt.X * ilen, pt.Y * ilen, pt.Z * ilen);
    }

    /// <summary>
    /// Creates a normalized vector from the given x, y, and z components.
    /// </summary>
    public static VectorD3D CreateNormalized(double x, double y, double z)
    {
      var k = x * x + y * y + z * z;

      if (0 == k)
        throw new ArgumentException("The provided data for x, y, z are all zero. This is not allowed for a vector to be normalized.", nameof(x));

      if (!(k > 0))
        throw new ArgumentException("The data for x, y or z contain invalid or infinite values", nameof(x));

      k = 1 / Math.Sqrt(k);
      return new VectorD3D(x * k, y * k, z * k);
    }

    /// <summary>
    /// Creates a vector that is the sum of two vectors.
    /// </summary>
    public static VectorD3D CreateSum(VectorD3D pt1, VectorD3D pt2)
    {
      return new VectorD3D(pt1.X + pt2.X, pt1.Y + pt2.Y, pt1.Z + pt2.Z);
    }

    /// <summary>
    /// Creates a scaled version of the given vector.
    /// </summary>
    public static VectorD3D CreateScaled(VectorD3D pt, double scale)
    {
      return new VectorD3D(pt.X * scale, pt.Y * scale, pt.Z * scale);
    }

    /// <summary>
    /// Calculates the cross product of two vectors.
    /// </summary>
    public static VectorD3D CrossProduct(VectorD3D a, VectorD3D b)
    {
      return new VectorD3D(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
    }

    /// <summary>
    /// Calculates the angle between two vectors in radians.
    /// </summary>
    public static double AngleBetweenInRadians(VectorD3D vector1, VectorD3D vector2)
    {
      vector1 = vector1.Normalized;
      vector2 = vector2.Normalized;
      double radians;
      if (VectorD3D.DotProduct(vector1, vector2) < 0.0)
      {
        radians = Math.PI - 2.0 * Math.Asin((vector1 + vector2).Length / 2.0);
      }
      else
      {
        radians = 2.0 * Math.Asin((vector1 - vector2).Length / 2.0);
      }

      return radians;
    }

    #endregion static functions
  }
}
