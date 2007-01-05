#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.LineCaps
{
  public class ArrowF10LineCap : LineCapExtension
  {
    CustomLineCap _cap;
    const float _designWidth = 2;

    public ArrowF10LineCap()
    {
      GraphicsPath hPath = new GraphicsPath();

      // Create the outline for our custom end cap.
      hPath.AddLine(new PointF(0, -_designWidth), new PointF(-_designWidth / 2, -_designWidth));
      hPath.AddLine(new PointF(-_designWidth / 2, -_designWidth), new PointF(0, 0));
      hPath.AddLine(new PointF(0, 0), new PointF(_designWidth / 2, -_designWidth));
      hPath.AddLine(new PointF(_designWidth / 2, -_designWidth), new PointF(0, -_designWidth));

      // Construct the hook-shaped end cap.
      _cap = new CustomLineCap(hPath, null);
      _cap.BaseInset = _designWidth;
    }

    public override string Name { get { return "ArrowF10"; } }
    public override float DefaultSize { get { return 8; } }

    CustomLineCap GetClone(Pen pen, float size)
    {
      CustomLineCap clone = (CustomLineCap)_cap.Clone();
      if (pen.Width * _designWidth < size)
        clone.WidthScale = pen.Width == 0 ? 1 : size / (pen.Width * _designWidth);
      else
        clone.WidthScale = 1;

      //clone.WidthScale = 1;
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
