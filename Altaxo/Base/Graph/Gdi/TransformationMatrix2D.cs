using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace Altaxo.Graph.Gdi
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
  public class TransformationMatrix2D : ICloneable
  {
    double sx, ry, rx, sy, dx, dy, determinant;

    public TransformationMatrix2D()
    {
      Reset();
    }

    public TransformationMatrix2D(double sxf, double ryf, double rxf, double syf, double dxf, double dyf)
    {
      SetElements(sxf, ryf, rxf, syf, dxf, dyf);
    }

    public TransformationMatrix2D(TransformationMatrix2D from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(TransformationMatrix2D from)
    {
			if (object.ReferenceEquals(this, from))
				return;

      this.sx = from.sx;
      this.rx = from.rx;
      this.ry = from.ry;
      this.sy = from.sy;
      this.dx = from.dx;
      this.dy = from.dy;
      this.determinant = from.determinant;
    }

		public TransformationMatrix2D Clone()
		{
			return (TransformationMatrix2D)MemberwiseClone();
		}

		object ICloneable.Clone()
		{
			return MemberwiseClone();
		}

    public bool CopyFrom(object o)
    {
			if (object.ReferenceEquals(this, o))
				return true;

      var from = o as TransformationMatrix2D;
      if (null != from)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }


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

    public void Reset()
    {
      sx = 1; ry = 0; sy = 1; rx = 0; dx = 0; dy = 0; determinant = 1;
    }

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


    public void TranslateAppend(double x, double y)
    {
      dx += x;
      dy += y;
    }
    public void TranslatePrepend(double x, double y)
    {
      dx += x * sx + y * rx;
      dy += x * ry + y * sy;
    }
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
    public void ScalePrepend(double x, double y)
    {
      sx *= x;
      rx *= y;
      ry *= x;
      sy *= y;
      determinant *= (x * y);
    }
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

    public void RotateAppend(double w)
    {
      w *= Math.PI / 180;
      double c = Math.Cos(w);
      double s = Math.Sin(w);
      double h1, h2;
      h1 = sx * c - ry * s;
      h2 = ry * c + sx * s;
      sx = h1; ry = h2;

      h1 = rx * c - sy * s;
      h2 = sy * c + rx * s;
      rx = h1; sy = h2;

      h1 = dx * c - dy * s;
      h2 = dy * c + dx * s;
      dx = h1; dy = h2;
    }
    public void RotatePrepend(double w)
    {
      w *= Math.PI / 180;
      double c = Math.Cos(w);
      double s = Math.Sin(w);
      double h1, h2;
      h1 = sx * c + rx * s;
      h2 = rx * c - sx * s;
      sx = h1; rx = h2;

      h1 = ry * c + sy * s;
      h2 = sy * c - ry * s;
      ry = h1; sy = h2;
    }

    public void AppendTransform(double sxf, double ryf, double rxf, double syf, double dxf, double dyf)
    {
      double h1, h2;

      h1 = sx * sxf + ry * rxf;
      h2 = sx * ryf + ry * syf;
      sx = h1; ry = h2;

      h1 = rx * sxf + sy * rxf;
      h2 = rx * ryf + sy * syf;
      rx = h1; sy = h2;

      h1 = dx * sxf + dy * rxf + dxf;
      h2 = dx * ryf + dy * syf + dyf;
      dx = h1;
      dy = h2;

      determinant *= (sxf * syf - rxf * ryf);
    }
		public void AppendTransform(TransformationMatrix2D t)
		{
			AppendTransform(t.sx, t.ry, t.rx, t.sy, t.dx, t.dy);
		}

		public void PrependTransform(TransformationMatrix2D t)
		{
			PrependTransform(t.sx, t.ry, t.rx, t.sy, t.dx, t.dy);
		}

    public void PrependInverseTransform(TransformationMatrix2D t)
    {
      PrependTransform(t.sy / t.determinant, -t.ry / t.determinant, -t.rx / t.determinant, t.sx / t.determinant, (t.dy * t.rx - t.dx * t.sy) / t.determinant, (t.dx * t.ry - t.dy * t.sx) / t.determinant);
    }

    public void AppendInverseTransform(TransformationMatrix2D t)
    {
      AppendTransform(t.sy / t.determinant, -t.ry / t.determinant, -t.rx / t.determinant, t.sx / t.determinant, (t.dy * t.rx - t.dx * t.sy) / t.determinant, (t.dx * t.ry - t.dy * t.sx) / t.determinant);
    }

    public void PrependTransform(System.Drawing.Drawing2D.Matrix t)
    {
      var e = t.Elements;
      PrependTransform(e[0], e[1], e[2], e[3], e[4], e[5]);
    }
    public void PrependTransform(double sxf, double ryf, double rxf, double syf, double dxf, double dyf)
    {
      double h1, h2;

      dx += sx * dxf + rx * dyf;
      dy += ry * dxf + sy * dyf;

      h1 = sx * sxf + rx * ryf;
      h2 = sx * rxf + rx * syf;
      sx = h1; rx = h2;

      h1 = ry * sxf + sy * ryf;
      h2 = ry * rxf + sy * syf;
      ry = h1; sy = h2;

      determinant *= (sxf * syf - rxf * ryf);
    }

    public double[] Elements
    {
      get
      {
        return new double[] { sx, ry, rx, sy, dx, dy };
      }
    }

    public double ScaleX
    {
      get
      {
        return Math.Sqrt(sx * sx + ry * ry);
      }
    }
    public double Rotation
    {
      get
      {
        return 180 * Math.Atan2(ry, sx) / Math.PI;
      }
    }

    public double Shear
    {
      get
      {
        return (rx * sx + sy * ry) / (sx * sy - rx * ry);
      }
    }
    public double ScaleY
    {
      get
      {
        return (sy * sx - rx * ry) / Math.Sqrt(sx * sx + ry * ry);
      }
    }
    public double X
    {
      get
      {
        return dx;
      }
    }
    public double Y
    {
      get
      {
        return dy;
      }
    }

    public void TransformPoint(ref double x, ref double y)
    {
      double xh = x * sx + y * rx + dx;
      double yh = x * ry + y * sy + dy;
      x = xh; y = yh;
    }

    public PointD2D TransformPoint(PointD2D pt)
    {
      return new PointD2D(pt.X * sx + pt.Y * rx + dx, pt.X * ry + pt.Y * sy + dy);
    }

    public PointF TransformPoint(PointF pt)
    {
      return new PointF((float)(pt.X * sx + pt.Y * rx + dx), (float)(pt.X * ry + pt.Y * sy + dy));
    }

		public void TransformPoints(PointF[] pts)
		{
			for (int i = 0; i < pts.Length; i++)
			{
				pts[i] = new PointF((float)(pts[i].X * sx + pts[i].Y * rx + dx), (float)(pts[i].X * ry + pts[i].Y * sy + dy));
			}
		}

		public void TransformVector(ref double x, ref double y)
		{
			double xh = x * sx + y * rx;
			double yh = x * ry + y * sy;
			x = xh; y = yh;
		}

    public PointD2D TransformVector(PointD2D pt)
    {
      return new PointD2D(pt.X * sx + pt.Y * rx, pt.X * ry + pt.Y * sy);
    }

		public PointF TransformVector(PointF pt)
		{
			return new PointF((float)(pt.X * sx + pt.Y * rx), (float)(pt.X * ry + pt.Y * sy));
		}


		public void TransformPath(System.Drawing.Drawing2D.GraphicsPath path)
		{
			TransformPoints(path.PathPoints);
		}


    public void InverseTransformPoint(ref double x, ref double y)
    {
      double xh = (x - dx) * sy + (dy - y) * rx;
      double yh = (dx - x) * ry + (y - dy) * sx;
      x = xh / determinant;
      y = yh / determinant;
    }

    public void InverseTransformVector(ref double x, ref double y)
    {
      double xh = (x ) * sy + (-y) * rx;
      double yh = (-x) * ry + (y) * sx;
      x = xh / determinant;
      y = yh / determinant;
    }

    public PointD2D InverseTransformPoint(PointD2D pt)
    {
      return new PointD2D(((pt.X - dx) * sy + (dy - pt.Y) * rx) / determinant, ((dx - pt.X) * ry + (pt.Y - dy) * sx) / determinant);
    }

    public PointF InverseTransformPoint(PointF pt)
    {
      return new PointF((float)(((pt.X - dx) * sy + (dy - pt.Y) * rx) / determinant), (float)(((dx - pt.X) * ry + (pt.Y - dy) * sx) / determinant));
    }

    public PointD2D InverseTransformVector(PointD2D pt)
    {
      return new PointD2D(((pt.X) * sy + (-pt.Y) * rx) / determinant, ((-pt.X) * ry + (pt.Y) * sx) / determinant);
    }

    public PointF InverseTransformVector(PointF pt)
		{
      return new PointF((float)(((pt.X ) * sy + (-pt.Y) * rx) / determinant), (float)(((-pt.X) * ry + (pt.Y) * sx) / determinant));
    }

    public static implicit operator System.Drawing.Drawing2D.Matrix(TransformationMatrix2D m)
    {
      return new System.Drawing.Drawing2D.Matrix((float)m.sx, (float)m.ry, (float)m.rx, (float)m.sy, (float)m.dx, (float)m.dy);
    }
  }
}
