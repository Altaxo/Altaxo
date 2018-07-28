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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Altaxo.Graph.Gdi.LineCaps
{
  /// <summary>
  /// Draws a cap that is a open circle. The midpoint of the circle is the designated end of the line.
  /// </summary>
  public class TriangleOLineCap : LineCapExtension
  {
    public TriangleOLineCap()
    {
    }

    public TriangleOLineCap(double minimumAbsoluteSizePt, double minimumRelativeSize)
      : base(minimumAbsoluteSizePt, minimumRelativeSize)
    {
    }

    public override LineCapExtension Clone(double minimumAbsoluteSizePt, double minimumRelativeSize)
    {
      return new TriangleOLineCap(minimumAbsoluteSizePt, minimumRelativeSize);
    }

    public override string Name { get { return "TriangleO"; } }

    public override double DefaultMinimumAbsoluteSizePt { get { return 8; } }

    public override double DefaultMinimumRelativeSize { get { return 4; } }

    private CustomLineCap GetClone(Pen pen, float size)
    {
      float endPoint;

      endPoint = pen.Width == 0 ? 1 : size / (pen.Width);
      if (endPoint <= 0)
        endPoint = 1e-3f;

      float c = 0.8660254f; //  0.5 / Math.Tan(30°);

      float r = 1; // 0.5/Math.Sin(30°);

      GraphicsPath hPath = new GraphicsPath();
      // Create the outline for our custom end cap.
      hPath.AddPolygon(new PointF[]{
        new PointF(0,-endPoint*0.866f+r),
        new PointF(endPoint/2 -c,-0.5f),
        new PointF(-endPoint/2 + c, -0.5f),
      });
      CustomLineCap clone = new CustomLineCap(null, hPath, LineCap.Flat, endPoint * 0.866f - r); // we set the stroke path only
      clone.SetStrokeCaps(LineCap.Flat, LineCap.Flat);
      return clone;
    }

    public override void SetStartCap(Pen pen, float size)
    {
      pen.StartCap = LineCap.Custom;
      pen.CustomStartCap = GetClone(pen, size);
    }

    public override void SetEndCap(Pen pen, float size)
    {
      pen.EndCap = LineCap.Custom;
      pen.CustomEndCap = GetClone(pen, size);
    }
  }

  /// <summary>
  /// Draws a cap that is a open circle. The midpoint of the circle is the designated end of the line.
  /// </summary>
  public class TriangleFLineCap : LineCapExtension
  {
    public TriangleFLineCap()
    {
    }

    public TriangleFLineCap(double minimumAbsoluteSizePt, double minimumRelativeSize)
      : base(minimumAbsoluteSizePt, minimumRelativeSize)
    {
    }

    public override LineCapExtension Clone(double minimumAbsoluteSizePt, double minimumRelativeSize)
    {
      return new TriangleFLineCap(minimumAbsoluteSizePt, minimumRelativeSize);
    }

    public override string Name { get { return "TriangleF"; } }

    public override double DefaultMinimumAbsoluteSizePt { get { return 8; } }

    public override double DefaultMinimumRelativeSize { get { return 4; } }

    private CustomLineCap GetClone(Pen pen, float size)
    {
      float scale = pen.Width == 0 ? 1 : size / pen.Width;
      if (scale <= 0)
        scale = 1e-3f;

      GraphicsPath hPath = new GraphicsPath();
      // Create the outline for our custom end cap.
      // Create the outline for our custom end cap.
      hPath.AddPolygon(new PointF[]{
        new PointF(0,-0.866f),
        new PointF(0.5f,0),
        new PointF(-0.5f,0),
      });
      CustomLineCap clone = new CustomLineCap(hPath, null, LineCap.Flat, 0);
      clone.WidthScale = scale;
      return clone;
    }

    public override void SetStartCap(Pen pen, float size)
    {
      pen.StartCap = LineCap.Custom;
      pen.CustomStartCap = GetClone(pen, size);
    }

    public override void SetEndCap(Pen pen, float size)
    {
      pen.EndCap = LineCap.Custom;
      pen.CustomEndCap = GetClone(pen, size);
    }
  }
}
