using Altaxo.Graph.Gdi.Plot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Plot.Data
{
  /// <summary>
  /// Interface to 2D function plot data.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IDocumentLeafNode" />
  /// <seealso cref="System.ICloneable" />
  public interface IXYFunctionPlotData : Altaxo.Main.IDocumentLeafNode, ICloneable
  {
    /// <summary>
    /// For a given layer, this call gets the points used for plotting the function. The boundaries (xorg and xend)
    /// are retrieved from the x-axis of the layer.
    /// </summary>
    /// <param name="layer">The layer.</param>
    /// <returns></returns>
    Processed2DPlotData GetRangesAndPoints(Gdi.IPlotArea layer);
  }
}
