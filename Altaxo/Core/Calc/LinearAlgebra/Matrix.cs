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

using System;

namespace Altaxo.Calc.LinearAlgebra
{
    internal class TransposableMatrix : IExtensibleMatrix<double>
    {
        private double[][] m_Array;
        private int m_NumVectors;
        private int m_VectorLen;
        private bool m_bVerticalVectors = false; // normally the matrix consists of several rows containing column vectors

        public TransposableMatrix()
        {
            m_NumVectors = 0;
            m_VectorLen = 0;
            m_bVerticalVectors = false;
        }

        public TransposableMatrix(int rows, int cols)
        {
            SetDimension(rows, cols);
        }

        public TransposableMatrix(double[][] arr)
        {
            // get the max number of columns in the matrix
            int cols = 0;
            for (int i = 0; i < arr.Length; i++)
                cols = Math.Max(cols, arr[i].Length);

            // set the dimension of our matrix
            SetDimension(arr.Length, cols);

            // copy the values in our array
            for (int i = 0; i < arr.Length; i++)
                Array.Copy(arr[i], 0, m_Array[i], 0, arr[i].Length);
        }

        #region IMatrix Members

        public void SetDimension(int rows, int cols)
        {
            m_NumVectors = rows;
            m_VectorLen = cols;
            m_bVerticalVectors = false;
            m_Array = new double[rows][];
            for (int i = 0; i < m_Array.Length; i++)
                m_Array[i] = new double[cols];
        }

        public double this[int i, int k]
        {
            get
            {
                return m_bVerticalVectors ? m_Array[k][i] : m_Array[i][k];
            }
            set
            {
                if (m_bVerticalVectors)
                    m_Array[k][i] = value;
                else
                    m_Array[i][k] = value;
            }
        }

        public int RowCount
        {
            get
            {
                return m_bVerticalVectors ? m_VectorLen : m_NumVectors;
            }
        }

        public int ColumnCount
        {
            get
            {
                return m_bVerticalVectors ? m_NumVectors : m_VectorLen;
            }
        }

        #endregion IMatrix Members

        public void AppendBottom(IROMatrix<double> a)
        {
            if (m_NumVectors == 0 && m_VectorLen == 0)
            {
                m_bVerticalVectors = false;
                m_NumVectors = a.RowCount;
                m_VectorLen = a.ColumnCount;
                m_Array = new double[m_NumVectors][];
                for (int i = 0; i < m_NumVectors; i++)
                    m_Array[i] = new double[m_VectorLen];
                MatrixMath.Copy(a, this);
            }
            else if (m_bVerticalVectors == false)
            {
                double[][] newArray = new double[m_NumVectors + a.RowCount][];
                for (int i = 0; i < m_NumVectors; i++)
                    newArray[i] = m_Array[i];
                for (int i = m_NumVectors; i < m_NumVectors + a.RowCount; i++)
                    newArray[i] = new double[m_VectorLen];
                m_Array = newArray;
                m_NumVectors += a.RowCount;

                MatrixMath.Copy(a, this, RowCount - a.RowCount, 0);
            }
            else
                throw new System.NotImplementedException("This worst case is not implemented yet.");
        }

        public void AppendRight(IROMatrix<double> a)
        {
            if (m_NumVectors == 0 && m_VectorLen == 0)
            {
                m_bVerticalVectors = true;
                m_NumVectors = a.ColumnCount;
                m_VectorLen = a.RowCount;
                m_Array = new double[m_NumVectors][];
                for (int i = 0; i < m_NumVectors; i++)
                    m_Array[i] = new double[m_VectorLen];
                MatrixMath.Copy(a, this);
            }
            else if (m_bVerticalVectors == true)
            {
                double[][] newArray = new double[m_NumVectors + a.ColumnCount][];
                for (int i = 0; i < m_NumVectors; i++)
                    newArray[i] = m_Array[i];
                for (int i = m_NumVectors; i < m_NumVectors + a.ColumnCount; i++)
                    newArray[i] = new double[m_VectorLen];
                m_Array = newArray;
                m_NumVectors += a.ColumnCount;

                MatrixMath.Copy(a, this, 0, ColumnCount - a.ColumnCount);
            }
            else
                throw new System.NotImplementedException("This worst case is not implemented yet.");
        }

