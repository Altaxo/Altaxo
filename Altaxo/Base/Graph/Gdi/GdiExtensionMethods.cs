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
		public static PointF[] ShortenedBy(this PointF[] polyline, RADouble marginAtStart, RADouble marginAtEnd)
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
				var plist = new List<PointF>();
				plist.Add(p0.Value);
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
		public static PointF[] ShortenedBy(this PointF[] polyline, int startIdx, int count, RADouble marginAtStart, RADouble marginAtEnd)
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
				var plist = new List<PointF>();
				plist.Add(p0.Value);
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

		#endregion Polyline constituted by an array of PointF

		#region String Alignement

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
		static PointF Calc_Curve_Bezier_Endpoint(PointF end, PointF adj, float tension)
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
		static void Calc_Curve_Bezier(PointF[] pts, int i, float tension, out PointF p1, out PointF p2)
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
		static void Calc_Curve_Bezier(PointF pts0, PointF pts1, PointF pts2, float tension, out PointF p1, out PointF p2)
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
		/// <param name="tension">The tension of the cardinal spline.</param>
		/// <returns>Bezier segments that constitute the closed curve.</returns>
		/// <remarks>Original name in Wine source: GdipAddPathClosedCurve2</remarks>
		public static PointF[] OpenCardinalSplineToBezierSegments(PointF[] points, int count)
		{
			return OpenCardinalSplineToBezierSegments(points, count, 0.5555555555555555555f);
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
			const float TENSION_CONST = 0.3f;

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
		/// <param name="tension">The tension of the cardinal spline.</param>
		/// <returns>Bezier segments that constitute the closed curve.</returns>
		/// <remarks>Original name in Wine source: GdipAddPathClosedCurve2</remarks>
		public static PointF[] ClosedCardinalSplineToBezierSegments(PointF[] points, int count, float tension)
		{
			const float TENSION_CONST = 0.3f;

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

		private static float Pow2(float x ) { return x * x; }

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
			for (int segmentStart = 0, segmentIndex = 0; segmentStart < points.Length; segmentStart += 3, ++segmentIndex)
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
						var tline = GetParameterOnLineSegmentFromDistanceToPoint(list[i].Item2, list[i+1].Item2, pivot, dsqr, true);
						return segmentIndex + (1 - tline) * list[i].Item1 + (tline) * list[i + 1].Item1;
					}
				}
			}

			return double.NaN;
		}

		public static PointF[] ShortenBezierCurve(PointF[] points, double distanceFromStart, double distanceFromEnd)
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
				if (fractionEnd > 0)
				{
					int lastStart = 3 * segmentLast;
					var shortenedSegment = ShortenBezierSegment(result[0 + lastStart], result[1 + lastStart], result[2 + lastStart], result[3 + lastStart], 1, (float)fractionEnd);
					result[0 + lastStart] = shortenedSegment.Item1;
					result[1 + lastStart] = shortenedSegment.Item2;
					result[2 + lastStart] = shortenedSegment.Item3;
					result[3 + lastStart] = shortenedSegment.Item4;
				}
			}

			return result;
		}

		#endregion
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