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
  /// Transformation matrix for affine transformations in 3D space.
  /// The elements M14, M24 and M34 are assumed to be 0, and M44 is assumed to be 1.
  /// Remember: when going from root to leaf node, <b>prepend</b> the new transformation to the existing transformation. And when going from leaf to root node, <b>append</b> the new transformation to the existing one.
  /// </summary>
  public struct Matrix4x3
  {
    #region Members

    /// <summary>
    /// For fast creation of the identity matrix.
    /// </summary>
    private static Matrix4x3 _identityMatrix;

    /// <summary>Gets the matrix element M[1,1].</summary>
    public double M11 { get; private set; }

    /// <summary>Gets the matrix element M[1,2].</summary>
    public double M12 { get; private set; }

    /// <summary>Gets the matrix element M[1,3].</summary>
    public double M13 { get; private set; }

    /// <summary>Gets the matrix element M[1,4] (is always = 0).</summary>
    public double M14 { get { return 0; } }

    /// <summary>Gets the matrix element M[2,1].</summary>
    public double M21 { get; private set; }

    /// <summary>Gets the matrix element M[2,2].</summary>
    public double M22 { get; private set; }

    /// <summary>Gets the matrix element M[2,2].</summary>
    public double M23 { get; private set; }

    /// <summary>Gets the matrix element M[2,4] (is always = 0).</summary>
    public double M24 { get { return 0; } }

    /// <summary>Gets the matrix element M[3,1].</summary>
    public double M31 { get; private set; }

    /// <summary>Gets the matrix element M[3,1].</summary>
    public double M32 { get; private set; }

    /// <summary>Gets the matrix element M[3,3].</summary>
    public double M33 { get; private set; }

    /// <summary>Gets the matrix element M[3,4] (is always = 0).</summary>
    public double M34 { get { return 0; } }

    /// <summary>Gets the matrix element M[4,1]. This is OffsetX.</summary>
    public double M41 { get; private set; }

    /// <summary>Gets the matrix element M[4,2]. This is OffsetY.</summary>
    public double M42 { get; private set; }

    /// <summary>Gets the matrix element M[4,3]. This is OffsetZ.</summary>
    public double M43 { get; private set; }

    /// <summary>Gets the matrix element M[4,4] (is always = 1).</summary>
    public double M44 { get { return 1; } }

    /// <summary>The determinant of the matrix.</summary>
    public double Determinant { get; private set; }

    #endregion Members

    #region Constructors

    static Matrix4x3()
    {
      _identityMatrix = new Matrix4x3(
          1, 0, 0,
          0, 1, 0,
          0, 0, 1,
          0, 0, 0,
          1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix4x3"/> struct.
    /// </summary>
    /// <param name="m11">The element M11.</param>
    /// <param name="m12">The element M12.</param>
    /// <param name="m13">The element M13.</param>
    /// <param name="m21">The element M21.</param>
    /// <param name="m22">The element M22.</param>
    /// <param name="m23">The element M23.</param>
    /// <param name="m31">The element M31.</param>
    /// <param name="m32">The element M32.</param>
    /// <param name="m33">The element M33.</param>
    /// <param name="m41">The element M41, which is offset x.</param>
    /// <param name="m42">The element M42, which is offset y.</param>
    /// <param name="m43">The element M43, which is offset z.</param>
    public Matrix4x3(
    double m11, double m12, double m13,
    double m21, double m22, double m23,
    double m31, double m32, double m33,
    double m41, double m42, double m43)
    {
      M11 = m11;
      M12 = m12;
      M13 = m13;
      M21 = m21;
      M22 = m22;
      M23 = m23;
      M31 = m31;
      M32 = m32;
      M33 = m33;
      M41 = m41;
      M42 = m42;
      M43 = m43;

      Determinant = -(m13 * m22 * m31) + m12 * m23 * m31 + m13 * m21 * m32 - m11 * m23 * m32 - m12 * m21 * m33 + m11 * m22 * m33;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix4x3"/> struct. For internal use only, since the determinant must be pre-calculated.
    /// </summary>
    /// <param name="m11">The element M11.</param>
    /// <param name="m12">The element M12.</param>
    /// <param name="m13">The element M13.</param>
    /// <param name="m21">The element M21.</param>
    /// <param name="m22">The element M22.</param>
    /// <param name="m23">The element M23.</param>
    /// <param name="m31">The element M31.</param>
    /// <param name="m32">The element M32.</param>
    /// <param name="m33">The element M33.</param>
    /// <param name="m41">The element M41, which is offset x.</param>
    /// <param name="m42">The element M42, which is offset y.</param>
    /// <param name="m43">The element M43, which is offset z.</param>
    /// <param name="determinant">The determinant of the new matrix.</param>
    private Matrix4x3(
    double m11, double m12, double m13,
    double m21, double m22, double m23,
    double m31, double m32, double m33,
    double m41, double m42, double m43,
    double determinant)
    {
      M11 = m11;
      M12 = m12;
      M13 = m13;
      M21 = m21;
      M22 = m22;
      M23 = m23;
      M31 = m31;
      M32 = m32;
      M33 = m33;
      M41 = m41;
      M42 = m42;
      M43 = m43;

      Determinant = determinant;
    }

    /// <summary>
    /// Creates a transformation matrix that uses three basis vectors, and a location to construct the matrix that transform points expressed in the three basis vectors to points in
    /// the coordinate system.
    /// </summary>
    /// <param name="xBasis">Basis vector for the x-direction.</param>
    /// <param name="yBasis">Basis vector for the y-direction.</param>
    /// <param name="zBasis">Basis vector for the z-direction.</param>
    /// <param name="origin">The origin of the coordinate system.</param>
    /// <returns>A transformation matrix that uses the three basis vectors, and a location</returns>
    public static Matrix4x3 NewFromBasisVectorsAndLocation(VectorD3D xBasis, VectorD3D yBasis, VectorD3D zBasis, PointD3D origin)
    {
      return new Matrix4x3(
        xBasis.X, xBasis.Y, xBasis.Z,
        yBasis.X, yBasis.Y, yBasis.Z,
        zBasis.X, zBasis.Y, zBasis.Z,
        origin.X, origin.Y, origin.Z);
    }

    public static Matrix4x3 NewTranslation(VectorD3D d)
    {
      return new Matrix4x3(
        1, 0, 0,
        0, 1, 0,
        0, 0, 1,
        d.X, d.Y, d.Z,
        1
        );
    }

    public static Matrix4x3 NewTranslation(double dx, double dy, double dz)
    {
      return new Matrix4x3(
        1, 0, 0,
        0, 1, 0,
        0, 0, 1,
        dx, dy, dz,
        1
        );
    }

    /// <summary>
    /// Gets a transformation matrix by specifying scale, shear, rotation and translation. The returned matrix is equivalent to Scale*Shear*Rotation*Translation.
    /// </summary>
    /// <param name="scaleX">The scale value x.</param>
    /// <param name="scaleY">The scale value y.</param>
    /// <param name="scaleZ">The scale value z.</param>
    /// <param name="shearX">The shear value x.</param>
    /// <param name="shearY">The shear value y.</param>
    /// <param name="shearZ">The shear value z.</param>
    /// <param name="angleXdeg">The rotation around x axis in degrees.</param>
    /// <param name="angleYdeg">The rotation around y axis in degrees</param>
    /// <param name="angleZdeg">The rotation around z axis in degrees</param>
    /// <param name="translateX">The translation in x direction.</param>
    /// <param name="translateY">The translation in y direction.</param>
    /// <param name="translateZ">The translation in z direction.</param>
    /// <returns>The transformation matrix. A point transformed with this matrix is first translated, then rotated, then sheared, then scaled.</returns>
    public static Matrix4x3 NewScalingShearingRotationDegreesTranslation(double scaleX, double scaleY, double scaleZ, double shearX, double shearY, double shearZ, double angleXdeg, double angleYdeg, double angleZdeg, double translateX, double translateY, double translateZ)
    {
      return NewScalingShearingRotationRadianTranslation(scaleX, scaleY, scaleZ, shearX, shearY, shearZ, (angleXdeg / 180) * Math.PI, (angleYdeg / 180) * Math.PI, (angleZdeg / 180) * Math.PI, translateX, translateY, translateZ);
    }

    /// <summary>
    /// Gets a transformation matrix by specifying scale, shear, rotation and translation. The returned matrix is equivalent to Scale*Shear*Rotation*Translation.
    /// </summary>
    /// <param name="scaleX">The scale value x.</param>
    /// <param name="scaleY">The scale value y.</param>
    /// <param name="scaleZ">The scale value z.</param>
    /// <param name="shearX">The shear value x.</param>
    /// <param name="shearY">The shear value y.</param>
    /// <param name="shearZ">The shear value z.</param>
    /// <param name="angleXrad">The rotation around x axis in radian.</param>
    /// <param name="angleYrad">The rotation around y axis in radian</param>
    /// <param name="angleZrad">The rotation around z axis in radian</param>
    /// <param name="translateX">The translation in x direction.</param>
    /// <param name="translateY">The translation in y direction.</param>
    /// <param name="translateZ">The translation in z direction.</param>
    /// <returns>The transformation matrix. A point transformed with this matrix is first translated, then rotated, then sheared, then scaled.</returns>
    public static Matrix4x3 NewScalingShearingRotationRadianTranslation(double scaleX, double scaleY, double scaleZ, double shearX, double shearY, double shearZ, double angleXrad, double angleYrad, double angleZrad, double translateX, double translateY, double translateZ)
    {
      double cosX = Math.Cos(angleXrad);
      double sinX = Math.Sin(angleXrad);

      double cosY = Math.Cos(angleYrad);
      double sinY = Math.Sin(angleYrad);

      double cosZ = Math.Cos(angleZrad);
      double sinZ = Math.Sin(angleZrad);

      return new Matrix4x3(
        scaleX * (cosY * cosZ - cosX * cosZ * shearY * sinY + shearY * sinX * sinZ),
        -(scaleX * (cosZ * shearY * sinX - cosY * sinZ + cosX * shearY * sinY * sinZ)),
        scaleX * (cosX * cosY * shearY + sinY),
        scaleY * (cosY * cosZ * shearZ - cosZ * sinX * sinY - cosX * sinZ),
        scaleY * (cosX * cosZ + cosY * shearZ * sinZ - sinX * sinY * sinZ),
        scaleY * (cosY * sinX + shearZ * sinY),
        scaleZ * (cosY * cosZ * shearX * shearZ - cosZ * (cosX + shearX * sinX) * sinY + (-(cosX * shearX) + sinX) * sinZ),
        scaleZ * (cosY * shearX * shearZ * sinZ + cosX * (cosZ * shearX - sinY * sinZ) - sinX * (cosZ + shearX * sinY * sinZ)),
        scaleZ * (cosX * cosY + cosY * shearX * sinX + shearX * shearZ * sinY),
        translateX,
        translateY,
        translateZ,
        scaleX * scaleY * scaleZ // Determinant
        );
    }

    /// <summary>
    /// Gets a transformation matrix by specifying rotation (translation is 0, shear is 0 and scale is 1).
    /// </summary>
    /// <param name="angleX">The rotation around x axis in degrees.</param>
    /// <param name="angleY">The rotation around y axis in degrees</param>
    /// <param name="angleZ">The rotation around z axis in degrees</param>
    /// <returns>The transformation matrix.</returns>
    public static Matrix4x3 NewRotation(double angleX, double angleY, double angleZ)
    {
      return NewScalingShearingRotationDegreesTranslation(1, 1, 1, 0, 0, 0, angleX, angleY, angleZ, 0, 0, 0);
    }

    /// <summary>
    /// Creates the rotation matrix from axis and angle radian.
    /// </summary>
    /// <param name="u">The axis about which the rotation takes place.</param>
    /// <param name="angleRadian">The rotation angle in radian.</param>
    /// <param name="center">The center of rotation.</param>
    /// <returns>Matrix that describes the drotation.</returns>
    public static Matrix4x3 NewRotationFromAxisAndAngleRadian(VectorD3D u, double angleRadian, PointD3D center)
    {
      double cosTheta = Math.Cos(angleRadian);
      double oMCosTheta = 1 - cosTheta;
      double sinTheta = Math.Sin(angleRadian);

      double m11 = cosTheta + u.X * u.X * oMCosTheta;
      double m12 = u.X * u.Y * oMCosTheta + u.Z * sinTheta;
      double m13 = u.Z * u.X * oMCosTheta - u.Y * sinTheta;

      double m21 = u.X * u.Y * oMCosTheta - u.Z * sinTheta;
      double m22 = cosTheta + u.Y * u.Y * oMCosTheta;
      double m23 = u.Z * u.Y * oMCosTheta + u.X * sinTheta;

      double m31 = u.X * u.Z * oMCosTheta + u.Y * sinTheta;
      double m32 = u.Y * u.Z * oMCosTheta - u.X * sinTheta;
      double m33 = cosTheta + u.Z * u.Z * oMCosTheta;

      double offsetX = 0, offsetY = 0, offsetZ = 0;

      if (center.X != 0.0 || center.Y != 0.0 || center.Z != 0.0)
      {
        offsetX = -center.X * m11 - center.Y * m21 - center.Z * m31 + center.X;
        offsetY = -center.X * m12 - center.Y * m22 - center.Z * m32 + center.Y;
        offsetZ = -center.X * m13 - center.Y * m23 - center.Z * m33 + center.Z;
      }

      return new Matrix4x3(m11, m12, m13, m21, m22, m23, m31, m32, m33, offsetX, offsetY, offsetZ);
    }

    #endregion Constructors

    #region Other properties

    /// <summary>
    /// Gets the identity matrix.
    /// </summary>
    public static Matrix4x3 Identity
    {
      get
      {
        return _identityMatrix;
      }
    }

    #endregion Other properties

    #region Transformation (of points, vectors, planes)

    /// <summary>
    /// Transforms the specified vector <paramref name="v"/>. For a vector transform, the offset elements M41..M43 are ignored.
    /// The transformation is carried out as a prepend transformation, i.e. result = v * matrix (v considered as horizontal vector).
    /// </summary>
    /// <param name="v">The vector to transform.</param>
    /// <returns>The transformed vector.</returns>
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

    /// <summary>
    /// Transforms the specified vector <paramref name="v"/>. For a vector transform, the offset elements M41..M43 are ignored.
    /// The transformation is carried out as a prepend transformation, i.e. result = v * matrix (v considered as horizontal vector).
    /// </summary>
    /// <param name="v">The vector to transform. The z component is assumed to be 0.</param>
    /// <returns>The transformed vector.</returns>
    public VectorD3D Transform(VectorD2D v)
    {
      double x = v.X;
      double y = v.Y;
      return new VectorD3D(
      x * M11 + y * M21,
      x * M12 + y * M22,
      x * M13 + y * M23
      );
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
      return new PointD3D(
      x * M11 + y * M21 + z * M31 + M41,
      x * M12 + y * M22 + z * M32 + M42,
      x * M13 + y * M23 + z * M33 + M43
      );
    }

    /// <summary>
    /// Transforms the specified point <paramref name="p"/>. For a point transform, the offset elements M41..M43 are used.
    /// The transformation is carried out as a prepend transformation, i.e. result = p * matrix (p considered as horizontal vector).
    /// </summary>
    /// <param name="p">The point to transform. The z component is assumed to be 0.</param>
    /// <returns>The transformed point.</returns>
    public PointD3D Transform(PointD2D p)
    {
      double x = p.X;
      double y = p.Y;
      return new PointD3D(
      x * M11 + y * M21 + M41,
      x * M12 + y * M22 + M42,
      x * M13 + y * M23 + M43
      );
    }

    /// <summary>
    /// Transforms the specified plane p.
    /// </summary>
    /// <param name="p">The plane to transform.</param>
    /// <returns>Plane transformed by this matrix.</returns>
    public PlaneD3D Transform(PlaneD3D p)
    {
      var x = p.X * (M22 * M33 - M23 * M32) + p.Y * (M13 * M32 - M12 * M33) + p.Z * (M12 * M23 - M13 * M22);
      var y = p.X * (M23 * M31 - M21 * M33) + p.Y * (M11 * M33 - M13 * M31) + p.Z * (M13 * M21 - M11 * M23);
      var z = p.X * (M21 * M32 - M22 * M31) + p.Y * (M12 * M31 - M11 * M32) + p.Z * (M11 * M22 - M12 * M21);

      var l = 1 / Math.Sqrt(x * x + y * y + z * z);
      x *= l;
      y *= l;
      z *= l;

      // Transform the point that was located on the original plane....
      var pp = Transform(new PointD3D(p.X * p.W, p.Y * p.W, p.Z * p.W));

      // but the transformed point is not neccessarly located from the origin in the direction of the new plane vector
      // thus we have to take the dot-product between the transformed normal and the transformed plane-point to get the new distance
      double w = x * pp.X + y * pp.Y + z * pp.Z;
      return new PlaneD3D(x, y, z, w);
    }

    #endregion Transformation (of points, vectors, planes)

    #region Inverse transformations (of points, vectors)

    /// <summary>
    /// Inverse transform a point p in such a way that the result will fullfill the relation p = result * matrix ( the * operator being the prepend transformation for points).
    /// </summary>
    /// <param name="p">The point p to inverse transform.</param>
    /// <returns>The inverse transformation of point <paramref name="p"/>.</returns>
    public PointD3D InverseTransform(PointD3D p)
    {
      return new PointD3D(
          (M23 * (M32 * (M41 - p.X) + M31 * (-M42 + p.Y)) + M22 * (-(M33 * M41) + M31 * M43 + M33 * p.X - M31 * p.Z) + M21 * (M33 * M42 - M32 * M43 - M33 * p.Y + M32 * p.Z)) / Determinant,

          (M13 * (M32 * (-M41 + p.X) + M31 * (M42 - p.Y)) + M12 * (M33 * M41 - M31 * M43 - M33 * p.X + M31 * p.Z) + M11 * (-(M33 * M42) + M32 * M43 + M33 * p.Y - M32 * p.Z)) / Determinant,

          (M13 * (M22 * (M41 - p.X) + M21 * (-M42 + p.Y)) + M12 * (-(M23 * M41) + M21 * M43 + M23 * p.X - M21 * p.Z) + M11 * (M23 * M42 - M22 * M43 - M23 * p.Y + M22 * p.Z)) / Determinant
          );
    }

    /// <summary>
    /// Inverse transform a vector p in such a way that the result will fullfill the relation p = result * matrix ( the * operator being the prepend transformation for vectors).
    /// </summary>
    /// <param name="p">The point p to inverse transform.</param>
    /// <returns>The inverse transformation of point <paramref name="p"/>.</returns>
    public VectorD3D InverseTransform(VectorD3D p)
    {
      return new VectorD3D(
          (-(M23 * M32 * p.X) + M22 * M33 * p.X + M23 * M31 * p.Y - M21 * M33 * p.Y - M22 * M31 * p.Z + M21 * M32 * p.Z) / Determinant,

          (M13 * M32 * p.X - M12 * M33 * p.X - M13 * M31 * p.Y + M11 * M33 * p.Y + M12 * M31 * p.Z - M11 * M32 * p.Z) / Determinant,

          (-(M13 * M22 * p.X) + M12 * M23 * p.X + M13 * M21 * p.Y - M11 * M23 * p.Y - M12 * M21 * p.Z + M11 * M22 * p.Z) / Determinant

          );
    }

    #endregion Inverse transformations (of points, vectors)

    #region Creation of new matrices by prepending or appending other matrices

    /// <summary>
    /// Appends a transformation matrix <paramref name="f"/> to this matrix, and returns a new matrix with the result. The original matrix is unchanged.
    /// </summary>
    /// <param name="f">The matrix to append.</param>
    /// <returns>A new matrix based on the existing one, but with matrix <paramref name="f"/> appended.</returns>
    public Matrix4x3 WithAppendedTransformation(Matrix4x3 f)
    {
      return new Matrix4x3(
       M11 * f.M11 + M12 * f.M21 + M13 * f.M31,
       M11 * f.M12 + M12 * f.M22 + M13 * f.M32,
       M11 * f.M13 + M12 * f.M23 + M13 * f.M33,

      M21 * f.M11 + M22 * f.M21 + M23 * f.M31,
      M21 * f.M12 + M22 * f.M22 + M23 * f.M32,
      M21 * f.M13 + M22 * f.M23 + M23 * f.M33,

      M31 * f.M11 + M32 * f.M21 + M33 * f.M31,
      M31 * f.M12 + M32 * f.M22 + M33 * f.M32,
      M31 * f.M13 + M32 * f.M23 + M33 * f.M33,

      M41 * f.M11 + M42 * f.M21 + M43 * f.M31 + f.M41,
      M41 * f.M12 + M42 * f.M22 + M43 * f.M32 + f.M42,
      M41 * f.M13 + M42 * f.M23 + M43 * f.M33 + f.M43,

      Determinant * f.Determinant
      );
    }

    /// <summary>
    /// Appends a transformation matrix <paramref name="f"/> to this matrix, and returns a new matrix with the result. The original matrix is unchanged.
    /// </summary>
    /// <param name="f">The matrix to append.</param>
    /// <returns>A new matrix based on the existing one, but with matrix <paramref name="f"/> appended.</returns>
    public Matrix4x3 WithAppendedTransformation(Matrix3x3 f)
    {
      return new Matrix4x3(
       M11 * f.M11 + M12 * f.M21 + M13 * f.M31,
       M11 * f.M12 + M12 * f.M22 + M13 * f.M32,
       M11 * f.M13 + M12 * f.M23 + M13 * f.M33,

      M21 * f.M11 + M22 * f.M21 + M23 * f.M31,
      M21 * f.M12 + M22 * f.M22 + M23 * f.M32,
      M21 * f.M13 + M22 * f.M23 + M23 * f.M33,

      M31 * f.M11 + M32 * f.M21 + M33 * f.M31,
      M31 * f.M12 + M32 * f.M22 + M33 * f.M32,
      M31 * f.M13 + M32 * f.M23 + M33 * f.M33,

      M41 * f.M11 + M42 * f.M21 + M43 * f.M31,
      M41 * f.M12 + M42 * f.M22 + M43 * f.M32,
      M41 * f.M13 + M42 * f.M23 + M43 * f.M33,

      Determinant * f.Determinant
      );
    }

    /// <summary>
    /// Prepends a transformation matrix <paramref name="l"/> to this matrix, and returns a new matrix with the result. The original matrix is unchanged.
    /// </summary>
    /// <param name="l">The matrix to prepend.</param>
    /// <returns>A new matrix based on the existing one, but with matrix <paramref name="l"/> prepended.</returns>
    public Matrix4x3 WithPrependedTransformation(Matrix4x3 l)
    {
      return new Matrix4x3(
        l.M11 * M11 + l.M12 * M21 + l.M13 * M31,
        l.M11 * M12 + l.M12 * M22 + l.M13 * M32,
        l.M11 * M13 + l.M12 * M23 + l.M13 * M33,
        l.M21 * M11 + l.M22 * M21 + l.M23 * M31,
        l.M21 * M12 + l.M22 * M22 + l.M23 * M32,
        l.M21 * M13 + l.M22 * M23 + l.M23 * M33,
        l.M31 * M11 + l.M32 * M21 + l.M33 * M31,
        l.M31 * M12 + l.M32 * M22 + l.M33 * M32,
        l.M31 * M13 + l.M32 * M23 + l.M33 * M33,
        l.M41 * M11 + l.M42 * M21 + l.M43 * M31 + M41,
        l.M41 * M12 + l.M42 * M22 + l.M43 * M32 + M42,
        l.M41 * M13 + l.M42 * M23 + l.M43 * M33 + M43,
        l.Determinant * Determinant
        );
    }

    #endregion Creation of new matrices by prepending or appending other matrices

    #region Prepend transformations

    /// <summary>
    /// Prepends a translate transformation to this matrix.
    /// </summary>
    /// <param name="x">The x component of translation.</param>
    /// <param name="y">The y component of translation.</param>
    /// <param name="z">The z component of translation.</param>
    public void TranslatePrepend(double x, double y, double z)
    {
      M41 += M11 * x + M21 * y + M31 * z;
      M42 += M12 * x + M22 * y + M32 * z;
      M43 += M13 * x + M23 * y + M33 * z;
    }

    /// <summary>
    /// Appends a translate transformation to this matrix.
    /// </summary>
    /// <param name="x">The x component of translation.</param>
    /// <param name="y">The y component of translation.</param>
    /// <param name="z">The z component of translation.</param>
    public void TranslateAppend(double x, double y, double z)
    {
      M41 += x;
      M42 += y;
      M43 += z;
    }

    /// <summary>
    /// Prepends a rotation transformation around x axis. The angle is specified in degrees.
    /// </summary>
    /// <param name="angleX">The angle in degrees.</param>
    public void RotationXDegreePrepend(double angleX)
    {
      double phi;
      phi = angleX * Math.PI / 180;
      double cx = Math.Cos(phi);
      double sx = Math.Sin(phi);

      double h21 = M21, h22 = M22, h23 = M23;

      M21 = cx * M21 + M31 * sx;
      M22 = cx * M22 + M32 * sx;
      M23 = cx * M23 + M33 * sx;
      M31 = cx * M31 - h21 * sx;
      M32 = cx * M32 - h22 * sx;
      M33 = cx * M33 - h23 * sx;
    }

    /// <summary>
    /// Prepends a rotation transformation around y axis. The angle is specified in degrees.
    /// </summary>
    /// <param name="angleY">The angle in degrees.</param>
    public void RotationYDegreePrepend(double angleY)
    {
      double phi;
      phi = angleY * Math.PI / 180;
      double cy = Math.Cos(phi);
      double sy = Math.Sin(phi);

      double h11 = M11, h12 = M12, h13 = M13;

      M11 = cy * M11 + M31 * sy;
      M12 = cy * M12 + M32 * sy;
      M13 = cy * M13 + M33 * sy;
      M31 = cy * M31 - h11 * sy;
      M32 = cy * M32 - h12 * sy;
      M33 = cy * M33 - h13 * sy;
    }

    /// <summary>
    /// Prepends a rotation transformation around z axis. The angle is specified in degrees.
    /// </summary>
    /// <param name="angleZ">The angle in degrees.</param>
    public void RotationZDegreePrepend(double angleZ)
    {
      double phi;
      phi = angleZ * Math.PI / 180;
      double cz = Math.Cos(phi);
      double sz = Math.Sin(phi);

      double h11 = M11, h12 = M12, h13 = M13;

      M11 = cz * M11 + M21 * sz;
      M12 = cz * M12 + M22 * sz;
      M13 = cz * M13 + M23 * sz;
      M21 = cz * M21 - h11 * sz;
      M22 = cz * M22 - h12 * sz;
      M23 = cz * M23 - h13 * sz;
    }

    #endregion Prepend transformations

    #region Append transformations

    /// <summary>
    /// Appends a transformation matrix <paramref name="f"/> to this matrix.
    /// </summary>
    /// <param name="f">The matrix to append.</param>
    public void AppendTransform(Matrix4x3 f)
    {
      double h1, h2, h3;

      h1 = M11 * f.M11 + M12 * f.M21 + M13 * f.M31;
      h2 = M11 * f.M12 + M12 * f.M22 + M13 * f.M32;
      h3 = M11 * f.M13 + M12 * f.M23 + M13 * f.M33;
      M11 = h1;
      M12 = h2;
      M13 = h3;

      h1 = M21 * f.M11 + M22 * f.M21 + M23 * f.M31;
      h2 = M21 * f.M12 + M22 * f.M22 + M23 * f.M32;
      h3 = M21 * f.M13 + M22 * f.M23 + M23 * f.M33;
      M21 = h1;
      M22 = h2;
      M23 = h3;

      h1 = M31 * f.M11 + M32 * f.M21 + M33 * f.M31;
      h2 = M31 * f.M12 + M32 * f.M22 + M33 * f.M32;
      h3 = M31 * f.M13 + M32 * f.M23 + M33 * f.M33;
      M31 = h1;
      M32 = h2;
      M33 = h3;

      h1 = M41 * f.M11 + M42 * f.M21 + M43 * f.M31 + f.M41;
      h2 = M41 * f.M12 + M42 * f.M22 + M43 * f.M32 + f.M42;
      h3 = M41 * f.M13 + M42 * f.M23 + M43 * f.M33 + f.M43;
      M41 = h1;
      M42 = h2;
      M43 = h3;

      Determinant *= f.Determinant;
    }

    /// <summary>
    /// Prepends a transformation matrix <paramref name="a"/> to this matrix.
    /// </summary>
    /// <param name="a">The matrix to prepend.</param>
    public void PrependTransform(Matrix4x3 a)
    {
      double h1, h2, h3, h4;

      h1 = M11 * a.M11 + M21 * a.M12 + M31 * a.M13;
      h2 = M11 * a.M21 + M21 * a.M22 + M31 * a.M23;
      h3 = M11 * a.M31 + M21 * a.M32 + M31 * a.M33;
      h4 = M11 * a.M41 + M21 * a.M42 + M31 * a.M43;
      M11 = h1;
      M21 = h2;
      M31 = h3;
      M41 += h4;

      h1 = M12 * a.M11 + M22 * a.M12 + M32 * a.M13;
      h2 = M12 * a.M21 + M22 * a.M22 + M32 * a.M23;
      h3 = M12 * a.M31 + M22 * a.M32 + M32 * a.M33;
      h4 = M12 * a.M41 + M22 * a.M42 + M32 * a.M43;
      M12 = h1;
      M22 = h2;
      M32 = h3;
      M42 += h4;

      h1 = M13 * a.M11 + M23 * a.M12 + M33 * a.M13;
      h2 = M13 * a.M21 + M23 * a.M22 + M33 * a.M23;
      h3 = M13 * a.M31 + M23 * a.M32 + M33 * a.M33;
      h4 = M13 * a.M41 + M23 * a.M42 + M33 * a.M43;
      M13 = h1;
      M23 = h2;
      M33 = h3;
      M43 += h4;

      Determinant *= a.Determinant;
    }

    /// <summary>
    /// Prepends a transformation matrix <paramref name="a"/> to this matrix.
    /// </summary>
    /// <param name="a">The matrix to prepend.</param>
    public void PrependTransform(Matrix3x3 a)
    {
      double h1, h2, h3;

      h1 = M11 * a.M11 + M21 * a.M12 + M31 * a.M13;
      h2 = M11 * a.M21 + M21 * a.M22 + M31 * a.M23;
      h3 = M11 * a.M31 + M21 * a.M32 + M31 * a.M33;
      M11 = h1;
      M21 = h2;
      M31 = h3;

      h1 = M12 * a.M11 + M22 * a.M12 + M32 * a.M13;
      h2 = M12 * a.M21 + M22 * a.M22 + M32 * a.M23;
      h3 = M12 * a.M31 + M22 * a.M32 + M32 * a.M33;
      M12 = h1;
      M22 = h2;
      M32 = h3;

      h1 = M13 * a.M11 + M23 * a.M12 + M33 * a.M13;
      h2 = M13 * a.M21 + M23 * a.M22 + M33 * a.M23;
      h3 = M13 * a.M31 + M23 * a.M32 + M33 * a.M33;
      M13 = h1;
      M23 = h2;
      M33 = h3;

      Determinant *= a.Determinant;
    }

    #endregion Append transformations

    #region Conversion to other matrices

    /// <summary>
    /// Gets the transposed inverse matrix of the matrix3x3 part of this matrix. The returned matrix is usually used to transform the normals of objects that will be transformed by this matrix.
    /// </summary>
    /// <returns>Transposed inverse matrix of the matrix3x3 part of this matrix.</returns>
    public Matrix3x3 GetTransposedInverseMatrix3x3()
    {
      return new Matrix3x3(
        (M22 * M33 - M23 * M32) / Determinant,
        (M23 * M31 - M21 * M33) / Determinant,
        (M21 * M32 - M22 * M31) / Determinant,
        (M13 * M32 - M12 * M33) / Determinant,
        (M11 * M33 - M13 * M31) / Determinant,
        (M12 * M31 - M11 * M32) / Determinant,
        (M12 * M23 - M13 * M22) / Determinant,
        (M13 * M21 - M11 * M23) / Determinant,
        (M11 * M22 - M12 * M21) / Determinant
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
      stb.Append("; ");
      stb.AppendFormat("M13=");
      stb.Append(M13);
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
      stb.Append(M23);
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
      stb.Append(";");
      stb.Append("}");

      stb.Append("}");

      return stb.ToString();
    }
  }
}
