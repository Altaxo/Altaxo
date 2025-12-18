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
using Altaxo.Collections;
using Altaxo.Graph.Plot.Data;
using Altaxo.Main;

namespace Altaxo.Data.Selections
{
  /// <summary>
  /// Represents a selection of rows for data operations.
  /// </summary>
  public interface IRowSelection : Main.IDocumentLeafNode, ICloneable, ITreeNode<IRowSelection>, IEquatable<IRowSelection>
  {
    /// <summary>
    /// Gets the selected row indices as segments of (startIndex, endIndexExclusive), beginning with no less than the start index and less than the maximum index.
    /// </summary>
    /// <param name="startIndex">The start index. Each segment that is returned has to start at an index being equal to or greater than this value.</param>
    /// <param name="maxIndexExclusive">The maximum index. Each segment that is returned has to have an endExclusive value being less than or equal to this value.</param>
    /// <param name="table">The underlying data column collection. All columns that are part of the row selection should either be standalone or belong to this collection.</param>
    /// <param name="totalRowCount">The maximum number of rows (and therefore the row index after the last inclusive row index) that could theoretically be returned, for instance if the selection is <see cref="AllRows"/>.
    /// This parameter is necessary because some of the selections (e.g. <see cref="RangeOfRowIndices"/>) work <b>relative</b> to the start or to the end of the maximum possible range, and therefore need this range for calculations.</param>
    /// <returns>The segments of selected row indices, beginning with a segment starting at no less than the start index and ending with a segment whose endExclusive value is less than or equal to the maximum index.</returns>
    IEnumerable<(int start, int endExclusive)> GetSelectedRowIndexSegmentsFromTo(int startIndex, int maxIndexExclusive, DataColumnCollection? table, int totalRowCount);

    /// <summary>
    /// Visits document references so path items (for example, tables and columns) can be replaced by other paths.
    /// This makes it possible to change a plot so that plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    void VisitDocumentReferences(DocNodeProxyReporter Report);

    /// <summary>
    /// Gets the columns used additionally by this selection, for example label columns or error columns used in plots.
    /// </summary>
    /// <returns>An enumeration of <see cref="ColumnInformationSimple"/> values. Each item contains the column name, as it should appear in dialogs,
    /// and a function that returns the column proxy for this column, in order to get or set the underlying column.</returns>
    IEnumerable<ColumnInformationSimple> GetAdditionallyUsedColumns();
  }

  /// <summary>
  /// Interface to a collection of row selections. Since this is itself a row selection, it extends the <see cref="IRowSelection"/> interface.
  /// </summary>
  public interface IRowSelectionCollection : IEnumerable<IRowSelection>, IRowSelection
  {
    /// <summary>
    /// Creates a new collection that contains all existing items plus the specified additional item.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>A new <see cref="IRowSelectionCollection"/> instance containing the additional item.</returns>
    IRowSelectionCollection WithAdditionalItem(IRowSelection item);

    /// <summary>
    /// Creates a new collection in which the item at the specified index is replaced.
    /// </summary>
    /// <param name="idx">The index of the item to replace.</param>
    /// <param name="item">The new item.</param>
    /// <returns>A new <see cref="IRowSelectionCollection"/> instance with the item replaced.</returns>
    IRowSelectionCollection WithChangedItem(int idx, IRowSelection item);

    /// <summary>
    /// Creates a new collection using the provided items.
    /// </summary>
    /// <param name="items">The items that make up the new collection.</param>
    /// <returns>A new <see cref="IRowSelectionCollection"/> instance containing the provided items.</returns>
    IRowSelectionCollection NewWithItems(IEnumerable<IRowSelection> items);

    /// <summary>
    /// Gets a value indicating whether a collection that contains only a single item returns the same selection as that item alone.
    /// This is the case, for example, for union and intersection collections, but not for exclusion of a union.
    /// </summary>
    /// <value>
    /// <c>true</c> if the collection with only one item returns the same selection as the one item alone; otherwise, <c>false</c>.
    /// </value>
    bool IsCollectionWithOneItemEquivalentToThisItem { get; }
  }

