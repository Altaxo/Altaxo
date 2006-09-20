using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Altaxo.Graph
{
  using PlotGroups;

  /// <summary>
  /// Interface for a plottable item.
  /// </summary>
  public interface IGPlotItem : ICloneable, Main.IChangedEventSource, Main.IDocumentNode
  {
    /// <summary>
    /// The name of the plot. It can be of different length. An argument of zero or less
    /// returns the shortest possible name, higher values return more verbose names.
    /// </summary>
    /// <param name="level">The naming level, 0 returns the shortest possible name, 1 or more returns more
    /// verbose names.</param>
    /// <returns>The name of the plot.</returns>
    string GetName(int level);

    /// <summary>
    /// The name of the plot. The style how to find the name is determined by the style argument. The possible
    /// styles depend on the type of plot item.
    /// </summary>
    /// <param name="style">The style determines the "verbosity" of the plot name.</param>
    /// <returns>The name of the plot.</returns>
    string GetName(string style);

    PlotItemCollection ParentCollection { get; }
    new object ParentObject { set; }

    /// <summary>
    /// Collects all possible group styles that can be applied to this plot item in
    /// styles.
    /// </summary>
    /// <param name="styles">The collection of group styles.</param>
    void CollectStyles(PlotGroupStyleCollection styles);

    /// <summary>
    /// Prepare the group styles before applying them. This function is called for <b>all</b> plot items in a group <b>before</b>
    /// the ApplyStyle function is called.
    /// </summary>
    /// <param name="styles">The collection of group styles.</param>
    void PrepareStyles(PlotGroupStyleCollection styles);

    /// <summary>
    /// Applies the group styles to this plot item. This function is called for all plot items in a group before
    /// the next function (for instance PreparePainting) is called.
    /// </summary>
    /// <param name="styles">The collection of group styles.</param>
    void ApplyStyles(PlotGroupStyleCollection styles);

    /// <summary>
    /// This routine ensures that the plot item updates all its cached data and send the appropriate
    /// events if something has changed. Called before the layer paint routine paints the axes because
    /// it must be ensured that the axes are scaled correctly before the plots are painted.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    void PreparePainting(IPlotArea layer);

    /// <summary>
    /// This paints the plot to the layer.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="layer">The plot layer.</param>
    void Paint(Graphics g, IPlotArea layer);

    /// <summary>
    /// Paints a symbol for this plot item for use in a legend.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="location">The rectangle where the symbol should be painted into.</param>
    void PaintSymbol(Graphics g, RectangleF location);

    /// <summary>
    /// Test wether the mouse hits a plot item. The default implementation here returns null.
    /// If you want to have a reaction on mouse click on a curve, implement this function.
    /// </summary>
    /// <param name="layer">The layer in which this plot item is drawn into.</param>
    /// <param name="hitpoint">The point where the mouse is pressed.</param>
    /// <returns>Null if no hit, or a <see cref="IHitTestObject" /> if there was a hit.</returns>
    IHitTestObject HitTest(IPlotArea layer, PointF hitpoint);

  }
}
