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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
  public class MatrixToDataTableConverter
  {
    private const string DefaultColumnBaseName = "C";
    private DataTable _destinationTable;
    private IROMatrix<double> _sourceMatrix;
    private string _columnNameBase;

    private List<Tuple<IReadOnlyList<double>, string>> _rowHeaderColumns = new List<Tuple<IReadOnlyList<double>, string>>();
    private List<Tuple<IReadOnlyList<double>, string>> _columnHeaderColumns = new List<Tuple<IReadOnlyList<double>, string>>();

    public MatrixToDataTableConverter(IROMatrix<double> sourceMatrix, DataTable destinationTable)
    {
      if (null == sourceMatrix)
        throw new ArgumentNullException("sourceMatrix");
      if (null == destinationTable)
        throw new ArgumentNullException("destinationTable");

      _sourceMatrix = sourceMatrix;
      _destinationTable = destinationTable;
    }

    #region Static Executors

    public static void SetContentFromMatrix(DataTable destinationTable, IROMatrix<double> matrix)
    {
      var c = new MatrixToDataTableConverter(matrix, destinationTable);
      c.Execute();
    }

    public static void SetContentFromMatrix(DataTable destinationTable, IROMatrix<double> matrix, string columnBaseName)
    {
      var c = new MatrixToDataTableConverter(matrix, destinationTable);
      c.ColumnBaseName = columnBaseName;
      c.Execute();
    }

    public static void SetContentFromMatrix(DataTable destinationTable, IROMatrix<double> matrix, string columnBaseName, IReadOnlyList<double> rowHeaderColumn, string rowHeaderColumnName, IReadOnlyList<double> colHeaderColumn, string colHeaderColumnName)
    {
      var c = new MatrixToDataTableConverter(matrix, destinationTable);
      c.ColumnBaseName = columnBaseName;
      c.AddMatrixColumnHeaderData(rowHeaderColumn, rowHeaderColumnName);
      c.AddMatrixColumnHeaderData(colHeaderColumn, colHeaderColumnName);
      c.Execute();
    }

    #endregion Static Executors

    #region Input properties

    public string ColumnBaseName
    {
      set
      {
        _columnNameBase = value;
      }
    }

    public event Func<int, string> ColumnNameGenerator;

    public void AddMatrixRowHeaderData(IReadOnlyList<double> vector, string name)
    {
      if (null == vector)
        throw new ArgumentNullException("vector");
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException("name");
      if (vector.Count != _sourceMatrix.Rows)
        throw new InvalidDimensionMatrixException("The number of elements of the provided vector must match the number of rows of the matrix.");

      _rowHeaderColumns.Add(new Tuple<IReadOnlyList<double>, string>(vector, name));
    }

    public void AddMatrixColumnHeaderData(IReadOnlyList<double> vector, string name)
    {
      if (null == vector)
        throw new ArgumentNullException("vector");
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException("name");
      if (vector.Count != _sourceMatrix.Columns)
        throw new InvalidDimensionMatrixException("The number of elements of the provided vector must match the number of columns of the matrix.");

      _columnHeaderColumns.Add(new Tuple<IReadOnlyList<double>, string>(vector, name));
    }

    #endregion Input properties

    private ColumnKind GetIndependendVariableColumnKind(int i)
    {
      switch (i)
      {
        case 0: return ColumnKind.X;
        case 1: return ColumnKind.Y;
        case 2: return ColumnKind.Z;
        default: return ColumnKind.Label;
      }
    }

    public void Execute()
    {
      using (var suspendToken = _destinationTable.SuspendGetToken())
      {
        var numRows = _sourceMatrix.Rows;
        var numCols = _sourceMatrix.Columns;

        int columnNumber = 0;

        var dataCols = _destinationTable.DataColumns;

        foreach (var tuple in _rowHeaderColumns)
        {
          var col = dataCols.EnsureExistenceAtPositionStrictly<DoubleColumn>(columnNumber, tuple.Item2, GetIndependendVariableColumnKind(columnNumber), 0);
          col.AssignVector = tuple.Item1;
          col.CutToMaximumLength(numRows);
          ++columnNumber;
        }

        for (int i = 0; i < _sourceMatrix.Columns; ++i)
        {
          string columnName;
          if (null != ColumnNameGenerator)
            columnName = ColumnNameGenerator(i);
          else
            columnName = string.Format("{0}{1}", string.IsNullOrEmpty(_columnNameBase) ? DefaultColumnBaseName : _columnNameBase, i);

          var col = dataCols.EnsureExistenceAtPositionStrictly<DoubleColumn>(columnNumber, columnName, ColumnKind.V, 0);
          col.AssignVector = new MatrixMath.MatrixColumnROVector(_sourceMatrix, i);
          col.CutToMaximumLength(numRows);
          ++columnNumber;
        }

        // property columns
        var numXDataCols = _rowHeaderColumns.Count;
        int propColumnNumber = 0;
        var propCols = _destinationTable.PropertyColumns;
        foreach (var tuple in _columnHeaderColumns)
        {
          var col = propCols.EnsureExistenceAtPositionStrictly<DoubleColumn>(propColumnNumber, tuple.Item2, GetIndependendVariableColumnKind(propColumnNumber), 0);
          VectorMath.Copy(tuple.Item1, col.ToVector(numXDataCols, _sourceMatrix.Columns));
          col.CutToMaximumLength(numXDataCols + _sourceMatrix.Columns);
          ++propColumnNumber;
        }

        suspendToken.Dispose();
      }
    }
  }
}