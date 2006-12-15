using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.LineCaps
{
  /// <summary>
  /// Draws a cap that is a line perpendicular to the end of the line, and on the right side of the line.
  /// </summary>
  public class RightBarLineCap : LineCapExtension
  {
    const float _designWidth = 2.5f;

    public override string Name { get { return "BarRight"; } }
    public override float DefaultSize { get { return 8; } }

    protected CustomLineCap GetClone(Pen pen, float size, bool startCap)
    {
      float endPoint;
      
      if (pen.Width * _designWidth < size)
        endPoint = pen.Width == 0 ? 1 : size / pen.Width;
      else
        endPoint = _designWidth;

      if (startCap)
        endPoint = -endPoint;

      GraphicsPath hPath = new GraphicsPath();
      // Create the outline for our custom end cap.
      hPath.AddLine(new PointF(endPoint<0 ? 0.5f : -0.5f, 0), new PointF(endPoint/2, 0));
      CustomLineCap clone = new CustomLineCap(null, hPath); // we set the stroke path only
      clone.SetStrokeCaps(LineCap.Flat, LineCap.Flat);
      return clone;
    }

    public override void SetStartCap(Pen pen, float size)
    {
      pen.StartCap = LineCap.Custom;
      pen.CustomStartCap = GetClone(pen, size,false);
    }
    public override void SetEndCap(Pen pen, float size)
    {
      pen.EndCap = LineCap.Custom;
      pen.CustomEndCap = GetClone(pen, size,true);
    }
  }
}
