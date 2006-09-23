using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.G2D.Plot.Groups
{
  public interface ICoordinateTransformingGroupStyle : ICloneable
  {
    void MergeXBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll);
    void MergeYBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll);
    void Paint(System.Drawing.Graphics g, IPlotArea layer, PlotItemCollection coll);

  }
}
