using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Altaxo.Data;
namespace Altaxo.Graph.Gdi.Plot.Data
{
  /// <summary>
  /// Allows access not only to the original physical plot data,
  /// but also to the plot ranges and to the plot points in absolute layer coordiates.
  /// </summary>
  public abstract class Processed2DPlotData
  {
    public PlotRangeList RangeList;
    public PointF[] PlotPointsInAbsoluteLayerCoordinates;

    public abstract AltaxoVariant GetXPhysical(int originalRowIndex);
    public abstract AltaxoVariant GetYPhysical(int originalRowIndex);
  }
}
