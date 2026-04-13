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
  /// RectangleD describes a rectangle in 3D space.
  /// </summary>
  [Serializable]
  public struct RectangleD3D
  {
    /// <summary>The x coordinate of the left side.</summary>
    private double _x;
    /// <summary>The y coordinate of the top side.</summary>
    private double _y;
    /// <summary>The z coordinate of the front side.</summary>
    private double _z;
    /// <summary>The size in x direction.</summary>
    private double _sizeX;
    /// <summary>The size in y direction.</summary>
    private double _sizeY;
    /// <summary>The size in z direction.</summary>
    private double _sizeZ;

    #region Serialization

    /// <summary>
    /// V1: 2023-01-14 Move from assembly AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Geometry.RectangleD3D", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RectangleD3D), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RectangleD3D)o;
        info.AddValue("X", s._x);
        info.AddValue("Y", s._y);
        info.AddValue("Z", s._z);
        info.AddValue("SizeX", s._sizeX);
        info.AddValue("SizeY", s._sizeY);
        info.AddValue("SizeZ", s._sizeZ);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (RectangleD3D?)o ?? new RectangleD3D();
        s._x = info.GetDouble("X");
        s._y = info.GetDouble("Y");
        s._z = info.GetDouble("Z");
        s._sizeX = info.GetDouble("SizeX");
        s._sizeY = info.GetDouble("SizeY");
        s._sizeZ = info.GetDouble("SizeZ");

        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleD3D"/> struct.
    /// </summary>
    /// <param name="posX">The x-coordinate of the left side.</param>
    /// <param name="posY">The y-coordinate of the top side.</param>
    /// <param name="posZ">The z-coordinate of the front side.</param>
    /// <param name="sizeX">The size in x-direction.</param>
    /// <param name="sizeY">The size in y-direction.</param>
    /// <param name="sizeZ">The size in z-direction.</param>
    public RectangleD3D(double posX, double posY, double posZ, double sizeX, double sizeY, double sizeZ)
    {
      _x = posX;
      _y = posY;
      _z = posZ;
      _sizeX = sizeX;
      _sizeY = sizeY;
      _sizeZ = sizeZ;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleD3D"/> struct.
    /// </summary>
    /// <param name="position">The position of the left-top-front corner.</param>
    /// <param name="size">The size vector.</param>
    public RectangleD3D(PointD3D position, VectorD3D size)
    {
      _x = position.X;
      _y = position.Y;
      _z = position.Z;
      _sizeX = size.X;
      _sizeY = size.Y;
      _sizeZ = size.Z;
    }

    #endregion Constructors

    #region Setter

    /// <summary>
    /// Returns a new instance with <see cref="X"/> set to the provided value.
    /// </summary>
    /// <param name="newX">The new x.</param>
    /// <returns>New instance with <see cref="X"/> set to the provided value.</returns>
    public RectangleD3D WithX(double newX)
    {
      return new RectangleD3D(newX, Y, Z, SizeX, SizeY, SizeZ);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Y"/> set to the provided value.
    /// </summary>
    /// <param name="newY">The new x.</param>
    /// <returns>New instance with <see cref="Y"/> set to the provided value.</returns>
    public RectangleD3D WithY(double newY)
    {
      return new RectangleD3D(X, newY, Z, SizeX, SizeY, SizeZ);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Z"/> set to the provided value.
    /// </summary>
    /// <param name="newZ">The new z.</param>
    /// <returns>New instance with <see cref="Z"/> set to the provided value.</returns>
    public RectangleD3D WithZ(double newZ)
    {
      return new RectangleD3D(X, Y, newZ, SizeX, SizeY, SizeZ);
    }

    /// <summary>
    /// Returns a new instance with <see cref="X"/> set to the X plus the provided value.
    /// </summary>
    /// <param name="addX">The value to add to <see cref="X"/>.</param>
    /// <returns>New instance with <see cref="X"/> set to the X plus the provided value.</returns>
    public RectangleD3D WithXPlus(double addX)
    {
      return new RectangleD3D(X + addX, Y, Z, SizeX, SizeY, SizeZ);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Y"/> set to the Y plus the provided value.
    /// </summary>
    /// <param name="addY">The value to add to <see cref="Y"/>.</param>
    /// <returns>New instance with <see cref="Y"/> set to the Y plus the provided value.</returns>
    public RectangleD3D WithYPlus(double addY)
    {
      return new RectangleD3D(X, Y + addY, Z, SizeX, SizeY, SizeZ);
    }

    /// <summary>
    /// Returns a new instance with <see cref="Z"/> set to the Z plus the provided value.
    /// </summary>
    /// <param name="addZ">The value to add to <see cref="Z"/>.</param>
    /// <returns>New instance with <see cref="Z"/> set to the Z plus the provided value.</returns>
    public RectangleD3D WithZPlus(double addZ)
    {
      return new RectangleD3D(X, Y, Z + addZ, SizeX, SizeY, SizeZ);
    }

    /// <summary>
    /// Returns a new instance with <see cref="SizeX"/> set to the provided value.
    /// </summary>
    /// <param name="newSizeX">The new size in x-direction.</param>
    /// <returns>A new rectangle with the updated x-size.</returns>
    public RectangleD3D WithSizeX(double newSizeX)
    {
      return new RectangleD3D(X, Y, Z, newSizeX, SizeY, SizeZ);
    }

    /// <summary>
    /// Returns a new instance with <see cref="SizeX"/> increased by the provided value.
    /// </summary>
    /// <param name="offsetSizeX">The size offset in x-direction.</param>
    /// <returns>A new rectangle with the updated x-size.</returns>
    public RectangleD3D WithSizeXPlus(double offsetSizeX)
    {
      return new RectangleD3D(X, Y, Z, SizeX + offsetSizeX, SizeY, SizeZ);
    }

    /// <summary>
    /// Returns a new instance with <see cref="SizeY"/> set to the provided value.
    /// </summary>
    /// <param name="newSizeY">The new size in y-direction.</param>
    /// <returns>A new rectangle with the updated y-size.</returns>
    public RectangleD3D WithSizeY(double newSizeY)
    {
      return new RectangleD3D(X, Y, Z, SizeX, newSizeY, SizeZ);
    }

    /// <summary>
    /// Returns a new instance with <see cref="SizeY"/> increased by the provided value.
    /// </summary>
    /// <param name="offsetSizeY">The size offset in y-direction.</param>
    /// <returns>A new rectangle with the updated y-size.</returns>
    public RectangleD3D WithSizeYPlus(double offsetSizeY)
    {
      return new RectangleD3D(X, Y, Z, SizeX, SizeY + offsetSizeY, SizeZ);
    }

    /// <summary>
    /// Returns a new instance with <see cref="SizeZ"/> set to the provided value.
    /// </summary>
    /// <param name="newSizeZ">The new size in z-direction.</param>
    /// <returns>A new rectangle with the updated z-size.</returns>
    public RectangleD3D WithSizeZ(double newSizeZ)
    {
      return new RectangleD3D(X, Y, Z, SizeX, SizeY, newSizeZ);
    }

    /// <summary>
    /// Returns a new instance with <see cref="SizeZ"/> increased by the provided value.
    /// </summary>
    /// <param name="offsetSizeZ">The size offset in z-direction.</param>
    /// <returns>A new rectangle with the updated z-size.</returns>
    public RectangleD3D WithSizeZPlus(double offsetSizeZ)
    {
      return new RectangleD3D(X, Y, Z, SizeX, SizeY, SizeZ + offsetSizeZ);
    }

    /// <summary>
    /// Returns a new instance with the specified size.
    /// </summary>
    /// <param name="newSize">The new size vector.</param>
    /// <returns>A new rectangle with the updated size.</returns>
    public RectangleD3D WithSize(VectorD3D newSize)
    {
      return new RectangleD3D(X, Y, Z, newSize.X, newSize.Y, newSize.Z);
    }

    /// <summary>
    /// Returns a new instance with the specified size offset applied.
    /// </summary>
    /// <param name="sizeOffset">The size offset.</param>
    /// <returns>A new rectangle with the updated size.</returns>
    public RectangleD3D WithSizePlus(VectorD3D sizeOffset)
    {
      return new RectangleD3D(X, Y, Z, SizeX + sizeOffset.X, SizeY + sizeOffset.Y, SizeZ + sizeOffset.Z);
    }

    #endregion Setter

    /// <summary>
    /// Gets the x-coordinate of the left side.
    /// </summary>
    public double X
    {
      get { return _x; }
    }

    /// <summary>
    /// Gets the x-coordinate of the right side.
    /// </summary>
    public double XPlusSizeX
    {
      get { return _x + _sizeX; }
    }

    /// <summary>
    /// Gets the x-coordinate of the rectangle center.
    /// </summary>
    public double XCenter
    {
      get { return _x + 0.5 * _sizeX; }
    }

    /// <summary>
    /// Gets the y-coordinate of the top side.
    /// </summary>
    public double Y
    {
      get { return _y; }
    }

    /// <summary>
    /// Gets the y-coordinate of the bottom side.
    /// </summary>
    public double YPlusSizeY
    {
      get { return _y + _sizeY; }
    }

    /// <summary>
    /// Gets the y-coordinate of the rectangle center.
    /// </summary>
    public double YCenter
    {
      get { return _y + 0.5 * _sizeY; }
    }

    /// <summary>
    /// Gets the z-coordinate of the front side.
    /// </summary>
    public double Z
    {
      get { return _z; }
    }

    /// <summary>
    /// Gets the z-coordinate of the back side.
    /// </summary>
    public double ZPlusSizeZ
    {
      get { return _z + _sizeZ; }
    }

    /// <summary>
    /// Gets the z-coordinate of the rectangle center.
    /// </summary>
    public double ZCenter
    {
      get { return _z + 0.5 * _sizeZ; }
    }

    /// <summary>
    /// Gets the size in x-direction.
    /// </summary>
    public double SizeX
    {
      get { return _sizeX; }
    }

    /// <summary>
    /// Gets the size in y-direction.
    /// </summary>
    public double SizeY
    {
      get { return _sizeY; }
    }

    /// <summary>
    /// Gets the size in z-direction.
    /// </summary>
    public double SizeZ
    {
      get { return _sizeZ; }
    }

    /// <summary>
    /// Gets the empty rectangle.
    /// </summary>
    public static RectangleD3D Empty
    {
      get
      {
        return new RectangleD3D();
      }
    }
    /// <summary>
    /// Gets a value indicating whether this rectangle is empty (all sizes are zero).
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return 0 == _sizeX && 0 == _sizeY && 0 == _sizeZ;
      }
    }

    /// <summary>
    /// Determines whether two rectangles are equal.
    /// </summary>
    /// <param name="p">The first rectangle.</param>
    /// <param name="q">The second rectangle.</param>
    /// <returns><see langword="true"/> if both rectangles are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(RectangleD3D p, RectangleD3D q)
    {
      return p._x == q._x && p._y == q._y && p._z == q._z && p._sizeX == q._sizeX && p._sizeY == q._sizeY && p._sizeZ == q._sizeZ;
    }

    /// <summary>
    /// Determines whether two rectangles are not equal.
    /// </summary>
    /// <param name="p">The first rectangle.</param>
    /// <param name="q">The second rectangle.</param>
    /// <returns><see langword="true"/> if both rectangles are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(RectangleD3D p, RectangleD3D q)
    {
      return !(p._x == q._x && p._y == q._y && p._z == q._z && p._sizeX == q._sizeX && p._sizeY == q._sizeY && p._sizeZ == q._sizeZ);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return _x.GetHashCode() + _y.GetHashCode() + _z.GetHashCode() + _sizeX.GetHashCode() + _sizeY.GetHashCode() + _sizeZ.GetHashCode();
    }
    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      return obj is RectangleD3D q && _x == q._x && _y == q._y && _z == q._z && _sizeX == q._sizeX && _sizeY == q._sizeY && _sizeZ == q._sizeZ;
    }
    /// <inheritdoc/>
    public override string ToString()
    {
      return string.Format("X={0}; Y={1}; Z={2}; SX={3}; SY={4}; SZ={5}", _x, _y, _z, _sizeX, _sizeY, _sizeZ);
    }
    /// <summary>
    /// Gets the location (left-top-front corner) of the rectangle.
    /// </summary>
    public PointD3D Location
    {
      get
      {
        return new PointD3D(_x, _y, _z);
      }
    }
    /// <summary>
    /// Gets the location plus size (right-bottom-back corner) of the rectangle.
    /// </summary>
    public PointD3D LocationPlusSize
    {
      get
      {
        return new PointD3D(_x + _sizeX, _y + _sizeY, _z + _sizeZ);
      }
    }
    /// <summary>
    /// Gets the center of this rectangle.
    /// </summary>
    /// <summary>
    /// Gets the center point of this rectangle.
    /// </summary>
    public PointD3D Center
    {
      get
      {
        return new PointD3D(_x + 0.5 * _sizeX, _y + 0.5 * _sizeY, _z + 0.5 * _sizeZ);
      }
    }

    /// <summary>
    /// Returns a new instance with the specified location.
    /// </summary>
    /// <param name="newLocation">The new location of the left-top-front corner.</param>
    /// <returns>A new rectangle with the updated location.</returns>
    public RectangleD3D WithLocation(PointD3D newLocation)
    {
      return new RectangleD3D(newLocation.X, newLocation.Y, newLocation.Z, SizeX, SizeY, SizeZ);
    }

    /// <summary>
    /// Gets the size vector of this rectangle.
    /// </summary>
    public VectorD3D Size
    {
      get
      {
        return new VectorD3D(_sizeX, _sizeY, _sizeZ);
      }
    }

    /// <summary>
    /// Determines whether the specified point is contained in this rectangle.
    /// </summary>
    /// <param name="p">The point to test.</param>
    /// <returns><see langword="true"/> if the point is contained in this rectangle; otherwise, <see langword="false"/>.</returns>
    public bool Contains(PointD3D p)
    {
      return p.X >= X && p.Y >= Y && p.Z >= Z && p.X <= (_x + _sizeX) && p.Y <= (_y + _sizeY) && p.Z <= (_z + _sizeZ);
    }

    /// <summary>Inflates the rectangle by the specified values <paramref name="x"/> and <paramref name="y"/>.</summary>
    /// <param name="x">The value used to inflate the rectangle in x-direction.</param>
    /// <param name="y">The value used to inflate the rectangle in y-direction.</param>
    /// <param name="z">The value used to inflate the rectangle in z-direction.</param>
    /// <returns>A new rectangle, inflated by the specified values <paramref name="x"/>, <paramref name="y"/> and <paramref name="z"/>.</returns>
    public RectangleD3D WithPadding(double x, double y, double z)
    {
      return new RectangleD3D(_x - x, _y - y, _z - z, _sizeX + x + x, _sizeY + y + y, _sizeZ + z + z);
    }

    /// <summary>Changes the location of the rectangle by the specified values for <paramref name="x"/> and <paramref name="y"/>.</summary>
    /// <param name="x">The x offset.</param>
    /// <param name="y">The y offset.</param>
    /// <param name="z">The z offset.</param>
    /// <returns>A new rectangle with an position that is offset with regard to the original position.</returns>
    public RectangleD3D WithOffset(double x, double y, double z)
    {
      return new RectangleD3D(_x + x, _y + y, _z + z, _sizeX, _sizeY, _sizeZ);
    }

    /// <summary>
    /// Expands this rectangle, so that it contains the point p.
    /// </summary>
    /// <param name="p">The point that should be contained in this rectangle.</param>
    /// <returns>The new rectangle that now contains the point p.</returns>
    private void ExpandToInclude(PointD3D p)
    {
      if (!(Contains(p)))
      {
        if (p.X < _x)
        {
          _sizeX += _x - p.X;
          _x = p.X;
        }
        else if (p.X > (_x + _sizeX))
        {
          _sizeX = p.X - _x;
        }

        if (p.Y < _y)
        {
          _sizeY += _y - p.Y;
          _y = p.Y;
        }
        else if (p.Y > (_y + _sizeY))
        {
          _sizeY = p.Y - _y;
        }

        if (p.Z < _z)
        {
          _sizeZ += _z - p.Z;
          _z = p.Z;
        }
        else if (p.Z > (_z + _sizeZ))
        {
          _sizeZ = p.Z - _z;
        }
      }
    }

    /// <summary>
    /// Creates a new rectangle that includes all the provided points.
    /// </summary>
    /// <param name="points">The points that the rectangle should include.</param>
    /// <returns>The rectangle that includes all the provided points.</returns>
    /// <exception cref="System.ArgumentException">Enumeration is empty!</exception>
    public static RectangleD3D NewRectangleIncludingAllPoints(IEnumerable<PointD3D> points)
    {
      var en = points.GetEnumerator();
      if (!en.MoveNext())
        throw new ArgumentException("Enumeration is empty!", nameof(points));

      var result = new RectangleD3D(en.Current, VectorD3D.Empty);

      while (en.MoveNext())
      {
        result.ExpandToInclude(en.Current);
      }

      return result;
    }

    /// <summary>
    /// Returns a rectangle that is based on the current rectangle, but was expanded to include all the provided points.
    /// </summary>
    /// <param name="points">The points to include.</param>
    /// <returns>A rectangle that is based on the current rectangle, but was expanded to include all the provided points.</returns>
    public RectangleD3D WithPointsIncluded(IEnumerable<PointD3D> points)
    {
      var result = this;
      foreach (var p in points)
        result.ExpandToInclude(p);
      return result;
    }

    /// <summary>
    /// Returns a rectangle that is based on the current rectangle, but was expanded to include all vertex points of the provided rectangle <paramref name="r"/>.
    /// </summary>
    /// <param name="r">The rectangle, whose vertices have to be included.</param>
    /// <returns>A rectangle that is based on the current rectangle, but was expanded to include all vertex points of the provided rectangle <paramref name="r"/>.</returns>
    public RectangleD3D WithRectangleIncluded(RectangleD3D r)
    {
      return WithPointsIncluded(r.Vertices);
    }

    /// <summary>
    /// Gets the vertices in binary order (x: 0th digit, y: 1st digit, z: 2nd digit).
    /// </summary>
    public IEnumerable<PointD3D> Vertices
    {
      get
      {
        yield return new PointD3D(_x, _y, _z);
        yield return new PointD3D(_x + _sizeX, _y, _z);
        yield return new PointD3D(_x, _y + _sizeY, _z);
        yield return new PointD3D(_x + _sizeX, _y + _sizeY, _z);
        yield return new PointD3D(_x, _y, _z + _sizeZ);
        yield return new PointD3D(_x + _sizeX, _y, _z + _sizeZ);
        yield return new PointD3D(_x, _y + _sizeY, _z + _sizeZ);
        yield return new PointD3D(_x + _sizeX, _y + _sizeY, _z + _sizeZ);
      }
    }
    /// <summary>
    /// Gets the edges of the rectangle (first bottom edges, then the pillars, and then the top edges).
    /// </summary>
    public IEnumerable<LineD3D> Edges
    {
      get
      {
        // Bottom
        yield return new LineD3D(new PointD3D(_x, _y, _z), new PointD3D(_x + _sizeX, _y, _z));
        yield return new LineD3D(new PointD3D(_x + _sizeX, _y, _z), new PointD3D(_x + _sizeX, _y + _sizeY, _z));
        yield return new LineD3D(new PointD3D(_x + _sizeX, _y + _sizeY, _z), new PointD3D(_x, _y + _sizeY, _z));
        yield return new LineD3D(new PointD3D(_x, _y + _sizeY, _z), new PointD3D(_x, _y, _z));

        // Pillars
        yield return new LineD3D(new PointD3D(_x, _y, _z), new PointD3D(_x, _y, _z + _sizeZ));
        yield return new LineD3D(new PointD3D(_x + _sizeX, _y, _z), new PointD3D(_x + _sizeX, _y, _z + _sizeZ));
        yield return new LineD3D(new PointD3D(_x + _sizeX, _y + _sizeY, _z), new PointD3D(_x + _sizeX, _y + _sizeY, _z + _sizeZ));
        yield return new LineD3D(new PointD3D(_x, _y + _sizeY, _z), new PointD3D(_x, _y + _sizeY, _z + _sizeZ));

        // Top
        yield return new LineD3D(new PointD3D(_x, _y, _z + _sizeZ), new PointD3D(_x + _sizeX, _y, _z + _sizeZ));
        yield return new LineD3D(new PointD3D(_x + _sizeX, _y, _z + _sizeZ), new PointD3D(_x + _sizeX, _y + _sizeY, _z + _sizeZ));
        yield return new LineD3D(new PointD3D(_x + _sizeX, _y + _sizeY, _z + _sizeZ), new PointD3D(_x, _y + _sizeY, _z + _sizeZ));
        yield return new LineD3D(new PointD3D(_x, _y + _sizeY, _z + _sizeZ), new PointD3D(_x, _y, _z + _sizeZ));
      }
    }
    /// <summary>
    /// Gets an enumeration of the planes of the faces of this rectangle (left, right, front, back, bottom, top).
    /// </summary>
    public IEnumerable<PlaneD3D> Planes
    {
      get
      {
        yield return new PlaneD3D(-1, 0, 0, 0); // Left
        yield return new PlaneD3D(1, 0, 0, _sizeX); // Right
        yield return new PlaneD3D(0, -1, 0, 0); // Front
        yield return new PlaneD3D(0, 1, 0, _sizeY); // Back
        yield return new PlaneD3D(0, 0, -1, 0); // Bottom
        yield return new PlaneD3D(0, 0, 1, _sizeZ); // Top
      }
    }
    /// <summary>
    /// Gets the triangle indices of all faces using the vertices returned by <see cref="Vertices"/>.
    /// The order is front, back, top, bottom, left, right.
    /// </summary>
    public IEnumerable<Tuple<int, int, int>> TriangleIndices
    {
      get
      {
        return GetTriangleIndices();
      }
    }
    /// <summary>
    /// Gets the triangle indices of all faces using the vertices returned by <see cref="Vertices"/>.
    /// The order is front, back, top, bottom, left, right.
    /// </summary>
    /// <returns>The triangle indices for all faces.</returns>
    public static IEnumerable<Tuple<int, int, int>> GetTriangleIndices()
    {
      // Front
      yield return new Tuple<int, int, int>(0, 2, 3);
      yield return new Tuple<int, int, int>(0, 3, 1);
      // Back
      yield return new Tuple<int, int, int>(4, 7, 6);
      yield return new Tuple<int, int, int>(4, 5, 7);
      // Top
      yield return new Tuple<int, int, int>(2, 6, 7);
      yield return new Tuple<int, int, int>(2, 7, 3);
      // Bottom
      yield return new Tuple<int, int, int>(0, 5, 4);
      yield return new Tuple<int, int, int>(0, 1, 5);
      // Left
      yield return new Tuple<int, int, int>(0, 4, 6);
      yield return new Tuple<int, int, int>(0, 6, 2);
      // Right
      yield return new Tuple<int, int, int>(1, 7, 5);
      yield return new Tuple<int, int, int>(1, 3, 7);
    }
    /// <summary>
    /// Gets a rectangle that includes the smallest circle around this rectangle.
    /// </summary>
    public RectangleD3D OuterCircleBoundingBox
    {
      get
      {
        double d = Calc.BasicFunctions.hypot(_sizeX, _sizeY, _sizeZ);
        return new RectangleD3D(_x + 0.5 * (_sizeX - d), _y + 0.5 * (_sizeY - d), _z + 0.5 * (_sizeZ - d), d, d, d);
      }
    }
  }
}
