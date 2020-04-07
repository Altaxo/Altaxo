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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Main;

namespace Altaxo.Graph.Plot.Data
{
  /// <summary>
  /// Interface to 2D function plot data.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IDocumentLeafNode" />
  /// <seealso cref="System.ICloneable" />
  public interface IXYFunctionPlotData : Altaxo.Main.IDocumentLeafNode, ICloneable
  {
    /// <summary>
    /// For a given layer, this call gets the points used for plotting the function. The boundaries (xorg and xend)
    /// are retrieved from the x-axis of the layer.
    /// </summary>
    /// <param name="layer">The layer.</param>
    /// <returns></returns>
    Processed2DPlotData GetRangesAndPoints(Gdi.IPlotArea layer);

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    void VisitDocumentReferences(DocNodeProxyReporter Report);
  }
}
