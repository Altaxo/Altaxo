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
		/// Elementwise application of a function to each element of a matrix. The result is stored in another matrix or in the same matrix.
		/// </summary>
		/// <param name="src1">Matrix to use the values from.</param>
		/// <param name="function">Function to be applied to each element of the matrix. The argument is the element of the source matrix.</param>
		/// <param name="result">Matrix to store the result. This may be the same instance as the source matrix.</param>
		public static void Map(IROMatrix<double> src1, Func<Double, Double> function, IMatrix<double> result)
		{
			if (src1 is null)
				throw new ArgumentNullException(nameof(src1));
			if (result is null)
				throw new ArgumentNullException(nameof(result));

			if (src1.RowCount != result.RowCount || src1.ColumnCount != result.ColumnCount)
				throw new RankException("Mismatch of dimensions of src1 and result");

			var cols = src1.ColumnCount; 
			var rows = src1.RowCount;
			for (int i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					result[i, j] = function(src1[i, j]);
				}
			}
		}

			/// <summary>
		/// Elementwise application of a function to each element of two matrices. The result is stored in another matrix or in the same matrix.
		/// </summary>
		/// <param name="src1">First matrix to use the values from.</param>
		/// <param name="src2">Second matrix to use the values from.</param>
		/// <param name="function">Function to be applied to each element of src1 and src2.</param>
		/// <param name="result">Matrix to store the result. This may be the same instance as one of the matrices src1 or src2.</param>
		public static void Map(IROMatrix<double> src1, IROMatrix<double> src2, Func<Double, Double, Double> function, IMatrix<double> result)
		{
			if (src1 is null)
				throw new ArgumentNullException(nameof(src1));
			if (src2 is null)
				throw new ArgumentNullException(nameof(src2));
			if (result is null)
				throw new ArgumentNullException(nameof(result));

			if (src1.RowCount != src2.RowCount || src1.ColumnCount != src2.ColumnCount)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.RowCount != result.RowCount || src1.ColumnCount != result.ColumnCount)
				throw new RankException("Mismatch of dimensions of src1 and result");

			var cols = src1.ColumnCount; 
			var rows = src1.RowCount;
			for (int i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					result[i, j] = function(src1[i, j], src2[i, j]);
				}
			}
		}

		/// <summary>
		/// Elementwise application of a function to each element of a matrix. The result is stored in another matrix or in the same matrix.
		/// </summary>
		/// <param name="src1">Matrix to use the values from.</param>
		/// <param name="function">Function to be applied to each element of the matrix. 1st argument is the row number, 2nd argument the column number, 3rd argument the element of the src matrix,.</param>
		/// <param name="result">Matrix to store the result. This may be the same instance as the source matrix.</param>
		public static void MapIndexed(IROMatrix<double> src1, Func<int, int, Double, Double> function, IMatrix<double> result)
		{
			if (src1 is null)
				throw new ArgumentNullException(nameof(src1));
			if (result is null)
				throw new ArgumentNullException(nameof(result));

			if (src1.RowCount != result.RowCount || src1.ColumnCount != result.ColumnCount)
				throw new RankException("Mismatch of dimensions of src1 and result");

			var cols = src1.ColumnCount; 
			var rows = src1.RowCount;
			for (int i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					result[i, j] = function(i, j, src1[i, j]);
				}
			}
		}


	

		/// <summary>
		/// Elementwise application of a function to each element of two matrices. The result is stored in another matrix or in the same matrix.
		/// </summary>
		/// <param name="src1">First matrix to use the values from.</param>
		/// <param name="src2">Second matrix to use the values from.</param>
		/// <param name="function">Function to be applied to each element of src1 and src2. 1st argument is the row number, 2nd argument is the column number, 3rd argument is the element of matrix src1, 4th argument is the element of matrix src2.</param>
		/// <param name="result">Matrix to store the result. This may be the same instance as one of the matrices src1 or src2.</param>
		public static void MapIndexed(IROMatrix<double> src1, IROMatrix<double> src2, Func<int, int, Double, Double, Double> function, IMatrix<double> result)
		{
			if (src1 is null)
				throw new ArgumentNullException(nameof(src1));
			if (src2 is null)
				throw new ArgumentNullException(nameof(src2));
			if (result is null)
				throw new ArgumentNullException(nameof(result));

			if (src1.RowCount != src2.RowCount || src1.ColumnCount != src2.ColumnCount)
				throw new RankException("Mismatch of dimensions of src1 and src2");
			if (src1.RowCount != result.RowCount || src1.ColumnCount != result.ColumnCount)
				throw new RankException("Mismatch of dimensions of src1 and result");

			var cols = src1.ColumnCount; 
			var rows = src1.RowCount;
			for (int i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					result[i, j] = function(i, j, src1[i, j], src2[i, j]);
				}
			}
		}

		// ------------------------ Map indexed with auxillary parameters -----------------------------------------

		/// <summary>
		/// Elementwise application of a function to each element of a matrix. The result is stored in another matrix or in the same matrix.
		/// </summary>
		/// <param name="src1">Matrix to use the values from.</param>
		/// <param name="parameter1">An auxillary parameter.</param>
		/// <param name="function">Function to be applied to each element of the matrix. 1st argument is the row number, 2nd argument the column number, 3rd argument the element of the src matrix, 4th argument the auxillary parameter1.</param>
		/// <param name="result">Matrix to store the result. This may be the same instance as the source matrix.</param>
		public static void MapIndexed<T1>(IROMatrix<double> src1, T1 parameter1, Func<int, int, Double, T1, Double> function, IMatrix<double> result)
		{
			if (src1 is null)
				throw new ArgumentNullException(nameof(src1));
			if (result is null)
				throw new ArgumentNullException(nameof(result));

			if (src1.RowCount != result.RowCount || src1.ColumnCount != result.ColumnCount)
				throw new RankException("Mismatch of dimensions of src1 and result");

			var cols = src1.ColumnCount; 
			var rows = src1.RowCount;
			for (int i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					result[i, j] = function(i, j, src1[i, j], parameter1);
				}
			}
		}


	} // class
} // namespace
