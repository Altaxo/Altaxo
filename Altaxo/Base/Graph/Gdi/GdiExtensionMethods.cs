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
}
