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

	} // class
} // namespace
