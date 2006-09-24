using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;


namespace Altaxo.Graph.Gdi.Plot.Groups
{
  public class CoordinateTransformingStyleBase
  {
    public static void MergeXBoundsInto(IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IXBoundsHolder)
        {
          IXBoundsHolder plotItem = (IXBoundsHolder)pi;
          plotItem.MergeXBoundsInto(pb);
        }
      }
    }

    public static void MergeYBoundsInto(IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IYBoundsHolder)
        {
          IYBoundsHolder plotItem = (IYBoundsHolder)pi;
          plotItem.MergeYBoundsInto(pb);
        }
      }
    }

    public static void Paint(System.Drawing.Graphics g, IPlotArea layer, PlotItemCollection coll)
    {
      foreach (IGPlotItem pi in coll)
        pi.Paint(g, layer);
    }


  }
}
