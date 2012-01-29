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
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph
{

  /// <summary>
  /// Represents a point location in 2D, or alternatively, a vector from the origin to a given point.
  /// </summary>
  [Serializable]
  public struct PointD2D
  {
    public double X;
    public double Y;

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PointD2D), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        PointD2D s = (PointD2D)obj;
        info.AddValue("X", s.X);
        info.AddValue("Y", s.Y);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        PointD2D s = null != o ? (PointD2D)o : new PointD2D();
        s.X = info.GetDouble("X");
        s.Y = info.GetDouble("Y");
        return s;
      }
    }
    #endregion


    public PointD2D(double x, double y)
    {
      X = x;
      Y = y;
    }

    public static PointD2D Empty
    {
      get
      {
        return new PointD2D(0, 0);
      }
    }

		public bool IsEmpty
		{
			get
			{
				return 0 == X && 0 == Y;
			}
		}

    public static bool operator ==(PointD2D p, PointD2D q)
    {
      return p.X == q.X && p.Y == q.Y;
    }
    public static bool operator !=(PointD2D p, PointD2D q)
    {
      return !(p.X == q.X && p.Y == q.Y);
    }
    public override int GetHashCode()
    {
      return X.GetHashCode() + Y.GetHashCode();
    }
    public override bool Equals(object obj)
    {
      if (obj is PointD2D)
      {
        PointD2D q = (PointD2D)obj;
        return X == q.X && Y == q.Y;
      }
      else
      {
        return false;
      }
    }
    public override string ToString()
    {
      return X.ToString() + "; " + Y.ToString();
    }

    public static explicit operator System.Drawing.PointF(PointD2D p)
    {
      return new System.Drawing.PointF((float)p.X, (float)p.Y);
    }

    public static implicit operator PointD2D(System.Drawing.PointF p)
    {
      return new PointD2D(p.X, p.Y);
    }

    public static explicit operator System.Drawing.SizeF(PointD2D p)
    {
      return new System.Drawing.SizeF((float)p.X, (float)p.Y);
    }

    public static implicit operator PointD2D(System.Drawing.SizeF p)
    {
      return new PointD2D(p.Width, p.Height);
    }

    /// <summary>
    /// Calculates the distance between this point and another point.
    /// </summary>
    /// <param name="p">Other point.</param>
    /// <returns>The distance between this point and point p.</returns>
    public double DistanceTo(PointD2D p)
    {
      return Distance(this, p);
    }

    public double VectorLength
    {
      get
      {
        return Math.Sqrt(X * X + Y * Y);
      }
    }

    public static PointD2D operator -(PointD2D p1, PointD2D p2)
    {
      return new PointD2D(p1.X - p2.X, p1.Y - p2.Y);
    }

    public static PointD2D operator +(PointD2D p1, PointD2D p2)
    {
      return new PointD2D(p1.X + p2.X, p1.Y + p2.Y);
    }

    public static PointD2D operator *(PointD2D p, double s)
    {
      return new PointD2D(p.X * s, p.Y * s);
    }
    
    public static PointD2D operator *(double s, PointD2D p)
    {
      return new PointD2D(p.X * s, p.Y * s);
    }

    public static PointD2D operator /(PointD2D p, double s)
    {
      return new PointD2D(p.X / s, p.Y / s);
    }
   

    public void FlipXY()
    {
      var h = X;
      X = Y;
      Y = h;
    }

    public PointD2D GetXYFlipped()
    {
      return new PointD2D(Y, X);
    }

    public PointD2D Get90DegreeRotated()
    {
      return new PointD2D(-Y, X);
    }

    public PointD2D GetSignFlipped()
    {
      return new PointD2D(-X, -Y);
    }

    public PointD2D GetRotatedByDegree(double rotation, PointD2D pivot)
    {
      return GetRotatedByRad(rotation * Math.PI / 180, pivot);  
    }

    public PointD2D GetRotatedByDegree(double rotation)
    {
      return GetRotatedByRad(rotation * Math.PI / 180, PointD2D.Empty);
    }


    public PointD2D GetRotatedByRad(double phi, PointD2D pivot)
    {
      double dx = X - pivot.X;
      double dy = Y - pivot.Y;

      if (phi != 0)
      {
        double cosphi = Math.Cos(phi);
        double sinphi = Math.Sin(phi);
        return new PointD2D(pivot.X + (dx * cosphi - dy * sinphi), pivot.Y + (dx * sinphi + dy * cosphi));
      }
      else
      {
        return new PointD2D(X, Y);
      }
    }

    public PointD2D GetRotatedByRad(double phi)
    {
      return GetRotatedByRad(phi, PointD2D.Empty);
    }

    public double DotProduct(PointD2D q)
    {
      return DotProduct(this, q);
    }

    public static double DotProduct(PointD2D p, PointD2D q)
    {
      return p.X * q.X + p.Y * q.Y;
    }

    public PointD2D GetNormalized()
    {
      var s = 1 / VectorLength;
      return new PointD2D(X * s, Y * s);
    }

    /// <summary>
    /// Calculates the distance between two points.
    /// </summary>
    /// <param name="p1">First point.</param>
    /// <param name="p2">Second point.</param>
    /// <returns>The distance between points p1 and p2.</returns>
    public static double Distance(PointD2D p1, PointD2D p2)
    {
      double x = p1.X - p2.X;
      double y = p1.Y - p2.Y;
      return Math.Sqrt(x * x + y * y);
    }


    /// <summary>
    /// Calculates the squared distance between a finite line and a point.
    /// </summary>
    /// <param name="point">The location of the point.</param>
    /// <param name="lineOrg">The location of the line origin.</param>
    /// <param name="lineEnd">The location of the line end.</param>
    /// <returns>The squared distance between the line (threated as having a finite length) and the point.</returns>
    public static double SquareDistanceLineToPoint(PointD2D point, PointD2D lineOrg, PointD2D lineEnd)
    {
      var linex = lineEnd.X - lineOrg.X;
      var liney = lineEnd.Y - lineOrg.Y;
      var pointx = point.X - lineOrg.X;
      var pointy = point.Y - lineOrg.Y;

      var rsquare = linex * linex + liney * liney;
      var xx = linex * pointx + liney * pointy;
      if (xx <= 0) // the point is located before the line, so use
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
    public static bool IsPointIntoDistance(PointD2D point, double distance, PointD2D lineOrg, PointD2D lineEnd)
    {
      // first a quick test if the point is far outside the circle
      // that is spanned from the middle of the line and has at least
      // a radius of half of the line length plus the distance
      var xm = (lineOrg.X + lineEnd.X) / 2;
      var ym = (lineOrg.Y + lineEnd.Y) / 2;
      var r = Math.Abs(lineOrg.X - xm) + Math.Abs(lineOrg.Y - ym) + distance;
      if (Math.Max(Math.Abs(point.X - xm), Math.Abs(point.Y - ym)) > r)
        return false;
      else
        return SquareDistanceLineToPoint(point, lineOrg, lineEnd) <= distance * distance;

    }
  }
}
