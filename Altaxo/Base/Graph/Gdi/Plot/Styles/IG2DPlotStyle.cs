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
using Altaxo.Graph.Gdi.Plot.Data;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
  using Graph.Plot.Groups;
  using Groups;

  public interface IG2DPlotStyle : ICloneable, Main.IChangedEventSource, Main.IDocumentNode
  {

    /// <summary>
    /// Adds all plot group styles that are not already in the externalGroups collection, and that
    /// are appropriate for this plot style. Furthermore, the group style must be intended for use as external group style.
    /// </summary>
    /// <param name="externalGroups">The collection of external group styles.</param>
    void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups);

    /// <summary>
    /// Looks in externalGroups and localGroups to find PlotGroupStyles that are appropriate for this style.
    /// If such PlotGroupStyles where not found, the function adds them to the localGroups collection.
    /// </summary>
    /// <param name="externalGroups">External plot groups. This collection remains unchanged and is provided here only to check whether or not the group style is already present in the externalGroups.</param>
    /// <param name="localGroups">Local plot groups. To this collection PlotGroupStyles are added if neccessary.</param>
    void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups);


    /// <summary>
    /// Applies the Group styles to the plot styles.
    /// </summary>
    /// <param name="externalGroups"></param>
    /// <param name="localGroups"></param>
    /// <param name="layer"></param>
    /// <param name="pdata"></param>
    void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata);


    /// <summary>
    /// Applies the group styles to the plot styles.
    /// </summary>
    /// <param name="externalGroups"></param>
    /// <param name="localGroups"></param>
    void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups);

    /// <summary>
    /// Paints the style.
    /// </summary>
    /// <param name="g">The graphics.</param>
    /// <param name="layer">Area to plot to</param>
    /// <param name="pdata">The preprocessed plot data used for plotting.</param>
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
    /// Sets the parent object
    /// </summary>
    new object ParentObject { set; }
  }
}
