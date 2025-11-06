#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using System.Linq;
using Altaxo.Collections;

namespace Altaxo.Data
{
  public static class Sorting
  {
    #region Helper classes

    #region Sorting foundation

    public delegate int RowComparismMethod(int i, int j);

    public delegate void RowSwapMethod(int i, int j);

    private static void downheap(int N, int k, RowComparismMethod CMP, RowSwapMethod swap)
    {
      while (k <= N / 2)
      {
        int j = 2 * k;

        if (j < N && CMP(j, j + 1) < 0)
        {
          j++;
        }

        if (CMP(k, j) < 0)
        {
          swap(j, k);
        }
        else
        {
          break;
        }

        k = j;
      }
    }

    /// <summary>
    /// Sorts elements in an order provided by the comparism method. Needs N log N operations. Worst case (already sorted) is something like 20% slower.
    /// </summary>
    /// <param name="count">Number of elements to sort (from indices 0 to count-1).</param>
    /// <param name="compare">A compare method wich takes two indices and compares the elements at those indices.</param>
    /// <param name="swap">A swap method which swaps the elements at two indices.</param>
    private static void HeapSort(int count, RowComparismMethod compare, RowSwapMethod swap)
    {
      int N;
      int k;
      if (count == 0)
      {
        return;                   // No data to sort
      }

      /* We have n_data elements, last element is at 'n_data-1', first at
                 '0' Set N to the last element number. */

      N = count - 1;

      k = N / 2;
      k++;                          // Compensate the first use of 'k--'
      do
      {
        k--;
        downheap(N, k, compare, swap);
      }
      while (k > 0);

      while (N > 0)
      {
        // first swap the elements
        swap(0, N);

        // then process the heap
        N--;

        downheap(N, 0, compare, swap);
      }
    }

    #endregion Sorting foundation

    /// <summary>
    /// Sorts the elements, but maintains the original order in the provided array. Instead, an array of indices is created. The elements are
    /// sorted in the sense that elementsToSort[indexArray[i]] is sorted afterwards. The standard comparer of the elements is used for comparison.
    /// </summary>
    /// <param name="elementsToSort">The list of elements to sort. The list is not changed, thus it can be readonly.</param>
    /// <returns>An array of indices, so that elementsToSort[indexArray[i]] (i = 0..Count-1) is sorted.</returns>
    public static int[] HeapSortVirtually<T>(IList<T> elementsToSort) where T : IComparable
    {
      return HeapSortVirtually(elementsToSort, null);
    }

    public static int[] CreateIdentityIndices(int count)
    {
      var result = new int[count];
      for (int i = count - 1; i >= 0; i--)
        result[i] = i;
      return result;
    }

    public static void ReverseArray(int[] arr)
    {
      int count = arr.Length;
      for (int i = 0, j = count - 1; i < j; i++, j--)
      {
        (arr[j], arr[i]) = (arr[i], arr[j]);
      }
    }

    /// <summary>
    /// Sorts the elements, but maintains the original order in the provided array. Instead, an array of indices is created. The elements are
    /// sorted in the sense that elementsToSort[indexArray[i]] is sorted afterwards. The standard comparer of the elements is used for comparison.
    /// </summary>
    /// <param name="elementsToSort">The list of elements to sort. The list is not changed, thus it can be readonly.</param>
    /// <param name="destinationIndexArray">Can be null. If you provide an array here with a length greater or equal to the number of elements to sort, that array is used as return value and filled with the indices.</param>
    /// <returns>An array of indices, so that elementsToSort[indexArray[i]] (i = 0..Count-1) is sorted.</returns>
    public static int[] HeapSortVirtually<T>(IList<T> elementsToSort, int[]? destinationIndexArray) where T : IComparable
    {
      if (destinationIndexArray is null || destinationIndexArray.Length < elementsToSort.Count)
        destinationIndexArray = new int[elementsToSort.Count];
      for (int i = elementsToSort.Count - 1; i >= 0; i--)
        destinationIndexArray[i] = i;

      HeapSort(
        elementsToSort.Count,
        delegate (int i, int j)
        { return elementsToSort[destinationIndexArray[i]].CompareTo(elementsToSort[destinationIndexArray[j]]); },
        delegate (int i, int j)
        { int ti = destinationIndexArray[i]; destinationIndexArray[i] = destinationIndexArray[j]; destinationIndexArray[j] = ti; }
      );

      return destinationIndexArray;
    }