        public override string ToString()
        {
            var s = new System.Text.StringBuilder();
            for (int i = 0; i < RowCount; i++)
            {
                s.Append("\n(");
                for (int j = 0; j < ColumnCount; j++)
                {
                    s.Append(this[i, j].ToString());
                    if (j + 1 < ColumnCount)
                        s.Append(",");
                    else
                        s.Append(")");
                }
            }
            return s.ToString();
        }

        public string MatrixForm { get { return ToString(); } }

        public void Transpose()
        {
            m_bVerticalVectors = !m_bVerticalVectors;
        }

        public static TransposableMatrix operator *(TransposableMatrix a, TransposableMatrix b)
        {
            // Presumtion:
            // a.Cols == b.Rows;
            if (a.ColumnCount != b.RowCount)
                throw new ArithmeticException(string.Format("Try to multiplicate a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!", a.RowCount, a.ColumnCount, b.RowCount, b.ColumnCount));

            int rows = a.RowCount;
            int cols = b.ColumnCount;
            var c = new TransposableMatrix(rows, cols);

            int summands = b.RowCount;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < summands; k++)
                        sum += a[i, k] * b[k, j];

                    c[i, j] = sum;
                }
            }
            return c;
        }

        public static TransposableMatrix operator -(TransposableMatrix a, TransposableMatrix b)
        {
            // Presumtion:
            // a.Cols == b.Rows;
            if (a.ColumnCount != b.ColumnCount || a.RowCount != b.RowCount)
                throw new ArithmeticException(string.Format("Try to subtract a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!", a.RowCount, a.ColumnCount, b.RowCount, b.ColumnCount));

            var c = new TransposableMatrix(a.RowCount, a.ColumnCount);
            for (int i = 0; i < c.RowCount; i++)
                for (int j = 0; j < c.ColumnCount; j++)
                    c[i, j] = a[i, j] - b[i, j];

            return c;
        }

        /// <summary>
        /// This will center the matrix so that the mean of each column is null.
        /// </summary>
        /// <returns>The mean, which is a horzizontal vector of dimension 1,col.</returns>
        public TransposableMatrix ColumnsToZeroMean()
        {
            // allocate the mean vector
            var mean = new TransposableMatrix(1, ColumnCount);

            for (int col = 0; col < ColumnCount; col++)
            {
                double sum = 0;
                for (int row = 0; row < RowCount; row++)
                    sum += this[row, col];
                sum /= RowCount; // calculate the mean
                for (int row = 0; row < RowCount; row++)
                    this[row, col] -= sum; // subtract the mean from every element in the column
                mean[0, col] = sum;
            }
            return mean;
        }

        public TransposableMatrix Submatrix(int rows, int cols, int rowoffset, int coloffset)
        {
            var c = new TransposableMatrix(rows, cols);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    c[i, j] = this[i + rowoffset, j + coloffset];

            return c;
        }

        public TransposableMatrix Submatrix(int rows, int cols)
        {
            return Submatrix(rows, cols, 0, 0);
        }

        public void SetColumn(int col, TransposableMatrix b)
        {
            if (col >= ColumnCount)
                throw new ArithmeticException(string.Format("Try to set column {0} in the matrix with dim({1},{2}) is not allowed!", col, RowCount, ColumnCount));
            if (b.ColumnCount != 1)
                throw new ArithmeticException(string.Format("Try to set column {0} with a matrix of more than one, namely {1} columns, is not allowed!", col, b.ColumnCount));
            if (RowCount != b.RowCount)
                throw new ArithmeticException(string.Format("Try to set column {0}, but number of rows of the matrix ({1}) not match number of rows of the vector ({2})!", col, RowCount, b.RowCount));

            for (int i = 0; i < RowCount; i++)
                this[i, col] = b[i, 0];
        }

        public void SetRow(int row, TransposableMatrix b)
        {
            if (row >= RowCount)
                throw new ArithmeticException(string.Format("Try to set row {0} in the matrix with dim({1},{2}) is not allowed!", row, RowCount, ColumnCount));
            if (b.RowCount != 1)
                throw new ArithmeticException(string.Format("Try to set row {0} with a matrix of more than one, namely {1} rows, is not allowed!", row, b.RowCount));
            if (ColumnCount != b.ColumnCount)
                throw new ArithmeticException(string.Format("Try to set row {0}, but number of columns of the matrix ({1}) not match number of colums of the vector ({2})!", row, ColumnCount, b.ColumnCount));

            for (int j = 0; j < ColumnCount; j++)
                this[row, j] = b[0, row];
        }

        public void NormalizeRows()
        {
            for (int i = 0; i < RowCount; i++)
            {
                double sum = 0;
                for (int j = 0; j < ColumnCount; j++)
                    sum += this[i, j] * this[i, j];
                sum = Math.Sqrt(sum);
                for (int j = 0; j < ColumnCount; j++)
                    this[i, j] /= sum;
            }
        }

        public double SumOfSquares()
        {
            double sum = 0;
            for (int i = 0; i < RowCount; i++)
                for (int j = 0; j < ColumnCount; j++)
                    sum += this[i, j] * this[i, j];
            return sum;
        }

        public bool IsEqualTo(TransposableMatrix m, double accuracy)
        {
            // Presumtion:
            // a.Cols == b.Rows;
            if (ColumnCount != m.ColumnCount || RowCount != m.RowCount)
                throw new ArithmeticException(string.Format("Try to compare a matrix of dim({0},{1}) with one of dim({2},{3}) is not possible!", m.RowCount, m.ColumnCount, RowCount, ColumnCount));

            double thresh = Math.Sqrt(m.SumOfSquares()) * accuracy;
            for (int i = 0; i < RowCount; i++)
                for (int j = 0; j < ColumnCount; j++)
                    if (Math.Abs(this[i, j] - m[i, j]) > thresh)
                        return false;

            return true;
        }

        public static void NIPALS(TransposableMatrix X, int numFactors, out TransposableMatrix factors, out TransposableMatrix loads)
        {
            X.ColumnsToZeroMean();

            double original_variance = Math.Sqrt(X.SumOfSquares());

            TransposableMatrix l = null;
            TransposableMatrix t_prev = null;
            TransposableMatrix t = null;

            loads = new TransposableMatrix(numFactors, X.ColumnCount);
            factors = new TransposableMatrix(X.RowCount, numFactors);

            for (int nFactor = 0; nFactor < numFactors; nFactor++)
            {
                // 1. Guess the transposed Vector lT, use first row of X matrix
                l = X.Submatrix(1, X.ColumnCount);    // l is now a horizontal vector

                for (int iter = 0; iter < 500; iter++)
                {
                    // 2. Calculate the new vector t for the factor values
                    l.Transpose(); // l is now a vertical vector
                    t = X * l; // t is also a vertical vector
                    l.Transpose(); // l horizontal again

                    t.Transpose(); // t is now horizontal

                    // Compare this with the previous one
                    if (t_prev != null && t_prev.IsEqualTo(t, 1E-6))
                        break;

                    // 3. Calculate the new loads
                    l = t * X; // gives a horizontal vector of load (= eigenvalue spectrum)
                               // normalize the (one) row
                    l.NormalizeRows(); // normalize the eigenvector spectrum

                    // 4. Goto step 2 or break after a number of iterations
                    t_prev = t;
                }

                // 5. Calculate the residual matrix
                t.Transpose(); // t is now vertical again
                factors.SetColumn(nFactor, t);
                loads.SetRow(nFactor, l);

                X = X - t * l; // X is now the residual matrix
            } // for all factors
        }
    }
}
