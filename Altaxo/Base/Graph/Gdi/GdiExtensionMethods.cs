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
using System.Drawing;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi
{
  public static class GdiExtensionMethods
  {
    public static PointF Subtract(this PointF p1, PointF p2)
    {
      return new PointF(p1.X - p2.X, p1.Y - p2.Y);
    }

    public static PointF Add(this PointF p1, PointF p2)
    {
      return new PointF(p1.X + p2.X, p1.Y + p2.Y);
    }

    public static PointF AddScaled(this PointF p1, PointF p2, float s)
    {
      return new PointF(p1.X + p2.X * s, p1.Y + p2.Y * s);
    }

    public static float VectorLength(this PointF p)
    {
      return (float)Math.Sqrt(p.X * p.X + p.Y * p.Y);
    }

    public static float VectorLengthSquared(this PointF p)
    {
      return (p.X * p.X + p.Y * p.Y);
    }

    public static float DistanceTo(this PointF p, PointF q)
    {
      var dx = p.X - q.X;
      var dy = p.Y - q.Y;
      return (float)Math.Sqrt(dx * dx + dy * dy);
    }

    public static float DistanceSquaredTo(this PointF p, PointF q)
    {
      var dx = p.X - q.X;
      var dy = p.Y - q.Y;
      return dx * dx + dy * dy;
    }

    public static PointF FlipXY(this PointF p)
    {
      return new PointF(p.Y, p.X);
    }

    public static PointF Rotate90Degree(this PointF p)
    {
      return new PointF(-p.Y, p.X);
    }

    public static PointF FlipSign(this PointF p)
    {
      return new PointF(-p.X, -p.Y);
    }

    public static float DotProduct(this PointF p, PointF q)
    {
      return p.X * q.X + p.Y * q.Y;
    }

    public static PointF Normalize(this PointF p)
    {
      var s = 1 / Math.Sqrt(p.X * p.X + p.Y * p.Y);
      return new PointF((float)(p.X * s), (float)(p.Y * s));
    }

    /// <summary>
    /// Multiply a size structure with a factor.
    /// </summary>
    /// <param name="s">The size to scale.</param>
    /// <param name="factor">The scale value.</param>
    /// <returns>SizeF structure as result of the multiplication of the original size with the factor.</returns>
    public static SizeF Scale(this SizeF s, double factor)
    {
      return new SizeF((float)(s.Width * factor), (float)(s.Height * factor));
    }

    /// <summary>
    /// Multiply a PointF structure with a factor.
    /// </summary>
    /// <param name="s">The point to scale.</param>
    /// <param name="factor">The scale value.</param>
    /// <returns>PointF structure as result of the multiplication of the original point with the factor.</returns>
    public static PointF Scale(this PointF s, double factor)
    {
      return new PointF((float)(s.X * factor), (float)(s.Y * factor));
    }

    /// <summary>
    /// Multiply a RectangleF structure with a factor.
    /// </summary>
    /// <param name="s">The rectangle to scale.</param>
    /// <param name="factor">The scale value.</param>
    /// <returns>RectangleF structure as result of the multiplication of the original rectangle with the factor.</returns>
    public static RectangleF Scale(this RectangleF s, double factor)
    {
      return new RectangleF((float)(s.X * factor), (float)(s.Y * factor), (float)(s.Width * factor), (float)(s.Height * factor));
    }

    /// <summary>
    /// Calculates the half of the original size.
    /// </summary>
    /// <param name="s">Original size.</param>
    /// <returns>Half of the original size</returns>
    public static SizeF Half(this SizeF s)
    {
      return new SizeF(s.Width / 2, s.Height / 2);
    }

    /// <summary>
    /// Calculates the center of the provided rectangle.
    /// </summary>
    /// <param name="r">The rectangle.</param>
    /// <returns>The position of the center of the rectangle.</returns>
    public static PointF Center(this RectangleF r)
    {
      return new PointF(r.X + r.Width / 2, r.Y + r.Height / 2);
    }

    /// <summary>
    /// Expands the rectangle r, so that is contains the point p.
    /// </summary>
    /// <param name="r">The rectangle to expand.</param>
    /// <param name="p">The point that should be contained in this rectangle.</param>
    /// <returns>The new rectangle that now contains the point p.</returns>
    public static RectangleF ExpandToInclude(this RectangleF r, PointF p)
    {
      if (!(r.Contains(p)))
      {
        if (p.X < r.X)
        {
          r.Width += r.X - p.X;
          r.X = p.X;
        }
        else if (p.X > r.Right)
        {
          r.Width = p.X - r.X;
        }

        if (p.Y < r.Y)
        {
          r.Height += r.Y - p.Y;
          r.Y = p.Y;
        }
        else if (p.Y > r.Bottom)
        {
          r.Height = p.Y - r.Y;
        }
      }
      return r;
    }

    #region Polyline constituted by an array of PointF

    /// <summary>
    /// Calculates the total length of a polyline.
    /// </summary>
    /// <param name="polyline">The polyline.</param>
    /// <returns>Total length of the polyline.</returns>
    /// <exception cref="ArgumentNullException">polyline</exception>
    /// <exception cref="ArgumentException">Polyline must have at least 2 points - polyline</exception>
    public static double TotalLineLength(this PointF[] polyline)
    {
      if (null == polyline)
        throw new ArgumentNullException(nameof(polyline));
      if (polyline.Length < 2)
        throw new ArgumentException("Polyline must have at least 2 points", nameof(polyline));

      double sum = 0;

      PointF prev = polyline[0];
      PointF curr;
      for (int i = 1; i < polyline.Length; ++i)
      {
        curr = polyline[i];
        var dx = curr.X - prev.X;
        var dy = curr.Y - prev.Y;
        sum += Math.Sqrt(dx * dx + dy * dy);
        prev = curr;
      }

      return sum;
    }

    /// <summary>
    /// Calculates the total length of a polyline.
    /// </summary>
    /// <param name="polyline">The points of the polyline. Only points starting from <paramref name="startIdx"/> with the number of points = <paramref name="count"/> are considered to be part of the polyline.</param>
    /// <param name="startIdx">Index of the first point of the polyline in array <paramref name="polyline"/>.</param>
    /// <param name="count">The number of points of the polyline. An exception is thrown if this argument is less than 2.</param>
    /// <returns>The total length of the polyline, from index <paramref name="startIdx"/> with number of points equal to <paramref name="count"/>.</returns>
    /// <exception cref="ArgumentNullException">polyline</exception>
    /// <exception cref="ArgumentException">Polyline must have at least 2 points - polyline</exception>
    public static double TotalLineLength(this PointF[] polyline, int startIdx, int count)
    {
      if (null == polyline)
        throw new ArgumentNullException(nameof(polyline));
      if (count < 2)
        throw new ArgumentException("Polyline must have at least 2 points", nameof(polyline));

      int nextIdx = startIdx + count; // 1 beyond the last point of the polyline

      double sum = 0;

      PointF prev = polyline[startIdx];
      PointF curr;
      for (int i = startIdx + 1; i < nextIdx; ++i)
      {
        curr = polyline[i];
        var dx = curr.X - prev.X;
        var dy = curr.Y - prev.Y;
        sum += Math.Sqrt(dx * dx + dy * dy);
        prev = curr;
      }

      return sum;
    }

    public static double LengthBetween(PointF p0, PointF p1)
    {
      var dx = p1.X - p0.X;
      var dy = p1.Y - p0.Y;
      return Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Interpolates between the points <paramref name="p0"/> and <paramref name="p1"/>.
    /// </summary>
    /// <param name="p0">The first point.</param>
    /// <param name="p1">The second point.</param>
    /// <param name="r">Relative way between <paramref name="p0"/> and <paramref name="p1"/> (0..1).</param>
    /// <returns>Interpolation between <paramref name="p0"/> and <paramref name="p1"/>. The return value is <paramref name="p0"/> if <paramref name="r"/> is 0. The return value is <paramref name="p1"/>  if <paramref name="r"/> is 1.  </returns>
    public static PointF Interpolate(PointF p0, PointF p1, double r)
    {
      double or = 1 - r;
      return new PointF((float)(or * p0.X + r * p1.X), (float)(or * p0.Y + r * p1.Y));
    }

    /// <summary>
    /// Returns a new, shortened polyline. If the shortened line would have zero or negative length, <c>null</c> is returned.
    /// </summary>
    /// <param name="polyline">The points of the original polyline.</param>
    /// <param name="marginAtStart">The margin at start. Either an absolute value, or relative to the total length of the polyline.</param>
    /// <param name="marginAtEnd">The margin at end. Either an absolute value, or relative to the total length of the polyline.</param>
    /// <returns>A new, shortened polyline. If the shortened line would have zero or negative length, <c>null</c> is returned.</returns>
    public static PointF[]? ShortenedBy(this PointF[] polyline, RADouble marginAtStart, RADouble marginAtEnd)
    {
      if (null == polyline)
        throw new ArgumentNullException(nameof(polyline));
      if (polyline.Length < 2)
        throw new ArgumentException("Polyline must have at least 2 points", nameof(polyline));

      double totLength = TotalLineLength(polyline);

      double a1 = marginAtStart.IsAbsolute ? marginAtStart.Value : marginAtStart.Value * totLength;
      double a2 = marginAtEnd.IsAbsolute ? marginAtEnd.Value : marginAtEnd.Value * totLength;

      if (!((a1 + a2) < totLength))
        return null;

      PointF? p0 = null;
      PointF? p1 = null;
      int i0 = 0;
      int i1 = 0;

      if (a1 <= 0)
      {
        p0 = Interpolate(polyline[0], polyline[1], a1 / totLength);
        i0 = 1;
      }
      else
      {
        double sum = 0, prevSum = 0;
        for (int i = 1; i < polyline.Length; ++i)
        {
          sum += LengthBetween(polyline[i], polyline[i - 1]);
          if (!(sum < a1))
          {
            p0 = Interpolate(polyline[i - 1], polyline[i], (a1 - prevSum) / (sum - prevSum));
            i0 = p0 != polyline[i] ? i : i + 1;
            break;
          }
          prevSum = sum;
        }
      }

      if (a2 <= 0)
      {
        p1 = Interpolate(polyline[polyline.Length - 2], polyline[polyline.Length - 1], 1 - a2 / totLength);
        i1 = polyline.Length - 2;
      }
      else
      {
        double sum = 0, prevSum = 0;
        for (int i = polyline.Length - 2; i >= 0; --i)
        {
          sum += LengthBetween(polyline[i], polyline[i + 1]);
          if (!(sum < a2))
          {
            p1 = Interpolate(polyline[i + 1], polyline[i], (a2 - prevSum) / (sum - prevSum));
            i1 = p1 != polyline[i] ? i : i - 1;
            break;
          }
          prevSum = sum;
        }
      }

      if (p0.HasValue && p1.HasValue)
      {
        var plist = new List<PointF>
        {
          p0.Value
        };
        for (int i = i0; i <= i1; ++i)
          plist.Add(polyline[i]);
        plist.Add(p1.Value);
        return plist.ToArray();
      }
      else
      {
        return null;
      }
    }

    /// <summary>
    /// Returns a new, shortened polyline. If the shortened line would have zero or negative length, <c>null</c> is returned.
    /// </summary>
    /// <param name="polyline">The points of the polyline. Only points starting from <paramref name="startIdx"/> with the number of points = <paramref name="count"/> are considered to be part of the polyline.</param>
    /// <param name="startIdx">Index of the first point of the polyline in array <paramref name="polyline"/>.</param>
    /// <param name="count">The number of points of the polyline. An exception is thrown if this argument is less than 2.</param>
    /// <param name="marginAtStart">The margin at start. Either an absolute value, or relative to the total length of the polyline.</param>
    /// <param name="marginAtEnd">The margin at end. Either an absolute value, or relative to the total length of the polyline.</param>
    /// <returns>A new, shortened polyline. If the shortened line would have zero or negative length, <c>null</c> is returned.</returns>
    public static PointF[]? ShortenedBy(this PointF[] polyline, int startIdx, int count, RADouble marginAtStart, RADouble marginAtEnd)
    {
      if (null == polyline)
        throw new ArgumentNullException(nameof(polyline));
      if (count < 2)
        throw new ArgumentException("Polyline must have at least 2 points", nameof(polyline));

      int nextIdx = startIdx + count;

      double totLength = TotalLineLength(polyline, startIdx, count);

      double a1 = marginAtStart.IsAbsolute ? marginAtStart.Value : marginAtStart.Value * totLength;
      double a2 = marginAtEnd.IsAbsolute ? marginAtEnd.Value : marginAtEnd.Value * totLength;

      if (!((a1 + a2) < totLength))
        return null;

      PointF? p0 = null;
      PointF? p1 = null;
      int i0 = 0;
      int i1 = 0;

      if (a1 <= 0)
      {
        p0 = Interpolate(polyline[startIdx], polyline[startIdx + 1], a1 / totLength);
        i0 = startIdx + 1;
      }
      else
      {
        double sum = 0, prevSum = 0;
        for (int i = startIdx + 1; i < nextIdx; ++i)
        {
          sum += LengthBetween(polyline[i], polyline[i - 1]);
          if (!(sum < a1))
          {
            p0 = Interpolate(polyline[i - 1], polyline[i], (a1 - prevSum) / (sum - prevSum));
            i0 = p0 != polyline[i] ? i : i + 1;
            break;
          }
          prevSum = sum;
        }
      }

      if (a2 <= 0)
      {
        p1 = Interpolate(polyline[nextIdx - 2], polyline[nextIdx - 1], 1 - a2 / totLength);
        i1 = nextIdx - 2;
      }
      else
      {
        double sum = 0, prevSum = 0;
        for (int i = nextIdx - 2; i >= startIdx; --i)
        {
          sum += LengthBetween(polyline[i], polyline[i + 1]);
          if (!(sum < a2))
          {
            p1 = Interpolate(polyline[i + 1], polyline[i], (a2 - prevSum) / (sum - prevSum));
            i1 = p1 != polyline[i] ? i : i - 1;
            break;
          }
          prevSum = sum;
        }
      }

      if (p0.HasValue && p1.HasValue)
      {
        var plist = new List<PointF>
        {
          p0.Value
        };
        for (int i = i0; i <= i1; ++i)
          plist.Add(polyline[i]);
        plist.Add(p1.Value);
        return plist.ToArray();
      }
      else
      {
        return null;
      }
    }

    /// <summary>
    /// Gets the fractional index into points of a polyline by searching a point along the polyline that has a specified distance to the start point of the polyline.
    /// </summary>
    /// <param name="points">The points of the polyline curve.</param>
    /// <param name="distanceFromStart">The distance to the start point of the polyline curve, i.e. to the point at index 0.</param>
    /// <returns>The fractional index of the global polyline curve where the point at this fractional index has a distance of <paramref name="distanceFromStart"/> to the start point at index 0.
    /// The return value is in the range [0..points.Length-1]. If no such point is found, then double.NaN is returned.</returns>
    public static double GetFractionalIndexFromDistanceFromStartOfPolylineCurve(PointF[] points, double distanceFromStart)
    {
      return GetFractionalIndexFromDistanceFromStartOfPartialPolylineCurve(points, 0, points.Length - 1, distanceFromStart);
    }

    /// <summary>
    /// Gets the fractional index into points of a partial polyline by searching a point along the partial polyline that has a specified distance to the start point of the partial polyline.
    /// </summary>
    /// <param name="points">The points of the global polyline curve. From this points only the partial polyline (indices <paramref name="startIndex"/> .. <paramref name="endIndex"/>) is considered.</param>
    /// <param name="startIndex">The first index of the partial polyline curve that is considered here.</param>
    /// <param name="endIndex">The last valid index of the partial polyline curve that is considered here.</param>
    /// <param name="distanceFromStart">The distance to the start of the partial polyline curve, i.e. to the point at index <paramref name="startIndex"/>.</param>
    /// <returns>The fractional index of the global polyline curve where the point at this fractional index has a distance of <paramref name="distanceFromStart"/> from the point at index <paramref name="startIndex"/>.
    /// The return value is in the range [<paramref name="startIndex"/> .. <paramref name="endIndex"/>]. If no such point is found, then double.NaN is returned.
    /// </returns>
    public static double GetFractionalIndexFromDistanceFromStartOfPartialPolylineCurve(PointF[] points, int startIndex, int endIndex, double distanceFromStart)
    {
      if (0 == distanceFromStart)
        return startIndex; // speed-up, but also fix a problem when the segment from startIndex to startIndex+1 has a length of zero

      var pivot = points[startIndex];
      var dsqr = distanceFromStart * distanceFromStart;
      var list = new List<Tuple<float, PointF>>();

      for (int segmentEnd = startIndex + 1; segmentEnd <= endIndex; ++segmentEnd)
      {
        // Find a point (either curve point or control point that is farther away than required
        if (pivot.DistanceSquaredTo(points[segmentEnd]) < dsqr)
        {
          continue;
        }

        var tline = GetParameterOnLineSegmentFromDistanceToPoint(points[segmentEnd - 1], points[segmentEnd], pivot, dsqr, false);
        return segmentEnd - 1 + tline;
      }

      return double.NaN;
    }

    /// <summary>
    /// Gets the fractional index into points of a polyline by searching a point along the polyline that has a specified distance to the end point of the polyline.
    /// </summary>
    /// <param name="points">The points of the polyline curve.</param>
    /// <param name="distanceFromEnd">The distance from the end point of the polyline curve, i.e. from the point at index <paramref name="points"/>].Length-1</param>
    /// <returns>The fractional index of the global polyline curve where the point at this fractional index has a distance of <paramref name="distanceFromEnd"/> from the point at index <paramref name="points"/>.Length-1.
    /// The return value is in the range [0..points.Length-1]. If no such points is found, then double.NaN is returned.</returns>
    public static double GetFractionalIndexFromDistanceFromEndOfPolylineCurve(PointF[] points, double distanceFromEnd)
    {
      return GetFractionalIndexFromDistanceFromEndOfPartialPolylineCurve(points, 0, points.Length - 1, distanceFromEnd);
    }

    /// <summary>
    /// Gets the fractional index into points of a partial polyline by searching a point along the partial polyline that has a specified distance to the end point of the partial polyline.
    /// </summary>
    /// <param name="points">The points of the global polyline curve. From this points only the partial polyline (indices <paramref name="firstBoundIndex"/> .. <paramref name="endStartIndex"/>) is considered.</param>
    /// <param name="firstBoundIndex">The first index of the partial polyline curve that is considered here.</param>
    /// <param name="endStartIndex">The last valid index of the partial polyline curve that is considered here.</param>
    /// <param name="distanceFromEnd">The distance from the end of the partial polyline curve, i.e. from the points[<paramref name="endStartIndex"/>].</param>
    /// <returns>The fractional index of the global polyline curve where the point at this fractional index has a distance of <paramref name="distanceFromEnd"/> from the point at index <paramref name="endStartIndex"/>.
    /// The return value is in the range [<paramref name="firstBoundIndex"/> .. <paramref name="endStartIndex"/>]. If no such point is found, then double.NaN is returned.
    /// </returns>
    public static double GetFractionalIndexFromDistanceFromEndOfPartialPolylineCurve(PointF[] points, int firstBoundIndex, int endStartIndex, double distanceFromEnd)
    {
      if (0 == distanceFromEnd)
        return endStartIndex; // speed-up, but also fix a problem when the segment from endStartIndex-1 to endStartIndex has a length of zero

      var pivot = points[endStartIndex];
      var dsqr = distanceFromEnd * distanceFromEnd;

      for (int segmentStart = endStartIndex - 1; segmentStart >= firstBoundIndex; --segmentStart)
      {
        // Find a point (either curve point or control point that is farther away than required
        if (pivot.DistanceSquaredTo(points[segmentStart]) < dsqr)
        {
          continue; // all points too close, thus continue with next Bezier segment
        }

        var tline = GetParameterOnLineSegmentFromDistanceToPoint(points[segmentStart], points[segmentStart + 1], pivot, dsqr, true);
        return segmentStart + tline;
      }

      return double.NaN;
    }

    /// <summary>
    /// Shortens a line segment and returns the line points of the shortened segment.
    /// </summary>
    /// <param name="P1">Start point p1 of the line segment.</param>
    /// <param name="P2">End point p2 of the line segment.</param>
    /// <param name="t0">Value in the range 0..1 to indicate the shortening at the beginning of the segment.</param>
    /// <param name="t1">Value in the range 0..1 to indicate the shortening at the beginning of the segment. This value must be greater than <paramref name="t0"/>.</param>
    /// <returns>The points of the shortened line segment.</returns>
    public static Tuple<PointF, PointF> ShortenLineSegment(PointF P1, PointF P2, float t0, float t1)
    {
      var u0 = 1 - t0;
      var u1 = 1 - t1;

      var PS1X = u0 * P1.X + t0 * P2.X;
      var PS1Y = u0 * P1.Y + t0 * P2.Y;

      var PS2X = u1 * P1.X + t1 * P2.X;
      var PS2Y = u1 * P1.Y + t1 * P2.Y;

      return new Tuple<PointF, PointF>(new PointF(PS1X, PS1Y), new PointF(PS2X, PS2Y));
    }

    /// <summary>
    /// Shortens a polyline at both sides in such a way that the new start has a distance <paramref name="distanceFromStart"/> to the original start of the polyline, and the
    /// new end has a distance <paramref name="distanceFromEnd"/> to the original end of the polyline. If the shortening leads to a polyline of zero length, then <c>null</c> is returned.
    /// </summary>
    /// <param name="points">The points of the polyline.</param>
    /// <param name="distanceFromStart">The distance of the start of the shortened polyline to the first point of the original polyline (at index 0).</param>
    /// <param name="distanceFromEnd">The distance of the end of the shortened polyline to the last point of the original polyline (at index <paramref name="points"/>.Length-1).</param>
    /// <returns>The shortened polyline, or null if the polyline was shortened to zero length.</returns>
    public static PointF[]? ShortenPolylineByDistanceFromStartAndEnd(this PointF[] points, double distanceFromStart, double distanceFromEnd)
    {
      return ShortenPartialPolylineByDistanceFromStartAndEnd(points, 0, points.Length - 1, distanceFromStart, distanceFromEnd);
    }

    /// <summary>
    /// Shortens a partial polyline, given by the first point at index <paramref name="startIndex"/> and the last index <paramref name="endIndex"/>, at both sides in such a way that the new start has a distance <paramref name="distanceFromStart"/> to the original start of the polyline, and the
    /// new end has a distance <paramref name="distanceFromEnd"/> to the original end of the polyline. If the shortening leads to a polyline of zero length, then <c>null</c> is returned.
    /// </summary>
    /// <param name="points">The points of the original global polyline.</param>
    /// <param name="startIndex">Index of first valid point of the partial polyline that is to be shortened.</param>
    /// <param name="endIndex">Index of the last valid point of the partial polyline that is to be shortened.</param>
    /// <param name="distanceFromStart">The distance of the start of the shortened polyline to the first point of the original polyline (at index 0).</param>
    /// <param name="distanceFromEnd">The distance of the end of the shortened polyline to the last point of the original polyline (at index <paramref name="points"/>.Length-1).</param>
    /// <returns>The shortened polyline, or null if the polyline was shortened to zero length.</returns>
    public static PointF[]? ShortenPartialPolylineByDistanceFromStartAndEnd(this PointF[] points, int startIndex, int endIndex, double distanceFromStart, double distanceFromEnd)
    {
      var fractionalIndexStart = GetFractionalIndexFromDistanceFromStartOfPartialPolylineCurve(points, startIndex, endIndex, distanceFromStart);
      if (double.IsNaN(fractionalIndexStart))
        return null; // there is no curve left after shortening

      var fractionalIndexEnd = GetFractionalIndexFromDistanceFromEndOfPartialPolylineCurve(points, startIndex, endIndex, distanceFromEnd);
      if (double.IsNaN(fractionalIndexEnd))
        return null; // there is no curve left after shortening

      if (!(fractionalIndexStart < fractionalIndexEnd))
        return null; // there is no curve left after shortening

      int segmentStart = (int)Math.Floor(fractionalIndexStart);
      int segmentLast = Math.Min((int)Math.Floor(fractionalIndexEnd), endIndex - 1);
      int subPoints = (segmentLast - segmentStart + 2);

      var result = new PointF[subPoints];
      Array.Copy(points, segmentStart, result, 0, subPoints);
      double fractionStart = fractionalIndexStart - segmentStart;
      double fractionEnd = fractionalIndexEnd - segmentLast;

      if (fractionStart > 0 && fractionEnd > 0 && segmentStart == segmentLast) // if there is only one segment to shorten, do it concurrently at the start and the end
      {
        var shortenedSegment = ShortenLineSegment(result[0], result[1], (float)fractionStart, (float)fractionEnd);
        result[0] = shortenedSegment.Item1;
        result[1] = shortenedSegment.Item2;
      }
      else
      {
        if (fractionStart > 0)
        {
          var shortenedSegment = ShortenLineSegment(result[0], result[1], (float)fractionStart, 1);
          result[0] = shortenedSegment.Item1;
          result[1] = shortenedSegment.Item2;
        }
        if (fractionEnd < 1)
        {
          int lastStart = segmentLast - segmentStart; // -segmentStart because subarray begins at segmentStart
          var shortenedSegment = ShortenLineSegment(result[lastStart], result[1 + lastStart], 0, (float)fractionEnd);
          result[0 + lastStart] = shortenedSegment.Item1;
          result[1 + lastStart] = shortenedSegment.Item2;
        }
      }
      return result;
    }

    #endregion Polyline constituted by an array of PointF

    #region String Alignment

    public static Altaxo.Drawing.Alignment ToAltaxo(System.Drawing.StringAlignment alignment)
    {
      Altaxo.Drawing.Alignment result;
      switch (alignment)
      {
        case StringAlignment.Near:
          result = Drawing.Alignment.Near;
          break;

        case StringAlignment.Center:
          result = Drawing.Alignment.Center;
          break;

        case StringAlignment.Far:
          result = Drawing.Alignment.Far;
          break;

        default:
          throw new NotImplementedException();
      }
      return result;
    }

    public static System.Drawing.StringAlignment ToGdi(Altaxo.Drawing.Alignment alignment)
    {
      System.Drawing.StringAlignment result;
      switch (alignment)
      {
        case Drawing.Alignment.Near:
          result = System.Drawing.StringAlignment.Near;
          break;

        case Drawing.Alignment.Center:
          result = System.Drawing.StringAlignment.Center;
          break;

        case Drawing.Alignment.Far:
          result = System.Drawing.StringAlignment.Far;
          break;

        default:
          throw new NotImplementedException();
      }
      return result;
    }

    #endregion String Alignement

    #region CardialSpline to BezierSegments

    // see also source of wine at: http://source.winehq.org/source/dlls/gdiplus/graphicspath.c#L445
    //

    /// <summary>
    /// Calculates Bezier points from cardinal spline endpoints.
    /// </summary>
    /// <param name="end">The end point.</param>
    /// <param name="adj">The adjacent point next to the endpoint.</param>
    /// <param name="tension">The tension.</param>
    /// <returns></returns>
    /// <remarks>Original name in Wine sources: calc_curve_bezier_endp</remarks>
    private static PointF Calc_Curve_Bezier_Endpoint(PointF end, PointF adj, float tension)
    {
      // tangent at endpoints is the line from the endpoint to the adjacent point
      return new PointF(
      (tension * (adj.X - end.X) + end.X),
      (tension * (adj.Y - end.Y) + end.Y));
    }

    /// <summary>
    /// Calculates the control points of the incoming and outgoing Bezier segment around the original point at index i+1.
    /// </summary>
    /// <param name="pts">The original points thought to be connected by a cardinal spline.</param>
    /// <param name="i">The index i. To calculate the control points, the indices i, i+1 and i+2 are used from the point array <paramref name="pts"/>.</param>
    /// <param name="tension">The tension of the cardinal spline.</param>
    /// <param name="p1">The Bezier control point that controls the slope towards the point <paramref name="pts"/>[i+1].</param>
    /// <param name="p2">The Bezier control point that controls the slope outwards from the point <paramref name="pts"/>[i+1].</param>
    /// <remarks>Original name in Wine source: calc_curve_bezier</remarks>
    private static void Calc_Curve_Bezier(PointF[] pts, int i, float tension, out PointF p1, out PointF p2)
    {
      /* calculate tangent */
      var diffX = pts[2 + i].X - pts[i].X;
      var diffY = pts[2 + i].Y - pts[i].Y;

      /* apply tangent to get control points */
      p1 = new PointF(pts[1 + i].X - tension * diffX, pts[1 + i].Y - tension * diffY);
      p2 = new PointF(pts[1 + i].X + tension * diffX, pts[1 + i].Y + tension * diffY);
    }

    /// <summary>
    /// Calculates the control points of the incoming and outgoing Bezier segment around the original point <paramref name="p1"/>.
    /// </summary>
    /// <param name="pts0">The previous point on a cardinal spline curce.</param>
    /// <param name="pts1">The point on a cardinal spline curve for which to calculate the incoming and outgoing Bezier control points.</param>
    /// <param name="pts2">The nex point on the cardinal spline curve.</param>
    /// <param name="tension">The tension of the cardinal spline.</param>
    /// <param name="p1">The Bezier control point that controls the slope towards the point <paramref name="pts1"/>.</param>
    /// <param name="p2">The Bezier control point that controls the slope outwards from the point <paramref name="pts1"/>.</param>
    /// <remarks>This function is not in the original Wine sources. It is introduced here for optimization to avoid allocation of a new array when converting closed cardinal spline curves.</remarks>
    private static void Calc_Curve_Bezier(PointF pts0, PointF pts1, PointF pts2, float tension, out PointF p1, out PointF p2)
    {
      /* calculate tangent */
      var diffX = pts2.X - pts0.X;
      var diffY = pts2.Y - pts0.Y;

      /* apply tangent to get control points */
      /* apply tangent to get control points */
      p1 = new PointF(pts1.X - tension * diffX, pts1.Y - tension * diffY);
      p2 = new PointF(pts1.X + tension * diffX, pts1.Y + tension * diffY);
    }

    /// <summary>
    /// Converts an open cardinal spline, given by the points in <paramref name="points"/>, to Bezier segments.
    /// </summary>
    /// <param name="points">The control points of the open cardinal spline curve.</param>
    /// <param name="count">Number of control points of the closed cardinal spline curve.</param>
    /// <returns>Bezier segments that constitute the closed curve.</returns>
    /// <remarks>Original name in Wine source: GdipAddPathClosedCurve2</remarks>
    public static PointF[] OpenCardinalSplineToBezierSegments(PointF[] points, int count)
    {
      return OpenCardinalSplineToBezierSegments(points, count, 0.5f);
    }

    /// <summary>
    /// Converts an open cardinal spline, given by the points in <paramref name="points"/>, to Bezier segments.
    /// </summary>
    /// <param name="points">The control points of the open cardinal spline curve.</param>
    /// <param name="count">Number of control points of the closed cardinal spline curve.</param>
    /// <param name="tension">The tension of the cardinal spline.</param>
    /// <returns>Bezier segments that constitute the closed curve.</returns>
    /// <remarks>Original name in Wine source: GdipAddPathClosedCurve2</remarks>
    public static PointF[] OpenCardinalSplineToBezierSegments(PointF[] points, int count, float tension)
    {
      const float TENSION_CONST = 1.0f / 3;
      ;

      int len_pt = count * 3 - 2;
      var pt = new PointF[len_pt];
      tension = tension * TENSION_CONST;

      var p1 = Calc_Curve_Bezier_Endpoint(points[0], points[1], tension);
      pt[0] = points[0];
      pt[1] = p1;
      var p2 = p1;

      for (int i = 0; i < count - 2; i++)
      {
        Calc_Curve_Bezier(points, i, tension, out p1, out p2);

        pt[3 * i + 2] = p1;
        pt[3 * i + 3] = points[i + 1];
        pt[3 * i + 4] = p2;
      }

      p1 = Calc_Curve_Bezier_Endpoint(points[count - 1], points[count - 2], tension);

      pt[len_pt - 2] = p1;
      pt[len_pt - 1] = points[count - 1];

      return pt;
    }

    /// <summary>
    /// Converts a closed cardinal spline, given by the points in <paramref name="points"/>, to Bezier segments.
    /// </summary>
    /// <param name="points">The control points of the closed cardinal spline curve.</param>
    /// <param name="count">Number of control points of the closed cardinal spline curve.</param>
    /// <returns>Bezier segments that constitute the closed curve.</returns>
    /// <remarks>Original name in Wine source: GdipAddPathClosedCurve2</remarks>
    public static PointF[] ClosedCardinalSplineToBezierSegments(PointF[] points, int count)
    {
      return ClosedCardinalSplineToBezierSegments(points, count, 0.5f);
    }

    /// <summary>
    /// Converts a closed cardinal spline, given by the points in <paramref name="points"/>, to Bezier segments.
    /// </summary>
    /// <param name="points">The control points of the closed cardinal spline curve.</param>
    /// <param name="count">Number of control points of the closed cardinal spline curve.</param>
    /// <param name="tension">The tension of the cardinal spline.</param>
    /// <returns>Bezier segments that constitute the closed curve.</returns>
    /// <remarks>Original name in Wine source: GdipAddPathClosedCurve2</remarks>
    public static PointF[] ClosedCardinalSplineToBezierSegments(PointF[] points, int count, float tension)
    {
      const float TENSION_CONST = 1.0f / 3;

      var len_pt = (count + 1) * 3 - 2;
      var pt = new PointF[len_pt];

      tension = tension * TENSION_CONST;

      PointF p1, p2;
      for (int i = 0; i < count - 2; i++)
      {
        Calc_Curve_Bezier(points, i, tension, out p1, out p2);

        pt[3 * i + 2] = p1;
        pt[3 * i + 3] = points[i + 1];
        pt[3 * i + 4] = p2;
      }

      // calculation around last point of the original curve
      Calc_Curve_Bezier(points[count - 2], points[count - 1], points[0], tension, out p1, out p2);
      pt[len_pt - 5] = p1;
      pt[len_pt - 4] = points[count - 1];
      pt[len_pt - 3] = p2;

      // calculation around first point of the original curve
      Calc_Curve_Bezier(points[count - 1], points[0], points[1], tension, out p1, out p2);
      pt[len_pt - 2] = p1;
      pt[len_pt - 1] = points[0]; // close path
      pt[0] = points[0]; // first point
      pt[1] = p2; // outgoing control point

      return pt;
    }

    /// <summary>
    /// Shortens a Bezier segment and returns the Bezier points of the shortened segment.
    /// </summary>
    /// <param name="P1">Control point p1 of the bezier segment.</param>
    /// <param name="P2">Control point p2 of the bezier segment.</param>
    /// <param name="P3">Control point p3 of the bezier segment.</param>
    /// <param name="P4">Control point p4 of the bezier segment.</param>
    /// <param name="t0">Value in the range 0..1 to indicate the shortening at the beginning of the segment.</param>
    /// <param name="t1">Value in the range 0..1 to indicate the shortening at the beginning of the segment. This value must be greater than <paramref name="t0"/>.</param>
    /// <returns>The control points of the shortened Bezier segment.</returns>
    /// <remarks>
    /// <para>See this source <see href="http://stackoverflow.com/questions/11703283/cubic-bezier-curve-segment"/> for explanation.</para>
    /// <para>The assumtion here is that the Bezier curve is parametrized using</para>
    /// <para>B(t) = (1−t)³ P1 + 3(1−t)² t P2 + 3(1−t) t² P3 + t³ P4</para>
    /// </remarks>
    public static Tuple<PointF, PointF, PointF, PointF> ShortenBezierSegment(PointF P1, PointF P2, PointF P3, PointF P4, float t0, float t1)
    {
      // the assumption was that the curve was parametrized using
      // B(t) = (1−t)³ P1 + 3(1−t)² t P2 + 3(1−t) t² P3 + t³ P4

      var u0 = 1 - t0;
      var u1 = 1 - t1;

      var PS1X = u0 * u0 * u0 * P1.X + (t0 * u0 * u0 + u0 * t0 * u0 + u0 * u0 * t0) * P2.X + (t0 * t0 * u0 + u0 * t0 * t0 + t0 * u0 * t0) * P3.X + t0 * t0 * t0 * P4.X;
      var PS1Y = u0 * u0 * u0 * P1.Y + (t0 * u0 * u0 + u0 * t0 * u0 + u0 * u0 * t0) * P2.Y + (t0 * t0 * u0 + u0 * t0 * t0 + t0 * u0 * t0) * P3.Y + t0 * t0 * t0 * P4.Y;

      var PS2X = u0 * u0 * u1 * P1.X + (t0 * u0 * u1 + u0 * t0 * u1 + u0 * u0 * t1) * P2.X + (t0 * t0 * u1 + u0 * t0 * t1 + t0 * u0 * t1) * P3.X + t0 * t0 * t1 * P4.X;
      var PS2Y = u0 * u0 * u1 * P1.Y + (t0 * u0 * u1 + u0 * t0 * u1 + u0 * u0 * t1) * P2.Y + (t0 * t0 * u1 + u0 * t0 * t1 + t0 * u0 * t1) * P3.Y + t0 * t0 * t1 * P4.Y;

      var PS3X = u0 * u1 * u1 * P1.X + (t0 * u1 * u1 + u0 * t1 * u1 + u0 * u1 * t1) * P2.X + (t0 * t1 * u1 + u0 * t1 * t1 + t0 * u1 * t1) * P3.X + t0 * t1 * t1 * P4.X;
      var PS3Y = u0 * u1 * u1 * P1.Y + (t0 * u1 * u1 + u0 * t1 * u1 + u0 * u1 * t1) * P2.Y + (t0 * t1 * u1 + u0 * t1 * t1 + t0 * u1 * t1) * P3.Y + t0 * t1 * t1 * P4.Y;

      var PS4X = u1 * u1 * u1 * P1.X + (t1 * u1 * u1 + u1 * t1 * u1 + u1 * u1 * t1) * P2.X + (t1 * t1 * u1 + u1 * t1 * t1 + t1 * u1 * t1) * P3.X + t1 * t1 * t1 * P4.X;
      var PS4Y = u1 * u1 * u1 * P1.Y + (t1 * u1 * u1 + u1 * t1 * u1 + u1 * u1 * t1) * P2.Y + (t1 * t1 * u1 + u1 * t1 * t1 + t1 * u1 * t1) * P3.Y + t1 * t1 * t1 * P4.Y;

      return new Tuple<PointF, PointF, PointF, PointF>(new PointF(PS1X, PS1Y), new PointF(PS2X, PS2Y), new PointF(PS3X, PS3Y), new PointF(PS4X, PS4Y));
    }

    /// <summary>
    /// Flattens a bezier segment, using only an absolute tolerance. The flattened points are stored together with their curve parameter t.
    /// </summary>
    /// <param name="absoluteTolerance">The absolute tolerance used for the flattening.</param>
    /// <param name="maxRecursionLevel">The maximum recursion level.</param>
    /// <param name="p0_0">The p0 0.</param>
    /// <param name="p1_0">The p1 0.</param>
    /// <param name="p2_0">The p2 0.</param>
    /// <param name="p3_0">The p3 0.</param>
    /// <param name="t0">The curve parameter that corresponds to the point <paramref name="p0_0"/>.</param>
    /// <param name="t1">The curve parameter that corresponds to the point <paramref name="p3_0"/>.</param>
    /// <param name="flattenedList">The list with flattened points.</param>
    /// <param name="insertIdx">Index in the <paramref name="flattenedList"/> where to insert the next calculated point.</param>
    /// <returns></returns>
    public static bool FlattenBezierSegment(double absoluteTolerance, int maxRecursionLevel, PointF p0_0, PointF p1_0, PointF p2_0, PointF p3_0, float t0, float t1, List<Tuple<float, PointF>> flattenedList, int insertIdx)
    {
      // First, test for absolute deviation of the curve

      double ux = 3 * p1_0.X - 2 * p0_0.X - p3_0.X;
      ux *= ux;

      double uy = 3 * p1_0.Y - 2 * p0_0.Y - p3_0.Y;
      uy *= uy;

      double vx = 3 * p2_0.X - 2 * p3_0.X - p0_0.X;
      vx *= vx;

      double vy = 3 * p2_0.Y - 2 * p3_0.Y - p0_0.Y;
      vy *= vy;

      if (ux < vx)
        ux = vx;
      if (uy < vy)
        uy = vy;

      if ((ux + uy) > absoluteTolerance) // tolerance here is 16*tol^2 (tol is the maximum absolute allowed deviation of the curve from the approximation)
      {
        var p0_1 = new PointF(0.5f * (p0_0.X + p1_0.X), 0.5f * (p0_0.Y + p1_0.Y));
        var p1_1 = new PointF(0.5f * (p1_0.X + p2_0.X), 0.5f * (p1_0.Y + p2_0.Y));
        var p2_1 = new PointF(0.5f * (p2_0.X + p3_0.X), 0.5f * (p2_0.Y + p3_0.Y));

        var p0_2 = new PointF(0.5f * (p0_1.X + p1_1.X), 0.5f * (p0_1.Y + p1_1.Y));
        var p1_2 = new PointF(0.5f * (p1_1.X + p2_1.X), 0.5f * (p1_1.Y + p2_1.Y));

        var p0_3 = new PointF(0.5f * (p0_2.X + p1_2.X), 0.5f * (p0_2.Y + p1_2.Y));

        float tMiddle = 0.5f * (t0 + t1);
        flattenedList.Insert(insertIdx, new Tuple<float, PointF>(tMiddle, p0_3));

        if (maxRecursionLevel > 0)
        {
          // now flatten the right side first
          FlattenBezierSegment(absoluteTolerance, maxRecursionLevel - 1, p0_3, p1_2, p2_1, p3_0, tMiddle, t1, flattenedList, insertIdx + 1);

          // and the left side
          FlattenBezierSegment(absoluteTolerance, maxRecursionLevel - 1, p0_0, p0_1, p0_2, p0_3, t0, tMiddle, flattenedList, insertIdx);
        }

        return true;
      }

      return false;
    }

    private static float Pow2(float x)
    {
      return x * x;
    }

    /// <summary>
    /// If a line segment is given by this parametric equation: p(t)=(1-t)*p0 + t*p1, this function calculates the parameter t at which
    /// the point p(t) has a squared distance <paramref name="dsqr"/> from the pivot point <paramref name="pivot"/>. The argument
    /// <paramref name="chooseFarSolution"/> determines which of two possible solutions is choosen.
    /// </summary>
    /// <param name="p0">Start point of the line segment.</param>
    /// <param name="p1">End point of the line segment.</param>
    /// <param name="pivot">Pivot point.</param>
    /// <param name="dsqr">Squared distance from the pivot point to a point on the line segment.</param>
    /// <param name="chooseFarSolution">If true, the solution with the higher t is returned, presuming that t is in the range [0,1].
    /// If false, the solution with the lower t is returned, presuming that t is in the range[0,1]. If neither of the both solutions is in
    /// the range [0,1], <see cref="double.NaN"/> is returned.
    /// </param>
    /// <returns></returns>
    public static double GetParameterOnLineSegmentFromDistanceToPoint(PointF p0, PointF p1, PointF pivot, double dsqr, bool chooseFarSolution)
    {
      var dx = p1.X - p0.X;
      var dy = p1.Y - p0.Y;
      var p0x = p0.X - pivot.X;
      var p0y = p0.Y - pivot.Y;
      var dx2dy2 = dx * dx + dy * dy;

      var sqrt = Math.Sqrt(dsqr * dx2dy2 - Pow2(dy * p0x - dx * p0y));
      var pre = -(dx * p0x + dy * p0y);

      var sol1 = (pre - sqrt) / dx2dy2;
      var sol2 = (pre + sqrt) / dx2dy2;

      if (chooseFarSolution)
      {
        if (0 <= sol2 && sol2 <= 1)
          return sol2;
        else if (0 <= sol1 && sol1 <= 1)
          return sol1;
        else
          return double.NaN;
      }
      else
      {
        if (0 <= sol1 && sol1 <= 1)
          return sol1;
        else if (0 <= sol2 && sol2 <= 1)
          return sol2;
        else
          return double.NaN;
      }
    }

    public static double GetFractionalIndexFromDistanceFromStartOfBezierCurve(PointF[] points, double distanceFromStart)
    {
      var pivot = points[0];
      var dsqr = distanceFromStart * distanceFromStart;
      var list = new List<Tuple<float, PointF>>();

      int segmentCount = (points.Length - 1) / 3;
      for (int segmentStart = 0, segmentIndex = 0; (segmentStart + 3) < points.Length; segmentStart += 3, ++segmentIndex)
      {
        // Find a point (either curve point or control point that is farther away than required
        if (pivot.DistanceSquaredTo(points[segmentStart + 1]) < dsqr &&
            pivot.DistanceSquaredTo(points[segmentStart + 2]) < dsqr &&
            pivot.DistanceSquaredTo(points[segmentStart + 3]) < dsqr)
        {
          continue;
        }

        list.Clear();
        list.Add(new Tuple<float, PointF>(0, points[segmentStart + 0]));
        list.Add(new Tuple<float, PointF>(1, points[segmentStart + 3]));
        FlattenBezierSegment(1, 12, points[segmentStart + 0], points[segmentStart + 1], points[segmentStart + 2], points[segmentStart + 3], 0, 1, list, 1);

        for (int i = 1; i < list.Count; ++i)
        {
          if (pivot.DistanceSquaredTo(list[i].Item2) >= dsqr)
          {
            var tline = GetParameterOnLineSegmentFromDistanceToPoint(list[i - 1].Item2, list[i].Item2, pivot, dsqr, false);
            return segmentIndex + (1 - tline) * list[i - 1].Item1 + (tline) * list[i].Item1;
          }
        }
      }

      return double.NaN;
    }

    public static double GetFractionalIndexFromDistanceFromEndOfBezierCurve(PointF[] points, double distanceFromEnd)
    {
      var pivot = points[points.Length - 1];
      var dsqr = distanceFromEnd * distanceFromEnd;
      var list = new List<Tuple<float, PointF>>();

      int segmentCount = (points.Length - 1) / 3;
      for (int segmentStart = points.Length - 4, segmentIndex = segmentCount - 1; segmentStart >= 0; segmentStart -= 3, --segmentIndex)
      {
        // Find a point (either curve point or control point that is farther away than required
        if (pivot.DistanceSquaredTo(points[segmentStart + 0]) < dsqr &&
            pivot.DistanceSquaredTo(points[segmentStart + 1]) < dsqr &&
            pivot.DistanceSquaredTo(points[segmentStart + 2]) < dsqr)
        {
          continue; // all points too close, thus continue with next Bezier segment
        }

        list.Clear();
        list.Add(new Tuple<float, PointF>(0, points[segmentStart + 0]));
        list.Add(new Tuple<float, PointF>(1, points[segmentStart + 3]));
        FlattenBezierSegment(1, 12, points[segmentStart + 0], points[segmentStart + 1], points[segmentStart + 2], points[segmentStart + 3], 0, 1, list, 1);

        for (int i = list.Count - 2; i >= 0; --i)
        {
          if (pivot.DistanceSquaredTo(list[i].Item2) >= dsqr)
          {
            var tline = GetParameterOnLineSegmentFromDistanceToPoint(list[i].Item2, list[i + 1].Item2, pivot, dsqr, true);
            return segmentIndex + (1 - tline) * list[i].Item1 + (tline) * list[i + 1].Item1;
          }
        }
      }

      return double.NaN;
    }

    public static PointF[]? ShortenBezierCurve(PointF[] points, double distanceFromStart, double distanceFromEnd)
    {
      int totalSegments = (points.Length - 1) / 3;
      double fractionalIndexStart = 0;
      double fractionalIndexEnd = totalSegments;

      fractionalIndexStart = GetFractionalIndexFromDistanceFromStartOfBezierCurve(points, distanceFromStart);
      if (double.IsNaN(fractionalIndexStart))
        return null; // there is no bezier curve left after shortening

      fractionalIndexEnd = GetFractionalIndexFromDistanceFromEndOfBezierCurve(points, distanceFromEnd);
      if (double.IsNaN(fractionalIndexEnd))
        return null; // there is no bezier curve left after shortening

      if (!(fractionalIndexStart < fractionalIndexEnd))
        return null; // there is no bezier curve left after shortening

      int segmentStart = (int)Math.Floor(fractionalIndexStart);
      int segmentLast = Math.Min((int)Math.Floor(fractionalIndexEnd), totalSegments - 1);
      int subPoints = 1 + 3 * (segmentLast - segmentStart + 1);

      var result = new PointF[subPoints];
      Array.Copy(points, segmentStart * 3, result, 0, subPoints);
      double fractionStart = fractionalIndexStart - segmentStart;
      double fractionEnd = fractionalIndexEnd - segmentLast;

      if (fractionStart > 0 && fractionEnd > 0 && segmentStart == segmentLast) // if there is only one segment to shorten, do it concurrently at the start and the end
      {
        var shortenedSegment = ShortenBezierSegment(result[0], result[1], result[2], result[3], (float)fractionStart, (float)fractionEnd);
        result[0] = shortenedSegment.Item1;
        result[1] = shortenedSegment.Item2;
        result[2] = shortenedSegment.Item3;
        result[3] = shortenedSegment.Item4;
      }
      else
      {
        if (fractionStart > 0)
        {
          var shortenedSegment = ShortenBezierSegment(result[0], result[1], result[2], result[3], (float)fractionStart, 1);
          result[0] = shortenedSegment.Item1;
          result[1] = shortenedSegment.Item2;
          result[2] = shortenedSegment.Item3;
          result[3] = shortenedSegment.Item4;
        }
        if (fractionEnd < 1)
        {
          int lastStart = 3 * (segmentLast - segmentStart); // -segmentStart because subarray begins at segmentStart
          var shortenedSegment = ShortenBezierSegment(result[0 + lastStart], result[1 + lastStart], result[2 + lastStart], result[3 + lastStart], 0, (float)fractionEnd);
          result[0 + lastStart] = shortenedSegment.Item1;
          result[1 + lastStart] = shortenedSegment.Item2;
          result[2 + lastStart] = shortenedSegment.Item3;
          result[3 + lastStart] = shortenedSegment.Item4;
        }
      }

      return result;
    }

    #endregion CardialSpline to BezierSegments

    #region Distance Line to Point

    /// <summary>
    /// Calculates the squared distance between a finite line and a point.
    /// </summary>
    /// <param name="point">The location of the point.</param>
    /// <param name="lineOrg">The location of the line origin.</param>
    /// <param name="lineEnd">The location of the line end.</param>
    /// <returns>The squared distance between the line (threated as having a finite length) and the point.</returns>
    public static double SquareDistanceLineToPoint(PointF point, PointF lineOrg, PointF lineEnd)
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
    public static bool IsPointIntoDistance(PointF point, double distance, PointF lineOrg, PointF lineEnd)
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

    /// <summary>
    /// Determines whether or not a given point is into a certain distance of a polyline.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="distance">The distance.</param>
    /// <param name="polyline">The polyline.</param>
    /// <returns>
    ///   <c>true</c> if the distance  between point and polyline is less than or equal to the specified distance; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsPointIntoDistance(PointF point, double distance, IEnumerable<PointF> polyline)
    {
      using (var iterator = polyline.GetEnumerator())
      {
        if (!iterator.MoveNext())
          return false; // no points in polyline
        var prevPoint = iterator.Current;

        while (iterator.MoveNext())
        {
          if (IsPointIntoDistance(point, distance, iterator.Current, prevPoint))
            return true;
          prevPoint = iterator.Current;
        }
      }
      return false;
    }

    #endregion Distance Line to Point
  }

  /// <summary>
  /// Structure to hold an initially horizontal-vertical oriented cross. By transforming it, the initial meaning
  /// of its defining points can change.
  /// </summary>
  public struct CrossF
  {
    /// <summary>Initially the center of the cross.</summary>
    public PointF Center;

    /// <summary>Initially the top of the cross.</summary>
    public PointF Top;

    /// <summary>Initially the bottom of the cross.</summary>
    public PointF Bottom;

    /// <summary>Initially the left of the cross.</summary>
    public PointF Left;

    /// <summary>Initially the right of the cross.</summary>
    public PointF Right;

    /// <summary>
    /// Creates an initially horizontal/vertical oriented cross.
    /// </summary>
    /// <param name="center">Center of the cross.</param>
    /// <param name="horzLength">Length of a horizontal arm of the cross (distance between Left and Center, and also distance between Center and Right).</param>
    /// <param name="vertLength">Length of a vertical arm of the cross (distance between Top and Center, and also distance between Center and Bottom)</param>
    public CrossF(PointF center, float horzLength, float vertLength)
    {
      Center = center;
      Top = new PointF(center.X, center.Y - vertLength);
      Bottom = new PointF(center.X, center.Y + vertLength);
      Left = new PointF(center.X - horzLength, center.Y);
      Right = new PointF(center.X + horzLength, center.Y);
    }

    private double sqr(double x)
    {
      return x * x;
    }

    /// <summary>
    /// Returns the minimum extension of the arms..
    /// </summary>
    /// <returns>Minimum of the extension in horizontal and vertical directions.</returns>
    public float GetMinExtension()
    {
      var horz = sqr(Left.X - Right.X) + sqr(Left.Y - Right.Y);
      var vert = sqr(Top.X - Bottom.X) + sqr(Top.Y - Bottom.Y);
      return (float)Math.Sqrt(Math.Min(horz, vert));
    }

    public void Translate(float dx, float dy)
    {
      Center.X += dx;
      Center.Y += dy;
    }
  }
}