    private class DataColumnCollectionRowSwapper
    {
      private List<DataColumn> _colsToSwap;

      public DataColumnCollectionRowSwapper(DataColumnCollection coll, int forColumnGroup)
      {
        _colsToSwap = [];
        for (int i = 0; i < coll.ColumnCount; ++i)
        {
          if (coll.GetColumnGroup(i) == forColumnGroup)
            _colsToSwap.Add(coll[i]);
        }
      }

      public void Swap(int i, int j)
      {
        foreach (DataColumn col in _colsToSwap)
        {
          (col[j], col[i]) = (col[i], col[j]);
        }
      }
    }

    private class DataTableColumnSwapper
    {
      private DataTable _table;
      private int _propColumns;

      public DataTableColumnSwapper(DataTable table)
      {
        _table = table;
        _propColumns = _table.PropertyColumnCount;
      }

      public void Swap(int i, int j)
      {
        _table.DataColumns.SwapColumnPositions(i, j);
        DataColumnCollection prop = _table.PropCols;
        for (int n = 0; n < _propColumns; n++)
        {
          (prop[n][j], prop[n][i]) = (prop[n][i], prop[n][j]);
        }
      }
    }

    private class DataTableSelectedColumnSwapper
    {
      private DataTable _table;
      private int _propColumns;
      private IAscendingIntegerCollection _selIndices;

      public DataTableSelectedColumnSwapper(DataTable table, IAscendingIntegerCollection selectedDataColumns)
      {
        _table = table;
        _propColumns = _table.PropertyColumnCount;
        _selIndices = selectedDataColumns;
      }

      public void Swap(int i, int j)
      {
        int iNew = _selIndices[i];
        int jNew = _selIndices[j];

        _table.DataColumns.SwapColumnPositions(iNew, jNew);
        DataColumnCollection prop = _table.PropCols;
        for (int n = 0; n < _propColumns; n++)
        {
          (prop[n][jNew], prop[n][iNew]) = (prop[n][iNew], prop[n][jNew]);
        }
      }
    }

    private class DoubleColumnComparer
    {
      private DoubleColumn _col;
      private bool _ascendingOrder;
      private bool _treatEmptyAsLowest;

      public DoubleColumnComparer(DoubleColumn sortCol, bool ascending, bool treatEmptyElementAsLowest)
      {
        _col = sortCol;
        _ascendingOrder = ascending;
        _treatEmptyAsLowest = treatEmptyElementAsLowest;
      }

      public int Compare(int i, int j)
      {
        var ai = _col[i];
        var aj = _col[j];

        if (ai < aj)
          return _ascendingOrder ? -1 : 1;
        else if (ai > aj)
          return _ascendingOrder ? +1 : -1;
        else if (double.IsNaN(ai) && !double.IsNaN(aj))
          return (_ascendingOrder ^ _treatEmptyAsLowest) ? +1 : +1;
        else if (!double.IsNaN(ai) && double.IsNaN(aj))
          return (_ascendingOrder ^ _treatEmptyAsLowest) ? -1 : +1;
        else
          return 0;
      }
    }

    private class SelectedDoubleColumnComparer
    {
      private DoubleColumn _col;
      private bool _ascendingOrder;
      private IAscendingIntegerCollection _selIndices;
      private bool _treatEmptyAsLowest;


      public SelectedDoubleColumnComparer(DoubleColumn sortCol, IAscendingIntegerCollection atSelectedIndices, bool ascending, bool treatEmptyElementsAsLowest)
      {
        _col = sortCol;
        _ascendingOrder = ascending;
        _selIndices = atSelectedIndices;
        _treatEmptyAsLowest = treatEmptyElementsAsLowest;
      }

