using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// Support of plotting properties, that can be grouped together, for instance color or line style.
  /// </summary>
  public interface IPlotGroupStyle : ICloneable
  {
    /// <summary>
    /// Called at the beginning of the preparation.
    /// </summary>
    void BeginPrepare();
    /// <summary>
    /// Called at the end of the preparation.
    /// </summary>
    void EndPrepare();

    /// <summary>
    /// PrepareStep is called every time after for each PlotItem Prepare is called.
    /// </summary>
    void PrepareStep();

    /// <summary>
    /// Determines if this style can have childs, i.e. other plot group
    /// styles that are incremented if this group style swaps over.
    /// When <see cref="Step" /> can return only zero, you should return here false. Otherwise
    /// you should return true.
    /// </summary>
    /// <returns></returns>
    bool CanHaveChilds();
    /// <summary>
    /// Increments/decrements the style. Returns true when the style was swapped around.
    /// </summary>
    /// <param name="step">Either +1 or -1 for increment or decrement.</param>
    /// <returns>Number of wraps.</returns>
    int Step(int step);

    /// <summary>
    /// Get/sets whether or not stepping is allowed.
    /// </summary>
    bool IsStepEnabled { get; set; }

    /// <summary>
    /// Return true when this group style contains valid grouping data.
    /// You should set IsInitialized to false when BeginPrepare is called.
    /// </summary>
    bool IsInitialized { get; }
  }
}
