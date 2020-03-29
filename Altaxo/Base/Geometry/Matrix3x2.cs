#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
  /// Transformation matrix for affine transformations in 2D space.
  /// The elements M13 and M23 are assumed to be 0, and M33 is assumed to be 1.
  /// Remember: when going from root to leaf node, <b>prepend</b> the new transformation to the existing transformation. And when going from leaf to root node, <b>append</b> the new transformation to the existing one.
  /// </summary>
  public readonly struct Matrix3x2 : IEquatable<Matrix3x2>, Main.IImmutable
  {
    #region Members

    /// <summary>
    /// For fast creation of the identity matrix.
    /// </summary>
    private static Matrix3x2 _identityMatrix;

    /// <summary>Gets the matrix element M[1,1].</summary>
    public double M11 { get; }

    /// <summary>Gets the matrix element M[1,2].</summary>
    public double M12 { get; }

    /// <summary>Gets the matrix element M[1,3] (is always = 0).</summary>
    public double M13 { get { return 0; } }

    /// <summary>Gets the matrix element M[2,1].</summary>
    public double M21 { get; }

    /// <summary>Gets the matrix element M[2,2].</summary>
    public double M22 { get; }

    /// <summary>Gets the matrix element M[2,3] (is always = 0).</summary>
    public double M23 { get { return 0; } }

    /// <summary>Gets the matrix element M[3,1].</summary>
    public double M31 { get; }

    /// <summary>Gets the matrix element M[3,1].</summary>
    public double M32 { get; }

    /// <summary>Gets the matrix element M[3,3] (is always = 1).</summary>
    public double M33 { get { return 1; } }

    /// <summary>The determinant of the matrix.</summary>
    public double Determinant { get; }

    #endregion Members

    #region Constructors

    static Matrix3x2()
    {
      _identityMatrix = new Matrix3x2(
          1, 0,
          0, 1,
          0, 0,
          1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix4x3"/> struct.
    /// </summary>
    /// <param name="m11">The element M11.</param>
    /// <param name="m12">The element M12.</param>
    /// <param name="m21">The element M21.</param>
    /// <param name="m22">The element M22.</param>
    /// <param name="m31">The element M31.</param>
    /// <param name="m32">The element M32.</param>
    public Matrix3x2(
    double m11, double m12,
    double m21, double m22,
    double m31, double m32
    )
    {
      M11 = m11;
      M12 = m12;
      M21 = m21;
      M22 = m22;
      M31 = m31;
      M32 = m32;

      Determinant = m11 * m22 - m21 * m12;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix4x3"/> struct. For internal use only, since the determinant must be pre-calculated.
    /// </summary>
    /// <param name="m11">The element M11.</param>
    /// <param name="m12">The element M12.</param>
    /// <param name="m21">The element M21.</param>
    /// <param name="m22">The element M22.</param>
    /// <param name="m31">The element M31.</param>
    /// <param name="m32">The element M32.</param>
    /// <param name="determinant">The determinant of the new matrix.</param>
    private Matrix3x2(
    double m11, double m12,
    double m21, double m22,
    double m31, double m32,
    double determinant)
    {
      M11 = m11;
      M12 = m12;
      M21 = m21;
      M22 = m22;
      M31 = m31;
      M32 = m32;

      Determinant = determinant;
    }

    /// <summary>
    /// Creates a transformation matrix that uses three basis vectors, and a location to construct the matrix that transform points expressed in the three basis vectors to points in
    /// the coordinate system.
    /// </summary>
    /// <param name="xBasis">Basis vector for the x-direction.</param>
    /// <param name="yBasis">Basis vector for the y-direction.</param>
    /// <param name="origin">The origin of the coordinate system.</param>
    /// <returns>A transformation matrix that uses the three basis vectors, and a location</returns>
    public static Matrix3x2 NewFromBasisVectorsAndLocation(VectorD2D xBasis, VectorD2D yBasis, PointD3D origin)
    {
      return new Matrix3x2(
        xBasis.X, xBasis.Y,
        yBasis.X, yBasis.Y,
        origin.X, origin.Y);
    }

    public static Matrix3x2 NewTranslation(VectorD2D d)
    {
      return new Matrix3x2(
        1, 0,
        0, 1,
        d.X, d.Y,
        1
        );
    }

    public static Matrix3x2 NewTranslation(double dx, double dy)
    {
      return new Matrix3x2(
        1, 0,
        0, 1,
        dx, dy,
        determinant: 1
        );
    }

    /// <summary>
    /// Gets a transformation matrix by specifying scale, shear, rotation and translation. The returned matrix is equivalent to Scale*Shear*Rotation*Translation.
    /// </summary>
    /// <param name="scaleX">The scale value x.</param>
    /// <param name="scaleY">The scale value y.</param>
    /// <param name="shearX">The shear value x.</param>
    /// <param name="shearY">The shear value y.</param>
    /// <param name="angleDegree">The rotation around x axis in degrees.</param>
    /// <param name="translateX">The translation in x direction.</param>
    /// <param name="translateY">The translation in y direction.</param>
    /// <returns>The transformation matrix. A point transformed with this matrix is first translated, then rotated, then sheared, then scaled.</returns>
    public static Matrix3x2 NewScalingShearingRotationDegreesTranslation(double scaleX, double scaleY, double shearX, double shearY, double angleDegree, double translateX, double translateY)
    {
      return NewScalingShearingRotationRadianTranslation(scaleX, scaleY, shearX, shearY, (angleDegree / 180) * Math.PI, translateX, translateY);
    }

    /// <summary>
    /// Gets a transformation matrix by specifying scale, shear, rotation and translation. The returned matrix is equivalent to Scale*Shear*Rotation*Translation.
    /// </summary>
    /// <param name="scaleX">The scale value x.</param>
    /// <param name="scaleY">The scale value y.</param>
    /// <param name="shearX">The shear in x direction.</param>
    /// <param name="shearY">The shear in y direction.</param>
    /// <param name="angle">The rotation around x axis in radian.</param>
    /// <param name="translateX">The translation in x direction.</param>
    /// <param name="translateY">The translation in y direction.</param>
    /// <returns>The transformation matrix. A point transformed with this matrix is first translated, then rotated, then sheared, then scaled.</returns>
    public static Matrix3x2 NewScalingShearingRotationRadianTranslation(double scaleX, double scaleY, double shearX, double shearY, double angle, double translateX, double translateY)
    {
      double cos = Math.Cos(angle);
      double sin = Math.Sin(angle);

      return new Matrix3x2(
        scaleX * (cos - sin * shearY), scaleX * (sin + cos * shearY),
        scaleY * (cos * shearX - sin), scaleY * (cos + sin * shearX),
        translateX, translateY,
        determinant: scaleX * scaleY * (1 - shearX * shearY) // Determinant
        );
    }

    /// <summary>
    /// Gets a transformation matrix by specifying rotation (translation is 0, shear is 0 and scale is 1).
    /// </summary>
    /// <param name="angleInDegrees">The rotation around x axis in degrees.</param>
    /// <returns>The transformation matrix.</returns>
    public static Matrix3x2 NewRotation(double angleInDegrees)
    {
      return NewScalingShearingRotationDegreesTranslation(1, 1, 0, 0, angleInDegrees, 0, 0);
    }



    #endregion Constructors

    #region Other properties

    /// <summary>
    /// Gets the identity matrix.
    /// </summary>
    public static Matrix3x2 Identity
    {
      get
      {
        return _identityMatrix;
      }
    }

    public bool IsIdentity
    {
      get
      {
        return
          M31 == 0 && M32 == 0 &&
          M11 == 1 && M22 == 1 &&
          M12 == 0 && M21 == 0;

      }
    }

    #endregion Other properties

    #region Transformation (of points, vectors)

    /// <summary>
    /// Transforms the specified vector <paramref name="v"/>. For a vector transform, the offset elements M31..M32 are ignored.
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
    /// Transforms the specified point <paramref name="p"/>. For a point transform, the offset elements M41..M43 are used.
    /// The transformation is carried out as a prepend transformation, i.e. result = p * matrix (p considered as horizontal vector).
    /// </summary>
    /// <param name="p">The point to transform.</param>
    /// <returns>The transformed point.</returns>
    public PointD2D Transform(PointD2D p)
    {
      double x = p.X;
      double y = p.Y;
      return new PointD2D(
      x * M11 + y * M21 + M31,
      x * M12 + y * M22 + M32
      );
    }

    #endregion Transformation (of points, vectors)

    #region Inverse transformations (of points, vectors)

    /// <summary>
    /// Inverse transform a point p in such a way that the result will fullfill the relation p = result * matrix ( the * operator being the prepend transformation for points).
    /// </summary>
    /// <param name="p">The point p to inverse transform.</param>
    /// <returns>The inverse transformation of point <paramref name="p"/>.</returns>
    public PointD2D InverseTransform(PointD2D p)
    {
      return new PointD2D(
          (M22 * (p.X - M31) + M21 * (M32 - p.Y)) / Determinant,
          (M12 * (M31 - p.X) + M11 * (p.Y - M32)) / Determinant
          );
    }

    /// <summary>
    /// Inverse transform a vector p in such a way that the result will fullfill the relation p = result * matrix ( the * operator being the prepend transformation for vectors).
    /// </summary>
    /// <param name="p">The point p to inverse transform.</param>
    /// <returns>The inverse transformation of Vector <paramref name="p"/>.</returns>
    public VectorD2D InverseTransform(VectorD2D p)
    {
      return new VectorD2D(
          (M22 * (p.X) + M21 * (-p.Y)) / Determinant,
          (M12 * (-p.X) + M11 * (p.Y)) / Determinant
          );
    }

    #endregion Inverse transformations (of points, vectors)

    #region Creation of new matrices by prepending or appending other matrices

    /// <summary>
    /// Appends a transformation matrix <paramref name="f"/> to this matrix, and returns a new matrix with the result. The original matrix is unchanged.
    /// </summary>
    /// <param name="f">The matrix to append.</param>
    /// <returns>A new matrix based on the existing one, but with matrix <paramref name="f"/> appended.</returns>
    public Matrix3x2 WithAppendedTransformation(Matrix3x2 f)
    {
      return new Matrix3x2(
       M11 * f.M11 + M12 * f.M21,
       M11 * f.M12 + M12 * f.M22,

      M21 * f.M11 + M22 * f.M21,
      M21 * f.M12 + M22 * f.M22,

      M31 * f.M11 + M32 * f.M21 + f.M31,
      M31 * f.M12 + M32 * f.M22 + f.M32,

      determinant: Determinant * f.Determinant
      );
    }

    /// <summary>
    /// Appends a transformation matrix <paramref name="f"/> to this matrix, and returns a new matrix with the result. The original matrix is unchanged.
    /// </summary>
    /// <param name="f">The matrix to append.</param>
    /// <returns>A new matrix based on the existing one, but with matrix <paramref name="f"/> appended.</returns>
    public Matrix3x2 WithAppendedTransformation(Matrix2x2 f)
    {
      return new Matrix3x2(
       M11 * f.M11 + M12 * f.M21,
       M11 * f.M12 + M12 * f.M22,

      M21 * f.M11 + M22 * f.M21,
      M21 * f.M12 + M22 * f.M22,

      M31 * f.M11 + M32 * f.M21,
      M31 * f.M12 + M32 * f.M22,

      determinant: Determinant * f.Determinant
      );
    }

    /// <summary>
    /// Prepends a transformation matrix <paramref name="l"/> to this matrix, and returns a new matrix with the result. The original matrix is unchanged.
    /// </summary>
    /// <param name="l">The matrix to prepend.</param>
    /// <returns>A new matrix based on the existing one, but with matrix <paramref name="l"/> prepended.</returns>
    public Matrix3x2 WithPrependedTransformation(Matrix3x2 l)
    {
      return new Matrix3x2(
        l.M11 * M11 + l.M12 * M21,
        l.M11 * M12 + l.M12 * M22,

        l.M21 * M11 + l.M22 * M21,
        l.M21 * M12 + l.M22 * M22,


        l.M31 * M11 + l.M32 * M21 + M31,
        l.M31 * M12 + l.M32 * M22 + M32,
        determinant: l.Determinant * Determinant
        );
    }

    /// <summary>
    /// Prepends a transformation matrix <paramref name="l"/> to this matrix, and returns a new matrix with the result. The original matrix is unchanged.
    /// </summary>
    /// <param name="l">The matrix to prepend.</param>
    /// <returns>A new matrix based on the existing one, but with matrix <paramref name="l"/> prepended.</returns>
    public Matrix3x2 WithPrependedTransformation(Matrix2x2 l)
    {
      return new Matrix3x2(
        l.M11 * M11 + l.M12 * M21,
        l.M11 * M12 + l.M12 * M22,

        l.M21 * M11 + l.M22 * M21,
        l.M21 * M12 + l.M22 * M22,

        M31,
        M32,
        determinant: l.Determinant * Determinant
        );
    }

    #endregion Creation of new matrices by prepending or appending other matrices





    #region Conversion to other matrices

    /// <summary>
    /// Gets the transposed inverse matrix of the matrix2x2 part of this matrix. The returned matrix is usually used to transform the normals of objects that will be transformed by this matrix.
    /// </summary>
    /// <returns>Transposed inverse matrix of the matrix2x2 part of this matrix.</returns>
    public Matrix2x2 GetTransposedInverseMatrix2x2()
    {
      return new Matrix2x2(
        (M22) / Determinant,
        (-M21) / Determinant,
        (-M12) / Determinant,
        (M11) / Determinant
        );
    }

    #endregion Conversion to other matrices

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
      stb.Append("}, ");

      stb.Append("{");
      stb.AppendFormat("M21=");
      stb.Append(M21);
      stb.Append("; ");
      stb.AppendFormat("M22=");
      stb.Append(M22);
      stb.Append("}, ");

      stb.Append("{");
      stb.AppendFormat("M31=");
      stb.Append(M31);
      stb.Append("; ");
      stb.AppendFormat("M32=");
      stb.Append(M32);
      stb.Append("}, ");

      stb.Append("}");

      return stb.ToString();
    }

    public bool Equals(Matrix3x2 other)
    {
      return
        M31 == other.M31 &&
        M32 == other.M32 &&
        M11 == other.M11 &&
        M22 == other.M22 &&
        M12 == other.M12 &&
        M21 == other.M21;
    }

    public static bool operator ==(Matrix3x2 x, Matrix3x2 y)
    {
      return
        x.M31 == y.M31 &&
        x.M32 == y.M32 &&
        x.M11 == y.M11 &&
        x.M22 == y.M22 &&
        x.M12 == y.M12 &&
        x.M21 == y.M21;
    }
    public static bool operator !=(Matrix3x2 x, Matrix3x2 y)
    {
      return !(x == y);
    }

    public override bool Equals(object obj)
    {
      return obj is Matrix3x2 y ? Equals(y) : false;
    }

    public override int GetHashCode()
    {
      return M11.GetHashCode() + 5 * M22.GetHashCode() + 7 * M31.GetHashCode() + 9 * M32.GetHashCode();
    }
  }
}