      public int Compare(int i, int j)
      {
        var ai = _col[_selIndices[i]];
        var aj = _col[_selIndices[j]];

        if (ai < aj)
          return _ascendingOrder ? -1 : 1;
        else if (ai > aj)
          return _ascendingOrder ? +1 : -1;
        else if (double.IsNaN(ai) && !double.IsNaN(aj))
          return (_ascendingOrder ^ _treatEmptyAsLowest) ? +1 : +1;
        else if (!double.IsNaN(ai) && double.IsNaN(aj))
          return (_ascendingOrder ^ _treatEmptyAsLowest) ? -1 : +1;
        else
          return 0;
      }
    }

    private class DataColumnComparer
    {
      private DataColumn _col;
      private bool _ascendingOrder;
      private bool _treatEmptyAsLowest;

      public DataColumnComparer(DataColumn sortCol, bool ascending, bool treatEmptyElementAsLowest)
      {
        _col = sortCol;
        _ascendingOrder = ascending;
      }

      public int Compare(int i, int j)
      {
        if (_col[i] < _col[j])
          return _ascendingOrder ? -1 : 1;
        else if (_col[i] > _col[j])
          return _ascendingOrder ? +1 : -1;
        else if (_col.IsElementEmpty(i) && !_col.IsElementEmpty(j))
          return (_ascendingOrder ^ _treatEmptyAsLowest) ? +1 : +1;
        else if (!_col.IsElementEmpty(i) && _col.IsElementEmpty(j))
          return (_ascendingOrder ^ _treatEmptyAsLowest) ? -1 : +1;
        else
          return 0;
      }
    }

    private class SelectedDataColumnComparer
    {
      private DataColumn _col;
      private bool _ascendingOrder;
      private IAscendingIntegerCollection _selIndices;
      private bool _treatEmptyAsLowest;


      public SelectedDataColumnComparer(DataColumn sortCol, IAscendingIntegerCollection atSelectedIndices, bool ascending, bool treatEmptyElementsAsLowest)
      {
        _col = sortCol;
        _ascendingOrder = ascending;
        _selIndices = atSelectedIndices;
        _treatEmptyAsLowest = treatEmptyElementsAsLowest;
      }

      public int Compare(int i, int j)
      {
        int ii = _selIndices[i];
        int jj = _selIndices[j];

        if (_col[ii] < _col[jj])
          return _ascendingOrder ? -1 : 1;
        else if (_col[ii] > _col[jj])
          return _ascendingOrder ? +1 : -1;
        else if (_col.IsElementEmpty(ii) && !_col.IsElementEmpty(jj))
          return (_ascendingOrder ^ _treatEmptyAsLowest) ? +1 : +1;
        else if (!_col.IsElementEmpty(ii) && _col.IsElementEmpty(jj))
          return (_ascendingOrder ^ _treatEmptyAsLowest) ? -1 : +1;
        else
          return 0;
      }
    }

    private class MultipleDataColumnComparer
    {
      /// <summary>
      /// Represents an array of tuples, where each tuple contains a <see cref="DataColumn"/> and a boolean indicating
      /// the sort order.
      /// </summary>
      /// <remarks>The <see cref="DataColumn"/> in each tuple specifies the column to be sorted, and the
      /// boolean value indicates the sort order. A value of <see langword="true"/> represents ascending order, while
      /// <see langword="false"/> represents descending order.</remarks>
      private (DataColumn col, bool ascendingOrder)[] _cols;

      /// <summary>
      /// Indicates whether empty values should be treated as the lowest possible value in comparisons.
      /// </summary>
      /// <remarks>When set to <see langword="true"/>, empty values are considered lower than any other
      /// value during comparisons.  When set to <see langword="false"/>, empty values are considered higher than any other value.</remarks>
      private bool _treatEmptyAsLowest;

      /// <summary>
      /// When the sort criterion are property columns, sometimes not all data columns are sorted, but only a selection of them.
      /// This collection holds the indices of the data columns to sort.
      /// </summary>
      private IAscendingIntegerCollection? _selectedDataColumns;


