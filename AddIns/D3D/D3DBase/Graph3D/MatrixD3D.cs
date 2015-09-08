#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph3D
{
	public class MatrixD3D
	{
		private double M11, M12, M13, M14;
		private double M21, M22, M23, M24;
		private double M31, M32, M33, M34;
		private double OffsetX, OffsetY, OffsetZ, M44;

		private static MatrixD3D _identityMatrix;

		static MatrixD3D()
		{
			_identityMatrix = new MatrixD3D(
				1, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, 1, 0,
				0, 0, 0, 0);
		}

		public static MatrixD3D Identity
		{
			get
			{
				return _identityMatrix;
			}
		}

		public MatrixD3D(
		double m11, double m12, double m13, double m14,
		double m21, double m22, double m23, double m24,
		double m31, double m32, double m33, double m34,
		double offsetX, double offsetY, double offsetZ, double m44)
		{
			M11 = m11; M12 = m12; M13 = m13; M14 = m14;
			M21 = m21; M22 = m22; M23 = m23; M24 = m24;
			M31 = m31; M32 = m32; M33 = m33; M34 = m34;
			OffsetX = offsetX; OffsetY = offsetY; OffsetZ = offsetZ; M44 = m44;
		}

		public VectorD3D Transform(VectorD3D v)
		{
			double x = v.X;
			double y = v.Y;
			double z = v.Z;
			return new VectorD3D(
			x * M11 + y * M21 + z * M31,
			x * M12 + y * M22 + z * M32,
			x * M13 + y * M23 + z * M33
			);
		}

		public PointD3D Transform(PointD3D p)
		{
			double x = p.X;
			double y = p.Y;
			double z = p.Z;
			return new PointD3D(
			x * M11 + y * M21 + z * M31 + OffsetX,
			x * M12 + y * M22 + z * M32 + OffsetY,
			x * M13 + y * M23 + z * M33 + OffsetZ
			);
		}
	}
}