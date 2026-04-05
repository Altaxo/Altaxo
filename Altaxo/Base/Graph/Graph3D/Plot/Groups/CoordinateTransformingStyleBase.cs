#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Graph3D.Plot.Groups
{
  using GraphicsContext;

  /// <summary>
  /// Provides helper methods for coordinate-transforming plot-group styles.
  /// </summary>
  public class CoordinateTransformingStyleBase
  {
    /// <summary>
    /// Merges x bounds from all eligible plot items into the specified boundaries.
    /// </summary>
    /// <param name="pb">The physical boundaries.</param>
    /// <param name="coll">The plot-item collection.</param>
    public static void MergeXBoundsInto(IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IXBoundsHolder)
        {
          var plotItem = (IXBoundsHolder)pi;
          plotItem.MergeXBoundsInto(pb);
        }
      }
    }

    /// <summary>
    /// Merges y bounds from all eligible plot items into the specified boundaries.
    /// </summary>
    /// <param name="pb">The physical boundaries.</param>
    /// <param name="coll">The plot-item collection.</param>
    public static void MergeYBoundsInto(IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IYBoundsHolder)
        {
          var plotItem = (IYBoundsHolder)pi;
          plotItem.MergeYBoundsInto(pb);
        }
      }
    }

    /// <summary>
    /// Merges z bounds from all eligible plot items into the specified boundaries.
    /// </summary>
    /// <param name="pb">The physical boundaries.</param>
    /// <param name="coll">The plot-item collection.</param>
    public static void MergeZBoundsInto(IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IZBoundsHolder)
        {
          var plotItem = (IZBoundsHolder)pi;
          plotItem.MergeZBoundsInto(pb);
        }
      }
    }

    /// <summary>
    /// Paints the plot items in reverse order.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="paintContext">The paint context.</param>
    /// <param name="layer">The plot layer.</param>
    /// <param name="coll">The plot-item collection.</param>
    public static void Paint(IGraphicsContext3D g, Altaxo.Graph.IPaintContext paintContext, IPlotArea layer, PlotItemCollection coll)
    {
      for (int i = coll.Count - 1; i >= 0; --i)
      {
        coll[i].Paint(g, paintContext, layer, i == coll.Count - 1 ? null : coll[i + 1], i == 0 ? null : coll[i - 1]);
      }
    }
  }
}
