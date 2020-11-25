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
  public struct Matrix3x3
  {
    /// <summary>Gets the matrix element M[1,1].</summary>
    public double M11 { get; private set; }

    /// <summary>Gets the matrix element M[1,2].</summary>
    public double M12 { get; private set; }

    /// <summary>Gets the matrix element M[1,3].</summary>
    public double M13 { get; private set; }

    /// <summary>Gets the matrix element M[2,1].</summary>
    public double M21 { get; private set; }

    /// <summary>Gets the matrix element M[2,2].</summary>
    public double M22 { get; private set; }

    /// <summary>Gets the matrix element M[2,2].</summary>
    public double M23 { get; private set; }

    /// <summary>Gets the matrix element M[3,1].</summary>
    public double M31 { get; private set; }

    /// <summary>Gets the matrix element M[3,1].</summary>
    public double M32 { get; private set; }

    /// <summary>Gets the matrix element M[3,3].</summary>
    public double M33 { get; private set; }

    /// <summary>The determinant of the matrix.</summary>
    public double Determinant { get; private set; }

    private static Matrix3x3 _identityMatrix;

    #region Constructors

    static Matrix3x3()
    {
      _identityMatrix = new Matrix3x3(
          1, 0, 0,
          0, 1, 0,
          0, 0, 1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix3x3"/> struct.
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
    public Matrix3x3(
    double m11, double m12, double m13,
    double m21, double m22, double m23,
    double m31, double m32, double m33)
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

      Determinant = -(m13 * m22 * m31) + m12 * m23 * m31 + m13 * m21 * m32 - m11 * m23 * m32 - m12 * m21 * m33 + m11 * m22 * m33;
    }

    /// <summary>
    /// Creates a transformation matrix that uses three basis vectors to construct the matrix that transform points expressed in the three basis vectors to points in
    /// the coordinate system.
    /// </summary>
    /// <param name="xBasis">Basis vector for the x-direction.</param>
    /// <param name="yBasis">Basis vector for the y-direction.</param>
    /// <param name="zBasis">Basis vector for the z-direction.</param>
    /// <returns>A transformation matrix that uses the three basis vectors, and a location</returns>
    public static Matrix3x3 NewFromBasisVectors(VectorD3D xBasis, VectorD3D yBasis, VectorD3D zBasis)
    {
      return new Matrix3x3(
        xBasis.X, xBasis.Y, xBasis.Z,
        yBasis.X, yBasis.Y, yBasis.Z,
        zBasis.X, zBasis.Y, zBasis.Z);
    }

    #endregion Constructors

    /// <summary>
    /// Gets the identity matrix.
    /// </summary>
    public static Matrix3x3 Identity
    {
      get
      {
        return _identityMatrix;
      }
    }

    /// <summary>
    /// Creates the rotation matrix from axis and angle radian.
    /// </summary>
    /// <param name="u">The axis about which the rotation takes place.</param>
    /// <param name="angleRadian">The rotation angle in radian.</param>
    /// <returns>Matrix that describes the drotation.</returns>
    public static Matrix3x3 CreateRotationMatrixFromAxisAndAngleRadian(VectorD3D u, double angleRadian)
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

      return new Matrix3x3(m11, m12, m13, m21, m22, m23, m31, m32, m33);
    }

    /// <summary>
    /// Transforms the specified vector <paramref name="v"/>.
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

    public PlaneD3D Transform(PlaneD3D o)
    {
      var x = o.X * (M22 * M33 - M23 * M32) + o.Y * (M13 * M32 - M12 * M33) + o.Z * (M12 * M23 - M13 * M22);
      var y = o.X * (M23 * M31 - M21 * M33) + o.Y * (M11 * M33 - M13 * M31) + o.Z * (M13 * M21 - M11 * M23);
      var z = o.X * (M21 * M32 - M22 * M31) + o.Y * (M12 * M31 - M11 * M32) + o.Z * (M11 * M22 - M12 * M21);

      var l = 1 / Math.Sqrt(x * x + y * y + z * z);
      x *= l;
      y *= l;
      z *= l;

      // Transform the point that was located on the original plane....
      var pp = Transform(new PointD3D(o.X * o.W, o.Y * o.W, o.Z * o.W));

      // but the transformed point is not neccessarly located from the origin in the direction of the new plane vector
      // thus we have to take the dot-product between the transformed normal and the transformed plane-point to get the new distance
      double w = x * pp.X + y * pp.Y + z * pp.Z;
      return new PlaneD3D(x, y, z, w);
    }

    public PointD3D TransformPoint(PointD3D p)
    {
      return Transform(p);
    }

    /// <summary>
    /// Transforms the specified point <paramref name="p"/>. Here, the point transform is carried out in the same way as the vector transform.
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
      x * M11 + y * M21 + z * M31,
      x * M12 + y * M22 + z * M32,
      x * M13 + y * M23 + z * M33
      );
    }

    /// <summary>
    /// Sets this transformation matrix by specifying translation, rotation, shear and scale.
    /// </summary>
    /// <param name="angleX">The rotation around x axis in degrees.</param>
    /// <param name="angleY">The rotation around y axis in degrees</param>
    /// <param name="angleZ">The rotation around z axis in degrees</param>
    /// <param name="shearX">The shear value x.</param>
    /// <param name="shearY">The shear value y.</param>
    /// <param name="shearZ">The shear value z.</param>
    /// <param name="scaleX">The scale value x.</param>
    /// <param name="scaleY">The scale value y.</param>
    /// <param name="scaleZ">The scale value z.</param>
    public void SetRotationShearScale(double angleX, double angleY, double angleZ, double shearX, double shearY, double shearZ, double scaleX, double scaleY, double scaleZ)
    {
      double phi;
      phi = angleX * Math.PI / 180;
      double cosX = Math.Cos(phi);
      double sinX = Math.Sin(phi);
      phi = angleY * Math.PI / 180;
      double cosY = Math.Cos(phi);
      double sinY = Math.Sin(phi);
      phi = angleZ * Math.PI / 180;
      double cosZ = Math.Cos(phi);
      double sinZ = Math.Sin(phi);

      M11 = scaleX * (cosY * cosZ - cosX * cosZ * shearY * sinY + shearY * sinX * sinZ);
      M12 = -(scaleX * (cosZ * shearY * sinX - cosY * sinZ + cosX * shearY * sinY * sinZ));
      M13 = scaleX * (cosX * cosY * shearY + sinY);
      M21 = scaleY * (cosY * cosZ * shearZ - cosZ * sinX * sinY - cosX * sinZ);
      M22 = scaleY * (cosX * cosZ + cosY * shearZ * sinZ - sinX * sinY * sinZ);
      M23 = scaleY * (cosY * sinX + shearZ * sinY);
      M31 = scaleZ * (cosY * cosZ * shearX * shearZ - cosZ * (cosX + shearX * sinX) * sinY + (-(cosX * shearX) + sinX) * sinZ);
      M32 = scaleZ * (cosY * shearX * shearZ * sinZ + cosX * (cosZ * shearX - sinY * sinZ) - sinX * (cosZ + shearX * sinY * sinZ));
      M33 = scaleZ * (cosX * cosY + cosY * shearX * sinX + shearX * shearZ * sinY);
      Determinant = scaleX * scaleY * scaleZ;
    }

    /// <summary>
    /// Gets a transformation matrix by specifying translation, rotation, shear and scale.
    /// </summary>
    /// <param name="angleX">The rotation around x axis in degrees.</param>
    /// <param name="angleY">The rotation around y axis in degrees</param>
    /// <param name="angleZ">The rotation around z axis in degrees</param>
    /// <param name="shearX">The shear value x.</param>
    /// <param name="shearY">The shear value y.</param>
    /// <param name="shearZ">The shear value z.</param>
    /// <param name="scaleX">The scale value x.</param>
    /// <param name="scaleY">The scale value y.</param>
    /// <param name="scaleZ">The scale value z.</param>
    /// <returns>The transformation matrix. A point transformed with this matrix is first translated, then rotated, then sheared, then scaled.</returns>
    public static Matrix3x3 FromTranslationRotationShearScale(double angleX, double angleY, double angleZ, double shearX, double shearY, double shearZ, double scaleX, double scaleY, double scaleZ)
    {
      var result = new Matrix3x3();
      result.SetRotationShearScale(angleX, angleY, angleZ, shearX, shearY, shearZ, scaleX, scaleY, scaleZ);
      return result;
    }

    /// <summary>
    /// Gets a transformation matrix by specifying rotation (translation is 0, shear is 0 and scale is 1).
    /// </summary>
    /// <param name="angleX">The rotation around x axis in degrees.</param>
    /// <param name="angleY">The rotation around y axis in degrees</param>
    /// <param name="angleZ">The rotation around z axis in degrees</param>
    /// <returns>The transformation matrix.</returns>
    public static Matrix3x3 FromRotation(double angleX, double angleY, double angleZ)
    {
      var result = new Matrix3x3();
      result.SetRotationShearScale(angleX, angleY, angleZ, 0, 0, 0, 1, 1, 1);
      return result;
    }

    #region Prepend transformations

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
    public void AppendTransform(Matrix3x3 f)
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

      Determinant *= f.Determinant;
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

    #region Inverse transformations

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

    #endregion Inverse transformations

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

      stb.Append("}");

      return stb.ToString();
    }
  }
}