      public MultipleDataColumnComparer(DataColumn[] sortCols, bool ascending, bool treatEmptyElementsAsLowest)
      {
        _cols = new (DataColumn col, bool ascendingOrder)[sortCols.Length];
        for (int k = 0; k < sortCols.Length; k++)
        {
          _cols[k] = (sortCols[k], ascending);
        }

        _treatEmptyAsLowest = treatEmptyElementsAsLowest;
      }

      public MultipleDataColumnComparer(DataColumn[] sortCols, bool[] ascending, bool treatEmptyElementsAsLowest)
      {
        if (sortCols.Length != ascending.Length)
          throw new ArgumentException("sortCols and ascending must have the same length");

        _cols = new (DataColumn col, bool ascendingOrder)[sortCols.Length];
        for (int k = 0; k < sortCols.Length; k++)
        {
          _cols[k] = (sortCols[k], ascending[k]);
        }
        _treatEmptyAsLowest = treatEmptyElementsAsLowest;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="MultipleDataColumnComparer"/> class, which compares multiple data
      /// columns based on specified sorting orders and options.
      /// </summary>
      /// <param name="cols">A read-only list of tuples, where each tuple contains a <see cref="DataColumn"/> to compare and a boolean
      /// indicating whether the comparison for that column should be in ascending order.</param>
      /// <param name="selectedDataColumns">A collection of integers representing the indices of the data columns to be included in the comparison.</param>
      /// <param name="treatEmptyElementsAsLowest">A boolean value indicating whether empty elements should be treated as the lowest value during comparison. If
      /// <see langword="true"/>, empty elements are considered lower than any other value; otherwise, they are treated
      /// as higher.</param>
      public MultipleDataColumnComparer(IReadOnlyList<(DataColumn column, bool inAscendingOrder)> cols, IAscendingIntegerCollection selectedDataColumns, bool treatEmptyElementsAsLowest)
      {
        _cols = cols.ToArray();
        _selectedDataColumns = selectedDataColumns;
        _treatEmptyAsLowest = treatEmptyElementsAsLowest;
      }

      public int Compare(int i, int j)
      {
        int ii = _selectedDataColumns is null ? i : _selectedDataColumns[i];
        int jj = _selectedDataColumns is null ? j : _selectedDataColumns[j];

        foreach (var (col, ascendingOrder) in _cols)
        {
          if (col[ii] < col[jj])
            return ascendingOrder ? -1 : +1;
          else if (col[ii] > col[jj])
            return ascendingOrder ? +1 : -1;
          else if (col.IsElementEmpty(ii) && !col.IsElementEmpty(jj))
            return (ascendingOrder ^ _treatEmptyAsLowest) ? +1 : -1;
          else if (!col.IsElementEmpty(ii) && col.IsElementEmpty(jj))
            return (ascendingOrder ^ _treatEmptyAsLowest) ? -1 : +1;
        }
        return 0;
      }
    }

    #endregion Helper classes

    /// <summary>
    /// Sorts the data rows of a table (more accurate: of all columns belonging to a column group, see below), using the data of column <paramref name="col"/> to determine the order.
    /// </summary>
    /// <param name="table">The table where the data columns should be sorted.</param>
    /// <param name="col">The column which is used for determining the order of the entries.
    /// This column has to belong to the table (otherwise an exception will be thrown). All columns with the same group number than this column will be sorted.</param>
    /// <param name="inAscendingOrder">If true, the table is sorted in ascending order. Otherwise, the table is sorted in descending order.</param>
    /// <param name="treatEmptyElementsAsLowest">If true, empty elements are treated as lower as any other non-empty element. If false, empty elements are treated as higher as any non-empty element.</param>
    public static void SortDataRows(this DataTable table, DataColumn col, bool inAscendingOrder, bool treatEmptyElementsAsLowest)
    {
      SortRows(table.DataColumns, col, inAscendingOrder, treatEmptyElementsAsLowest);
    }

