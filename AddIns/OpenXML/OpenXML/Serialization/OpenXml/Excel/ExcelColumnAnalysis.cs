#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Altaxo.Serialization.OpenXml.Excel
{
  /// <summary>
  /// Analyzes a single column and returns the most likely .NET type for the column values.
  /// </summary>
  public class ExcelColumnAnalysis
  {
    /// <summary>
    /// Gets the zero-based row and column indices corresponding to the specified cell's reference.
    /// </summary>
    /// <remarks>The method interprets the cell reference in the format used by spreadsheet applications,
    /// where columns are represented by letters and rows by numbers. The returned indices are zero-based, so the first
    /// row and column are both 0.</remarks>
    /// <param name="cell">The cell from which to extract the row and column indices. The cell's reference must be in the standard format
    /// (e.g., "C12").</param>
    /// <returns>A tuple containing the zero-based row index and column index of the cell. Returns (-1, -1) if the cell reference
    /// is null or empty.</returns>
    public static (int RowIndex, int ColumnIndex) GetRowAndColumnIndexFromCell(Cell? cell)
    {
      var r = cell?.CellReference?.Value; // like "C12"
      if (string.IsNullOrEmpty(r))
        return (-1, -1);

      int col = 0;
      int i = 0;
      for (; i < r.Length; i++)
      {
        char ch = r[i];
        if (ch < 'A' || ch > 'Z')
          break;
        col = (col * 26) + (ch - 'A' + 1);
      }

      int row = 0;
      for (; i < r.Length; i++)
      {
        char ch = r[i];
        if (ch < '0' || ch > '9')
          break;
        row = (row * 10) + (ch - '0');
      }

      return (row - 1, col - 1); // 0-based
    }

    /// <summary>
    /// Gets the zero-based column index from the specified cell's reference.
    /// </summary>
    /// <remarks>This method interprets the column portion of the cell's reference (such as "C" in "C12") and
    /// converts it to a zero-based index. For example, column "A" returns 0, "B" returns 1, and so on.</remarks>
    /// <param name="cell">The cell from which to extract the column index. Must have a valid cell reference (e.g., "C12").</param>
    /// <returns>The zero-based column index corresponding to the cell's reference, or -1 if the cell reference is null or empty.</returns>
    public static int GetColumnIndexFromCell(Cell cell)
    {
      var r = cell.CellReference?.Value; // like "C12"
      if (string.IsNullOrEmpty(r))
        return -1;

      int col = 0;
      int i = 0;
      for (; i < r.Length; i++)
      {
        char ch = r[i];
        if (ch < 'A' || ch > 'Z')
          break;
        col = (col * 26) + (ch - 'A' + 1);
      }
      return col - 1; // 0-based
    }

    /// <summary>
    /// Gets the existing columns of a worksheet as lists of cells.
    /// </summary>
    /// <param name="sheetData">The sheet data to enumerate.</param>
    /// <returns>
    /// A list of columns, where each column is represented by a list of cells. Only cells that exist in the XML are included.
    /// The columns are ordered by their column index.
    /// </returns>
    public static List<List<Cell>> GetColumns(SheetData sheetData)
    {
      var columns = new Dictionary<int, List<Cell>>();
      int rowCount = 0;
      foreach (var row in sheetData.Elements<Row>())
      {
        bool anythingAdded = false;
        foreach (var cell in row.Elements<Cell>())
        {
          if (cell?.CellValue?.Text == null)
            continue;

          var colIndex = GetColumnIndexFromCell(cell);
          if (colIndex < 0)
            continue;
          if (!columns.TryGetValue(colIndex, out var column))
          {
            column = new List<Cell>();
            columns[colIndex] = column;
            EnsureListSize(column, rowCount);
          }
          column.Add(cell);
          anythingAdded = true;
        }
        if (anythingAdded)
        {
          rowCount++;
          foreach (var column in columns.Values)
          {
            EnsureListSize(column, rowCount);
          }
        }

      }

      return columns.Values.ToList();
    }

    /// <summary>
    /// Ensures that the specified list contains at least the specified number of elements by adding default values as
    /// needed.
    /// </summary>
    /// <remarks>If the list already contains at least the specified number of elements, no action is taken.
    /// If additional elements are added, they are initialized to the default value for type T.</remarks>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to be extended if its current count is less than the specified size. Cannot be null.</param>
    /// <param name="size">The minimum number of elements the list should contain. Must be non-negative.</param>
    private static void EnsureListSize<T>(List<T> list, int size)
    {
      while (list.Count < size)
        list.Add(default!);
    }


    /// <summary>
    /// Determines the most likely .NET type of an Excel column by analyzing the cell contents.
    /// </summary>
    /// <param name="column">The column to analyze.</param>
    /// <param name="numberOfHeaderRows">The number of header rows to skip before analyzing data cells.</param>
    /// <returns>
    /// The inferred .NET type (e.g., <see cref="int"/>, <see cref="double"/>, <see cref="DateTime"/>, <see cref="string"/>),
    /// or <see langword="null"/> if no data could be analyzed.
    /// </returns>
    public static Type? GetTypeOfColumn(List<Cell> column, int numberOfHeaderRows)
    {
      int numberOfIntegers = 0;
      int numberOfFloats = 0;
      int numberOfStrings = 0;
      int numberOfDates = 0;
      int numberOfNulls = 0;


      int idxCell;
      for (idxCell = 0; idxCell < column.Count; ++idxCell)
      {
        if (GetRowAndColumnIndexFromCell(column[idxCell]).RowIndex >= numberOfHeaderRows)
          break;
      }

      for (; idxCell < column.Count; ++idxCell)
      {
        var cell = column[idxCell];
        if (cell is null)
          continue;

        if (string.IsNullOrEmpty(cell.CellValue?.Text))
        {
          numberOfNulls++;
          continue;
        }

        if (cell.DataType is null || !cell.DataType.HasValue || cell.DataType.Value == CellValues.Number)
        {
          if (cell.CellValue.TryGetInt(out var _))
            numberOfIntegers++;
          else if (cell.CellValue.TryGetDouble(out var _))
            numberOfFloats++;
          else
            throw new NotImplementedException();

          continue;
        }

        if (cell.DataType.HasValue && cell.DataType.Value == CellValues.Date)
        {
          if (cell.CellValue.TryGetDateTime(out var _))
            numberOfDates++;
          else
            throw new NotImplementedException();

          continue;
        }

        if (cell.DataType.HasValue && cell.DataType.Value == CellValues.Boolean)
        {
          if (cell.CellValue.TryGetBoolean(out var _))
            numberOfIntegers++;
          else
            throw new NotImplementedException();

          continue;
        }

        {
          numberOfStrings++;
        }
      } // end for


      if (0 == numberOfIntegers + numberOfFloats + numberOfDates + numberOfStrings)
        return null;


      var types = new[] { typeof(int), typeof(double), typeof(DateTime), typeof(string) };
      var scores = new double[4]
        {
          250d * numberOfIntegers,
          250d * (numberOfFloats + numberOfIntegers),
          300d * numberOfDates,
          100d * (numberOfStrings+numberOfDates+numberOfFloats+numberOfIntegers), // score of string, has the basis value of 100
        };

      Array.Sort(scores, types);

      return types[^1];
    }
  } // end class
}
