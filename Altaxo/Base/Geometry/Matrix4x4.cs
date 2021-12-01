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

#nullable enable
using System.Text;

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
      _determinant = null;
    }

    public Matrix4x4(Matrix4x3 a)
    {
      M11 = a.M11;
      M12 = a.M12;
      M13 = a.M13;
      M14 = a.M14;
      M21 = a.M21;
      M22 = a.M22;
      M23 = a.M23;
      M24 = a.M24;
      M31 = a.M31;
      M32 = a.M32;
      M33 = a.M33;
      M34 = a.M34;
      M41 = a.M41;
      M42 = a.M42;
      M43 = a.M43;
      M44 = a.M44;
      _determinant = null;
    }

    double? _determinant;
    public double Determinant
    {
      get
      {
        return _determinant ??=
          M12 * M24 * M33 * M41 - M12 * M23 * M34 * M41 - M11 * M24 * M33 * M42 + M11 * M23 * M34 * M42 +
          M11 * M24 * M32 * M43 - M12 * M24 * M32 * M43 + M12 * M21 * M34 * M43 - M11 * M22 * M34 * M43 +
          M14 * (-(M22 * M33 * M41) + M23 * M32 * (M41 - M42) + M21 * M33 * M42 - M21 * M32 * M43 +
          M22 * M32 * M43) + (-(M11 * M23 * M32) + M12 * M23 * M32 - M12 * M21 * M33 +
          M11 * M22 * M33) * M44 + M13 * (M22 * M34 * M41 - M21 * M34 * M42 +
          M24 * M32 * (-M41 + M42) + M21 * M32 * M44 - M22 * M32 * M44);
      }
    }

    public Matrix4x4 Inverse()
    {
      var d = Determinant;

      return new Matrix4x4
        (
          (-(M24 * M33 * M42) + M23 * M34 * M42 + M24 * M32 * M43 - M22 * M34 * M43 - M23 * M32 * M44 + M22 * M33 * M44) * d,
          (M14 * M33 * M42 - M13 * M34 * M42 - M14 * M32 * M43 + M12 * M34 * M43 + M13 * M32 * M44 - M12 * M33 * M44) * d,
          (-(M14 * M23 * M42) + M13 * M24 * M42 + M14 * M22 * M43 - M12 * M24 * M43 - M13 * M22 * M44 + M12 * M23 * M44) * d,
          (M14 * M23 * M32 - M13 * M24 * M32 - M14 * M22 * M33 + M12 * M24 * M33 + M13 * M22 * M34 - M12 * M23 * M34) * d,

          (M24 * M33 * M41 - M23 * M34 * M41 - M24 * M32 * M43 + M21 * M34 * M43 + M23 * M32 * M44 - M21 * M33 * M44) * d,
          (-(M14 * M33 * M41) + M13 * M34 * M41 + M14 * M32 * M43 - M11 * M34 * M43 - M13 * M32 * M44 + M11 * M33 * M44) * d,
          (M14 * M23 * M41 - M13 * M24 * M41 - M14 * M21 * M43 + M11 * M24 * M43 + M13 * M21 * M44 - M11 * M23 * M44) * d,
          (-(M14 * M23 * M32) + M13 * M24 * M32 + M14 * M21 * M33 - M11 * M24 * M33 - M13 * M21 * M34 + M11 * M23 * M34) * d,

          (M22 * M34 * M41 - M21 * M34 * M42 + M24 * M32 * (-M41 + M42) + M21 * M32 * M44 - M22 * M32 * M44) * d,
          (-(M12 * M34 * M41) + M14 * M32 * (M41 - M42) + M11 * M34 * M42 - M11 * M32 * M44 + M12 * M32 * M44) * d,
          (-(M14 * M22 * M41) + M12 * M24 * M41 + M14 * M21 * M42 - M11 * M24 * M42 - M12 * M21 * M44 + M11 * M22 * M44) * d,
          (M14 * (-M21 + M22) * M32 + M11 * M24 * M32 - M12 * M24 * M32 + M12 * M21 * M34 - M11 * M22 * M34) * d,

          (-(M22 * M33 * M41) + M23 * M32 * (M41 - M42) + M21 * M33 * M42 - M21 * M32 * M43 + M22 * M32 * M43) * d,
          (M12 * M33 * M41 - M11 * M33 * M42 + M13 * M32 * (-M41 + M42) + M11 * M32 * M43 - M12 * M32 * M43) * d,
          (M13 * M22 * M41 - M12 * M23 * M41 - M13 * M21 * M42 + M11 * M23 * M42 + M12 * M21 * M43 - M11 * M22 * M43) * d,
          (M13 * (M21 - M22) * M32 - M11 * M23 * M32 + M12 * M23 * M32 - M12 * M21 * M33 + M11 * M22 * M33) * d
        );
    }

    public double this[int r, int c]
    {
      get
      {
        return (r, c) switch
        {
          (0, 0) => M11,
          (0, 1) => M12,
          (0, 2) => M13,
          (0, 3) => M14,

          (1, 0) => M21,
          (1, 1) => M22,
          (1, 2) => M23,
          (1, 3) => M24,

          (2, 0) => M31,
          (2, 1) => M32,
          (2, 2) => M33,
          (2, 3) => M34,

          (3, 0) => M41,
          (3, 1) => M42,
          (3, 2) => M43,
          (3, 3) => M44,

          _ => throw new System.ArgumentOutOfRangeException()
        };
      }
      /* No set accessor since this matrix should be immutable
      set
      {
        switch (r,c)
        {
          case (0, 0): M11 = value; break;
          case (0, 1): M12 = value; break;
          case (0, 2): M13 = value; break;
          case (0, 3): M14 = value; break;

          case (1, 0): M21 = value; break;
          case (1, 1): M22 = value; break;
          case (1, 2): M23 = value; break;
          case (1, 3): M24 = value; break;

          case (2, 0): M31 = value; break;
          case (2, 1): M32 = value; break;
          case (2, 2): M33 = value; break;
          case (2, 3): M34 = value; break;

          case (3, 0): M41 = value; break;
          case (3, 1): M42 = value; break;
          case (3, 2): M43 = value; break;
          case (3, 3): M44 = value; break;
        }
      }
      */
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

    /// <summary>
    /// Decomposes this matrix into a product Q*R, in which  Q is an orthonormal matrix, and R is an upper triangular matrix,
    /// using the Gram–Schmidt process. This process is also known as QR-decomposition (<see href="https://en.wikipedia.org/wiki/QR_decomposition#QL,_RQ_and_LQ_decompositions"/>).
    /// </summary>
    /// <returns>The two matrices Q (orthonormal) and R (upper triangular).</returns>
    public (Matrix4x4 Q, Matrix4x4 R) DecomposeIntoQR()
    {
      var a1 = new VectorD4D(M11, M21, M31, M41);
      var c1 = a1;
      var e1 = c1.Normalized;

      var a2 = new VectorD4D(M12, M22, M32, M42);
      var c2 = a2 - VectorD4D.DotProduct(a2, e1) * e1;
      var e2 = c2.Normalized;

      var a3 = new VectorD4D(M13, M23, M33, M43);
      var c3 = a3 - VectorD4D.DotProduct(a3, e1) * e1 - VectorD4D.DotProduct(a3, e2) * e2;
      var e3 = c3.Normalized;

      var a4 = new VectorD4D(M14, M24, M34, M44);
      var c4 = a4 - VectorD4D.DotProduct(a4, e1) * e1 - VectorD4D.DotProduct(a4, e2) * e2 - VectorD4D.DotProduct(a4, e3) * e3;
      var e4 = c4.Normalized;

      var q = new Matrix4x4(
                e1.X, e2.X, e3.X, e4.X,
                e1.Y, e2.Y, e3.Y, e4.Y,
                e1.Z, e2.Z, e3.Z, e4.Z,
                e1.W, e2.W, e3.W, e4.W
                );

      var r = new Matrix4x4(
                VectorD4D.DotProduct(a1, e1), VectorD4D.DotProduct(a2, e1), VectorD4D.DotProduct(a3, e1), VectorD4D.DotProduct(a4, e1),
                0, VectorD4D.DotProduct(a2, e2), VectorD4D.DotProduct(a3, e2), VectorD4D.DotProduct(a4, e2),
                0, 0, VectorD4D.DotProduct(a3, e3), VectorD4D.DotProduct(a4, e3),
                0, 0, 0, VectorD4D.DotProduct(a4, e4)
                );

      return (q, r);
    }


    /// <summary>
    /// Decomposes this matrix into a product R*Q, in which R is an upper triangular matrix, and Q is an orthonormal matrix,
    /// using the Gram–Schmidt process. This process is also known as RQ-decomposition (<see href="https://en.wikipedia.org/wiki/QR_decomposition#QL,_RQ_and_LQ_decompositions"/>).
    /// </summary>
    /// <returns>The two matrices R (upper triangular) and Q (orthonormal).</returns>

    public (Matrix4x4 R, Matrix4x4 Q) DecomposeIntoRQ()
    {
      var a1 = new VectorD4D(M41, M42, M43, M44);
      var c1 = a1;
      var e1 = c1.Normalized;

      var a2 = new VectorD4D(M31, M32, M33, M34);
      var c2 = a2 - VectorD4D.DotProduct(a2, e1) * e1;
      var e2 = c2.Normalized;

      var a3 = new VectorD4D(M21, M22, M23, M24);
      var c3 = a3 - VectorD4D.DotProduct(a3, e1) * e1 - VectorD4D.DotProduct(a3, e2) * e2;
      var e3 = c3.Normalized;

      var a4 = new VectorD4D(M11, M12, M13, M14);
      var c4 = a4 - VectorD4D.DotProduct(a4, e1) * e1 - VectorD4D.DotProduct(a4, e2) * e2 - VectorD4D.DotProduct(a4, e3) * e3;
      var e4 = c4.Normalized;

      var q = new Matrix4x4(
                e4.X, e4.Y, e4.Z, e4.W,
                e3.X, e3.Y, e3.Z, e3.W,
                e2.X, e2.Y, e2.Z, e2.W,
                e1.X, e1.Y, e1.Z, e1.W
                );

      var r = new Matrix4x4(
                VectorD4D.DotProduct(a4, e4), VectorD4D.DotProduct(a4, e3), VectorD4D.DotProduct(a4, e2), VectorD4D.DotProduct(a4, e1),
                0, VectorD4D.DotProduct(a3, e3), VectorD4D.DotProduct(a3, e2), VectorD4D.DotProduct(a3, e1),
                0, 0, VectorD4D.DotProduct(a2, e2), VectorD4D.DotProduct(a2, e1),
                0, 0, 0, VectorD4D.DotProduct(a1, e1)
                );

      return (r, q);
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
