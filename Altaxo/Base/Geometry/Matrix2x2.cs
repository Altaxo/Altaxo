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

namespace Altaxo.Geometry
{
	/// <summary>
	/// Transformation matrix for affine transformations without translation in 3D space.
	/// </summary>
	public struct Matrix2x2
	{
		/// <summary>Gets the matrix element M[1,1].</summary>
		public double M11 { get; private set; }

		/// <summary>Gets the matrix element M[1,2].</summary>
		public double M12 { get; private set; }

		/// <summary>Gets the matrix element M[2,1].</summary>
		public double M21 { get; private set; }

		/// <summary>Gets the matrix element M[2,2].</summary>
		public double M22 { get; private set; }

		public double Determinant { get; private set; }

		private static Matrix2x2 _identityMatrix;

		#region Constructors

		static Matrix2x2()
		{
			_identityMatrix = new Matrix2x2(
					1, 0,
					0, 1);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix2x2"/> struct.
		/// </summary>
		/// <param name="m11">The element M11.</param>
		/// <param name="m12">The element M12.</param>
		/// <param name="m21">The element M21.</param>
		/// <param name="m22">The element M22.</param>
		public Matrix2x2(
		double m11, double m12,
		double m21, double m22)
		{
			M11 = m11; M12 = m12;
			M21 = m21; M22 = m22;

			Determinant = m11 * m22 - m21 * m12; ;
		}

		/// <summary>
		/// Creates a transformation matrix that uses three basis vectors to construct the matrix that transform points expressed in the three basis vectors to points in
		/// the coordinate system.
		/// </summary>
		/// <param name="xBasis">Basis vector for the x-direction.</param>
		/// <param name="yBasis">Basis vector for the y-direction.</param>
		/// <returns>A transformation matrix that uses the three basis vectors, and a location</returns>
		public static Matrix2x2 NewFromBasisVectors(VectorD2D xBasis, VectorD2D yBasis)
		{
			return new Matrix2x2(
				xBasis.X, xBasis.Y,
				yBasis.X, yBasis.Y
				);
		}

		#endregion Constructors

		/// <summary>
		/// Gets the identity matrix.
		/// </summary>
		public static Matrix2x2 Identity
		{
			get
			{
				return _identityMatrix;
			}
		}

		/// <summary>
		/// Transforms the specified vector <paramref name="v"/>.
		/// The transformation is carried out as a prepend transformation, i.e. result = v * matrix (v considered as horizontal vector).
		/// </summary>
		/// <param name="v">The vector to transform.</param>
		/// <returns>The transformed vector.</returns>
		public VectorD2D Transform(VectorD2D v)
		{
			double x = v.X;
			double y = v.Y;
			return new VectorD2D(
			x * M11 + y * M21,
			x * M12 + y * M22
			);
		}

		/// <summary>
		/// Transforms the specified point <paramref name="p"/>. Here, the point transform is carried out in the same way as the vector transform.
		/// The transformation is carried out as a prepend transformation, i.e. result = p * matrix (p considered as horizontal vector).
		/// </summary>
		/// <param name="p">The point to transform.</param>
		/// <returns>The transformed point.</returns>
		public PointD2D Transform(PointD2D p)
		{
			double x = p.X;
			double y = p.Y;
			return new PointD2D(
			x * M11 + y * M21,
			x * M12 + y * M22
			);
		}



		#region Append transformations

		/// <summary>
		/// Appends a transformation matrix <paramref name="f"/> to this matrix.
		/// </summary>
		/// <param name="f">The matrix to append.</param>
		public void AppendTransform(Matrix2x2 f)
		{
			double h1, h2;

			h1 = M11 * f.M11 + M12 * f.M21;
			h2 = M11 * f.M12 + M12 * f.M22;
			M11 = h1; M12 = h2;

			h1 = M21 * f.M11 + M22 * f.M21;
			h2 = M21 * f.M12 + M22 * f.M22;
			M21 = h1; M22 = h2;

			Determinant *= f.Determinant;
		}

		/// <summary>
		/// Prepends a transformation matrix <paramref name="a"/> to this matrix.
		/// </summary>
		/// <param name="a">The matrix to prepend.</param>
		public void PrependTransform(Matrix2x2 a)
		{
			double h1, h2;

			h1 = M11 * a.M11 + M21 * a.M12;
			h2 = M11 * a.M21 + M21 * a.M22;
			M11 = h1; M21 = h2;

			h1 = M12 * a.M11 + M22 * a.M12;
			h2 = M12 * a.M21 + M22 * a.M22;
			M12 = h1; M22 = h2;

			Determinant *= a.Determinant;
		}

		#endregion Append transformations

		#region Inverse transformations

		/// <summary>
		/// Inverse transform a vector p in such a way that the result will fullfill the relation p = result * matrix ( the * operator being the prepend transformation for vectors).
		/// </summary>
		/// <param name="p">The vector p to inverse transform.</param>
		/// <returns>The inverse transformation of point <paramref name="p"/>.</returns>
		public VectorD2D InverseTransform(VectorD3D p)
		{
			return new VectorD2D((M22 * p.X - M21 * p.Y) / Determinant, (M11 * p.Y - M12 * p.X) / Determinant);
		}

		#endregion Inverse transformations

		public override string ToString()
		{
			var stb = new StringBuilder(4 * 12);

			stb.Append("{");

			stb.Append("{");
			stb.AppendFormat("M11="); stb.Append(M11); stb.Append("; ");
			stb.AppendFormat("M12="); stb.Append(M12); stb.Append("; ");
			stb.Append("}, ");

			stb.Append("{");
			stb.AppendFormat("M21="); stb.Append(M21); stb.Append("; ");
			stb.AppendFormat("M22="); stb.Append(M22); stb.Append("; ");
			stb.Append("}, ");

			stb.Append("}");

			return stb.ToString();
		}
	}
}