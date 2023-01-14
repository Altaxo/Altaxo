#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

namespace Altaxo.Geometry
{
  public static class GeometryToSystemDrawingConversions
  {
    public static System.Drawing.PointF ToGdi(this PointD2D pt) => new System.Drawing.PointF((float)pt.X, (float)pt.Y);

    public static System.Drawing.SizeF ToGdiSize(this PointD2D pt) => new System.Drawing.SizeF((float)pt.X, (float)pt.Y);

    public static PointD2D FromGdi(this System.Drawing.PointF pt) => new PointD2D(pt.X, pt.Y);

    public static PointD2D ToPointD2D(this System.Drawing.SizeF pt) => new PointD2D(pt.Width, pt.Height);
    public static PointD2D ToPointD2D(this System.Drawing.PointF pt) => new PointD2D(pt.X, pt.Y);

    #region Rectangle

    public static System.Drawing.RectangleF ToGdi(this RectangleD2D r)
    {
      return new System.Drawing.RectangleF((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);
    }

    public static System.Drawing.Rectangle ToGdiRectangle(this RectangleD2D r)
    {
      return new System.Drawing.Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
    }

    public static RectangleD2D ToAxo(this System.Drawing.RectangleF r)
    {
      return new RectangleD2D(r.X, r.Y, r.Width, r.Height);
    }

    public static RectangleD2D ToAxo(this System.Drawing.Rectangle r)
    {
      return new RectangleD2D(r.X, r.Y, r.Width, r.Height);
    }

    #endregion

    #region MatrixD2D transformations

    public static System.Drawing.PointF TransformPoint(this MatrixD2D m, System.Drawing.PointF pt)
    {
      return new System.Drawing.PointF((float)(pt.X * m.SX + pt.Y * m.RX + m.DX), (float)(pt.X * m.RY + pt.Y * m.SY + m.DY));
    }

    public static void TransformPoints(this MatrixD2D m, System.Drawing.PointF[] pts)
    {
      for (int i = 0; i < pts.Length; i++)
      {
        pts[i] = new System.Drawing.PointF((float)(pts[i].X * m.SX + pts[i].Y * m.RX + m.DX), (float)(pts[i].X * m.RY + pts[i].Y * m.SY + m.DY));
      }
    }

    public static System.Drawing.PointF TransformVector(this MatrixD2D m, System.Drawing.PointF pt)
    {
      return new System.Drawing.PointF((float)(pt.X * m.SX + pt.Y * m.RX), (float)(pt.X * m.RY + pt.Y * m.SY));
    }

    public static System.Drawing.PointF InverseTransformPoint(this MatrixD2D m, System.Drawing.PointF pt)
    {
      return new System.Drawing.PointF((float)(((pt.X - m.DX) * m.SY + (m.DY - pt.Y) * m.RX) / m.Determinant), (float)(((m.DX - pt.X) * m.RY + (pt.Y - m.DY) * m.SX) / m.Determinant));
    }

    public static System.Drawing.PointF InverseTransformVector(this MatrixD2D m, System.Drawing.PointF pt)
    {
      return new System.Drawing.PointF((float)(((pt.X) * m.SY + (-pt.Y) * m.RX) / m.Determinant), (float)(((-pt.X) * m.RY + (pt.Y) * m.SX) / m.Determinant));
    }

    public static void PrependTransform(this MatrixD2D m, System.Drawing.Drawing2D.Matrix t)
    {
      var e = t.Elements;
      m.PrependTransform(e[0], e[1], e[2], e[3], e[4], e[5]);
    }

    public static System.Drawing.Drawing2D.Matrix ToGdi(this MatrixD2D m)
    {
      return new System.Drawing.Drawing2D.Matrix((float)m.SX, (float)m.RY, (float)m.RX, (float)m.SY, (float)m.DX, (float)m.DY);
    }

    public static void TransformPath(this MatrixD2D m, System.Drawing.Drawing2D.GraphicsPath path)
    {
      path.Transform(m.ToGdi());
    }

    #endregion



  }
}
