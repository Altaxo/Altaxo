#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Gdi.Plot.Groups
{
  /// <summary>
  /// Plot group style, which changes plot items by transforming their coordinates, i.e. by shifting them on the x- or y-axis (or both).
  /// </summary>
  public interface ICoordinateTransformingGroupStyle : Main.IDocumentLeafNode, ICloneable
  {
    /// <summary>
    /// Merges the X bounds of all plot items in the collection <paramref name="coll"/> into the boundaries <paramref name="pb"/>.
    /// If the group style transforms the x values, of course the transformed values should be merged into.
    /// </summary>
    /// <param name="layer">The layer.</param>
    /// <param name="pb">The physical boundaries to merge with.</param>
    /// <param name="coll">The collection of plot items.</param>
    void MergeXBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll);

    /// <summary>
    /// Merges the Y bounds of all plot items in the collection <paramref name="coll"/> into the boundaries <paramref name="pb"/>.
    /// If the group style transforms the y values, of course the transformed values should be merged into.
    /// </summary>
    /// <param name="layer">The layer.</param>
    /// <param name="pb">The physical boundaries to merge with.</param>
    /// <param name="coll">The collection of plot items.</param>
    void MergeYBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll);

    /// <summary>
    /// Prepare the paint of the plot items that belongs to this group style.
    /// </summary>
    /// <param name="g">Graphics context used for drawing.</param>
    /// <param name="paintContext">The paint context</param>
    /// <param name="layer">Plot layer.</param>
    /// <param name="coll">Collection of plot items to draw.</param>
    void PaintPreprocessing(System.Drawing.Graphics g, IPaintContext paintContext, IPlotArea layer, PlotItemCollection coll);

    /// <summary>
    /// Finishes the painting of the plot items that belongs to this style. Paints the end.
    /// </summary>
    void PaintPostprocessing();

    /// <summary>
    /// Paints the child of a plot item collection.
    /// </summary>
    /// <param name="g">Graphics context used for drawing.</param>
    /// <param name="context">The paint context.</param>
    /// <param name="layer">Plot layer.</param>
    /// <param name="collection">Collection of plot items to draw.</param>
    /// <param name="indexOfChild">Index of the item that should be painted in the <paramref name="collection"/>.</param>
    void PaintChild(System.Drawing.Graphics g, IPaintContext context, IPlotArea layer, PlotItemCollection collection, int indexOfChild);
  }
}
