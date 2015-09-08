using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph3D
{
	public struct VectorD3D
	{
		public double X;
		public double Y;
		public double Z;

		public VectorD3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public double Length
		{
			get
			{
				return Math.Sqrt(X * X + Y * Y + Z * Z);
			}
		}

		public void Normalize()
		{
			var s = 1 / Length;
			X *= s;
			Y *= s;
			Z *= s;
		}

		#region operators

		public static VectorD3D operator +(VectorD3D a, VectorD3D b)
		{
			return new VectorD3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public static VectorD3D operator -(VectorD3D a, VectorD3D b)
		{
			return new VectorD3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static VectorD3D operator *(VectorD3D a, double b)
		{
			return new VectorD3D(a.X * b, a.Y * b, a.Z * b);
		}

		public static VectorD3D operator *(double b, VectorD3D a)
		{
			return new VectorD3D(a.X * b, a.Y * b, a.Z * b);
		}

		public static VectorD3D operator -(VectorD3D b)
		{
			return new VectorD3D(-b.X, -b.Y, -b.Z);
		}

		#endregion operators

		#region static functions

		public static double DotProduct(VectorD3D a, VectorD3D b)
		{
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}

		public static VectorD3D CreateNormalized(VectorD3D pt)
		{
			var ilen = 1 / pt.Length;
			return new VectorD3D(pt.X * ilen, pt.Y * ilen, pt.Z * ilen);
		}

		public static VectorD3D CreateSum(VectorD3D pt1, VectorD3D pt2)
		{
			return new VectorD3D(pt1.X + pt2.X, pt1.Y + pt2.Y, pt1.Z + pt2.Z);
		}

		public static VectorD3D CreateScaled(VectorD3D pt, double scale)
		{
			return new VectorD3D(pt.X * scale, pt.Y * scale, pt.Z * scale);
		}

		public static VectorD3D CrossProduct(VectorD3D a, VectorD3D b)
		{
			return new VectorD3D(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
		}

		public static double AngleBetweenInRadians(VectorD3D vector1, VectorD3D vector2)
		{
			vector1.Normalize();
			vector2.Normalize();
			double radians;
			if (VectorD3D.DotProduct(vector1, vector2) < 0.0)
			{
				radians = Math.PI - 2.0 * Math.Asin((vector1 + vector2).Length / 2.0);
			}
			else
			{
				radians = 2.0 * Math.Asin((vector1 - vector2).Length / 2.0);
			}

			return radians;
		}

		#endregion static functions
	}
}