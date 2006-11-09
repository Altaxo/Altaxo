using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.PlotGroups
{
  /// <summary>
  /// Enumerates the strictness of the coupling between plot items into a plot group.
  /// </summary>
  public enum PlotGroupStrictness
  {
    /// <summary>
    /// Only the property are coupled, like color and symbols.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// If the plot styles have the same substyles (for instance both have scatter styles), then the style's properties
    /// are set to the same values.
    ///
    /// </summary>
    Exact,

    /// <summary>
    /// The style of the master item is copyied exactly to the style of the slave items (including all substyles).
    /// </summary>
    Strict
  }
}
