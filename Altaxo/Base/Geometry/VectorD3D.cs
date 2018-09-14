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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Geometry
{
  public struct VectorD3D : IEquatable<VectorD3D>
  {
    public double X { get; private set; }
    public double Y { get; private set; }
    public double Z { get; private set; }

    #region Serialization

    /// <summary>
    /// 2015-11-16 initial version 0.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VectorD3D), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VectorD3D)obj;
        info.AddValue("X", s.X);
        info.AddValue("Y", s.Y);
        info.AddValue("Z", s.Z);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var x = info.GetDouble("X");
        var y = info.GetDouble("Y");
        var z = info.GetDouble("Z");
        return new VectorD3D(x, y, z);
      }
    }

    #endregion Serialization

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
    /// <param name="newY">The new x.</param>
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

    public double Length
    {
      get
      {
        return Math.Sqrt(X * X + Y * Y + Z * Z);
      }
    }

    public double SquareOfLength
    {
      get
      {
        return (X * X + Y * Y + Z * Z);
      }
    }

    public VectorD3D Normalized { get { var s = 1 / Length; return new VectorD3D(s * X, s * Y, s * Z); } }

    public bool Equals(VectorD3D other)
    {
      return X == other.X && Y == other.Y && Z == other.Z;
    }

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

    public override bool Equals(object obj)
    {
      if (obj is VectorD3D)
      {
        var from = (VectorD3D)obj;
        return X == from.X && Y == from.Y && Z == from.Z;
      }
      else
      {
        return false;
      }
    }

    public override int GetHashCode()
    {
      return X.GetHashCode() + 7 * Y.GetHashCode() + 13 * Z.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format(System.Globalization.CultureInfo.InvariantCulture,
        "VectorD3D({0}, {1}, {2})", X, Y, Z);
    }

    #region operators

    public static VectorD3D operator +(VectorD3D a, VectorD3D b)
    {
      return new VectorD3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static VectorD3D operator -(VectorD3D a, VectorD3D b)
    {
      return new VectorD3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static VectorD3D operator *(VectorD3D a, double b)
    {
      return new VectorD3D(a.X * b, a.Y * b, a.Z * b);
    }

    public static VectorD3D operator *(double b, VectorD3D a)
    {
      return new VectorD3D(a.X * b, a.Y * b, a.Z * b);
    }

    public static VectorD3D operator /(VectorD3D a, double b)
    {
      return new VectorD3D(a.X / b, a.Y / b, a.Z / b);
    }

    public static VectorD3D operator -(VectorD3D b)
    {
      return new VectorD3D(-b.X, -b.Y, -b.Z);
    }

    public static bool operator ==(VectorD3D a, VectorD3D b)
    {
      return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    }

    public static bool operator !=(VectorD3D a, VectorD3D b)
    {
      return !(a.X == b.X && a.Y == b.Y && a.Z == b.Z);
    }

    public static explicit operator VectorD3D(PointD3D v)
    {
      return new VectorD3D(v.X, v.Y, v.Z);
    }

    #endregion operators

    #region static functions

    public static VectorD3D MultiplicationElementwise(VectorD3D a, VectorD3D b)
    {
      return new VectorD3D(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    }

    public static double DotProduct(VectorD3D a, VectorD3D b)
    {
      return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }

    public static VectorD3D CreateNormalized(VectorD3D pt)
    {
      var ilen = 1 / pt.Length;
      return new VectorD3D(pt.X * ilen, pt.Y * ilen, pt.Z * ilen);
    }

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

    public static VectorD3D CreateSum(VectorD3D pt1, VectorD3D pt2)
    {
      return new VectorD3D(pt1.X + pt2.X, pt1.Y + pt2.Y, pt1.Z + pt2.Z);
    }

    public static VectorD3D CreateScaled(VectorD3D pt, double scale)
    {
      return new VectorD3D(pt.X * scale, pt.Y * scale, pt.Z * scale);
    }

    public static VectorD3D CrossProduct(VectorD3D a, VectorD3D b)
    {
      return new VectorD3D(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
    }

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
