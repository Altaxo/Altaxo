#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Altaxo.Graph.Plot.Groups;

namespace Altaxo.Graph.Gdi.Plot
{
  using Plot.Groups;

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
    /// styles. The styles collected here should be only external styles, i.e. such styles
    /// that are indended to be share between different plot items.
    /// </summary>
    /// <param name="styles">The collection of group styles where to add external group styles.</param>
    void CollectStyles(PlotGroupStyleCollection styles);

    /// <summary>
    /// Prepare the group styles before applying them. This function is called for <b>all</b> plot items in a group <b>before</b>
    /// the ApplyStyle function is called.
    /// </summary>
    /// <param name="styles">The collection of group styles.</param>
    /// <param name="layer">The plot layer.</param>
    void PrepareStyles(PlotGroupStyleCollection styles, IPlotArea layer);

    /// <summary>
    /// Applies the group styles to this plot item. This function is called for all plot items in a group before
    /// the next function (for instance PreparePainting) is called.
    /// </summary>
    /// <param name="styles">The collection of group styles.</param>
    void ApplyStyles(PlotGroupStyleCollection styles);

    /// <summary>
    /// Sets the plot style (or sub plot styles) in this item according to a template provided by the plot item in the template argument.
    /// </summary>
    /// <param name="template">The template item to copy the plot styles from.</param>
    /// <param name="strictness">Denotes the strictness the styles are copied from the template. See <see cref="PlotGroupStrictness" /> for more information.</param>
    void SetPlotStyleFromTemplate(IGPlotItem template, PlotGroupStrictness strictness);

    /// <summary>
    /// This routine ensures that the plot item updates all its cached data and send the appropriate
    /// events if something has changed. Called before the layer paint routine paints the axes because
    /// it must be ensured that the axes scales are scaled correctly before the plots are painted.
    /// This function is called before the call to PrepareStyles.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    void PrepareScales(IPlotArea layer);

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
