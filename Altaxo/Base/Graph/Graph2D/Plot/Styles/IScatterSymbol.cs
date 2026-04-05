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
using Altaxo.Drawing;
using Clipper2Lib;

namespace Altaxo.Graph.Graph2D.Plot.Styles
{
  /// <summary>
  /// Represents a symbol shape for a 2D scatter plot. Instances of this class must be immutable. They still need to be cloneable,
  /// because in a list of scatter symbols we need unique instances.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IImmutable" />
  public interface IScatterSymbol : Main.IImmutable, ICloneable
  {
    /// <summary>
    /// Gets the frame of the symbol, or null if the symbol does not have a frame.
    /// </summary>
    /// <value>
    /// The frame of the symbol.
    /// </value>
    ScatterSymbols.IScatterSymbolFrame? Frame { get; }

    /// <summary>
    /// Returns a copy of this symbol with the specified frame.
    /// </summary>
    /// <param name="frame">The frame to apply.</param>
    /// <returns>A symbol instance with the specified frame.</returns>
    IScatterSymbol WithFrame(ScatterSymbols.IScatterSymbolFrame? frame);

    /// <summary>
    /// Gets the inset of this symbol, or null if the symbol does not have an inset.
    /// </summary>
    /// <value>
    /// The inset of the symbol.
    /// </value>
    ScatterSymbols.IScatterSymbolInset? Inset { get; }

    /// <summary>
    /// Returns a copy of this symbol with the specified inset.
    /// </summary>
    /// <param name="inset">The inset to apply.</param>
    /// <returns>A symbol instance with the specified inset.</returns>
    IScatterSymbol WithInset(ScatterSymbols.IScatterSymbolInset? inset);

    /// <summary>
    /// Gets the fill color of the symbol.
    /// </summary>
    NamedColor FillColor { get; }

    /// <summary>
    /// Returns a copy of this symbol with the specified fill color.
    /// </summary>
    /// <param name="fillColor">The fill color to apply.</param>
    /// <returns>A symbol instance with the specified fill color.</returns>
    IScatterSymbol WithFillColor(NamedColor fillColor);

    /// <summary>Determines which colors of the scatter symbol are affected by the plot color.</summary>
    ScatterSymbols.PlotColorInfluence PlotColorInfluence { get; }

    /// <summary>
    /// Returns a copy of this symbol with the specified plot-color influence.
    /// </summary>
    /// <param name="plotColorInfluence">The plot-color influence to apply.</param>
    /// <returns>A symbol instance with the specified plot-color influence.</returns>
    IScatterSymbol WithPlotColorInfluence(ScatterSymbols.PlotColorInfluence plotColorInfluence);

    /// <summary>
    /// Gets the width of internal structures (line e.g. the frame), relative to the symbol size.
    /// </summary>
    /// <value>
    /// The width of internal structures (line e.g. the frame), relative to the symbol size.
    /// </value>
    double RelativeStructureWidth { get; }

    /// <summary>
    /// Returns a copy of this symbol with the specified relative structure width.
    /// </summary>
    /// <param name="relativeStructureWidth">The relative width of internal structures.</param>
    /// <returns>A symbol instance with the specified relative structure width.</returns>
    IScatterSymbol WithRelativeStructureWidth(double relativeStructureWidth);

    /// <summary>
    /// Gets the design size of this scatter symbol.
    /// </summary>
    /// <value>
    /// The design size of this scatter symbol.
    /// </value>
    double DesignSize { get; }

    /// <summary>
    /// Calculates the polygons of the outer frame, the inner frame, and the inset.
    /// </summary>
    /// <param name="relativeStructureWidth">The relative width of the internal structures, e.g. the frame, relative to the symbol size.
    /// If this argument is null, the value of this instance (<see cref="RelativeStructureWidth"/>) will be used.</param>
    /// <param name="framePolygon">On return: the frame polygon (null if no frame present).</param>
    /// <param name="insetPolygon">On return: the inset polygon (null if no inset present).</param>
    /// <param name="fillPolygon">On return: the fill polygon (null if no fill shape present).</param>
    void CalculatePolygons(
      double? relativeStructureWidth,
      out Paths64? framePolygon,
      out Paths64? insetPolygon,
      out Paths64? fillPolygon);
  }
}
