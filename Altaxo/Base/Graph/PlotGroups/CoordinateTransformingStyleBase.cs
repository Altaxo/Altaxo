using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.G2D;
using Altaxo.Graph.G2D.Plot;


namespace Altaxo.Graph.G2D.Plot.Groups
{
  public class CoordinateTransformingStyleBase
  {
    public static void MergeXBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IXBoundsHolder)
        {
          IXBoundsHolder plotItem = (IXBoundsHolder)pi;
          plotItem.MergeXBoundsInto(layer, pb);
        }
      }
    }

    public static void MergeYBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IYBoundsHolder)
        {
          IYBoundsHolder plotItem = (IYBoundsHolder)pi;
          plotItem.MergeYBoundsInto(layer, pb);
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
