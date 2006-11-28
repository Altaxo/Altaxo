using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// Enumerates the strictness of the coupling between plot items into a plot group.
  /// </summary>
  public enum PlotGroupStrictness
  {
    /// <summary>
    /// Only the properties are coupled by means of the plot group styles, like color and symbols.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// If the plot styles have the same substyles (for instance both have scatter styles), then the style's properties
    /// are set to the same values before the plot groups are applied.
    /// </summary>
    Exact = 1,

    /// <summary>
    /// The style of the master item is copyied exactly to the style of all other items in the plot group (including all substyles). Then
    /// the plot groups are applied.
    /// </summary>
    Strict = 2
  }
}
