using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.LineCaps
{
  /// <summary>
  /// Draws a cap that is a line perpendicular to the end of the line, and on the left side of the line.
  /// </summary>
  public class LeftBarLineCap : RightBarLineCap
  {

    public override string Name { get { return "BarLeft"; } }
    public override void SetStartCap(Pen pen, float size)
    {
      pen.StartCap = LineCap.Custom;
      pen.CustomStartCap = GetClone(pen, size, true);
    }
    public override void SetEndCap(Pen pen, float size)
    {
      pen.EndCap = LineCap.Custom;
      pen.CustomEndCap = GetClone(pen, size, false);
    }
  }
}
