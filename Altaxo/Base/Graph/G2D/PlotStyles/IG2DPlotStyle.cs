using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Altaxo.Graph
{
  using PlotGroups;

  public interface IG2DPlotStyle : ICloneable, Main.IChangedEventSource, Main.IDocumentNode
  {
    /// <summary>
    /// Looks in externalGroups and localGroups to find PlotGroupStyles that are appropriate for this style.
    /// If such PlotGroupStyles where not found, the function adds them to the localGroups collection.
    /// </summary>
    /// <param name="ExternalGroups">External plot groups. This collection remains unchanged.</param>
    /// <param name="localGroups">Local plot groups. To this collection PlotGroupStyles are added if neccessary.</param>
    void AddLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups);


    /// <summary>
    /// Applies the Group styles to the plot styles.
    /// </summary>
    /// <param name="externalGroups"></param>
    /// <param name="localGroups"></param>
    void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups);


    /// <summary>
    /// Applies the Group styles to the plot styles.
    /// </summary>
    /// <param name="externalGroups"></param>
    /// <param name="localGroups"></param>
    void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups);

    /// <summary>
    /// Paints the style.
    /// </summary>
    /// <param name="g">The graphics.</param>
    /// <param name="layer">Area to plot to</param>
    /// <param name="rangeList">Range list for the ptArray parameter.</param>
    /// <param name="ptArray">Array of plotting points in relative coordinates.</param>
    void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata);

  
    /// <summary>
    /// Paints a appropriate symbol in the given rectangle. The width of the rectangle is mandatory, but if the heigth is too small,
    /// you should extend the bounding rectangle and set it as return value of this function.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="bounds">The bounds, in which the symbol should be painted.</param>
    /// <returns>If the height of the bounding rectangle is sufficient for painting, returns the original bounding rectangle. Otherwise, it returns a rectangle that is
    /// inflated in y-Direction. Do not inflate the rectangle in x-direction!</returns>
    RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds);


    /// <summary>
    /// Sets the parent object of the style.
    /// </summary>
    /// <param name="parent">The parent object.</param>
    object ParentObject { set; }
  }
}
