#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
  /// A rectangle in 2D space.
  /// </summary>
  [Serializable]
  public struct RectangleD2D : IEquatable<RectangleD2D>
  {
    private double _x, _y, _w, _h;

    /// <summary>
    /// The x location (the x coordinate of the left side).
    /// </summary>
    public double X
    {
      get { return _x; }
      set { _x = value; }
    }

    /// <summary>
    /// The y location (the y coordinate of the top side).
    /// </summary>
    public double Y
    {
      get { return _y; }
      set { _y = value; }
    }

    /// <summary>
    /// The width of the rectangle.
    /// </summary>
    public double Width
    {
      get { return _w; }
      set { _w = value; }
    }

    /// <summary>
    /// The height of the rectangle.
    /// </summary>
    public double Height
    {
      get { return _h; }
      set { _h = value; }
    }

    #region Serialization

    /// <summary>
    /// V1: 2015-11-15 Version 1 - Move to Altaxo.Geometry namespace and renaming to RectangleD2D
    /// V2: 2023-01-14 Move from assembly AltaxoBase to AltaxoCore
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.RectangleD", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Geometry.RectangleD2D", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RectangleD2D), 2)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RectangleD2D)obj;
        info.AddValue("X", s.X);
        info.AddValue("Y", s.Y);
        info.AddValue("Width", s.Width);
        info.AddValue("Height", s.Height);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        RectangleD2D s = o is not null ? (RectangleD2D)o : new RectangleD2D();
        s.X = info.GetDouble("X");
        s.Y = info.GetDouble("Y");
        s.Width = info.GetDouble("Width");
        s.Height = info.GetDouble("Height");

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleD2D"/> struct.
    /// </summary>
    /// <param name="x">The x location (left).</param>
    /// <param name="y">The y location (top).</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public RectangleD2D(double x, double y, double width, double height)
      : this()
    {
      _x = x;
      _y = y;
      _w = width;
      _h = height;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleD2D"/> struct.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="size">The size.</param>
    public RectangleD2D(PointD2D position, PointD2D size)
    {
      _x = position.X;
      _y = position.Y;
      _w = size.X;
      _h = size.Y;
    }

    /// <summary>
    /// Gets the empty rectangle with the location (0, 0).
    /// </summary>
    public static RectangleD2D Empty
    {
      get
      {
        return new RectangleD2D();
      }
    }

    /// <summary>
    /// Gets a value indicating whether the rectangle is empty. Per definition,
    /// it is empty if both width and height are zero.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this rectangle is empty; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmpty
    {
      get
      {
        return 0 == _w && 0 == _h;
      }
    }

    /// <summary>
    /// Creates a rectangle from the left, top, right and bottom coordinate values.
    /// </summary>
    /// <param name="a">Left(x)-top(y) point.</param>
    /// <param name="b">Right(x)-bottom(y) point.</param>
    /// <returns>The created rectangle.</returns>
    public static RectangleD2D FromLTRB(PointD2D a, PointD2D b)
    {
      return new RectangleD2D(a.X, a.Y, b.X - a.X, b.Y - a.Y);
    }

    /// <summary>
    /// Creates a rectangle from the left, top, right and bottom coordinate values.
    /// </summary>
    /// <param name="ax">The left x.</param>
    /// <param name="ay">The top y.</param>
    /// <param name="bx">The right x.</param>
    /// <param name="by">The bottom y.</param>
    /// <returns>The created rectangle</returns>
    public static RectangleD2D FromLTRB(double ax, double ay, double bx, double by)
    {
      return new RectangleD2D(ax, ay, bx - ax, by - ay);
    }

    /// <inheritdoc/>
    public bool Equals(RectangleD2D q)
    {
      return _x == q._x && _y == q._y && _w == q._w && _h == q._h;
    }

    /// <inheritdoc/>
    public static bool operator ==(RectangleD2D p, RectangleD2D q)
    {
      return p.Equals(q);
    }

    /// <inheritdoc/>
    public static bool operator !=(RectangleD2D p, RectangleD2D q)
    {
      return !(p.Equals(q));
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      unchecked
      {
        return X.GetHashCode() + 5 * Y.GetHashCode() + 7 * Width.GetHashCode() + 11 * Height.GetHashCode();
      }
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      if (obj is RectangleD2D)
      {
        var q = (RectangleD2D)obj;
        return _x == q._x && _y == q._y && _w == q._w && _h == q._h;
      }
      else
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return string.Format("X={0}; Y={1}; W={2}; H={3}", _x, _y, _w, _h);
      ;
    }

    /// <summary>
    /// Gets or sets the location (of the left-top corner of the rectangle).
    /// </summary>
    /// <value>
    /// The location.
    /// </value>
    public PointD2D Location
    {
      get
      {
        return new PointD2D(_x, _y);
      }
      set
      {
        _x = value.X;
        _y = value.Y;
      }
    }

    /// <summary>
    /// Gets or sets the size of the rectangle.
    /// </summary>
    public PointD2D Size
    {
      get
      {
        return new PointD2D(_w, _h);
      }
      set
      {
        _w = value.X;
        _h = value.Y;
      }
    }

    /// <summary>
    /// Gets the y coordinate of the top.
    /// </summary>
    public double Top { get { return _y; } }

    /// <summary>
    /// Gets the y coordinate of the bottom.
    /// </summary>
    public double Bottom { get { return _y + _h; } }

    /// <summary>
    /// Gets the x coordinate of the left.
    /// </summary>
    public double Left { get { return _x; } }

    /// <summary>
    /// Gets the x coordinate of the right.
    /// </summary>
    public double Right { get { return _x + _w; } }

    /// <summary>
    /// Gets the left(x)-top(y) point of the rectangle.
    /// </summary>
    public PointD2D LeftTop { get { return new PointD2D(_x, _y); } }

    /// <summary>
    /// Gets the center(x)-top(y) point of the rectangle.
    /// </summary>
    public PointD2D CenterTop { get { return new PointD2D(_x + _w / 2, _y); } }

    /// <summary>
    /// Gets the right(x)-top(y) point of the rectangle.
    /// </summary>
    public PointD2D RightTop { get { return new PointD2D(_x + _w, _y); } }

    /// <summary>
    /// Gets the left(x)-center(y) point of the rectangle.
    /// </summary>
    public PointD2D LeftCenter { get { return new PointD2D(_x, _y + _h / 2); } }

    /// <summary>
    /// Gets the center(x)-center(y) point of the rectangle.
    /// </summary>
    public PointD2D CenterCenter { get { return new PointD2D(_x + _w / 2, _y + _h / 2); } }

    /// <summary>
    /// Gets the right(x)-center(y) point of the rectangle.
    /// </summary>
    public PointD2D RightCenter { get { return new PointD2D(_x + _w, _y + _h / 2); } }

    /// <summary>
    /// Gets the left(x)-bottom(y) point of the rectangle.
    /// </summary>
    public PointD2D LeftBottom { get { return new PointD2D(_x, _y + _h); } }

    /// <summary>
    /// Gets the center(x)-bottom(y) point of the rectangle.
    /// </summary>
    public PointD2D CenterBottom { get { return new PointD2D(_x + _w / 2, _y + _h); } }

    /// <summary>
    /// Gets the right(x)-bottom(y) point of the rectangle.
    /// </summary>
    public PointD2D RightBottom { get { return new PointD2D(_x + _w, _y + _h); } }

    /// <summary>
    /// Gets a new rectangle that is the left half of this rectangle.
    /// </summary>
    public RectangleD2D LeftHalf { get { return new RectangleD2D(_x, _y, _w / 2, _h); } }

    /// <summary>
    /// Gets a new rectangle that is the right half of this rectangle.
    /// </summary>
    public RectangleD2D RightHalf { get { return new RectangleD2D(_x + _w / 2, _y, _w / 2, _h); } }

    /// <summary>
    /// Gets a new rectangle that is the top half of this rectangle.
    /// </summary>
    public RectangleD2D TopHalf { get { return new RectangleD2D(_x, _y, _w, _h / 2); } }

    /// <summary>
    /// Gets a new rectangle that is the bottom half of this rectangle.
    /// </summary>
    public RectangleD2D BottomHalf { get { return new RectangleD2D(_x, _y + _h / 2, _w, _h / 2); } }


    public bool Contains(PointD2D p)
    {
      return _x <= p.X && p.X < (_x + _w) && _y <= p.Y && p.Y < (_y + _h);
    }
    public bool Contains(RectangleD2D r)
    {
      return (_x <= r._x) && (r._x + r._w) <= (_x + _w) &&
             (_y <= r._y) && (r._y + r._h) <= (_y + _h);
    }

    /// <summary>
    /// Tests if this rectangle and the provided rectangle intersects.
    /// </summary>
    /// <param name="r">The rectangle to test.</param>
    /// <returns>True if this rectangle and the provided rectangle have a non-empty intersection area; otherwise <c>false</c>.</returns>
    public bool IntersectsWith(RectangleD2D r)
    {
      return (r._x < _x + _w) && (_x < (r._x + r._w)) &&
              (r._y < _y + _h) && (_y < r._y + r._h);
    }


    /// <summary>Inflates the rectangle by the specified values <paramref name="x"/> and <paramref name="y"/>.</summary>
    /// <param name="x">The value used to inflate the rectangle in x-direction.</param>
    /// <param name="y">The value used to inflate the rectangle in y-direction.</param>
    public void Inflate(double x, double y)
    {
      _x -= x;
      _w += x + x;
      _y -= y;
      _h += y + y;
    }

    /// <summary>Changes the location of the rectangle by the specified values for <paramref name="x"/> and <paramref name="y"/>.</summary>
    /// <param name="x">The x offset.</param>
    /// <param name="y">The y offset.</param>
    public void Offset(double x, double y)
    {
      _x += x;
      _y += y;
    }

    /// <summary>
    /// Expands this rectangle, so that it contains the point p.
    /// </summary>
    /// <param name="p">The point that should be contained in this rectangle.</param>
    /// <returns>The new rectangle that now contains the point p.</returns>
    public void ExpandToInclude(PointD2D p)
    {
      if (!(Contains(p)))
      {
        if (p.X < X)
        {
          Width += X - p.X;
          X = p.X;
        }
        else if (p.X > Right)
        {
          Width = p.X - X;
        }

        if (p.Y < Y)
        {
          Height += Y - p.Y;
          Y = p.Y;
        }
        else if (p.Y > Bottom)
        {
          Height = p.Y - Y;
        }
      }
    }

    /// <summary>
    /// Gets the vertices in binary order (x: 0th digit, y: 1st digit).
    /// </summary>
    /// <value>
    /// The vertices of this rectangle.
    /// </value>
    public IEnumerable<PointD2D> Vertices
    {
      get
      {
        yield return new PointD2D(_x, _y);
        yield return new PointD2D(_x + _w, _y);
        yield return new PointD2D(_x, _y + _h);
        yield return new PointD2D(_x + _w, _y + _h);
      }
    }

    /// <summary>
    /// Creates a rectangle that includes all the provided points.
    /// </summary>
    /// <param name="points">The points that the rectangle should include.</param>
    /// <returns>The rectangle that includes all the provided points.</returns>
    /// <exception cref="System.ArgumentException">Enumeration is empty!</exception>
    public static RectangleD2D NewRectangleIncludingAllPoints(IEnumerable<PointD2D> points)
    {
      var en = points.GetEnumerator();
      if (!en.MoveNext())
        throw new ArgumentException("Enumeration is empty!", nameof(points));

      var result = new RectangleD2D(en.Current, PointD2D.Empty);

      while (en.MoveNext())
      {
        result.ExpandToInclude(en.Current);
      }

      return result;
    }

    /// <summary>
    /// Expands this rectangle, so that it now includes its former shape, and the rectangle provided in the argument.
    /// </summary>
    /// <param name="rect">The rectangle to include.</param>
    public void ExpandToInclude(RectangleD2D rect)
    {
      ExpandToInclude(rect.LeftTop);
      ExpandToInclude(rect.LeftBottom);
      ExpandToInclude(rect.RightTop);
      ExpandToInclude(rect.RightBottom);
    }

    /// <summary>
    /// Gets the rectangular bounding box that includes both rectangles.
    /// </summary>
    /// <param name="first">The first rectangle.</param>
    /// <param name="other">The other rectangle.</param>
    /// <returns>The rectangular bounding box that includes both rectangles.
    /// If only one rectangle is provided, that rectangle is returned.
    /// If neither rectangle is provided, the return value is null.
    /// </returns>
    /// <exception cref="System.InvalidProgramException"></exception>
    public static RectangleD2D? ExpandToInclude(RectangleD2D? first, RectangleD2D? other)
    {
      if (first.HasValue && other.HasValue)
      {
        var result = first.Value;
        result.ExpandToInclude(other.Value);
        return result;
      }
      else if (first is null && other is null)
        return null;
      else if (first.HasValue)
        return first;
      else if (other.HasValue)
        return other;
      else
        throw new InvalidProgramException();
    }


    /// <summary>Expands the rectangle by the specified margin.</summary>
    /// <param name="margin">The margin.</param>
    public void Expand(Margin2D margin)
    {
      _x -= margin.Left;
      _w += (margin.Left + margin.Right);
      _y -= margin.Top;
      _h += (margin.Top + margin.Bottom);
    }

    /// <summary>Gets a rectangle that includes the smallest circle around this rectangle.</summary>
    /// <value>A rectangle that includes the smallest circle around this rectangle.</value>
    public RectangleD2D OuterCircleBoundingBox
    {
      get
      {
        double d = Hypot(_w, _h);
        return new RectangleD2D(_x + 0.5 * (_w - d), _y + 0.5 * (_h - d), d, d);
      }
    }

    /// <summary>
    /// The standard hypot() function for two arguments taking care of overflows and zerodivides.
    /// </summary>
    /// <param name="x">First argument.</param>
    /// <param name="y">Second argument.</param>
    /// <returns>Square root of the sum of x-square and y-square.</returns>
    private static double Hypot(double x, double y)
    {
      double xabs = Math.Abs(x);
      double yabs = Math.Abs(y);
      double min, max;

      if (xabs < yabs)
      {
        min = xabs;
        max = yabs;
      }
      else
      {
        min = yabs;
        max = xabs;
      }

      if (min == 0)
      {
        return max;
      }

      {
        double u = min / max;
        return max * Math.Sqrt(1 + u * u);
      }
    }
  }
}
