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
using System.Text;

namespace Altaxo.Graph.Plot.Styles
{
  using Altaxo.Data;
  using Groups;

  /// <summary>
  /// Common interface to all plot styles, both 2D and 3D plot styles.
  /// </summary>
  /// <seealso cref="Altaxo.Main.ICopyFrom" />
  /// <seealso cref="Altaxo.Main.IChangedEventSource" />
  /// <seealso cref="Altaxo.Main.IChildChangedEventSink" />
  /// <seealso cref="Altaxo.Main.IDocumentLeafNode" />
  public interface IGPlotStyle :
    Main.ICopyFrom,
    Main.IChangedEventSource,
    Main.IChildChangedEventSink,
    Main.IDocumentLeafNode
  {
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
    IEnumerable<(
      string ColumnLabel, // Column label
      IReadableColumn Column, // the column as it was at the time of this call
      string ColumnName, // the name of the column (last part of the column proxies document path)
      Action<IReadableColumn> ColumnSetAction // action to set the column during Apply of the controller
      )> GetAdditionallyUsedColumns();

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
