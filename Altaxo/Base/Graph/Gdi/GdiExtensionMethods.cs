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