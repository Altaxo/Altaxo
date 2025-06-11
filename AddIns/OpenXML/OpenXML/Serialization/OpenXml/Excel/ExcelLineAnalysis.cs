#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using System.Collections.Immutable;
using Altaxo.Serialization.Ascii;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Altaxo.Serialization.OpenXml.Excel
{
  /// <summary>
  /// Analyse a single line of text with regard to different separation strategies, and store
  /// the result in a dictionary that contains entries for the separation strategy and the resulting structure.
  /// </summary>
  public class ExcelLineAnalysis
  {
    /// <summary>
    /// Analyse the provided line of text with regard to one separation stragegy and returns the resulting structure.
    /// </summary>
    /// <param name="row">The row of a Excel spreadsheet.</param>
    /// <returns>The resulting structure.</returns>
    public static AsciiLineComposition GetStructure(Row row)
    {
      var tabStruc = ImmutableArray.CreateBuilder<AsciiColumnInfo>();

      foreach (Cell cell in row.Elements<Cell>())
      {
        if (string.IsNullOrEmpty(cell.CellValue?.Text))
        {
          tabStruc.Add(AsciiColumnInfo.DBNull);
          continue;
        }



        if (cell.DataType is null || !cell.DataType.HasValue || cell.DataType.Value == CellValues.Number)
        {
          if (cell.CellValue.TryGetInt(out var _))
            tabStruc.Add(AsciiColumnInfo.Integer);
          else if (cell.CellValue.TryGetDouble(out var _))
            tabStruc.Add(AsciiColumnInfo.FloatWithDecimalSeparator);
          else
            throw new NotImplementedException();

          continue;
        }

        if (cell.DataType.HasValue && cell.DataType.Value == CellValues.Date)
        {
          if (cell.CellValue.TryGetDateTime(out var _))
            tabStruc.Add(AsciiColumnInfo.DateTime);
          else
            throw new NotImplementedException();

          continue;
        }

        if (cell.DataType.HasValue && cell.DataType.Value == CellValues.Boolean)
        {
          if (cell.CellValue.TryGetBoolean(out var _))
            tabStruc.Add(AsciiColumnInfo.Integer);
          else
            throw new NotImplementedException();

          continue;
        }

        {
          tabStruc.Add(AsciiColumnInfo.Text);
        }
      } // end for

      return new AsciiLineComposition(tabStruc.ToImmutable());
    }
  } // end class
}
