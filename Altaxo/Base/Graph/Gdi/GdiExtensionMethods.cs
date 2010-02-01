using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Altaxo.Graph.Gdi
{
	public static class GdiExtensionMethods
	{
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
			return new SizeF(s.Width /2, s.Height /2);
		}

		/// <summary>
		/// Calculates the center of the provided rectangle.
		/// </summary>
		/// <param name="r">The rectangle.</param>
		/// <returns>The position of the center of the rectangle.</returns>
		public static PointF Center(this RectangleF r)
		{
			return new PointF(r.X + r.Width/2, r.Y +  r.Height/2);
		}
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

		private double sqr(double x) { return x * x; }
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
	}
}
