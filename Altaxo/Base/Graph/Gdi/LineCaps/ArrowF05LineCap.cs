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

namespace Altaxo.Graph.Gdi.LineCaps.Foo
{
  public class ArrowF05LineCap : LineCapExtension
  {
    public ArrowF05LineCap()
    {
    }

    public ArrowF05LineCap(double minimumAbsoluteSizePt, double minimumRelativeSize)
      : base(minimumAbsoluteSizePt, minimumRelativeSize)
    {
    }

    public override LineCapExtension Clone(double minimumAbsoluteSizePt, double minimumRelativeSize)
    {
      return new ArrowF05LineCap(minimumAbsoluteSizePt, minimumRelativeSize);
    }

    public override string Name { get { return "ArrowF05"; } }

    public override double DefaultMinimumAbsoluteSizePt { get { return 8; } }

    public override double DefaultMinimumRelativeSize { get { return 4; } }

    private CustomLineCap GetClone(Pen pen, float size)
    {
      float scale = pen.Width == 0 ? 1 : size / (2 * pen.Width);
      if (scale <= 0)
        scale = 1e-3f;

      var hPath = new GraphicsPath();
      hPath.AddPolygon(new PointF[]{
      new PointF(0, 0),
      new PointF(-1, -1),
      new PointF(1, -1),
    });

      // Construct the hook-shaped end cap.
      var clone = new CustomLineCap(hPath, null, LineCap.Flat, 1)
      {
        WidthScale = scale
      };
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
