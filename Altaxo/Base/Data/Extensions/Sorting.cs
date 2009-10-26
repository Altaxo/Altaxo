using System;
using System.Collections.Generic;
using System.Text;


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
    /// <param name="count">Number of elements to sort.</param>
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

    class DataColumnCollectionRowSwapper
    {
      DataColumnCollection _data;
      int _columns;
      AltaxoVariant _var;

      public DataColumnCollectionRowSwapper(DataColumnCollection coll)
      {
        _data = coll;
        _columns = _data.ColumnCount;
      }

      public void Swap(int i, int j)
      {
        for(int n=0;n<_columns;n++)
        {
          _var = _data[n][i];
          _data[n][i] = _data[n][j];
          _data[n][j] = _var;
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

		#endregion


		/// <summary>
		/// Sorts the data rows of a table, using a specified column.
		/// </summary>
		/// <param name="table">The table where the data columns should be sorted.</param>
		/// <param name="col">The column which is used for determining the order of the entries.</param>
		/// <param name="inAscendingOrder">If true, the table is sorted in ascending order. Otherwise, the table is sorted in descending order.</param>
		public static void SortDataRows(this DataTable table, DataColumn col, bool inAscendingOrder)
    {
			SortRows(table.DataColumns, col, inAscendingOrder);
    }

		/// <summary>
		/// Sorts the data rows of a DataColumnCollection, using a specified column.
		/// </summary>
		/// <param name="table">The DataColumnCollection where the data columns should be sorted.</param>
		/// <param name="col">The column which is used for determining the order of the entries.</param>
		/// <param name="inAscendingOrder">If true, the table is sorted in ascending order. Otherwise, the table is sorted in descending order.</param>
		public static void SortRows(this DataColumnCollection table, DataColumn col, bool inAscendingOrder)
		{
			if (!table.ContainsColumn(col))
				throw new ArgumentException("The sorting column provided must be part of the DataColumnCollection (otherwise the swap algorithm can not sort this column)");

			if (col is DoubleColumn)
			{
				HeapSort(col.Count, new DoubleColumnComparer((DoubleColumn)col, inAscendingOrder).Compare, new DataColumnCollectionRowSwapper(table).Swap);
			}
			else
			{
				HeapSort(col.Count, new DataColumnComparer(col, inAscendingOrder).Compare, new DataColumnCollectionRowSwapper(table).Swap);
			}
		}

		/// <summary>
		/// Sort the order of the data columns (not rows!) of a table based on a specified property column. The relationship of property data to data columns is maintained.
		/// </summary>
		/// <param name="table">The table where to sort the columns.</param>
		/// <param name="propCol">The property column where the sorting order is based on.</param>
		/// <param name="inAscendingOrder">If true, the sorting is in ascending order. If false, the sorting is in descending order.</param>
    public static void SortColumnsByPropertyColumn(this DataTable table, DataColumn propCol, bool inAscendingOrder)
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
  }
}

