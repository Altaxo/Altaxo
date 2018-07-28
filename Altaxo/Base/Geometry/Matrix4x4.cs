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
  /// Transformation matrix for general non-affine transformations in 3D space.
  /// This matrix is used mainly for intermediate results.
  /// </summary>
  public struct Matrix4x4
  {
    /// <summary>Gets the matrix element M[1,1].</summary>
    public double M11 { get; private set; }

    /// <summary>Gets the matrix element M[1,2].</summary>
    public double M12 { get; private set; }

    /// <summary>Gets the matrix element M[1,3].</summary>
    public double M13 { get; private set; }

    /// <summary>Gets the matrix element M[1,4].</summary>
    public double M14 { get; private set; }

    /// <summary>Gets the matrix element M[2,1].</summary>
    public double M21 { get; private set; }

    /// <summary>Gets the matrix element M[2,2].</summary>
    public double M22 { get; private set; }

    /// <summary>Gets the matrix element M[2,2].</summary>
    public double M23 { get; private set; }

    /// <summary>Gets the matrix element M[2,4].</summary>
    public double M24 { get; private set; }

    /// <summary>Gets the matrix element M[3,1].</summary>
    public double M31 { get; private set; }

    /// <summary>Gets the matrix element M[3,1].</summary>
    public double M32 { get; private set; }

    /// <summary>Gets the matrix element M[3,3].</summary>
    public double M33 { get; private set; }

    /// <summary>Gets the matrix element M[3,4].</summary>
    public double M34 { get; private set; }

    /// <summary>Gets the matrix element M[4,1]. This is OffsetX.</summary>
    public double M41 { get; private set; }

    /// <summary>Gets the matrix element M[4,2]. This is OffsetY.</summary>
    public double M42 { get; private set; }

    /// <summary>Gets the matrix element M[4,3]. This is OffsetZ.</summary>
    public double M43 { get; private set; }

    /// <summary>Gets the matrix element M[4,4].</summary>
    public double M44 { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix4x4"/> struct.
    /// </summary>
    /// <param name="m11">The element M11.</param>
    /// <param name="m12">The element M12.</param>
    /// <param name="m13">The element M13.</param>
    /// <param name="m14">The element M14.</param>
    /// <param name="m21">The element M21.</param>
    /// <param name="m22">The element M22.</param>
    /// <param name="m23">The element M23.</param>
    /// <param name="m24">The element M24.</param>
    /// <param name="m31">The element M31.</param>
    /// <param name="m32">The element M32.</param>
    /// <param name="m33">The element M33.</param>
    /// <param name="m34">The element M34.</param>
    /// <param name="m41">The element M41, which is offset x.</param>
    /// <param name="m42">The element M42, which is offset y.</param>
    /// <param name="m43">The element M43, which is offset z.</param>
    /// <param name="m44">The element M44.</param>

    public Matrix4x4(
    double m11, double m12, double m13, double m14,
    double m21, double m22, double m23, double m24,
    double m31, double m32, double m33, double m34,
    double m41, double m42, double m43, double m44)
    {
      M11 = m11;
      M12 = m12;
      M13 = m13;
      M14 = m14;
      M21 = m21;
      M22 = m22;
      M23 = m23;
      M24 = m24;
      M31 = m31;
      M32 = m32;
      M33 = m33;
      M34 = m34;
      M41 = m41;
      M42 = m42;
      M43 = m43;
      M44 = m44;
    }

    /// <summary>
    /// Transforms the specified point <paramref name="p"/>. For a point transform, the offset elements M41..M43 are used.
    /// The transformation is carried out as a prepend transformation, i.e. result = p * matrix (p considered as horizontal vector).
    /// </summary>
    /// <param name="p">The point to transform.</param>
    /// <returns>The transformed point.</returns>
    public PointD3D Transform(PointD3D p)
    {
      double x = p.X;
      double y = p.Y;
      double z = p.Z;

      double tw = x * M14 + y * M24 + z * M34 + M44;

      return new PointD3D(
      (x * M11 + y * M21 + z * M31 + M41) / tw,
      (x * M12 + y * M22 + z * M32 + M42) / tw,
      (x * M13 + y * M23 + z * M33 + M43) / tw
      );
    }

    /// <summary>
    /// Returns a new matrix based on the current matrix, but onto which another transformation was prepended.
    /// </summary>
    /// <param name="l">The matrix to prepend.</param>
    /// <returns>New matrix based on the current matrix, but onto which another transformation was prepended.</returns>
    public Matrix4x4 WithPrependedTransformation(Matrix4x3 l)
    {
      return new Matrix4x4(
        l.M11 * M11 + l.M12 * M21 + l.M13 * M31, l.M11 * M12 + l.M12 * M22 + l.M13 * M32, l.M11 * M13 + l.M12 * M23 + l.M13 * M33, l.M11 * M14 + l.M12 * M24 + l.M13 * M34,
        l.M21 * M11 + l.M22 * M21 + l.M23 * M31, l.M21 * M12 + l.M22 * M22 + l.M23 * M32, l.M21 * M13 + l.M22 * M23 + l.M23 * M33, l.M21 * M14 + l.M22 * M24 + l.M23 * M34,
        l.M31 * M11 + l.M32 * M21 + l.M33 * M31, l.M31 * M12 + l.M32 * M22 + l.M33 * M32, l.M31 * M13 + l.M32 * M23 + l.M33 * M33, l.M31 * M14 + l.M32 * M24 + l.M33 * M34,
        l.M41 * M11 + l.M42 * M21 + l.M43 * M31 + M41, l.M41 * M12 + l.M42 * M22 + l.M43 * M32 + M42, l.M41 * M13 + l.M42 * M23 + l.M43 * M33 + M43, l.M41 * M14 + l.M42 * M24 + l.M43 * M34 + M44
   );
    }

    public override string ToString()
    {
      var stb = new StringBuilder(12 * 12);

      stb.Append("{");

      stb.Append("{");
      stb.AppendFormat("M11=");
      stb.Append(M11);
      stb.Append("; ");
      stb.AppendFormat("M12=");
      stb.Append(M12);
      stb.Append("; ");
      stb.AppendFormat("M13=");
      stb.Append(M13);
      stb.Append("; ");
      stb.AppendFormat("M14=");
      stb.Append(M14);
      stb.Append(";");
      stb.Append("}, ");

      stb.Append("{");
      stb.AppendFormat("M21=");
      stb.Append(M21);
      stb.Append("; ");
      stb.AppendFormat("M22=");
      stb.Append(M22);
      stb.Append("; ");
      stb.AppendFormat("M23=");
      stb.Append(M24);
      stb.Append("; ");
      stb.AppendFormat("M24=");
      stb.Append(M24);
      stb.Append(";");
      stb.Append("}, ");

      stb.Append("{");
      stb.AppendFormat("M31=");
      stb.Append(M31);
      stb.Append("; ");
      stb.AppendFormat("M32=");
      stb.Append(M32);
      stb.Append("; ");
      stb.AppendFormat("M33=");
      stb.Append(M33);
      stb.Append("; ");
      stb.AppendFormat("M34=");
      stb.Append(M34);
      stb.Append(";");
      stb.Append("}, ");

      stb.Append("{");
      stb.AppendFormat("M41=");
      stb.Append(M41);
      stb.Append("; ");
      stb.AppendFormat("M42=");
      stb.Append(M42);
      stb.Append("; ");
      stb.AppendFormat("M43=");
      stb.Append(M43);
      stb.Append("; ");
      stb.AppendFormat("M44=");
      stb.Append(M44);
      stb.Append(";");
      stb.Append("}");

      stb.Append("}");

      return stb.ToString();
    }
  }
}