    /// <summary>
    /// Sorts the data rows of a table (more accurate: of all columns belonging to a column group, see below), using multiple specified column.
    /// </summary>
    /// <param name="table">The table where the data columns should be sorted.</param>
    /// <param name="cols">The columns which are used for determining the order of the entries. The sorting will be done by cols[0], then cols[1] and so on.
    /// All this columns has to belong to the table and need to have the same column group number (otherwise an exception will be thrown). All columns with the same group number than this columns will be included in the sort process.</param>
    /// <param name="inAscendingOrder">If true, the table is sorted in ascending order. Otherwise, the table is sorted in descending order.</param>
    /// <param name="treatEmptyElementsAsLowest">If true, empty elements are treated as lower as any other non-empty element. If false, empty elements are treated as higher as any non-empty element.</param>
    public static void SortDataRows(this DataTable table, DataColumn[] cols, bool inAscendingOrder, bool treatEmptyElementsAsLowest)
    {
      SortRows(table.DataColumns, cols, inAscendingOrder, treatEmptyElementsAsLowest: treatEmptyElementsAsLowest);
    }

    /// <summary>
    /// Sorts the data rows of a DataColumnCollection (more accurate: of all columns belonging to a column group, see below), using a specified column.
    /// </summary>
    /// <param name="table">The DataColumnCollection where the data columns should be sorted.</param>
    /// <param name="col">The column which is used for determining the order of the entries.
    /// This column has to belong to the table (otherwise an exception will be thrown). All columns with the same group number than this column will be included in the sort process.</param>
    /// <param name="inAscendingOrder">If true, the table is sorted in ascending order. Otherwise, the table is sorted in descending order.</param>
    /// <param name="treatEmptyElementsAsLowest">If true, empty elements are treated as lower as any other non-empty element. If false, empty elements are treated as higher as any non-empty element.</param>
    public static void SortRows(this DataColumnCollection table, DataColumn col, bool inAscendingOrder, bool treatEmptyElementsAsLowest)
    {
      if (!table.ContainsColumn(col))
        throw new ArgumentException("The sorting column provided must be part of the DataColumnCollection (otherwise the swap algorithm can not sort this column)");

      using (var token = table.SuspendGetToken())
      {
        int columnGroup = table.GetColumnGroup(col);
        if (col is DoubleColumn)
        {
          HeapSort(col.Count, new DoubleColumnComparer((DoubleColumn)col, inAscendingOrder, treatEmptyElementsAsLowest).Compare, new DataColumnCollectionRowSwapper(table, columnGroup).Swap);
        }
        else
        {
          HeapSort(col.Count, new DataColumnComparer(col, inAscendingOrder, treatEmptyElementsAsLowest).Compare, new DataColumnCollectionRowSwapper(table, columnGroup).Swap);
        }
      }
    }

    /// <summary>
    /// Sorts the data rows of a DataColumnCollection (more accurate: of all columns belonging to a column group, see below), using a specified column.
    /// </summary>
    /// <param name="table">The DataColumnCollection where the data columns should be sorted.</param>
    /// <param name="cols">The columns which are used for determining the order of the entries. The sorting will be done by cols[0], then cols[1] and so on.
    /// All this columns has to belong to the table and need to have the same column group number (otherwise an exception will be thrown). All columns with the same group number than this columns will be included in the sort process.</param>
    /// <param name="inAscendingOrder">If true, the table is sorted in ascending order. Otherwise, the table is sorted in descending order.</param>
    /// <param name="treatEmptyElementsAsLowest">If true, empty elements are treated as lower as any other non-empty element. If false, empty elements are treated as higher as any non-empty element.</param>
    public static void SortRows(this DataColumnCollection table, DataColumn[] cols, bool inAscendingOrder, bool treatEmptyElementsAsLowest)
    {
      if (cols is null || cols.Length == 0)
        throw new ArgumentException("cols is null or empty");

      using (var token = table.SuspendGetToken())
      {
        int groupNumber = table.GetColumnGroup(cols[0]);
        for (int i = 1; i < cols.Length; i++)
        {
          if (groupNumber != table.GetColumnGroup(cols[i]))
            throw new ArgumentException(string.Format("cols[{0}] has a deviating group number from cols[0]. Only columns belonging to the same group can be sorted", i));
        }

        for (int k = 0; k < cols.Length; k++)
        {
          if (!table.ContainsColumn(cols[k]))
            throw new ArgumentException("The sorting columnd provided must all be part of the DataColumnCollection (otherwise the swap algorithm can not sort this column)");
        }

        HeapSort(cols[0].Count, new MultipleDataColumnComparer(cols, inAscendingOrder, treatEmptyElementsAsLowest: treatEmptyElementsAsLowest).Compare, new DataColumnCollectionRowSwapper(table, groupNumber).Swap);
      }
    }

