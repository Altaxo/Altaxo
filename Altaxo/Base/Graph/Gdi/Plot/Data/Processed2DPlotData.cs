using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Altaxo.Data;
namespace Altaxo.Graph.Gdi.Plot.Data
{
  using Graph.Plot.Data;
  /// <summary>
  /// Allows access not only to the original physical plot data,
  /// but also to the plot ranges and to the plot points in absolute layer coordiates.
  /// </summary>
  public abstract class Processed2DPlotData : I3DPhysicalVariantAccessor
  {
    public PlotRangeList RangeList;
    public PointF[] PlotPointsInAbsoluteLayerCoordinates;

    public abstract AltaxoVariant GetXPhysical(int originalRowIndex);
    public abstract AltaxoVariant GetYPhysical(int originalRowIndex);
    public virtual AltaxoVariant GetZPhysical(int originalRowIndex)
    {
      return new AltaxoVariant(0.0);
    }

    /// <summary>
    /// Returns true if the z coordinate is used. Return false if the z coordinate is always 0 (zero), so we can
    /// </summary>
    public virtual bool IsZUsed { get { return false; } }
    /// <summary>
    /// Returns true if the z-value is constant. In this case some optimizations can be made.
    /// </summary>
    public virtual bool IsZConstant { get { return true; } }

  }
}
