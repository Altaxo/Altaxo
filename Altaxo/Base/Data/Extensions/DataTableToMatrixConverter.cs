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
using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	/// <summary>
	/// Class to assist in executing a 2D Fourier transform on data originating from an Altaxo <see cref="DataTable"/>.
	/// </summary>
	public class DataTableToMatrixConverter
	{
		#region Input members

		protected DataTable _sourceTable;

		protected IAscendingIntegerCollection _selectedColumns, _selectedRows, _selectedPropertyColumns;

		protected Func<int, int, IMatrix> _matrixGenerator;

		protected double? _replacementValueForNaNMatrixElements;

		protected double? _replacementValueForInfiniteMatrixElements;

		#endregion Input members

		// Working members

		#region Working / resulting members

		private bool _executionDone;

		private IAscendingIntegerCollection _participatingDataColumns, _participatingDataRows;
		private int _dataColumnsGroupNumber;

		private INumericColumn _rowHeaderColumn, _columnHeaderColumn;

		private IMatrix _resultingMatrix;
		private IVector _rowHeaderVector;
		private IVector _columnHeaderVector;

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

		protected virtual IMatrix DefaultMatrixGenerator(int rows, int columns)
		{
			return new JaggedArrayMatrix(rows, columns);
		}

		#region Input Properties

		public IAscendingIntegerCollection SelectedDataColumns
		{
			set
			{
				_selectedColumns = value;
			}
		}

		public IAscendingIntegerCollection SelectedDataRows
		{
			set
			{
				_selectedRows = value;
			}
		}

		public IAscendingIntegerCollection SelectedPropertyColumns
		{
			set
			{
				_selectedPropertyColumns = value;
			}
		}

		public Func<int, int, IMatrix> MatrixGenerator
		{
			set
			{
				if (null == value)
					throw new ArgumentNullException("MatrixGenerator");

				_matrixGenerator = value;
			}
		}

		public double ReplacementValueForNaNMatrixElements
		{
			set
			{
				_replacementValueForNaNMatrixElements = value;
			}
		}

		public double ReplacementValueForInfiniteMatrixElements
		{
			set
			{
				_replacementValueForInfiniteMatrixElements = value;
			}
		}

		#endregion Input Properties

		#region Output properties

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

		public IAscendingIntegerCollection GetParticipatingDataRows()
		{
			if (null == _participatingDataColumns)
				GetParticipatingDataColumns();
			if (null == _participatingDataRows)
				_participatingDataRows = GetParticipatingDataRows(_sourceTable, _selectedRows, _participatingDataColumns);
			return _participatingDataRows;
		}

		public bool AreAllAvailableRowsIncluded()
		{
			GetParticipatingDataColumns();
			GetParticipatingDataRows();
			int maxNumberOfRows = 0;
			for (int i = 0; i < _participatingDataColumns.Count; ++i)
			{
				maxNumberOfRows = Math.Max(maxNumberOfRows, _sourceTable[_participatingDataColumns[i]].Count);
			}
			return maxNumberOfRows == _participatingDataRows.Count;
		}

		public bool AreAllAvailableColumnsOfGroupIncluded()
		{
			GetParticipatingDataColumns();
			GetParticipatingDataRows();
			int columnsAvailable = 0;
			for (int i = 0; i < _participatingDataColumns.Count; ++i)
			{
				if (_sourceTable.DataColumns[i] is INumericColumn && _sourceTable.DataColumns.GetColumnKind(i) == ColumnKind.V && _sourceTable.DataColumns.GetColumnGroup(i) == _dataColumnsGroupNumber)
					++columnsAvailable;
			}
			return columnsAvailable == _participatingDataColumns.Count;
		}

		public int DataColumnsGroupNumber
		{
			get
			{
				GetParticipatingDataColumns();
				return _dataColumnsGroupNumber;
			}
		}

		public int NumberOfRows
		{
			get
			{
				GetParticipatingDataRows();
				return _participatingDataRows.Count;
			}
		}

		public int NumberOfColumns
		{
			get
			{
				GetParticipatingDataColumns();
				return _participatingDataColumns.Count;
			}
		}

		public IMatrix ResultingMatrix
		{
			get
			{
				if (!_executionDone)
					throw new InvalidOperationException("Resulting matrix is not known yet. Please call Execute first.");

				return _resultingMatrix;
			}
		}

		public INumericColumn RowHeaderColumn
		{
			get
			{
				if (!_executionDone)
					throw new InvalidOperationException("RowHeaderColumn is not known yet. Please call Execute first.");

				return _rowHeaderColumn;
			}
		}

		public IVector RowHeaderVector
		{
			get
			{
				if (!_executionDone)
					throw new InvalidOperationException("RowHeaderVector is not known yet. Please call Execute first.");

				return _rowHeaderVector;
			}
		}

		public INumericColumn ColumnHeaderColumn
		{
			get
			{
				if (!_executionDone)
					throw new InvalidOperationException("ColumnHeaderColumn is not known yet. Please call Execute first.");

				return _columnHeaderColumn;
			}
		}

		public IVector ColumnHeaderVector
		{
			get
			{
				if (!_executionDone)
					throw new InvalidOperationException("ColumnHeaderVector is not known yet. Please call Execute first.");

				return _columnHeaderVector;
			}
		}

		public double RowSpacingOrValue(double defaultValue)
		{
			var spacing = GetRowSpacing();
			return spacing.HasValue ? spacing.Value : defaultValue;
		}

		public double ColumnSpacingOrValue(double defaultValue)
		{
			var spacing = GetColumnSpacing();
			return spacing.HasValue ? spacing.Value : defaultValue;
		}

		public double? GetRowSpacing()
		{
			string msg;
			double result;
			return TryGetRowSpacing(out result, out msg) ? (double?)result : null;
		}

		public double? GetColumnSpacing()
		{
			string msg;
			double result;
			return TryGetColumnSpacing(out result, out msg) ? (double?)result : null;
		}

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

		public static bool TryGetRowOrColumnSpacing(IROVector headerVector, string rowOrColumn, out double rowSpacing, out string errorMessage)
		{
			var spacing = new Calc.LinearAlgebra.VectorSpacingEvaluator(headerVector);

			if (!spacing.IsStrictlyMonotonicIncreasing)
			{
				errorMessage = string.Format("The values of the {0} header vector are not monotonically increasing. Thus {0} spacing could not be evaluated.", rowOrColumn);
				rowSpacing = double.NaN;
				return false;
			}

			if (!spacing.IsStrictlyEquallySpaced)
			{
				errorMessage = string.Format("The values of the {0} header vector are not strictly equally spaced. The relative deviation is {1}. Thus {0} spacing could not be evaluated.", rowOrColumn, spacing.RelativeSpaceDeviation);
				rowSpacing = spacing.SpaceMeanValue;
				return false;
			}

			errorMessage = null;
			rowSpacing = spacing.SpaceMeanValue;
			return true;
		}

		#endregion Output properties

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
				int rows = int.MaxValue;
				foreach (int i in participatingColumns)
					rows = Math.Min(rows, dc[i].Count);
				result.AddRange(0, rows);
			}

			return result;
		}

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

		protected virtual void FillMatrix()
		{
			// reserve two arrays (one for real part, which is filled with the table contents)
			// and the imaginary part - which is left zero here)

			var numColumns = _participatingDataColumns.Count;
			var numRows = _participatingDataRows.Count;

			_resultingMatrix = _matrixGenerator(numRows, numColumns);

			if (null == _resultingMatrix || _resultingMatrix.Rows != numRows || _resultingMatrix.Columns != numColumns)
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