#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Geometry
{
  /// <summary>
  /// Represents a 2D transformation matrix.
  /// </summary>
  /// <remarks>
  /// The following transformation is represented by this matrix:
  /// <code>
  ///             |sx, ry, 0|
  /// |x, y, 1| * |rx, sy, 0| = |x', y', 1|
  ///             |dx, dy, 1|
  /// </code>
  /// where (x,y) are the world coordinates, and (x', y') are the page coordinates.
  /// <para>
  /// An alternative interpretation of this matrix is a rhombus,
  /// where the absolute coordinate of its origin is given by (dx, dy), and which is spanned by
  /// the two basis vectors (sx,ry) and (rx, sy).
  /// By inverse transformation of a given point one gets the coordinates inside this rhombus in terms of the spanning vectors.
  /// </para>
  /// </remarks>
  [Serializable]
  public class MatrixD2D : ICloneable
  {
    private double sx, ry, rx, sy, dx, dy, determinant;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixD2D"/> class.
    /// </summary>
    public MatrixD2D()
    {
      Reset();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixD2D"/> class with specified elements.
    /// </summary>
    /// <param name="sxf">Scale X.</param>
    /// <param name="ryf">Rotation Y.</param>
    /// <param name="rxf">Rotation X.</param>
    /// <param name="syf">Scale Y.</param>
    /// <param name="dxf">Translation X.</param>
    /// <param name="dyf">Translation Y.</param>
    public MatrixD2D(double sxf, double ryf, double rxf, double syf, double dxf, double dyf)
    {
      SetElements(sxf, ryf, rxf, syf, dxf, dyf);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixD2D"/> class by copying another matrix.
    /// </summary>
    /// <param name="from">The matrix to copy from.</param>
    public MatrixD2D(MatrixD2D from)
    {
      CopyFrom(from);
    }

    /// <summary>
    /// Copies the elements from another <see cref="MatrixD2D"/> instance.
    /// </summary>
    /// <param name="from">The matrix to copy from.</param>
    public void CopyFrom(MatrixD2D from)
    {
      if (ReferenceEquals(this, from))
        return;

      sx = from.sx;
      rx = from.rx;
      ry = from.ry;
      sy = from.sy;
      dx = from.dx;
      dy = from.dy;
      determinant = from.determinant;
    }

    /// <summary>
    /// Creates a shallow copy of this matrix.
    /// </summary>
    /// <returns>A shallow copy of this matrix.</returns>
    public MatrixD2D Clone()
    {
      return (MatrixD2D)MemberwiseClone();
    }

    /// <inheritdoc/>
    object ICloneable.Clone()
    {
      return MemberwiseClone();
    }

    /// <summary>
    /// Copies the elements from another object if it is a <see cref="MatrixD2D"/>.
    /// </summary>
    /// <param name="o">The object to copy from.</param>
    /// <returns>True if copy was successful, otherwise false.</returns>
    public bool CopyFrom(object o)
    {
      if (ReferenceEquals(this, o))
        return true;

      var from = o as MatrixD2D;
      if (from is not null)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Sets the elements of the matrix.
    /// </summary>
    /// <param name="sxf">Scale X.</param>
    /// <param name="ryf">Rotation Y.</param>
    /// <param name="rxf">Rotation X.</param>
    /// <param name="syf">Scale Y.</param>
    /// <param name="dxf">Translation X.</param>
    /// <param name="dyf">Translation Y.</param>
    public void SetElements(double sxf, double ryf, double rxf, double syf, double dxf, double dyf)
    {
      sx = sxf;
      ry = ryf;
      rx = rxf;
      sy = syf;
      dx = dxf;
      dy = dyf;
      determinant = sx * sy - rx * ry;
    }

    /// <summary>
    /// Resets the matrix to the identity transformation.
    /// </summary>
    public void Reset()
    {
      sx = 1;
      ry = 0;
      sy = 1;
      rx = 0;
      dx = 0;
      dy = 0;
      determinant = 1;
    }

    /// <summary>
    /// Sets the matrix using translation, rotation, shear, and scale values.
    /// </summary>
    /// <param name="dxf">Translation X.</param>
    /// <param name="dyf">Translation Y.</param>
    /// <param name="angle">Rotation angle in degrees.</param>
    /// <param name="shear">Shear value.</param>
    /// <param name="scaleX">Scale X.</param>
    /// <param name="scaleY">Scale Y.</param>
    public void SetTranslationRotationShearxScale(double dxf, double dyf, double angle, double shear, double scaleX, double scaleY)
    {
      double w = angle * Math.PI / 180;
      double c = Math.Cos(w);
      double s = Math.Sin(w);

      sx = c * scaleX;
      ry = s * scaleX;
      rx = c * scaleY * shear - s * scaleY;
      sy = s * scaleY * shear + c * scaleY;
      dx = dxf;
      dy = dyf;

      determinant = scaleX * scaleY;
    }

    /// <summary>
    /// Appends a translation to the matrix.
    /// </summary>
    /// <param name="x">Translation X.</param>
    /// <param name="y">Translation Y.</param>
    public void TranslateAppend(double x, double y)
    {
      dx += x;
      dy += y;
    }

    /// <summary>
    /// Prepends a translation to the matrix.
    /// </summary>
    /// <param name="x">Translation X.</param>
    /// <param name="y">Translation Y.</param>
    public void TranslatePrepend(double x, double y)
    {
      dx += x * sx + y * rx;
      dy += x * ry + y * sy;
    }

    /// <summary>
    /// Appends a scale transformation to the matrix.
    /// </summary>
    /// <param name="x">Scale X.</param>
    /// <param name="y">Scale Y.</param>
    public void ScaleAppend(double x, double y)
    {
      sx *= x;
      rx *= x;
      dx *= x;
      ry *= y;
      sy *= y;
      dy *= y;
      determinant *= (x * y);
    }

    /// <summary>
    /// Prepends a scale transformation to the matrix.
    /// </summary>
    /// <param name="x">Scale X.</param>
    /// <param name="y">Scale Y.</param>
    public void ScalePrepend(double x, double y)
    {
      sx *= x;
      rx *= y;
      ry *= x;
      sy *= y;
      determinant *= (x * y);
    }

    /// <summary>
    /// Appends a shear transformation to the matrix.
    /// </summary>
    /// <param name="x">Shear X.</param>
    /// <param name="y">Shear Y.</param>
    public void ShearAppend(double x, double y)
    {
      double h1;
      h1 = sx + x * ry;
      ry += y * sx;
      sx = h1;

      h1 = rx + x * sy;
      sy += y * rx;
      rx = h1;

      h1 = dx + x * dy;
      dy += y * dx;
      dx = h1;

      if (0 != y && 0 != x)
        determinant *= (1 - x * y);
    }

    /// <summary>
    /// Prepends a shear transformation to the matrix.
    /// </summary>
    /// <param name="x">Shear X.</param>
    /// <param name="y">Shear Y.</param>
    public void ShearPrepend(double x, double y)
    {
      double h1;
      h1 = sx + y * rx;
      rx += x * sx;
      sx = h1;

      h1 = ry + y * sy;
      sy += x * ry;
      ry = h1;

      if (0 != y && 0 != x)
        determinant *= (1 - x * y);
    }

    /// <summary>
    /// Appends a rotation transformation to the matrix.
    /// </summary>
    /// <param name="w">Rotation angle in degrees.</param>
    public void RotateAppend(double w)
    {
      w *= Math.PI / 180;
      double c = Math.Cos(w);
      double s = Math.Sin(w);
      double h1, h2;
      h1 = sx * c - ry * s;
      h2 = ry * c + sx * s;
      sx = h1;
      ry = h2;

      h1 = rx * c - sy * s;
      h2 = sy * c + rx * s;
      rx = h1;
      sy = h2;

      h1 = dx * c - dy * s;
      h2 = dy * c + dx * s;
      dx = h1;
      dy = h2;
    }

    /// <summary>
    /// Prepends a rotation transformation to the matrix.
    /// </summary>
    /// <param name="w">Rotation angle in degrees.</param>
    public void RotatePrepend(double w)
    {
      w *= Math.PI / 180;
      double c = Math.Cos(w);
      double s = Math.Sin(w);
      double h1, h2;
      h1 = sx * c + rx * s;
      h2 = rx * c - sx * s;
      sx = h1;
      rx = h2;

      h1 = ry * c + sy * s;
      h2 = sy * c - ry * s;
      ry = h1;
      sy = h2;
    }

    /// <summary>
    /// Appends a transformation to the matrix using specified elements.
    /// </summary>
    /// <param name="sxf">Scale X.</param>
    /// <param name="ryf">Rotation Y.</param>
    /// <param name="rxf">Rotation X.</param>
    /// <param name="syf">Scale Y.</param>
    /// <param name="dxf">Translation X.</param>
    /// <param name="dyf">Translation Y.</param>
    public void AppendTransform(double sxf, double ryf, double rxf, double syf, double dxf, double dyf)
    {
      double h1, h2;

      h1 = sx * sxf + ry * rxf;
      h2 = sx * ryf + ry * syf;
      sx = h1;
      ry = h2;

      h1 = rx * sxf + sy * rxf;
      h2 = rx * ryf + sy * syf;
      rx = h1;
      sy = h2;

      h1 = dx * sxf + dy * rxf + dxf;
      h2 = dx * ryf + dy * syf + dyf;
      dx = h1;
      dy = h2;

      determinant *= (sxf * syf - rxf * ryf);
    }

    /// <summary>
    /// Appends a transformation to the matrix using another <see cref="MatrixD2D"/>.
    /// </summary>
    /// <param name="t">The matrix to append.</param>
    public void AppendTransform(MatrixD2D t)
    {
      AppendTransform(t.sx, t.ry, t.rx, t.sy, t.dx, t.dy);
    }

    /// <summary>
    /// Prepends a transformation to the matrix using another <see cref="MatrixD2D"/>.
    /// </summary>
    /// <param name="t">The matrix to prepend.</param>
    public void PrependTransform(MatrixD2D t)
    {
      PrependTransform(t.sx, t.ry, t.rx, t.sy, t.dx, t.dy);
    }

    /// <summary>
    /// Prepends the inverse of a transformation to the matrix.
    /// </summary>
    /// <param name="t">The matrix whose inverse to prepend.</param>
    public void PrependInverseTransform(MatrixD2D t)
    {
      PrependTransform(t.sy / t.determinant, -t.ry / t.determinant, -t.rx / t.determinant, t.sx / t.determinant, (t.dy * t.rx - t.dx * t.sy) / t.determinant, (t.dx * t.ry - t.dy * t.sx) / t.determinant);
    }

    /// <summary>
    /// Appends the inverse of a transformation to the matrix.
    /// </summary>
    /// <param name="t">The matrix whose inverse to append.</param>
    public void AppendInverseTransform(MatrixD2D t)
    {
      AppendTransform(t.sy / t.determinant, -t.ry / t.determinant, -t.rx / t.determinant, t.sx / t.determinant, (t.dy * t.rx - t.dx * t.sy) / t.determinant, (t.dx * t.ry - t.dy * t.sx) / t.determinant);
    }

    /// <summary>
    /// Prepends a transformation to the matrix using specified elements.
    /// </summary>
    /// <param name="sxf">Scale X.</param>
    /// <param name="ryf">Rotation Y.</param>
    /// <param name="rxf">Rotation X.</param>
    /// <param name="syf">Scale Y.</param>
    /// <param name="dxf">Translation X.</param>
    /// <param name="dyf">Translation Y.</param>
    public void PrependTransform(double sxf, double ryf, double rxf, double syf, double dxf, double dyf)
    {
      double h1, h2;

      dx += sx * dxf + rx * dyf;
      dy += ry * dxf + sy * dyf;

      h1 = sx * sxf + rx * ryf;
      h2 = sx * rxf + rx * syf;
      sx = h1;
      rx = h2;

      h1 = ry * sxf + sy * ryf;
      h2 = ry * rxf + sy * syf;
      ry = h1;
      sy = h2;

      determinant *= (sxf * syf - rxf * ryf);
    }

    /// <summary>
    /// Gets the elements of the matrix as an array.
    /// </summary>
    public double[] Elements
    {
      get
      {
        return new double[] { sx, ry, rx, sy, dx, dy };
      }
    }

    /// <summary>Gets the scale X element.</summary>
    public double SX { get { return sx; } }
    /// <summary>Gets the rotation X element.</summary>
    public double RX { get { return rx; } }
    /// <summary>Gets the rotation Y element.</summary>
    public double RY { get { return ry; } }
    /// <summary>Gets the scale Y element.</summary>
    public double SY { get { return sy; } }
    /// <summary>Gets the translation X element.</summary>
    public double DX { get { return dx; } }
    /// <summary>Gets the translation Y element.</summary>
    public double DY { get { return dy; } }
    /// <summary>Gets the determinant of the matrix.</summary>
    public double Determinant => determinant;

    /// <summary>
    /// Gets the scale X value.
    /// </summary>
    public double ScaleX
    {
      get
      {
        return Math.Sqrt(sx * sx + ry * ry);
      }
    }

    /// <summary>
    /// Gets the rotation angle in degrees.
    /// </summary>
    public double Rotation
    {
      get
      {
        return 180 * Math.Atan2(ry, sx) / Math.PI;
      }
    }

    /// <summary>
    /// Gets the shear value.
    /// </summary>
    public double Shear
    {
      get
      {
        return (rx * sx + sy * ry) / (sx * sy - rx * ry);
      }
    }

    /// <summary>
    /// Gets the scale Y value.
    /// </summary>
    public double ScaleY
    {
      get
      {
        return (sy * sx - rx * ry) / Math.Sqrt(sx * sx + ry * ry);
      }
    }

    /// <summary>
    /// Gets the translation X value.
    /// </summary>
    public double X
    {
      get
      {
        return dx;
      }
    }

    /// <summary>
    /// Gets the translation Y value.
    /// </summary>
    public double Y
    {
      get
      {
        return dy;
      }
    }

    /// <summary>
    /// Transforms a point using this matrix.
    /// </summary>
    /// <param name="x">The x coordinate (input and output).</param>
    /// <param name="y">The y coordinate (input and output).</param>
    public void TransformPoint(ref double x, ref double y)
    {
      double xh = x * sx + y * rx + dx;
      double yh = x * ry + y * sy + dy;
      x = xh;
      y = yh;
    }

    /// <summary>
    /// Transforms a point using this matrix.
    /// </summary>
    /// <param name="pt">The point to transform.</param>
    /// <returns>The transformed point.</returns>
    public PointD2D TransformPoint(PointD2D pt)
    {
      return new PointD2D(pt.X * sx + pt.Y * rx + dx, pt.X * ry + pt.Y * sy + dy);
    }

    /// <summary>
    /// Transforms a vector using this matrix (ignores translation).
    /// </summary>
    /// <param name="x">The x component (input and output).</param>
    /// <param name="y">The y component (input and output).</param>
    public void TransformVector(ref double x, ref double y)
    {
      double xh = x * sx + y * rx;
      double yh = x * ry + y * sy;
      x = xh;
      y = yh;
    }

    /// <summary>
    /// Transforms a vector using this matrix (ignores translation).
    /// </summary>
    /// <param name="pt">The vector to transform.</param>
    /// <returns>The transformed vector.</returns>
    public PointD2D TransformVector(PointD2D pt)
    {
      return new PointD2D(pt.X * sx + pt.Y * rx, pt.X * ry + pt.Y * sy);
    }



    /// <summary>
    /// Inverse transforms a point using this matrix.
    /// </summary>
    /// <param name="x">The x coordinate (input and output).</param>
    /// <param name="y">The y coordinate (input and output).</param>
    public void InverseTransformPoint(ref double x, ref double y)
    {
      double xh = (x - dx) * sy + (dy - y) * rx;
      double yh = (dx - x) * ry + (y - dy) * sx;
      x = xh / determinant;
      y = yh / determinant;
    }

    /// <summary>
    /// Inverse transforms a vector using this matrix.
    /// </summary>
    /// <param name="x">The x component (input and output).</param>
    /// <param name="y">The y component (input and output).</param>
    public void InverseTransformVector(ref double x, ref double y)
    {
      double xh = (x) * sy + (-y) * rx;
      double yh = (-x) * ry + (y) * sx;
      x = xh / determinant;
      y = yh / determinant;
    }

    /// <summary>
    /// Inverse transforms a point using this matrix.
    /// </summary>
    /// <param name="pt">The point to inverse transform.</param>
    /// <returns>The inverse transformed point.</returns>
    public PointD2D InverseTransformPoint(PointD2D pt)
    {
      return new PointD2D(((pt.X - dx) * sy + (dy - pt.Y) * rx) / determinant, ((dx - pt.X) * ry + (pt.Y - dy) * sx) / determinant);
    }

    /// <summary>
    /// Gets the inverse of the matrix.
    /// </summary>
    /// <returns>The inverse matrix.</returns>
    public MatrixD2D Inverse()
    {
      return new MatrixD2D(
        sy / determinant,
        -ry / determinant,
        -rx / determinant,
        sx / determinant,
        (dy * rx - dx * sy) / determinant,
        (dx * ry - dy * sy) / determinant
        )
      { determinant = 1 / determinant };
    }

    /// <summary>
    /// Inverse transforms a vector using this matrix.
    /// </summary>
    /// <param name="pt">The vector to inverse transform.</param>
    /// <returns>The inverse transformed vector.</returns>
    public PointD2D InverseTransformVector(PointD2D pt)
    {
      return new PointD2D(((pt.X) * sy + (-pt.Y) * rx) / determinant, ((-pt.X) * ry + (pt.Y) * sx) / determinant);
    }


  }
}
