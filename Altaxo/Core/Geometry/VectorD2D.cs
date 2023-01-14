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
    /// <param name="newY">The new x.</param>
    /// <returns>New instance with <see cref="Y"/> set to the provided value.</returns>
    public VectorD2D WithY(double newY)
    {
      return new VectorD2D(X, newY);
    }

    public double Length
    {
      get
      {
        return Math.Sqrt(X * X + Y * Y);
      }
    }

    public double SquareOfLength
    {
      get
      {
        return (X * X + Y * Y);
      }
    }

    public VectorD2D Normalized { get { var s = 1 / Length; return new VectorD2D(s * X, s * Y); } }

    public bool Equals(VectorD2D other)
    {
      return X == other.X && Y == other.Y;
    }

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
    /// <value>
    /// <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmpty { get { return 0 == X && 0 == Y; } }

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

    public override int GetHashCode()
    {
      return X.GetHashCode() + 7 * Y.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format(System.Globalization.CultureInfo.InvariantCulture,
        "VectorD2D({0}, {1})", X, Y);
    }

    #region operators

    public static VectorD2D operator +(VectorD2D a, VectorD2D b)
    {
      return new VectorD2D(a.X + b.X, a.Y + b.Y);
    }

    public static VectorD2D operator -(VectorD2D a, VectorD2D b)
    {
      return new VectorD2D(a.X - b.X, a.Y - b.Y);
    }

    public static VectorD2D operator *(VectorD2D a, double b)
    {
      return new VectorD2D(a.X * b, a.Y * b);
    }

    public static VectorD2D operator *(double b, VectorD2D a)
    {
      return new VectorD2D(a.X * b, a.Y * b);
    }

    public static VectorD2D operator /(VectorD2D a, double b)
    {
      return new VectorD2D(a.X / b, a.Y / b);
    }

    public static VectorD2D operator -(VectorD2D b)
    {
      return new VectorD2D(-b.X, -b.Y);
    }

    public static bool operator ==(VectorD2D a, VectorD2D b)
    {
      return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(VectorD2D a, VectorD2D b)
    {
      return !(a.X == b.X && a.Y == b.Y);
    }

    public static explicit operator VectorD2D(PointD2D v)
    {
      return new VectorD2D(v.X, v.Y);
    }

    #endregion operators

    #region static functions

    public static VectorD2D MultiplicationElementwise(VectorD2D a, VectorD2D b)
    {
      return new VectorD2D(a.X * b.X, a.Y * b.Y);
    }

    public static double DotProduct(VectorD2D a, VectorD2D b)
    {
      return a.X * b.X + a.Y * b.Y;
    }

    public static VectorD2D CreateNormalized(VectorD2D pt)
    {
      var ilen = 1 / pt.Length;
      return new VectorD2D(pt.X * ilen, pt.Y * ilen);
    }

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

    public static VectorD2D CreateSum(VectorD2D pt1, VectorD2D pt2)
    {
      return new VectorD2D(pt1.X + pt2.X, pt1.Y + pt2.Y);
    }

    public static VectorD2D CreateScaled(VectorD2D pt, double scale)
    {
      return new VectorD2D(pt.X * scale, pt.Y * scale);
    }

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
