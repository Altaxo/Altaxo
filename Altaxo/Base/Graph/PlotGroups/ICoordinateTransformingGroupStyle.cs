using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Graph.Axes.Boundaries;

namespace Altaxo.Graph.PlotGroups
{
  public interface ICoordinateTransformingGroupStyle : ICloneable
  {
    void MergeXBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll);
    void MergeYBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll);
    void Paint(System.Drawing.Graphics g, IPlotArea layer, PlotItemCollection coll);

  }
}
