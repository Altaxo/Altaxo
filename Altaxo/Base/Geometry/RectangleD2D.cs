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

using System;
using System.Collections.Generic;

namespace Altaxo.Geometry
{
  /// <summary>
  /// RectangleD describes a rectangle in 2D space.
  /// </summary>
  [Serializable]
  public struct RectangleD2D : IEquatable<RectangleD2D>
  {
    private double _x, _y, _w, _h;

    public double X
    {
      get { return _x; }
      set { _x = value; }
    }

    public double Y
    {
      get { return _y; }
      set { _y = value; }
    }

    public double Width
    {
      get { return _w; }
      set { _w = value; }
    }

    public double Height
    {
      get { return _h; }
      set { _h = value; }
    }

    #region Serialization

    /// <summary>
    /// 2015-11-15 Version 1 - Move to Altaxo.Geometry namespace and renaming to RectangleD2D
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.RectangleD", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RectangleD2D), 1)]
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

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        RectangleD2D s = null != o ? (RectangleD2D)o : new RectangleD2D();
        s.X = info.GetDouble("X");
        s.Y = info.GetDouble("Y");
        s.Width = info.GetDouble("Width");
        s.Height = info.GetDouble("Height");

        return s;
      }
    }

    #endregion Serialization

    public RectangleD2D(double x, double y, double width, double height)
      : this()
    {
      _x = x;
      _y = y;
      _w = width;
      _h = height;
    }

    public RectangleD2D(PointD2D position, PointD2D size)
    {
      _x = position.X;
      _y = position.Y;
      _w = size.X;
      _h = size.Y;
    }

    public static RectangleD2D Empty
    {
      get
      {
        return new RectangleD2D();
      }
    }

    public bool IsEmpty
    {
      get
      {
        return 0 == _w && 0 == _h;
      }
    }

    public static RectangleD2D FromLTRB(PointD2D a, PointD2D b)
    {
      return new RectangleD2D(a.X, a.Y, b.X - a.X, b.Y - a.Y);
    }

    public static RectangleD2D FromLTRB(double ax, double ay, double bx, double by)
    {
      return new RectangleD2D(ax, ay, bx - ax, by - ay);
    }

    public bool Equals(RectangleD2D q)
    {
      return _x == q._x && _y == q._y && _w == q._w && _h == q._h;
    }

    public static bool operator ==(RectangleD2D p, RectangleD2D q)
    {
      return p.Equals(q);
    }

    public static bool operator !=(RectangleD2D p, RectangleD2D q)
    {
      return !(p.Equals(q));
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return X.GetHashCode() + 5 * Y.GetHashCode() + 7 * Width.GetHashCode() + 11 * Height.GetHashCode();
      }
    }

    public override bool Equals(object obj)
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

    public override string ToString()
    {
      return string.Format("X={0}; Y={1}; W={2}; H={3}", _x, _y, _w, _h);
      ;
    }

    public static explicit operator System.Drawing.RectangleF(RectangleD2D r)
    {
      return new System.Drawing.RectangleF((float)r._x, (float)r._y, (float)r._w, (float)r._h);
    }

    public static explicit operator System.Drawing.Rectangle(RectangleD2D r)
    {
      return new System.Drawing.Rectangle((int)r._x, (int)r._y, (int)r._w, (int)r._h);
    }

    public static implicit operator RectangleD2D(System.Drawing.RectangleF r)
    {
      return new RectangleD2D(r.X, r.Y, r.Width, r.Height);
    }

    public static implicit operator RectangleD2D(System.Drawing.Rectangle r)
    {
      return new RectangleD2D(r.X, r.Y, r.Width, r.Height);
    }

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

    public double Top { get { return _y; } }

    public double Bottom { get { return _y + _h; } }

    public double Left { get { return _x; } }

    public double Right { get { return _x + _w; } }

    public PointD2D LeftTop { get { return new PointD2D(_x, _y); } }

    public PointD2D LeftBottom { get { return new PointD2D(_x, _y + _h); } }

    public PointD2D RightTop { get { return new PointD2D(_x + _w, _y); } }

    public PointD2D RightBottom { get { return new PointD2D(_x + _w, _y + _h); } }

    public PointD2D CenterCenter { get { return new PointD2D(_x + _w / 2, _y + _h / 2); } }

    public bool Contains(PointD2D p)
    {
      return p.X >= X && p.Y >= Y && p.X <= Right && p.Y <= Bottom;
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

    public void ExpandToInclude(RectangleD2D rect)
    {
      ExpandToInclude(rect.LeftTop);
      ExpandToInclude(rect.LeftBottom);
      ExpandToInclude(rect.RightTop);
      ExpandToInclude(rect.RightBottom);
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
