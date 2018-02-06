#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Plot
{
	/// <summary>
	/// Common interface of both 2D (<see cref="Altaxo.Graph.Gdi.Plot.IGPlotItem"/>) and 3D plot (<see cref="Altaxo.Graph.Graph3D.Plot.IGPlotItem"/>) items.
	/// </summary>
	/// <seealso cref="Altaxo.Main.ICopyFrom" />
	/// <seealso cref="Altaxo.Main.IChangedEventSource" />
	/// <seealso cref="Altaxo.Main.IDocumentLeafNode" />
	public interface IGPlotItem : Main.ICopyFrom, Main.IChangedEventSource, Main.IDocumentLeafNode
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
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		void VisitDocumentReferences(DocNodeProxyReporter Report);

		/// <summary>
		/// Gets an object that holds the data used for plotting. If not applicable for this kind of plot item, null is returned.
		/// </summary>
		/// <value>
		/// The object that holds the data used for plotting.
		/// </value>
		IDocumentLeafNode DataObject { get; }

		/// <summary>
		/// Gets an object that holds the style(s) used for plotting. If not applicable for this kind of plot item, null is returned.
		/// </summary>
		/// <value>
		/// The object that holds the style(s) used for plotting.
		/// </value>
		IDocumentLeafNode StyleObject { get; }
	}
}