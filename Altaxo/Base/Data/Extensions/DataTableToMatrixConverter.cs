#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	/// <summary>
	/// Class that helps to assembly numerical matrix data from a <see cref="DataTable" /> and selected data columns, seclected data rows, and selected property columns /&gt;.
	/// </summary>
	public class DataTableToMatrixConverter
	{
		#region Input members

		/// <summary>Underlying data table.</summary>
		protected DataTable _sourceTable;

		/// <summary>Collection of selected data columns.</summary>
		protected IAscendingIntegerCollection _selectedColumns;

		/// <summary>Collection of selected data rows.</summary>
		protected IAscendingIntegerCollection _selectedRows;

		/// <summary>Collection of selected property columns.</summary>
		protected IAscendingIntegerCollection _selectedPropertyColumns;

		/// <summary>
		/// Function that is used to generate a writeable matrix. First argument is number of rows, the 2nd argument is the number of columns of the matrix. The result shold be a writeable matrix with the corresponding number of rows and columns.
		/// </summary>
		protected Func<int, int, IMatrix<double>> _matrixGenerator;

		/// <summary>If this value is not null, it is used to replace all values in the matrix that are <see cref="M:System.Double.NaN"/>.</summary>
		protected double? _replacementValueForNaNMatrixElements;

		/// <summary>If this value is not null, it is used to replace all occurences of values in the matrix that are infinite.</summary>
		protected double? _replacementValueForInfiniteMatrixElements;

		#endregion Input members

		// Working members

		#region Working / resulting members

		/// <summary>True if the execution is done, i.e. the input data (table, selections) is converted into a matrix and row / column header columns.</summary>
		private bool _executionDone;

		/// <summary>The indices of the data columns of the <see cref="_sourceTable"/> that contribute to the resulting matrix.</summary>
		private IAscendingIntegerCollection _participatingDataColumns;

		/// <summary>The indices of the data rows of the <see cref="_sourceTable"/> that contribute to the resulting matrix.</summary>
		private IAscendingIntegerCollection _participatingDataRows;

		/// <summary>The group number of the data columns that contribute to the matrix. All columns must have this group number.</summary>
		private int _dataColumnsGroupNumber;

		/// <summary>Column that correlate each row of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.</summary>
		private INumericColumn _rowHeaderColumn;

		/// <summary>Column that correlate each column of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.</summary>
		private INumericColumn _columnHeaderColumn;

		/// <summary>Resulting matrix.</summary>
		private IMatrix<double> _resultingMatrix;

		/// <summary>Resulting row header vector. The members of this vector correspond to the row of the matrix with the same index.</summary>
		private IVector<double> _rowHeaderVector;

		/// <summary>Resulting column header vector. The members of this vector correspond to the column of the matrix with the same index.</summary>
		private IVector<double> _columnHeaderVector;

		#endregion Working / resulting members

		/// <summary>
		/// Initializes a new instance of the <see cref="DataTableToMatrixConverter"/> class.
		/// </summary>
		/// <param name="sourceTable">The source table. This member is mandatory. Before you call <see cref="Execute"/>, you can set the other properties of this class as you like.</param>
		/// <exception cref="System.ArgumentNullException">SourceTable must not be null</exception>
		public DataTableToMatrixConverter(DataTable sourceTable)
		{
			if (null == sourceTable)
				throw new ArgumentNullException("SourceTable must not be null");

			_sourceTable = sourceTable;
			_matrixGenerator = DefaultMatrixGenerator;
		}

		/// <summary>
		/// Gets the defaults the matrix generator, which generates a <see cref="JaggedArrayMatrix"/>.
		/// </summary>
		/// <param name="rows">Number of rows.</param>
		/// <param name="columns">Number of columns.</param>
		/// <returns></returns>
		protected virtual IMatrix<double> DefaultMatrixGenerator(int rows, int columns)
		{
			return new JaggedArrayMatrix(rows, columns);
		}

		#region Input Properties

		/// <summary>
		/// Sets the collection of indices of selected data columns.
		/// </summary>
		/// <value>
		/// The indices of selected data columns.
		/// </value>
		public IAscendingIntegerCollection SelectedDataColumns
		{
			set
			{
				_selectedColumns = value;
			}
		}

		/// <summary>
		/// Sets the collection of indices of selected data rows.
		/// </summary>
		/// <value>
		/// The indices of selected data rows.
		/// </value>
		public IAscendingIntegerCollection SelectedDataRows
		{
			set
			{
				_selectedRows = value;
			}
		}

		/// <summary>
		/// Sets the collection of indices of selected property columns.
		/// </summary>
		/// <value>
		/// The collection of indices of selected property columns.
		/// </value>
		public IAscendingIntegerCollection SelectedPropertyColumns
		{
			set
			{
				_selectedPropertyColumns = value;
			}
		}

		/// <summary>
		/// Sets the matrix generator that is used to generate the resulting matrix.
		/// </summary>
		/// <value>
		/// The matrix generator. First argument is the number of rows, 2nd argument the number of columns of the matrix to generate.
		/// </value>
		/// <exception cref="System.ArgumentNullException">MatrixGenerator is null.</exception>
		public Func<int, int, IMatrix<double>> MatrixGenerator
		{
			set
			{
				if (null == value)
					throw new ArgumentNullException("MatrixGenerator");

				_matrixGenerator = value;
			}
		}

		/// <summary>
		/// Sets a value that is used to replace all values in the matrix that are <see cref="M:System.Double.NaN"/>.
		/// </summary>
		/// /// <value>
		/// The replacement value for NaN matrix elements.
		/// </value>
		public double ReplacementValueForNaNMatrixElements
		{
			set
			{
				_replacementValueForNaNMatrixElements = value;
			}
		}

		/// <summary>
		/// Sets a value that is used to replace all values in the matrix that are Infinite.
		/// </summary>
		/// /// <value>
		/// The replacement value for infinite matrix elements.
		/// </value>
		public double ReplacementValueForInfiniteMatrixElements
		{
			set
			{
				_replacementValueForInfiniteMatrixElements = value;
			}
		}

		#endregion Input Properties

		#region Output properties

		/// <summary>
		/// Gets the indices of the data columns of the data table that contribute to the matrix.
		/// </summary>
		/// <returns>Indices of the participating data columns of the data table.</returns>
		/// <exception cref="InvalidDimensionMatrixException">No columns found that can be used for the matrix. Thus number of columns of the matrix would be zero.</exception>
		public IAscendingIntegerCollection GetParticipatingDataColumns()
		{
			if (null == _participatingDataColumns)
			{
				_participatingDataColumns = GetParticipatingDataColumns(_sourceTable, _selectedColumns);

				if (0 == _participatingDataColumns.Count)
					throw new InvalidDimensionMatrixException("No columns found that can be used for the matrix. Thus number of columns of the matrix would be zero.");

				_dataColumnsGroupNumber = _sourceTable.DataColumns.GetColumnGroup(_participatingDataColumns[0]);
			}

			return _participatingDataColumns;
		}

		/// <summary>
		/// Gets the indices of the data rows of the table that contribute to the matrix.
		/// </summary>
		/// <returns></returns>
		public IAscendingIntegerCollection GetParticipatingDataRows()
		{
			if (null == _participatingDataColumns)
				GetParticipatingDataColumns();
			if (null == _participatingDataRows)
				_participatingDataRows = GetParticipatingDataRows(_sourceTable, _selectedRows, _participatingDataColumns);
			return _participatingDataRows;
		}

		/// <summary>
		/// Determines, whether all available rows are included in the matrix.
		/// </summary>
		/// <returns><c>True</c> if all available rows are included in the matrix. Otherwise, if for instance only some user selected rows are included, the return value is <c>false</c>.</returns>
		public bool AreAllAvailableRowsIncluded()
		{
			GetParticipatingDataColumns();
			GetParticipatingDataRows();
			int maxNumberOfRows = _participatingDataColumns.Select(i => _sourceTable[i].Count).MaxOrDefault(0);
			return maxNumberOfRows == _participatingDataRows.Count;
		}

		/// <summary>
		/// Determines, whether all available columns (with same group number and ColumnKind.V) are included in the matrix.
		/// </summary>
		/// <returns><c>True</c> if all available columns (with same group number and ColumnKind.V) are included in the matrix. Otherwise, <c>false</c> is returned.</returns>
		public bool AreAllAvailableColumnsOfGroupIncluded()
		{
			GetParticipatingDataColumns();
			GetParticipatingDataRows();

			var coll = _sourceTable.DataColumns;
			int columnsAvailable = 0;
			for (int i = coll.ColumnCount - 1; i >= 0; --i)
			{
				if (_sourceTable.DataColumns[i] is INumericColumn && _sourceTable.DataColumns.GetColumnKind(i) == ColumnKind.V && _sourceTable.DataColumns.GetColumnGroup(i) == _dataColumnsGroupNumber)
					++columnsAvailable;
			}
			return columnsAvailable == _participatingDataColumns.Count;
		}

		/// <summary>
		/// Gets the common group number of all data columns that are included in the matrix.
		/// </summary>
		/// <value>
		/// The group number of the data columns included in the matrix.
		/// </value>
		public int DataColumnsGroupNumber
		{
			get
			{
				GetParticipatingDataColumns();
				return _dataColumnsGroupNumber;
			}
		}

		/// <summary>
		/// Gets the number of rows of the resulting matrix.
		/// </summary>
		/// <value>
		/// The number of rows of the resulting matrix.
		/// </value>
		public int NumberOfRows
		{
			get
			{
				GetParticipatingDataRows();
				return _participatingDataRows.Count;
			}
		}

		/// <summary>
		/// Gets the number of columns of the resulting matrix.
		/// </summary>
		/// <value>
		/// The number of columns of the resulting matrix.
		/// </value>
		public int NumberOfColumns
		{
			get
			{
				GetParticipatingDataColumns();
				return _participatingDataColumns.Count;
			}
		}

		/// <summary>
		/// Gets the resulting matrix. The resulting matrix is available only after calling <see cref="Execute"/>.
		/// </summary>
		/// <value>
		/// The resulting matrix.
		/// </value>
		/// <exception cref="System.InvalidOperationException">Resulting matrix is not known yet. Please call Execute first.</exception>
		public IMatrix<double> ResultingMatrix
		{
			get
			{
				if (!_executionDone)
					throw new InvalidOperationException("Resulting matrix is not known yet. Please call Execute first.");

				return _resultingMatrix;
			}
		}

		/// <summary>
		/// Gets the resulting row header column. Attention: to correspond elements in this column to the rows of the resulting matrix, <see cref="GetParticipatingDataRows()"/> must be used to get the collection of indices.
		/// Alternatively, use <see cref="RowHeaderVector"/>.
		/// The row header column is a column that correlate each row of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.
		/// </summary>
		/// <value>
		/// The row header column.
		/// </value>
		/// <exception cref="System.InvalidOperationException">RowHeaderColumn is not known yet. Please call Execute first.</exception>
		public INumericColumn RowHeaderColumn
		{
			get
			{
				if (!_executionDone)
					throw new InvalidOperationException("RowHeaderColumn is not known yet. Please call Execute first.");

				return _rowHeaderColumn;
			}
		}

		/// <summary>
		/// Gets the resulting row header vector. Each member of this vector corresponds to the row of the matrix with the same index.
		/// The row header vector is a vector that correlate each row of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.
		/// </summary>
		/// <value>
		/// The row header vector.
		/// </value>
		/// <exception cref="System.InvalidOperationException">RowHeaderVector is not known yet. Please call Execute first.</exception>
		public IVector<double> RowHeaderVector
		{
			get
			{
				if (!_executionDone)
					throw new InvalidOperationException("RowHeaderVector is not known yet. Please call Execute first.");

				return _rowHeaderVector;
			}
		}

		/// <summary>
		/// Gets the resulting column header column. Attention: to correspond elements in this column to the columns of the resulting matrix, <see cref="GetParticipatingDataColumns()"/> must be used to get the collection of indices.
		/// Alternatively, use <see cref="ColumnHeaderVector"/>.
		/// The column header column is a column that correlate each column of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.
		/// </summary>
		/// <value>
		/// The column header column.
		/// </value>
		/// <exception cref="System.InvalidOperationException">ColumnHeaderColumn is not known yet. Please call Execute first.</exception>
		public INumericColumn ColumnHeaderColumn
		{
			get
			{
				if (!_executionDone)
					throw new InvalidOperationException("ColumnHeaderColumn is not known yet. Please call Execute first.");

				return _columnHeaderColumn;
			}
		}

		/// <summary>
		/// Gets the resulting column header vector. Each member of this vector corresponds to the column of the matrix with the same index.
		/// The column header vector is a vector that correlate each column of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.
		/// </summary>
		/// <value>
		/// The column header vector.
		/// </value>
		/// <exception cref="System.InvalidOperationException">ColumnHeaderVector is not known yet. Please call Execute first.</exception>
		public IVector<double> ColumnHeaderVector
		{
			get
			{
				if (!_executionDone)
					throw new InvalidOperationException("ColumnHeaderVector is not known yet. Please call Execute first.");

				return _columnHeaderVector;
			}
		}

		/// <summary>
		/// Get the uniform row spacing value, or, if the row header vector is not uniformly spaced, a default value provided in <paramref name="defaultValue"/>.
		/// </summary>
		/// <param name="defaultValue">The default value used if the row header vector is not uniformly spaced.</param>
		/// <returns>Row spacing value.</returns>
		public double RowSpacingOrValue(double defaultValue)
		{
			var spacing = GetRowSpacing();
			return spacing.HasValue ? spacing.Value : defaultValue;
		}

		/// <summary>
		/// Get the uniform column spacing value, or, if the column header vector is not uniformly spaced, a default value provided in <paramref name="defaultValue"/>.
		/// </summary>
		/// <param name="defaultValue">The default value used if the column header vector is not uniformly spaced.</param>
		/// <returns>Row spacing value.</returns>
		public double ColumnSpacingOrValue(double defaultValue)
		{
			var spacing = GetColumnSpacing();
			return spacing.HasValue ? spacing.Value : defaultValue;
		}

		/// <summary>
		/// Get the uniform row spacing value, or, if the row header vector is not uniformly spaced, <c>null</c>.
		/// </summary>
		/// <returns>Row spacing value (or <c>null</c> if the row header vector is not uniformly spaced).</returns>
		public double? GetRowSpacing()
		{
			string msg;
			double result;
			return TryGetRowSpacing(out result, out msg) ? (double?)result : null;
		}

		/// <summary>
		/// Get the uniform column spacing value, or, if the column header vector is not uniformly spaced, <c>null</c>.
		/// </summary>
		/// <returns>Column spacing value (or <c>null</c> if the column header vector is not uniformly spaced).</returns>
		public double? GetColumnSpacing()
		{
			string msg;
			double result;
			return TryGetColumnSpacing(out result, out msg) ? (double?)result : null;
		}

		/// <summary>
		/// Tries to get the uniform row spacing value, or, if the row header vector is not uniformly spaced, <c>false</c> is returned along with a diagnostic message.
		/// </summary>
		/// <param name="rowSpacing">If the function is successful, contains the row spacing value.</param>
		/// <param name="errorMessage">If the function is not successful, contains a diagnostic error message.</param>
		/// <returns><c>True</c> if the function was successful, otherwise <c>False</c>.</returns>
		/// <exception cref="System.InvalidOperationException">RowHeaderVector is not known yet, thus row spacing is not known. Please call Execute first.</exception>
		public bool TryGetRowSpacing(out double rowSpacing, out string errorMessage)
		{
			if (!_executionDone)
				throw new InvalidOperationException("RowHeaderVector is not known yet, thus row spacing is not known. Please call Execute first.");

			if (null == _rowHeaderVector)
			{
				errorMessage = "A row header was not selected, or the selected column is not a numeric column. Thus row spacing could not be evaluated.";
				rowSpacing = double.NaN;
				return false;
			}

			return TryGetRowOrColumnSpacing(_rowHeaderVector, "row", out rowSpacing, out errorMessage);
		}

		/// <summary>
		/// Tries to get the uniform column spacing value, or, if the column header vector is not uniformly spaced, <c>false</c> is returned along with a diagnostic message.
		/// </summary>
		/// <param name="columnSpacing">If the function is successful, contains the column spacing value.</param>
		/// <param name="errorMessage">If the function is not successful, contains a diagnostic error message.</param>
		/// <returns><c>True</c> if the function was successful, otherwise <c>False</c>.</returns>
		/// <exception cref="System.InvalidOperationException">ColumnHeaderVector is not known yet, thus column spacing is not known. Please call Execute first.</exception>
		public bool TryGetColumnSpacing(out double columnSpacing, out string errorMessage)
		{
			if (!_executionDone)
				throw new InvalidOperationException("ColumnHeaderVector is not known yet, thus row spacing is not known. Please call Execute first.");

			if (null == _columnHeaderVector)
			{
				errorMessage = "A column header was not selected, or the selected column is not a numeric column. Thus column spacing could not be evaluated.";
				columnSpacing = double.NaN;
				return false;
			}

			return TryGetRowOrColumnSpacing(_columnHeaderVector, "column", out columnSpacing, out errorMessage);
		}

		/// <summary>
		/// Tries to get the uniform spacing value of a vector.
		/// </summary>
		/// <param name="headerVector">The vector to investigate.</param>
		/// <param name="rowOrColumn">Contains either "row" or "column" to include in the diagnostic error message.</param>
		/// <param name="spacingValue">If the function is successful, contains the uniform spacing value.</param>
		/// <param name="errorMessage">If the function is not successful, contains a diagnostic error message.</param>
		/// <returns><c>True</c> if the function was successful, otherwise <c>False</c>.</returns>
		public static bool TryGetRowOrColumnSpacing(IReadOnlyList<double> headerVector, string rowOrColumn, out double spacingValue, out string errorMessage)
		{
			var spacing = new Calc.LinearAlgebra.VectorSpacingEvaluator(headerVector);

			if (!spacing.IsStrictlyMonotonicIncreasing)
			{
				errorMessage = string.Format("The values of the {0} header vector are not monotonically increasing. Thus {0} spacing could not be evaluated.", rowOrColumn);
				spacingValue = double.NaN;
				return false;
			}

			if (!spacing.IsStrictlyEquallySpaced)
			{
				errorMessage = string.Format("The values of the {0} header vector are not strictly equally spaced. The relative deviation is {1}. Thus {0} spacing could not be evaluated.", rowOrColumn, spacing.RelativeSpaceDeviation);
				spacingValue = spacing.SpaceMeanValue;
				return false;
			}

			errorMessage = null;
			spacingValue = spacing.SpaceMeanValue;
			return true;
		}

		#endregion Output properties

		/// <summary>
		/// Performs the assembling for the resulting matrix and the row / column header vectors. After calling this function, the properties <see cref="ResultingMatrix"/>, <see cref="RowHeaderVector"/> and
		/// <see cref="ColumnHeaderVector"/> are available.
		/// </summary>
		public void Execute()
		{
			_executionDone = false;

			GetParticipatingDataColumns();
			GetParticipatingDataRows();

			CheckContentForNaNandInfiniteValues(_sourceTable.DataColumns, _participatingDataRows, _participatingDataColumns, !_replacementValueForNaNMatrixElements.HasValue, !_replacementValueForInfiniteMatrixElements.HasValue);

			GetRowHeaderVector();
			GetColumnHeaderVector();

			FillMatrix();

			_executionDone = true;
		}

		/// <summary>
		/// Creates and fills the column header vector.
		/// </summary>
		private void GetColumnHeaderVector()
		{
			// find out if there is a y property column or not
			_columnHeaderColumn = null;
			if (null != _selectedPropertyColumns && _selectedPropertyColumns.Count > 0)
			{
				// then use the first numeric column as y column that you find
				for (int i = 0; i < _selectedPropertyColumns.Count; i++)
				{
					if (_sourceTable.PropCols[_selectedPropertyColumns[i]] is INumericColumn)
					{
						_columnHeaderColumn = (INumericColumn)_sourceTable.PropCols[_selectedPropertyColumns[i]];
						break;
					}
				}
			}

			if (null != _columnHeaderColumn)
			{
				double[] arr = new double[_participatingDataColumns.Count];
				for (int i = 0; i < _participatingDataColumns.Count; ++i)
					arr[i] = _columnHeaderColumn[_participatingDataColumns[i]];
				_columnHeaderVector = VectorMath.ToVector(arr);
			}
		}

		/// <summary>
		/// Creates and fills the row header vector.
		/// </summary>
		private void GetRowHeaderVector()
		{
			// find out if there is a xcolumn or not
			int group = _sourceTable.DataColumns.GetColumnGroup(_participatingDataColumns[0]);
			DataColumn xcol = _sourceTable.DataColumns.FindXColumnOfGroup(group);
			_rowHeaderColumn = xcol as INumericColumn;

			if (null != _rowHeaderColumn)
			{
				double[] arr = new double[_participatingDataRows.Count];
				for (int i = 0; i < _participatingDataRows.Count; ++i)
					arr[i] = _rowHeaderColumn[_participatingDataRows[i]];
				_rowHeaderVector = VectorMath.ToVector(arr);
			}
		}

		#region Static helper functions

		/// <summary>
		/// Gets the indices of data columns that can participate in a matrix area, by providing a data table and the selected column.
		/// The participating data columns must have ColumnKind.V, and must share the same group number.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="selectedColumns">The selected data columns of the provided table. You can provide <c>null</c> for this parameter. This is considered as if all columns of the table are selected.</param>
		/// <returns></returns>
		public static AscendingIntegerCollection GetParticipatingDataColumns(DataTable table, IAscendingIntegerCollection selectedColumns)
		{
			var result = new Altaxo.Collections.AscendingIntegerCollection();
			int? groupNumber = null;
			if (selectedColumns == null || selectedColumns.Count == 0) // No columns selected - than we assume all columns, but only V-columns
			{
				var dc = table.DataColumns;
				for (int i = 0; i < table.DataColumnCount; ++i)
				{
					if (dc[i] is Altaxo.Data.INumericColumn && dc.GetColumnKind(i) == ColumnKind.V && (!groupNumber.HasValue || groupNumber.Value == dc.GetColumnGroup(i)))
					{
						result.Add(i);
					}
				}
			}
			else
			{
				for (int k = 0; k < selectedColumns.Count; ++k)
				{
					var dc = table.DataColumns;
					int i = selectedColumns[k];
					if (dc[i] is DoubleColumn && dc.GetColumnKind(i) == ColumnKind.V && (!groupNumber.HasValue || groupNumber.Value == dc.GetColumnGroup(i)))
					{
						result.Add(i);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Gets the data rows that participate in a matrix area by providing a table, the collection of selected data rows, and the collection of selected data columns.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="selectedRows">The selected data rows.</param>
		/// <param name="participatingColumns">The data columns that participate in the matrix area.</param>
		/// <returns>The collection of indices of data rows that participate in the matrix area.</returns>
		public static Altaxo.Collections.AscendingIntegerCollection GetParticipatingDataRows(DataTable table, IAscendingIntegerCollection selectedRows, IAscendingIntegerCollection participatingColumns)
		{
			var result = new AscendingIntegerCollection();

			if (null != selectedRows && selectedRows.Count > 0)
			{
				result.Add(selectedRows);
			}
			else
			{
				var dc = table.DataColumns;
				int rows = participatingColumns.Select(i => dc[i].Count).MinOrDefault(0);

				result.AddRange(0, rows);
			}

			return result;
		}

		/// <summary>
		/// Checks the content of a matrix area of a table for NaN and infinite values. If such values are found, an <see cref="InvalidOperationException"/> is thrown with a diagnostic message.
		/// </summary>
		/// <param name="table">The table to check.</param>
		/// <param name="participatingRows">The rows participating in the matrix area.</param>
		/// <param name="participatingColumns">The columns participating in the matrix area.</param>
		/// <param name="checkForNaN">If set to <c>true</c>, this function will check for NaN values.</param>
		/// <param name="checkForInfinity">If set to <c>true</c>, this function will check for Infinite values.</param>
		/// <exception cref="System.InvalidOperationException">Is thrown if NaN or Infinite values are found in the matrix area.</exception>
		public static void CheckContentForNaNandInfiniteValues(DataColumnCollection table, IAscendingIntegerCollection participatingRows, IAscendingIntegerCollection participatingColumns, bool checkForNaN, bool checkForInfinity)
		{
			if (!checkForInfinity || checkForNaN)
				return; // nothing to do then

			for (int i = 0; i < participatingColumns.Count; ++i)
			{
				var c = table[participatingColumns[i]];

				for (int j = 0; j < participatingRows.Count; ++j)
				{
					double x = c[participatingRows[j]];

					if (checkForNaN && double.IsNaN(x))
						throw new InvalidOperationException(string.Format("Array value is NaN (Not a Number) at column={0}, row={1}", participatingColumns[i], participatingRows[j]));
					if (checkForInfinity && !(x >= double.MinValue && x <= double.MaxValue))
						throw new InvalidOperationException(string.Format("Array value is not a finite number at column={0}, row={1}", participatingColumns[i], participatingRows[j]));
				}
			}
		}

		#endregion Static helper functions

		/// <summary>
		/// Creates the resulting matrix and fills it with values.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">The matrix generator worked not as expected. Either the generated matrix is null, or the dimensions of the returned matrix deviate from the provided dimensions.</exception>
		protected virtual void FillMatrix()
		{
			// reserve two arrays (one for real part, which is filled with the table contents)
			// and the imaginary part - which is left zero here)

			var numColumns = _participatingDataColumns.Count;
			var numRows = _participatingDataRows.Count;

			_resultingMatrix = _matrixGenerator(numRows, numColumns);

			if (null == _resultingMatrix || _resultingMatrix.RowCount != numRows || _resultingMatrix.ColumnCount != numColumns)
				throw new InvalidOperationException("The matrix generator worked not as expected. Either the generated matrix is null, or the dimensions of the returned matrix deviate from the provided dimensions.");

			// fill the real part with the table contents
			for (int i = 0; i < numColumns; i++)
			{
				var col = (Altaxo.Data.INumericColumn)_sourceTable[_participatingDataColumns[i]];
				for (int j = 0; j < numRows; j++)
				{
					var x = col[_participatingDataRows[j]];

					if (double.IsNaN(x) && _replacementValueForNaNMatrixElements.HasValue)
						x = _replacementValueForNaNMatrixElements.Value;
					else if (!(x > double.MinValue && x < double.MaxValue) && _replacementValueForInfiniteMatrixElements.HasValue)
						x = _replacementValueForInfiniteMatrixElements.Value;

					_resultingMatrix[j, i] = x;
				}
			}
		}
	}
}