  /// <summary>
  /// Helper methods for <see cref="IRowSelection"/> instances.
  /// </summary>
  public static class IRowSelectionExtensions
  {
    /// <summary>
    /// Gets the selected row indices continuously, beginning with no less than the start index and less than the maximum index.
    /// </summary>
    /// <param name="rowSelection">The row selection.</param>
    /// <param name="startIndex">The start index. Each row index that is returned has to be equal to or greater than this value.</param>
    /// <param name="maxIndexExclusive">The maximum index. Each row index that is returned has to be less than this value.</param>
    /// <param name="table">The underlying data column collection. All columns that are part of the row selection should either be standalone or belong to this collection.</param>
    /// <param name="totalRowCount">The maximum number of rows (and therefore the row index after the last inclusive row index) that could theoretically be returned, for instance if the selection is <see cref="AllRows"/>.
    /// This parameter is necessary because some of the selections (e.g. <see cref="RangeOfRowIndices"/>) work <b>relative</b> to the start or to the end of the maximum possible range, and therefore need this range for calculations.</param>
    /// <returns>The selected row indices, beginning with no less than the start index and less than the maximum index.</returns>
    public static IEnumerable<int> GetSelectedRowIndicesFromTo(this IRowSelection rowSelection, int startIndex, int maxIndexExclusive, DataColumnCollection? table, int totalRowCount)
    {
      foreach (var segment in rowSelection.GetSelectedRowIndexSegmentsFromTo(startIndex, maxIndexExclusive, table, totalRowCount))
      {
        for (int i = segment.start; i < segment.endExclusive; ++i)
          yield return i;
      }
    }

    /// <summary>
    /// Gets the index of the original row in the data table by providing the index of the filtered row.
    /// </summary>
    /// <param name="rowSelection">The row selection (the filter).</param>
    /// <param name="filteredRowIndex">Index of the filtered row.</param>
    /// <param name="table">The underlying data table.</param>
    /// <param name="totalRowCount">The total row count.</param>
    /// <returns>The index of the original row in the data table corresponding to the index of the filtered row. If the index of the filtered row is equal to or higher than the total number
    /// of filtered rows, the return value is <c>null</c>.</returns>
    public static int? GetOriginalRowIndexForFilteredRowIndex(this IRowSelection rowSelection, int filteredRowIndex, DataColumnCollection? table, int totalRowCount)
    {
      var remaining = filteredRowIndex;
      foreach (var segment in rowSelection.GetSelectedRowIndexSegmentsFromTo(0, int.MaxValue, table, totalRowCount))
      {
        var segmentLength = segment.endExclusive - segment.start;
        if (remaining < segmentLength)
          return segment.start + remaining;
        else
          remaining -= segmentLength;
      }
      return null;
    }

    /// <summary>
    /// Gets the index of the filtered row by providing the index of the original row in the data table.
    /// </summary>
    /// <param name="rowSelection">The row selection (the filter).</param>
    /// <param name="originalRowIndex">Index of the original row in the data table.</param>
    /// <param name="table">The underlying data table.</param>
    /// <param name="totalRowCount">The total row count.</param>
    /// <returns>The index of the filtered row corresponding to the index of the original row in the data table. If the index of the original row is not included by the filter,
    /// the return value is <c>null</c>.</returns>
    public static int? GetFilteredRowIndexForOriginalRowIndex(this IRowSelection rowSelection, int originalRowIndex, DataColumnCollection? table, int totalRowCount)
    {
      var filteredRowIndexOffset = 0;
      foreach (var segment in rowSelection.GetSelectedRowIndexSegmentsFromTo(0, totalRowCount, table, totalRowCount))
      {
        if (originalRowIndex < segment.start)
          break;
        if (originalRowIndex < segment.endExclusive)
          return filteredRowIndexOffset + (originalRowIndex - segment.start);
        else
          filteredRowIndexOffset += segment.endExclusive - segment.start;
      }
      return null;
    }
  }
}
