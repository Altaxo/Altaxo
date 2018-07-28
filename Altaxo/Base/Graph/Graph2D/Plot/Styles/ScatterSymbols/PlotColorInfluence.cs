using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
  /// <summary>
  /// Determines how the plot color influences the colors of a scatter symbol.
  /// </summary>
  [Flags]
  public enum PlotColorInfluence
  {
    /// <summary>
    /// The plot color has no influence on any of the colors in the scatter symbol.
    /// </summary>
    None = 0,

    /// <summary>
    /// The fill color is controlled by the plot color, but the original alpha value of the fill color is preserved.
    /// </summary>
    FillColorPreserveAlpha = 1,

    /// <summary>
    /// The fill color is fully controlled by the plot color.
    /// </summary>
    FillColorFull = 2,

    /// <summary>
    /// The frame color is controlled by the plot color, but the original alpha value of the frame color is preserved.
    /// </summary>
    FrameColorPreserveAlpha = 4,

    /// <summary>
    /// The frame color is controlled by the plot color.
    /// </summary>
    FrameColorFull = 8,

    /// <summary>
    /// The inset color is controlled by the plot color, but the original alpha value of the inset color is preserved.
    /// </summary>
    InsetColorPreserveAlpha = 16,

    /// <summary>
    /// The inset color is controlled by the plot color.
    /// </summary>
    InsetColorFull = 32,
  }
}
