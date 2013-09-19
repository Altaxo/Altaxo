using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	public static class RectangleExtensions
	{
		/// <summary>
		/// Calculates the dimensions of the greatest (by area) rectangle included in an outer rectangle, where the inner rectangle is rotated by some degrees with respect to the outer rectangle.
		/// </summary>
		/// <param name="outerRectangleSize">Size of the outer rectangle.</param>
		/// <param name="rotationAngleDegree">The rotation angle of the inner rectangle with respect to the outer rectangle (in degrees).</param>
		/// <returns>The size of the greatest (by area) inner rectangle that fits into the outer rectangle.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// X-Size of outer rectangle must be > 0
		/// or
		/// Y-Size of outer rectangle must be > 0
		/// or
		/// rotationAngleDegree was invalid
		/// </exception>
		public static PointD2D GetIncludedRotatedRectangleSize(this PointD2D outerRectangleSize, double rotationAngleDegree)
		{
			if (!(outerRectangleSize.X > 0))
				throw new ArgumentOutOfRangeException("X-Size of outer rectangle must be > 0");
			if (!(outerRectangleSize.Y > 0))
				throw new ArgumentOutOfRangeException("Y-Size of outer rectangle must be > 0");

			// fix rotationAngleDegree in a range of -180 to <180
			rotationAngleDegree = Math.Abs(rotationAngleDegree); // only positive angles
			var div = Math.Floor(rotationAngleDegree / 360);
			rotationAngleDegree -= div * 360; // 0..360
			if (rotationAngleDegree > 180)
				rotationAngleDegree = 360 - rotationAngleDegree; // 0..180
			if (rotationAngleDegree > 90)
				rotationAngleDegree = 180 - rotationAngleDegree; // 0..90

			if (!(0 <= rotationAngleDegree && rotationAngleDegree <= 90))
				throw new ArgumentOutOfRangeException("rotationAngleDegree");

			var cosPhi = Math.Cos(rotationAngleDegree * Math.PI / 180);
			var sinPhi = Math.Sin(rotationAngleDegree * Math.PI / 180);

			if (outerRectangleSize.X == outerRectangleSize.Y) // Special case outer rectangle is a square
			{
				var denominator = cosPhi + sinPhi;
				return new PointD2D(
					outerRectangleSize.X / denominator,
					outerRectangleSize.Y / denominator
					);
			}
			else if (outerRectangleSize.Y > outerRectangleSize.X) // Special case more height than width
			{
				var htw = outerRectangleSize.Y / outerRectangleSize.X;
				var phiLimit1 = 180 * Math.Atan(htw - Math.Sqrt(htw * htw - 1)) / Math.PI;
				var phiLimit2 = 180 * Math.Atan(htw + Math.Sqrt(htw * htw - 1)) / Math.PI;
				if (rotationAngleDegree < phiLimit1 || rotationAngleDegree > phiLimit2)
				{
					var denominator = cosPhi * cosPhi - sinPhi * sinPhi;
					return new PointD2D(
							(outerRectangleSize.Y * sinPhi - outerRectangleSize.X * cosPhi) / denominator,
							(outerRectangleSize.Y * cosPhi - outerRectangleSize.X * sinPhi) / denominator
							);
				}
				else
				{
					return new PointD2D(0.5 * outerRectangleSize.X / cosPhi, 0.5 * outerRectangleSize.X / sinPhi);
				}
			}
			else if (outerRectangleSize.X > outerRectangleSize.Y)// case more width than height
			{
				var wth = outerRectangleSize.X / outerRectangleSize.X;
				var phiLimit1 = 180 * Math.Atan(wth - Math.Sqrt(wth * wth - 1)) / Math.PI;
				var phiLimit2 = 180 * Math.Atan(wth + Math.Sqrt(wth * wth - 1)) / Math.PI;
				if (rotationAngleDegree < phiLimit1 || rotationAngleDegree > phiLimit2)
				{
					var denominator = cosPhi * cosPhi - sinPhi * sinPhi;
					return new PointD2D(
							(outerRectangleSize.Y * sinPhi - outerRectangleSize.X * cosPhi) / denominator,
							(outerRectangleSize.Y * cosPhi - outerRectangleSize.X * sinPhi) / denominator
							);
				}
				else
				{
					return new PointD2D(0.5 * outerRectangleSize.Y / sinPhi, 0.5 * outerRectangleSize.Y / cosPhi);
				}
			}
			else
			{
				throw new InvalidProgramException("Check this case, it should be handled anywhere above");
			}
		}

		public static RectangleD GetIncludedRotatedRectanglePositionSize(this PointD2D outerRectangleSize, double rotationAngleDegree)
		{
			var childSize = GetIncludedRotatedRectangleSize(outerRectangleSize, rotationAngleDegree);

			var center = childSize / 2;
			rotationAngleDegree -= 360 * Math.Floor(rotationAngleDegree / 360);
			var cosPhi = Math.Cos(Math.PI * rotationAngleDegree / 180);
			var sinPhi = Math.Cos(Math.PI * rotationAngleDegree / 180);

			var childLeftUpper = new PointD2D(-childSize.X / 2, -childSize.Y / 2);

			// rotate childLeftUpper by Phi
			var rotChildLeftUpper = new PointD2D(cosPhi * childLeftUpper.X + sinPhi * childLeftUpper.Y, -sinPhi * childLeftUpper.X + cosPhi * childLeftUpper.Y);

			var childPos = rotChildLeftUpper - center;

			return new RectangleD(childPos, childSize);
		}

		public static RectangleD GetIncludedRotatedRectanglePositionSize(this RectangleD outerRectangle, double rotationAngleDegree)
		{
			var rect = GetIncludedRotatedRectanglePositionSize(outerRectangle.Size, rotationAngleDegree);
			return new RectangleD(rect.LeftTop + outerRectangle.LeftTop, rect.Size);
		}
	}
}