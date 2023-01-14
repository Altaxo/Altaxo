#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
  public struct VectorD4D : IEquatable<VectorD4D>
  {
    public double X { get; private set; }
    public double Y { get; private set; }
    public double Z { get; private set; }
    public double W { get; private set; }

    #region Serialization

    /// <summary>
    /// V0: 2015-11-16 initial version 0.
    /// V1: 2023-01-14 Move from assembly AltaxoBase to AltaxoCore
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Geometry.VectorD4D", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VectorD4D), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VectorD4D)obj;
        info.AddValue("X", s.X);
        info.AddValue("Y", s.Y);
        info.AddValue("Z", s.Z);
        info.AddValue("W", s.W);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var x = info.GetDouble("X");
        var y = info.GetDouble("Y");
        var z = info.GetDouble("Z");
        var w = info.GetDouble("W");
        return new VectorD4D(x, y, z, w);
      }
    }

    #endregion Serialization

    public VectorD4D(double x, double y, double z, double w)
    {
      X = x;
      Y = y;
      Z = z;
      W = w;
    }

    /// <summary>
    /// Returns a new instance with <see cref="X"/> set to the provided value.
    /// </summary>
    /// <param name="newX">The new x.</param>
    /// <returns>New instance with <see cref="X"/> set to the provided value.</returns>
    public VectorD4D WithX(double newX)
    {
      return new VectorD4D(newX, Y, Z, W);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Y"/> set to the provided value.
    /// </summary>
    /// <param name="newY">The new x.</param>
    /// <returns>New instance with <see cref="Y"/> set to the provided value.</returns>
    public VectorD4D WithY(double newY)
    {
      return new VectorD4D(X, newY, Z, W);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Z"/> set to the provided value.
    /// </summary>
    /// <param name="newZ">The new z.</param>
    /// <returns>New instance with <see cref="Z"/> set to the provided value.</returns>
    public VectorD4D WithZ(double newZ)
    {
      return new VectorD4D(X, Y, newZ, W);
    }

    /// <summary>
    /// Returns a new instance with <see cref="W"/> set to the provided value.
    /// </summary>
    /// <param name="newZ">The new z.</param>
    /// <returns>New instance with <see cref="Z"/> set to the provided value.</returns>
    public VectorD4D WithW(double newW)
    {
      return new VectorD4D(X, Y, Z, newW);
    }

    public double Length
    {
      get
      {
        return Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
      }
    }

    public double SquareOfLength
    {
      get
      {
        return (X * X + Y * Y + Z * Z + W * W);
      }
    }

    public VectorD4D Normalized { get { var s = 1 / Length; return new VectorD4D(s * X, s * Y, s * Z, s * W); } }

    public bool Equals(VectorD4D other)
    {
      return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
    }

    public static VectorD4D Empty
    {
      get
      {
        return new VectorD4D();
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is empty, i.e. all elements are zero.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmpty { get { return 0 == X && 0 == Y && 0 == Z && 0 == W; } }

    public override bool Equals(object? obj)
    {
      return obj is VectorD4D from && X == from.X && Y == from.Y && Z == from.Z && W == from.W;
    }

    public override int GetHashCode()
    {
      return X.GetHashCode() + 7 * Y.GetHashCode() + 13 * Z.GetHashCode() + 19 * W.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format(System.Globalization.CultureInfo.InvariantCulture,
        "VectorD3D({0}, {1}, {2}, {3})", X, Y, Z, W);
    }

    #region operators

    public static VectorD4D operator +(VectorD4D a, VectorD4D b)
    {
      return new VectorD4D(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
    }

    public static VectorD4D operator -(VectorD4D a, VectorD4D b)
    {
      return new VectorD4D(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
    }

    public static VectorD4D operator *(VectorD4D a, double b)
    {
      return new VectorD4D(a.X * b, a.Y * b, a.Z * b, a.W * b);
    }

    public static VectorD4D operator *(double b, VectorD4D a)
    {
      return new VectorD4D(a.X * b, a.Y * b, a.Z * b, a.W * b);
    }

    public static VectorD4D operator /(VectorD4D a, double b)
    {
      return new VectorD4D(a.X / b, a.Y / b, a.Z / b, a.W / b);
    }

    public static VectorD4D operator -(VectorD4D b)
    {
      return new VectorD4D(-b.X, -b.Y, -b.Z, -b.W);
    }

    public static bool operator ==(VectorD4D a, VectorD4D b)
    {
      return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
    }

    public static bool operator !=(VectorD4D a, VectorD4D b)
    {
      return !(a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W);
    }

    public static explicit operator VectorD4D(PointD3D v)
    {
      return new VectorD4D(v.X, v.Y, v.Z, 1);
    }

    #endregion operators

    #region static functions

    public static VectorD4D MultiplicationElementwise(VectorD4D a, VectorD4D b)
    {
      return new VectorD4D(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
    }

    public static double DotProduct(VectorD4D a, VectorD4D b)
    {
      return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
    }

    public static VectorD4D CreateNormalized(VectorD4D pt)
    {
      var ilen = 1 / pt.Length;
      return new VectorD4D(pt.X * ilen, pt.Y * ilen, pt.Z * ilen, pt.W * ilen);
    }

    public static VectorD4D CreateNormalized(double x, double y, double z, double w)
    {
      var k = x * x + y * y + z * z + w * w;

      if (0 == k)
        throw new ArgumentException("The provided data for x, y, z, w are all zero. This is not allowed for a vector to be normalized.", nameof(x));

      if (!(k > 0))
        throw new ArgumentException("The data for x, y, z or w contain invalid or infinite values", nameof(x));

      k = 1 / Math.Sqrt(k);
      return new VectorD4D(x * k, y * k, z * k, w * k);
    }

    public static VectorD4D CreateSum(VectorD4D pt1, VectorD4D pt2)
    {
      return new VectorD4D(pt1.X + pt2.X, pt1.Y + pt2.Y, pt1.Z + pt2.Z, pt1.W + pt2.W);
    }

    public static VectorD4D CreateScaled(VectorD4D pt, double scale)
    {
      return new VectorD4D(pt.X * scale, pt.Y * scale, pt.Z * scale, pt.W * scale);
    }

    #endregion static functions
  }
}
