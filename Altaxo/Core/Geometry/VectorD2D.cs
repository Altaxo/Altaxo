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

#nullable enable
using System;

namespace Altaxo.Geometry
{
  /// <summary>
  /// Represents a vector with members of type Double in 2D space.
  /// </summary>
  public struct VectorD2D : IEquatable<VectorD2D>
  {
    /// <summary>
    /// Gets the x component of this vector.
    /// </summary>
    /// <value>
    /// The x component of this vector.
    /// </value>
    public double X { get; private set; }

    /// <summary>
    /// Gets the y component of this vector.
    /// </summary>
    /// <value>
    /// The y component of this vector.
    /// </value>
    public double Y { get; private set; }

    #region Serialization

    /// <summary>
    /// V0: 2016-03-31 initial version.
    /// V1: 2023-01-14 Move from assembly AltaxoBase to AltaxoCore
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Geometry.VectorD2D", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VectorD2D), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VectorD2D)obj;
        info.AddValue("X", s.X);
        info.AddValue("Y", s.Y);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var x = info.GetDouble("X");
        var y = info.GetDouble("Y");
        return new VectorD2D(x, y);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorD2D"/> struct.
    /// </summary>
    /// <param name="x">The x component.</param>
    /// <param name="y">The y component.</param>
    public VectorD2D(double x, double y)
    {
      X = x;
      Y = y;
    }

    /// <summary>
    /// Returns a new instance with <see cref="X"/> set to the provided value.
    /// </summary>
    /// <param name="newX">The new x.</param>
    /// <returns>New instance with <see cref="X"/> set to the provided value.</returns>
    public VectorD2D WithX(double newX)
    {
      return new VectorD2D(newX, Y);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Y"/> set to the provided value.
    /// </summary>
    /// <param name="newY">The new y.</param>
    /// <returns>New instance with <see cref="Y"/> set to the provided value.</returns>
    public VectorD2D WithY(double newY)
    {
      return new VectorD2D(X, newY);
    }

    /// <summary>
    /// Gets the length of the vector.
    /// </summary>
    public double Length
    {
      get
      {
        return Math.Sqrt(X * X + Y * Y);
      }
    }

    /// <summary>
    /// Gets the squared length of the vector.
    /// </summary>
    public double SquareOfLength
    {
      get
      {
        return (X * X + Y * Y);
      }
    }

    /// <summary>
    /// Gets the normalized version of this vector (length 1).
    /// </summary>
    public VectorD2D Normalized { get { var s = 1 / Length; return new VectorD2D(s * X, s * Y); } }

    /// <inheritdoc/>
    public bool Equals(VectorD2D other)
    {
      return X == other.X && Y == other.Y;
    }

    /// <summary>
    /// Gets an empty vector (0,0).
    /// </summary>
    public static VectorD2D Empty
    {
      get
      {
        return new VectorD2D();
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is empty, i.e. all elements are zero.
    /// </summary>
    public bool IsEmpty { get { return 0 == X && 0 == Y; } }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      if (obj is VectorD2D from)
      {
        return X == from.X && Y == from.Y;
      }
      else
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return X.GetHashCode() + 7 * Y.GetHashCode();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return string.Format(System.Globalization.CultureInfo.InvariantCulture,
        "VectorD2D({0}, {1})", X, Y);
    }

    #region operators

    /// <summary>
    /// Adds two vectors component-wise.
    /// </summary>
    public static VectorD2D operator +(VectorD2D a, VectorD2D b)
    {
      return new VectorD2D(a.X + b.X, a.Y + b.Y);
    }

    /// <summary>
    /// Subtracts two vectors component-wise.
    /// </summary>
    public static VectorD2D operator -(VectorD2D a, VectorD2D b)
    {
      return new VectorD2D(a.X - b.X, a.Y - b.Y);
    }

    /// <summary>
    /// Multiplies a vector by a scalar.
    /// </summary>
    public static VectorD2D operator *(VectorD2D a, double b)
    {
      return new VectorD2D(a.X * b, a.Y * b);
    }

    /// <summary>
    /// Multiplies a scalar by a vector.
    /// </summary>
    public static VectorD2D operator *(double b, VectorD2D a)
    {
      return new VectorD2D(a.X * b, a.Y * b);
    }

    /// <summary>
    /// Divides a vector by a scalar.
    /// </summary>
    public static VectorD2D operator /(VectorD2D a, double b)
    {
      return new VectorD2D(a.X / b, a.Y / b);
    }

    /// <summary>
    /// Negates a vector.
    /// </summary>
    public static VectorD2D operator -(VectorD2D b)
    {
      return new VectorD2D(-b.X, -b.Y);
    }

    /// <summary>
    /// Checks if two vectors are equal.
    /// </summary>
    public static bool operator ==(VectorD2D a, VectorD2D b)
    {
      return a.X == b.X && a.Y == b.Y;
    }

    /// <summary>
    /// Checks if two vectors are not equal.
    /// </summary>
    public static bool operator !=(VectorD2D a, VectorD2D b)
    {
      return !(a.X == b.X && a.Y == b.Y);
    }

    /// <summary>
    /// Converts a <see cref="PointD2D"/> to a <see cref="VectorD2D"/>.
    /// </summary>
    /// <param name="v">The point to convert.</param>
    public static explicit operator VectorD2D(PointD2D v)
    {
      return new VectorD2D(v.X, v.Y);
    }

    #endregion operators

    #region static functions

    /// <summary>
    /// Multiplies two vectors elementwise.
    /// </summary>
    public static VectorD2D MultiplicationElementwise(VectorD2D a, VectorD2D b)
    {
      return new VectorD2D(a.X * b.X, a.Y * b.Y);
    }

    /// <summary>
    /// Calculates the dot product of two vectors.
    /// </summary>
    public static double DotProduct(VectorD2D a, VectorD2D b)
    {
      return a.X * b.X + a.Y * b.Y;
    }

    /// <summary>
    /// Creates a normalized version of the given vector.
    /// </summary>
    public static VectorD2D CreateNormalized(VectorD2D pt)
    {
      var ilen = 1 / pt.Length;
      return new VectorD2D(pt.X * ilen, pt.Y * ilen);
    }

    /// <summary>
    /// Creates a normalized vector from the given x and y components.
    /// </summary>
    public static VectorD2D CreateNormalized(double x, double y)
    {
      var k = x * x + y * y;

      if (0 == k)
      {
        throw new ArgumentException("The provided data for x, y are all zero. This is not allowed for a vector to be normalized.", nameof(x));
      }

      if (!(k > 0))
      {
        throw new ArgumentException("The data for x, y contain invalid or infinite values", nameof(x));
      }

      k = 1 / Math.Sqrt(k);
      return new VectorD2D(x * k, y * k);
    }

    /// <summary>
    /// Creates a vector that is the sum of two vectors.
    /// </summary>
    public static VectorD2D CreateSum(VectorD2D pt1, VectorD2D pt2)
    {
      return new VectorD2D(pt1.X + pt2.X, pt1.Y + pt2.Y);
    }

    /// <summary>
    /// Creates a scaled version of the given vector.
    /// </summary>
    public static VectorD2D CreateScaled(VectorD2D pt, double scale)
    {
      return new VectorD2D(pt.X * scale, pt.Y * scale);
    }

    /// <summary>
    /// Calculates the angle between two vectors in radians.
    /// </summary>
    public static double AngleBetweenInRadians(VectorD2D vector1, VectorD2D vector2)
    {
      vector1 = vector1.Normalized;
      vector2 = vector2.Normalized;
      double radians;
      if (VectorD2D.DotProduct(vector1, vector2) < 0.0)
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
