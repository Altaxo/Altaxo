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
using Altaxo.Graph.Plot.Data;
using Altaxo.Main;

namespace Altaxo.Data.Selections
{
  public interface IRowSelection : Main.IDocumentLeafNode, ICloneable
  {
    /// <summary>
    /// Gets the selected row indices as segments of (startIndex, endIndexExclusive), beginning with no less than the start index and less than the maximum index.
    /// </summary>
    /// <param name="startIndex">The start index. Each segment that is returned has to start at an index being equal to or greater than this value.</param>
    /// <param name="maxIndexExclusive">The maximum index.  Each segment that is returned has to have an endExclusive value being less than or equal to this value.</param>
    /// <param name="table">The underlying data column collection. All columns that are part of the row selection should either be standalone or belong to this collection.</param>
    /// <param name="totalRowCount">The maximum number of rows (and therefore the row index after the last inclusive row index) that could theoretically be returned, for instance if the selection is <see cref="AllRows"/>.
    /// This parameter is neccessary because some of the selections (e.g. <see cref="RangeOfRowIndices"/>) work <b>relative</b> to the start or to the end of the maximum possible range, and therefore need this range for calculations.  </param>
    /// <returns>The segments of selected row indices, beginning with a segment starting at no less than the start index and ending with a segment whose endExclusive value is less than or equal to the maximum index.</returns>
    IEnumerable<(int start, int endExclusive)> GetSelectedRowIndexSegmentsFromTo(int startIndex, int maxIndexExclusive, DataColumnCollection? table, int totalRowCount);

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
    IEnumerable<ColumnInformationSimple> GetAdditionallyUsedColumns();
  }

  /// <summary>
  /// Interface to a collection of row selections. Since this is itself a row selection, it extends <see cref="IRowSelection"/> interface.
  /// </summary>
  /// <seealso cref="T:System.Collections.Generic.IEnumerable{Selections.IRowSelection}" />
  /// <seealso cref="T:Altaxo.Data.Selections.IRowSelection" />
  public interface IRowSelectionCollection : IEnumerable<IRowSelection>, IRowSelection
  {
    IRowSelectionCollection WithAdditionalItem(IRowSelection item);

    IRowSelectionCollection WithChangedItem(int idx, IRowSelection item);

    IRowSelectionCollection NewWithItems(IEnumerable<IRowSelection> items);

    /// <summary>
    /// Gets a value indicating whether the collection with only one item returns the same selection than the one item alone.
    /// This is the case e.g. for union and intersection, but not for exclusion of a union.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the collection with only one item returns the same selection than the one item alone.
    /// </value>
    bool IsCollectionWithOneItemEquivalentToThisItem { get; }
  }

  /// <summary>
  /// Helper class for <see cref="IRowSelection"/> instances.
  /// </summary>
  public static class IRowSelectionExtensions
  {
    /// <summary>
    /// Gets the selected row indices continuously, beginning with no less than the start index and less than the maximum index.
    /// </summary>
    /// <param name="rowSelection">The row selection.</param>
    /// <param name="startIndex">The start index. Each row index that is returned has to be equal to or greater than this value.</param>
    /// <param name="maxIndexExclusive">The maximum index.  Each row index that is returned has to be less than this value.</param>
    /// <param name="table">The underlying data column collection. All columns that are part of the row selection should either be standalone or belong to this collection.</param>
    /// <param name="totalRowCount">The maximum number of rows (and therefore the row index after the last inclusive row index) that could theoretically be returned, for instance if the selection is <see cref="AllRows"/>.
    /// This parameter is neccessary because some of the selections (e.g. <see cref="RangeOfRowIndices"/>) work <b>relative</b> to the start or to the end of the maximum possible range, and therefore need this range for calculations.  </param>
    /// <returns>The selected row indices, beginning with no less than the start index and less than the maximum index.</returns>
    public static IEnumerable<int> GetSelectedRowIndicesFromTo(this IRowSelection rowSelection, int startIndex, int maxIndexExclusive, DataColumnCollection? table, int totalRowCount)
    {
      foreach (var segment in rowSelection.GetSelectedRowIndexSegmentsFromTo(startIndex, maxIndexExclusive, table, totalRowCount))
      {
        for (int i = segment.start; i < segment.endExclusive; ++i)
          yield return i;
      }
    }
  }
}
