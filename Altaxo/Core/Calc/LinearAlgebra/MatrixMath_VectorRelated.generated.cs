#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.LinearAlgebra
{
  // Unary functions not returning a vector, valid for all vector types

  public static partial class MatrixMath
  {

// ******************************************* Unary functions not returning a vector, valid for all non-null vector types  ********************

// ******************************************** Definitions for Double *******************************************

		/// <summary>
		/// Gets the column of a matrix copied into a vector.
		/// </summary>
		/// <param name="sourceMatrix">Matrix to copy from</param>
		/// <param name="columnNumber">Number of column of the matrix to be copied.</param>
		/// <param name="destinationVector">Vector to copy the column data to.</param>
		public static void CopyColumn(this IROMatrix<double> sourceMatrix, int columnNumber, double[] destinationVector)
		{
			if (columnNumber < 0 || columnNumber >= sourceMatrix.ColumnCount)
				throw new ArgumentOutOfRangeException(nameof(columnNumber), "column must be greater than or equal to zero and less than ColumnLength.");
			if (destinationVector is null)
				throw new ArgumentNullException(nameof(destinationVector));
			if (destinationVector.Length != sourceMatrix.RowCount)
				throw new RankException("Length of destinationVector does not match number of rows of source matrix");

			var rows = sourceMatrix.RowCount;
			for (int i = 0; i < rows; ++i)
			{
				destinationVector[i] = sourceMatrix[i, columnNumber];
			}
		}

        /// <summary>
		/// Gets the column of a matrix copied into a vector.
		/// </summary>
		/// <param name="destinationMatrix">Matrix to copy from</param>
		/// <param name="columnNumber">Number of column of the matrix to be copied.</param>
		/// <param name="sourceVector">Vector to copy the column data to.</param>
		public static void SetColumn(this IMatrix<double> destinationMatrix, int columnNumber, double[] sourceVector)
		{
			if (columnNumber < 0 || columnNumber >= destinationMatrix.ColumnCount)
				throw new ArgumentOutOfRangeException(nameof(columnNumber), "column must be greater than or equal to zero and less than ColumnLength.");
			if (sourceVector is null)
				throw new ArgumentNullException(nameof(sourceVector));
			if (sourceVector.Length != destinationMatrix.RowCount)
				throw new RankException("Length of sourceVector does not match number of rows of destination matrix");

			var rows = destinationMatrix.RowCount;
			for (int i = 0; i < rows; ++i)
			{
				 destinationMatrix[i, columnNumber] = sourceVector[i];
			}
		}

        /// <summary>
		/// Gets the row of a matrix copied into a vector.
		/// </summary>
		/// <param name="sourceMatrix">Matrix to copy from</param>
		/// <param name="rowNumber">Number of row of the matrix to be copied.</param>
		/// <param name="destinationVector">Vector to copy the column data to.</param>
		public static void CopyRow(this IROMatrix<double> sourceMatrix, int rowNumber, double[] destinationVector)
		{
			if (rowNumber < 0 || rowNumber >= sourceMatrix.RowCount)
				throw new ArgumentOutOfRangeException(nameof(rowNumber), "row must be greater than or equal to zero and less than RowCount.");
			if (destinationVector is null)
				throw new ArgumentNullException(nameof(destinationVector));
			if (destinationVector.Length != sourceMatrix.ColumnCount)
				throw new RankException("Length of destinationVector does not match number of columns of source matrix");

			var cols = sourceMatrix.ColumnCount;
			for (int i = 0; i < cols; ++i)
			{
				destinationVector[i] = sourceMatrix[rowNumber, i];
			}
		}

         /// <summary>
		/// Gets the row of a matrix copied into a vector.
		/// </summary>
		/// <param name="destinationMatrix">Matrix to copy from</param>
		/// <param name="rowNumber">Number of row of the matrix to be copied.</param>
		/// <param name="sourceVector">Vector to copy the column data to.</param>
		public static void SetRow(this IMatrix<double> destinationMatrix, int rowNumber, double[] sourceVector)
		{
			if (rowNumber < 0 || rowNumber >= destinationMatrix.RowCount)
				throw new ArgumentOutOfRangeException(nameof(rowNumber), "row must be greater than or equal to zero and less than RowCount.");
			if (sourceVector is null)
				throw new ArgumentNullException(nameof(sourceVector));
			if (sourceVector.Length != destinationMatrix.ColumnCount)
				throw new RankException("Length of sourceVector does not match number of columns of destination matrix");

			var cols = destinationMatrix.ColumnCount;
			for (int i = 0; i < cols; ++i)
			{
				 destinationMatrix[rowNumber, i] = sourceVector[i];
			}
		}


// ******************************************** Definitions for Double *******************************************

		/// <summary>
		/// Gets the column of a matrix copied into a vector.
		/// </summary>
		/// <param name="sourceMatrix">Matrix to copy from</param>
		/// <param name="columnNumber">Number of column of the matrix to be copied.</param>
		/// <param name="destinationVector">Vector to copy the column data to.</param>
		public static void CopyColumn(this IROMatrix<double> sourceMatrix, int columnNumber, IVector<double> destinationVector)
		{
			if (columnNumber < 0 || columnNumber >= sourceMatrix.ColumnCount)
				throw new ArgumentOutOfRangeException(nameof(columnNumber), "column must be greater than or equal to zero and less than ColumnLength.");
			if (destinationVector is null)
				throw new ArgumentNullException(nameof(destinationVector));
			if (destinationVector.Count != sourceMatrix.RowCount)
				throw new RankException("Length of destinationVector does not match number of rows of source matrix");

			var rows = sourceMatrix.RowCount;
			for (int i = 0; i < rows; ++i)
			{
				destinationVector[i] = sourceMatrix[i, columnNumber];
			}
		}

        /// <summary>
		/// Gets the column of a matrix copied into a vector.
		/// </summary>
		/// <param name="destinationMatrix">Matrix to copy from</param>
		/// <param name="columnNumber">Number of column of the matrix to be copied.</param>
		/// <param name="sourceVector">Vector to copy the column data to.</param>
		public static void SetColumn(this IMatrix<double> destinationMatrix, int columnNumber, IReadOnlyList<double> sourceVector)
		{
			if (columnNumber < 0 || columnNumber >= destinationMatrix.ColumnCount)
				throw new ArgumentOutOfRangeException(nameof(columnNumber), "column must be greater than or equal to zero and less than ColumnLength.");
			if (sourceVector is null)
				throw new ArgumentNullException(nameof(sourceVector));
			if (sourceVector.Count != destinationMatrix.RowCount)
				throw new RankException("Length of sourceVector does not match number of rows of destination matrix");

			var rows = destinationMatrix.RowCount;
			for (int i = 0; i < rows; ++i)
			{
				 destinationMatrix[i, columnNumber] = sourceVector[i];
			}
		}

        /// <summary>
		/// Gets the row of a matrix copied into a vector.
		/// </summary>
		/// <param name="sourceMatrix">Matrix to copy from</param>
		/// <param name="rowNumber">Number of row of the matrix to be copied.</param>
		/// <param name="destinationVector">Vector to copy the column data to.</param>
		public static void CopyRow(this IROMatrix<double> sourceMatrix, int rowNumber, IVector<double> destinationVector)
		{
			if (rowNumber < 0 || rowNumber >= sourceMatrix.RowCount)
				throw new ArgumentOutOfRangeException(nameof(rowNumber), "row must be greater than or equal to zero and less than RowCount.");
			if (destinationVector is null)
				throw new ArgumentNullException(nameof(destinationVector));
			if (destinationVector.Count != sourceMatrix.ColumnCount)
				throw new RankException("Length of destinationVector does not match number of columns of source matrix");

			var cols = sourceMatrix.ColumnCount;
			for (int i = 0; i < cols; ++i)
			{
				destinationVector[i] = sourceMatrix[rowNumber, i];
			}
		}

         /// <summary>
		/// Gets the row of a matrix copied into a vector.
		/// </summary>
		/// <param name="destinationMatrix">Matrix to copy from</param>
		/// <param name="rowNumber">Number of row of the matrix to be copied.</param>
		/// <param name="sourceVector">Vector to copy the column data to.</param>
		public static void SetRow(this IMatrix<double> destinationMatrix, int rowNumber, IReadOnlyList<double> sourceVector)
		{
			if (rowNumber < 0 || rowNumber >= destinationMatrix.RowCount)
				throw new ArgumentOutOfRangeException(nameof(rowNumber), "row must be greater than or equal to zero and less than RowCount.");
			if (sourceVector is null)
				throw new ArgumentNullException(nameof(sourceVector));
			if (sourceVector.Count != destinationMatrix.ColumnCount)
				throw new RankException("Length of sourceVector does not match number of columns of destination matrix");

			var cols = destinationMatrix.ColumnCount;
			for (int i = 0; i < cols; ++i)
			{
				 destinationMatrix[rowNumber, i] = sourceVector[i];
			}
		}


	} // class
} // namespace
