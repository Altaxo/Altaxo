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

using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Data
{
	/// <summary>
	/// Options for transposing a worksheet.
	/// </summary>
	public class TransposeOptions : ICloneable
	{
		private int _availableNumberOfDataColumns;
		private int _availableNumberOfPropertyColumns;

		/// <summary>
		/// Gets the available number of data columns.
		/// </summary>
		/// <value>
		/// The available number of data columns.
		/// </value>
		public int AvailableNumberOfDataColumns { get { return _availableNumberOfDataColumns; } }

		/// <summary>
		/// Gets the available number of property columns.
		/// </summary>
		/// <value>
		/// The available number of property columns.
		/// </value>
		public int AvailableNumberOfPropertyColumns { get { return _availableNumberOfPropertyColumns; } }

		/// <summary>
		/// Gets or sets the number of data columns to transpose.
		/// </summary>
		/// <value>
		/// The number of data columns to transpose.
		/// </value>
		public int DataColumnsMoveToPropertyColumns { get; set; }

		/// <summary>
		/// Gets or sets the number of property columns to transpose.
		/// </summary>
		/// <value>
		/// The number of property columns to transpose.
		/// </value>
		public int PropertyColumnsMoveToDataColumns { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TransposeOptions"/> class.
		/// </summary>
		/// <param name="availableNumberOfDataColumns">The available number of data columns.</param>
		/// <param name="availableNumberOfPropertyColumns">The available number of property columns.</param>
		public TransposeOptions(int availableNumberOfDataColumns, int availableNumberOfPropertyColumns)
		{
			_availableNumberOfDataColumns = availableNumberOfDataColumns;
			_availableNumberOfPropertyColumns = availableNumberOfPropertyColumns;
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	/// <summary>
	/// Contains methods to transpose a worksheet.
	/// </summary>
	public static class Transposing
	{
		/// <summary>
		/// Tests if the transpose of a table is possible.
		/// </summary>
		/// <param name="table">Table to test.</param>
		/// <param name="numConvertedDataColumns">Number of data columns (beginning from index 0) that will be converted to property columns.</param>
		/// <param name="indexOfProblematicColumn">On return, if transpose is not possible, will give the index of the first column which differs in type from the first transposed data column.</param>
		/// <returns>True when the transpose is possible without problems, false otherwise.</returns>
		public static bool TransposeIsPossible(this DataTable table, int numConvertedDataColumns, out int indexOfProblematicColumn)
		{
			if (numConvertedDataColumns < 0)
				throw new ArgumentOutOfRangeException("numConvertedDataColumns is less than zero");

			indexOfProblematicColumn = 0;
			if (numConvertedDataColumns >= table.DataColumnCount)
				return true; // when all columns convert to property columns, that will be no problem

			System.Type masterColumnType = table[numConvertedDataColumns].GetType();

			for (int i = numConvertedDataColumns + 1; i < table.DataColumnCount; i++)
			{
				if (table[i].GetType() != masterColumnType)
				{
					indexOfProblematicColumn = i;
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Transpose a worksheet.
		/// </summary>
		/// <param name="table">The table to transpose.</param>
		/// <param name="numConvertedDataColumns">Number of data columns that will be converted to property columns.</param>
		/// <param name="numConvertedPropertyColumns">Number of property columns that will become data columns.</param>
		/// <param name="allowUserInteraction">If set to true, and transpose is not possible without problems, the user will be ask to cancel the transpose.
		/// If set to false, the transpose will be performed anyway. (But you can ask if transpose is possible by calling <c>IsTransposePossible</c>.
		/// </param>
		/// <returns>Null if the transpose was performed without problems, otherwise a error message would be given.</returns>
		static public string Transpose(this DataTable table, int numConvertedDataColumns, int numConvertedPropertyColumns, bool allowUserInteraction)
		{
			int datacols = Math.Min(table.DataColumnCount, numConvertedDataColumns);
			int propcols = Math.Min(table.PropertyColumnCount, numConvertedPropertyColumns);

			// test if the transpose is possible
			int indexDifferentColumn;
			if (!TransposeIsPossible(table, datacols, out indexDifferentColumn))
			{
				if (allowUserInteraction)
				{
					string message = string.Format("The columns to transpose have not all the same type. The type of column[{0}] ({1}) differs from the type of column[{2}] ({3}). Continue anyway?",
				 indexDifferentColumn,
				 table[indexDifferentColumn].GetType(),
				 datacols,
				 table[datacols].GetType());

					bool result = Current.Gui.YesNoMessageBox(message, "Attention", false);
					if (result == false)
						return "Cancelled by user";
				}
			}

			string error = table.Transpose(datacols, propcols);
			if (error != null && allowUserInteraction)
			{
				Current.Gui.ErrorMessageBox(error);
			}

			return error;
		}

		/// <summary>
		/// Transpose transpose the table, i.e. exchange columns and rows
		/// this can only work if all columns in the table are of the same type
		/// </summary>
		/// <param name="srcTable">Table to transpose.</param>
		/// <param name="numberOfDataColumnsChangeToPropertyColumns">Number of data columns that are changed to property columns before transposing the table.</param>
		/// <param name="numberOfPropertyColumnsChangeToDataColumns">Number of property columns that are changed to data columns before transposing the table.</param>
		/// <returns>null if succeeded, error string otherwise</returns>
		public static string TransposeInline(DataTable srcTable, int numberOfDataColumnsChangeToPropertyColumns, int numberOfPropertyColumnsChangeToDataColumns)
		{
			numberOfDataColumnsChangeToPropertyColumns = Math.Max(numberOfDataColumnsChangeToPropertyColumns, 0);
			numberOfDataColumnsChangeToPropertyColumns = Math.Min(numberOfDataColumnsChangeToPropertyColumns, srcTable.DataColumnCount);

			numberOfPropertyColumnsChangeToDataColumns = Math.Max(numberOfPropertyColumnsChangeToDataColumns, 0);
			numberOfPropertyColumnsChangeToDataColumns = Math.Min(numberOfPropertyColumnsChangeToDataColumns, srcTable.PropertyColumnCount);

			// first, save the first data columns that are changed to property columns
			Altaxo.Data.DataColumnCollection savedDataColumns = new DataColumnCollection();
			Altaxo.Data.DataColumnCollection savedPropColumns = new DataColumnCollection();

			Altaxo.Collections.IAscendingIntegerCollection savedDataColIndices = Altaxo.Collections.ContiguousIntegerRange.FromStartAndCount(0, numberOfDataColumnsChangeToPropertyColumns);
			Altaxo.Collections.IAscendingIntegerCollection savedPropColIndices = Altaxo.Collections.ContiguousIntegerRange.FromStartAndCount(0, numberOfPropertyColumnsChangeToDataColumns);
			// store the columns temporarily in another collection and remove them from the original collections
			srcTable.DataColumns.MoveColumnsTo(savedDataColumns, 0, savedDataColIndices);
			srcTable.PropertyColumns.MoveColumnsTo(savedPropColumns, 0, savedPropColIndices);

			// now transpose the data columns
			srcTable.DataColumns.Transpose();

			savedDataColumns.InsertRows(0, numberOfPropertyColumnsChangeToDataColumns); // take offset caused by newly inserted prop columns->data columns into account
			savedDataColumns.MoveColumnsTo(srcTable.PropertyColumns, 0, savedDataColIndices);

			savedPropColumns.RemoveRows(0, numberOfDataColumnsChangeToPropertyColumns); // take offset caused by data columns changed to property columns into account
			savedPropColumns.MoveColumnsTo(srcTable.DataColumns, 0, savedPropColIndices);

			// now insert both the temporary stored DataColumnCollections at the beginning

			return null; // no error message
		}

		/// <summary>
		/// Tests whether or not all columns in this collection have the same type.
		/// </summary>
		/// <param name="col">The column collection containing the columns to test.</param>
		/// <param name="selectedColumnIndices">The column indices of the columns to test.</param>
		/// <param name="firstdifferentcolumnindex">Out: returns the first column that has a different type from the first column</param>.
		/// <returns>True if all selected columns are of the same type. True is also returned if the number of selected columns is zero.</returns>
		public static bool AreAllColumnsOfTheSameType(DataColumnCollection col, IContiguousIntegerRange selectedColumnIndices, out int firstdifferentcolumnindex)
		{
			firstdifferentcolumnindex = 0;

			if (0 == col.ColumnCount || 0 == selectedColumnIndices.Count)
				return true;

			System.Type firstType = col[selectedColumnIndices[0]].GetType();

			int len = selectedColumnIndices.Count;
			for (int i = 0; i < len; i++)
			{
				if (col[selectedColumnIndices[i]].GetType() != firstType)
				{
					firstdifferentcolumnindex = selectedColumnIndices[i];
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Transpose transpose the table, i.e. exchange columns and rows
		/// this can only work if all columns in the table are of the same type
		/// </summary>
		/// <param name="srcTable">Table to transpose.</param>
		/// <param name="numberOfDataColumnsChangeToPropertyColumns">Number of data columns that are changed to property columns before transposing the table.</param>
		/// <param name="numberOfPropertyColumnsChangeToDataColumns">Number of property columns that are changed to data columns before transposing the table.</param>
		/// <param name="destTable">Table in which the transposed table should be stored.</param>
		/// <returns>null if succeeded, error string otherwise</returns>
		public static void Transpose(DataTable srcTable, int numberOfDataColumnsChangeToPropertyColumns, int numberOfPropertyColumnsChangeToDataColumns, DataTable destTable)
		{
			if (null == srcTable)
				throw new ArgumentNullException(nameof(srcTable));
			if (null == destTable)
				throw new ArgumentNullException(nameof(destTable));
			if (object.ReferenceEquals(srcTable, destTable))
				throw new ArgumentException(nameof(srcTable) + " and " + nameof(destTable) + " are identical. This inline transpose operation is not supported.");

			numberOfDataColumnsChangeToPropertyColumns = Math.Max(numberOfDataColumnsChangeToPropertyColumns, 0);
			numberOfDataColumnsChangeToPropertyColumns = Math.Min(numberOfDataColumnsChangeToPropertyColumns, srcTable.DataColumnCount);

			numberOfPropertyColumnsChangeToDataColumns = Math.Max(numberOfPropertyColumnsChangeToDataColumns, 0);
			numberOfPropertyColumnsChangeToDataColumns = Math.Min(numberOfPropertyColumnsChangeToDataColumns, srcTable.PropertyColumnCount);

			var selSrcColumnIndices = ContiguousIntegerRange.FromStartAndEndExclusive(numberOfDataColumnsChangeToPropertyColumns, srcTable.DataColumnCount);

			int firstDifferentColumnIndex;
			if (!AreAllColumnsOfTheSameType(srcTable.DataColumns, selSrcColumnIndices, out firstDifferentColumnIndex))
			{
				throw new InvalidOperationException("The data columns to transpose are not of the same type. The first column that has a deviating type is column number " + firstDifferentColumnIndex.ToString());
			}

			destTable.DataColumns.ClearData();
			destTable.PropCols.ClearData();

			// 1st, copy the property columns to data columns
			for (int i = 0; i < numberOfPropertyColumnsChangeToDataColumns; ++i)
			{
				var destCol = destTable.DataColumns.EnsureExistenceAtPositionStrictly(i, srcTable.PropertyColumns.GetColumnName(i), srcTable.PropertyColumns[i].GetType(), srcTable.PropertyColumns.GetColumnKind(i), srcTable.PropertyColumns.GetColumnGroup(i));
				var srcCol = srcTable.PropertyColumns[i];
				for (int j = numberOfDataColumnsChangeToPropertyColumns, k = 0; j < srcCol.Count; ++j, ++k)
					destCol[k] = srcCol[j];
			}

			// 2rd, transpose the data columns

			int srcRows = 0;
			foreach (int i in selSrcColumnIndices)
				srcRows = Math.Max(srcRows, srcTable.DataColumns[i].Count);

			// create as many columns in destTable as srcRows and fill them with data
			Type columnType = selSrcColumnIndices.Count > 0 ? srcTable.DataColumns[selSrcColumnIndices[0]].GetType() : null;
			for (int i = 0; i < srcRows; ++i)
			{
				var destCol = destTable.DataColumns.EnsureExistenceAtPositionStrictly(numberOfPropertyColumnsChangeToDataColumns + i, "Row" + i.ToString(), columnType, ColumnKind.V, 0);
				int k = 0;
				foreach (int j in selSrcColumnIndices)
					destCol[k++] = srcTable.DataColumns[j][i];
			}

			// 3rd, copy the first data columns to property columns
			for (int i = 0; i < numberOfDataColumnsChangeToPropertyColumns; ++i)
			{
				var destCol = destTable.PropertyColumns.EnsureExistenceAtPositionStrictly(i, srcTable.DataColumns.GetColumnName(i), srcTable.DataColumns[i].GetType(), srcTable.DataColumns.GetColumnKind(i), srcTable.DataColumns.GetColumnGroup(i));
				var srcCol = srcTable.DataColumns[i];
				for (int j = numberOfPropertyColumnsChangeToDataColumns, k = 0; k < srcCol.Count; ++j, ++k)
					destCol[j] = srcCol[k];
			}

			// 4th, fill the rest of the property columns with the rest of the data columns
			for (int i = 0; i < numberOfDataColumnsChangeToPropertyColumns; ++i)
			{
				for (int j = 0; j < numberOfPropertyColumnsChangeToDataColumns; ++j)
				{
					try
					{
						destTable.PropertyColumns[i][j] = srcTable.PropertyColumns[j][i];
					}
					catch { }
				}
			}

			// and 5th, copy the remaining property columns to property columns
			for (int i = numberOfPropertyColumnsChangeToDataColumns, j = numberOfDataColumnsChangeToPropertyColumns; i < srcTable.PropertyColumns.ColumnCount; ++i, ++j)
			{
				var destCol = destTable.PropertyColumns.EnsureExistenceAtPositionStrictly(j, srcTable.PropertyColumns.GetColumnName(i), srcTable.PropertyColumns[i].GetType(), srcTable.PropertyColumns.GetColumnKind(i), srcTable.DataColumns.GetColumnGroup(i));
				destCol.Data = srcTable.PropertyColumns[i];
			}
		}
	}
}