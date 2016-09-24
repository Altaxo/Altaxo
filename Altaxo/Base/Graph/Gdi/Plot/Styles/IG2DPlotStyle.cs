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

using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
	using Altaxo.Data;
	using Groups;

	public interface IG2DPlotStyle :
		Main.ICopyFrom,
		Main.IChangedEventSource,
		Main.IChildChangedEventSink,
		Main.IDocumentLeafNode
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
		void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata);

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
		void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata, Processed2DPlotData prevItemData, Processed2DPlotData nextItemData);

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
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		void VisitDocumentReferences(DocNodeProxyReporter Report);

		/// <summary>
		/// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
		/// </summary>
		/// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
		/// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
		IEnumerable<Tuple<
			string, // Column label
			IReadableColumn, // the column as it was at the time of this call
			string, // the name of the column (last part of the column proxies document path)
			Action<IReadableColumn> // action to set the column during Apply of the controller
			>> GetAdditionallyUsedColumns();

		/// <summary>
		/// Prepares the scale(s) of the plot style. This is intended to be used with plot styles which
		/// have an internal scale, for instance <see cref="Gdi.Plot.Styles.ColumnDrivenColorPlotStyle"/> or
		/// <see cref="Gdi.Plot.Styles.ColumnDrivenSymbolSizePlotStyle"/>, which should act on this call with updating their internal scale.
		/// </summary>
		/// <param name="layer">The parent layer.</param>
		void PrepareScales(IPlotArea layer);

		/// <summary>
		/// Copies from a template style, but either with or without data references. If the choice is without data references, data references, as for instance in <see cref="ErrorBarZPlotStyle"/>, are left alone, i.e. have the same value as before this call.
		/// </summary>
		/// <param name="from">The style to copy from.</param>
		/// <param name="copyWithDataReferences">If true, data references are copyied from the template style to this style. If false, the data references of this style are left as they are.</param>
		/// <returns>True if any properties could be copied from the template style; otherwise, false.</returns>
		bool CopyFrom(object from, bool copyWithDataReferences);

		/// <summary>
		/// Clones the style, but either with or without data references. Thus, if <paramref name="copyWithDataReferences"/> is false, data references, as for instance in <see cref="ErrorBarZPlotStyle"/>, are left empty in the cloned instance.
		/// </summary>
		/// <param name="copyWithDataReferences">If true, data references are cloned to the new instance. If false, data references in the cloned instance are empty.</param>
		/// <returns>Cloned instance, but either with cloned data references or with empty data references, depending on <paramref name="copyWithDataReferences"/>.</returns>
		object Clone(bool copyWithDataReferences);
	}
}