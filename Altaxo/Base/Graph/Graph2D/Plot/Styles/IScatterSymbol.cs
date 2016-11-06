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

using Altaxo.Drawing;
using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph2D.Plot.Styles
{
	/// <summary>
	/// Represents a symbol shape for a 3D scatter plot. Instances of this class have to be immutable. They still need to be cloneable,
	/// because in a list of scatter symbols we need unique instances.
	/// </summary>
	/// <seealso cref="Altaxo.Main.IImmutable" />
	public interface IScatterSymbol : Main.IImmutable, ICloneable
	{
		ScatterSymbols.IScatterSymbolFrame Frame { get; }
		ScatterSymbols.IScatterSymbolInset Inset { get; }

		NamedColor FillColor { get; }

		/// <summary>Determines which colors of the scatter symbol are affected by the plot color.</summary>
		ScatterSymbols.PlotColorInfluence PlotColorInfluence { get; }

		/// <summary>
		/// Gets the width of internal structures (line e.g. the frame), relative to the symbol size.
		/// </summary>
		/// <value>
		/// The width of internal structures (line e.g. the frame), relative to the symbol size.
		/// </value>
		double RelativeStructureWidth { get; }

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
		/// <param name="framePolygon">On return: the frame polygon.</param>
		/// <param name="insetPolygon">On return: the inset polygon.</param>
		/// <param name="fillPolygon">On return: the fill polygon.</param>
		void CalculatePolygons(
			double? relativeStructureWidth,
			out List<List<ClipperLib.IntPoint>> framePolygon,
			out List<List<ClipperLib.IntPoint>> insetPolygon,
			out List<List<ClipperLib.IntPoint>> fillPolygon);
	}
}