    /// <summary>
    /// Sorts the data rows of a table (more accurate: of all columns belonging to a column group, see below), using multiple specified column.
    /// </summary>
    /// <param name="table">The table where the data columns should be sorted.</param>
    /// <param name="cols">The columns which are used for determining the order of the entries. The sorting will be done by cols[0], then cols[1] and so on.
    /// All this columns has to belong to the table and need to have the same column group number (otherwise an exception will be thrown). All columns with the same group number than this columns will be included in the sort process.</param>
    /// <param name="treatEmptyElementsAsLowest">If true, empty elements are treated as lower as any other non-empty element. If false, empty elements are treated as higher as any non-empty element.</param>
    public static void SortRows(this DataColumnCollection table, IReadOnlyList<(DataColumn column, bool inAscendingOrder)> cols, bool treatEmptyElementsAsLowest)
    {
      if (cols is null || cols.Count == 0)
        throw new ArgumentException("cols is null or empty");

      int groupNumber = table.GetColumnGroup(cols[0].column);
      for (int i = 1; i < cols.Count; i++)
      {
        if (groupNumber != table.GetColumnGroup(cols[i].column))
          throw new ArgumentException(string.Format("cols[{0}] has a deviating group number from cols[0]. Only columns belonging to the same group can be sorted", i));
      }

      int rowCount = 0;
      for (int k = 0; k < cols.Count; k++)
      {
        if (!table.ContainsColumn(cols[k].column))
          throw new ArgumentException("The sorting columnd provided must all be part of the DataColumnCollection (otherwise the swap algorithm can not sort this column)");

        rowCount = Math.Max(rowCount, cols[k].column.Count);
      }


      var dataColumns = cols.Select(c => c.column).ToArray();
      var inAscendingOrder = cols.Select(c => c.inAscendingOrder).ToArray();
      using (var token = table.SuspendGetToken())
      {
        HeapSort(rowCount, new MultipleDataColumnComparer(dataColumns, inAscendingOrder, treatEmptyElementsAsLowest: treatEmptyElementsAsLowest).Compare, new DataColumnCollectionRowSwapper(table, groupNumber).Swap);
      }
    }

    /// <summary>
    /// Sort the order of the data columns (not rows!) of a table based on a specified property column. The relationship of property data to data columns is maintained.
    /// </summary>
    /// <param name="table">The table where to sort the columns.</param>
    /// <param name="propCol">The property column where the sorting order is based on.</param>
    /// <param name="inAscendingOrder">If true, the sorting is in ascending order. If false, the sorting is in descending order.</param>
    /// <param name="treatEmptyElementsAsLowest">If true, empty elements are treated as lower as any other non-empty element. If false, empty elements are treated as higher as any non-empty element.</param>
    public static void SortDataColumnsByPropertyColumn(this DataTable table, DataColumn propCol, bool inAscendingOrder, bool treatEmptyElementsAsLowest)
    {
      if (!table.PropCols.ContainsColumn(propCol))
        throw new ArgumentException("The sorting column provided must be part of the table.PropertyColumnCollection (otherwise the swap algorithm can not sort this column)");

      using (var token = table.SuspendGetToken())
      {
        if (propCol is DoubleColumn)
        {
          HeapSort(propCol.Count, new DoubleColumnComparer((DoubleColumn)propCol, inAscendingOrder, treatEmptyElementsAsLowest).Compare, new DataTableColumnSwapper(table).Swap);
        }
        else
        {
          HeapSort(propCol.Count, new DataColumnComparer(propCol, inAscendingOrder, treatEmptyElementsAsLowest).Compare, new DataTableColumnSwapper(table).Swap);
        }
      }
    }

