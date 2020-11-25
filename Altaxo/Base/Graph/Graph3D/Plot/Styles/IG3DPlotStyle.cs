#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;
using Altaxo.Graph.Graph3D.Plot.Data;
using Altaxo.Main;

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
  using Altaxo.Data;
  using Groups;

  public interface IG3DPlotStyle : Altaxo.Graph.Plot.Styles.IGPlotStyle
  {
    /// <summary>
    /// Adds all plot group styles that are not already in the externalGroups collection, and that
    /// are appropriate for this plot style. Furthermore, the group style must be intended for use as external group style.
    /// </summary>
    /// <param name="externalGroups">The collection of external group styles.</param>
    void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups);

    /// <summary>
    /// Looks in externalGroups and localGroups to find PlotGroupStyles that are appropriate for this plot style.
    /// If such PlotGroupStyles were not found, the function adds them to the localGroups collection.
    /// </summary>
    /// <param name="externalGroups">External plot groups. This collection remains unchanged and is provided here only to check whether or not the group style is already present in the externalGroups.</param>
    /// <param name="localGroups">Local plot groups. To this collection PlotGroupStyles are added if neccessary.</param>
    void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups);

    /// <summary>
    /// Prepares the group styles by showing them to this plot style.
    /// </summary>
    /// <param name="externalGroups">External plot group styles, i.e. plot group styles from the plot item collection.</param>
    /// <param name="localGroups">Internal plot group styles of the plot item this plot style belongs to.</param>
    /// <param name="layer">Plot layer the plot item belonging to this plot style resides in.</param>
    /// <param name="pdata">The preprocessed plot data of the plot item.</param>
    void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed3DPlotData pdata);

    /// <summary>
    /// Applies the group styles to this plot styles.
    /// </summary>
    /// <param name="externalGroups">External plot group styles, i.e. plot group styles from the plot item collection.</param>
    /// <param name="localGroups">Internal plot group styles of this plot item.</param>
    void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups);

    /// <summary>
    /// Paints the style.
    /// </summary>
    /// <param name="g">The graphics.</param>
    /// <param name="layer">Area to plot to</param>
    /// <param name="pdata">The preprocessed plot data used for plotting.</param>
    /// <param name="prevItemData">Plot data of the previous plot item.</param>
    /// <param name="nextItemData">Plot data of the next plot item.</param>
    void Paint(IGraphicsContext3D g, IPlotArea layer, Processed3DPlotData pdata, Processed3DPlotData? prevItemData, Processed3DPlotData? nextItemData);

    /// <summary>
    /// Paints a appropriate symbol in the given rectangle. The width of the rectangle is mandatory, but if the heigth is too small,
    /// you should extend the bounding rectangle and set it as return value of this function.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="bounds">The bounds, in which the symbol should be painted.</param>
    /// <returns>If the height of the bounding rectangle is sufficient for painting, returns the original bounding rectangle. Otherwise, it returns a rectangle that is
    /// inflated in y-Direction. Do not inflate the rectangle in x-direction!</returns>
    RectangleD3D PaintSymbol(IGraphicsContext3D g, RectangleD3D bounds);

    /// <summary>
    /// Prepares the scale(s) of the plot style. This is intended to be used with plot styles which
    /// have an internal scale, for instance <see cref="Gdi.Plot.Styles.ColumnDrivenColorPlotStyle"/> or
    /// <see cref="Gdi.Plot.Styles.ColumnDrivenSymbolSizePlotStyle"/>, which should act on this call with updating their internal scale.
    /// </summary>
    /// <param name="layer">The parent layer.</param>
    void PrepareScales(IPlotArea layer);
  }
}
