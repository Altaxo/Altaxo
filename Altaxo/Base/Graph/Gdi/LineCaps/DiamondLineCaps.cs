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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Altaxo.Graph.Gdi.LineCaps
{
  /// <summary>
  /// Draws a cap that is a open circle. The midpoint of the circle is the designated end of the line.
  /// </summary>
  public class DiamondOLineCap : GdiLineCapBase
  {
    public override Type ExtendsType => typeof(Altaxo.Drawing.LineCaps.DiamondOLineCap);


    protected override CustomLineCap GetCustomLineCap(Pen pen, float size, bool isEndCap)
    {
      float endPoint;

      endPoint = pen.Width == 0 ? 1 : size / (pen.Width * 2) - 0.70710678118654752440084436210485f;

      if (endPoint < 0)
        endPoint = 1e-3f * pen.Width;

      var hPath = new GraphicsPath();
      // Create the outline for our custom end cap.
      hPath.AddPolygon(new PointF[]{
        new PointF(0,-endPoint),
        new PointF(endPoint,0),
        new PointF(0, endPoint),
        new PointF(-endPoint,0),
      });
      var clone = new CustomLineCap(null, hPath, LineCap.Flat, endPoint); // we set the stroke path only
      clone.SetStrokeCaps(LineCap.Flat, LineCap.Flat);
      return clone;
    }


  }

  /// <summary>
  /// Draws a cap that is a open circle. The midpoint of the circle is the designated end of the line.
  /// </summary>
  public class DiamondFLineCap : GdiLineCapBase
  {
    public override Type ExtendsType => typeof(Altaxo.Drawing.LineCaps.DiamondFLineCap);


    protected override CustomLineCap GetCustomLineCap(Pen pen, float size, bool isEndCap)
    {
      float scale = pen.Width == 0 ? 1 : size / (pen.Width * 2);
      if (scale <= 0)
        scale = 1e-3f;

      var hPath = new GraphicsPath();
      // Create the outline for our custom end cap.
      // Create the outline for our custom end cap.
      hPath.AddPolygon(new PointF[]{
         new PointF(0,-1),
          new PointF(1,0),
          new PointF(0, 1),
        new PointF(-1,0)
      });
      var clone = new CustomLineCap(hPath, null, LineCap.Flat, 0)
      {
        WidthScale = scale
      }; // we set the stroke path only
      return clone;
    }


  }
}
