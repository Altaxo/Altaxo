using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Collections;

namespace Altaxo.Data
{
  public static class Sorting
	{
		#region Helper classes

		#region Sorting foundation
		public delegate int RowComparismMethod(int i, int j);
    public delegate void RowSwapMethod(int i, int j);



    static void downheap(int N, int k, RowComparismMethod CMP, RowSwapMethod swap)
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
    static void HeapSort(int count, RowComparismMethod compare, RowSwapMethod swap)
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
    #endregion


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
				int ai = arr[i];
				arr[i] = arr[j];
				arr[j] = ai;
			}
		}

		/// <summary>
		/// Sorts the elements, but maintains the original order in the provided array. Instead, an array of indices is created. The elements are
		/// sorted in the sense that elementsToSort[indexArray[i]] is sorted afterwards. The standard comparer of the elements is used for comparison.
		/// </summary>
		/// <param name="elementsToSort">The list of elements to sort. The list is not changed, thus it can be readonly.</param>
		/// <param name="destinationIndexArray">Can be null. If you provide an array here with a length greater or equal to the number of elements to sort, that array is used as return value and filled with the indices.</param>
		/// <returns>An array of indices, so that elementsToSort[indexArray[i]] (i = 0..Count-1) is sorted.</returns>
		public static int[] HeapSortVirtually<T>(IList<T> elementsToSort, int[] destinationIndexArray) where T : IComparable
		{
			if (destinationIndexArray == null || destinationIndexArray.Length < elementsToSort.Count)
				destinationIndexArray = new int[elementsToSort.Count];
			for (int i = elementsToSort.Count - 1; i >= 0; i--)
				destinationIndexArray[i] = i;

			HeapSort(
				elementsToSort.Count,
				delegate(int i, int j) { return elementsToSort[destinationIndexArray[i]].CompareTo(elementsToSort[destinationIndexArray[j]]); },
				delegate(int i, int j) { int ti = destinationIndexArray[i]; destinationIndexArray[i] = destinationIndexArray[j]; destinationIndexArray[j] = ti; }
			);

			return destinationIndexArray;
		}

    class DataColumnCollectionRowSwapper
    {
			List<DataColumn> _colsToSwap;
      AltaxoVariant _var;

      public DataColumnCollectionRowSwapper(DataColumnCollection coll, int forColumnGroup)
      {
				_colsToSwap = new List<DataColumn>();
				for (int i = 0; i < coll.ColumnCount; ++i)
				{
					if (coll.GetColumnGroup(i) == forColumnGroup)
						_colsToSwap.Add(coll[i]);
				}
      }

			public void Swap(int i, int j)
      {
        foreach(DataColumn col in _colsToSwap)
        {
						_var = col[i];
						col[i] = col[j];
						col[j] = _var;
        }
      }
    }

    class DataTableColumnSwapper
    {
      DataTable _table;
      int _propColumns;

      AltaxoVariant _var;

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
          _var = prop[n][i];
          prop[n][i] = prop[n][j];
          prop[n][j] = _var;
        }
      }
    }

		class DataTableSelectedColumnSwapper
		{
			DataTable _table;
			int _propColumns;
			IAscendingIntegerCollection _selIndices;

			AltaxoVariant _var;

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
					_var = prop[n][iNew];
					prop[n][iNew] = prop[n][jNew];
					prop[n][jNew] = _var;
				}
			}
		}

    class DoubleColumnComparer
    {
      DoubleColumn _col;
      bool _ascendingOrder;
      public DoubleColumnComparer(DoubleColumn sortCol, bool ascending)
      {
        _col = sortCol;
        _ascendingOrder = ascending;
      }

      public int Compare(int i, int j)
      {
        return _ascendingOrder ? _col[i].CompareTo(_col[j]) : _col[j].CompareTo(_col[i]);
      }
    }

		class SelectedDoubleColumnComparer
		{
			DoubleColumn _col;
			bool _ascendingOrder;
			IAscendingIntegerCollection _selIndices;

			public SelectedDoubleColumnComparer(DoubleColumn sortCol, IAscendingIntegerCollection atSelectedIndices, bool ascending)
			{
				_col = sortCol;
				_ascendingOrder = ascending;
				_selIndices = atSelectedIndices;
			}

			public int Compare(int i, int j)
			{
				int iNew = _selIndices[i];
				int jNew = _selIndices[j];
				return _ascendingOrder ? _col[iNew].CompareTo(_col[jNew]) : _col[jNew].CompareTo(_col[iNew]);
			}
		}

    class DataColumnComparer
    {
      DataColumn _col;
      bool _ascendingOrder;
      public DataColumnComparer(DataColumn sortCol, bool ascending)
      {
        _col = sortCol;
        _ascendingOrder = ascending;
      }

      public int Compare(int i, int j)
      {
        if (_col[i] == _col[j])
          return 0;
        else if (_col[i] < _col[j])
          return _ascendingOrder ? -1 : 1;
        else
          return _ascendingOrder ? +1 : -1;
      }
		}

		class SelectedDataColumnComparer
		{
			DataColumn _col;
			bool _ascendingOrder;
			IAscendingIntegerCollection _selIndices;
			public SelectedDataColumnComparer(DataColumn sortCol, IAscendingIntegerCollection atSelectedIndices, bool ascending)
			{
				_col = sortCol;
				_ascendingOrder = ascending;
				_selIndices = atSelectedIndices;
			}

			public int Compare(int i, int j)
			{
				int iNew = _selIndices[i];
				int jNew = _selIndices[j];

				if (_col[iNew] == _col[jNew])
					return 0;
				else if (_col[iNew] < _col[jNew])
					return _ascendingOrder ? -1 : 1;
				else
					return _ascendingOrder ? +1 : -1;
			}
		}

		class MultipleDataColumnComparer
		{
			DataColumn[] _col;
			bool _ascendingOrder;


			public MultipleDataColumnComparer(DataColumn[] sortCols, bool ascending)
			{
				_col = sortCols;
				_ascendingOrder = ascending;
			}

			public int Compare(int i, int j)
			{
				for(int k=0;k<_col.Length;k++)
				{
				if (_col[k][i] == _col[k][j])
					continue;
				else if (_col[k][i] < _col[k][j])
					return _ascendingOrder ? -1 : 1;
				else
					return _ascendingOrder ? +1 : -1;
				}
				return 0;
			}
		}


		#endregion


		/// <summary>
		/// Sorts the data rows of a table (more accurate: of all columns belonging to a column group, see below), using the data of column <paramref name="col"/> to determine the order. 
		/// </summary>
		/// <param name="table">The table where the data columns should be sorted.</param>
		/// <param name="col">The column which is used for determining the order of the entries.
		/// This column has to belong to the table (otherwise an exception will be thrown). All columns with the same group number than this column will be sorted.</param>
		/// <param name="inAscendingOrder">If true, the table is sorted in ascending order. Otherwise, the table is sorted in descending order.</param>
		public static void SortDataRows(this DataTable table, DataColumn col, bool inAscendingOrder)
    {
			SortRows(table.DataColumns, col, inAscendingOrder);
    }

		/// <summary>
		/// Sorts the data rows of a table (more accurate: of all columns belonging to a column group, see below), using multiple specified column.
		/// </summary>
		/// <param name="table">The table where the data columns should be sorted.</param>
		/// <param name="cols">The columns which are used for determining the order of the entries. The sorting will be done by cols[0], then cols[1] and so on.
		/// All this columns has to belong to the table and need to have the same column group number (otherwise an exception will be thrown). All columns with the same group number than this columns will be included in the sort process.</param>
		/// <param name="inAscendingOrder">If true, the table is sorted in ascending order. Otherwise, the table is sorted in descending order.</param>
		public static void SortDataRows(this DataTable table, DataColumn[] cols, bool inAscendingOrder)
		{
			SortRows(table.DataColumns, cols, inAscendingOrder);
		}

		/// <summary>
		/// Sorts the data rows of a DataColumnCollection (more accurate: of all columns belonging to a column group, see below), using a specified column.
		/// </summary>
		/// <param name="table">The DataColumnCollection where the data columns should be sorted.</param>
		/// <param name="col">The column which is used for determining the order of the entries.
		/// This column has to belong to the table (otherwise an exception will be thrown). All columns with the same group number than this column will be included in the sort process.</param>
		/// <param name="inAscendingOrder">If true, the table is sorted in ascending order. Otherwise, the table is sorted in descending order.</param>
		public static void SortRows(this DataColumnCollection table, DataColumn col, bool inAscendingOrder)
		{
			if (!table.ContainsColumn(col))
				throw new ArgumentException("The sorting column provided must be part of the DataColumnCollection (otherwise the swap algorithm can not sort this column)");

			int columnGroup = table.GetColumnGroup(col);

			if (col is DoubleColumn)
			{
				HeapSort(col.Count, new DoubleColumnComparer((DoubleColumn)col, inAscendingOrder).Compare, new DataColumnCollectionRowSwapper(table,columnGroup).Swap);
			}
			else
			{
				HeapSort(col.Count, new DataColumnComparer(col, inAscendingOrder).Compare, new DataColumnCollectionRowSwapper(table,columnGroup).Swap);
			}
		}


		/// <summary>
		/// Sorts the data rows of a DataColumnCollection (more accurate: of all columns belonging to a column group, see below), using a specified column.
		/// </summary>
		/// <param name="table">The DataColumnCollection where the data columns should be sorted.</param>
		/// <param name="cols">The columns which are used for determining the order of the entries. The sorting will be done by cols[0], then cols[1] and so on.
		/// ll this columns has to belong to the table and need to have the same column group number (otherwise an exception will be thrown). All columns with the same group number than this columns will be included in the sort process.</param>
		/// <param name="inAscendingOrder">If true, the table is sorted in ascending order. Otherwise, the table is sorted in descending order.</param>
		public static void SortRows(this DataColumnCollection table, DataColumn[] cols, bool inAscendingOrder)
		{
			if (cols == null || cols.Length == 0)
				throw new ArgumentException("cols is null or empty");

			int groupNumber = table.GetColumnGroup(cols[0]);
			for (int i = 1; i < cols.Length; i++)
			{
				if (groupNumber != table.GetColumnGroup(cols[i]))
					throw new ArgumentException(string.Format("cols[{0}] has a deviating group number from cols[0]. Only columns belonging to the same group can be sorted",i));
			}

			for (int k = 0; k < cols.Length; k++)
			{
				if (!table.ContainsColumn(cols[k]))
					throw new ArgumentException("The sorting columnd provided must all be part of the DataColumnCollection (otherwise the swap algorithm can not sort this column)");
			}

				HeapSort(cols[0].Count, new MultipleDataColumnComparer(cols, inAscendingOrder).Compare, new DataColumnCollectionRowSwapper(table,groupNumber).Swap);
		}

		/// <summary>
		/// Sort the order of the data columns (not rows!) of a table based on a specified property column. The relationship of property data to data columns is maintained.
		/// </summary>
		/// <param name="table">The table where to sort the columns.</param>
		/// <param name="propCol">The property column where the sorting order is based on.</param>
		/// <param name="inAscendingOrder">If true, the sorting is in ascending order. If false, the sorting is in descending order.</param>
    public static void SortDataColumnsByPropertyColumn(this DataTable table, DataColumn propCol, bool inAscendingOrder)
    {
      if (!table.PropCols.ContainsColumn(propCol))
        throw new ArgumentException("The sorting column provided must be part of the table.PropertyColumnCollection (otherwise the swap algorithm can not sort this column)");

      if (propCol is DoubleColumn)
      {
        HeapSort(propCol.Count, new DoubleColumnComparer((DoubleColumn)propCol, inAscendingOrder).Compare, new DataTableColumnSwapper(table).Swap);
      }
      else
      {
        HeapSort(propCol.Count, new DataColumnComparer(propCol, inAscendingOrder).Compare, new DataTableColumnSwapper(table).Swap);
      }
    }


		/// <summary>
		/// Sort the order of the data columns (not rows!) of a table based on a specified property column. The relationship of property data to data columns is maintained.
		/// </summary>
		/// <param name="table">The table where to sort the columns.</param>
		/// <param name="propCol">The property column where the sorting order is based on.</param>
		/// <param name="inAscendingOrder">If true, the sorting is in ascending order. If false, the sorting is in descending order.</param>
		public static void SortDataColumnsByPropertyColumn(this DataTable table, IAscendingIntegerCollection selectedDataCols, DataColumn propCol, bool inAscendingOrder)
		{
			if (!table.PropCols.ContainsColumn(propCol))
				throw new ArgumentException("The sorting column provided must be part of the table.PropertyColumnCollection (otherwise the swap algorithm can not sort this column)");

			if (propCol is DoubleColumn)
			{
				HeapSort(selectedDataCols.Count, new SelectedDoubleColumnComparer((DoubleColumn)propCol, selectedDataCols, inAscendingOrder).Compare, new DataTableSelectedColumnSwapper(table,selectedDataCols).Swap);
			}
			else
			{
				HeapSort(selectedDataCols.Count, new SelectedDataColumnComparer(propCol, selectedDataCols, inAscendingOrder).Compare, new DataTableSelectedColumnSwapper(table, selectedDataCols).Swap);
			}
		}

  }
}