    /// <summary>
    /// Sort the order of the data columns (not rows!) of a table based on a specified property column. The relationship of property data to data columns is maintained.
    /// </summary>
    /// <param name="table">The table where to sort the columns.</param>
    /// <param name="selectedDataCols">Data columns to sort.</param>
    /// <param name="propCol">The property column where the sorting order is based on.</param>
    /// <param name="inAscendingOrder">If true, the sorting is in ascending order. If false, the sorting is in descending order.</param>
    /// <param name="treatEmptyElementsAsLowest">If true, empty elements are treated as lower as any other non-empty element. If false, empty elements are treated as higher as any non-empty element.</param>
    public static void SortDataColumnsByPropertyColumn(this DataTable table, IAscendingIntegerCollection selectedDataCols, DataColumn propCol, bool inAscendingOrder, bool treatEmptyElementsAsLowest)
    {
      if (!table.PropCols.ContainsColumn(propCol))
        throw new ArgumentException("The sorting column provided must be part of the table.PropertyColumnCollection (otherwise the swap algorithm can not sort this column)");

      using (var token = table.SuspendGetToken())
      {
        if (propCol is DoubleColumn)
        {
          HeapSort(selectedDataCols.Count, new SelectedDoubleColumnComparer((DoubleColumn)propCol, selectedDataCols, inAscendingOrder, treatEmptyElementsAsLowest).Compare, new DataTableSelectedColumnSwapper(table, selectedDataCols).Swap);
        }
        else
        {
          HeapSort(selectedDataCols.Count, new SelectedDataColumnComparer(propCol, selectedDataCols, inAscendingOrder, treatEmptyElementsAsLowest).Compare, new DataTableSelectedColumnSwapper(table, selectedDataCols).Swap);
        }
      }
    }

    /// <summary>
    /// Sort the order of the data columns (not rows!) of a table based on multiple specified property columns. The relationship of property data to data columns is maintained.
    /// </summary>
    /// <param name="table">The table containing the property columns used as sort criteria.</param>
    /// <param name="cols">List of tuples of property columns and the correpsonding sort direction. The first element has highest priority.</param>
    /// <param name="sortColumnsOnlyFromPropertyGroup">If true, only those data columns are moved in position, that have the same group number as the property column that are used as sort criterion.</param>
    /// <param name="treatEmptyElementsAsLowest">If true, empty elements (e.g, NaN) are treated as lower as any other non-empty element. If false, empty elements are treated as higher as any other non-empty element.</param>
    public static void SortDataColumnsByPropertyColumns(this DataTable table, IReadOnlyList<(DataColumn column, bool inAscendingOrder)> cols, bool sortColumnsOnlyFromPropertyGroup, bool treatEmptyElementsAsLowest)
    {
      if (cols is null || cols.Count == 0)
        throw new ArgumentException("cols is null or empty");

      int groupNumber = table.PropCols.GetColumnGroup(cols[0].column);
      for (int i = 1; i < cols.Count; i++)
      {
        if (groupNumber != table.PropCols.GetColumnGroup(cols[i].column))
          throw new ArgumentException(string.Format("cols[{0}] has a deviating group number from cols[0]. Only columns belonging to the same group can be sorted", i));
      }

      var propertyColumns = cols.Select(c => c.column).ToArray();
      var inAscendingOrder = cols.Select(c => c.inAscendingOrder).ToArray();

      AscendingIntegerCollection selectedDataColumns = new AscendingIntegerCollection();
      if (sortColumnsOnlyFromPropertyGroup)
      {
        for (int i = 0; i < table.DataColumnCount; i++)
        {
          if (table.DataColumns.GetColumnGroup(i) == groupNumber)
          {
            selectedDataColumns.Add(i);
          }
        }
      }
      else
      {
        selectedDataColumns.AddRange(0, table.DataColumnCount);
      }

      using (var token = table.SuspendGetToken())
      {
        HeapSort(selectedDataColumns.Count, new MultipleDataColumnComparer(cols, selectedDataColumns, treatEmptyElementsAsLowest).Compare, new DataTableSelectedColumnSwapper(table, selectedDataColumns).Swap);
      }
    }
  }
}
