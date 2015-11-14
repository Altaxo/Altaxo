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

using Altaxo.Collections;
using Altaxo.Geometry;
using Altaxo.Graph.Plot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Graph3D.Plot
{
	using Altaxo.Graph;
	using Altaxo.Graph.Plot;
	using Altaxo.Graph.Plot.Groups;
	using Altaxo.Main;
	using GraphicsContext;
	using Groups;

	/// <summary>
	/// Interface for a plottable item.
	/// </summary>
	public interface IGPlotItem : Main.ICopyFrom, Main.IChangedEventSource, Main.IDocumentLeafNode, ITreeListNodeWithParent<IGPlotItem>
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

		/// <summary>
		/// The collection where this plot item belongs to. Can be null for the root item.
		/// </summary>
		PlotItemCollection ParentCollection { get; }

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
		void PrepareGroupStyles(PlotGroupStyleCollection styles, IPlotArea layer);

		/// <summary>
		/// Applies the group styles to this plot item. This function is called for all plot items in a group before
		/// the next function (for instance PreparePainting) is called.
		/// </summary>
		/// <param name="styles">The collection of group styles.</param>
		void ApplyGroupStyles(PlotGroupStyleCollection styles);

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
		/// Called before painting takes place.
		/// </summary>
		/// <param name="context">The painting context.</param>
		void PaintPreprocessing(IPaintContext context);

		/// <summary>
		/// This paints the plot to the layer.
		/// </summary>
		/// <param name="g">The graphics context.</param>
		/// <param name="context">The painting context.</param>
		/// <param name="layer">The plot layer.</param>
		/// <param name="previousPlotItem">Previous plot item.</param>
		/// <param name="nextPlotItem">Next plot item. Can be null.</param>
		/// <returns>A data object, which can be used by the next plot item for some styles (like fill style).</returns>
		void Paint(IGraphicContext3D g, IPaintContext context, IPlotArea layer, IGPlotItem previousPlotItem, IGPlotItem nextPlotItem);

		/// <summary>
		/// Called after painting has finished. Can be used to release resources.
		/// </summary>
		void PaintPostprocessing();

		/// <summary>
		/// Paints a symbol for this plot item for use in a legend.
		/// </summary>
		/// <param name="g">The graphics context.</param>
		/// <param name="location">The rectangle where the symbol should be painted into.</param>
		void PaintSymbol(IGraphicContext3D g, RectangleD3D location);

		/// <summary>
		/// Test wether the mouse hits a plot item. The default implementation returns null.
		/// If you want to have a reaction on mouse click on a curve, return a <see cref="IHitTestObject"/>.
		/// </summary>
		/// <param name="layer">The layer in which this plot item is drawn into.</param>
		/// <param name="hitpoint">The point where the mouse is pressed.</param>
		/// <returns>Null if no hit, or a <see cref="IHitTestObject" /> if there was a hit.</returns>
		IHitTestObject HitTest(IPlotArea layer, Ray3D hitpoint);

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		void VisitDocumentReferences(DocNodeProxyReporter Report);
	}
}