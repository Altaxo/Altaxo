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

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Data
{
  /// <summary>
  /// Converts a numeric matrix into the columns of a <see cref="DataTable"/>.
  /// </summary>
  public class MatrixToDataTableConverter
  {
    private const string DefaultColumnBaseName = "C";
    private DataTable _destinationTable;
    private IROMatrix<double> _sourceMatrix;
    private string? _columnNameBase;

    private List<Tuple<IReadOnlyList<double>, string>> _rowHeaderColumns = new List<Tuple<IReadOnlyList<double>, string>>();
    private List<Tuple<IReadOnlyList<double>, string>> _columnHeaderColumns = new List<Tuple<IReadOnlyList<double>, string>>();

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixToDataTableConverter"/> class.
    /// </summary>
    /// <param name="sourceMatrix">The source matrix.</param>
    /// <param name="destinationTable">The destination table.</param>
    public MatrixToDataTableConverter(IROMatrix<double> sourceMatrix, DataTable destinationTable)
    {
      if (sourceMatrix is null)
        throw new ArgumentNullException(nameof(sourceMatrix));
      if (destinationTable is null)
        throw new ArgumentNullException(nameof(destinationTable));

      _sourceMatrix = sourceMatrix;
      _destinationTable = destinationTable;
    }

    #region Static Executors

    /// <summary>
    /// Replaces the content of the destination table with the data from the specified matrix.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="matrix">The source matrix.</param>
    public static void SetContentFromMatrix(DataTable destinationTable, IROMatrix<double> matrix)
    {
      var c = new MatrixToDataTableConverter(matrix, destinationTable);
      c.Execute();
    }

    /// <summary>
    /// Replaces the content of the destination table with the data from the specified matrix using a custom base name for generated columns.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="matrix">The source matrix.</param>
    /// <param name="columnBaseName">The base name used for generated column names.</param>
    public static void SetContentFromMatrix(DataTable destinationTable, IROMatrix<double> matrix, string columnBaseName)
    {
      var c = new MatrixToDataTableConverter(matrix, destinationTable)
      {
        ColumnBaseName = columnBaseName
      };
      c.Execute();
    }

    /// <summary>
    /// Replaces the content of the destination table with the data from the specified matrix and adds row and column header data.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="matrix">The source matrix.</param>
    /// <param name="columnBaseName">The base name used for generated column names.</param>
    /// <param name="rowHeaderColumn">The values for the row-header column.</param>
    /// <param name="rowHeaderColumnName">The name of the row-header column.</param>
    /// <param name="colHeaderColumn">The values for the column-header column.</param>
    /// <param name="colHeaderColumnName">The name of the column-header column.</param>
    public static void SetContentFromMatrix(DataTable destinationTable, IROMatrix<double> matrix, string columnBaseName, IReadOnlyList<double> rowHeaderColumn, string rowHeaderColumnName, IReadOnlyList<double> colHeaderColumn, string colHeaderColumnName)
    {
      var c = new MatrixToDataTableConverter(matrix, destinationTable)
      {
        ColumnBaseName = columnBaseName
      };
      c.AddMatrixColumnHeaderData(rowHeaderColumn, rowHeaderColumnName);
      c.AddMatrixColumnHeaderData(colHeaderColumn, colHeaderColumnName);
      c.Execute();
    }

    #endregion Static Executors

    #region Input properties

    /// <summary>
    /// Sets the base name used for generated data-column names.
    /// </summary>
    public string ColumnBaseName
    {
      set
      {
        _columnNameBase = value;
      }
    }

    /// <summary>
    /// Gets or sets a delegate that generates the name for each matrix data column.
    /// </summary>
    public event Func<int, string>? ColumnNameGenerator;

    /// <summary>
    /// Adds a row-header data vector that is written as a data column before the matrix values.
    /// </summary>
    /// <param name="vector">The row-header values.</param>
    /// <param name="name">The name of the generated column.</param>
    public void AddMatrixRowHeaderData(IReadOnlyList<double> vector, string name)
    {
      if (vector is null)
        throw new ArgumentNullException("vector");
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException("name");
      if (vector.Count != _sourceMatrix.RowCount)
        throw new InvalidDimensionMatrixException("The number of elements of the provided vector must match the number of rows of the matrix.");

      _rowHeaderColumns.Add(new Tuple<IReadOnlyList<double>, string>(vector, name));
    }

    /// <summary>
    /// Adds a column-header data vector that is written as a property column for the generated matrix columns.
    /// </summary>
    /// <param name="vector">The column-header values.</param>
    /// <param name="name">The name of the generated property column.</param>
    public void AddMatrixColumnHeaderData(IReadOnlyList<double> vector, string name)
    {
      if (vector is null)
        throw new ArgumentNullException("vector");
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException("name");
      if (vector.Count != _sourceMatrix.ColumnCount)
        throw new InvalidDimensionMatrixException("The number of elements of the provided vector must match the number of columns of the matrix.");

      _columnHeaderColumns.Add(new Tuple<IReadOnlyList<double>, string>(vector, name));
    }

    #endregion Input properties

    private ColumnKind GetIndependendVariableColumnKind(int i)
    {
      switch (i)
      {
        case 0:
          return ColumnKind.X;
        case 1:
          return ColumnKind.Y;
        case 2:
          return ColumnKind.Z;
        default:
          return ColumnKind.Label;
      }
    }

    /// <summary>
    /// Executes the configured conversion and writes the matrix data into the destination table.
    /// </summary>
    public void Execute()
    {
      using (var suspendToken = _destinationTable.SuspendGetToken())
      {
        var numRows = _sourceMatrix.RowCount;
        var numCols = _sourceMatrix.ColumnCount;

        int columnNumber = 0;

        var dataCols = _destinationTable.DataColumns;

        foreach (var tuple in _rowHeaderColumns)
        {
          var col = dataCols.EnsureExistenceAtPositionStrictly<DoubleColumn>(columnNumber, tuple.Item2, GetIndependendVariableColumnKind(columnNumber), 0);
          col.AssignVector = tuple.Item1;
          col.CutToMaximumLength(numRows);
          ++columnNumber;
        }

        for (int i = 0; i < _sourceMatrix.ColumnCount; ++i)
        {
          string columnName;
          if (ColumnNameGenerator is not null)
            columnName = ColumnNameGenerator(i);
          else
            columnName = string.Format("{0}{1}", string.IsNullOrEmpty(_columnNameBase) ? DefaultColumnBaseName : _columnNameBase, i);

          var col = dataCols.EnsureExistenceAtPositionStrictly<DoubleColumn>(columnNumber, columnName, ColumnKind.V, 0);
          col.AssignVector = MatrixMath.ColumnToROVector(_sourceMatrix, i);
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
          VectorMath.Copy(tuple.Item1, col.ToVector(numXDataCols, _sourceMatrix.ColumnCount));
          col.CutToMaximumLength(numXDataCols + _sourceMatrix.ColumnCount);
          ++propColumnNumber;
        }

        suspendToken.Dispose();
      }
    }
  }
}
